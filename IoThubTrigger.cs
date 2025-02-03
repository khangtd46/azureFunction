using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class IoThubTrigger
    {
        private readonly ILogger<IoThubTrigger> _logger;

        public IoThubTrigger(ILogger<IoThubTrigger> logger)
        {
            _logger = logger;
        }

        [Function(nameof(IoThubTrigger))]
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
