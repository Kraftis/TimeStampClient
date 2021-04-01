using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Disig.RemoteHsm.Web.Infrastructure.Proxy
{
    public class ProxyService
    {
        public ProxyService(IOptions<SharedProxyOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options.Value;
            Client = new HttpClient(Options.MessageHandler ?? new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });
        }

        public SharedProxyOptions Options { get; private set; }
        internal HttpClient Client { get; private set; }

        public bool FilterRequest(HttpRequest httpRequest)
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            if (this.Options.FilterRequest == null)
            {
                return true;
            }
            else
            {
                return this.Options.FilterRequest.Invoke(httpRequest);
            }
        }
    }
}
