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
    public class FundController : Controller
    {
        private IFundRepository _fundRepository;

        public FundController(IFundRepository fundRepository)
        {
            _fundRepository = fundRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, Fund fund)
        {
            //_fundRepository.Create(fund);
            fund.Id = _fundRepository.Create(fund);
            _fundRepository.DisposeDBObjects();
            return Json(new[] { fund }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _fundRepository.Read();
            IQueryable<Fund> funds = JsonSerializer.Deserialize<List<Fund>>(result).AsQueryable();
            _fundRepository.DisposeDBObjects();
            DataSourceResult dsResult = funds.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, Fund fund)
        {
            _fundRepository.Update(fund, (int)fund.Id);
            _fundRepository.DisposeDBObjects();
            return Json(new[] { fund }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Fund fund)
        {
            _fundRepository.Delete((int)fund.Id);
            _fundRepository.DisposeDBObjects();
            return Json(new[] { fund }.ToDataSourceResult(request, ModelState));
        }
    }
}
