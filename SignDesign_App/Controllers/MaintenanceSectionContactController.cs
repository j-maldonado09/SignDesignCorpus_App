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
    public class MaintenanceSectionContactController : Controller
    {
        private IMaintenanceSectionContactRepository _maintenanceSectionContactRepository;

        public MaintenanceSectionContactController(IMaintenanceSectionContactRepository maintenanceSectionContactRepository)
        {
            _maintenanceSectionContactRepository = maintenanceSectionContactRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, MaintenanceSectionContact maintenanceSectionContact)
        {
            //_maintenanceSectionContactRepository.Create(maintenanceSectionContact);
            maintenanceSectionContact.Id = _maintenanceSectionContactRepository.Create(maintenanceSectionContact);
            _maintenanceSectionContactRepository.DisposeDBObjects();
            return Json(new[] { maintenanceSectionContact }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _maintenanceSectionContactRepository.Read();
            IQueryable<MaintenanceSectionContact> maintenanceSectionContacts = JsonSerializer.Deserialize<List<MaintenanceSectionContact>>(result).AsQueryable();
            _maintenanceSectionContactRepository.DisposeDBObjects();
            DataSourceResult dsResult = maintenanceSectionContacts.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, MaintenanceSectionContact maintenanceSectionContact)
        {
            _maintenanceSectionContactRepository.Update(maintenanceSectionContact, (int)maintenanceSectionContact.Id);
            _maintenanceSectionContactRepository.DisposeDBObjects();
            return Json(new[] { maintenanceSectionContact }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, MaintenanceSectionContact maintenanceSectionContact)
        {
            _maintenanceSectionContactRepository.Delete((int)maintenanceSectionContact.Id);
            _maintenanceSectionContactRepository.DisposeDBObjects();
            return Json(new[] { maintenanceSectionContact }.ToDataSourceResult(request, ModelState));
        }
    }
}