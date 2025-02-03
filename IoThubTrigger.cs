using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

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
                var data = JsonConvert.DeserializeObject<OeeRawData>(result);
                if (data != null)
                {
                    string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
                    InsertIntoDatabase(connectionString, data);
                }
            }
        }

        public void InsertIntoDatabase(string connectionString, OeeRawData data)
        {
            string query = @"INSERT INTO OptixRawDataDemo1 
                        (LocalTimestamp, PlannedCut, TotalCut, GoodCut, TargetSpeed, ActualSpeed, Uptime, 
                         EmployeeID, MachineID, BatchID, Downtime, DowntimeCode, BadCut, BadCutCode)
                        VALUES (@LocalTimestamp, @PlannedCut, @TotalCut, @GoodCut, @TargetSpeed, @ActualSpeed, @Uptime, 
                                @EmployeeID, @MachineID, @BatchID, @Downtime, @DowntimeCode, @BadCut, @BadCutCode)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LocalTimestamp", (object?)data.LocalTimestamp ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PlannedCut", (object?)data.PlannedCut ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TotalCut", (object?)data.TotalCut ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GoodCut", (object?)data.GoodCut ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TargetSpeed", (object?)data.TargetSpeed ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActualSpeed", (object?)data.ActualSpeed ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Uptime", (object?)data.Uptime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeID", (object?)data.EmployeeID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MachineID", (object?)data.MachineID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BatchID", (object?)data.BatchID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Downtime", (object?)data.Downtime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DowntimeCode", (object?)data.DowntimeCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BadCut", (object?)data.BadCut ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BadCutCode", (object?)data.BadCutCode ?? DBNull.Value); 
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
