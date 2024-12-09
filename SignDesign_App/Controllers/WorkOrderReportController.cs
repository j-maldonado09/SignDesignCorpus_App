using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using SignDesignCorpus;
using SignDesignCorpus.ViewModels;
using System.Text.Json;

namespace SignDesign_App.Controllers
{
    public class WorkOrderReportController : Controller
    {
        private WorkOrderReport _workOrderReport;

        public WorkOrderReportController(WorkOrderReport workOrderReport)
        {
            _workOrderReport = workOrderReport;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _workOrderReport.Read();
            IQueryable<WorkOrderReportViewModel> workOrderReports = JsonSerializer.Deserialize<List<WorkOrderReportViewModel>>(result).AsQueryable();
            _workOrderReport.DisposeDBObjects();
            DataSourceResult dsResult = workOrderReports.ToDataSourceResult(request);
            return Json(dsResult);
        }
    }
}
