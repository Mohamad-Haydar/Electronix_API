using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/create-payment-intent")]
    // [ApiExplorerSettings(IgnoreApi = true)]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")]
    public class PaymentIntentApiController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentIntentApiController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create(List<CheckoutProductVM> products)
        {
            var amount = await CalculateOrderAmount(products);
            if (amount == -1)
            {
                return BadRequest();
            }
            var paymentIntentService = new PaymentIntentService();

            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });

            return Json(new { clientSecret = paymentIntent.ClientSecret });
        }

        private async Task<long> CalculateOrderAmount(List<CheckoutProductVM> products)
        {
            var lineItems = new List<SessionLineItemOptions>();
            long amount = 0;
            double itemPrcie = 0;
            foreach (var item in products)
            {
                var product = await _unitOfWork.Product.Get(x => x.Id == item.ProductId);
                var variation = await _unitOfWork.ProductVariant.Get(x => x.Id == item.ProductVariantId && x.ProductId == item.ProductId);

                if (product == null || variation == null)
                {
                    return -1;
                }
                if (variation.Qty < item.Qty)
                {
                    return -1;
                }
                itemPrcie = (1 - (product.Discount / 100)) * variation.Price * item.Qty * 100;
                amount += (long)itemPrcie;
            }

            // after making the pay, remove these products from the db
            var removing = await ReduceProductFromDb(products);
            if (!removing)
            {
                return -1;
            }

            _unitOfWork.Save();
            return amount;
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
    }
}