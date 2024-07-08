/*
=========================================================================================================
  Module      : 注文取込クラス(OrderImport_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using MarketplaceWebServiceOrders.Model;
using w2.App.Common.Mall.Mapping;
using w2.Commerce.Batch.LiaiseAmazonMall.Util;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.User;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Import
{
	/// <summary>
	/// 注文取込クラス
	/// </summary>
	public partial class OrderImport : ImportBase
	{
		#region -SetUserId ユーザID設定
		/// <summary>
		/// ユーザID設定
		/// </summary>
		/// <returns>ユーザID</returns>
		private string SetUserId()
		{
			this.AmazonOrder.IsNewUser = false;

			// ログインにメールアドレスを利用している場合は、メールアドレスをキーに登録済みユーザであるか検索
			if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
				&& (this.AmazonOrder.Order.IsSetBuyerEmail() || this.IsImportedOrder))
			{
				this.AmazonOrder.User = new UserService().GetUserByMailAddr(this.AmazonOrder.Order.BuyerEmail, this.Accessor);
			}

			// 登録済みユーザであった場合は、取得したユーザIDを設定
			if (this.AmazonOrder.User != null)
			{
				this.AmazonOrder.DeleteNecessaryUserId = SetDeleteNecessaryUserId();
				return this.AmazonOrder.User.UserId;
			}

			// 取込済み注文の場合は、ユーザIDを変えない
			if (this.IsImportedOrder) return this.AmazonOrder.ImportedOrder.UserId;

			// ユーザID発番
			this.AmazonOrder.IsNewUser = true;
			var userId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			return userId;
		}
		#endregion

		#region -SetDeleteNecessaryUserId 削除が必要なユーザIDを設定
		/// <summary>
		/// 削除が必要なユーザIDを設定
		/// </summary>
		/// <returns>削除が必要なユーザID</returns>
		/// <remarks>
		/// 保留注文取込時にはBuyerMailが含まれていないため仮ユーザIDを発番して設定している。
		/// 確定注文取込時にメールアドレスをキーに登録済みユーザであることが分かった場合、仮ユーザIDを削除する
		/// </remarks>
		private string SetDeleteNecessaryUserId()
		{
			if (this.IsImportedOrder == false) return string.Empty;

			if (this.AmazonOrder.User.UserId != this.AmazonOrder.ImportedOrder.UserId)
			{
				return this.AmazonOrder.ImportedOrder.UserId;
			}
			return string.Empty;
		}
		#endregion

		#region -ConvertOrderPaymentKbn 支払区分変換
		/// <summary>
		/// 支払区分変換
		/// </summary>
		/// <returns>支払区分</returns>
		private string ConvertOrderPaymentKbn()
		{
			string orderPaymentKbn = string.Empty;
			if (this.AmazonOrder.Order.IsSetPaymentMethod() == false)
			{
				return orderPaymentKbn;
			}

			orderPaymentKbn = AmazonMappingManager.GetInstance().ConvertOrderPaymentKbn(
				this.MallId,
				this.AmazonOrder.Order.PaymentMethod);

			return orderPaymentKbn;
		}
		#endregion

		#region -SetOrderStatus 注文ステータス設定
		/// <summary>
		/// 注文ステータス設定
		/// </summary>
		/// <returns>注文ステータス</returns>
		private string SetOrderStatus()
		{
			string orderStatus = string.Empty;
			switch (this.AmazonOrder.Order.OrderStatus)
			{
				case Constants.AMAZON_MALL_ORDER_STATUS_PENDING:
					orderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;
					break;

				case Constants.AMAZON_MALL_ORDER_STATUS_PARTIALLY_SHIPPED:
				case Constants.AMAZON_MALL_ORDER_STATUS_UNSHIPPED:
					orderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
					break;

				case Constants.AMAZON_MALL_ORDER_STATUS_CANCELED:
					if (this.IsImportedOrder == false)
					{
						orderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED;
					}
					else
					{
						orderStatus = (this.AmazonOrder.ImportedOrder.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
							? Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED
							: Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED;
					}
					break;
			}
			return orderStatus;
		}
		#endregion

		#region -SetRelationMemo 外部連携メモ設定
		/// <summary>
		/// 外部連携メモ設定
		/// </summary>
		/// <returns>外部連携メモ</returns>
		/// <remarks>ギフト包装の商品の場合に設定</remarks>
		private string SetRelationMemo()
		{
			var relationMemo = new StringBuilder();
			foreach (var orderItem in this.AmazonOrder.OrderItem)
			{
				// ギフト未設定の場合はスキップ
				if ((orderItem.IsSetGiftMessageText() == false) && (orderItem.IsSetGiftWrapLevel() == false)) continue;
				relationMemo.AppendLine(string.Format("Amazon注文商品ID：{0}", orderItem.OrderItemId));
				if (orderItem.IsSetGiftWrapLevel())
				{
					relationMemo.AppendLine(string.Format("ギフト包装ID：{0}", orderItem.GiftWrapLevel));
				}
				if (orderItem.IsSetGiftMessageText())
				{
					relationMemo.AppendLine(string.Format("ギフトメッセージ：\r\n{0}", orderItem.GiftMessageText));
				}
			}
			return relationMemo.ToString();
		}
		#endregion

		#region -SetRegulationMemo 調整金額メモ設定
		/// <summary>
		/// 調整金額メモ設定
		/// </summary>
		/// <returns>調整金額メモ</returns>
		private string SetRegulationMemo()
		{
			var regulationMemo = new StringBuilder();
			regulationMemo.AppendLine(string.Format("ギフト包装料：{0}",
				StringUtility.ToPrice(this.AmazonOrder.OrderItem.Sum(i => i.IsSetGiftWrapPrice() ? Convert.ToDecimal(i.GiftWrapPrice.Amount) : 0))));
			regulationMemo.AppendLine(string.Format("プロモーション値引き額：{0}",
				StringUtility.ToPrice(this.AmazonOrder.OrderItem.Sum(i => i.IsSetPromotionDiscount() ? Convert.ToDecimal(i.PromotionDiscount.Amount) * -1 : 0))));
			regulationMemo.AppendLine(string.Format("配送料値引き額：{0}",
				StringUtility.ToPrice((this.AmazonOrder.OrderItem.Sum(i => i.IsSetShippingDiscount() ? Convert.ToDecimal(i.ShippingDiscount.Amount) * -1 : 0)))));
			regulationMemo.AppendLine(string.Format("代引き手数料値引き額：{0}",
				StringUtility.ToPrice((this.AmazonOrder.OrderItem.Sum(i => i.IsSetCODFeeDiscount() ? Convert.ToDecimal(i.CODFeeDiscount.Amount) * -1 : 0)))));
			regulationMemo.AppendLine(string.Format(
				"Amazonポイント利用分：{0}",
				StringUtility.ToPrice(this.AmazonOrder.Order.IsSetPaymentExecutionDetail()
					? this.AmazonOrder.Order.PaymentExecutionDetail.Sum(i =>
						(i.PaymentMethod == Constants.AMAZON_PAYMENTMETHOD_POINTSACCOUNT) ? ConvertUtility.ConvertStringToDecimal(i.Payment.Amount) * -1 : 0)
					: 0)));
			regulationMemo.AppendLine(string.Format(
				"Amazonギフト券利用分：{0}",
				StringUtility.ToPrice(this.AmazonOrder.Order.IsSetPaymentExecutionDetail()
					? this.AmazonOrder.Order.PaymentExecutionDetail.Sum(i =>
						(i.PaymentMethod == Constants.AMAZON_PAYMENTMETHOD_GIFTCARD) ? ConvertUtility.ConvertStringToDecimal(i.Payment.Amount) * -1 : 0)
					: 0)));

			return regulationMemo.ToString();
		}
		#endregion

		#region -SetOrderPriceTotal 支払金額合計設定
		/// <summary>
		/// 支払金額合計設定
		/// </summary>
		/// <returns></returns>
		/// <remarks>元注文のため最終請求金額にも同一金額を設定する</remarks>
		private decimal SetOrderPriceTotal()
		{
			var amazonOrderTotal = this.AmazonOrder.Order.IsSetOrderTotal() ? ConvertUtility.ConvertStringToDecimal(this.AmazonOrder.Order.OrderTotal.Amount) : 0;

			// 代金引換の場合、購入時に事前に支払った金額分を計算する(Amazonポイント利用分 - Amazonギフト券利用分)
			var codPriorPayment = this.AmazonOrder.Order.IsSetPaymentExecutionDetail()
					? this.AmazonOrder.Order.PaymentExecutionDetail.Sum(i =>
						(i.PaymentMethod == Constants.AMAZON_PAYMENTMETHOD_POINTSACCOUNT || i.PaymentMethod == Constants.AMAZON_PAYMENTMETHOD_GIFTCARD)
							? ConvertUtility.ConvertStringToDecimal(i.Payment.Amount) : 0)
					: 0;
			return amazonOrderTotal - codPriorPayment;
		}
		#endregion


		#region -SetOrderPriceRegulation 調整金額設定
		/// <summary>
		/// 調整金額設定
		/// </summary>
		/// <returns>調整金額</returns>
		private decimal SetOrderPriceRegulation()
		{
			// ギフト包装料 + ギフト包装料に対する税金 - プロモーション値引きの合計額 - 配送料の値引き額 - 代引き手数料の値引き額
			var orderPriceRegulation = this.AmazonOrder.OrderItem.Sum(i =>
				((i.IsSetGiftWrapPrice() ? ConvertUtility.ConvertStringToDecimal(i.GiftWrapPrice.Amount) : 0))
				+ (i.IsSetGiftWrapTax() ? ConvertUtility.ConvertStringToDecimal(i.GiftWrapTax.Amount) : 0)
				- (i.IsSetPromotionDiscount() ? ConvertUtility.ConvertStringToDecimal(i.PromotionDiscount.Amount) : 0)
				- (i.IsSetShippingDiscount() ? ConvertUtility.ConvertStringToDecimal(i.ShippingDiscount.Amount) : 0)
				- (i.IsSetCODFeeDiscount() ? ConvertUtility.ConvertStringToDecimal(i.CODFeeDiscount.Amount) : 0));

			// 代金引換の場合、購入時に事前に支払った金額分を計算し、減算する(Amazonポイント利用分 - Amazonギフト券利用分)
			var codPriorPayment = this.AmazonOrder.Order.IsSetPaymentExecutionDetail()
					? this.AmazonOrder.Order.PaymentExecutionDetail.Sum(i =>
						(i.PaymentMethod == "PointsAccount" || i.PaymentMethod == "GC") ? ConvertUtility.ConvertStringToDecimal(i.Payment.Amount) : 0)
					: 0;
			return orderPriceRegulation - codPriorPayment;
		}
		#endregion

		#region -SetMallLinkStatus モール連携ステータス設定
		/// <summary>
		/// モール連携ステータス設定
		/// </summary>
		/// <returns>モール連携ステータス</returns>
		private string SetMallLinkStatus()
		{
			string mallLinkStatus = string.Empty;
			switch (this.AmazonOrder.Order.OrderStatus)
			{
				case Constants.AMAZON_MALL_ORDER_STATUS_PENDING:
					mallLinkStatus = Constants.FLG_ORDER_MALL_LINK_STATUS_PEND_ODR;
					break;

				case Constants.AMAZON_MALL_ORDER_STATUS_UNSHIPPED:
				case Constants.AMAZON_MALL_ORDER_STATUS_PARTIALLY_SHIPPED:
					mallLinkStatus = Constants.FLG_ORDER_MALL_LINK_STATUS_UNSHIP_ODR;
					break;

				case Constants.AMAZON_MALL_ORDER_STATUS_CANCELED:
					mallLinkStatus = Constants.FLG_ORDER_MALL_LINK_STATUS_CANCEL;
					break;
			}
			return mallLinkStatus;
		}
		#endregion

		#region -GetProductByAmazonSKU Amazon出品者SKUから商品情報取得
		/// <summary>
		/// Amazon出品者SKUから商品情報取得
		/// </summary>
		/// <param name="sellerSKU">出品者SKU</param>
		/// <returns>商品情報</returns>
		private ProductModel GetProductByAmazonSKU(string sellerSKU)
		{
			var searchCondition = CreateProductSearchCondition(sellerSKU);
			var product = new ProductService().GetProductByAmazonSku(Constants.CONST_DEFAULT_SHOP_ID, searchCondition);
			// 見つからなかったらエラー出す
			if ((product == null) || product.Length == 0) { throw new NoMatchItemException(sellerSKU); }

			return product[0];
		}
		#endregion

		#region -CreateProductSearchCondition 商品情報検索条件作成
		/// <summary>
		/// 商品情報検索条件作成
		/// </summary>
		/// <returns></returns>
		private string CreateProductSearchCondition(string sellerSKU)
		{
			// 商品紐づけ用のカラム名を設定
			var noVariationColumn = string.Format("variation_{0}", Constants.SELLERSKU_FOR_NO_VARIATION);
			var useVariationColumn = Constants.SELLERSKU_FOR_USE_VARIATION;

			return string.Format(
				"{0} = '{1}' OR {2} = '{3}'",
				noVariationColumn,
				sellerSKU,
				useVariationColumn,
				sellerSKU);
		}
		#endregion

		#region -SetOwnerKbn 注文者区分設定
		/// <summary>
		/// 注文者区分設定
		/// </summary>
		/// <returns>注文者区分</returns>
		private string SetOwnerKbn()
		{
			string ownerKbn = string.Empty;
			// 取込済み注文の場合はそのまま注文者区分を引き継ぐ（※ただし削除が必要なユーザ情報だった場合は、メールアドレスをキーに取得したユーザ情報を設定）
			if (this.IsImportedOrder)
			{
				ownerKbn = string.IsNullOrEmpty(this.AmazonOrder.DeleteNecessaryUserId)
					? this.AmazonOrder.ImportedOrder.Owner.OwnerKbn
					: this.AmazonOrder.User.UserKbn;
				this.AmazonOrder.UserKbn = ownerKbn;
				return ownerKbn;
			}

			// 新規注文の場合は、「ログイン時のメールアドレス使用有無」と「新規会員登録時の顧客区分」の設定によって判定
			if (this.AmazonOrder.IsNewUser)
			{
				if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED))
				{
					ownerKbn = (Constants.LIAISE_AMAZON_MALL_DEFAULT_USER_KBN == Constants.USER_KBN_USER)
						? Constants.FLG_USER_USER_KBN_PC_USER : Constants.FLG_USER_USER_KBN_PC_GUEST;
				}
				else
				{
					ownerKbn = (Constants.LIAISE_AMAZON_MALL_DEFAULT_USER_KBN == Constants.USER_KBN_USER)
						? Constants.FLG_USER_USER_KBN_OFFLINE_USER : Constants.FLG_USER_USER_KBN_OFFLINE_GUEST;
				}
			}

			this.AmazonOrder.UserKbn = ownerKbn;
			return ownerKbn;
		}
		#endregion

		#region -SetOrderItemCooperationId OrderItem商品連携ID設定
		/// <summary>
		/// OrderItem商品連携ID設定
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		/// <param name="product">商品情報</param>
		/// <param name="amazonOrderItem">Amazon注文商品情報</param>
		/// <returns>商品連携ID設定後注文商品情報</returns>
		private OrderItemModel SetOrderItemCooperationId(OrderItemModel orderItem, ProductModel product, OrderItem amazonOrderItem)
		{
			// Configに指定したカラム名を無理やり加工してプロパティ名を作成
			var cooperationIdColumn = product.UseVariationFlg == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE
				? (Constants.SELLERSKU_FOR_USE_VARIATION).Replace("variation_", string.Empty)
				: Constants.SELLERSKU_FOR_NO_VARIATION;
			var propertyNameWorkList = cooperationIdColumn.Split('_');
			var cooperation = propertyNameWorkList[0].Substring(0, 1).ToUpper() + propertyNameWorkList[0].Substring(1, propertyNameWorkList[0].Length - 1);
			var id = propertyNameWorkList[1].Substring(0, 1).ToUpper() + propertyNameWorkList[1].Substring(1, propertyNameWorkList[1].Length - 1);

			var propertyName = string.Concat(cooperation, id);
			var type = typeof(OrderItemModel);
			var propertyInfo = type.GetProperty(propertyName);
			propertyInfo.SetValue(orderItem, amazonOrderItem.SellerSKU);

			return orderItem;
		}
		#endregion

		#region -SetShippingTime 配送希望時間帯設定
		/// <summary>
		/// 配送希望時間帯設定
		/// </summary>
		/// <returns>配送希望時間帯ID</returns>
		private string SetShippingTime()
		{
			string shippingTime = string.Empty;
			if ((this.AmazonOrder.Order.IsSetEarliestDeliveryDate() == false) || (this.AmazonOrder.Order.IsSetLatestDeliveryDate() == false))
			{
				return shippingTime;
			}

			shippingTime = AmazonMappingManager.GetInstance().ConvertShippingTime(
				this.MallId,
				this.AmazonOrder.Order.EarliestDeliveryDate,
				this.AmazonOrder.Order.LatestDeliveryDate);

			return shippingTime;
		}
		#endregion
	}
}
