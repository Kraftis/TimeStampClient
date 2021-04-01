using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disig.RemoteHsm.Web.Infrastructure.Proxy
{
    public static class ProxyServiceCollectionExtensions
    {
        public static IServiceCollection AddProxy(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.AddSingleton<ProxyService>();
        }

        public static IServiceCollection AddProxy(this IServiceCollection services, Action<SharedProxyOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);
            return services.AddSingleton<ProxyService>();
        }
    }
}
