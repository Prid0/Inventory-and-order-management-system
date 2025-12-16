using Microsoft.AspNetCore.Mvc;
using Pim.Utility;

namespace Product_And_Inventory_Mangement.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private readonly LoggedInUserId _loggedInUserId;

        protected BaseApiController(LoggedInUserId loggedInUserId)
        {
            _loggedInUserId = loggedInUserId;
        }

        protected int userId => _loggedInUserId.GetUserId();
    }
}
