using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.ambassador.support.lib.ViewModel
{
    public class FinishedGoodViewModel
    {
		[Key]
		public string KodeBarang { get; set; }
		public string NamaBarang { get; set; }
		public string UnitQtyName { get; set; }
		public string SaldoAwal { get; set; }
		public string Pemasukan { get; set; }
		public string Pengeluaran { get; set; }
		public string Penyesuaian { get; set; }
		public string StockOpname { get; set; }
		public string Selisih { get; set; }
		public string SaldoBuku { get; set; }
	}
}
