using System;
using System.ComponentModel.DataAnnotations;

namespace TestMVC.Models
{
    public abstract class BaseEntity : IValidatableObject
    {
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        public DateTime AdditionDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        public DateTime EditDate { get; set; }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EditDate < AdditionDate)
                yield return new ValidationResult("Дата изменения не может быть меньше даты добавления", new[] { nameof(AdditionDate), nameof(EditDate) });
        }
    }
}
