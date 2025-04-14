namespace TodoistApi.Requests;

public record UpdateProjectTaskRequest(int Id, string Name, bool IsDone, string? Description, DateTime? Until);
