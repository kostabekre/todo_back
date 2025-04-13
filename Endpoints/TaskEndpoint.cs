using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoistApi.Context;
using TodoistApi.Models;
using TodoistApi.Requests;
using TodoistApi.Responses;

namespace TodoistApi.Endpoints;

public static class TaskEndpoint
{
    public static void AddTaskEndpoints(this WebApplication app)
    {
        app.MapPost("/api/tasks/create", async (CreateProjectTaskRequest request,
            TasksContext context,
            ClaimsPrincipal userPrincipal,
            UserManager<TaskUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(userPrincipal);

            if (user == null)
            {
                return Results.NotFound();
            }

            var defaultProject = await context.Projects
                .Where(p => p.User.Id == user.Id && p.Name == "_default")
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync();

            if (defaultProject == null)
            {
                defaultProject = new Project() { Name = "_default", User = user, Tasks = [], Id = 0 };
                context.Projects.Add(defaultProject);
            }

            var task = new ProjectTask() { 
                Id = 0,
                Name = request.Name,
                Description = request.Description,
                Project = defaultProject,
                Until = request.Until,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            defaultProject.Tasks.Add(task);

            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("CreateTask")
        .RequireAuthorization();

        app.MapGet("/api/tasks", async (TasksContext context,
            ClaimsPrincipal userPrincipal,
            UserManager<TaskUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(userPrincipal);

            if (user == null)
            {
                return Results.NotFound("User is not founded.");
            }

            var defaultProject = await context.Projects
                .Where(p => p.User.Id == user.Id && p.Name == "_default")
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync();

            if (defaultProject == null)
            {
                defaultProject = new Project() { Name = "_default", User = user, Tasks = [], Id = 0 };
                context.Projects.Add(defaultProject);
                await context.SaveChangesAsync();
            }

            return Results.Ok(defaultProject.Tasks
                    .Select(t => ProjectTaskMapper.ConvertToResponse(t)));
        })
        .WithName("GetTasksOfDefaultProject")
        .RequireAuthorization();
    }
}
