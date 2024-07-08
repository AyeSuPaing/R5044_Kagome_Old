/*
=========================================================================================================
  Module      : Recalculation Api Facade(RecalculationApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using w2.App.Common.Botchan;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.App.Common.Recommend;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.ContentsLog;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Holiday.Helper;
using w2.Domain.Point;
using w2.Domain.Recommend;
using w2.Domain.ShopShipping;
using w2.Domain.User;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Recalculation api facade
	/// </summary>
	[Serializable]
	public class RecalculationApiFacade : OrderCommon
	{
		/// <summary>
		/// Create cart by request
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモー</param>
		/// <returns>Cart object</returns>
		public CartObject CreateCartByRequest(
			RecalculationRequest request,
			ref List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var cart = request.CartObject;
			var cartDicsount = request.DiscountInfoObject;
			var cartShippings = request.OrderShippingObject;
			var cartProducts = request.OrderProductLists;
			var cartPaymentObject = request.OrderPaymentObject;
			var cartOrderOwner = request.OrderOwnerObject;

			var isGuestUser = string.IsNullOrEmpty(cart.UserId);
			var memberRankId = string.Empty;
			var user = new UserModel();

			if (string.IsNullOrEmpty(cart.UserId) == false)
			{
				user = new UserService().Get(cart.UserId);
				if (user == null)
				{
					errorList.Add(BotchanMessageManager.MessagesCode.USER_NOT_EXISTS);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.USER_NOT_EXISTS.ToString())
						.Replace("@@ 1 @@", cart.UserId);
					return null;
				}
				memberRankId = MemberRankOptionUtility.GetMemberRankId(cart.UserId);
			}

			var orderKbn = (Constants.SMARTPHONE_OPTION_ENABLED && (cart.OrderKbn == Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE))
				? Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE
				: Constants.FLG_ORDER_ORDER_KBN_PC;

			var cartList = string.IsNullOrEmpty(cart.CartId)
				? new CartObjectList(
					cart.UserId,
					orderKbn,
					true)
				: CartObjectList.GetUserCartList(
					cart.UserId,
					orderKbn,
					string.Empty);

			var cartObject = cartList.GetCart(cart.CartId);

			if (cartObject != null) DeleteCart(cartObject.CartId);

			cartList.Items.RemoveRange(0, cartList.Items.Count);
			cartObject = new CartObject(
				cart.UserId,
				orderKbn,
				Constants.CONST_DEFAULT_SHOP_ID,
				string.Empty,
				false,
				true,
				memberRankId);

			var fixedPurchaseId = cartObject.HasFixedPurchase
				? OrderCommon.CreateFixedPurchaseId(cartObject.ShopId)
				: string.Empty;

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

				var productShippingType = StringUtility.ToEmpty(productData[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE]);
				if ((cartObject.Items.Count > 0) && (cartObject.Items.First().ShippingType != productShippingType))
				{
					errorList.Add(BotchanMessageManager.MessagesCode.PRODUCT_SHIPPING_KBN_DIFF);
					return null;
				}

				var cartProduct = new CartProduct(
					productData[0],
					RecalculationUtility.GetAddCartKbn(cart.OrderDivision),
					StringUtility.ToEmpty(productData[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
					product.ProductCount,
					true,
					new ProductOptionSettingList(),
					fixedPurchaseId,
					new ContentsLogModel());

				if (string.IsNullOrEmpty(cart.CartId) == false)
				{
					cartObject.UpdateCartId(cart.CartId);
				}

				var sameCartProduct = cartObject.GetSameProduct(cartProduct);
				if (cartObject.CheckProductMaxSellQuantity(cartProduct, sameCartProduct))
				{
					sameCartProduct.SetProductCount(cartObject.CartId, sameCartProduct.CountSingle + product.ProductCount);
					cartObject.Calculate(true, isCartItemChanged: true);
				}
				else
				{
					cartObject.Add(cartProduct);
				}

				cartObject.Payment = new CartPayment()
				{
					PaymentId = cartPaymentObject.PaymentId,
				};

				if (cartList.Items.Count == 0)
				{
					cartList.Items.Add(cartObject);
				}
				else
				{
					cartList.Items[0] = cartObject;
				}

				var isFixedPurchase = (cart.OrderDivision == Constants.FLG_ADD_CART_KBN_FIXEDPURCHASE);
				var productFixedPurchaseFlg = StringUtility.ToEmpty(productData[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]);
				if ((isFixedPurchase && (Constants.FIXEDPURCHASE_OPTION_ENABLED == false))
					|| (isFixedPurchase && (productFixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)))
				{
					errorList.Add(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_FIXED_PURCHASE_INVALID);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_FIXED_PURCHASE_INVALID.ToString())
						.Replace("@@ 1 @@", cartProduct.ProductJointName);
					return null;
				}
				if ((isFixedPurchase == false) && (productFixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
				{
					errorList.Add(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_INVALID);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_INVALID.ToString())
						.Replace("@@ 1 @@", cartProduct.ProductJointName);
					return null;
				}

				// レコメンド処理
				if (StringUtility.ToEmpty(cart.RecommendFlag) == Database.Common.Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_VALID)
				{
					if (Constants.RECOMMEND_OPTION_ENABLED)
					{
						var recommend = new RecommendExtractor(
								cartObject.CartUserId,
								memberRankId,
								cartList.Items.ToArray())
							.Exec(cartList.Items.ToArray());

						if ((recommend != null)
						    && (recommend.ChatbotUseFlg == Database.Common.Constants.FLG_RECOMMEND_CHATBOT_USE_FLG_VALID)
						    && (recommend.CanDisplayOrderConfirmPage))
						{
							cartList.Items.First().ShippingType = cartObject.Items.First().ShippingType;
							cartList.AddRecommendItem(recommend);
							cartObject = cartList.Items.FirstOrDefault();
							cartObject.TargetProductRecommends.Add(cartProduct);
							if (recommend.Items.First().IsFixedPurchase)
							{
								cartShippings.FixedPurchaseKbn = recommend.Items.First().FixedPurchaseKbn;
								cartShippings.CourseBuySetting = recommend.Items.First().FixedPurchaseSetting1;
							}
						}
					}
					else
					{
						errorList.Add(BotchanMessageManager.MessagesCode.RECOMMEND_OPTION_OFF);
						memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.RECOMMEND_OPTION_OFF.ToString());
						return null;
					}
				}

				var validPaymentList = GetValidPaymentList(cartObject, cart.UserId);
				var canUserPayment = validPaymentList.Any(item => (item.PaymentId == cartPaymentObject.PaymentId));
				// 支払い制限
				if (cartProduct.LimitedPaymentIds.Contains(cartPaymentObject.PaymentId) && canUserPayment)
				{
					errorList.Add(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_LIMITED_PAYMENT);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_LIMITED_PAYMENT.ToString())
						.Replace("@@ 1 @@", cartProduct.ProductJointName); ;
					return null;
				}

				// 商品処理
				var dvProduct = ProductCommon.GetProductVariationInfo(
					Domain.Constants.CONST_DEFAULT_SHOP_ID,
					product.ProductId,
					product.VariationId,
					memberRankId);
				var messagesCode = CheckCartProductForChatBot(cartProduct, dvProduct, cart.UserId, memberRankId);
				if (messagesCode != BotchanMessageManager.MessagesCode.NONE)
				{
					errorList.Add(messagesCode);
					memo = BotchanMessageManager.ConversionErrorMessage(messagesCode, cartObject.Coupon, cartProduct, dvProduct);
					return null;
				}
			}

			if (cartObject.Items.Count == 0) return null;

			// ノベルティ処理
			if (Constants.NOVELTY_OPTION_ENABLED
				&& (cart.AddNoveltyFlag == Constants.FLG_NOVELTY_VALID_FLG_VALID)
				&& (string.IsNullOrEmpty(cart.CartId) == false))
			{
				var cartNoveltyList = new CartNoveltyList(cartList);
				var noveltyItems = BotChanUtility.CreateNoveltyItems(
					cart.CartId,
					cartNoveltyList,
					cartList).ToList();

				noveltyItems.ForEach(item => cartObject.Add(item));
			}

			if (cartDicsount == null) cartDicsount = new RecalculationRequest.DiscountInfo();

			if (isGuestUser && (cartOrderOwner == null))
			{
				var shippingZip = cartShippings.ShippingZip.Split('-');
				cartObject.Owner = new CartOwner(
					string.Empty,
					cartShippings.ShippingName,
					cartShippings.ShippingName1,
					cartShippings.ShippingName2,
					cartShippings.ShippingNameKana,
					cartShippings.ShippingNameKana1,
					cartShippings.ShippingNameKana2,
					string.Empty,
					string.Empty,
					cartShippings.ShippingZip,
					(shippingZip.Length > 0) ? shippingZip[0] : string.Empty,
					(shippingZip.Length > 1) ? shippingZip[1] : string.Empty,
					cartShippings.ShippingAddr1,
					cartShippings.ShippingAddr2,
					cartShippings.ShippingAddr3,
					cartShippings.ShippingAddr4,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					cartShippings.ShippingTel1,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					true,
					string.Empty,
					null,
					RegionManager.GetInstance().Region.CountryIsoCode,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId,
					RegionManager.GetInstance().Region.CurrencyCode,
					RegionManager.GetInstance().Region.CurrencyLocaleId);
			}
			else if (isGuestUser)
			{
				var ownerZip = cartOrderOwner.Zip.Split('-');
				cartObject.Owner = new CartOwner(
					"",
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

			// クーポン処理
			if (string.IsNullOrEmpty(cartDicsount.CouponCode) == false)
			{
				if (Constants.W2MP_COUPON_OPTION_ENABLED)
				{
					var coupons = new CouponService().GetAllUserCouponsFromCouponCode(
						Constants.W2MP_DEPT_ID,
						cart.UserId,
						cartDicsount.CouponCode);
					if ((coupons == null) || (coupons.Length == 0))
					{
						errorList.Add(BotchanMessageManager.MessagesCode.COUPON_NO_COUPON_ERROR);
						memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.COUPON_NO_COUPON_ERROR.ToString());
						return null;
					}
					var coupon = coupons.First();
					cartObject.Coupon = new CartCoupon(coupon);
					if (CouponCheck(cart.UserId, cartObject, coupon, ref errorList, ref memo) == false) return null;
				}
				else
				{
					errorList.Add(BotchanMessageManager.MessagesCode.COUPON_OPTION_OFF);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.COUPON_OPTION_OFF.ToString());
					return null;
				}
			}

			// ポイント処理
			if (string.IsNullOrEmpty(cartDicsount.OrderPointUse) == false) {
				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
	
					var usePoint = decimal.Parse(cartDicsount.OrderPointUse);
					cartObject.SetUsePoint(usePoint, RecalculationUtility.GetPointUsePrice(usePoint));
					if (PointCheck(cart.UserId, cartObject, usePoint, ref errorList, ref memo) == false) return null;
				}
				else
				{
					errorList.Add(BotchanMessageManager.MessagesCode.POINT_OPTION_OFF);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.POINT_OPTION_OFF.ToString());
					return null;
				}
			}

			cartObject.ReceiptFlg = request.OrderPaymentObject.ReceiptFlg ?? "0";
			cartObject.Calculate(false, true, true, true);

			if (cartObject.UsePointPrice >
				(TaxCalculationUtility.GetPriceTaxIncluded(cartObject.PriceSubtotal, cartObject.PriceSubtotalTax)
				 - cartObject.MemberRankDiscount
				 - cartObject.UseCouponPriceForProduct
				 - cartObject.FixedPurchaseDiscount
				 - cartObject.FixedPurchaseMemberDiscountAmount)
			)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.COUPON_PRICE_SUBTOTAL_MINUS_COUPON_ERROR);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.COUPON_PRICE_SUBTOTAL_MINUS_COUPON_ERROR.ToString());
				return null;
			}

			// 配送希望日処理
			var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cartObject.Items.First().ShippingType);
			if (string.IsNullOrEmpty(cartShippings.ShippingDate) == false)
			{
				var shippingDateList = GetShippingDate(shopShipping, DateTimeUtility.FormatType.ShortDate2Letter);
				if (shippingDateList.Contains(cartShippings.ShippingDate) == false)
				{
					errorList.Add(BotchanMessageManager.MessagesCode.NOT_ALLOWED_SHIPPING_DATE);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.NOT_ALLOWED_SHIPPING_DATE.ToString())
						+ string.Join(",", shippingDateList);
					return null;
				}
			}

			// カート作成処理
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
				{ Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE, Constants.COUNTRY_ISO_CODE_JP },
			};
			cartShipping.UpdateShippingAddr(shippingAddr);
	
			cartShipping.ShippingMethod = string.IsNullOrEmpty(cartShippings.ShippingMethod)
				? Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS
				: cartShippings.ShippingMethod;
			cartObject.Shippings.First().UpdateShippingAddr(cartShipping);
			if (string.IsNullOrEmpty(cartShippings.ShippingDate) == false)
			{
				cartObject.Shippings.First().ShippingDate = DateTime.Parse(cartShippings.ShippingDate);
				cartObject.Shippings.First().SpecifyShippingDateFlg = true;
			}
			if (string.IsNullOrEmpty(cartShippings.ShippingTime) == false)
			{
				cartObject.Shippings.First().ShippingTime = cartShippings.ShippingTime;
				cartObject.Shippings.First().SpecifyShippingTimeFlg = true;
			}

			var zipcodeUtil = new ZipcodeSearchUtility(cartShippings.ShippingZip.Replace("-", string.Empty));
			if (zipcodeUtil.Success)
			{
				var zipAddress2 = string.Format("{0}{1}", zipcodeUtil.CityName, zipcodeUtil.TownName);
				if ((zipcodeUtil.PrefectureName != cartShipping.Addr1) && (zipAddress2 != cartShipping.Addr2))
				{
					errorList.Add(BotchanMessageManager.MessagesCode.FRONT_ZIPCODE_NO_ADDR);
					return null;
				}
			}
			else
			{
				errorList.Add(BotchanMessageManager.MessagesCode.FRONT_ZIPCODE_NO_ADDR);
				return null;
			}

			cartObject.Payment = new CartPayment()
			{
				PaymentId = cartPaymentObject.PaymentId,
			};
			cartObject.Payment.PriceExchange = GetPaymentPrice(
				cartObject.ShopId,
				cartObject.Payment.PaymentId,
				cartObject.PriceSubtotal,
				cartObject.PriceCartTotalWithoutPaymentPrice);

			if (cartObject.Payment.PaymentId == Database.Common.Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
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
						cartObject.Payment.UserCreditCardName =
							cartObject.Payment.UserCreditNameComplementFlg
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
			cartObject.SettlementCurrency = CurrencyManager.GetSettlementCurrency(cartObject.Payment.PaymentId);
			cartObject.SettlementRate = CurrencyManager.GetSettlementRate(cartObject.SettlementCurrency);
			cartObject.SettlementAmount = CurrencyManager.GetSettlementAmount(
				cartObject.PriceTotal,
				cartObject.SettlementRate,
				cartObject.SettlementCurrency);

			if (cartObject.HasFixedPurchase)
			{
				if (string.IsNullOrEmpty(cartShippings.FixedPurchaseKbn)
					|| string.IsNullOrEmpty(cartShippings.CourseBuySetting))
				{
					errorList.Add(BotchanMessageManager.MessagesCode.FIXEDPURCHASEORDER_ERROR);
					return null;
				}

				if ((((cartShippings.FixedPurchaseKbn == "01") && (shopShipping.FixedPurchaseKbn1Flg == "1"))
					|| ((cartShippings.FixedPurchaseKbn == "02") && (shopShipping.FixedPurchaseKbn2Flg == "1"))
					|| ((cartShippings.FixedPurchaseKbn == "03") && (shopShipping.FixedPurchaseKbn3Flg == "1"))) == false)
				{

					var canSetFixedPurchaseKbn = new List<string>();
					if (shopShipping.FixedPurchaseKbn1Flg == "1") canSetFixedPurchaseKbn.Add("01");
					if (shopShipping.FixedPurchaseKbn2Flg == "1") canSetFixedPurchaseKbn.Add("02");
					if (shopShipping.FixedPurchaseKbn3Flg == "1") canSetFixedPurchaseKbn.Add("03");

					errorList.Add(BotchanMessageManager.MessagesCode.NOT_ALLOWED_FIXEDPURCHASE_KBN);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.NOT_ALLOWED_FIXEDPURCHASE_KBN.ToString())
						+ string.Join(",", canSetFixedPurchaseKbn);
					return null;
				}

				var fixedPurchaseKbn1SettingList = shopShipping.FixedPurchaseKbn1Setting.Split(',');
				var fixedPurchaseKbn3SettingList = shopShipping.FixedPurchaseKbn3Setting
					.Replace("(","").Replace(")","").Split(',');

				if ((cartShippings.FixedPurchaseKbn == "01") || (cartShippings.FixedPurchaseKbn == "02"))
				{
					var monthInterval = cartShippings.CourseBuySetting.Split(',')[0];
					if (fixedPurchaseKbn1SettingList.Contains(monthInterval) == false)
					{
						errorList.Add(BotchanMessageManager.MessagesCode.NOT_ALLOWED_FIXEDPURCHASE_MONTHLY_INTERVAL);
						memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.NOT_ALLOWED_FIXEDPURCHASE_MONTHLY_INTERVAL.ToString())
							+ shopShipping.FixedPurchaseKbn1Setting;
						return null;
					}
				}
				if (cartShippings.FixedPurchaseKbn == "03")
				{
					var dayInterval = cartShippings.CourseBuySetting;
					if (fixedPurchaseKbn3SettingList.Contains(dayInterval) == false)
					{
						errorList.Add(BotchanMessageManager.MessagesCode.NOT_ALLOWED_FIXEDPURCHASE_DAY_INTERVAL);
						memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.NOT_ALLOWED_FIXEDPURCHASE_DAY_INTERVAL.ToString())
							+ shopShipping.FixedPurchaseKbn3Setting;
						return null;
					}
				}

				var creditBranchNoTmp = 0;
				var now = DateTime.Now;
				var fixedPurchase = new FixedPurchaseModel
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
					LastChanged = Constants.FLG_LASTCHANGED_BOTCHAN,
				};

				var nextShippingDate = GetNextShippingDate(fixedPurchase, shopShipping);
				var nextNextShippingDate = GetNextNextShippingDate(fixedPurchase, shopShipping);

				cartObject.Shippings.First().UpdateNextShippingDates(nextShippingDate, nextNextShippingDate);
				cartObject.Shippings.First().UpdateFixedPurchaseSetting(
					cartShippings.FixedPurchaseKbn,
					cartShippings.CourseBuySetting,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan);
			}

			cartObject.Calculate(false, true, true, true);

			if (string.IsNullOrEmpty(cartShippings.ShippingMethod) == false)
			{
				if ((cartShippings.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
					&& IsAvailableShippingKbnMail(cartObject.Items) == false)
				{
					errorList.Add(BotchanMessageManager.MessagesCode.NOT_ALLOWED_USE_SHIPPING_KBN_MAIL);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.NOT_ALLOWED_USE_SHIPPING_KBN_MAIL.ToString());
					return null;
				}
				cartObject.Shippings[0].ShippingMethod = cartShippings.ShippingMethod;
			}

			if ((cartObject.IsMailDelivery && (cartPaymentObject.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)))
			{
				errorList.Add(BotchanMessageManager.MessagesCode.FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_COLLECT);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode
					.FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_COLLECT.ToString());
				return null;
			}

			// 配送不可エリアチェック
			var unavailableShipping = BotChanUtility.CheckUnavailableShippingAreaForBotChat(cartObject);
			if (unavailableShipping)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.UNAVAILABLE_SHIPPING_AREA_ERROR);
				return null;
			}

			return cartObject;
		}

		/// <summary>
		/// ポイントチェック処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cartObject">カートオブジェクト</param>
		/// <param name="usePoint">ポイント</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモ</param>
		/// <returns></returns>
		private static bool PointCheck(
			string userId,
			CartObject cartObject,
			decimal usePoint,
			ref List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var pointMaster = new PointService().GetPointMaster().Where(
				x => x.PointKbn == Constants.FLG_POINT_POINT_KBN_BASE).ToList();

			var userPoint = PointOptionUtility.GetUserPoint(userId);
			var userPointCorrectedZero = (userPoint.PointUsable) > 0 ? userPoint.PointUsable : 0;
			if (usePoint > userPointCorrectedZero)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.FRONT_POINT_USE_MAX_ERROR);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_POINT_USE_MAX_ERROR.ToString())
					.Replace("@@ 1 @@", userPointCorrectedZero.ToString("#"));
				return false;
			}

			if (usePoint > cartObject.PointUsablePrice)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR);
				memo = MessageManager
					.GetMessages(BotchanMessageManager.MessagesCode.FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR.ToString())
					.Replace("@@ 1 @@", cartObject.PriceSubtotal.ToString("#"));
				return false;
			}

			if (pointMaster.Any())
			{
				var usableUnit = pointMaster.First().UsableUnit;
				if ((usePoint % usableUnit) != 0)
				{
					errorList.Add(BotchanMessageManager.MessagesCode.FRONT_POINT_USABLE_UNIT_ERROR);
					memo = MessageManager
						.GetMessages(BotchanMessageManager.MessagesCode.FRONT_POINT_USABLE_UNIT_ERROR.ToString())
						.Replace("@@ 1 @@", usableUnit.ToString());
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// クーポンチェック処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cartObject">カートオブジェクト</param>
		/// <param name="coupon"></param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモ</param>
		/// <returns>チェック結果</returns>
		private static bool CouponCheck(
			string userId,
			CartObject cartObject,
			UserCouponDetailInfo coupon,
			ref List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var couponErrorCode = CouponOptionUtility.CheckUseCoupon(coupon, StringUtility.ToEmpty(userId), cartObject.Owner.MailAddr);
			var error = BotchanMessageManager.GetErrorMessage(couponErrorCode);
			if (error != BotchanMessageManager.MessagesCode.NONE)
			{
				errorList.Add(error);
				memo = BotchanMessageManager.ConversionErrorMessage(error, cartObject.Coupon);
				return false;
			}

			if (coupon.ValidFlg == Constants.FLG_COUPON_VALID_FLG_INVALID)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.COUPON_INVALID_ERROR);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.COUPON_INVALID_ERROR.ToString())
					.Replace("@@ 1 @@", coupon.CouponDispName);
				return false;
			}

			if (cartObject.Items.Where(item => item.ProductType == Constants.FLG_PRODUCT_PRODUCT_TYPE_PRODUCT)
				    .Any(cartProduct => CouponOptionUtility.IsCouponApplyCartProduct(coupon, cartProduct)) == false)
			{
				errorList.Add(BotchanMessageManager.MessagesCode.COUPON_TARGET_PRODUCT_ERROR);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.COUPON_TARGET_PRODUCT_ERROR.ToString())
					.Replace("@@ 1 @@", coupon.CouponDispName);
				return false;
			}

			foreach (var cp in cartObject.Items.Where(cp => CouponOptionUtility.IsCouponApplyCartProduct(coupon, cp) == false))
			{
				errorList.Add(BotchanMessageManager.MessagesCode.COUPON_EXCEPTIONAL_PRODUCT_ERROR);
				memo = MessageManager
					.GetMessages(BotchanMessageManager.MessagesCode.COUPON_EXCEPTIONAL_PRODUCT_ERROR.ToString())
					.Replace("@@ 1 @@", coupon.CouponDispName)
					.Replace("@@ 2 @@", cp.ProductJointName);
				return false;
			}

			if (coupon.UsablePrice.HasValue)
			{
				if (cartObject.PriceSubtotal < coupon.UsablePrice.Value)
				{
					errorList.Add(BotchanMessageManager.MessagesCode.COUPON_USABLE_PRICE_ERROR);
					memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.COUPON_USABLE_PRICE_ERROR.ToString())
						.Replace("@@ 1 @@", coupon.CouponDispName)
						.Replace("@@ 2 @@", CurrencyManager.ToPrice(coupon.UsablePrice.Value));
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 配送希望日取得
		/// </summary>
		/// <param name="shopShipping">店舗配送種別マスタ</param>
		/// <param name="formatType">日付フォマード</param>
		private static List<string> GetShippingDate(ShopShippingModel shopShipping, DateTimeUtility.FormatType formatType)
		{
			var startAddDayCount = shopShipping.ShippingDateSetBegin.GetValueOrDefault(0);
			var maxAddDayCount = shopShipping.ShippingDateSetEnd.GetValueOrDefault(0);
			var startDateAfterHoliday = HolidayUtil.GetDateOfBusinessDay(
				DateTime.Now,
				shopShipping.BusinessDaysForShipping,
				true);

			var result = new List<string>();
			for (var loop = startAddDayCount; loop < (startAddDayCount + maxAddDayCount); loop++)
			{
				var target = startDateAfterHoliday.AddDays(loop);
				result.Add(DateTimeUtility.ToStringFromRegion(target, formatType));
			}
			return result;
		}

		/// <summary>
		/// カート商品整合性チェック
		/// </summary>
		/// <param name="cpTarget">チェック対象カート商品</param>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="memberRankId">ランクID</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// 配送種別変更チェック → 商品状態チェック → 販売価格変更チェックの順にチェック
		/// </remarks>
		public static BotchanMessageManager.MessagesCode CheckCartProductForChatBot(
			CartProduct cpTarget,
			DataView drvProduct,
			string userId,
			string memberRankId)
		{
			var error = CheckCartProduct(cpTarget, drvProduct[0], userId);
			var result = (error == OrderErrorcode.NoError)
				? BotchanMessageManager.MessagesCode.NONE
				: BotchanMessageManager.GetMessagesCode(error);
			return result;
		}

		/// <summary>
		/// レスポンス作成
		/// </summary>
		/// <param name="cart">カート</param>
		/// <returns>レスポンス</returns>
		public RecalculationResponse CreateResponseByCart(CartObject cart)
		{
			var orderProducts = new List<RecalculationResponse.OrderProductList>();
			var recommendOriginallyProducts = new List<RecalculationResponse.RecommendOriginallyProductList>();
			var creditCardInfo = new RecalculationResponse.CreditCardInfo();

			foreach (var product in cart.Items)
			{
				var orderProduct = new RecalculationResponse.OrderProductList
				{
					ProductId = product.ProductId,
					VariationId = product.VariationId,
					ItemQuantity = product.Count.ToString(),
					ProductSaleId = product.ProductSaleId,
					ProductName = product.ProductName,
					ItemPrice = product.Price.ToString("#"),
					ProductTaxRate = product.PriceTax.ToString("#"),
					IsFixedPurchase = product.IsFixedPurchase,
				};
				orderProducts.Add(orderProduct);

				if (product.IsRecommendItem)
				{
					var recommend = new RecommendService().GetContainer(product.ShopId, product.RecommendId);
					var productTagetId = recommend.IsUpsell
						? recommend.UpsellTargetItem.RecommendUpsellTargetItemProductId
						: recommend.ApplyConditionItems.First().RecommendApplyConditionItemProductId;

					var productRecommend = cart.TargetProductRecommends.Find(item => item.ProductId == productTagetId);
					if(productRecommend == null) continue;
					var recommendOriginallyProductList = new RecalculationResponse.RecommendOriginallyProductList
					{
						ProductId = productRecommend.ProductId,
						VariationId = productRecommend.VariationId,
						ItemQuantity = productRecommend.Count.ToString(),
						ProductSaleId = productRecommend.ProductSaleId,
						ProductName = productRecommend.ProductName,
						ItemPrice = productRecommend.Price.ToString("#"),
						ProductTaxRate = productRecommend.PriceTax.ToString("#"),
						RecommendKbn = recommend.RecommendKbn,
						RecommendProductId = product.ProductId,
					};

					recommendOriginallyProducts.Add(recommendOriginallyProductList);
				}
			}

			// 指定可能配送希望日リスト作成
			var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cart.Items.First().ShippingType);
			var shippingDateList = GetShippingDate(shopShipping, DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);

			// 指定可能配送希望時間帯リスト作成
			var shippingTimeList = new DeliveryCompanyService().Get(
				shopShipping.GetDefaultDeliveryCompany(true).DeliveryCompanyId).GetShippingTimeList();

			var dataResult = new RecalculationResponse.Data
			{
				CartObject = new RecalculationResponse.Cart
				{
					CartId = cart.CartId,
					UserId = cart.CartUserId,
					ShopId = cart.ShopId,
					SupplierId = cart.Items.First().SupplierId,
					FixedPurchaseFlg = cart.Items.First().IsFixedPurchase
						? Constants.FLG_CART_FIXED_PURCHASE_FLG_ON
						: Constants.FLG_CART_FIXED_PURCHASE_FLG_OFF,
				},

				ReceiveOrderInfoObject = new RecalculationResponse.ReceiveOrderInfo
				{
					OrderPriceSubtotal = cart.PriceSubtotal.ToString("#"),
					OrderPriceTax = cart.PriceTax.ToString("#"),
					OrderPriceShipping = cart.PriceShipping.ToString("#"),
					OrderPriceExchange = cart.Payment.PriceExchange.ToString("#"),
					OrderPriceRegulation = cart.PriceRegulation.ToString("#"),
					OrderPriceRepayment = null,
					OrderPriceTotal = cart.PriceTotal.ToString("#"),
					OrderDiscountSetPrice = cart.TotalPriceDiscount.ToString("#"),
					OrderPointUse = cart.UsePoint.ToString("#"),
					OrderPointUseYen = cart.UsePointPrice.ToString("#"),
					OrderPointAdd = cart.AddPoint.ToString("#"),
					CardKbn = (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						? cart.OrderKbn
						: null,
					CardInstruments = (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						? cart.Payment.CreditInstallmentsCode
						: null,
					CardTranId = null,
					ShippingId = cart.ShippingType,
					MemberRankDiscountPrice = cart.MemberRankDiscount.ToString("#"),
					DigitalContentsFlg = cart.Items.First().IsDigitalContents
						? Constants.FLG_CART_DIGITAL_CONTENTS_FLG_ON
						: Constants.FLG_CART_DIGITAL_CONTENTS_FLG_OFF,
					OnlinePaymentStatus = cart.OnlinePaymentStatus,
					FixedPurchaseDiscountPrice = cart.FixedPurchaseDiscount.ToString("#"),
					PaymentOrderId = null,
					FixedPurchaseMemberDiscountAmount = cart.FixedPurchaseMemberDiscountAmount.ToString(),
					LastBilledAmount = cart.PriceTotal.ToString("#"),
					ExternalPaymentStatus = null,
					ExternalPaymentErrorMessage = null,
					ExternalPaymentAuthDate = null,
					OrderPriceSubtotalTax = cart.PriceSubtotalTax.ToString("#"),
					OrderPriceShippingTax = cart.PriceShipping.ToString("#"),
					ShippingMemo = cart.ShippingMemo,
					ExternalPaymentCooperationLog = null,
					ShippingTaxRate = cart.ShippingTaxRate.ToString("#"),
					PaymentTaxRate = cart.PaymentTaxRate.ToString("#"),
					OrderCountOrder = null,
					InvoiceBundleFlg = cart.GetInvoiceBundleFlg(),
					ReceiptFlg = cart.ReceiptFlg,
					ReceiptOutputFlg = null,
					ReceiptAddress = cart.ReceiptAddress,
					ReceiptProviso = cart.ReceiptProviso,
					FixedPurchaseKbn = cart.Shippings[0].FixedPurchaseKbn,
					FixedPurchaseSetting = cart.Shippings[0].FixedPurchaseSetting,
				},
				OrderProducts = orderProducts.ToArray(),
				ShippingDateList = shippingDateList,
				ShippingTimes = shippingTimeList.Select(item => 
					new RecalculationResponse.ShippingTimeList { ShippingTimeId = item.Key, ShippingTimeMessage = item.Value }).ToArray(),
			};

			if (recommendOriginallyProducts.Count > 0)
			{
				dataResult.RecommendOriginallyProducts = recommendOriginallyProducts.ToArray();
			}

			var cartPayment = cart.Payment;
			if (cartPayment != null)
			{
				dataResult.ReceiveOrderInfoObject.CardInstallmentsCode = cartPayment.CreditInstallmentsCode;
				dataResult.ReceiveOrderInfoObject.OrderPriceExchangeTax = cartPayment.PriceExchange.ToString("#");

				if (cartPayment.UserCreditCard != null)
				{
					creditCardInfo.CardDispName = cartPayment.UserCreditCard.CardDispName;
					creditCardInfo.LastFourDigit = cartPayment.UserCreditCard.LastFourDigit;
					creditCardInfo.ExpirationMonth = cartPayment.UserCreditCard.ExpirationMonth;
					creditCardInfo.AuthorName = cartPayment.UserCreditCard.AuthorName;

					dataResult.CreditCardInfoObject = creditCardInfo;
				}
				else if (cartPayment.UserCreditCardRegistFlg && cartPayment.UserCreditNameComplementFlg)
				{
					creditCardInfo.CreditNameComplementFlg =
						Constants.BOTCHAN_API_CREDIT_NAME_COMPLEMENT_FLAG_VALID;

					dataResult.CreditCardInfoObject = creditCardInfo;
				}
			}

			var cartSetPromotion = cart.SetPromotions;
			if (cartSetPromotion != null)
			{
				dataResult.ReceiveOrderInfoObject.SetpromotionProductDiscountAmount
					= cartSetPromotion.ProductDiscountAmount.ToString();
				dataResult.ReceiveOrderInfoObject.SetpromotionShippingChargeDiscountAmount
					= cartSetPromotion.ShippingChargeDiscountAmount.ToString();
				dataResult.ReceiveOrderInfoObject.SetpromotionPaymentChargeDiscountAmount
					= cartSetPromotion.PaymentChargeDiscountAmount.ToString();
			}

			var fixedPurchase = cart.FixedPurchase;
			if (fixedPurchase != null)
			{
				dataResult.ReceiveOrderInfoObject.FixedPurchaseOrderCount
					= fixedPurchase.OrderCount.ToString();
				dataResult.ReceiveOrderInfoObject.FixedPurchaseShippedCount
					= fixedPurchase.ShippedCount.ToString();
			}

			var result = new RecalculationResponse
			{
				Result = true,
				Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
				MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
				DataResult = dataResult
			};

			return result;
		}

		/// <summary>
		/// 定期購入次回配送日の取得
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="shopShipping">配送種別情報</param>
		/// <remarks>未来かどうかで、次々回→次回にスライド or 再計算 を切り替え</remarks>
		/// <returns>日付</returns>
		private DateTime GetNextShippingDate(FixedPurchaseModel fixedPurchase, ShopShippingModel shopShipping)
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
					DateTime.Now,
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
		/// <remarks>未来かどうかで、次々回→次回にスライド or 再計算 を切り替え</remarks>
		/// <returns>日付</returns>
		private DateTime GetNextNextShippingDate(FixedPurchaseModel fixedPurchase, ShopShippingModel shopShipping)
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
					DateTime.Now,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);
			}
			return dateTime;
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
	}
}
