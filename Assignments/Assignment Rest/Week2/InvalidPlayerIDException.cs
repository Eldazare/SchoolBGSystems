using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Week2
{
    public class InvalidPlayerIDException : Exception
    {
        public InvalidPlayerIDException(string message) : base(message){

        }
    }


    public class InvalidPlayerIDFilter : ExceptionFilterAttribute{
        public override void OnException(ExceptionContext context){
            if(context.Exception is InvalidPlayerIDException){
                context.Result = new BadRequestObjectResult("playerID not found in database: "+context.Exception.Message);
            }
        }
    }
}