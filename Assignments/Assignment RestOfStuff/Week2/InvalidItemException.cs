using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Week2
{
    public class InvalidItemException : Exception
    {
        public InvalidItemException(string message) : base(message){

        }
    }

    public class InvalidItemExceptionFilter : ExceptionFilterAttribute{
        public override void OnException(ExceptionContext context){
            if(context.Exception is InvalidItemException){
                context.Result = new BadRequestObjectResult("Invalid Item detected with message: "+context.Exception.Message);
            }
        }
    }
}