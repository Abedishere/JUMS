using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JUMS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using global::RabbitMQ.Client; // Force the correct namespace

namespace JUMS.Infrastructure.Messaging
{
    public interface IRabbitMQPublisher
    {
        Task PublishCourseCreatedAsync(Course course);
    }

    public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
    {
        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly IConfiguration _configuration;
        private readonly global::RabbitMQ.Client.IConnection _connection;
        private readonly string _queueName;
        private bool _disposed;

        public RabbitMQPublisher(ILogger<RabbitMQPublisher> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Retrieve and validate configuration values
            var host = _configuration["RabbitMQ:Host"];
            if (string.IsNullOrEmpty(host))
                throw new InvalidOperationException("RabbitMQ:Host is not configured.");

            var queueName = _configuration["RabbitMQ:QueueName"];
            if (string.IsNullOrEmpty(queueName))
                throw new InvalidOperationException("RabbitMQ:QueueName is not configured.");

            // Create a new ConnectionFactory using the fully qualified type
            var factory = new global::RabbitMQ.Client.ConnectionFactory()
            {
                HostName = host
                // Add other properties (like Port, UserName, Password) if needed.
            };

            _connection = factory.CreateConnection();
            _queueName = queueName;
        }

        public Task PublishCourseCreatedAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            try
            {
                // Create a new channel per publish call to ensure thread safety.
                using (var channel = _connection.CreateModel())
                {
                    // Declare the queue (idempotent)
                    channel.QueueDeclare(
                        queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var message = JsonSerializer.Serialize(new
                    {
                        course.Id,
                        course.Title,
                        course.StartDate,
                        course.EndDate,
                        course.TeacherId
                    });

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _queueName,
                        basicProperties: null,
                        body: body);

                    _logger.LogInformation("Published course created message: {Message}", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing course created message.");
                throw;
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }
}
