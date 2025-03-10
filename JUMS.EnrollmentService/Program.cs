using Microsoft.EntityFrameworkCore;
using JUMS.EnrollmentService.Data;
using JUMS.EnrollmentService.Messaging;
using MassTransit;

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

// 3) Configure MassTransit for RabbitMQ and register the consumer
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CourseCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            // Use the default credentials; adjust as needed.
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(builder.Configuration["RabbitMQ:QueueName"], e =>
        {
            e.ConfigureConsumer<CourseCreatedConsumer>(context);
        });
    });
});

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<CourseCreatedConsumer>();

    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        // Host & queue config
        rabbitCfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        rabbitCfg.ReceiveEndpoint(builder.Configuration["RabbitMQ:QueueName"], e =>
        {
            e.ConfigureConsumer<CourseCreatedConsumer>(context);
        });
    });
});
builder.Services.AddMassTransitHostedService();

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