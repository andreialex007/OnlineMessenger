using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using OnlineMessenger.Domain.Exceptions;
using ValidationException = OnlineMessenger.Domain.Exceptions.ValidationException;

namespace OnlineMessenger.Domain.Utils
{
    public static class ValidationExtensions
    {
        public static List<ValidationError> GetValidationErrors(this object obj)
        {
            var validationErrors = new List<ValidationError>();
            var type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
                validationErrors.AddRange(GetValidationErrors(propertyInfo, obj));
            return validationErrors;
        }

        private static IEnumerable<ValidationError> GetValidationErrors(PropertyInfo propertyInfo, object obj)
        {
            string propertyName = propertyInfo.Name;
            object propertyValue = propertyInfo.GetValue(obj);
            return propertyInfo.GetCustomAttributes<ValidationAttribute>()
                .Where(x => !x.IsValid(propertyValue))
                .Select(x => new ValidationError(propertyName, x.ErrorMessage));
        }

        /// <summary>
        /// Performs validation of object based on dataannotations of object, throws validation exception if object not valid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objToValidate"></param>
        public static void ThrowIfInvalid<T>(this T objToValidate)
        {
            var errors = GetValidationErrors(objToValidate);
            if (errors.Any())
                throw new ValidationException(errors);
        }
    }
}