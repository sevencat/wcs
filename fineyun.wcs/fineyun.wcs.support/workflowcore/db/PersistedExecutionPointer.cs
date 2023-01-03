using FreeSql.DataAnnotations;
using WorkflowCore.Models;

namespace fineyun.wcs.support.workflowcore.db;

[Table(Name = "workflowcore_executionpointer")]
[Index("IX_ExecutionPointer_WorkflowId", "WorkflowId", false)]
public class PersistedExecutionPointer
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	[Column(IsNullable = false)]
	public bool Active { get; set; }

	[Column(IsNullable = false)]
	public int RetryCount { get; set; }

	public DateTime? EndTime { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string EventData { get; set; }

	[Column(StringLength = 100)]
	public string EventKey { get; set; }

	[Column(StringLength = 100)]
	public string EventName { get; set; }

	[Column(IsNullable = false)]
	public bool EventPublished { get; set; }

	[Column(StringLength = 50)]
	public string Id { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string PersistenceData { get; set; }

	public DateTime? SleepUntil { get; set; }

	public DateTime? StartTime { get; set; }

	[Column(IsNullable = false)]
	public int StepId { get; set; }

	[Column(StringLength = 100)]
	public string StepName { get; set; }

	[Column(IsNullable = false)]

	public long WorkflowId { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string Children { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string ContextItem { get; set; }

	[Column(StringLength = 100)]
	public string PredecessorId { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string Outcome { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string Scope { get; set; }

	[Column(IsNullable = false)]
	public int Status { get; set; } = (int)PointerStatus.Legacy;


	[Column(IsIgnore = true)]
	public List<PersistedExtensionAttribute> ExtensionAttributes { get; set; } = new();
}