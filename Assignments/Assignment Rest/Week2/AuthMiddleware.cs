using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Week2
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _conf;
        public AuthMiddleware(RequestDelegate next, IConfiguration conf){
            _next = next;
            _conf = conf;
        }
        //[InvalidOperationExecptionFilter]
        public async Task Invoke(HttpContext context){
            try{
                if (context.Request.Headers.ContainsKey("x-api-key")){
                    if (context.Request.Headers["x-api-key"] == _conf["x-api-key"]){
                        var claim = new Claim(ClaimTypes.Role, "User");
                        ((ClaimsIdentity)context.User.Identity).AddClaim(claim);
                        await _next.Invoke(context);
                        // Without any claims, the authentication will throw bad error :F
                    } else if (context.Request.Headers["x-api-key"] == _conf["x-api-key-admin"]){
                        var claim = new Claim(ClaimTypes.Role, "Admin");
                        ((ClaimsIdentity)context.User.Identity).AddClaim(claim);
                        await _next.Invoke(context);
                    } else {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Forbidden: Incorrect api key.");
                        return;
                    }
                } else {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Bad request: No api key detected");
                    return;
                }
            } catch (InvalidOperationException ex){
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden? (InvalidOperationException): "+ex.Message);
            }
        }


        public class InvalidOperationExecptionFilter : ExceptionFilterAttribute{
            public override void OnException(ExceptionContext context){
            if(context.Exception is InvalidOperationException){
                context.Result = new ForbidResult("Forbidden?: "+context.Exception.Message);
            }
        }
        }
    }
}