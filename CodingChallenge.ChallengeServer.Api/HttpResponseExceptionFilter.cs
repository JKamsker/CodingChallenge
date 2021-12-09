
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Web.Http;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is null)
        {
            return;
        }

        if (context.Exception is HttpResponseException httpResponseException)
        {
            context.Result = new ObjectResult(httpResponseException.Message)
            {
                StatusCode = (int)httpResponseException.Response.StatusCode
            };

            context.ExceptionHandled = true;
        }
        //else
        //{
        //    context.Result = new ObjectResult("Unknown error")
        //    {
        //        StatusCode = 500
        //    };

        //    context.ExceptionHandled = true;
        //}
    }
}