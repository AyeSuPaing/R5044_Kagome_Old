/*
=========================================================================================================
  Module      : ヤマト決済(後払い) SMS認証結果POST通知受信クラス(PaymentYamatoKaSmsResultReceiver.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Register;

namespace w2.App.Common.Order.Payment.YamatoKa
{
	/// <summary>
	/// ヤマト後払いSMS認証結果POST通知受信クラス
	/// </summary>
	public class PaymentYamatoKaSmsResultReceiver
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="req">リクエスト</param>
		public PaymentYamatoKaSmsResultReceiver(HttpRequest req)
		{
			this.ResultData = new PaymentYamatoKaSmsResultData(req);
		}

		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			if (this.ResultData.SendDiv != PaymentYamatoKaSendDiv.SmsAvailable) return;

			new OrderRegisterFixedPurchase(
				Constants.FLG_LASTCHANGED_SYSTEM,
				true,
				false,
				new FixedPurchaseMailSendTiming("")).YamatoKaSmsNoticeRecievedProcess(this.ResultData);
		}

		/// <summary>SMS判定結果POST通知データ</summary>
		private PaymentYamatoKaSmsResultData ResultData { get; set; }
	}
}
