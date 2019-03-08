using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;

namespace DirtBnBWebAPI.Controllers
{
    [Obsolete("This class only acts a basic template. Do not use it for actual API calls", false)]
    public class UserController : ApiController
    {
        [Route("api/users")]
        [HttpGet]
        public HttpResponseMessage GetUsers()
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            var users = userPersistenceService.GetUsers();
            HttpResponseMessage response;
            if (users == null || users.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No users found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, users);
            return response;
        }

        [Route("api/users/{id}")]
        [HttpGet]
        public HttpResponseMessage GetUser(long id, [FromBody] User userPasswordObject)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            User user = userPersistenceService.GetUser(id);
            HttpResponseMessage response;
            if (user == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "User not found.");
                return response;
            }

            if (userPasswordObject.password != user.password)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "Incorrect password. Please try logging again.");
                return response;
            }

            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [Route("api/users/")]
        [HttpPost]
        public HttpResponseMessage CreateUser([FromBody]User user)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            HttpResponseMessage response;

            if (string.IsNullOrEmpty(user.name) || string.IsNullOrEmpty(user.phoneNumber) || string.IsNullOrEmpty(user.emailAddress) || string.IsNullOrEmpty(user.password))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "All fields are mandatory. Please try again.");
                return response;
            }

            long id;
            id = userPersistenceService.SaveUser(user);
            user.userID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, user);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("users/{0}", id));
            return response;
        }

        [Route("api/users/{id}")]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage UpdateUser(long id, [FromBody]User user)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            bool userExists = false;
            userExists = userPersistenceService.UpdateUser(id, user);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, user);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "User not found.");
                return response;
            }
        }

        [Route("api/users/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteUser(long id)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            bool userExists = false;
            userExists = userPersistenceService.DeleteUser(id);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "User deleted.");
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "User not found.");
                return response;
            }
        }
    }
}