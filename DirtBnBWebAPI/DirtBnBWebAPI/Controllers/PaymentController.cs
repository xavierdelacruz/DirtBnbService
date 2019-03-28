using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;
using System.Web.Http.Cors;

namespace DirtBnBWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PaymentController : ApiController
    {
        [Route("api/payments")]
        [HttpGet]
        public HttpResponseMessage GetPayments()
        {
            PaymentPersistenceService PaymentPersistenceService = new PaymentPersistenceService();
            var Payments = PaymentPersistenceService.GetPayments();
            HttpResponseMessage response;
            if (Payments == null || Payments.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No Payments found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Payments);
            return response;
        }

        [Route("api/Payments/{id}")]
        [HttpGet]
        public HttpResponseMessage GetPayment(long id)
        {
            PaymentPersistenceService PaymentPersistenceService = new PaymentPersistenceService();
            Payment Payment = PaymentPersistenceService.GetPayment(id);
            HttpResponseMessage response;
            if (Payment == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Payment not found.");
                return response;
            }
            return Request.CreateResponse(HttpStatusCode.OK, Payment);
        }

        [Route("api/Payments/")]
        [HttpPost]
        public HttpResponseMessage CreatePayment([FromBody]Payment Payment)
        {
            PaymentPersistenceService PaymentPersistenceService = new PaymentPersistenceService();
            HttpResponseMessage response;

            if (String.IsNullOrEmpty(Payment.CCNumber))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "CCNumber is a required field. Please try again.");
            }
            // TODO: sanity checking on CCNumber, make sure its all digits
            
            long id = PaymentPersistenceService.SavePayment(Payment);
            if (id == -1)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Ensure referential integrity. Please try again.");
                return response;
            }

            Payment.paymentID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, Payment);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("Payments/{0}", id));
            return response;
        }
    }
}