/*
=========================================================================================================
  Module      : 電算システム入金情報取得ページ処理(GetDskCvsPayInfo.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Order.Payment;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

public partial class Payment_GetDskCvsPayInfo : BasePage
{
	// 入金通知パラメタ
	private const string RESULT_DATAKBN = "datakbn";		// 通知処理区分
	private const string RESULT_DSKID = "dskid";			// 注文管理ID
	private const string RESULT_AMOUNT = "amount";			// 金額
	private const string RESULT_PAYDATE = "paydate";		// 店舗入金日時
	private const string RESULT_CONVENI = "conveni";		// 入金コンビニチェーン
	private const string RESULT_RECEIPTNO = "receiptno";	// コンビニ受付番号
	private const string RESULT_ORDERID = "orderid";		// お客様注文番号

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// メッセージチェック
		//------------------------------------------------------
		Hashtable htParam = new Hashtable();
		htParam.Add(RESULT_ORDERID, StringUtility.ToEmpty(Request.Form[RESULT_ORDERID]));
		htParam.Add(RESULT_DSKID, StringUtility.ToEmpty(Request.Form[RESULT_DSKID]));
		htParam.Add(RESULT_PAYDATE, StringUtility.ToEmpty(Request.Form[RESULT_PAYDATE]));

		try
		{
			// お客様注文番号が存在する場合
			if (((string)htParam[RESULT_ORDERID]).Length > 0)
			{
				// 入金ステータス更新
				var orderId = (string)htParam[RESULT_ORDERID];
				string strPayDate = (string)htParam[RESULT_PAYDATE];
				var updated = new OrderService().UpdatePaymentStatusForCvs(
					orderId,
					Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
					DateTime.Parse(strPayDate.Substring(0, 4) + "/" + strPayDate.Substring(4, 2) + "/" + strPayDate.Substring(6, 2) + " " + strPayDate.Substring(8, 2) + ":" + strPayDate.Substring(10, 2) + ":" + strPayDate.Substring(12, 2)),
					(string)htParam[RESULT_DSKID],
					Constants.FLG_LASTCHANGED_CGI,
					UpdateHistoryAction.Insert);
				// メッセージチェック不一致
				if (updated == 0) throw new ApplicationException("更新に失敗しました");

				// 情報おとす
				PaymentFileLogger.WritePaymentLog(
					true,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
					PaymentFileLogger.PaymentType.Dsk,
					PaymentFileLogger.PaymentProcessingType.PaymentNotification,
					"",
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, orderId}
					});

				// [<SENBDATA>STATUS=800</SENBDATA>]を出力し、
				// ねっとe-furi.comサーバに正常受信を通知する
				Response.Write("<SENBDATA>STATUS=800</SENBDATA>");
			}
			else
			{
				// メッセージチェック不一致
				throw new ApplicationException("注文IDが存在しません");
			}
		}
		catch (Exception ex)
		{
			var message = "正しくないコンビニ入金情報です。";
			Response.Write(message);

			AppLogger.WriteError(ex);

			// 情報おとす
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
				PaymentFileLogger.PaymentType.Dsk,
				PaymentFileLogger.PaymentProcessingType.PaymentNotification,
				message,
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, (string)htParam[RESULT_ORDERID]},
					{Constants.FIELD_SHOPSHIPPING_PAYMENT_RELATION_ID, StringUtility.ToEmpty(Request.Form[RESULT_DSKID])},
					{"store_payment_date",StringUtility.ToEmpty(Request.Form[RESULT_PAYDATE])} //店舗入金日時
				});
			Response.StatusCode = 500;
		}

		// HTTPレスポンス出力
		Response.End();
	}
}
