using System.Web.Http.Results;
using EO.Base;
using Habr.Common.DTOs.ResponceDTOs;
using Habr.Common.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Habr.Common.Exceptions
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILoggerManager _logger;

        public ExceptionFilter(ILoggerManager logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            int statusCode = 500;

            switch (context.Exception)
            {
                case NotFoundException:
                    {
                        statusCode = ((NotFoundException)context.Exception).ErrorCode;
                        break;
                    }
                case BadRequestException:
                    {
                        statusCode = ((BadRequestException)context.Exception).ErrorCode;
                        break;
                    }
                case BusinessException:
                    {
                        statusCode = ((BusinessException)context.Exception).ErrorCode;
                        break;
                    }
                case AuthenticationException:
                    {
                        statusCode = ((AuthenticationException)context.Exception).ErrorCode;
                        break;
                    }
                case AuthorizationException:
                    {
                        statusCode = ((AuthorizationException)context.Exception).ErrorCode;
                        break;
                    }
                case ValidationException:
                    {
                        statusCode = ((ValidationException)context.Exception).ErrorCode;
                        break;
                    }
            }

            var exception = context.Exception;

            if (statusCode != 500)
            {        
                var response = new ClientErrorResponse(statusCode.ToString(), exception.Message);

                context.HttpContext.Response.StatusCode = statusCode;
                context.Result = new JsonResult(response);

                _logger.LogWarning($"{statusCode} {exception.Message}");
            }
            else
            {      
                var response = new ServerErrorResponse(statusCode.ToString(), exception.Message, exception.StackTrace);

                context.HttpContext.Response.StatusCode = statusCode;
                context.Result = new JsonResult(response);

                _logger.LogError($"{statusCode} {exception.Message} {exception.StackTrace}");
            }

            base.OnException(context);
        }
    }
}
