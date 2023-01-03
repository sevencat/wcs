using FreeSql.DataAnnotations;

namespace fineyun.wcs.support.workflowcore.db;

[Table(Name = "workflowcore_scheduledcommand")]
[Index("IX_ScheduledCommand_CommandName_Data", "CommandName,Data", true)]
[Index("IX_ScheduledCommand_ExecuteTime", "ExecuteTime", false)]
public class PersistedScheduledCommand
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	[Column(StringLength = 200)]
	public string CommandName { get; set; }

	[Column(StringLength = 500)]
	public string Data { get; set; }

	[Column(IsNullable = false)]
	public long ExecuteTime { get; set; }
}