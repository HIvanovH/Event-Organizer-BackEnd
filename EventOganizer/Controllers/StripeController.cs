using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace EventOganizer.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class StripeController : ControllerBase
        {
            [HttpPost("CreatePaymentIntent")]
            public ActionResult CreatePaymentIntent([FromBody] Dictionary<string, int> data)
            {
                int amount = data["amount"];

                if (amount <= 0)
                {
                    return BadRequest("Invalid amount.");
                }
            amount *=100;
                StripeConfiguration.ApiKey = "sk_test_51NmMkNA82S8GS4aYkW1GcDoQiy30bNoDaG9975FOqB7bzbTqrF9KAe5HCO0dNRwtnLp0hRYztD1i9I25ygcK9pNh00CFrsdMq6";

                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "bgn",
                    PaymentMethodTypes = new List<string> { "card" },
                };

                var service = new PaymentIntentService();
                PaymentIntent paymentIntent = service.Create(options);

                return Ok(new { clientSecret = paymentIntent.ClientSecret });
            }


            [HttpPost("PaymentSuccess")]
            public ActionResult PaymentSuccess([FromBody] Dictionary<string, string> json)
            {
                var paymentIntentId = json["paymentIntentId"];
                
                return Ok();
            }
        }
    }

