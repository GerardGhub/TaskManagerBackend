using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTaskManager.Identity;
using MvcTaskManager.Models;
using MvcTaskManager.ViewModels;

namespace MvcTaskManager.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db;

        public ProjectsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("api/projects")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            //System.Threading.Thread.Sleep(1000);
            List<Project> projects = await db.Projects.Include("ClientLocation").ToListAsync();

            List<ProjectViewModel> projectsViewModel = new List<ProjectViewModel>();
            foreach (var project in projects)
            {
                projectsViewModel.Add(
                    new ProjectViewModel() 
                    { ProjectID = project.ProjectID, 
                        ProjectName = project.ProjectName, 
                        TeamSize = project.TeamSize, 
                        DateOfStart = project.DateOfStart.ToString("dd/MM/yyyy"), 
                        Active = project.Active, 
                        ClientLocation = project.ClientLocation, 
                        ClientLocationID = project.ClientLocationID, 
                        Status = project.Status });
            }
            return Ok(projectsViewModel);
        }

        [HttpGet]
        [Route("api/projects/search/{searchby}/{searchtext}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Search(string searchBy, string searchText)
        {
            List<Project> projects = null;
            if (searchBy == "ProjectID")
                projects = await db.Projects.Include("ClientLocation").Where(temp => temp.ProjectID.ToString().Contains(searchText)).ToListAsync();
            else if (searchBy == "ProjectName")
                projects = await db.Projects.Include("ClientLocation").Where(temp => temp.ProjectName.Contains(searchText)).ToListAsync();
            if (searchBy == "DateOfStart")
                projects = await db.Projects.Include("ClientLocation").Where(temp => temp.DateOfStart.ToString().Contains(searchText)).ToListAsync();
            if (searchBy == "TeamSize")
                projects = await db.Projects.Include("ClientLocation").Where(temp => temp.TeamSize.ToString().Contains(searchText)).ToListAsync();

            List<ProjectViewModel> projectsViewModel = new List<ProjectViewModel>();
            foreach (var project in projects)
            {
                projectsViewModel.Add(new ProjectViewModel() { ProjectID = project.ProjectID, ProjectName = project.ProjectName, TeamSize = project.TeamSize, DateOfStart = project.DateOfStart.ToString("dd/MM/yyyy"), Active = project.Active, ClientLocation = project.ClientLocation, ClientLocationID = project.ClientLocationID, Status = project.Status });
            }

            return Ok(projectsViewModel);
        }

        [HttpGet]
        [Route("api/projects/searchbyprojectid/{ProjectID}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetProjectByProject(int ProjectID)
        {
            Project project = db.Projects.Include("ClientLocation").Where(temp => temp.ProjectID == ProjectID).FirstOrDefault();
            if (project != null)
            {
                ProjectViewModel projectViewModel = new ProjectViewModel() { ProjectID = project.ProjectID, ProjectName = project.ProjectName, TeamSize = project.TeamSize, DateOfStart = project.DateOfStart.ToString("dd/MM/yyyy"), Active = project.Active, ClientLocation = project.ClientLocation, ClientLocationID = project.ClientLocationID, Status = project.Status };
                return Ok(projectViewModel);
            }
            else
                return new EmptyResult();
        }

        [HttpPost]
        [Route("api/projects")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody] Project project)
        {
            project.ClientLocation = null;
            db.Projects.Add(project);
            await db.SaveChangesAsync();

            Project existingProject = await db.Projects.Include("ClientLocation").Where(temp => temp.ProjectID == project.ProjectID).FirstOrDefaultAsync();
            ProjectViewModel projectViewModel = new ProjectViewModel() { ProjectID = existingProject.ProjectID, ProjectName = existingProject.ProjectName, TeamSize = existingProject.TeamSize, DateOfStart = existingProject.DateOfStart.ToString("dd/MM/yyyy"), Active = existingProject.Active, ClientLocation = existingProject.ClientLocation, ClientLocationID = existingProject.ClientLocationID, Status = existingProject.Status };

            return Ok(projectViewModel);
        }

        [HttpPut]
        [Route("api/projects")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Put([FromBody] Project project)
        {
            Project existingProject = await db.Projects.Where(temp => temp.ProjectID == project.ProjectID).FirstOrDefaultAsync();
            if (existingProject != null)
            {
                existingProject.ProjectName = project.ProjectName;
                existingProject.DateOfStart = project.DateOfStart;
                existingProject.TeamSize = project.TeamSize;
                existingProject.Active = project.Active;
                existingProject.ClientLocationID = project.ClientLocationID;
                existingProject.Status = project.Status;
                existingProject.ClientLocation = null;
                db.SaveChanges();

                Project existingProject2 = await db.Projects.Include("ClientLocation").Where(temp => temp.ProjectID == project.ProjectID).FirstOrDefaultAsync();
                ProjectViewModel projectViewModel = new ProjectViewModel() { ProjectID = existingProject2.ProjectID, ProjectName = existingProject2.ProjectName, TeamSize = existingProject2.TeamSize, ClientLocationID = existingProject2.ClientLocationID, DateOfStart = existingProject2.DateOfStart.ToString("dd/MM/yyyy"), Active = existingProject2.Active, Status = existingProject2.Status, ClientLocation = existingProject2.ClientLocation };
                return Ok(projectViewModel);
            }
            else
            {
                return null;
            }
        }

        [HttpDelete]
        [Route("api/projects")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<int> Delete(int ProjectID)
        {
            Project existingProject = await db.Projects.Where(temp => temp.ProjectID == ProjectID).FirstOrDefaultAsync();
            if (existingProject != null)
            {
                db.Projects.Remove(existingProject);
                await db.SaveChangesAsync();
                return ProjectID;
            }
            else
            {
                return -1;
            }
        }




    }
}


