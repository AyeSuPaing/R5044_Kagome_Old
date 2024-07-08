/*
=========================================================================================================
  Module      : ソフトバンクペイメント基底クラス(PaymentSBPSBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント基底クラス
	/// </summary>
	public abstract class PaymentSBPSBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="hashEncoding">ハッシュエンコーディング</param>
		public PaymentSBPSBase(
			PaymentSBPSSetting settings,
			Encoding hashEncoding)
		{
			this.Settings = settings;
			this.HashCalculator = new PaymentSBPSHashCalculator(this.Settings.HashKey, hashEncoding);
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="messageDetail">メッセージ詳細</param>
		/// <param name="paymentType">種別</param>
		/// <param name="isSuccess">成功ならtrue、失敗ならfalse</param>
		/// <param name="idKeyAndValueDictionary">ログに入れるID名と値のdictionary</param>
		protected void WriteLog(
			bool? isSuccess,
			string paymentType,
			PaymentFileLogger.PaymentProcessingType processingContent,
			string message,
			string messageDetail = "",
			Dictionary<string, string> idKeyAndValueDictionary = null)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				paymentType,
				PaymentFileLogger.PaymentType.Sbps,
				processingContent,
				message + ((string.IsNullOrEmpty(messageDetail) == false) ? "\t" + messageDetail : ""),
				idKeyAndValueDictionary);
		}

		/// <summary>SBPS設定</summary>
		protected PaymentSBPSSetting Settings { get; private set; }
		/// <summary>SBPSハッシュ計算クラス</summary>
		protected PaymentSBPSHashCalculator HashCalculator { get; private set; }

		/// <summary>
		/// 商品アイテム
		/// </summary>
		public struct ProductItem
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id">ID</param>
			/// <param name="name">商品名</param>
			/// <param name="count">商品数</param>
			/// <param name="priceSubtotal">商品小計</param>
			public ProductItem(string id, string name, int count, decimal priceSubtotal)
			{
				this.Id = id;
				this.Name = name;
				this.Count = count;
				this.PriceSubtotal = priceSubtotal;
			}
			/// <summary>ID</summary>
			public string Id;
			/// <summary>商品名</summary>
			public string Name;
			/// <summary>商品数</summary>
			public int Count;
			/// <summary>商品小計</summary>
			public decimal PriceSubtotal;
		}
	}
}
