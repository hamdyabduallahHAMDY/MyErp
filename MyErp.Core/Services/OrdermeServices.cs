using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services.ServicesUtilities
{
    public class OrdermeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServicesUtilities _servicesUtilities;
        private readonly IMapper _mapper;
        private readonly Errors<Orderme> _errors = new();

        public OrdermeServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _servicesUtilities = new(unitOfWork);
            _mapper = mapper;
        }

        // 1. List all orders with details
        public async Task<MainResponse<Orderme>> GetOrdermeList()
        {
            var response = new MainResponse<Orderme>();
            var orders = (await _unitOfWork.Ordermes.GetAll()).ToList();

            foreach (var order in orders)
            {
                var details = await _unitOfWork.Ordermedetails.Find(x => x.OrdermeId == order.Id);
                order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();
            }

            response.acceptedObjects = orders;
            return response;
        }

        // 2. Get order by ID with details
        public async Task<MainResponse<Orderme>> GetOrderme(int id)
        {
            var response = new MainResponse<Orderme>();
            var order = await _unitOfWork.Ordermes.GetById(id);

            if (order == null)
            {
                response.errors = new List<string> { _errors.ObjectNotFound() };
                return response;
            }

            var details = await _unitOfWork.Ordermedetails.Find(x => x.OrdermeId == id);
            order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();

            response.acceptedObjects = new List<Orderme> { order };
            return response;
        }

        // 3. Add new orders with totals and stock actions
        public async Task<MainResponse<Orderme>> AddOrdermes(List<OrderCreateDTO> dtoList)
        {
            var response = new MainResponse<Orderme>();

            try
            {

                //firstValidation
                #region
                // 1) DTO validation (structure, FKs, enums) — no price compare
                var valid = await ValidateDTO.OrdermeDTO(dtoList);
                // 2) Map DTOs ? Entities
                var acceptedEntities = _mapper.Map<List<Orderme>>(valid.acceptedObjects) ?? new List<Orderme>();
                var rejectedEntities = _mapper.Map<List<Orderme>>(valid.rejectedObjects) ?? new List<Orderme>();
                List<String> err = (valid.errors);
                response.rejectedObjects = rejectedEntities;
                response.errors = err;
                #endregion

                // If there are rejected entities, collect their errors
                if (rejectedEntities.Any())
                {
                    // Add errors from rejected objects into the response
                    foreach (var rejectedEntity in rejectedEntities)
                    {
                        response.rejectedObjects = rejectedEntities;
                        return response;
                    }
                }

                //if the Order is  OfferPrice
                if (acceptedEntities.All(x => (int)x.orderType == 7 && ((int)x.baseordertype == 1 || (int)x.baseordertype == 4)))
                {


                    foreach (var order in acceptedEntities)
                    {
                        ValidationUtilities.CalculateOrderTotal(order);
                        ValidationUtilities.FillSomeVariables(_unitOfWork, order);
                        var x = await _unitOfWork.Ordermes.Add(acceptedEntities);
                        response.acceptedObjects.Add(order);


                        return response;
                    }


                    var added = await _unitOfWork.Ordermes.Add(acceptedEntities);

                }

                //if the Order is PurchaseOrSaleOrder
                if (acceptedEntities.All(x => (int)x.orderType == 7 && ((int)x.baseordertype == 2 || (int)x.baseordertype == 5)))
                {


                    // Process accepted entities (this part is unchanged)
                    foreach (var order in acceptedEntities)
                    {
                        ValidationUtilities.CalculateOrderTotal(order);
                        ValidationUtilities.FillSomeVariables(_unitOfWork, order);
                        if (acceptedEntities.Any())
                        {
                            response.acceptedObjects.Add(order);
                            //    return response;
                        }
                    }


                    // 3) perform domain validations
                    foreach (var order in acceptedEntities)
                    {
                        var stockErrors = await ValidationUtilities.ValidateStockAsync(_unitOfWork, new List<Orderme> { order });
                        var cashErrors = await ValidationUtilities.ValidateCashAsync(_unitOfWork, new List<Orderme> { order });
                        var domainErrors = new List<string>();
                        if (stockErrors?.Any() == true) domainErrors.AddRange(stockErrors);
                        if (cashErrors?.Any() == true) domainErrors.AddRange(cashErrors);

                        // If there are any domain errors, add them to the response
                        if (domainErrors.Any())
                        {
                            response.rejectedObjects ??= new List<Orderme>();
                            response.rejectedObjects.Add(order);
                            response.acceptedObjects.Remove(order);
                            // Remove the order from accepted list as it failed during the calculation
                            acceptedEntities.Remove(order);
                            return response;
                        }
                        var added = await _unitOfWork.Ordermes.Add(acceptedEntities);

                    }

                    // Check if there are any errors before saving
                    if (response.errors.Any())
                    {
                        // Stop the save process if errors exist
                        return response;
                    }


                }

                //AdditionRequest
                if (acceptedEntities.All(x => (int)x.orderType == 7 && ((int)x.offerprice != 3 || (int)x.offerprice != 6)))
                {


                    // Process accepted entities (this part is unchanged)
                    foreach (var order in acceptedEntities)
                    {
                        ValidationUtilities.CalculateOrderTotal(order);
                        ValidationUtilities.FillSomeVariables(_unitOfWork, order);
                        if (acceptedEntities.Any())
                        {
                            response.acceptedObjects.Add(order);
                            //    return response;
                        }
                    }


                    // 3) perform domain validations
                    foreach (var order in acceptedEntities)
                    {
                        var stockErrors = await ValidationUtilities.ValidateStockAsync(_unitOfWork, new List<Orderme> { order });
                        // var cashErrors = await ValidationUtilities.ValidateCashAsync(_unitOfWork, new List<Orderme> { order });
                        var domainErrors = new List<string>();
                        if (stockErrors?.Any() == true) domainErrors.AddRange(stockErrors);
                        //          if (cashErrors?.Any() == true) domainErrors.AddRange(cashErrors);

                        // If there are any domain errors, add them to the response
                        if (domainErrors.Any())
                        {
                            response.rejectedObjects ??= new List<Orderme>();
                            response.rejectedObjects.Add(order);
                            response.acceptedObjects.Remove(order);
                            // Remove the order from accepted list as it failed during the calculation
                            acceptedEntities.Remove(order);
                            return response;
                        }
                        var added = await _unitOfWork.Ordermes.Add(acceptedEntities);

                    }


                    // Check if there are any errors before saving
                    if (response.errors.Any())
                    {
                        // Stop the save process if errors exist
                        return response;
                    }


                    // 5)  post-save actions (cash flow and stock actions)

                    //    var cashFlowService = new CashFlowService(_unitOfWork);
                    foreach (var order in acceptedEntities)
                    {
                        try
                        {
                            // Record Cash Flow
                            int ot = Convert.ToInt32((int)order.orderType);
                            bool isSales = (ot == 0 || ot == 2);
                            bool isPurchase = (ot == 1 || ot == 5);
                            bool isSalesReturn = (ot == 3);
                            bool isPurchaseReturn = (ot == 4);
                            bool isInflow = isSales || isPurchaseReturn;

                            //await cashFlowService.RecordCashMovementAsync(
                            //    order.CashAndBankId,
                            //    order.Id,
                            //    order.TotalPrice,
                            //    isInflow, // true=inflow, false=outflow
                            //    order.orderType.ToString(),
                            //    order.CustomerName
                            //);

                            // Record Stock Actions
                            if (order.Ordermedetails != null && order.Ordermedetails.Any())
                            {
                                int sign = (isSales || isPurchaseReturn) ? -1 : +1;

                                var rows = new List<StockActionDetails>();
                                foreach (var detail in order.Ordermedetails)
                                {
                                    var x = await _unitOfWork.Products.GetById(detail.ProductId);
                                    var prodname = x.Name;
                                    var last = (await _unitOfWork.StockActionDetailss
                                        .Find(x => x.ProductId == detail.ProductId && x.StockId == order.StockId))
                                        .OrderByDescending(x => x.Id)
                                        .FirstOrDefault();

                                    int prevFinal = last?.FinalValue ?? 0;

                                    rows.Add(new StockActionDetails
                                    {
                                        OrdermeId = order.Id,
                                        ProductId = detail.ProductId,
                                        StockId = order.StockId,
                                        qty = detail.qty,
                                        price = detail.price,
                                        Total = detail.qty * detail.price,
                                        FinalValue = prevFinal + (sign * detail.qty),
                                        Notes = order.Notes1,
                                        CategoryId = detail.categoryId,
                                        UnitTypes = detail.unitcode,
                                        productName = prodname,
                                        internalId = order.internalId,
                                        serialnumber = detail.serialnumber,
                                        createdProddate = detail.createdProddate,
                                        expiredate = detail.expiredate
                                    });
                                }

                                if (rows.Any())
                                {
                                    await _unitOfWork.StockActionDetailss.Add(rows);

                                    var stockReqDTOs = new List<StockReqDTO>();

                                    //var stockReqDTO = new StockReqDTO
                                    //{
                                    //    OrdermeId = order.Id.ToString(), // Get OrderId from Orderme
                                    //    Status = true,
                                    //    Details = order.Ordermedetails.Select(d => new StockReqDetailDTO
                                    //    {
                                    //        ProductId = d.ProductId,  // Map ProductId from Ordermedetail
                                    //        Quantity = d.qty         // Map Quantity from Ordermedetail
                                    //    }).ToList()
                                    //};
                                    //stockReqDTOs.Add(stockReqDTO);
                                    //var stockReqServices = new StockReqServices(_unitOfWork, _mapper);
                                    //await stockReqServices.addReq(stockReqDTOs);
                                }


                            }
                        }
                        catch (Exception ex)
                        {

                            response.errors ??= new List<string>();
                            response.errors.Add($"Post-save actions failed for Order #{order.internalId}: {ex.Message}");
                        }
                    }
                }

                //if the Order is FinalOrder
                if (acceptedEntities.All(x => (int)x.orderType == 0 || (int)x.orderType == 1 && x.offerprice != 0 || x.offerprice != null))
                {


                    // Process accepted entities (this part is unchanged)
                    foreach (var order in acceptedEntities)
                    {
                        ValidationUtilities.CalculateOrderTotal(order);
                        ValidationUtilities.FillSomeVariables(_unitOfWork, order);
                        if (acceptedEntities.Any())
                        {
                            response.acceptedObjects.Add(order);
                            //    return response;
                        }
                    }


                    // 3) perform domain validations
                    foreach (var order in acceptedEntities)
                    {
                        var stockErrors = await ValidationUtilities.ValidateStockAsync(_unitOfWork, new List<Orderme> { order });
                        var cashErrors = await ValidationUtilities.ValidateCashAsync(_unitOfWork, new List<Orderme> { order });
                        var domainErrors = new List<string>();
                        if (stockErrors?.Any() == true) domainErrors.AddRange(stockErrors);
                        //          if (cashErrors?.Any() == true) domainErrors.AddRange(cashErrors);

                        // If there are any domain errors, add them to the response
                        if (domainErrors.Any())
                        {
                            response.rejectedObjects ??= new List<Orderme>();
                            response.rejectedObjects.Add(order);
                            response.acceptedObjects.Remove(order);
                            // Remove the order from accepted list as it failed during the calculation
                            acceptedEntities.Remove(order);
                            return response;
                        }
                        var added = await _unitOfWork.Ordermes.Add(acceptedEntities);

                    }


                    // Check if there are any errors before saving
                    if (response.errors.Any())
                    {
                        // Stop the save process if errors exist
                        return response;
                    }


                    // 5)  post-save actions (cash flow and stock actions)

                    var cashFlowService = new CashFlowServices(_unitOfWork, _mapper);
                    foreach (var order in acceptedEntities)
                    {
                        try
                        {
                            // Record Cash Flow
                            int ot = Convert.ToInt32((int)order.orderType);
                            bool isSales = (ot == 0 || ot == 2);
                            bool isPurchase = (ot == 1 || ot == 5);
                            bool isSalesReturn = (ot == 3);
                            bool isPurchaseReturn = (ot == 4);
                            bool isInflow = isSales || isPurchaseReturn;

                            await cashFlowService.RecordCashMovementAsync(
                                order.CashAndBankId,
                                order.Id,
                                order.TotalPrice,
                                isInflow, // true=inflow, false=outflow
                                order.orderType.ToString(),
                                order.CustomerName
                            );

                            // Record Stock Actions
                            if (order.Ordermedetails != null && order.Ordermedetails.Any())
                            {
                                int sign = (isSales || isPurchaseReturn) ? -1 : +1;

                                //var rows = new List<StockActionDetails>();
                                //foreach (var detail in order.Ordermedetails)
                                //{
                                //    var x = await _unitOfWork.Products.GetById(detail.ProductId);
                                //    var prodname = x.Name;
                                //    var last = (await _unitOfWork.StockActionDetailss
                                //        .Find(x => x.ProductId == detail.ProductId && x.StockId == order.StockId))
                                //        .OrderByDescending(x => x.Id)
                                //        .FirstOrDefault();

                                //    int prevFinal = last?.FinalValue ?? 0;

                                //    rows.Add(new StockActionDetails
                                //    {
                                //        OrdermeId = order.Id,
                                //        ProductId = detail.ProductId,
                                //        StockId = order.StockId,
                                //        qty = detail.qty,
                                //        price = detail.price,
                                //        Total = detail.qty * detail.price,
                                //        FinalValue = prevFinal + (sign * detail.qty),
                                //        Notes = order.Notes1,
                                //        CategoryId = detail.categoryId,
                                //        UnitTypes = detail.unitcode,
                                //        productName = prodname,
                                //        internalId = order.internalId,
                                //        serialnumber = detail.serialnumber,
                                //        createdProddate = detail.createdProddate,
                                //        expiredate = detail.expiredate
                                //    });
                                //}

                                //if (rows.Any())
                                //{
                                //    await _unitOfWork.StockActionDetailss.Add(rows);

                                //    var stockReqDTOs = new List<StockReqDTO>();

                                //var stockReqDTO = new StockReqDTO
                                //{
                                //    OrdermeId = order.Id.ToString(), // Get OrderId from Orderme
                                //    Status = true,
                                //    Details = order.Ordermedetails.Select(d => new StockReqDetailDTO
                                //    {
                                //        ProductId = d.ProductId,  // Map ProductId from Ordermedetail
                                //        Quantity = d.qty         // Map Quantity from Ordermedetail
                                //    }).ToList()
                                //};
                                //stockReqDTOs.Add(stockReqDTO);
                                //var stockReqServices = new StockReqServices(_unitOfWork, _mapper);
                                //await stockReqServices.addReq(stockReqDTOs);
                            }


                        }

                        catch (Exception ex)
                        {

                            response.errors ??= new List<string>();
                            response.errors.Add($"Post-save actions failed for Order #{order.internalId}: {ex.Message}");
                        }
                    }
                }


                //if the Order is DirectOrder
                if (acceptedEntities.All(x => x.orderType == 0 || (int)x.orderType == 1 && x.offerprice == 0 || x.offerprice == null))
                {


                    // Process accepted entities (this part is unchanged)
                    foreach (var order in acceptedEntities)
                    {
                        ValidationUtilities.CalculateOrderTotal(order);
                        ValidationUtilities.FillSomeVariables(_unitOfWork, order);
                        if (acceptedEntities.Any())
                        {
                            response.acceptedObjects.Add(order);
                            //    return response;
                        }
                    }


                    // 3) perform domain validations
                    foreach (var order in acceptedEntities)
                    {
                        var stockErrors = await ValidationUtilities.ValidateStockAsync(_unitOfWork, new List<Orderme> { order });
                        var cashErrors = await ValidationUtilities.ValidateCashAsync(_unitOfWork, new List<Orderme> { order });
                        var domainErrors = new List<string>();
                        if (stockErrors?.Any() == true) domainErrors.AddRange(stockErrors);
                        //          if (cashErrors?.Any() == true) domainErrors.AddRange(cashErrors);

                        // If there are any domain errors, add them to the response
                        if (domainErrors.Any())
                        {
                            response.rejectedObjects ??= new List<Orderme>();
                            response.rejectedObjects.Add(order);
                            response.acceptedObjects.Remove(order);
                            // Remove the order from accepted list as it failed during the calculation
                            acceptedEntities.Remove(order);
                            return response;
                        }
                        var added = await _unitOfWork.Ordermes.Add(acceptedEntities);

                    }


                    // Check if there are any errors before saving
                    if (response.errors.Any())
                    {
                        // Stop the save process if errors exist
                        return response;
                    }


                    // 5)  post-save actions (cash flow and stock actions)

                    var cashFlowService = new CashFlowServices(_unitOfWork, _mapper);
                    foreach (var order in acceptedEntities)
                    {
                        try
                        {
                            // Record Cash Flow
                            int ot = Convert.ToInt32((int)order.orderType);
                            bool isSales = (ot == 0 || ot == 2);
                            bool isPurchase = (ot == 1 || ot == 5);
                            bool isSalesReturn = (ot == 3);
                            bool isPurchaseReturn = (ot == 4);
                            bool isInflow = isSales || isPurchaseReturn;

                            await cashFlowService.RecordCashMovementAsync(
                                order.CashAndBankId,
                                order.Id,
                                order.TotalPrice,
                                isInflow, // true=inflow, false=outflow
                                order.orderType.ToString(),
                                order.CustomerName
                            );

                            // Record Stock Actions
                            if (order.Ordermedetails != null && order.Ordermedetails.Any())
                            {
                                int sign = (isSales || isPurchaseReturn) ? -1 : +1;

                                var rows = new List<StockActionDetails>();
                                foreach (var detail in order.Ordermedetails)
                                {
                                    var x = await _unitOfWork.Products.GetById(detail.ProductId);
                                    var prodname = x.Name;
                                    var last = (await _unitOfWork.StockActionDetailss
                                        .Find(x => x.ProductId == detail.ProductId && x.StockId == order.StockId))
                                        .OrderByDescending(x => x.Id)
                                        .FirstOrDefault();

                                    int prevFinal = last?.FinalValue ?? 0;

                                    rows.Add(new StockActionDetails
                                    {
                                        OrdermeId = order.Id,
                                        ProductId = detail.ProductId,
                                        StockId = order.StockId,
                                        qty = detail.qty,
                                        price = detail.price,
                                        Total = detail.qty * detail.price,
                                        FinalValue = prevFinal + (sign * detail.qty),
                                        Notes = order.Notes1,
                                        CategoryId = detail.categoryId,
                                        UnitTypes = detail.unitcode,
                                        productName = prodname,
                                        internalId = order.internalId,
                                        serialnumber = detail.serialnumber,
                                        createdProddate = detail.createdProddate,
                                        expiredate = detail.expiredate
                                    });
                                }

                                if (rows.Any())
                                {
                                    //await _unitOfWork.StockActionDetailss.Add(rows);

                                    //var stockReqDTOs = new List<StockReqDTO>();

                                    ////var stockReqDTO = new StockReqDTO
                                    ////{
                                    ////    OrdermeId = order.Id.ToString(), // Get OrderId from Orderme
                                    ////    Status = true,
                                    ////    Details = order.Ordermedetails.Select(d => new StockReqDetailDTO
                                    ////    {
                                    ////        ProductId = d.ProductId,  // Map ProductId from Ordermedetail
                                    ////        Quantity = d.qty         // Map Quantity from Ordermedetail
                                    ////    }).ToList()
                                    ////};
                                    //stockReqDTOs.Add(stockReqDTO);
                                    //var stockReqServices = new StockReqServices(_unitOfWork, _mapper);
                                    //await stockReqServices.addReq(stockReqDTOs);
                                }

                            }
                        }

                        catch (Exception ex)
                        {

                            response.errors ??= new List<string>();
                            response.errors.Add($"Post-save actions failed for Order #{order.internalId}: {ex.Message}");
                        }
                    }
                }


                //return finito
                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { "Internal server error. " + ex.Message };
                return response;
            }
        }


        // 4. Update order and details
        public async Task<MainResponse<Orderme>> UpdateStockActions(int id, List<OrderCreateDTO> dtoList)
        {
            var response = new MainResponse<Orderme>();
            try
            {
                // 1. Validate DTOs
                var valid = await ValidateDTO.OrdermeDTO(dtoList, true);
                var acceptedEntities = _mapper.Map<List<Orderme>>(valid.acceptedObjects) ?? new List<Orderme>();
                var rejectedEntities = _mapper.Map<List<Orderme>>(valid.rejectedObjects) ?? new List<Orderme>();

                var existingOrder = await _unitOfWork.Ordermes.GetById(id);
                if (existingOrder == null)
                {
                    response.errors = new List<string> { _errors.ObjectNotFoundWithId(id) };
                    return response;
                }

                if (!acceptedEntities.Any())
                {
                    response.errors = valid.errors ?? new List<string> { _errors.ObjectNotFoundWithId(id) };
                    response.rejectedObjects = rejectedEntities;
                    return response;
                }

                var updatedOrder = acceptedEntities[0];

                // 2. Compute totals
                ValidationUtilities.CalculateOrderTotal(updatedOrder);
                ValidationUtilities.FillSomeVariables(_unitOfWork, updatedOrder);

                // 3. Domain validations
                var stockErrors = await ValidationUtilities.ValidateStockAsync(_unitOfWork, new List<Orderme> { updatedOrder });
                var cashErrors = await ValidationUtilities.ValidateCashAsync(_unitOfWork, new List<Orderme> { updatedOrder });
                var domainErrors = new List<string>();
                if (stockErrors?.Any() == true) domainErrors.AddRange(stockErrors);
                if (cashErrors?.Any() == true) domainErrors.AddRange(cashErrors);

                if (domainErrors.Any())
                {
                    response.errors = (valid.errors ?? new List<string>()).Concat(domainErrors).ToList();
                    response.rejectedObjects = rejectedEntities;
                    return response;
                }

                // 4. Update order header
                _mapper.Map(existingOrder, updatedOrder);

                await _unitOfWork.Ordermes.Update(new List<Orderme> { existingOrder });

                // 5. Update order details
                var updatedDetails = updatedOrder.Ordermedetails ?? new List<Ordermedetail>();
                var currentDetails = (await _unitOfWork.Ordermedetails.Find(d => d.OrdermeId == id)).ToList();
                var updatedDetailIds = new HashSet<int>(updatedDetails.Where(d => d.Id != 0).Select(d => d.Id));

                var newDetails = new List<Ordermedetail>();
                var detailsToUpdate = new List<Ordermedetail>();

                foreach (var detail in updatedDetails)
                {
                    detail.OrdermeId = id;

                    if (detail.Id != 0)
                    {
                        var existingDetail = currentDetails.FirstOrDefault(d => d.Id == detail.Id);
                        if (existingDetail != null)
                        {
                            _mapper.Map(existingDetail, detail);
                            detailsToUpdate.Add(existingDetail);
                        }
                    }
                    else
                    {
                        detail.Id = 0;
                        newDetails.Add(detail);
                    }
                }

                if (newDetails.Any())
                    await _unitOfWork.Ordermedetails.Add(newDetails);

                if (detailsToUpdate.Any())
                    await _unitOfWork.Ordermedetails.Update(detailsToUpdate);

                var detailsToDelete = currentDetails
                    .Where(cd => !updatedDetailIds.Contains(cd.Id))
                    .ToList();

                foreach (var detail in detailsToDelete)
                    await _unitOfWork.Ordermedetails.DeletePhysical(d => d.Id == detail.Id);

                // 6. Update stock actions (remove old, add new)
                // Remove old stock actions for this order
                var oldStockActions = await _unitOfWork.StockActionDetailss.Find(s => s.OrdermeId == id);
                foreach (var sa in oldStockActions)
                    await _unitOfWork.StockActionDetailss.DeletePhysical(x => x.Id == sa.Id);

                // Add new stock actions
                int ot = Convert.ToInt32(existingOrder.orderType);
                bool isSales = (ot == 0 || ot == 2);
                bool isPurchase = (ot == 1 || ot == 5);
                bool isSalesReturn = (ot == 3);
                bool isPurchaseReturn = (ot == 4);
                int sign = (isSales || isPurchaseReturn) ? -1 : +1;

                var rows = new List<StockActionDetails>();
                foreach (var d in updatedDetails)
                {
                    int stockId = TryGetDetailStockId(d) ?? existingOrder.StockId;
                    if (d.ProductId <= 0 || stockId <= 0) continue;

                    var last = (await _unitOfWork.StockActionDetailss
                        .Find(x => x.ProductId == d.ProductId && x.StockId == stockId))
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

                    int prevFinal = last?.FinalValue ?? 0;

                    rows.Add(new StockActionDetails
                    {
                        StockActionsId = null,
                        StockActiontransferId = null,
                        OrdermeId = existingOrder.Id,
                        ProductId = d.ProductId,
                        StockId = stockId,
                        qty = d.qty,
                        price = d.price,
                        Total = d.qty * d.price,
                        FinalValue = prevFinal + (sign * d.qty),
                        Notes = existingOrder.Notes1,
                        CategoryId = d.categoryId,
                        UnitTypes = d.unitcode,
                        productName = d.Productname,
                        internalId = existingOrder.internalId,
                        serialnumber = d.serialnumber,
                        createdProddate = d.createdProddate,
                        expiredate = d.expiredate,
                    });
                }
                if (rows.Any())
                    await _unitOfWork.StockActionDetailss.Add(rows);

                // 7. Finalize
                response.acceptedObjects = new List<Orderme> { existingOrder };
                if (rejectedEntities.Any())
                {
                    response.rejectedObjects = rejectedEntities;
                    if (valid.errors?.Any() == true)
                        response.errors = (response.errors ?? new List<string>()).Concat(valid.errors).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { "Internal server error. " + ex.Message };
                return response;
            }

            static int? TryGetDetailStockId(Ordermedetail detail)
            {
                var pi = detail.GetType().GetProperty("StockId");
                if (pi == null) return null;
                var raw = pi.GetValue(detail);
                return raw as int? ?? (raw is int v ? v : (int?)null);
            }
        }
        // 5. Delete order
        public async Task<MainResponse<Orderme>> deleteStockActions(int id)
        {
            MainResponse<Orderme> response = new MainResponse<Orderme>();

            // 1. First get the StockAction with all its details
            var stockAction = await _unitOfWork.Ordermes.GetById(id);

            if (stockAction == null)
            {
                string error = _errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }

            // 2. Get all related StockActionDetails
            var details = await _unitOfWork.Ordermedetails.Find(d => d.OrdermeId == id);

            // 3. Delete all details using your deleteStockActionDetails() method
            foreach (var detail in details)
            {
                await deleteStockActionDetails(detail.Id);
            }

            // 4. Now delete the main StockAction
            var area = await _unitOfWork.Ordermes.DeletePhysical(p => p.Id == id);

            if (area == null)
            {
                string error = _errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<Orderme> { area.First() };
            return response;
        }

        //
        public async Task<MainResponse<Ordermedetail>> deleteStockActionDetails(int id)
        {
            MainResponse<Ordermedetail> response = new MainResponse<Ordermedetail>();

            var area = await _unitOfWork.Ordermedetails.DeletePhysical(p => p.Id == id);

            if (area == null)
            {
                string error = _errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }
            response.acceptedObjects = new List<Ordermedetail> { area.First() };
            return response;
        }

        // 6. Orders by product, with details
        public async Task<MainResponse<OrdermeDTO>> GetOrdersByProductId(int productId, int orderType, DateTime? startDate = null, DateTime? endDate = null)
        {
            var response = new MainResponse<OrdermeDTO>();

            try
            {
                if (startDate.HasValue && startDate.Value == DateTime.MinValue) startDate = null;
                if (endDate.HasValue && endDate.Value == DateTime.MinValue) endDate = null;
                if (endDate.HasValue)
                    endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);

                var orders = await _unitOfWork.Ordermes.GetBy(o =>
                    o.Ordermedetails.Any(d => d.ProductId == productId)
                    && Convert.ToInt32(o.orderType) == orderType
                    && (!startDate.HasValue || o.DateTime >= startDate.Value)
                    && (!endDate.HasValue || o.DateTime <= endDate.Value)
                );

                if (orders == null || !orders.Any())
                {
                    response.errors = new List<string> { "No orders found matching the given criteria." };
                    return response;
                }

                foreach (var order in orders)
                {
                    var details = await _unitOfWork.Ordermedetails.Find(d => d.OrdermeId == order.Id && d.ProductId == productId);
                    order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();
                }

                var dtoOrders = _mapper.Map<List<OrdermeDTO>>(orders);
                response.acceptedObjects = dtoOrders;
                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { "Internal server error. " + ex.Message };
                return response;
            }
        }

        // 7. Ordermedetails by ProductId
        public async Task<MainResponse<OrdermedetailDTO>> GetOrdermedetailsByProductId(int productId)
        {
            var response = new MainResponse<OrdermedetailDTO>();
            try
            {
                var details = await _unitOfWork.Ordermedetails.GetAll(d => d.ProductId == productId);

                if (details == null || !details.Any())
                {
                    response.errors = new List<string> { "No Ordermedetails found for this product." };
                    return response;
                }

                response.acceptedObjects = _mapper.Map<List<OrdermedetailDTO>>(details);
                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { "Internal server error. " + ex.Message };
                return response;
            }
        }

        // 8. Orders by invoice type and payment method
        public async Task<MainResponse<Orderme>> GetOrdersByInvoiceTypeAndWayOfPayment(int invoiceType, List<int> wayOfPayment)
        {
            var response = new MainResponse<Orderme>();

            var allOrders = await _unitOfWork.Ordermes.GetAll(x => (int)x.orderType == invoiceType);

            if (wayOfPayment != null && wayOfPayment.Any())
                allOrders = allOrders.Where(x => wayOfPayment.Contains((int)x.wayofpayemnt));

            var ordersList = allOrders.ToList();

            foreach (var order in ordersList)
            {
                var details = await _unitOfWork.Ordermedetails.GetAll(x => x.OrdermeId == order.Id);
                order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();
                foreach (var detail in order.Ordermedetails)
                    detail.Orderme = null; // prevent circular reference
            }

            response.acceptedObjects = ordersList;
            return response;
        }

        // 9. Orders by customer and type
        public async Task<MainResponse<OrdermeDTO>> GetOrdersByCustomerId(int customerId, int orderType)
        {
            var response = new MainResponse<OrdermeDTO>();
            try
            {
                var orders = await _unitOfWork.Ordermes.GetBy(o => o.customerId == customerId && Convert.ToInt32(o.orderType) == orderType);

                if (orders == null || !orders.Any())
                {
                    response.errors = new List<string> { "No orders found for this customer with the specified order type." };
                    return response;
                }

                foreach (var order in orders)
                {
                    var details = await _unitOfWork.Ordermedetails.Find(x => x.OrdermeId == order.Id);
                    order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();
                }

                var dtoOrders = _mapper.Map<List<OrdermeDTO>>(orders);
                response.acceptedObjects = dtoOrders;
                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { "Internal server error. " + ex.Message };
                return response;
            }
        }

        // 10. Top 10 products by sales
        public async Task<MainResponse<ProductSalesDTO>> GetTop10ProductsByOrderType1(int ordertype, DateTime startDate, DateTime endDate)
        {
            var response = new MainResponse<ProductSalesDTO>();

            try
            {
                endDate = endDate.Date.AddDays(1).AddTicks(-1);

                var orders = await _unitOfWork.Ordermes.GetBy(o =>
                    Convert.ToInt32(o.orderType) == ordertype &&
                    o.DateTime >= startDate &&
                    o.DateTime <= endDate
                );

                if (orders == null || !orders.Any())
                {
                    response.errors = new List<string> { "No orders found with the given criteria." };
                    return response;
                }

                var productQuantities = new Dictionary<int, int>();

                foreach (var order in orders)
                {
                    var details = await _unitOfWork.Ordermedetails.Find(d => d.OrdermeId == order.Id);
                    foreach (var detail in details)
                    {
                        if (!productQuantities.ContainsKey(detail.ProductId))
                            productQuantities[detail.ProductId] = 0;
                        productQuantities[detail.ProductId] += detail.qty;
                    }
                }

                if (!productQuantities.Any())
                {
                    response.errors = new List<string> { "No product sales found within the specified date range." };
                    return response;
                }

                var top10ProductIds = productQuantities
                    .OrderByDescending(kv => kv.Value)
                    .Take(10)
                    .Select(kv => kv.Key)
                    .ToList();

                var top10Products = await _unitOfWork.Products.Find(p => top10ProductIds.Contains(p.Id));

                if (top10Products == null || !top10Products.Any())
                {
                    response.errors = new List<string> { "Top products not found in product catalog." };
                    return response;
                }

                var productSalesDtos = top10Products.Select(p => new ProductSalesDTO
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    QtySold = productQuantities.TryGetValue(p.Id, out int qty) ? qty : 0
                }).ToList();

                response.acceptedObjects = productSalesDtos;
                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { "Internal server error. " + ex.Message };
                return response;
            }
        }

        // 11. Orders by category
        public async Task<MainResponse<OrdermeDTO>> GetOrdersByCategoryId(int categoryId)
        {
            var response = new MainResponse<OrdermeDTO>();

            try
            {
                var productsInCategory = await _unitOfWork.Products.Find(p => p.CategoryId == categoryId);

                if (productsInCategory == null || !productsInCategory.Any())
                {
                    response.errors = new List<string> { "No products found for this category." };
                    return response;
                }

                var productIds = productsInCategory.Select(p => p.Id).ToList();
                var orderDetails = await _unitOfWork.Ordermedetails.Find(d => productIds.Contains(d.ProductId));

                if (orderDetails == null || !orderDetails.Any())
                {
                    response.errors = new List<string> { "No order details found for the products in this category." };
                    return response;
                }

                var orderIds = orderDetails.Select(d => d.OrdermeId).Distinct().ToList();
                var orders = await _unitOfWork.Ordermes.Find(o => orderIds.Contains(o.Id));

                if (orders == null || !orders.Any())
                {
                    response.errors = new List<string> { "No orders found for the products in this category." };
                    return response;
                }

                foreach (var order in orders)
                {
                    var details = orderDetails.Where(d => d.OrdermeId == order.Id).ToList();
                    order.Ordermedetails = details;
                }

                var orderDtos = _mapper.Map<List<OrdermeDTO>>(orders);
                response.acceptedObjects = orderDtos;
                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { "Internal server error. " + ex.Message };
                return response;
            }
        }

        // 12. Get stagnant products (not sold for N days)
        public async Task<List<StagnantProductDTO>> GetStagnantProducts(int daysWithoutSale = 30, int? stockId = null, int? categoryId = null)
        {
            var cutoff = DateTime.UtcNow.Date.AddDays(-daysWithoutSale);

            var allProducts = (await _unitOfWork.Products.GetAll()).ToList();
            if (categoryId.HasValue && categoryId.Value > 0)
                allProducts = allProducts.Where(p => p.CategoryId == categoryId.Value).ToList();

            if (!allProducts.Any())
                return new List<StagnantProductDTO>();

            var productIds = allProducts.Select(p => p.Id).ToHashSet();
            var salesOrders = await _unitOfWork.Ordermes.GetBy(o => Convert.ToInt32(o.orderType) == 1);

            if (salesOrders == null || !salesOrders.Any())
            {
                return allProducts.Select(p => new StagnantProductDTO
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    LastSoldAt = null,
                    DaysSinceLastSale = int.MaxValue / 365,
                    TotalQtySoldEver = 0
                }).ToList();
            }

            var salesOrderIds = salesOrders.Select(o => o.Id).ToList();
            var salesOrderById = salesOrders.ToDictionary(o => o.Id, o => o);
            var allSalesDetails = await _unitOfWork.Ordermedetails.Find(d => salesOrderIds.Contains(d.OrdermeId));

            if (stockId.HasValue && stockId.Value > 0)
            {
                var detailsWithStock = allSalesDetails.Where(d =>
                {
                    var detailStockProp = d.GetType().GetProperty("StockId");
                    int? detailStock = detailStockProp != null ? (int?)detailStockProp.GetValue(d) : null;
                    if (detailStock.HasValue && detailStock.Value != 0)
                        return detailStock.Value == stockId.Value;
                    var order = salesOrderById.GetValueOrDefault(d.OrdermeId);
                    return order != null && order.StockId == stockId.Value;
                }).ToList();
                allSalesDetails = detailsWithStock;
            }

            var lastSoldPerProduct = new Dictionary<int, DateTime>();
            var totalQtyPerProduct = new Dictionary<int, int>();

            foreach (var d in allSalesDetails)
            {
                if (!productIds.Contains(d.ProductId)) continue;
                var ord = salesOrderById.GetValueOrDefault(d.OrdermeId);
                if (ord == null) continue;
                var soldAt = ord.DateTime;
                if (!lastSoldPerProduct.ContainsKey(d.ProductId) || lastSoldPerProduct[d.ProductId] < soldAt)
                    lastSoldPerProduct[d.ProductId] = soldAt;
                if (!totalQtyPerProduct.ContainsKey(d.ProductId))
                    totalQtyPerProduct[d.ProductId] = 0;
                totalQtyPerProduct[d.ProductId] += d.qty;
            }

            var stagnant = new List<StagnantProductDTO>();
            foreach (var p in allProducts)
            {
                DateTime? lastSold = lastSoldPerProduct.ContainsKey(p.Id) ? lastSoldPerProduct[p.Id] : (DateTime?)null;
                if (lastSold == null || lastSold.Value < cutoff)
                {
                    var daysSince = lastSold.HasValue
                        ? (int)(DateTime.UtcNow.Date - lastSold.Value.Date).TotalDays
                        : int.MaxValue / 365;

                    stagnant.Add(new StagnantProductDTO
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        LastSoldAt = lastSold,
                        DaysSinceLastSale = daysSince,
                        TotalQtySoldEver = totalQtyPerProduct.GetValueOrDefault(p.Id, 0)
                    });
                }
            }

            stagnant = stagnant
                .OrderByDescending(x => x.LastSoldAt.HasValue)
                .ThenBy(x => x.LastSoldAt ?? DateTime.MinValue)
                .ToList();

            return stagnant;
        }

        //13. Get orders with orderType
        public async Task<MainResponse<Orderme>> GetOrdersByOrderTypeWithDetails(int orderType)
        {
            var response = new MainResponse<Orderme>();

            // Get all orders with this orderType
            var orders = await _unitOfWork.Ordermes.GetAll(o =>
        o.orderType != null && o.orderType.ToString() == orderType.ToString());



            if (orders == null || !orders.Any())
            {
                response.errors = new List<string> { "No orders found with the given order type." };
                return response;
            }

            var orderList = orders.ToList();

            // Load details for each order (optional: use parallelism for speed if a lot of orders)
            foreach (var order in orderList)
            {
                var details = await _unitOfWork.Ordermedetails.Find(d => d.OrdermeId == order.Id);
                order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();
            }

            response.acceptedObjects = orderList;
            return response;
        }

        //14. Add order in POS 
        public async Task<MainResponse<Orderme>> CreatePOSOrderAsync(POSOrderRequest request)
        {
            // prepare an OrderCreateDTO in the same structure your AddOrdermes() expects
            var dto = new OrderCreateDTO
            {
                InternalId = "POS created Order",
                UserId = 1,
                StockId = 1,
                CashAndBankId = request.CashandBankId,
                customerId = request.CustomerId,
                CurrencyId = request.CurrencyId,
                OrderType = 0,              // 0 = sales
                wayofpayemnt = paymentmesod.C,         // Cash
                Notes1 = "POS generated order",
                OrdermeDetails = new List<OrdermedetailDTO>()

            };

            foreach (var p in request.Products)
            {
                var product = await _unitOfWork.Products.GetById(p.ProductId);
                if (product == null)
                    continue;

                dto.OrdermeDetails.Add(new OrdermedetailDTO
                {
                    ProductId = p.ProductId,
                    qty = p.Qty,
                    price = product.sellingprice1 ?? 0m,
                    discount = product.discount ?? 0m,
                    tax = product.Tax ?? 0m,
                    unitcode = product.UnitCode,
                    categoryId = product.CategoryId
                });
            }

            var result = await AddOrdermes(new List<OrderCreateDTO> { dto });
            return result;
        }

        // 15. Get orders by TreasuryCode
        public async Task<MainResponse<Orderme>> GetOrdersByTreasuryCode(string treasuryCode)
        {
            var response = new MainResponse<Orderme>();

            var orders = await _unitOfWork.Ordermes.GetBy(o => o.treasuryAcc == treasuryCode);

            if (orders == null || !orders.Any())
            {
                response.errors = new List<string> { $"No orders found with TreasuryCode: {treasuryCode}" };
                return response;
            }

            foreach (var order in orders)
            {
                var details = await _unitOfWork.Ordermedetails.Find(d => d.OrdermeId == order.Id);
                order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();
            }

            response.acceptedObjects = orders.ToList();
            return response;
        }

        // 16. Get orders by baseordertype
        public async Task<MainResponse<Orderme>> GetOrderByBaseOrderType(int baseOrderTypeId, int ord)
        {
            var response = new MainResponse<Orderme>();

            // Check if the baseOrderTypeId is a valid enum value
            if ((!Enum.IsDefined(typeof(BaseOrderType), baseOrderTypeId)) && (!Enum.IsDefined(typeof(OrderType), ord)))
            {
                response.errors = new List<string> { $"Invalid BaseOrderType or ordertype ID: {baseOrderTypeId}" };
                return response;
            }


            // Convert baseOrderTypeId to the Enum type
            BaseOrderType baseOrderType = (BaseOrderType)baseOrderTypeId;

            // Use LINQ to get orders where baseOrderType is the valid enum value
            var allOrders = await _unitOfWork.Ordermes.GetAll(x => x.baseordertype == baseOrderType);

            if (allOrders == null || !allOrders.Any())
            {
                response.errors = new List<string> { $"No orders found with BaseOrderType {baseOrderType}." };
                return response;
            }

            // Load order details
            foreach (var order in allOrders)
            {
                var details = await _unitOfWork.Ordermedetails.Find(d => d.OrdermeId == order.Id);
                order.Ordermedetails = details?.ToList() ?? new List<Ordermedetail>();
            }

            // Return the orders with their details
            response.acceptedObjects = allOrders.ToList();
            return response;
        }




    }










}
