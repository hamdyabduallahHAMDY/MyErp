using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
namespace MyErp.Core.Validation
{
    public static class ValidateDTO
    {
        public async static Task<MainResponse<AreaDTO>> AreaDTO(List<AreaDTO> insertDTO ,bool isupdate = false)
        {
            MainResponse<AreaDTO> response = new MainResponse<AreaDTO>();
            Errors<AreaDTO> err = new Errors<AreaDTO>();
            bool hasError = false;

            foreach (var area in insertDTO)
            {
                var DBarea = await ADO.GetExecuteQueryMySql<Models.Area>($"select * from Areas  where areaId = {area.AreaId}");

                if (DBarea.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(area.AreaId));
                    response.rejectedObjects.Add(area);
                    hasError = true;
                    continue;
                }
                if (!area.AreaId.ValidateDescription() || !area.AreaId.ValidateNoSpecialChars())
                {
                    response.errors.Add(err.ObjectErrorInInternalid(area.AreaId));
                    response.rejectedObjects.Add(area);
                    continue;
                }

                if (!area.AreaName.ValidateDescription() || !area.AreaId.ValidateNoSpecialChars())
                {
                    response.errors.Add(err.ObjectErrorInName(area.AreaName, area.AreaId));
                    response.rejectedObjects.Add(area);
                    continue;
                }
                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(area);
            }
            return response;

        }//finished
        public async static Task<MainResponse<BranchDTO>> BranchDTO(List<BranchDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<BranchDTO> response = new MainResponse<BranchDTO>();
            Errors<BranchDTO> err = new Errors<BranchDTO>();
            bool hasError = false;

            foreach (var area in insertDTO)
            {
                var DBarea = await ADO.GetExecuteQueryMySql<Models.Branch>($"select * from Branchs  where Code = {area.Code}");

                if (DBarea.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(area.Code));
                    response.rejectedObjects.Add(area);
                    hasError = true;
                    continue;
                }
                if (!area.Code.ValidateDescription() || !area.Code.ValidateNoSpecialChars())
                {
                    response.errors.Add(err.ObjectErrorInInternalid(area.Code));
                    response.rejectedObjects.Add(area);
                    continue;
                }

                if (!area.Name.ValidateDescription() || !area.Name.ValidateNoSpecialChars())
                {
                    response.errors.Add(err.ObjectErrorInName(area.Name, area.Code));
                    response.rejectedObjects.Add(area);
                    continue;
                }
                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(area);
            }
            return response;

        }//finished
        public async static Task<MainResponse<CashAndBanksDTO>> CashAndBanksDTO(List<CashAndBanksDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<CashAndBanksDTO> response = new MainResponse<CashAndBanksDTO>();
            Errors<CashAndBanksDTO> err = new Errors<CashAndBanksDTO>();
            bool hasError = false;
            foreach (var cashAndBanks in insertDTO)
            {
                var DBcashAndBanks = await ADO.GetExecuteQueryMySql<Models.CashAndBanks>($"select * from CashAndBanks  where Code = '{cashAndBanks.Code}'");
                if (DBcashAndBanks.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(cashAndBanks.Code));
                    response.rejectedObjects.Add(cashAndBanks);
                    hasError = true;
                    continue;
                }
                if (!cashAndBanks.Code.ValidateDescription() || !cashAndBanks.Code.ValidateNoSpecialChars())
                {
                    response.errors.Add(err.ObjectErrorInInternalid(cashAndBanks.Code));
                    response.rejectedObjects.Add(cashAndBanks);
                    continue;
                }
                if (!cashAndBanks.Name.ValidateDescription() || !cashAndBanks.Name.ValidateNoSpecialChars())
                {
                    response.errors.Add(err.ObjectErrorInName(cashAndBanks.Name, cashAndBanks.Code));
                    response.rejectedObjects.Add(cashAndBanks);
                    continue;
                }
                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(cashAndBanks);
            }
            return response;
        }

        public async static Task<MainResponse<CategoryDTO>> CategoryDTO(List<CategoryDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<CategoryDTO> response = new MainResponse<CategoryDTO>();
            Errors<CategoryDTO> err = new Errors<CategoryDTO>();
            bool hasError = false;
            foreach (var category in insertDTO)
            {
                var DBcategory = await ADO.GetExecuteQueryMySql<Models.Category>($"select * from Categorys  where Code = '{category.Internal_Id}'");
                if (DBcategory.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(category.Internal_Id));
                    response.rejectedObjects.Add(category);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(category);
            }
            return response;
        }

        public async static Task<MainResponse<CashFlowDTO>> CashFlowDTO(List<CashFlowDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<CashFlowDTO> response = new MainResponse<CashFlowDTO>();
            Errors<CashFlowDTO> err = new Errors<CashFlowDTO>();
            bool hasError = false;
            foreach (var cashFlow in insertDTO)
            {
                var DBcashFlow = await ADO.GetExecuteQueryMySql<Models.CashFlow>($"select * from CashFlows  where Id = {cashFlow.CashAndBankId}");
                if (DBcashFlow.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(cashFlow.CashAndBankId.ToString()));
                    response.rejectedObjects.Add(cashFlow);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(cashFlow);
            }
            return response;
        }

        public async static Task<MainResponse<CurrencyDTO>> CurrencyDTO(List<CurrencyDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<CurrencyDTO> response = new MainResponse<CurrencyDTO>();
            Errors<CurrencyDTO> err = new Errors<CurrencyDTO>();
            bool hasError = false;
            foreach (var currency in insertDTO)
            {
                var DBcurrency = await ADO.GetExecuteQueryMySql<Models.Currency>($"select * from Currencies  where InternalId = '{currency.InternalId}'");
                if (DBcurrency.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(currency.InternalId));
                    response.rejectedObjects.Add(currency);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(currency);
            }
            return response;
        }

        public async static Task<MainResponse<CustomerDTO>> CustomerDTO(List<CustomerDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<CustomerDTO> response = new MainResponse<CustomerDTO>();
            Errors<CustomerDTO> err = new Errors<CustomerDTO>();
            bool hasError = false;
            foreach (var customer in insertDTO)
            {
                var DBcustomer = await ADO.GetExecuteQueryMySql<Models.Customer>($"select * from customers  where InternalId = '{customer.InternalId}'");
                if (DBcustomer.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.InternalId));
                    response.rejectedObjects.Add(customer);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(customer);
            }
            return response;
        }

        public async static Task<MainResponse<EmployeeDTO>> EmployeeDTO(List<EmployeeDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<EmployeeDTO> response = new MainResponse<EmployeeDTO>();
            Errors<EmployeeDTO> err = new Errors<EmployeeDTO>();
            bool hasError = false;
            foreach (var employee in insertDTO)
            {
                var DBemployee = await ADO.GetExecuteQueryMySql<Models.Employee>($"select * from Employees  where InternalId = {employee.InternalId}");
                if (DBemployee.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(employee.InternalId.ToString()));
                    response.rejectedObjects.Add(employee);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(employee);
            }
            return response;
        }

        public async static Task<MainResponse<OrdermeDTO>> OrdermeDTO(List<OrdermeDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<OrdermeDTO> response = new MainResponse<OrdermeDTO>();
            Errors<OrdermeDTO> err = new Errors<OrdermeDTO>();
            bool hasError = false;
            foreach (var orderme in insertDTO)
            {
                var DBorderme = await ADO.GetExecuteQueryMySql<Models.Orderme>($"select * from Ordermes  where internalId = '{orderme.internalId}'");
                if (DBorderme.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(orderme.internalId));
                    response.rejectedObjects.Add(orderme);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(orderme);
            }
            return response;
        }

        public async static Task<MainResponse<OrdermedetailDTO>> OrdermedetailDTO(List<OrdermedetailDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<OrdermedetailDTO> response = new MainResponse<OrdermedetailDTO>();
            Errors<OrdermedetailDTO> err = new Errors<OrdermedetailDTO>();
            bool hasError = false;
            foreach (var ordermedetail in insertDTO)
            {
                var DBordermedetail = await ADO.GetExecuteQueryMySql<Models.Ordermedetail>($"select * from Ordermedetails  where internalId = '{ordermedetail.internalId}'");
                if (DBordermedetail.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(ordermedetail.internalId));
                    response.rejectedObjects.Add(ordermedetail);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(ordermedetail);
            }
            return response;
        }

        public async static Task<MainResponse<ProductDTO>> ProductDTO(List<ProductDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<ProductDTO> response = new MainResponse<ProductDTO>();
            Errors<ProductDTO> err = new Errors<ProductDTO>();
            bool hasError = false;
            foreach (var product in insertDTO)
            {
                var DBproduct = await ADO.GetExecuteQueryMySql<Models.Product>($"select * from Products  where InternalId = '{product.InternalId}'");
                if (DBproduct.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(product.InternalId));
                    response.rejectedObjects.Add(product);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(product);
            }
            return response;
        }

        public async static Task<MainResponse<ProductTypeDTO>> ProductTypeDTO(List<ProductTypeDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<ProductTypeDTO> response = new MainResponse<ProductTypeDTO>();
            Errors<ProductTypeDTO> err = new Errors<ProductTypeDTO>();
            bool hasError = false;
            foreach (var productType in insertDTO)
            {
                var DBproductType = await ADO.GetExecuteQueryMySql<Models.ProductType>($"select * from ProductTypes  where internalId = {productType.internalId}");
                if (DBproductType.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(productType.internalId.ToString()));
                    response.rejectedObjects.Add(productType);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(productType);
            }
            return response;
        }

        public async static Task<MainResponse<SalesManDTO>> SalesManDTO(List<SalesManDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<SalesManDTO> response = new MainResponse<SalesManDTO>();
            Errors<SalesManDTO> err = new Errors<SalesManDTO>();
            bool hasError = false;
            foreach (var salesMan in insertDTO)
            {
                var DBsalesMan = await ADO.GetExecuteQueryMySql<Models.SalesMan>($"select * from SalesMen  where salesmanId = '{salesMan.salesmanId}'");
                if (DBsalesMan.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(salesMan.salesmanId));
                    response.rejectedObjects.Add(salesMan);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(salesMan);
            }
            return response;
        }

        public async static Task<MainResponse<StockDTO>> StockDTO(List<StockDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<StockDTO> response = new MainResponse<StockDTO>();
            Errors<StockDTO> err = new Errors<StockDTO>();
            bool hasError = false;
            foreach (var stock in insertDTO)
            {
                var DBstock = await ADO.GetExecuteQueryMySql<Models.Stock>($"select * from Stocks  where Code = '{stock.Code}'");
                if (DBstock.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(stock.Code));
                    response.rejectedObjects.Add(stock);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(stock);
            }
            return response;
        }

        public async static Task<MainResponse<StockActionDetailsDTO>> StockActionDetailsDTO(List<StockActionDetailsDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<StockActionDetailsDTO> response = new MainResponse<StockActionDetailsDTO>();
            Errors<StockActionDetailsDTO> err = new Errors<StockActionDetailsDTO>();
            bool hasError = false;
            foreach (var stockActionDetails in insertDTO)
            {
                var DBstockActionDetails = await ADO.GetExecuteQueryMySql<Models.StockActionDetails>($"select * from StockActionDetails  where internalId = '{stockActionDetails.internalId}'");
                if (DBstockActionDetails.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(stockActionDetails.internalId));
                    response.rejectedObjects.Add(stockActionDetails);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(stockActionDetails);
            }
            return response;
        }

        public async static Task<MainResponse<StockActionsDTO>> StockActionsDTO(List<StockActionsDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<StockActionsDTO> response = new MainResponse<StockActionsDTO>();
            Errors<StockActionsDTO> err = new Errors<StockActionsDTO>();
            bool hasError = false;
            foreach (var stockActions in insertDTO)
            {
                var DBstockActions = await ADO.GetExecuteQueryMySql<Models.StockActions>($"select * from StockActions  where physicalinvNumber = '{stockActions.physicalinvNumber}'");
                if (DBstockActions.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(stockActions.physicalinvNumber));
                    response.rejectedObjects.Add(stockActions);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(stockActions);
            }
            return response;
        }

        public async static Task<MainResponse<StockActiontransferDTO>> StockActiontransferDTO(List<StockActiontransferDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<StockActiontransferDTO> response = new MainResponse<StockActiontransferDTO>();
            Errors<StockActiontransferDTO> err = new Errors<StockActiontransferDTO>();
            bool hasError = false;
            foreach (var stockActiontransfer in insertDTO)
            {
                var DBstockActiontransfer = await ADO.GetExecuteQueryMySql<Models.StockActiontransfer>($"select * from StockActiontransfers  where physicalinvNumber = '{stockActiontransfer.physicalinvNumber}'");
                if (DBstockActiontransfer.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(stockActiontransfer.physicalinvNumber));
                    response.rejectedObjects.Add(stockActiontransfer);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(stockActiontransfer);
            }
            return response;
        }

        public async static Task<MainResponse<StockReqDTO>> StockReqDTO(List<StockReqDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<StockReqDTO> response = new MainResponse<StockReqDTO>();
            Errors<StockReqDTO> err = new Errors<StockReqDTO>();
            bool hasError = false;
            foreach (var stockReq in insertDTO)
            {
                var DBstockReq = await ADO.GetExecuteQueryMySql<Models.StockReq>($"select * from StockReqs  where Id = 0");
                if (DBstockReq.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist("0"));
                    response.rejectedObjects.Add(stockReq);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(stockReq);
            }
            return response;
        }

        public async static Task<MainResponse<StockReqDetailDTO>> StockReqDetailDTO(List<StockReqDetailDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<StockReqDetailDTO> response = new MainResponse<StockReqDetailDTO>();
            Errors<StockReqDetailDTO> err = new Errors<StockReqDetailDTO>();
            bool hasError = false;
            foreach (var stockReqDetail in insertDTO)
            {
                var DBstockReqDetail = await ADO.GetExecuteQueryMySql<Models.StockReqDetail>($"select * from StockReqDetails  where Id = 0");
                if (DBstockReqDetail.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist("0"));
                    response.rejectedObjects.Add(stockReqDetail);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(stockReqDetail);
            }
            return response;
        }

        public async static Task<MainResponse<StockTakingDTO>> StockTakingDTO(List<StockTakingDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<StockTakingDTO> response = new MainResponse<StockTakingDTO>();
            Errors<StockTakingDTO> err = new Errors<StockTakingDTO>();
            bool hasError = false;
            foreach (var stockTaking in insertDTO)
            {
                var DBstockTaking = await ADO.GetExecuteQueryMySql<Models.StockTaking>($"select * from StockTakings  where code = '{stockTaking.code}'");
                if (DBstockTaking.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(stockTaking.code));
                    response.rejectedObjects.Add(stockTaking);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(stockTaking);
            }
            return response;
        }

        public async static Task<MainResponse<TreasuryDTO>> TreasuryDTO(List<TreasuryDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<TreasuryDTO> response = new MainResponse<TreasuryDTO>();
            Errors<TreasuryDTO> err = new Errors<TreasuryDTO>();
            bool hasError = false;
            foreach (var treasury in insertDTO)
            {
                var DBtreasury = await ADO.GetExecuteQueryMySql<Models.Treasury>($"select * from Treasuries  where Code = {treasury.Code}");
                if (DBtreasury.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(treasury.Code.ToString()));
                    response.rejectedObjects.Add(treasury);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(treasury);
            }
            return response;
        }

        public async static Task<MainResponse<UserDTO>> UserDTO(List<UserDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<UserDTO> response = new MainResponse<UserDTO>();
            Errors<UserDTO> err = new Errors<UserDTO>();
            bool hasError = false;
            foreach (var user in insertDTO)
            {
                var DBuser = await ADO.GetExecuteQueryMySql<Models.User>($"select * from Users  where Name = '{user.Name}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(user.Name));
                    response.rejectedObjects.Add(user);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(user);
            }
            return response;
        }
    }

}
