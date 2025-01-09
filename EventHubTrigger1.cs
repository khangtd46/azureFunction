using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class EventHubTrigger1
    {
        private readonly ILogger<EventHubTrigger1> _logger;

        public EventHubTrigger1(ILogger<EventHubTrigger1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(EventHubTrigger1))]
        public void Run([EventHubTrigger("samples-workitems", Connection = "EventHubConnection")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                string result = System.Text.Encoding.UTF8.GetString(@event.Body.Span);
                _logger.LogInformation("Event Body: {body}", result);
                _logger.LogInformation("Event Content-Type: {contentType}", @event.ContentType);
            }
        }
    }
}
