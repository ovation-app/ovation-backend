using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ovation.Application.Constants;
using Sentry.OpenTelemetry;

namespace Ovation.Persistence.Observability.Tracing
{
    public static class SentryConfiguration
    {
        public static IWebHostBuilder UseCustomSentry(this IWebHostBuilder webHostBuilder, IConfiguration configuration)
        {
            return webHostBuilder.UseSentry(options =>
            {
                options.Dsn = Constant.SentryDNS;
                options.TracesSampleRate = 1.0;
                options.Debug = true;
                options.SendDefaultPii = true;
                options.SampleRate = 1.0f;
                options.UseOpenTelemetry();
                options.AddEntityFramework();
                options.UseOpenTelemetry();
            });
        }

    }
}
