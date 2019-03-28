using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GuestController : ApiController
    {
        [Route("api/guests")]
        [HttpGet]
        public HttpResponseMessage GetGuests()
        {
            GuestPersistenceService guestPersistenceService = new GuestPersistenceService();
            var guests = guestPersistenceService.GetGuests();
            HttpResponseMessage response;
            if (guests == null || guests.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No Guests found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, guests);
            return response;
        }

        [Route("api/guests/{id}/{password}")]
        [HttpGet]
        public HttpResponseMessage GetGuest(long id, string password)
        {
            GuestPersistenceService guestPersistenceService = new GuestPersistenceService();
            Guest guest = guestPersistenceService.GetGuest(id);
            HttpResponseMessage response;
            if (guest == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found.");
                return response;
            }

            if (password != guest.password)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "Incorrect password. Please try logging again.");
                return response;
            }

            return Request.CreateResponse(HttpStatusCode.OK, guest);
        }

        [Route("api/guests/")]
        [HttpPost]
        public HttpResponseMessage CreateGuest([FromBody]Guest guest)
        {
            GuestPersistenceService guestPersistenceService = new GuestPersistenceService();
            HttpResponseMessage response;

            if (string.IsNullOrEmpty(guest.name)
                || string.IsNullOrEmpty(guest.phoneNumber)
                || string.IsNullOrEmpty(guest.emailAddress)
                || string.IsNullOrEmpty(guest.password))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "All fields are mandatory. Please try again.");
                return response;
            }

            var id = guestPersistenceService.SaveGuest(guest);
            if (id < 0)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "A user with the same email address has already been created.");
                return response;
            }
            guest.userID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, guest);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("guests/{0}", id));
            return response;
        }

        // TODO: UPDATE USER BASED ON ID WITH PASSWORD CHECK

        [Route("api/guests/{id}")]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage UpdateGuest(long id, [FromBody]Guest guest)
        {
            GuestPersistenceService guestPersistenceService = new GuestPersistenceService();
            bool userExists = false;
            HttpResponseMessage response;
            try
            {
                userExists = guestPersistenceService.UpdateGuest(id, guest); if (userExists)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, guest);
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found.");
                    return response;
                }
            }
            catch (MySqlException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "The email is already in use. Please try again.");
                return response;
            }       
        }

        [Route("api/guests/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteHost(long id)
        {
            GuestPersistenceService guestPersistenceService = new GuestPersistenceService();
            bool userExists = false;
            userExists = guestPersistenceService.DeleteGuest(id);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "Guest deleted.");
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found.");
                return response;
            }
        }

        [Route("api/guests/powerusers")]
        [HttpGet]
        public HttpResponseMessage GetGuestsReservedAllAccommodations()
        {
            GuestPersistenceService guestPersistenceService = new GuestPersistenceService();
            var guests = guestPersistenceService.GetGuestsStayedAllAccommodations();
            HttpResponseMessage response;
            if (guests == null || guests.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No guests found that have reserved all accommodations.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, guests);
            return response;
        }
    }
}