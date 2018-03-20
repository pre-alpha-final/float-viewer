using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FloatViewer.Models;
using FloatViewer.Services;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace FloatViewer.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IFloatService _floatService;

		public IList<Project> Projects { get; set; }
		public bool HasProjects => Projects?.Count > 0;

		[Required]
		[EmailAddress]
		[BindProperty]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[BindProperty]
		public string Password { get; set; }

		public IndexModel(IFloatService floatService)
		{
			_floatService = floatService;
		}

		public async Task OnGetAsync()
		{
			Projects = _floatService.Projects;
		}

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
		{
			Projects = await _floatService.GetProjectsAsync(Email, Password);

			return RedirectToPage();
		}

		public JToken JsonParse(string json)
		{
			return JObject.Parse(json);
		}
	}
}
