namespace Unico.Admin.Api.Models
{
    // use envelope inspired by https://github.com/peterboyer/restful-api-design-tips#responses (and https://medium.com/@peterboyer/learn-restful-api-design-ideals-c5ec915a430f)
    public class EnvelopedResult<T>
    {

        public EnvelopedResult(T data)
        {
            this.data = data;
        }
        public EnvelopedResult(T data, ErrorResponse[] error)
        {
            this.data = data;
            this.errors = error;
        }

        public T data { get; set; }
        public ErrorResponse[] errors { get; set; }
    }
}
