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
        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var toDoItem = await _toDoContext.ToDoItems.ToListAsync();
            foreach(var toDo in  toDoItem)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = toDo.Id,
                    Title=toDo.Title,
                    Description=toDo.Description,
                    ToDoStatus=toDo.ToDoStatus
                });
            }
            return await Task.FromResult(response);
        }
        
        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if(request.Id<=0 ||request.Title==string.Empty || request.Description==string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Not Valid"));

            }

            var todoItems= await _toDoContext.ToDoItems.FirstOrDefaultAsync(t=>t.Id==request.Id);
            if(todoItems==null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No task with this id {request.Id}"));
            }
            todoItems.Title=request.Title;
            todoItems.Description=request.Description;
            todoItems.ToDoStatus=request.ToDoStatus;
            await _toDoContext.SaveChangesAsync();
            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = request.Id
            });
        }

    }
}
