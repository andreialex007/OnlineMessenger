using System.Collections.Generic;
using System.Linq;

namespace OnlineMessenger.Domain.Exceptions
{
    public class EntityErrors<T> where T : new()
    {
        public EntityErrors(T entityObject, List<ValidationError> validationErrors)
        {
            EntityObject = entityObject;
            ValidationErrors = validationErrors;
        }

        public T EntityObject { get; set; }
        public List<ValidationError> ValidationErrors { get; set; }

        public bool IsErrors
        {
            get
            {
                return ValidationErrors.Any();
            }
        }
    }
}
