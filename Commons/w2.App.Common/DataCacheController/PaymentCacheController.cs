/*
=========================================================================================================
  Module      : 決済種別キャッシュコントローラ(PaymentCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.Payment;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 決済種別キャッシュコントローラ
	/// </summary>
	public class PaymentCacheController : DataCacheControllerBase<PaymentModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentCacheController()
			: base(RefreshFileType.Payment)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.PaymentService.GetAllWithPrice(Constants.CONST_DEFAULT_SHOP_ID);
		}

		/// <summary>
		/// 取得（有効なもの全て、価格情報も含めて）
		/// </summary>
		/// <returns>モデル</returns>
		public PaymentModel[] GetValidAllWithPrice()
		{
			var result = this.CacheData.Where(p => p.IsValid).ToArray();
			return result;
		}

		/// <summary>
		/// 全ての決済種別(価格情報も含む)を取得
		/// </summary>
		/// <returns>決済種別モデル</returns>
		public PaymentModel[] GetAllWithPrice()
		{
			return this.CacheData;
		}

		/// <summary>
		/// 決済種別IDより決済種別名を取得
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>決済種別名</returns>
		public string GetPaymentName(string paymentId)
		{
			var payment = Get(paymentId);
			var result = (payment != null) ? payment.PaymentName : string.Empty;
			return result;
		}

		/// <summary>
		/// 決済種別モデル取得
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>決済種別モデル</returns>
		public PaymentModel Get(string paymentId)
		{
			var result = this.CacheData.FirstOrDefault(m => (m.PaymentId == paymentId));
			return result;
		}
	}
}
