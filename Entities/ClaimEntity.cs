using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class ClaimEntity
	{
        [Key]
        public Guid ClaimId { get; set; }
        public ProviderEntity Provider { get; set; }
        public PatientEntity Patient { get; set; }
        public string? RevenueCode { get; set; }
        public string? CPTCode { get; set; }
        public string? CoveredUnits { get; set; }
        public string? NonCoveredUnits { get; set; }
        public decimal? CoveredCharges { get; set; }
        public decimal? NonCoveredCharges { get; set; }
        public DateTime? FromServiceDate { get; set; }
        public DateTime? ToServiceDate { get; set; }
        public decimal TotalCharges { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
