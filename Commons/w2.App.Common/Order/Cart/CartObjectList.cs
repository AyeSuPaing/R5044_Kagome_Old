/*
=========================================================================================================
  Module      : カートオブジェクトリストクラス(CartObjectList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.DataCacheController;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Cart;
using w2.Domain.ContentsLog;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Order.Helper;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserShipping;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カートオブジェクトリストクラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public partial class CartObjectList : IEnumerable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">カートに紐づくユーザID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="updateCartDb">カートテーブルに保存するか</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		public CartObjectList(
			string userId,
			string orderKbn,
			bool updateCartDb,
			string fixedPurchaseId = "",
			string memberRankId = "",
			SqlAccessor accessor = null,
			FixedPurchaseModel fixedPurchase = null)
		{
			this.UserId = userId;
			this.MemberRankId = string.IsNullOrEmpty(memberRankId) ? MemberRankOptionUtility.GetMemberRankId(this.UserId, accessor) : memberRankId;
			this.OrderKbn = orderKbn;
			this.Items = new List<CartObject>();
			this.UpdateCartDb = updateCartDb;
			this.FixedPurchaseId = fixedPurchaseId;
			this.FixedPurchase = fixedPurchase;
			this.CanSetUserDefaultOrderSettingShipping = true;
			this.IsLandingCart = false;
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
		/// 注文情報からカート情報生成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="orders">注文情報</param>
		/// <returns>カートオブジェクトリスト</returns>
		public static CartObjectList CreateCartObjectListByOrder(string userId, string orderKbn, IEnumerable<OrderModel> orders)
		{
			var cartList = new CartObjectList(userId, orderKbn, false);
			cartList.Items.AddRange(orders.Select(CartObject.CreateCartByOrder));

			var firstCart = cartList.Items.FirstOrDefault();
			if (firstCart != null)
			{
				cartList.SubscriptionBoxCourseId = firstCart.SubscriptionBoxCourseId;
			}
			return cartList;
		}

		/// <summary>
		/// ユーザに紐づくカート情報取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderKbn">注文区分</param>
		/// <param name="cartIdList">カートID一覧カートID一覧(カンマ区切りの平文)</param>
		public static CartObjectList GetUserCartList(string userId, string orderKbn, string cartIdList = "")
		{
			//------------------------------------------------------
			// カートリスト作成
			//------------------------------------------------------
			CartObjectList colResult = new CartObjectList(userId, orderKbn, true);

			//------------------------------------------------------
			// DBからカート情報取得
			//------------------------------------------------------
			DataView dvCartData = (userId != "") ? GetUserCart(userId) : GetGuestUserCart(cartIdList);

			//------------------------------------------------------
			// カートリストへカート＆商品紐付け
			//------------------------------------------------------
			string strCartId = null;
			CartObject coCart = null;
			CartProductSet cpsProductSet = null;
			string strProductSetIdTmp = null;
			string strProductSetNoTmp = null;
			var cartIdTemp = string.Empty;
			foreach (DataRowView drvCartProduct in dvCartData)
			{
				//------------------------------------------------------
				// 新しいカートオブジェクト作成
				//------------------------------------------------------
				if ((string)drvCartProduct[Constants.FIELD_CART_CART_ID] != strCartId)
				{
					if (Constants.CARTCOPY_OPTION_ENABLED == false)
					{
						// 配送種別＆ダウンロードフラグの同じカートは復元しない
						if ((coCart != null)
							&& ((string)drvCartProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE] == coCart.ShippingType)
							&& (((string)drvCartProduct[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID) == coCart.IsDigitalContentsOnly)
							&& (((string)drvCartProduct[Constants.FIELD_CART_FIXED_PURCHASE_FLG] == Constants.FLG_CART_FIXED_PURCHASE_FLG_ON) == coCart.IsFixedPurchaseOnly)
							&& (((string)drvCartProduct[Constants.FIELD_CART_GIFT_ORDER_FLG] == Constants.FLG_CART_GIFT_ORDER_FLG_FLG_ON) == coCart.IsGift))
						{
							// 配送種別＆ダウンロードフラグの同じカート情報を削除し、次のカートへ
							DeleteCartDB((string)drvCartProduct[Constants.FIELD_CART_CART_ID]);
							continue;
						}
					}

					// カートID更新
					strCartId = (string)drvCartProduct[Constants.FIELD_CART_CART_ID];

					// カートオブジェクト生成
					coCart = new CartObject(
						strCartId,
						userId,
						orderKbn,
						(string)drvCartProduct[Constants.FIELD_CART_SHOP_ID],
						(string)drvCartProduct[Constants.FIELD_CART_CART_DIV_TYPE1],
						(string)drvCartProduct[Constants.FIELD_CART_CART_DIV_TYPE2] == Constants.FLG_CART_DIGITAL_CONTENTS_FLG_ON, // Vupで DIV_TYPE2="" のときは false
						updateCartDb: true,
						memberRankId: "",
						subscriptionBoxCourseId: (string)drvCartProduct[Constants.FIELD_CART_CART_DIV_TYPE3]);
					coCart.CheckProductDeleted();

					// 紐づける
					colResult.AddCartVirtural(coCart);
				}

				//------------------------------------------------------
				// 通常商品紐付け
				//------------------------------------------------------
				string strProductSetId = (string)drvCartProduct[Constants.FIELD_CART_PRODUCT_SET_ID];
				string strProductSetNo = StringUtility.ToEmpty(drvCartProduct[Constants.FIELD_CART_PRODUCT_SET_NO]);
				if (strProductSetId.Length == 0)
				{
					bool blCartProductAddFlg = true;
					if (Constants.CARTCOPY_OPTION_ENABLED == false)
					{
						foreach (CartObject cartObject in colResult.Items)
						{
							foreach (CartProduct cp in cartObject.Items)
							{
								// 別々のカートに同じ商品が存在する場合、カートへ復元する商品は最初に取得したカートの商品のみとする。
								// （同じ商品が別々のカートに復元されることでカート分割されてしまう現象を回避する。）
								if ((cp.ShopId == (string)drvCartProduct[Constants.FIELD_CART_SHOP_ID])
									&& (cp.ProductId == (string)drvCartProduct[Constants.FIELD_CART_PRODUCT_ID])
									&& (cp.VariationId == (string)drvCartProduct[Constants.FIELD_CART_VARIATION_ID])
									&& (cp.IsFixedPurchase == ((string)drvCartProduct[Constants.FIELD_CART_FIXED_PURCHASE_FLG] == Constants.FLG_CART_FIXED_PURCHASE_FLG_ON))
									&& (cp.IsGift == ((string)drvCartProduct[Constants.FIELD_CART_GIFT_ORDER_FLG] == Constants.FLG_CART_GIFT_ORDER_FLG_FLG_ON))
									&& (cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() == (string)drvCartProduct[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]))
								{
									blCartProductAddFlg = false;
									break;
								}
							}
						}
					}
					else
					{
						blCartProductAddFlg = true;
					}

					if (blCartProductAddFlg)
					{
						DataView dvProduct = ProductCommon.GetProductVariationInfo(
							(string)drvCartProduct[Constants.FIELD_CART_SHOP_ID],
							(string)drvCartProduct[Constants.FIELD_CART_PRODUCT_ID],
							(string)drvCartProduct[Constants.FIELD_CART_VARIATION_ID],
							MemberRankOptionUtility.GetMemberRankId(userId));

						var subscriptionBoxCourseId = (string)drvCartProduct[Constants.FIELD_CART_CART_DIV_TYPE3];
						if (dvProduct.Count != 0)
						{
							Constants.AddCartKbn addCartKbn;
							if (Constants.FIXEDPURCHASE_OPTION_ENABLED && ((string)drvCartProduct[Constants.FIELD_CART_FIXED_PURCHASE_FLG] == Constants.FLG_CART_FIXED_PURCHASE_FLG_ON))
							{
								addCartKbn = string.IsNullOrEmpty(subscriptionBoxCourseId)
									? Constants.AddCartKbn.FixedPurchase
									: Constants.AddCartKbn.SubscriptionBox;
							}
							else if (Constants.GIFTORDER_OPTION_ENABLED && ((string)drvCartProduct[Constants.FIELD_CART_GIFT_ORDER_FLG] == Constants.FLG_CART_GIFT_ORDER_FLG_FLG_ON))
							{
								addCartKbn = Constants.AddCartKbn.GiftOrder;
							}
							else
							{
								addCartKbn = Constants.AddCartKbn.Normal;
							}

							var cartProduct = new CartProduct(
									dvProduct[0],
									addCartKbn,
									(string)drvCartProduct[Constants.FIELD_CART_PRODUCTSALE_ID],
									(int)drvCartProduct[Constants.FIELD_CART_PRODUCT_COUNT],
									coCart.UpdateCartDb,
									(string)drvCartProduct[Constants.FIELD_CART_PRODUCT_OPTION_TEXTS],
									fixedPurchaseId: "",
									cartId: (string)drvCartProduct[Constants.FIELD_CART_CART_ID],
									contentsLogModel: null,
									subscriptionBoxCourseId: subscriptionBoxCourseId)
							{
								NoveltyId = (string)drvCartProduct[Constants.FIELD_CART_NOVELTY_ID],
								RecommendId = (string)drvCartProduct[Constants.FIELD_CART_RECOMMEND_ID]
							};

							// カート投入（まだ再計算しない）
							coCart.AddVirtural(cartProduct, false);
						}
					}
				}
				//------------------------------------------------------
				// セット商品紐付け
				//------------------------------------------------------
				else
				{
					if ((strProductSetId != strProductSetIdTmp) || (strProductSetNo != strProductSetNoTmp) || (strCartId != cartIdTemp))
					{
						strProductSetIdTmp = strProductSetId;
						strProductSetNoTmp = strProductSetNo;
						cartIdTemp = strCartId;

						// セット情報取得（無ければ追加無し）
						DataView dvProductSet = ProductCommon.GetProductSetInfo(
							(string)drvCartProduct[Constants.FIELD_CART_SHOP_ID],
							strProductSetId);

						if (dvProductSet.Count == 0)
						{
							continue;
						}
						cpsProductSet = new CartProductSet(dvProductSet[0], (int)drvCartProduct[Constants.FIELD_CART_PRODUCT_SET_COUNT], int.Parse(strProductSetNo), true, strCartId);
					}

					// セット商品として追加
					CartProduct cpProduct = cpsProductSet.AddProductVirtual(
						(string)drvCartProduct[Constants.FIELD_CART_SHOP_ID],
						(string)drvCartProduct[Constants.FIELD_CART_PRODUCT_ID],
						(string)drvCartProduct[Constants.FIELD_CART_VARIATION_ID],
						(int)drvCartProduct[Constants.FIELD_CART_PRODUCT_COUNT]);

					if (cpProduct == null)
					{
						//商品がなければ削除
						coCart.RemoveProduct(
							(string)drvCartProduct[Constants.FIELD_CART_SHOP_ID],
							(string)drvCartProduct[Constants.FIELD_CART_PRODUCT_ID],
							(string)drvCartProduct[Constants.FIELD_CART_VARIATION_ID],
							Constants.AddCartKbn.Normal,
							"",
							"");

						continue;
					}

					// カート投入（まだ再計算しない）
					cpProduct.CartId = (string)drvCartProduct[Constants.FIELD_CART_CART_ID];
					coCart.AddVirtural(cpProduct, false);
				}
			}

			//------------------------------------------------------
			// 別々のカートから同じ商品を削除して空になったカートを削除する。
			//------------------------------------------------------
			List<CartObject> lDeleteCartObject = new List<CartObject>();
			foreach (CartObject cartObject in colResult)
			{
				if (cartObject.Items.Count == 0)
				{
					lDeleteCartObject.Add(cartObject);
				}
			}
			foreach (CartObject cartObject in lDeleteCartObject)
			{
				colResult.DeleteCartVurtual(cartObject);
			}

			//------------------------------------------------------
			// 最後に再計算
			//------------------------------------------------------
			colResult.Items.ForEach(c => c.Calculate(true, isCartItemChanged: true));

			// カートリストの後半部分はデジタルOnlyのカートにする
			colResult.Items = colResult.Items.OrderBy(x => x.IsDigitalContentsOnly).ToList();
			return colResult;
		}

		/// <summary>
		/// カートテーブルのユーザID更新
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		public void UpdateUserIdToCartDb(string strUserId)
		{
			foreach (CartObject co in this.Items)
			{
				// カートのユーザID更新(DBも更新)
				co.UpdateCartUserId(strUserId);

				// カートの再計算(ポイント等再計算)（ユーザID紐付けの際はデフォルト配送先で計算する）
				co.CalculateWithDefaultShipping();
			}

			// プロパティ更新
			this.UserId = strUserId;
		}

		/// <summary>
		/// 全カートの配送料計算
		/// ※配送先が指定していなければ、デフォルト配送先で配送料計算を行う
		/// </summary>
		public void CalculateAllCart()
		{
			foreach (var cart in this.Items)
			{
				var isDefaultShipping = true;
				if ((cart.Shippings != null) && (cart.Shippings.Count > 0))
				{
					// 配送先が指定されている（=郵便番号1がセットされている）とする
					var shipping = cart.Shippings[0];
					isDefaultShipping = cart.IsGift
						? string.IsNullOrEmpty(shipping.SenderZip1)
						: string.IsNullOrEmpty(shipping.Zip1);
				}
				cart.Calculate(isDefaultShipping, true);
			}
		}

		/// <summary>
		/// ユーザIDに紐づくカート情報取得(ログイン時)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>カート情報</returns>
		private static DataView GetUserCart(string userId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Cart", "GetUserCart"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_CART_USER_ID, userId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// カートIDに紐づくカート情報取得(クッキーから)
		/// </summary>
		/// <param name="cartIdList">カートID一覧カートID一覧(カンマ区切りの平文)</param>
		/// <returns>カート情報</returns>
		private static DataView GetGuestUserCart(string cartIdList)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Cart", "GetGuestUserCart"))
			{
				StringBuilder targetCartIdList = new StringBuilder();
				targetCartIdList.Append("'").Append(cartIdList.Replace(",", "','")).Append("'");
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ cart_id @@", targetCartIdList.ToString());
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}
		}

		/// <summary>
		/// カートを取得
		/// </summary>
		/// <param name="strCartId">カートID</param>
		/// <returns>カートオブジェクト</returns>
		public CartObject GetCart(string strCartId)
		{
			foreach (CartObject coCart in this.Items)
			{
				if (coCart.CartId == strCartId)
				{
					return coCart;
				}
			}

			return null;
		}

		/// <summary>
		/// 商品データ取得（セット内の商品も含めてどちらかをかえす）
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <returns>カート商品情報</returns>
		public CartProduct GetProductEither(string strShopId, string strProductId, string strVariationId)
		{
			CartProduct cpResult = null;
			foreach (CartObject co in this.Items)
			{
				cpResult = co.GetProductEither(strShopId, strProductId, strVariationId);

				if (cpResult != null)
				{
					break;
				}
			}

			return cpResult;
		}

		/// <summary>
		/// カートへの商品投入処理
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strTimeSaleId">タイムセールID</param>
		/// <param name="iProductCount">追加商品数</param>
		/// <param name="posl">商品付帯情報一覧</param>
		/// <param name="contentsLogModel">コンテンツログ</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextDateTime">次回配送日</param>
		/// <param name="userId">ユーザID</param>
		public CartProduct AddProduct(
			DataRowView drvProduct,
			Constants.AddCartKbn addCartKbn,
			string strTimeSaleId,
			int iProductCount,
			ProductOptionSettingList posl,
			ContentsLogModel contentsLogModel = null,
			string languageCode = null,
			string languageLocaleId = null,
			string subscriptionBoxCourseId = "",
			string lastChanged = "",
			DateTime nextDateTime = default(DateTime),
			string userId = null)
		{
			if (userId != null) this.UserId = userId;
			var cartProduct = new CartProduct(
				drvProduct,
				addCartKbn,
				strTimeSaleId,
				iProductCount,
				this.UpdateCartDb,
				posl,
				this.FixedPurchaseId,
				contentsLogModel,
				subscriptionBoxCourseId,
				lastChanged,
				nextDateTime);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				cartProduct.LanguageCode = languageCode;
				cartProduct.LanguageLocaleId = languageLocaleId;
			}

			return AddProduct(cartProduct);
		}
		/// <summary>
		/// カートへの商品投入処理
		/// </summary>
		/// <param name="product">カート商品情報</param>
		/// <param name="isCartSelect">選択カートか？</param>
		public CartProduct AddProduct(CartProduct product, bool isCartSelect = false)
		{
			return AddProduct(product, false, isCartSelect);
		}
		/// <summary>
		/// カートへの商品投入処理
		/// </summary>
		/// <param name="product">カート商品情報</param>
		/// <param name="doNotCartSeparation">カート分割を行わないよう制御するか</param>
		/// <param name="isCartSelect">選択カートか？</param>
		public CartProduct AddProduct(CartProduct product, bool doNotCartSeparation, bool isCartSelect)
		{
			lock (this)
			{
				// 全カート内に同一商品があるか判定
				foreach (var co in this.Items)
				{
					var sameCartProduct = co.GetSameProduct(product);

					// 初回選択された頒布会の場合、商品数を更新
					if ((sameCartProduct != null) && co.IsFirstSelectionSubscriptionBox)
					{
						sameCartProduct.SetProductCount(co.CartId, product.CountSingle);
						return sameCartProduct;
					}

					// カート合計が最大同時合計可能数を超えていなければ、商品購入数を加算して再計算する
					if (co.CheckProductMaxSellQuantity(product, sameCartProduct, isCartSelect))
					{
						// 商品購入数を加算
						sameCartProduct.SetProductCount(co.CartId, sameCartProduct.CountSingle + product.CountSingle);	// DBにも更新
						co.Calculate(true, isCartItemChanged: true);
						return sameCartProduct;
					}
				}

				// 店舗ID、配送種別（カート分割判断基準１）
				string shopId = product.ShopId;
				string shippingType = product.ShippingType;
				// デジタルコンテンツフラグ（カート分割判断基準２）
				bool isDigitalContents = product.IsDigitalContents;

				//------------------------------------------------------
				// 追加対象カート取得
				//------------------------------------------------------
				// 同じ店舗IDかつ分割基準のものを取得
				CartObject coTarget = null;
				foreach (var co in this.Items)
				{
					if (coTarget != null)
					{
						break;
					}

					if (co.CanAddCartObject(product, doNotCartSeparation, isCartSelect)) coTarget = co;
				}

				// 見つからなければ新たにカート作成（この時点で採番されるがレコードにデータは挿入されません）
				if (coTarget == null)
				{
					// 作成
					coTarget = new CartObject(
						this.UserId,
						this.OrderKbn,
						shopId,
						shippingType,
						product.IsDigitalContents,
						this.UpdateCartDb,
						this.MemberRankId,
						product.SubscriptionBoxCourseId)
					{
						PayPalCooperationInfo = this.PayPalCooperationInfo,
						FixedPurchase = this.FixedPurchase,
					// CartOwner紐付け（整合性があわなくなるのを防ぐ）
						Owner = this.Owner
					};

					// カートリストへひもづけ
					if (product.IsDigitalContents)
					{
						this.Items.Add(coTarget);
					}
					else
					{
						// [□カート1の配送先へ配送する] に対応するため
						// カートリストの後半部分はデジタルOnlyのカートにする
						for (int i = 0; i < this.Items.Count; i++)
						{
							if (this.Items[i].IsDigitalContentsOnly && product.IsRecommendItem == false)
							{
								this.Items.Insert(i, coTarget); // 途中に挿入
								break;
							}
						}
						if (this.Items.Contains(coTarget) == false) this.Items.Add(coTarget); // 最後に挿入
					}
				}

				var isCartListLp = (HttpContext.Current != null)
					&& (HttpContext.Current.Session != null)
					&& ((bool?)HttpContext.Current.Session[Constants.SESSION_KEY_FRONT_IS_CARTLIST_LP] ?? false);

				if (isCartListLp
					&& (HttpContext.Current.Session[Constants.SESSION_KEY_FRONT_LOGIN_USER_ID] != null))
				{
					coTarget.UpdateUserId((string)HttpContext.Current.Session[Constants.SESSION_KEY_FRONT_LOGIN_USER_ID]);
				}

				// 選択カートの場合、選択カートIDを設定
				if (isCartSelect)
				{
					product.CartIdSelect = product.CartId;
					product.CartId = coTarget.CartId;
				}

				// カートのユーザIDが空の場合に設定
				if (string.IsNullOrEmpty(coTarget.CartUserId))
					coTarget.CartUserId = string.IsNullOrEmpty(this.UserId)
						? string.Empty
						: this.UserId;

				//------------------------------------------------------
				// カートへ商品投入
				//------------------------------------------------------
				return coTarget.Add(product);
			}
		}

		/// <summary>
		/// カートへの商品セット投入処理（セットNo.更新）
		/// </summary>
		/// <param name="cpsProductSet">カートセット商品情報</param>
		public void AddProductSet(CartProductSet cpsProductSet)
		{
			// セットNo設定
			int iProductSetNo = 1;	// 初期値
			foreach (CartObject co in this.Items)
			{
				foreach (CartProduct cp in co.Items)
				{
					if (cp.IsSetItem)
					{
						if ((cp.ProductSet.ProductSetId == cpsProductSet.ProductSetId)
							&& (cp.ProductSet.ProductSetNo >= iProductSetNo)
							&& (cp.ProductSet.CartId == cpsProductSet.CartId))
						{
							iProductSetNo = cp.ProductSet.ProductSetNo + 1;
						}
					}
				}
			}
			cpsProductSet.ProductSetNo = iProductSetNo;

			foreach (CartProduct cp in cpsProductSet.Items)
			{
				if (Constants.CARTCOPY_OPTION_ENABLED)
				{
					AddProduct(cp, true);
				}
				else
				{
					AddProduct(cp);
				}

			}
		}

		/// <summary>
		/// カート追加（DB更新しない）
		/// </summary>
		/// <param name="CartObject">カート情報</param>
		public void AddCartVirtural(CartObject co)
		{
			this.Items.Add(co);
		}

		/// <summary>
		/// カート情報削除（オブジェクトのみ）
		/// </summary>
		/// <param name="CartObject">カート情報</param>
		public void DeleteCartVurtual(CartObject co)
		{
			// 先頭カート？
			if ((this.Items.IndexOf(co) == 0) && (this.Items.Count > 1))
			{
				CartObject coRemainedCart = this.Items[1];	// 遺されるカート

				//------------------------------------------------------
				// カート配送情報継承設定
				//------------------------------------------------------
				CartShipping csRemaindShipping = coRemainedCart.GetShipping();
				if (csRemaindShipping != null)
				{
					if (csRemaindShipping.IsSameShippingAsCart1)
					{
						// "非表示"であればカート1の配送先を継承
						coRemainedCart.SetShippingAddressOnly(co.GetShipping());
					}
					else
					{
						// カート1に繰り上がるのでカート1と等しいとする
						csRemaindShipping.IsSameShippingAsCart1 = true;
					}
				}

				//------------------------------------------------------
				// カート決済情報継承設定
				//------------------------------------------------------
				if (coRemainedCart.Payment != null)
				{
					if (coRemainedCart.Payment.IsSamePaymentAsCart1)
					{
						coRemainedCart.Payment = co.Payment;
					}
				}
			}

			//------------------------------------------------------
			// カート削除
			//------------------------------------------------------
			this.Items.Remove(co);
		}

		/// <summary>
		/// カート商品情報削除（商品数0になればカート自体も削除）
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID(nullのときはすべて削除)</param>
		public bool DeleteProduct(string strShopId, string strProductId, string strVariationId, Constants.AddCartKbn addCartKbn, string strProductSaleId)
		{
			foreach (CartObject co in this.Items)
			{
				// 削除処理（成功するとforeachコレクションの構造が変化するため、すぐ抜ける。）
				if (DeleteProduct(co.CartId, strShopId, strProductId, strVariationId, addCartKbn, strProductSaleId))
				{
					return true;
				}
			}

			return false;
		}
		/// <summary>
		/// カート商品情報削除（商品数0になればカート自体も削除）
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID(nullのときはすべて削除)</param>
		public bool DeleteProduct(string strShopId, string strProductId, string strVariationId, Constants.AddCartKbn addCartKbn, string strProductSaleId, string strProductOptionValue)
		{
			foreach (CartObject co in this.Items)
			{
				// 削除処理（成功するとforeachコレクションの構造が変化するため、すぐ抜ける。）
				if (DeleteProduct(co.CartId, strShopId, strProductId, strVariationId, addCartKbn, strProductSaleId, strProductOptionValue))
				{
					return true;
				}
			}

			return false;
		}
		/// <summary>
		/// カート商品情報削除（商品数0になればカート自体も削除）
		/// </summary>
		/// <param name="strCartId">カートID</param>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID(nullのときはすべて削除)</param>
		public bool DeleteProduct(string strCartId, DataRowView drvProduct, Constants.AddCartKbn addCartKbn, string strProductSaleId)
		{
			return DeleteProduct(strCartId, (string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID], (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID], addCartKbn, strProductSaleId);
		}
		/// <summary>
		/// カート商品情報削除（商品数0になればカート自体も削除）
		/// </summary>
		/// <param name="strCartId">カートID</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID(nullのときはすべて削除)</param>
		public bool DeleteProduct(string strCartId, string strShopId, string strProductId, string strVariationId, Constants.AddCartKbn addCartKbn, string strProductSaleId)
		{
			return DeleteProduct(strCartId, strShopId, strProductId, strVariationId, addCartKbn, strProductSaleId, "");
		}
		/// <summary>
		/// カート商品情報削除（商品数0になればカート自体も削除）
		/// </summary>
		/// <param name="strCartId">カートID</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID(nullのときはすべて削除)</param>
		public bool DeleteProduct(string strCartId, string strShopId, string strProductId, string strVariationId, Constants.AddCartKbn addCartKbn, string strProductSaleId, string strProductOptionValue)
		{
			// カート情報取得
			CartObject co = GetCart(strCartId);
			if (co == null)
			{
				return false;
			}

			// カート商品削除（商品数0になれば該当カートをカートリストから削除）
			bool blResult = co.RemoveProduct(strShopId, strProductId, strVariationId, addCartKbn, strProductSaleId, strProductOptionValue);
			if (co.Items.Count == 0)
			{
				DeleteCartVurtual(co);
			}

			return blResult;
		}

		/// <summary>
		/// カート商品セット情報削除（商品数0になればカート自体も削除）
		/// </summary>
		/// <param name="strProductSetId">商品セットID</param>
		/// <param name="iProductSetNo">商品セットNo</param>
		public bool DeleteProductSet(string strProductSetId, int iProductSetNo)
		{
			foreach (CartObject co in this.Items)
			{
				// 削除処理（成功するとforeachコレクションの構造が変化するため、すぐ抜ける。）
				if (DeleteProductSet(co.CartId, strProductSetId, iProductSetNo))
				{
					return true;
				}
			}

			return false;
		}
		/// <summary>
		/// カート商品セット情報削除（商品数0になればカート自体も削除）
		/// </summary>
		/// <param name="strCartId">カートID</param>
		/// <param name="strProductSetId">商品セットID</param>
		/// <param name="iProductSetNo">商品セットNo</param>
		public bool DeleteProductSet(string strCartId, string strProductSetId, int iProductSetNo)
		{
			// カート情報取得
			CartObject co = GetCart(strCartId);
			if (co == null)
			{
				return false;
			}

			// カート商品セット削除（商品数0になれば該当カートをカートリストから削除）
			bool blResult = co.RemoveProductSet(strProductSetId, iProductSetNo);
			if (co.Items.Count == 0)
			{
				DeleteCartVurtual(co);
			}

			return blResult;
		}

		/// <summary>
		/// カート分割基準変更チェック
		/// </summary>
		/// <returns>変更件数</returns>
		/// <remarks>分割基準などの値が更新されていたら再作成</remarks>
		public int UpdateProductCartDivTypeChanges()
		{
			int iResult = 0;

			if (this.UpdateCartDb == false)
			{
				return 0;
			}

			// カートに対してループ
			foreach (CartObject co in this.Items)
			{
				DataView dvCartProducts = co.GetCartProductsEither();

				// カート内の商品に対してループ
				foreach (CartProduct cp in co.Items)
				{
					var drvProduct = OrderCommon.GetCartProductFromDataView(
						dvCartProducts,
						cp.ShopId,
						cp.ProductId,
						cp.VariationId,
						(cp.IsFixedPurchase || cp.IsSubscriptionBox),
						cp.ProductSaleId);

					// 配送種別orデジタルコンテンツフラグ が変更された商品がある？
					if ((co.ShippingType != (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE])
						|| co.IsDigitalContentsOnly != ((string)drvProduct[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID))
					{
						// セット商品だったらセット削除
						if (cp.IsSetItem)
						{
							co.RemoveProductSet(cp.ProductSet.ProductSetId, cp.ProductSet.ProductSetNo);

							// 商品数0になれば該当カートをカートリストから削除
							if (co.Items.Count == 0)
							{
								DeleteCartVurtual(co);
							}
						}
						// 通常商品だったら別カートへ
						else
						{
							// 商品数を知りたいため、削除対象商品データ取得
							List<CartProduct> lDeleteCartProducts = new List<CartProduct>();
							foreach (CartProduct cpTemp in co.Items)
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
								// 元のカートから該当商品削除
								co.RemoveProduct(drvProduct, cpDelete.AddCartKbn, cpDelete.ProductSaleId, cpDelete.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());

								// 商品数0になれば該当カートをカートリストから削除
								if (co.Items.Count == 0)
								{
									DeleteCartVurtual(co);
								}

								// カート追加（別のカートに追加されると思われる）
								AddProduct(new CartProduct(drvProduct, cpDelete.AddCartKbn, "", cpDelete.CountSingle, this.UpdateCartDb, cpDelete.ProductOptionSettingList));
							}
						}

						iResult++;
					}

					// カートの数が変わっているかもしれないので、breakして再帰処理でもう一回チェックする
					if (iResult != 0)
					{
						break;
					}
				}

				// カートの数が変わっているかもしれないので、breakして再帰処理でもう一回チェックする
				if (iResult != 0)
				{
					break;
				}

			} //foreach (CartObject co in this.Items)

			// もう一回チェック
			if (iResult != 0)
			{
				iResult += UpdateProductCartDivTypeChanges();
			}

			return iResult;
		}

		/// <summary>
		/// カート内の商品情報がマスタから削除されているかチェック（セット商品以外）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// カート内の商品情報がマスタから削除されている場合、その商品をカート情報から削除
		/// 戻り値にどの商品が削除されたのかをメッセージとして返す
		/// </remarks>
		public string CheckProductDeleted()
		{
			var sbErrorMessages = new StringBuilder();

			//------------------------------------------------------
			// foreach用のリスト作成（削除される可能性があるので）
			//------------------------------------------------------
			var lCartObjects = new List<CartObject>();
			foreach (var co in this.Items)
			{
				lCartObjects.Add(co);
			}

			foreach (var co in lCartObjects)
			{
				if (this.UpdateCartDb)
				{
					// 頒布会は同一カートに頒布会以外の商品が入らないためカートオブジェクト単位で確認
					if (co.IsSubscriptionBox)
					{
						var subscriptionBox = new SubscriptionBoxService().GetByCourseId(co.SubscriptionBoxCourseId);

						// コースIDが存在しない or 無効になっている場合対象商品をすべて削除
						if ((subscriptionBox == null)
							|| (subscriptionBox.IsValid == false))
						{
							DeleteCartVurtual(co);
							// エラーメッセージ取得
							sbErrorMessages
								.Append(
									string.Format(
										CommerceMessages.GetMessages(
											CommerceMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_NOT_MEET_PERIOD_NUMBERTIME),
											co.SubscriptionBoxDisplayName))
								.Append("\r\n");

							// カート情報が削除されるため、
							// カートリスト情報にカート情報が無い場合はループを抜ける必要がある
							if (this.Items.Count == 0)
							{
								break;
							}

							// カートの中身を削除しているため次のカートへ
							continue;
						}
					}

					//------------------------------------------------------
					// カート商品情報取得（未削除商品）
					//------------------------------------------------------
					var dvProducts = new CartService().GetProductForDeleteCheck(co.CartId);

					//------------------------------------------------------
					// 削除商品が見つかれば削除＆エラーメッセージ生成
					//------------------------------------------------------
					foreach (DataRowView drvProduct in dvProducts)
					{
						if (drvProduct[Constants.FIELD_PRODUCT_NAME] == DBNull.Value)
						{
							// カートから商品情報削除（商品数0になればカート自体も削除）
							DeleteProduct(co.CartId, drvProduct, Constants.AddCartKbn.Normal, "");
							DeleteProduct(co.CartId, drvProduct, Constants.AddCartKbn.FixedPurchase, "");
							DeleteProduct(co.CartId, drvProduct, Constants.AddCartKbn.GiftOrder, "");

							// エラーメッセージ取得
							sbErrorMessages.Append(OrderCommon.GetErrorMessage(OrderErrorcode.ProductDeleted, (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])).Append("\r\n");
						}
					}

					// 商品削除中にカート情報が削除されるため、
					// カートリスト情報にカート情報が無い場合はループを抜ける必要がある
					if (this.Items.Count == 0)
					{
						break;
					}
				}
			}

			return sbErrorMessages.ToString();
		}

		/// <summary>
		/// カート内のセット商品の整合性が取れているかチェック
		/// </summary>
		/// <param name="blUpdateCart">カート更新あり</param>
		/// <returns>エラーメッセージ</returns>
		public string CheckProductSet(bool blUpdateCart)
		{
			StringBuilder sbErrorMessages = new StringBuilder();
			foreach (CartObject co in this.Items)
			{
				string strErrorMessage = co.CheckProductSet(blUpdateCart);
				if (strErrorMessage.Length != 0)
				{
					sbErrorMessages.Append(strErrorMessage);

					// 商品数0になれば該当カートをカートリストから削除し、再帰呼び出し
					if (co.Items.Count == 0)
					{
						this.DeleteCartVurtual(co);	// 削除

						CheckProductSet(blUpdateCart);	// 再帰呼び出し
						break;
					}
				}
			}

			return sbErrorMessages.ToString();
		}

		/// <summary>
		/// カート内の商品の配送先割振前の商品数を設定
		/// </summary>
		/// <remarks>主にギフト商品への用途</remarks>
		public void SetProductCountBeforeDivide()
		{
			foreach (var product in this.Items.SelectMany(cart => cart.Items))
			{
				product.SetCountBeforeDivide();
			}
		}

		/// <summary>
		/// カートのDELETE処理
		/// </summary>
		/// <param name="strCartId">カートID</param>
		/// <returns>処理件数</returns>
		public static int DeleteCartDB(string strCartId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement SqlStatement = new SqlStatement("Cart", "DeleteCart"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_CART_CART_ID, strCartId);           // カートID

				// ステートメント実行
				return SqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 注文者情報を設定（※全てのカート情報が同じ注文者情報を参照する）
		/// </summary>
		/// <param name="coOwner">注文者情報</param>
		public void SetOwner(CartOwner coOwner)
		{
			this.Owner = coOwner;

			foreach (CartObject co in this.Items)
			{
				co.Owner = coOwner;
			}
		}

		/// <summary>
		/// デフォルト注文方法を設定
		/// </summary>
		/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
		/// <param name="hasSelectedShippingMethod">選択された配送方法あり</param>
		public void SetDefaultOrderSettingForUserDefaultOrder(UserDefaultOrderSettingModel userDefaultOrderSetting, bool hasSelectedShippingMethod)
		{
			// カート情報が0件の場合、デフォルト注文方法の設定は行わない
			if (this.Items.Count == 0) return;

			var paymentId = userDefaultOrderSetting.PaymentId;
			var shippingNo = userDefaultOrderSetting.UserShippingNo.ToString();
			// 既定の配送先情報が存在する、かつ選択された配送方法なしの場合、デフォルト配送先方法を設定する
			if (string.IsNullOrEmpty(shippingNo) == false && (hasSelectedShippingMethod == false))
			{
				SetDefaultOrderShippingForUserDefaultOrder(shippingNo);
			}
			// 既定の支払方法が存在する、かつ選択された支払方法なしの場合、デフォルト支払方法を設定する
			if (string.IsNullOrEmpty(paymentId) == false)
			{
				SetUserDefaultOrderPaymentForUserDefaultOrder(userDefaultOrderSetting, paymentId);
			}

			// Set Default Order Invoice For User Default Order
			var invoiceNo = userDefaultOrderSetting.UserInvoiceNo.ToString();
			if (OrderCommon.DisplayTwInvoiceInfo()
				&& (string.IsNullOrEmpty(invoiceNo) == false))
			{
				SetDefaultOrderInvoiceForUserDefaultOrder(invoiceNo);
			}

			// 注文メモを設定
			SetOrderMemoForUserDefaultOrder();

			SetOrderExtendForUserDefaultOrder();
		}

		/// <summary>
		/// デフォルト支払方法設定が存在するかの判定を設定
		/// </summary>
		/// <param name="userDefaultOrderSetting">デフォルト注文方法設定情報</param>
		public void SetDefaultPaymentExistForUserDefaultOrder(UserDefaultOrderSettingModel userDefaultOrderSetting)
		{
			this.UserDefaultOrderSettingParm.IsUserDefaultPaymentExist
				= ((userDefaultOrderSetting != null) && (userDefaultOrderSetting.PaymentId != null));
		}

		/// <summary>
		/// 注文者情報設定
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		private void SetOwnerForUserDefaultOrder(UserModel user)
		{
			this.SetOwner(new CartOwner(user));
		}

		/// <summary>
		/// 注文メモを設定
		/// </summary>
		private void SetOrderMemoForUserDefaultOrder()
		{
			this.Items.ToList().ForEach(cartObj => cartObj.CreateOrderMemo(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC));
		}

		/// <summary>
		/// 注文拡張項目を設定
		/// </summary>
		private void SetOrderExtendForUserDefaultOrder()
		{
			this.Items.ToList().ForEach(cartObj => cartObj.CreateOrderExtend());
		}

		/// <summary>
		/// デフォルト支払方法を設定
		/// </summary>
		/// <param name="userDefaultOrderSetting">デフォルト注文方法設定情報</param>
		/// <param name="paymentId">決済種別ID</param>
		private void SetUserDefaultOrderPaymentForUserDefaultOrder(UserDefaultOrderSettingModel userDefaultOrderSetting, string paymentId)
		{
			var paymentName = DataCacheControllerFacade.GetPaymentCacheController().Get(paymentId).PaymentName;
			var cartPayment = new CartPayment();
			var creditCard = (userDefaultOrderSetting.CreditBranchNo != null)
				? UserCreditCard.Get(this.UserId, userDefaultOrderSetting.CreditBranchNo.Value)
				: null;

			// デフォルト支払設定がクレジットカードの場合、決済情報（クレジットカード用）更新
			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				string creditInstallmentsCode = null;

				try
				{
					creditInstallmentsCode = OrderCommon.GetCreditInstallmentsDefaultValue();
				}
				catch (Exception ex)
				{
					creditInstallmentsCode = Constants.FIELD_CREDIT_INSTALLMENTS_VALUE;
					FileLogger.WriteError(ex.Message);
				}

				cartPayment.UpdateCartPayment(
					paymentId,
					paymentName,
					userDefaultOrderSetting.CreditBranchNo.ToString(),
					creditCardCompanyCode: null,
					creditCardNo1: null,
					creditCardNo2: null,
					creditCardNo3: null,
					creditCard.LastFourDigit,
					creditCard.ExpirationMonth,
					creditCard.ExpirationYear,
					creditInstallmentsCode,
					creditSecurityCode: string.Empty,
					creditCard.AuthorName,
					paymentObject: null,
					isSamePaymentAsCart1: false,
					rakutenCvvToken: string.Empty,
					null);
			}
			else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			{
				cartPayment.UpdateCartPayment(
					paymentId,
					paymentName,
					CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,
					creditCardCompanyCode: null,
					creditCardNo1: null,
					creditCardNo2: null,
					creditCardNo3: null,
					creditCardNo4: null,
					creditExpireMonth: null,
					creditExpireYear: null,
					creditInstallmentsCode: null,
					creditSecurityCode: string.Empty,
					creditAuthorName: null,
					paymentObject: null,
					isSamePaymentAsCart1: false,
					rakutenCvvToken: null,
					newebPayCreditInstallmentsCode: NewebPayConstants.FLG_CREDIT_CARD_ONCE_TIME,
					creditBincode: string.Empty);
			}
			else if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& OrderCommon.IsPaymentCvsTypeZeus)
			{
				cartPayment.UpdateCartPayment(
					paymentId,
					paymentName,
					creditCardBranchNo: null,
					creditCardCompanyCode: null,
					creditCardNo1: null,
					creditCardNo2: null,
					creditCardNo3: null,
					creditCardNo4: null,
					creditExpireMonth: null,
					creditExpireYear: null,
					creditInstallmentsCode: null,
					creditSecurityCode: string.Empty,
					creditAuthorName: null,
					paymentObject: new PaymentZeusCvs(userDefaultOrderSetting.ZeusCvsType),
					isSamePaymentAsCart1: false,
					newebPayCreditInstallmentsCode: NewebPayConstants.FLG_CREDIT_CARD_ONCE_TIME,
					rakutenCvvToken: string.Empty);
			}
			else if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& OrderCommon.IsPaymentCvsTypePaygent)
			{
				cartPayment.UpdateCartPayment(
					paymentId,
					paymentName,
					creditCardBranchNo: null,
					creditCardCompanyCode: null,
					creditCardNo1: null,
					creditCardNo2: null,
					creditCardNo3: null,
					creditCardNo4: null,
					creditExpireMonth: null,
					creditExpireYear: null,
					creditInstallmentsCode: null,
					creditSecurityCode: string.Empty,
					creditAuthorName: null,
					paymentObject: new PaymentPaygentCvs(userDefaultOrderSetting.PaygentCvsType),
					isSamePaymentAsCart1: false,
					newebPayCreditInstallmentsCode: string.Empty,
					rakutenCvvToken: string.Empty);
			}
			else if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten))
			{
				cartPayment.UpdateCartPayment(
					paymentId,
					paymentName,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					string.Empty,
					null,
					null,
					false,
					string.Empty,
					string.Empty,
					string.Empty,
					userDefaultOrderSetting.RakutenCvsType);
			}
			else
			{
				cartPayment.UpdateCartPayment(
					paymentId,
					paymentName,
					(paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
						? userDefaultOrderSetting.CreditBranchNo.ToString()
						: null,
					creditCardCompanyCode: null,
					creditCardNo1: null,
					creditCardNo2: null,
					creditCardNo3: null,
					creditCardNo4: null,
					creditExpireMonth: null,
					creditExpireYear: null,
					creditInstallmentsCode: null,
					creditSecurityCode: string.Empty,
					creditAuthorName: null,
					paymentObject: null,
					false,
					rakutenCvvToken: string.Empty);
			}

			// 全てのカートにコピー
			foreach (var cart in this.Items.Where(cart => (cart.Payment == null)))
			{
				cart.Payment = cartPayment.Clone();
			}

			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				var userCreditCard = UserCreditCard.Get(this.UserId, userDefaultOrderSetting.CreditBranchNo.Value);
				this.PayPalCooperationInfo = new PayPalCooperationInfo(userCreditCard);
			}

		}

		/// <summary>
		/// デフォルト配送先方法を設定
		/// </summary>
		/// <param name="shippingNo">配送先枝番</param>
		public void SetDefaultOrderShippingForUserDefaultOrder(string shippingNo)
		{
			var userDefaultShipping = GetUserForUserDefaultOrder(shippingNo);
			// 注文者情報設定
			SetOwnerForUserDefaultOrder(new UserService().Get(this.UserId));
			// カート配送先住所情報設定
			SetShippingAddrForUserDefaultOrder(userDefaultShipping);
			// カート配送日設定
			SetShippingDateTimeForUserDefaultOrder();
			// 配送方法設定
			SetShippingMethodForUserDefaultOrder();
			// 配送先枝番設定
			SetAddrKbnForUserDefaultOrder(shippingNo);
			// 別出荷フラグ設定
			SetAnotherShippingFlag();
		}

		/// <summary>
		/// Set Default Order Invoice For User Default Order
		/// </summary>
		/// <param name="invoiceNo">Invoice No</param>
		private void SetDefaultOrderInvoiceForUserDefaultOrder(string invoiceNo)
		{
			var userDefaultInvoice = new TwUserInvoiceService().Get(this.UserId, int.Parse(invoiceNo));
			if (userDefaultInvoice != null)
			{
				foreach (var cart in this.Items.Where(cart => string.IsNullOrEmpty(cart.Shippings[0].UniformInvoiceType)))
				{
					cart.Shippings[0].UpdateInvoice(
						userDefaultInvoice.TwUniformInvoice,
						userDefaultInvoice.TwUniformInvoiceOption1,
						userDefaultInvoice.TwUniformInvoiceOption2,
						userDefaultInvoice.TwCarryType,
						userDefaultInvoice.TwCarryTypeOption);
				}
			}
		}

		/// <summary>
		/// カート配送先住所情報設定
		/// </summary>
		/// <param name="userDefaultShipping">既定の配送先情報</param>
		private void SetShippingAddrForUserDefaultOrder(UserModel userDefaultShipping)
		{
			this.Items.ToList().ForEach(cartObj => UpdateShippingAddrForUserDefaultOrder(userDefaultShipping, cartObj));
		}

		/// カート配送日更新
		/// </summary>
		private void SetShippingDateTimeForUserDefaultOrder()
		{
			var dateSetFlg = OrderCommon.GetShopShipping(Constants.CONST_DEFAULT_SHOP_ID, this.Items[0].ShippingType).ShippingDateSetFlg;
			var timeSetFlg = (this.Items[0].Shippings[0].DeliveryCompanyId != null)
				? new DeliveryCompanyService().Get(this.Items[0].Shippings[0].DeliveryCompanyId).ShippingTimeSetFlg : Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID;
			this.Items.ToList().ForEach(cartObj => UpdateShippingDateTimeForUserDefaultOrder(cartObj, dateSetFlg, timeSetFlg));
		}

		/// <summary>
		/// 配送方法設定
		/// </summary>
		private void SetShippingMethodForUserDefaultOrder()
		{
			this.Items.ToList().ForEach(UpdateShippingMethodForUserDefaultOrder);
		}

		/// <summary>
		/// 配送先枝番設定
		/// </summary>
		/// <param name="shippingNo">配送先枝番</param>
		private void SetAddrKbnForUserDefaultOrder(string shippingNo)
		{
			var isAddressShippingNo = ((shippingNo != Constants.FLG_USERSHIPPING_OWNER_SHIPPING_NO) && (shippingNo != ""));
			if (isAddressShippingNo)
			{
				this.Items.ToList().ForEach(
					cartObj =>
					{
						cartObj.Shippings[0].ShippingNo = cartObj.Shippings[0].ShippingAddrKbn = shippingNo;
					});
			}
		}

		/// <summary>
		// 別出荷フラグ設定
		/// </summary>
		private void SetAnotherShippingFlag()
		{
			this.Items.ToList().ForEach(cartObj => cartObj.UpdateAnotherShippingFlag());
		}

		/// <summary>
		/// カート配送先住所情報更新
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="cartObj">カートオブジェクト</param>
		private void UpdateShippingAddrForUserDefaultOrder(UserModel user, CartObject cartObj)
		{
			cartObj.Shippings[0].UpdateShippingAddr(
				user.Name1,
				user.Name2,
				user.NameKana1,
				user.NameKana2,
				user.Zip,
				user.Zip1,
				user.Zip2,
				user.Addr1,
				user.Addr2,
				user.Addr3,
				user.Addr4,
				user.Addr5,
				user.AddrCountryIsoCode,
				user.AddrCountryName,
				user.CompanyName,
				user.CompanyPostName,
				user.Tel1,
				user.Tel1_1,
				user.Tel1_2,
				user.Tel1_3,
				true,
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);

			// カートの配送先がコンビニでない場合かつ注文方法にコンビニが選択されている場合は設定する
			if ((cartObj.Shippings[0].ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				&& (user.DefaultConvenienceStoreUseFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
			{
				cartObj.Shippings[0].ConvenienceStoreFlg = user.DefaultConvenienceStoreUseFlg;
				cartObj.Shippings[0].ConvenienceStoreId = user.DefaultConvenienceStoreId;
				cartObj.Shippings[0].ShippingReceivingStoreType = user.DefaultShippingReceivingStoreType;
			}
		}

		/// <summary>
		/// カート配送日更新
		/// </summary>
		/// <param name="cartObj">カートオブジェクト</param>
		/// <param name="dateSetFlg">配送希望日</param>
		/// <param name="timeSetFlg">時間帯設定</param>
		private void UpdateShippingDateTimeForUserDefaultOrder(CartObject cartObj, string dateSetFlg, string timeSetFlg)
		{
			cartObj.Shippings[0].UpdateShippingDateTime(
				(dateSetFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID),
				(timeSetFlg == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID),
				cartObj.Shippings[0].ShippingDate,
				null,
				null);
		}

		/// <summary>
		/// 配送方法更新
		/// </summary>
		/// <param name="cartObj">カートオブジェクト</param>
		private void UpdateShippingMethodForUserDefaultOrder(CartObject cartObj)
		{
			var shopShipping = new ShopShippingService().Get(cartObj.ShopId, cartObj.ShippingType);
			var shippingMethod = ((cartObj.Payment != null)
				&& (cartObj.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
				&& cartObj.Shippings[0].IsMail)
					? Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS
					: OrderCommon.GetShippingMethod(cartObj.Items);
			var shippingCompanyId = ((shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail).First(i => i.IsDefault).DeliveryCompanyId;
			cartObj.Shippings[0].UpdateShippingMethod(shippingMethod, shippingCompanyId);
		}

		/// <summary>
		/// デフォルト配送先方法取得
		/// </summary>
		/// <param name="shippingNo">配送先枝番</param>
		/// <returns>ユーザー情報</returns>
		private UserModel GetUserForUserDefaultOrder(string shippingNo)
		{
			var user = new UserService().Get(this.UserId);
			int outShippingNo;
			int.TryParse(shippingNo, out outShippingNo);
			var userShipping = new UserShippingService().Get(this.UserId, outShippingNo);
			var isOwnerShippingNo = (shippingNo == Constants.FLG_USERSHIPPING_OWNER_SHIPPING_NO);
			var isAddressShippingNo = ((isOwnerShippingNo == false) && (shippingNo != string.Empty) && (userShipping != null));
			var defaultConvenienceStoreId =
				(userShipping != null) ? userShipping.ShippingReceivingStoreId : string.Empty;
			var defaultConvenienceStoreUseFlg
				= (userShipping != null)
					? userShipping.ShippingReceivingStoreFlg
					: Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
			var defaultShippingReceivingStoreType = (userShipping != null)
				? userShipping.ShippingReceivingStoreType
				: string.Empty;

			return new UserModel
			{
				UserKbn = user.UserKbn,
				Name1 = (isOwnerShippingNo) ? user.Name1 : (isAddressShippingNo) ? userShipping.ShippingName1 : string.Empty,
				Name2 = (isOwnerShippingNo) ? user.Name2 : (isAddressShippingNo) ? userShipping.ShippingName2 : string.Empty,
				NameKana1 = (isOwnerShippingNo) ? user.NameKana1 : (isAddressShippingNo) ? userShipping.ShippingNameKana1 : string.Empty,
				NameKana2 = (isOwnerShippingNo) ? user.NameKana2 : (isAddressShippingNo) ? userShipping.ShippingNameKana2 : string.Empty,
				MailAddr = user.MailAddr,
				MailAddr2 = user.MailAddr2,
				Zip = (isOwnerShippingNo) ? user.Zip : (isAddressShippingNo) ? userShipping.ShippingZip : string.Empty,
				Zip1 = (isOwnerShippingNo) ? user.Zip1 : (isAddressShippingNo) ? userShipping.ShippingZip1 : string.Empty,
				Zip2 = (isOwnerShippingNo) ? user.Zip2 : (isAddressShippingNo) ? userShipping.ShippingZip2 : string.Empty,
				Addr1 = (isOwnerShippingNo) ? user.Addr1 : (isAddressShippingNo) ? userShipping.ShippingAddr1 : string.Empty,
				Addr2 = (isOwnerShippingNo) ? user.Addr2 : (isAddressShippingNo) ? userShipping.ShippingAddr2 : string.Empty,
				Addr3 = (isOwnerShippingNo) ? user.Addr3 : (isAddressShippingNo) ? userShipping.ShippingAddr3 : string.Empty,
				Addr4 = (isOwnerShippingNo) ? user.Addr4 : (isAddressShippingNo) ? userShipping.ShippingAddr4 : string.Empty,
				Addr5 = (isOwnerShippingNo) ? user.Addr5 : (isAddressShippingNo) ? userShipping.ShippingAddr5 : string.Empty,
				AddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
					? (isOwnerShippingNo) ? user.AddrCountryIsoCode : (isAddressShippingNo) ? userShipping.ShippingCountryIsoCode : string.Empty
					: string.Empty,
				AddrCountryName = Constants.GLOBAL_OPTION_ENABLE
					? (isOwnerShippingNo) ? user.AddrCountryName : (isAddressShippingNo) ? userShipping.ShippingCountryName : string.Empty
					: string.Empty,
				CompanyName = user.CompanyName,
				CompanyPostName = user.CompanyPostName,
				Tel1 = (isOwnerShippingNo) ? user.Tel1 : (isAddressShippingNo) ? userShipping.ShippingTel1 : string.Empty,
				Tel1_1 = (isOwnerShippingNo) ? user.Tel1_1 : (isAddressShippingNo) ? userShipping.ShippingTel1_1 : string.Empty,
				Tel1_2 = (isOwnerShippingNo) ? user.Tel1_2 : (isAddressShippingNo) ? userShipping.ShippingTel1_2 : string.Empty,
				Tel1_3 = (isOwnerShippingNo) ? user.Tel1_3 : (isAddressShippingNo) ? userShipping.ShippingTel1_3 : string.Empty,
				Tel2_1 = user.Tel2_1,
				Tel2_2 = user.Tel2_2,
				Tel2_3 = user.Tel2_3,
				Sex = user.Sex,
				Birth = user.Birth,
				DefaultConvenienceStoreId = defaultConvenienceStoreId,
				DefaultConvenienceStoreUseFlg = defaultConvenienceStoreUseFlg,
				DefaultShippingReceivingStoreType = defaultShippingReceivingStoreType
			};
		}

		/// <summary>
		/// 配送方法自動判定(商品同梱を考慮))
		/// </summary>
		/// <param name="fixedPurchase">FixedPurchase Model</param>
		public void SetShippingMethod(FixedPurchaseModel fixedPurchase)
		{
			if (fixedPurchase.Shippings[0].ShippingMethod != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL) return;

			OrderCommon.SetShippingMethod(
				this.Items,
				this.UserId,
				fixedPurchase.OrderPaymentKbn,
				fixedPurchase.Shippings[0].ShippingMethod,
				string.Empty,
				string.Empty);
		}

		/// <summary>
		/// 配送不可エリアか
		/// </summary>
		/// <param name="cartList">カートオブジェクト</param>
		/// <returns>配送不可エリアならtrue</returns>
		public bool CheckUnavailableShippingArea(CartObjectList cartList)
		{
			var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
				cartList.Items[0].ShippingType,
				cartList.Items[0].Shippings[0].DeliveryCompanyId);
			var shippingZip = cartList.Items[0].Shippings[0].HyphenlessZip;

			return OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, shippingZip);
		}

		/// <summary>
		/// クレジットトークン有効期限チェック
		/// </summary>
		/// <param name="clearTokenIfExpired">チェックエラーでトークンをクリアするか</param>
		/// <returns>チェックOKか</returns>
		public bool CheckCreditTokenExpired(bool clearTokenIfExpired)
		{
			if (OrderCommon.CreditTokenUse == false) return true;

			var result = true;
			var payments =
				this.Items.Select(cart => cart.Payment)
					.Where(payment => payment != null)
					.Where(payment => payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT).ToList();
			payments.ForEach(
				payment =>
				{
					// 有効期限チェック（カート1と同じ決済トークンはチェックしない）
					if ((payment.CreditToken != null)
						&& payment.CreditToken.IsExpired)
					{
						result = false;
						if (clearTokenIfExpired) payment.CreditToken = null;
					}
				});
			return result;
		}

		/// <summary>
		/// カート一覧の商品合計数取得
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <returns>カート一覧の商品合計数</returns>
		public int GetProductCountTotalInCartList(CartProduct cartProduct)
		{
			return GetSameProductsInCartList(cartProduct).Sum(product => product.Count);
		}

		/// <summary>
		/// カート情報の商品合計数取得
		/// </summary>
		/// <param name="cartObject">カート情報</param>
		/// <param name="cartProduct">カート商品</param>
		/// <returns>カート情報の商品合計数</returns>
		public int ProductCountTotalInCart(CartObject cartObject, CartProduct cartProduct)
		{
			return GetSameProductsInCart(cartObject, cartProduct).Sum(product => product.Count);
		}

		/// <summary>
		/// カート一覧の商品情報取得
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <returns>カート一覧の商品情報</returns>
		private List<CartProduct> GetSameProductsInCartList(CartProduct cartProduct)
		{
			var products = new List<CartProduct>();
			this.Items.ToList().ForEach(cartObj => products.AddRange(GetSameProductsInCart(cartObj, cartProduct)));

			return products;
		}

		/// <summary>
		/// カート情報の商品情報取得
		/// </summary>
		/// <param name="cartObject">カート情報</param>
		/// <param name="cartProduct">カート商品</param>
		/// <returns>カート情報の商品情報</returns>
		private List<CartProduct> GetSameProductsInCart(CartObject cartObject, CartProduct cartProduct)
		{
			var cartProducts = cartObject.Items.FindAll(product => (product.ShopId == cartProduct.ShopId)
				&& (product.ProductId == cartProduct.ProductId)
				&& (product.VariationId == cartProduct.VariationId));

			return cartProducts;
		}

		/// <summary>
		/// 購入商品を過去に購入したことがあるか（類似配送先を含む）
		/// </summary>
		/// <returns>ユーザーに重複情報が含まれるか</returns>
		public bool CheckFixedProductOrderLimit()
		{
			// DBに対してチェックを実施
			this.Items.ForEach(cart => cart.CheckProductOrderLimit());

			// カート内でチェックを実施
			var isCartCheckResult = CheckCartFixedProductOrder();

			return (this.HasOrderHistorySimilarShipping || isCartCheckResult);
		}

		/// <summary>
		/// 購入商品を過去に購入したことがあるか（類似配送先を含む）
		/// （カート間チェック）
		/// </summary>
		/// <returns>ユーザーに重複情報が含まれるか</returns>
		private bool CheckCartFixedProductOrder()
		{
			foreach (var cart in this.Items)
			{
				// チェック対象の商品がなければ次のカート
				var cartItems = string.IsNullOrEmpty(cart.OrderCombineParentOrderId)
					? cart.Items
					: cart.TargetProductListForCheckProductOrderLimit;
				var targetProducts = cartItems.Where(product => product.IsOrderLimitProduct).ToArray();

				if (targetProducts.Length == 0) continue;

				// 他のカートにチェック対象の商品がなければ次のカート
				var targetCarts = this.Items.Where(targetCart =>
					targetCart.Items.Any(product =>
						targetProducts.Any(targetProduct => targetProduct.IsCartProductLimit(product))));

				// 配送先のチェック
				var targetShippings = targetCarts.Where(targetCart => targetCart.CartId != cart.CartId).SelectMany(
					targetCart => targetCart.Shippings.Select(
						targetShipping => targetShipping.CreateOrderShipping())).ToArray();
				var shippings = cart.Shippings.Select(shipping => shipping.CreateOrderShipping()).ToArray();

				cart.HasNotFirstTimeByCart = targetShippings.Any(
					targetShipping => shippings.Any(
						shipping => new OrderShippingMatching().MatchOrderShipping(targetShipping, shipping)));
				cart.IsOrderLimit |= cart.HasNotFirstTimeByCart;
			}

			var result = this.Items.Any(cart => cart.HasNotFirstTimeByCart);
			return result;
		}

		/// <summary>
		/// カート情報から注文情報生成
		/// </summary>
		/// <param name="orderOld">元注文情報</param>
		/// <param name="orderOwnerUserId">注文者ユーザーID</param>
		/// <returns>注文情報</returns>
		public OrderModel[] CreateOrder(OrderModel[] orderOld, string orderOwnerUserId = "")
		{
			var orders = this.Items.Select(item =>
				orderOld.Any(old => old.OrderId == item.OrderId)
					? item.CreateNewOrder(orderOld.First(old => old.OrderId == item.OrderId))
					: item.CreateNewOrder()).ToArray();
			return orders;
		}

		/// <summary>
		/// ユーザが配送方法を未確定の場合は「Cart_ShippingMethod_UnSelected_Priority」の優先度より配送方法を確定
		/// </summary>
		public void CartListShippingMethodUserUnSelected()
		{
			var shopShippingCacheController = DataCacheControllerFacade.GetShopShippingCacheController();
			foreach (var cartObject in this.Items)
			{
				var shopShipping = shopShippingCacheController.Get(cartObject.ShippingType);
				cartObject.CartShippingMethodUserUnSelected(shopShipping);
			}
		}

		/// <summary>
		/// 定期会員フラグ設定
		/// </summary>
		/// <param name="fixedPurchaseMemberFlg">定期会員フラグ</param>
		public void SetFixedPurchaseMemberFlgForCartObject(string fixedPurchaseMemberFlg)
		{
			this.Items.ForEach(cart => cart.IsFixedPurchaseMember = (fixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON));
		}

		/// <summary>
		/// Check Item Relate With Service Convenience Store
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>True: If Relate Or Flase: If Not Relate</returns>
		private bool CheckItemRelateWithServiceConvenienceStore(CartObject cart)
		{
			var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cart.ShippingType);

			var deliveryCompanyIds = shopShipping.CompanyList
				.Select(model => model.DeliveryCompanyId)
				.Distinct()
				.ToArray();

			return deliveryCompanyIds.Any(item => item == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);
		}

		/// <summary>
		/// 配送先情報を同期する
		/// </summary>
		public void UpdateShipping()
		{
			foreach (var co in this.Items)
			{
				foreach (var cs in co.Shippings)
				{
					// 配送先の同期
					if (cs.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
					{
						//「配送先が注文者の住所」となっている配送先を同期
						cs.UpdateShippingAddr(this.Owner, false);
					}

					// 送り主の同期
					if ((co.Shippings.Count > 1) && cs.IsSameSenderAsShipping1)
					{
						//「配送先1と同じ送り主を指定する」となっている送り主を同期
						cs.UpdateSenderAddr(cs.CartObject.Shippings[0], true);
					}
					else if ((cs.SenderAddrKbn == CartShipping.AddrKbn.Owner)
						&& (cs.IsSameSenderAsShipping1 == false))
					{
						//「注文者を送り主とする」となっている送り主を同期
						cs.UpdateSenderAddr(this.Owner, false);
					}
				}
			}
		}

		/// <summary>
		/// DBと同期（オブジェクトを正とし、DBへと格納する）
		/// </summary>
		public void SyncCartDb()
		{
			foreach (var cart in this.Items)
			{
				DeleteCartDB(cart.CartId);

				cart.SyncProductDb();
			}
		}

		/// <summary>
		/// Is Update FixedPurchase Shipping Pattern
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <param name="fixedPurchaseKbn">Fixed Purchase Kbn</param>
		/// <param name="fixedPurchaseSetting">Fixed Purchase Setting</param>
		public bool IsUpdateFixedPurchaseShippingPattern(
			CartObject cart,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting)
		{
			return cart.IsUpdateFixedPurchaseShippingPattern(
				cart.Items,
				fixedPurchaseKbn,
				fixedPurchaseSetting);
		}

		/// <summary>
		/// 配送間隔日を取得
		/// ■取得ルール
		/// 配送日間隔指定：登録されている最初の日数
		/// 月間隔日付指定：購入日から１ヶ月後
		/// 週・曜日指定：購入日から１ヶ月後
		/// </summary>
		/// <param name="shipping">配送種別マスタ</param>
		/// <param name="fixedPurchaseKbnFlg">配送間隔パターン</param>
		/// <returns>配送間隔日</returns>
		private string GetFixedPurchaseSetting(ShopShippingModel shipping, string fixedPurchaseKbnFlg)
		{
			// 配送日間隔指定
			if (fixedPurchaseKbnFlg == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
			{
				// 登録されている最初の日数を取得
				var fixedPurchaseKbn = shipping.FixedPurchaseKbn3Setting.Split(',').First();
				return fixedPurchaseKbn;
			}
			// 月間隔日付指定
			else if (fixedPurchaseKbnFlg == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
			{
				// 購入日から１ヶ月後を設定
				var day = (DateTime.Today.Day > 28) ? -1 : DateTime.Today.Day;
				var fixedPurchaseKbn = string.Format("{0},{1}",
					shipping.FixedPurchaseKbn1Setting.Split(',').First(),
					ValueText.GetValueText(
						Constants.TABLE_SHOPSHIPPING,
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST,
						day.ToString()));
				return fixedPurchaseKbn;
				}
			// 週・曜日指定
			else
			{
				var nextMonthDate = DateTime.Today.AddMonths(1);
				var weekNum = DateTimeUtility.GetWeekNum(nextMonthDate).ToString();
				var week = (int)nextMonthDate.DayOfWeek;
				// 購入日から１ヶ月後を設定
				var fixedPurchaseKbn = string.Format("{0},{1}", weekNum, week.ToString());
				return fixedPurchaseKbn;
			}
			
		}

		/// <summary>
		/// ユーザーデフォルト注文設定のチェック(全てのカート)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>エラーメッセージキー</returns>
		public string CheckDefaultOrderSettingAllCart(string userId)
		{
			if (!Constants.TWOCLICK_OPTION_ENABLE) return "";
			var userDefaultOrderSetting = DomainFacade.Instance.UserDefaultOrderSettingService.Get(userId);
			if (userDefaultOrderSetting == null) return "";

			foreach (var cart in this.Items)
			{
				if (cart.Payment == null) continue;

				// 後付款(TriLink後払い)の場合は注文者の住所と配送先が台湾の時のみ利用可能
				if (TriLinkAfterPayHelper.CheckUsedPaymentForTriLinkAfterPay(
					cart.Payment.PaymentId,
					cart.Shippings[0].ShippingCountryIsoCode,
					cart.Owner.AddrCountryIsoCode))
				{
					return CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY2;
				}

				cart.SetDefaultExternalPaymentType(userDefaultOrderSetting);

				if (cart.CheckDefaultOrderSettingPaymentIsValid(
					userId,
					userDefaultOrderSetting,
					this.IsMultiCart)) return CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_LIMITED_PAYMENT;
			}

			return "";
		}

		/// <summary>
		/// クーポン利用可否のチェック(全てのカート)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userMailAddress">ユーザーメールアドレス</param>
		/// <returns>TRUE:正常なクーポン情報、FALSE:不正なクーポン情報</returns>
		public bool CheckCouponUseInfoAllCart(string userId, string userMailAddress)
		{
			if (!Constants.W2MP_COUPON_OPTION_ENABLED) return true;
			foreach (var cart in this.Items)
			{
				if (cart.Coupon == null) continue;

				// クーポン情報取得(※クーポンID、枝番に該当)
				var coupons = DomainFacade.Instance.CouponService.GetAllUserCouponsFromCouponId(
					cart.Coupon.DeptId,
					StringUtility.ToEmpty(userId),
					cart.Coupon.CouponId,
					cart.Coupon.CouponNo);

				// 各種チェック（エラーの場合はカートページへ）
				// クーポンコード存在チェック
				if (coupons.Length == 0) return false;

				// 未使用クーポンチェック(回数制限有りクーポンのみ)
				var mailAddress = (cart.Owner != null) ? cart.Owner.MailAddr : userMailAddress;
				if (CouponOptionUtility.CheckUseCoupon(coupons[0], StringUtility.ToEmpty(userId), mailAddress) != CouponErrorcode.NoError) return false;

				// クーポン有効性チェック
				if (CouponOptionUtility.CheckCouponValidWithCart(cart, coupons[0]) != "") return false;

				// クーポン割引額チェック（カート情報にクーポン情報が設定されている場合のみ）
				if (CouponOptionUtility.CheckDiscount(cart.Coupon, coupons[0]) != CouponErrorcode.NoError) return false;
			}

			return true;
		}

		/// <summary>
		/// 最初のカートかどうか
		/// </summary>
		/// <param name="cart">カート</param>
		/// <returns>最初のカート: true</returns>
		public bool IsFirstCart(CartObject cart)
		{
			var firstCart = this.Items.First();
			return (cart == firstCart);
		}

		/// <summary>
		/// 受注IDからカートを取得
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>カートオブジェクト</returns>
		public CartObject GetCartWithOrderId(string orderId)
		{
			var cart = this.Items.FirstOrDefault(item => (item.OrderId == orderId));
			return cart;
		}

		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>会員ランクID</summary>
		public string MemberRankId
		{
			get { return m_MemberRankId; }
			set
			{
				m_MemberRankId = value;
				if (this.Items != null)
				{
					foreach (CartObject co in this.Items)
					{
						co.MemberRankId = value;
					}
				}
			}
		}
		private string m_MemberRankId = null;
		/// <summary>注文区分</summary>
		public string OrderKbn { get; private set; }
		/// <summary>カートリスト総合計</summary>
		public decimal PriceCartListTotal
		{
			get
			{
				decimal dCartListPriceTotal = 0;
				foreach (CartObject co in this.Items)
				{
					dCartListPriceTotal += co.PriceTotal;
				}

				return dCartListPriceTotal;
			}
		}
		/// <summary>カートリスト総合計（決済手数料抜き）</summary>
		public decimal PriceCartListTotalWithOutPaymentPrice
		{
			get
			{
				decimal dPrice = 0;
				foreach (CartObject co in this.Items)
				{
					dPrice += co.PriceCartTotalWithoutPaymentPrice;
				}

				return dPrice;
			}
		}
		/// <summary>カートリスト</summary>
		public List<CartObject> Items { get; private set; }
		/// <summary>複数カートか</summary>
		public bool IsMultiCart
		{
			get { return (this.Items.Count > 1); }
		}
		/// <summary>注文者情報</summary>
		public CartOwner Owner { get; set; }
		/// <summary>DB更新設定</summary>
		public bool UpdateCartDb { get; set; }
		/// <summary>定期購入あり判定</summary>
		public bool HasFixedPurchase
		{
			get
			{
				foreach (CartObject co in this.Items)
				{
					if (co.HasFixedPurchase)
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>頒布会に紐づくの商品があるか</summary>
		public bool HasSubscriptionBox
		{
			get
			{
				return this.Items.Any(cp => (cp.IsSubscriptionBox));
			}
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; private set; }
		/// <summary>定期購入情報</summary>
		public FixedPurchaseModel FixedPurchase { get; private set; }
		/// <summary>ギフトあり判定</summary>
		public bool HasGift
		{
			get
			{
				foreach (CartObject co in this.Items)
				{
					if (co.IsGift)
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>初回購入時ポイント（合計）</summary>
		public decimal TotalFirstBuyPoint
		{
			get
			{
				decimal dTotalFirstBuyPoint = 0;
				foreach (CartObject co in this.Items)
				{
					// 初回購入金額毎：加算数の場合
					// （先頭カートのポイントのみ有効とする為、ポイント設定後ループを抜ける）
					if (co.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_NUM)
					{
						dTotalFirstBuyPoint = co.FirstBuyPoint;
						break;
					}
					// 初回購入金額毎：加算率の場合
					// （カート毎の初回購入ポイントを足していき、合計値を返却）
					else if (co.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_RATE)
					{
						dTotalFirstBuyPoint += co.FirstBuyPoint;
					}
				}

				return dTotalFirstBuyPoint;
			}
		}
		/// <summary>購入時ポイント（合計）</summary>
		public decimal TotalBuyPoint
		{
			get
			{
				decimal dTotalBuyPoint = 0;
				foreach (CartObject co in this.Items)
				{
					dTotalBuyPoint += co.BuyPoint;
				}

				return dTotalBuyPoint;
			}
		}
		/// <summary>カート画面遷移先(PCのみ)</summary>
		public string CartNextPage { get; set; }
		/// <summary>カートに類似ユーザーによる購入履歴があるか</summary>
		public bool HasOrderHistorySimilarShipping
		{
			get
			{
				return this.Items.Any(cart =>
					((cart.ProductOrderLmitOrderIds.Length > 0)
					|| cart.HasNotFirstTimeByCart
					|| cart.IsCompliantOrderLimitProduct));
			}
		}
		/// <summary>デフォルト注文方法設定パラメータ</summary>
		public UserDefaultOrderSettingParameter UserDefaultOrderSettingParm
		{
			get
			{
				if (m_UserDefaultOrderSettingParm == null)
				{
					m_UserDefaultOrderSettingParm = new UserDefaultOrderSettingParameter();
				}
				return m_UserDefaultOrderSettingParm;
			}
			set { m_UserDefaultOrderSettingParm = value; }
		}
		private UserDefaultOrderSettingParameter m_UserDefaultOrderSettingParm = null;
		/// <summary>ペイパル連携情報</summary>
		public PayPalCooperationInfo PayPalCooperationInfo
		{
			get
			{
				if (this.Items.Count == 0) return null;
				return this.Items.First().PayPalCooperationInfo;
			}
			set { this.Items.ForEach(c => c.PayPalCooperationInfo = value); }
		}
		/// <summary>購入制限の対象商品が含まれているか</summary>
		public bool HasOrderLimitProduct
		{
			get { return this.Items.Any(item => item.HasOrderLimitProduct); }
		}
		/// <summary>Novelty Ids Delete</summary>
		public List<string> NoveltyIdsDelete { get; set; }
		/// <summary>Is All Product Relate With Service Shipping Convenience Store</summary>
		public bool IsAllProductRelateWithServiceShippingConvenienceStore
		{
			get
			{
				var result = (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
					&& this.Items.All(CheckItemRelateWithServiceConvenienceStore));

				return result;
			}
		}
		/// <summary>User's default shipping can be set</summary>
		public bool CanSetUserDefaultOrderSettingShipping { get; set; }
		/// <summary>ランディングカートか</summary>
		public bool IsLandingCart { get; set; }
		/// <summary>LPカートページID</summary>
		public string LandingCartPageId { get; set; }
		/// <summary>LPページ絶対パス（ルートパスを含む）</summary>
		public string LandingCartInputAbsolutePath { get; set; }
		/// <summary>Authentication code</summary>
		public string AuthenticationCode { get; set; }
		/// <summary>Has authentication code</summary>
		public bool HasAuthenticationCode { get; set; }
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>全カートの商品合計金額</summary>
		public decimal ItemTotalPrice
		{
			get
			{
				var result = this.Items.Sum(cart => cart.PriceSubtotal);
				return result;
			}
		}
		/// <summary>全カートの商品総数</summary>
		public int ItemTotalCount
		{
			get
			{
				var result = this.Items.Sum(cart => cart.Items.Count);
				return result;
			}
		}

		/// <summary>
		/// デフォルト注文方法設定パラメータクラス
		/// </summary>
		[Serializable]
		public class UserDefaultOrderSettingParameter
		{
			/// <summary>デフォルト配送先登録するか</summary>
			public bool IsUserDefaultShippingRegister { get; set; }
			/// <summary>デフォルト支払方法登録するか</summary>
			public bool IsUserDefaultPaymentRegister { get; set; }
			/// <summary>デフォルト支払方法存在するか</summary>
			public bool IsUserDefaultPaymentExist { get; set; }
			/// <summary>デフォルト配送先指定が変更されたかの判定</summary>
			public bool IsChangedUserDefaultShipping { get; set; }
			/// <summary>デフォルト支払方法指定が変更されたかの判定</summary>
			public bool IsChangedUserDefaultPayment { get; set; }
			/// <summary>デフォルト配送先用カート番号</summary>
			public int UserDefaultShippingCartNo { get; set; }
			/// <summary>デフォルト支払方法用カート番号</summary>
			public int UserDefaultPaymentCartNo { get; set; }
			/// <summary>User Default Invoice No</summary>
			public int UserDefaultInvoiceNo { get; set; }
			/// <summary>Is User Default Invoice Register</summary>
			public bool IsUserDefaultInvoiceRegister { get; set; }
			/// <summary>Is Changed User Default Invoice</summary>
			public bool IsChangedUserDefaultInvoice { get; set; }
		}
	}
}
