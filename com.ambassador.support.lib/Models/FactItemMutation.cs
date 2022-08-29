using System;
using System.Collections.Generic;
using System.Text;

namespace com.ambassador.support.lib.Models
{
    public class FactItemMutation
    {
        
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Poid { get; set; }
        public string Ro { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public double? Quantity { get; set; }
        public string UnitQtyCode { get; set; }
        public string UnitQtyName { get; set; }
		public string SupplierType { get; set; }
        public string ClassificationCode { get; set; }
        public string ClassificationName { get; set; }
        public int? StockId { get; set; }
    }
}
