﻿using System;
using System.IO;
using Dev2.Runtime.Diagnostics;
using Dev2.Runtime.WebServer.Responses;

namespace Dev2.Runtime.WebServer.Handlers
{
    public class WebsiteServiceHandler : AbstractWebRequestHandler
    {
        readonly ServiceInvoker _serviceInvoker = new ServiceInvoker();

        public override void ProcessRequest(ICommunicationContext ctx)
        {
            // Read post data which is expected to be JSON
            string args;
            using(var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding))
            {
                args = reader.ReadToEnd();
            }

            var className = GetClassName(ctx);
            var methodName = GetMethodName(ctx);
            var dataListID = GetDataListID(ctx);
            var workspaceID = GetWorkspaceID(ctx);
            dynamic result;
            try
            {
                Guid workspaceGuid;
                Guid.TryParse(workspaceID, out workspaceGuid);

                Guid dataListGuid;
                Guid.TryParse(dataListID, out dataListGuid);

                //
                // NOTE: result.ToString() MUST return JSON
                //

                // This is where stuff starts going wrong!!!!
                // We need to execute in context of the user requesting the execution, not the flipping server user ;)
                var userPrinciple = ctx.Request.User;
                if(userPrinciple != null)
                {
                    System.Threading.Thread.CurrentPrincipal = userPrinciple;
                }

                result = _serviceInvoker.Invoke(className, methodName, args, workspaceGuid, dataListGuid);
            }
            catch(Exception ex)
            {
                result = new ValidationResult
                {
                    ErrorMessage = ex.Message
                };
            }
            ctx.Send(new StringResponseWriter(result.ToString(), ContentTypes.Json));
        }
    }
}