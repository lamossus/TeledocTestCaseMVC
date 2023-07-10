using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TestMVC.Models.Client;

namespace TestMVC.Models
{
    public class Founder : BaseEntity
    {
        [Key]
        public int FounderId { get; set; }
        [Required(ErrorMessage = "ИНН является необходимым полем")]
        public string Inn { get; set; } = string.Empty;
        [Required(ErrorMessage = "ФИО является необходимым полем")]
        [MaxLength(200)]
        [RegularExpression("^[^\\d]+$", ErrorMessage = "ФИО не может содержать цифры")]
        public string FullName { get; set; } = string.Empty;
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            base.Validate(validationContext);

            if (Inn != null && Inn.Length != 12)
                yield return new ValidationResult("ИНН учредителя должен состоять из 12 цифр", new[] { nameof(Inn) });

            if (Client != null && Client.Type == ClientType.IP)
                yield return new ValidationResult("У ИП не может существовать учредителей", new[] {nameof(Client)});
        }
    }
}
