using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenImis.Security.Security;
using OpenImis.ModulesV2;
using OpenImis.ModulesV2.ServiceModule;
using Newtonsoft.Json;

namespace OpenImis.RestApi.Controllers.V2 { 
    
    [ApiVersion("2")]
    [AllowAnonymous]
    [Route("api/")]
    [ApiController]

    public class ServiceController : Controller
    {
        public ServiceController()
        {
        }
        //[HasRights(Rights.DiagnosesDownload)]
        [HttpGet]   //To allow the sending of data that could be used by the consumers
        [Route("GetListServiceItems")]


        public IActionResult GetListServiceItems()
        {
            ClassServiceRequest getRequest = new ClassServiceRequest();
            var response = getRequest.SerializeDr();
            return Json(response);
        }

    }
}

/**************** Generated token to execute your Get Cheque until 08/08/2022 ********************************************************************************************
 * 
 * 
    "access_token": 
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyVVVJRCI6IjRhMWZiYjBlLWFhMzctNGRkYS04OTE0LTk3YzQ3YTViNzY2NSIsImV4cCI6MTY2MDEyNTAxNiwiaXNzIjoiaHR0cDovL29wZW5pbWlzLm9yZyIsImF1ZCI6Imh0dHA6Ly9vcGVuaW1pcy5vcmcifQ.nmr29RUSbLcFYKACXp1c8kaLZBmoZJagMWwLj6kN__A",
  "expires_on": "2022-08-10T10:50:16.8424129+01:00"

*   API Version to provide in this case: 2

*********************************************************************************************************************************************************/
