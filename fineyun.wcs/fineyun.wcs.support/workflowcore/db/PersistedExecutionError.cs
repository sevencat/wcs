using FreeSql.DataAnnotations;

namespace fineyun.wcs.support.workflowcore.db;

[Table(Name = "workflowcore_executionerror")]
public class PersistedExecutionError
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	[Column(IsNullable = false)]
	public DateTime ErrorTime { get; set; }

	[Column(StringLength = 100)]
	public string ExecutionPointerId { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string Message { get; set; }

	[Column(StringLength = 100)]
	public string WorkflowId { get; set; }
}