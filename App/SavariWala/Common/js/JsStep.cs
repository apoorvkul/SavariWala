using System;
using System.Collections.Generic;

namespace SavariWala.Common.Js
{
	public class JsStep
	{
		public JsCoordinate start_location { get; set; }
		public JsCoordinate end_location { get; set; }
		public JsDisplayValue duration { get; set; }
		public JsDisplayValue distance { get; set; }
		public string html_instructions { get; set; }
	}
}

