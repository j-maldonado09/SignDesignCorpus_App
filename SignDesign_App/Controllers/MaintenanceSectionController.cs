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
    public class MaintenanceSectionController : Controller
    {
        private IMaintenanceSectionRepository _maintenanceSectionRepository;

        public MaintenanceSectionController(IMaintenanceSectionRepository maintenanceSectionRepository)
        {
            _maintenanceSectionRepository = maintenanceSectionRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, MaintenanceSection maintenanceSection)
        {
            //_maintenanceSectionRepository.Create(maintenanceSection);
            maintenanceSection.Id = _maintenanceSectionRepository.Create(maintenanceSection);
            _maintenanceSectionRepository.DisposeDBObjects();
            return Json(new[] { maintenanceSection }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _maintenanceSectionRepository.Read();
            IQueryable<MaintenanceSection> maintenanceSections = JsonSerializer.Deserialize<List<MaintenanceSection>>(result).AsQueryable();
            _maintenanceSectionRepository.DisposeDBObjects();
            DataSourceResult dsResult = maintenanceSections.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, MaintenanceSection maintenanceSection)
        {
            _maintenanceSectionRepository.Update(maintenanceSection, (int)maintenanceSection.Id);
            _maintenanceSectionRepository.DisposeDBObjects();
            return Json(new[] { maintenanceSection }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, MaintenanceSection maintenanceSection)
        {
            _maintenanceSectionRepository.Delete((int)maintenanceSection.Id);
            _maintenanceSectionRepository.DisposeDBObjects();
            return Json(new[] { maintenanceSection }.ToDataSourceResult(request, ModelState));
        }
    }
}
