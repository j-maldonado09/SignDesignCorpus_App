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
    public class DistrictContactController : Controller
    {
        private IDistrictContactRepository _districtContactRepository;

        public DistrictContactController(IDistrictContactRepository districtContactRepository)
        {
            _districtContactRepository = districtContactRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, DistrictContact districtContact)
        {
            //_districtContactRepository.Create(districtContact);
            districtContact.Id = _districtContactRepository.Create(districtContact);
            _districtContactRepository.DisposeDBObjects();
            return Json(new[] { districtContact }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _districtContactRepository.Read();
            IQueryable<DistrictContact> districtContacts = JsonSerializer.Deserialize<List<DistrictContact>>(result).AsQueryable();
            _districtContactRepository.DisposeDBObjects();
            DataSourceResult dsResult = districtContacts.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, DistrictContact districtContact)
        {
            _districtContactRepository.Update(districtContact, (int)districtContact.Id);
            _districtContactRepository.DisposeDBObjects();
            return Json(new[] { districtContact }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, DistrictContact districtContact)
        {
            _districtContactRepository.Delete((int)districtContact.Id);
            _districtContactRepository.DisposeDBObjects();
            return Json(new[] { districtContact }.ToDataSourceResult(request, ModelState));
        }
    }
}