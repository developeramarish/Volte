using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qmmands;
using RestSharp;
using Volte.Core;
using Volte.Services;
using Version = Volte.Version;

namespace Gommon
{
    public static partial class Extensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection coll, VolteBot bot) =>
            //add all other services; formerly in the VolteBot class
            coll.AddVolteServices()
                .AddSingleton<HandlerService>()
                .AddSingleton(new RestClient {UserAgent = $"Volte/{Version.FullVersion}"})
                .AddSingleton<HttpClient>()
                .AddSingleton<CancellationTokenSource>()
                .AddSingleton(bot);

        public static IServiceCollection AddVolteServices(this IServiceCollection coll)
        {
            //get all the classes that are Volte[Event]Services, aren't abstract, and don't have the System.ObsoleteAttribute attribute.
            foreach (var service in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.HasAttribute<ObsoleteAttribute>() && 
                            (t.Inherits<VolteService>()) && !t.IsAbstract))
            {
                coll.TryAddSingleton(service);
            }

            return coll;
        }

        public static void Get<T>(this IServiceProvider provider, out T service)
            => service = provider.GetRequiredService<T>();
    }
}