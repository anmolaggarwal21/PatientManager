using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public string PatientName{ get; set; }


        public DateTime? AdmissionDate { get; set; }

        public string? Payer { get; set; }
        public PatientEntity? Patient { get; set; }

		public bool FoundPatient { get; set; }

        //public DateTime? CreatedDate { get; set; }

        /// <summary>
        ///  Billing item 
        /// </summary>
        public BillingItem BillingItem { get; set; }
    }

	public class BillingItem
	{
		/// <summary>
		/// POS data 
		/// </summary>
        public BillingData? POSBillingItem { get; set; }

		/// <summary>
		/// Billing data for all other than POS
		/// </summary>
		public List<BillingData>? OtherBillingItems { get; set;}
    }

	public class BillingData
	{
		/// <summary>
		/// It will be mapped to type
		/// </summary>
        public string Type { get; set; }

		/// <summary>
		///  It will have data respective to the admission date and the number of days for particular type
		/// </summary>
        public List<Data> Data { get; set; }


    }

	public class Data
	{
		public int NoOfDays { get; set; }
        public DateTime? AdmissionDate { get; set; }

        public Data()
        {
            
        }
        public Data(int NoOfDays, DateTime AdmissionDate)
        {
			this.NoOfDays = NoOfDays;
			this.AdmissionDate = AdmissionDate;
        }
    }
}
