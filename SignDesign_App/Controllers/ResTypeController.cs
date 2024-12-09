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
    public class ResTypeController : Controller
    {
        private IResTypeRepository _resTypeRepository;

        public ResTypeController(IResTypeRepository resTypeRepository)
        {
            _resTypeRepository = resTypeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, ResType resType)
        {
            //_resTypeRepository.Create(resType);
            resType.Id = _resTypeRepository.Create(resType);
            _resTypeRepository.DisposeDBObjects();
            return Json(new[] { resType }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _resTypeRepository.Read();
            IQueryable<ResType> resTypes = JsonSerializer.Deserialize<List<ResType>>(result).AsQueryable();
            _resTypeRepository.DisposeDBObjects();
            DataSourceResult dsResult = resTypes.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, ResType resType)
        {
            _resTypeRepository.Update(resType, (int)resType.Id);
            _resTypeRepository.DisposeDBObjects();
            return Json(new[] { resType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, ResType resType)
        {
            _resTypeRepository.Delete((int)resType.Id);
            _resTypeRepository.DisposeDBObjects();
            return Json(new[] { resType }.ToDataSourceResult(request, ModelState));
        }
    }
}
