using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CryptoPortfolioService_WebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Endpoint1"];
            var dynamicPort = endpoint.IPEndpoint.Port;
            var prefix = $"http://{endpoint.IPEndpoint.Address}:{dynamicPort}/health-monitoring/";

            Trace.WriteLine($"Started web role on endpoint: {prefix}");

            return base.OnStart();
        }
    }
}
