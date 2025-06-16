using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CineAPI.Entities.Validaciones
{
    public class PasswordAttribute : ValidationAttribute
    {
        private const int LongitudMin = 8;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (password.Length < LongitudMin)
            {
                return new ValidationResult($"La contraseña debe tener al menos {LongitudMin} caracteres.");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return new ValidationResult("La contraseña debe contener al menos una letra minúscula.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("La contraseña debe contener al menos una letra mayúscula.");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return new ValidationResult("La contraseña debe contener al menos un número.");
            }

            if (!Regex.IsMatch(password, @"[\W_]")) //W es cualquier caracter no alfanumérico
            {
                return new ValidationResult("La contraseña debe contener al menos un carácter especial.");
            }

            return ValidationResult.Success;
        }
    }
}
