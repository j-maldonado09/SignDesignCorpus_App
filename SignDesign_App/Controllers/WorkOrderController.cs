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
using System.Globalization;

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

        // Contructor **************************************************************************************************************
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

        // Index **************************************************************************************************************
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
            var users = GetUsersInRole("USER").Result;
            var supervisors = GetUsersInRole("SUPERVISOR").Result;
            var admins = GetUsersInRole("ADMIN").Result;
            ViewData["maintenanceSectionRequestors"] = users.Concat(supervisors).Concat(admins).Distinct();
            ViewData["maintenanceSectionApprovers"] = supervisors;
            ViewData["areaEngineers"] = GetUsersInRole("ENGINEER").Result;
            ViewData["districtApprovers"] = admins;
            //ViewData["currentUserId"] = CheckCurrentUserRole("SUPERVISOR").Result;

            return View();
        }

        //  **************************************************************************************************************
        public async Task<string> CheckCurrentUserRole(string role)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var roles = _userManager.GetRolesAsync(currentUser).Result;
            if (roles[0] == role)
                return currentUser.Id;
            else
                return null;
        }

        // Create **************************************************************************************************************
        [HttpPost]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request, [FromBody] WorkOrderHelperModel workOrder)
        {
            int workOrderId = _workOrderRepository.Create(workOrder);
            workOrder.Id = workOrderId;
            _workOrderRepository.DisposeDBObjects();
            return Json(new[] { workOrder }.ToDataSourceResult(request, ModelState));
        }

        // Read all work orders (main grid) *****************************************************************************************
        public async Task<IActionResult> Read([DataSourceRequest] DataSourceRequest request)
        {
            string result = _workOrderRepository.Read();            
            IQueryable<WorkOrderViewModel> workOrders = JsonSerializer.Deserialize<List<WorkOrderViewModel>>(result).AsQueryable();
            _workOrderRepository.DisposeDBObjects();

            //filter work orders by logged in user's maintennace section id
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            string currentUserRole = _userManager.GetRolesAsync(currentUser).Result[0];
            if (currentUserRole == "USER" || currentUserRole == "SUPERVISOR")
                workOrders = workOrders.Where(x => x.MaterialRequestedById == currentUser.MaintenanceSectionId);
            else if(currentUserRole == "ENGINEER") //only show work orders that are in AREA ENGINEER status
                workOrders = workOrders.Where(x => x.Status == "AREA ENGINEER");
            DataSourceResult dsResult = workOrders.ToDataSourceResult(request);            
            return Json(dsResult);
        }

        // Update **************************************************************************************************************
        [HttpPost]
        public IActionResult Update([DataSourceRequest] DataSourceRequest request, [FromBody] WorkOrderHelperModel workOrder)
        {
            _workOrderRepository.Update(workOrder, (int)workOrder.Id);
            _workOrderRepository.DisposeDBObjects();
            return Json(new[] { workOrder }.ToDataSourceResult(request, ModelState));
        }

        // Delete **************************************************************************************************************
        [AcceptVerbs("Post")]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, WorkOrderViewModel workOrder)
        {
            _workOrderRepository.Delete((int)workOrder.Id);
            _workOrderRepository.DisposeDBObjects();
            return Json(new[] { workOrder }.ToDataSourceResult(request, ModelState));
        }

        // Read info of a specific work order (inside work order form) ************************************************************************
        public JsonResult ReadWorkOrders(int id)
        {
            string result = _workOrderRepository.ReadWorkOrders(id);
            _workOrderRepository.DisposeDBObjects();
            return Json(result);
        }

        // Get the parameters (fields) values from a specific maintenance section **************************************************
        public JsonResult ReadMaintenanceSectionParameters(int? id)
        {
            string result = _maintenanceSectionRepository.ReadMaintenanceSectionParameters(id);
            _maintenanceSectionRepository.DisposeDBObjects();
            return Json(result);
        }

        // Read projects **********************************************************************************************************
        public IEnumerable<Project> GetProjects()
        {
            string result = _projectRepository.Read();
            IEnumerable<Project> projects = JsonSerializer.Deserialize<List<Project>>(result).AsEnumerable();
            _projectRepository.DisposeDBObjects();
            return projects;
        }

        // Read Res Types **************************************************************************************************************
        public IEnumerable<ResType> GetResTypes()
        {
            string result = _resTypeRepository.Read();
            IEnumerable<ResType> resTypes = JsonSerializer.Deserialize<List<ResType>>(result).AsEnumerable();
            _resTypeRepository.DisposeDBObjects();
            return resTypes;
        }

        // Read Tasks **************************************************************************************************************
        public IEnumerable<Task_> GetTasks()
        {
            string result = _taskRepository.Read();
            IEnumerable<Task_> tasks = JsonSerializer.Deserialize<List<Task_>>(result).AsEnumerable();
            _taskRepository.DisposeDBObjects();
            return tasks;
        }

        // Read Activities **************************************************************************************************************
        public IEnumerable<Activity> GetActivities()
        {
            string result = _activityRepository.Read();
            IEnumerable<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(result).AsEnumerable();
            _activityRepository.DisposeDBObjects();
            return activities;
        }

        // Read users from a role ************************************************************************************************
        public async Task<IEnumerable<ApplicationUser>> GetUsersInRole(string role)
        {
            IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(role);
            return users;
        }

        // Read PC Buses **************************************************************************************************************
        public IEnumerable<PCBus> GetPCBuses()
        {
            string result = _pcBusRepository.Read();
            IEnumerable<PCBus> pCBuses = JsonSerializer.Deserialize<List<PCBus>>(result).AsEnumerable();
            _pcBusRepository.DisposeDBObjects();
            return pCBuses;
        }

        // Read Accounts **************************************************************************************************************
        public IEnumerable<Account> GetAccounts()
        {
            string result = _accountRepository.Read();
            IEnumerable<Account> accounts = JsonSerializer.Deserialize<List<Account>>(result).AsEnumerable();
            _accountRepository.DisposeDBObjects();
            return accounts;
        }

        // Read district contacts **************************************************************************************************************
        public IEnumerable<DistrictContact> GetDistrictContacts()
        {
            string result = _districtContactRepository.Read();
            IEnumerable<DistrictContact> districtContacts = JsonSerializer.Deserialize<List<DistrictContact>>(result).AsEnumerable();
            _districtContactRepository.DisposeDBObjects();
            return districtContacts;
        }

        // Read RDCs **************************************************************************************************************
        public IEnumerable<RegionalDistributionCenter> GetRegionalDistributionCenters()
        {
            string result = _regionalDistributionCnterRepository.Read();
            IEnumerable<RegionalDistributionCenter> regionalDistributionCenters = JsonSerializer.Deserialize<List<RegionalDistributionCenter>>(result).AsEnumerable();
            _regionalDistributionCnterRepository.DisposeDBObjects();
            return regionalDistributionCenters;
        }

        // Read departments **************************************************************************************************************
        public IEnumerable<Department> GetDepartments()
        {
            string result = _departmentRepository.Read();
            IEnumerable<Department> departments = JsonSerializer.Deserialize<List<Department>>(result).AsEnumerable();
            _departmentRepository.DisposeDBObjects();
            return departments;
        }

        // Read funds **************************************************************************************************************
        public IEnumerable<Fund> GetFunds()
        {
            string result = _fundRepository.Read();
            IEnumerable<Fund> funds = JsonSerializer.Deserialize<List<Fund>>(result).AsEnumerable();
            _fundRepository.DisposeDBObjects();
            return funds;
        }

        // Read NIGPs **************************************************************************************************************
        public IActionResult GetNIGPs([DataSourceRequest] DataSourceRequest request)
        {
            string result = _nigpRepository.Read();
            IQueryable<NIGP> nigps = JsonSerializer.Deserialize<List<NIGP>>(result).AsQueryable();
            _nigpRepository.DisposeDBObjects();
            return Json(nigps.ToDataSourceResult(request));
        }

        // Read NIGPs (Auxiliary action) *******************************************************************************************************
        public string GetNIGPsAux()
        {
            string result = _nigpRepository.Read();
            _nigpRepository.DisposeDBObjects();
            return result;
        }

        // Read Maintenance sections **************************************************************************************************************
        public IEnumerable<MaintenanceSection> GetMaintenanceSections()
        {
            string result = _maintenanceSectionRepository.Read();
            IEnumerable<MaintenanceSection> maintenanceSections = JsonSerializer.Deserialize<List<MaintenanceSection>>(result).AsEnumerable();
            _maintenanceSectionRepository.DisposeDBObjects();
            return maintenanceSections;
        }

        // Read Maintenance section name **************************************************************************************************************
        public string GetMaintenanceSectionName(int? id)
        {
            string result = _maintenanceSectionRepository.Read(id);
            List<MaintenanceSection> maintenanceSections = JsonSerializer.Deserialize<List<MaintenanceSection>>(result).AsEnumerable().ToList();
            return maintenanceSections[0].Name;
        }

        // Read sign shops **************************************************************************************************************
        public IActionResult GetSignShops([DataSourceRequest] DataSourceRequest request)
        {
            string result = _signShopRepository.Read();
            IQueryable<SignShop> signShops = JsonSerializer.Deserialize<List<SignShop>>(result).AsQueryable();
            _signShopRepository.DisposeDBObjects();
            return Json(signShops.ToDataSourceResult(request));
        }

        // Read years **************************************************************************************************************
        public IEnumerable<Year> GetYears()
        {
            string result = _yearRepository.Read();
            IEnumerable<Year> years = JsonSerializer.Deserialize<List<Year>>(result).AsEnumerable();
            _yearRepository.DisposeDBObjects();
            return years;
        }

        // Sends notification email ***************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> SendMail([FromBody] WorkOrderHelperModel workOrder)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var roles = _userManager.GetRolesAsync(currentUser).Result;
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            string usersList = "";
            string status = workOrder.Status;
            string currentUserRole = _userManager.GetRolesAsync(currentUser).Result[0];
            var htmlSubject = "Sign Request";

            if (status == "CREATED" || status == "APPROVED")
            {
                applicationUsers = GetUsersInRole("SUPERVISOR").Result.ToList();
                var maintenanceSectionId = currentUser.MaintenanceSectionId;
                if (currentUserRole == "ADMIN")
                    maintenanceSectionId = workOrder.MaterialRequestedById;
                applicationUsers = applicationUsers
                        .Where(user => user.MaintenanceSectionId == maintenanceSectionId)
                        .ToList();
                foreach (ApplicationUser user in applicationUsers)
                {
                    usersList += user.Email + ";";                    
                }
                if (usersList.Length > 0)
                    usersList = usersList.Remove(usersList.Length - 1, 1);
            }
            else if(status == "REQUESTED")
            {
                usersList = "CRP_Sign_Design@txdot.gov";
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                string maintenanceSectionName = textInfo.ToTitleCase(GetMaintenanceSectionName(currentUser.MaintenanceSectionId).ToLower());
                htmlSubject = htmlSubject + " from " + maintenanceSectionName;
            }
            htmlSubject = htmlSubject + " for work order #" + workOrder.Id;                    
            string htmlMessage = "<h2>A new sign request has been updated</h2>" +
                "<h4>Please login to your account to see updated request. To login, click <a href='https://signdesign.azurewebsites.net/'>here</a>.</h4>";

            await _eMailSender.SendEmailAsync(usersList, htmlSubject , htmlMessage);
            return View("Index");
        }

        // Generate the PDF *****************************************************************************************************
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

        // Saves the sign image that was uploaded with the kendo upload control ***************************************************
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

        // Removes the sign image that was uploaded with the kendo upload control *************************************************
        public ActionResult RemoveUploadedImage(string[] fileNames)
        {
            //// The parameter of the Remove action must be called "fileNames".

            //if (fileNames != null)
            //{
            //    foreach (var fullName in fileNames)
            //    {
            //        var fileName = Path.GetFileName(fullName);
            //        var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "signs", fileName);

            //        // TODO: Verify user permissions.

            //        if (System.IO.File.Exists(physicalPath))
            //        {
            //            System.IO.File.Delete(physicalPath);
            //        }
            //    }
            //}

            // Return an empty string to signify success.
            return Content("");
        }

        // Saves the installed sign image that was uploaded with the kendo upload control ******************************************
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

        // Removes the sign image that was uploaded with the kendo upload control **********************************************************
        public ActionResult RemoveUploadedInstalledImage(string[] fileNames)
        {
            //// The parameter of the Remove action must be called "fileNames".

            //if (fileNames != null)
            //{
            //    foreach (var fullName in fileNames)
            //    {
            //        var fileName = Path.GetFileName(fullName);
            //        var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "installed_signs", fileName);

            //        // TODO: Verify user permissions.

            //        if (System.IO.File.Exists(physicalPath))
            //        {
            //            System.IO.File.Delete(physicalPath);
            //        }
            //    }
            //}

            // Return an empty string to signify success.
            return Content("");
        }

        // Saves the attachment that was uploaded with the kendo upload control *****************************************************************
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

        // Removes the attachment that was uploaded with the kendo upload control ****************************************************************
        public ActionResult RemoveUploadedAttachment(string[] fileNames)
        {
            //// The parameter of the Remove action must be called "fileNames".

            //if (fileNames != null)
            //{
            //    foreach (var fullName in fileNames)
            //    {
            //        var fileName = Path.GetFileName(fullName);
            //        var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "attachments", fileName);

            //        // TODO: Verify user permissions.

            //        if (System.IO.File.Exists(physicalPath))
            //        {
            //            System.IO.File.Delete(physicalPath);
            //        }
            //    }
            //}

            // Return an empty string to signify success.
            return Content("");
        }
    }
}
