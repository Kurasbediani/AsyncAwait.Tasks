using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await UpdateHeadersAsync(context);

        await _next(context);
    }

    async Task UpdateHeadersAsync(HttpContext context)
    {
        string path = context.Request.Path;
        var staticRegTask = _statisticService.RegisterVisitAsync(path);
        var getVisitsCount = _statisticService.GetVisitsCountAsync(path); ;

        Console.WriteLine(staticRegTask.Status); // just for debugging purposes

        await staticRegTask;

        var total = await getVisitsCount;
        context.Response.Headers.Add(
            CustomHttpHeaders.TotalPageVisits,
            total.ToString());
    }
}
