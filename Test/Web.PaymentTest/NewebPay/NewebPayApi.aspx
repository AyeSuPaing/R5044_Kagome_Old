<%--
=========================================================================================================
  Module      : NewebPay Api(NewebPayApi.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" %>

<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="w2.Common.Logger" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.NewebPay" %>
<%@ Import Namespace="w2.App.Common.ShopMessage" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.Domain.Order" %>

<!DOCTYPE html>

<script runat="server">
	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var data = new Dictionary<string, string>();
		var isCVS = false;
		var isCredit = false;
		var isATM = false;
		var isWebATM = false;
		var isCVSCOM = false;
		var isLinePay = false;
		var isBarcode = false;
		var isCreditWithInstallment = false;
		var creditInstallment = string.Empty;
		var tokenTerm = string.Empty;
		this.FormContent = new StringBuilder();

		if (Request.HttpMethod != "POST") return;

		if (Request.Form.AllKeys.All(item => string.IsNullOrEmpty(item)) == false)
		{
			data = Request.Form.AllKeys.ToDictionary(item => item, item => Request.Form[item]);
		}

		if (data.ContainsKey(NewebPayConstants.PARAM_TRADE_INFO))
		{
			Session["displayData"] = data;
		}
		else
		{
			data = (Dictionary<string, string>)Session["displayData"];
		}

		if (data == null)
			Response.Redirect(Constants.PATH_ROOT_FRONT_PC);

		var decodedTradeInfo = NewebPayUtility.DecryptAES256(data[NewebPayConstants.PARAM_TRADE_INFO]);
		var parsedTradeInfo = HttpUtility.ParseQueryString(decodedTradeInfo, Encoding.ASCII);
		lProductName.Text = parsedTradeInfo[NewebPayConstants.PARAM_ITEM_DESC];
		lMerchantName.Text = ShopMessageUtil.GetMessage("ShopName");
		lOrderId.Text = parsedTradeInfo[NewebPayConstants.PARAM_MERCHANT_ORDER_NO];
		lOrderAmount.Text = lPaidAmount.Text = parsedTradeInfo[NewebPayConstants.PARAM_AMOUNT];
		tbMail.Text = parsedTradeInfo[NewebPayConstants.PARAM_EMAIL];
		hfCustomerUrl.Value = parsedTradeInfo[NewebPayConstants.PARAM_CUSTOMER_URL];
		hfClientBackUrl.Value = parsedTradeInfo[NewebPayConstants.PARAM_CLIENT_BACK_URL];
		hfNotifyUrl.Value = parsedTradeInfo[NewebPayConstants.PARAM_NOTIFY_URL];
		hfReturnUrl.Value = parsedTradeInfo[NewebPayConstants.PARAM_RETURN_URL];
		isCVS = (parsedTradeInfo[NewebPayConstants.PARAM_CVS] == "1");
		isCredit = (parsedTradeInfo[NewebPayConstants.PARAM_CREDIT] == "1");
		isATM = (parsedTradeInfo[NewebPayConstants.PARAM_ATM] == "1");
		isWebATM = (parsedTradeInfo[NewebPayConstants.PARAM_WEBATM] == "1");
		isCVSCOM = (parsedTradeInfo[NewebPayConstants.PARAM_CVSCOM] == "2");
		isLinePay = (parsedTradeInfo[NewebPayConstants.PARAM_LINEPAY] == "1");
		isBarcode = (parsedTradeInfo[NewebPayConstants.PARAM_BARCODE] == "1");
		isCreditWithInstallment = (parsedTradeInfo.AllKeys.Contains(NewebPayConstants.PARAM_INST_FLAG)
			&& parsedTradeInfo[NewebPayConstants.PARAM_INST_FLAG] != string.Empty);
		if (isCredit) tokenTerm = parsedTradeInfo[NewebPayConstants.PARAM_TOKEN_TERM];
		if (isCreditWithInstallment) creditInstallment = parsedTradeInfo[NewebPayConstants.PARAM_INST_FLAG];

		var xdocPath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Xml",
			"NewebPay",
			"NewebPayApi.xml");
		var xdoc = XDocument.Load(xdocPath);

		var newebPayResponse = new NewebPayResponse();
		newebPayResponse.Result = new NewebPayResponseResultDetail();
		var trimmedNotifyUrl = (string.IsNullOrEmpty(hfNotifyUrl.Value) == false)
			? hfNotifyUrl.Value.Substring(32)
			: string.Empty;
		var trimmedCustomerUrl = (string.IsNullOrEmpty(hfCustomerUrl.Value) == false)
			? hfCustomerUrl.Value.Substring(32)
			: string.Empty;
		var trimmedReturnUrl = (string.IsNullOrEmpty(hfReturnUrl.Value) == false)
			? hfReturnUrl.Value.Substring(32)
			: string.Empty;
		var trimmedClientBackUrl = (string.IsNullOrEmpty(hfClientBackUrl.Value) == false)
			? hfClientBackUrl.Value.Substring(32)
			: string.Empty;

		if (isCredit)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_CREDIT;
			newebPayResponse.Message = xdoc.Root.Elements("MessageCredit").First().Value;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.Card6No = xdoc.Root.Elements("Card6No").First().Value;
			newebPayResponse.Result.Card4No = xdoc.Root.Elements("Card4No").First().Value;
			newebPayResponse.Result.ExpireDate = xdoc.Root.Elements("ExpireDateCredit").First().Value;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			if (string.IsNullOrEmpty(tokenTerm) == false)
				newebPayResponse.Result.TokenUseStatus = xdoc.Root.Elements("TokenUseStatus").First().Value;
			lPaymentName.Text = "信用卡一次付清";

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedNotifyUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		if (isCVS)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_CVS;
			newebPayResponse.Message = xdoc.Root.Elements("MessageCVS").First().Value;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.ExpireDate = xdoc.Root.Elements("ExpireDateCVS").First().Value;
			newebPayResponse.Result.CodeNo = xdoc.Root.Elements("CodeNoCVS").First().Value;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			lPaymentName.Text = "超商代碼繳費";

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedCustomerUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		if (isATM)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_ATM;
			newebPayResponse.Message = xdoc.Root.Elements("MessageATM").First().Value;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.ExpireDate = xdoc.Root.Elements("ExpireDateATM").First().Value;
			newebPayResponse.Result.CodeNo = xdoc.Root.Elements("CodeNoATM").First().Value;
			newebPayResponse.Result.PayBankCode = xdoc.Root.Elements("BankCode").First().Value;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			lPaymentName.Text = "ATM轉帳";

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedCustomerUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		if (isWebATM)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_WEBATM;
			newebPayResponse.Message = xdoc.Root.Elements("MessageWebATM").First().Value;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.PayTime = xdoc.Root.Elements("PayTime").First().Value;
			newebPayResponse.Result.PayBankCode = xdoc.Root.Elements("PayBankCode").First().Value;
			newebPayResponse.Result.PayerAccount5Code = xdoc.Root.Elements("PayerAccount5Code").First().Value;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			lPaymentName.Text = "Web ATM";

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedNotifyUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		if (isCVSCOM)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_CVSCOM;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Result.StoreCode = xdoc.Root.Elements("StoreCode").First().Value;
			newebPayResponse.Result.StoreName = xdoc.Root.Elements("StoreName").First().Value;
			newebPayResponse.Result.StoreType = xdoc.Root.Elements("StoreType").First().Value;
			newebPayResponse.Result.StoreAddr = xdoc.Root.Elements("StoreAddr").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			lPaymentName.Text = "超商取貨";

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedCustomerUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		if (isBarcode)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_BARCODE;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Result.Barcode1 = xdoc.Root.Elements("Barcode1").First().Value;
			newebPayResponse.Result.Barcode2 = xdoc.Root.Elements("Barcode2").First().Value;
			newebPayResponse.Result.Barcode3 = xdoc.Root.Elements("Barcode3").First().Value;
			newebPayResponse.Result.ExpireDate = xdoc.Root.Elements("ExpireDateBarcode").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			lPaymentName.Text = "條碼繳費";

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedCustomerUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		if (isLinePay)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_LINEPAY;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Result.ReturnCode = xdoc.Root.Elements("ReturnCode").First().Value;
			newebPayResponse.Result.ReturnMessage = xdoc.Root.Elements("ReturnMessage").First().Value;
			newebPayResponse.Result.PaymentUrl = xdoc.Root.Elements("PaymentUrl").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			lPaymentName.Text = "LINE Pay";

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedCustomerUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		if (isCreditWithInstallment)
		{
			newebPayResponse.Status = xdoc.Root.Elements("Status").First().Value;
			newebPayResponse.Result.MerchantID = Constants.NEWEBPAY_PAYMENT_MERCHANTID;
			newebPayResponse.Result.PaymentType = NewebPayConstants.PARAM_CREDIT;
			newebPayResponse.Result.TradeNo = xdoc.Root.Elements("TradeNo").First().Value;
			newebPayResponse.Message = xdoc.Root.Elements("MessageCredit").First().Value;
			newebPayResponse.Result.Amount = int.Parse(lOrderAmount.Text);
			newebPayResponse.Result.Card6No = xdoc.Root.Elements("Card6No").First().Value;
			newebPayResponse.Result.Card4No = xdoc.Root.Elements("Card4No").First().Value;
			newebPayResponse.Result.ExpireDate = xdoc.Root.Elements("ExpireDateCredit").First().Value;
			newebPayResponse.Result.MerchantOrderNo = lOrderId.Text;
			lPaymentName.Text = string.Format("信用卡分期付款（{0}期）", creditInstallment);

			this.Url = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedNotifyUrl);

			this.ClientBackUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedClientBackUrl);
		}

		var status = xdoc.Root.Elements("Status").First().Value;
		var arrayTradeInfo = decodedTradeInfo.Split('&');

		if (arrayTradeInfo[11].Contains("+"))
		{
			newebPayResponse.Status = status = xdoc.Root.Elements("StatusError").First().Value;
			newebPayResponse.Message = xdoc.Root.Elements("MessageError").First().Value;

			this.Url = string.Format(
				"{0}{1}{2}&Status={3}&Message={4}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				trimmedReturnUrl,
				status,
				newebPayResponse.Message);
		}

		var tempData = JsonConvert.SerializeObject(newebPayResponse);
		var tradeInfo = NewebPayUtility.EncryptAES256(tempData);

		var hashKey = NewebPayUtility.CreateHashKeyAndHashIV(
			NewebPayConstants.CONST_REQUEST_KEY_HASH_KEY,
			Constants.NEWEBPAY_PAYMENT_HASHKEY);
		var hashIV = NewebPayUtility.CreateHashKeyAndHashIV(
			NewebPayConstants.CONST_REQUEST_KEY_HASH_IV,
			Constants.NEWEBPAY_PAYMENT_HASHIV);

		var tradeShaTemp = string.Format(
			"{0}&{1}&{2}",
			hashKey,
			tradeInfo,
			hashIV);

		var tradeSha = NewebPayUtility.GetHashSha256(tradeShaTemp);

		var parameters = new List<Tuple<string, string>>
		{
			new Tuple<string, string>("Status", status),
			new Tuple<string, string>("MerchantID", Constants.NEWEBPAY_PAYMENT_MERCHANTID),
			new Tuple<string, string>("TradeInfo", tradeInfo),
			new Tuple<string, string>("TradeSha", tradeSha),
			new Tuple<string, string>("Version", "1.5"),
		};

		var format = "<input type='hidden' name='{0}' value='{1}'>";
		this.FormContent.Append("<html>")
			.Append("<body onload='document.forms[0].submit()'>")
			.AppendFormat("<form name='Newebpay' action='{0}' method='post'>", this.Url);
		foreach (var parameter in parameters)
		{
			this.FormContent.AppendFormat(format, parameter.Item1, parameter.Item2);
		}
		this.FormContent.Append("</form>")
			.Append("</body>")
			.Append("</html>");

		if (arrayTradeInfo[11].Contains("+"))
		{
			Response.Clear();
			Response.Write(this.FormContent.ToString());
			Response.End();
		}
	}

	/// <summary>
	/// Button OK Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOK_Click(object sender, EventArgs e)
	{
		Response.Clear();
		Response.Write(this.FormContent.ToString());
		Response.End();
	}

	/// <summary>
	/// Button Back Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(this.ClientBackUrl))
			Response.Redirect(Constants.PATH_ROOT_FRONT_PC);

		Response.Redirect(this.ClientBackUrl);
	}

	/// <summary>Url</summary>
	protected string Url { get; set; }

	/// <summary>Form Content</summary>
	protected StringBuilder FormContent { get; set; }

	/// <summary>Client Back Url</summary>
	protected string ClientBackUrl { get; set; }
</script>
<style>
	.boxedInfo {
		border: 1px solid;
		width: 500px;
		height: 145px;
		padding: 10px;
	}

	.line {
		width: 500px;
		border-bottom: 1px dashed black;
	}

	.boxedMailBox {
		border: 1px solid;
		width: 500px;
		height: 75px;
		padding: 10px;
	}
</style>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title>藍新Payモック</title>
</head>
<body>
	<form id="Form1" runat="server">
		<div style="text-align: center">
			<h1 style="font-weight: lighter; margin: 0">藍新Payモック</h1>
		</div>
		<hr />
		<div class="boxedInfo">
			<div class="line">
				<h4 style="margin: 0">訂單資訊</h4>
			</div>
			<div style="margin-top: 5px">
				商品名稱：
				<asp:Literal ID="lProductName" runat="server"></asp:Literal><br />
				商店名稱：
				<asp:Literal ID="lMerchantName" runat="server"></asp:Literal><br />
				商店訂單編號：
				<asp:Literal ID="lOrderId" runat="server"></asp:Literal><br />
				訂單金額：
				<asp:Literal ID="lOrderAmount" runat="server"></asp:Literal><br />
				應付金額：
				<asp:Literal ID="lPaidAmount" runat="server"></asp:Literal>
				<asp:HiddenField ID="hfCustomerUrl" runat="server" />
				<asp:HiddenField ID="hfClientBackUrl" runat="server" />
				<asp:HiddenField ID="hfNotifyUrl" runat="server" />
				<asp:HiddenField ID="hfReturnUrl" runat="server" />
			</div>
		</div>
		<div class="line" style="margin: 10px"></div>
		<div class="boxedMailBox">
			<div class="line">
				<h4 style="margin: 0">選擇付款方式</h4>
			</div>
			藍新金流支付方式:&nbsp;&nbsp;<asp:Literal ID="lPaymentName" runat="server"></asp:Literal><br />
			填寫付款人信箱：
			<asp:TextBox ID="tbMail" Width="200" runat="server"></asp:TextBox><br />
		</div>
		<div style="margin-top: 10px">
			<asp:Button ID="btnOK" runat="server" Text="已閱讀並同意服務條款，確認送出" OnClick="btnOK_Click" />
			<asp:Button ID="btnBack" runat="server" Text="返回商店" OnClick="btnBack_Click" />
		</div>
		<hr />
	</form>
</body>
</html>
