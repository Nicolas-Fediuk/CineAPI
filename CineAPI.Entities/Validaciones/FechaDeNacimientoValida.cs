using System.ComponentModel.DataAnnotations;

namespace CineAPI.Validaciones
{
    public class FechaDeNacimientoValida : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("El campo fecha de nacimiento es obligatorio.");
            }

            if(value is DateTime fechaDeNac)
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - fechaDeNac.Year;

                if (fechaDeNac.Date > hoy.AddYears(-edad))
                {
                    edad--;
                }

                if (edad < 18)
                {
                    return new ValidationResult("Tenés que ser mayor de 18 años.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
