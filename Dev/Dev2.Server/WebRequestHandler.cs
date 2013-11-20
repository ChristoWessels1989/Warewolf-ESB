﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Dev2.Common;
using Dev2.DataList.Contract;
using Dev2.DataList.Contract.Binary_Objects;
using Dev2.DynamicServices;
using Dev2.Runtime;
using Dev2.Runtime.Diagnostics;
using Dev2.Server.DataList.Translators;
using Dev2.Web;
using Dev2.Workspaces;
using DEV2.MultiPartFormPasser;
using Unlimited.Applications.WebServer;
using Unlimited.Applications.WebServer.Responses;
using Unlimited.Framework;

namespace Dev2
{
    public class WebRequestHandler
    {
        readonly List<DataListFormat> _publicFormats = new DataListTranslatorFactory().FetchAllFormats().Where(c => c.ContentType != "").ToList();
        readonly ServiceInvoker _serviceInvoker = new ServiceInvoker();

        string _location;
        public string Location { get { return _location ?? (_location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)); } }

        //CommunicationContextCallback GetHandler { get { return Get; } }
        //CommunicationContextCallback PostHandler { get { return Post; } }
        //CommunicationContextCallback InvokeServiceHandler { get { return InvokeService; } }
        //CommunicationContextCallback GetWebResourceHandler { get { return GetWebResource; } }

        public void Get(ICommunicationContext ctx)
        {
            var postDataListID = GetDataListID(ctx);
            if(postDataListID != null)
            {
                Post(ctx);
                return;
            }

            var serviceName = GetServiceName(ctx);
            var workspaceID = GetWorkspaceID(ctx);

            dynamic d = new UnlimitedObject();
            d.Service = serviceName;

            d.WebServerUrl = ctx.Request.Uri.ToString();
            d.Dev2WebServer = String.Format("{0}://{1}", ctx.Request.Uri.Scheme, ctx.Request.Uri.Authority);

            var data = GetPostData(ctx, Guid.Empty.ToString());

            if(!String.IsNullOrEmpty(data))
            {
                d.PostData = data;
                d.Add(new UnlimitedObject().GetStringXmlDataAsUnlimitedObject(data));
            }

            CommunicationResponseWriter responseWriter = CreateForm(d, serviceName, workspaceID, ctx.FetchHeaders(), _publicFormats);
            ctx.Send(responseWriter);
        }

        public void Post(ICommunicationContext ctx)
        {
            var serviceName = GetServiceName(ctx);
            var instanceId = GetInstanceID(ctx);
            var bookmark = GetBookmark(ctx);
            var postDataListID = GetDataListID(ctx);
            var workspaceID = GetWorkspaceID(ctx);

            UnlimitedObject formData = null;

            dynamic d = new UnlimitedObject();

            var xml = GetPostData(ctx, postDataListID);

            if(!String.IsNullOrEmpty(xml))
            {
                formData = new UnlimitedObject().GetStringXmlDataAsUnlimitedObject(xml);
            }

            d.Service = serviceName;
            d.InstanceId = instanceId;
            d.Bookmark = bookmark;
            d.WebServerUrl = ctx.Request.Uri.ToString();
            d.Dev2WebServer = String.Format("{0}://{1}", ctx.Request.Uri.Scheme, ctx.Request.Uri.Authority);
            if(formData != null)
            {
                d.AddResponse(formData);
            }

            CommunicationResponseWriter responseWriter = CreateForm(d, serviceName, workspaceID, ctx.FetchHeaders(), _publicFormats);
            ctx.Send(responseWriter);
        }

        public void InvokeService(ICommunicationContext ctx)
        {
            var methodName = GetAction(ctx);

            // Read post data which is expected to be JSON
            string args;
            using(var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding))
            {
                args = reader.ReadToEnd();
            }

            var className = ctx.Request.BoundVariables["name"];
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
                result = _serviceInvoker.Invoke(className, methodName, args, workspaceGuid, dataListGuid);
            }
            catch(Exception ex)
            {
                result = new ValidationResult
                {
                    ErrorMessage = ex.Message
                };
            }
            ctx.Send(new StringCommunicationResponseWriter(result.ToString(), "application/json"));
        }

        public void GetWebResource(ICommunicationContext ctx)
        {
            var uriString = ctx.Request.Uri.OriginalString;


            if(uriString.IndexOf("wwwroot", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                // http://127.0.0.1:1234/services/"/themes/system/js/json2.js"

                Get(ctx);
                return;
            }

            var website = ctx.Request.BoundVariables["website"];
            var path = ctx.Request.BoundVariables["path"];
            var extension = Path.GetExtension(uriString);

            if(string.IsNullOrEmpty(extension))
            {
                //
                // REST request e.g. http://localhost:1234/wwwroot/sources/server
                //
                const string ContentToken = "getParameterByName(\"content\")";

                var layoutFilePath = string.Format("{0}\\webs\\{1}\\layout.htm", Location, website);
                var contentPath = string.Format("\"/{0}/views/{1}.htm\"", website, path);

                ctx.Send(new DynamicFileCommunicationResponseWriter(layoutFilePath, ContentToken, contentPath));
                return;
            }

            // Should get url's with the following signatures
            //
            // http://localhost:1234/wwwroot/sources/Scripts/jquery-1.7.1.js
            // http://localhost:1234/wwwroot/sources/Content/Site.css
            // http://localhost:1234/wwwroot/sources/images/error.png
            // http://localhost:1234/wwwroot/sources/Views/Dialogs/SaveDialog.htm
            // http://localhost:1234/wwwroot/views/sources/server.htm
            //
            // We support only 1 level below the Views folder 
            // If path is a string without a backslash then we are processing the following request
            //       http://localhost:1234/wwwroot/views/sources/server.htm
            // If path is a string with a backslash then we are processing the following request
            //       http://localhost:1234/wwwroot/sources/Views/Dialogs/SaveDialog.htm
            //
            if(!string.IsNullOrEmpty(path) && path.IndexOf('/') == -1)
            {
                uriString = uriString.Replace(path, "");
            }
            var result = GetFileFromPath(new Uri(uriString));

            ctx.Send(result);
        }

        static CommunicationResponseWriter CreateForm(dynamic d, string serviceName, string workspaceID, NameValueCollection headers, List<DataListFormat> publicFormats)
        {
            // properly setup xml extraction ;)
            string payload = String.Empty;
            if(d.PostData is string)
            {
                payload = d.PostData;
                payload = payload.Replace(GlobalConstants.PostDataStart, "").Replace(GlobalConstants.PostDataEnd, "");
                payload = payload.Replace("<Payload>", "<XmlData>").Replace("</Payload>", "</XmlData>");
            }


            string correctedUri = d.XmlString.Replace("&", "").Replace(GlobalConstants.PostDataStart, "").Replace(GlobalConstants.PostDataEnd, "");
            correctedUri = correctedUri.Replace("<Payload>", "<XmlData>").Replace("</Payload>", "</XmlData>");
            string executePayload;
            IDataListCompiler compiler = DataListFactory.CreateDataListCompiler();
            Guid workspaceGuid;

            if(workspaceID != null)
            {
                if(!Guid.TryParse(workspaceID, out workspaceGuid))
                {
                    workspaceGuid = WorkspaceRepository.Instance.ServerWorkspace.ID;
                }
            }
            else
            {
                workspaceGuid = WorkspaceRepository.Instance.ServerWorkspace.ID;
            }

            ErrorResultTO errors;
            var allErrors = new ErrorResultTO();
            var dataObject = new DsfDataObject(correctedUri, GlobalConstants.NullDataListID, payload) { IsFromWebServer = true };

            // now process headers ;)
            if(headers != null)
            {
                var isRemote = headers.Get(HttpRequestHeader.Cookie.ToString());
                var remoteID = headers.Get(HttpRequestHeader.From.ToString());

                if(isRemote != null && remoteID != null)
                {
                    if(isRemote.Equals(GlobalConstants.RemoteServerInvoke))
                    {
                        // we have a remote invoke ;)
                        dataObject.RemoteInvoke = true;
                    }

                    dataObject.RemoteInvokerID = remoteID;
                }
            }

            // now set the emition type ;)
            int loc;
            if(!String.IsNullOrEmpty(serviceName) && (loc = serviceName.LastIndexOf(".", StringComparison.Ordinal)) > 0)
            {
                // default it to xml
                dataObject.ReturnType = EmitionTypes.XML;

                if(loc > 0)
                {
                    var typeOf = serviceName.Substring((loc + 1)).ToUpper();
                    EmitionTypes myType;
                    if(Enum.TryParse(typeOf, out myType))
                    {
                        dataObject.ReturnType = myType;
                    }

                    // adjust the service name to drop the type ;)

                    // avoid .wiz amendments ;)
                    if(!typeOf.ToLower().Equals(GlobalConstants.WizardExt))
                    {
                        serviceName = serviceName.Substring(0, loc);
                        dataObject.ServiceName = serviceName;
                    }

                }
            }
            else
            {
                // default it to xml
                dataObject.ReturnType = EmitionTypes.XML;
            }

            // ensure service gets set ;)
            if(dataObject.ServiceName == null)
            {
                dataObject.ServiceName = serviceName;
            }

            var esbEndpoint = new EsbServicesEndpoint();

            var executionDlid = esbEndpoint.ExecuteRequest(dataObject, workspaceGuid, out errors);
            allErrors.MergeErrors(errors);

            // Fetch return type ;)
            var formatter = publicFormats.FirstOrDefault(c => c.PublicFormatName == dataObject.ReturnType)
                ?? publicFormats.FirstOrDefault(c => c.PublicFormatName == EmitionTypes.XML);

            // force it to XML if need be ;)

            // Fetch and convert DL ;)
            if(executionDlid != GlobalConstants.NullDataListID)
            {
                dataObject.DataListID = executionDlid;
                dataObject.WorkspaceID = workspaceGuid;
                dataObject.ServiceName = serviceName;


                executePayload = esbEndpoint.FetchExecutionPayload(dataObject, formatter, out errors);

                allErrors.MergeErrors(errors);
                compiler.UpsertSystemTag(executionDlid, enSystemTag.Dev2Error, allErrors.MakeDataListReady(), out errors);
            }
            else
            {
                if(dataObject.ReturnType == EmitionTypes.XML)
                {

                    executePayload = "<FatalError> <Message> An internal error occured while executing the service request </Message>";
                    executePayload += allErrors.MakeDataListReady();
                    executePayload += "</FatalError>";
                }
                else
                {
                    // convert output to JSON ;)
                    executePayload = "{ \"FatalError\": \"An internal error occured while executing the service request\",";
                    executePayload += allErrors.MakeDataListReady(false);
                    executePayload += "}";
                }
            }

            // Clean up the datalist from the server
            if(!dataObject.WorkflowResumeable && executionDlid != GlobalConstants.NullDataListID)
            {
                compiler.ForceDeleteDataListByID(executionDlid);
            }

            // old HTML throw back ;)
            if(dataObject.ReturnType == EmitionTypes.WIZ)
            {
                int start = (executePayload.IndexOf("<Dev2System.FormView>", StringComparison.Ordinal) + 21);
                int end = (executePayload.IndexOf("</Dev2System.FormView>", StringComparison.Ordinal));
                int len = (end - start);
                if(len > 0)
                {
                    if(dataObject.ReturnType == EmitionTypes.WIZ)
                    {
                        string tmp = executePayload.Substring(start, (end - start));
                        string result = CleanupHtml(tmp);
                        const string DocType = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">";
                        return new StringCommunicationResponseWriter(String.Format("{0}\r\n{1}", DocType, result));
                    }
                }
            }

            // JSON Data ;)
            if(executePayload.IndexOf("</JSON>", StringComparison.Ordinal) >= 0)
            {
                int start = executePayload.IndexOf(GlobalConstants.OpenJSON, StringComparison.Ordinal);
                if(start >= 0)
                {
                    int end = executePayload.IndexOf(GlobalConstants.CloseJSON, StringComparison.Ordinal);
                    start += GlobalConstants.OpenJSON.Length;

                    executePayload = CleanupHtml(executePayload.Substring(start, (end - start)));
                    if(!String.IsNullOrEmpty(executePayload))
                    {
                        return new StringCommunicationResponseWriter(executePayload, "application/json");
                    }
                }
            }

            // else handle the format requested ;)
            return new StringCommunicationResponseWriter(executePayload, formatter.ContentType);

        }

        static string GetPostData(ICommunicationContext ctx, string postDataListID)
        {
            var formData = new UnlimitedObject();

            var isXmlData = false;

            var baseStr = HttpUtility.UrlDecode(ctx.Request.Uri.ToString());
            if(baseStr != null)
            {
                var startIdx = baseStr.IndexOf("?", StringComparison.Ordinal);
                if(startIdx > 0)
                {
                    var payload = baseStr.Substring((startIdx + 1));
                    if(payload.IsXml())
                    {
                        formData = new UnlimitedObject().GetStringXmlDataAsUnlimitedObject(payload);
                        isXmlData = true;
                    }
                }
            }

            if(!isXmlData)
            {
                foreach(var key in ctx.Request.QueryString.AllKeys)
                {
                    formData.CreateElement(key).SetValue(ctx.Request.QueryString[key]);
                }

                if((!String.IsNullOrEmpty(ctx.Request.ContentType)) && ctx.Request.ContentType.ToUpper().Contains("MULTIPART"))
                {
                    var parser = new MultipartDataParser(ctx.Request.InputStream, ctx.Request.ContentType, ctx.Request.ContentEncoding);
                    var results = parser.ParseStream();

                    results.ForEach(result =>
                    {
                        var textObj = result.FormValue as TextObj;
                        if(textObj != null)
                        {
                            formData.CreateElement(result.FormKey).SetValue(textObj.ValueVar);
                        }
                        else
                        {
                            var fileObj = result.FormValue as FileObj;
                            if(fileObj != null)
                            {
                                if(fileObj.ValueVar.LongLength > 0)
                                {
                                    formData.CreateElement(result.FormKey).SetValue(Convert.ToBase64String(fileObj.ValueVar));
                                    formData.CreateElement(String.Format("{0}_filename", result.FormKey)).SetValue(fileObj.FileName);
                                }
                            }
                        }
                    });
                }
                else
                {
                    string data;
                    if(postDataListID != null && new Guid(postDataListID) != Guid.Empty)
                    {
                        //TrevorCake
                        var dlid = new Guid(postDataListID);
                        ErrorResultTO errors;
                        string error;
                        var datalListServer = DataListFactory.CreateDataListServer();
                        var dataList = datalListServer.ReadDatalist(dlid, out errors);
                        datalListServer.DeleteDataList(dlid, false);
                        IBinaryDataListEntry dataListEntry;
                        dataList.TryGetEntry(GlobalConstants.PostData, out dataListEntry, out error);
                        data = dataListEntry.FetchScalar().TheValue;
                    }
                    else
                    {
                        using(var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding))
                        {
                            try
                            {
                                data = reader.ReadToEnd();
                            }
                            catch(Exception ex)
                            {
                                ServerLogger.LogError(ex);
                                data = "";
                            }
                        }
                    }

                    try
                    {
                        if(DataListUtil.IsXml(data))
                        {
                            formData.Add(new UnlimitedObject(XElement.Parse(data)));
                        }
                        else if(data.StartsWith("{") && data.EndsWith("}")) // very simple JSON check!!!
                        {
                            formData.CreateElement("Args").SetValue(data);
                        }
                        else
                        {
                            var keyValuePairs = data.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                            foreach(var keyValuePair in keyValuePairs)
                            {
                                var keyValue = keyValuePair.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                                if(keyValue.Length > 1)
                                {
                                    if(keyValue[1].StartsWith("{") && keyValue[keyValue.Length - 1].EndsWith("}"))
                                    {
                                        var parameterName = keyValue[0];
                                        var jsonData = keyValue.ToList();
                                        jsonData.Remove(parameterName);
                                        formData.CreateElement(parameterName).SetValue(String.Join("=", jsonData));
                                        continue;
                                    }
                                    var formFieldValue = HttpUtility.UrlDecode(keyValue[1]);
                                    try
                                    {
                                        // ReSharper disable AssignNullToNotNullAttribute
                                        formFieldValue = XElement.Parse(formFieldValue).ToString();
                                        // ReSharper restore AssignNullToNotNullAttribute
                                    }
                                    catch(Exception ex)
                                    {
                                        ServerLogger.LogError(ex);
                                    }
                                    formData.CreateElement(keyValue[0]).SetValue(formFieldValue);
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        ServerLogger.LogError(ex);
                    }
                }
            }

            // Still need to remvove the rubish from the string
            var tmpOut = formData.XmlString.Replace("<XmlData>", GlobalConstants.PostDataStart).Replace("</XmlData>", GlobalConstants.PostDataEnd).Replace("<XmlData />", "");
            // Replace the DataList tag for test... still a hack
            tmpOut = tmpOut.Replace("<DataList>", String.Empty).Replace("</DataList>", String.Empty);

            return tmpOut;
        }

        static string CleanupHtml(string result)
        {
            var html = result;

            html = html.Replace("&amp;amp;", "&");
            html = html.Replace("&lt;", "<").Replace("&gt;", ">");
            html = html.Replace("lt;", "<").Replace("gt;", ">");
            html = html.Replace("&amp;gt;", ">").Replace("&amp;lt;", "<");
            html = html.Replace("&amp;amp;amp;lt;", "<").Replace("&amp;amp;amp;gt;", ">");
            html = html.Replace("&amp;amp;lt;", "<").Replace("&amp;amp;gt;", ">");
            html = html.Replace("&<", "<").Replace("&>", ">");
            html = html.Replace("&quot;", "\"");

            return html;
        }

        static string GetServiceName(ICommunicationContext ctx)
        {
            var serviceName = ctx.Request.BoundVariables["servicename"];
            return serviceName;
        }

        static string GetWorkspaceID(ICommunicationContext ctx)
        {
            return ctx.Request.QueryString["wid"] ?? ctx.Request.BoundVariables["clientid"];
        }

        static string GetDataListID(ICommunicationContext ctx)
        {
            return ctx.Request.QueryString[GlobalConstants.DLID];
        }

        static string GetBookmark(ICommunicationContext ctx)
        {
            return ctx.Request.BoundVariables["bookmark"];
        }

        static string GetInstanceID(ICommunicationContext ctx)
        {
            return ctx.Request.BoundVariables["instanceid"];
        }

        static string GetAction(ICommunicationContext ctx)
        {
            return ctx.Request.BoundVariables["action"];
        }

        CommunicationResponseWriter GetFileFromPath(Uri uri)
        {
            var filePath = string.Format("{0}\\Webs{1}\\{2}", Location,
                Path.GetDirectoryName(uri.LocalPath),
                Path.GetFileName(uri.LocalPath));
            return GetFileFromPath(filePath);
        }

        static CommunicationResponseWriter GetFileFromPath(string filePath)
        {
            var supportedFileExtensions = ConfigurationManager.AppSettings["SupportedFileExtensions"];
            var extension = Path.GetExtension(filePath);
            var ext = string.IsNullOrEmpty(extension) ? "" : extension;
            var isSupportedExtensionList = supportedFileExtensions.Split(new[] { ',' })
                .ToList()
                .Where(supportedExtension => supportedExtension.Trim().Equals(ext, StringComparison.InvariantCultureIgnoreCase));

            if(string.IsNullOrEmpty(supportedFileExtensions) || !isSupportedExtensionList.Any())
            {
                return new NotFoundCommunicationResponseWriter();
            }

            if(File.Exists(filePath))
            {
                string contentType;
                switch(ext.ToLower())
                {
                    case ".js":
                        contentType = "text/javascript";
                        break;

                    case ".css":
                        contentType = "text/css";
                        break;

                    case ".ico":
                        contentType = "image/x-icon";
                        break;

                    case ".bm":
                    case ".bmp":
                        contentType = "image/bmp";
                        break;

                    case ".gif":
                        contentType = "image/gif";
                        break;

                    case ".jpeg":
                    case ".jpg":
                        contentType = "image/jpg";
                        break;

                    case ".tiff":
                        contentType = "image/tiff";
                        break;

                    case ".png":
                        contentType = "image/png";
                        break;

                    case ".htm":
                    case ".html":
                        contentType = "text/html";
                        break;

                    default:
                        return new NotFoundCommunicationResponseWriter();
                }

                return new StaticFileCommunicationResponseWriter(filePath, contentType);
            }


            return new NotFoundCommunicationResponseWriter();
        }

    }
}
