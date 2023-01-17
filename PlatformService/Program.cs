using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Configure the HTTP request pipeline.

if (builder.Environment.IsDevelopment() == true)
{
    Console.WriteLine("using in mem db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}
else
{
    Console.WriteLine("using sql server db");
    builder.Services.AddDbContext<AppDbContext>
      (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ICommandDataClient, HTTPCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

var app = builder.Build();

Console.WriteLine($"Command Service Endpoint {builder.Configuration["CommandService"]}");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    PrepDb.PrepPopulation(app, builder.Environment.IsDevelopment());
}
else
{
    PrepDb.PrepPopulation(app, builder.Environment.IsProduction());
}

app.UseHttpsRedirection();


app.MapControllers();

app.UseRouting();

app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGrpcService<GrpcPlatformService>();

    endpoints.MapGet("/Protos/platforms.proto", async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
    });

});



app.Run();
