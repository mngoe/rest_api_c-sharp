﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using OpenImis.ePayment.Escape;
using OpenImis.ePayment.Escape.Payment.Models;
using OpenImis.ePayment.Data;
using OpenImis.ePayment.Extensions;
using OpenImis.ePayment.Formaters;
using OpenImis.ePayment.Models;
using OpenImis.ePayment.Models.Payment;
using OpenImis.ePayment.Models.Payment.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenImis.ePayment.Responses;
using Microsoft.Extensions.Logging;
using OpenImis.ePayment.QueueSystem;

namespace OpenImis.ePayment.Controllers
{
    [ApiVersion("3")]
    public class PaymentController : PaymentBaseController
    {
        private ImisPayment imisPayment;
        private IHostingEnvironment env;
        private readonly BackgroundWorkerQueue _backgroundWorkerQueue;
        private readonly GepgFileRequestLogger _gepgFileLogger;
        private readonly ILogger _logger;

        public PaymentController(IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory, BackgroundWorkerQueue backgroundWorkerQueue) : base(configuration, hostingEnvironment, loggerFactory)
        {
            env = hostingEnvironment;
            _backgroundWorkerQueue = backgroundWorkerQueue;
            imisPayment = new ImisPayment(configuration, hostingEnvironment, loggerFactory);
            _gepgFileLogger = new GepgFileRequestLogger(hostingEnvironment, loggerFactory);
            _logger = loggerFactory.CreateLogger<PaymentController>();
        }

#if CHF

        #region Step 1: Control Number Management 

        [HttpPost]
        [Route("api/GetControlNumber")]
        [ProducesResponseType(typeof(GetControlNumberResp), 200)]
        [ProducesResponseType(typeof(ErrorResponseV2), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CHFRequestControlNumberForMultiplePolicies([FromBody] IntentOfPay intent)
        {
            return await base.GetControlNumber(intent);
        }

        [HttpPost]
        [Route("api/GetControlNumber/Single")]
        public override async Task<IActionResult> CHFRequestControlNumberForSimplePolicy([FromBody]IntentOfSinglePay intent)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
                return BadRequest(new { success = false, message = error, error_occured = true, error_message = error });
            }

            intent.phone_number = intent.Msisdn;
            intent.enrolment_officer_code = intent.OfficerCode;
            intent.SmsRequired = true;

            if (String.IsNullOrEmpty(intent.enrolment_officer_code))
                intent.EnrolmentType = EnrolmentType.Renewal + 1;

            intent.SetDetails();

            var result = await base.GetControlNumber(intent);

            // Check if the product requested has enough CNs left
            try
            {
                var count = await _payment.ControlNumbersToBeRequested(intent.ProductCode);
                if (count > 0)
                {
                    // The following method is Async, but we do not await it since we don't want to wait for the result
                     _ = RequestBulkControlNumbers(new RequestBulkControlNumbersModel { ControlNumberCount = count, ProductCode = intent.ProductCode });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in CHFRequestControlNumberForSimplePolicy");
            }
            return result;
        }

        [HttpPost]
        [Route("api/GetReqControlNumber")]
        public async Task<IActionResult> GePGReceiveControlNumber([FromBody] gepgBillSubResp model)
        {
            if (!ModelState.IsValid)
                return BadRequest(imisPayment.ControlNumberResp(GepgCodeResponses.GepgResponseCodes["Invalid Request Data"]));

            int billId;
            if (model.HasValidSignature)
            {
                ControlNumberResp ControlNumberResponse;
                foreach (var bill in model.BillTrxInf)
                {
                    ControlNumberResponse = new ControlNumberResp()
                    {
                        internal_identifier = bill.BillId,
                        control_number = bill.PayCntrNum,
                        error_occured = bill.TrxStsCode == GepgCodeResponses.GepgResponseCodes["Successful"].ToString()?false:true,
                        error_message = bill.TrxStsCode
                    };
                
                    billId = bill.BillId;

                    string reconc = JsonConvert.SerializeObject(ControlNumberResponse);
                    _gepgFileLogger.Log(billId, "CN_Response", reconc);

                    try
                    {
                        var response = base.GetReqControlNumber(ControlNumberResponse);
                        if (ControlNumberResponse.error_occured == true)
                        {
                            var rejectedReason = imisPayment.PrepareRejectedReason(billId, bill.TrxStsCode);
                            imisPayment.setRejectedReason(billId, rejectedReason);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error in GePGReceiveControlNumber");
                        throw;
                    }
                }

                return Ok(imisPayment.ControlNumberResp(GepgCodeResponses.GepgResponseCodes["Successful"]));
            }
            else
            {
                foreach (var bill in model.BillTrxInf)
                {
                    billId = bill.BillId;

                    string reconc = JsonConvert.SerializeObject(model);
                    _gepgFileLogger.Log(billId, "CN_Response_InvalidSignature", reconc);
                    imisPayment.setRejectedReason(billId, GepgCodeResponses.GepgResponseCodes["Invalid Signature"] + ":Invalid Signature");
                }
                
                return Ok(imisPayment.ControlNumberResp(GepgCodeResponses.GepgResponseCodes["Invalid Signature"]));
            }

        }

        [NonAction]
        public override Task<IActionResult> GetControlNumber([FromBody] IntentOfPay intent)
        {
            return base.GetControlNumber(intent);
        }

        [NonAction]
        public override Task<IActionResult> GetReqControlNumber([FromBody] ControlNumberResp model)
        {
            return base.GetReqControlNumber(model);
        }

        [NonAction]
        public override Task<IActionResult> PostReqControlNumberAck([FromBody] Acknowledgement model)
        {
            return base.PostReqControlNumberAck(model);
        }

        #endregion

        #region Step 2 (optional): Cancel Payment 
        [HttpPost]
        [Route("api/payment/cancel")]
        [ProducesResponseType(typeof(DataMessage), 200)]
        [ProducesResponseType(typeof(ErrorResponseV2), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CHFCancelOnePayment([FromBody] PaymentCancelModel model)
        {
            ImisPayment payment = new ImisPayment(_configuration, _hostingEnvironment, _loggerFactory);
            DataMessage dt = new DataMessage();

            if (model.payment_id !=0 || model.control_number != null)
            {
                // if payment_id is in payload it will be used else it will be calculated from CN
                int paymentId = model.payment_id != 0 ? model.payment_id : payment.GetPaymentId(model.control_number);

                model.payment_id = paymentId;

                if (paymentId != 0)
                {
                    var ack = await payment.GePGPostCancelPayment(paymentId);

                    if (ack.GetType() == typeof(DataMessage))
                    {
                        return Ok((DataMessage)ack);
                    }

                    GePGPaymentCancelResponse response = (GePGPaymentCancelResponse)ack;

                    if (response.gepgBillCanclResp.BillCanclTrxDt.Count() > 0 && 
                        response.gepgBillCanclResp.BillCanclTrxDt[0].TrxSts == TrxSts.Success)
                    {
                        return await base.CancelPayment(model);
                    }
                    else
                    {
                        return Ok(new DataMessage
                        {
                            Data = response,
                            Code = 3,
                            ErrorOccured = true,
                            MessageValue = "CancelPayment:3:Failed to cancel"
                        });
                    }
                }
                else
                {
                    //Todo: move hardcoded message to translation file
                    return Ok(new DataMessage
                    {
                        Code = 2,
                        ErrorOccured = true,
                        MessageValue = "CancelPayment:2:Control Number doesn't exists",
                    });
                }
            }
            
            return Ok(new DataMessage
                {
                    Code = 1,
                    ErrorOccured = true,
                    MessageValue = "CancelPayment:1:Missing Control Number",
                });
            
        }

        [NonAction]
        public override async Task<IActionResult> CancelPayment([FromBody] PaymentCancelBaseModel model)
        {
            return await base.CancelPayment(model);
        }

        #endregion

        #region Step 3: Payment Reception Managent
        [HttpPost]
        [Route("api/GetPaymentData")]
        public async Task<IActionResult> GetPaymentChf([FromBody] gepgPmtSpInfo model)
        {
            int billId;
            if (model.HasValidSignature)
            {
                if (!ModelState.IsValid)
                    return BadRequest(imisPayment.PaymentResp(GepgCodeResponses.GepgResponseCodes["Invalid Request Data"]));

                object _response = null;

                foreach (var payment in model.PymtTrxInf)
                {

                    PaymentData pay = new PaymentData()
                    {
                        control_number = payment.PayCtrNum,
                        //insurance_product_code = payment.PayRefId,
                        enrolment_officer_code = payment.PyrName,
                        transaction_identification = payment.TrxId,
                        received_amount = Convert.ToDouble(payment.PaidAmt),
                        received_date = DateTime.UtcNow.ToString("yyyy/MM/dd"),
                        payment_date = payment.TrxDtTm,
                        payment_origin = payment.PspName,
                        receipt_identification = payment.PayRefId,
                        payer_phone_number = payment.PyrCellNum
                    };

                    billId = payment.BillId;

                    string reconc = JsonConvert.SerializeObject(_response);
                    _gepgFileLogger.Log(billId, "Payment", reconc);


                    _backgroundWorkerQueue.QueueBackgroundWorkerItem(async token =>
                    {
                        await base.GetPaymentData(pay);
                    });
                    // _response = await base.GetPaymentData(pay);

                }               

                return Ok(imisPayment.PaymentResp(GepgCodeResponses.GepgResponseCodes["Successful"]));
            }
            else
            {
                foreach (var payment in model.PymtTrxInf)
                {
                    billId = payment.BillId;

                    string reconc = JsonConvert.SerializeObject(model);
                    _gepgFileLogger.Log(billId, "PaymentInvalidSignature", reconc);
                    imisPayment.setRejectedReason(billId, GepgCodeResponses.GepgResponseCodes["Invalid Signature"] + ":Invalid Signature");

                }

                return Ok(imisPayment.PaymentResp(GepgCodeResponses.GepgResponseCodes["Invalid Signature"]));
            }

        }

        [NonAction]
        public override async Task<IActionResult> GetPaymentData([FromBody] PaymentData model)
        {
            return await base.GetPaymentData(model);
        }
        #endregion

        #region Step 4: Payments Reconciliation Managent
        [HttpGet]
        [Route("api/Reconciliation")]
        public async Task<IActionResult> Reconciliation(int daysAgo)
        {
            List<object> done = new List<object>();
            // Make loop for all product from database that have account follow SP[0-9]{3} and do the function for all sp codes
            var productsSPCodes = imisPayment.GetProductsSPCode();
            if (productsSPCodes.Count > 0)
            {
                foreach (String productSPCode in productsSPCodes)
                {
                    var result = await imisPayment.RequestReconciliationReportAsync(daysAgo, productSPCode);
                    //check if we have done result - if no - then return 500
                    System.Reflection.PropertyInfo pi = result.GetType().GetProperty("resp");
                    done.Add(result);
                }
            }
            else
            {
                //return not found - no sp codes to proceed 
                return NotFound();
            }
            return Ok(done);
        }
        
        [HttpPost]
        [Route("api/GetReconciliationData")]
        public IActionResult GetReconciliation([FromBody] gepgSpReconcResp model)
        {
            if (model.HasValidSignature) 
            { 
                if (!ModelState.IsValid)
                    return BadRequest(imisPayment.ReconciliationResp(GepgCodeResponses.GepgResponseCodes["Invalid Request Data"]));

                string reconc = JsonConvert.SerializeObject(model);
                _gepgFileLogger.Log("Reconc_Data", reconc);
                
                foreach (var recon in model.ReconcTrxInf)
                {
                    var paymentToCompare = imisPayment.GetPaymentToReconciliate(recon);
                    if (paymentToCompare != null)
                    {
                        int paymentStatus = (int)paymentToCompare.GetType().GetProperty("paymentStatus").GetValue(paymentToCompare);
                        if (paymentStatus <= PaymentStatus.Received)
                        {
                            imisPayment.updateReconciliatedPaymentAsync(recon.SpBillId, model.ReconcBatchInfo.SpReconcReqId);
                            //TODO update policy
                        }
                        else if (paymentStatus == PaymentStatus.Matched)
                        {
                            imisPayment.MatchPayment(new MatchModel
                            {
                                internal_identifier = Convert.ToInt32(recon.SpBillId),
                                audit_user_id = 0
                            });
                        }
                    }
                    else
                    {
                        //send error if payment from GePG not found in IMIS
                        if (imisPayment.CheckPaymentExistError(recon.SpBillId))
                        {
                            imisPayment.updateReconciliatedPaymentError(recon.SpBillId, model.ReconcBatchInfo.SpReconcReqId);
                            imisPayment.setRejectedReason(int.Parse(recon.SpBillId), GepgCodeResponses.GepgResponseCodes["No payment(s) found for specified bill control number"] + ":No payment(s) found for specified bill control number");
                        }
                    }

                }
                return Ok(imisPayment.ReconciliationResp(GepgCodeResponses.GepgResponseCodes["Successful"]));
            }
            else
            {
                string reconc = JsonConvert.SerializeObject(model);
                _gepgFileLogger.Log("Reconc_DataInvalidSig", reconc);

                foreach (var recon in model.ReconcTrxInf)
                {
                    imisPayment.setRejectedReason(int.Parse(recon.SpBillId), GepgCodeResponses.GepgResponseCodes["Invalid Signature"] + ":Invalid Signature");
                }

                return Ok(imisPayment.ReconciliationResp(GepgCodeResponses.GepgResponseCodes["Invalid Signature"]));
            }
        }

        [HttpPost]
        [Route("api/WebMatchPayment")]
        public async Task<IActionResult> WebMatchPayment([FromBody]WebMatchModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error_occured = true, error_message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage });

            // Todo: remove api_key check because is checked by authentication layer
            if (model.api_key != "Xdfg8796021ff89Df4654jfjHeHidas987vsdg97e54ggdfHjdt")
                return BadRequest(new { error_occured = true, error_message = "Unauthorized request" });

            try
            {
                MatchModel match = new MatchModel()
                {
                    internal_identifier = model.internal_identifier,
                    audit_user_id = model.audit_user_id
                };

                var response = await base.MatchPayment(match);
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in WebMatchPayment");
                return BadRequest(new { error_occured = true, error_message = "Unknown Error Occured" });
            }

        }
        #endregion        
#endif
    }
}