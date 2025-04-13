using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoistApi.Context;
using TodoistApi.Models;

namespace TodoistApi.Endpoints;

public static class ProjectEndpoint
{
    public static void AddProjectEndpoint(this WebApplication app)
    {
        app.MapPost("/api/project/create", (TasksContext context) =>
        {
            throw new NotImplementedException();
        })
        .WithName("CreateProject")
        .RequireAuthorization();

        app.MapGet("/api/project/{id}/get_tasks", async (int id,
            TasksContext context,
            ClaimsPrincipal userPrincipal,
            UserManager<TaskUser> userManager) =>
        {
            var userId = userManager.GetUserId(userPrincipal);

            if (userId == null)
            {
            return Results.NotFound();
            }

            var foundedProject = await context.Projects
            .Where(p => p.Id == id && p.User.Id == userId)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync();

            if (foundedProject == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(foundedProject.Tasks);
        })
        .WithName("GetTasks")
        .RequireAuthorization();
    }
}
