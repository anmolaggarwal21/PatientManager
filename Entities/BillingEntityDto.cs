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

        public string PatientName { get; set; }


        public string? AdmissionDate { get; set; }

        public string? Payer { get; set; }
        public PatientEntity? Patient { get; set; }

        public bool FoundPatient { get; set; }

        //public DateTime? CreatedDate { get; set; }

        /// <summary>
        ///  Billing item 
        /// </summary>
        public BillingItem BillingItem { get; set; }

        public bool ShowDetails { get; set; }

        public List<TypeDetails>? TypeDetails { get; set; }
    }


    public class TypeDetails
    {
        public string? Type { get; set; }
        public string? AdmissionDate { get; set; }
        public string? NoOfDays1 { get; set; }
        public string?  NoOfDays2 { get; set; }

        public decimal Amount { get; set; }
        public string? Code { get; set; }


    }
}
