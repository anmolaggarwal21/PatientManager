using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Table("BillingEntity")]
    public class BillingEntity
    {
        [Key]
        public Guid BillingId { get; set; }

        public string PatientDetails { get; set; }

        public string POS { get; set; }

        public int CareDay { get; set; }
        public string? Payer { get; set; }

        public string? RAndB { get; set; }
        public string? MDVisits { get; set; }
        public string? RBVisists { get; set; }
        public string? LVNVisits { get; set; }

        public string? HAVisits { get; set; }
        public string? SWVisits { get; set; }
        public string? SWWPHone { get; set; }
        public string? SCVisists { get; set; }
        public string? PatientName { get; set; }

        public PatientEntity? Patient { get; set; }
        public BillingDetailsEntity BillingDetails { get; set; }

        public bool FoundPatient { get; set; }

        public DateTime? CreateDate { get; set; }

    }

}
