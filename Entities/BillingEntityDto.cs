using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BillingEntityDto 
    {

        public Guid BillingId { get; set; }
        public PatientEntity? Patient { get; set; }
        public bool ShowDetails { get; set; }
        public bool FoundPatient { get; set; }

        public DateTime? CreateDate { get; set; }

        public BillingDetailsEntity BillingDetails { get; set; }

        public List<TypeDetails> TypeDetails { get; set; }
    }

    public class TypeDetails
    {
        public Guid BillingId { get; set; }

        public string PatientDetails { get; set; }

        public string POS { get; set; }

        public int CareDay { get; set; }
        public string? Payer { get; set; }

        public string? RAndB { get; set; }
        public string? MDVisits { get; set; }
        public string? RNVisits { get; set; }
        public string? LVNVisits { get; set; }

        public string? HAVisits { get; set; }
        public string? SWVisits { get; set; }
        public string? SWWPHone { get; set; }
        public string? SCVisists { get; set; }
        public string? PatientName { get; set; }
    }

    public class BillingAmount
    {

        public string Code { get; set; }
        public long StartNoOfDays { get; set; }
        public long EndNoOfDays { get; set; }
        public decimal Amount { get; set; }
        public DateTime? AdmitDate { get; set; }
    }

}
