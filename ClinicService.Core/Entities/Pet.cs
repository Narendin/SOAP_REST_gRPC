using ClinicService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.Core.Entities
{
    [Table("Pets")]
    public class Pet : BaseEntity<Guid>
    {
        [ForeignKey(nameof(Client))]
        public Guid ClientId { get; set; }

        [Column]
        [StringLength(20)]
        public string Name { get; set; }

        [Column]
        public DateTime Birthday { get; set; }

        public Client Client { get; set; }

        [InverseProperty(nameof(Consultation.Pet))]
        public ICollection<Consultation> Consultations { get; set; } = new HashSet<Consultation>();
    }
}