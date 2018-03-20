using FloatViewer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FloatViewer.Services
{
	public interface IFloatService
	{
		Task<IList<Project>> GetProjectsAsync(string accessToken);
	}
}
