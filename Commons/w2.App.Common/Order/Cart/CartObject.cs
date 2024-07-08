/*
=========================================================================================================
  Module      : カートオブジェクトクラス(CartObject.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Input.Order;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order.Cart;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Wrapper;
using w2.Domain;
using w2.Domain.Cart;
using w2.Domain.ContentsLog;
using w2.Domain.Coupon;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.MemberRank;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Order;
using w2.Domain.OrderMemoSetting;
using w2.Domain.Payment;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.TargetList;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.Cart;
using w2.Domain.SetPromotion;
using w2.Domain.TaskScheduleHistory;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カートオブジェクトクラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class CartObject : IEnumerable
	{
		/// <summary>配送料金上書き用</summary>
		private decimal m_shippingPrice = -1m;
		/// <summary>支払い金額総合計上書き用</summary>
		private decimal m_priceTotal = -1m;

		#region 古い形式のコンストラクタ（非推奨）
		/// <summary>
		/// コンストラクタ（カートID採番）
		/// </summary>
		/// <param name="userId">カートに紐づくユーザID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="updateCartDb">カート更新の有無</param>
		/// <remarks>この時点でレコードは挿入されない</remarks>
		[Obsolete("[V5.2] デジタルコンテンツのカート分割対応（問題なければ廃止予定）")]
		public CartObject(string userId, string orderKbn, string shopId, string shippingType, bool updateCartDb)
			: this(GetNewCartId(), userId, orderKbn, shopId, shippingType, updateCartDb)
		{
		}
		/// <summary>
		/// コンストラクタ（カートID採番しない）
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <param name="userId">カートに紐づくユーザID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="updateCartDb">カート更新の有無</param>
		/// <remarks>この時点でレコードは挿入されない</remarks>
		[Obsolete("[V5.2] デジタルコンテンツのカート分割対応（問題なければ廃止予定）")]
		public CartObject(string cartId, string userId, string orderKbn, string shopId, string shippingType, bool updateCartDb)
		{
			SetCartObjectParameters(cartId, userId, orderKbn, shopId, shippingType, updateCartDb);
		}
		#endregion

		/// <summary>
		/// コンストラクタ（カートID採番: 商品投入によるカート追加） - デジタルコンテンツのカート分割対応
		/// </summary>
		/// <param name="userId">カートに紐づくユーザID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="isDigitalContentsOnly">デジタルコンテンツ</param>
		/// <param name="updateCartDb">カート更新の有無</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public CartObject(
			string userId,
			string orderKbn,
			string shopId,
			string shippingType,
			bool isDigitalContentsOnly,
			bool updateCartDb,
			string memberRankId = "",
			string subscriptionBoxCourseId = "",
			SqlAccessor accessor = null)
			: this(
				GetNewCartId(),
				userId,
				orderKbn,
				shopId,
				shippingType,
				isDigitalContentsOnly,
				updateCartDb,
				memberRankId,
				subscriptionBoxCourseId,
				accessor)
		{
		}

		/// <summary>
		/// コンストラクタ（カートID採番しない: DBからのカート復元） - デジタルコンテンツのカート分割対応
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <param name="userId">カートに紐づくユーザID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="isDigitalContentsOnly">デジタルコンテンツ</param>
		/// <param name="updateCartDb">カート更新の有無</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="accessor">アクセサ</param>
		public CartObject(
			string cartId,
			string userId,
			string orderKbn,
			string shopId,
			string shippingType,
			bool isDigitalContentsOnly,
			bool updateCartDb,
			string memberRankId = "",
			string subscriptionBoxCourseId = "",
			SqlAccessor accessor = null)
		{
			SetCartObjectParameters(
				cartId,
				userId,
				orderKbn,
				shopId,
				shippingType,
				updateCartDb,
				memberRankId,
				subscriptionBoxCourseId,
				accessor);
			this.IsDigitalContentsOnly = isDigitalContentsOnly;
		}

		/// <summary>
		/// コンストラクタの内部処理を切り出したプライベートメソッド
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <param name="userId">カートに紐づくユーザID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="updateCartDb">カート更新の有無</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="accessor">アクセサ</param>
		/// <remarks>コンストラクタの内部処理を、一時的に切り出しました。非推奨の古いコンストラクタ削除後に戻す予定です。</remarks>
		private void SetCartObjectParameters(string cartId,
			string userId,
			string orderKbn,
			string shopId,
			string shippingType,
			bool updateCartDb,
			string memberRankId = "",
			string subscriptionBoxCourseId = "",
			SqlAccessor accessor = null)
		{
			this.CartId = cartId;
			this.CartUserId = userId;
			this.OrderUserId = userId;
			this.MemberRankId = string.IsNullOrEmpty(memberRankId)
				? MemberRankOptionUtility.GetMemberRankId(this.CartUserId, accessor)
				: memberRankId;
			// 会員ランクIDの引数が空であれば、上記の処理でユーザーDBの会員ランクID取得済みのため、
			// 以降の処理ではカートの会員ランクIDプロパティの値が空でもユーザーDBの会員ランクIDを再度取得しないようにする
			this.IsMemberRankIdFromDb = (string.IsNullOrEmpty(memberRankId)
				&& (string.IsNullOrEmpty(this.CartUserId) == false));
			this.OrderKbn = orderKbn;
			this.ShopId = shopId;
			this.ShippingType = shippingType;
			this.UpdateCartDb = updateCartDb;

			this.Items = new List<CartProduct>();
			this.Shippings = new List<CartShipping>();
			this.Shippings.Add(new CartShipping(this));	// 空の配送先情報を作成（配送料格納用）
			this.SetPromotions = new CartSetPromotionList();
			this.PriceInfoByTaxRate = new List<CartPriceInfoByTaxRate>();

			this.ManagementMemo = string.Empty;
			this.ShippingMemo = string.Empty;
			this.ProductOrderLmitOrderIds = new string[0];
			this.ReflectMemoToFixedPurchase = true;
			// 配送料・決済手数料税率情報の設定
			this.ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE;
			this.PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE;
			// 購入制限チェック対象商品
			this.TargetProductListForCheckProductOrderLimit = new List<CartProduct>();
			this.ReceiptFlg = Constants.FLG_ORDER_RECEIPT_FLG_OFF;
			this.ReceiptAddress = "";
			this.ReceiptProviso = "";
			this.IsUseSameReceiptInfoAsCart1 = false;
			this.IsPreApprovedLinePayPayment = false;
			this.TargetProductRecommends = new List<CartProduct>();
			this.IsBotChanOrder = false;
			this.OrderExtend = OrderExtendCommon.CreateOrderExtend();
			this.SubscriptionBoxCourseId = subscriptionBoxCourseId;
			this.FixedPurchaseDisplayCount = 0;
			this.SubscriptionBoxFixedAmountList = new List<CartSubscriptionBoxFixedAmount>();

			SetSubscriptionBoxInformation();
		}

		/// <summary>
		/// IEnumerable.GetEnumerator()の実装
		/// </summary>
		/// <returns>IEnumerator</returns>
		public IEnumerator GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		/// <summary>
		/// カートテーブルのユーザID更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="isRegistUser">ユーザー新規登録をするか</param>
		public void UpdateCartUserId(string userId, bool isRegistUser = false)
		{
			// DB更新
			if (this.UpdateCartDb)
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("Cart", "UpdateUserId"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_CART_CART_ID, this.CartId);
					htInput.Add(Constants.FIELD_CART_USER_ID, userId);

					sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}
			}

			// プロパティ更新
			this.UpdateUserId(userId);
			this.OrderUserId = userId;
			this.UpdateMemberRank(userId, isRegistUser);
		}

		/// <summary>
		/// 会員ランク更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="isRegistUser">ユーザー新規登録をするか</param>
		public void UpdateMemberRank(string userId, bool isRegistUser = false)
		{
			if (string.IsNullOrEmpty(userId)) return;

			this.MemberRankId = isRegistUser
				? MemberRankOptionUtility.GetDefaultMemberRank()
				: MemberRankOptionUtility.GetMemberRankId(userId);
		}

		/// <summary>
		/// ユーザID更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		public void UpdateUserId(string userId)
		{
			this.CartUserId = userId;
			if (string.IsNullOrEmpty(userId) == false) this.CouponUserId = userId;
		}

		/// <summary>
		/// 新カートID取得
		/// </summary>
		/// <returns>新カートID</returns>
		public static string GetNewCartId()
		{
			// 12桁詰めで取得
			return NumberingUtilityWrapper.Instance.CreateNewNumber(Constants.CONST_DEFAULT_SHOP_ID, Constants.NUMBER_KEY_CART_ID).ToString().PadLeft(Constants.CONST_CART_ID_LENGTH, '0');
		}

		/// <summary>
		/// 商品投入
		/// </summary>
		/// <param name="cpProduct">カート商品情報</param>
		/// <param name="isCopyCart">コピーカートか？</param>
		/// <returns>投入カート商品</returns>
		/// <remarks>カートの配送種別更新は呼出もとで行う</remarks>
		public CartProduct Add(CartProduct cpProduct, bool isCopyCart = false)
		{
			CartProduct cpAddenProduct = null;
			int iInserted = 0;

			//------------------------------------------------------
			// ギフトチェック
			//------------------------------------------------------
			if ((this.Items.Count > 0)
				&& ((cpProduct.IsGift && (this.IsGift == false))
					|| ((cpProduct.IsGift == false) && (this.IsGift))))
			{
				return null;
			}

			this.HasFixedPurchaseUsedToBe = ((this.HasFixedPurchaseUsedToBe) || cpProduct.IsFixedPurchase);

			lock (this)
			{
				//------------------------------------------------------
				// 通常商品の場合の商品追加＆存在チェック
				//------------------------------------------------------
				if (cpProduct.IsSetItem == false)
				{
					if (this.UpdateCartDb)
					{
						iInserted = UpdateCartDbAddProduct(cpProduct);
					}

					// プロパティへ保持する商品情報追加
					if (((iInserted == 1) && (isCopyCart == false)) || (this.UpdateCartDb == false))
					{
						cpAddenProduct = AddVirtural(cpProduct);
					}
				}
				//------------------------------------------------------
				// セット商品の場合のカートの商品追加＆存在チェック
				//------------------------------------------------------
				else
				{
					iInserted = UpdateCartDbAddProductSetItem(cpProduct);

					// プロパティへ保持する商品情報追加
					if ((iInserted == 1) && (isCopyCart == false))
					{
						cpAddenProduct = AddVirtural(cpProduct);
					}
				}

				// コンテンツログ上書き
				if (cpProduct.ContentsLog != null) this.ContentsLog = cpProduct.ContentsLog;
			}

			return cpAddenProduct;
		}

		/// <summary>
		/// 通常商品の商品追加
		/// </summary>
		/// <param name="cartProuct">カート商品</param>
		/// <returns>更新件数</returns>
		private int UpdateCartDbAddProduct(CartProduct cartProuct)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Cart", "AddProduct"))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_CART_CART_ID, this.CartId },
					{ Constants.FIELD_CART_USER_ID, this.CartUserId },
					{ Constants.FIELD_CART_SHOP_ID, cartProuct.ShopId },
					{ Constants.FIELD_CART_PRODUCT_ID, cartProuct.ProductId },
					{ Constants.FIELD_CART_VARIATION_ID, cartProuct.VariationId },
					{ Constants.FIELD_CART_PRODUCT_COUNT, cartProuct.Count },
					{ Constants.FIELD_CART_CART_DIV_TYPE1, cartProuct.ShippingType },
					{ Constants.FIELD_CART_CART_DIV_TYPE2, cartProuct.IsDigitalContents ? Constants.FLG_CART_DIGITAL_CONTENTS_FLG_ON : Constants.FLG_CART_DIGITAL_CONTENTS_FLG_OFF },
					{ Constants.FIELD_CART_CART_DIV_TYPE3, (cartProuct.AddCartKbn == Constants.AddCartKbn.SubscriptionBox) ? cartProuct.SubscriptionBoxCourseId : string.Empty },
					{ Constants.FIELD_CART_FIXED_PURCHASE_FLG, (cartProuct.IsFixedPurchase || cartProuct.IsSubscriptionBox) ? Constants.FLG_CART_FIXED_PURCHASE_FLG_ON : Constants.FLG_CART_FIXED_PURCHASE_FLG_OFF },
					{ Constants.FIELD_CART_GIFT_ORDER_FLG, cartProuct.IsGift ? Constants.FLG_CART_GIFT_ORDER_FLG_FLG_ON : Constants.FLG_CART_GIFT_ORDER_FLG_FLG_OFF },
					{ Constants.FIELD_CART_PRODUCTSALE_ID, cartProuct.ProductSaleId },
					{ Constants.FIELD_CART_PRODUCT_OPTION_TEXTS, cartProuct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() },
					{ Constants.FIELD_CART_NOVELTY_ID, cartProuct.NoveltyId },
					{ Constants.FIELD_CART_RECOMMEND_ID, cartProuct.RecommendId },
				};

				// SQLステートメント実行（既に同じ商品があると、-1がかえる）
				return statement.ExecStatementWithOC(accessor, input);
			}
		}

		/// <summary>
		/// セット商品の商品追加
		/// </summary>
		/// <param name="cartProuct">カート商品</param>
		/// <returns>更新件数</returns>
		private int UpdateCartDbAddProductSetItem(CartProduct cartProuct)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Cart", "AddProductSetItem"))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_CART_CART_ID, this.CartId },
					{ Constants.FIELD_CART_USER_ID, this.CartUserId },
					{ Constants.FIELD_CART_SHOP_ID, cartProuct.ShopId },
					{ Constants.FIELD_CART_PRODUCT_ID, cartProuct.ProductId },
					{ Constants.FIELD_CART_VARIATION_ID, cartProuct.VariationId },
					{ Constants.FIELD_CART_PRODUCT_COUNT, cartProuct.CountSingle },
					{ Constants.FIELD_CART_PRODUCT_SET_ID, cartProuct.ProductSet.ProductSetId },
					{ Constants.FIELD_CART_PRODUCT_SET_NO, cartProuct.ProductSet.ProductSetNo },
					{ Constants.FIELD_CART_PRODUCT_SET_COUNT, cartProuct.ProductSet.ProductSetCount },
					{ Constants.FIELD_CART_CART_DIV_TYPE1, cartProuct.ShippingType },
					{ Constants.FIELD_CART_CART_DIV_TYPE2, cartProuct.IsDigitalContents ? Constants.FLG_CART_DIGITAL_CONTENTS_FLG_ON : Constants.FLG_CART_DIGITAL_CONTENTS_FLG_OFF },
				};

				// SQLステートメント実行（既に同じ商品があると、-1がかえる）
				return statement.ExecStatementWithOC(accessor, input);
			}
		}

		/// <summary>
		/// 複数商品を一括投入（DBへは保存しない）
		/// </summary>
		/// <param name="products">商品情報配列</param>
		/// <param name="execCalculate">再計算を行うか</param>
		/// <returns>追加商品配列</returns>
		public CartProduct[] AddProductsVirtual(CartProduct[] products, bool execCalculate = true)
		{
			var addedProducts = products.Select(product => AddVirtural(product, execCalculate)).ToArray();
			return addedProducts;
		}

		/// <summary>
		/// 商品投入（DBへは保存しない）
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strTimeSaleId">タイムセールID</param>
		/// <param name="iCount">商品数</param>
		/// <param name="strProductOptionSettingTexts">商品付帯情報</param>
		/// <returns>追加商品</returns>
		/// <remarks>カートの配送種別更新は呼出もとで行う</remarks>
		public CartProduct AddVirtural(DataRowView drvProduct, Constants.AddCartKbn addCartKbn, string strTimeSaleId, int iCount, string strProductOptionSettingTexts)
		{
			return AddVirtural(new CartProduct(drvProduct, addCartKbn, strTimeSaleId, iCount, this.UpdateCartDb, strProductOptionSettingTexts, ""));
		}
		/// <summary>
		/// 商品投入（DBへは保存しない）
		/// </summary>
		/// <param name="cpProduct">カート商品</param>
		/// <param name="execCalculate">再計算を行うか</param>
		/// <returns>追加商品</returns>
		/// <remarks>カートの配送種別更新は呼出もとで行う</remarks>
		public CartProduct AddVirtural(CartProduct cpProduct, bool execCalculate = true)
		{
			//------------------------------------------------------
			// ギフトチェック
			//------------------------------------------------------
			if ((this.Items.Count > 0)
				&& ((cpProduct.IsGift && (this.IsGift == false))
					|| ((cpProduct.IsGift == false) && (this.IsGift))))
			{
				return null;
			}

			//------------------------------------------------------
			// 最初のギフト商品投入の場合は配送先は新規登録とする
			//------------------------------------------------------
			if ((this.Items.Count == 0)
				&& cpProduct.IsGift
				&& (this.Shippings.Count == 1)
				&& (this.HasFixedPurchase == false)
				&& (this.HasSubscriptionBox == false))
			{
				this.Shippings[0].ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
			}

			//------------------------------------------------------
			// プロパティへそのまま追加
			//------------------------------------------------------
			this.Items.Add(cpProduct);

			//------------------------------------------------------
			// 再計算（商品追加の際はデフォルト配送先で計算する）
			//------------------------------------------------------
			if (execCalculate)
			{
				Calculate(true, isCartItemChanged: true);
			}

			// 定期配送パターン指定済みの場合は有効な設定でない場合クリアする
			if (ValidateFixedPurchaseShippingPattern(cpProduct) == false)
			{
				var shipping = GetShipping();
				shipping.UpdateFixedPurchaseSetting(string.Empty, string.Empty, 0, 0);
				shipping.CalculateNextShippingDates();
			}

			return cpProduct;
		}

		/// <summary>
		/// 商品セット削除
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		public bool RemoveProductSet(CartProductSet cpsProductSet)
		{
			return RemoveProductSet(cpsProductSet.ProductSetId, cpsProductSet.ProductSetNo);
		}
		/// <summary>
		/// 商品セット削除
		/// </summary>
		/// <param name="strProductSetId">商品セットID</param>
		/// <param name="iProductSetNo">商品セットNo</param>
		public bool RemoveProductSet(string strProductSetId, int iProductSetNo)
		{
			bool blResult = false;

			//------------------------------------------------------
			// 保持する商品情報チェック
			//------------------------------------------------------
			foreach (CartProduct cp in new List<CartProduct>(this.Items))
			{
				if (cp.IsSetItem)
				{
					if ((cp.ProductSet.ProductSetId == strProductSetId)
						&& (cp.ProductSet.ProductSetNo == iProductSetNo))
					{
						//------------------------------------------------------
						// プロパティから削除
						//------------------------------------------------------
						this.Items.Remove(cp);

						//------------------------------------------------------
						// カートの商品削除
						//------------------------------------------------------
						if (this.UpdateCartDb)
						{
							using (SqlAccessor sqlAccessor = new SqlAccessor())
							using (SqlStatement sqlStatement = new SqlStatement("Cart", "DeleteProductSetItem"))
							{
								Hashtable htInput = new Hashtable();
								htInput.Add(Constants.FIELD_CART_CART_ID, this.CartId);
								htInput.Add(Constants.FIELD_CART_SHOP_ID, this.ShopId);
								htInput.Add(Constants.FIELD_CART_PRODUCT_ID, cp.ProductId);
								htInput.Add(Constants.FIELD_CART_VARIATION_ID, cp.VariationId);
								htInput.Add(Constants.FIELD_CART_PRODUCT_SET_ID, strProductSetId);
								htInput.Add(Constants.FIELD_CART_PRODUCT_SET_NO, iProductSetNo);

								int iUpdated = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
							}
						}

						//------------------------------------------------------
						// 再計算（商品削除の際はデフォルト配送先で計算する）
						//------------------------------------------------------
						Calculate(true, isCartItemChanged: true);

						blResult = true;
					}
				}
			}

			return blResult;

			// 商品数0になったときのオブジェクト削除は呼び出し元で行う
		}

		/// <summary>
		/// 商品削除
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID</param>
		/// <param name="ProductOptionValue">商品付帯情報</param>
		public bool RemoveProduct(DataRowView drvProduct, Constants.AddCartKbn addCartKbn, string strProductSaleId, string ProductOptionValue)
		{
			return RemoveProduct(
				(string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID],
				(string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID],
				(string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
				addCartKbn,
				strProductSaleId,
				ProductOptionValue);
		}
		/// <summary>
		/// 商品削除
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID</param>
		/// <param name="strProductOptionValue">商品付帯情報</param>
		/// <param name="isProductDeteleOnly">商品削除のみ</param>
		/// <returns>成功可否</returns>
		public bool RemoveProduct(
			string strShopId,
			string strProductId,
			string strVariationId,
			Constants.AddCartKbn addCartKbn,
			string strProductSaleId,
			string strProductOptionValue,
			bool isProductDeteleOnly = false)
		{
			//------------------------------------------------------
			// ＤＢの商品削除
			//------------------------------------------------------
			if (this.UpdateCartDb)
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("Cart", "DeleteProduct"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_CART_CART_ID, this.CartId);
					htInput.Add(Constants.FIELD_CART_SHOP_ID, strShopId);
					htInput.Add(Constants.FIELD_CART_PRODUCT_ID, strProductId);
					htInput.Add(Constants.FIELD_CART_VARIATION_ID, strVariationId);
					htInput.Add(
						Constants.FIELD_CART_FIXED_PURCHASE_FLG,
						((addCartKbn == Constants.AddCartKbn.FixedPurchase)
							|| (addCartKbn == Constants.AddCartKbn.SubscriptionBox))
							? Constants.FLG_CART_FIXED_PURCHASE_FLG_ON
							: Constants.FLG_CART_FIXED_PURCHASE_FLG_OFF);
					htInput.Add(Constants.FIELD_CART_PRODUCTSALE_ID, strProductSaleId);
					htInput.Add(Constants.FIELD_CART_PRODUCT_OPTION_TEXTS, strProductOptionValue);
					htInput.Add(
						Constants.FIELD_CART_GIFT_ORDER_FLG,
						(addCartKbn == Constants.AddCartKbn.GiftOrder)
							? Constants.FLG_CART_GIFT_ORDER_FLG_FLG_ON
							: Constants.FLG_CART_GIFT_ORDER_FLG_FLG_OFF);

					int iUpdated = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}
			}

			// 商品削除のみの場合、処理終了
			if (isProductDeteleOnly) return true;

			//------------------------------------------------------
			// 保持する商品情報削除＆再計算
			//------------------------------------------------------
			foreach (var cp in this.Items.Where(cp => cp.IsSetItem == false))
			{
				switch (addCartKbn)
				{
					case Constants.AddCartKbn.Normal:
						if ((cp.ShopId == strShopId)
							&& (cp.ProductId == strProductId)
							&& (cp.VariationId == strVariationId)
							&& (cp.IsFixedPurchase == false)
							&& (cp.IsSubscriptionBox == false)
							&& (cp.IsGift == (addCartKbn == Constants.AddCartKbn.GiftOrder))
							&& (cp.ProductSaleId == strProductSaleId)
							&& (cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() == strProductOptionValue))
						{
							RemoveCartItem(cp);
							return true;
						}
						break;

					case Constants.AddCartKbn.FixedPurchase:
						if ((cp.ShopId == strShopId)
							&& (cp.ProductId == strProductId)
							&& (cp.VariationId == strVariationId)
							&& (cp.IsFixedPurchase)
							&& (cp.IsSubscriptionBox == false)
							&& (cp.IsGift == (addCartKbn == Constants.AddCartKbn.GiftOrder))
							&& (cp.ProductSaleId == strProductSaleId)
							&& (cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() == strProductOptionValue))
						{
							RemoveCartItem(cp);
							return true;
						}
						break;

					case Constants.AddCartKbn.SubscriptionBox:
						if ((cp.ShopId == strShopId)
							&& (cp.ProductId == strProductId)
							&& (cp.VariationId == strVariationId)
							&& cp.IsFixedPurchase
							&& cp.IsSubscriptionBox
							&& (cp.IsGift == (addCartKbn == Constants.AddCartKbn.GiftOrder))
							&& (cp.ProductSaleId == strProductSaleId)
							&& (cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() == strProductOptionValue))
						{
							RemoveCartItem(cp);
							return true;
						}
						break;

					case Constants.AddCartKbn.GiftOrder:
						if ((cp.ShopId == strShopId)
							&& (cp.ProductId == strProductId)
							&& (cp.VariationId == strVariationId)
						&& ((cp.IsFixedPurchase == (addCartKbn == Constants.AddCartKbn.FixedPurchase))
							|| (cp.IsSubscriptionBox == (addCartKbn == Constants.AddCartKbn.SubscriptionBox)))
						&& (cp.IsGift == (addCartKbn == Constants.AddCartKbn.GiftOrder))
						&& (cp.ProductSaleId == strProductSaleId)
						&& (cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() == strProductOptionValue))
					{
							RemoveCartItem(cp);
							return true;
						}
						break;
				}
			}

			return false;
			// 商品数0になったときのオブジェクト削除は呼び出し元で行う
		}

		/// <summary>
		/// カートアイテムを削除＆再計算
		/// </summary>
		/// <param name="cp">カート内アイテム</param>
		private void RemoveCartItem(CartProduct cp)
		{
			// プロパティから削除
			this.Items.Remove(cp);
			this.Shippings.ForEach(shipping =>
			{
				shipping.ProductCounts.ForEach(pcount =>
				{
					if (pcount.Product == cp)
					{
						shipping.ProductCounts.Remove(pcount);
					}
				});
			});

			// 再計算（商品削除の際はデフォルト配送先で計算する）
			Calculate(true, isCartItemChanged: true);
		}

		/// <summary>
		/// デフォルト配送先情報で再計算
		/// </summary>
		public void CalculateWithDefaultShipping()
		{
			Calculate(true);
		}

		/// <summary>
		/// カート配送先情報で再計算
		/// </summary>
		/// <param name="isOrderCombined">注文同梱有無</param>
		public void CalculateWithCartShipping(bool isOrderCombined = false)
		{
			Calculate(false, isOrderCombined: isOrderCombined);
		}

		/// <summary>
		/// 再計算処理
		/// </summary>
		/// <param name="isDefaultShipping">デフォルト配送先利用</param>
		/// <param name="isCartItemChanged">カート商品変更有無</param>
		/// <param name="isShippingChanged">配送先変更有無</param>
		/// <param name="isPaymentChanged">決済変更有無</param>
		/// <param name="fixedPurchaseOrderCount">定期購入回数</param>
		/// <param name="isOrderCombined">注文同梱有無</param>
		/// <param name="isSecndFixedPurchase">2回目以降の定期金額表示からの呼び出しか</param>
		/// <param name="isManagerModify">管理画面での注文編集時か</param>
		/// <param name="fixedPurchaseDisplayCount">定期購入表示回数</param>
		/// <param name="isFixedPurchaseOrderPrice">定期金額表示からの呼び出しか</param>
		/// <param name="isFrontOrderModify">フロント受注編集からの呼び出しか</param>
		/// <param name="isFixedPurchaseOrderPrice">定期金額表示からの呼び出しか</param>
		/// <remarks>
		/// 総合計は決済手数料を含まない（注文実行時に決済手数料を含める必要あり）
		/// 定期購入回数は、注文確認画面に各注文回数ごとの金額を表示するための計算に使用
		/// </remarks>
		public void Calculate(
			bool isDefaultShipping,
			bool isCartItemChanged = false,
			bool isShippingChanged = false,
			bool isPaymentChanged = false,
			int fixedPurchaseOrderCount = 0,
			bool isOrderCombined = false,
			bool isSecndFixedPurchase = false,
			bool isManagerModify = false,
			int fixedPurchaseDisplayCount = 0,
			bool isFixedPurchaseOrderPrice = false,
			bool isFrontOrderModify = false)
		{
			this.IsSecondFixedPurchase = isSecndFixedPurchase;
			this.FixedPurchaseDisplayCount = fixedPurchaseDisplayCount;
			this.IsOrderModifyInput = isManagerModify;
			this.IsOrderModify = isFrontOrderModify;
			this.IsOrderModifyInput = isFrontOrderModify;

			// 配送料再計算
			isShippingChanged |= CalculateShippingPrice(isDefaultShipping);

			// Calculate scheduled shipping date
			CalculateScheduledShippingDate(isDefaultShipping, isOrderCombined);

			// 商品金額合計再計算
			isCartItemChanged |= CalculatePriceSubTotal();

			// 割引金額初期化
			InitDiscountPrice();

			// 会員ランク更新(CrossPoint連携時)
			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				UpdateMemberRankByCrossPoint();
			}

			// セットプロモーション計算
			CalculateSetPromotion(isCartItemChanged, isShippingChanged, isPaymentChanged, isManagerModify);

			// 配送料再計算
			CalculateShippingPrice(isDefaultShipping);

			// 定期購入割引額計算
			CalculateFixedPurchaseDiscount(fixedPurchaseOrderCount, isFixedPurchaseOrderPrice);

			// 会員ランク割引額計算
			CalculateMemberRankDiscountPrice();

			// 定期会員割引額計算
			CalculateFixedPurchaseMemberDiscount();

			// クーポン系計算
			CalculateCouponFamily();

			// 使用ポイント割引額計算
			CalculatePointDisCountProductPrice();

			// 調整金額を按分計算
			CalculatePriceRegulation();

			// 税額計算
			CalculateTaxPrice();

			// ポイント系計算
			CalculatePointFamily();

			// 配送料無料時の請求料金計算
			CalculateFreeShippingFee();
		}

		/// <summary>
		/// 商品金額合計再計算
		/// </summary>
		/// <returns>商品小計が変わったか</returns>
		public bool CalculatePriceSubTotal()
		{
			var priceBefore = this.PriceSubtotal;
			this.Items.ToList().ForEach(cp => cp.Calculate());

			this.Shippings
				.ForEach(shipping => shipping.ProductCounts
					.ForEach(pc => pc.PriceSubtotalAfterDistribution = shipping.IsDutyFree
						? (pc.Product.Price * pc.Count)
						: (pc.Product.PricePretax * pc.Count)));

			var subscriptionBoxFixedAmountList = new List<CartSubscriptionBoxFixedAmount>();

			this.PriceSubtotal = 0;
			var isChanged = false;

			// 加算済み頒布会定額コースのIDリスト
			var calculatedSubscriptionBoxCourseIds = new List<string>();
			foreach (var cp in this.Items)
			{
				// 割引額の按分は実際の税込価格に対して行う
				var priceSubtotalPreTax = this.HasUnAllocatedProductToShipping
					? this.Shippings[0].IsDutyFree ? cp.PriceSubtotal : cp.PriceSubtotalPretax
					: this.Shippings.SelectMany(shipping => shipping.ProductCounts).Where(pc => (pc.Product == cp))
						.Sum(pc => pc.PriceSubtotalAfterDistribution);
				// 商品小計(按分適用後)の初期化
				cp.PriceSubtotalAfterDistribution = priceSubtotalPreTax;
				// キャンペーン（ノベルティ、クーポンなど）適用商品小計(按分適用後)の初期化
				cp.PriceSubtotalAfterDistributionForCampaign = priceSubtotalPreTax;

				// 頒布会の計算済み商品を除く
				if (calculatedSubscriptionBoxCourseIds.Contains(cp.SubscriptionBoxCourseId)) continue;

				// 定額コースの計算
				if (cp.IsSubscriptionBoxFixedAmount())
				{
					this.PriceSubtotal += cp.SubscriptionBoxFixedAmount.Value;
					calculatedSubscriptionBoxCourseIds.Add(cp.SubscriptionBoxCourseId);
					subscriptionBoxFixedAmountList.Add(
						new CartSubscriptionBoxFixedAmount(
							cp.SubscriptionBoxCourseId,
							cp.SubscriptionBoxFixedAmount.Value));
					continue;
				}

				// 表示回数分の計算
				if ((string.IsNullOrEmpty(cp.SubscriptionBoxCourseId) == false) && (this.FixedPurchaseDisplayCount > 0))
				{
					var updateSubscriptionBoxResult = GetSubscriptionBoxNextProducts(this.FixedPurchaseDisplayCount);
					decimal total = 0;
					if (updateSubscriptionBoxResult.Result == SubscriptionBoxGetNextProductsResult.ResultTypes.Success || updateSubscriptionBoxResult.Result == SubscriptionBoxGetNextProductsResult.ResultTypes.PartialFailure)
					{
						foreach (var item in updateSubscriptionBoxResult.NextProducts)
						{
							var product = ProductCommon.GetProductVariationInfo(
								this.ShopId,
								item.ProductId,
								item.VariationId,
								this.MemberRankId);

							var cartProduct = SetFixedPurhcasePriceForCartProduct(
								new CartProduct(
									product[0],
									Constants.AddCartKbn.SubscriptionBox,
									"",
									item.ItemQuantity,
									true,
									new ProductOptionSettingList(),
									string.Empty,
									null,
									this.SubscriptionBoxCourseId));

							total += cartProduct.Price * item.ItemQuantity;
						}
					}

					this.PriceSubtotal += total;
					calculatedSubscriptionBoxCourseIds.Add(cp.SubscriptionBoxCourseId);
					continue;
				}

				// セット個数を考慮した小計を加算
				this.PriceSubtotal += cp.PriceSubtotal;
			}

			this.SubscriptionBoxFixedAmountList = subscriptionBoxFixedAmountList;
			return priceBefore != this.PriceSubtotal;
		}

		/// <summary>
		/// 頒布会次回配送商品取得
		/// </summary>
		/// <param name="displayCount">表示回数</param>
		/// <returns>頒布会次回配送商品更新結果</returns>
		private SubscriptionBoxGetNextProductsResult GetSubscriptionBoxNextProducts(int displayCount)
		{
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);
			if (subscriptionBox == null) return new SubscriptionBoxGetNextProductsResult();

			var nextDay = this.Shippings.Select(d => d.NextShippingDate).FirstOrDefault();
			var fixedPurchaseSetting = this.Shippings.Select(t => t.FixedPurchaseSetting).FirstOrDefault();
			var monthMulti = Convert.ToInt16(fixedPurchaseSetting.Split(',')[0]);
			var monthNow = GetMonthToCalculate(nextDay, monthMulti, displayCount);
			var year = DateTime.Now.Year;
			year += monthNow / 12;
			monthNow = (monthNow % 12) == 0 ? 12 : monthNow % 12;
			var fixedPurchaseKbn = this.Shippings.Select(s => s.FixedPurchaseKbn).FirstOrDefault();
			var datetimeCompare = GetDateTimeCompareForSubscriptionBox(
				fixedPurchaseKbn,
				displayCount - 1,
				nextDay,
				monthMulti,
				fixedPurchaseSetting,
				year,
				monthNow);

			var defaultOrderProducts = subscriptionBox.GetDefaultProducts(displayCount);

			var canTakeOver = defaultOrderProducts
				.Any(defaultProduct => string.IsNullOrEmpty(defaultProduct.ProductId));
			if (canTakeOver == false)
			{
				var result = new SubscriptionBoxService().GetFixedPurchaseNextProduct(
					this.SubscriptionBoxCourseId,
					string.Empty,
					this.MemberRankId,
					datetimeCompare,
					displayCount,
					null);
				return result;
			}
			var beforeSubscriptionCount = 1;
			var takeOverCompare = datetimeCompare;
			// 引き継ぎ対象の商品があるかチェック
			for (beforeSubscriptionCount = displayCount - 1; beforeSubscriptionCount > 1; beforeSubscriptionCount--)
			{
				takeOverCompare = GetDateTimeCompareForSubscriptionBox(
					fixedPurchaseKbn,
					beforeSubscriptionCount - 1,
					nextDay,
					monthMulti,
					fixedPurchaseSetting,
					year,
					monthNow);
				defaultOrderProducts = subscriptionBox.GetDefaultProducts(beforeSubscriptionCount, takeOverCompare);
				canTakeOver = defaultOrderProducts
					.Any(defaultOrderProduct => string.IsNullOrEmpty(defaultOrderProduct.ProductId));
				if (canTakeOver == false) break;
			}

			// 表示回数が2回目の場合と、2回目から表示回数までが全て前回の商品を引き継ぐの場合はカートと同じ金額を入れる
			if ((beforeSubscriptionCount == 1))
			{
				// 初回商品に必須商品があった場合必須商品のみで計算する
				var hasSubscriptionBoxNecessary = (subscriptionBox.IsNumberTime)
					? subscriptionBox.DefaultOrderProducts
						.Where(defaultItem => defaultItem.Count == 1)
						.Any(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
					: subscriptionBox.DefaultOrderProducts
						.Where(defaultItem => (defaultItem.TermSince <= DateTime.Now.Date)
								&& (DateTime.Now.Date <= defaultItem.TermUntil))
						.Any(p => (p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID));
				if (hasSubscriptionBoxNecessary
					&& (subscriptionBox.CanFirstSelectable == false))
				{
					takeOverCompare = DateTime.Now.Date;
				}
				else
				{
					var nextProducts = new List<FixedPurchaseItemModel>();
					foreach (var item in this.Items)
					{
						var selectableProduct = subscriptionBox.SelectableProducts
							.First(sp => (sp.ProductId == item.ProductId)
								&& (sp.VariationId == item.VariationId));
						// 選択可能期間か判定
						var since = selectableProduct.SelectableSince ?? datetimeCompare;
						var until = selectableProduct.SelectableUntil ?? datetimeCompare;
						if ((datetimeCompare < since) || (datetimeCompare > until)) continue;

						nextProducts.Add(
							new FixedPurchaseItemModel()
							{
								ProductId = item.ProductId,
								VariationId = item.VariationId,
								SupplierId = item.SupplierId,
								ItemQuantity = item.Count,
								ItemQuantitySingle = item.CountSingle
							});
					}

					if (nextProducts.Any() == false) return new SubscriptionBoxGetNextProductsResult();
					return new SubscriptionBoxGetNextProductsResult
					{
						NextProducts = nextProducts.ToArray(),
						Result = SubscriptionBoxGetNextProductsResult.ResultTypes.Success,
						NextCount = displayCount + 1,
					};
				}
			}

			// 引き継ぎ対象の商品を取得する
			var getNextProductsResult = new SubscriptionBoxService().GetFixedPurchaseNextProduct(
				this.SubscriptionBoxCourseId,
				string.Empty,
				this.MemberRankId,
				takeOverCompare,
				beforeSubscriptionCount,
				null);
			return getNextProductsResult;
		}

		/// <summary>
		/// 配送料金再計算
		/// </summary>
		/// <param name="blSetDefaultShipping">デフォルト配送先利用フラグ</param>
		/// <remarks>配送料金が変わったか</remarks>
		private bool CalculateShippingPrice(bool blSetDefaultShipping)
		{
			var priceBefore = this.PriceShipping;

			var prefectures = Constants.TW_COUNTRY_SHIPPING_ENABLE
				? new List<string>(Constants.TW_CITIES_LIST)
				: new List<string>(Constants.STR_PREFECTURES_LIST);

			ShopShippingModel shopShipping = null;
			foreach (CartShipping cartShipping in this.Shippings)
			{
				var address = string.Empty;
				var zip = string.Empty;
				if (blSetDefaultShipping)
				{
					address = Constants.TW_COUNTRY_SHIPPING_ENABLE
						? Constants.CONST_DEFAULT_SHIPPING_ADDRESS2_TW
						: Constants.CONST_DEFAULT_SHIPPING_ADDR1;
				}
				else
				{
					address = cartShipping.IsTaiwanCountryShippingEnable
						? cartShipping.Addr2
						: cartShipping.Addr1;
					zip = cartShipping.IsShippingAddrJp
						? cartShipping.Zip1 + cartShipping.Zip2
						: cartShipping.Zip;
				}

				// 配送サービスが未確定の場合、配送サービスを確定
				if (string.IsNullOrEmpty(cartShipping.DeliveryCompanyId))
				{
					if (shopShipping == null) shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(this.ShippingType);
					cartShipping.CartShippingShippingMethodUserUnSelected(shopShipping);
				}

				var shippingInfo = GetShippingInfoFromDB(
					prefectures.IndexOf(address) + 1,
					zip,
					cartShipping.DeliveryCompanyId,
					prefectures.Count);
				if (shippingInfo != null)
				{
					cartShipping.SetShippingPrice(shippingInfo);
				}
				else if (cartShipping.UseGlobalShippingPrice)
				{
					// 海外用にshippingInfo特定
					shippingInfo = DataCacheControllerFacade.GetShopShippingCacheController().Get(this.ShippingType);

					// 海外送料を使うと判断した場合
					if (shippingInfo != null) cartShipping.SetShippingPrice(shippingInfo);
				}
			}
			var priceAfter = this.PriceShipping;
			return (priceBefore != priceAfter);
		}

		/// <summary>
		/// 配送料未設定エラーが出ている場合、エラーページへ移動
		/// </summary>
		public void CheckGlobalShippingPriceCalcError()
		{
			foreach (var cartShipping in this.Shippings)
			{
				// 海外送料で、配送料未設定エラーが出ている場合、エラーページへ移動
				if (cartShipping.UseGlobalShippingPrice && cartShipping.IsGlobalShippingPriceCalcError)
				{
					cartShipping.TransitionToErrorScreenByGlobalShippingPriceCalcError();
				}
			}
		}

		/// <summary>
		/// 割引額初期化
		/// </summary>
		public void InitDiscountPrice()
		{
			this.Items.ForEach(item => item.ItemPriceRegulation = 0m);
			this.ShippingPriceDiscountAmount = 0m;
			this.PaymentPriceDiscountAmount = 0m;
		}

		/// <summary>
		/// 会員ランク更新（CrossPointから取得）
		/// </summary>
		public void UpdateMemberRankByCrossPoint()
		{
			if (string.IsNullOrEmpty(this.CartUserId)) return;

			// なるべく最新を取得するため、セッションからではなくAPIを叩く
			var crossPointUser = new CrossPointUserApiService().Get(this.CartUserId);
			if (crossPointUser == null) return;
			this.MemberRankId = crossPointUser.MemberRankId;
		}

		#region セットプロモーション計算系
		/// <summary>
		/// セットプロモーション計算
		/// </summary>
		/// <param name="isCartItemChanged">カート商品変更有無</param>
		/// <param name="isShippingChanged">配送先変更有無</param>
		/// <param name="isPaymentChanged">決済変更有無</param>
		/// <param name="isManagerModify">管理画面での注文編集時か</param>
		private void CalculateSetPromotion(bool isCartItemChanged, bool isShippingChanged, bool isPaymentChanged, bool isManagerModify = false)
		{
			if (Constants.SETPROMOTION_OPTION_ENABLED
				&& (isCartItemChanged
					|| isShippingChanged
					|| isPaymentChanged
					|| (this.SetPromotions.Items.Count == 0)
					|| IsFreeShippingCouponUse()
					|| IsFreeShippingFlgCouponUse()))
			{
				// セットプロモーション情報クリア
				ClearCartSetPromotion();

				// 適用優先順OPがONなら適用優先順,OFFなら最安順
				var cartSetPromotionList = Constants.SETPROMOTION_APPLY_ORDER_OPTION_ENABLED
					? CartSetPromotionCalculator.GetSetPromotionsByPriority(this)
					: CartSetPromotionCalculator.GetLowestCartSetPromotionList(this, isManagerModify);

				if (this.IsOrderModify
					&& (this.SetPromotionsOld != null)
					&& this.SetPromotionsOld.Any()
					&& ((cartSetPromotionList == null)
						|| (cartSetPromotionList.Items.Any() == false)))
				{
					cartSetPromotionList = CartSetPromotionCalculator.GetLowestCartSetPromotionList(this, isManagerModify, this.SetPromotionsOld);
				}

				// カートにセット
				if (cartSetPromotionList != null)
				{
					this.SetPromotions = cartSetPromotionList;

					// 商品紐づけ
					foreach (CartSetPromotion setPromotion in this.SetPromotions)
					{
						// 複数個の同じ商品に同一のセットプロモーションが紐づけされるときに、適用される数だけセットプロモーションを紐づけたい。
						// そのため同じ商品に同一セットプロモーションが適用される場合は、適用するセットプロモーションの数だけ追加する処理を行っている。
						var sameSetPromotionCount = new Dictionary<int, int>();
						foreach (CartSetPromotion.Item item in setPromotion.tempItems)
						{
							if (item.Product.QuantityAllocatedToSet.ContainsKey(setPromotion.CartSetPromotionNo) == false)
							{
								item.Product.QuantityAllocatedToSet.Add(setPromotion.CartSetPromotionNo, item.Quantity * setPromotion.SetCount);
								if (sameSetPromotionCount.ContainsKey(setPromotion.CartSetPromotionNo) == false)
								{
									sameSetPromotionCount.Add(setPromotion.CartSetPromotionNo, item.Quantity);
								}
							}
							else
							{
								sameSetPromotionCount[setPromotion.CartSetPromotionNo] += item.Quantity;
								item.Product.QuantityAllocatedToSet[setPromotion.CartSetPromotionNo] = sameSetPromotionCount[setPromotion.CartSetPromotionNo];
							}
						}
					}
				}
			}

			if (this.IsAllItemsSubscriptionBoxFixedAmount == false) CalculateSetPromotionProductDiscountPrice();

			CalculateSetPromotionShippingaAndPaymentDiscountPrice();
		}
		#endregion

		/// <summary>
		/// セットプロモーション商品割引額計算
		/// </summary>
		private void CalculateSetPromotionProductDiscountPrice()
		{
			var productsAllocatedToSetAndShipping = new List<Hashtable>();
			var fixedAmountItemsExcluded = this.Items
				.Where(item => item.IsSubscriptionBoxFixedAmount() == false)
				.ToArray();

			// 初めにセットプロモーション割引情報をクリア(再計算前の情報が残っている場合があるため)
			if (this.IsSecondFixedPurchase == false)
			{
				foreach (var cp in fixedAmountItemsExcluded)
				{
					cp.DiscountedPrice.Clear();
				}
			}

			// 商品・配送先・セットプロモーションごとに商品個数を持つハッシュを作成
			if (this.HasUnAllocatedProductToShipping)
			{
				// 通常/セット商品登録
				productsAllocatedToSetAndShipping.AddRange(
					fixedAmountItemsExcluded
						.Where(cp => cp.QuantitiyUnallocatedToSet != 0)
						.Select(
							product => new Hashtable
							{
								{ "product", product },
								{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, 0 },
								{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "" },
								{
									Constants.FIELD_ORDERITEM_ITEM_QUANTITY,
									product.IsSetItem ? product.Count : product.QuantitiyUnallocatedToSet
								},
							}
						));

				// セットプロモーション商品登録
				productsAllocatedToSetAndShipping.AddRange(this.SetPromotions
					.Cast<CartSetPromotion>()
					.SelectMany(setPromotion => setPromotion.Items.Select(product => new Hashtable()
						{
							{"product", product},
							{Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, 0},
							{Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, setPromotion.CartSetPromotionNo.ToString()},
							{Constants.FIELD_ORDERITEM_ITEM_QUANTITY, product.QuantityAllocatedToSet[setPromotion.CartSetPromotionNo]}
						}
					)));

				// セットプロモーション非適用商品の金額を格納
				if (this.IsSecondFixedPurchase == false)
				{
					foreach (var ht in productsAllocatedToSetAndShipping)
					{
						((CartProduct)ht["product"]).DiscountedPriceUnAllocatedToSet
							= ((CartProduct)ht["product"]).Price * ((CartProduct)ht["product"]).QuantitiyUnallocatedToSet;
					}
				}
			}
			else
			{
				List<Hashtable> products = new List<Hashtable>();

				// 配送先商品をばらす
				products.AddRange(Enumerable.Range(0, this.Shippings.Count)
					.SelectMany(index =>
					{
						var productList = this.Shippings[index].ProductCounts
							.SelectMany(productCount => Enumerable.Range(0, productCount.Count).Select(i => new Hashtable()
							{
									{ "product", productCount.Product },
									{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, index }
							}));
						return productList;
					}));

				productsAllocatedToSetAndShipping.AddRange(
					fixedAmountItemsExcluded
						.Where(cartProduct => products.Any(cp => (CartProduct)cp["product"] == cartProduct))
						.SelectMany(
							cartProduct =>
							{
								// 対象商品を抽出
								var targetProducts = products.FindAll(cp => (CartProduct)cp["product"] == cartProduct);
								var targetProductsWithSetAndShipping = new List<Hashtable>();

								// セットプロモーション情報を追加
								targetProductsWithSetAndShipping.AddRange(
									Enumerable.Range(0, cartProduct.QuantitiyUnallocatedToSet)
										.Select(
											index =>
											{
												targetProducts[index].Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "");
												return targetProducts[index];
											}));

								var i = cartProduct.QuantitiyUnallocatedToSet;
								foreach (var setpromotionitem in cartProduct.QuantityAllocatedToSet)
								{
									targetProductsWithSetAndShipping.AddRange(Enumerable.Range(i, setpromotionitem.Value)
										.Select(index =>
											{
												targetProducts[index].Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, setpromotionitem.Key.ToString());
												return targetProducts[index];
											}));
									i += setpromotionitem.Value;
								}

								// 配送先、セットプロモーションでグループ化
								var groupedTargetProduct = targetProductsWithSetAndShipping.GroupBy(p => p[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO] + "," + p[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]);
								return groupedTargetProduct
									.Select(product => new Hashtable()
										{
											{"product", cartProduct},
											{Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, int.Parse(product.Key.Split(',')[0])},
											{Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, product.Key.Split(',')[1]},
											{Constants.FIELD_ORDERITEM_ITEM_QUANTITY, product.ToList().Count}
									});
							}));
			}

			// 商品金額割引の場合は商品按分価格を設定
			foreach (CartSetPromotion cartSetPromotion in this.SetPromotions.Items.Where(sp => sp.IsDiscountTypeProductDiscount))
			{
				var stackedDiscountAmount = 0m;
				var targetProductCounts = new List<CartShipping.ProductCount>();
				foreach (Hashtable ht in productsAllocatedToSetAndShipping.Where(
					ht => (string)ht[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]
						== cartSetPromotion.CartSetPromotionNo.ToString()))
				{
					var discountPrice = CartSetPromotionCalculator.GetDistributedDiscountAmount(
						cartSetPromotion,
						(CartProduct)ht["product"],
						(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						this.Shippings[0].IsDutyFree);
					((CartProduct)ht["product"]).PriceSubtotalAfterDistribution -= discountPrice;
					if (this.IsSecondFixedPurchase == false)
					{
						((CartProduct)ht["product"]).DiscountedPrice[cartSetPromotion.CartSetPromotionNo]
							= (((CartProduct)ht["product"]).Price * ((CartProduct)ht["product"]).QuantityAllocatedToSet[cartSetPromotion.CartSetPromotionNo]) - discountPrice;
					}
					if (this.HasUnAllocatedProductToShipping == false)
					{
						var targetProduct = this.Shippings[(int)ht[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO]]
							.ProductCounts.Find(pc => pc.Product == (CartProduct)ht["product"]);
						targetProduct.PriceSubtotalAfterDistribution -= discountPrice;
						targetProductCounts.Add(targetProduct);
					}
					// キャンペーン（ノベルティ、クーポンなど）適用比較対象金額用に商品小計セット
					((CartProduct)ht["product"]).PriceSubtotalAfterDistributionForCampaign =
						((CartProduct)ht["product"]).PriceSubtotalAfterDistribution;
					stackedDiscountAmount += discountPrice;
				}

				var fractionDiscountPrice = cartSetPromotion.ProductDiscountAmount - stackedDiscountAmount;
				if (this.HasUnAllocatedProductToShipping == false)
				{
					var weightProduct = targetProductCounts
						.OrderByDescending(pc => pc.PriceSubtotalAfterDistribution).First();
					weightProduct.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
					weightProduct.Product.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
					if (this.IsSecondFixedPurchase == false) weightProduct.Product.DiscountedPrice[cartSetPromotion.CartSetPromotionNo] -= fractionDiscountPrice;
					weightProduct.Product.PriceSubtotalAfterDistributionForCampaign -= fractionDiscountPrice;
				}
				else
				{
					var weightItem = cartSetPromotion.Items
						.OrderByDescending(item => item.PriceSubtotalAfterDistribution)
						.First();
					weightItem.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
					if (this.IsSecondFixedPurchase == false) weightItem.DiscountedPrice[cartSetPromotion.CartSetPromotionNo] -= fractionDiscountPrice;
					weightItem.PriceSubtotalAfterDistributionForCampaign -= fractionDiscountPrice;
				}
			}

			// 商品金額割引以外の場合は商品価格をそのまま設定
			foreach (var cartSetPromotion in this.SetPromotions.Items.Where(sp => (sp.IsDiscountTypeProductDiscount == false)))
			{
				foreach (var ht in productsAllocatedToSetAndShipping.Where(
					ht => (string)ht[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]
						== cartSetPromotion.CartSetPromotionNo.ToString()))
				{
					if (this.IsSecondFixedPurchase == false)
					{
						((CartProduct)ht["product"]).DiscountedPrice[cartSetPromotion.CartSetPromotionNo]
							= (((CartProduct)ht["product"]).Price
								* ((CartProduct)ht["product"]).QuantityAllocatedToSet[cartSetPromotion.CartSetPromotionNo]);
					}
				}
			}
		}

		/// <summary>
		/// セットプロモーション配送料・決済手数料割引額計算
		/// </summary>
		private void CalculateSetPromotionShippingaAndPaymentDiscountPrice()
		{
			this.PaymentPriceDiscountAmount = this.SetPromotions.IsPaymentChargeFree
				? this.PaymentPriceForCalculationDiscountAndTax
				: 0m;
			this.ShippingPriceDiscountAmount = (this.SetPromotions.IsShippingChargeFree
				|| IsFreeShippingCouponUse()
				|| IsFreeShippingFlgCouponUse())
				? this.ShippingPriceForCalculationDiscountAndTax
				: 0m;
		}

		/// <summary>
		/// 会員ランク割引額計算
		/// </summary>
		private void CalculateMemberRankDiscountPrice()
		{
			//------------------------------------------------------
			// 会員ランク割引額計算
			//------------------------------------------------------
			this.MemberRankDiscount = 0;

			if (Constants.MEMBER_RANK_OPTION_ENABLED
				&& ((string.IsNullOrEmpty(this.CartUserId) == false)
					|| (string.IsNullOrEmpty(this.MemberRankId) == false)))
			{
				//------------------------------------------------------
				// 会員ランク割引対象金額取得
				//------------------------------------------------------
				decimal dSubtotalForMemberRankDiscount = 0;

				// 頒布会定額コース商品以外で計算
				var targetProducts = this.Items
					.Where(product => product.IsMemberRankDiscount && (product.IsSubscriptionBoxFixedAmount() == false))
					.ToList();
				var fixedAmountItemsExcluded = targetProducts.Sum(product => product.PriceSubtotalAfterDistribution);
				dSubtotalForMemberRankDiscount += fixedAmountItemsExcluded;

				if (this.HasSubscriptionBoxFixedAmountItem)
				{
					// 頒布会定額コース商品のみで計算（頒布会定額コースの場合は加算する）
					var targetItemsFixedAmount = this.Items
						.Where(
							product => product.IsMemberRankDiscount
								&& product.IsSubscriptionBoxFixedAmount()
								&& this.Items
									.Where(item => item.SubscriptionBoxCourseId == product.SubscriptionBoxCourseId)
									.All(item => item.IsMemberRankDiscount))
						.ToList();
					var fixedAmountTotal = targetItemsFixedAmount
						.Distinct(item => item.SubscriptionBoxCourseId)
						.Sum(item => item.SubscriptionBoxFixedAmount.Value);
					dSubtotalForMemberRankDiscount += fixedAmountTotal;
					targetProducts.AddRange(targetItemsFixedAmount);
				}

				//------------------------------------------------------
				// 会員ランク割引額取得
				//------------------------------------------------------
				string strRankId = (string.IsNullOrEmpty(this.MemberRankId) && (this.IsMemberRankIdFromDb == false))
					? MemberRankOptionUtility.GetMemberRankId(this.CartUserId)
					: this.MemberRankId;
				this.MemberRankDiscount = MemberRankOptionUtility.GetDiscountPrice(strRankId, dSubtotalForMemberRankDiscount);
				ProrateDiscountPrice(targetProducts, this.MemberRankDiscount, true);
				ProrateDiscountPriceToProduct(targetProducts, this.MemberRankDiscount);
			}
		}

		/// <summary>
		/// クーポン系計算
		/// </summary>
		private void CalculateCouponFamily()
		{
			this.UseCouponPrice = 0;
			this.UseMaxCouponPrice = 0;

			// クーポン情報が設定されていない場合
			if ((!Constants.W2MP_COUPON_OPTION_ENABLED) || (this.Coupon == null)) return;

			//------------------------------------------------------
			// 商品合計(クーポン適用対象分)取得
			//------------------------------------------------------
			var coupon = DomainFacade.Instance.CouponService.GetAllUserCouponsFromCouponId(
				this.Coupon.DeptId,
				this.CouponUserId,
				this.Coupon.CouponId,
				this.Coupon.CouponNo);

			var couponApplyCartProduct = (coupon.Length > 0)
				? this.Items
					.Where(item => (item.IsSubscriptionBoxFixedAmount() == false)
						&& CouponOptionUtility.IsCouponApplyCartProduct(coupon[0], item))
					.ToList()
				: new List<CartProduct>();

			var priceSubTotalCoupon = couponApplyCartProduct
				.Sum(item => item.PriceSubtotalAfterDistribution);

			var subscriptionBoxFixedItems = this.Items
				.Where(item => item.IsSubscriptionBoxFixedAmount())
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Where(items => items.All(item => CouponOptionUtility.IsCouponApplyCartProduct(coupon[0], item)));

			this.IsCouponNotApplicableByOrderCombined = (this.Items.Count(item => item.IsSubscriptionBoxFixedAmount()) > 0)
				&& subscriptionBoxFixedItems.Any() == false
				&& this.IsOrderCombined;

			priceSubTotalCoupon += subscriptionBoxFixedItems
				.Sum(items => items.FirstOrDefault()?.SubscriptionBoxFixedAmount ?? 0m);

			//------------------------------------------------------
			// クーポン割引額取得
			//------------------------------------------------------
			switch (this.Coupon.DiscountKbn)
			{
				case CartCoupon.CouponDiscountKbn.Price:
					this.UseMaxCouponPrice = this.Coupon.DiscountPrice;
					this.UseCouponPrice = (priceSubTotalCoupon > this.UseMaxCouponPrice) ? this.UseMaxCouponPrice : priceSubTotalCoupon;
					break;

				case CartCoupon.CouponDiscountKbn.Rate:
					// クーポン割引率からクーポン割引額取得
					// 計算方法：商品小計(クーポン適用対象分) * クーポン割引率 
					this.UseMaxCouponPrice = RoundingCalculationUtility.GetRoundPercentDiscountFraction(priceSubTotalCoupon, this.Coupon.DiscountRate);
					this.UseCouponPrice = (priceSubTotalCoupon > this.UseMaxCouponPrice) ? this.UseMaxCouponPrice : priceSubTotalCoupon;
					break;

				case CartCoupon.CouponDiscountKbn.FreeShipping:
					this.UseMaxCouponPrice = this.PriceShipping;
					this.UseCouponPrice = this.ShippingPriceForCalculationDiscountAndTax;
					this.ShippingPriceDiscountAmount = this.ShippingPriceForCalculationDiscountAndTax;
					break;
			}
			if (IsFreeShippingFlgCouponUse())
			{
				this.UseMaxCouponPrice = this.UseMaxCouponPrice + this.PriceShipping;
				this.UseCouponPrice = this.UseCouponPrice + this.ShippingPriceForCalculationDiscountAndTax;
				this.ShippingPriceDiscountAmount = this.ShippingPriceForCalculationDiscountAndTax;
			}
			ProrateDiscountPrice(couponApplyCartProduct, this.UseCouponPriceForProduct, false);
			ProrateDiscountPriceToProduct(couponApplyCartProduct, this.UseCouponPriceForProduct);
		}

		/// <summary>
		/// 商品ごとのポイント割引後の金額を計算
		/// </summary>
		private void CalculatePointDisCountProductPrice()
		{
			if (Constants.W2MP_POINT_OPTION_ENABLED == false) return;

			ProrateDiscountPrice(this.Items, this.UsePointPrice, false);
			ProrateDiscountPriceToProduct(this.Items, this.UsePointPrice);
		}

		/// <summary>
		/// ポイント系計算
		/// </summary>
		public void CalculatePointFamily()
		{
			// ポイントOPが無効、購入商品がない、またはポイント付与不可の注文の場合、ポイント計算しない
			if ((Constants.W2MP_POINT_OPTION_ENABLED == false)
				|| (this.IsOrderGrantablePoint == false)
				|| (this.Items.Count == 0))
			{
				return;
			}

			// ポイントルール毎の購入時獲得ポイント
			this.BuyPoints = new Dictionary<string, decimal>();
			this.PointKbns = new List<Hashtable>(); 
			foreach (var rule in PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_BUY))
			{
				string dmy;
				this.BuyPoints[rule.PointRuleId] = PointOptionUtility.GetOrderPointAdd(
					this,
					Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
					out dmy,
					this.FixedPurchase,
					rule);
				
				var pointKbn = new Hashtable 
				{ 
					{ Constants.FIELD_USERPOINT_POINT_KBN, rule.PointKbn }, 
					{ Constants.FIELD_USERPOINT_POINT_INC_KBN, Constants.FLG_POINTRULE_POINT_INC_KBN_BUY }, 
					{ Constants.FIELD_USERPOINT_POINT_RULE_ID, rule.PointRuleId }, 
					{ Constants.FIELD_USERPOINT_POINT_RULE_KBN, rule.PointRuleKbn },
				}; 
				this.PointKbns.Add(pointKbn);
			}
			// 購入時獲得ポイント
			this.BuyPoint = this.BuyPoints.Values.Sum();

			// 初回購入判断
			var firstBuy = ((this.CartUserId == "") || DomainFacade.Instance.OrderService.CheckOrderFirstBuy(this.CartUserId, this.OrderId));
			if (string.IsNullOrEmpty(this.OrderCombineParentOrderId) == false)
			{
				var orderIdList = new List<string>(this.OrderCombineParentOrderId.Split(',')).ToArray();
				firstBuy = PointOptionUtility.CheckOrderFirstBuyForOrderCombine(this.CartUserId, orderIdList);
			}

			this.FirstBuyPoints = new Dictionary<string, decimal>();
			if (firstBuy)
			{
				SetFirstBuyPoint();
			}
			else
			{
				this.FirstBuyPoint = 0;
			}
		}

		/// <summary>
		/// 初回購入時ポイントをカートにセット
		/// </summary>
		public void SetFirstBuyPoint()
		{
			string strFirstBuyPointKbnTmp;
			// ポイントルール毎の初回購入時ポイント
			foreach (var rule in PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY))
			{
				this.FirstBuyPoints[rule.PointRuleId] = PointOptionUtility.GetOrderPointAdd(
					this,
					Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY,
					out strFirstBuyPointKbnTmp,
					this.FixedPurchase,
					rule);
			}

			// 初回購入時ポイント取得
			this.FirstBuyPoint = PointOptionUtility.GetOrderPointAdd(
				this,
				Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY,
				out strFirstBuyPointKbnTmp,
				this.FixedPurchase);

			this.FirstBuyPoint = this.FirstBuyPoints.Values.Sum();
			this.FirstBuyPointKbn = strFirstBuyPointKbnTmp;
		}

		/// <summary>
		/// 定期購入割引計算
		/// </summary>
		/// <param name="currentFixedPurchaseOrderCount">現時点ての定期購入回数</param>
		/// <param name="isFixedPurchaseOrderPrice">定期金額表示からの呼び出しか</param>
		private void CalculateFixedPurchaseDiscount(int currentFixedPurchaseOrderCount, bool isFixedPurchaseOrderPrice = false)
		{
			if (string.IsNullOrEmpty(this.OrderCombineParentOrderId) == false)
			{
				// 注文同梱の場合親注文の購入回数、通常注文の場合定期台帳の回数を参照する
				if (this.IsCombineParentOrderHasFixedPurchase || this.IsBeforeCombineCartHasFixedPurchase)
				{
					// フロントに注文同梱を行う場合は、1回目のみ定期購入割引額が注文商品の元注文の割引額により計算される
					// 2回目以降の次回定期購入は定期購入回数により最新の定期購入割引マスタを取得して再計算される
					CalculateFixedPurchaseDiscountPrice(
						(this.CombineParentOrderFixedPurchaseOrderCount + currentFixedPurchaseOrderCount),
						(currentFixedPurchaseOrderCount < 1));
				}
				// 親注文と子注文が定期でない場合定期割引を適用しない
				else
				{
					this.FixedPurchaseDiscount = 0;
				}
			}
			else if (this.FixedPurchase != null)
			{
				// 注文回数はこの時点では前回分の回数のため今回分の回数とするため+1する
				CalculateFixedPurchaseDiscountPrice(this.FixedPurchase.OrderCount + 1);
			}
			else if (this.HasFixedPurchase)
			{
				// 定期購入台帳情報がなく、定期購入がある場合初回の割引設定を確認する
				CalculateFixedPurchaseDiscountPrice(
					1 + currentFixedPurchaseOrderCount,
					isFixedPurchaseOrderPrice: isFixedPurchaseOrderPrice);
			}
			else
			{
				// 定期商品がない場合には定期購入割引額を0にする
				this.FixedPurchaseDiscount = 0;
			}
		}

		/// <summary>
		/// 定期購入割引価格計算(注文同梱後1回目の定期購入の場合は割引価格を引き継ぐ)
		/// </summary>
		/// <param name="fixedPurchaseOrderCount">定期購入回数</param>
		/// <param name="isCombine">注文同梱か</param>
		/// <param name="isFixedPurchaseOrderPrice">定期金額表示からの呼び出しか</param>
		private void CalculateFixedPurchaseDiscountPrice(
			int fixedPurchaseOrderCount,
			bool isCombine = false,
			bool isFixedPurchaseOrderPrice = false)
		{
			// 同梱注文でない場合、定期購入割引額を最新の商品定期購入割引設定マスタを取得して再計算する
			if (isCombine == false)
			{
				var calculatedFixedPurchaseDiscountTotal = 0m;

				if ((fixedPurchaseOrderCount > 1)
					&& this.IsSubscriptionBox
					&& isFixedPurchaseOrderPrice)
				{
					var updateSubscriptionBoxResult = GetSubscriptionBoxNextProducts(fixedPurchaseOrderCount);

					foreach (var item in updateSubscriptionBoxResult.NextProducts)
					{
						var product = ProductCommon.GetProductVariationInfo(
							this.ShopId,
							item.ProductId,
							item.VariationId,
							this.MemberRankId);

						var cartProduct = SetFixedPurhcasePriceForCartProduct(
							new CartProduct(
								product[0],
								Constants.AddCartKbn.SubscriptionBox,
								"",
								item.ItemQuantity,
								true,
								new ProductOptionSettingList(),
								string.Empty,
								null,
								this.SubscriptionBoxCourseId));

						// 割引額の按分は実際の税込価格に対して行う
						var priceSubtotalPreTax = this.HasUnAllocatedProductToShipping
							? this.Shippings[0].IsDutyFree ? cartProduct.PriceSubtotal : cartProduct.PriceSubtotalPretax
							: this.Shippings.SelectMany(shipping => shipping.ProductCounts)
								.Where(pc => (pc.Product == cartProduct)).Sum(pc => pc.PriceSubtotalAfterDistribution);
						// 商品小計(按分適用後)の初期化
						cartProduct.PriceSubtotalAfterDistribution = priceSubtotalPreTax;

						calculatedFixedPurchaseDiscountTotal +=
							CalculateFixedPurchaseDiscountByDiscountSetting(fixedPurchaseOrderCount, cartProduct);
					}
				}
				else
				{
					var targetProducts = this.Items.Where((p => p.IsFixedPurchase)).ToList();
					//商品IDごと商品数集計
					var productCount = targetProducts.GroupBy(cp => cp.ProductId)
						.Select(cp => new { ProductId = cp.Key, Count = cp.Sum(p => p.Count) })
						.ToDictionary(cp => cp.ProductId, cp => cp.Count);

					foreach (var product in targetProducts)
					{
						calculatedFixedPurchaseDiscountTotal += 
							CalculateFixedPurchaseDiscountByDiscountSetting(fixedPurchaseOrderCount, product, productCount[product.ProductId]);
					}
				}

				this.FixedPurchaseDiscount = calculatedFixedPurchaseDiscountTotal;
			}
			// 同梱注文の場合、子注文と親注文の定期購入割引価格の合計を各商品に対して按分処理を実行する
			else
			{
				// 定期購入設定情報が指定されている商品を対象とする
				var targetProducts = this.Items.Where(product => product.IsFixedPurchaseDiscountItem).ToList();

				// 対象商品の定期購入設定情報から商品個々の割引額を計算する
				var calculatedFixedPurchaseDiscountTotal = 0m;
				foreach (var product in targetProducts)
				{
					calculatedFixedPurchaseDiscountTotal +=
						CalculateFixedPurchaseDiscountByDiscountSetting(
							product,
							product.FixedPurchaseDiscountType,
							product.FixedPurchaseDiscountValue);
				}

				// 親子注文の定期購入割引の合計額と、計算した商品個々の割引額の合計に差異があった場合は按分を行う。
				var differenceFromCombinedDiscountPrice =
					this.FixedPurchaseDiscount - calculatedFixedPurchaseDiscountTotal;
				ProrateDiscountPrice(targetProducts, differenceFromCombinedDiscountPrice, true);
				ProrateDiscountPriceToProduct(targetProducts, differenceFromCombinedDiscountPrice);
			}
		}

		#region CalculateFixedPurchaseDiscountPriceBySetting 定期購入割引設定より商品毎の定期購入割引額を計算する
		/// <summary>
		/// 定期購入割引設定より商品毎の定期購入割引額を計算する(最新の商品定期購入割引設定を参照する)
		/// </summary>
		/// <param name="fixedPurchaseOrderCount">定期購入回数</param>
		/// <param name="product">カート商品</param>
		/// <param name="productOrderCount">商品数</param>
		/// <returns>割引額</returns>
		private decimal CalculateFixedPurchaseDiscountByDiscountSetting(
			int fixedPurchaseOrderCount,
			CartProduct product,
			int productOrderCount = 0)
		{
			// 定期購入割引設定取得
			var discountSetting = DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService.GetApplyFixedPurchaseDiscountSetting(
				this.ShopId,
				product.ProductId,
				(Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD == Constants.FLG_FIXEDPURCHASE_COUNT)
					? fixedPurchaseOrderCount
					: GetFixedPurchaseItemOrderCount(product),
					productOrderCount == 0 ? product.Count : productOrderCount);

			if (discountSetting == null) return 0;

			product.FixedPurchaseDiscountType = discountSetting.DiscountType;
			product.FixedPurchaseDiscountValue = discountSetting.DiscountValue;

			var fixedPurchaseDiscountPrice = CalculateFixedPurchaseDiscountByDiscountSetting(
				product,
				discountSetting.DiscountType,
				discountSetting.DiscountValue);

			return fixedPurchaseDiscountPrice;
		}
		/// <summary>
		/// 定期購入割引設定値より商品毎の定期購入割引額を計算する
		/// </summary>
		/// <param name="product">カート商品</param>
		/// <param name="fixedPurchaseDiscountType">定期購入割引種別</param>
		/// <param name="fixedPurchaseDiscountValue">定期購入割引値引き値</param>
		/// <returns>割引額</returns>
		private decimal CalculateFixedPurchaseDiscountByDiscountSetting(
			CartProduct product,
			string fixedPurchaseDiscountType,
			decimal? fixedPurchaseDiscountValue)
		{
			var fixedPurchaseDiscountPrice = PriceCalculator.GetFixedPurchaseDiscountPrice(
				fixedPurchaseDiscountType,
				fixedPurchaseDiscountValue,
				product.PriceSubtotalAfterDistribution,
				product.Count);

			ProrateDiscountPrice(
				new List<CartProduct> { product },
				fixedPurchaseDiscountPrice,
				true);
			ProrateDiscountPriceToProduct(
				new List<CartProduct> { product },
				fixedPurchaseDiscountPrice);

			return fixedPurchaseDiscountPrice;
		}
		#endregion

		/// <summary>
		/// 定期商品購入回数の取得
		/// </summary>
		/// <param name="product">カート内商品</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>定期商品購入回数</returns>
		public int GetFixedPurchaseItemOrderCount(CartProduct product, SqlAccessor accessor = null)
		{
			if (Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD == Constants.FLG_FIXEDPURCHASE_COUNT) return 0;

			// 注文同梱 注文同梱元注文の商品購入回数（注文時点）を引き継ぐ
			if (string.IsNullOrEmpty(this.OrderCombineParentOrderId) == false)
			{
				// 最も大きい回数取得
				var fixedPurchaseOrderItemCount = this.OrderCombineParentOrderId.Split(',')
					.Max(combineOrderId => new OrderService().GetFixedPurchaseItemOrderCount(
						combineOrderId,
						product.ProductId,
						product.VariationId,
						accessor));
				// 2回目以降金額表示時なら、商品購入数と商品表示回数を加算
				return this.IsSecondFixedPurchase
					? (fixedPurchaseOrderItemCount + this.FixedPurchaseDisplayCount - 1)
					: fixedPurchaseOrderItemCount;
			}

			//受注情報編集画面かFrontでの注文編集時
			if (this.IsOrderModifyInput || this.IsOrderModify)
			{
				var fixedPurchaseOrderItemCount = string.IsNullOrEmpty(product.FixedPurchaseItemOrderCountInput)
					? 0
					: int.Parse(product.FixedPurchaseItemOrderCountInput);
				return fixedPurchaseOrderItemCount;
			}

			// 定期台帳 定期台帳の購入回数(注文基準)を+1する
			if ((this.FixedPurchase != null) && this.FixedPurchase.Shippings.Any())
			{
				var fixedPurchaseOrderItem = this.FixedPurchase.Shippings[0].Items
					.FirstOrDefault(item => ((item.ProductId == product.ProductId)
						&& (item.VariationId == product.VariationId)));
				var fixedPurchaseOrderItemCount = (fixedPurchaseOrderItem == null)
					? 1
					: (fixedPurchaseOrderItem.ItemOrderCount + 1);
				return fixedPurchaseOrderItemCount;
			}

			// 2回目以降の定期商品表示から呼び出す場合、そのパーツで設定した値を使う
			if (this.IsSecondFixedPurchase && product.FixedPurchaseItemOrderCount.HasValue)
			{
				return product.FixedPurchaseItemOrderCount.Value;
			}

			return 1;
		}

		#region 定期会員割引率計算系
		/// <summary>
		/// 定期会員割引系計算
		/// </summary>
		private void CalculateFixedPurchaseMemberDiscount()
		{
			if (this.IsApplyFixedPurchaseMemberDiscount == false) return;

			// 定期会員割引率
			var fixedPurchaseMemberDiscountRate = MemberRankOptionUtility.GetFixedPurchaseMemberDiscountRate(this.MemberRankId);
			// 定期会員割引適用対象商品
			var targetDiscountProducts = this.Items.Where(product => product.IsApplyFixedPurchaseMemberDiscount).ToList();
			// 定期会員割引適用対象商品金額合計
			var targetDiscountSubTotal = targetDiscountProducts.Select(cartProduct => cartProduct.PriceSubtotalAfterDistribution).Sum();
			// 定期会員割引額の設定
			this.FixedPurchaseMemberDiscountAmount
				= RoundingCalculationUtility.GetRoundPercentDiscountFraction(targetDiscountSubTotal, fixedPurchaseMemberDiscountRate);

			ProrateDiscountPrice(targetDiscountProducts, this.FixedPurchaseMemberDiscountAmount, true);
			ProrateDiscountPriceToProduct(targetDiscountProducts, this.FixedPurchaseMemberDiscountAmount);
		}
		#endregion

		#region 調整金額を商品・配送料・決済手数料で按分計算
		/// <summary>
		/// 調整金額を商品・配送料・決済手数料で按分計算
		/// </summary>
		private void CalculatePriceRegulation()
		{
			if (this.PriceRegulation == 0) return;

			// 頒布会定額以外の商品
			var productsExcludedSubscriptionBoxFixedAmount = this.Items.Where(item => item.IsSubscriptionBoxFixedAmount() == false).ToArray();
			// 頒布会定額以外の商品合計金額
			var priceTotalExcludedSubscriptionBoxFixedAmount = productsExcludedSubscriptionBoxFixedAmount.Sum(item => item.PriceSubtotalAfterDistribution);
			// 頒布会定額の合計金額
			var priceTotalSubscriptionBoxFixedAmountOnly = this.SubscriptionBoxFixedAmountList.Sum(box => box.PriceSubtotalAfterDistribution);
			// 全ての商品合計金額
			var priceTotal = priceTotalExcludedSubscriptionBoxFixedAmount + priceTotalSubscriptionBoxFixedAmountOnly;

			// 調整金額適用対象金額取得
			var paymentPrice = this.PaymentPriceForCalculationDiscountAndTax - this.PaymentPriceDiscountAmount;
			var paymentRegulationPrice = 0m;
			var shippingPrice = this.ShippingPriceForCalculationDiscountAndTax - this.ShippingPriceDiscountAmount;
			var shippingRegulationPrice = 0m;
			priceTotal += paymentPrice;
			priceTotal += shippingPrice;

			var stackedRegulationPrice = 0m;
			if (priceTotal != 0)
			{
				// 頒布会定額商品以外の商品に商品調整金額を設定
				foreach (var cp in productsExcludedSubscriptionBoxFixedAmount)
				{
					// 計算方法：商品小計 - (商品小計 / 商品合計 * 調整金額)
					// ※端数切捨て
					var regulationPrice = PriceCalculator
						.GetDistributedPrice(
							this.PriceRegulation,
							cp.PriceSubtotalAfterDistribution,
							priceTotal)
						.ToPriceDecimal() ?? 0m;
					if (this.HasUnAllocatedProductToShipping == false)
					{
						var allProductPrice = this.Shippings.Sum(s => s.ProductCounts.Where(pc => pc.Product == cp).Sum(pc => pc.PriceSubtotalAfterDistribution));
						if (allProductPrice > 0)
							this.Shippings.ForEach(
								s =>
								{
									s.ProductCounts
										.Where(pc => pc.Product == cp)
										.ToList()
										.ForEach(pc => pc.ItemPriceRegulation += PriceCalculator
											.GetDistributedPrice(
												regulationPrice,
												pc.PriceSubtotalAfterDistribution,
												allProductPrice));
								});
					}
					stackedRegulationPrice += regulationPrice;
					cp.ItemPriceRegulation = regulationPrice;
				}

				// 頒布会定額に調整金額を設定
				foreach (var subscriptionBox in this.SubscriptionBoxFixedAmountList)
				{
					var regulationPrice = PriceCalculator
						.GetDistributedPrice(
							this.PriceRegulation,
							subscriptionBox.PriceSubtotalAfterDistribution,
							priceTotal)
						.ToPriceDecimal() ?? 0m;

					stackedRegulationPrice += regulationPrice;
					subscriptionBox.ItemPriceRegulation = regulationPrice;
				}

				shippingRegulationPrice += PriceCalculator
					.GetDistributedPrice(
						this.PriceRegulation,
						shippingPrice,
						priceTotal)
					.ToPriceDecimal() ?? 0m;
				stackedRegulationPrice += shippingRegulationPrice;

				paymentRegulationPrice += PriceCalculator
					.GetDistributedPrice(
						this.PriceRegulation,
						paymentPrice,
						priceTotal)
					.ToPriceDecimal() ?? 0m;
				stackedRegulationPrice += paymentRegulationPrice;
			}

			// 実際の調整金額と商品毎の調整金額の合計を合わせるために、最高価格の商品または配送料・決済手数料に端数分を重み付けする
			if (this.HasUnAllocatedProductToShipping == false)
			{
				var weightProduct = this.Shippings
					.SelectMany(shipping => shipping.ProductCounts)
					.OrderByDescending(pc => pc.PriceSubtotalAfterDistribution)
					.First();
				if (weightProduct.PriceSubtotalAfterDistribution != 0)
				{
					weightProduct.ItemPriceRegulation += this.PriceRegulation - stackedRegulationPrice;
					weightProduct.Product.ItemPriceRegulation += this.PriceRegulation - stackedRegulationPrice;
				}
				else if (shippingPrice != 0)
				{
					shippingRegulationPrice += this.PriceRegulation - stackedRegulationPrice;
				}
				else
				{
					paymentRegulationPrice += this.PriceRegulation - stackedRegulationPrice;
				}
			}
			else
			{
				var weightItem = productsExcludedSubscriptionBoxFixedAmount
					.OrderByDescending(item => item.PriceSubtotalAfterDistribution)
					.FirstOrDefault();
				var weightSubscriptionBox = this.SubscriptionBoxFixedAmountList
					.OrderByDescending(box => box.PriceSubtotalAfterDistribution)
					.FirstOrDefault();

				var isSetToProduct = IsAddFractionPriceToProduct(weightItem, weightSubscriptionBox);

				// 頒布会定額以外の商品があり、商品の方が金額が高いかつ金額が0でない
				if (isSetToProduct && (weightItem != null) && (weightItem.PriceSubtotalAfterDistribution != 0))
				{
					weightItem.ItemPriceRegulation += (this.PriceRegulation - stackedRegulationPrice);
				}
				// 頒布会定額があり、頒布会定額の金額の方が高いかつ金額が0でない
				else if ((weightSubscriptionBox != null) && (weightSubscriptionBox.PriceSubtotalAfterDistribution != 0))
				{
					weightSubscriptionBox.ItemPriceRegulation += (this.PriceRegulation - stackedRegulationPrice);
				}
				// 配送料が0でない
				else if (shippingPrice != 0)
				{
					shippingRegulationPrice += this.PriceRegulation - stackedRegulationPrice;
				}
				// 上記以外の場合
				else
				{
					paymentRegulationPrice += this.PriceRegulation - stackedRegulationPrice;
				}
			}
			// 調整金額は符号逆にする
			this.ShippingPriceDiscountAmount -= shippingRegulationPrice;
			this.PaymentPriceDiscountAmount -= paymentRegulationPrice;
		}
		#endregion

		/// <summary>
		/// 商品セットチェック
		/// </summary>
		/// <param name="blOnlyCheck">チェックのみ</param>
		/// <returns>エラーメッセージ</returns>
		public string CheckProductSet(bool blUpdateCart)
		{
			StringBuilder sbErrorMessages = new StringBuilder();

			//------------------------------------------------------
			// セット商品抜き出し
			//------------------------------------------------------
			List<CartProductSet> lCartProductSets = new List<CartProductSet>();
			foreach (CartProduct cp in this.Items)
			{
				if ((cp.IsSetItem) && (lCartProductSets.IndexOf(cp.ProductSet) == -1))
				{
					lCartProductSets.Add(cp.ProductSet);
				}
			}

			//------------------------------------------------------
			// ループしてチェック
			//------------------------------------------------------
			foreach (CartProductSet cpsProductSet in lCartProductSets)
			{
				try
				{
					//------------------------------------------------------
					// 商品削除チェック
					//------------------------------------------------------
					if (this.UpdateCartDb)
					{
						using (SqlAccessor sqlAccessor = new SqlAccessor())
						using (SqlStatement sqlStatement = new SqlStatement("Cart", "GetProductSetItemForDeleteCheck"))
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_CART_CART_ID, this.CartId);
							htInput.Add(Constants.FIELD_CART_PRODUCT_SET_ID, cpsProductSet.ProductSetId);
							htInput.Add(Constants.FIELD_CART_PRODUCT_SET_NO, cpsProductSet.ProductSetNo);

							DataView dvProductsTmp = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
							foreach (DataRowView drv in dvProductsTmp)
							{
								if (drv[Constants.FIELD_PRODUCT_NAME] == DBNull.Value)
								{
									throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCTSET_PRODUCT_UNBUYABLE));
								}
							}
						}
					}

					//------------------------------------------------------
					// セット商品定義変更チェック
					//------------------------------------------------------
					DataView dvProductSetItems = null;
					using (SqlAccessor sqlAccessor = new SqlAccessor())
					using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "GetProductSetProducts"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_PRODUCTSET_SHOP_ID, cpsProductSet.ShopId);
						htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID, cpsProductSet.ProductSetId);

						dvProductSetItems = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					}

					// 商品セット定義がなければエラー（削除）
					if (dvProductSetItems.Count == 0)
					{
						throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCTSET_PRODUCT_UNBUYABLE));
					}

					//------------------------------------------------------
					// 各商品チェック
					//------------------------------------------------------
					int iParentCount = 0;
					int iChildCount = 0;
					foreach (CartProduct cpProduct in cpsProductSet.Items)
					{
						// 目的の商品をみつける
						DataRowView drvProduct = null;
						foreach (DataRowView drv in dvProductSetItems)
						{
							if ((cpProduct.ShopId == (string)drv[Constants.FIELD_PRODUCTVARIATION_SHOP_ID])
								&& (cpProduct.ProductId == (string)drv[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID])
								&& (cpProduct.VariationId == (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]))
							{
								drvProduct = drv;
								break;
							}
						}
						if (drvProduct == null)
						{
							throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CART_NO_ADJUSTMENT));
						}

						// 配送種別がカートと異なっていたらエラー（削除）
						if (this.ShippingType != (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE])
						{
							throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCTSET_SHIPPING_TYPE_CHANGE));
						}

						// デジタルコンテンツフラグがカートと異なっていたらエラー（削除）
						if (this.IsDigitalContentsOnly != ((string)drvProduct[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID))
						{
							throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCTSET_DIGITAL_CONTENTS_FLG_CHANGE));
						}

						// セット設定可能数を超えていたらエラー（削除）
						if (((drvProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN] != DBNull.Value) && ((int)drvProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN] > cpProduct.CountSingle))
							|| ((drvProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX] != DBNull.Value) && ((int)drvProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX] < cpProduct.CountSingle)))
						{
							throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCTSET_UNBUYABLE_COUNT));
						}

						// セット価格が変わっていたら価格更新（価格更新）
						if ((decimal)drvProduct[Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE] != cpProduct.Price)
						{
							if (blUpdateCart)
							{
								cpProduct.SetPrice((decimal)drvProduct[Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE]);
							}

							string strErrorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCT_PRICE_CHANGE).Replace("@@ 1 @@", cpProduct.ProductJointName);
							sbErrorMessages.Append(strErrorMessage);
						}

						// 親・子の個数カウント
						switch ((string)drvProduct[Constants.FIELD_PRODUCTSETITEM_FAMILY_FLG])
						{
							case Constants.FLG_PRODUCTSETITEM_FAMILY_FLG_PARENT:
								iParentCount += cpProduct.CountSingle;
								break;

							case Constants.FLG_PRODUCTSETITEM_FAMILY_FLG_CHILD:
								iChildCount += cpProduct.CountSingle;
								break;
						}
					}

					// 親子購入数チェック（削除）
					if (((dvProductSetItems[0][Constants.FIELD_PRODUCTSET_PARENT_MIN] != DBNull.Value) && ((int)dvProductSetItems[0][Constants.FIELD_PRODUCTSET_PARENT_MIN] > iParentCount))
						|| ((dvProductSetItems[0][Constants.FIELD_PRODUCTSET_CHILD_MIN] != DBNull.Value) && ((int)dvProductSetItems[0][Constants.FIELD_PRODUCTSET_CHILD_MIN] > iChildCount)))
					{
						throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCTSET_ITEM_UNBUYABLE_COUNT_LOW));
					}
					if (((dvProductSetItems[0][Constants.FIELD_PRODUCTSET_PARENT_MAX] != DBNull.Value) && ((int)dvProductSetItems[0][Constants.FIELD_PRODUCTSET_PARENT_MAX] < iParentCount))
						|| ((dvProductSetItems[0][Constants.FIELD_PRODUCTSET_CHILD_MAX] != DBNull.Value) && ((int)dvProductSetItems[0][Constants.FIELD_PRODUCTSET_CHILD_MAX] < iChildCount)))
					{
						throw new CartException(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCTSET_ITEM_UNBUYABLE_COUNT_HIGH));
					}
				}
				catch (CartException ex)
				{
					if (blUpdateCart)
					{
						// 商品セット削除
						RemoveProductSet(cpsProductSet);
					}

					// エラーメッセージ追加
					sbErrorMessages.Append(ex.Message);
				}
			}

			return sbErrorMessages.ToString();
		}


		/// <summary>
		/// 商品情報取得（セット内の商品も含めてどちらかをかえす）
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <returns>カート商品情報</returns>
		public CartProduct GetProductEither(string strShopId, string strProductId, string strVariationId)
		{
			foreach (CartProduct cp in this.Items)
			{
				if ((cp.ShopId == strShopId)
					&& (cp.ProductId == strProductId)
					&& (cp.VariationId == strVariationId))
				{
					return cp;
				}
			}

			return null;
		}

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="IsSetItem">セットアイテムフラグ</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID</param>
		/// <returns>カート商品情報</returns>
		public CartProduct GetProduct(string strShopId, string strProductId, string strVariationId, bool IsSetItem, Constants.AddCartKbn addCartKbn, string strProductSaleId)
		{
			foreach (CartProduct cp in this.Items)
			{
				if ((cp.ShopId == strShopId)
					&& (cp.ProductId == strProductId)
					&& (cp.VariationId == strVariationId)
					&& (cp.IsSetItem == IsSetItem)
					&& (cp.IsFixedPurchase == (addCartKbn == Constants.AddCartKbn.FixedPurchase))
					&& (cp.IsGift == (addCartKbn == Constants.AddCartKbn.GiftOrder))
					&& (cp.ProductSaleId == strProductSaleId))
				{
					return cp;
				}
			}

			return null;
		}
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="isSetItem">セットアイテムフラグ</param>
		/// <param name="isFixedPurchase">定期購入フラグ</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="productOptionValue">商品付帯情報</param>
		/// <param name="productBundleId">商品同梱ID</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="productPrice">商品単価</param>
		/// <returns>カート商品情報</returns>
		public CartProduct GetProduct(
			string shopId,
			string productId,
			string variationId,
			bool isSetItem,
			bool isFixedPurchase,
			string productSaleId,
			string productOptionValue,
			string productBundleId = "",
			string subscriptionBoxCourseId = "",
			decimal? productPrice = null)
		{
			foreach (CartProduct cp in this.Items)
			{
				if ((cp.ShopId == shopId)
					&& (cp.ProductId == productId)
					&& (cp.VariationId == variationId)
					&& (cp.IsSetItem == isSetItem)
					&& (cp.IsFixedPurchase == isFixedPurchase)
					&& (cp.IsSubscriptionBox == (string.IsNullOrEmpty(subscriptionBoxCourseId) == false))
					&& (cp.ProductSaleId == productSaleId)
					&& (cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() == productOptionValue)
					&& (cp.ProductBundleId == productBundleId)
					&& (cp.SubscriptionBoxCourseId == subscriptionBoxCourseId)
					&& ((productPrice == null)
						|| (cp.Price == productPrice)))
				{
					return cp;
				}
			}

			return null;
		}

		/// <summary>
		/// カート内に対象商品と同一商品があれば取得
		/// </summary>
		/// <param name="targetProduct">対象商品</param>
		/// <returns>カート内の同一商品(なければnull)</returns>
		public CartProduct GetSameProduct(CartProduct targetProduct)
		{
			var result = this.Items.Find(cp => cp.CheckSameProduct(targetProduct));
			return result;
		}

		/// <summary>
		/// カート内に対象商品と同一商品があれば取得
		/// </summary>
		/// <param name="targetProduct">対象商品</param>
		/// <returns>カート内の同一商品(なければnull)</returns>
		public CartProduct GetSameProductWithoutOptionSetting(CartProduct targetProduct)
		{
			var result = this.Items.Find(cp => cp.CheckSameProduct(targetProduct));
			return result;
		}

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <returns>商品情報</returns>
		/// <param name="cpProduct">商品情報</param>
		/// <returns>カート商品情報</returns>
		public DataRowView GetProduct(CartProduct cpProduct)
		{
			DataView dvProductVariation = ProductCommon.GetProductVariationInfo(
				cpProduct.ShopId,
				cpProduct.ProductId,
				cpProduct.VariationId,
				this.MemberRankId);

			return (dvProductVariation.Count != 0) ? dvProductVariation[0] : null;
		}

		/// <summary>
		/// 商品セット情報取得
		/// </summary>
		/// <param name="strProductSetId">商品セットID</param>
		/// <param name="strProductSetNo">商品セットNo.</param>
		/// <returns>商品セット情報</returns>
		public CartProductSet GetProductSet(string strProductSetId, int strProductSetNo)
		{
			foreach (CartProduct cp in this.Items)
			{
				if ((cp.IsSetItem) && (cp.ProductSet != null))
				{
					if ((cp.ProductSet.ProductSetId == strProductSetId)
						&& (cp.ProductSet.ProductSetNo == strProductSetNo))
					{
						return cp.ProductSet;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <returns>商品情報</returns>
		public DataView GetCartProductsEither()
		{
			return GetCartProductsEither("");
		}
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="strWhere"></param>
		/// <returns></returns>
		public DataView GetCartProductsEither(string strWhere)
		{
			DataView dvResult = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Cart", "GetCart"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_CART_CART_ID, this.CartId);
				htInput.Add(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID, this.MemberRankId);

				sqlStatement.Statement += strWhere;
				dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			return dvResult;
		}

		/// <summary>
		/// 配送種別情報取得
		/// </summary>
		/// <param name="iShippingZone">配送料地帯区分</param>
		/// <param name="strZip">離島検索用郵便番号</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="minShippingZoneNo">Min shipping zone no</param>
		/// <returns>Shop shipping information</returns>
		private ShopShippingModel GetShippingInfoFromDB(
			int iShippingZone,
			string strZip,
			string deliveryCompanyId = "",
			int minShippingZoneNo = 47)
		{
			var shipping = DomainFacade.Instance.ShopShippingService.GetFromZipcode(
				this.ShopId,
				this.ShippingType,
				iShippingZone,
				strZip,
				deliveryCompanyId,
				minShippingZoneNo);
			return shipping;
		}

		/// <summary>
		/// 配送料を固定値に上書きする
		/// </summary>
		/// <param name="price">配送料金</param>
		public void SetPriceShipping(decimal price)
		{
			m_shippingPrice = price;
		}

		/// <summary>
		/// 商品合計金額を固定値に上書きする
		/// </summary>
		/// <param name="price">商品合計金額</param>
		/// <param name="tax">商品合計金額税額</param>
		public void SetPriceSubtotal(decimal price, decimal tax)
		{
			this.PriceSubtotal = price;
			this.PriceSubtotalTax = tax;
		}

		/// <summary>
		/// 支払い金額総合計を固定値に上書きする
		/// </summary>
		/// <param name="price">支払い金額総合計</param>
		public void SetPriceTotal(decimal price)
		{
			m_priceTotal = price;
		}

		/// <summary>
		/// 利用ポイントセット
		/// </summary>
		/// <param name="dUsePoint">利用ポイント</param>
		/// <param name="dUsePointPrice">利用ポイント金額</param>
		/// <returns>エラー文言</returns>
		public void SetUsePoint(decimal dUsePoint, decimal dUsePointPrice)
		{
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				this.UsePoint = dUsePoint;
				this.UsePointPrice = dUsePointPrice;
			}
		}

		/// <summary>
		/// 配送先情報取得（※カートに対して配送先が１つ固定の場合のみ使用）
		/// </summary>
		/// <returns>配送先情報</returns>
		public CartShipping GetShipping()
		{
			if (this.Shippings.Count != 0)
			{
				return this.Shippings[0];
			}

			return null;
		}

		/// <summary>
		/// 配送先情報取得（※カートに対して配送先が複数の場合に使用）
		/// </summary>
		/// <returns>配送先情報のリスト</returns>
		public List<CartShipping> GetShippings()
		{
			if (this.Shippings.Count != 0)
			{
				return this.Shippings;
			}

			return null;
		}

		/// <summary>
		///  配送先住情報設定（※カートに対して配送先が１つ固定の場合のみ使用）
		/// </summary>
		/// <param name="csShipping">配送先情報</param>
		public void SetShippingAddressOnly(CartShipping csShipping)
		{
			if (this.Shippings.Count != 0)
			{
				this.Shippings[0].UpdateShippingAddr(
					csShipping.Name1,
					csShipping.Name2,
					csShipping.NameKana1,
					csShipping.NameKana2,
					csShipping.Zip,
					csShipping.Zip1,
					csShipping.Zip2,
					csShipping.Addr1,
					csShipping.Addr2,
					csShipping.Addr3,
					csShipping.Addr4,
					csShipping.Addr5,
					csShipping.ShippingCountryIsoCode,
					csShipping.ShippingCountryName,
					csShipping.CompanyName,
					csShipping.CompanyPostName,
					csShipping.Tel1,
					csShipping.Tel1_1,
					csShipping.Tel1_2,
					csShipping.Tel1_3,
					csShipping.IsSameShippingAsCart1,
					csShipping.ShippingAddrKbn);

				// アドレス帳保存情報セット
				this.Shippings[0].UpdateUserShippingRegistSetting(
					csShipping.UserShippingRegistFlg,
					csShipping.UserShippingName);
			}
			else
			{
				SetShippingAddressAndShippingDateTime(csShipping);
			}
		}
		/// <summary>
		/// 配送先住所＆配送日時情報設定（※カートに対して配送先が１つ固定の場合のみ使用）
		/// </summary>
		/// <param name="shipping">配送先情報</param>
		public void SetShippingAddressAndShippingDateTime(CartShipping shipping)
		{
			this.Shippings.Clear();
			this.Shippings.Add(shipping);
		}

		/// <summary>
		/// 現在のカートに投入されている商品合計から送料無料までの差額取得
		/// </summary>
		/// <returns>送料無料までの差額</returns>
		public decimal GetDifferenceToFreeShippingPrice()
		{
			decimal dDifference = 0;
			foreach (CartShipping cartShipping in this.Shippings)
			{
				// 配送先配送料無料最低金額設定されている場合は、設定した金額を表示
				if (cartShipping.IsConditionalShippingPriceFree)
				{
					dDifference += cartShipping.ConditionalShippingPriceThreshold.Value;
					continue;
				}
				dDifference += cartShipping.ShippingFreePrice;
			}
			// 商品小計 - セップロ・会員ランク割引・定期会員割引・定期回数割引
			dDifference -=
				this.PriceSubtotal
				- (this.IsAllItemsSubscriptionBoxFixedAmount ? 0m : this.SetPromotions.ProductDiscountAmount)
				- this.MemberRankDiscount
				- this.FixedPurchaseMemberDiscountAmount
				- this.FixedPurchaseDiscount;

			return (dDifference > 0) ? dDifference : 0;
		}

		/// <summary>
		/// 配送料無料判定
		/// </summary>
		/// <returns>配送料無料フラグ</returns>
		public bool IsFreeShipping()
		{
			return this.PriceShipping == 0;
		}

		/// <summary>
		/// クーポン利用判定
		/// </summary>
		/// <returns>クーポン利用</returns>
		public bool IsCouponUse()
		{
			var isCouponUse = (Constants.W2MP_COUPON_OPTION_ENABLED && (this.Coupon != null));
			return isCouponUse;
		}

		/// <summary>
		/// 配送料無料クーポン利用判定
		/// </summary>
		/// <returns>配送料無料利用フラグ</returns>
		public bool IsFreeShippingCouponUse()
		{
			var isFreeShippingCouponUse = ((Constants.W2MP_COUPON_OPTION_ENABLED)
				&& (this.Coupon != null)
				&& (this.Coupon.IsFreeShipping()));
			return isFreeShippingCouponUse;
		}

		/// <summary>
		/// クーポン配送料無料利用判定
		/// </summary>
		/// <returns>クーポン配送料無料利用判定</returns>
		public bool IsFreeShippingFlgCouponUse()
		{
			return ((this.Coupon != null) && (this.Coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID));
		}

		/// <summary>
		/// 割引金額有りクーポン配送料無料利用判定
		/// </summary>
		/// <returns>クーポン配送料無料利用判定</returns>
		public bool IsFreeShippingWithDiscountMoneyCouponUse()
		{
			return (IsFreeShippingFlgCouponUse()
				&& ((this.Coupon.DiscountPrice != 0)
					|| (this.Coupon.DiscountRate != 0)));
		}

		/// <summary>
		/// 割引金額無しクーポン配送料無料利用判定
		/// </summary>
		/// <returns>クーポン配送料無料利用判定</returns>
		public bool IsFreeShippingWithoutDiscountMoneyCouponUse()
		{
			return (IsFreeShippingFlgCouponUse()
				&& (this.Coupon.DiscountPrice == 0)
				&& (this.Coupon.DiscountRate == 0));
		}

		/// <summary>
		/// ブラックリスト型クーポン利用判定
		/// </summary>
		/// <returns>ブラックリスト型クーポン利用フラグ</returns>
		public bool IsBlacklistCouponUse()
		{
			return (this.Coupon != null) ? this.Coupon.IsBlacklistCoupon() : false;
		}

		/// <summary>
		/// 会員ランク配送料無料最低金額設定利用判定
		/// </summary>
		/// <returns>会員ランク配送料無料最低金額設定利用しているか</returns>
		public bool IsMemberRankFreeShippingThresholdUse()
		{
			// 会員ランク配送料割引方法判断用、会員ランク詳細を取得
			var memberRankDetail = MemberRankOptionUtility.GetMemberRankList()
				.FirstOrDefault(memberRank => memberRank.MemberRankId == this.MemberRankId);
			var result = (Constants.MEMBER_RANK_OPTION_ENABLED
				&& (memberRankDetail != null)
				&& (memberRankDetail.ShippingDiscountType
					== Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD));
			return result;
		}

		/// <summary>
		/// 注文者情報の住所に出荷するか判定
		/// </summary>
		/// <returns>注文者情報の住所に出荷するか</returns>
		public bool IsShippingAddressOwner()
		{
			var result = (this.Owner.ConcatenateAddressWithoutCountryName() == this.GetShipping().ConcatenateAddressWithoutCountryName());
			return result;
		}

		/// <summary>
		/// 按分適用後の商品小計取得
		/// </summary>
		/// <returns>按分適用後の商品小計</returns>
		public decimal GetPriceSubtotalDistributionAll()
		{
			decimal dPriceSubtotal = 0;
			foreach (CartProduct cp in this.Items)
			{
				dPriceSubtotal += cp.PriceSubtotalAfterDistribution;
			}

			return dPriceSubtotal;
		}

		/// <summary>
		/// カート内の商品情報がマスタから削除されているかチェック		※m_cartObjectListより移植
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// カート内の商品情報がマスタから削除されている場合、その商品をカート情報から削除
		/// 戻り値にどの商品が削除されたのかをメッセージとして返す
		/// </remarks>
		public string CheckProductDeleted()
		{
			StringBuilder sbErrorMessages = new StringBuilder();

			if (this.UpdateCartDb)
			{
				//------------------------------------------------------
				// カート商品情報取得（未削除商品）
				//------------------------------------------------------
				var dvProducts = new CartService().GetProductForDeleteCheck(this.CartId);

				//------------------------------------------------------
				// 削除商品が見つかれば削除＆エラーメッセージ生成
				//------------------------------------------------------
				foreach (DataRowView drvProduct in dvProducts)
				{
					if (drvProduct[Constants.FIELD_PRODUCT_NAME] == System.DBNull.Value)
					{
						// カートから商品情報削除（商品数0になればカート自体も削除）
						CartProduct cp = GetProductEither((string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID], (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
						var productOptionSettingSelectValues = (cp == null) ? (string)drvProduct[Constants.FIELD_CART_PRODUCT_OPTION_TEXTS] : cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues();
						RemoveProduct((string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID], (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID], Constants.AddCartKbn.Normal, "", productOptionSettingSelectValues);
						RemoveProduct((string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID], (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID], Constants.AddCartKbn.FixedPurchase, "", productOptionSettingSelectValues);
						RemoveProduct((string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID], (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID], Constants.AddCartKbn.Normal, "", productOptionSettingSelectValues);

						// エラーメッセージ取得
						sbErrorMessages.Append(OrderCommon.GetErrorMessage(OrderErrorcode.ProductDeleted, (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]));
					}
				}
			}

			return sbErrorMessages.ToString();
		}

		/// <summary>
		/// カート分割基準変更チェック		※m_cartObjectListより移植
		/// </summary>
		/// <returns>変更件数</returns>
		/// <remarks>分割基準などの値が更新されていたら再作成</remarks>
		public int UpdateProductCartDivTypeChanges()
		{
			int iResult = 0;

			DataView dvProducts = GetCartProductsEither();

			// カート内の商品に対してループ
			foreach (CartProduct cp in this.Items)
			{
				DataRowView drvProduct = OrderCommon.GetCartProductFromDataView(dvProducts, cp.ShopId, cp.ProductId, cp.VariationId, cp.IsFixedPurchase, cp.ProductSaleId);
				if (drvProduct == null)
				{
					continue;
				}

				// 配送種別orデジタルコンテンツフラグ が変更された商品がある？
				if ((this.ShippingType != (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE])
					|| this.IsDigitalContentsOnly != (((string)drvProduct[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID)))
				{
					// セット商品だったらセット削除
					if (cp.IsSetItem)
					{
						this.RemoveProductSet(cp.ProductSet.ProductSetId, cp.ProductSet.ProductSetNo);
					}
					// 通常商品だったら別カートへ
					else
					{
						// 商品数を知りたいため、削除対象商品データ取得
						List<CartProduct> lDeleteCartProducts = new List<CartProduct>();
						foreach (CartProduct cpTemp in this.Items)
						{
							if ((cpTemp.ShopId == (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_SHOP_ID])
								&& (cpTemp.ProductId == (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID])
								&& (cpTemp.VariationId == (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
								&& (cpTemp.IsSetItem == false))
							{
								lDeleteCartProducts.Add(cpTemp);
							}
						}
						foreach (CartProduct cpDelete in lDeleteCartProducts)
						{
							RemoveProduct(drvProduct, cpDelete.AddCartKbn, cpDelete.ProductSaleId, cpDelete.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());
						}
					}

					iResult++;
				}

				if (iResult != 0)
				{
					// カートの数が変わっているかもしれないので、breakして再帰処理でもう一回チェックする
					break;
				}
			}

			if (iResult != 0)
			{
				// もう一回チェック
				iResult += UpdateProductCartDivTypeChanges();
			}

			return iResult;
		}

		/// <summary>
		/// 支払コンビニ名取得
		/// </summary>
		/// <returns>支払コンビニ名</returns>
		public string GetPaymentCvsName()
		{
			// 支払がコンビニ前払い以外 かつ コンビニ後払い以外 また支払オブジェクトがnullの場合は空文字を返却
			if ((this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& (this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				return "";
			}

			if ((Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Dsk)
				&& (this.Payment.PaymentObject is PaymentDskCvs))
			{
				return ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CvsCodeValueTextFieldName, ((PaymentDskCvs)this.Payment.PaymentObject).ConveniType);
			}
			else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.SBPS)
			{
				return ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CvsCodeValueTextFieldName, this.Payment.SBPSWebCvsType.ToString());
			}
			else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc)
			{
				return ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CvsCodeValueTextFieldName, this.Payment.YamatoKwcCvsType.ToString());
			}
			else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo)
			{
				return ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CvsCodeValueTextFieldName, this.Payment.GmoCvsType);
			}
			else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten)
			{
				var cvsType = string.IsNullOrEmpty(this.Payment.RakutenCvsType)
					? RakutenConstants.CVS_TYPE_DEFAULT
					: this.Payment.RakutenCvsType;

				return ValueText.GetValueText(
					Constants.TABLE_ORDER,
					OrderCommon.CvsCodeValueTextFieldName,
					cvsType);
			}
			else if (OrderCommon.IsPaymentCvsTypeZeus
				&& (this.Payment.PaymentObject is PaymentZeusCvs))
			{
				return ValueText.GetValueText(
					Constants.TABLE_ORDER,
					OrderCommon.CvsCodeValueTextFieldName,
					this.Payment.GetZeusCvsType());
			}
			else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Paygent)
			{
				return ValueText.GetValueText(
					Constants.TABLE_ORDER,
					OrderCommon.CvsCodeValueTextFieldName,
					this.Payment.GetPaygentCvsType());
			}

			return "";
		}

		/// <summary>
		/// 注文拡張項目の作成
		/// </summary>
		public void CreateOrderExtend()
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED && (this.OrderExtendFirstCreate == false))
			{
				this.OrderExtendFirstCreate = true;
				this.OrderExtend = OrderExtendCommon.SetDefaultInput(this.OrderExtend);
			}
		}

		/// <summary>
		/// 無ければ注文メモを作成する
		/// </summary>
		/// <param name="strDisplayKbn">表示区分</param>
		/// <returns>注文メモ設定リスト</returns>
		public void CreateOrderMemo(string strDisplayKbn)
		{
			// 注文メモ作成
			if (this.OrderMemos == null)
			{
				var dvOrderMemo = new OrderMemoSettingService().GetOrderMemoSettingInDataView(strDisplayKbn);

				// 翻訳情報設定
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					var orderMemoIdList = dvOrderMemo.Cast<DataRowView>()
						.Select(drv => (string)drv[Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID]).ToArray();
					var translationSettings = NameTranslationCommon.GetOrderMemoSettingTranslationSettings(
						orderMemoIdList,
						RegionManager.GetInstance().Region.LanguageCode,
						RegionManager.GetInstance().Region.LanguageLocaleId);

					dvOrderMemo = NameTranslationCommon.SetTranslationDataToDataView(
						dvOrderMemo,
						Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING,
						translationSettings);
				}

				this.OrderMemos = new List<CartOrderMemo>();
				foreach (DataRowView drv in dvOrderMemo)
				{
					var beforeTranslationOrderMemoName =
						drv.DataView.Table.Columns.Contains("before_translation_order_memo_name")
							? (string)drv["before_translation_order_memo_name"]
							: String.Empty;
					this.OrderMemos.Add(new CartOrderMemo(drv, beforeTranslationOrderMemoName));
				}
			}
			else
			{
				// 注文メモ名称だけ切り替える
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					if (this.OrderMemos.Any() == false) return;

					var orderMemoIdList = this.OrderMemos.Select(orderMemo => orderMemo.OrderMemoID).ToList();
					var searchCondition = new NameTranslationSettingSearchCondition
					{
						DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING,
						MasterId1List = orderMemoIdList,
						LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
						LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
					};
					var translationSettings =
						new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

					this.OrderMemos = this.OrderMemos
						.Select(orderMemo => SetOrderMemoTranslationName(orderMemo, translationSettings)).ToList();
				}
			}
		}

		/// <summary>
		/// 注文メモ名称翻訳名設定
		/// </summary>
		/// <param name="orderMemo">注文メモ情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>注文メモ情報</returns>
		private CartOrderMemo SetOrderMemoTranslationName(CartOrderMemo orderMemo, NameTranslationSettingModel[] translationSettings)
		{
			var orderMemoNameTranslationSetting =
				translationSettings.FirstOrDefault(
					setting => (setting.MasterId1 == orderMemo.OrderMemoID)
						&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_ORDERMEMOSETTING_ORDER_MEMO_NAME));

			orderMemo.OrderMemoName = (orderMemoNameTranslationSetting != null)
				? orderMemoNameTranslationSetting.AfterTranslationalName
				: orderMemo.BeforeTranslationOrderMemoName;

			return orderMemo;
		}

		/// <summary>
		/// 注文メモを取得する
		/// </summary>
		/// <returns></returns>
		public string GetOrderMemos()
		{
			if (this.OrderMemos == null) return "";

			StringBuilder sbOrderMemo = new StringBuilder();
			foreach (CartOrderMemo com in this.OrderMemos)
			{
				// メモ名称は[](大括弧)で括り、改行区切りでタイトルと入力内容を登録
				if ((Constants.ORDERMEMO_REGISTERMODE == Constants.OrderMemoRegisterMode.Always)
					|| ((Constants.ORDERMEMO_REGISTERMODE == Constants.OrderMemoRegisterMode.Modified) && (com.DefaultText != com.InputText)))
				{
					if (string.IsNullOrEmpty(sbOrderMemo.ToString()) == false) sbOrderMemo.Append("\r\n");
					if (string.IsNullOrEmpty(com.OrderMemoName) == false) sbOrderMemo.Append("[").Append(com.OrderMemoName).Append("]");
					sbOrderMemo.Append("\r\n").Append(com.InputText);
				}
			}

			return sbOrderMemo.ToString();
		}

		/// <summary>
		/// 注文メモを取得する（注文確認画面表示用）
		/// </summary>
		/// <returns></returns>
		public string GetOrderMemosForOrderConfirm()
		{
			StringBuilder sbOrderMemo = new StringBuilder();
			foreach (CartOrderMemo com in this.OrderMemos)
			{
				if (string.IsNullOrEmpty(com.OrderMemoName) == false) sbOrderMemo.Append("[").Append(com.OrderMemoName).Append("]");
				sbOrderMemo.Append("\r\n").Append(com.InputText).Append("\r\n");
			}

			return sbOrderMemo.ToString();
		}

		/// <summary>
		/// セットプロモーション情報クリア
		/// </summary>
		public void ClearCartSetPromotion()
		{
			// セットプロモーション情報クリア
			this.SetPromotions.Clear();

			// 商品とセットプロモーションの紐づけクリア
			foreach (CartProduct item in this.Items)
			{
				item.QuantityAllocatedToSet.Clear();
			}
		}

		/// <summary>
		/// 別出荷フラグを更新
		/// </summary>
		public void UpdateAnotherShippingFlag()
		{
			this.Shippings.ForEach(
				shipping => shipping.AnotherShippingFlag =
					(shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						? Constants.FLG_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG
						: shipping.IsShippingStorePickup
							? Constants.FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG
							: IsAnotherShippingFlagValid(shipping)
								? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID
								: Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID);
		}

		/// <summary>
		/// 別出荷フラグをチェック
		/// </summary>
		/// <param name="shipping">配送先情報</param>
		/// <returns>有効:true</returns>
		private bool IsAnotherShippingFlagValid(CartShipping shipping)
		{
			return ((StringUtility.ToEmpty(this.Owner.Name1) != (StringUtility.ToEmpty(shipping.Name1)))
					|| (StringUtility.ToEmpty(this.Owner.Name2) != (StringUtility.ToEmpty(shipping.Name2)))
					|| (StringUtility.ToEmpty(this.Owner.Zip1) != (StringUtility.ToEmpty(shipping.Zip1)))
					|| (StringUtility.ToEmpty(this.Owner.Zip2) != (StringUtility.ToEmpty(shipping.Zip2)))
					|| (StringUtility.ToEmpty(this.Owner.Addr1) != (StringUtility.ToEmpty(shipping.Addr1)))
					|| (StringUtility.ToEmpty(this.Owner.Addr2) != (StringUtility.ToEmpty(shipping.Addr2)))
					|| (StringUtility.ToEmpty(this.Owner.Addr3) != (StringUtility.ToEmpty(shipping.Addr3)))
					|| (StringUtility.ToEmpty(this.Owner.Addr4) != (StringUtility.ToEmpty(shipping.Addr4)))
					|| (StringUtility.ToEmpty(this.Owner.CompanyName) != (StringUtility.ToEmpty(shipping.CompanyName)))
					|| (StringUtility.ToEmpty(this.Owner.CompanyPostName) != (StringUtility.ToEmpty(shipping.CompanyPostName)))
					|| (StringUtility.ToEmpty(this.Owner.Tel1_1) != (StringUtility.ToEmpty(shipping.Tel1_1)))
					|| (StringUtility.ToEmpty(this.Owner.Tel1_2) != (StringUtility.ToEmpty(shipping.Tel1_2)))
					|| (StringUtility.ToEmpty(this.Owner.Tel1_3) != (StringUtility.ToEmpty(shipping.Tel1_3))));
		}

		#region +CanUseAmazonPayment Amazonペイメントが使えるかどうか
		/// <summary>
		/// Amazonペイメントが使えるかどうか
		/// </summary>
		/// <returns>
		/// True：利用可
		/// False：利用不可
		/// </returns>
		public bool CanUseAmazonPayment()
		{
			var amazonPaymentId = OrderCommon.GetAmazonPayPaymentId();

			// 定期の場合は定期利用可能決済を見る
			if ((this.HasFixedPurchase)
				&& (Constants.CAN_FIXEDPURCHASE_PAYMENTIDS.Contains(amazonPaymentId) == false))
			{
				return false;
			}

			if ((this.GetShipping() != null)
				&& (this.GetShipping().PaymentSelectionFlg == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID))
			{
				// 配送種別で制限がある場合は配送種別の利用可能決済を見る
				if (this.GetShipping().PermittedPaymentIds.Contains(amazonPaymentId) == false)
				{
					return false;
				}
			}

			// 商品の利用制限決済に引っかかった場合は不可
			if (this.Items.Any(x => x.LimitedPaymentIds.Contains(amazonPaymentId)))
			{
				return false;
			}

			// OrderOwner決済種別判別
			var amazonPay = new PaymentService().Get(this.ShopId, amazonPaymentId);
			if(amazonPay.MobileDispFlg == Constants.FLG_PRODUCT_MOBILE_DISP_FLG_EC)
			{
				return false;
			}

			// 金額チェック
			if (this.HasFixedPurchase)
			{
				if (this.PriceCartTotalWithoutPaymentPrice > Constants.PAYMENT_AMAZON_AUTO_PAY_USABLE_PRICE_MAX) return false;
			}
			else
			{
				if (amazonPay.UsablePriceMax != null)
				{
					var usablePriceMax = (decimal)amazonPay.UsablePriceMax;
					if (this.PriceCartTotalWithoutPaymentPrice > usablePriceMax) return false;
				}
				if (amazonPay.UsablePriceMin != null)
				{
					var usablePriceMin = (decimal)amazonPay.UsablePriceMin;
					if (this.PriceCartTotalWithoutPaymentPrice < usablePriceMin) return false;
				}
			}

			// 上記判定でOKなら利用可能
			return true;
		}
		#endregion

		/// <summary>
		/// 注文情報からカート情報作成(DB保存なし)
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>カート情報</returns>
		public static CartObject CreateCartByOrder(OrderModel order)
		{
			var fixedPurchase = (order.IsFixedPurchaseOrder)
				? new FixedPurchaseService().Get(order.FixedPurchaseId)
				: null;
			var cart = CreateCartObject(order, fixedPurchase, false);
			cart.OrderId = order.OrderId;
			cart.OrderUserId = order.UserId;

			// 会員情報系
			var user = new UserService().Get(cart.CartUserId);
			cart.IsFixedPurchaseMember = user.IsFixedPurchaseMember;

			// Set settlement amount
			cart.SettlementAmount = order.SettlementAmount;
			cart.SettlementCurrency = order.SettlementCurrency;
			cart.SettlementRate = order.SettlementRate;

			// 再計算
			cart.Calculate(false);

			return cart;
		}

		/// <summary>
		/// 注文情報からカート情報作成(DB保存なし) 注文同梱処理用
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <returns>作成されたカート情報</returns>
		public static CartObject CreateCartForOrderCombine(OrderModel order, FixedPurchaseModel fixedPurchase)
		{
			// カート作成
			var cart = CreateCartObject(order, fixedPurchase, true);
			return cart;
		}

		/// <summary>
		/// 注文情報からカート情報作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="excludeBundleFlg">注文同梱フラグ</param>
		/// <returns>作成されたカート情報</returns>
		private static CartObject CreateCartObject(OrderModel order, FixedPurchaseModel fixedPurchase, bool excludeBundleFlg)
		{
			// カート作成
			var cart = (string.IsNullOrEmpty(order.PaymentCartId) == false)
				? new CartObject(
					order.PaymentCartId,
					order.UserId,
					order.OrderKbn,
					order.ShopId,
					order.ShippingId,
					order.IsDigitalContents,
					false,
					string.Empty,
					order.SubscriptionBoxCourseId)
				: new CartObject(
					order.UserId,
					order.OrderKbn,
					order.ShopId,
					order.ShippingId,
					order.IsDigitalContents,
					false,
					string.Empty,
					order.SubscriptionBoxCourseId);

			// 配送料・決済手数料税率情報の設定
			cart.ShippingTaxRate = order.ShippingTaxRate;
			cart.PaymentTaxRate = order.PaymentTaxRate;

			// ポイントセット
			var pointMaster = new PointService().GetPointMaster()
				.FirstOrDefault(master => (master.DeptId == Constants.W2MP_DEPT_ID) && (master.PointKbn == Constants.FLG_POINT_POINT_KBN_BASE));
			var usePointPrice = OrderPointUseHelper.GetOrderPointUsePrice(order.OrderPointUse, pointMaster);
			cart.SetUsePoint(order.OrderPointUse, usePointPrice);

			// クーポンセット
			if (order.Coupons.Any())
			{
				var dv = new CouponService().GetAllUserCouponsFromCouponCodeIncludeUnavailable(Constants.W2MP_DEPT_ID, order.UserId, order.Coupons[0].CouponCode);
				cart.Coupon = (dv.Count() != 0) ? new CartCoupon(dv[0]) : null;
			}

			// 注文者「お知らせメール配信希望」情報抽出
			var oderOwner = new UserService();
			var oderOwnerMailFlg = oderOwner.Get(order.UserId).MailFlg == Constants.FLG_USER_MAILFLG_OK;
			// 注文者情報セット
			cart.Owner = CreateCartOwnerFromOrderOwner(order.Owner, oderOwnerMailFlg);
			// 注文商品追加
			AddCartProductFromOrderItems(cart, order, excludeBundleFlg);

			// 配送先情報更新
			UpdateCartShippingFromOrderShipping(cart.Shippings[0], order.Shippings[0], true);

			// 配送先情報追加
			if (order.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON) AddShippingFromOrderShipping(cart, order.Shippings);

			var shippingCompany = new DeliveryCompanyService().Get(cart.Shippings[0].DeliveryCompanyId);

			cart.Shippings[0].ShippingTimeMessage = (shippingCompany == null) ? "指定無し" : shippingCompany.GetShippingTimeMessage(cart.Shippings[0].ShippingTime, "指定無し");

			cart.Shippings[0].ShippingMethod = order.Shippings[0].ShippingMethod;
			cart.Shippings[0].DeliveryCompanyId = order.Shippings[0].DeliveryCompanyId;
			cart.Shippings[0].ConvenienceStoreFlg = order.Shippings[0].ShippingReceivingStoreFlg;
			cart.Shippings[0].ConvenienceStoreId = order.Shippings[0].ShippingReceivingStoreId;
			cart.Shippings[0].ShippingReceivingStoreType = order.Shippings[0].ShippingReceivingStoreType;

			// 定期注文の場合、定期配送設定と次回配送日・次々回配送日をセット
			if (fixedPurchase != null)
			{
				var shipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(order.ShippingId);
				cart.Shippings[0].UpdateFixedPurchaseSetting(
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					shipping.FixedPurchaseShippingDaysRequired,
					shipping.FixedPurchaseMinimumShippingSpan);
				cart.Shippings[0].UpdateNextShippingDates(fixedPurchase.NextShippingDate.Value, fixedPurchase.NextNextShippingDate.Value);
			}

			// Get order invoice
			if (OrderCommon.DisplayTwInvoiceInfo())
			{
				var orderInvoice = new TwOrderInvoiceService().GetOrderInvoice(order.OrderId, order.Shippings[0].OrderShippingNo);
				if (orderInvoice != null)
				{
					cart.Shippings[0].UpdateInvoice(
						orderInvoice.TwUniformInvoice,
						orderInvoice.TwUniformInvoiceOption1,
						orderInvoice.TwUniformInvoiceOption2,
						orderInvoice.TwCarryType,
						orderInvoice.TwCarryTypeOption);
				}
			}

			// 商品小計セット
			cart.PriceSubtotal = order.OrderPriceSubtotal;

			// 決済情報セット
			cart.Payment = CreateCartPayment(order, true);
			cart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
				cart.ShopId,
				cart.Payment.PaymentId,
				cart.PriceSubtotal,
				cart.PriceCartTotalWithoutPaymentPrice);
			cart.PriceRegulation = order.OrderPriceRegulation;

			// 注文メモセット
			var displayKbn = (order.OrderKbn == Constants.FLG_ORDER_ORDER_KBN_MOBILE)
				? Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_MOBILE
				: Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC;
			cart.OrderMemos = CreateCartMemoFromOrderMemo(displayKbn, order.Memo, cart.Owner);

			// 調整金額
			cart.PriceRegulation = order.OrderPriceRegulation;
			// 管理メモ
			cart.ManagementMemo = order.ManagementMemo;
			// 配送メモ
			cart.ShippingMemo = order.ShippingMemo;
			// 外部連携メモ
			cart.RelationMemo = order.RelationMemo;
			// 調整金額メモ
			cart.RegulationMemo = order.RegulationMemo;
			// 税率毎価格情報
			cart.PriceInfoByTaxRate = order.OrderPriceByTaxRates
				.Select(orderPriceByTaxRate => new CartPriceInfoByTaxRate(orderPriceByTaxRate)).ToList();

			// 広告コード最新分
			cart.AdvCodeNew = order.AdvcodeNew;

			// コンバージョン情報
			cart.ContentsLog = new ContentsLogModel
			{
				ContentsType = order.InflowContentsType,
				ContentsId = order.InflowContentsId
			};

			// 領収書情報設定
			cart.ReceiptFlg = order.ReceiptFlg;
			cart.ReceiptAddress = order.ReceiptAddress;
			cart.ReceiptProviso = order.ReceiptProviso;

			// Set Is Order Sales Settled
			cart.IsOrderSalesSettled = (excludeBundleFlg
				&& (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED));

			// Set External Payment Type
			cart.Payment.ExternalPaymentType = order.ExternalPaymentType;

			cart.OrderExtend = OrderExtendCommon.ConvertOrderExtend(order);

			return cart;
		}

		/// <summary>
		/// カート商品情報作成
		/// </summary>
		/// <param name="orderItems">注文商品</param>
		/// <param name="memberRankId">会員ランクId</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>作成したカート商品情報</returns>
		private static CartProduct[] CreateCartProductFromOrderItems(OrderItemModel[] orderItems, string memberRankId, string fixedPurchaseId)
		{
			var cartProducts = new List<CartProduct>();
			foreach (var item in orderItems)
			{
				var dv = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, memberRankId);
				var cp = new CartProduct(
					dv[0],
					(item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON) ? Constants.AddCartKbn.FixedPurchase : Constants.AddCartKbn.Normal,
					StringUtility.ToEmpty(dv[0][Constants.FIELD_ORDERITEM_PRODUCTSALE_ID]),
					item.ItemQuantity,
					false,
					item.ProductOptionTexts,
					(item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON) ? fixedPurchaseId : "");
				cartProducts.Add(cp);
			}

			return cartProducts.ToArray();
		}

		/// <summary>
		/// カート商品情報作成
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="order">注文情報</param>
		/// <param name="excludeBundleFlg">注文同梱フラグ</param>
		private static void AddCartProductFromOrderItems(CartObject cart, OrderModel order, bool excludeBundleFlg)
		{
			// 注文商品リスト取得
			var orderItems = order.Shippings.SelectMany(s => s.Items).ToArray();
			if (excludeBundleFlg) orderItems = orderItems.Where(item => item.IsProductBundleItem == false).ToArray();

			// カート商品セットリスト取得
			var cartProductSets = orderItems
				.Where(
					orderItem => ((string.IsNullOrEmpty(orderItem.ProductSetId) == false)
				&& (ProductCommon.GetProductSetInfo(orderItem.ShopId, orderItem.ProductSetId).Count != 0)))
				.GroupBy(orderItem => orderItem.ProductSetId)
				.Select(orderItem => orderItem.First())
				.ToDictionary(
					orderItem => orderItem.ProductSetId,
					orderItem =>
				{
					var productSetInfo = ProductCommon.GetProductSetInfo(orderItem.ShopId, orderItem.ProductSetId);
					return new CartProductSet(productSetInfo[0], orderItem.ProductSetCount.GetValueOrDefault(), orderItem.ProductSetNo.GetValueOrDefault(), false);
				});

			// カート商品追加
			foreach (var orderItem in orderItems)
			{
				// 返品商品の場合は、次の商品へ
				if (orderItem.IsReturnItem) continue;
				// 削除対象の場合は、次の商品へ
				if (orderItem.DeleteTarget) continue;

				var itemQuantity = orderItem.ItemQuantity;
				var productPrice = orderItem.ProductPrice;
				var productTaxRate = orderItem.ProductTaxRate;
				var addCartKbn = (order.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)
					? Constants.AddCartKbn.GiftOrder
					: orderItem.IsFixedPurchaseItem
						? (string.IsNullOrEmpty(order.SubscriptionBoxCourseId) == false)
							? Constants.AddCartKbn.SubscriptionBox
							: Constants.AddCartKbn.FixedPurchase
						: Constants.AddCartKbn.Normal;

				var cartProduct = cart.GetProduct(
					orderItem.ShopId,
					orderItem.ProductId,
					orderItem.VariationId,
					(string.IsNullOrEmpty(orderItem.ProductSetId) == false),
					orderItem.IsFixedPurchaseItem,
					orderItem.ProductsaleId,
					orderItem.ProductOptionTexts,
					orderItem.ProductBundleId,
					orderItem.SubscriptionBoxCourseId);

				// カート商品が存在する場合は、商品数セット
				if (cartProduct != null)
				{
					cartProduct.Count += itemQuantity;
					cartProduct.CountSingle += itemQuantity;
					continue;
				}

				// カート商品が存在しない場合は、カート商品追加
				var product = ProductCommon.GetProductVariationInfo(
					orderItem.ShopId,
					orderItem.ProductId,
					orderItem.VariationId,
					order.MemberRankId);
				if (product.Count == 0) continue;

				var cartProductTemp = new CartProduct(
					product[0],
					addCartKbn,
					orderItem.ProductsaleId,
					itemQuantity,
					false,
					orderItem.ProductOptionTexts,
					orderItem.IsFixedPurchaseItem ? order.FixedPurchaseId : string.Empty,
					null,
					null,
					orderItem.SubscriptionBoxCourseId,
					isOrderCombine: excludeBundleFlg)
				{
					NoveltyId = orderItem.NoveltyId,
					RecommendId = orderItem.RecommendId,
					ProductBundleId = orderItem.ProductBundleId,
					BundleItemDisplayType = orderItem.BundleItemDisplayType,
					FixedPurchaseDiscountType = orderItem.FixedPurchaseDiscountType,
					FixedPurchaseDiscountValue = orderItem.FixedPurchaseDiscountValue,
				};

				// セット商品？
				if (cartProductSets.ContainsKey(orderItem.ProductSetId))
				{
					cartProductTemp = cartProductSets[orderItem.ProductSetId].AddProductVirtual(product[0], cartProductTemp.CountSingle);
				}

				if (cartProductTemp != null)
				{
					// 商品税率をセット
					// ※元注文の税率を正とする
					cartProductTemp.TaxRate = productTaxRate;

					// 商品価格セット
					// ※入力内容を正とする
					cartProductTemp.SetPrice(productPrice);

					// カートに追加
					cart.AddVirtural(cartProductTemp, false);
				}
			}
		}

		/// <summary>
		/// カート注文者情報作成
		/// </summary>
		/// <param name="orderOwner">注文者情報</param>
		/// <param name="mailFlg">お知らせメールの配信希望</param>
		/// <returns>カート注文者情報</returns>
		private static CartOwner CreateCartOwnerFromOrderOwner(OrderOwnerModel orderOwner, bool mailFlg)
		{
			var cartOwner = new CartOwner(
				orderOwner.OwnerKbn,
				orderOwner.OwnerName,
				orderOwner.OwnerName1,
				orderOwner.OwnerName2,
				orderOwner.OwnerNameKana,
				orderOwner.OwnerNameKana1,
				orderOwner.OwnerNameKana2,
				orderOwner.OwnerMailAddr,
				orderOwner.OwnerMailAddr2,
				orderOwner.OwnerZip,
				(orderOwner.OwnerZip.Contains('-')) ? orderOwner.OwnerZip.Split('-')[0] : string.Empty,
				(orderOwner.OwnerZip.Contains('-')) ? orderOwner.OwnerZip.Split('-')[1] : string.Empty,
				orderOwner.OwnerAddr1,
				orderOwner.OwnerAddr2,
				orderOwner.OwnerAddr3,
				orderOwner.OwnerAddr4,
				orderOwner.OwnerAddr5,
				orderOwner.OwnerAddrCountryIsoCode,
				orderOwner.OwnerAddrCountryName,
				orderOwner.OwnerCompanyName,
				orderOwner.OwnerCompanyPostName,
				orderOwner.OwnerTel1,
				// 電話番号の値がブランクの場合でも配列化できるよう"--"を結合
				(orderOwner.OwnerTel1.Contains('-')) ? (orderOwner.OwnerTel1 + "--").Split('-')[0] : string.Empty,
				(orderOwner.OwnerTel1.Contains('-')) ? (orderOwner.OwnerTel1 + "--").Split('-')[1] : string.Empty,
				(orderOwner.OwnerTel1.Contains('-')) ? (orderOwner.OwnerTel1 + "--").Split('-')[2] : string.Empty,
				orderOwner.OwnerTel2,
				(orderOwner.OwnerTel2.Contains('-')) ? (orderOwner.OwnerTel2 + "--").Split('-')[0] : string.Empty,
				(orderOwner.OwnerTel2.Contains('-')) ? (orderOwner.OwnerTel2 + "--").Split('-')[1] : string.Empty,
				(orderOwner.OwnerTel2.Contains('-')) ? (orderOwner.OwnerTel2 + "--").Split('-')[2] : string.Empty,
				mailFlg,
				orderOwner.OwnerSex,
				orderOwner.OwnerBirth,
				orderOwner.AccessCountryIsoCode,
				orderOwner.DispLanguageCode,
				orderOwner.DispLanguageLocaleId,
				orderOwner.DispCurrencyCode,
				orderOwner.DispCurrencyLocaleId);

			return cartOwner;
		}

		/// <summary>
		/// カート配送先情報更新
		/// </summary>
		/// <param name="cartShipping">カート配送先情報</param>
		/// <param name="orderShipping">注文配送先情報</param>
		/// <param name="isSameShippingAsCart1">配送先がカート1と等しいか</param>
		private static void UpdateCartShippingFromOrderShipping(CartShipping cartShipping, OrderShippingModel orderShipping, bool isSameShippingAsCart1)
		{
			cartShipping.UpdateShippingAddr(
				orderShipping.DataSource,
				isSameShippingAsCart1,
				(orderShipping.AnotherShippingFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)
					? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
					: (orderShipping.AnotherShippingFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG)
						? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP
						: (orderShipping.AnotherShippingFlg == Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID)
							? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER
							: CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);

			cartShipping.UpdateShippingDateTime(
				orderShipping.ShippingDate.HasValue,
				(string.IsNullOrEmpty(orderShipping.ShippingTime) == false),
				orderShipping.ShippingDate,
				orderShipping.ShippingTime,
				"");

			cartShipping.AnotherShippingFlag = orderShipping.AnotherShippingFlg;

			cartShipping.DeliveryCompanyId = orderShipping.DeliveryCompanyId;
			cartShipping.ScheduledShippingDate = orderShipping.ScheduledShippingDate;
		}

		/// <summary>
		/// カート配送先情報追加
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="orderShippings">注文配送先情報リスト</param>
		private static void AddShippingFromOrderShipping(CartObject cart, OrderShippingModel[] orderShippings)
		{
			// 配送先追加
			cart.Shippings.AddRange(Enumerable.Range(1, orderShippings.Length - 1).Select(index =>
			{
				var cartShipping = new CartShipping(cart);
				cartShipping.ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
				var shippingZip = orderShippings[index].ShippingZip.Split('-');
				var shippingTel1 = orderShippings[index].ShippingTel1.Split('-');
				cartShipping.UpdateShippingAddr(
					orderShippings[index].DataSource,
					true,
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
				cartShipping.ShippingMethod = orderShippings[index].ShippingMethod;
				cartShipping.DeliveryCompanyId = orderShippings[index].DeliveryCompanyId;
				return cartShipping;
			}));

			// 配送先に紐づける
			var cartShippingIndex = 0;
			foreach (var orderShipping in orderShippings)
			{
				var cartShipping = cart.Shippings[cartShippingIndex];
				foreach (var orderItem in orderShipping.Items)
				{
					// 返品商品の場合は、次の商品へ
					if (orderItem.IsReturnItem) continue;
					// 削除対象の場合は、次の商品へ
					if (orderItem.DeleteTarget) continue;

					var cartProduct = cart.Items.Find(i =>
						(i.ShopId == orderItem.ShopId)
						&& (i.ProductId == orderItem.ProductId)
						&& (i.VariationId == orderItem.VariationId));
					cart.Shippings[cartShippingIndex].ProductCounts.Add(new CartShipping.ProductCount(cartProduct, orderItem.ItemQuantity));
				}
				cartShippingIndex++;
			}
			// 商品数セット
			cart.Items.ForEach(cartProduct =>
			{
				var productCounts = cart.Shippings.Select(s => s.ProductCounts.Where(p => p.Product == cartProduct).Where(p => p != null));
				var sum = productCounts.Sum(s => s.Sum(p => p.Count));
				cartProduct.CountSingle = sum;
				cartProduct.Calculate();
			});
		}

		/// <summary>
		/// カート決済情報作成
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="isSamePaymentAsCart1">カート1と同じ決済か</param>
		/// <returns>カート決済情報</returns>
		private static CartPayment CreateCartPayment(
			OrderModel order,
			bool isSamePaymentAsCart1)
		{
			var cartPayment = new CartPayment
			{
				PaymentId = order.OrderPaymentKbn,
				PaymentName = order.PaymentName,
				IsSamePaymentAsCart1 = isSamePaymentAsCart1
			};

			// クレジットカード or ペイパル決済 or Paidy翌月払い決済の場合は既存のユーザークレジットカード情報をセット
			if ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				|| ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
					&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct))
			{
				var userCreditCard = new UserCreditCardService().Get(order.UserId, (int)order.CreditBranchNo);
				cartPayment.CreditCardBranchNo = StringUtility.ToEmpty(userCreditCard.BranchNo);
				cartPayment.CreditCardCompany = order.CardKbn;
				cartPayment.CreditExpireYear = userCreditCard.ExpirationYear;
				cartPayment.CreditExpireMonth = userCreditCard.ExpirationMonth;
				cartPayment.CreditInstallmentsCode = order.CardInstallmentsCode;
				cartPayment.CreditAuthorName = userCreditCard.AuthorName;
				cartPayment.PaidyToken = (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					? userCreditCard.CooperationId
					: string.Empty;

				if (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					cartPayment.CreditCardNo4 = userCreditCard.LastFourDigit;
					cartPayment.UserCreditCardRegistFlg = (string.IsNullOrEmpty(userCreditCard.CardDispName) == false);
				}
			}

			// Bind transaction id for Atone and Aftee
			if ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
			{
				cartPayment.CardTranId = order.CardTranId;
			}

			// Bind credit installment codes for Neweb Pay
			if ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (order.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
			{
				cartPayment.NewebPayCreditInstallmentsCode = order.CardInstallmentsCode;
			}

			return cartPayment;
		}

		/// <summary>
		/// 注文メモからカート注文メモを作成する
		/// </summary>
		/// <param name="displayKbn">表示区分</param>
		/// <param name="orderMemo">注文メモ</param>
		/// <param name="cartOwner">カート注文者情報</param>
		/// <returns>カート注文メモ</returns>
		public static List<CartOrderMemo> CreateCartMemoFromOrderMemo(string displayKbn, string orderMemo, CartOwner cartOwner)
		{
			// 空のカート注文メモ作成
			CartOrderMemo[] cartOrderMemos = null;

			if (Constants.GLOBAL_OPTION_ENABLE == false)
			{
				cartOrderMemos = new OrderMemoSettingService().GetOrderMemoSettingInDataView(displayKbn).Cast<DataRowView>()
					.Select(drv => new CartOrderMemo(drv)).ToArray();
			}
			else
			{
				cartOrderMemos = new OrderMemoSettingService().GetOrderMemoSettingContainsGlobalSetting(displayKbn, cartOwner.DispLanguageCode, cartOwner.DispLanguageLocaleId)
					.Cast<DataRowView>().Select(drv => new CartOrderMemo(drv)).ToArray();
			}

			var orderMemos = new List<CartOrderMemo>();
			CartOrderMemo creatingMemo = null;

			foreach (var line in orderMemo.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
			{
				var isOrderMemoName = false;
				foreach (var cartOrderMemo in cartOrderMemos)
				{
					// 注文メモの行から作成対象のカート注文メモを判定する
					if (line == string.Format("[{0}]", cartOrderMemo.OrderMemoName))
					{
						// 作成中のメモがあった場合、作成対象のメモを変更する前にリストに追加
						if (creatingMemo != null) orderMemos.Add(creatingMemo);

						creatingMemo = (CartOrderMemo)cartOrderMemo.Clone();
						creatingMemo.InputText = "";
						isOrderMemoName = true;
						break;
					}
				}

				if (creatingMemo == null) creatingMemo = new CartOrderMemo("", "", null, null, "", 0, "", "", null);

				// メモ名称以外の行の場合に、作成中のカート注文メモに追加する
				if (isOrderMemoName == false) creatingMemo.InputText += line + "\r\n";
			}

			// 作成中のカート注文メモがある場合、リストに追加する
			if (creatingMemo != null) orderMemos.Add(creatingMemo);

			return orderMemos;
		}

		/// <summary>
		/// カート追加可能か？
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <param name="doNotCartSeparation">カート分割を行わないよう制御するか</param>
		/// <param name="isCartSelect">選択カートか？</param>
		/// <returns>true：追加可、false：追加不可 </returns>
		public bool CanAddCartObject(CartProduct cartProduct, bool doNotCartSeparation, bool isCartSelect = false)
		{
			if (((isCartSelect == false) || this.Items.Any(product => product.CartIdSelect == cartProduct.CartId))
				&& (this.ShopId == cartProduct.ShopId)
				&& (this.ShippingType == cartProduct.ShippingType)
				&& (this.IsDigitalContentsOnly == cartProduct.IsDigitalContents)
				&& (this.IsOrderCombined || (this.SubscriptionBoxCourseId == cartProduct.SubscriptionBoxCourseId)))
			{
				if (doNotCartSeparation) return true;

				switch (cartProduct.AddCartKbn)
				{
					case Constants.AddCartKbn.Normal:
						if ((this.IsFixedPurchaseOnly == false)
							&& (this.IsGift == false))
						{
							return true;
						}
						break;

					case Constants.AddCartKbn.FixedPurchase:
						if ((this.IsFixedPurchaseOnly)
							|| ((Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION == false) && (this.IsGift == false)))
						{
							return true;
						}
						break;

					case Constants.AddCartKbn.SubscriptionBox:
						if (this.IsSubscriptionBox && (this.IsGift == false))
						{
							return true;
						}
						break;

					case Constants.AddCartKbn.GiftOrder:
						if (this.IsGift)
						{
							return true;
						}
						break;
				}
			}

			return false;
		}

		/// <summary>
		/// 最大同時購入可能数チェック
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <param name="sameCartProduct">同一カート商品</param>
		/// <param name="isCartSelect">カート選択か？</param>
		/// <returns>true：最大購入可能数以下、false：最大購入可能数以上</returns>
		public bool CheckProductMaxSellQuantity(CartProduct cartProduct, CartProduct sameCartProduct, bool isCartSelect = false)
		{
			var isProductMaxSellQuantity = ((sameCartProduct != null) && (isCartSelect == false)
				&& ((sameCartProduct.CountSingle + cartProduct.CountSingle) <= sameCartProduct.ProductMaxSellQuantity));
			return isProductMaxSellQuantity;
		}

		/// <summary>
		/// カートコピー
		/// </summary>
		/// <param name="isCreateNewCartId">カートIDを新規採番するか</param>
		/// <returns>コピーカート</returns>
		public CartObject Copy(bool isCreateNewCartId = true)
		{
			var copyCart = DeepCopy<CartObject>(this);
			if(isCreateNewCartId) copyCart.CartId = GetNewCartId();
			copyCart.Owner = this.Owner;	// 注文者はディープコピーしない

			return copyCart;
		}

		/// <summary>
		/// ディープコピー
		/// </summary>
		/// <typeparam name="T">オブジェクトタイプ</typeparam>
		/// <param name="obj">コピー元オブジェクト</param>
		/// <returns>コピー先オブジェクト</returns>
		private static T DeepCopy<T>(T copyFromObj)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, copyFromObj);
				stream.Position = 0;

				return (T)formatter.Deserialize(stream);
			}
		}

		/// <summary>
		/// 定期購入価格設定
		/// </summary>
		public void SetFixedPurchasePrice()
		{
			// カート内商品に設定されている定期初回購入価格を、2回目以降の定期購入価格に更新
			this.Items = this.Items.Select(SetFixedPurhcasePriceForCartProduct).ToList();
		}

		/// <summary>
		/// カート商品情報に定期購入価格設定
		/// </summary>
		/// <param name="cartProduct">カート商品情報</param>
		/// <returns>カート商品情報</returns>
		public CartProduct SetFixedPurhcasePriceForCartProduct(CartProduct cartProduct)
		{
			var product = new ProductService().GetProductVariation(Constants.CONST_DEFAULT_SHOP_ID, cartProduct.ProductId, cartProduct.VariationId, this.MemberRankId);
			cartProduct.SetPrice(product.VariationFixedPurchasePrice ?? product.MemberRankPrice ?? product.SpecialPrice ?? product.Price);
			return cartProduct;
		}

		/// <summary>
		/// カートオブジェクト複製
		/// </summary>
		/// <returns>複製したカートオブジェクト</returns>
		public CartObject CloneCart()
		{
			var clone = (CartObject)MemberwiseClone();
			clone.Items = this.Items.Where(i => (i.IsFixedPurchase || i.IsSubscriptionBox)).Select(i => i.Clone()).ToList();
			clone.Shippings = this.Shippings.Select(i => i.Clone()).ToList();
			clone.Payment = this.Payment.Clone();
			clone.SetPromotions = this.SetPromotions.Clone();
			clone.PriceInfoByTaxRate = this.PriceInfoByTaxRate.Select(i => i.Clone()).ToList();
			return clone;
		}

		/// <summary>
		/// カートオブジェクト複製(カート商品リストのみ)
		/// </summary>
		/// <returns>複製したカートオブジェクト</returns>
		public CartObject CloneCartItem()
		{
			var clone = (CartObject)MemberwiseClone();
			clone.Items = this.Items.Where(i => i.IsFixedPurchase).Select(i => i.Clone()).ToList();
			return clone;
		}

		/// <summary>
		/// 購入商品を過去に購入したことがあるか（類似配送先を含む）
		/// </summary>
		/// <returns>ユーザーに重複情報が含まれるか</returns>
		public bool CheckProductOrderLimit()
		{
			// 注文同梱の場合は、親注文の商品を判定対象外にする
			var targetProductList = string.IsNullOrEmpty(this.OrderCombineParentOrderId)
				? this.Items
				: this.TargetProductListForCheckProductOrderLimit;
			this.ProductOrderLmitOrderIds = new string[0];

			if (this.HasFixedPurchase)
			{
				var fixedPurchaseProductIdList = string.Format(
					"'{0}'",
					string.Join(
						"','",
						targetProductList
							.Where(item => item.IsFixedPurchase)
							.Select(item => item.ProductId)));
				this.ProductOrderLmitOrderIds = this.Shippings.SelectMany(
					shipping => new OrderService().GetOrderIdForFixedProductOrderLimitCheck(
						shipping.CreateOrderShipping(),
						CreateOrderForCheck(),
						this.Owner.CreateModel(this.OrderId),
						this.ShopId,
						fixedPurchaseProductIdList,
						new[] { this.OrderId })).ToArray();
			}

			if ((this.ProductOrderLmitOrderIds.Any() == false)
				&& this.Items.Any(i => (i.AddCartKbn == Constants.AddCartKbn.Normal)))
			{
				var productIdList = string.Format(
					"'{0}'",
					string.Join(
						"','",
						targetProductList
							.Where(item => (item.IsFixedPurchase == false))
							.Select(item => item.ProductId)));
				this.ProductOrderLmitOrderIds = this.Shippings.SelectMany(
					shipping => new OrderService().GetOrderIdForProductOrderLimitCheck(
						shipping.CreateOrderShipping(),
						CreateOrderForCheck(),
						this.Owner.CreateModel(this.OrderId),
						this.ShopId,
						productIdList,
						new[] { this.OrderId })).ToArray();
			}

			if (this.ProductOrderLmitOrderIds.Length > 0)
			{
				this.IsOrderLimit = true;
			}

			// カート内重複チェック
			this.DuplicateLimitProductIds = this.Items
				.Where(product => product.IsOrderLimitProduct)
				.GroupBy(product => product.ProductId)
				.Where(product => product.Count() > 1)
				.Select(group => group.Key).ToArray();
			this.IsOrderLimit |= this.IsCompliantOrderLimitProduct;

			return (this.ProductOrderLmitOrderIds.Length > 0);
		}

		/// <summary>
		/// 宅配便か？（ギフト購入時用）
		/// </summary>
		/// <param name="currentCartShippingIndex">現在のカート配送先情報インデックス</param>
		/// <returns>true：宅配便、false：メール便</returns>
		public bool IsExpressDeliveryForGift(int currentCartShippingIndex)
		{
			return ((this.Shippings.Count > 0) && this.Shippings[currentCartShippingIndex].IsExpress);
		}

		/// <summary>
		/// カート情報で注文情報を作成
		/// </summary>
		/// <para name="orderId">受注ID</para>
		/// <returns>新規注文情報</returns>
		public OrderModel CreateNewOrder(string orderId = "")
		{
			// HACK:カート情報を用いて新規注文情報を作成する際に利用する。
			var order = CreateOrder(orderId);
			order.Owner = this.Owner.CreateModel(order.OrderId);
			order.Shippings = this.Shippings.Select(shipping => shipping.CreateOrderShipping()).ToArray();
			return order;
		}
		/// <summary>
		/// カート情報で注文情報を生成
		/// </summary>
		/// <param name="orderOld">注文情報</param>
		/// <returns>更新用注文情報</returns>
		public OrderModel CreateNewOrder(OrderModel orderOld)
		{
			// HACK:カート情報を用いて注文情報を更新する際に利用する。
			var order = CreateOrder(DeepCopy(orderOld));
			return order;
		}

		/// <summary>
		/// 注文情報作成
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>注文情報</returns>
		private OrderModel CreateOrder(string orderId = "")
		{
			var order = CreateOrder(
				 new OrderModel
				{
					OrderId = orderId,
					UserId = this.CartUserId,
					OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP,
					OrderDate = DateTime.Now,
					OrderKbn = this.OrderKbn,
					CombinedOrgOrderIds = this.OrderCombineParentOrderId ?? string.Empty,
					ManagementMemo = OrderCommon.GetNotFirstTimeFixedPurchaseManagementMemo(this.ManagementMemo, this.ProductOrderLmitOrderIds, this.HasNotFirstTimeByCart),
					ShippingMemo = this.ShippingMemo,
					RelationMemo = this.RelationMemo ?? string.Empty,
					ExtendStatus39 = (this.HasNotFirstTimeOrderIdList && this.HasNotFirstTimeByCart)
						? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON
						: Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
					ExtendStatusDate39 = DateTime.Now,
					OrderItemCount = this.Items.Count,
					OrderProductCount = this.Items.Sum(item => item.Count),
					CardKbn = this.Payment.IsCredit
						? this.Payment.CreditCardCompany
						: string.Empty,
					CardInstruments = (this.Payment.IsCredit && OrderCommon.CreditInstallmentsSelectable)
						? ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, this.Payment.CreditInstallmentsCode)
						: string.Empty,
					CardInstallmentsCode = (this.Payment.IsCredit && OrderCommon.CreditInstallmentsSelectable)
						? this.Payment.CreditInstallmentsCode
						: string.Empty,
					Memo = this.GetOrderMemos(),
					RegulationMemo = this.RegulationMemo ?? string.Empty,
					GiftFlg = this.IsGift
						? Constants.FLG_ORDER_GIFT_FLG_ON
						: Constants.FLG_ORDER_GIFT_FLG_OFF,
					DigitalContentsFlg = this.IsDigitalContentsOnly
						? Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON
						: Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF,
					OrderTaxIncludedFlg = Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX,
					OrderTaxRate = 0, // 使用しない
					OrderTaxRoundType = Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
					ShippingTaxRate = this.ShippingTaxRate,
					PaymentTaxRate = this.PaymentTaxRate,
					LastAuthFlg = Constants.FLG_ORDER_LAST_AUTH_FLG_ON,
					DateCreated = DateTime.Now,
					DateChanged = DateTime.Now,
					LastChanged = Constants.FLG_LASTCHANGED_USER,
					AdvcodeNew = this.AdvCodeNew,
				});

			return order;
		}
		/// <summary>
		/// 注文情報生成
		/// </summary>
		/// <param name="orderOld">元注文情報</param>
		/// <returns>注文情報</returns>
		private OrderModel CreateOrder(OrderModel orderOld)
		{
			var order = new OrderModel(orderOld.DataSource)
			{
				ShopId = string.IsNullOrEmpty(orderOld.ShopId)
					? this.ShopId
					: orderOld.ShopId,
				OrderId = (string.IsNullOrEmpty(orderOld.OrderId) == false)
					? orderOld.OrderId
					: OrderCommon.CreateOrderId(string.IsNullOrEmpty(orderOld.ShopId) ? this.ShopId : orderOld.ShopId),
				OrderPaymentKbn = this.Payment.PaymentId,
				PaymentName = this.Payment.PaymentName,
				CardTranId = (this.Payment.PaymentId == orderOld.OrderPaymentKbn)
					? orderOld.CardTranId
					: string.Empty,
				OrderPriceSubtotal = this.PriceSubtotal,
				OrderPriceSubtotalTax = this.PriceSubtotalTax,
				OrderPriceShipping = this.PriceShipping,
				OrderPriceExchange = this.Payment.PriceExchange,
				OrderPriceRegulation = this.PriceRegulation,
				OrderPriceTotal = this.PriceTotal,
				OrderPriceTax = this.PriceTax,
				LastBilledAmount = this.PriceTotal,
				OrderDiscountSetPrice = this.Items.Where(item => item.IsSetItem).Sum(item => item.SetDiscountItemPrice),
				ShippingId = this.ShippingType,
				MemberRankDiscountPrice = Constants.MEMBER_RANK_OPTION_ENABLED
					? this.MemberRankDiscount
					: 0,
				OrderCouponUse = Constants.W2MP_COUPON_OPTION_ENABLED
					? this.UseCouponPrice
					: 0,
				FixedPurchaseMemberDiscountAmount = (Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.MEMBER_RANK_OPTION_ENABLED)
					? this.FixedPurchaseMemberDiscountAmount
					: 0,
				FixedPurchaseDiscountPrice = Constants.FIXEDPURCHASE_OPTION_ENABLED
					? this.FixedPurchaseDiscount
					: 0,
				SettlementCurrency = this.SettlementCurrency,
				SettlementRate = this.SettlementRate,
				SettlementAmount = this.SettlementAmount,
				ExternalPaymentType = StringUtility.ToEmpty(this.Payment.ExternalPaymentType),
				OrderPointAdd = this.FirstBuyPoint + this.BuyPoint,
			};
			order.Items = CreateOrderItems(orderOld);
			order.Shippings = this.Shippings.Select(shipping => shipping.CreateOrderShipping()).ToArray();
			foreach (var shipping in order.Shippings)
			{
				shipping.OrderId = order.OrderId;
			}
			order.SetPromotions = Constants.SETPROMOTION_OPTION_ENABLED
				? CreateOrderSetPromotions(orderOld)
				: new OrderSetPromotionModel[0];
			order.Coupons = (this.Coupon != null)
				? new[] { this.Coupon.CreateModel(order.OrderId, order.IsMobileOrder) }
				: new OrderCouponModel[0];
			order.OrderPriceByTaxRates = (this.PriceInfoByTaxRate != null)
				? this.PriceInfoByTaxRate.Select(taxRate => taxRate.CreateModel()).ToArray()
				: new OrderPriceByTaxRateModel[0];

			return order;
		}

		/// <summary>
		/// 注文情報作成(購入商品を過去に購入したことがあるかのチェック用)
		/// </summary>
		/// <returns>注文情報</returns>
		private OrderModel CreateOrderForCheck()
		{
			var order = new OrderModel
			{
				UserId = this.CartUserId,
				OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP,
				OrderDate = DateTime.Now,
				OrderKbn = this.OrderKbn,
				CombinedOrgOrderIds = this.OrderCombineParentOrderId ?? string.Empty,
				ManagementMemo = OrderCommon.GetNotFirstTimeFixedPurchaseManagementMemo(
					this.ManagementMemo,
					this.ProductOrderLmitOrderIds,
					this.HasNotFirstTimeByCart),
				ShippingMemo = this.ShippingMemo,
				RelationMemo = this.RelationMemo ?? string.Empty,
				ExtendStatus39 = (this.HasNotFirstTimeOrderIdList && this.HasNotFirstTimeByCart)
					? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON
					: Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
				ExtendStatusDate39 = DateTime.Now,
				OrderItemCount = this.Items.Count,
				OrderProductCount = this.Items.Sum(item => item.Count),
				CardKbn = this.Payment.IsCredit ? this.Payment.CreditCardCompany : string.Empty,
				CardInstruments = (this.Payment.IsCredit && OrderCommon.CreditInstallmentsSelectable)
					? ValueText.GetValueText(
						Constants.TABLE_ORDER,
						OrderCommon.CreditInstallmentsValueTextFieldName,
						this.Payment.CreditInstallmentsCode)
					: string.Empty,
				CardInstallmentsCode = (this.Payment.IsCredit && OrderCommon.CreditInstallmentsSelectable)
					? this.Payment.CreditInstallmentsCode
					: string.Empty,
				Memo = this.GetOrderMemos(),
				RegulationMemo = this.RegulationMemo ?? string.Empty,
				GiftFlg = this.IsGift ? Constants.FLG_ORDER_GIFT_FLG_ON : Constants.FLG_ORDER_GIFT_FLG_OFF,
				DigitalContentsFlg = this.IsDigitalContentsOnly
					? Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON
					: Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF,
				OrderTaxIncludedFlg = Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX,
				OrderTaxRate = 0, // 使用しない
				OrderTaxRoundType = Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
				ShippingTaxRate = this.ShippingTaxRate,
				PaymentTaxRate = this.PaymentTaxRate,
				LastAuthFlg = Constants.FLG_ORDER_LAST_AUTH_FLG_ON,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_USER,
			};
			order.Shippings = this.Shippings.Select(shipping => shipping.CreateOrderShipping()).ToArray();
			foreach (var shipping in order.Shippings)
			{
				shipping.OrderId = order.OrderId;
			}

			return order;
		}

		/// <summary>
		/// 注文商品情報生成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns></returns>
		public OrderItemModel[] CreateOrderItems(OrderModel order)
		{
			var orderItems = (order.Items != null)
				? order.Items.Where(i => i.IsReturnItem).ToList()
				: new List<OrderItemModel>();

			if (this.IsGift == false)
			{
				var orderItemNo = (order.Items != null) ? order.Items.Length : 0;
				// 通常/セット商品(同梱商品を除く)
				orderItems.AddRange(this.Items
					.Where(p => ((p.QuantitiyUnallocatedToSet != 0) && (p.IsBundle == false)))
					.Select(p =>
					{
						orderItemNo++;
						return p.CreateOrderItem(
							order,
							p.IsSetItem ? p.Count : p.QuantitiyUnallocatedToSet,
							p.IsSetItem ? p.CountSingle : p.QuantitiyUnallocatedToSet,
							1,
							orderItemNo,
							null,
							null);
					}).ToArray());

				// セットプロモーション商品
				foreach (var setpromotion in this.SetPromotions.Items)
				{
					var orderSetpromotionItemNo = 0;
					orderItems.AddRange(setpromotion.Items
						.Select(p =>
						{
							orderItemNo++;
							orderSetpromotionItemNo++;
							return p.CreateOrderItem(
								order,
								p.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo],
								p.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo],
								1,
								orderItemNo,
								setpromotion.CartSetPromotionNo,
								orderSetpromotionItemNo);
						}).ToArray());
				}

				// 同梱商品
				orderItems.AddRange(this.Items
					.Where(p => ((p.QuantitiyUnallocatedToSet != 0) && p.IsBundle))
					.Select(p =>
					{
						orderItemNo++;
						return p.CreateOrderItem(
							order,
							p.IsSetItem ? p.Count : p.QuantitiyUnallocatedToSet,
							p.IsSetItem ? p.CountSingle : p.QuantitiyUnallocatedToSet,
							1,
							orderItemNo,
							null,
							null);
					}).ToArray());
			}
			// ギフト注文？
			else
			{
				var allocatedToSetAndShippingProducts = new List<AllocatedToSetAndShippingProduct>();

				// セットプロモーションあり？
				if (this.SetPromotions.Items.Any())
				{
					var orderShippingNo = 0;
					var cartProducts = new List<AllocatedToSetAndShippingProduct>();
					foreach (var shipping in this.Shippings)
					{
						cartProducts.AddRange(shipping.ProductCounts
							.Select((p, index) => new AllocatedToSetAndShippingProduct { Product = p.Product, OrderShippingNo = orderShippingNo })
							.ToArray());
						orderShippingNo++;
					}
					foreach (var product in this.Items)
					{
						var targetCartProducts = cartProducts.Where(p => p.Product == product).ToArray();
						var index = 0;

						product.QuantityAllocatedToSet.Cast<KeyValuePair<int, int>>().ToList()
							.ForEach(setpromotionItem =>
							{
								targetCartProducts[index].OrderSetpromotionNo = setpromotionItem.Key;
								index++;
							});
						allocatedToSetAndShippingProducts.AddRange(
							targetCartProducts.Select(p => new
								{
									Product = p.Product,
									Key = p.OrderShippingNo + "," + (p.OrderSetpromotionNo.HasValue ? p.OrderSetpromotionNo.Value.ToString() : string.Empty)
								})
								.GroupBy(p => p.Key)
								.Select(p => new AllocatedToSetAndShippingProduct
								{
									Product = product,
									OrderShippingNo = int.Parse(p.Key.Split(',')[0]),
									OrderSetpromotionNo = (string.IsNullOrEmpty(p.Key.Split(',')[1]) == false) ? int.Parse(p.Key.Split(',')[1]) : (int?)null,
									ItemQuantity = p.ToList().Count
								}).ToArray());
					}

				}
				else
				{
					var orderShippingNo = 1;
					foreach (var shipping in this.Shippings)
					{
						allocatedToSetAndShippingProducts.AddRange(shipping.ProductCounts
							.Select(s => new AllocatedToSetAndShippingProduct
							{
								Product = s.Product,
								OrderShippingNo = orderShippingNo,
								OrderSetpromotionNo = null,
								ItemQuantity = s.Count
							}).ToArray());
						orderShippingNo++;
					}
				}

				// 通常商品
				var orderItemNo = 0;
				orderItems.AddRange(allocatedToSetAndShippingProducts
					.Where(p => (p.OrderSetpromotionNo.HasValue == false))
					.Select(p =>
					{
						orderItemNo++;
						return p.Product.CreateOrderItem(
							order,
							p.ItemQuantity,
							p.ItemQuantity,
							p.OrderShippingNo,
							orderItemNo,
							null,
							null);
					}).ToArray());

				// セットプロモーション商品
				foreach (var setpromotion in this.SetPromotions.Items)
				{
					var orderSetpromotionItemNo = 0;
					orderItems.AddRange(allocatedToSetAndShippingProducts
						.Where(p => (p.OrderSetpromotionNo.GetValueOrDefault(-1) == setpromotion.CartSetPromotionNo))
						.Select(p =>
						{
							orderItemNo++;
							orderSetpromotionItemNo++;
							return p.Product.CreateOrderItem(
								order,
								p.ItemQuantity,
								p.ItemQuantity,
								p.OrderShippingNo,
								orderItemNo,
								setpromotion.CartSetPromotionNo,
								orderSetpromotionItemNo);
						}).ToArray());
				}
			}

			// 商品連番セット
			var orderItemIndex = 1;
			foreach (var orderItemsGroupByOrderShippingNo in orderItems.GroupBy(i => i.OrderShippingNo).ToArray())
			{
				foreach (var item in orderItemsGroupByOrderShippingNo.ToArray())
				{
					item.OrderItemNo = orderItemIndex;
					orderItemIndex++;
				}
			}

			return orderItems.ToArray();
		}

		/// <summary>
		/// 注文セットプロモーション情報生成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>注文セットプロモーション情報</returns>
		private OrderSetPromotionModel[] CreateOrderSetPromotions(OrderModel order)
		{
			var orderSetPromotion = this.SetPromotions.Items.Select(cartSetPromotion =>
				new OrderSetPromotionModel
				{
					OrderId = order.OrderId,
					OrderSetpromotionNo = cartSetPromotion.CartSetPromotionNo,
					SetpromotionId = cartSetPromotion.SetpromotionId,
					SetpromotionName = cartSetPromotion.SetpromotionName,
					SetpromotionDispName = (order.IsMobileOrder && (string.IsNullOrEmpty(cartSetPromotion.SetpromotionDispNameMobile) == false))
						? cartSetPromotion.SetpromotionDispNameMobile
						: cartSetPromotion.SetpromotionDispName,
					UndiscountedProductSubtotal = cartSetPromotion.UndiscountedProductSubtotal,
					ProductDiscountFlg = cartSetPromotion.ProductDiscountFlg,
					ProductDiscountAmount = cartSetPromotion.ProductDiscountAmount,
					ShippingChargeFreeFlg = cartSetPromotion.ShippingChargeFreeFlg,
					ShippingChargeDiscountAmount = cartSetPromotion.ShippingChargeDiscountAmount,
					PaymentChargeFreeFlg = cartSetPromotion.PaymentChargeFreeFlg,
					PaymentChargeDiscountAmount = 0
				}).ToArray();

			// 注文情報から配送料・決済手数料をセット
			if (orderSetPromotion.Any(setpromotion => setpromotion.IsDiscountTypeShippingChargeFree && setpromotion.ShippingChargeDiscountAmount == 0))
			{
				orderSetPromotion.First(setpromotion => setpromotion.IsDiscountTypeShippingChargeFree).ShippingChargeDiscountAmount = order.OrderPriceShipping;
			}
			if (orderSetPromotion.Any(setpromotion => setpromotion.IsDiscountTypePaymentChargeFree))
			{
				orderSetPromotion.First(setpromotion => setpromotion.IsDiscountTypePaymentChargeFree).PaymentChargeDiscountAmount = order.OrderPriceExchange;
			}

			return orderSetPromotion;
		}

		/// <summary>
		/// Is Update FixedPurchase Shipping Pattern
		/// </summary>
		/// <param name="cartProducts">List Cart Product</param>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="fixedPurchaseSetting">定期購入設定</param>
		/// <returns>パターンをセットしたか</returns>
		/// <remarks>設定が無効の場合は定期配送パターン初期化</remarks>
		public bool IsUpdateFixedPurchaseShippingPattern(
			List<CartProduct> cartProducts,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting)
		{
			foreach (var cartProduct in cartProducts)
			{
				if (ValidateFixedPurchaseShippingPattern(
					cartProduct,
					fixedPurchaseKbn,
					fixedPurchaseSetting) == false) return false;
			}
			var shipping = new ShopShippingService().Get(this.Items[0].ShopId, this.ShippingType);
			UpdateFixedPurchaseSettingAndNextShippingDates(
				shipping,
				fixedPurchaseKbn,
				fixedPurchaseSetting);
			return true;
		}

		/// <summary>
		/// 定期配送パターンと次回・次々回配送日を設定
		/// </summary>
		/// <param name="shopShipping">配送種別マスタ</param>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="fixedPurchaseSetting">定期購入設定</param>
		public void UpdateFixedPurchaseSettingAndNextShippingDates(
			ShopShippingModel shopShipping,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting)
		{
			var service = new FixedPurchaseService();
			foreach (var shipping in this.Shippings)
			{
				shipping.UpdateFixedPurchaseSetting(
					fixedPurchaseKbn,
					fixedPurchaseSetting,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan);

				// 次回・次々回配送日計算
				var calculateMode = service.GetCalculationMode(
					fixedPurchaseKbn,
					Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
				var nextShipDate = service.CalculateFollowingShippingDate(
					fixedPurchaseKbn,
					fixedPurchaseSetting,
					DateTime.Now,
					shopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);

				var nextNextShipDate = service.CalculateNextNextShippingDate(
					fixedPurchaseKbn,
					fixedPurchaseSetting,
					shipping.NextNextShippingDate,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);

				// 次回・次々回配送日の設定
				shipping.UpdateNextShippingDates(
					nextShipDate,
					nextNextShipDate);
			}
		}

		/// <summary>
		/// 設定されている定期配送パターンは全ての商品で適正か
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <return>カート商品の設定されている定期配送パターンは適正か</return>
		public bool ValidateFixedPurchaseShippingPattern(CartProduct cartProduct)
		{
			return ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false) || (this.HasFixedPurchase == false))
				? true
				: ValidateFixedPurchaseShippingPattern(cartProduct, GetShipping().FixedPurchaseKbn, GetShipping().FixedPurchaseSetting);
		}
		/// <summary>
		/// カート商品の設定されている定期配送パターンは適正か
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <param name="fixedPurchaseKbn">定期配送区分</param>
		/// <param name="fixedPurchaseSetting">定期配送設定</param>
		/// <return>カート商品の設定されている定期配送パターンは適正か</return>
		private bool ValidateFixedPurchaseShippingPattern(CartProduct cartProduct, string fixedPurchaseKbn, string fixedPurchaseSetting)
		{
			// 定期商品ではない場合はチェックしない
			if (cartProduct.IsFixedPurchase == false) return true;

			switch (fixedPurchaseKbn)
			{
				// 月間隔日付指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					if ((fixedPurchaseSetting.Split(',').Length > 1)
						&& (ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST)
							.Any(date => date.Value == fixedPurchaseSetting.Split(',')[1]) == false)) return false;
					return OrderCommon.IsEffectiveFixedPurchaseSetting(
						new string[] { cartProduct.LimitedFixedPurchaseKbn1Setting },
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING,
						this.ShippingType,
						fixedPurchaseSetting);

				// 週・曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					// 異常値でないかのみチェック
					if ((fixedPurchaseSetting.Split(',').Length > 2)
						&& (ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST)
							.Any(week => week.Value == fixedPurchaseSetting.Split(',')[1]) == false)) return false;
					if ((fixedPurchaseSetting.Split(',').Length > 2)
						&& (ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST)
							.Any(day => day.Value == fixedPurchaseSetting.Split(',')[2]) == false)) return false;
					return OrderCommon.IsEffectiveFixedPurchaseSetting(
						new string[] { cartProduct.LimitedFixedPurchaseKbn1Setting },
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING,
						this.ShippingType,
						fixedPurchaseSetting);

				// 配送日間隔指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					return OrderCommon.IsEffectiveFixedPurchaseSetting(
						new string[] { cartProduct.LimitedFixedPurchaseKbn3Setting },
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING,
						this.ShippingType,
						fixedPurchaseSetting);

				// 週間隔・曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					if ((fixedPurchaseSetting.Split(',').Length > 1) == false) return false;
					var isValidSettingDaysOfWeek = ValueText.GetValueItemArray(
							Constants.TABLE_SHOPSHIPPING,
							Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST)
						.Any(day => (day.Value == fixedPurchaseSetting.Split(',')[1]));
					var isValidfixedPurchaseShippingPatern = isValidSettingDaysOfWeek
						? OrderCommon.IsEffectiveFixedPurchaseSetting(
							new[] { cartProduct.LimitedFixedPurchaseKbn4Setting },
							Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1,
							this.ShippingType,
							fixedPurchaseSetting)
						: false;
					return isValidfixedPurchaseShippingPatern;

				default:
					return false;
			}
		}

		/// <summary>
		/// 税額を計算
		/// </summary>
		/// <param name="taxExcludedFractionRounding">税率丸め方法</param>
		public void CalculateTaxPrice(string taxExcludedFractionRounding = "")
		{
			if (string.IsNullOrEmpty(taxExcludedFractionRounding)) taxExcludedFractionRounding = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
			var priceByTaxRate = new List<Hashtable>();
			var orderPriceSubtotalTax = 0m;
			// 税率毎の購入金額を算出する
			if (this.HasUnAllocatedProductToShipping == false)
			{
				priceByTaxRate.AddRange(this.Shippings.SelectMany(shipping => shipping.ProductCounts, (value, productCount) => new { IsDutyFree = value.IsDutyFree, pc = productCount })
					.GroupBy(productInfo => new
					{
						TaxRate = productInfo.pc.Product.TaxRate,
						IsDutyFree = productInfo.IsDutyFree
					})
					.Select(groupedInfo => new Hashtable
					{
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, groupedInfo.Key.TaxRate },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE ,
							(groupedInfo.Sum(productInfo => productInfo.pc.PriceSubtotalAfterDistributionAndRegulation)) },
						{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , groupedInfo.Key.IsDutyFree
							? 0m
							: groupedInfo.Sum(productInfo => productInfo.pc.PriceSubtotalAfterDistributionAndRegulation)},
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
					}).ToList());
				orderPriceSubtotalTax = this.Shippings.Where(shipping => (shipping.IsDutyFree == false))
					.SelectMany(shipping => shipping.ProductCounts).Sum(pc => pc.ItemPriceTax);
			}
			else
			{
				// 定額頒布会商品以外で計算
				var fixedAmountItemsExcluded = this.Items
					.Where(item => item.IsSubscriptionBoxFixedAmount() == false)
					.ToArray();
				priceByTaxRate.AddRange(
					fixedAmountItemsExcluded
						.GroupBy(
							item => new
							{
								TaxRate = item.TaxRate
							})
						.Select(
							groupedInfo => new Hashtable
							{
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , groupedInfo.Key.TaxRate },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE ,
									(groupedInfo.Sum(item => item.PriceSubtotalAfterDistributionAndRegulation)) },
								{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , this.Shippings[0].IsDutyFree
									? 0m
									: groupedInfo.Sum(item => item.PriceSubtotalAfterDistributionAndRegulation) },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
							})
						.ToList());
				orderPriceSubtotalTax = this.Shippings[0].IsDutyFree
					? 0m
					: fixedAmountItemsExcluded.Sum(item => item.PriceSubtotalTax);

				// 頒布会定額コース商品のみで計算
				if (this.HasSubscriptionBoxFixedAmountItem)
				{
					var itemsGroupByFixedAmountCourse = this.Items
						.Where(item => item.IsSubscriptionBoxFixedAmount())
						.GroupBy(item => item.SubscriptionBoxCourseId);
					foreach (var courseGroup in itemsGroupByFixedAmountCourse)
					{
						var subscriptionBox = DataCacheControllerFacade.GetSubscriptionBoxCacheController().Get(courseGroup.Key);
						var taxRate = DomainFacade.Instance.ProductTaxCategoryService.Get(subscriptionBox.TaxCategoryId).TaxRate;
						var fixedAmount = this.SubscriptionBoxFixedAmountList
							.First(subscriptionBoxFixedAmount => subscriptionBoxFixedAmount.SubscriptionBoxCourseId == courseGroup.Key)
							.PriceSubtotalAfterDistributionAndRegulation;

						priceByTaxRate.Add(
							new Hashtable
							{
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, taxRate },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, fixedAmount },
								{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE, fixedAmount },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
								{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
							});

						orderPriceSubtotalTax += TaxCalculationUtility.GetTaxPrice(
							fixedAmount,
							taxRate,
							Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
					}
				}
			}

			priceByTaxRate.Add(new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.ShippingTaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
					{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE ,this.ShippingPriceWithDiscount },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
				});
			priceByTaxRate.Add(new Hashtable
				{ 
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.PaymentTaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
					{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE ,this.PaymentPriceWithDiscount },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
				});
			priceByTaxRate.AddRange(
				this.PriceInfoByTaxRate.Select(
					priceInfoByTaxRate => new Hashtable
					{
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, priceInfoByTaxRate.TaxRate },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, 0m },
						{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , this.Shippings[0].IsDutyFree
							? 0m
							: priceInfoByTaxRate.ReturnPriceCorrection },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, priceInfoByTaxRate.ReturnPriceCorrection },
					}));
			this.PriceInfoByTaxRate.Clear();
			this.PriceInfoByTaxRate.AddRange(priceByTaxRate
				.GroupBy(price => new
				{
					taxRate = price[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
				})
				.Select(item =>
					new CartPriceInfoByTaxRate
					{
						OrderId = this.OrderId,
						TaxRate = (decimal)item.Key.taxRate,
						PriceSubtotal = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
						PriceShipping = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
						PricePayment = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
						ReturnPriceCorrection = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE]),
						PriceTotal = item.Sum(itemKey =>
							((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE])),
						TaxPrice = TaxCalculationUtility.GetTaxPrice(item.Sum(itemKey =>
							((decimal)itemKey[TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
							(decimal)item.Key.taxRate,
							taxExcludedFractionRounding,
							true)
					}));

			this.PriceTax = this.PriceInfoByTaxRate.Sum(price => price.TaxPrice);
			this.PriceSubtotalTax = orderPriceSubtotalTax;
		}

		/// <summary>
		/// Calculate scheduled shipping date
		/// </summary>
		/// <param name="isDefaultShipping">Is default shipping</param>
		/// <param name="isOrderCombined">注文同梱有無</param>
		public void CalculateScheduledShippingDate(bool isDefaultShipping, bool isOrderCombined = false)
		{
			if(isOrderCombined) return;
			foreach (var shipping in this.Shippings)
			{
				var isoCode = string.Empty;
				var prefecture = string.Empty;
				var zip = string.Empty;

				if (isDefaultShipping)
				{
					prefecture = Constants.TW_COUNTRY_SHIPPING_ENABLE
						? Constants.CONST_DEFAULT_SHIPPING_ADDRESS2_TW
						: Constants.CONST_DEFAULT_SHIPPING_ADDR1;
				}
				else
				{
					isoCode = shipping.ShippingCountryIsoCode;
					prefecture = shipping.IsTaiwanCountryShippingEnable
						? shipping.Addr2
						: shipping.Addr1;
					zip = shipping.Zip1 + shipping.Zip2;
				}

				shipping.CalculateScheduledShippingDate(this.ShopId, isoCode, prefecture, zip);
			}
		}

		/// <summary>
		/// ユーザが配送方法を未確定の場合は「Cart_ShippingMethod_UnSelected_Priority」の優先度より配送方法を確定
		/// </summary>
		/// <param name="model">配送種別モデル</param>
		public void CartShippingMethodUserUnSelected(ShopShippingModel model)
		{
			foreach (var cartObjectShipping in this.Shippings)
			{
				cartObjectShipping.CartShippingShippingMethodUserUnSelected(model);
				cartObjectShipping.UpdateShippingMethod(model);
			}
		}

		#region カート商品に割引金額を按分
		/// <summary>
		/// カート商品に割引金額を按分
		/// </summary>
		/// <param name="products">対象商品</param>
		/// <param name="discountTotal">割引き金額</param>
		/// <param name="isDiscountForCampaign">キャンペーン(クーポン・ノベルティ)適用比較対象の割引か</param>
		public void ProrateDiscountPrice(List<CartProduct> products, decimal discountTotal, bool isDiscountForCampaign)
		{
			if (discountTotal == 0) return;

			var stackedDiscountAmount = 0m;

			var targetProducts = ((products != null) && products.Any())
				? products
				: this.Items;

			if (targetProducts.Any() == false) return;

			// 頒布会定額を除く商品
			var targetProductsUsePriceSubtotal = targetProducts
				.Where(product => product.IsSubscriptionBoxFixedAmount() == false)
				.ToArray();
			// 対象商品のうち、定額である頒布会のID（IDの重複なし）
			var targetCourseIds = targetProducts
				.Where(product => product.IsSubscriptionBoxFixedAmount())
				.Select(product => product.SubscriptionBoxCourseId)
				.Distinct()
				.ToArray();
			// 対象になる定額の頒布会IDと金額
			var targetSubscriptionBoxFixedAmountList = this.SubscriptionBoxFixedAmountList
				.Where(list => targetCourseIds.Contains(list.SubscriptionBoxCourseId))
				.ToArray();

			// 頒布会定額以外の商品合計金額
			var targetPriceTotalExcludedFixedAmount = targetProductsUsePriceSubtotal
				.Sum(item => item.PriceSubtotalAfterDistribution);
			// 頒布会定額のみの商品合計金額
			var targetPriceTotalFixedAmountOnly = targetSubscriptionBoxFixedAmountList
				.Sum(list => list.PriceSubtotalAfterDistribution);
			// 頒布会定額を含む対象商品の合計金額を算出
			var targetPriceTotal = targetPriceTotalExcludedFixedAmount + targetPriceTotalFixedAmountOnly;

			// 頒布会定額以外に設定
			foreach (var cp in targetProductsUsePriceSubtotal)
			{
				var discountPrice = PriceCalculator
					.GetDistributedPrice(
						discountTotal,
						cp.PriceSubtotalAfterDistribution,
						targetPriceTotal)
					.ToPriceDecimal() ?? 0m;
				if (this.HasUnAllocatedProductToShipping == false)
				{
					var stackedDiscountPriceForShippingProductCounts = 0m;
					var allProductPrice = this.Shippings
						.Sum(s => s.ProductCounts
							.Where(pc => pc.Product == cp)
							.Sum(pc => pc.PriceSubtotalAfterDistribution));
					foreach (var productCounts in this.Shippings.SelectMany(shipping => shipping.ProductCounts).Where(pc => pc.Product == cp))
					{
						var discountPriceForShippingProductCounts = PriceCalculator.GetDistributedPrice(
							discountPrice,
							productCounts.PriceSubtotalAfterDistribution,
							allProductPrice);
						stackedDiscountPriceForShippingProductCounts += discountPriceForShippingProductCounts;
						productCounts.PriceSubtotalAfterDistribution -= discountPriceForShippingProductCounts;
					}

					var fractionDiscountPriceForShippingProductCounts = discountPrice - stackedDiscountPriceForShippingProductCounts;
					var weightProduct = this.Shippings
						.SelectMany(shipping => shipping.ProductCounts)
						.OrderByDescending(pc => pc.PriceSubtotalAfterDistribution)
						.First(pc => pc.Product == cp);
					weightProduct.PriceSubtotalAfterDistribution -= fractionDiscountPriceForShippingProductCounts;
				}
				cp.PriceSubtotalAfterDistribution -= discountPrice;
				cp.PriceSubtotalAfterDistributionForCampaign -= (isDiscountForCampaign
					? discountPrice
					: 0m);
				stackedDiscountAmount += discountPrice;
			}

			// 頒布会定額に設定
			foreach (var subscriptionBox in targetSubscriptionBoxFixedAmountList)
			{
				var discountPrice = PriceCalculator
					.GetDistributedPrice(
						discountTotal,
						subscriptionBox.PriceSubtotalAfterDistribution,
						targetPriceTotal)
					.ToPriceDecimal() ?? 0m;

				subscriptionBox.PriceSubtotalAfterDistribution -= discountPrice;
				stackedDiscountAmount += discountPrice;
			}

			var fractionDiscountPrice = discountTotal - stackedDiscountAmount;
			if (this.HasUnAllocatedProductToShipping == false)
			{
				var weightProduct = this.Shippings
					.SelectMany(shipping => shipping.ProductCounts)
					.OrderByDescending(pc => pc.PriceSubtotalAfterDistribution)
					.First(pc => (targetProductsUsePriceSubtotal.Contains(pc.Product)));
				weightProduct.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
				weightProduct.Product.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
				weightProduct.Product.PriceSubtotalAfterDistributionForCampaign -= (isDiscountForCampaign
					? fractionDiscountPrice
					: 0m);
			}
			else
			{
				var weightItem = targetProductsUsePriceSubtotal
					.OrderByDescending(item => item.PriceSubtotalAfterDistribution)
					.FirstOrDefault();
				var weightSubscriptionBox = targetSubscriptionBoxFixedAmountList
					.OrderByDescending(item => item.PriceSubtotalAfterDistribution)
					.FirstOrDefault();

				// 定額以外の商品があり商品の方が金額が高いなら商品に端数を割り当てる
				if (IsAddFractionPriceToProduct(weightItem, weightSubscriptionBox))
				{
					weightItem.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
					weightItem.PriceSubtotalAfterDistributionForCampaign -= (isDiscountForCampaign
						? fractionDiscountPrice
						: 0m);
				}
				else
				{
					weightSubscriptionBox.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
				}
			}
		}
		/// <summary>
		/// カート商品に割引金額を按分(商品単位)
		/// </summary>
		/// <param name="products">対象商品</param>
		/// <param name="discountTotal">割引き金額</param>
		public void ProrateDiscountPriceToProduct(List<CartProduct> products, decimal discountTotal)
		{
			if (discountTotal == 0 || this.IsSecondFixedPurchase) return;

			var targetProducts = ((products != null) && products.Any()) ? products : this.Items;
			if (targetProducts.Count == 0) return;

			// 頒布会定額を除く商品
			var targetProductsUsePriceSubtotal = targetProducts
				.Where(product => product.IsSubscriptionBoxFixedAmount() == false)
				.ToArray();
			// 対象商品のうち、定額である頒布会のID（IDの重複なし）
			var targetCourseIds = targetProducts
				.Where(product => product.IsSubscriptionBoxFixedAmount())
				.Select(product => product.SubscriptionBoxCourseId)
				.Distinct()
				.ToArray();
			// 対象になる定額の頒布会IDと金額
			var subscriptionBoxFixedAmountList = this.SubscriptionBoxFixedAmountList
				.Where(list => targetCourseIds.Contains(list.SubscriptionBoxCourseId))
				.ToArray();

			// 頒布会定額以外の商品合計金額
			var targetPriceTotalExcludedFixedAmount = targetProductsUsePriceSubtotal
					.Sum(item => item.DiscountedPrice
						.Select(price => price.Value)
						.Sum())
				+ targetProductsUsePriceSubtotal
					.Sum(item => item.DiscountedPriceUnAllocatedToSet);
			// 頒布会定額のみの商品合計金額
			var targetPriceFixedAmountOnly = subscriptionBoxFixedAmountList
				.Sum(box => box.DiscountedPrice);
			// 頒布会定額を含む対象商品の合計金額
			var targetPriceTotal = targetPriceTotalExcludedFixedAmount + targetPriceFixedAmountOnly;

			var stackedDiscountAmount = 0m;

			var rounding = DecimalUtility.Format.RoundDown;
			switch (Constants.DISCOUNTED_PRICE_FRACTION_ROUNDING)
			{
				case Constants.FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_UP:
					rounding = DecimalUtility.Format.RoundUp;
					break;

				case Constants.FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_OFF:
					rounding = DecimalUtility.Format.Round;
					break;
			}

			// 頒布会定額以外に設定
			foreach (var cp in targetProductsUsePriceSubtotal)
			{
				foreach (var cartSetPromotion in this.SetPromotions.Items.Where(
					sp => (cp.DiscountedPrice.Any(dp => dp.Key == sp.CartSetPromotionNo))))
				{
					var discountPrice = PriceCalculator.GetDistributedPrice(
						discountTotal,
						cp.DiscountedPrice[cartSetPromotion.CartSetPromotionNo],
						targetPriceTotal).ToPriceDecimal(rounding) ?? 0m;
					cp.DiscountedPrice[cartSetPromotion.CartSetPromotionNo] -= discountPrice;
					stackedDiscountAmount += discountPrice;
				}

				if (cp.DiscountedPriceUnAllocatedToSet > 0)
				{
					var discountPrice = PriceCalculator.GetDistributedPrice(
						discountTotal,
						cp.DiscountedPriceUnAllocatedToSet,
						targetPriceTotal).ToPriceDecimal(rounding) ?? 0m;
					cp.DiscountedPriceUnAllocatedToSet -= discountPrice;
					stackedDiscountAmount += discountPrice;
				}
			}

			// 頒布会定額に設定
			foreach (var subscriptionBox in subscriptionBoxFixedAmountList)
			{
				var discountPrice = PriceCalculator.GetDistributedPrice(
					discountTotal,
					subscriptionBox.DiscountedPrice,
					targetPriceTotal).ToPriceDecimal(rounding) ?? 0m;
				subscriptionBox.DiscountedPrice -= discountPrice;
				stackedDiscountAmount += discountPrice;
			}

			var fractionDiscountPrice = discountTotal - stackedDiscountAmount;
			var weightItem = targetProductsUsePriceSubtotal
				.OrderByDescending(item => item.PriceSubtotalAfterDistribution)
				.FirstOrDefault();
			var weightSubscriptionBox = subscriptionBoxFixedAmountList
				.OrderByDescending(box => box.PriceSubtotalAfterDistribution)
				.FirstOrDefault();

			// 定額以外の商品があり商品の方が金額が高いなら商品に端数を割り当てる
			if (IsAddFractionPriceToProduct(weightItem, weightSubscriptionBox))
			{
				if (weightItem.DiscountedPriceUnAllocatedToSet > 0)
				{
					weightItem.DiscountedPriceUnAllocatedToSet -= fractionDiscountPrice;
				}
				else if (this.SetPromotions.Items.Any(
					sp => (weightItem.DiscountedPrice.Any(dp => dp.Key == sp.CartSetPromotionNo))))
				{
					var items = this.SetPromotions.Items.First(
						sp => (weightItem.DiscountedPrice.Any(dp => dp.Key == sp.CartSetPromotionNo)));
					weightItem.DiscountedPrice[items.CartSetPromotionNo] -= fractionDiscountPrice;
				}
			}
			else
			{
				weightSubscriptionBox.DiscountedPrice -= fractionDiscountPrice;
			}
		}
		#endregion

		/// <summary>
		/// Is Product Novelty Has Delete
		/// </summary>
		/// <param name="productId">Product Id</param>
		/// <param name="variationId">Variation Id</param>
		/// <param name="noveltyIdsDelete">Novelty Ids Delete</param>
		/// <param name="cartNoveltys">Cart Noveltys</param>
		/// <returns>Check Product Novelty Has Delete</returns>
		public bool IsProductNoveltyHasDelete(
			string productId,
			string variationId,
			List<string> noveltyIdsDelete,
			CartNovelty[] cartNoveltys)
		{
			if ((noveltyIdsDelete == null) || (noveltyIdsDelete.Count == 0)) return false;

			foreach (var noveltyIdDelete in noveltyIdsDelete)
			{
				var cartNovelty = cartNoveltys.FirstOrDefault(item => item.NoveltyId == noveltyIdDelete);
				if (cartNovelty == null) continue;

				if ((productId == cartNovelty.GrantItemList[0].ProductId)
					&& (variationId == cartNovelty.GrantItemList[0].VariationId)) return true;
			}

			return false;
		}

		/// <summary>
		/// 請求書同梱フラグ取得
		/// </summary>
		/// <returns>カートの情報で請求書同梱使えるかどうか判断してフラグを返す</returns>
		public string GetInvoiceBundleFlg()
		{
			if ((this.Payment == null)
				|| ((this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
				|| this.IsGift)
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			if (this.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				return GetNPAfterPayInvoiceBundleFlg();
			}

			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.Atodene:
					return GetAtodenePaymentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Gmo:
					return GetGmoPaymentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Dsk:
					return GetDskDeferredPaymentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Atobaraicom:
					return GetAtobaraicomPaymentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Score:
					return GetScoreDeferredPaymentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Veritrans:
					return GetVeritransPaymentInvoiceBundleFlg();
			}

			return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		/// <summary>
		/// Atodene後払いの請求書同梱フラグを取得
		/// </summary>
		/// <returns>カートの情報でAtodeneの請求書同梱使えるかどうか判断してフラグを返す</returns>
		public string GetAtodenePaymentInvoiceBundleFlg()
		{
			if ((Constants.PAYMENT_SETTING_ATODENE_USE_INVOICE_BUNDLE_SERVICE == false)
				|| (this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var result = CreateNewOrder().JudgmentInvoiceBundleFlg();
			return result;
		}

		/// <summary>
		/// Gmo後払いの請求書同梱フラグを取得
		/// </summary>
		/// <returns>カートの情報でGmoの請求書同梱使えるかどうか判断してフラグを返す</returns>
		private string GetGmoPaymentInvoiceBundleFlg()
		{
			if ((Constants.PAYMENT_SETTING_GMO_DEFERRED_INVOICEBUNDLE == false)
				|| (this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var ownerZip = this.Owner.Zip;
			var ownerAddr = this.Owner.ConcatenateAddressWithoutCountryName();
			var shippingZip = GetShipping().Zip;
			var shippingAddr = GetShipping().ConcatenateAddressWithoutCountryName();

			return ((shippingZip == ownerZip) && (shippingAddr == ownerAddr))
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		/// <summary>
		/// DSK後払いの請求書同梱フラグを取得
		/// </summary>
		/// <returns>カートの情報でDSK後払いの請求書同梱使えるかどうか判断してフラグを返す</returns>
		public string GetDskDeferredPaymentInvoiceBundleFlg()
		{
			if ((Constants.PAYMENT_SETTING_DSK_DEFERRED_USE_INVOICE_BUNDLE == false)
				|| (this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var result = CreateNewOrder().JudgmentInvoiceBundleFlg();
			return result;
		}

		/// <summary>
		/// スコア後払いの請求書同梱フラグを取得
		/// </summary>
		/// <returns>カートの情報でスコア後払いの請求書同梱使えるかどうか判断してフラグを返す</returns>
		public string GetScoreDeferredPaymentInvoiceBundleFlg()
		{
			if ((Constants.PAYMENT_SETTING_SCORE_DEFERRED_USE_INVOICE_BUNDLE == false)
				|| (this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var ownerAddr = this.Owner.ConcatenateAddressWithoutCountryName();
			var shippingAddr = GetShipping().ConcatenateAddressWithoutCountryName();

			return shippingAddr == ownerAddr
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		/// <summary>
		/// ベリトランス後払いの請求書同梱フラグを取得
		/// </summary>
		/// <returns>カートの情報でベリトランス後払いの請求書同梱使えるかどうか判断してフラグを返す</returns>
		public string GetVeritransPaymentInvoiceBundleFlg()
		{
			if ((Constants.PAYMENT_SETTING_VERITRANS_USE_INVOICE_BUNDLE == false)
				|| (this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var result = CreateNewOrder().JudgmentInvoiceBundleFlg();
			return result;
		}

		/// <summary>
		/// Get NP After Pay Invoice Bundle Flg
		/// </summary>
		/// <returns>Invoice Bundle Flg</returns>
		private string GetNPAfterPayInvoiceBundleFlg()
		{
			if ((Constants.PAYMENT_NP_AFTERPAY_INVOICEBUNDLE == false)
				|| (this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var ownerAddress = this.Owner.ConcatenateAddressWithoutCountryName();
			var shippingAddress = GetShipping().ConcatenateAddressWithoutCountryName();

			var invoiceBundleFlg = (shippingAddress == ownerAddress)
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			return invoiceBundleFlg;
		}

		/// <summary>
		/// Update cart id
		/// </summary>
		/// <param name="cartId">Cart id</param>
		public void UpdateCartId(string cartId)
		{
			this.CartId = cartId;
		}

		/// <summary>
		/// DBと同期（オブジェクトを正とし、DBへと格納する）
		/// </summary>
		public void SyncProductDb()
		{
			foreach (var product in this.Items)
			{
				if (product.IsSetItem)
				{
					UpdateCartDbAddProductSetItem(product);
				}
				else
				{
					UpdateCartDbAddProduct(product);
				}
			}
		}

		/// <summary>
		/// Atobaraicom後払いの請求書同梱フラグを取得
		/// </summary>
		/// <returns>カートの情報でAtobaraicomの請求書同梱使えるかどうか判断してフラグを返す</returns>
		private string GetAtobaraicomPaymentInvoiceBundleFlg()
		{
			if ((Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_BUNDLE_SERVICE == false)
				|| (this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var ownerZip = this.Owner.Zip;
			var ownerAddr = this.Owner.ConcatenateAddressWithoutCountryName();
			var shippingZip = GetShipping().Zip;
			var shippingAddr = GetShipping().ConcatenateAddressWithoutCountryName();

			return ((shippingZip == ownerZip) && (shippingAddr == ownerAddr))
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		/// <summary>
		/// 外部決済種別設定
		/// </summary>
		/// <param name="userDefaultOrderSetting">ユーザーデフォルト注文設定</param>
		public void SetDefaultExternalPaymentType(UserDefaultOrderSettingModel userDefaultOrderSetting)
		{
			if (this.Payment.PaymentId != userDefaultOrderSetting.PaymentId) return;

			// Set default external payment type for Ec Payment
			if ((userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (string.IsNullOrEmpty(this.Payment.ExternalPaymentType)))
			{
				this.Payment.ExternalPaymentType = Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT;
			}

			// Set Default External Payment Type For NewebPay
			if ((userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (string.IsNullOrEmpty(this.Payment.ExternalPaymentType)))
			{
				this.Payment.ExternalPaymentType = Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT;
			}
		}

		/// <summary>
		/// ユーザーデフォルト注文設定の決済種別利用可能チェック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userDefaultOrderSetting">ユーザーデフォルト注文設定</param>
		/// <param name="IsMultiCart">複数カート注文か</param>
		/// <returns>TRUE:デフォルト注文設定の決済種別が利用不可、FALSE:デフォルト注文設定の決済種別が利用可能</returns>
		public bool CheckDefaultOrderSettingPaymentIsValid(string userId, UserDefaultOrderSettingModel userDefaultOrderSetting, bool IsMultiCart)
		{
			if (this.Payment.PaymentId != userDefaultOrderSetting.PaymentId) return false;

			// 有効な決済種別取得
			var validPaymentList = OrderCommon.GetValidPaymentList(
				this,
				userId,
				isMultiCart: IsMultiCart);
			// 商品情報の決済利用不可以外の決済種別取得
			var validPayments = OrderCommon.GetPaymentsUnLimitByProduct(this, validPaymentList);
			var validPaymentIds = validPayments
				.Select(item => item.PaymentId).ToList();

			var isNotValidPayment = (validPaymentIds.Contains(userDefaultOrderSetting.PaymentId) == false);

			return isNotValidPayment;
		}

		/// <summary>
		/// 注文拡張項目の更新
		/// </summary>
		/// <param name="item">更新内容</param>
		public void UpdateOrderExtend(Dictionary<string, CartOrderExtendItem> item)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return;

			this.OrderExtend = item;
		}

		/// <summary>
		/// 月を計算して取得
		/// </summary>
		/// <param name="nextDay">次の日</param>
		/// <param name="monthMulti">月の倍数</param>
		/// <param name="displayCount">表示回数</param>
		/// <returns>月</returns>
		private int GetMonthToCalculate(DateTime nextDay, int monthMulti, int displayCount)
		{
			if (displayCount == 1) return nextDay.Month;

			var result = nextDay.Month + ((displayCount - 1) * monthMulti);
			return result;
		}

		/// <summary>
		/// 頒布会向け比較用日付取得
		/// </summary>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="displayCount">表示回数</param>
		/// <param name="nextDay">次の日</param>
		/// <param name="monthMulti">月の倍数</param>
		/// <param name="fixedPurchaseSetting">定期購入設定</param>
		/// <param name="year">年</param>
		/// <param name="monthNow">現在の月</param>
		/// <returns>日付</returns>
		[Obsolete("何故かSQLとの通信が発生する。同じロジックがあるはずなので製品に入れる前に廃止する。")]
		private DateTime GetDateTimeCompareForSubscriptionBox(
			string fixedPurchaseKbn,
			int displayCount,
			DateTime nextDay,
			int monthMulti,
			string fixedPurchaseSetting,
			int year,
			int monthNow)
		{
			switch (fixedPurchaseKbn)
			{
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					if (displayCount > 1)
					{
						var result = nextDay.AddMonths((displayCount - 1) * monthMulti);
						return result;
					}
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					if (displayCount > 1)
					{
						var week = fixedPurchaseSetting.Split(',')[1];
						var day = fixedPurchaseSetting.Split(',')[2];
						var dateFinal = year + "-" + monthNow + "-" + "01";

						var input = new Hashtable
						{
							{ Constants.FLG_ORDER_FIXEDPURCHASE_WEEK, week },
							{ Constants.FLG_ORDER_FIXEDPURCHASE_DAY, day },
							{ Constants.FLG_ORDER_FIXEDPURCHASE_DATETIME_COMPARE, Convert.ToDateTime(dateFinal) }
						};
						var getNextDate = DomainFacade.Instance.SubscriptionBoxService.GetNextDate(input);
						return Convert.ToDateTime(getNextDate);
					}
					break;

				default:
					{
						var day = this.Shippings.Select(t => t.FixedPurchaseSetting).FirstOrDefault();
						var result = nextDay.AddDays(Convert.ToInt16(day) * (displayCount - 1));
						return result;
					}
					break;
			}
			return nextDay;
		}

		/// <summary>
		/// 頒布会コース情報を設定
		/// </summary>
		public void SetSubscriptionBoxInformation()
		{
			if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return;
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);
			this.SubscriptionBoxDisplayName = subscriptionBox.DisplayName;
			this.SubscriptionBoxFixedAmount = subscriptionBox.FixedAmount;
		}

		/// <summary>
		/// 定期台帳に紐づく頒布会が存在するか
		/// </summary>
		/// <returns>頒布会が存在するか(紐づく頒布会がない場合はTRUE)</returns>
		public static bool CheckExistFixedPurchaseLinkSubscriptionBox(
			string subscriptionBoxCourseId,
			string fixedPurchaseId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			// そもそも定期台帳に頒布会が紐づいていない場合
			if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return true;

			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);
			if (subscriptionBox != null) return true;

			// 定期台帳に紐づく頒布会がない場合は定期購入ステータスをその他エラーにする。
			new FixedPurchaseService().UpdateForFailedOrder(
					fixedPurchaseId,
					lastChanged,
					UpdateHistoryAction.Insert,
					accessor);
			return false;
		}

		/// <summary>
		/// CrossPointリクエスト用の商品明細を取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>商品明細</returns>
		/// <remarks>
		/// 商品毎に按分すると1円誤差が発生するため、按分金額を別フィールドとして連携する
		/// </remarks>
		public static OrderDetail[] GetOrderDetails(CartObject cart)
		{
			var details = cart.Items
				.Select(
					product =>
					{
						var productPrice = Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED ? product.PriceIncludedOptionPrice : product.Price;
						var priceTax = TaxCalculationUtility.GetTaxPrice(
							productPrice,
							product.TaxRate,
							product.TaxRoundType,
							(product.TaxIncludedFlg == Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_ON));

						var cooperationIdExist = (Constants.CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO != 0)
							&& (string.IsNullOrEmpty(product.CooperationId[(Constants.CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO - 1)]) == false);

						var price = (product.TaxIncludedFlg == Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_ON)
							? (productPrice - (product.PriceTax + product.OptionPriceTax))
							: productPrice;

						var detail = new OrderDetail
						{
							JanCode = cooperationIdExist ? product.CooperationId[(Constants.CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO - 1)] : Constants.CROSS_POINT_DUMMY_JANCODE,
							ProductName = product.ProductJointName,
							ProductId = product.VariationId,
							Price = price,
							SalesPrice = price,
							Quantity = product.Count,
							Tax = (priceTax * product.Count),
							ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_PRODUCT,
						};
						return detail;
					})
				.Concat(CrossPointUtility.GetSellingCosts(cart))
				.Where(detail => ((detail.JanCode != Constants.CROSS_POINT_DUMMY_JANCODE) || (detail.SalesPrice != 0m)))
				.ToArray();

			return details;
		}

		/// <summary>
		/// CrossPointリクエスト用の商品明細を取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>商品明細</returns>
		/// <remarks>
		/// 商品毎に按分すると1円誤差が発生するため、按分金額を別フィールドとして連携する
		/// </remarks>
		/// <returns>リクエスト用の商品詳細</returns>
		public static OrderDetail[] GetOrderDetails(OrderModel order)
		{
			var details = order.Items
				.Select(
					product =>
					{
						var priceTax = TaxCalculationUtility.GetTaxPrice(
							product.ProductPrice,
							product.ProductTaxRate,
							product.ProductTaxRoundType,
							(product.ProductTaxIncludedFlg == Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_ON));

						var cooperationIdExist = (Constants.CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO != 0)
							&& (string.IsNullOrEmpty(product.CooperationIdList[(Constants.CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO - 1)]) == false);

						var price = (product.ProductTaxIncludedFlg == Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_ON)
							? (product.ProductPrice - priceTax)
							: product.ProductPrice;

						var detail = new OrderDetail
						{
							JanCode = cooperationIdExist ? product.CooperationIdList[(Constants.CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO - 1)] : Constants.CROSS_POINT_DUMMY_JANCODE,
							ProductName = product.ProductName,
							ProductId = product.VariationId,
							Price = price,
							SalesPrice = price,
							Quantity = product.ItemQuantity,
							Tax = (priceTax * product.ItemQuantity),
							ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_PRODUCT,
						};
						return detail;
					})
				.Concat(CrossPointUtility.GetSellingCosts(order))
				.Where(detail => ((detail.JanCode != Constants.CROSS_POINT_DUMMY_JANCODE) || (detail.SalesPrice != 0m)))
				.ToArray();

			return details;
		}

		/// <summary>
		/// 頒布会商品を除外する
		/// </summary>
		/// <remarks>
		/// 定期注文の2回目以降配送情報表示時に、頒布会が注文同梱されていると一緒に商品が表示、計算されてしまうため、<br/>
		/// 頒布会商品を除外するために利用。<br/>
		/// （頒布会と定期の注文同梱時、2回目以降はそれぞれ別の台帳を持つため、一緒に表示されないようにしている。）
		/// </remarks>
		public void ExcludeSubscriptionBoxItem()
		{
			var items = this.Items.Where(item => item.IsSubscriptionBox == false).ToArray();
			this.Items.Clear();
			AddProductsVirtual(items, execCalculate: false);
		}

		/// <summary>
		/// カートの頒布会コースの頒布会商品のみをセットする
		/// </summary>
		/// <remarks>
		/// 頒布会注文の2回目以降配送情報表示時に、異なるコースの頒布会が注文同梱されていると一緒に商品が表示、計算されてしまうため、<br/>
		/// 同梱子注文の（カートにセットされる）頒布会コースの商品のみセットするために利用。<br/>
		/// （異なる頒布会コースでの注文同梱時、2回目以降はそれぞれ別の台帳を持つため、一緒に表示されないようにしている。）
		/// </remarks>
		public void SetCartSubscriptionBoxCourseItem()
		{
			var items = this.Items
				.Where(item => item.SubscriptionBoxCourseId == this.SubscriptionBoxCourseId)
				.ToArray();
			this.Items.Clear();
			AddProductsVirtual(items, execCalculate: false);
		}

		/// <summary>
		/// 定期台帳に登録する商品を取得
		/// </summary>
		/// <returns>定期台帳に登録する商品</returns>
		public CartProduct[] GetItemsRegisteredFixedPurchase()
		{
			// 注文同梱時に新規頒布会を登録する場合は、登録するコースの商品のみを取得
			if (this.IsShouldRegistSubscriptionForCombine)
			{
				var subscriptionBoxItemsRegistered = this.Items
					.Where(item => item.SubscriptionBoxCourseId == this.SubscriptionBoxCourseId)
					.ToArray();
				return subscriptionBoxItemsRegistered;
			}

			// 注文同梱時に頒布会ではない新規定期台帳を登録する場合は、頒布会ではない定期商品のみを取得
			if (this.IsBeforeCombineCartHasFixedPurchase)
			{
				var fixedPurchaseItemsRegistered = this.Items
					.Where(item => item.IsFixedPurchase && (item.IsSubscriptionBox == false))
					.ToArray();
				return fixedPurchaseItemsRegistered;
			}

			var items = this.Items
				.Where(item => item.IsFixedPurchase || item.IsSubscriptionBox)
				.ToArray();
			return items;
		}

		/// <summary>
		/// 1種類の頒布会定額コースの商品のみが含まれるか
		/// </summary>
		/// <remarks>通常の定額頒布会 or 同コース同梱の定額頒布会であるかを確認</remarks>
		/// <returns>1種類の頒布会定額コースの商品のみが含まれるであればTRUE</returns>
		public bool HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
		{
			// 注文同梱であれば、定額頒布会以外が含まれていないかつ同コース頒布会での同梱ならTRUE
			var fixedAmountCourseIdCount = this.Items
				.Where(item => item.IsSubscriptionBoxFixedAmount())
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Count();
			return (fixedAmountCourseIdCount == 1) && this.IsAllItemsSubscriptionBoxFixedAmount;
		}

		/// <summary>
		/// 同コースでの注文同梱したカートに対して、頒布会定額コースの制限チェックを行う
		/// </summary>
		/// <remarks>
		/// コース制限チェックでエラーがあった場合、<br/>
		/// SubscriptionBoxErrorMsg プロパティにエラーメッセージを保持する。
		/// </remarks>
		/// <returns>チェック対象ではない or チェックでエラーが無ければTRUE</returns>
		public bool CheckFixedAmountForCombineWithSameCourse()
		{
			// 頒布会定額コース商品が含まれない、または同コースでの注文同梱ではない場合は、チェックを行わずTRUEとして返す
			if ((this.HasSubscriptionBoxFixedAmountItem == false)
				|| (IsOrderCombinedWithSameSubscriptionBoxCourse() == false)) return true;

			var isSuccessFixedAmountCheck = CheckSubscriptionBoxFixedAmount();
			return isSuccessFixedAmountCheck;
		}

		/// <summary>
		/// 同じ頒布会コースでの注文同梱か
		/// </summary>
		/// <returns>同じ頒布会コース同士の注文同梱であればTRUE</returns>
		public bool IsOrderCombinedWithSameSubscriptionBoxCourse()
		{
			if (this.IsOrderCombined == false) return false;

			// 頒布会商品以外が含まれていればFALSE
			var hasItemsNotSubscriptionBox = this.Items.Any(item => item.IsSubscriptionBox == false);
			if (hasItemsNotSubscriptionBox) return false;

			var fixedAmountCourseIdCount = this.Items
				.Where(item => item.IsSubscriptionBox)
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Count();
			return fixedAmountCourseIdCount == 1;
		}

		/// <summary>
		/// 頒布会定額コースの数量・商品種類数・金額チェックを行う
		/// </summary>
		/// <remarks>エラーがあった場合、SubscriptionBoxErrorMsg プロパティにエラーメッセージを保持する。</remarks>
		/// <returns>エラーが無ければTRUE</returns>
		public bool CheckSubscriptionBoxFixedAmount()
		{
			var errorMessage = new StringBuilder();
			var fixedAmountCourseGroup = this.Items
				.Where(item => item.IsSubscriptionBoxFixedAmount())
				.GroupBy(item => item.SubscriptionBoxCourseId);
			foreach (var courseGroup in fixedAmountCourseGroup)
			{
				// 数量チェック
				var itemQuantityTotal = courseGroup.Sum(item => item.Count);
				var errorMessageForLimitProduct =
					OrderCommon.CheckLimitProductOrderForSubscriptionBox(courseGroup.Key, itemQuantityTotal);
				if (string.IsNullOrEmpty(errorMessageForLimitProduct) == false)
				{
					errorMessage.AppendLine(errorMessageForLimitProduct);
				}

				// 商品種類数チェック
				var errorMessageForProductCount = OrderCommon.GetSubscriptionBoxProductOfNumberError(
					courseGroup.Key,
					courseGroup.Count(),
					true);
				if (string.IsNullOrEmpty(errorMessageForProductCount) == false)
				{
					errorMessage.AppendLine(errorMessageForProductCount);
				}

				// 合計金額チェック
				var itemPriceSubtotal = courseGroup.Sum(item => item.PriceSubtotal);
				var errorMessageForTotalAmount = OrderCommon.GetSubscriptionBoxTotalAmountError(
					courseGroup.Key,
					itemPriceSubtotal);
				if (string.IsNullOrEmpty(errorMessageForTotalAmount) == false)
				{
					errorMessage.AppendLine(errorMessageForTotalAmount);
				}
			}

			if (string.IsNullOrEmpty(errorMessage.ToString()) == false)
			{
				this.SubscriptionBoxErrorMsg = errorMessage.ToString();
				return false;
			}

			return true;
		}

		/// <summary>
		/// 割引や調整金額の計算で端数があり頒布会定額商品が含まれている場合、商品の方に端数を割り当てるかどうか
		/// </summary>
		/// <param name="weightItem">一番金額の高い商品</param>
		/// <param name="weightBox">一番金額の高い頒布会（定額）</param>
		/// <returns>商品に割り当てる場合true</returns>
		private bool IsAddFractionPriceToProduct(CartProduct weightItem, CartSubscriptionBoxFixedAmount weightBox)
		{
			var hasItem = weightItem != null;
			var hasSubscriptionBox = weightBox != null;
			var isItemPriceHigher = hasItem
				&& hasSubscriptionBox
				&& (weightItem.PriceSubtotalAfterDistribution > weightBox.PriceSubtotalAfterDistribution);
			var result = (hasItem && (isItemPriceHigher || (hasSubscriptionBox == false)));
			return result;
		}

		/// <summary>
		/// Update cart shippings address
		/// </summary>
		/// <param name="cartShippings">List cart shipping</param>
		public void UpdateCartShippingsAddr(List<CartShipping> cartShippings)
		{
			if (this.Shippings.Count != cartShippings.Count) return;

			foreach (var item in cartShippings.Select((value, index) => new { index, value }))
			{
				this.Shippings[item.index].UpdateShippingAddr(item.value);
				this.Shippings[item.index].UpdateSenderAddr(item.value, item.value.IsSameSenderAsShipping1);
			}
		}

		/// <summary>
		/// 配送料無料時の請求料金計算
		/// </summary>
		private void CalculateFreeShippingFee()
		{
			var shippings = GetShippings();
			foreach (var shipping in shippings)
			{
				shipping.IsUseFreeShippingFee = false;

				if ((Constants.FREE_SHIPPING_FEE_OPTION_ENABLED == false)
					|| (shipping.PriceShipping != 0)) return;

				var productCountExcludeFreeShipping = shipping.ProductCounts
					.Where(shippingProduct => shippingProduct.Product.ExcludeFreeShippingFlg == Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID)
					.Sum(shippingProduct => shippingProduct.Count);

				if (productCountExcludeFreeShipping == 0) return;

				// 配送料複数個無料商品個数
				var pluralShippingPriceFreeCount = shipping.ProductCounts
					.Where(
						cartProduct =>
							(cartProduct.Product.IsPluralShippingPriceFree == Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_VALID)
								&& (cartProduct.Count > 1))
					.Sum(cartProduct => cartProduct.Count - 1);

				if ((shipping.FreeShippingFee == 0)
					|| (shipping.ShippingZoneNo > CartShipping.MAX_SHIPPING_ZONE_NO)) return;

				var freeShippingFee = 0m;
				switch (shipping.CalculationPluralKbn)
				{
					case Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE:
						freeShippingFee = shipping.FreeShippingFee * (productCountExcludeFreeShipping - pluralShippingPriceFreeCount);
						break;

					case Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE:
						freeShippingFee = shipping.FreeShippingFee + (shipping.PluralShippingPrice * (productCountExcludeFreeShipping - 1));
						break;

					default:
						freeShippingFee = shipping.FreeShippingFee;
						break;
				}
				shipping.IsUseFreeShippingFee = true;

				var memberRankDetail = MemberRankOptionUtility.GetMemberRankList()
					.FirstOrDefault(memberRank => memberRank.MemberRankId == this.MemberRankId);
				if ((Constants.MEMBER_RANK_OPTION_ENABLED == false) || (memberRankDetail == null))
				{
					memberRankDetail = new MemberRankModel
					{
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						ShippingDiscountValue = null,
					};
				}

				// クーポンまたは、セットプロモーションまたは、会員ランク割引による配送料無料割引があるか
				var isFreeShipping = ((this.Coupon != null)
					&& (this.Coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
					|| this.SetPromotions.Items
						.Any(cartSetPromotion => cartSetPromotion.IsDiscountTypeShippingChargeFree)
					|| (memberRankDetail.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE);

				shipping.PriceShipping = isFreeShipping
					? shipping.PriceShipping + freeShippingFee
					: freeShippingFee;
			}
		}

		/// <summary>カートID</summary>
		public string CartId { get; private set; }
		/// <summary>カートユーザID（カートテーブルに紐付く）</summary>
		public string CartUserId { get; set; }
		/// <summary>クーポン向けユーザID</summary>
		/// <remarks>
		/// クーポン取得処理では会員向けかをuser_idの存在で判断している箇所があるが、
		/// 未登録会員（オフライン会員）の注文時に不都合があるためカートに一度登録されたクーポン情報の再取得については
		/// dummyというユーザーIDで取得するようにする。（これを利用しないと会員対象クーポンが取得出来ずシステムエラーとなる）
		/// </remarks>
		private string CouponUserId
		{
			get
			{
				var result = string.IsNullOrEmpty(m_couponUserId)
					? (((string.IsNullOrEmpty(this.CartUserId) && (this.Owner != null)
						&& UserService.IsUser(this.Owner.OwnerKbn))
						? CouponOptionUtility.DUMMY_USER_ID_FOR_USER_COUPON_CHECK
						: this.CartUserId))
					: m_couponUserId;
				return result;
			}
			set
			{
				m_couponUserId = value;
			}
		}
		/// <summary>クーポン向けユーザIDメンバー変数</summary>
		private string m_couponUserId = string.Empty;
		/// <summary>注文ユーザID（注文時のユーザーID。ゲストも含む）</summary>
		public string OrderUserId { get; set; }
		/// <summary>会員ランクID</summary>
		public string MemberRankId { get; set; }
		/// <summary>ユーザーDBから取得した会員ランクIDであるか</summary>
		public bool IsMemberRankIdFromDb { get; private set; }
		/// <summary>受注ID</summary>
		public string OrderId { get; set; }
		/// <summary>注文区分</summary>
		public string OrderKbn { get; private set; }
		/// <summary>店舗ID</summary>
		public string ShopId { get; private set; }
		/// <summary>配送種別 ＆ カート分割基準1</summary>
		public string ShippingType { get; set; }
		/// <summary>デジタルコンテンツ専用カートか ＆ カート分割基準2</summary>
		public bool IsDigitalContentsOnly { get; private set; }
		/// <summary>商品リスト</summary>
		public List<CartProduct> Items { get; private set; }
		/// <summary>税率毎の商品価格情報リスト</summary>
		public List<CartPriceInfoByTaxRate> PriceInfoByTaxRate { get; private set; }
		/// <summary>商品合計金額</summary>
		public decimal PriceSubtotal { get; private set; }
		/// <summary>配送料金</summary>
		public decimal PriceShipping
		{
			get
			{
				var result = (m_shippingPrice < 0)
					? MemberRankOptionUtility.GetMemberRankPriceShipping(
						this.MemberRankId,
						this.Shippings.Sum(shipping => shipping.PriceShipping))
					: m_shippingPrice;
				return result;
			}
		}
		/// <summary>調整金額</summary>
		public decimal PriceRegulation { get; set; }
		/// <summary>調整金額(按分した商品小計分)</summary>
		public decimal ItemPriceRegulation { get; set; }
		/// <summary>調整金額(按分した手数料分)</summary>
		public decimal ServicePriceRegulation { get; set; }
		/// <summary>調整金額(返品用金額補正含む)</summary>
		public decimal PriceRegulationTotal
		{
			get
			{
				return this.PriceRegulation
					+ this.PriceInfoByTaxRate.Sum(priceByTaxRate => priceByTaxRate.ReturnPriceCorrection);
			}
		}
		public string RegulationMemo { get; set; }
		/// <summary>税額総計</summary>
		public decimal PriceTax { get; set; }
		/// <summary>商品消費税額小計</summary>
		public decimal PriceSubtotalTax { get; set; }
		/// <summary>配送料割引額</summary>
		public decimal ShippingPriceDiscountAmount { get; set; }
		/// <summary>決済手数料割引額</summary>
		public decimal PaymentPriceDiscountAmount { get; set; }
		/// <summary>決済手数料</summary>
		public decimal PaymentPrice
		{
			get
			{
				return this.Payment.PriceExchange;
			}
		}
		/// <summary>配送料税率</summary>
		public decimal ShippingTaxRate { get; set; }
		/// <summary>決済手数料税率</summary>
		public decimal PaymentTaxRate { get; set; }
		/// <summary>カート支払い金額合計（商品合計＋配送料＋調整金額－クーポン割引額－ポイント利用料－セットプロモーション商品割引額－セットプロモーション配送料割引額 - 定期購入割引額 - 定期会員割引額）</summary>
		public decimal PriceCartTotalWithoutPaymentPrice
		{
			get
			{
				return TaxCalculationUtility.GetPriceTaxIncluded(this.PriceSubtotal, this.PriceSubtotalTax)
					+ this.PriceShipping
					+ this.PriceRegulationTotal
					- this.MemberRankDiscount
					- this.UseCouponPrice
					- this.UsePointPrice
					- (this.IsAllItemsSubscriptionBoxFixedAmount ? 0m : this.SetPromotions.ProductDiscountAmount)
					- this.SetPromotions.ShippingChargeDiscountAmount
					- this.FixedPurchaseDiscount
					- this.FixedPurchaseMemberDiscountAmount;
			}
		}
		/// <summary>支払い金額総合計（カート支払い金額合計 ＋決済手数料－セットプロモーション決済手数料割引額）</summary>
		public decimal PriceTotal
		{
			get
			{
				var result = (m_priceTotal < 0) 
					? this.PriceCartTotalWithoutPaymentPrice 
						+ ((this.Payment != null) 
							? this.Payment.PriceExchange
							: 0)
						- (((this.Payment != null) && this.SetPromotions.IsPaymentChargeFree) 
							? this.SetPromotions.PaymentChargeDiscountAmount 
							: 0)
					: m_priceTotal;
				return result;
			}
		}
		/// <summary>
		/// ポイント利用可能額
		/// （商品合計金額(税込み)
		///		- 会員ランク割引額 - クーポン割引額（商品割引額）- セットプロモーション商品割引額 - 定期購入割引額 - 定期会員割引額）
		/// ※ポイント利用可能額には調整金額を含めない
		/// </summary>
		public decimal PointUsablePrice
		{
			get
			{
				return
					TaxCalculationUtility.GetPriceTaxIncluded(this.PriceSubtotal, this.PriceSubtotalTax)
					- this.MemberRankDiscount
					- this.UseCouponPriceForProduct
					- (this.IsAllItemsSubscriptionBoxFixedAmount ? 0m : this.SetPromotions.ProductDiscountAmount)
					- this.FixedPurchaseDiscount
					- this.FixedPurchaseMemberDiscountAmount;
			}
		}
		/// <summary>
		/// クーポン利用可能額
		/// （商品合計金額(税込み)
		///		- 会員ランク割引額 - セットプロモーション商品割引額 - 定期購入割引額 - 定期会員割引額）
		/// ※ポイント利用可能額には調整金額を含めない
		/// </summary>
		public decimal PriceSubtotalForCampaign
		{
			get
			{
				return
					TaxCalculationUtility.GetPriceTaxIncluded(this.PriceSubtotal, this.PriceSubtotalTax)
					- this.MemberRankDiscount
					- (this.IsAllItemsSubscriptionBoxFixedAmount ? 0m : this.SetPromotions.ProductDiscountAmount)
					- this.FixedPurchaseDiscount
					- this.FixedPurchaseMemberDiscountAmount;
			}
		}
		/// <summary>利用ポイント</summary>
		public decimal UsePoint { get; set; }
		/// <summary>利用ポイント（金額換算後）</summary>
		public decimal UsePointPrice { get; set; }
		/// <summary>購入後獲得ポイント</summary>
		public decimal BuyPoint { get; private set; }
		/// <summary>購入後獲得ポイント(ポイントルールIDをキーとし、それぞれにもつ)</summary>
		public Dictionary<string, decimal> BuyPoints { get; private set; } 
		/// <summary>購入後獲得ポイント区分</summary>
		public List<Hashtable> PointKbns { get; private set; } 
		/// <summary>初回購入特別獲得ポイント付与区分</summary>
		public string FirstBuyPointKbn { get; private set; }
		/// <summary>初回購入特別獲得ポイント</summary>
		/// <remarks>※全てのカートに格納されるが、先頭のカートのみ有効とする</remarks>
		public decimal FirstBuyPoint { get; set; }
		/// <summary>初回購入特別獲得ポイント(ポイントルールIDをキーとし、それぞれにもつ)</summary>
		public Dictionary<string, decimal> FirstBuyPoints { get; set; }
		/// <summary>総獲得ポイント</summary>
		public decimal AddPoint { get { return (this.BuyPoint + this.FirstBuyPoint); } }
		/// <summary>次回購入の利用ポイントの全適用フラグ</summary>
		public bool UseAllPointFlg { get; set; }
		/// <summary>配送先情報</summary>
		public List<CartShipping> Shippings { get; private set; }
		/// <summary>支払方法情報</summary>
		public CartPayment Payment { get; set; }
		/// <summary>注文者情報</summary>
		public CartOwner Owner { get; set; }
		/// <summary>クーポン情報</summary>
		public CartCoupon Coupon { get; set; }
		/// <summary>クーポン割引額</summary>
		public decimal UseCouponPrice { get; set; }
		/// <summary>クーポン割引額(商品への割引額)</summary>
		/// <remarks>商品小計上限へのクーポン割引金額チェック時に参照する</remarks>
		public decimal UseCouponPriceForProduct
		{
			get
			{
				if (IsFreeShippingCouponUse())
				{
					return 0m;
				}
				else
				{
					if (IsFreeShippingWithDiscountMoneyCouponUse()) return (this.UseCouponPrice - this.PriceShipping);
					if (IsFreeShippingWithoutDiscountMoneyCouponUse()) return 0m;
					return this.UseCouponPrice;
				}
			}
		}
		/// <summary>クーポン割引額(最大割引額)</summary>
		public decimal UseMaxCouponPrice { get; set; }
		/// <summary>会員ランク割引額</summary>
		public decimal MemberRankDiscount { get; set; }
		/// <summary>注文メモリスト</summary>
		public List<CartOrderMemo> OrderMemos { get; set; }
		/// <summary>注文拡張項目</summary>
		public Dictionary<string, CartOrderExtendItem> OrderExtend { get; set; }
		/// <summary>注文拡張項目 デフォルトを設定するか</summary>
		public bool OrderExtendFirstCreate { get; set; }
		/// <summary>セットプロモーション情報</summary>
		public CartSetPromotionList SetPromotions { get; set; }
		/// <summary>DB更新設定</summary>
		public bool UpdateCartDb { get; set; }
		/// <summary>配送料無料購入金額設定フラグ</summary>
		public string ShippingFreePriceFlg
		{
			get { return (this.Shippings.Count > 0) ? this.Shippings[0].ShippingFreePriceFlg : Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID; }
		}
		/// <summary>配送料無料購入金額設定</summary>
		public decimal ShippingFreePrice
		{
			get { return this.Shippings.Sum<CartShipping>(shipping => shipping.ShippingFreePrice); }
		}
		/// <summary>配送料無料案内表示フラグ</summary>
		public string AnnounceFreeShippingFlg
		{
			get { return (this.Shippings.Count > 0) ? this.Shippings[0].AnnounceFreeShippingFlg : Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_VALID; }
		}
		/// <summary>定期購入あり判定</summary>
		public bool HasFixedPurchase
		{
			get
			{
				foreach (CartProduct cp in this.Items)
				{
					if (cp.IsFixedPurchase)
					{
						return true;
					}
				}
				if (this.Items.Count == 0) return this.HasFixedPurchaseUsedToBe;
				return false;
			}
		}
		/// <summary>Has Subscription Box</summary>
		public bool HasSubscriptionBox
		{
			get
			{
				if (this.Items.Count == 0) return this.HasFixedPurchaseUsedToBe;
				if (this.Items.Any(cp => (cp.IsSubscriptionBox))) return true;
				return false;
			}
		}
		/// <summary>頒布会定額コースか判定</summary>
		public bool IsSubscriptionBoxFixedAmount
		{
			get
			{
				return (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
					&& this.SubscriptionBoxFixedAmount.HasValue
					&& (this.SubscriptionBoxFixedAmount != 0));
			}
		}
		/// <summary>頒布会か</summary>
		public bool IsSubscriptionBox
		{
			get { return (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (this.HasSubscriptionBox || (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false))); }
		}
		/// <summary>頒布会(回数)の前回の商品を引き継ぐ設定か</summary>
		public bool IsSubscriptionBoxTakeOver { get; set; }
		/// <summary>頒布会コース定額価格</summary>
		public decimal? SubscriptionBoxFixedAmount { get; set; }
		/// <summary>頒布会定額コース表示名</summary>
		public string SubscriptionBoxDisplayName { get; set; }
		/// <summary>頒布会購入回数</summary>
		public int? OrderSubscriptionBoxOrderCount { get; set; }
		/// <summary>定期購入あり判定(前は定期カートだった)</summary>
		private bool HasFixedPurchaseUsedToBe { get; set; }
		/// <summary>定期購入専用カートか</summary>
		public bool IsFixedPurchaseOnly
		{
			get { return (Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION && this.HasFixedPurchase); }
		}
		/// <summary>ギフト専用カートか</summary>
		public bool IsGift
		{
			get { return (this.Items.Count > 0) && this.Items[0].IsGift; }
		}
		/// <summary>セット商品あり判定</summary>
		public bool HasSetItem
		{
			get
			{
				foreach (CartProduct cp in this.Items)
				{
					if (cp.IsSetItem)
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>デジタルコンテンツ商品あり判定</summary>
		public bool HasDigitalContents
		{
			get
			{
				foreach (CartProduct cp in this.Items)
				{
					if (cp.IsDigitalContents)
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>配送料別途見積もり表示フラグ</summary>
		public bool ShippingPriceSeparateEstimateFlg
		{
			get { return (this.Shippings.Count > 0) ? this.Shippings[0].ShippingPriceSeparateEstimateFlg : false; }
		}
		/// <summary>配送料別途見積もり文言</summary>
		public string ShippingPriceSeparateEstimateMessage
		{
			get { return (this.Shippings.Count > 0) ? this.Shippings[0].ShippingPriceSeparateEstimateMessage : ""; }
		}
		/// <summary>配送料別途見積もり文言（モバイル）</summary>
		public string ShippingPriceSeparateEstimateMessageMobile
		{
			get { return (this.Shippings.Count > 0) ? this.Shippings[0].ShippingPriceSeparateEstimateMessageMobile : ""; }
		}
		/// <summary>配送料未確定文言を表示するか</summary>
		public bool IsDisplayShippingPriceUnsettled
		{
			get
			{
				var result = (Constants.GLOBAL_OPTION_ENABLE) && (this.Shippings.Count > 0);
				return result;
			}
		}
		/// <summary>宅配便か？</summary>
		public bool IsExpressDelivery
		{
			get { return (this.Shippings.Count > 0) && this.Shippings[0].IsExpress; }
		}
		/// <summary>メール便か？</summary>
		public bool IsMailDelivery
		{
			get { return (this.Shippings.Count > 0) && this.Shippings[0].IsMail; }
		}
		/// <summary>注文完了</summary>
		public bool IsOrderDone { get; set; }
		/// <summary>Amazon Payで利用する注文ID</summary>
		public string AmazonOrderReferenceId { get; set; }
		/// <summary>外部支払契約ID</summary>
		public string ExternalPaymentAgreementId { get; set; }
		/// <summary>定期購入割引額</summary>
		public decimal FixedPurchaseDiscount { get; set; }
		/// <summary>定期購入台帳情報</summary>
		public FixedPurchaseModel FixedPurchase { get; set; }
		/// <summary>選択されたクーポン</summary>
		public string SelectedCoupon { get; set; }
		/// <summary> クーポン入力方法 </summary>
		public string CouponInputMethod { get; set; }
		/// <summary> クーポンBOX表示 </summary>
		public bool CouponBoxVisible { get; set; }
		/// <summary>定期会員割引額</summary>
		public decimal FixedPurchaseMemberDiscountAmount { get; set; }
		/// <summary>定期会員判定</summary>
		public bool IsFixedPurchaseMember { get; set; }
		/// <summary>定期購入あり判定（定期購入カート分離用）</summary>
		public bool HasFixedPurchaseForCartSeparation { get; set; }
		/// <summary>定期会員割引適用あり判定</summary>
		public bool IsApplyFixedPurchaseMemberDiscount
		{
			get
			{
				return (
					Constants.MEMBER_RANK_OPTION_ENABLED
					&& Constants.FIXEDPURCHASE_OPTION_ENABLED
					&& (this.IsFixedPurchaseMember || this.HasFixedPurchase || this.HasFixedPurchaseForCartSeparation)
				);
			}
		}
		/// <summary>外部連携メモ</summary>
		public string RelationMemo { get; set; }
		/// <summary>管理メモ</summary>
		public string ManagementMemo { get; set; }
		/// <summary>配送メモ</summary>
		public string ShippingMemo { get; set; }
		/// <summary>決済メモ</summary>
		public string PaymentMemo { get; set; }
		/// <summary>注文同梱親注文ID</summary>
		public string OrderCombineParentOrderId { get; set; }
		/// <summary>Order Combine Parent Tran Id</summary>
		public string OrderCombineParentTranId { get; set; }
		/// <summary>注文同梱 親注文が定期商品を持っていたか</summary>
		public bool IsCombineParentOrderHasFixedPurchase { get; set; }
		/// <summary>注文同梱 親注文がクーポンを利用していたか</summary>
		public bool IsCombineParentOrderUseCoupon { get; set; }
		/// <summary>注文同梱 親注文の定期購入注文回数</summary>
		public int CombineParentOrderFixedPurchaseOrderCount { get; set; }
		/// <summary>注文同梱 注文同梱前カートが定期商品を持っていたか</summary>
		public bool IsBeforeCombineCartHasFixedPurchase { get; set; }
		/// <summary>割引金額(定期注文)</summary>
		public decimal DiscountPriceForFixedPurchase
		{
			get
			{
				// 定期購入割引額、会員ランク割引額、セットプロモーション割引額
				var result = this.FixedPurchaseDiscount
					+ this.MemberRankDiscount
					+ (this.IsAllItemsSubscriptionBoxFixedAmount
						? (this.SetPromotions.PaymentChargeDiscountAmount
							+ this.SetPromotions.ShippingChargeDiscountAmount)
						: this.SetPromotions.TotalDiscountAmount);
				return result;
			}
		}
		/// <summary>カートコピー完了メッセージを表示するか？</summary>
		public bool IsCartCopyCompleteMesseges { get; set; }
		/// <summary>カート削除完了メッセージを表示するか？</summary>
		public bool IsCartDeleteCompleteMesseges { get; set; }
		/// <summary>過去の購入履歴の注文ID</summary>
		public string[] ProductOrderLmitOrderIds { get; set; }
		/// <summary>購入に制限があるか</summary>
		public bool IsOrderLimit { set; get; }
		/// <summary>管理メモ（重複注文ID）を持っているか</summary>
		public bool HasNotFirstTimeOrderIdList
		{
			get
			{
				return (this.ProductOrderLmitOrderIds.Length > 0);
			}
		}
		/// <summary>定期購入の初回購入ではないか。（カート間での配送先が重複している）</summary>
		public bool HasNotFirstTimeByCart { get; set; }
		/// <summary>レコメンド商品が投入されているか</summary>
		public bool HasRecommendItem
		{
			get { return (this.Items.Any(item => item.IsRecommendItem)); }
		}
		/// <summary>注文完了ページでのレコメンド適用前注文ID</summary>
		public string BeforeRecommendOrderId { get; set; }
		/// <summary>ペイパル連携情報</summary>
		public PayPalCooperationInfo PayPalCooperationInfo { get; set; }
		/// <summary>購入制限の対象商品が含まれているか</summary>
		public bool HasOrderLimitProduct
		{
			get { return this.Items.Any(item => item.IsOrderLimitProduct); }
		}
		/// <summary>カート内で購入制限を満たしているか</summary>
		public bool IsCompliantOrderLimitProduct
		{
			get
			{
				return ((this.DuplicateLimitProductIds != null)
					&& this.DuplicateLimitProductIds.Any());
			}
		}
		/// <summary>カート内で重複している購入制限対象商品ID</summary>
		public string[] DuplicateLimitProductIds { get; set; }
		/// <summary>配送先に未割当の商品があるか</summary>
		private bool HasUnAllocatedProductToShipping
		{
			get
			{
				var hasUnAllocatedProductToShipping = true;

				if (this.IsGift)
				{
					// 配送先に割り当てられてる商品個数
					var shippingItemsCount = this.Shippings.Sum(shipping => shipping.ProductCounts.Sum(pc => pc.Count));

					// カート内の商品個数
					var cartItemsCount = this.Items.Sum(item => item.CountSingle);

					// 配送先に割り当てられていない商品があるかどうか
					hasUnAllocatedProductToShipping = shippingItemsCount != cartItemsCount;
				}

				return hasUnAllocatedProductToShipping;
			}
		}
		/// <summary>決済金額</summary>
		public decimal SettlementAmount { get; set; }
		/// <summary>決済通貨</summary>
		public string SettlementCurrency { get; set; }
		/// <summary>決済金額</summary>
		public decimal SettlementRate { get; set; }
		/// <summary>送金額</summary>
		public decimal SendingAmount
		{
			get { return CurrencyManager.GetSendingAmount(this.PriceTotal, this.SettlementAmount, this.SettlementCurrency); }
		}
		/// <summary>Reflect memo to fixed purchase</summary>
		public bool ReflectMemoToFixedPurchase { get; set; }
		/// <summary>Only reflect memo to fixed purchase</summary>
		public bool ReflectMemoToFixedPurchaseVisible
		{
			get { return this.HasFixedPurchase; }
		}
		/// <summary>Is Return Cart</summary>
		public bool IsReturnCart { get; set; }
		/// <summary>コンテンツログ</summary>
		public ContentsLogModel ContentsLog { get; set; }

		/// <summary>入力された配送料金（配送料手動入力の時のみ使用）</summary>
		public decimal? EnteredShippingPrice { get; set; }
		/// <summary>入力された決済手数料金（決済手数料手動入力の時のみ使用）</summary>
		public decimal? EnteredPaymentPrice { get; set; }
		/// <summary>配送料(割引額/税額計算用)</summary>
		public decimal ShippingPriceForCalculationDiscountAndTax { get { return this.EnteredShippingPrice ?? this.PriceShipping; } }
		/// <summary>配送料(割引後)</summary>
		public decimal ShippingPriceWithDiscount { get { return this.ShippingPriceForCalculationDiscountAndTax - this.ShippingPriceDiscountAmount; } }
		/// <summary>決済手数料金(割引額/税額計算用)</summary>
		public decimal PaymentPriceForCalculationDiscountAndTax
		{
			get
			{
				return this.EnteredPaymentPrice ?? ((this.Payment != null) ? this.Payment.PriceExchange : 0);
			}
		}
		/// <summary>決済手数料(割引後)</summary>
		public decimal PaymentPriceWithDiscount { get { return this.PaymentPriceForCalculationDiscountAndTax - this.PaymentPriceDiscountAmount; } }
		/// <summary>広告コード（最新分）</summary>
		public string AdvCodeNew { get; set; }
		/// <summary>購入制限チェック対象商品</summary>
		public List<CartProduct> TargetProductListForCheckProductOrderLimit { get; set; }
		/// <summary>領収書希望フラグ</summary>
		public string ReceiptFlg { get; set; }
		/// <summary>領収書の宛名</summary>
		public string ReceiptAddress { get; set; }
		/// <summary>領収書の但し書き</summary>
		public string ReceiptProviso { get; set; }
		/// <summary>カート番号「１」と同じ領収書を指定するか</summary>
		public bool IsUseSameReceiptInfoAsCart1 { get; set; }
		/// <summary>Is Show Display Convenience Store</summary>
		public bool IsShowDisplayConvenienceStore { get; set; }
		/// <summary>Is Shipping Convenience Store</summary>
		public bool IsShippingConvenienceStore { get; set; }
		/// <summary>モールID</summary>
		public string MallId { get; set; }
		/// <summary>頒布会コースID（注文同梱時は子注文のIDを保持）</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>定期購入表示回数</summary>
		public int FixedPurchaseDisplayCount { get; set; }
		/// <summary>Subscription Box Error Message</summary>
		public string SubscriptionBoxErrorMsg { get; set; }
		/// <summary>ポイント付与可能の注文か</summary>
		public bool IsOrderGrantablePoint
		{
			get
			{
				if (string.IsNullOrEmpty(this.MallId)) return true;
				return ((this.MallId == Constants.FLG_ORDER_MALL_ID_OWN_SITE)
					|| (this.MallId == Constants.FLG_USER_MALL_ID_URERU_AD));
			}
		}
		/// <summary>Token Atone</summary>
		public string TokenAtone { get; set; }
		/// <summary>Token Aftee</summary>
		public string TokenAftee { get; set; }
		/// <summary>Total Price Discount</summary>
		public decimal TotalPriceDiscount
		{
			get
			{
				var result = this.MemberRankDiscount
					+ this.UseCouponPrice
					+ this.UsePointPrice
					+ (this.IsAllItemsSubscriptionBoxFixedAmount ? 0m : this.SetPromotions.ProductDiscountAmount)
					+ this.SetPromotions.ShippingChargeDiscountAmount
					+ this.FixedPurchaseDiscount
					+ this.FixedPurchaseMemberDiscountAmount;
				return result;
			}
		}
		/// <summary>Is Order Sales Settled</summary>
		public bool IsOrderSalesSettled { get; set; }
		/// <summary>Online Payment Status</summary>
		public string OnlinePaymentStatus { get; set; }
		/// <summary>Is Return All Items</summary>
		public bool IsReturnAllItems { get; set; }
		/// <summary>Is Not Update Information For Payment Atone Or Aftee</summary>
		public bool IsNotUpdateInformationForPaymentAtoneOrAftee { get; set; }
		/// <summary>Is Payment Atone Or Aftee</summary>
		public bool IsPaymentAtoneOrAftee
		{
			get
			{
				var payment = new List<string>
				{
					Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					Constants.FLG_PAYMENT_PAYMENT_ID_ATONE
				};
				var result = ((this.Payment != null)
					&& payment.Contains(this.Payment.PaymentId));
				return result;
			}
		}
		/// <summary>Is Pre-Approved Line Pay payment</summary>
		public bool IsPreApprovedLinePayPayment { get; set; }
		/// <summary>Target product recommends</summary>
		public List<CartProduct> TargetProductRecommends { get; set; }
		/// <summary>ゲスト購入か</summary>
		public bool IsGuestUser { get; set; }
		/// <summary>BOTCHAN注文か</summary>
		public bool IsBotChanOrder { get; set; }
		/// <summary>GMO後払い_デバイス情報</summary>
		public string DeviceInfo { get; set; }
		/// <summary>定期配送設定を保持しているか</summary>
		public bool HasFixedPurchaseShippingPattern
		{
			get
			{
				return (this.HasFixedPurchase && GetShipping().HasFixedPurchaseSetting);
			}
		}
		/// <summary>Is Landing Use NewebPay</summary>
		public bool IsLandingUseNewebPay { get; set; }
		/// <summary>カートユーザーが適用されるターゲットリスト</summary>
		public string[] TargetLists
		{
			get
			{
				if (string.IsNullOrEmpty(this.CartUserId))
				{
					return new string[0];
				}

				// ターゲットリストがNullの場合、DBからデータを取得
				m_targetLists = m_targetLists ?? new TargetListService().GetHitTargetListId(
					Constants.CONST_DEFAULT_DEPT_ID,
					this.CartUserId,
					Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST);
				return m_targetLists;
			}
			set { m_targetLists = value; }
		}
		/// <summary>カートユーザーが適用されるターゲットリスト格納用</summary>
		private string[] m_targetLists;
		/// <summary>2回目以降の定期金額表示か</summary>
		public bool IsSecondFixedPurchase { get; set; }
		/// <summary>Is receipt flag on</summary>
		public bool IsReceiptFlagOn
		{
			get { return (this.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON); }
		}
		/// <summary>Is has first buy point</summary>
		public bool IsHasFirstBuyPoint
		{
			get { return (this.FirstBuyPoint != 0); }
		}
		/// <summary>Purchase price total</summary>
		public decimal PurchasePriceTotal
		{
			get
			{
				var totalDiscount = this.MemberRankDiscount
					+ this.UseCouponPriceForProduct
					+ this.FixedPurchaseDiscount
					+ this.FixedPurchaseMemberDiscountAmount
					+ this.SetPromotions.ProductDiscountAmount;

				var result = (this.PriceSubtotal - totalDiscount);
				return result;
			}
		}
		/// <summary>Can use point for purchase</summary>
		public bool CanUsePointForPurchase
		{
			get
			{
				return ((this.PurchasePriceTotal >= Constants.POINT_MINIMUM_PURCHASEPRICE)
					|| this.HasRecommendItem);
			}
		}
		/// <summary>前回と同じ商品であるか</summary>
		public bool IsSameProducts { get; set; } 
		/// <summary>頒布会・定期何回目</summary>
		public int SubscriptionTimes { get; set; } 
		/// <summary>頒布会何回目まで商品重複されている</summary>
		public int DuplicatedSubscriptionTimesTo { get; set; }
		/// <summary>受注情報編集画面での注文編集時か</summary>
		private bool IsOrderModifyInput { get; set; }
		/// <summary>注文同梱されているか</summary>
		public bool IsOrderCombined => string.IsNullOrEmpty(this.OrderCombineParentOrderId) == false;
		/// <summary>定期台帳が登録済みの注文同士での注文同梱か（通常注文の場合はTRUE）</summary>
		public bool IsOrderCombinedWithRegisteredSubscription { get; set; }
		/// <summary>頒布会が含まれる注文同梱か</summary>
		public bool IsOrderCombinedWithSubscriptionBox => this.IsOrderCombined && this.HasSubscriptionBox;
		/// <summary>注文同梱時に頒布会含む定期台帳を登録するか</summary>
		public bool IsRegisterFixedPurchaseWhenOrderCombine
		{
			get
			{
				// 定期台帳を登録するか（頒布会が含まれない場合）
				// 親：通常、子：定期
				var isRegisterFixedPurchase =
					(this.IsCombineParentOrderHasFixedPurchase == false)
						&& this.IsBeforeCombineCartHasFixedPurchase;

				// 定期台帳を登録するか（頒布会が含まれる場合）
				// 親：頒布会、子：定期
				var isRegisterFixedPurchaseWithSubscriptionBox =
					this.IsOrderCombinedWithSubscriptionBox
						&& (this.IsOrderCombinedWithRegisteredSubscription == false)
						&& (IsOrderCombinedWithSameSubscriptionBoxCourse() == false);

				// 頒布会台帳を登録するか
				// 親：子と同コース頒布会以外、子：頒布会
				var isRegisterSubscriptionBox = this.IsShouldRegistSubscriptionForCombine;

				return isRegisterFixedPurchase
					|| isRegisterFixedPurchaseWithSubscriptionBox
					|| isRegisterSubscriptionBox;
			}
		}
		/// <summary>注文同梱時に頒布会台帳を登録するか（注文同梱時の子注文が新規頒布会の場合TRUE）</summary>
		public bool IsShouldRegistSubscriptionForCombine { get; set; }
		/// <summary>定額頒布会商品が含まれるか</summary>
		public bool HasSubscriptionBoxFixedAmountItem
		{
			get
			{
				var result = this.Items.Any(item => item.IsSubscriptionBoxFixedAmount());
				return result;
			}
		}
		/// <summary>商品が全て頒布会定額コース商品か</summary>
		public bool IsAllItemsSubscriptionBoxFixedAmount
		{
			get
			{
				var hasItemsNotFixedAmount = this.Items.Any(item => item.IsSubscriptionBoxFixedAmount() == false);
				return hasItemsNotFixedAmount == false;
			}
		}
		/// <summary>Is shipping store pickup</summary>
		public bool IsShippingStorePickup { get; set; }
		#region セットプロモーション、配送情報に紐づいた商品情報(注文商品情報作成用) 
		/// <summary>
		/// セットプロモーション、配送情報に紐づいた商品情報
		/// </summary>
		private class AllocatedToSetAndShippingProduct
		{
			/// <summary>カート商品</summary>
			public CartProduct Product { get; set; }
			/// <summary>注文配送先枝番</summary>
			public int OrderShippingNo { get; set; }
			/// <summary>注文セットプロモーション枝番</summary>
			public int? OrderSetpromotionNo { get; set; }
			/// <summary>注文個数</summary>
			public int ItemQuantity { get; set; }
		}
		#endregion

		/// <summary>注文完了ページレコメンドチェックか</summary>
		public bool IsNeedCheckOrderCompleteRecommend { get; set; }
		/// <summary>注文同梱によってクーポン適応外の商品が存在するか</summary>
		public bool IsCouponNotApplicableByOrderCombined { get; set; }
		/// <summary>定額頒布会価格リスト</summary>
		public List<CartSubscriptionBoxFixedAmount> SubscriptionBoxFixedAmountList { get; set; }
		/// <summary>Frontでの注文編集時か</summary>
		private bool IsOrderModify { get; set; }

		/// <summary>既存のセットプロモーション</summary>
		public List<SetPromotionModel> SetPromotionsOld { get; set; }
		/// <summary>配送料無料時の請求料金文言表示か</summary>
		public bool IsDisplayFreeShiipingFeeText
		{
			get
			{
				return Constants.FREE_SHIPPING_FEE_OPTION_ENABLED
					&& GetShipping().IsUseFreeShippingFee
					&& (GetShipping().ShippingZoneNo <= CartShipping.MAX_SHIPPING_ZONE_NO);
			}
		}
		/// <summary>初回選択された頒布会か</summary>
		public bool IsFirstSelectionSubscriptionBox { get; set; }
	}
}
