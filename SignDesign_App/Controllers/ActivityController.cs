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
    public class ActivityController : Controller
    {
        private IActivityRepository _activityRepository;

        public ActivityController(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, Activity activity)
        {
            //_activityRepository.Create(activity);
            activity.Id = _activityRepository.Create(activity);
            _activityRepository.DisposeDBObjects();
            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _activityRepository.Read();
            IQueryable<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(result).AsQueryable();
            _activityRepository.DisposeDBObjects();
            DataSourceResult dsResult = activities.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [AcceptVerbs("Post")]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, Activity activity)
        {
            _activityRepository.Update(activity, (int)activity.Id);
            _activityRepository.DisposeDBObjects();
            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Activity activity)
        {
            _activityRepository.Delete((int)activity.Id);
            _activityRepository.DisposeDBObjects();
            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }
    }
}
