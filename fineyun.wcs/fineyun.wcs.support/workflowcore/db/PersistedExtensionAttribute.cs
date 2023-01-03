using FreeSql.DataAnnotations;

namespace fineyun.wcs.support.workflowcore.db;

[Table(Name = "workflowcore_extensionattribute")]
[Index("IX_ExtensionAttribute_ExecutionPointerId", "ExecutionPointerId", false)]
public class PersistedExtensionAttribute
{
	[Column(IsIdentity = true, IsPrimary = true)]
	public long PersistenceId { get; set; }

	[Column(StringLength = 100)]
	public string AttributeKey { get; set; }

	[Column(DbType = "LONGTEXT")]
	public string AttributeValue { get; set; }

	[Column(IsNullable = false)]
	public long ExecutionPointerId { get; set; }
}