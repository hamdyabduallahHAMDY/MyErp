using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using MyErp.Core.DTO;
using MyErp.Core.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace MyErp.Core.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {

            //            /*Map User*/
            //            CreateMap<UserDTO, User>();
            //            /*Map User*/



            //            //stockaction DTOs
            //            CreateMap<StockActionDetails, StockActionDetailsDTO>().ReverseMap();
            //            CreateMap<StockActionDetails, StockActionsDetailsDTOSpecial>();

            //            CreateMap<CashFlow, CashFlowDTO>()
            //               .ForMember(dest => dest.CashAndBankName, opt => opt.MapFrom(src => src.c.Name));
            //            CreateMap<CashFlowCreateDTO, CashFlow>();


            //            CreateMap<CashAndBank, CashAndBankDTO>();
            //            CreateMap<CashAndBankDTO, CashAndBank>();

            //            CreateMap<CashAndBank, ChasnadbankCreateorUpdateDto>().ReverseMap();


            //            CreateMap<TreasuryCreateDTO, Treasury>().ReverseMap();

            //            // For update (API to Entity)
            //            CreateMap<TREASURYUPDATEDTO, Treasury>();

            //            // For entity to DTO (display/list)
            //            CreateMap<Treasury, TreasuryDTO>();
            //            // (Optional) For DTO to entity, if you need it
            //            CreateMap<TreasuryDTO, Treasury>();

            //            CreateMap<StockReq, StockReqDTO>().ReverseMap();
            //            CreateMap<StockReqDetail, StockReqDetailDTO>().ReverseMap();




            //            CreateMap<StockActionsDetailswithnumberingDTO, StockActionDetails>().ReverseMap();
            //            CreateMap<StockActiontransfer, StockActiontransferDTO>();
            //            CreateMap<StockActiontransferDTO, StockActiontransfer>();


            //            CreateMap<StockActionsDTO, StockActions>();
            //            CreateMap<StockActionsDetailsDTO, StockActionDetails>();



            //            // For normal order reading/viewing
            //            CreateMap<Orderme, OrdermeDTO>();
            //            CreateMap<OrdermeDTO, Orderme>();
            //            CreateMap<Ordermedetail, OrdermedetailDTO>();
            //            CreateMap<OrdermedetailDTO, Ordermedetail>();

            //            // For order creation (create DTO to entity)
            //            CreateMap<OrderCreateDTO, Orderme>();
            //            CreateMap<orderDetailsDTO, Ordermedetail>();
            //            CreateMap<Orderme, OrderCreateDTO>();




            //            CreateMap<orderDetailsDTO, OrderDetails>()
            //                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => ValidationUtilities.ParseInt(src.ProductId)))
            //                .ForMember(dest => dest.StockId, opt => opt.MapFrom(src => ValidationUtilities.ParseInt(src.StockId)));


            //            /*Map Branch*/
            //            CreateMap<BranchDTO, Branch>();
            //            /*Map Branch*/

            //            CreateMap<CustomerDTO, Customer>()
            //                .ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => ValidationUtilities.ParseCustomerType(src.CustomerType)))
            //                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => ValidationUtilities.ParseEntityType(src.EntityType)));

            //            CreateMap<ProductDTO, Product>().ForMember(d => d.image, o => o.Ignore());
            //            CreateMap<Product, ProductDTO>();

            //            CreateMap<StockDTO, Stock>().ReverseMap();

            //            CreateMap<CategoryDTO, Category>().ReverseMap();

            //            CreateMap<StockActions, StockActionsDTO>().ReverseMap();

            //            CreateMap<EmployeeDTO, Employee>().ReverseMap();

            //            CreateMap<StockTakingDTO, StockTaking>().ReverseMap();

            //            CreateMap<Currency, CurrencyDTO>().ReverseMap();

            //            CreateMap<Customer, CustomerDTO>();

            CreateMap<Area, AreaDTO>().ReverseMap();

            //            CreateMap<ProductType, ProductTypeDTO>().ReverseMap();

            //            CreateMap<SalesMan, SalesManDTO>().ReverseMap();

            //        }
            //    }


            //    public class Mapping<T, X> : Profile
            //    {
            //        public Mapping()
            //        {
            //            CreateMap<T, X>();
            //            /*CreateMap<T, X>()
            //                .ForMember(dest => dest.Orderdetails, opt => opt.MapFrom(src => src.Orderdetails));

            //            CreateMap<OrderDetails, orderDetailsDTO>()
            //                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.ToString())); // Assuming ProductId should be a string in DTO*/
            //        }
            //        ///


        }
    }
}