using System.Collections.Generic;

namespace FloatViewer.Models
{
	public class Project : JsonContainer
	{
		public IEnumerable<Person> Persons { get; set; }
	}
}
