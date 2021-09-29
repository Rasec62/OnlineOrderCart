using System.Collections.Generic;

namespace OnlineOrderCart.Common.Responses
{
    public class GenericResponse<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
        public List<T> ListResults { get; set; }
    }
}
