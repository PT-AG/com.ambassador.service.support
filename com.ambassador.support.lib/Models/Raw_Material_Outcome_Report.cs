using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace com.ambassador.support.lib.Models
{
    public class Raw_Material_Outcome_Report
    {
        [Key]
        public int Id { get; set; }
        public string BeacukaiNo { get; set; }
        public string HsCode { get; set; }
        public int SeriBarang { get; set; }
        public string UENNo { get; set; }
        public string RONo { get; set; }
        public string DONo { get; set; }
        public DateTime? UENDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UomUnit { get; set; }
        [Column(TypeName = "decimal(14,4)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(14,4)")]
        public decimal QuantitySubcon { get; set; }
        public string ReceiptSubconName { get; set; }
        public string MatchStatus { get; set; }
        public string PIBPosted { get; set; }

    }
}
