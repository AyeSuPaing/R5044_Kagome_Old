/*
=========================================================================================================
  Module      : Order Register Api Facade(OrderRegisterApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Botchan;
using w2.App.Common.DataCacheController;
using w2.App.Common.FacebookConversion;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.Register;
using w2.App.Common.Product;
using w2.App.Common.Recommend;
using w2.App.Common.Util;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.Coupon;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Recommend;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.Botchan;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Order register api facade
	/// </summary>
	[Serializable]
	public class OrderRegisterApiFacade : OrderRegisterBase
	{
		/// <summary>注文完了時会員登録用メール情報</summary>
		public const string ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE = "order_key_mail_for_user_register_when_order_complete";
		/// <summary>注文ID</summary>
		public const string REQUEST_KEY_ORDER_ID = "odid";

		/// <summary>
		/// Order Register ApiFacade
		/// </summary>
		/// <param name="isUser">Is user</param>
		public OrderRegisterApiFacade(bool isUser)
			: base(ExecTypes.Pc, isUser, Constants.FLG_LASTCHANGED_USER)
		{
			this.GoogleAnalyticsParams = new List<Hashtable>();
		}

		/// <summary>
		/// カートリクエスト作成
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモ</param>
		/// <returns>Cart object</returns>
		public CartObject CreateCartByRequest(
			OrderRegisterRequest request, 
			List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var cart = request.CartObject;
			var dvCart = OrderRegisterUtility.GetCart(cart.CartId);
			if (dvCart.Count == 0)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.CART_VARIANCE);
				return null;
			}
			var cartDiscount = request.DiscountInfoObject;
			var cartShippings = request.OrderShippingObject;
			var cartProducts = request.OrderProductLists;
			var cartPaymentObject = request.OrderPaymentObject;
			var cartOrderOwner = request.OrderOwnerObject;

			var isGuestUser = false;
			var memberRankId = string.Empty;
			var user = new UserModel();
			var orderKbn = (Constants.SMARTPHONE_OPTION_ENABLED && (cart.OrderKbn == Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE))
				? Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE
				: Constants.FLG_ORDER_ORDER_KBN_PC;

			if (string.IsNullOrEmpty(cart.UserId))
			{
				isGuestUser = true;
				cart.UserId = UserService.CreateNewUserId(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.NUMBER_KEY_USER_ID,
					Constants.CONST_USER_ID_HEADER,
					Constants.CONST_USER_ID_LENGTH);
			}
			else
			{
				user = new UserService().Get(cart.UserId);
				memberRankId = MemberRankOptionUtility.GetMemberRankId(cart.UserId);
			}

			var cartList = CartObjectList.GetUserCartList(
				cart.UserId,
				orderKbn,
				string.Empty);

			var cartObject = cartList.GetCart(cart.CartId);

			cartObject = new CartObject(
				cart.CartId,
				cart.UserId,
				orderKbn,
				Constants.CONST_DEFAULT_SHOP_ID,
				string.Empty,
				false,
				false,
				memberRankId);
			cartList.Items.RemoveRange(0, cartList.Items.Count);
			cartObject.IsGuestUser = isGuestUser;
			cartObject.AdvCodeNew = cart.AdvCode ?? string.Empty;

			var fixedPurchaseId = cartObject.HasFixedPurchase
				? OrderCommon.CreateFixedPurchaseId(cartObject.ShopId)
				: string.Empty;

			var listProduct = new List<DataRowView>();
			foreach (var product in cartProducts)
			{
				var productData = ProductCommon.GetProductVariationInfo(
					Constants.CONST_DEFAULT_SHOP_ID,
					product.ProductId,
					product.VariationId,
					memberRankId);

				if ((productData.Count == 0) || (product.ProductCount == 0))
				{
					errorList.Add(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_NO_ITEM);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_NO_ITEM.ToString())
						.Replace("@@ 1 @@", product.ProductId)
						.Replace("@@ 2 @@", product.VariationId);
					return null;
				}

				listProduct.Add(productData[0]);

				var cartProduct = new CartProduct(
					productData[0],
					OrderRegisterUtility.GetAddCartKbn(cart.OrderDivision),
					StringUtility.ToEmpty(productData[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
					product.ProductCount,
					true,
					new ProductOptionSettingList(),
					fixedPurchaseId,
					new ContentsLogModel());

				cartObject.Add(cartProduct);
				cartList.Items.Add(cartObject);
			}

			if (cartObject.Items.Count == 0)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_NO_ITEM);
				return null;
			}

			if (Constants.NOVELTY_OPTION_ENABLED
				&& (cart.AddNoveltyFlag == Constants.FLG_NOVELTY_VALID_FLG_VALID))
			{
				var cartNoveltyList = new CartNoveltyList(cartList);
				var noveltyItems = BotChanUtility.CreateNoveltyItems(
					cart.CartId,
					cartNoveltyList,
					cartList);

				foreach (var noveltyItem in noveltyItems)
				{
					cartObject.Add(noveltyItem);
				}
			}

			if (cartDiscount == null) cartDiscount = new OrderRegisterRequest.DiscountInfo();
			if (string.IsNullOrEmpty(cartDiscount.OrderPointUse)) cartDiscount.OrderPointUse = "0";

			var usePoint = decimal.Parse(cartDiscount.OrderPointUse);
			cartObject.SetUsePoint(
				usePoint,
				OrderRegisterUtility.GetPointUsePrice(usePoint));

			if (string.IsNullOrEmpty(cartDiscount.CouponCode) == false)
			{
				var userCouponDetailInfo = new CouponService().GetAllUserCouponsFromCouponCode(
					Constants.W2MP_DEPT_ID,
					cart.UserId,
					cartDiscount.CouponCode);
				cartObject.Coupon = new CartCoupon(userCouponDetailInfo[0]);
			}

			if (isGuestUser)
			{
				var ownerZip = cartOrderOwner.Zip.Split('-');
				cartObject.Owner = new CartOwner(
					request.OwnerKbn,
					string.Format("{0} {1}", cartOrderOwner.Name1, cartOrderOwner.Name2),
					cartOrderOwner.Name1,
					cartOrderOwner.Name2,
					string.Format("{0} {1}", cartOrderOwner.NameKana1, cartOrderOwner.NameKana2),
					cartOrderOwner.NameKana1,
					cartOrderOwner.NameKana2,
					cartOrderOwner.MailAddr,
					"",
					cartOrderOwner.Zip,
					(ownerZip.Length > 0) ? ownerZip[0] : string.Empty,
					(ownerZip.Length > 1) ? ownerZip[1] : string.Empty,
					cartOrderOwner.Addr1,
					cartOrderOwner.Addr2,
					cartOrderOwner.Addr3,
					cartOrderOwner.Addr4,
					"",
					"",
					"",
					"",
					"",
					cartOrderOwner.Tel1,
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					true,
					cartOrderOwner.Sex,
					DateTime.ParseExact(cartOrderOwner.Birth, "yyyyMMdd", null),
					RegionManager.GetInstance().Region.CountryIsoCode,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId,
					RegionManager.GetInstance().Region.CurrencyCode,
					RegionManager.GetInstance().Region.CurrencyLocaleId);
			}
			else
			{
				cartObject.Owner = new CartOwner(user);
			}

			var cartShipping = new CartShipping(cartObject);

			var shippingAddr = new Hashtable
			{
				{ Constants.FIELD_USERSHIPPING_NAME, cartShippings.Name },
				{ Constants.FIELD_USER_NAME1, (string.IsNullOrEmpty(cartShippings.ShippingName1) == false)
					? cartShippings.ShippingName1
					: cartShippings.ShippingName.Split('　')[0]},
				{ Constants.FIELD_USER_NAME2, (string.IsNullOrEmpty(cartShippings.ShippingName2) == false)
					? cartShippings.ShippingName2
					: (cartShippings.ShippingName.Split('　').Length > 1) ? cartShippings.ShippingName.Split('　')[1] : ""},
				{ Constants.FIELD_USER_NAME_KANA1, (string.IsNullOrEmpty(cartShippings.ShippingNameKana1) == false)
					? cartShippings.ShippingNameKana1
					: cartShippings.ShippingNameKana.Split('　')[0]},
				{ Constants.FIELD_USER_NAME_KANA2, (string.IsNullOrEmpty(cartShippings.ShippingNameKana2) == false)
					? cartShippings.ShippingNameKana2
					: (cartShippings.ShippingNameKana.Split('　').Length > 1) ? cartShippings.ShippingNameKana.Split('　')[1] : ""},
				{ Constants.FIELD_USER_ZIP, cartShippings.ShippingZip },
				{ Constants.FIELD_USER_ADDR1, cartShippings.ShippingAddr1 },
				{ Constants.FIELD_USER_ADDR2, cartShippings.ShippingAddr2 },
				{ Constants.FIELD_USER_ADDR3, cartShippings.ShippingAddr3 },
				{ Constants.FIELD_USER_ADDR4, cartShippings.ShippingAddr4 },
				{ Constants.FIELD_USER_TEL1, cartShippings.ShippingTel1 },
				{ Constants.FIELD_USER_COMPANY_NAME, cartShippings.ShippingCompanyName },
				{ Constants.FIELD_USER_COMPANY_POST_NAME, cartShippings.ShippingCompanyPostName },
			};

			cartShipping.UpdateShippingAddr(shippingAddr);
			cartShipping.UpdateTel1();
			cartShipping.ShippingCountryIsoCode = cartObject.Owner.AddrCountryIsoCode;
			var shippingZip = cartShippings.ShippingZip.Split('-');
			cartShipping.Zip1 = (shippingZip.Length > 0) ? shippingZip[0] : string.Empty;
			cartShipping.Zip2 = (shippingZip.Length > 1) ? shippingZip[1] : string.Empty;
			cartObject.Shippings.First().UpdateShippingAddr(cartShipping);
			if (string.IsNullOrEmpty(cartShippings.ShippingDate) == false)
			{
				cartObject.Shippings.First().ShippingDate = DateTime.Parse(cartShippings.ShippingDate);
				cartObject.Shippings.First().ShippingDateForCalculation = DateTime.Parse(cartShippings.ShippingDate);
				cartObject.Shippings.First().SpecifyShippingDateFlg = true;
			}

			cartObject.Payment = new CartPayment()
			{
				PaymentId = cartPaymentObject.PaymentId,
				PaymentName = new PaymentService().GetPaymentName(Constants.CONST_DEFAULT_SHOP_ID, cartPaymentObject.PaymentId),
			};
			cartObject.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
				cartObject.ShopId,
				cartObject.Payment.PaymentId,
				cartObject.PriceSubtotal,
				cartObject.PriceCartTotalWithoutPaymentPrice);

			if (cartObject.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				if (cartPaymentObject.BranchNo == 0)
				{
					cartObject.Payment.CreditCardNo1 = cartPaymentObject.CreditCardNo.Substring(0, 4);
					cartObject.Payment.CreditCardNo2 = cartPaymentObject.CreditCardNo.Substring(4, 4);
					cartObject.Payment.CreditCardNo3 = cartPaymentObject.CreditCardNo.Substring(8, 4);
					cartObject.Payment.CreditCardNo4 = cartPaymentObject.CreditCardNo.Substring(12, 4);
					cartObject.Payment.CreditExpireMonth = 
						string.IsNullOrEmpty(cartPaymentObject.ExpirationMonth.ToString()) 
							? ""
							: cartPaymentObject.ExpirationMonth.ToString("00");
					cartObject.Payment.CreditExpireYear =
						string.IsNullOrEmpty(cartPaymentObject.ExpirationYear.ToString())
							? ""
							: cartPaymentObject.ExpirationYear.ToString("00");
					cartObject.Payment.CreditToken =
						CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(cartPaymentObject.CreditToken);

					if (cartPaymentObject.CreditRegistrationFlag == Constants.BOTCHAN_API_CREDIT_REGIST_FLAG_VALID)
					{
						cartObject.Payment.UserCreditCardRegistFlg = true;
						cartObject.Payment.UserCreditNameComplementFlg =
							(string.IsNullOrEmpty(cartPaymentObject.CreditRegistrationName)
							|| (StringUtility.ToEmpty(cartPaymentObject.CreditRegistrationName).Length > Constants.BOTCHAN_API_CREDIT_NAME_MAX_LENGTH));
						cartObject.Payment.UserCreditCardName = cartObject.Payment.UserCreditNameComplementFlg
								? Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME
								: StringUtility.ToEmpty(cartPaymentObject.CreditRegistrationName);
					}
				}
				else
				{
					cartObject.Payment.CreditCardBranchNo = cartPaymentObject.BranchNo.ToString();
					cartObject.Payment.UserCreditCard = UserCreditCard.Get(cart.UserId, cartPaymentObject.BranchNo);
				}
				cartObject.Payment.CreditInstallmentsCode = StringUtility.ToEmpty(cartPaymentObject.CreditInstallments);
				cartObject.Payment.CreditAuthorName = cartPaymentObject.AuthorName;
				cartObject.Payment.CreditSecurityCode = StringUtility.ToEmpty(cartPaymentObject.CreditSecurityCode);
				cartObject.Payment.CreditCardCompany = string.Empty;
			}

			cartObject.ShippingType = cartObject.Items.First().ShippingType;

			// 配送サービスが未確定の場合、配送サービスを確定
			if (string.IsNullOrEmpty(cartShipping.DeliveryCompanyId))
			{
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cartObject.ShippingType);
				cartShipping.CartShippingShippingMethodUserUnSelected(shopShipping);
			}

			// 配送希望時間帯情報を格納
			if (string.IsNullOrEmpty(cartShippings.ShippingTime) == false)
			{
				cartObject.Shippings.First().ShippingTime = cartShippings.ShippingTime;
				cartObject.Shippings.First().ShippingTimeMessage = new DeliveryCompanyService()
					.Get(cartShipping.DeliveryCompanyId).GetShippingTimeMessage(cartShippings.ShippingTime);
				cartObject.Shippings.First().SpecifyShippingTimeFlg = true;
			}

			cartObject.SettlementCurrency = CurrencyManager.GetSettlementCurrency(cartObject.Payment.PaymentId);
			cartObject.SettlementRate = CurrencyManager.GetSettlementRate(cartObject.SettlementCurrency);
			cartObject.ReceiptFlg = request.OrderPaymentObject.ReceiptFlg ?? "0";

			if (cartObject.HasFixedPurchase)
			{
				var creditBranchNoTmp = 0;
				var now = DateTime.Now;
				var fixedPurchase = new FixedPurchaseModel()
				{
					FixedPurchaseId = fixedPurchaseId,
					LastOrderDate = now,
					CardKbn = cartShipping.FixedPurchaseKbn,
					FixedPurchaseKbn = cartShippings.FixedPurchaseKbn,
					FixedPurchaseSetting1 = cartShippings.CourseBuySetting,
					OrderCount = 1,
					UserId = cartObject.OrderUserId,
					ShopId = cartObject.ShopId,
					OrderKbn = cartObject.OrderKbn,
					OrderPaymentKbn = cartObject.Payment.PaymentId,
					FixedPurchaseDateBgn = now,
					LastChanged = this.LastChanged,
					CreditBranchNo = int.TryParse(cartObject.Payment.CreditCardBranchNo, out creditBranchNoTmp)
						? (int?)creditBranchNoTmp
						: null,
					NextShippingDate = cartShipping.NextShippingDate,
					NextNextShippingDate = cartShipping.NextNextShippingDate,
					FixedPurchaseManagementMemo = cartObject.ManagementMemo,
					ShippingMemo = cartObject.ShippingMemo,
					CardInstallmentsCode = cartObject.Payment.CreditInstallmentsCode,
					AccessCountryIsoCode = cartObject.Owner.AccessCountryIsoCode,
					DispLanguageCode = cartObject.Owner.DispLanguageCode,
					DispLanguageLocaleId = cartObject.Owner.DispLanguageLocaleId,
					DispCurrencyCode = cartObject.Owner.DispCurrencyCode,
					DispCurrencyLocaleId = cartObject.Owner.DispCurrencyLocaleId,
					ExternalPaymentAgreementId = cartObject.ExternalPaymentAgreementId ?? string.Empty,
				};

				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cartObject.Items.First().ShippingType);
				cartShipping.CartShippingShippingMethodUserUnSelected(shopShipping);
				DateTime baseDate;
				if (cartObject.GetShipping().ShippingDate.HasValue == false)
				{
					baseDate = OrderCommon.GetFirstShippingDateBasedOnToday(
						cartObject.ShopId,
						shopShipping.FixedPurchaseShippingDaysRequired,
						cartObject.GetShipping().ShippingDate,
						cartObject.GetShipping().ShippingMethod,
						cartShipping.DeliveryCompanyId,
						cartObject.GetShipping().ShippingCountryIsoCode,
						cartObject.GetShipping().IsTaiwanCountryShippingEnable
							? cartObject.GetShipping().Addr2
							: cartObject.GetShipping().Addr1,
						cartObject.GetShipping().Zip);
				}
				else
				{
					baseDate = cartObject.GetShipping().ShippingDate.Value;
				}

				var nextShippingDate = GetNextShippingDate(fixedPurchase, shopShipping, baseDate);
				var nextNextShippingDate = GetNextNextShippingDate(fixedPurchase, shopShipping, baseDate);

				cartObject.Shippings.First().UpdateNextShippingDates(nextShippingDate, nextNextShippingDate);

				// ２回目以降商品配送パターン切替
				if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
				{
					var cartFixedPurchaseNextShippingProduct = cartObject.Items
						.FirstOrDefault(cartProduct => cartProduct.CanSwitchProductFixedPurchaseNextShippingSecondTime());
					if (cartFixedPurchaseNextShippingProduct != null)
					{
						cartObject.Shippings[0].CanSwitchProductFixedPurchaseNextShippingSecondTime = true;
						cartObject.Shippings[0].UpdateNextShippingItemFixedPurchaseInfos(
							cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn,
							cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting);
						cartObject.Shippings[0].CalculateNextShippingItemNextNextShippingDate();
						cartObject.Shippings.First().UpdateNextShippingDates(nextShippingDate, cartObject.Shippings[0].NextNextShippingDate);
					}
				}

				cartObject.Shippings.First().UpdateFixedPurchaseSetting(
					cartShippings.FixedPurchaseKbn,
					cartShippings.CourseBuySetting,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan);
			}

			cartObject.Calculate(false, true, false, true);
			cartObject.SettlementAmount = CurrencyManager.GetSettlementAmount(
				cartObject.PriceTotal,
				cartObject.SettlementRate,
				cartObject.SettlementCurrency);

			if (string.IsNullOrEmpty(cartShippings.ShippingMethod) == false)
			{
				cartObject.Shippings[0].ShippingMethod = cartShippings.ShippingMethod;
			}

			// 配送不可エリアチェック
			var unavailableShipping = BotChanUtility.CheckUnavailableShippingAreaForBotChat(cartObject);
			if (unavailableShipping)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.UNAVAILABLE_SHIPPING_AREA_ERROR);
				return null;
			}

			// 別出荷フラグを更新
			cartObject.UpdateAnotherShippingFlag();

			return cartObject;
		}

		/// <summary>
		/// カートリクエスト作成
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモ</param>
		/// <param name="oldOrder">レコメンド前注文</param>
		/// <param name="otherInfo">その他情報</param>
		/// <returns>Cart object</returns>
		public CartObjectList CreateCartByOldOrder(
			OrderRegisterRequest request,
			List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo,
			ref OrderModel oldOrder,
			ref Hashtable otherInfo)
		{
			if (Constants.RECOMMEND_OPTION_ENABLED == false)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.RECOMMEND_OPTION_OFF);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.RECOMMEND_OPTION_OFF.ToString());
				return null;
			}

			var cart = request.CartObject;

			var oldOrderModel = new OrderService().Get(cart.OldOrderId);
			if (oldOrderModel == null)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.NONE_ORIGINAL_ORDER);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.NONE_ORIGINAL_ORDER.ToString());
				return null;
			}

			if (oldOrderModel.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.REGIST_ORDER_FAILURE);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.REGIST_ORDER_FAILURE.ToString())
					.Replace("@@ 1 @@", MessageManager.GetMessages(BotchanMessageManager.MessagesCode.INVALID_RECOMMEND_ORDER_STATUS.ToString()));
				return null;
			}

			var orderUserId = oldOrderModel.UserId;
			var cartList = CartObjectList.CreateCartObjectListByOrder(
				orderUserId,
				cart.OrderKbn,
				new[] { oldOrderModel });

			var targetCartList = cartList.Items.ToArray();

			var recommend = new RecommendExtractor(orderUserId, oldOrderModel.MemberRankId, cartList.Items.ToArray())
				.Exec(targetCartList, Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_COMPLETE);

			if ((recommend == null)
				|| (recommend.CanDisplayOrderCompletePage == false)
				|| (recommend.ChatbotUseFlg == Database.Common.Constants.FLG_RECOMMEND_CHATBOT_USE_FLG_INVALID))
			{
				errorList.Add(BotchanMessageManager.MessagesCode.NONE_RECOMMEND_TARGET);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.NONE_RECOMMEND_TARGET.ToString());
				return null;
			}

			//レスポンスを元に配送パターンを強制更新
			if (cartList.HasFixedPurchase)
			{
				foreach (var item in recommend.Items)
				{
					item.FixedPurchaseKbn = request.OrderShippingObject.FixedPurchaseKbn;
					item.FixedPurchaseSetting1 = request.OrderShippingObject.CourseBuySetting;
				}
			}

			var recommendHistoryNo = new RecommendService().GetNewRecommendHistoryNoAndInsertRecommendHistory(new RecommendHistoryModel
			{
				ShopId = recommend.ShopId,
				RecommendId = recommend.RecommendId,
				UserId = orderUserId,
				TargetOrderId = targetCartList.First().OrderId ?? string.Empty,
				DisplayKbn = Constants.FLG_RECOMMENDHISTORY_DISPLAY_KBN_FRONT,
				OrderedFlg = Constants.FLG_RECOMMENDHISTORY_ORDERED_FLG_DISP,
				LastChanged = Constants.FLG_LASTCHANGED_USER
			});

			otherInfo.Add(Constants.CONST_RECOMMEND_INFO, recommend);
			otherInfo.Add(Constants.CONST_RECOMMEND_HISTORY_NO, recommendHistoryNo);
			oldOrder = oldOrderModel;

			cartList.AddRecommendItem(recommend);

			cartList.Items.ForEach(item => item.UpdateAnotherShippingFlag());

			return cartList;
		}

		/// <summary>
		/// 注文後レコメンド実施予定
		/// </summary>
		/// <param name="cartList">カート</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモー</param>
		/// <returns>レスポンス</returns>
		public OrderRegisterResponse CreateResponseByRecommendItem(
			CartObjectList cartList,
			List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var orderProducts = new List<OrderRegisterResponse.OrderProductList>();
			foreach (CartObject cart in cartList)
			{
				var tempList = cart.Items
					.Where(item=>item.IsRecommendItem)
					.Select(product => new OrderRegisterResponse.OrderProductList
				{
					ProductId = product.ProductId,
					VariationId = product.VariationId,
					ItemQuantity = product.Count.ToString(),
					ProductSaleId = product.ProductSaleId,
					ProductName = product.ProductName,
					ItemPrice = product.Price.ToString("#"),
					ProductTaxRate = product.PriceTax.ToString("#"),
					IsFixedPurchase = product.IsFixedPurchase,
				});
				orderProducts.AddRange(tempList);
			}

			var response = new OrderRegisterResponse
			{
				Result = true,
				Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
				MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
				DataResult = new OrderRegisterResponse.Data()
				{
					OrderProducts = orderProducts.ToArray(),
				}
			};
			return response;
		}

		/// <summary>
		/// 注文後レコメンド実施
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="oldOrder">過去注文情報</param>
		/// <param name="otherInfo">補足情報</param>
		/// <param name="memo">メモー</param>
		/// <param name="isReauthComplete">再与信済か</param>
		/// <returns>レスポンス</returns>
		public OrderRegisterResponse RegisterOrderByRecommend(
			CartObjectList cartList,
			List<BotchanMessageManager.MessagesCode> errorList,
			OrderModel oldOrder,
			Hashtable otherInfo,
			ref string memo,
			ref bool isReauthComplete)
		{
			cartList.Items.ForEach(
				cart => cart.Shippings.ForEach(
					shipping => shipping.UpdateShippingMethod(
						DataCacheControllerFacade.GetShopShippingCacheController().Get(cart.ShippingType))));

			cartList.CalculateAllCart();

			foreach (var co in cartList.Items)
			{
				co.SettlementCurrency = CurrencyManager.GetSettlementCurrency(co.Payment.PaymentId);
				co.SettlementRate = CurrencyManager.GetSettlementRate(co.SettlementCurrency);
				co.SettlementAmount = CurrencyManager.GetSettlementAmount(co.PriceTotal, co.SettlementRate, co.SettlementCurrency);
				co.IsBotChanOrder = true;
			}

			var success = ResultTypes.Fail;
			var registeredOrder = new List<string>();
			var user = new UserService().Get(oldOrder.UserId);
			var isFirstCart = true;
			var excludeOrderIds = new [] { oldOrder.OrderId };
			var oldOrderList = new[] { oldOrder };

			using (var productBundler = new ProductBundler(
				cartList.Items,
				oldOrder.UserId,
				oldOrder.AdvcodeFirst,
				oldOrder.AdvcodeNew,
				excludeOrderIds))
			{
				var bundledCartList = productBundler.CartList.Where(cart => cart.Items.Any()).ToArray();
				var alreadyOtherCartReauthByAtodene = false;
				var disablePaymentCancelOrderId = string.Empty;

				foreach (var cart in bundledCartList)
				{
					var newOrder = (oldOrder.OrderId == cart.OrderId)
						? cart.CreateNewOrder(oldOrder)
						: cart.CreateNewOrder();
					if (oldOrder.OrderId != cart.OrderId)
					{
						cart.OrderId = newOrder.OrderId;
						cart.OrderUserId = user.UserId;

						newOrder.PaymentOrderId = string.Empty;
						newOrder.AdvcodeFirst = oldOrder.AdvcodeFirst;
						newOrder.AdvcodeNew = oldOrder.AdvcodeNew;

						var cvsDefOrder = oldOrderList.Where(old => old.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
						var existOrderIdCart = bundledCartList.Where(bc => string.IsNullOrEmpty(bc.OrderId) == false);
						var isAtodeneCancelCart = cvsDefOrder.Any(old => existOrderIdCart.Any(c => (c.OrderId == old.OrderId)) == false);

						var isAtodeneReauth = OrderCommon.IsAtodeneReauthByNewOrder(
							oldOrderList.Where(old => old.OrderId != cart.OrderId).ToList(),
								newOrder.OrderPaymentKbn);
						if (isAtodeneReauth && (alreadyOtherCartReauthByAtodene == false) && isAtodeneCancelCart)
						{
							var cancelAtodeneOrders = oldOrderList.Where(old =>
								(old.OrderId != cart.OrderId) && (old.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
							var maxAmountCancelOrder = cancelAtodeneOrders.OrderByDescending(old => old.LastBilledAmount).FirstOrDefault();
							alreadyOtherCartReauthByAtodene = true;

							success = ExecReauthAndNewOrder(
								oldOrder,
								newOrder.DataSource,
								cart,
								false,
								isFirstCart,
								false,
								true,
								out disablePaymentCancelOrderId,
								newOrder);
						}
						else
						{
							success = this.Exec(newOrder.DataSource, cart, cart.IsGuestUser, isFirstCart);
						}
					}
					else if (newOrder.Items.Any(item => item.IsRecommendItem)
						|| (newOrder.Items.Length != oldOrder.Items.Length)
						|| newOrder.Items.Any(item =>
							(item.ItemQuantitySingle != ((oldOrder.Items.FirstOrDefault(oldItem => ((oldItem.ProductId == item.ProductId) && (oldItem.VariationId == item.VariationId))) != null)
								? oldOrder.Items.First(oldItem => ((oldItem.ProductId == item.ProductId) && (oldItem.VariationId == item.VariationId))).ItemQuantitySingle
								: 0))))
					{
						//セットプロモーション割引を再計算
						newOrder.RecalculateSetPromotionDiscountAmount();

						success = UpdateOrder(newOrder, oldOrder, cart, user, ref isReauthComplete);
					}
					isFirstCart = false;
					registeredOrder.Add(newOrder.OrderId);
				}

				if (success == ResultTypes.Success)
				{
					oldOrderList.Where(old => registeredOrder.Any(orderId => (orderId == old.OrderId)) == false)
							.ToList().ForEach(order => CancelOrder(order, user, disablePaymentCancelOrderId));
				}
			}

			var recommendModel = (RecommendModel)otherInfo[Constants.CONST_RECOMMEND_INFO];
			var recommendHistoryNo = (int)otherInfo[Constants.CONST_RECOMMEND_HISTORY_NO];

			new RecommendService().UpdateBuyOrderedFlg(
				recommendModel.ShopId,
				recommendModel.RecommendId,
				oldOrder.UserId,
				recommendHistoryNo,
				Constants.FLG_LASTCHANGED_USER);

			OrderRegisterResponse response;
			if (success == ResultTypes.Success)
			{
				response = new OrderRegisterResponse
				{
					Result = true,
					Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
					MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
					DataResult = new OrderRegisterResponse.Data
					{
						OrderId = string.Join(",", registeredOrder),
					}
				};
			}
			else
			{
				errorList.Add(BotchanMessageManager.MessagesCode.REGIST_ORDER_FAILURE);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.REGIST_ORDER_FAILURE.ToString())
					.Replace("@@ 1 @@", string.Join("#", this.ErrorMessages));
				return new OrderRegisterResponse();
			}

			return response;
		}

		/// <summary>
		/// 注文キャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="user">ユーザー情報</param>
		/// <param name="disablePaymentCancelOrderId">外部決済キャンセルを行わない受注ID</param>
		private void CancelOrder(OrderModel order, UserModel user, string disablePaymentCancelOrderId)
		{
			order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED;
			var updater = new OrderUpdaterBotchan(
				user,
				order,
				Constants.FLG_LASTCHANGED_USER);
			var errorMessage = string.Empty;
			if (updater.ExecuteCanceOrder(
				true,
				OrderHistory.ActionType.FrontRecommend,
				UpdateHistoryAction.Insert,
				out errorMessage,
				disablePaymentCancelOrderId)
					&& order.IsFixedPurchaseOrder)
			{
				var fixedPurchaseService = new FixedPurchaseService();
				var fixedPurchaseContainer = fixedPurchaseService.GetContainer(order.FixedPurchaseId);
				fixedPurchaseService.CancelFixedPurchase(
					fixedPurchaseContainer,
					string.Empty,
					string.Empty,
					Constants.FLG_LASTCHANGED_USER,
					Constants.CONST_DEFAULT_DEPT_ID,
					Constants.W2MP_POINT_OPTION_ENABLED,
					UpdateHistoryAction.DoNotInsert);
				fixedPurchaseService.UpdateInvalidate(
					order.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
			}

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				this.ErrorMessages.Add(errorMessage);
				return;
			}

			// メール送信
			updater.SendOrderCancelMail(CartObject.CreateCartByOrder(order), true);
		}

		/// <summary>
		/// 注文情報更新
		/// </summary>
		/// <param name="newOrder">注文情報</param>
		/// <param name="oldOrder">過去注文情報</param>
		/// <param name="newOrderCart">新注文カートオブジェクト</param>
		/// <param name="user">ユーザー情報</param>
		/// <param name="isReauthComplete">再与信済か</param>
		private ResultTypes UpdateOrder(
			OrderModel newOrder,
			OrderModel oldOrder,
			CartObject newOrderCart,
			UserModel user,
			ref bool isReauthComplete)
		{
			var errorMessage = string.Empty;
			var updater = new OrderUpdaterBotchan(
				user,
				newOrder,
				oldOrder,
				user.UserMemo,
				user.UserManagementLevelId,
				Constants.FLG_LASTCHANGED_USER);

			var transactionName = updater.CreateOrderNew(
				true,
				true,
				UpdateHistoryAction.DoNotInsert,
				out errorMessage);
			if (string.IsNullOrEmpty(transactionName) == false)
			{
				this.ErrorMessages.Add(
					MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER.ToString()));
				this.ErrorMessages.Add(errorMessage);
				return ResultTypes.Fail;
			}

			// 外部決済連携実行
			var isExecuteExternalPayment = updater.ExecuteExternalPayment(
				ReauthCreatorFacade.ExecuteTypes.System,
				ReauthCreatorFacade.ExecuteTypes.System,
				UpdateHistoryAction.DoNotInsert,
				out errorMessage);

			isReauthComplete = true;

			if (isExecuteExternalPayment || (string.IsNullOrEmpty(errorMessage) == false))
			{
				OrderCommon.AppendExternalPaymentCooperationLog(
					isExecuteExternalPayment,
					newOrder.OrderId,
					isExecuteExternalPayment ? LogCreator.CreateMessage(newOrder.OrderId, "") : errorMessage,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
			}

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				this.ErrorMessages.Add(errorMessage);
				return ResultTypes.Fail;
			}

			// 注文情報更新
			var isSuccess = updater.ExecuteUpdateOrderAndRegisterUpdateHistory(
				false,
				false,
				isExecuteExternalPayment,
				true,
				true,
				OrderHistory.ActionType.FrontRecommend,
				UpdateHistoryAction.Insert,
				out errorMessage);
			if (isSuccess != OrderUpdaterBotchan.ResultType.Success)
			{
				this.ErrorMessages.Add(
					(isSuccess == OrderUpdaterBotchan.ResultType.OutOfStock)
						? MessageManager.GetMessages(BotchanMessageManager.MessagesCode.PRODUCTSTOCK_OUT_OF_STOCK_ERROR.ToString())
						: MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER.ToString()));

				return ResultTypes.Fail;
			}

			// 定期台帳登録・更新
			if (newOrder.Items.Any(item => item.IsFixedPurchaseItem))
			{
				if (oldOrder.IsFixedPurchaseOrder)
				{
					// 更新
					updater.UpdateFixedPurchaseOrderForRecommendAtOrderComplete(newOrderCart, UpdateHistoryAction.DoNotInsert);
				}
				else
				{
					// 新規
					updater.RegisterFixedPurchaseOrder(newOrderCart, UpdateHistoryAction.DoNotInsert);
				}
			}

			// メール送信
			updater.SendOrderUpdateMail(newOrderCart,true);

			return ResultTypes.Success;
		}

		/// <summary>
		/// 注文登録レスポンス作成
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモー</param>
		/// <returns>レスポンス</returns>
		public OrderRegisterResponse RegisterOrderByCart(
			CartObject cart,
			List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var order = cart.CreateNewOrder();
			cart.OrderId = order.OrderId;
			cart.IsBotChanOrder = true;

			var advCodeNow = cart.AdvCodeNew;
			var advCodeIsValid = AdvCodeIsValid(Constants.W2MP_DEPT_ID, advCodeNow);

			if ((Constants.W2MP_AFFILIATE_OPTION_ENABLED)
				&& (string.IsNullOrEmpty(advCodeNow) == false)
				&& advCodeIsValid)
			{
				var advCodeFirstStr = (cart.IsGuestUser)
					? string.Empty
					: new UserService().Get(cart.CartUserId).AdvcodeFirst;

				var advCodeFirst = GetAdvCodeFirst(advCodeNow, advCodeFirstStr);
				order.AdvcodeFirst = advCodeFirst;
				order.AdvcodeNew = advCodeNow;

				AdvCodeLogRegister(Constants.W2MP_DEPT_ID, advCodeNow, "00pc");
			}
			else
			{
				order.AdvcodeFirst = string.Empty;
				order.AdvcodeNew = string.Empty;
			}

			var ht = order.DataSource;
			var orderUser = new UserService().Get(order.UserId);

			//ゲストユーザーの場合userがいないのでエラー回避
			if (orderUser != null)
			{
				ht[Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME] = orderUser.OrderCountOrderRealtime;
			}

			var success = this.Exec(ht, cart, cart.IsGuestUser, true);

			if (Constants.W2MP_AFFILIATE_OPTION_ENABLED && (success == ResultTypes.Success))
			{
				var user = new UserService().Get(cart.CartUserId);
				if (string.IsNullOrEmpty(user.AdvcodeFirst)
					&& (string.IsNullOrEmpty(advCodeNow) == false)
					&& advCodeIsValid)
				{
					new UserService().UpdateAdvCodeFirst(
						user.UserId,
						advCodeNow,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert);
				}
			}

			OrderRegisterResponse response;
			if (success == ResultTypes.Success)
			{
				response = new OrderRegisterResponse
				{
					Result = true,
					Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
					MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
					DataResult = new OrderRegisterResponse.Data()
					{
						OrderId = order.OrderId,
						CreditNameComplementFlg = cart.Payment.UserCreditNameComplementFlg
							? Constants.BOTCHAN_API_CREDIT_NAME_COMPLEMENT_FLAG_VALID
							: null,
					}
				};

				if (Constants.MARKETING_FACEBOOK_CAPI_OPTION_ENABLED)
				{
					var facebookConversionUtility = new FacebookConversionUtility();
					var customDataObject = facebookConversionUtility.CreateCustomDataPurchase(order.OrderId);
					var convertFaceBook = facebookConversionUtility.CreateConvertFaceBookRequest(
						FacebookConversionConstants.FACEBOOK_EVENT_NAME_PURCHASE,
						order.UserId,
						string.Format(
							"{0}{1}?{2}={3}",
							Constants.PATH_ROOT,
							Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL,
							REQUEST_KEY_ORDER_ID, order.OrderId),
						string.Empty,
						true,
						customDataObject);
					new FacebookConversionApiFacade().CallAPIFacebook(convertFaceBook);
				}
			}
			else
			{
				// ZEUSのクレカ決済時APIエラーメッセージ不要
				if ((Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Zeus)
					|| (this.ErrorMessages.Any(msg => ((msg == CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR))
						|| (msg == CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR)))) == false))
				{
					this.ErrorMessages.Add(this.ApiErrorMessage);
				}
				if (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF
					&& Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene
					&& this.ApiErrorMessage.Contains("審査中"))
				{
					response = new OrderRegisterResponse
					{
						Result = true,
						Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
						MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
						DataResult = new OrderRegisterResponse.Data()
						{
							Status = "reviewing"
						}
					};
					return response;
				}

				errorList.Add(BotchanMessageManager.MessagesCode.REGIST_ORDER_FAILURE);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.REGIST_ORDER_FAILURE.ToString())
					.Replace("@@ 1 @@", string.Join("#", this.ErrorMessages));
				return new OrderRegisterResponse();
			}
			return response;
		}

		/// <summary>
		/// 初回広告コード取得
		/// </summary>
		/// <param name="advCodeNow">現在の広告コード</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <returns>初回広告コード</returns>
		protected string GetAdvCodeFirst(string advCodeNow, string advCodeFirst)
		{
			// 初回広告コードがあれば
			if (string.IsNullOrEmpty(advCodeFirst) == false) return advCodeFirst;

			return StringUtility.ToEmpty(advCodeNow);
		}

		/// <summary>
		/// 注文完了時の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>アラート文言</returns>
		public override string OrderCompleteProcesses(
			Hashtable order,
			CartObject cart,
			UpdateHistoryAction updateHistoryAction)
		{
			var dispAlertMessages = new StringBuilder();
			try
			{
				// 注文同梱で既存の注文台帳を更新する場合スキップ(親注文定期購購入なしでかつ子注文定期購入あり)
				if (cart.HasFixedPurchase
					&& (string.IsNullOrEmpty(cart.OrderCombineParentOrderId)
						|| ((cart.IsCombineParentOrderHasFixedPurchase == false)
							&& cart.IsBeforeCombineCartHasFixedPurchase)))
				{
					this.TransactionName = "4-1.定期購入ステータス更新処理";

					//注文者のリージョンデータを最新に更新
					cart.Owner.UpdateRegion(RegionManager.GetInstance().Region);
					// 仮登録の定期台帳を更新（更新履歴とともに）
					UpdateFixedPurchaseStatusTempToNormal(order, UpdateHistoryAction.Insert);
				}

				// コンテンツログ登録
				if (cart.ContentsLog != null)
				{
					var orderKbn = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_KBN]);
					var contentsLog = new ContentsLogModel(cart.ContentsLog.DataSource)
					{
						AccessKbn =
							(orderKbn == Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE)
								? Constants.FLG_CONTENTSLOG_ACCESS_KBN_SP
								: Constants.FLG_CONTENTSLOG_ACCESS_KBN_PC,
						ReportType = Constants.FLG_CONTENTSLOG_REPORT_TYPE_CV,
						Date = DateTime.Now,
						Price = cart.PriceTotal,
					};
					new ContentsLogService().Insert(contentsLog);
				}

				this.TransactionName = "4-2-1.メール送信処理";

				if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE
					&& (order[ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] != null))
				{
					SendUserRegisterMail((Hashtable)order[ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE]);
				}

				if (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				{
					if (string.IsNullOrEmpty(cart.OrderCombineParentOrderId))
					{
						// 注文完了メールを送信
						SendOrderMails(order, cart, this.IsUser);
					}
					else
					{
						// 注文同梱完了メールを送信
						OrderCombineUtility.SendOrderCombineCompleteMailToUser(order, cart);
					}
				}

				this.TransactionName = "4-3クレジット登録確定処理";
				dispAlertMessages.Append(
					UpdateUserCreditCard(order, cart, this.IsUser, UpdateHistoryAction.DoNotInsert));

				// 注文同梱された場合、注文対象のカートがDBにないため削除処理をスキップ
				if (string.IsNullOrEmpty(cart.OrderCombineParentOrderId))
				{
					this.TransactionName = "4-4.カート削除処理"; // セッションからはまだ消えない

					dispAlertMessages.Append(DeleteCart(order, cart));

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertAllForOrder(
							StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
							this.LastChanged);
					}
				}
			}
			catch (Exception exception)
			{
				throw new Exception(this.TransactionName + " でエラーが発生しました。", exception);
			}
			return dispAlertMessages.ToString();
		}

		/// <summary>
		/// 外部決済かどうかチェック
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>外部決済か</returns>
		protected override bool CheckExternalPayment(Hashtable order, CartObject cart)
		{
			return false;
		}

		/// <summary>
		/// 注文完了後の処理（セッションを利用するもの）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public override void AfterOrderCompleteProcesses(
			Hashtable order,
			CartObject cart,
			UpdateHistoryAction updateHistoryAction)
		{
			try
			{
				this.TransactionName = "5-1.アフィリエイト情報登録処理";

				RegistAffiliateInfo(order, HttpContext.Current.Request);

				this.TransactionName = "5-2.GoogleAnalytics用パラメータ作成";

				this.GoogleAnalyticsParams.Add(
					CreateGoogleAnalyticsParams(
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
						cart));

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertAllForOrder(
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
						this.LastChanged);
				}

				// 頒布会の次回配送商品を変更
				UpdateNextSubscriptionBoxProduct(cart, order);
			}
			catch (Exception exception)
			{
				throw new Exception(this.TransactionName + " でエラーが発生しました。", exception);
			}
		}

		/// <summary>
		/// 注文完了スキップ時の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		public override void SkipOrderCompleteProcesses(Hashtable order, CartObject cart)
		{
			try
			{
				this.GoogleAnalyticsParams.Add(
					CreateGoogleAnalyticsParams(
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
						cart));
			}
			catch (Exception exception)
			{
				throw new Exception("5-1.GoogleAnalytics用パラメータ作成でエラーが発生しました。", exception);
			}
		}

		/// <summary>
		/// アフィリエイト情報登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="request">HTTPリクエスト</param>
		public static void RegistAffiliateInfo(Hashtable order, HttpRequest request)
		{
			var linkshareCookie = CookieManager.Get("Affiliate_LinkShare");

			if ((Constants.AFFILIATE_LINKSHARE_VALID) && (linkshareCookie != null))
			{
				using (var sqlAccessor = new SqlAccessor())
				using (var sqlStatement = new SqlStatement("AffiliateCoopLog", "InsertAffiliateCoopLog"))
				{
					var input = new Hashtable();
					input.Add(Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN, Constants.FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_LINKSHARE_REP);
					input.Add(Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID, order[Constants.FIELD_ORDER_ORDER_ID]);
					input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1, linkshareCookie.Values[Constants.REQUEST_KEY_LINK_AFFILIATE_LST_ID]);
					input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA2, linkshareCookie.Values[Constants.REQUEST_KEY_LINK_AFFILIATE_ARRIVE_DATETIME]);
					input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_STATUS, Constants.FLG_AFFILIATECOOPLOG_COOP_STATUS_WAIT);
					input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11, linkshareCookie.Values[Constants.REQUEST_KEY_LINK_AFFILIATE_TAG_ID]);

					var update = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
				}
			}
		}

		/// <summary>
		/// GoogleAnalytics用パラメータ作成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cart">カート情報</param>
		/// <returns>パラメータ情報</returns>
		protected static Hashtable CreateGoogleAnalyticsParams(string orderId, CartObject cart)
		{
			var orderParams = new Hashtable();

			// 注文情報
			orderParams.Add(Constants.FIELD_ORDER_ORDER_ID, orderId);
			orderParams.Add(Constants.FIELD_ORDER_USER_ID, cart.OrderUserId);
			orderParams.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, cart.PriceSubtotal);
			// 商品小計税額（w2_Order.order_price_subtotal_tax）は注文割引分が含まれるため、
			// 明細金額（税金額）（w2_OrderItem.item_price_tax）の合計を税額商品合計金額とする
			// ※アフィリエイトタグも同様の計算を行っている
			orderParams.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX + "_sum", cart.Items.Select(i => i.PriceTax).Sum());
			orderParams.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, cart.PriceTotal);

			// 配送料（配送料が0円の場合は空文字を設定）
			orderParams.Add(Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING,
				(cart.PriceShipping > 0)
					? StringUtility.ToEmpty(cart.PriceShipping)
					: string.Empty);

			// 注文者
			orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, cart.Owner.OwnerKbn);
			orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP, cart.Owner.Zip1 + "-" + cart.Owner.Zip2);
			orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, cart.Owner.Addr1);
			orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, cart.Owner.Addr2);
			orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_SEX, cart.Owner.Sex);
			orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH, cart.Owner.Birth);

			// 注文商品情報
			var items = new List<Hashtable>();
			foreach (CartProduct product in cart.Items)
			{
				// 同一商品が存在する？
				var tempItem = items.Find(tmpItem
					=> ((product.ShopId == StringUtility.ToEmpty(tmpItem[Constants.FIELD_ORDERITEM_SHOP_ID]))
						&& (product.ProductId == StringUtility.ToEmpty(tmpItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]))
						&& (product.VariationId == StringUtility.ToEmpty(tmpItem[Constants.FIELD_ORDERITEM_VARIATION_ID]))));
				if (tempItem != null)
				{
					// 数量を加算
					tempItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] =
						(int)tempItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] + product.CountSingle;
					continue;
				}

				// 追加
				var item = new Hashtable();
				item.Add(Constants.FIELD_ORDERITEM_SHOP_ID, cart.ShopId);
				item.Add(Constants.FIELD_ORDERITEM_ORDER_ID, orderId);
				item.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, product.ProductId);
				item.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, product.VariationId);
				item.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME, product.ProductJointName);
				item.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE, product.Price);
				item.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, product.TaxRate);
				item.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, product.CountSingle);
				items.Add(item);
			}

			orderParams.Add("order_items", items);
			return orderParams;
		}

		/// <summary>
		/// 定期購入次回配送日の取得
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="shopShipping">配送種別情報</param>
		/// <param name="baseDate">基準日</param>
		/// <remarks>未来かどうかで、次々回→次回にスライド or 再計算 を切り替え</remarks>
		/// <returns>日付</returns>
		private DateTime GetNextShippingDate(FixedPurchaseModel fixedPurchase, ShopShippingModel shopShipping, DateTime baseDate)
		{
			DateTime dateTime;
			if (CheckFutureShippingDate(fixedPurchase.NextNextShippingDate.Value))
			{
				dateTime = fixedPurchase.NextNextShippingDate.Value;
			}
			else
			{
				var service = new FixedPurchaseService();
				var calculateMode = service.GetCalculationMode(
					fixedPurchase.FixedPurchaseKbn,
					Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
				dateTime = service.CalculateFollowingShippingDate(
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					baseDate,
					shopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);
			}
			return dateTime;
		}

		/// <summary>
		/// 定期購入次々回配送日の取得
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="shopShipping">配送種別情報</param>
		/// <param name="baseDate">基準日</param>
		/// <remarks>未来かどうかで、次々回→次回にスライド or 再計算 を切り替え</remarks>
		/// <returns>日付</returns>
		private DateTime GetNextNextShippingDate(FixedPurchaseModel fixedPurchase, ShopShippingModel shopShipping, DateTime baseDate)
		{
			DateTime dateTime;
			var service = new FixedPurchaseService();
			var calculateMode = service.GetCalculationMode(
				fixedPurchase.FixedPurchaseKbn,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			if (CheckFutureShippingDate(fixedPurchase.NextNextShippingDate.Value))
			{
				dateTime = service.CalculateFollowingShippingDate(
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					fixedPurchase.NextNextShippingDate.Value,
					shopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);
			}
			else
			{
				dateTime = service.CalculateNextNextShippingDate(
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					baseDate,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);
			}
			return dateTime;
		}

		/// <summary>
		/// 広告コード存在チェック
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strAdvCode">広告コード</param>
		/// <returns>true:存在</returns>
		private static bool AdvCodeIsValid(string strDeptId, string strAdvCode)
		{
			var blIsValid = false;

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("AdvCode", "GetAdvCodeCount"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_ADVCODE_DEPT_ID, strDeptId },
					{ Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, strAdvCode }
				};

				blIsValid = ((int)sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput)[0][0] != 0);
			}

			return blIsValid;
		}

		/// <summary>
		/// 広告コード履歴登録
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strAdvCode">広告コード</param>
		/// <param name="strCareerId">キャリアID</param>
		private static void AdvCodeLogRegister(string strDeptId, string strAdvCode, string strCareerId)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("AdvCode", "InsertAdvCodeLog"))
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				var htInput = new Hashtable
				{
					{ Constants.FIELD_ADVCODE_DEPT_ID, strDeptId },
					{ Constants.FIELD_ADVCODELOG_ACCESS_DATE, DateTime.Now.ToString("yyyy/MM/dd") },
					{ Constants.FIELD_ADVCODELOG_ACCESS_TIME, DateTime.Now.ToString("HH:mm:ss") },
					{ Constants.FIELD_ADVCODELOG_ADVERTISEMENT_CODE, strAdvCode },
					{ Constants.FIELD_ADVCODELOG_CAREER_ID, strCareerId },
					{ Constants.FIELD_ADVCODELOG_MOBILE_UID, "" },
					{ Constants.FIELD_ADVCODELOG_ACCESS_USER_ID, DateTime.Now.ToString("yyyyMMdd") + "_" + Guid.NewGuid() }
				};
				sqlStatement.ExecStatement(sqlAccessor, htInput);

				sqlAccessor.CommitTransaction();
				sqlAccessor.CloseConnection();
			}
		}

		/// <summary>
		/// 配送日が未来か？
		/// </summary>
		/// <param name="shippingDate">配送日</param>
		/// <returns>true:未来</returns>
		private bool CheckFutureShippingDate(DateTime shippingDate)
		{
			return shippingDate > DateTime.Today;
		}

		/// <summary>GoogleAnaliticsログ出力用の注文情報</summary>
		public List<Hashtable> GoogleAnalyticsParams { get; set; }
	}
}
