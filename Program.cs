using Microsoft.EntityFrameworkCore;
using TodoistApi.Context;
using TodoistApi.Endpoints;
using TodoistApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TasksContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddIdentityApiEndpoints<TaskUser>()
    .AddEntityFrameworkStores<TasksContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGroup("/account").MapIdentityApi<TaskUser>();
app.UseAuthentication();
app.UseAuthorization();

app.AddProjectEndpoint();

app.AddTaskEndpoints();

app.Run();
