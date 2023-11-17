using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Web_API.Controllers
{
    [Route("session-status")]
    [ApiController]
    public class SessionStatusController : Controller
    {
        [HttpGet]
        public ActionResult SessionStatus([FromQuery] string session_id)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            return Json(new { status = session.RawJObject["status"], customer_email = session.RawJObject["customer_details"]["email"] });
        }
    }
}