﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ImisRestApi.Chanels.Sms;
using ImisRestApi.Data;
using ImisRestApi.Escape.Models;
using ImisRestApi.Models;
using ImisRestApi.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ImisRestApi.Controllers
{
    public class PoliciesController : PoliciesBaseController
    {
        private ImisPolicy policies;

        private IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;


        public PoliciesController(IConfiguration configuration, IHostingEnvironment hostingEnvironment) : base(configuration)
        {
            policies = new ImisPolicy(configuration);

            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Policies/Renew_Policy")]
        public IActionResult Renew_PolicyChf([FromBody] ChfPolicy model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
                return BadRequest(new { success = false, message = error });
            }

            Policy policy = new Policy()
            {
                InsuranceNumber = model.InsuranceNumber,
                EnrollmentOfficerCode = model.OfficerCode,
                ProductCode = model.ProductCode,
                Date = DateTime.UtcNow.ToString("yyyy/MM/dd")
            };

            JsonResult resp = (JsonResult)base.Renew_Policy(policy);
            DataMessage message = (DataMessage)resp.Value;

            var response = new RenewPolicyResponse(message.Code, false, 0).Message;

            if (!response.ErrorOccured)
            {

                return Ok(new { success = true, message = response.MessageValue });
            }
            else
            {
                return BadRequest(new { success = false, message = response.MessageValue });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Policies/Get_CommissionsChf")]
        public IActionResult Get_CommissionsChf([FromBody] ChfGetCommissionInputs model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
                return BadRequest(new { success = false, message = error });
            }

            GetCommissionInputs commision = new GetCommissionInputs()
            {
                enrolment_officer_code = model.officer_code,
                insrance_product_code = model.product_code,
                mode = CommissionMode.Paid,
                year = model.year,
                month = model.month,
                payer = model.payer
            };

            JsonResult resp = (JsonResult)base.Get_Commissions(commision);
            DataMessage message = (DataMessage)resp.Value;

            var commissionString = JsonConvert.SerializeObject(message.Data);
            var commissionAmount = JsonConvert.DeserializeObject<List<CommissionAmount>>(commissionString).FirstOrDefault();


            var response = new GetCommissionResponse(message.Code, false, 0).Message;

            Language language = Language.Secondary;

            if (model.language == 2)
            {
                language = Language.Primary;
            }

            ImisSms sms = new ImisSms(_configuration, _hostingEnvironment, language);

            var txtmsgTemplate = sms.GetMessage("CommissionInformSms");


            var txtmsg = string.Format(txtmsgTemplate,
                     new DateTime(Convert.ToInt32(model.year), Convert.ToInt32(model.month), 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("en")),
                     model.year,
                     commissionAmount.Amount
                     );


            sms.QuickSms(txtmsg, model.msisdn);

            if (!response.ErrorOccured)
            {

                return Ok(new { success = true, message = response.MessageValue });
            }
            else
            {
                return BadRequest(new { success = false, message = response.MessageValue });
            }

        }

        [NonAction]
        public override IActionResult Renew_Policy([FromBody] Policy model)
        {
            return base.Renew_Policy(model);
        }


    }
}