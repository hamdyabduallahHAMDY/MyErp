using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Security.Claims;

namespace MyErp.Core.Services
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            var requestId = Guid.NewGuid().ToString();
            var user = context.User?.Identity?.Name ?? "Anonymous";
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NoUserId";
            var method = context.Request.Method;
            var path = context.Request.Path;
            var ip = context.Connection.RemoteIpAddress?.ToString();

            Logger.Logs.Log(
                $"[REQ START]  User:[{user}] UserId:[{userId}] IP:[{ip}] {method} {path}"
            );

            try
            {
                await _next(context);

                sw.Stop();

                Logger.Logs.Log(
                    $"[REQ END] User:[{user}] Status:[{context.Response.StatusCode}] Time:[{sw.ElapsedMilliseconds}ms] {method} {path}"
                );

                if (sw.ElapsedMilliseconds > 3000)
                {
                    Logger.Logs.Log(
                        $"[SLOW REQUEST] User:[{user}] Time:[{sw.ElapsedMilliseconds}ms] User:[{user}] {method} {path}"
                    );
                }
            }
            catch (Exception ex)
            {
                sw.Stop();

                Logger.Logs.Log(
                    $"[REQ ERROR] User:[{user}] Time:[{sw.ElapsedMilliseconds}ms] User:[{user}] {method} {path} Error:[{ex}]"
                );

                throw;
            }
        }
    }
}
