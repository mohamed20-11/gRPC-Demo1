using Grpc.Core;
using Microsoft.EntityFrameworkCore;
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
        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request , ServerCallContext context)
        {
            if(request.Id<=0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must be greater than 0"));
            }

            var toDoItem = await _toDoContext.ToDoItems.FirstOrDefaultAsync(t=>t.Id ==request.Id);
            if(toDoItem != null)
            {
                return await Task.FromResult(new ReadToDoResponse
                {
                    Id=toDoItem.Id,
                    Title=toDoItem.Title,
                    Description=toDoItem.Description,
                    ToDoStatus= toDoItem.ToDoStatus
                });
            }
            throw new RpcException(new Status(StatusCode.NotFound, $"No task with this id {request.Id}"));
        }
    }
}
