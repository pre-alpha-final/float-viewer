using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FloatViewer.Controls;
using FloatViewer.Extensions;
using FloatViewer.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace FloatViewer.Services.Implementation
{
	public class FloatService : IFloatService
	{
		public IList<Person> Persons { get; set; } = new List<Person>();
		public IList<Project> Projects { get; set; } = new List<Project>();

		public async Task<bool> IsLoginRequiredAsync(ISession session)
		{
			var result = await new SessionAwareWebClient(session).Get("https://login.float.com");

			return result.Contains(" Login Site ");
		}

		public async Task LogInAsync(ISession session, string email, string password)
		{
			var result = await new SessionAwareWebClient(session).Post("https://login.float.com/login", new NameValueCollection
			{
				{ "LoginForm[email]", email },
				{ "LoginForm[password]", password },
				{ "yt0", "Sign in" }
			});

			if (result.Contains("Incorrect email or password"))
				throw new ArgumentException("Wrong login credentials");
		}

		public async Task<IList<Project>> GetProjectsAsync(ISession session)
		{
			Persons = await ExtractData<Person>(session, "https://login.float.com/api/f1/people", "$.people");
			Projects = await ExtractData<Project>(session, "https://login.float.com/api/f1/projects", "$.projects");

			foreach (var project in Projects)
			{
				var projectId = JToken.Parse(project.ContentJson).SelectToken("$.project_id");
				project.Persons = Persons
					.Where(e => JToken.Parse(e.ContentJson).SelectTokens("$.project_ids[*]").Contains($"{projectId}")).ToList();
			}

			return Projects;
		}

		private async Task<IList<T>> ExtractData<T>(ISession session, string url, string selector) where T : JsonContainer, new()
		{
			var list = new List<T>();

			var content = await new SessionAwareWebClient(session).Get(url);
			if (string.IsNullOrWhiteSpace(content) == false)
			{
				list.AddRange(JToken.Parse(content)
					.SelectTokens(selector)
					.Children()
					.Select(token => new T {ContentJson = $"{token}"}));
			}

			return list;
		}
	}
}
