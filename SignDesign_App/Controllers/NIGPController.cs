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
    public class NIGPController : Controller
    {
        private INIGPRepository _nigpRepository;

        public NIGPController(INIGPRepository nigpRepository)
        {
            _nigpRepository = nigpRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, NIGP nigp)
        {
            //_nigpRepository.Create(nigp);
            nigp.Id = _nigpRepository.Create(nigp);
            _nigpRepository.DisposeDBObjects();
            return Json(new[] { nigp }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _nigpRepository.Read();
            IQueryable<NIGP> nigps = JsonSerializer.Deserialize<List<NIGP>>(result).AsQueryable();
            _nigpRepository.DisposeDBObjects();
            DataSourceResult dsResult = nigps.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, NIGP nigp)
        {
            _nigpRepository.Update(nigp, (int)nigp.Id);
            _nigpRepository.DisposeDBObjects();
            return Json(new[] { nigp }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, NIGP nigp)
        {
            _nigpRepository.Delete((int)nigp.Id);
            _nigpRepository.DisposeDBObjects();
            return Json(new[] { nigp }.ToDataSourceResult(request, ModelState));
        }
    }
}
