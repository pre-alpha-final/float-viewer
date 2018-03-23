using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;

namespace FloatViewer.Controls
{
	public class SessionAwareWebClient : WebClient
	{
		private const string SessionAwareWebClientCookieKey = "SessionAwareWebClientCookieKey";
		private readonly ISession _session;

		public SessionAwareWebClient(ISession session)
		{
			_session = session;
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			var request = base.GetWebRequest(address);

			if (request is HttpWebRequest webRequest)
			{
				var cookieContainer = new CookieContainer();
				foreach (Cookie cookie in GetSessionCookies())
				{
					cookieContainer.Add(address, cookie);
				}
				webRequest.CookieContainer = cookieContainer;
			}

			return request;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			var response = base.GetWebResponse(request);

			if (request is HttpWebRequest webRequest)
			{
				var sessionCookies = GetSessionCookies();
				foreach (Cookie cookie in webRequest.CookieContainer.GetCookies(request.RequestUri))
				{
					sessionCookies.Add(cookie);
				}
				_session.SetString(SessionAwareWebClientCookieKey, JsonConvert.SerializeObject(sessionCookies));
			}

			return response;
		}

		private CookieCollection GetSessionCookies()
		{
			var sessionCookies = _session.GetString(SessionAwareWebClientCookieKey) ?? string.Empty;
			var cookies = JsonConvert.DeserializeObject(sessionCookies, typeof(List<Cookie>)) as List<Cookie> ?? new List<Cookie>();
			var cookieCollection = new CookieCollection();
			foreach (var cookie in cookies)
			{
				cookieCollection.Add(cookie);
			}

			return cookieCollection;
		}
	}
}
