using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Services
{
    public class GoalServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<Goal> Errors = new Errors<Goal>();


        public GoalServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task<MainResponse<Goal>> getGoalsList()
        {
            MainResponse<Goal> response = new MainResponse<Goal>();

            var goals = await _unitOfWork.Goals.GetAll();

            response.acceptedObjects = goals.ToList();

            return response;
        }
        public async Task<MainResponse<Goal>> getGoal(int id)
        {
            MainResponse<Goal> response = new MainResponse<Goal>();

            var goal = await _unitOfWork.Goals.GetById(id);

            if (goal == null)
            {
                response.errors.Add(Errors.ObjectNotFound());
                return response;
            }

            response.acceptedObjects = new List<Goal> { goal };

            return response;
        }
        public async Task<MainResponse<Goal>> addGoal(List<GoalDTO> goalDTOs)
        {
            MainResponse<Goal> response = new MainResponse<Goal>();

            try
            {
                var validList = await ValidateDTO.GoalDTO(goalDTOs);

                List<Goal> goalList =_mapper.Map<List<Goal>>(validList.acceptedObjects);

                List<Goal> rejectedGoals = _mapper.Map<List<Goal>>(validList.rejectedObjects);

                if (goalList != null && goalList.Count > 0)
                {
                    await _unitOfWork.Goals.Add(goalList);
                    response.acceptedObjects = goalList;
                }

                if (rejectedGoals != null && rejectedGoals.Count > 0)
                {
                    response.rejectedObjects = rejectedGoals;
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
        public async Task<MainResponse<Goal>> updateGoal(int id, List<GoalDTO> goalUpdated)
        {
            var response = new MainResponse<Goal>();

            try
            {
                var validList = await ValidateDTO.GoalDTO(goalUpdated, true);

                var existingGoal =
                    await _unitOfWork.Goals.GetFirst(g => g.Id == id);

                if (existingGoal is null)
                {
                    response.errors.Add($"Cannot find Goal with Id {id}.");

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(
                            _mapper.Map<List<Goal>>(validList.rejectedObjects));

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    return response;
                }

                if (validList.acceptedObjects == null ||
                    validList.acceptedObjects.Count == 0)
                {
                    response.errors.Add("No valid payload to update Goal.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(
                            _mapper.Map<List<Goal>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingGoal);

                await _unitOfWork.Goals.Update(existingGoal);

                response.acceptedObjects.Add(existingGoal);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(
                        _mapper.Map<List<Goal>>(validList.rejectedObjects));

                if (validList.errors?.Any() == true)
                    response.errors.AddRange(validList.errors);
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
        public async Task<MainResponse<Goal>> deleteGoal(int id)
        {
            MainResponse<Goal> response = new MainResponse<Goal>();

            var goal =
                await _unitOfWork.Goals.DeletePhysical(g => g.Id == id);

            if (goal == null)
            {
                response.errors.Add(Errors.ObjectNotFoundWithId(id));
                return response;
            }

            response.acceptedObjects = new List<Goal> { goal.First() };

            return response;
        }
        public async Task<MainResponse<Goal>> deleteAll()
        {
            MainResponse<Goal> response = new MainResponse<Goal>();
            try
            {
                var deletedLeads = await _unitOfWork.Goals.DeletePhysical(p => true);
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
}
