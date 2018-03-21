using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FloatViewer.Controls;
using FloatViewer.Extensions;
using FloatViewer.Models;
using Newtonsoft.Json.Linq;

namespace FloatViewer.Services.Implementation
{
	public class FloatService : IFloatService
	{
		private readonly CookieAwareWebClient _cookieAwareWebClient = new CookieAwareWebClient();

		public IList<Person> Persons { get; set; } = new List<Person>();
		public IList<Project> Projects { get; set; } = new List<Project>();

		public async Task<IList<Project>> GetProjectsAsync(string login, string password)
		{
			var result = await LogIn(login, password);
			if (result.Contains("Incorrect email or password"))
				throw new ArgumentException("Wrong login credentials");

			Persons = await ExtractData<Person>("https://login.float.com/api/f1/people", "$.people");
			Projects = await ExtractData<Project>("https://login.float.com/api/f1/projects", "$.projects");

			foreach (var project in Projects)
			{
				var projectId = JToken.Parse(project.ContentJson).SelectToken("$.project_id");
				project.Persons = Persons
					.Where(e => JToken.Parse(e.ContentJson).SelectTokens("$.project_ids[*]").Contains($"{projectId}")).ToList();
			}

			return Projects;
		}

		private Task<string> LogIn(string userName, string password)
		{
			return _cookieAwareWebClient.Post("https://login.float.com/login", new NameValueCollection
				{
					{ "LoginForm[email]", userName },
					{ "LoginForm[password]", password },
					{ "yt0", "Sign in" }
				});
		}

		private async Task<IList<T>> ExtractData<T>(string url, string selector) where T : JsonContainer, new()
		{
			var list = new List<T>();

			var content = await _cookieAwareWebClient.Get(url);
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
