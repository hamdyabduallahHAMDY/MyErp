using AutoMapper;
using Logger;
using Microsoft.AspNetCore.SignalR;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using System.Text.Json;

namespace MyErp.Core.Services;

public class CalendarTaskServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly NotificationService _notificationService;
    Errors<CalenderTask> Errors = new Errors<CalenderTask>();

    public CalendarTaskServices(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<NotificationHub> hub, NotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hub = hub;
        _notificationService = notificationService;
    }

    public async Task<MainResponse<CalenderTask>> getCalendarTasksList()
    {
        MainResponse<CalenderTask> response = new MainResponse<CalenderTask>();

        var tasks = await _unitOfWork.CalenderTasks.GetAll();

        response.acceptedObjects = tasks.ToList();

        return response;
    }

    public async Task<MainResponse<CalenderTask>> getCalendarTask(int id)
    {
        MainResponse<CalenderTask> response = new MainResponse<CalenderTask>();

        var task = await _unitOfWork.CalenderTasks.GetById(id);

        if (task == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<CalenderTask> { task };

        return response;
    }


    public async Task<MainResponse<CalenderTask>> addCalendarTask(
        CalenderTaskDTO tasks,
        string createdby,
        List<string> assignedto)
    {
        MainResponse<CalenderTask> response = new MainResponse<CalenderTask>();

        try
        {
            var validList = await ValidateDTO.CalenderTaskDTO(tasks);

            List<CalenderTask> taskList = _mapper.Map<List<CalenderTask>>(validList.acceptedObjects);
            List<CalenderTask> rejectedTasks = _mapper.Map<List<CalenderTask>>(validList.rejectedObjects);

            // ✅ Ensure list is safe
            var assignedUsers = assignedto ?? new List<string>();

            if (taskList != null && taskList.Count > 0)
            {
                foreach (var task in taskList)
                {
                    task.CreatedBy = createdby;

                    // (Optional but recommended) keep consistency
                    task.AssignedTo = string.Join(",", assignedUsers);

                    // 🔥 Create DB notification per user
                    foreach (var user in assignedUsers)
                    {
                        var notification = new Notification
                        {
                            title = "New Calendar Task Assigned",
                            UserId = user,
                            Message = task.Title,
                            CreatedAt = DateTime.Now,
                            IsRead = false,
                            CreatedBy = createdby
                        };

                        await _unitOfWork.Notifications.Add(notification);

                        // 🔥 SignalR push
                        await _hub.Clients.User(user)
                            .SendAsync("ReceiveNotification", new
                            {
                                title = "New Calendar Task Assigned",
                                taskTitle = task.Title,
                                assignedBy = createdby,
                                message = $"\"{task.Title}\" has been assigned to you",
                                type = "CalendarsTaskAssigned",
                                createdAt = DateTime.UtcNow
                            });
                    }
                }

                await _unitOfWork.CalenderTasks.Add(taskList);

                response.acceptedObjects = taskList;
            }

            if (rejectedTasks != null && rejectedTasks.Count > 0)
            {
                response.rejectedObjects = rejectedTasks;
                response.errors = validList.errors;
            }

            return response;
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            response.errors.Add(ex.Message);

            if (ex.InnerException != null)
                response.errors.Add(ex.InnerException.Message);

            return response;
        }
    }
    public async Task<MainResponse<CalenderTask>> updateCalendarTask(int id, CalenderTaskDTO taskUpdated)
    {
        var response = new MainResponse<CalenderTask>();

        try
        {
            var validList = await ValidateDTO.CalenderTaskDTO(taskUpdated, true);

            var existingTask = await _unitOfWork.CalenderTasks.GetFirst(a => a.Id == id);

            if (existingTask is null)
            {
                response.errors?.Add($"Cannot find Calendar Task with ID {id}.");

                if (validList.errors?.Any() == true)
                    response.errors?.AddRange(validList.errors);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects?.AddRange(_mapper.Map<List<CalenderTask>>(validList.rejectedObjects));

                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors?.Add("No valid payload to update Calendar Task. Fix validation errors and try again.");

                if (validList.errors?.Any() == true)
                    response.errors?.AddRange(validList.errors);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects?.AddRange(_mapper.Map<List<CalenderTask>>(validList.rejectedObjects));

                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingTask);

            await _unitOfWork.CalenderTasks.Update(existingTask);

            response.acceptedObjects?.Add(existingTask);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects?.AddRange(_mapper.Map<List<CalenderTask>>(validList.rejectedObjects));

            if (validList.errors?.Any() == true)
                response.errors?.AddRange(validList.errors);
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            response.errors?.Add(ex.Message);
            if (ex.InnerException != null) response.errors?.Add(ex.InnerException.Message);
        }

        return response;
    }

    public async Task<MainResponse<CalenderTask>> deleteCalendarTask(int id)
    {
        MainResponse<CalenderTask> response = new MainResponse<CalenderTask>();

        var task = await _unitOfWork.CalenderTasks.DeletePhysical(p => p.Id == id);

        if (task == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<CalenderTask> { task.First() };

        return response;
    }

    public async Task<MainResponse<CalenderTask>> deleteAll()
    {
        MainResponse<CalenderTask> response = new MainResponse<CalenderTask>();
        try
        {
            var deletedLeads = await _unitOfWork.CalenderTasks.DeletePhysical(p => true);
            if (deletedLeads == null || !deletedLeads.Any())
            {
                response.errors?.Add($"No leads found to delete.");
                return response;
            }
            else
            {
                response.acceptedObjects = deletedLeads.ToList();
            }
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            response.errors?.Add(ex.Message);
        }
        return response;
    }
}