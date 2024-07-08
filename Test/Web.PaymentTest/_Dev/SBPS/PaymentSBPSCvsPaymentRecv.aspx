<%@ Page Language="C#" %>
<%@ Import Namespace="System.Xml.Linq" %>
<%@ Import Namespace="w2.App.Common.Order" %>

<script runat="server">

	/// <summary>
	/// ページロード（XML受取）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		PaymentSBPSCvsPaymentApiRcv apiRcv = new PaymentSBPSCvsPaymentApiRcv();

		// 読み込み
		if (Request.InputStream.CanRead == false)
		{
			WriteResponse(false, apiRcv.CreateResponseXml(false, "XMLが読み込めません。"));
			return;
		}
		string requestXml = null;
		using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.InputStream))
		{
			requestXml = reader.ReadToEnd();
		}

		// 解析
		try
		{
			apiRcv.Receive(requestXml);
		}
		catch (Exception ex)
		{
			WriteResponse(false, apiRcv.CreateResponseXml(false, "XMLが解析できません。" + ex.Message));
			return;
		}
		
		// 処理
		switch (apiRcv.ResponseData.RecType)
		{
				// 入金処理
			case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.PrompReport:
			case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.FixReport:
				break;

				// 入金戻し
			case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.PrompReportCansel:
			case PaymentSBPSCvsPaymentApiRcvResponseData.RecTypes.FixReportCansel:
				break;
		}

		// OKを返す
		WriteResponse(true, "");
	}

	/// <summary>
	/// レスポンス出力
	/// </summary>
	/// <param name="result"></param>
	/// <param name="responseXml"></param>
	private void WriteResponse(bool result, string responseXml)
	{
		Response.Write("<?xml version=\"1.0\" encoding=\"Shift_JIS\"?>\r\n");
		Response.Write(responseXml);
	}
</script>
