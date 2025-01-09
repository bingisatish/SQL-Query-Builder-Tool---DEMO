using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SqlBuilderTool.Models;
using SqlBuilderTool.Services;

namespace SqlBuilderTool.Data
{
    public class SqlScriptRepository : ISqlScriptService
    {
        private readonly ApplicationDbContext _context;     
        private readonly ILogger<SqlScriptRepository> _logger;

        public SqlScriptRepository(ApplicationDbContext context, 
                ILogger<SqlScriptRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<SqlScript> GetAllScripts()
        {
            try
            {
                // Using FromSqlRaw to execute the stored procedure
                var scripts = _context.SqlScripts
                    .FromSqlRaw("EXEC GetAllSqlFileData")
                    .ToList();

                if (!scripts.Any())
                {
                    _logger.LogInformation("No scripts found");
                    return Enumerable.Empty<SqlScript>();
                }

                return scripts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve SQL scripts");
                throw new ApplicationException("Failed to retrieve SQL scripts", ex);
            }
        }

        public SqlScript GetScriptById(int id)
        {
            try
            {
                var script = _context.SqlScripts
                    .FirstOrDefault(s => s.Id == id);

                if (script == null)
                {
                    _logger.LogInformation($"Script with ID {id} not found");
                    return null;
                }

                return new SqlScript
                {
                    Id = script.Id,
                    BPMS = script.BPMS,
                    ClientName = script.ClientName,
                    ReportType = script.ReportType,
                    Script = script.Script
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve SQL script with ID {id}");
                throw new ApplicationException($"Failed to retrieve SQL script with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<SqlScript>> GetScriptsAsync()
        {
            try
            {
                // Using FromSqlRaw to execute the stored procedure asynchronously
                var scripts = await _context.SqlScripts
                    .FromSqlRaw("EXEC GetAllSqlFileData")
                    .ToListAsync();

                if (!scripts.Any())
                {
                    _logger.LogInformation("No scripts found");
                    return Enumerable.Empty<SqlScript>();
                }

                return scripts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve SQL scripts");
                throw new ApplicationException("Failed to retrieve SQL scripts", ex);
            }
        }

        public async Task<SqlScript> GetScriptByIdAsync(int id)
        {
            try
            {
                var script = await _context.SqlScripts
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (script == null)
                {
                    _logger.LogInformation($"Script with ID {id} not found");
                    return null;
                }

                return new SqlScript
                {
                    Id = script.Id,
                    BPMS = script.BPMS,
                    ClientName = script.ClientName,
                    ReportType = script.ReportType,
                    Script = script.Script
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve SQL script with ID {id}");
                throw new ApplicationException($"Failed to retrieve SQL script with ID {id}", ex);
            }
        }
    }
}
