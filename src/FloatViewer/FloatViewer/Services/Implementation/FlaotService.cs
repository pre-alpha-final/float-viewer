using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FloatViewer.Models;
using Newtonsoft.Json.Linq;

namespace FloatViewer.Services.Implementation
{
	public class FlaotService : IFloatService
	{
		public IList<Person> Persons { get; set; } = new List<Person>();
		public IList<Project> Projects { get; set; } = new List<Project>();

		public async Task<IList<Project>> GetProjectsAsync(string accessToken)
		{
			Persons = await GetFromApiAsync<Person>("https://api.float.com/v3/people?project_id=1428797", accessToken);
			Projects = await GetFromApiAsync<Project>("https://api.float.com/v3/projects", accessToken);

			return Projects;
		}

		private async Task<IList<T>> GetFromApiAsync<T>(string url, string accessToken) where T : JsonContainer, new()
		{
			var content = await GetContent(url, accessToken);

			var list = new List<T>();
			if (string.IsNullOrWhiteSpace(content) == false)
			{
				foreach (var token in JToken.Parse(content).SelectTokens("$").Children())
				{
					list.Add(new T { ContentJson = $"{token}" });
				}
			}

			return list;
		}

		private Task<string> GetContent(string url, string accessToken)
		{
			try
			{
				using (var client = new WebClient())
				{
					client.Headers["User-Agent"] =
						"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";
					client.Headers["Authorization"] =
						$"Bearer {accessToken}";

					var result = client.DownloadString(new Uri(url));

					return Task.FromResult(result);
				}
			}
			catch (Exception e)
			{
				// ignore
			}

			return null;
		}
	}
}
