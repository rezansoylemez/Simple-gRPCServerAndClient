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
    public override async Task<UserProtoBuf.HelloReply> AddUsers(IAsyncStreamReader<User> requestStream, ServerCallContext context)
    {
        var users = new List<User>();

        await foreach (var user in requestStream.ReadAllAsync())
        {
            users.Add(user);

            Console.WriteLine($"Received User: ID={user.Id}, Name={user.Name}, Email={user.Email}");

          var createdUser =   await _context.Users.AddAsync( new Models.User{
                Name = user.Name,
                Email = user.Email,
            });
            //_context.SaveChangesAsync(createdUser, new CancellationToken());
            // Diğer özellikleri de listeleyebilirsiniz
        }

        // Veritabanına kullanıcıları ekleyin veya başka işlemler yapın
        // ...

        var message =  new UserProtoBuf.HelloReply 
        { 
            Message = "Users added successfully" };
        
        return message ;

    }
   public override async Task Chat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync())
        {
            Console.WriteLine($"Received Message: {message.Text}");

            // Gelen mesajı istemciye geri gönderin
            await responseStream.WriteAsync(new ChatMessage { Text = $"Server: Received - {message.Text}" });
        }
    }

}
