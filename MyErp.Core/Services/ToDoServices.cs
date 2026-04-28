using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using OfficeOpenXml;
using System.Linq;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Type = MyErp.Core.Models.Type;

namespace MyErp.Core.Services
{
    public class ToDoServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly Errors<ToDo> _errors = new();

        public ToDoServices(IUnitOfWork unitOfWork,IMapper mapper,IHubContext<NotificationHub> hub)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hub = hub;
        }

        public async Task<byte[]> GenerateToDoExcelTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("ToDo Template");

            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Description";
            worksheet.Cells[1, 3].Value = "AssignedTo";
            worksheet.Cells[1, 4].Value = "CustomerName";
            worksheet.Cells[1, 5].Value = "IsChecked";

            worksheet.Cells[2, 1].Value = "Finish ERP Module";
            worksheet.Cells[2, 2].Value = "Complete ToDo API and UI";
            worksheet.Cells[2, 3].Value = "user1";
            worksheet.Cells[2, 4].Value = "Customer A";
            worksheet.Cells[2, 5].Value = 0;

            using (var range = worksheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<MainResponse<ToDo>> GetAllByOdooCustomerType(Type userType, string customerName)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    response.errors.Add("Customer name is required.");
                    return response;
                }

                IEnumerable<ToDo> todos;

                if (userType == Type.Odoo_Development)
                {
                    todos = await _unitOfWork.ToDos.GetAll(x =>
                        x.ProjectType == ProjectType.Development_Odoo &&
                        x.CustomerName == customerName);
                }
                else if (userType == Type.Odoo_Implementation)
                {
                    todos = await _unitOfWork.ToDos.GetAll(x =>
                        x.ProjectType == ProjectType.Implementation_Odoo &&
                        x.CustomerName == customerName);
                }
                else
                {
                    todos = await _unitOfWork.ToDos.GetAll(x => x.CustomerName == customerName);
                }

                var list = todos?.ToList() ?? new List<ToDo>();

                if (!list.Any())
                {
                    response.errors.Add(_errors.ObjectNotFound());
                    return response;
                }

                response.acceptedObjects = list;
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

        public async Task<MainResponse<ToDo>> GetAll(List<string> allowedUsers)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>
            {
                acceptedObjects = new List<ToDo>(),
                rejectedObjects = new List<ToDo>(),
                errors = new List<string>()
            };

            try
            {
                if (allowedUsers == null || !allowedUsers.Any())
                {
                    return response;
                }

                allowedUsers = allowedUsers
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                if (!allowedUsers.Any())
                {
                    return response;
                }

                var predicate = BuildAllowedUsersPredicate(allowedUsers);

                var leads = await _unitOfWork.ToDos
                    .GetQueryable()
                    .Where(predicate)
                    .ToListAsync();

                if (!leads.Any())
                {
                    response.errors.Add(_errors.ObjectNotFound());
                    return response;
                }

                response.acceptedObjects = leads;
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

        private static Expression<Func<ToDo, bool>> BuildAllowedUsersPredicate(List<string> allowedUsers)
        {
            var param = Expression.Parameter(typeof(ToDo), "l");
            Expression body = Expression.Constant(false);

            foreach (var user in allowedUsers)
            {
                var createdByExpr = Expression.Equal(
                    Expression.Property(param, nameof(ToDo.CreatedBy)),
                    Expression.Constant(user)
                );

                var assignedToExpr = Expression.Equal(
                    Expression.Property(param, nameof(ToDo.AssignedTo)),
                    Expression.Constant(user)
                );

                var userMatchExpr = Expression.OrElse(createdByExpr, assignedToExpr);
                body = Expression.OrElse(body, userMatchExpr);
            }

            return Expression.Lambda<Func<ToDo, bool>>(body, param);
        }

        //public async Task<MainResponse<ToDo>> GetAll(string currentUser)
        //{
        //    var response = new MainResponse<ToDo>();

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(currentUser))
        //        {
        //            response.acceptedObjects = new List<ToDo>();
        //            return response;
        //        }

        //        var query = _unitOfWork.ToDos.GetQueryable();

        //        var todos = await query
        //            .Where(t => t.CreatedBy == currentUser || t.AssignedTo == currentUser)
        //            .ToListAsync();

        //        if (!todos.Any())
        //        {
        //            response.errors.Add(_errors.ObjectNotFound());
        //            return response;
        //        }

        //        var now = DateTime.Now;
        //        var today = now.Date;

        //        foreach (var todo in todos)
        //        {
        //            if (!todo.Daily)
        //                continue;

        //            if (!todo.LastCheckedAt.HasValue)
        //                continue;

        //            if (todo.LastCheckedAt.Value.Date < today)
        //            {
        //                todo.ischecked = (Status)0;
        //                todo.LastCheckedAt = null;
        //                await _unitOfWork.ToDos.Update(todo);
        //            }
        //        }

        //        response.acceptedObjects = todos;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Log(ex.ToString());
        //        response.errors.Add(ex.Message);
        //        if (ex.InnerException != null)
        //            response.errors.Add(ex.InnerException.Message);
        //    }

        //    return response;
        //}

        public async Task<MainResponse<ToDo>> GetById(int id, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var todo = await _unitOfWork.ToDos.GetById(id);

                if (todo == null)
                {
                    response.errors.Add(_errors.ObjectNotFoundWithId(id));
                    return response;
                }

                if (!CanAccess(todo, currentUser))
                {
                    response.errors.Add("You are not allowed to access this ToDo.");
                    return response;
                }

                if (todo.Daily && todo.LastCheckedAt.HasValue && todo.LastCheckedAt.Value.Date < DateTime.Now.Date)
                {
                    todo.ischecked = (Status)0;
                    todo.LastCheckedAt = null;
                    await _unitOfWork.ToDos.Update(todo);
                }

                response.acceptedObjects.Add(todo);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> GetByStatus(int status, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    response.acceptedObjects = new List<ToDo>();
                    return response;
                }

                var todos = await _unitOfWork.ToDos.GetAll(x =>
                    (int)x.ischecked == status &&
                    (x.CreatedBy == currentUser || x.AssignedTo == currentUser));

                var list = todos?.ToList() ?? new List<ToDo>();

                if (!list.Any())
                {
                    response.errors.Add(_errors.ObjectNotFound());
                    return response;
                }

                response.acceptedObjects = list;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> GetByType(Type type, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var todos = await _unitOfWork.ToDos.GetAll(x =>
                    x.Type == type &&
                    (x.CreatedBy == currentUser || x.AssignedTo == currentUser));

                var list = todos?.ToList() ?? new List<ToDo>();

                if (!list.Any())
                {
                    response.errors.Add(_errors.ObjectNotFound());
                    return response;
                }

                response.acceptedObjects = list;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> AddToDo(ToDoDTO dto, string currentUser, Type userType, string assignedUserId)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var validList = await ValidateDTO.ToDoDTO(dto);

                var accepted = _mapper.Map<List<ToDo>>(validList.acceptedObjects);
                var rejected = _mapper.Map<List<ToDo>>(validList.rejectedObjects);

                if (accepted != null && accepted.Count > 0)
                {
                    foreach (var todo in accepted)
                    {
                        todo.CreatedBy = currentUser;
                        todo.Type = userType;
                        todo.LastCheckedAt = todo.ischecked == (Status)1 ? DateTime.Now : null;
                    }

                    await _unitOfWork.ToDos.Add(accepted);

                    foreach (var todo in accepted)
                    {
                        if (!string.IsNullOrWhiteSpace(assignedUserId))
                        {
                            var notification = new Notification
                            {
                                title = "You received a new ToDo Task!",
                                UserId = assignedUserId,
                                Message = todo.Title,
                                CreatedAt = DateTime.Now,
                                IsRead = false,
                                CreatedBy = currentUser
                            };
                            await _unitOfWork.Notifications.Add(notification);

                            await _hub.Clients.User(assignedUserId).SendAsync("ReceiveNotification", new
                            {
                                title = "You received a new ToDo Task!",
                                taskTitle = todo.Title,
                                assignedBy = todo.CreatedBy,
                                message = $"\"{todo.Title}\" has been assigned to you",
                                type = "TaskAssigned",
                                taskId = todo.Id,
                                createdAt = DateTime.UtcNow
                            });
                        }
                    }

                    response.acceptedObjects = accepted;
                }

                if (rejected != null && rejected.Count > 0)
                {
                    response.rejectedObjects = rejected;
                    response.errors = validList.errors ?? new List<string>();
                }
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

        public async Task<MainResponse<ToDo>> ImportFromExcel(
            IFormFile excelFile,
            string currentUser,
            Type userType)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    response.errors.Add("Excel file is empty.");
                    return response;
                }

                var extension = Path.GetExtension(excelFile.FileName).ToLower();

                if (extension != ".xlsx")
                {
                    response.errors.Add("Only .xlsx files are allowed.");
                    return response;
                }

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

                var rows = worksheet.Dimension?.Rows ?? 0;

                if (rows <= 1)
                {
                    response.errors.Add("Excel file has no data rows.");
                    return response;
                }

                if (rows > 10000)
                {
                    response.errors.Add("Maximum allowed rows is 10,000.");
                    return response;
                }

                var parsedRows = new List<(int RowNumber, ToDoDTO Dto)>();
                var excelTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // =========================
                // 1) Parse Excel only
                // =========================
                for (var r = 2; r <= rows; r++)
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
                        ischecked = int.TryParse(worksheet.Cells[r, 5].Text, out var v) ? v : 0
                    };

                    parsedRows.Add((r, dto));
                    excelTitles.Add(title);
                }

                if (!parsedRows.Any())
                {
                    response.errors.Add("No valid rows found.");
                    return response;
                }

                // =========================
                // 2) Query DB once only for matching titles
                // =========================
                var existingTitles = _unitOfWork.ToDos
                    .GetQueryable()
                    .Where(x => x.Title != null && excelTitles.Contains(x.Title))
                    .Select(x => x.Title)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var titlesInExcel = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var todosToAdd = new List<ToDo>();

                // =========================
                // 3) Validate + Map
                // =========================
                foreach (var row in parsedRows)
                {
                    var dto = row.Dto;
                    var rowNumber = row.RowNumber;

                    if (!titlesInExcel.Add(dto.Title))
                    {
                        response.errors.Add($"Row {rowNumber}: Duplicate title inside Excel file.");
                        continue;
                    }

                    var valid = ValidateDTO.ValidateToDoDTO(dto, existingTitles);

                    if (valid.acceptedObjects == null || valid.acceptedObjects.Count == 0)
                    {
                        foreach (var error in valid.errors)
                            response.errors.Add($"Row {rowNumber}: {error}");

                        continue;
                    }

                    var todo = _mapper.Map<ToDo>(valid.acceptedObjects.First());

                    todo.CreatedBy = currentUser;
                    todo.Type = userType;
                    //todo.LastCheckedAt = todo.ischecked == Status.Checked
                    //    ? DateTime.UtcNow
                    //    : null;

                    todosToAdd.Add(todo);

                    existingTitles.Add(dto.Title);
                }

                if (!todosToAdd.Any())
                {
                    if (!response.errors.Any())
                        response.errors.Add("No valid rows found.");

                    return response;
                }

                // =========================
                // 4) Save once
                // =========================
                await _unitOfWork.ToDos.Add(todosToAdd);
                response.acceptedObjects = new List<ToDo>();
                Logs.Log($"User [{currentUser}] imported {todosToAdd.Count} ToDo rows. Errors: {response.errors.Count}");
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
        public async Task<MainResponse<ToDo>> UpdateToDo(int id, ToDoDTO dto, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
      
                var validList = await ValidateDTO.ToDoDTO(dto, true);

                var existingTodo = await _unitOfWork.ToDos.GetFirst(x => x.Id == id);

                if (existingTodo == null)
                {
                    response.errors?.Add($"Cannot find ToDo with ID {id}.");
                    return response;
                }

                if (!CanAccess(existingTodo, currentUser))
                {
                    response.errors?.Add("You are not allowed to update this ToDo.");
                    return response;
                }

                if (validList.acceptedObjects == null || validList.acceptedObjects.Count == 0)
                {
                    response.errors?.Add("No valid payload to update.");
                    return response;
                }

                var validDto = validList.acceptedObjects[0];

                var originalCreatedBy = existingTodo.CreatedBy;
                var originalType = existingTodo.Type;

                _mapper.Map(validDto, existingTodo);

                existingTodo.CreatedBy = originalCreatedBy;
                existingTodo.Type = originalType;

                if (validDto.ischecked == 1)
                    existingTodo.LastCheckedAt = DateTime.Now;
                else if (validDto.ischecked == 0)
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

        public async Task<MainResponse<ToDo>> UpdateStatus(int id, int status, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var todo = await _unitOfWork.ToDos.GetFirst(x => x.Id == id);

                if (todo == null)
                {
                    response.errors.Add($"ToDo with ID {id} not found.");
                    return response;
                }

                if (!CanAccess(todo, currentUser))
                {
                    response.errors.Add("You are not allowed to update this ToDo.");
                    return response;
                }

                todo.ischecked = (Status)status;

                if (status == 1 || status == 2 || status == 3)
                    todo.LastCheckedAt = DateTime.Now;
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

        public async Task<MainResponse<ToDo>> DeleteToDo(int id, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var existingTodo = await _unitOfWork.ToDos.GetFirst(x => x.Id == id);

                if (existingTodo == null)
                {
                    response.errors.Add(_errors.ObjectNotFoundWithId(id));
                    return response;
                }

                if (!CanAccess(existingTodo, currentUser))
                {
                    response.errors.Add("You are not allowed to delete this ToDo.");
                    return response;
                }

                var deleted = await _unitOfWork.ToDos.DeletePhysical(x => x.Id == id);

                var deletedList = deleted?.ToList() ?? new List<ToDo>();

                if (!deletedList.Any())
                {
                    response.errors.Add(_errors.ObjectNotFoundWithId(id));
                    return response;
                }

                response.acceptedObjects = deletedList;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> DeleteAll(string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    response.errors.Add("Current user not found.");
                    return response;
                }

                var todos = await _unitOfWork.ToDos.GetAll(x => x.CreatedBy == currentUser);

                var list = todos?.ToList() ?? new List<ToDo>();

                if (!list.Any())
                {
                    response.errors.Add("There are no ToDos created by this user.");
                    return response;
                }

                var deleted = await _unitOfWork.ToDos.Delete(x => x.CreatedBy == currentUser);
                response.acceptedObjects = deleted?.ToList() ?? new List<ToDo>();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> DeleteGroup(List<int> ids, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                if (ids == null || ids.Count == 0)
                {
                    response.errors.Add("IDs list is empty.");
                    return response;
                }

                response.acceptedObjects = new List<ToDo>();

                foreach (var id in ids.Distinct())
                {
                    var existingTodo = await _unitOfWork.ToDos.GetFirst(x => x.Id == id);

                    if (existingTodo == null)
                    {
                        response.errors.Add($"ToDo with ID = {id} not found.");
                        continue;
                    }

                    if (!CanAccess(existingTodo, currentUser))
                    {
                        response.errors.Add($"You are not allowed to delete ToDo with ID = {id}.");
                        continue;
                    }

                    var deleted = await _unitOfWork.ToDos.Delete(x => x.Id == id);
                    var deletedList = deleted?.ToList() ?? new List<ToDo>();

                    if (deletedList.Any())
                        response.acceptedObjects.AddRange(deletedList);
                }

                if (!response.acceptedObjects.Any() && !response.errors.Any())
                {
                    response.errors.Add("No ToDos were deleted.");
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> GetByAssignedTo(string assignedTo, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                if (string.IsNullOrWhiteSpace(assignedTo))
                {
                    response.errors.Add("AssignedTo is required.");
                    return response;
                }

                if (!string.Equals(assignedTo, currentUser, StringComparison.OrdinalIgnoreCase))
                {
                    response.errors.Add("You are not allowed to access another user's assigned tasks.");
                    return response;
                }

                var todos = await _unitOfWork.ToDos.GetAll(x => x.AssignedTo == assignedTo);
                var list = todos?.ToList() ?? new List<ToDo>();

                if (!list.Any())
                {
                    response.errors.Add(_errors.ObjectNotFound());
                    return response;
                }

                response.acceptedObjects = list;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> GetByCustomer(string customer, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var todos = await _unitOfWork.ToDos.GetAll(x =>
                    x.CustomerName == customer &&
                    (x.CreatedBy == currentUser || x.AssignedTo == currentUser));

                var list = todos?.ToList() ?? new List<ToDo>();

                if (!list.Any())
                {
                    response.errors.Add(_errors.ObjectNotFound());
                    return response;
                }

                response.acceptedObjects = list;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<ToDo>> GetByCustomerAndType(string customer, Type type, string currentUser)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var todos = await _unitOfWork.ToDos.GetAll(x =>
                    x.CustomerName == customer &&
                    x.Type == type &&
                    (x.CreatedBy == currentUser || x.AssignedTo == currentUser));

                var list = todos?.ToList() ?? new List<ToDo>();

                if (!list.Any())
                {
                    response.errors.Add(_errors.ObjectNotFound());
                    return response;
                }

                response.acceptedObjects = list;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        private static bool CanAccess(ToDo todo, string currentUser)
        {
            if (todo == null || string.IsNullOrWhiteSpace(currentUser))
                return false;

            return string.Equals(todo.CreatedBy, currentUser, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(todo.AssignedTo, currentUser, StringComparison.OrdinalIgnoreCase);
        }
    }
}