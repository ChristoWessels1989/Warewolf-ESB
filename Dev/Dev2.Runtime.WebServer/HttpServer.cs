﻿using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Dev2;
using Dev2.Common;
using Dev2.Runtime.WebServer;
using HttpFramework;
using HttpFramework.Sessions;

namespace Unlimited.Applications.WebServer
{
    public sealed class HttpServer : IDisposable
    {
        #region Instance Fields
        private bool _isDisposed;
        private Dev2Endpoint[] _endPoints;
        private HttpFramework.HttpServer[] _servers;
        private Dictionary<string, List<HttpRequestHandler>> _handlers;
        #endregion

        #region Public Properties
        public bool IsDisposed { get { return _isDisposed; } }
        #endregion

        #region Constructor
        public HttpServer(Dev2Endpoint[] endPoints)
        {
            _endPoints = endPoints;
            _servers = new HttpFramework.HttpServer[(endPoints).Length];
            _handlers = new Dictionary<string, List<HttpRequestHandler>>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < _servers.Length; i++)
            {
                HttpFramework.HttpServer current = _servers[i] = new HttpFramework.HttpServer();
                current.ExceptionThrown += new ExceptionHandler(Server_ExceptionThrown);
                current.Add(new EntryModule(this));

                // enable https endpoint ;)
                if (endPoints[i].IsSecured)
                {
                    var cert = new X509Certificate(endPoints[i].CertificatePath);

                    current.Start(endPoints[i].Address, endPoints[i].Port, cert, SslProtocols.Default, null, false);
                }
                else
                {
                    // enable normal http traffic ;)
                    current.Start(endPoints[i].Address, endPoints[i].Port);
                }
                
            }
        }
        #endregion

        #region Handler Addition
        public void AddHandler(string httpMethod, string uriTemplate, CommunicationContextCallback callback)
        {
            if (_isDisposed) throw new ObjectDisposedException("HttpServer");
            if (String.IsNullOrEmpty(httpMethod)) throw new ArgumentNullException("httpMethod");
            if (callback == null) throw new ArgumentNullException("callback");
            if (String.IsNullOrEmpty(uriTemplate)) throw new ArgumentNullException("uriTemplate");

            HttpRequestHandler handler = new HttpRequestHandler(httpMethod, new UriTemplate(uriTemplate), callback);
            List<HttpRequestHandler> all;
            if (!_handlers.TryGetValue(httpMethod, out all)) _handlers.Add(httpMethod, all = new List<HttpRequestHandler>());
            all.Add(handler);
        }
        #endregion

        #region Request Handling
        private void Process(IHttpRequest request, IHttpResponse response, IHttpSession session, HttpRequestHandler handler, UriTemplateMatch match)
        {
            CommunicationContext context = new CommunicationContext(request, response, session, match);
            handler.Callback(context);
        }
        #endregion

        #region Exception Handling
        private void Server_ExceptionThrown(object source, Exception exception)
        {

        }
        #endregion

        #region Disposal Handling
        ~HttpServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_servers != null)
                {
                    for (int i = 0; i < _servers.Length; i++)
                    {
                        try
                        {
                            _servers[i].Stop();
                            _servers[i] = null;
                        }
                        catch(Exception ex)
                        {
                            ServerLogger.LogError(ex);
                        }
                    }
                }
            }
        }
        #endregion

        #region EntryModule
        private sealed class EntryModule : HttpFramework.HttpModules.HttpModule
        {
            private HttpServer _owner;

            public EntryModule(HttpServer owner)
            {
                _owner = owner;
            }

            public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
            {
                if (_owner._isDisposed) return false;

                Uri uri = request.Uri;
                if (!uri.IsAbsoluteUri || uri.IsFile) return false;

                List<HttpRequestHandler> all;

                if (_owner._handlers.TryGetValue(request.Method, out all))
                {
                    Uri prefix = new Uri(uri.Scheme + "://" + uri.Authority);
                    
                    HttpRequestHandler current;

                    for (int i = 0; i < all.Count; i++)
                    {
                        current = all[i];
                        UriTemplateMatch match = current.Template.Match(prefix, uri);

                        if (match != null)
                        {
                            _owner.Process(request, response, session, current, match);
                            return true;
                        }
                    }
                }

                return false;
            }
        }
        #endregion

        #region HttpRequestHandler
        private sealed class HttpRequestHandler
        {
            private string _httpMethod;
            private UriTemplate _template;
            private CommunicationContextCallback _callback;

            public string HttpMethod { get { return _httpMethod; } }
            public UriTemplate Template { get { return _template; } }
            public CommunicationContextCallback Callback { get { return _callback; } }

            public HttpRequestHandler(string httpMethod, UriTemplate template, CommunicationContextCallback callback)
            {
                _httpMethod = httpMethod;
                _template = template;
                _callback = callback;
            }
        }
        #endregion
    }
}
