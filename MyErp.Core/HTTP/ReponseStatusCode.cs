using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace MyErp.Core.HTTP
{

    public class ResponseStatusCode<T>
    {
        public static IActionResult GetApiResponseCode(MainResponse<T> result, string mthod)
        {
            if (result.rejectedObjects?.Count > 0 && result.errors?.Count > 0 && result.acceptedObjects?.Count > 0)
            {
                /*return new BadRequestObjectResult(result);*/
                return new ObjectResult(result.Serialize()) { StatusCode = 300 };
            }
            if (result.rejectedObjects?.Count > 0 && result.errors?.Count > 0 && result.acceptedObjects?.Count == 0)
            {
                /*return new BadRequestObjectResult(result);*/
                return new ObjectResult(result.Serialize()) { StatusCode = 303 };
            }
            else if (result.acceptedObjects?.Count == 0 && result.rejectedObjects?.Count == 0 && result.errors?.Count > 0)
            {
                return new NotFoundObjectResult(result.Serialize());
            }
            else if (result.acceptedObjects?.Count == 0 && result.rejectedObjects?.Count == 0 && result.errors?.Count == 0)
            {
                return new ObjectResult(result.Serialize()) { StatusCode = 405 };
            }
            else
            {
                return new ObjectResult(result.Serialize());
            }
        }
    }
}
