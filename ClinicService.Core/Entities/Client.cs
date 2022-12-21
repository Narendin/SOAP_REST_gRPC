using ClinicService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.Core.Entities
{
    [Table("Clients")]
    public class Client : BaseEntity<Guid>
    {
        [Column]
        [StringLength(50)]
        public string? Document { get; set; }

        [Column]
        [StringLength(255)]
        public string? SurName { get; set; }

        [Column]
        [StringLength(255)]
        public string? FirstName { get; set; }

        [Column]
        [StringLength(255)]
        public string? Patronymic { get; set; }

        [InverseProperty(nameof(Pet.Client))]
        public ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();

        [InverseProperty(nameof(Consultation.Client))]
        public ICollection<Consultation> Consultations { get; set; } = new HashSet<Consultation>();
    }
}