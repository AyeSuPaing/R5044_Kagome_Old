/*
=========================================================================================================
  Module      : GMOアトカラ データ送信ページ(PostGmoAtokaraAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Web.Services;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.Common.Logger;
using w2.Domain.Order;

/// <summary>
/// GMOアトカラ データ送信ページ
/// </summary>
public partial class Payment_GmoAtokara_PostGmoAtokaraAuth : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		// パラメータ取得
		var orderId = this.Request[Constants.REQUEST_KEY_ORDER_ID];

		// セッション情報をファイルに保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(this.Session, orderId);

		// 送信データ作成
		CreatePostData(orderId);
	}

	/// <summary>
	/// 送信データ作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void CreatePostData(string orderId)
	{
		var order = new OrderService().Get(orderId);

		var atokaraOrderRegister = new PaymentGmoAtokaraOrderRegister();

		rPostData.DataSource = atokaraOrderRegister.CreatePostData(order);
		rPostData.DataBind();
	}

	/// <summary>
	/// ログ出力
	/// </summary>
	/// <param name="isSuccess">成功か</param>
	/// <param name="data">データ</param>
	[WebMethod]
	public static void WriteLog(bool isSuccess, object data)
	{
		try
		{
			var message = JsonConvert.SerializeObject(data);
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA,
				PaymentFileLogger.PaymentType.GmoAtokara,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				message);
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
		}
	}
}
