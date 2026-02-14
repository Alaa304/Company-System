using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RouteG01.BLL;
using RouteG01.BLL.Services.AttachementService;
using RouteG01.BLL.Services.Classess;
using RouteG01.BLL.Services.Interfaces;
using RouteG01.DAL.Data.Contexts;
using RouteG01.DAL.Models.Shared;
using RouteG01.DAL.Repositories.Classess;
using RouteG01.DAL.Repositories.Interfaces;

namespace RouteG01.Pl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            #region Configure Services
            builder.Services.AddDbContext<ApplicationDbContext>(options=>
            {
                //options.UseSqlServer(builder.Configuration["ConnectionStrings.DefaultConnection"]);
                //options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseLazyLoadingProxies();
            });//2: Register To Service in  DI Container
            builder.Services.AddScoped<IDepartmentRepository,DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService,DepartmentService>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IAttachementService, AttachementService>();
            builder.Services.AddScoped<IunitOfWork, UnitOfWork>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            
            #endregion


           var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // ✅ الصح في .NET 8

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

        }
    }
}
