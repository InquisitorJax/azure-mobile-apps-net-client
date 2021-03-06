﻿// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.TestFramework;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    /// <summary>
    /// Base class for functional tests.
    /// </summary>
    [FunctionalTest]
    public class FunctionalTestBase : TestBase
    {
        static MobileServiceClient staticClient;

        /// <summary>
        /// Get a client pointed at the test server without request logging.
        /// </summary>
        /// <returns>The test client.</returns>
        public MobileServiceClient GetClient()
        {
            if (staticClient == null)
            {
                string runtimeUrl = this.GetTestSetting("MobileServiceRuntimeUrl");
                
                staticClient = new MobileServiceClient(runtimeUrl, new LoggingHttpHandler(this));

                // Uncomment this and replace proxyUri with your Android device IP address to enable debugging in Fiddler
                /*
                string proxyUri = "YOUR_DEVICE_IP_ADDRESS";
                staticClient = new MobileServiceClient(runtimeUrl, new HttpClientHandler
                {
                    Proxy = new Microsoft.WindowsAzure.MobileServices.Test.FunctionalTests.WebProxy(new System.Uri(proxyUri))
                });
                */
            }
            return staticClient;
        }
    }

    class LoggingHttpHandler : DelegatingHandler
    {
        public TestBase Test { get; private set; }

        public LoggingHttpHandler(TestBase test)
        {
            Test = test;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Test.Log("    >>> {0} {1} {2}", request.Method, request.RequestUri, request.Content);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            Test.Log("    <<< {0} {1} {2}", response.StatusCode, response.ReasonPhrase, response.Content);
            return response;
        }
    }

    class WebProxy : IWebProxy
    {
        private readonly Uri uri;

        public WebProxy(Uri uri)
        {
            this.uri = uri;
        }

        public ICredentials Credentials
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Uri GetProxy(Uri destination)
        {
            return uri;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
