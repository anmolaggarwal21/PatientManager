using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Table("BillingDetailsEntity")]
    public class BillingDetailsEntity
    {
        [Key]
        public Guid BillingDetailsId { get; set; }

       

        public string? BenefitPeriod { get; set; }

        public DateTime? FieldTwentySeven { get; set; }

        public DateTime? FromFieldSeventySeven { get; set; }

        public DateTime? ToFieldSeventySeven { get; set; }

        public DateTime? FieldEOC { get; set; }

        public string? FieldDropDown { get; set; }
        public int? FieldRX { get; set; }
    }
}
