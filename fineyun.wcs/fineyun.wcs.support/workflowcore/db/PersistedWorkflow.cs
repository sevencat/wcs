using FreeSql.DataAnnotations;

namespace fineyun.wcs.support.workflowcore.db;

[Table(Name = "workflowcore_workflow")]
[Index("IX_Workflow_InstanceId", "InstanceId", true)]
[Index("IX_Workflow_NextExecution", "NextExecution", false)]
public class PersistedWorkflow
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	public DateTime? CompleteTime { get; set; }

	[Column(IsNullable = false)]
	public DateTime CreateTime { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string Data { get; set; }

	[Column(StringLength = 500)]
	public string Description { get; set; }

	[Column(StringLength = 64, IsNullable = false)]
	public string InstanceId { get; set; }

	public long? NextExecution { get; set; }

	[Column(IsNullable = false)]
	public int Status { get; set; }

	[Column(IsNullable = false)]
	public int Version { get; set; }

	[Column(StringLength = 200)]
	public string WorkflowDefinitionId { get; set; }

	[Column(StringLength = 200)]
	public string Reference { get; set; }

	[Column(IsIgnore = true)]
	public Dictionary<string, PersistedExecutionPointer> ExecutionPointers { get; set; } = new();
}