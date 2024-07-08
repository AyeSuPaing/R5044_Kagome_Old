<%--
=========================================================================================================
  Module      : Check Order(check_order.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth" %>
<%@ Import Namespace="w2.Common.Util" %>

<script runat="server">
	/// <summary>
	/// Main
	/// </summary>
	void Main()
	{
		try
		{
			var contractCode = Request.Form[ZcomConst.PARAM_CONTRACT_CODE];
			var orderNumber = Request.Form[ZcomConst.PARAM_ORDER_NUMBER];
			var zcomTransactionInfos = (List<Hashtable>)Cache[Constants.CACHE_KEY_PAYMENT_CREDIT_ZCOM_TRANSACTION_INFO];

			if (zcomTransactionInfos == null) throw new Exception();

			var zcomTransactionInfo = zcomTransactionInfos.FirstOrDefault(item =>
				(StringUtility.ToEmpty(item[ZcomConst.PARAM_CONTRACT_CODE]) == contractCode)
				&& (StringUtility.ToEmpty(item[ZcomConst.PARAM_ORDER_NUMBER]) == orderNumber));

			if (zcomTransactionInfo.Count == 0) throw new Exception();

			var response = new ZcomCheckAuthResponse
			{
				ResponseObject = new ZcomCheckAuthResponse.Response[]
				{
					new ZcomCheckAuthResponse.Response { ContractCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_CONTRACT_CODE]) },
					new ZcomCheckAuthResponse.Response { Version = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ORDER_NUMBER]) },
					new ZcomCheckAuthResponse.Response { CharacterCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_CHARACTER_CODE]) },
					new ZcomCheckAuthResponse.Response { ProcessCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_PROCESS_CODE]) },
					new ZcomCheckAuthResponse.Response { State = ZcomConst.STATUS_PROVISIONAL_SALES },
					new ZcomCheckAuthResponse.Response { UserId = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_USER_ID]) },
					new ZcomCheckAuthResponse.Response { UserName = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_USER_NAME]) },
					new ZcomCheckAuthResponse.Response { UserMailAdd = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_USER_MAIL_ADD]) },
					new ZcomCheckAuthResponse.Response { LangId = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_LANG_ID]) },
					new ZcomCheckAuthResponse.Response { ItemCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ITEM_CODE]) },
					new ZcomCheckAuthResponse.Response { ItemName = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ITEM_NAME]) },
					new ZcomCheckAuthResponse.Response { OrderNumber = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ORDER_NUMBER]) },
					new ZcomCheckAuthResponse.Response { TransCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_TRANS_CODE]) },
					new ZcomCheckAuthResponse.Response { StCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ST_CODE]) },
					new ZcomCheckAuthResponse.Response { MissionCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_MISSION_CODE]) },
					new ZcomCheckAuthResponse.Response { CurrencyId = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_CURRENCY_ID]) },
					new ZcomCheckAuthResponse.Response { ItemPrice = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ITEM_PRICE]) },
					new ZcomCheckAuthResponse.Response { Memo1 = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_MEMO1]) },
					new ZcomCheckAuthResponse.Response { Memo2 = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_MEMO2]) },
					new ZcomCheckAuthResponse.Response { ReceiptDate = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_RECEIPT_DATE]) },
					new ZcomCheckAuthResponse.Response { PaymentCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_PAYMENT_CODE]) },
					new ZcomCheckAuthResponse.Response { ChargeDate = DateTime.Now.ToString() },
				}
			};

			zcomTransactionInfos.Remove(zcomTransactionInfo);
			Response.ContentType = ZcomConst.CONTENT_TYPE_XML;
			Response.Write(response.CreateResponseString());
		}
		catch (Exception)
		{
			var response = new ZcomCheckAuthResponse
			{
				ResponseObject = new ZcomCheckAuthResponse.Response[]
				{
					new ZcomCheckAuthResponse.Response { State = ZcomConst.RESULT_CODE_SYSTEM_ERROR },
				}
			};

			Response.ContentType = ZcomConst.CONTENT_TYPE_XML;
			Response.Write(response.CreateResponseString());
		}
	}
</script>
<% this.Main(); %>