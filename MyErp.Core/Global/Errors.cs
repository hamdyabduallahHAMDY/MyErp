using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Global
{
    public class Errors<T>
    {
        public string ObjectErrorInPhoneNum(int Internalid, string id)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid PhoneNumber: ({id}) !!";
        }
        public string ObjectNotFound()
        {
            return $"This {typeof(T).Name} Not Found In Database !!";
        }
        public string ObjectNotFoundWithId(int id)
        {
            return $"This {typeof(T).Name} with id = <{id}> Not Found In Database !!";
        }
        public string ObjectNotFoundString(string id)
        {
            return $"This {typeof(T).Name} with id = <{id}> Not Found In Database !!";
        }
        public string ObjectErrorInName(string name, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Name: ({name}) !!";
        }
        public string ObjectErrorInDiscount(decimal discount, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid discount: ({discount}) !!";
        }
        public string ObjectErrorInAddress(int id, string name, string Address)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{id}> : Has Invalid Name: ({name}) !!";
        }
        public string ObjectErrorInInternalid(string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Internal ID ({Internalid}) !!";
        }
        public string ObjectErrorInvExist(string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Exsist Before In Database !!";
        }
        public string ObjectErrorInTaxType(string taxType, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Customer Type: ({taxType}) !!, Customer Type must be B, P, or F. ";
        }
        public string ObjectErrorInTaxNumber(string taxNumber, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Tax Registration Number : ({taxNumber}) !!, Tax Registration must be 9 digits for Businesses.";
        }
        public string ObjectErrorInNationalId(string taxNumber, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid National Id : ({taxNumber}) !!, National Id must be 14 digits for Persons.";
        }
        public string ObjectErrorInCountryCode(string countryCode, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Country Code: ({countryCode}) !!, Country Code must be ISO ex.EG";
        }
        public string ObjectErrorInUnitType(string unitType, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Unit Code: ({unitType})";
        }
        public string ObjectErrorInStockTransfer(string internalid, int stockidfrom, int stockidto)
        {
            return $"this transfer with internal id  {typeof(T).Name}  <{internalid}>   have the same stock destination  {stockidfrom} , {stockidto}";
        }
        public string ObjectErrorNigativeVlue(decimal num, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Nigative Value: ({num})";
        }
        public string ObjectErrorEnumValue(string enumVal, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Enum Value: ({enumVal})";
        }
        ////
        ///KSA
        public string ObjectErrorInCustomerSchemaID(string custSchemaID, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Customer Schema ID: ({custSchemaID}) !!, Customer Schema ID must be TIN, CRN, MOM, MLS, NAT, GCC,IQA ,PAS ,OTH. ";
        }
        public string ObjectErrorInCustomerSchemaValue(string taxNumber, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has Invalid Customer SchemaValue : ({taxNumber}) !!.";
        }

        public string ObjectIsEmpty(string fieldName, string internalId)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{internalId}> : Has an Empty Field : ({fieldName}) !!.";
        }

        public string ObjectHasWrongValue(string fieldName, string value, string Internalid)
        {
            return $"This {typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has a Wrong Value : ({value}) For Field ({fieldName}) !!.";
        }

        public string ObjectCustomError(string message, string Internalid)
        {
            return $"{typeof(T).Name.Replace("DTO", "")} with Internal Id =<{Internalid}> : Has an Error : ({message}) !!.";
        }

        internal string ObjectErrorInName(string name, int id)
        {
            throw new NotImplementedException();
        }

        ///


    }

}