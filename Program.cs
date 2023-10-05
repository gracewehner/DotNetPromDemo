using Microsoft.AspNetCore.Http.Extensions;
using Prometheus;
using Prometheus.HttpMetrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMetricServer(options =>
{
    options.Port = 1234;
});

//Metrics.SuppressDefaultMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.

var expiringMetricFactory = Metrics.WithManagedLifetime(expiresAfter: TimeSpan.FromMinutes(15));

app.UseRouting();

app.UseHttpMetrics(options =>
    {
        options.AddCustomLabel("url", context => context.Request.GetDisplayUrl());
        options.InProgress.Gauge = expiringMetricFactory.CreateGauge(
            "demo_http_requests_in_progress",
            "The number of requests currently in progress in the ASP.NET Core pipeline. One series without controller/action label values counts all in-progress requests, with separate series existing for each controller-action pair.",
            // Let's say that we have all the labels present, as automatic label set selection does not work if we use a custom metric.
            labelNames: HttpRequestLabelNames.All
                    // ... except for "Code" which is only possible to identify after the request is already finished.
                    .Except(new[] { "code" })
                    // ... plus the custom "url" label that we defined above.
                    .Concat(new[] { "url" })
                    .ToArray())
        .WithExtendLifetimeOnUse();

        options.RequestCount.Counter = expiringMetricFactory.CreateCounter(
        "demo_http_requests_received_total",
        "Provides the count of HTTP requests that have been processed by the ASP.NET Core pipeline.",
        // Let's say that we have all the labels present, as automatic label set selection does not work if we use a custom metric.
        labelNames: HttpRequestLabelNames.All
                // ... plus the custom "url" label that we defined above.
                .Concat(new[] { "url" })
                .ToArray())
        .WithExtendLifetimeOnUse();

        options.RequestDuration.Histogram = expiringMetricFactory.CreateHistogram(
            "demo_http_request_duration_seconds",
            "The duration of HTTP requests processed by an ASP.NET Core application.",
            // Let's say that we have all the labels present, as automatic label set selection does not work if we use a custom metric.
            labelNames: HttpRequestLabelNames.All
                    // ... plus the custom "url" label that we defined above.
                    .Concat(new[] { "url" })
                    .ToArray(),
            new HistogramConfiguration
            {
                // 1 ms to 32K ms buckets
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
            })
        .WithExtendLifetimeOnUse();
    });

app.UseAuthorization();

app.MapControllers();

app.Run();
