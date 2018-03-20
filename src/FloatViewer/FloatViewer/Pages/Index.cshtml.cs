using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FloatViewer.Models;
using FloatViewer.Services;
using Newtonsoft.Json.Linq;

namespace FloatViewer.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IFloatService _floatService;

		public IList<Project> Projects { get; set; }
		public bool HasProjects => Projects?.Count > 0;

		[BindProperty]
		public string AuthenticationToken { get; set; }

		public IndexModel(IFloatService floatService)
		{
			_floatService = floatService;
		}

		public async Task OnGetAsync()
		{
		}

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostUpdateAsync()
		{
			return RedirectToPage();
		}

		public JToken JsonParse(string json)
		{
			return JObject.Parse(json);
		}
	}
}
