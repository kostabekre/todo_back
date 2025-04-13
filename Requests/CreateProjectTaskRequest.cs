namespace TodoistApi.Requests;

public record CreateProjectTaskRequest(string Name, string? Description, DateTime? Until);
