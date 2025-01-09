using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlBuilderTool.Models
{
    public class SqlScript
{
    public int Id { get; set; }
    
    [Required]
    [Column("BPMS")]
    public string BPMS { get; set; } = string.Empty;
    
    [Required]
    [Column("Client Name")]
    public string ClientName { get; set; } = string.Empty;
    
    [Required]
    [Column("Report Type")]
    public string ReportType { get; set; } = string.Empty;
    
    [Required]
    [Column("Script")]
    public string Script { get; set; } = string.Empty;
}
    public class ExecuteQueryModel
    {
        public string Query { get; set; } = string.Empty;
    }
}
