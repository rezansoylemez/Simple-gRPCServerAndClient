// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using gRPCServer; 
using UserProtoBuf;
//using UserProtoBuf;

Console.WriteLine("Hello, World!");


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