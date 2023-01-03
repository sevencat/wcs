using FreeSql.DataAnnotations;

namespace fineyun.wcs.support.workflowcore.db;

[Table(Name = "workflowcore_event")]
[Index("IX_Event_EventId", "EventId", true)]
[Index("IX_Event_EventTime", "EventTime", false)]
[Index("IX_Event_IsProcessed", "IsProcessed", false)]
[Index("IX_Event_EventName_EventKey", "EventName,EventKey", false)]
public class PersistedEvent
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string EventData { get; set; }

	[Column(IsNullable = false)]
	public Guid EventId { get; set; }

	[Column(StringLength = 200)]
	public string EventName { get; set; }

	[Column(StringLength = 200)]
	public string EventKey { get; set; }

	[Column(IsNullable = false)]
	public DateTime EventTime { get; set; }

	[Column(IsNullable = false)]
	public bool IsProcessed { get; set; }
}