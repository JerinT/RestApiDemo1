using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Responses
{
    public class ValidationFailureResponse
    {
        public IEnumerable<ValidationResponse> Errors { get; init; }
    }
    public class ValidationResponse
    {
        public string PropertyName { get; init; }
        public string ErrorMessage { get; init; }
    }
}
