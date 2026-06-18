using Training_Api.Exceptions;

namespace Training_Api.Middleware
{
    public class MyExceptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (NotFoundExceptions ex)
            {
                context.Response.StatusCode = 404;
                
                
            }
            catch (UnAuthorizeExceptions ex)
            {
            }
            catch (BadRequestExceptions ex)
            {
            }
            catch (ConflictExceptions ex) 
            {
            }
    }
}
