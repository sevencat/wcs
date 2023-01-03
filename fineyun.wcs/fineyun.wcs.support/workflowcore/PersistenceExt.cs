using fineyun.wcs.common.ext;
using fineyun.wcs.support.workflowcore.db;
using Newtonsoft.Json;
using WorkflowCore.Models;

namespace fineyun.wcs.support.workflowcore;

public static class PersistenceExt
{
	private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
		{ TypeNameHandling = TypeNameHandling.All };

	public static TWorkflow ToPersistable(this WorkflowInstance instance, TWorkflow persistable = null)
	{
		persistable ??= new TWorkflow();

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
				var ret = new TExecutionPointer
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
					persistedAttr = new TExtensionAttribute();
					persistedEP.ExtensionAttributes.Add(persistedAttr);
				}

				persistedAttr.AttributeKey = attr.Key;
				persistedAttr.AttributeValue = JsonConvert.SerializeObject(attr.Value, SerializerSettings);
			}
		}

		return persistable;
	}

	public static WorkflowInstance ToWorkflowInstance(this TWorkflow instance)
	{
		WorkflowInstance result = new WorkflowInstance();
		result.Data = JsonConvert.DeserializeObject(instance.Data, SerializerSettings);
		result.Description = instance.Description;
		result.Reference = instance.Reference;
		result.Id = instance.InstanceId.ToString();
		result.NextExecution = instance.NextExecution;
		result.Version = instance.Version;
		result.WorkflowDefinitionId = instance.WorkflowDefinitionId;
		result.Status = (WorkflowStatus)instance.Status;
		result.CreateTime = DateTime.SpecifyKind(instance.CreateTime, DateTimeKind.Utc);
		if (instance.CompleteTime.HasValue)
			result.CompleteTime = DateTime.SpecifyKind(instance.CompleteTime.Value, DateTimeKind.Utc);

		result.ExecutionPointers = new ExecutionPointerCollection(instance.ExecutionPointers.Count + 8);

		foreach (var ep in instance.ExecutionPointers.Values)
		{
			var pointer = new ExecutionPointer
			{
				Id = ep.Id,
				StepId = ep.StepId,
				Active = ep.Active
			};

			if (ep.SleepUntil.HasValue)
				pointer.SleepUntil = DateTime.SpecifyKind(ep.SleepUntil.Value, DateTimeKind.Utc);

			pointer.PersistenceData =
				JsonConvert.DeserializeObject(ep.PersistenceData ?? string.Empty, SerializerSettings);

			if (ep.StartTime.HasValue)
				pointer.StartTime = DateTime.SpecifyKind(ep.StartTime.Value, DateTimeKind.Utc);

			if (ep.EndTime.HasValue)
				pointer.EndTime = DateTime.SpecifyKind(ep.EndTime.Value, DateTimeKind.Utc);

			pointer.StepName = ep.StepName;

			pointer.RetryCount = ep.RetryCount;
			pointer.PredecessorId = ep.PredecessorId;
			pointer.ContextItem = JsonConvert.DeserializeObject(ep.ContextItem ?? string.Empty, SerializerSettings);

			if (!string.IsNullOrEmpty(ep.Children))
				pointer.Children = ep.Children.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

			pointer.EventName = ep.EventName;
			pointer.EventKey = ep.EventKey;
			pointer.EventPublished = ep.EventPublished;
			pointer.EventData = JsonConvert.DeserializeObject(ep.EventData ?? string.Empty, SerializerSettings);
			pointer.Outcome = JsonConvert.DeserializeObject(ep.Outcome ?? string.Empty, SerializerSettings);
			pointer.Status =(PointerStatus) ep.Status;

			if (!string.IsNullOrEmpty(ep.Scope))
				pointer.Scope = new List<string>(ep.Scope.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

			foreach (var attr in ep.ExtensionAttributes)
			{
				pointer.ExtensionAttributes[attr.AttributeKey] =
					JsonConvert.DeserializeObject(attr.AttributeValue, SerializerSettings);
			}

			result.ExecutionPointers.Add(pointer);
		}

		return result;
	}
}