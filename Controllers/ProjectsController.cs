using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;

namespace MessageForAzarab.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,DCC")]
        public IActionResult Create()
        {
            var project = new Project
            {
                StartDate = DateTime.Now,
                IsActive = true
            };
            return View(project);
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                // بررسی تکراری نبودن کد پروژه
                var existingProject = await _projectService.GetProjectByCodeAsync(project.ProjectCode);
                if (existingProject != null)
                {
                    ModelState.AddModelError("ProjectCode", "این کد پروژه قبلاً در سیستم ثبت شده است");
                    return View(project);
                }

                await _projectService.CreateProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // بررسی تکراری نبودن کد پروژه
                var existingProject = await _projectService.GetProjectByCodeAsync(project.ProjectCode);
                if (existingProject != null && existingProject.Id != project.Id)
                {
                    ModelState.AddModelError("ProjectCode", "این کد پروژه قبلاً در سیستم ثبت شده است");
                    return View(project);
                }

                await _projectService.UpdateProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _projectService.DeleteProjectAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 