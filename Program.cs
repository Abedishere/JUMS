using Microsoft.EntityFrameworkCore;
using JUMS.EnrollmentService.Data;
using JUMS.EnrollmentService.Messaging;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1) Configure EF Core to use PostgreSQL
builder.Services.AddDbContext<EnrollmentContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 2) Add controllers & swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3) Register the RabbitMQ consumer as a hosted service
builder.Services.AddHostedService<CourseCreatedConsumer>();

var app = builder.Build();

// Enable swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map controller endpoints
app.MapControllers();

// Run the microservice
app.Run();