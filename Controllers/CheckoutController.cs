using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")]
    public class CheckoutController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private static string s_wasmClientURL = string.Empty;

        public CheckoutController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult> CheckoutOrder([FromBody] List<CheckoutProductVM> products, [FromServices] IServiceProvider sp)
        {
            var referer = Request.Headers.Referer;
            s_wasmClientURL = referer[0];

            // Build the URL to which the customer will be redirected after paying.
            var server = sp.GetRequiredService<IServer>();

            var serverAddressesFeature = server.Features.Get<IServerAddressesFeature>();

            string? thisApiUrl = null;

            if (serverAddressesFeature is not null)
            {
                thisApiUrl = serverAddressesFeature.Addresses.FirstOrDefault();
            }

            if (thisApiUrl is not null)
            {
                var sessionId = await CheckOut(products, thisApiUrl);
                if (sessionId == null)
                {
                    return BadRequest(new { status = "error", message = "please enter a valid input" });
                }
                var pubKey = _configuration["Stripe:PubKey"];

                var checkoutOrderResponse = new CheckoutOrderResponse()
                {
                    SessionId = sessionId,
                    PubKey = pubKey
                };

                return Ok(checkoutOrderResponse);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [NonAction]
        public async Task<string> CheckOut(List<CheckoutProductVM> products, string thisApiUrl)
        {
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in products)
            {
                var product = await _unitOfWork.Product.Get(x => x.Id == item.ProductId);
                var variation = await _unitOfWork.ProductVariant.Get(x => x.Id == item.ProductVariantId && x.ProductId == item.ProductId);

                if (product == null || variation == null)
                {
                    return null;
                }
                if (variation.Qty < item.Qty)
                {
                    return null;
                }

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long?)variation.Price * item.Qty,
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Title,
                            Description = product.Description,
                            Images = new List<string> { product.ImageUrl }
                        },
                    },
                    Quantity = item.Qty,
                });
            }

            // after making the pay, remove these products from the db
            var removing = await ReduceProductFromDb(products);
            if (!removing)
            {
                return null;
            }

            _unitOfWork.Save();
            var options = new SessionCreateOptions
            {
                // Stripe calls the URLs below when certain checkout events happen such as success and failure.
                SuccessUrl = $"{thisApiUrl}/checkout/success?sessionId=" + "{CHECKOUT_SESSION_ID}", // Customer paid.
                CancelUrl = s_wasmClientURL + "failed",  // Checkout cancelled.
                PaymentMethodTypes = new List<string> // Only card available in test mode?
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment" // One-time payment. Stripe supports recurring 'subscription' payments.
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Id;
        }

        private async Task<bool> ReduceProductFromDb(List<CheckoutProductVM> products)
        {
            foreach (var item in products)
            {
                var variation = await _unitOfWork.ProductVariant.Get(x => x.Id == item.ProductVariantId && x.ProductId == item.ProductId);
                if (variation == null)
                {
                    return false;
                }
                variation.Qty -= item.Qty;
            }
            return true;
        }


        [HttpGet("success")]
        // Automatic query parameter handling from ASP.NET.
        // Example URL: https://localhost:7051/checkout/success?sessionId=si_123123123123
        public ActionResult CheckoutSuccess(string sessionId)
        {
            var sessionService = new SessionService();
            var session = sessionService.Get(sessionId);

            // Here you can save order and customer details to your database.
            var total = session.AmountTotal.Value;
            var customerEmail = session.CustomerDetails.Email;

            return Redirect(s_wasmClientURL + "success");
        }

    }
}