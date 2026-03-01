using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services
{
    public class ToDoServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<ToDo> Errors = new Errors<ToDo>();

        public ToDoServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET ALL
        public async Task<MainResponse<ToDo>> getToDoList()
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            var todos = await _unitOfWork.ToDos.GetAll();
            
            // OPTIONAL: "Daily reset" display logic (no DB update)
            // If it wasn't checked today -> treat as unchecked in returned objects
            var today = DateTime.UtcNow.Date;
            var x = today;
            foreach (var t in todos)
            {
                // If LastCheckedAt is nullable, use: t.LastCheckedAt?.Date
                // If not nullable (your current model), this still works but new tasks default to "today" (bad).
                var last = t.LastCheckedAt?.Date;

                if (last != today)
                {
                    // force unchecked in response (in-memory only)
                    t.ischecked = (IsChecked)0;
                }
                else
                {
                    t.ischecked = (IsChecked)1;
                }
            }

            response.acceptedObjects = todos.ToList();
            return response;
        }

        // GET BY ID
        public async Task<MainResponse<ToDo>> getToDo(int id)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            ToDo todo = await _unitOfWork.ToDos.GetById(id);

            if (todo == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            // daily reset display logic
            var today = DateTime.UtcNow.Date;
            if (todo.LastCheckedAt?.Date != today)
                todo.ischecked = (IsChecked)0;
            else
                todo.ischecked = (IsChecked)1;

            response.acceptedObjects = new List<ToDo> { todo };
            return response;
        }
        public async Task<MainResponse<ToDo>> getByStatus(int status)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();
            ToDo todo = await _unitOfWork.ToDos.GetFirst(a => (int)a.ischecked == status);
            if(todo == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }
            var today = DateTime.UtcNow.Date;
            if (todo.LastCheckedAt?.Date != today)
                todo.ischecked = (IsChecked)0;
            else
                todo.ischecked = (IsChecked)1;

            response.acceptedObjects = new List<ToDo> { todo };
            return response;

        }
        // ADD
        public async Task<MainResponse<ToDo>> addToDo(List<ToDoDTO> todos)
        {
            MainResponse<ToDo> response = new MainResponse<ToDo>();

            try
            {
                // You need to implement this like ValidateDTO.UserDTO(...)
                var validList = await ValidateDTO.ToDoDTO(todos);

                List<ToDo> todoList = _mapper.Map<List<ToDo>>(validList.acceptedObjects);
                List<ToDo> rejectedList = _mapper.Map<List<ToDo>>(validList.rejectedObjects);

                if (todoList != null && todoList.Count > 0)
                {
                    // Important: Set LastCheckedAt only when checked
                    foreach (var t in todoList)
                    {
                        // dto.ischecked is int -> map/cast to enum
                        // If checked => set LastCheckedAt = now; else set to old date
                        if ((int)t.ischecked == 1)
                            t.LastCheckedAt = DateTime.UtcNow;
                        else
                            t.LastCheckedAt = DateTime.MinValue; // better: null if you change to DateTime?
                    }

                    await _unitOfWork.ToDos.Add(todoList);
                    response.acceptedObjects = todoList;
                }

                if (rejectedList != null && rejectedList.Count > 0)
                {
                    response.rejectedObjects = rejectedList;
                    response.errors = validList.errors ?? new List<string>();
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

        // UPDATE
        public async Task<MainResponse<ToDo>> updateToDo(int id, List<ToDoDTO> todoUpdated)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                // You need to implement this like ValidateDTO.UserDTO(...)
                var validList = await ValidateDTO.ToDoDTO(todoUpdated, true);

                var existingTodo = await _unitOfWork.ToDos.GetFirst(a => a.Id == id);
                if (existingTodo is null)
                {
                    response.errors.Add($"Cannot find ToDo with ID {id}.");

                    if (validList.rejectedObjects?.Any() == true && validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    response.rejectedObjects.AddRange(_mapper.Map<List<ToDo>>(validList.rejectedObjects));
                    return response;
                }

                if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
                {
                    response.errors.Add("No valid payload to update ToDo. Fix validation errors and try again.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<ToDo>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                // map dto -> entity
                _mapper.Map(dto, existingTodo);

                // handle LastCheckedAt based on ischecked
                // If checked => set now; if unchecked => set old date (or null if you change the model)
                if (dto.ischecked == 1)
                    existingTodo.LastCheckedAt = DateTime.UtcNow;
                else
                    existingTodo.LastCheckedAt = DateTime.MinValue; // better: null with DateTime?

                await _unitOfWork.ToDos.Update(existingTodo);

                response.acceptedObjects.Add(existingTodo);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<ToDo>>(validList.rejectedObjects));

                if (validList.errors?.Any() == true)
                    response.errors.AddRange(validList.errors);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
                if (ex.InnerException != null) response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }

        //UPDATE STATUS ONLY
        public async Task<MainResponse<ToDo>> UpdateStatus(int id, int status)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var existingTicket =
                    await _unitOfWork.ToDos.GetFirst(t => t.Id == id);

                if (existingTicket == null)
                {
                    response.errors.Add($"Ticket with ID {id} not found.");
                    return response;
                }

                existingTicket.ischecked = (IsChecked)status;

                await _unitOfWork.ToDos.Update(existingTicket);

                response.acceptedObjects ??= new List<ToDo>();
                response.acceptedObjects.Add(existingTicket);
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

            var todo = await _unitOfWork.ToDos.DeletePhysical(p => p.Id == id);

            if (todo == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<ToDo> { todo.First() };
            return response;
        }

        //toggle check/uncheck
        public async Task<MainResponse<ToDo>> toggleCheck(int id, bool check)
        {
            var response = new MainResponse<ToDo>();

            try
            {
                var existingTodo = await _unitOfWork.ToDos.GetFirst(a => a.Id == id);
                if (existingTodo is null)
                {
                    response.errors.Add($"Cannot find ToDo with ID {id}.");
                    return response;
                }

                if (check)
                    existingTodo.LastCheckedAt = DateTime.UtcNow;
                else
                    existingTodo.LastCheckedAt = null;

                existingTodo.ischecked = check ? (IsChecked)1 : (IsChecked)0;

                await _unitOfWork.ToDos.Update(existingTodo);
                response.acceptedObjects.Add(existingTodo);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
                if (ex.InnerException != null) response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }
    }
}