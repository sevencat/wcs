using fineyun.wcs.support.workflowcore.db;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace fineyun.wcs.support.workflowcore;

public class FreeSqlPersistenceProvider : IPersistenceProvider
{
	private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
	private readonly IFreeSql _fsql;

	public FreeSqlPersistenceProvider(IFreeSql fsql)
	{
		_fsql = fsql;
	}

	public Task<string> CreateNewWorkflow(WorkflowInstance workflow,
		CancellationToken cancellationToken = new CancellationToken())
	{
		workflow.Id = Guid.NewGuid().ToString();
		var persistable = workflow.ToPersistable();
		_fsql.Transaction(() =>
		{

			long workflowId = _fsql.InsertOrUpdate<PersistedWorkflow>().SetSource(persistable).ExecuteAffrows();
			foreach (var ep in persistable.ExecutionPointers.Values)
			{
				ep.WorkflowId = workflowId;
				var epid = _fsql.Insert(ep).ExecuteIdentity();
				foreach (var attr in ep.ExtensionAttributes)
				{
					attr.ExecutionPointerId = epid;
					_fsql.Insert(attr).ExecuteIdentity();
				}
			}
		});
		return Task.FromResult(workflow.Id);
	}

	public Task PersistWorkflow(WorkflowInstance workflow,
		CancellationToken cancellationToken = new CancellationToken())
	{
		_fsql.Transaction(() =>
		{
			//加载原来的
			var existingEntity = _fsql.Select<PersistedWorkflow>().Where(x => x.InstanceId == workflow.Id).First();
			if (existingEntity != null)
			{
				existingEntity.ExecutionPointers = _fsql.Select<PersistedExecutionPointer>()
					.Where(x => x.WorkflowId == existingEntity.PersistenceId)
					.ToDictionary(x => x.Id);
				foreach (var ep in existingEntity.ExecutionPointers.Values)
				{
					ep.ExtensionAttributes = _fsql.Select<PersistedExtensionAttribute>()
						.Where(x => x.ExecutionPointerId == ep.PersistenceId)
						.ToList();
				}
			}
			//转换
			var persistable = workflow.ToPersistable(existingEntity);
			//删除原来的
			if (existingEntity != null)
			{
				foreach (var ep in existingEntity.ExecutionPointers.Values)
				{
					ep.ExtensionAttributes = _fsql.Select<PersistedExtensionAttribute>()
						.Where(x => x.ExecutionPointerId == ep.PersistenceId)
						.ToList();
					_fsql.Delete<PersistedExtensionAttribute>().Where(x => x.ExecutionPointerId == ep.PersistenceId)
						.ExecuteAffrows();
				}

				_fsql.Delete<PersistedExecutionPointer>().Where(x => x.WorkflowId == existingEntity.PersistenceId)
					.ExecuteAffrows();
				_fsql.Delete<PersistedWorkflow>().Where(x => x.PersistenceId == existingEntity.PersistenceId)
					.ExecuteAffrows();
			}
			
			//新增插入
			long workflowId = _fsql.InsertOrUpdate<PersistedWorkflow>().SetSource(persistable).ExecuteAffrows();
			foreach (var ep in persistable.ExecutionPointers.Values)
			{
				ep.WorkflowId = workflowId;
				var epid = _fsql.Insert(ep).ExecuteIdentity();
				foreach (var attr in ep.ExtensionAttributes)
				{
					attr.ExecutionPointerId = epid;
					_fsql.Insert(attr).ExecuteIdentity();
				}
			}
		});
		return Task.CompletedTask;
	}

	public Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type, DateTime? createdFrom, DateTime? createdTo, int skip,
		int take)
	{
		throw new NotImplementedException();
	}

	public Task<WorkflowInstance> GetWorkflowInstance(string Id, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEventSubscription(EventSubscription subscription,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task TerminateSubscription(string eventSubscriptionId, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetSubscription(string eventSubscriptionId, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetFirstOpenSubscription(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<bool> SetSubscriptionToken(string eventSubscriptionId, string token, string workerId, DateTime expiry,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task ClearSubscriptionToken(string eventSubscriptionId, string token,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEvent(Event newEvent, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<Event> GetEvent(string id, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task MarkEventProcessed(string id, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task MarkEventUnprocessed(string id, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public Task ScheduleCommand(ScheduledCommand command)
	{
		throw new NotImplementedException();
	}

	public Task ProcessCommands(DateTimeOffset asOf, Func<ScheduledCommand, Task> action, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public bool SupportsScheduledCommands { get; }
	public Task PersistErrors(IEnumerable<ExecutionError> errors, CancellationToken cancellationToken = new CancellationToken())
	{
		throw new NotImplementedException();
	}

	public void EnsureStoreExists()
	{
		throw new NotImplementedException();
	}
}