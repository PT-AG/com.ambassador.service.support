using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace com.ambassador.support.lib.ViewModel
{
	public class ScrapViewModel
	{
		[Key]
		public string ClassificationId { get; set; }
		public string ClassificationCode { get; set; }
		public string ClassificationName { get; set; }
		public string StockId { get; set; }
		public string DestinationId { get; set; }
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
