using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Snackis.Components;
using Snackis.Components.Account;
using Snackis.Data;
using Snackis.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Skriver ut detaljerad information om fel i Blazor Server, vilket är användbart under utveckling.
// kommentarer ut efter problemet är löst, och ta bort DetailedErrors = true i produktion, eftersom det kan läcka känslig information.
builder.Services.AddServerSideBlazor()
.AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var connectionString = builder.Configuration.GetConnectionString("AzureConnection") ?? throw new InvalidOperationException("Connection string 'AzureConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDiscussionRepository, DiscussionRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IWallPostRepository, WallPostRepository>();
builder.Services.AddSingleton<IFileStorage>(new LocalFileStorage(builder.Environment.WebRootPath));

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<DiscussionService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<FeedService>();

builder.Services.AddScoped<UnreadMessagesState>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient("BlogApi", client =>
{
    //client.BaseAddress = new Uri("https://localhost:5001/api/blogs");
    client.BaseAddress = new Uri("https://blazorsnackis-c8fpgjged8daatdu.swedencentral-01.azurewebsites.net/");
});

builder.Services.AddControllers();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Seedar en admin om det inte finns någon
var app = builder.Build();
await DbSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

// Ser till att filer via wwwroot hanteras korrekt, t.ex. bilder och CSS-filer.
app.UseStaticFiles();

// API (controllers)
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();