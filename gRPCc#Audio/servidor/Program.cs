using System;

class Program
{
    static void Main(string[] args)
    {
       var builder = WebApplication.CreateBuilder(args);
       builder.Services.AddGrpc();
       var app = builder.Build();
       app.MapGrpcService<AudioServicer>();
       app.Run();
    }
}
