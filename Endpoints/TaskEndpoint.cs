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
        app.MapDelete("/api/tasks/{id}", async (int id, TasksContext context,
            ClaimsPrincipal userPrincipal,
            UserManager<TaskUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(userPrincipal);

            if (user == null)
            {
                return Results.NotFound();
            }

            var deletedRows = await context.Tasks
                .Where(t => t.Id == id && t.Project.User.Id == user.Id)
                .ExecuteDeleteAsync();

            return deletedRows > 0 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteTask")
        .RequireAuthorization();

        app.MapPut("/api/tasks/{id}", async (int id, UpdateProjectTaskRequest request,
            TasksContext context,
            ClaimsPrincipal userPrincipal,
            UserManager<TaskUser> userManager) =>
        {
            if (id != request.Id)
            {
                return Results.BadRequest();
            }

            var user = await userManager.GetUserAsync(userPrincipal);

            if (user == null)
            {
                return Results.NotFound();
            }

            var task = await context.Tasks
                .Where(t => t.Id == request.Id && t.Project.User.Id == user.Id)
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return Results.NotFound();
            }

            if (request.Name != null)
            {
                task.Name = request.Name;
            }

            if (request.IsDone != null)
            {
                task.IsDone = (bool)request.IsDone;
            }

            if (request.Description != null)
            {
                task.Description = request.Description;
            }

            if (request.Until != null)
            {
                task.Until =  ((DateTime)request.Until).ToUniversalTime();
            }

            task.Updated = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Results.Ok();

        })
        .WithName("UpdateTask")
        .RequireAuthorization();


        app.MapPost("/api/tasks", async (CreateProjectTaskRequest request,
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
                Until = request.Until == null ? null : ((DateTime)request.Until).ToUniversalTime(),
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
            UserManager<TaskUser> userManager, int page = 1, int limit = 10) =>
        {
            if (page < 1 || limit < 1)
            {
                return Results.BadRequest("Page arguments are wrong");
            }

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

            var tasks = defaultProject.Tasks
                .OrderBy(t => t.Created)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(t => ProjectTaskMapper.ConvertToResponse(t));

            return Results.Ok(tasks);
        })
        .WithName("GetTasksOfDefaultProject")
        .RequireAuthorization();
    }
}
