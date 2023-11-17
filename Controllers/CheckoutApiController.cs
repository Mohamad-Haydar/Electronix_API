using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Web_API.Controllers
{
    [Route("create-checkout-session")]
    [ApiController]
    public class CheckoutApiController : Controller
    {
        [HttpPost]
        public ActionResult Create()
        {
            var domain = "http://localhost:3000";
            var options = new SessionCreateOptions
            {
                UiMode = "embedded",
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = "{{PRICE_ID}}",
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                ReturnUrl = domain + "/return?session_id={CHECKOUT_SESSION_ID}"
            };
            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { clientSecret = session.RawJObject["client_secret"] });
        }
    }
}