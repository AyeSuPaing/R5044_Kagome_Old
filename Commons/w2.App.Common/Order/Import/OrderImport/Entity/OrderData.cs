/*
=========================================================================================================
  Module      : 注文データクラス(OrderData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.MemberRank;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.User;

namespace w2.App.Common.Order.Import.OrderImport.Entity
{
	/// <summary>
	/// 注文データ
	/// </summary>
	public class OrderData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderData">注文データ</param>
		public OrderData(Hashtable orderData)
		{
			this.CsvOrderData = new List<Hashtable>() { orderData };
		}

		/// <summary>
		/// チェックデータ
		/// </summary>
		/// <returns>データOKか？</returns>
		public bool CheckData()
		{
			var result = true;

			this.UserOrg = new UserService().Get(this.UserId);

			if (this.OrderId == "")
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NOORDER_ID));
				result = false;
			}

			if (this.UserId == "")
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NOUSER_ID));
				result = false;
			}
			else
			{
				var duplicatedUser = false;
				if (this.CsvOrderData.Count > 1)
				{
					duplicatedUser = (this.CsvOrderData.Select(i => i[Constants.FIELD_ORDER_USER_ID]).Distinct().ToArray().Length > 1);
				}
				if (duplicatedUser)
				{
					this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_MULTI_USER_ID));
					result = false;
				}
				else if (this.UserOrg != null)
				{
					if (this.UserOrg.Name != this.Order.Owner.OwnerName)
					{
						this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NAME_NOT_SAME)
							.Replace("@@ 1 @@", this.UserOrg.Name)
							.Replace("@@ 2 @@", this.Order.Owner.OwnerName));
						result = false;
					}
				}
			}

			// 定期台帳登録用の項目チェック
			var fixedPurchaseFieldErrorMessage = CheckFixedPurchaseField(this.CsvOrderData.ToArray());
			if (string.IsNullOrEmpty(fixedPurchaseFieldErrorMessage) == false)
			{
				this.ErrorMessage.AppendLine(fixedPurchaseFieldErrorMessage);
				result = false;
			}

			// 注文商品チェック
			var orderItemErrorMessage = CheckOrderItem(this.CsvOrderData.ToArray());
			if (string.IsNullOrEmpty(orderItemErrorMessage) == false)
			{
				this.ErrorMessage.AppendLine(orderItemErrorMessage);
				result = false;
			}

			// 定期設定関連チェック
			var fixedPurchaseSettingErrorMessage = this.IsRegistFixedPurchase 
				? CheckFixedPurchaseSettingByOrderItem(this.CsvOrderData.ToArray()) 
				: "";
			if (string.IsNullOrEmpty(fixedPurchaseSettingErrorMessage) == false)
			{
				this.ErrorMessage.AppendLine(fixedPurchaseSettingErrorMessage);
				result = false;
			}

			// 領収書情報チェック
			var receiptInfoErrorMessage = CheckReceipInfo(this.CsvOrderData.ToArray());
			if (string.IsNullOrEmpty(receiptInfoErrorMessage) == false)
			{
				this.ErrorMessage.AppendLine(receiptInfoErrorMessage);
				result = false;
			}

			// Check Shipping Country Iso Code
			var shippingCountryIsoCodeErrorMessage = CheckShippingCountryIsoCode(this.CsvOrderData.ToArray());
			if (string.IsNullOrEmpty(shippingCountryIsoCodeErrorMessage) == false)
			{
				this.ErrorMessage.AppendLine(shippingCountryIsoCodeErrorMessage);
				result = false;
			}

			return result;
		}

		/// <summary>
		/// 定期台帳登録用の項目チェック
		/// </summary>
		/// <param name="order">注文データ</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckFixedPurchaseField(Hashtable[] order)
		{
			var fixedPurchaseId = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			var fixedPurchaseKbn = StringUtility.ToEmpty(order[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]);
			var fixedPurchaseSetting1 = StringUtility.ToEmpty(order[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]);

			if (string.IsNullOrEmpty(fixedPurchaseId)
				&& ((string.IsNullOrEmpty(fixedPurchaseKbn) == false) || (string.IsNullOrEmpty(fixedPurchaseSetting1) == false)))
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NO_FIXED_PURCHASE_ID);
			}

			if ((new FixedPurchaseService().Get(StringUtility.ToEmpty(this.FixedPurchaseId)) != null)
				&& ((string.IsNullOrEmpty(fixedPurchaseKbn) == false) || (string.IsNullOrEmpty(fixedPurchaseSetting1) == false)))
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_EXIST_FIXED_PURCHASE_ID);
			}

			if (string.IsNullOrEmpty(fixedPurchaseKbn) ^ string.IsNullOrEmpty(fixedPurchaseSetting1))
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_THE_OTHER_FIXED_PURCHASE_ID);
			}

			return "";
		}

		/// <summary>
		/// 注文データチェック
		/// </summary>
		/// <param name="order">注文データ</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckOrderItem(Hashtable[] order)
		{
			var user = new UserService().Get(StringUtility.ToEmpty(order[0][Constants.FIELD_USER_USER_ID]));
			var shopShipping = new ShopShippingService().Get(
				StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_SHOP_ID]),
				StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_SHIPPING_ID]));
			var payment = DataCacheControllerFacade.GetPaymentCacheController().Get(
				StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]));

			if (shopShipping == null)
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IRREGULAR_SHIPPING_ID)
					.Replace("@@ 1 @@", StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_SHIPPING_ID]));
			}

			if (payment == null)
			{
				MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NOT_EXIST_PAYMENT)
					.Replace("@@ 1 @@", StringUtility.ToEmpty(order[0][Constants.FIELD_ORDERITEM_PRODUCT_ID]));
			}
			else if ((shopShipping.PaymentSelectionFlg == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID) && shopShipping.PermittedPaymentIds.Split(',').Contains(payment.PaymentId) == false)
			{
				MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PAYMENT_SHIPPING_ID_ERROR)
					.Replace("@@ 1 @@", shopShipping.ShippingId)
					.Replace("@@ 2 @@", payment.PaymentId)
					.Replace("@@ 3 @@", shopShipping.PermittedPaymentIds);
			}

			// 商品ごとのチェック
			var productService = new ProductService();
			foreach (var orderItem in this.CsvOrderData)
			{
				var product = productService.GetProductVariation(
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDER_SHOP_ID]),
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID]),
					(user != null) ? user.MemberRankId : "");

				if (StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDER_SHIPPING_ID]) != shopShipping.ShippingId)
				{
					return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_SHIPPING_ID_NOT_SAME)
						.Replace("@@ 1 @@", StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]))
						.Replace("@@ 2 @@", product.ShippingId);
				}

				if (product == null)
				{
					return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NOT_EXIST_PRODUCT_ID)
						.Replace("@@ 1 @@", StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]));
				}
				else
				{
					if (product.IsBuyable == false)
					{
						return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DISABLE_PRODUCT_ID)
							.Replace("@@ 1 @@", StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]));
					}

					if (user == null)
					{
						if (string.IsNullOrEmpty(product.BuyableMemberRank) == false)
						{
							return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_MEMBER_RANK_LACK)
								.Replace("@@ 1 @@", StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]))
								.Replace("@@ 2 @@", product.BuyableMemberRank);
						}
					}
					else
					{
						var productMemberRank = DomainFacade.Instance.MemberRankService.GetMemberRankList().FirstOrDefault(model => model.MemberRankId == product.BuyableMemberRank);
						var userMemberRank = DomainFacade.Instance.MemberRankService.GetMemberRankList().FirstOrDefault(model => model.MemberRankId == user.MemberRankId);
						if ((productMemberRank != null) && ((userMemberRank == null) || (productMemberRank.MemberRankOrder < userMemberRank.MemberRankOrder)))
						{
							return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_MEMBER_RANK_LACK_NAME)
								.Replace("@@ 1 @@", StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]))
								.Replace("@@ 2 @@", product.BuyableMemberRank)
								.Replace("@@ 3 @@", user.MemberRankId);
						}
					}
				}
			}

			return "";
		}

		/// <summary>
		/// 定期関連チェック
		/// </summary>
		/// <param name="order">注文データ</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckFixedPurchaseSettingByOrderItem(Hashtable[] order)
		{
			var user = new UserService().Get(StringUtility.ToEmpty(order[0][Constants.FIELD_USER_USER_ID]));
			var shopShipping = new ShopShippingService().Get(
				StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_SHOP_ID]),
				StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_SHIPPING_ID]));
			var payment = DataCacheControllerFacade.GetPaymentCacheController().Get(
				StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]));

			var fixedPurchaseKbn = StringUtility.ToEmpty(order[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]);
			var fixedPurchaseSetting1 = StringUtility.ToEmpty(order[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]);

			if (shopShipping.FixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID)
			{
				if (CheckFixedPurchaseKbn(fixedPurchaseKbn) == false)
				{
					return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_FIXED_PURCHASE)
						.Replace("@@ 1 @@", shopShipping.ShippingId)
						.Replace("@@ 2 @@", fixedPurchaseKbn); ;
				}

				if ((fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
					&& (((shopShipping.IsValidFixedPurchaseKbn1Flg) && (shopShipping.FixedPurchaseKbn1Setting
						.Split(',').Contains(fixedPurchaseSetting1.Split(',')[0]))) == false))
				{
					return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_FIXED_PURCHASE_CYCLUS)
						.Replace("@@ 1 @@", shopShipping.ShippingId)
						.Replace("@@ 2 @@", fixedPurchaseKbn)
						.Replace("@@ 3 @@", fixedPurchaseSetting1);
				}

				if ((fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY)
					&& (shopShipping.IsValidFixedPurchaseKbn2Flg == false))
				{
					return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_FIXED_PURCHASE)
						.Replace("@@ 1 @@", shopShipping.ShippingId)
						.Replace("@@ 2 @@", fixedPurchaseKbn);
				}

				if ((fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
					&& ((shopShipping.IsValidFixedPurchaseKbn3Flg && shopShipping.FixedPurchaseKbn3Setting
						.Split(',').Contains(fixedPurchaseSetting1)) == false))
				{
					return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_FIXED_PURCHASE_CYCLUS)
						.Replace("@@ 1 @@", shopShipping.ShippingId)
						.Replace("@@ 2 @@", fixedPurchaseKbn)
						.Replace("@@ 3 @@", fixedPurchaseSetting1);
				}
			}
			else if (shopShipping.FixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_INVALID) // elseだけでも良いが条件書いた方がわかりやすいと思うんで。
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_SHIPPING_ID)
					.Replace("@@ 1 @@", shopShipping.ShippingId);
			}

			if (payment != null && payment.IsValidFixedPurchase == false)
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_PAYMENT)
					.Replace("@@ 1 @@", payment.PaymentId);
			}

			// 商品ごとのチェック
			var productService = new ProductService();
			foreach (var orderItem in this.CsvOrderData)
			{
				var product = productService.GetProductVariation(
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDER_SHOP_ID]),
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID]),
					(user != null) ? user.MemberRankId : "");

				if (product.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
				{
					return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_PRODUCT_ID)
						.Replace("@@ 1 @@", product.ProductId);
				}
			}

			return "";
		}

		/// <summary>
		/// 領収書情報チェック
		/// </summary>
		/// <param name="order">注文データ</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckReceipInfo(Hashtable[] order)
		{
			// 領収書対応OPが無効の場合、何もしない
			if (Constants.RECEIPT_OPTION_ENABLED == false) return "";

			var receiptFlg = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_RECEIPT_FLG]);
			var receiptAddress = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_RECEIPT_ADDRESS]);
			var receiptProviso = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_RECEIPT_PROVISO]);

			// 領収書希望フラグチェック（「0、1」以外はエラー）
			if ((receiptFlg != Constants.FLG_ORDER_RECEIPT_FLG_OFF)
				&& (receiptFlg != Constants.FLG_ORDER_RECEIPT_FLG_ON))
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_RECEIPT_INPUT_ERROR);
			}

			// 領収書の宛名チェック
			if((receiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON)
				&& string.IsNullOrEmpty(receiptAddress))
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NECCESARY_FROM);
			}

			// 領収書の但し書きチェック
			if((receiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON)
				&& string.IsNullOrEmpty(receiptProviso))
			{
				return MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NECCESARY_PROVISO);
			}

			return "";
		}

		/// <summary>
		/// Check Shipping Country Iso Code
		/// </summary>
		/// <param name="order">order</param>
		/// <returns>Error Message</returns>
		private string CheckShippingCountryIsoCode(Hashtable[] order)
		{
			var orderPaymentKbn = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
			if ((Constants.GLOBAL_OPTION_ENABLE == false)
				|| string.IsNullOrEmpty(orderPaymentKbn)
				|| (orderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)) return string.Empty;

			var ownerCountryIsoCode = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE]);
			var shippingCountryIsoCode = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]);
			var isNotOwnerJp = (string.IsNullOrEmpty(ownerCountryIsoCode)
				|| (GlobalAddressUtil.IsCountryJp(ownerCountryIsoCode) == false));
			var isNotShippingJp = (string.IsNullOrEmpty(shippingCountryIsoCode)
				|| (GlobalAddressUtil.IsCountryJp(shippingCountryIsoCode) == false));
			if (isNotOwnerJp || isNotShippingJp)
			{
				return NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3);
			}
			return string.Empty;
		}

		/// <summary>
		/// 定期購入区分チェック
		/// </summary>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <returns>チェック結果</returns>
		private bool CheckFixedPurchaseKbn(string fixedPurchaseKbn)
		{
			switch (fixedPurchaseKbn)
			{
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					return true;

				default:
					return false;
			}
		}

		/// <summary>注文データ</summary>
		public List<Hashtable> CsvOrderData { get; set; }
		/// <summary>注文情報</summary>
		public OrderModel Order { get; set; }
		/// <summary>ユーザー</summary>
		public UserModel User { get; set; }
		/// <summary>ユーザー</summary>
		public UserModel UserOrg { get; set; }
		/// <summary>カート</summary>
		public CartObject Cart { get; set; }
		/// <summary>クーポン</summary>
		public OrderCouponModel Coupon { get; set; }
		/// <summary>注文ID</summary>
		public string OrderId { get { return (string)this.CsvOrderData[0][Constants.FIELD_ORDER_ORDER_ID]; } }
		/// <summary>ユーザーID</summary>
		public string UserId { get { return (string)this.CsvOrderData[0][Constants.FIELD_ORDER_USER_ID]; } }
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get { return (string)this.CsvOrderData[0][Constants.FIELD_ORDER_FIXED_PURCHASE_ID]; } }
		/// <summary>定期購入区分</summary>
		public string FixedPurchaseKbn { get { return (string)this.CsvOrderData[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]; } }
		/// <summary>定期購入設定１</summary>
		public string FixedPurchaseSetting1 { get { return (string)this.CsvOrderData[0][Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]; } }
		/// <summary>頒布会注文回数</summary>
		public string SubscriptionBoxOrderCount
		{
			get
			{
				return (string)this.CsvOrderData[0][Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT];
			}
		}
		/// <summary>定期台帳を作成する？</summary>
		public bool IsRegistFixedPurchase
		{
			get
			{
				return (Constants.FIXEDPURCHASE_OPTION_ENABLED
					&& (string.IsNullOrEmpty(this.FixedPurchaseKbn) == false)
					&& (string.IsNullOrEmpty(this.FixedPurchaseSetting1) == false)
					&& (string.IsNullOrEmpty(this.FixedPurchaseId) == false));
			}
		}
		/// <summary>エラーメッセージ</summary>
		public StringBuilder ErrorMessage { get { return this.m_errorMessage; } }
		private readonly StringBuilder m_errorMessage = new StringBuilder();
	}
}
