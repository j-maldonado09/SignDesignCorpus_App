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
    public class DepartmentController : Controller
    {
        private IDepartmentRepository _departmentRepository;

        public DepartmentController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, Department department)
        {
            //_departmentRepository.Create(department);
            department.Id = _departmentRepository.Create(department);
            _departmentRepository.DisposeDBObjects();
            return Json(new[] { department }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _departmentRepository.Read();
            IQueryable<Department> departments = JsonSerializer.Deserialize<List<Department>>(result).AsQueryable();
            _departmentRepository.DisposeDBObjects();
            DataSourceResult dsResult = departments.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, Department department)
        {
            _departmentRepository.Update(department, (int)department.Id);
            _departmentRepository.DisposeDBObjects();
            return Json(new[] { department }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Department department)
        {
            _departmentRepository.Delete((int)department.Id);
            _departmentRepository.DisposeDBObjects();
            return Json(new[] { department }.ToDataSourceResult(request, ModelState));
        }
    }
}
