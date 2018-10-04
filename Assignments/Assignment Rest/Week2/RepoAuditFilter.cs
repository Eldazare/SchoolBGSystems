using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Week2
{
    public class RepoAuditFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //Console.WriteLine("Written");
            var ipAdd = context.HttpContext.Connection.LocalIpAddress;
            string ret = "A request from ip address "+ipAdd+" to ban player started at "+DateTime.UtcNow;
            var repo = (IRepository)context.HttpContext.RequestServices.GetService(typeof(IRepository));
            repo.WriteLog(ret);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Console.WriteLine("Executing...");
            var ipAdd = context.HttpContext.Connection.LocalIpAddress;
            string ret = "A request from ip address "+ipAdd+" to ban player ended at "+DateTime.UtcNow;
            var repo = (IRepository)context.HttpContext.RequestServices.GetService(typeof(IRepository));
            repo.WriteLog(ret);
        }
    }
}