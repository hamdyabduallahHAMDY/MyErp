using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class CalendarTaskServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    Errors<CalenderTask> Errors = new Errors<CalenderTask>();

    public CalendarTaskServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

    public async Task<MainResponse<CalenderTask>> addCalendarTask(List<CalenderTaskDTO> tasks , string createdby)
    {
        MainResponse<CalenderTask> response = new MainResponse<CalenderTask>();

        try
        {
            var validList = await ValidateDTO.CalenderTaskDTO(tasks);

            List<CalenderTask> taskList = _mapper.Map<List<CalenderTask>>(validList.acceptedObjects);
            List<CalenderTask> rejectedTasks = _mapper.Map<List<CalenderTask>>(validList.rejectedObjects);

            if (taskList != null && taskList.Count > 0)
            {
                foreach (var task in taskList)
                {
                   // task.CreatedBy = createdby;
                    task.ReminderTime = task.EndTime.AddMinutes(-task.ReminderMinutesBefore);
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
            if (ex.InnerException != null) response.errors.Add(ex.InnerException.Message);
            return response;
        }
    }

    public async Task<MainResponse<CalenderTask>> updateCalendarTask(int id, List<CalenderTaskDTO> taskUpdated)
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