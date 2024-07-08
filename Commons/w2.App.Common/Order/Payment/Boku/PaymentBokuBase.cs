/*
=========================================================================================================
  Module      : Payment Boku Base(PaymentBokuBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku
{
	/// <summary>
	/// Payment Boku Base
	/// </summary>
	public abstract class PaymentBokuBase
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuBase(PaymentBokuSetting settings)
		{
			this.Settings = settings;
			this.ErrorMessage = string.Empty;
		}
		#endregion

		#region Methods
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="isSuccess">成功ならtrue、失敗ならfalse</param>
		/// <param name="paymentType">種別</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="message">メッセージ</param>
		/// <param name="messageDetail">メッセージ詳細</param>
		/// <param name="idKeyAndValueDictionary">ログに入れるID名と値のdictionary</param>
		protected void WriteLog(
			bool? isSuccess,
			string paymentType,
			PaymentFileLogger.PaymentProcessingType processingContent,
			string message,
			string messageDetail = "",
			Dictionary<string, string> idKeyAndValueDictionary = null)
		{
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				paymentType,
				PaymentFileLogger.PaymentType.Boku,
				processingContent,
				message + ((string.IsNullOrEmpty(messageDetail) == false) ? "\t" + messageDetail : string.Empty),
				idKeyAndValueDictionary);
		}
		#endregion

		#region Properties
		/// <summary>Boku settings</summary>
		protected PaymentBokuSetting Settings { get; private set; }
		/// <summary>Error message</summary>
		public string ErrorMessage { get; set; }
		/// <summary>Has error</summary>
		public bool HasError
		{
			get { return (string.IsNullOrEmpty(this.ErrorMessage) == false); }
		}
		#endregion
	}
}
