namespace Application.Bases
{
    public interface IResponseHandler
    {
        public Response<T> Deleted<T>(string Message = null);
        public Response<T> Success<T>(T entity, object Meta = null);
        public Response<T> Unauthorized<T>(string Message = null);

        public Response<T> BadRequest<T>(string Message = null);
        public Response<T> UnprocessableEntity<T>(string Message = null);
        public Response<T> NotFound<T>(string message = null);
        public Response<T> Created<T>(T entity, object Meta = null);

    }
}
