using SqlBuilderTool.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SqlBuilderTool.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
            ILogger<ApplicationDbContext> logger)
            : base(options)
        {
            _logger = logger;
        }

        public DbSet<SqlScript> SqlScripts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SqlScript>(entity =>
            {
                entity.HasNoKey();
                
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.BPMS).HasColumnName("BPMS");
                entity.Property(e => e.ClientName).HasColumnName("ClientName");
                entity.Property(e => e.ReportType).HasColumnName("ReportType");
                entity.Property(e => e.Script).HasColumnName("Script");
            });
        }

        public async Task<List<SqlScript>> GetSqlScriptsAsync()
        {
            try
            {
                var scripts = await SqlScripts
                    .FromSqlRaw("EXEC GetAllSqlFileDatas")
                    .ToListAsync();

                if (!scripts.Any())
                {
                    _logger.LogInformation("No SQL scripts found in the database");
                    return new List<SqlScript>();
                }

                return scripts;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while fetching SQL scripts");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching SQL scripts");
                throw;
            }
        }

        public async Task<SqlScript?> GetSqlScriptByIdAsync(int id)
        {
            try
            {
                var script = await SqlScripts
                    .FromSqlRaw("EXEC GetSqlFileDataById @Id={0}", id)
                    .FirstOrDefaultAsync();

                if (script == null)
                {
                    _logger.LogInformation($"SQL script with ID {id} not found");
                    return null;
                }

                return script;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching SQL script with ID: {Id}", id);
                throw;
            }
        }

        public List<SqlScript> GetSqlScripts()
        {
            try
            {
                {
                    _logger.LogInformation("No SQL scripts found in the database");
                    return new List<SqlScript>();
                }                
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while fetching SQL scripts");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching SQL scripts");
                throw;
            }
        }

        public SqlScript? GetSqlScriptById(int id)
        {
            try
            {
                var script = SqlScripts
                    .FromSqlRaw("EXEC GetSqlFileDataById @Id={0}", id)
                    .FirstOrDefault();

                if (script == null)
                {
                    _logger.LogInformation($"SQL script with ID {id} not found");
                    return null;
                }

                return script;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching SQL script with ID: {Id}", id);
                throw;
            }
        }
    }
}
