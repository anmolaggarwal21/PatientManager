using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PatientEntity
    {
        [Key]
        public Guid PatientId { get; set; }

        [Required]
        public string FullName { get; set; }
		[DataType(DataType.Date)]
		public DateTime? DOB { get; set; }

        public string? PhoneNumber { get; set; }

        public GenderEnum Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string state { get; set; }
        public int? ZipCode { get; set; }

		[DataType(DataType.Date)]
		public DateTime? AdmissionDate { get; set; }

        public string? DiagnosisCode1 { get; set; }
        public string? DiagnosisCode2 { get; set; }
        public string? DiagnosisCode3 { get; set; }
        public string? DiagnosisCode4 { get; set; }
        public string? DiagnosisCode5 { get; set; }
        public string? DiagnosisCode6 { get; set; }

        public string? AttendingPhysicianFullName { get; set; }
        public long? AttendingPhysicianNPI { get; set; }
        public string? ReferringPhysicianFullName { get; set; }
        public long? ReferringPhysicianNPI { get; set; }

        public string? OtherPhysicianFullName { get; set; }
        public long? OtherPhysicianNPI { get; set; }
        public DateTime CreateDate { get; set; }

        public ProviderEntity Provider { get; set; }

    }
}
