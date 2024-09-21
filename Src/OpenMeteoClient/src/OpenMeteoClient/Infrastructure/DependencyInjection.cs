using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMeteoClient.Application.Interfaces;
using OpenMeteoClient.Application.Services;
using OpenMeteoClient.Infrastructure.ApiClients;
using OpenMeteoClient.Infrastructure.Interfaces;
using Polly;
using Polly.Extensions.Http;

namespace OpenMeteoClient.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOpenMeteoClient(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient<IOpenMeteoApiClient, OpenMeteoApiClient>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddScoped<IWeatherForecastService, WeatherForecastService>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}