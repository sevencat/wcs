using fineyun.wcs.common.ext;
using fineyun.wcs.support.workflowcore.db;
using Newtonsoft.Json;
using WorkflowCore.Models;

namespace fineyun.wcs.support.workflowcore;

public static class PersistenceExt
{
	private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
		{ TypeNameHandling = TypeNameHandling.All };

	public static PersistedWorkflow ToPersistable(this WorkflowInstance instance,
		PersistedWorkflow persistable = null)
	{
		persistable ??= new PersistedWorkflow();

		persistable.Data = JsonConvert.SerializeObject(instance.Data, SerializerSettings);
		persistable.Description = instance.Description;
		persistable.Reference = instance.Reference;
		persistable.InstanceId = instance.Id;
		persistable.NextExecution = instance.NextExecution;
		persistable.Version = instance.Version;
		persistable.WorkflowDefinitionId = instance.WorkflowDefinitionId;
		persistable.Status = (int)instance.Status;
		persistable.CreateTime = instance.CreateTime;
		persistable.CompleteTime = instance.CompleteTime;

		foreach (var ep in instance.ExecutionPointers)
		{
			var persistedEP = persistable.ExecutionPointers.GetOrAddNew(ep.Id, (id) =>
			{
				var ret = new PersistedExecutionPointer
				{
					Id = ep.Id ?? Guid.NewGuid().ToString()
				};
				return ret;
			});

			persistedEP.StepId = ep.StepId;
			persistedEP.Active = ep.Active;
			persistedEP.SleepUntil = ep.SleepUntil;
			persistedEP.PersistenceData = JsonConvert.SerializeObject(ep.PersistenceData, SerializerSettings);
			persistedEP.StartTime = ep.StartTime;
			persistedEP.EndTime = ep.EndTime;
			persistedEP.StepName = ep.StepName;
			persistedEP.RetryCount = ep.RetryCount;
			persistedEP.PredecessorId = ep.PredecessorId;
			persistedEP.ContextItem = JsonConvert.SerializeObject(ep.ContextItem, SerializerSettings);
			persistedEP.Children = string.Empty;

			foreach (var child in ep.Children)
				persistedEP.Children += child + ";";

			persistedEP.EventName = ep.EventName;
			persistedEP.EventKey = ep.EventKey;
			persistedEP.EventPublished = ep.EventPublished;
			persistedEP.EventData = JsonConvert.SerializeObject(ep.EventData, SerializerSettings);
			persistedEP.Outcome = JsonConvert.SerializeObject(ep.Outcome, SerializerSettings);
			persistedEP.Status = (int)ep.Status;

			persistedEP.Scope = string.Empty;
			foreach (var item in ep.Scope)
				persistedEP.Scope += item + ";";

			foreach (var attr in ep.ExtensionAttributes)
			{
				var persistedAttr = persistedEP.ExtensionAttributes.FirstOrDefault(x => x.AttributeKey == attr.Key);
				if (persistedAttr == null)
				{
					persistedAttr = new PersistedExtensionAttribute();
					persistedEP.ExtensionAttributes.Add(persistedAttr);
				}

				persistedAttr.AttributeKey = attr.Key;
				persistedAttr.AttributeValue = JsonConvert.SerializeObject(attr.Value, SerializerSettings);
			}
		}

		return persistable;
	}
}