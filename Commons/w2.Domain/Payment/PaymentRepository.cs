/*
=========================================================================================================
  Module      : 決済種別リポジトリ (PaymentRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.Payment
{
	/// <summary>
	/// 決済種別リポジトリ
	/// </summary>
	public class PaymentRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Payment";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PaymentRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>モデル</returns>
		public PaymentModel Get(string shopId, string paymentId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAYMENT_SHOP_ID, shopId},
				{Constants.FIELD_PAYMENT_PAYMENT_ID, paymentId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new PaymentModel(dv[0]);
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
			var ht = new Hashtable
			{
				{Constants.FIELD_PAYMENT_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new PaymentModel(drv)).ToArray();
		}
		#endregion

		#region +GetPaymentList 決済種別情報一覧データビューを表示分だけ取得
		/// <summary>
		/// 決済種別情報一覧データビューを表示分だけ取得
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
			Hashtable htInput = new Hashtable
			{
				{Constants.FIELD_PAYMENT_SHOP_ID, shopId},
				{"bgn_row_num", displayNumber * (pageNumber - 1) + 1},
				{"end_row_num", displayNumber * pageNumber},

				// Amazonペイメントオプション
				{"AmazonPaymentOption", amazonPaymentOptionEnabled ? "1" : "0"},
				{"AmazonPaymentCV2Enabled", amazonPaymentCv2Enabled ? "1" : "0"},
				{"AmazonPaymentID", Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT},
				{"AmazonPaymentCV2ID", Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2},

				// GMO payment
				{"GmoPaymentOption", paymentGmoPostEnabled ? "1" : "0"},
				{"GmoPaymentPayAsYouGo", Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO},
				{"GmoPaymentFrameGuarantee", Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE},

				// GMOアトカラ
				{"GmoAtokaraOption", paymentGmoAtokaraEnabled ? "1" : "0"},
				{"GmoAtokaraPaymentId", Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA},
			};
			var dv = Get(XML_KEY_NAME, "GetPaymentList", htInput);
			return dv.Cast<DataRowView>().Select(drv => new PaymentModel(drv)).ToArray();
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
			Hashtable htInput = new Hashtable
			{
				{Constants.FIELD_PAYMENT_SHOP_ID, shopId},

				// Amazonペイメントオプション
				{"AmazonPaymentOption", amazonPaymentOptionEnabled ? "1" : "0"},
				{"AmazonPaymentCV2Enabled", amazonPaymentCv2Enabled ? "1" : "0"},
				{"AmazonPaymentID", Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT},
				{"AmazonPaymentCV2ID", Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2},

				// GMO payment
				{"GmoPaymentOption", paymentGmoPostEnabled ? "1" : "0"},
				{"GmoPaymentPayAsYouGo", Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO},
				{"GmoPaymentFrameGuarantee", Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE},

				// GMOアトカラ
				{"GmoAtokaraOption", paymentGmoAtokaraEnabled ? "1" : "0"},
				{"GmoAtokaraPaymentId", Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA},
			};
			var dv = Get(XML_KEY_NAME, "GetPaymentListEnabled", htInput);
			return dv.Cast<DataRowView>().Select(drv => new PaymentModel(drv)).ToArray();
		}
		#endregion

		#region +GetRelatedPrice 価格取得（関連する全て）
		/// <summary>
		/// 価格取得（関連する全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>モデル列</returns>
		public PaymentPriceModel[] GetRelatedPrice(string shopId, string paymentId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAYMENT_SHOP_ID, shopId},
				{Constants.FIELD_PAYMENT_PAYMENT_ID, paymentId},
			};
			var dv = Get(XML_KEY_NAME, "GetRelatedPrice", ht);
			return dv.Cast<DataRowView>().Select(drv => new PaymentPriceModel(drv)).ToArray();
		}
		#endregion

		#region +GetAllPrice 価格取得（全て）
		/// <summary>
		/// 価格取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		public PaymentPriceModel[] GetAllPrice(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAYMENT_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetAllPrice", ht);
			return dv.Cast<DataRowView>().Select(drv => new PaymentPriceModel(drv)).ToArray();
		}
		#endregion

		#region +GetPaymentNamesByPaymentIds
		/// <summary>
		/// Get payment names by payment ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="paymentIds">Payment ids</param>
		/// <returns>Payment names</returns>
		internal string[] GetPaymentNamesByPaymentIds(string shopId, string[] paymentIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PAYMENT_SHOP_ID, shopId },
			};
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ payment_ids @@",
					string.Join(",", paymentIds.Select(paymentId => string.Format("'{0}'", paymentId.Replace("'", "''"))))),
			};
			var dv = Get(XML_KEY_NAME, "GetPaymentNamesByPaymentIds", input, replaces: replaceKeyValues);
			var paymentNames = dv.Cast<DataRowView>()
				.Select(drv => (string)drv[Constants.FIELD_PAYMENT_PAYMENT_NAME])
				.ToArray();
			return paymentNames;
		}
		#endregion

		#region +GetValidPayments
		/// <summary>
		/// Get valid payments
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>A array of payment models</returns>
		internal PaymentModel[] GetValidPayments(string shopId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PAYMENT_SHOP_ID, shopId },
			};
			var dv = Get(XML_KEY_NAME, "GetValidPayments", input);

			return dv.Cast<DataRowView>()
				.Select(drv => new PaymentModel(drv))
				.ToArray();
		}
		#endregion
	}
}
