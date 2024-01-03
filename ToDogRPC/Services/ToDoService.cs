using Grpc.Core;
using ToDogRPC.Data;
using ToDogRPC.Models;
using ToDogRPC.Protos;

namespace ToDogRPC.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly ApplicationDbContext _toDoContext;

        public ToDoService(ApplicationDbContext toDoContext)
        {
            _toDoContext = toDoContext;
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if(request.Title ==String.Empty || request.Description ==String.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Supply a valid object"));
            }
            var toDoItem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description,

            };
            await _toDoContext.AddAsync(toDoItem);
            await _toDoContext.SaveChangesAsync();
            return await Task.FromResult(new CreateToDoResponse
            {
                Id=toDoItem.Id
            });
        }
    }
}
