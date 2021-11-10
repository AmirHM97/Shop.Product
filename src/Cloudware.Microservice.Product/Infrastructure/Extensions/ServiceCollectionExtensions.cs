using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Common.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Serialization;

namespace Cloudware.Microservice.Product.Infrastructure.Extensions
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next,
            IHostingEnvironment env,
            ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string message = "Error Occurred!!!";
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            long ClwStatusCode = 500;
            string requestId = context.TraceIdentifier ?? Guid.NewGuid().ToString();
            
            try
            {
                await _next(context);
                //if (context.Response.StatusCode > 299)
                //{
                //  await WriteToResponseWithInputAsync(new ApiResult((int)context.Response.StatusCode, (int)context.Response.StatusCode,requestId,message));
                //}
            }
            catch (AppException exception)
            {
                _logger.LogError(exception, exception.Message);
                httpStatusCode = exception.HttpStatusCode;
                ClwStatusCode = exception.ClwStatusCode;


                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    if (exception.InnerException != null)
                    {
                        dic.Add("InnerException.Exception", exception.InnerException.Message);
                        dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                    }
                    if (exception.AdditionalData != null)
                        dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                    message = JsonConvert.SerializeObject(dic);
                }
                else
                {
                    message = exception.Message;
                }
                // message = exception.Message;
                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                _logger.LogError(exception, exception.Message);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(exception, exception.Message);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
           
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                var appException = exception.InnerException as AppException;
                if (appException == null)
                {

                    if (_env.IsDevelopment())
                    {
                        var dic = new Dictionary<string, string>
                        {
                            ["Exception"] = exception.Message,
                            ["StackTrace"] = exception.StackTrace,
                        };
                        message = JsonConvert.SerializeObject(dic);
                        // ClwStatusCode= ((Cloudware.Utilities.Common.Exceptions.AppException)exception.InnerException).ClwStatusCode;
                    }
                }
                else
                {
                    if (_env.IsDevelopment())
                    {
                        var dic = new Dictionary<string, string>
                        {
                            ["Exception"] = appException.Message,
                            ["StackTrace"] = appException.StackTrace,
                        };
                        ClwStatusCode = appException.ClwStatusCode;
                        httpStatusCode = appException.HttpStatusCode;
                        message = JsonConvert.SerializeObject(dic);
                    }

                }
                await WriteToResponseAsync();
            }

            async Task WriteToResponseAsync()
            {
                if (context.Response.HasStarted)
                    throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

                var result = new ApiResult(ClwStatusCode, (int)httpStatusCode, requestId, message);
                var json = JsonConvert.SerializeObject(result);

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
               
               // response.Write(JsonConvert.SerializeObject(Data, jsonSerializerSettings));
                await context.Response.WriteAsync(json);
            }
            async Task WriteToResponseWithInputAsync(ApiResult apiResult)
            {
                //if (context.Response.HasStarted)
                //  throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");
                var newResponse = new HttpResponseMessage();
               
                var result = apiResult;
                var json = JsonConvert.SerializeObject(result);

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            void SetUnAuthorizeResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                //unauthorized error code
                ClwStatusCode = 500;

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace
                    };
                    if (exception is SecurityTokenExpiredException tokenException)
                        dic.Add("Expires", tokenException.Expires.ToString());

                    message = JsonConvert.SerializeObject(dic);
                }
            }
        }
    }
   
}
