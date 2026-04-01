using AutoMapper;
using MyErp.Core.DTO;
using MyErp.Core.Models;
namespace MyErp.Core.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {

            
            CreateMap<Ticket, TicketDTO>().ReverseMap();
            CreateMap<Ticket, TickectinvioceDTO>().ReverseMap();
            CreateMap<Contract, ContractDTO>().ReverseMap();
            CreateMap<UserSession, UserSessionDTO>().ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<FAQ, FAQDTO>().ReverseMap();          
            CreateMap<ToDo, ToDoDTO>().ReverseMap();
            CreateMap<ToDoDTO, ToDo>()
    .ForMember(d => d.ischecked, o => o.MapFrom(s => (IsChecked)s.ischecked));
            CreateMap<DocumentDTO, Document>()
                .ForMember(dest => dest.Attachment,
                    opt => opt.Ignore());

            CreateMap<Document, DocumentDTO>()
                .ForMember(dest => dest.Attachment,
                    opt => opt.Ignore()); CreateMap<ToDo, ToDoDTO>()
                .ForMember(d => d.ischecked, o => o.MapFrom(s => (int)s.ischecked));

            CreateMap<FAQDTO, FAQ>()
    .ForMember(dest => dest.Attachment, opt => opt.Ignore());
            CreateMap<CalenderTask, CalenderTaskDTO>().ReverseMap();
            //CreateMap<Orderme, OrderCreateDTO>().ReverseMap();
            CreateMap<Lead, LeadDTO>().ReverseMap();
            CreateMap<Email, EmailDTO>().ReverseMap();
            CreateMap<Goal , GoalDTO>().ReverseMap();
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
