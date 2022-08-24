using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenImis.Security.Security;
using OpenImis.ModulesV2;
using OpenImis.ModulesV2.ChequeModule;
using Newtonsoft.Json;

namespace OpenImis.RestApi.Controllers.V2 { 
    
    [ApiVersion("2")]
    [AllowAnonymous]
    [Route("api/")]
    [ApiController]

    public class ChequeController : Controller
    {
        public ChequeController()
        {
        }
        //[HasRights(Rights.DiagnosesDownload)]
        [HttpGet]   //To allow the sending of data that could be used by the consumers
        [Route("GetListChequeItems")]
        public IActionResult GetListChequeItems()
        {
            ClassChequeRequest getRequest = new ClassChequeRequest();
            var response = getRequest.SerializeDr();
            return Json(response);
        }

    }
}