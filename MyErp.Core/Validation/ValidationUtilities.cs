using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Services;
using System.Text.RegularExpressions;

namespace MyErp.Core.Validation
{
    public static class ValidationUtilities

    {
        //ERP
        //=========TotalCalc==================

        public static void CalculateOrderTotal(Orderme entity)
        {

            var Final = entity.TotalPrice;


            foreach (var d in entity.Ordermedetails)
            {
                d.total = d.qty * d.price;
                d.total = d.total + d.tax;
                d.total = d.total - d.discount;
                Final += d.total;
            }
            var FinDiscount = entity.totDiscount;
            var NetTotal = Final - FinDiscount;
            entity.TotalPrice = NetTotal;
        }


        //======Fill Some Things========================

        public static void FillSomeVariables(IUnitOfWork uow, Orderme order)
        {

            order.treasuryAcc = TreasuryServices.GetCode((int)order.orderType);
            var custname = uow.Customers.GetById(order.customerId);
            order.CustomerName = custname.Result.Name;
            order.CustomerCountry = custname.Result.Country;
            order.CustomerPhone = custname.Result.Country;


        }






        // ========= STOCK and CASH  VALIDATION =========

        public static async Task<List<string>> ValidateStockAsync(IUnitOfWork uow, IEnumerable<Orderme> orders)
        {
            var errors = new List<string>();
            if (orders == null)
                return errors;

            foreach (var order in orders)
            {

                var lines = order?.Ordermedetails;
                if (lines == null || lines.Count == 0)
                    continue;

                // Order type → sign
                //isSalesOrSalesCredit || isPurchaseOrPurchsaceCredit
                int ot = Convert.ToInt32((int)order.orderType);
                bool isSales = (ot == 0 || ot == 2);
                bool isPurchase = (ot == 1 || ot == 5);
                bool isSalesReturn = (ot == 3);
                bool isPurchaseReturn = (ot == 4);

                int sign = (isSales || isPurchaseReturn) ? -1 : +1;

                foreach (var d in lines)
                {
                    // prefer per-line StockId if your model supports it; fallback to order.StockId
                    int stockId = TryGetDetailStockId(d) ?? order.StockId;

                    if (d.ProductId <= 0 || stockId <= 0)
                        continue;

                    var last = (await uow.StockActionDetailss
                        .Find(x => x.ProductId == d.ProductId && x.StockId == stockId))
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

                    int prevFinal = last?.FinalValue ?? 0;

                    // if we're removing from stock, ensure enough exists
                    if (sign < 0 && prevFinal < d.qty)
                    {
                        errors.Add($"Cannot remove {d.qty} of ProductId {d.ProductId} from Stock {stockId}. Available: {prevFinal}. Order #{order.internalId}.");
                    }
                }
            }

            return errors;

            // local helper to read a nullable int property named "StockId" on the detail, if it exists
            static int? TryGetDetailStockId(Ordermedetail detail)
            {
                // if your detail has a StockId property, switch to: return detail.StockId;
                var pi = detail.GetType().GetProperty("StockId");
                if (pi == null) return null;
                var raw = pi.GetValue(detail);
                return raw as int? ?? (raw is int v ? v : (int?)null);
            }
        }
        public static async Task<List<string>> ValidateCashAsync(IUnitOfWork uow, IEnumerable<Orderme> orders)
        {
            var errors = new List<string>();
            if (orders == null)
                return errors;

            foreach (var order in orders)
            {
                // Parse enum safely
                if (!Enum.IsDefined(typeof(OrderType), order.orderType))
                    continue;

                var type = (OrderType)order.orderType;
                var basetype = (BaseOrderType)order.baseordertype;
                // These order types require cash OUT validation (must have enough in account)
                bool requiresOutflowCheck = type == OrderType.Purchase
                                         || type == OrderType.ReturnSale
                                         || type == OrderType.CreditPurchase
                                         || basetype == BaseOrderType.orderpurhcase;

                if (!requiresOutflowCheck)
                    continue;

                var account = await uow.CashAndBankss.GetById(order.CashAndBankId);


                // For CreditPurchase: use creditpayed (actual payment now). For others: use TotalPrice.
                decimal amountToCheck =
                    type == OrderType.CreditPurchase
                    ? (decimal)order.CreditPayment
                    : order.TotalPrice;

                if (account.CurrentBalance < amountToCheck)
                {
                    errors.Add($"Not enough balance in {account.Name}. Needed: {amountToCheck}, available: {account.CurrentBalance}.");
                    return errors;
                }
            }

            return errors;
        }

        //========= Finished Validation ================







        //END ERP
        //-----public usage validation methods-----//
        public static bool CheckIsPositiveValue(this decimal? value, bool isQty)
        {
            if (value < 0 && isQty == true)
            {
                return false;
            }
            else if (value < 0 && isQty == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool IsNotNull<T>(this T value)
        {
            return value != null;
        }
        public static bool ValidateId(this string? id)

        {

            if (id != null & id != "0" & id != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ValidateNoSpecialChars(this string? input)
        {
            if (input == null || input == "")
                return false;

            // Allow only letters, numbers, and spaces
            foreach (char c in input)
            {
                if (!char.IsLetterOrDigit(c) && c != ' ')
                    return false;
            }

            return true;
        }

        public static bool ValidateDescription(this string? desc)
        {

            if (desc != null)
            {

                if (desc == "")
                {
                    return false;
                }
                else
                    return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ContainEG(this string? value)
        {
            return Regex.IsMatch(value, "EG");
        }
        public static bool IselevenDigits(this string? value)
        {
            return Regex.IsMatch(value, @"^\d{11}$");
        }
        public static bool IsNineDigits(this string? value)
        {

            return Regex.IsMatch(value, @"^\d{9}$");
        }
        public static bool isthreetindigit(this string? value)
        {
            return Regex.IsMatch(value, @"\d{13}$");
        }
        public static bool IsfourteenDigits(this string? value)
        {

            return Regex.IsMatch(value, @"^\d{14}$");
        }
        public static bool IsfifteenDigits(this string? value)
        {

            return Regex.IsMatch(value, @"^\d{15}$");
        }
        public static bool IstenDigits(this string? value)
        {

            return Regex.IsMatch(value, @"^\d{10}$");
        }
        public static bool CheckEnum<T>(this string? enumValue) where T : Enum
        {
            return Enum.TryParse(typeof(T), enumValue, true, out _);
        }
        public static InvoiceType ParseInvoiceTypeStatus(string type)
        {
            return Enum.TryParse<InvoiceType>(type, true, out var parsedStatus) ? parsedStatus : InvoiceType.Invoice;
        }
        public static OrderType ParseOrderTypeStatus(string type)
        {
            return Enum.TryParse<OrderType>(type, true, out var parsedStatus) ? parsedStatus : OrderType.Sale;
        }
        public static CustomerType ParseCustomerType(string type)
        {
            return Enum.TryParse<CustomerType>(type, true, out var parsedStatus) ? parsedStatus : CustomerType.Person;
        }
        public static InventoryPermissionType ParseInventoryPermissionType(string type)
        {
            return Enum.TryParse<InventoryPermissionType>(type, true, out var parsedStatus) ? parsedStatus : InventoryPermissionType.ProductWithdrawal;
        }
        public static EntityType ParseEntityType(string type)
        {
            return Enum.TryParse<EntityType>(type, true, out var parsedStatus) ? parsedStatus : EntityType.Customer;
        }
        public static ControlTypes ParseControlType(string type)
        {
            return Enum.TryParse<ControlTypes>(type, true, out var parsedStatus) ? parsedStatus : ControlTypes.Form;
        }
        public static decimal ParseDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? result : 0m; // Default to 0 if parsing fails
        }
        public static decimal ParseInt(string value)
        {
            return int.TryParse(value, out var result) ? result : 0; // Default to 0 if parsing fails
        }

        ////////
        //KSA
        public static bool CheckCustomerSchemaID(string custSchemaID)
        {
            if (Enum.TryParse<SchemeID>(custSchemaID, true, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckCustomerSchemaValue(string custSchemaID, string custSchemaValue)
        {
            if (custSchemaID == "TIN")
            {
                if (custSchemaValue.IstenDigits())
                {
                    return true;
                }
                else
                    return false;
            }
            if (custSchemaID == "CRN")
            {
                if (custSchemaValue.IstenDigits())
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        public static bool CheckCustomerRegiserationNumberSA(string vatNo)
        {
            return Regex.IsMatch(vatNo, @"^3\d{13}3$");
        }
        public static bool IsDigitsOrPlusOnly(this string? value)
        {
            // Allows: + and digits only
            return !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value, @"^\+?\d+$");
        }


        public static bool CheckVATCategoryCode(string vatCategoryCode)
        {
            if (Enum.TryParse<VatCategoryCode>(vatCategoryCode, true, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //END of public usage validation methods-----//

    }
}
