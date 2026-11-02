using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.ambassador.support.lib.Models
{
    public class BEACUKAI_ADDED_DETAIL
    {
        [Key]
        public string DetailBCId { get; set; }
        public string BCId { get; set; }
        [ForeignKey("BCId")]
        public virtual BEACUKAI_ADDED BEACUKAI_ADDED { get; set; }

        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitQtyCode { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public string CAR { get; set; }
        public string UomUnit { get; set; }
    }
}
