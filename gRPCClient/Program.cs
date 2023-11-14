using System.Data.Common;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using gRPCServer; 
using UserProtoBuf;
//using UserProtoBuf;

Console.WriteLine("Hello, World!");

Console.WriteLine("Server Streaming");
var channel = GrpcChannel.ForAddress("http://localhost:5520"); 
//Bu channel dan baglantı olusturmak için
var userClient = new UserProto.UserProtoClient(channel);

//var greetClient = new Greeter.GreeterClient(channel);



//greetClient.SayHello(request : new HelloRequest { Name = "Name Test"} );


//userClient.GetUser(request : new GetUserRequest { Id = 1 });

var stream = userClient.ListUsers(new ListUsersRequest{});

CancellationTokenSource cancellationToken = new CancellationTokenSource();

 while (await stream
 .ResponseStream
 .MoveNext(cancellationToken
 .Token)){System.Console.WriteLine(stream.ResponseStream.Current.Name);}


Console.WriteLine("Client Streaming");
 using (var call = userClient.AddUsers())
{
      // Kullanıcıları stream üzerinden gönderin
    await call.RequestStream.WriteAsync(new User {
            Name = "John", Email = "john@example.com" });
    await call.RequestStream.WriteAsync(new User {Name = "Jane", Email = "jane@example.com" });
            // Diğer kullanıcıları ekleyin

    await call.RequestStream.CompleteAsync();

    var response = await call.ResponseAsync;
    Console.WriteLine($"Server Response: {response.Message}");
}
 
Console.WriteLine("Bi-Dic Streaming");
using (var call = userClient.Chat())
{
   var requestStream = call.RequestStream;
            var responseStream = call.ResponseStream;

            var sendTask = Task.Run(async () =>
            {
                await requestStream.WriteAsync(new ChatMessage { Text = "Hello from client" });
                await requestStream.WriteAsync(new ChatMessage { Text = "How are you?" });
                await requestStream.CompleteAsync();
            });

            var receiveTask = Task.Run(async () =>
            {
                while (await responseStream.MoveNext(cancellationToken.Token))
                {
                    var message = responseStream.Current;
                    Console.WriteLine($"Received Message: {message.Text}");
                }
            });

            await Task.WhenAll(sendTask, receiveTask);
}