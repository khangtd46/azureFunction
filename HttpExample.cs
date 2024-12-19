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
    public class ToDoItem
    {   
        public Guid Id { get; set; }
        public int? order { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public bool? completed { get; set; }
    }
    public class OutputType
    {
        [SqlOutput("dbo.ToDo", connectionStringSetting: "SqlConnectionString")]
        public ToDoItem ToDoItem { get; set; }
        public HttpResponseData HttpResponse { get; set; }
    }
    public class HttpExample
    {
        private readonly ILogger<HttpExample> _logger;

        public HttpExample(ILogger<HttpExample> logger)
        {
            _logger = logger;
        }

        [Function("HttpExample")]
        public static async Task<OutputType> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PostFunction")] HttpRequestData req,
                FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("PostToDo");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ToDoItem toDoItem = JsonConvert.DeserializeObject<ToDoItem>(requestBody);

            // generate a new id for the todo item
            toDoItem.Id = Guid.NewGuid();
            toDoItem.title = "asdsadsada";
            // set Url from env variable ToDoUri
            toDoItem.url = toDoItem.Id.ToString();

            // if completed is not provided, default to false
            if (toDoItem.completed == null)
            {
                toDoItem.completed = false;
            }
            
            return new OutputType()
            {
                ToDoItem = toDoItem,
                HttpResponse = req.CreateResponse(System.Net.HttpStatusCode.Created)
            };
        }
    }
}
