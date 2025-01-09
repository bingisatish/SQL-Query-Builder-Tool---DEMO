using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SqlBuilderTool.Data;
using SqlBuilderTool.Models;
using System.Data;

namespace SqlBuilderTool.Services
{
    public interface ISqlScriptService
    {
        Task<IEnumerable<SqlScript>> GetScriptsAsync();
        IEnumerable<SqlScript> GetAllScripts();
        Task<SqlScript?> GetScriptByIdAsync(int id);
    }

    public class SqlScriptService : ISqlScriptService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SqlScriptService> _logger;

        public SqlScriptService(
            ApplicationDbContext context,
            ILogger<SqlScriptService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SqlScript>> GetScriptsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching scripts asynchronously");
                return await _context.SqlScripts
                    .FromSqlRaw("EXEC GetAllSqlFileData")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scripts asynchronously");
                throw;
            }
        }

        public IEnumerable<SqlScript> GetAllScripts()
        {
            try
            {
                _logger.LogInformation("Fetching all scripts");
                return _context.SqlScripts
                    .FromSqlRaw("EXEC GetAllSqlFileData")
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all scripts");
                throw;
            }
        }

        public async Task<SqlScript?> GetScriptByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching script with ID: {id}");
            
                var script = await _context.SqlScripts
                    .FromSqlRaw("EXEC GetSqlScriptById @Id", 
                        new SqlParameter("@Id", SqlDbType.Int) { Value = id })
                    .FirstOrDefaultAsync();

                if (script == null)
                {
                    _logger.LogWarning($"No script found with ID: {id}");
                    return null;
                }
                
                 _logger.LogInformation($"Successfully retrieved script with ID: {id}");
                return script;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving script with ID: {id}");
                throw;
            }
        }
    }
}
