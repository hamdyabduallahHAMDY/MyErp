using AutoMapper;
using MyErp.Core.DTO;
using MyErp.Core.Models;
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

            //CreateMap<Area, AreaDTO>().ReverseMap();
            //CreateMap<Branch, BranchDTO>().ReverseMap();
            //CreateMap<CashAndBanks, CashAndBanksDTO>().ReverseMap();
            //CreateMap<CashFlow, CashFlowDTO>().ReverseMap();
            //CreateMap<Category, CategoryDTO>().ReverseMap();
            //CreateMap<Currency, CurrencyDTO>().ReverseMap();
            //CreateMap<Customer, CustomerDTO>().ReverseMap();
            //CreateMap<Employee, EmployeeDTO>().ReverseMap();
            //CreateMap<Orderme, OrdermeDTO>().ReverseMap();
            //CreateMap<Ordermedetail, OrdermedetailDTO>().ReverseMap();
            //CreateMap<Product, ProductDTO>().ReverseMap();
            //CreateMap<ProductType, ProductTypeDTO>().ReverseMap();
            //CreateMap<SalesMan, SalesManDTO>().ReverseMap();
            //CreateMap<Stock, StockDTO>().ReverseMap();
            //CreateMap<StockActionDetails, StockActionDetailsDTO>().ReverseMap();
            //CreateMap<StockActions, StockActionsDTO>().ReverseMap();
            //CreateMap<StockActiontransfer, StockActiontransferDTO>().ReverseMap();
            //CreateMap<StockReq, StockReqDTO>().ReverseMap();
            //CreateMap<StockReqDetail, StockReqDetailDTO>().ReverseMap();
            //CreateMap<StockTaking, StockTakingDTO>().ReverseMap();
            //CreateMap<Treasury, TreasuryDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Ticket, TicketDTO>().ReverseMap();
            CreateMap<Contract, ContractDTO>().ReverseMap();
            CreateMap<UserSession, UserSessionDTO>().ReverseMap();
            //CreateMap<Orderme, OrderCreateDTO>().ReverseMap();
        }
    }


    public class Mapping<T, X> : Profile
    {
        public Mapping()
        {
            CreateMap<T, X>();
            /*CreateMap<T, X>()
                .ForMember(dest => dest.Orderdetails, opt => opt.MapFrom(src => src.Orderdetails));

            CreateMap<OrderDetails, orderDetailsDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.ToString())); // Assuming ProductId should be a string in DTO*/
        }
        ///


    }
}
