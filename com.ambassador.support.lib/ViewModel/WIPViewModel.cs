using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.ambassador.support.lib.ViewModel
{
    public class WIPViewModel
    {
		[Key]
		public string Kode { get; set; }
		public string Comodity { get; set; }
		public string UnitQtyName { get; set; }
		public string WIP { get; set; }
	}
}
