using TodoistApi.Models;

namespace TodoistApi.Responses;

public static class ProjectTaskMapper
{
    public static GetProjectTaskResponse ConvertToResponse(ProjectTask task)
    {
        return new GetProjectTaskResponse(task.Id, task.Name, task.Description, task.Created, task.Updated, task.IsDone, task.Until);
    }
}

