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
		CancellationToken cancellationToken = new())
	{
		workflow.Id = Guid.NewGuid().ToString();
		var persistable = workflow.ToPersistable();
		_fsql.Insert(persistable).ExecuteIdentity();
		return Task.FromResult(workflow.Id);
	}

	public Task PersistWorkflow(WorkflowInstance workflow,
		CancellationToken cancellationToken = new())
	{
		_fsql.Transaction(() =>
		{
			//加载原来的
			var existingEntity = _fsql.Select<TWorkflow>().Where(x => x.InstanceId == workflow.Id).First();
			//转换
			var persistable = workflow.ToPersistable(existingEntity);
			//删除原来的
			if (existingEntity != null)
			{
				_fsql.Delete<TWorkflow>().Where(x => x.PersistenceId == existingEntity.PersistenceId)
					.ExecuteAffrows();
			}

			//新增插入
			_fsql.Insert(persistable).ExecuteAffrows();
		});
		return Task.CompletedTask;
	}

	public Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt,
		CancellationToken cancellationToken = new())
	{
		var now = asAt.ToUniversalTime().Ticks;
		var ids = _fsql.Select<TWorkflow>()
			.Where(x => x.NextExecution.HasValue && (x.NextExecution <= now) &&
			            (x.Status == (int)WorkflowStatus.Runnable))
			.ToList(x => x.InstanceId);
		return Task.FromResult<IEnumerable<string>>(ids);
	}

	public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type,
		DateTime? createdFrom, DateTime? createdTo, int skip,
		int take)
	{
		var query = _fsql.Select<TWorkflow>();
		if (status.HasValue)
			query = query.Where(x => x.Status == (int)status.Value);

		if (!String.IsNullOrEmpty(type))
			query = query.Where(x => x.WorkflowDefinitionId == type);

		if (createdFrom.HasValue)
			query = query.Where(x => x.CreateTime >= createdFrom.Value);

		if (createdTo.HasValue)
			query = query.Where(x => x.CreateTime <= createdTo.Value);

		var dbitems = await query.Skip(skip).Take(take).ToListAsync();
		return dbitems.Select(item => item.ToWorkflowInstance()).ToList();
	}

	public async Task<WorkflowInstance> GetWorkflowInstance(string Id,
		CancellationToken cancellationToken = new())
	{
		var dbitem =await _fsql.Select<TWorkflow>().Where(x => x.InstanceId == Id).FirstAsync(cancellationToken);
		if (dbitem == null)
			return null;
		return dbitem.ToWorkflowInstance();
	}

	public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids,
		CancellationToken cancellationToken = new())
	{
		if (ids == null)
			return new List<WorkflowInstance>();
		var dbitems =await _fsql.Select<TWorkflow>().Where(x => ids.Contains(x.InstanceId)).ToListAsync(cancellationToken);
		return dbitems.Select(item => item.ToWorkflowInstance()).ToList();
	}

	public Task<string> CreateEventSubscription(EventSubscription subscription,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task TerminateSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetSubscription(string eventSubscriptionId,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<EventSubscription> GetFirstOpenSubscription(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<bool> SetSubscriptionToken(string eventSubscriptionId, string token, string workerId, DateTime expiry,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task ClearSubscriptionToken(string eventSubscriptionId, string token,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateEvent(Event newEvent, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<Event> GetEvent(string id, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task MarkEventProcessed(string id, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task MarkEventUnprocessed(string id, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public Task ScheduleCommand(ScheduledCommand command)
	{
		throw new NotImplementedException();
	}

	public Task ProcessCommands(DateTimeOffset asOf, Func<ScheduledCommand, Task> action,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public bool SupportsScheduledCommands { get; } = false;

	public Task PersistErrors(IEnumerable<ExecutionError> errors,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void EnsureStoreExists()
	{
		Log.Info("EnsureStoreExists");
	}
}