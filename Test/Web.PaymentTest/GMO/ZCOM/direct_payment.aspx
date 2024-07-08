<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom.Direct" %>
<script runat="server">
void Main()
{
	try
	{
		var response = new ZcomDirectResponse();
		var res = new List<ZcomDirectResponse.Result>();
		var transCode = string.Format("Zmock{0:yyyyMMddHHmmssfff}", DateTime.Now);
		var isUseZcom3DSMock = (Constants.PAYMENT_CREDIT_USE_ZCOM3DS_MOCK
			&& (string.IsNullOrEmpty(Request.Form[ZcomConst.PARAM_CARD_NUMBER]) == false));

		if (isUseZcom3DSMock)
		{
			res.Add(new ZcomDirectResponse.Result { result = ZcomConst.RESULT_CODE_REDIRECT_URL });
			res.Add(new ZcomDirectResponse.Result { Status = ZcomConst.STATUS_UNBILLED });
			res.Add(new ZcomDirectResponse.Result { AccessUrl = Constants.PAYMENT_CREDIT_ZCOM_APIURL_ACCESSAUTH_MOCK });
			res.Add(new ZcomDirectResponse.Result { Mode = "4" });
			res.Add(new ZcomDirectResponse.Result { PaymentCode = ZcomConst.CONST_PAYMENT_METHOD_CREDIT_CARD_SETTLEMENT });
			res.Add(new ZcomDirectResponse.Result { TransCodeHash = transCode });
			AddTransactionInfos(transCode);
		}
		else
		{
			res.Add(new ZcomDirectResponse.Result { result = ZcomConst.RESULT_CODE_OK });
			res.Add(new ZcomDirectResponse.Result { Status = ZcomConst.STATUS_PROVISIONAL_SALES });
		}

		res.Add(new ZcomDirectResponse.Result { TransCode = transCode });
		response.Results = res.ToArray();
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new ZcomDirectResponse();
		var res = new List<ZcomDirectResponse.Result>();
		res.Add(new ZcomDirectResponse.Result() { result = ZcomConst.RESULT_CODE_SYSTEM_ERROR });
		res.Add(new ZcomDirectResponse.Result() { ErrCode = "999" });
		res.Add(new ZcomDirectResponse.Result() { ErrDetail = ex.Message });
		response.Results = res.ToArray();

		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}

/// <summary>
/// Add transaction infos
/// </summary>
/// <param name="transCode">Trans code</param>
void AddTransactionInfos(string transCode)
{
	var zcomTransactionInfos = new List<Hashtable>
	{
		new Hashtable
		{
			{ ZcomConst.PARAM_CONTRACT_CODE, Request.Form[ZcomConst.PARAM_CONTRACT_CODE] },
			{ ZcomConst.PARAM_VERSION, Request.Form[ZcomConst.PARAM_VERSION] },
			{ ZcomConst.PARAM_CHARACTER_CODE, Request.Form[ZcomConst.PARAM_CHARACTER_CODE] },
			{ ZcomConst.PARAM_PROCESS_CODE, Request.Form[ZcomConst.PARAM_PROCESS_CODE] },
			{ ZcomConst.PARAM_USER_ID, Request.Form[ZcomConst.PARAM_USER_ID] },
			{ ZcomConst.PARAM_USER_NAME, Request.Form[ZcomConst.PARAM_USER_NAME] },
			{ ZcomConst.PARAM_USER_MAIL_ADD, Request.Form[ZcomConst.PARAM_USER_MAIL_ADD] },
			{ ZcomConst.PARAM_LANG_ID, Request.Form[ZcomConst.PARAM_LANG_ID] },
			{ ZcomConst.PARAM_ITEM_CODE, Request.Form[ZcomConst.PARAM_ITEM_CODE] },
			{ ZcomConst.PARAM_ITEM_NAME, Request.Form[ZcomConst.PARAM_ITEM_NAME] },
			{ ZcomConst.PARAM_ORDER_NUMBER, Request.Form[ZcomConst.PARAM_ORDER_NUMBER] },
			{ ZcomConst.PARAM_TRANS_CODE, transCode },
			{ ZcomConst.PARAM_ST_CODE, Request.Form[ZcomConst.PARAM_ST_CODE] },
			{ ZcomConst.PARAM_MISSION_CODE, Request.Form[ZcomConst.PARAM_MISSION_CODE] },
			{ ZcomConst.PARAM_CURRENCY_ID, Request.Form[ZcomConst.PARAM_CURRENCY_ID] },
			{ ZcomConst.PARAM_ITEM_PRICE, Request.Form[ZcomConst.PARAM_ITEM_PRICE] },
			{ ZcomConst.PARAM_MEMO1, Request.Form[ZcomConst.PARAM_MEMO1] },
			{ ZcomConst.PARAM_MEMO2, Request.Form[ZcomConst.PARAM_MEMO2] },
			{ ZcomConst.PARAM_RECEIPT_DATE, DateTime.Now.ToString() },
			{ ZcomConst.PARAM_TRANS_CODE_HASH, transCode },
			{ ZcomConst.PARAM_PAYMENT_CODE, ZcomConst.CONST_PAYMENT_METHOD_CREDIT_CARD_SETTLEMENT },
			{ ZcomConst.PARAM_BACK_URL, Request.Form[ZcomConst.PARAM_BACK_URL] },
			{ ZcomConst.PARAM_ERR_URL, Request.Form[ZcomConst.PARAM_ERR_URL] },
			{ ZcomConst.PARAM_SUCCESS_URL, Request.Form[ZcomConst.PARAM_SUCCESS_URL] },
		}
	};

	var preTransactionInfo = (List<Hashtable>)Cache[Constants.CACHE_KEY_PAYMENT_CREDIT_ZCOM_TRANSACTION_INFO];
	if (preTransactionInfo != null)
	{
		zcomTransactionInfos.AddRange(preTransactionInfo);
	}

	Cache[Constants.CACHE_KEY_PAYMENT_CREDIT_ZCOM_TRANSACTION_INFO] = zcomTransactionInfos;
}
</script>
<% this.Main(); %>