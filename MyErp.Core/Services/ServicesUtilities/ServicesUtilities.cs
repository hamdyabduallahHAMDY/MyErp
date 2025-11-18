using MyErp.Core.Interfaces;

namespace MyErp.Core.Services.ServicesUtilities
{
    public class ServicesUtilities
    {
        private IUnitOfWork UnitOfWork { get; set; }
        public ServicesUtilities(IUnitOfWork unitOfWork)
        {

            UnitOfWork = unitOfWork;

        }

    }
}
