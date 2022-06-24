using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenImis.Security.Security;
using OpenImis.ModulesV1;
using OpenImis.ModulesV1.ChequeModule;

namespace OpenImis.RestApi.Controllers.V1 { 
    
    [ApiVersion("1")]
    [Route("api/")]
    [ApiController]

    public class ChequeController : Controller
    {
        public ChequeController()
        {
        }
        [HasRights(Rights.DiagnosesDownload)]
        [HttpGet]   //To allow the sending of data that could be used by the consumers
        [Route("GetListChequeItems")]
        //[ProducesResponseType(typeof(void), 200)]
        //[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]

        public IActionResult GetListChequeItems()
        {
            ClassChequeRequest getRequest = new ClassChequeRequest();
            var response = getRequest.GetdataFromSQLDB();
            return Json(response);
        }

    }
}

