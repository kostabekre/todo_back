namespace TodoistApi.Models;

public class Project
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public TaskUser User { get; set; } = null!;
    public ICollection<ProjectTask> Tasks { get; set; } = null!;
}
