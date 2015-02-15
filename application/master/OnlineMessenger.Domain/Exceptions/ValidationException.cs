using System.Collections.Generic;

namespace OnlineMessenger.Domain.Exceptions
{
    public class ValidationException : OnlineMessengerException
    {
        public List<ValidationError> ValidationErrors { get; private set; }

        public ValidationException(List<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }

        public ValidationException(string name, string descriptioin)
        {
            ValidationErrors = new List<ValidationError>
                               {
                                   new ValidationError(name, descriptioin)
                               };
        }
    }
}
