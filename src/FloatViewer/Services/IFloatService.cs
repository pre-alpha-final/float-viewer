using FloatViewer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FloatViewer.Services
{
	public interface IFloatService
	{
		IList<Person> Persons { get; set; }
		IList<Project> Projects { get; set; }

		Task<bool> IsLoginRequiredAsync(ISession session);
		Task LogInAsync(ISession session, string email, string password);
		Task<IList<Project>> GetProjectsAsync(ISession session);
	}
}
