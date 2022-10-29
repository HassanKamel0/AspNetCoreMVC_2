using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WoodyMovie.Data;
using WoodyMovie.Mail;
using WoodyMovie.Repository;
using WoodyMovie.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization(op=>op.ResourcesPath="Resources");
builder.Services.AddMvc().AddViewLocalization(op => op.ResourcesPath = "Resources").AddDataAnnotationsLocalization();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddTransient<IMailHelper, MailHelper>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddLocalization();
builder.Services.AddAuthentication()
    .AddFacebook(options => 
    {
        options.AppId = "2011994002343682";
        options.AppSecret = "cece386d5dc90a64435aa5b933116df4";
    })
    .AddGoogle(options =>
    {
        options.ClientId = "2011994002343682";
        options.ClientSecret = "cece386d5dc90a64435aa5b933116df4";
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
var supportedCulture = new[] { "ar-SA", "en-US" };
app.UseRequestLocalization(r => {
    r.AddSupportedUICultures(supportedCulture);
    r.AddSupportedCultures(supportedCulture);
    r.SetDefaultCulture("en-US");
});
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
