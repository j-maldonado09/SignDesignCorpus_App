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
    public class AccountController : Controller
    {
        private IAccountRepositoy _accountRepository;

        public AccountController(IAccountRepositoy accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, Account account)
        {
            //_accountRepository.Create(account);
            account.Id = _accountRepository.Create(account);
            _accountRepository.DisposeDBObjects();
            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _accountRepository.Read();
            IQueryable<Account> accounts = JsonSerializer.Deserialize<List<Account>>(result).AsQueryable();
            _accountRepository.DisposeDBObjects();
            DataSourceResult dsResult = accounts.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, Account account)
        {
            _accountRepository.Update(account, (int)account.Id);
            _accountRepository.DisposeDBObjects();
            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Account account)
        {
            _accountRepository.Delete((int)account.Id);
            _accountRepository.DisposeDBObjects();
            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }
    }
}
