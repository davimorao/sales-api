using MediatR;

namespace Sales.Application.Commands
{
    public class DeleteProductCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }

        public DeleteProductCommand(long id)
        {
            Id = id;
        }
    }
}
