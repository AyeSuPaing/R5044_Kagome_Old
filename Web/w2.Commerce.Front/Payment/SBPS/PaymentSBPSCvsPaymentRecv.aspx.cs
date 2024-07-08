/*
=========================================================================================================
  Module      : SBPS コンビニ決済 入金受取ページ処理(PaymentSBPSCvsPaymentRevc.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;

public partial class Payment_SBPS_PaymentSBPSCvsPaymentRecv : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// SBPS Webコンビニ入金ステータスアップデータ実行
		OrderPaymentStatusUpdaterForSBPSCvs updater = new OrderPaymentStatusUpdaterForSBPSCvs();
		string responseString = updater.Exec(Request.InputStream);
		PaymentFileLogger.WritePaymentLog(
			null,
			"",
			PaymentFileLogger.PaymentType.Sbps,
			PaymentFileLogger.PaymentProcessingType.Unknown,
			"コンビニ決済入金受取ページ処理開始");

		// レスポンス書きだし
		Response.Write(responseString);
	}
}