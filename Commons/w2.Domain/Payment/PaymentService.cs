/*
=========================================================================================================
  Module      : 決済種別サービス (PaymentService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;

namespace w2.Domain.Payment
{
	/// <summary>
	/// 決済種別サービス
	/// </summary>
	public class PaymentService : ServiceBase, IPaymentService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>モデル</returns>
		public PaymentModel Get(string shopId, string paymentId)
		{
			using (var repository = new PaymentRepository())
			{
				// 取得
				var model = repository.Get(shopId, paymentId);
				if (model == null) return null;

				// 価格
				model.PriceList = repository.GetRelatedPrice(shopId, paymentId);

				return model;
			}
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public PaymentModel[] GetAll(string shopId)
		{
			using (var repository = new PaymentRepository())
			{
				// 取得
				var models = repository.GetAll(shopId);
				if (models.Length == 0) return new PaymentModel[0];

				return models;
			}
		}
		#endregion

		#region +GetPaymentList 決済種別情報一覧を表示分だけ取得
		/// <summary>
		/// 決済種別情報一覧を表示分だけ取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="displayNumber">表示件数</param>
		/// <param name="pageNumber">ページャ</param>
		/// <param name="amazonPaymentOptionEnabled">Amazonペイメントオプションが有効か</param>
		/// <param name="amazonPaymentCv2Enabled">AmazonペイメントオプションCV2が有効か</param>
		/// <param name="paymentGmoPostEnabled">payment GMO post enabled</param>
		/// <param name="paymentGmoAtokaraEnabled">GMOアトカラが有効か</param>
		/// <returns>モデル</returns>
		public PaymentModel[] GetPaymentList(
			string shopId,
			int displayNumber,
			int pageNumber,
			bool amazonPaymentOptionEnabled,
			bool amazonPaymentCv2Enabled,
			bool paymentGmoPostEnabled,
			bool paymentGmoAtokaraEnabled)
		{
			using (var repository = new PaymentRepository())
			{
				// 取得
				var models = repository.GetPaymentList(
					shopId,
					displayNumber,
					pageNumber,
					amazonPaymentOptionEnabled,
					amazonPaymentCv2Enabled,
					paymentGmoPostEnabled,
					paymentGmoAtokaraEnabled);
				return (models.Length != 0) ? models : new PaymentModel[0];
			}
		}
		#endregion

		#region +GetPaymentList 決済種別情報一覧取得(有効判定あり)
		/// <summary>
		/// 決済種別情報一覧取得(有効判定あり)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="amazonPaymentOptionEnabled">Amazonペイメントオプションが有効か</param>
		/// <param name="amazonPaymentCv2Enabled">AmazonペイメントオプションCV2が有効か</param>
		/// <param name="paymentGmoPostEnabled">payment GMO post enabled</param>
		/// <param name="paymentGmoAtokaraEnabled">GMOアトカラが有効か</param>
		/// <returns>モデル</returns>
		public PaymentModel[] GetPaymentListEnabled(
			string shopId,
			bool amazonPaymentOptionEnabled,
			bool amazonPaymentCv2Enabled,
			bool paymentGmoPostEnabled,
			bool paymentGmoAtokaraEnabled)
		{
			using (var repository = new PaymentRepository())
			{
				// 取得
				var models = repository.GetPaymentListEnabled(
					shopId,
					amazonPaymentOptionEnabled,
					amazonPaymentCv2Enabled,
					paymentGmoPostEnabled,
					paymentGmoAtokaraEnabled);
				return (models.Length != 0) ? models : new PaymentModel[0];
			}
		}
		#endregion

		#region +GetAllWithPrice 取得（価格情報も含めた全て）
		/// <summary>
		/// GetAllWithPrice（価格情報も含めた全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public PaymentModel[] GetAllWithPrice(string shopId)
		{
			using (var repository = new PaymentRepository())
			{
				// 取得
				var paymentList = repository.GetAll(shopId);
				var paymentPriceList = repository.GetAllPrice(shopId);
				if (paymentList.Length == 0) return new PaymentModel[0];
				foreach (var payment in paymentList)
				{
					var paymentPriceInfo =
						paymentPriceList.Where(paymentPrice => (payment.PaymentId == paymentPrice.PaymentId));
					payment.PriceList = paymentPriceInfo.ToArray();
				}

				return paymentList;
			}
		}
		#endregion

		#region +GetValidAll 取得（有効なもの全て）
		/// <summary>
		/// 取得（有効なもの全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public PaymentModel[] GetValidAll(string shopId)
		{
			return this.GetAll(shopId).Where(p => p.IsValid).ToArray();
		}
		#endregion

		#region +GetValidFixedPurchaseAll 取得（定期購入で有効なもの全て）
		/// <summary>
		/// 取得（定期購入で有効なもの全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public PaymentModel[] GetValidAllForFixedPurchase(string shopId)
		{
			return this.GetAll(shopId).Where(p => p.IsValidFixedPurchase).ToArray();
		}
		#endregion

		#region +GetValidUserDefaultPayment 取得（既定のお支払方法で有効なもの全て）
		/// <summary>
		/// 取得（既定のお支払方法で有効なもの全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public PaymentModel[] GetValidUserDefaultPayment(string shopId)
		{
			return this.GetAll(shopId).Where(p => p.IsValidUserDefaultPayment).ToArray();
		}
		#endregion

		#region +GetPaymentName 決済種別IDより決済種別名を取得
		/// <summary>
		/// 決済種別IDより決済種別名を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>決済種別名</returns>
		public string GetPaymentName(string shopId, string paymentId)
		{
			using (var repository = new PaymentRepository())
			{
				var model = repository.Get(shopId, paymentId);
				if (model == null) return string.Empty;
				return model.PaymentName;
			}
		}
		#endregion

		#region +GetPaymentNamesByPaymentIds
		/// <summary>
		/// Get payment names by payment ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="paymentIds">Payment ids</param>
		/// <returns>Payment names</returns>
		public string[] GetPaymentNamesByPaymentIds(string shopId, string[] paymentIds)
		{
			using (var repository = new PaymentRepository())
			{
				var paymentNames = repository.GetPaymentNamesByPaymentIds(shopId, paymentIds);
				return paymentNames;
			}
		}
		#endregion

		#region +GetValidPayments
		/// <summary>
		/// Get valid payments
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>An array of payment models</returns>
		public PaymentModel[] GetValidPayments(string shopId)
		{
			using (var repository = new PaymentRepository())
			{
				var models = repository.GetValidPayments(shopId);
				return models;
			}
		}
		#endregion
	}
}
