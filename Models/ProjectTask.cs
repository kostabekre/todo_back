namespace TodoistApi.Models;

public class ProjectTask
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Updated { get; set; }
    public bool IsDone { get; set; }
    public DateTime? Until { get; set; }
    public Project Project { get; set; } = null!;
}
