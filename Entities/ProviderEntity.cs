using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class ProviderEntity
    {
        [Key]
        public Guid ProviderId { get; set; }
        [Required]
        public string LegalName { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        
        public int? ZipCode { get; set; }

        
        public long? NPI { get; set; }
        public long? TaxId { get; set; }
        public string? ProviderNumber { get; set; }

        public DateTime CreateDate { get; set; }

       

    }
}