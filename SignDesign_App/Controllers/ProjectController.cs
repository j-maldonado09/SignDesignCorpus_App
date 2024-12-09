using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using SignDesignCorpus;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SignDesignCorpusApp.Controllers
{
    [Authorize(Roles = "SUPERADMIN,ADMIN,SUPERVISOR,USER")]
    public class ProjectController : Controller
    {
        private IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, Project project)
        {
            //_projectRepository.Create(project);
            project.Id = _projectRepository.Create(project);
            _projectRepository.DisposeDBObjects();
            return Json(new[] { project }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            string result = _projectRepository.Read();
            IQueryable<Project> projects = JsonSerializer.Deserialize<List<Project>>(result).AsQueryable();
            _projectRepository.DisposeDBObjects();
            DataSourceResult dsResult = projects.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, Project project)
        {
            _projectRepository.Update(project, (int)project.Id);
            _projectRepository.DisposeDBObjects();
            return Json(new[] { project }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Project project)
        { 
            _projectRepository.Delete((int)project.Id);
            _projectRepository.DisposeDBObjects();
            return Json(new[] { project }.ToDataSourceResult(request, ModelState));
        }
    }
}
