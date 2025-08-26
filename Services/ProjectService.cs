using MessageForAzarab.Data;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MessageForAzarab.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        
        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.OrderByDescending(p => p.StartDate).ToListAsync();
        }
        
        public async Task<List<Project>> GetActiveProjectsAsync()
        {
            return await _context.Projects.Where(p => p.IsActive).OrderByDescending(p => p.StartDate).ToListAsync();
        }
        
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }
        
        public async Task<Project?> GetProjectByCodeAsync(string code)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.ProjectCode == code);
        }
        
        public async Task<Project> CreateProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }
        
        public async Task UpdateProjectAsync(Project project)
        {
            var existingProject = await _context.Projects.FindAsync(project.Id);
            if (existingProject != null)
            {
                _context.Entry(existingProject).CurrentValues.SetValues(project);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }
    }
} 