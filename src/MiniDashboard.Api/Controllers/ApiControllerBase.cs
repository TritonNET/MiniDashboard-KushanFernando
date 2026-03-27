using Microsoft.AspNetCore.Mvc;

namespace MiniDashboard.Api.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected async Task<ActionResult<T>> HandleAsync<T>(Func<Task<T>> action)
        {
            try
            {
                var result = await action();
                return Ok(result);
            }
            catch (ArgumentNullException ex) 
            {
                return BadRequest(ex.Message); 
            }
            catch (ArgumentException ex) 
            { 
                return BadRequest(ex.Message); 
            }
            catch (InvalidOperationException ex) 
            { 
                return NotFound(ex.Message); 
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message); 
            }
        }

        protected async Task<ActionResult> HandleAsync(Func<Task<bool>> action)
        {
            try
            {
                bool ok = await action();
                return ok ? Ok() : NotFound();
            }
            catch (ArgumentNullException ex)
            { 
                return BadRequest(ex.Message); 
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message); 
            }
            catch (InvalidOperationException ex) 
            { 
                return NotFound(ex.Message); 
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message); 
            }
        }
    }
}
