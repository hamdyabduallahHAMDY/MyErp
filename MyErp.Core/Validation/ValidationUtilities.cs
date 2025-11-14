using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyErp.Core.Models;

namespace MyErp.Core.Validation
{
    public static class ValidationUtilities
    {
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
