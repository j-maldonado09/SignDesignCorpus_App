using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignDesign_App.Models;
using SignDesignCorpus;
using SignDesignCorpus.ViewModels;
using System.Text.Json;

namespace SignDesign_App.Controllers
{
    public class MaintenanceSectionParametersController : Controller
    {
        private IProjectRepository _projectRepository;
        private IResTypeRepository _resTypeRepository;
        private ITaskRepository _taskRepository;
        private IActivityRepository _activityRepository;
        private IPCBusRepository _pcBusRepository;
        private IAccountRepositoy _accountRepository;
        private IDepartmentRepository _departmentRepository;
        private IFundRepository _fundRepository;
        private IMaintenanceSectionRepository _maintenanceSectionRepository;
        private IWebHostEnvironment _webHostEnvironment;
        private UserManager<ApplicationUser> _userManager;

        public MaintenanceSectionParametersController(IProjectRepository projectRepository, IResTypeRepository resTypeRepository, 
            ITaskRepository taskRepository, IActivityRepository activityRepository, IPCBusRepository pCBusRepository, IAccountRepositoy accountRepositoy,
            IDepartmentRepository departmentRepository, IFundRepository fundRepository, IMaintenanceSectionRepository maintenanceSectionRepository,
            IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _resTypeRepository = resTypeRepository;
            _taskRepository = taskRepository;
            _activityRepository = activityRepository;
            _pcBusRepository = pCBusRepository;
            _accountRepository = accountRepositoy;
            _departmentRepository = departmentRepository;
            _fundRepository = fundRepository;
            _maintenanceSectionRepository = maintenanceSectionRepository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //ViewData["maintenanceSectionParameters"] = GetMaintenanceSection(GetCurrentUserMaintenanceSection().Result);
            ViewData["departments"] = GetDepartments();
            ViewData["accounts"] = GetAccounts();
            ViewData["funds"] = GetFunds();
            ViewData["tasks"] = GetTasks();
            ViewData["pCBuses"] = GetPCBuses();
            ViewData["projects"] = GetProjects();
            ViewData["activities"] = GetActivities();
            ViewData["resTypes"] = GetResTypes();
            return View(GetMaintenanceSection(GetCurrentUserMaintenanceSection().Result));
        }

        [HttpPost]
        public void SaveMaintenanceSectionParameters([FromBody] MaintenanceSectionParametersViewModel maintenanceSectionParameters)
        { 
            _maintenanceSectionRepository.UpdateMaintenanceSectionParameters(maintenanceSectionParameters, maintenanceSectionParameters.Id);
            _maintenanceSectionRepository.DisposeDBObjects();
        }

        public async Task<int?> GetCurrentUserMaintenanceSection()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            return currentUser.MaintenanceSectionId;
        }

        public List<MaintenanceSectionParametersViewModel> GetMaintenanceSection(int? id)
        {
            string result = _maintenanceSectionRepository.ReadMaintenanceSectionParameters(id);
            List<MaintenanceSectionParametersViewModel> maintenanceSectionParameters = JsonSerializer.Deserialize<List<MaintenanceSectionParametersViewModel>>(result);
            return maintenanceSectionParameters;
        }

        public IEnumerable<Project> GetProjects()
        {
            string result = _projectRepository.Read();
            List<Project> projects = JsonSerializer.Deserialize<List<Project>>(result);
            projects.Insert(0, new Project() { Id = -1, Name = "<NONE>" });
            _projectRepository.DisposeDBObjects();
            return projects;
        }

        public IEnumerable<ResType> GetResTypes()
        {
            string result = _resTypeRepository.Read();
            List<ResType> resTypes = JsonSerializer.Deserialize<List<ResType>>(result);
            resTypes.Insert(0, new ResType() { Id = -1, Name = "<NONE>" });
            _resTypeRepository.DisposeDBObjects();
            return resTypes;
        }

        public IEnumerable<Task_> GetTasks()
        {
            string result = _taskRepository.Read();
            List<Task_> tasks = JsonSerializer.Deserialize<List<Task_>>(result);
            tasks.Insert(0, new Task_() { Id = -1, Name = "<NONE>" });
            _taskRepository.DisposeDBObjects();
            return tasks;
        }

        public IEnumerable<Activity> GetActivities()
        {
            string result = _activityRepository.Read();
            List<Activity> activities = JsonSerializer.Deserialize<List<Activity>>(result);
            activities.Insert(0, new Activity() { Id = -1, Name = "<NONE>" });
            _activityRepository.DisposeDBObjects();
            return activities;
        }

        public IEnumerable<PCBus> GetPCBuses()
        {
            string result = _pcBusRepository.Read();
            List<PCBus> pCBuses = JsonSerializer.Deserialize<List<PCBus>>(result);
            pCBuses.Insert(0, new PCBus() { Id = -1, Name = "<NONE>" });
            _pcBusRepository.DisposeDBObjects();
            return pCBuses;
        }

        public IEnumerable<Account> GetAccounts()
        {
            string result = _accountRepository.Read();
            List<Account> accounts = JsonSerializer.Deserialize<List<Account>>(result);
            accounts.Insert(0, new Account() { Id = -1, Name = "<NONE>" });
            _accountRepository.DisposeDBObjects();
            return accounts;
        }

        public IEnumerable<Department> GetDepartments()
        {
            string result = _departmentRepository.Read();
            List<Department> departments = JsonSerializer.Deserialize<List<Department>>(result);
            departments.Insert(0, new Department() {Id=-1, Name="<NONE>"});
            _departmentRepository.DisposeDBObjects();
            return departments;
        }

        public IEnumerable<Fund> GetFunds()
        {
            string result = _fundRepository.Read();
            List<Fund> funds = JsonSerializer.Deserialize<List<Fund>>(result);
            funds.Insert(0, new Fund() { Id = -1, Name = "<NONE>" });
            _fundRepository.DisposeDBObjects();
            return funds;
        }
    }
}
