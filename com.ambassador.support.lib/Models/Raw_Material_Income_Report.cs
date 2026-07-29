using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.ambassador.support.lib.Models
{
    public class Raw_Material_Income_Report
    {
        [Key]
        public int Id { get; set; }

        public DateTime? RecordDate { get; set; }

        public string CustomsType { get; set; }

        public string BeacukaiNo { get; set; }

        public DateTime? BeacukaiDate { get; set; }

        public string HsCode { get; set; }

        public int SeriBarang { get; set; }

        public string URNNo { get; set; }

        public DateTime? URNDate { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string SmallUomUnit { get; set; }

        [Column(TypeName = "decimal(14,4)")]
        public decimal SmallQuantity { get; set; }

        public string CurrencyCode { get; set; }

        [Column(TypeName = "decimal(14,4)")]
        public decimal Price { get; set; }

        public string StorageName { get; set; }

        public string SupplierName { get; set; }

        public string Country { get; set; }
    }
}
