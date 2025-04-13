using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoistApi.Models;

namespace TodoistApi.Context;

public class TasksContext : IdentityDbContext<TaskUser>
{
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }

    public TasksContext(DbContextOptions<TasksContext> options) : base(options)
    {
        
    }
}
