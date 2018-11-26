﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImisRestApi.Chanels;
using ImisRestApi.Chanels.Payment.Models;
using ImisRestApi.Data;
using ImisRestApi.Models;
using ImisRestApi.Models.Payment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ImisRestApi.Controllers
{
    public class PaymentController : PaymentBaseController
    {
        private ImisPayment p;
        public PaymentController(IConfiguration configuration, IHostingEnvironment hostingEnvironment) :base(configuration, hostingEnvironment)
        {
            p = new ImisPayment(configuration, hostingEnvironment);
        }

        [HttpPost]
        [Route("api/GetControlNumber/Single")]
        public async Task<IActionResult> Index([FromBody]IntentOfSinglePay payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            payment.SetDetails();

            var response = await _payment.SaveIntent(payment);

            return Ok(new { error_occured = response.ErrorOccured, error_message = response.MessageValue, control_number = response.Data });
        }

        [HttpPost]
        [Route("api/GetReconciliationData")]
        public IActionResult GetReconciliation([FromBody]GepgReconcMessage model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(p.ReconciliationResp());
        }

        [NonAction]
        public override IActionResult GetReqControlNumber([FromBody] ControlNumberResp model)
        {
            return base.GetReqControlNumber(model);
        }

        [NonAction]
        public override IActionResult GetPaymentData([FromBody] PaymentData model)
        {
            return base.GetPaymentData(model);
        }

        [NonAction]
        public override IActionResult PostReqControlNumberAck([FromBody] Acknowledgement model)
        {
            return base.PostReqControlNumberAck(model);
        }

        [HttpPost]
        [Route("api/GetPaymentData")]
        public IActionResult GetPaymentChf([FromBody]GepgPaymentMessage model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<PymtTrxInf> payments = model.PymtTrxInf;
            foreach (var payment in payments)
            {
                PaymentData pay = new PaymentData() {
                    PaymentId = payment.BillId,
                    ControlNumber = payment.PayCtrNum.ToString(),
                    ProductCode = payment.PayRefId,
                    EnrolmentOfficerCode = payment.PyrName,
                    TransactionId = payment.TrxId,
                    ReceivedAmount = Convert.ToDouble(payment.BillAmt),
                    ReceivedDate = payment.PaymentDate,
                    PaymentDate = payment.PaymentDate,
                    PaymentOrigin = payment.PaymentOrigin,
                    ReceiptNumber = payment.PspReceiptNumber,
                    PhoneNumber = payment.PyrCellNum.ToString()
                };

                base.GetPaymentData(pay);

            }

            return Ok(p.PaymentResp());
        }

        [HttpPost]
        [Route("api/GetReqControlNumber")]
        public IActionResult GetReqControlNumberChf([FromBody] GepgBillResponse model)
         {
            foreach (var bill in model.BillTrxRespInf)
            {
                ControlNumberResp ControlNumberResponse = new ControlNumberResp()
                {
                    PaymentId = bill.BillId,
                    ControlNumber = bill.PayCntrNum.ToString(),
                    ErrorOccured = false,
                    ErrorMessage = bill.TrxStsCode
                };

                try
                {
                    var response = base.GetReqControlNumber(ControlNumberResponse);
                }
                catch (Exception e)
                {

                    throw e;
                }
            }

            return Ok(p.ControlNumberResp());
        }
    }
}