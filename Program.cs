using Hangfire;
using Hangfire.PostgreSql;
using JUMS.Domain.Infrastructure;
using JUMS.Domain.Interfaces;
using JUMS.Infrastructure.BackgroundJobs;
using JUMS.Infrastructure.Localization;
using JUMS.Infrastructure.Messaging;
using JUMS.Infrastructure.MultiTenancy;
using JUMS.Infrastructure.Persistence;
using JUMS.Infrastructure.Persistence.Repositories;
using JUMS.Infrastructure.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Infrastructure;
using JUMS.Infrastructure.Messaging;


var builder = WebApplication.CreateBuilder(args);

// Register Memory Cache
builder.Services.AddMemoryCache();

// Add Localization services (assuming your resource files are in "Resources")
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add controllers and API versioning
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register IHttpContextAccessor and TenantProvider for multi-tenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Replace the default IModelCacheKeyFactory with our tenant-aware version.
builder.Services.AddSingleton<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();

// Configure EF Core to use PostgreSQL with tenant support
builder.Services.AddDbContext<JUMSContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});

// Configure Hangfire to use PostgreSQL storage
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

// Register the shared localizer
builder.Services.AddSingleton<ISharedLocalizer, SharedLocalizer>();

// Register repository implementations
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();

// Register Notification service
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Background Job Service
builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();

// Register RabbitMQ Publisher
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

var app = builder.Build();

// Configure localization middleware
var supportedCultures = new[] { "en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

// Use Hangfire Dashboard (available at /hangfire)
app.UseHangfireDashboard("/hangfire");

// Register recurring background jobs on startup
using (var scope = app.Services.CreateScope())
{
    var backgroundJobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
    backgroundJobService.RegisterRecurringJobs();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
