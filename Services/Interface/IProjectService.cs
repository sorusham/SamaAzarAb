using MessageForAzarab.Models;

namespace MessageForAzarab.Services.Interface
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllProjectsAsync();
        Task<List<Project>> GetActiveProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project?> GetProjectByCodeAsync(string code);
        Task<Project> CreateProjectAsync(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(int id);
    }
} 