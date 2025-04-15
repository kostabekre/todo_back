namespace TodoistApi.Responses;

public record GetProjectTaskResponse(int Id, string Name, string? Description, DateTime Created, DateTime Updated, bool IsDone, DateTime? Until);
