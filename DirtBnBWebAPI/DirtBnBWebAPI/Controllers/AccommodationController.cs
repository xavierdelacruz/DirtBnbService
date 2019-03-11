using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.Controllers
{
    public class AccommodationController : ApiController
    {
        [Route("api/accommodations")]
        [HttpGet]
        public HttpResponseMessage GetAccomodations()
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            var accommodations = accommodationPersistenceService.GetAccommodations();
            HttpResponseMessage response;
            if (accommodations == null || accommodations.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No acc found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, accommodations);
            return response;
        }

        [Route("api/accommodations/{id}")]
        [HttpGet]
        public HttpResponseMessage GetHost(long id)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            Accommodation accommodation = accommodationPersistenceService.GetAccommodation(id);
            HttpResponseMessage response;
            if (accommodation == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Accommodation not found.");
                return response;
            }
            return Request.CreateResponse(HttpStatusCode.OK, accommodation);
        }

        [Route("api/accommodations/")]
        [HttpPost]
        public HttpResponseMessage CreateAccommodation([FromBody]Accommodation accommodation)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            HttpResponseMessage response;

            if (accommodation.parking == null
                || accommodation.wifi == null
                || accommodation.tv == null
                || accommodation.airConditioning == null
                || accommodation.generalAppliances == null
                || string.IsNullOrEmpty(accommodation.bedSize)
                || accommodation.pricePerNight.Equals(0)
                || string.IsNullOrEmpty(accommodation.houseNumber)
                || accommodation.hostUserID == null
                || string.IsNullOrEmpty(accommodation.postalCode)
                || string.IsNullOrEmpty(accommodation.city)
                || string.IsNullOrEmpty(accommodation.street)
                || string.IsNullOrEmpty(accommodation.province))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "All fields are mandatory. Please try again.");
                return response;
            }

            var id = accommodationPersistenceService.SaveAccommodation(accommodation);
            if (id < 0)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "A user with the same email address has already been created");
                return response;
            }
            accommodation.accommodationID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, accommodation);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("accommodations/{0}", id));
            return response;
        }

        [Route("api/accommodations/{id}")]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage UpdateAccommodation(long id, [FromBody]Accommodation accommodation)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            bool accommodationExists = false;
            HttpResponseMessage response;

            accommodationExists = accommodationPersistenceService.UpdateAccommodation(id, accommodation);

            if (accommodationExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, accommodation);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Accommodation not found.");
                return response;
            }
        }

        [Route("api/accommodations/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteHost(long id)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            bool userExists = false;
            userExists = accommodationPersistenceService.DeleteAccommodation(id);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "Accommodation deleted.");
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Accommodation not found.");
                return response;
            }
        }
    }
}