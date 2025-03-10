using System;
using System.Threading.Tasks;
using JUMS.Domain.Entities;
using JUMS.EnrollmentService.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JUMS.Infrastructure.Messaging
{
    // Your existing interface, unchanged
    public interface IRabbitMQPublisher
    {
        Task PublishCourseCreatedAsync(Course course);
    }

    // New MassTransit-based implementation
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RabbitMQPublisher> _logger;

        public RabbitMQPublisher(IPublishEndpoint publishEndpoint, ILogger<RabbitMQPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishCourseCreatedAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            try
            {
                // Create a message DTO to publish
                var message = new CourseCreatedMessage
                {
                    Id = course.Id,
                    Title = course.Title,
                    StartDate = course.StartDate,
                    EndDate = course.EndDate,
                    TeacherId = course.TeacherId
                };

                // Publish using MassTransit’s IPublishEndpoint
                await _publishEndpoint.Publish(message);

                _logger.LogInformation("Published course created message: {@Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing course created message.");
                throw;
            }
        }
    }
}