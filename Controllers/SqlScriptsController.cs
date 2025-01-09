using Microsoft.AspNetCore.Mvc;
using SqlBuilderTool.Models;
using SqlBuilderTool.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SqlBuilderTool.Services;

namespace SqlBuilderTool.Controllers
{
    public class SqlScriptsController : Controller
    {
        private readonly ISqlScriptService _sqlScriptService;
        private readonly ILogger<SqlScriptsController> _logger;
        private readonly string _connectionString;

        public SqlScriptsController(
            ISqlScriptService sqlScriptService,
            ILogger<SqlScriptsController> logger,
            IConfiguration configuration)
        {
            _sqlScriptService = sqlScriptService ?? throw new ArgumentNullException(nameof(sqlScriptService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // Default action for the view
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Accessing Index page");
                var scripts = _sqlScriptService.GetAllScripts();
                return View(scripts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while accessing Index page");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetScripts()
        {
            try
            {
                _logger.LogInformation("Getting scripts from database");

                var scripts = await _sqlScriptService.GetScriptsAsync();

                _logger.LogInformation($"Retrieved {scripts.Count()} scripts");

                var result = new
                {
                    data = scripts,
                    recordsTotal = scripts.Count(),
                    recordsFiltered = scripts.Count()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scripts from database");
                return Json(new
                {
                    data = new List<object>(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    error = "An error occurred while fetching the data"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetScriptDetails(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("GetAllSqlFileData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if ((int)reader["ID"] == id)
                                {
                                    return Json(new
                                    {
                                        success = true,
                                        id = reader["ID"],
                                        name = reader["ReportType"],
                                        script = reader["Script"]
                                    });
                                }
                            }
                        }
                    }
                }

                return Json(new { success = false, message = "Script not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting details for script with ID: {id}");
                return Json(new
                {
                    success = false,
                    message = "Error fetching script details",
                    error = ex.Message
                });
            }
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestStoredProc()
        {
            try
            {
                _logger.LogInformation("Testing stored procedure connection");
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("GetAllSqlFileData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var hasRows = reader.HasRows;
                            var count = 0;
                            while (await reader.ReadAsync())
                            {
                                count++;
                            }
                            _logger.LogInformation($"Test successful. HasRows: {hasRows}, Count: {count}");
                            return Json(new { success = true, hasRows, count });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing stored procedure");
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace // Only in development
                });
            }
        }

        // Add error handling action
        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
