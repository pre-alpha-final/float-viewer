using System.Collections.Generic;

namespace FloatViewer.Models
{
	public class Project
	{
		public IEnumerable<Person> Persons { get; set; }
		public string ContentJson { get; set; }
	}
}
