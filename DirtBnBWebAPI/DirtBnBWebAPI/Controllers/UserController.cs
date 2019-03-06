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
        public User Get(long id)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            User user = userPersistenceService.GetUser(id);
            return user;
        }

        [Route("api/users/")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]User user)
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
        public void Put(int id, [FromBody]User user)
        {
        }

        [Route("api/users/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}