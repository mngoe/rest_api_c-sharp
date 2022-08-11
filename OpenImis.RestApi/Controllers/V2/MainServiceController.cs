using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenImis.Security.Security;
using OpenImis.ModulesV2;
using OpenImis.ModulesV2.MainServiceModule;
using Newtonsoft.Json;

namespace OpenImis.RestApi.Controllers.V2 { 
    
    [ApiVersion("2")]
    [AllowAnonymous]
    [Route("api/")]
    [ApiController]

    public class MainServiceController : Controller
    {
        public MainServiceController()
        {
        }
        //[HasRights(Rights.DiagnosesDownload)]
        [HttpGet]   //To allow the sending of data that could be used by the consumers
        [Route("GetListMainServiceItems")]


        public IActionResult GetListMainServiceItems()
        {
            ClassMainServiceRequest getRequest = new ClassMainServiceRequest();
            var response = getRequest.SerializeDr();
            return Json(response);
        }

    }
}
