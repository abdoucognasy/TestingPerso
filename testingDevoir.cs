using Microsoft.Xrm.Sdk;
using System;

namespace TestingPerso
{
    public class testingDevoir : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            ITracingService tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
            IOrganizationServiceFactory serviceFactory = serviceProvider.GetService(typeof(ITracingService)) as IOrganizationServiceFactory;
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                switch (context.MessageName)
                {
                    case "create":
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
