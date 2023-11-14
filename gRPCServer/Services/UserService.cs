using Grpc.Core; 
using Microsoft.EntityFrameworkCore;
using UserProtoBuf;



namespace gRPCServer.Services;

public class UserService : UserProto.UserProtoBase
{
    private readonly BaseDbContext _context;
    private readonly ILogger<GreeterService> _logger;
    public UserService(ILogger<GreeterService> logger , BaseDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task ListUsers(ListUsersRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
    {
        
        try
        { 
            var users = await _context.Users.ToListAsync();
            
            foreach (var user in users)
            {
             


                await responseStream.WriteAsync(new User
                { 
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            throw;  
        }  
    }
}

