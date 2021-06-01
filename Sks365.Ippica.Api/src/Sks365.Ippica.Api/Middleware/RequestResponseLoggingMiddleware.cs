using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Sks365.Ippica.Common.Config.Abstraction;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.SessionTracker.Client;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sks365.Ippica.Api.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IAppSettings _appSettings;
        private readonly ISessionTracker _sessionTracker;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IAppSettings appSettings,
                                                ISessionTracker sessionTracker)
        {
            _logger = logger;
            _next = next;
            _appSettings = appSettings;
            _sessionTracker = sessionTracker;
        }

        public async Task Invoke(HttpContext context)
        {

            //First, get the incoming request
            var jsonRequest = await GetJson(context.Request);
            var requestDate = DateTime.Now;
            var endpoint = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            var httpMethod = context.Request.Method;
            var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();

            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                try
                {
                    //...and use that for the temporary response body
                    context.Response.Body = responseBody;

                    //Continue down the Middleware pipeline, eventually returning to this class
                    await _next(context);

                    //Format the response from the server
                    var jsonResponse = await GetJson(context.Response);
                    var responseDate = DateTime.Now;
                    var httpStatusCode = context.Response.StatusCode;
                    var isSwagger = context.Request.Path.Value.Contains("swagger", StringComparison.OrdinalIgnoreCase);
                    var isSettleBet = context.Request.Path.HasValue &&
                                      context.Request.Path.Value.Length >= 9 &&
                                      context.Request.Path.Value.Right(9).Equals("SettleBet", StringComparison.OrdinalIgnoreCase);
                    var isCancelBet = context.Request.Path.HasValue &&
                                      context.Request.Path.Value.Length >= 9 &&
                                      context.Request.Path.Value.Right(9).Equals("CancelBet", StringComparison.OrdinalIgnoreCase);
                    var isItaly = context.Request.Host.HasValue &&
                                  context.Request.Host.Value.Length >= 2 &&
                                  context.Request.Host.Value.Right(2).Equals("it", StringComparison.OrdinalIgnoreCase);

                    if (!isSwagger)
                    {
                        var ticketId = string.Empty;
                        var externalId = string.Empty;
                        var session = string.Empty;
                        var userAccount = string.Empty;

                        if (!string.IsNullOrEmpty(jsonRequest))
                        {
                            var json = JObject.Parse(jsonRequest);

                            if (isSettleBet || isCancelBet)
                            {
                                externalId = TryGetValue(json, "ticket_id"); //MST sends ADM ID in the field TicketId
                            }
                            else
                            {
                                ticketId = TryGetValue(json, "ticket_id");
                                externalId = TryGetValue(json, "external_id");
                            }

                            session = TryGetValue(json, "session");
                            var userId = GetUserIdFromSession(session);
                            userAccount = (userId > 0) ? userId.ToString() : string.Empty;

                            if (string.IsNullOrEmpty(userAccount))
                                userAccount = TryGetValue(json, "user_account");
                        }

                        var logRequest = new LogRequest()
                        {
                            HttpMethod = httpMethod,
                            Endpoint = endpoint,
                            RequestBody = jsonRequest,
                            ResponseBody = jsonResponse,
                            HttpStatusCode = httpStatusCode,
                            Session = session,
                            TicketId = ticketId,
                            ExternalId = externalId,
                            UserAccount = userAccount,
                            RemoteIpAddress = remoteIpAddress,
                            RequestDate = requestDate,
                            ResponseDate = responseDate
                        };

                        InsertLogRequest(logRequest);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    //throw;
                }

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }


        private string TryGetValue(JObject input, string key)
        {
            foreach (var property in input.Properties())
            {
                if (property.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    return property.Value.Value<string>();

                if (property.Value.Type == JTokenType.Object)
                    return TryGetValue((JObject)property.Value, key);
            }

            return string.Empty;
        }


        private async Task<string> GetJson(HttpRequest request)
        {
            var body = request.Body;

            //Enable buffering so body can be read multiple times
            request.EnableBuffering();

            var jsonRequest = string.Empty;
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                jsonRequest = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            return jsonRequest;
        }

        private async Task<string> GetJson(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string jsonResponse = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return jsonResponse;
        }

        private string GetUserNameFromSession(string session)
        {
            var sessionData = (_sessionTracker.GetSession(session)).Result;
            return sessionData?.Username;
        }

        private string GetBookmakerIdFromSession(string session)
        {
            var sessionData = (_sessionTracker.GetSession(session)).Result;
            return sessionData?.BookmakerId.ToString();
        }

        private void InsertLogRequest(LogRequest logRequest)
        {
            var mstConnectionString = _appSettings.ConnectionStrings.Mst;

            using (var unitOfWork = new MstUnitOfWork(new RepositoryFactory(new DataContext(mstConnectionString))))
            {
                unitOfWork.BetRepository.InsertLogRequest(logRequest);
            }
        }


        private int GetUserIdFromSession(string session)
        {
            var userId = 0;

            if (!string.IsNullOrEmpty(session))
            {
                var userName = GetUserNameFromSession(session);
                var bookmakerIdStr = GetBookmakerIdFromSession(session);

                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(bookmakerIdStr))
                {
                    var bookmakerId = (BookmakerEnum)Enum.Parse(typeof(BookmakerEnum), bookmakerIdStr);

                    var isbetsConnectionString = _appSettings.ConnectionStrings.Isbets;
                    using (var unitOfWork = new IsbetsUnitOfWork(new RepositoryFactory(new DataContext(isbetsConnectionString))))
                    {
                        var user = unitOfWork.UserRepository.GetUser(userName, bookmakerId);
                        userId = user?.UserId ?? 0;
                    }
                }
            }

            return userId;
        }
    }
}
