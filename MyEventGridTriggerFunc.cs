// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public class EventGridTriggerType
    {
        [SqlOutput("dbo.ToDo", connectionStringSetting: "SqlConnectionString")]
        public ToDoItem ToDoItem { get; set; }
    }
    public class Test {

      public string Type {get; set;} = string.Empty;
      public string Msg {get; set;} = string.Empty;

  }
    public class MyEventGridTriggerFunc
    {
        private readonly ILogger<MyEventGridTriggerFunc> _logger;

        public MyEventGridTriggerFunc(ILogger<MyEventGridTriggerFunc> logger)
        {
            _logger = logger;
        }

        [Function(nameof(MyEventGridTriggerFunc))]
        public EventGridTriggerType Run([EventGridTrigger] CloudEvent cloudEvent)
        {
            var toDoItem = new ToDoItem();
            toDoItem.Id = Guid.NewGuid();

            Test data = JsonConvert.DeserializeObject<Test>(cloudEvent.Data.ToString());
            toDoItem.title = data.Type;
            // set Url from env variable ToDoUri
            toDoItem.url = data.Msg;
            if (toDoItem.completed == null)
            {
                toDoItem.completed = false;
            }
            _logger.LogInformation($@"Data: {cloudEvent.Data.ToString()},
                                      DataContentType: {cloudEvent.DataContentType ?? "null"},
                                      DataSchema: {cloudEvent.DataSchema ?? "null"},
                                      ExtensionAttributes: {JsonConvert.SerializeObject(cloudEvent.ExtensionAttributes)},
                                      Id: {cloudEvent.Id},
                                      Source: {cloudEvent.Source},
                                      Subject: {cloudEvent.Subject},
                                      Time: {cloudEvent.Time},
                                      Type: {cloudEvent.Type}");

            return new EventGridTriggerType{
                ToDoItem = toDoItem
            };
        }
    }
}
