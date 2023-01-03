using fineyun.wcs.common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fineyun.wcs.http;

public class AnnotationsFilter : IActionFilter
{
	public void OnActionExecuted(ActionExecutedContext context)
	{

	}

	public void OnActionExecuting(ActionExecutingContext context)
	{
		if (!context.ModelState.IsValid)
		{
			var message = context.ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
			context.Result = new JsonResult(RspCommon.Fail(-1, message));
		}
	}
}