using Microsoft.EntityFrameworkCore;
using PlatformService.SyncDataServices.Http;
using PLatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Configure the HTTP request pipeline.

if(builder.Environment.IsDevelopment())
{
    Console.WriteLine("using in mem db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}
else
{
    Console.WriteLine("using sql server db");
        builder.Services.AddDbContext<AppDbContext>
        (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn")));
}

    builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ICommandDataClient, HTTPCommandDataClient>();

var app = builder.Build();

Console.WriteLine($"Command Service Endpoint {builder.Configuration["CommandService"]}");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    PrepDb.PrepPopulation(app, builder.Environment.IsProduction());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
