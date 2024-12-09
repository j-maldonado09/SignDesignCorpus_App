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
    public class RegionalDistributionCenterController : Controller
    {
        private IRegionalDistributionCenterRepository _regionalDistributionCenterRepository;

        public RegionalDistributionCenterController(IRegionalDistributionCenterRepository regionalDistributionCenterRepository)
        {
            _regionalDistributionCenterRepository = regionalDistributionCenterRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, RegionalDistributionCenter regionalDistributionCenter)
        {
            //_regionalDistributionCenterRepository.Create(regionalDistributionCenter);
            regionalDistributionCenter.Id = _regionalDistributionCenterRepository.Create(regionalDistributionCenter);
            _regionalDistributionCenterRepository.DisposeDBObjects();
            return Json(new[] { regionalDistributionCenter }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _regionalDistributionCenterRepository.Read();
            IQueryable<RegionalDistributionCenter> regionalDistributionCenters = JsonSerializer.Deserialize<List<RegionalDistributionCenter>>(result).AsQueryable();
            _regionalDistributionCenterRepository.DisposeDBObjects();
            DataSourceResult dsResult = regionalDistributionCenters.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, RegionalDistributionCenter regionalDistributionCenter)
        {
            _regionalDistributionCenterRepository.Update(regionalDistributionCenter, (int)regionalDistributionCenter.Id);
            _regionalDistributionCenterRepository.DisposeDBObjects();
            return Json(new[] { regionalDistributionCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, RegionalDistributionCenter regionalDistributionCenter)
        {
            _regionalDistributionCenterRepository.Delete((int)regionalDistributionCenter.Id);
            _regionalDistributionCenterRepository.DisposeDBObjects();
            return Json(new[] { regionalDistributionCenter }.ToDataSourceResult(request, ModelState));
        }
    }
}
