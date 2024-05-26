using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Azure;
using System;
using System.Threading.Tasks;

namespace Company.Function
{
    public class alert_poc1_func
    {
        private readonly ILogger<alert_poc1_func> _logger;

        public alert_poc1_func(ILogger<alert_poc1_func> logger)
        {
            _logger = logger;
        }

        [Function("alert_poc1_func")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("Starting function execution");

                string workspaceId = "26efe585-44c3-43a6-b660-fcb4989a1427";
                
                var client = new LogsQueryClient(new DefaultAzureCredential());

                Response<LogsQueryResult> result = await client.QueryWorkspaceAsync(
                    workspaceId,
                    "union * | where OperationName contains \"alert_poc1_func\"",
                    new QueryTimeRange(TimeSpan.FromDays(1)));

                LogsTable table = result.Value.Table;

                _logger.LogInformation("Query executed successfully, rows: {RowCount}", table.Rows.Count);

                // Log data of the first two rows
                int rowsToLog = Math.Min(table.Rows.Count, 2);
                for (int i = 0; i < rowsToLog; i++)
                {
                    var row = table.Rows[i];
                    string rowData = string.Join(", ", row.Select(cell => cell.ToString()));
                    _logger.LogInformation("Row {RowIndex}: {RowData}", i + 1, rowData);
                }

                return new OkObjectResult(new { RowCount = table.Rows.Count });
            }
            catch (CredentialUnavailableException credEx)
            {
                _logger.LogError(credEx, "Credential issue: {Message}", credEx.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (RequestFailedException reqEx)
            {
                _logger.LogError(reqEx, "Request failed: {Message}", reqEx.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while querying logs");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
