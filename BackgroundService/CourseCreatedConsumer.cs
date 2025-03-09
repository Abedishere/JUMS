using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using JUMS.EnrollmentService.Data;
using JUMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JUMS.EnrollmentService.Messaging
{
    public class CourseCreatedConsumer : BackgroundService
    {
        private readonly ILogger<CourseCreatedConsumer> _logger;
        private readonly IConfiguration _configuration;
        private readonly EnrollmentContext _context;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName;

        public CourseCreatedConsumer(
            ILogger<CourseCreatedConsumer> logger,
            IConfiguration configuration,
            EnrollmentContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;

            var host = _configuration["RabbitMQ:Host"];
            var queueName = _configuration["RabbitMQ:QueueName"];
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(queueName))
                throw new Exception("RabbitMQ configuration is missing or incomplete.");

            _queueName = queueName;

            // Create a connection & channel
            var factory = new ConnectionFactory() { HostName = host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the queue (idempotent)
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += new Action<object, object>(async (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message: {Message}", message);

                try
                {
                    var courseData = JsonSerializer.Deserialize<CourseCreatedMessage>(message);
                    if (courseData != null)
                    {
                        // Process & store the new course in the enrollment DB
                        await ProcessCourseCreatedAsync(courseData);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message: {Message}", message);
                }
            });

            // Start consuming messages
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessCourseCreatedAsync(CourseCreatedMessage courseData)
        {
            // Check if this course already exists in the enrollment DB
            var existingCourse = await _context.Courses.FindAsync(courseData.Id);
            if (existingCourse != null)
            {
                _logger.LogInformation("Course {Id} already exists. Skipping.", courseData.Id);
                return;
            }

            // Create a new Course entity from the message
            var newCourse = new Course(
                courseData.Id,
                courseData.Title,
                courseData.StartDate,
                courseData.EndDate,
                courseData.TeacherId);

            // Insert into the Enrollment DB
            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New course created in Enrollment DB: {Id} - {Title}",
                newCourse.Id, newCourse.Title);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    // Simple DTO for deserializing "course.created" messages
    public class CourseCreatedMessage
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid TeacherId { get; set; }
    }
}
