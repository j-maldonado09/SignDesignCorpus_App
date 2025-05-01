using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SignDesignCorpus;
using SignDesign_App.ViewModels;
//using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using SignDesign_App.Services;
using SignDesign_App.Models;
using System.Text.Json;

namespace SignDesign_App.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IEmailSender _emailSender;
        private readonly EMailSender _emailSender;
        private readonly IMaintenanceSectionRepository _maintenanceSectionRepository;

        //public AccountController(UserManager<IdentityUser> userManager,
        //                         SignInManager<IdentityUser> signInManager,
        //                         RoleManager<IdentityRole> roleManager,
        //                         IEmailSender emailSender)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //    _roleManager = roleManager;
        //    _emailSender = emailSender;
        //}

        public IdentityController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 EMailSender emailSender, IMaintenanceSectionRepository maintenanceSectionRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _maintenanceSectionRepository = maintenanceSectionRepository;
        }

        // ***************************************************************************************************
        //                                      Display list of users.
        // ***************************************************************************************************
        //[HttpGet]
        [Authorize(Roles = "SUPERADMIN")]
        public IActionResult Index()
        {
            IQueryable<ApplicationUser> users = _userManager.Users.OrderBy(u => u.Email);            
            return View(users);
        }

        // ***************************************************************************************************
        //                                  Register a new user (HttpGet).
        // ***************************************************************************************************
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["maintenanceSections"] = GetMaintenanceSections();
            return View();
        }

        // ***************************************************************************************************
        //                                  Register a new user (HttpPost).
        // ***************************************************************************************************
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // No se permite utilizar "superadmin" como parte de la dirección de correo electrónico.
                if (model.Email.Split('@')[0].ToLower().Contains("superadmin"))
                {
                    ModelState.AddModelError("", "Invalid email address");
                    ModelState.AddModelError("", "Please use other email address");

                    return View(model);
                }

                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    ContactFirstName = model.FirstName,
                    ContactLastName = model.LastName,
                    ContactOrganizationType = model.OrganizationType,
                    MaintenanceSectionId = model.MaintenanceSectionId
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, "GUEST");

                    if (result.Succeeded)
                    {
                        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        string confirmationLink = Url.Action("ConfirmEmail", "Identity", new { token, email = user.Email }, Request.Scheme);
                        string htmlMessage = "<h2>Please click on the link below to confirm your email address</h2>" +
                            "<h3><a href=" + confirmationLink + ">Click here to confirm your account</a></h3><hr>" +
                            "<h3>Or use the following link:</h3>" +
                            "<h3><a href=" + confirmationLink + ">" + confirmationLink + "</a></h3>";

                        try
                        {
                            await _emailSender.SendEmailAsync(user.Email, "Account registration confirmation", htmlMessage);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "An error has occurred sending the confirmation email");
                            ModelState.AddModelError("", ex.Message);

                            return View(model);
                        }

                        return RedirectToAction("SuccessRegistration", "Identity");
                    }
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ViewData["maintenanceSections"] = GetMaintenanceSections();
            }

                return View(model);
        }

        // ***************************************************************************************************
        //                      Validate user versus the confirmation token.
        // ***************************************************************************************************
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            // User with specified email not found.
            if (user == null)
            {
                ModelState.AddModelError("", $"The user email {email} does not exist to verify/validate");
                return View();
            }

            // Confirm user versus token.
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        // ***************************************************************************************************
        //          Display a success registration view to the user after registration.
        // ***************************************************************************************************
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SuccessRegistration()
        {
            return View();
        }

        // ***************************************************************************************************
        //                                      User Login (HttpGet).
        // ***************************************************************************************************
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await ValidateDefaultUsersAndGroups();
            return View();
        }

        // ***************************************************************************************************
        //                                      User Login (HttpPost).
        // ***************************************************************************************************
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login credentials are not valid.");
                    ModelState.AddModelError("", "Please try again.");
                }
            }

            return View(model);
        }

        // ***************************************************************************************************
        // Validate that the following default gropus exist:
        // SUPERADMIN, ADMIN, SUPERVISOR, DATACLERKS, USER, VISITORS
        //
        // Validate that default user SUPERADMIN exists.
        // If it does not then it is added and made part of SUPERADMIN group.
        // ***************************************************************************************************
        public async Task<IActionResult> ValidateDefaultUsersAndGroups()
        {
            await CreateRole("SUPERADMIN");         // SUPERADMIN
            await CreateRole("ADMIN");              // District contacts that approve 
            await CreateRole("ENGINEER");           // Area engineer that approves certian requests
            await CreateRole("SUPERVISOR");         // Maintenance Section contacts that approve
            await CreateRole("USER");               // Maintenance Section contacts that request
            await CreateRole("GUEST");              // Default role (no privileges)
            await CreateSuperadminUser();

            return null;
        }

        // ***************************************************************************************************
        //                                      Create new role.
        // ***************************************************************************************************
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var result = _roleManager.FindByNameAsync(roleName);

            if (result.Result == null)
            {
                IdentityRole identityRole = new IdentityRole();
                identityRole.Name = roleName;
                await _roleManager.CreateAsync(identityRole);
            }

            return null;
        }

        // ***************************************************************************************************
        //                    Create SUPERADMIN user and make it part of SUPERADMIN group.
        // ***************************************************************************************************
        public async Task<IActionResult> CreateSuperadminUser()
        {
            if (_userManager.GetUsersInRoleAsync("SUPERADMIN").Result.Count == 0)
            {
                ApplicationUser user = new ApplicationUser { UserName = "superadmin@superadmin.com", Email = "superadmin@superadmin.com", EmailConfirmed = true, ContactFirstName = "SuperAdmin",
                                                            ContactLastName = "SuperAdmin", ContactOrganizationType = "SuperAdmin"};
                await _userManager.CreateAsync(user, "SuperAdm1n#*.");
                await _userManager.AddToRoleAsync(user, "SUPERADMIN");
            }
            
            return null;
        }

        // ***************************************************************************************************
        //                                          User logout.
        // ***************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ***************************************************************************************************
        //                                  Edit User (HttpGet)
        // ***************************************************************************************************
        [HttpGet]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> EditUser(string id)
        {
            ViewData["maintenanceSections"] = GetMaintenanceSections();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ModelState.AddModelError("", $"User ID not found = {id}");
                return View();
            }

            var role = await _userManager.GetRolesAsync(user);
            if (role.Count == 0)
            {
                ModelState.AddModelError("", $"The user email {user.Email} does not belong to any group.");
                return View();
            }

            List<IdentityRole> unsortedGroups = _roleManager.Roles.ToList();
            List<IdentityRole> sortedGroups = new List<IdentityRole>();
            SetGroupsOrder(unsortedGroups, sortedGroups, "GUEST");
            SetGroupsOrder(unsortedGroups, sortedGroups, "USER");
            SetGroupsOrder(unsortedGroups, sortedGroups, "SUPERVISOR");
            SetGroupsOrder(unsortedGroups, sortedGroups, "ENGINEER");
            SetGroupsOrder(unsortedGroups, sortedGroups, "ADMIN");
            SetGroupsOrder(unsortedGroups, sortedGroups, "SUPERADMIN");

            EditUserRoleViewModel editUserRoleViewModel = new EditUserRoleViewModel
            {
                FirstName = user.ContactFirstName,
                LastName = user.ContactLastName,
                OrganizationType = user.ContactOrganizationType,
                Email = user.Email,
                Group = role[0],
                PreviousGroup = role[0],
                //Groups = _roleManager.Roles.OrderBy(r => r.Name)
                Groups = sortedGroups.AsQueryable(),
                MaintenanceSectionId = user.MaintenanceSectionId
            };

            return View(editUserRoleViewModel);
        }

        // ***************************************************************************************************
        //                                       Set order of the groups
        // ***************************************************************************************************
        private void SetGroupsOrder(List<IdentityRole> unsortedGroups, List<IdentityRole> sortedGroups, string group)
        {
            foreach (var item in unsortedGroups)
            {
                if (item.Name == group)
                {
                    sortedGroups.Add(item);
                }
            }
        }

        // ***************************************************************************************************
        //                                          Edit User (HttpPost)
        // ***************************************************************************************************
        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> EditUser(EditUserRoleViewModel model)
        {
            // Se busca y valida el usuario con el ID en cuestión.
            var user = await _userManager.FindByIdAsync(model.id);
            if (user == null)
            {
                ModelState.AddModelError("", $"User ID not found = {model.id}");
                return View();
            }

            // Se borra el usuario de rol al que pertenece.
            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, model.PreviousGroup);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Se agrega el usuario al nuevo rol.
            result = await _userManager.AddToRoleAsync(user, model.Group);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            user.ContactFirstName = model.FirstName;
            user.ContactLastName = model.LastName;
            user.ContactOrganizationType = model.OrganizationType;
            user.MaintenanceSectionId = model.MaintenanceSectionId;
            user.Email = model.Email;
            user.UserName = model.Email;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Identity");
        }

        // ***************************************************************************************************
        //                                  Delete User (HttpGet)
        // ***************************************************************************************************
        [HttpGet]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ModelState.AddModelError("", $"User ID not found = {id}");
                return View();
            }

            var role = await _userManager.GetRolesAsync(user);
            if (role.Count == 0)
            {
                ModelState.AddModelError("", $"The user email {user.Email} does not belong to any group.");
                return View();
            }

            EditUserRoleViewModel editUserRoleViewModel = new EditUserRoleViewModel
            {
                FirstName = user.ContactFirstName,
                LastName = user.ContactLastName,
                OrganizationType = user.ContactOrganizationType,
                Email = user.Email,
                Group = role[0],
                PreviousGroup = role[0],
                Groups = _roleManager.Roles.OrderBy(r => r.Name)
            };

            return View(editUserRoleViewModel);
        }

        // ***************************************************************************************************
        //                                          Delete User (HttpPost)
        // ***************************************************************************************************
        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteUser(EditUserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.id);
            if (user == null)
            {
                ModelState.AddModelError("", $"User ID not found = {model.id}");
                return View();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Index", "Identity");
        }

        // ***************************************************************************************************
        //                                          Forgot Password (HttpGet)
        // ***************************************************************************************************
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        // ***************************************************************************************************
        //                                          Forgot Password (HttpPost)
        // ***************************************************************************************************
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string passwordResetLink = Url.Action("ResetPassword", "Identity", new { email = model.Email, token = passwordResetToken }, Request.Scheme);
                    string htmlMessage = "<h2>Please click in the link below to reset your password</h2>" +
                            "<h3><a href=" + passwordResetLink + ">Click here to reset your password</a></h3><hr>" +
                            "<h3>Or use the following link:</h3>" +
                            "<h3><a href=" + passwordResetLink + ">" + passwordResetLink + "</a></h3>";

                    try
                    {
                        await _emailSender.SendEmailAsync(user.Email, "Reset password", htmlMessage);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "There was an error sending the password reset email");
                        ModelState.AddModelError("", ex.Message);
                        return View(model);
                    }
                }
                
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        // ***************************************************************************************************
        //                                     Reset Password (HttpGet)
        // ***************************************************************************************************
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "The token entered to reset the password is invalid ");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        // ***************************************************************************************************
        //                                     Reset Password (HttpPost)
        // ***************************************************************************************************
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(model.Email);
                
                if (user == null)
                {
                    ModelState.AddModelError("", $"User ID not found = {model.ToString}");
                    return View(model);
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    return View("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // ***************************************************************************************************
        //                                     Get Maintenance Sections
        // ***************************************************************************************************
        private IEnumerable<MaintenanceSection> GetMaintenanceSections()
        {
            string result = _maintenanceSectionRepository.Read();
            IEnumerable<MaintenanceSection> maintenanceSections = JsonSerializer.Deserialize<List<MaintenanceSection>>(result).AsEnumerable();
            _maintenanceSectionRepository.DisposeDBObjects();
            return maintenanceSections;
        }

        // ***************************************************************************************************
        //                                     Exception Handler.
        // ***************************************************************************************************
        //public IActionResult Exception(ExceptionViewModel model)
        //{
        //    return View(model);
        //}

    }
}
