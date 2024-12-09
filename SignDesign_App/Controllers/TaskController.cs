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
    public class TaskController : Controller
    {
        private ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, Task_ task)
        {
            //_taskRepository.Create(task);
            task.Id = _taskRepository.Create(task);
            _taskRepository.DisposeDBObjects();
            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _taskRepository.Read();
            IQueryable<Task_> tasks = JsonSerializer.Deserialize<List<Task_>>(result).AsQueryable();
            _taskRepository.DisposeDBObjects();
            DataSourceResult dsResult = tasks.ToDataSourceResult(request);
            return Json(dsResult);
        }
        
        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, Task_ task)
        {
            _taskRepository.Update(task, (int)task.Id);
            _taskRepository.DisposeDBObjects();
            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Task_ task)
        {
            _taskRepository.Delete((int)task.Id);
            _taskRepository.DisposeDBObjects();
            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }
    }
}
