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
    public class SignShopController : Controller
    {
        private ISignShopRepository _signShopRepository;

        public SignShopController(ISignShopRepository signShopRepository)
        {
            _signShopRepository = signShopRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, SignShop signShop)
        {
            //_signShopRepository.Create(signShop);
            signShop.Id = _signShopRepository.Create(signShop);
            _signShopRepository.DisposeDBObjects();
            return Json(new[] { signShop }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _signShopRepository.Read();
            IQueryable<SignShop> signShops = JsonSerializer.Deserialize<List<SignShop>>(result).AsQueryable();
            _signShopRepository.DisposeDBObjects();
            DataSourceResult dsResult = signShops.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, SignShop signShop)
        {
            _signShopRepository.Update(signShop, (int)signShop.Id);
            _signShopRepository.DisposeDBObjects();
            return Json(new[] { signShop }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, SignShop signShop)
        {
            _signShopRepository.Delete((int)signShop.Id);
            _signShopRepository.DisposeDBObjects();
            return Json(new[] { signShop }.ToDataSourceResult(request, ModelState));
        }
    }
}
