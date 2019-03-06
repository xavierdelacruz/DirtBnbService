using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;

namespace DirtBnBWebAPI.Controllers
{
    public class UserController : ApiController
    {
        [Route("api/users")]
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            return userPersistenceService.GetUsers();
        }

        [Route("api/users/{id}")]
        [HttpGet]
        public User GetUser(long id)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            User user = userPersistenceService.GetUser(id);
            return user;
        }

        [Route("api/users/")]
        [HttpPost]
        public HttpResponseMessage CreateUser([FromBody]User user)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            long id;
            id = userPersistenceService.SaveUser(user);
            user.userID = id;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("users/{0}", id));
            return response;
        }

        [Route("api/users/{id}")]
        [HttpPut]
        public HttpResponseMessage UpdateUser(long id, [FromBody]User user)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            bool userExists = false;
            userExists = userPersistenceService.UpdateUser(id, user);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
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
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
        }
    }
}