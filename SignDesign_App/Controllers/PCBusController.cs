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
    public class PCBusController : Controller
    {
        private IPCBusRepository _pcBusRepository;

        public PCBusController(IPCBusRepository pCBusRepository)
        {
            _pcBusRepository = pCBusRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, PCBus pcBus)
        {
            //_pcBusRepository.Create(pcBus);
            pcBus.Id = _pcBusRepository.Create(pcBus);
            _pcBusRepository.DisposeDBObjects();
            return Json(new[] { pcBus }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _pcBusRepository.Read();
            IQueryable<PCBus> pCBuses = JsonSerializer.Deserialize<List<PCBus>>(result).AsQueryable();
            _pcBusRepository.DisposeDBObjects();
            DataSourceResult dsResult = pCBuses.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, PCBus pCBus)
        {
            _pcBusRepository.Update(pCBus, (int)pCBus.Id);
            _pcBusRepository.DisposeDBObjects();
            return Json(new[] { pCBus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, PCBus pCBus)
        {
            _pcBusRepository.Delete((int)pCBus.Id);
            _pcBusRepository.DisposeDBObjects();
            return Json(new[] { pCBus }.ToDataSourceResult(request, ModelState));
        }
    }
}
