using ClinicService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.Core.Entities
{
    [Table("Consultations")]
    public class Consultation : BaseEntity<Guid>
    {
        [ForeignKey(nameof(Client))]
        public Guid ClientId { get; set; }

        [ForeignKey(nameof(Pet))]
        public Guid PetId { get; set; }

        [Column]
        public DateTime ConsultationDate { get; set; }

        [Column]
        public string? Description { get; set; }

        public Client Client { get; set; }
        public Pet Pet { get; set; }
    }
}