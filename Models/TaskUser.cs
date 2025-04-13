using Microsoft.AspNetCore.Identity;

namespace TodoistApi.Models;

public class TaskUser : IdentityUser
{
    public ICollection<Project> Projects { get; set; } = null!;
}
