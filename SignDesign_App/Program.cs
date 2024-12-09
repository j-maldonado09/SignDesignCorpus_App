using SignDesign_App.Services;
using SignDesign_App.Data;
using DataTier;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignDesign_App.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options => {
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true;

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Identity/Login");
builder.Services.AddControllersWithViews()
                // Maintain property names during serialization. See:
                // https://github.com/aspnet/Announcements/issues/194
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
// Add Kendo UI services to the services container"
builder.Services.AddKendo();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Microsoft.Data.SqlClient.SqlConnection>();
builder.Services.AddTransient<DataTier.Tools>();
builder.Services.AddTransient<SignDesignCorpus.IWorkOrderRepository, SignDesignCorpus.WorkOrderRepository>();
builder.Services.AddTransient<SignDesignCorpus.IProjectRepository, SignDesignCorpus.ProjectRepository>();
builder.Services.AddTransient<SignDesignCorpus.IMaintenanceSectionContactRepository, SignDesignCorpus.MaintenanceSectionContactRepository>();
builder.Services.AddTransient<SignDesignCorpus.IResTypeRepository, SignDesignCorpus.ResTypeRepository>();
builder.Services.AddTransient<SignDesignCorpus.IActivityRepository, SignDesignCorpus.ActivityRepository>();
builder.Services.AddTransient<SignDesignCorpus.ITaskRepository, SignDesignCorpus.TaskRepository>();
builder.Services.AddTransient<SignDesignCorpus.IPCBusRepository, SignDesignCorpus.PCBusRepository>();
builder.Services.AddTransient<SignDesignCorpus.IAccountRepositoy, SignDesignCorpus.AccountRepository>();
builder.Services.AddTransient<SignDesignCorpus.IDistrictContactRepository, SignDesignCorpus.DistrictContactRepository>();
builder.Services.AddTransient<SignDesignCorpus.IRegionalDistributionCenterRepository, SignDesignCorpus.RegionalDistributionCenterRepository>();
builder.Services.AddTransient<SignDesignCorpus.IDepartmentRepository, SignDesignCorpus.DepartmentRepository>();
builder.Services.AddTransient<SignDesignCorpus.IFundRepository, SignDesignCorpus.FundRepository>();
builder.Services.AddTransient<SignDesignCorpus.INIGPRepository, SignDesignCorpus.NIGPRepository>();
builder.Services.AddTransient<SignDesignCorpus.IMaintenanceSectionRepository, SignDesignCorpus.MaintenanceSectionRepository>();
builder.Services.AddTransient<SignDesignCorpus.ISignShopRepository, SignDesignCorpus.SignShopRepository>();
builder.Services.AddTransient<SignDesignCorpus.IYearRepository, SignDesignCorpus.YearRepository>();
builder.Services.AddTransient<SignDesignCorpus.WorkOrderReport>();
builder.Services.AddTransient<DataTier.Interfaces.IUnitOfWork, DataTier.UnitOfWorkSQLServer_MSFT>();
builder.Services.AddTransient<EMailSender>();
builder.Services.AddTransient<ExportPdf>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();