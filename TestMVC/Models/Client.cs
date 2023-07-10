using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestMVC.Models
{
    public class Client : BaseEntity
    {
        public enum ClientType
        {
            [Display(Name="ИП")]
            IP,
            [Display(Name = "ЮЛ")]
            UL
        }

        [Key]
        public int ClientId { get; set; }
        [DisplayName("ИНН")]
        [Required(ErrorMessage = "ИНН является необходимым полем")]
        [RegularExpression("([0-9]+)", ErrorMessage = "ИНН должен содержать только цифры")]
        public string? Inn { get; set; }
        [DisplayName("Наименование")]
        [Required(ErrorMessage = "Наименование является необходимым полем")]
        public string? Name { get; set; }
        [DisplayName("Тип")]
        [Required(ErrorMessage = "Тип является необходимым полем")]
        public ClientType Type { get; set; }
        public IList<Founder> Founders { get; set; } = new List<Founder>();


        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            base.Validate(validationContext);

            if (Inn != null && Inn.Length != 12 && Type == ClientType.IP)
                yield return new ValidationResult("ИНН ИП должен состоять из 12 цифр", new[] {nameof(Inn)});
            if (Inn != null && Inn.Length != 10 && Type == ClientType.UL)
                yield return new ValidationResult("ИНН ЮЛ должен состоять из 10 цифр", new[] { nameof(Inn) });

            if (EditDate < AdditionDate)
                yield return new ValidationResult("Дата изменения не может быть меньше даты добавления", new[] { nameof(AdditionDate), nameof(EditDate) });

            if (Type == ClientType.IP && Founders != null && Founders.Count > 0)
                yield return new ValidationResult("У ИП не может существовать учредителей", new[] { nameof(Type), nameof(Founders) });
        }
    }
}
