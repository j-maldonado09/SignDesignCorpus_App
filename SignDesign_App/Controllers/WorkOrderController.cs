using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using SignDesignCorpus;
using System.Text.Json;
using System.Linq;
using SignDesign_App.Services;
using SignDesignCorpus.ViewModels;
using SignDesignCorpus.HelperModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using SignDesign_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SignDesignCorpusApp.Controllers
{
    [Authorize(Roles = "SUPERADMIN,ADMIN,ENGINEER,SUPERVISOR,USER")]
    public class WorkOrderController : Controller
    {
        private IWorkOrderRepository _workOrderRepository;
        private IProjectRepository _projectRepository;
        private IMaintenanceSectionContactRepository _maintenanceSectionContactRepository;
        private IResTypeRepository _resTypeRepository;
        private ITaskRepository _taskRepository;
        private IActivityRepository _activityRepository;
        private IPCBusRepository _pcBusRepository;
        private IAccountRepositoy _accountRepository;
        private IDistrictContactRepository _districtContactRepository;
        private IRegionalDistributionCenterRepository _regionalDistributionCnterRepository;
        private IDepartmentRepository _departmentRepository;
        private IFundRepository _fundRepository;
        private INIGPRepository _nigpRepository;
        private IMaintenanceSectionRepository _maintenanceSectionRepository;
        private ISignShopRepository _signShopRepository;
        private IYearRepository _yearRepository;
        private EMailSender _eMailSender;
        private ExportPdf _exportPdf;
        private IWebHostEnvironment _webHostEnvironment;
        private UserManager<ApplicationUser> _userManager;

        public WorkOrderController(IWorkOrderRepository workOrderRepository, IProjectRepository projectRepository, 
            IMaintenanceSectionContactRepository maintenanceSectionContact, 
            IResTypeRepository resTypeRepository, ITaskRepository taskRepository, IActivityRepository activityRepository, 
            IPCBusRepository pCBusRepository, IAccountRepositoy accountRepositoy, IDistrictContactRepository districtContactRepository, 
            IRegionalDistributionCenterRepository regionalDistributionCnterRepository, IDepartmentRepository departmentRepository, 
            INIGPRepository nigpRepository, IFundRepository fundRepository, IMaintenanceSectionRepository maintenanceSectionRepository,
            ISignShopRepository signShopRepository, IYearRepository yearRepository, EMailSender eMailSender, ExportPdf exportPdf, 
            IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _workOrderRepository = workOrderRepository;
            _projectRepository = projectRepository;
            _maintenanceSectionContactRepository = maintenanceSectionContact;
            _resTypeRepository = resTypeRepository;
            _taskRepository = taskRepository;
            _activityRepository = activityRepository;
            _pcBusRepository = pCBusRepository;
            _accountRepository = accountRepositoy;
            _districtContactRepository = districtContactRepository;
            _regionalDistributionCnterRepository = regionalDistributionCnterRepository;
            _departmentRepository = departmentRepository;
            _fundRepository = fundRepository;
            _nigpRepository = nigpRepository;
            _maintenanceSectionRepository = maintenanceSectionRepository;
            _signShopRepository = signShopRepository;
            _yearRepository = yearRepository;
            _eMailSender = eMailSender;
            _exportPdf = exportPdf;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ViewData["regionalDistributionCenters"] = GetRegionalDistributionCenters();
            ViewData["years"] = GetYears();
            ViewData["departments"] = GetDepartments();
            ViewData["accounts"] =  GetAccounts();
            ViewData["funds"] = GetFunds();
            ViewData["tasks"] = GetTasks();
            ViewData["pCBuses"] = GetPCBuses();
            ViewData["projects"] = GetProjects();
            ViewData["activities"] = GetActivities();
            ViewData["resTypes"] = GetResTypes();
            //ViewData["maintenanceSectionContacts"] = GetMaintenanceSectionContacts();
            ViewData["districtContacts"] = GetDistrictContacts();
            ViewData["maintenanceSections"] = GetMaintenanceSections();

            //var users = GetUsersInRole("SUPERVISOR").Result;
            ViewData["maintenanceSectionRequestors"] = GetUsersInRole("USER").Result;
            ViewData["maintenanceSectionApprovers"] = GetUsersInRole("SUPERVISOR").Result;
            ViewData["areaEngineers"] = GetUsersInRole("ENGINEER").Result;
            ViewData["districtApprovers"] = GetUsersInRole("ADMIN").Result;
            //ViewData["currentUserId"] = CheckCurrentUserRole("SUPERVISOR").Result;

            return View();
        }

        //public async Task<int> CheckCurrentUserRole(IEnumerable<ApplicationUser> users)
        public async Task<string> CheckCurrentUserRole(string role)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            //List<ApplicationUser> newUsers = users.ToList();
            //return newUsers.IndexOf(currentUser);
            var roles = _userManager.GetRolesAsync(currentUser).Result;
            if (roles[0] == role)
                return currentUser.Id;
            else
                return null;
        }

        [HttpPost]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, [FromBody] WorkOrderHelperModel workOrder)
        {
            _workOrderRepository.Create(workOrder);
            _workOrderRepository.DisposeDBObjects();
            return Json(new[] { workOrder }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _workOrderRepository.Read();
            IQueryable<WorkOrderViewModel> workOrders = JsonSerializer.Deserialize<List<WorkOrderViewModel>>(result).AsQueryable();
            _workOrderRepository.DisposeDBObjects();
            DataSourceResult dsResult = workOrders.ToDataSourceResult(request);
            return Json(dsResult);
        }

        [HttpPost]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, [FromBody] WorkOrderHelperModel workOrder)
        {
            _workOrderRepository.Update(workOrder, (int)workOrder.Id);
            _workOrderRepository.DisposeDBObjects();
            return Json(new[] { workOrder }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult ReadWorkOrders(int id)
        {
            string result = _workOrderRepository.ReadWorkOrders(id);
            _workOrderRepository.DisposeDBObjects();
            return Json(result);
        }

        public JsonResult ReadMaintenanceSectionParameters(int? id)
        {
            string result = _maintenanceSectionRepository.ReadMaintenanceSectionParameters(id);
            _maintenanceSectionRepository.DisposeDBObjects();
            return Json(result);
        }

        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, WorkOrderViewModel workOrder)
        {
            _workOrderRepository.Delete((int)workOrder.Id);
            _workOrderRepository.DisposeDBObjects();
            return Json(new[] { workOrder }.ToDataSourceResult(request, ModelState));
        }

        //public IActionResult GetProjects([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _projectRepository.Read();
        //    IQueryable<Project> projects = JsonSerializer.Deserialize<List<Project>>(result).AsQueryable();
        //    _projectRepository.DisposeDBObjects();
        //    return Json(projects.ToDataSourceResult(request));
        //}

        public IEnumerable<Project> GetProjects()
        {
            string result = _projectRepository.Read();
            IEnumerable<Project> projects = JsonSerializer.Deserialize<List<Project>>(result).AsEnumerable();
            _projectRepository.DisposeDBObjects();
            return projects;
        }

        //public IActionResult GetResTypes([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _resTypeRepository.Read();
        //    IQueryable<ResType> resTypes = JsonSerializer.Deserialize<List<ResType>>(result).AsQueryable();
        //    _resTypeRepository.DisposeDBObjects();
        //    return Json(resTypes.ToDataSourceResult(request));
        //}

        public IEnumerable<ResType> GetResTypes()
        {
            string result = _resTypeRepository.Read();
            IEnumerable<ResType> resTypes = JsonSerializer.Deserialize<List<ResType>>(result).AsEnumerable();
            _resTypeRepository.DisposeDBObjects();
            return resTypes;
        }

        //public IActionResult GetTasks([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _taskRepository.Read();
        //    IQueryable<Task_> tasks = JsonSerializer.Deserialize<List<Task_>>(result).AsQueryable();
        //    _resTypeRepository.DisposeDBObjects();
        //    return Json(tasks.ToDataSourceResult(request));
        //}

        public IEnumerable<Task_> GetTasks()
        {
            string result = _taskRepository.Read();
            IEnumerable<Task_> tasks = JsonSerializer.Deserialize<List<Task_>>(result).AsEnumerable();
            _taskRepository.DisposeDBObjects();
            return tasks;
        }

        //public IActionResult GetActivities([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _activityRepository.Read();
        //    IQueryable<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(result).AsQueryable();
        //    _activityRepository.DisposeDBObjects();
        //    return Json(activities.ToDataSourceResult(request));
        //}

        public IEnumerable<Activity> GetActivities()
        {
            string result = _activityRepository.Read();
            IEnumerable<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(result).AsEnumerable();
            _activityRepository.DisposeDBObjects();
            return activities;
        }

        //public IActionResult GetMaintenanceSectionContacts([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _maintenanceSectionContactRepository.Read();
        //    IQueryable<MaintenanceSectionContact> maintenanceSectionContacts = JsonSerializer.Deserialize<List<MaintenanceSectionContact>>(result).AsQueryable();
        //    _projectRepository.DisposeDBObjects();
        //    return Json(maintenanceSectionContacts.ToDataSourceResult(request));
        //}

        //public IEnumerable<MaintenanceSectionContact> GetMaintenanceSectionContacts()
        //{
        //    string result = _maintenanceSectionContactRepository.Read();
        //    IEnumerable<MaintenanceSectionContact> maintenanceSectionContacts = JsonSerializer.Deserialize<List<MaintenanceSectionContact>>(result).AsEnumerable();
        //    _maintenanceSectionContactRepository.DisposeDBObjects();
        //    return maintenanceSectionContacts;
        //}

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRole(string role)
        {
            IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(role);
            return users;
        }

        //public IActionResult GetPCBuses([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _pcBusRepository.Read();
        //    IQueryable<PCBus> pCBuses = JsonSerializer.Deserialize<List<PCBus>>(result).AsQueryable();
        //    _pcBusRepository.DisposeDBObjects();
        //    return Json(pCBuses.ToDataSourceResult(request));
        //}

        public IEnumerable<PCBus> GetPCBuses()
        {
            string result = _pcBusRepository.Read();
            IEnumerable<PCBus> pCBuses = JsonSerializer.Deserialize<List<PCBus>>(result).AsEnumerable();
            _pcBusRepository.DisposeDBObjects();
            return pCBuses;
        }

        //public IActionResult GetAccounts([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _accountRepository.Read();
        //    IQueryable<Account> accounts = JsonSerializer.Deserialize<List<Account>>(result).AsQueryable();
        //    _accountRepository.DisposeDBObjects();
        //    return Json(accounts.ToDataSourceResult(request));
        //}

        public IEnumerable<Account> GetAccounts()
        {
            string result = _accountRepository.Read();
            IEnumerable<Account> accounts = JsonSerializer.Deserialize<List<Account>>(result).AsEnumerable();
            _accountRepository.DisposeDBObjects();
            return accounts;
        }

        //public IActionResult GetDistrictContacts([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _districtContactRepository.Read();
        //    IQueryable<DistrictContact> districtContacts = JsonSerializer.Deserialize<List<DistrictContact>>(result).AsQueryable();
        //    _districtContactRepository.DisposeDBObjects();
        //    return Json(districtContacts.ToDataSourceResult(request));
        //}

        public IEnumerable<DistrictContact> GetDistrictContacts()
        {
            string result = _districtContactRepository.Read();
            IEnumerable<DistrictContact> districtContacts = JsonSerializer.Deserialize<List<DistrictContact>>(result).AsEnumerable();
            _districtContactRepository.DisposeDBObjects();
            return districtContacts;
        }

        //public IActionResult GetRegionalDistributionCenters([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _regionalDistributionCnterRepository.Read();
        //    IQueryable<RegionalDistributionCenter> regionalDistributionCenters = JsonSerializer.Deserialize<List<RegionalDistributionCenter>>(result).AsQueryable();
        //    _regionalDistributionCnterRepository.DisposeDBObjects();
        //    return Json(regionalDistributionCenters.ToDataSourceResult(request));
        //}

        public IEnumerable<RegionalDistributionCenter> GetRegionalDistributionCenters()
        {
            string result = _regionalDistributionCnterRepository.Read();
            IEnumerable<RegionalDistributionCenter> regionalDistributionCenters = JsonSerializer.Deserialize<List<RegionalDistributionCenter>>(result).AsEnumerable();
            _regionalDistributionCnterRepository.DisposeDBObjects();
            return regionalDistributionCenters;
        }

        //public IActionResult GetDepartments([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _departmentRepository.Read();
        //    IQueryable<Department> departments = JsonSerializer.Deserialize<List<Department>>(result).AsQueryable();
        //    _departmentRepository.DisposeDBObjects();
        //    return Json(departments.ToDataSourceResult(request));
        //}

        public IEnumerable<Department> GetDepartments()
        {
            string result = _departmentRepository.Read();
            IEnumerable<Department> departments = JsonSerializer.Deserialize<List<Department>>(result).AsEnumerable();
            _departmentRepository.DisposeDBObjects();
            return departments;
        }

        //public IActionResult GetFunds([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _fundRepository.Read();
        //    IQueryable<Fund> funds = JsonSerializer.Deserialize<List<Fund>>(result).AsQueryable();
        //    _fundRepository.DisposeDBObjects();
        //    return Json(funds.ToDataSourceResult(request));
        //}

        public IEnumerable<Fund> GetFunds()
        {
            string result = _fundRepository.Read();
            IEnumerable<Fund> funds = JsonSerializer.Deserialize<List<Fund>>(result).AsEnumerable();
            _fundRepository.DisposeDBObjects();
            return funds;
        }

        public IActionResult GetNIGPs([DataSourceRequest] DataSourceRequest request)
        {
            string result = _nigpRepository.Read();
            IQueryable<NIGP> nigps = JsonSerializer.Deserialize<List<NIGP>>(result).AsQueryable();
            _nigpRepository.DisposeDBObjects();
            return Json(nigps.ToDataSourceResult(request));
        }

        public string GetNIGPsAux()
        {
            string result = _nigpRepository.Read();
            _nigpRepository.DisposeDBObjects();
            return result;
        }

        //public IActionResult GetMaintenanceSections([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _maintenanceSectionRepository.Read();
        //    IQueryable<MaintenanceSection> maintenanceSections = JsonSerializer.Deserialize<List<MaintenanceSection>>(result).AsQueryable();
        //    _maintenanceSectionRepository.DisposeDBObjects();
        //    return Json(maintenanceSections.ToDataSourceResult(request));
        //}

        public IEnumerable<MaintenanceSection> GetMaintenanceSections()
        {
            string result = _maintenanceSectionRepository.Read();
            IEnumerable<MaintenanceSection> maintenanceSections = JsonSerializer.Deserialize<List<MaintenanceSection>>(result).AsEnumerable();
            _maintenanceSectionRepository.DisposeDBObjects();
            return maintenanceSections;
        }

        public IActionResult GetSignShops([DataSourceRequest] DataSourceRequest request)
        {
            string result = _signShopRepository.Read();
            IQueryable<SignShop> signShops = JsonSerializer.Deserialize<List<SignShop>>(result).AsQueryable();
            _signShopRepository.DisposeDBObjects();
            return Json(signShops.ToDataSourceResult(request));
        }

        //public IActionResult GetYears([DataSourceRequest] DataSourceRequest request)
        //{
        //    string result = _yearRepository.Read();
        //    IQueryable<Year> years = JsonSerializer.Deserialize<List<Year>>(result).AsQueryable();
        //    _yearRepository.DisposeDBObjects();
        //    return Json(years.ToDataSourceResult(request));
        //}

        public IEnumerable<Year> GetYears()
        {
            string result = _yearRepository.Read();
            IEnumerable<Year> years = JsonSerializer.Deserialize<List<Year>>(result).AsEnumerable();
            _yearRepository.DisposeDBObjects();
            return years;
        }

        public async Task<IActionResult> SendMail()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var roles = _userManager.GetRolesAsync(currentUser).Result;
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            string usersList = "";

            switch (roles[0])
            {
                case "USER":
                    applicationUsers = GetUsersInRole("SUPERVISOR").Result.ToList();

                    //filter supervisor that belongs to user's maintenance section
                    applicationUsers = applicationUsers
                        .Where(user => user.MaintenanceSectionId == currentUser.MaintenanceSectionId)
                        .ToList();
                    break;
                case "SUPERVISOR":
                    applicationUsers = GetUsersInRole("ADMIN").Result.ToList();
                    break;
            }

            foreach (ApplicationUser user in applicationUsers)
            {
                usersList += user.Email + ";";
            }
            usersList = usersList.Remove(usersList.Length - 1, 1);

            string htmlMessage = "<h1>A new sign request has been updated</h1>" +
                "<h3>Please login to your account to see updated request</h3>";

            await _eMailSender.SendEmailAsync(usersList, "Sign Request", htmlMessage);
            return View("Index");
        }

        public string ExportPdf([FromBody] WorkOrderHelperModel workOrder)
        {
            string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath);
            string result = "";

            result = _projectRepository.Read(workOrder.ProjectId);
            List<Project> project = JsonSerializer.Deserialize<List<Project>>(result).ToList<Project>();
            _projectRepository.DisposeDBObjects();

            result = _resTypeRepository.Read(workOrder.ResTypeId);
            List<ResType> resType = JsonSerializer.Deserialize<List<ResType>>(result).ToList<ResType>();
            _resTypeRepository.DisposeDBObjects();

            result = _taskRepository.Read(workOrder.TaskId);
            List<Task_> task = result is null ? null : JsonSerializer.Deserialize<List<Task_>>(result).ToList<Task_>();
            _taskRepository.DisposeDBObjects();

            result = _activityRepository.Read(workOrder.ActivityId);
            List<Activity> activity = JsonSerializer.Deserialize<List<Activity>>(result).ToList<Activity>();
            _activityRepository.DisposeDBObjects();

            //result = _maintenanceSectionContactRepository.Read(workOrder.RequestedByMaintenanceId);
            //List<MaintenanceSectionContact> maintenanceSectionContactRequestedBy = JsonSerializer.Deserialize<List<MaintenanceSectionContact>>(result).ToList<MaintenanceSectionContact>();
            //_maintenanceSectionContactRepository.DisposeDBObjects();

            ApplicationUser maintenanceSectionContactRequestedBy  = _userManager.FindByIdAsync(workOrder.RequestedByMaintenanceId).Result;

            //result = _maintenanceSectionContactRepository.Read(workOrder.ApprovedByMaintenanceId);
            //List<MaintenanceSectionContact> maintenanceSectionContactApprovedBy = result is null ? null : JsonSerializer.Deserialize<List<MaintenanceSectionContact>>(result).ToList<MaintenanceSectionContact>();
            //_maintenanceSectionContactRepository.DisposeDBObjects();

            ApplicationUser maintenanceSectionContactApprovedBy = _userManager.FindByIdAsync(workOrder.ApprovedByMaintenanceId).Result;

            result = _pcBusRepository.Read(workOrder.PCBusId);
            List<PCBus> pcBus = JsonSerializer.Deserialize<List<PCBus>>(result).ToList<PCBus>();
            _pcBusRepository.DisposeDBObjects();

            result = _accountRepository.Read(workOrder.AccountId);
            List<Account> account = JsonSerializer.Deserialize<List<Account>>(result).ToList<Account>();
            _accountRepository.DisposeDBObjects();

            //result = _districtContactRepository.Read(workOrder.ApprovedByDistrictId);
            //List<DistrictContact> districtContact = result is null ? null : JsonSerializer.Deserialize<List<DistrictContact>>(result).ToList<DistrictContact>();
            //_districtContactRepository.DisposeDBObjects();

            ApplicationUser districtContact = _userManager.FindByIdAsync(workOrder.ApprovedByDistrictId).Result;

            result = _regionalDistributionCnterRepository.Read(workOrder.MaterialRequestedFromId);
            List<RegionalDistributionCenter> regionalDistributionCenter = JsonSerializer.Deserialize<List<RegionalDistributionCenter>>(result).ToList<RegionalDistributionCenter>();
            _regionalDistributionCnterRepository.DisposeDBObjects();

            result = _departmentRepository.Read(workOrder.DepartmentId);
            List<Department> department = JsonSerializer.Deserialize<List<Department>>(result).ToList<Department>();
            _departmentRepository.DisposeDBObjects();

            result = _fundRepository.Read(workOrder.FundId);
            List<Fund> fund = JsonSerializer.Deserialize<List<Fund>>(result).ToList<Fund>();
            _fundRepository.DisposeDBObjects();

            result = _maintenanceSectionRepository.Read(workOrder.MaterialRequestedById);
            List<MaintenanceSection> maintenanceSection = JsonSerializer.Deserialize<List<MaintenanceSection>>(result).ToList<MaintenanceSection>();
            _maintenanceSectionRepository.DisposeDBObjects();

            //result = _signShopRepository.Read(workOrder.signsh);
            //List<Department> department = JsonSerializer.Deserialize<List<Department>>(result).ToList<Department>();

            WorkOrderNamesHelperModel workOrderNamesHelperModel = new WorkOrderNamesHelperModel();

            

            workOrderNamesHelperModel.AccountName = account[0].Name;
            workOrderNamesHelperModel.ActivityName = activity[0].Name;
            //workOrderNamesHelperModel.ApprovedByDistrictName = districtContact?[0].FirstName + " " + districtContact?[0].LastName;
            workOrderNamesHelperModel.ApprovedByDistrictName = districtContact?.ContactFullName;
            //workOrderNamesHelperModel.ApprovedByMaintenanceName = maintenanceSectionContactApprovedBy?[0].FirstName + " " + maintenanceSectionContactApprovedBy?[0].LastName;
            workOrderNamesHelperModel.ApprovedByMaintenanceName = maintenanceSectionContactApprovedBy?.ContactFullName;
            workOrderNamesHelperModel.DepartmentName = department[0].Name;
            workOrderNamesHelperModel.FundName = fund[0].Name;
            workOrderNamesHelperModel.MaterialRequestedByAddress = maintenanceSection[0].Street;
            workOrderNamesHelperModel.MaterialRequestedByCity = maintenanceSection[0].City;
            workOrderNamesHelperModel.MaterialRequestedByEmail = maintenanceSection[0].Email;
            workOrderNamesHelperModel.MaterialRequestedByName = maintenanceSection[0].Name;
            workOrderNamesHelperModel.MaterialRequestedByNumber = maintenanceSection[0].Number;
            workOrderNamesHelperModel.MaterialRequestedByState = maintenanceSection[0].State;
            workOrderNamesHelperModel.MaterialRequestedByZipCode = maintenanceSection[0].ZipCode;
            workOrderNamesHelperModel.MaterialRequestedFromName = regionalDistributionCenter[0].Name;
            workOrderNamesHelperModel.PCBusName = pcBus[0].Name;
            workOrderNamesHelperModel.ProjectName = project[0].Name;
            //workOrderNamesHelperModel.RequestedByMaintenanceName = maintenanceSectionContactRequestedBy[0].FirstName + " " + maintenanceSectionContactRequestedBy[0].LastName;
            workOrderNamesHelperModel.RequestedByMaintenanceName = maintenanceSectionContactRequestedBy.ContactFullName;
            workOrderNamesHelperModel.ResTypeName = resType[0].Name;
            workOrderNamesHelperModel.TaskName = task?[0].Name;

            _exportPdf.CreatePdf(workOrder, workOrderNamesHelperModel, physicalPath);

            //string physicalPath = "wwwroot/KironGitHub.pdf";

            //byte[] pdfBytes = System.IO.File.ReadAllBytes(Path.Combine(physicalPath, "SignRequest.pdf"));
            //MemoryStream ms = new MemoryStream(pdfBytes);
            //return new FileStreamResult(ms, "application/pdf");

            return "SignRequest_" + workOrderNamesHelperModel.MaterialRequestedByNumber + "-" + workOrder.Id + ".pdf";
            //return "SignRequest.pdf";
        }

        // Saves the sign image that was uploaded with the kendo upload control
        public async Task<ActionResult> SaveUploadedImage(IEnumerable<IFormFile> files)
        {
            // The Name attribute of the Kendo Upload component is "files" and it must match the parameter name of this action.
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "signs", fileName);

                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            // Return an empty string to signify success.
            return Content("");
        }

        // Removed the sign image that was uploaded with the kendo upload control
        public ActionResult RemoveUploadedImage(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames".

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "signs", fileName);

                    // TODO: Verify user permissions.

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success.
            return Content("");
        }

        // Saves the installed sign image that was uploaded with the kendo upload control
        public async Task<ActionResult> SaveUploadedInstalledImage(IEnumerable<IFormFile> filesInstalled)
        {
            // The Name attribute of the Kendo Upload component is "files" and it must match the parameter name of this action.
            if (filesInstalled != null)
            {
                foreach (var file in filesInstalled)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "installed_signs", fileName);

                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            // Return an empty string to signify success.
            return Content("");
        }

        // Removed the sign image that was uploaded with the kendo upload control
        public ActionResult RemoveUploadedInstalledImage(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames".

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "installed_signs", fileName);

                    // TODO: Verify user permissions.

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success.
            return Content("");
        }

        // Saves the attachment that was uploaded with the kendo upload control
        public async Task<ActionResult> SaveUploadedAttachment(IEnumerable<IFormFile> attachments)
        {
            // The Name attribute of the Kendo Upload component is "attachments" and it must match the parameter name of this action.
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(attachment.ContentDisposition);

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "attachments", fileName);

                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(fileStream);
                    }
                }
            }

            // Return an empty string to signify success.
            return Content("");
        }

        // Removed the attachment that was uploaded with the kendo upload control
        public ActionResult RemoveUploadedAttachment(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames".

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "attachments", fileName);

                    // TODO: Verify user permissions.

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success.
            return Content("");
        }
    }
}
