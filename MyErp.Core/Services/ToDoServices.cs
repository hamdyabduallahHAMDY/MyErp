using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using OfficeOpenXml;
using Type = MyErp.Core.Models.Type;

namespace MyErp.Core.Services
{
    public class ToDoServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<ToDo> Errors = new Errors<ToDo>();
        private readonly IHubContext<NotificationHub> _hub;
        private readonly NotificationService _notservices;


        public ToDoServices(IUnitOfWork unitOfWork, IMapper mapper , IHubContext<NotificationHub> hub, NotificationService notificationService)
        {
            _notservices = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hub = hub;
        }


        public async Task<byte[]> GenerateToDoExcelTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("ToDo Template");

            // ================= HEADERS =================
            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Description";
            worksheet.Cells[1, 3].Value = "AssignedTo";
            worksheet.Cells[1, 4].Value = "CreatedBy";
            worksheet.Cells[1, 5].Value = "ischecked";

            // ================= DEMO ROW =================
            worksheet.Cells[2, 1].Value = "Finish ERP Module";
            worksheet.Cells[2, 2].Value = "Complete ToDo API and UI";
            worksheet.Cells[2, 3].Value = "user1";
            worksheet.Cells[2, 4].Value = "admin";
            worksheet.Cells[2, 5].Value = 0;

            // ================= STYLING (optional but nice 👀) =================
            using (var range = worksheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        //Get All by OdooType
        public async Task<MainResponse<ToDo>> GetAllByOdooCustomerType(Type odooType , string customername)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();
            try
            {
                if (odooType == Type.Odoo_Development) {
                    var todos = await _unitOfWork.ToDos.GetAll(a => a.ProjectType == ProjectType.Development_Odoo && a.CustomerName == customername);
                    if (todos == null || !todos.Any())
                    {
                        response.errors?.Add(Errors.ObjectNotFound());
                        return response;
                    }

                    response.acceptedObjects = todos.ToList();
                }
                if (odooType == Type.Odoo_Implementation)
                {
                    var todos = await _unitOfWork.ToDos.GetAll(a => a.ProjectType == ProjectType.Implementation_Odoo && a.CustomerName == customername);
                    if (todos == null || !todos.Any())
                    {
                        response.errors?.Add(Errors.ObjectNotFound());
                        return response;
                    }

                    response.acceptedObjects = todos.ToList();
                }
                if (odooType == Type.Odoo)
                {
                    var todos = await _unitOfWork.ToDos.GetAll(a => a.CustomerName == customername);
                    if (todos == null || !todos.Any())
                    {
                        response.errors?.Add(Errors.ObjectNotFound());
                        return response;
                    }
                    response.acceptedObjects = todos.ToList();

                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
                if (ex.InnerException != null)
                    response.errors?.Add(ex.InnerException.Message);
            }
            return response;
        }

        // GET ALL (with Daily reset)
        public async Task<MainResponse<ToDo>> GetAll(string allowedUsers)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            try
            {
                //  Safety check
                if (allowedUsers == null || !allowedUsers.Any())
                {
                    response.acceptedObjects = new List<ToDo>();
                    return response;
                }

               // var allowed = allowedUsers.ToList();

                //  Get IQueryable instead of GetAll()
                var query = _unitOfWork.ToDos.GetQueryable();

                //  Apply filtering in memory (same as Email)
                var todos = query
                    .AsEnumerable()
                    .Where(t => t.CreatedBy == allowedUsers 
                    || t.AssignedTo ==allowedUsers)
                    .ToList();

                if (todos == null || !todos.Any())
                {
                    response.errors?.Add(Errors.ObjectNotFound());
                    return response;
                }

                var today = DateTime.UtcNow;

                //  Daily reset logic
                foreach (var todo in todos)
                {
                    if (todo.Daily && todo.LastCheckedAt.HasValue)
                    {
                        if ((today - todo.LastCheckedAt.Value).TotalMinutes >= 2)
                        {
                            todo.ischecked = (Status)0;

                            await _unitOfWork.ToDos.Update(todo);
                        }
                    }
                }

                response.acceptedObjects = todos;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }

        // GET BY ID
        public async Task<MainResponse<ToDo>> getToDo(int id)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            try
            {
                var todo = await _unitOfWork.ToDos.GetById(id);

                if (todo == null)
                {
                    response.errors.Add(Errors.ObjectNotFound());
                    return response;
                }

                var today = DateTime.UtcNow.Date;

                if (todo.Daily && todo.LastCheckedAt?.Date != today)
                    todo.ischecked = (Status)0;

                response.acceptedObjects?.Add(todo);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        // GET BY STATUS
        public async Task<MainResponse<ToDo>> getByStatus(int status, List<string> allowedUsers)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            try
            {
                // Safety check
                if (allowedUsers == null || !allowedUsers.Any())
                {
                    response.acceptedObjects = new List<ToDo>();
                    return response;
                }

                var allowed = allowedUsers.ToList();

                // Get all todos first (like your pattern)
                var todos = await _unitOfWork.ToDos.GetAll(a => (int)a.ischecked == status);

                if (todos == null || !todos.Any())
                {
                    response.errors.Add(Errors.ObjectNotFound());
                    return response;
                }

                // Apply access filtering 🔥
                var filteredTodos = todos
                    .Where(t => allowed.Contains(t.CreatedBy))
                    .ToList();

                response.acceptedObjects = filteredTodos;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        //Get By Type
        public async Task<MainResponse<ToDo>> getByType(Type type)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();
            try
            {
          
                var todos = await _unitOfWork.ToDos.GetAll(a => a.Type == type);
                if (todos == null || !todos.Any())
                {
                    response.errors?.Add(Errors.ObjectNotFound());
                    return response;
                }
                
                response.acceptedObjects = todos.ToList();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
            }
            return response;
        }

        // ADD
        public async Task<MainResponse<ToDo>> addToDo(ToDoDTO todos , string name , Type usertype , string AssignedId)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            try
            {
                var validList = await ValidateDTO.ToDoDTO(todos);

                var todoList = _mapper.Map<List<ToDo>>(validList.acceptedObjects);
                var rejectedList = _mapper.Map<List<ToDo>>(validList.rejectedObjects);
                foreach (var todo in todoList)
                {
                    todo.LastCheckedAt = DateTime.UtcNow;
                    todo.CreatedBy = name;
                }
                if (todoList != null && todoList.Count > 0)
                {
                    foreach (var todo in todoList)
                    {
                        todo.CreatedBy = name;
                        todo.Type = usertype;
                        var notification = new Notification
                        {
                            UserId = AssignedId,
                            Message = todo.Title,
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false,
                            CreatedBy = name
                        };
                        await _unitOfWork.Notifications.Add(notification);
                        await _hub.Clients.User(AssignedId)
                   .SendAsync(AssignedId, $"A new ToDo '{todo.Title}' has been assigned to you By {todo.CreatedBy}");
                        //await _notservices.AddNotificationAsync(AssignedId, $"A new ToDo '{todo.Title}' has been assigned to you.");
                    }
                    await _unitOfWork.ToDos.Add(todoList);
                    
                    response.acceptedObjects = todoList;
                }

                if (rejectedList != null && rejectedList.Count > 0)
                {
                    response.rejectedObjects = rejectedList;
                    response.errors = validList.errors ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors?.Add(ex.InnerException.Message);
            }

            return response;
        }

        // IMPORT FROM EXCEL
        public async Task<MainResponse<ToDo>> ImportFromExcel(IFormFile excelFile)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    response.errors.Add("Excel file is empty.");
                    return response;
                }

                var todosToAdd = new List<ToDo>();

                using var ms = new MemoryStream();
                await excelFile.CopyToAsync(ms);
                ms.Position = 0;

                using var package = new ExcelPackage(ms);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    response.errors.Add("Worksheet not found.");
                    return response;
                }

                int rows = worksheet.Dimension?.Rows ?? 0;

                for (int r = 2; r <= rows; r++)
                {
                    var title = worksheet.Cells[r, 1].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(title))
                        continue;

                    var dto = new ToDoDTO
                    {
                        Title = title,
                        Description = worksheet.Cells[r, 2].Text?.Trim(),
                        AssignedTo = worksheet.Cells[r, 3].Text?.Trim(),
                        CustomerName = worksheet.Cells[r, 4].Text?.Trim(),
                        ischecked = int.TryParse(worksheet.Cells[r, 5].Text, out int v) ? v : 0
                    };

                    var todo = _mapper.Map<ToDo>(dto);

                    todosToAdd.Add(todo);
                }

                if (!todosToAdd.Any())
                {
                    response.errors.Add("No valid rows found.");
                    return response;
                }

                await _unitOfWork.ToDos.Add(todosToAdd);

                response.acceptedObjects = todosToAdd;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }
        // UPDATE
        public async Task<MainResponse<ToDo>> updateToDo(int id, ToDoDTO todoUpdated , string name)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var validList = await ValidateDTO.ToDoDTO(todoUpdated, true);

                var existingTodo = await _unitOfWork.ToDos.GetFirst(a => a.Id == id);

                if (existingTodo == null)
                {
                    response.errors.Add($"Cannot find ToDo with ID {id}.");
                    return response;
                }

                if (validList.acceptedObjects == null || validList.acceptedObjects.Count == 0)
                {
                    response.errors?.Add("No valid payload to update.");
                    return response;
                }

                var dto = validList.acceptedObjects[0];
                existingTodo.CreatedBy = name; // Preserve original creator
                _mapper.Map(dto, existingTodo);

                if (dto.ischecked == 1)
                    existingTodo.LastCheckedAt = DateTime.UtcNow;
                else
                    existingTodo.LastCheckedAt = null;

                await _unitOfWork.ToDos.Update(existingTodo);

                response.acceptedObjects.Add(existingTodo);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }
        // UPDATE STATUS
        public async Task<MainResponse<ToDo>> UpdateStatus(int id, int status)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var todo = await _unitOfWork.ToDos.GetFirst(t => t.Id == id);

                if (todo == null)
                {
                    response.errors.Add($"ToDo with ID {id} not found.");
                    return response;
                }

                todo.ischecked = (Status)status;

                if (status == 1 || status == 2 || status == 3)
                    todo.LastCheckedAt = DateTime.UtcNow;
                else
                    todo.LastCheckedAt = null;

                await _unitOfWork.ToDos.Update(todo);

                response.acceptedObjects.Add(todo);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }
        // DELETE
        public async Task<MainResponse<ToDo>> deleteToDo(int id)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            try
            {
                var todo = await _unitOfWork.ToDos.DeletePhysical(p => p.Id == id);

                if (todo == null)
                {
                    response.errors.Add(Errors.ObjectNotFoundWithId(id));
                    return response;
                }

                response.acceptedObjects = new List<ToDo> { todo.First() };
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }
        // DELETE ALL
        public async Task<MainResponse<ToDo>> deleteAll()
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();
            try
            {
                var todos = await _unitOfWork.ToDos.GetAll();
                if (todos == null || !todos.Any())
                {
                    response.errors.Add("There is no even Any ToDos (:");
                    return response;
                }
                var deletedTodos = await _unitOfWork.ToDos.DeletePhysical(p => true);
                response.acceptedObjects = deletedTodos.ToList();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }
            return response;

        }
        // DELETE GROUP
        public async Task<MainResponse<ToDo>> deleteGroup(List<int> ids)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            try
            {
                foreach (var id in ids)
                {
                    var deletedTodos = await _unitOfWork.ToDos.DeletePhysical(p => p.Id == id);
                    if (deletedTodos == null || !deletedTodos.Any())
                    {
                        response.errors?.Add($"id = {id} not found");
                        return response;
                    }
                    else
                    {
                        response.acceptedObjects = deletedTodos.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }
            return response;
        }
        // GET BY ASSIGNED TO
        public async Task<MainResponse<ToDo>> GetByAssignedTo(string assignedTo)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();
            try
            {
                var todos = await _unitOfWork.ToDos.GetAll(a => a.AssignedTo == assignedTo);
                if (todos == null || !todos.Any())
                {
                    response.errors?.Add(Errors.ObjectNotFound());
                    return response;
                }
                response.acceptedObjects = todos.ToList();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
            }
            return response;
        }

        public async Task<MainResponse<ToDo>> getByCustomer(string customer)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();
            try
            {
                var todos = await _unitOfWork.ToDos.GetAll(a => a.CustomerName == customer);
                if (todos == null || !todos.Any())
                {
                    response.errors?.Add(Errors.ObjectNotFound());
                    return response;
                }
                response.acceptedObjects = todos.ToList();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
            }
            return response;

        }
        public async Task<MainResponse<ToDo>> getByCustomerandType(string customer , Type type)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();
            try
            {
                var todos = await _unitOfWork.ToDos.GetAll(a => a.CustomerName == customer && a.Type  == type);
                if (todos == null || !todos.Any())
                {
                    response.errors?.Add(Errors.ObjectNotFound());
                    return response;
                }
                response.acceptedObjects = todos.ToList();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
            }
            return response;

        }

   

    }
}