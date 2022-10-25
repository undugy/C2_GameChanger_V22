using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
namespace Server.MiddleWare;

public class ResultFilter:IResultFilter
{

    private ILogger _logger;

    public ResultFilter(ILogger<ResultFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnResultExecuting(ResultExecutingContext context)
    {
        ObjectResult objectResult = (ObjectResult)context.Result;
        var result = JsonConvert.SerializeObject(objectResult.Value);
        objectResult.Value = result;
     
    }
    public void OnResultExecuted(ResultExecutedContext context)
    {
        
    }

    
}