using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class UserServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<User> Errors = new Errors<User>();
    public UserServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<User>> getUserList()
    {
        MainResponse<User> response = new MainResponse<User>();
        var user = await _unitOfWork.Users.GetAll();
        response.acceptedObjects = user.ToList();
        return response;
    }
    public async Task<MainResponse<User>> getUser(int id)
    {
        MainResponse<User> response = new MainResponse<User>();
        var user = await _unitOfWork.Users.GetById(id);

        if (user == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<User> { user };
        return response;
    }
    public async Task<MainResponse<User>> updateUser(int id, List<UserDTO> userUpdated)
    {
        var response = new MainResponse<User>();

        try
        {
            var validList = await ValidateDTO.UserDTO(userUpdated , true);

            var existingUser = await _unitOfWork.Users.GetFirst(a => a.Id == id);
            if (existingUser is null)
            {
                response.errors.Add($"Cannot find User with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<User>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update User. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<User>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingUser);

            await _unitOfWork.Users.Update(existingUser);

            response.acceptedObjects.Add(existingUser);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<User>>(validList.rejectedObjects));
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

    public async Task<MainResponse<User>> addUser(List<UserDTO> user)
    {
        MainResponse<User> response = new MainResponse<User>();
        try
        {
            var validList = await ValidateDTO.UserDTO(user);

            List<User> userlist = _mapper.Map<List<User>>(validList.acceptedObjects);
            List<User> rejecteduser = _mapper.Map<List<User>>(validList.rejectedObjects);
            if (userlist != null && userlist.Count() > 0)
            {
                var userlists = await _unitOfWork.Users.Add(userlist);
                response.acceptedObjects = userlist;
            }
            if (rejecteduser != null && rejecteduser.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejecteduser;
                response.errors = err;
            }
            return response;
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            return response;
        }
    }
    public async Task<MainResponse<User>> deleteUser(int id)
    {
        MainResponse<User> response = new MainResponse<User>();

        var user = await _unitOfWork.Users.DeletePhysical(p => p.Id == id);

        if (user == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<User> { user.First() };
        return response;
    }
}
