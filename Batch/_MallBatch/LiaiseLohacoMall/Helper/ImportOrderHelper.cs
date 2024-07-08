/*
=========================================================================================================
  Module      : 注文取込のヘルパークラス(ImportOrderHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.LohacoCreatorWebApi.OrderInfo;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.LohacoCreatorWebApi;
using w2.App.Common.Mall.Mapping;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;
using w2.Domain.User;

namespace w2.Commerce.MallBatch.LiaiseLohacoMall.Helper
{
	/// <summary>
	/// 注文取込のヘルパークラス
	/// </summary>
	public class ImportOrderHelper
	{
		#region +CreateOrderData 注文情報作成
		/// <summary>
		/// 注文情報作成
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="info">注文情報</param>
		/// <param name="warningList">警告一覧</param>
		/// <returns>注文情報</returns>
		public static OrderModel CreateOrderData(string mallId, string userId, OrderInfoResponse.OrderInfoResult info, ref List<string> warningList)
		{
			var orderId = StringUtility.ToEmpty(info.OrderInfo.OrderId);
			var item = CreateOrderItems(orderId, info.OrderInfo.Item);
			var owner = CreateOrderOwner(
				orderId,
				GetUserKbn(
					info.OrderInfo.DeviceType,
					(info.OrderInfo.Buyer != null) && (info.OrderInfo.Buyer.IsLogin.HasValue) && (info.OrderInfo.Buyer.IsLogin.Value)),
					info.OrderInfo.Pay,
					ref warningList);
			var shipping = CreateOrderShipping(mallId, orderId, info.OrderInfo.Ship, info.OrderInfo.Pay, ref warningList);
			shipping.Items = item;
			var itemSubtotalPriceTax = item.Sum(x => x.ItemPriceTax);
			// リアルタイム累計購入回数取得
			var user = new UserService().Get(userId);
			var orderCount = ((user == null) ? 0 : user.OrderCountOrderRealtime);
			var order = new OrderModel
			{
				OrderId = orderId,
				UserId = userId,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				OrderKbn = (info.OrderInfo.DeviceType == LohacoConstants.DeviceType.Pc) ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
				MallId = mallId,
				OrderPaymentKbn = LohacoMappingManager.GetInstance().ConvertOrderPaymentKbn(mallId, StringUtility.ToEmpty(info.OrderInfo.Pay.PayMethod)),
				// 警告があれば、「仮注文」ステータスとして注文登録。正常の場合、「注文済み」ステータスとして注文登録
				OrderStatus = (warningList.Count > 0) ? Constants.FLG_ORDER_ORDER_STATUS_TEMP : Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
				OrderDate = info.OrderInfo.OrderTime ?? DateTime.Now,
				OrderItemCount = item.Length,
				OrderProductCount = item.Sum(x => x.ItemQuantity),
				OrderPriceSubtotal = item.Sum(x => x.ItemPrice),
				OrderPriceSubtotalTax = itemSubtotalPriceTax,
				OrderPriceShipping = info.OrderInfo.Detail.ShipCharge ?? 0,
				OrderPriceExchange = info.OrderInfo.Detail.PayCharge ?? 0,
				OrderPriceTotal = info.OrderInfo.Detail.TotalPrice?? 0,
				OrderPriceRegulation = CalculateRegulationPrice(info),
				// カード種別(CardBrand)、カード番号下4桁（CardNumberLast4）フィールドが2018年3月28日から廃止された
				CardKbn = string.Empty,
				CardTranId = string.Empty,
				CardInstruments = string.Empty,
				CardInstallmentsCode = string.Empty,
				ShippingId = Constants.LOHACO_DEFAULT_SHIPPING_ID,
				FixedPurchaseId = string.Empty,
				AdvcodeFirst = string.Empty,
				AdvcodeNew = string.Empty,
				CareerId = string.Empty,
				MobileUid = string.Empty,
				RemoteAddr = string.Empty,
				Memo = string.Empty,
				WrappingMemo = string.Empty,
				PaymentMemo = string.Empty,
				ManagementMemo = CreateManagementMemo(info),
				RelationMemo = CreateRelationMemo(info, warningList),
				ReturnExchangeReasonMemo = string.Empty,
				RegulationMemo = CreateRegulationMemo(info),
				RepaymentMemo = string.Empty,
				DateCreated = info.OrderInfo.OrderTime ?? DateTime.Now,
				DateChanged = info.OrderInfo.LastUpdateTime ?? DateTime.Now,
				LastChanged = StringUtility.ToEmpty(info.OrderInfo.OperationUser),
				MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank(),
				GiftFlg = Constants.FLG_ORDER_GIFT_FLG_OFF,
				PaymentOrderId = string.Empty,
				CombinedOrgOrderIds = string.Empty,
				LastBilledAmount = info.OrderInfo.Detail.TotalPrice?? 0,
				ExternalPaymentErrorMessage = string.Empty,
				ExternalOrderId = string.Empty,
				ExternalImportStatus = Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_SUCCESS,
				LastAuthFlg = null,
				MallLinkStatus = string.Empty,
				ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE,
				PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE,
				Items = item,
				Owner = owner,
				Shippings = new[] { shipping },
				OrderCountOrder = orderCount + 1,
			};

			var orderPriceByTaxRate = OrderCommon.CreateOrderPriceByTaxRate(order);
			order.OrderPriceByTaxRates = orderPriceByTaxRate;
			order.OrderPriceTax = orderPriceByTaxRate.Sum(priceByTaxRate => priceByTaxRate.TaxPriceByRate);

			return order;
		}
		#endregion

		#region +CreateUserData ユーザ情報の作成
		/// <summary>
		/// 注文者のユーザ情報の作成
		/// </summary>
		/// <param name="mallId">ロハコモールID</param>
		/// <param name="pay">請求情報</param>
		/// <param name="deviceType">デバイス種類</param>
		/// <param name="buyer">バイヤー情報</param>
		/// <param name="warningList">警告一覧</param>
		/// <returns>注文者のユーザ情報</returns>
		public static UserModel CreateNewUserData(
			string mallId,
			OrderInfoResponse.Pay pay,
			LohacoConstants.DeviceType? deviceType,
			OrderInfoResponse.Buyer buyer,
			ref List<string> warningList)
		{
			var userId = GetNewUserId();
			// 最大文字列数を超える可能項目の調整
			var address = StringUtility.SplitAddress(pay.BillCity, pay.BillAddress1, pay.BillAddress2);
			if (address == null)
			{
				warningList.Add(
					string.Format("ユーザの住所情報（{0}）が最大文字列数（{1}）を超えました。",
					string.Concat(pay.BillCity, pay.BillAddress1, pay.BillAddress2),
					Constants.ADDRESS_MAX_LENGTH));
			}
			var name1 = StringUtility.ToZenkaku(pay.BillLastName).Trim();
			var name2 = StringUtility.ToZenkaku(pay.BillFirstName).Trim();
			var nameKana1 = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana1.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? StringUtility.ToEmpty(pay.BillLastNameKana).Trim()
					: StringUtility.ToZenkakuHiragana(pay.BillLastNameKana).Trim();
			var nameKana2 = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana2.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? StringUtility.ToEmpty(pay.BillFirstNameKana).Trim()
					: StringUtility.ToZenkakuHiragana(pay.BillFirstNameKana).Trim();
			AdjustFields("ユーザ", ref name1, ref name2, ref nameKana1, ref nameKana2, ref warningList);

			// 郵便番号の分割
			var zipCode = StringUtility.SplitJapanZipCode(pay.BillZipCode);
			if (zipCode == null)
			{
				warningList.Add(string.Format("ユーザの郵便番号（{0}）は日本郵便番号（7桁・8桁）ではありません。",
					StringUtility.ToEmpty(pay.BillZipCode)));
			}

			var phoneNumber = StringUtility.ToPhoneNumber(pay.BillPhoneNumber);
			var phoneNumberList = phoneNumber.Split('-');

			var user = new UserModel()
			{
				UserId = userId,
				UserKbn = GetUserKbn(deviceType, (buyer != null) && buyer.IsLogin.HasValue && buyer.IsLogin.Value),
				MallId = mallId,
				Name = string.Concat(name1, name2),
				Name1 = name1,
				Name2 = name2,
				NameKana = string.Concat(nameKana1, nameKana2),
				NameKana1 = nameKana1,
				NameKana2 = nameKana2,
				MailAddr = StringUtility.ToEmpty(pay.BillMailAddress),
				MailAddr2 = string.Empty,
				Zip = (zipCode == null) ? string.Empty : string.Format("{0}-{1}", zipCode[0], zipCode[1]),
				Zip1 = (zipCode == null) ? string.Empty : zipCode[0],
				Zip2 = (zipCode == null) ? string.Empty : zipCode[1],
				Addr = (address == null) ? StringUtility.ToEmpty(pay.BillPrefecture) : string.Concat(pay.BillPrefecture, pay.BillCity, pay.BillAddress1, pay.BillAddress2),
				Addr1 = StringUtility.ToEmpty(pay.BillPrefecture),
				Addr2 = (address == null) ? string.Empty : address[0],
				Addr3 = (address == null) ? string.Empty : address[1],
				Addr4 = (address == null) ? string.Empty : address[2],
				Tel1 = StringUtility.ToPhoneNumber(pay.BillPhoneNumber),
				Tel1_1 = phoneNumberList[0],
				Tel1_2 = phoneNumberList[1],
				Tel1_3 = phoneNumberList[2],
				Sex = Constants.FLG_USER_SEX_UNKNOWN,
				Birth = null,
				BirthYear = string.Empty,
				BirthMonth = string.Empty,
				BirthDay = string.Empty,
				LoginId = string.Empty,
				Password = string.Empty,
				MailFlg = Constants.FLG_USER_MAILFLG_UNKNOWN,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH,
				MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank(),
				RecommendUid = string.Empty,
				RemoteAddr = string.Empty,
				CompanyName = string.Empty,
				CompanyPostName = string.Empty,
				NickName = string.Empty,
			};
			return user;
		}
		#endregion

		#region +GetUserKbn ユーザ区分の取得
		/// <summary>
		/// ユーザ区分の取得
		/// </summary>
		/// <param name="deviceType">デバイス種類</param>
		/// <param name="isLogin">ログインしているかどうか</param>
		/// <returns>ユーザ区分</returns>
		private static string GetUserKbn(LohacoConstants.DeviceType? deviceType, bool isLogin)
		{
			if (deviceType.GetValueOrDefault() == LohacoConstants.DeviceType.Pc)
			{
				return (isLogin) ? Constants.FLG_USER_USER_KBN_PC_USER : Constants.FLG_USER_USER_KBN_PC_GUEST;
			}
			else
			{
				return (isLogin) ? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER : Constants.FLG_USER_USER_KBN_SMARTPHONE_GUEST;
			}
		}
		#endregion

		#region -CreateManagementMemo 管理メモの作成
		/// <summary>
		/// 管理メモの作成
		/// </summary>
		/// <param name="info">注文情報</param>
		/// <returns>管理メモ</returns>
		private static string CreateManagementMemo(OrderInfoResponse.OrderInfoResult info)
		{
			var managementMemo = new StringBuilder();

			// 社内メモ
			managementMemo.AppendLine(string.Format("[社内メモ]{0}", info.OrderInfo.Notes));
			// 配送メモ
			managementMemo.AppendLine(string.Format("[配送メモ]{0}", info.OrderInfo.Ship.ShipNotes));

			return managementMemo.ToString();
		}
		#endregion

		#region -CreateRelationMemo 連携メモの作成
		/// <summary>
		/// 連携メモの作成
		/// </summary>
		/// <param name="info">注文情報</param>
		/// <param name="warningList">警告一覧</param>
		/// <returns>連携メモ</returns>
		private static string CreateRelationMemo(OrderInfoResponse.OrderInfoResult info, List<string> warningList)
		{
			var relationMemo = new StringBuilder();

			// 注文伝票出力状態
			if (info.OrderInfo.PrintSlipTime != null)
			{
				relationMemo.AppendLine(string.Format("[注文伝票出力状態]{0}に出力済み", info.OrderInfo.PrintSlipTime));
			}
			// 納品書出力有無
			if (info.OrderInfo.Pay.NeedDetailedSlip.HasValue && info.OrderInfo.Pay.NeedDetailedSlip.Value)
			{
				relationMemo.AppendLine("[納品書出力有無]納品書必要");
			}
			// 宅配BOX指定
			if (info.OrderInfo.Ship.DeliveryBoxType != LohacoConstants.DeliveryBoxType.NotSpecifyMailBox)
			{
				relationMemo.AppendLine(string.Format(
					"[宅配BOX指定]{0}",
					(info.OrderInfo.Ship.DeliveryBoxType == LohacoConstants.DeliveryBoxType.WantMailBox) ? "希望" : "不可"));
			}
			// ギフト包装有無
			if (info.OrderInfo.Ship.NeedGiftWrap.HasValue && info.OrderInfo.Ship.NeedGiftWrap.Value)
			{
				relationMemo.AppendLine("[ギフト包装有無]ギフト包装有り");
				relationMemo.AppendLine(string.Format("ギフト包装種類：{0}、ギフトメッセージ：{1}", info.OrderInfo.Ship.GiftWrapType, info.OrderInfo.Ship.GiftWrapMessage));
			}
			// [のし有無
			if (info.OrderInfo.Ship.NeedGiftWrap.HasValue && info.OrderInfo.Ship.NeedGiftWrap.Value)
			{
				relationMemo.AppendLine("[のし有無]のし有り");
				relationMemo.AppendLine(string.Format("のし種類：{0}、名入れ：{1}", info.OrderInfo.Ship.GiftWrapPaperType, info.OrderInfo.Ship.GiftWrapName));
			}
			// 配送方法
			relationMemo.AppendLine(string.Format(
				"[配送方法]{0}({1})",
				info.OrderInfo.Ship.ShipMethodName, info.OrderInfo.Ship.ShipMethod));
			// 配送日時指定
			relationMemo.AppendLine(string.Format(
				"[配送日時指定]{0} {1}",
				StringUtility.ToDateString(info.OrderInfo.Ship.ShipRequestDate, Constants.JAPAN_DATE_FORMAT_SHORT),
				info.OrderInfo.Ship.ShipRequestTime));
			// ポイント利用
			relationMemo.AppendLine(string.Format("[ポイント利用有無]{0}pt", info.OrderInfo.Detail.UsePoint));

			// PayPay利用
			relationMemo.AppendLine(string.Format("[PayPay利用有無]{0}円", StringUtility.ToNumeric(info.OrderInfo.Detail.UsePaypayAmount)));

			// クーポン利用
			if (info.OrderInfo.CouponType != LohacoConstants.CouponType.NoCoupon)
			{
				relationMemo.AppendLine("[クーポン利用有無]あり");
				relationMemo.AppendLine(string.Format(
					"クーポン種類：{0}、クーポンコード：{1}、クーポン名：{2}、クーポン値引き額：{3}円",
					(info.OrderInfo.CouponType == LohacoConstants.CouponType.MallCoupon) ? "モールクーポン" : "スタアクーポン",
					(info.OrderInfo.CouponType == LohacoConstants.CouponType.MallCoupon) ? info.OrderInfo.CouponCampaignCode : info.OrderInfo.StoreCouponCode,
					(info.OrderInfo.CouponType == LohacoConstants.CouponType.MallCoupon) ? string.Empty : info.OrderInfo.StoreCouponName,
					(info.OrderInfo.CouponType == LohacoConstants.CouponType.MallCoupon) ? info.OrderInfo.Detail.TotalMallCouponDiscount : info.OrderInfo.Detail.StoreCouponDiscount));
			}
			// カード支払い情報
			if (StringUtility.ToEmpty(info.OrderInfo.Pay.PayMethod).Equals(Constants.PAY_METHOD_CREDIT_CARD))
			{
				switch (info.OrderInfo.Pay.CardPayType.Value)
				{
					case LohacoConstants.CardPayType.BonusPayment:
						relationMemo.AppendLine("[カード支払い情報]支払区分：ボーナス一括払い");
						break;

					case LohacoConstants.CardPayType.DividedPayment:
						relationMemo.AppendLine(string.Format("[カード支払い情報]支払区分：分割払い（{0}回）", info.OrderInfo.Pay.CardPayCount));
						break;

					case LohacoConstants.CardPayType.RevolvingPayment:
						relationMemo.AppendLine("[カード支払い情報]支払区分：リボ払い");
						break;

					case LohacoConstants.CardPayType.FullPayment:
					default:
						relationMemo.AppendLine("[カード支払い情報]支払区分：一括払い");
						break;
				}
			}
			// バイヤー会員ランク情報
			if ((info.OrderInfo.Buyer != null) && (string.IsNullOrWhiteSpace(info.OrderInfo.Buyer.FspLicenseCode) == false))
			{
				relationMemo.AppendLine(string.Format("[Fspライセンス情報]{0}（{1}）", info.OrderInfo.Buyer.FspLicenseCode, info.OrderInfo.Buyer.FspLicenseName));
			}
			// 警告項目
			if (warningList.Count > 0)
			{
				relationMemo.AppendLine("[調整必要情報]");
				foreach (var warning in warningList)
				{
					relationMemo.AppendLine(string.Concat("・", warning));
				}
			}

			return relationMemo.ToString();
		}
		#endregion

		#region -CreateRegulationMemo 調整金額メモの作成
		/// <summary>
		/// 調整金額メモの作成
		/// </summary>
		/// <param name="info">注文情報</param>
		/// <returns>調整金額メモ</returns>
		private static string CreateRegulationMemo(OrderInfoResponse.OrderInfoResult info)
		{
			var regulationMemo = new StringBuilder();

			// ギフト包装料
			if (info.OrderInfo.Ship.NeedGiftWrap.HasValue && info.OrderInfo.Ship.NeedGiftWrap.Value)
			{
				regulationMemo.AppendLine(string.Format("[ギフト包装料]{0}円", StringUtility.ToNumeric(info.OrderInfo.Detail.GiftWrapCharge)));
			}
			// 値引き
			if (info.OrderInfo.Detail.Discount > 0)
			{
				regulationMemo.AppendLine(string.Format("[値引き]{0}円", StringUtility.ToNumeric(info.OrderInfo.Detail.Discount)));
			}
			// クーポン割引き額は商品代に按分されたので、調整金額に記載しない
			// 調整額
			if (info.OrderInfo.Detail.Adjustments.HasValue && (info.OrderInfo.Detail.Adjustments != 0))
			{
				regulationMemo.AppendLine(string.Format("[調整額]{0}円", StringUtility.ToNumeric(info.OrderInfo.Detail.Adjustments)));
			}
			// 利用ポイント数
			if (info.OrderInfo.Detail.UsePoint > 0)
			{
				regulationMemo.AppendLine(string.Format("[利用ポイント数]{0}pt", info.OrderInfo.Detail.UsePoint));
			}
			// 利用ポイント数
			if (info.OrderInfo.Detail.UsePaypayAmount > 0)
			{
				regulationMemo.AppendLine(string.Format("[PayPay利用額]{0}円", StringUtility.ToNumeric(info.OrderInfo.Detail.UsePaypayAmount)));
				
			}

			return regulationMemo.ToString();
		}
		#endregion

		#region -CalculateRegulationPrice 調整金額の計算
		/// <summary>
		/// 調整金額の計算
		/// </summary>
		/// <param name="info">注文情報</param>
		/// <returns>調整金額</returns>
		private static decimal CalculateRegulationPrice(OrderInfoResponse.OrderInfoResult info)
		{
			return (info.OrderInfo.Detail.GiftWrapCharge?? 0)
				- (info.OrderInfo.Detail.Discount?? 0)
				- (info.OrderInfo.Detail.TotalMallCouponDiscount?? 0)
				+ (info.OrderInfo.Detail.Adjustments?? 0)
				- (info.OrderInfo.Detail.UsePoint?? 0)
				- (info.OrderInfo.Detail.UsePaypayAmount?? 0);
		}
		#endregion

		#region -CreateOrderOwner 注文者情報作成
		/// <summary>
		/// 注文者情報作成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="userKbn">注文者区分</param>
		/// <param name="pay">請求情報</param>
		/// <param name="warningList">警告一覧</param>
		/// <returns>注文者情報</returns>
		private static OrderOwnerModel CreateOrderOwner(string orderId, string userKbn, OrderInfoResponse.Pay pay, ref List<string> warningList)
		{
			// 最大文字列数を超える可能項目の調整
			var address = StringUtility.SplitAddress(pay.BillCity, pay.BillAddress1, pay.BillAddress2);
			if (address == null)
			{
				warningList.Add(string.Format("注文者の住所情報（{0}）が最大文字列数（{1}）を超えました。",
					string.Concat(pay.BillCity, pay.BillAddress1, pay.BillAddress2),
					Constants.ADDRESS_MAX_LENGTH));
			}
			var name1 = StringUtility.ToZenkaku(pay.BillLastName).Trim();
			var name2 = StringUtility.ToZenkaku(pay.BillFirstName).Trim();
			var nameKana1 = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana1.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? StringUtility.ToEmpty(pay.BillLastNameKana).Trim()
					: StringUtility.ToZenkakuHiragana(pay.BillLastNameKana).Trim();
			var nameKana2 = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana2.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? StringUtility.ToEmpty(pay.BillFirstNameKana).Trim()
					: StringUtility.ToZenkakuHiragana(pay.BillFirstNameKana).Trim();
			AdjustFields("注文者", ref name1, ref name2, ref nameKana1, ref nameKana2, ref warningList);

			var owner = new OrderOwnerModel
			{
				OrderId = orderId,
				OwnerKbn = userKbn,
				OwnerName = string.Concat(name1, name2),
				OwnerNameKana = string.Concat(nameKana1, nameKana2),
				OwnerMailAddr = StringUtility.ToEmpty(pay.BillMailAddress),
				OwnerZip = StringUtility.ToEmpty(pay.BillZipCode),
				OwnerAddr1 = StringUtility.ToEmpty(pay.BillPrefecture),
				OwnerAddr2 = (address == null) ? string.Empty : address[0],
				OwnerAddr3 = (address == null) ? string.Empty : address[1],
				OwnerAddr4 = (address == null) ? string.Empty : address[2],
				OwnerTel1 = StringUtility.ToPhoneNumber(pay.BillPhoneNumber),
				OwnerTel2 = string.Empty,
				OwnerTel3 = string.Empty,
				OwnerFax = string.Empty,
				OwnerCompanyName = string.Empty,
				OwnerCompanyPostName = string.Empty,
				OwnerCompanyExectiveName = string.Empty,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				OwnerName1 = name1,
				OwnerName2 = name2,
				OwnerNameKana1 = nameKana1,
				OwnerNameKana2 = nameKana2,
				OwnerMailAddr2 = string.Empty,
			};
			return owner;
		}
		#endregion

		#region -CreateOrderShipping 配送先情報作成
		/// <summary>
		/// 配送先情報作成
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="ship">配送情報</param>
		/// <param name="pay">請求情報</param>
		/// <param name="warningList">警告一覧</param>
		/// <returns>配送先情報</returns>
		private static OrderShippingModel CreateOrderShipping(string mallId, string orderId, OrderInfoResponse.Ship ship, OrderInfoResponse.Pay pay, ref List<string> warningList)
		{
			// 最大文字列数を超える可能項目の調整
			var address = StringUtility.SplitAddress(ship.ShipCity, ship.ShipAddress1, ship.ShipAddress2);
			if (address == null)
			{
				warningList.Add(string.Format("配送先の住所情報（{0}）が最大文字列数（{1}）を超えました。",
					string.Concat(ship.ShipCity, ship.ShipAddress1, ship.ShipAddress2),
					Constants.ADDRESS_MAX_LENGTH));
			}
			var name1 = StringUtility.ToZenkaku(ship.ShipLastName).Trim();
			var name2 = StringUtility.ToZenkaku(ship.ShipFirstName).Trim();
			var nameKana1 = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? StringUtility.ToEmpty(ship.ShipLastNameKana).Trim()
					: StringUtility.ToZenkakuHiragana(ship.ShipLastNameKana).Trim();
			var nameKana2 = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? StringUtility.ToEmpty(ship.ShipFirstNameKana).Trim()
					: StringUtility.ToZenkakuHiragana(ship.ShipFirstNameKana).Trim();
			AdjustFields("配送先", ref name1, ref name2, ref nameKana1, ref nameKana2, ref warningList);

			// 別送フラグのチェック
			var ownerAddress = StringUtility.SplitAddress(pay.BillCity, pay.BillAddress1, pay.BillAddress2);
			var isAnotherShippingFlg = StringUtility.IsAnotherShippingFlagValid(
				StringUtility.ToEmpty(pay.BillLastName),
				StringUtility.ToEmpty(pay.BillFirstName),
				StringUtility.ToEmpty(pay.BillZipCode),
				StringUtility.ToEmpty(pay.BillPrefecture),
				ownerAddress[0],
				ownerAddress[1],
				ownerAddress[2],
				StringUtility.ToPhoneNumber(pay.BillPhoneNumber),
				StringUtility.ToEmpty(ship.ShipLastName),
				StringUtility.ToEmpty(ship.ShipFirstName),
				StringUtility.ToEmpty(ship.ShipZipCode),
				StringUtility.ToEmpty(ship.ShipPrefecture),
				address[0],
				address[1],
				address[2],
				StringUtility.ToPhoneNumber(ship.ShipPhoneNumber));

			// 配送先情報作成
			var shipping = new OrderShippingModel
			{
				OrderId = orderId,
				// 複数配送先・ギフト対応しないので、「1」に固定
				OrderShippingNo = 1,
				ShippingName = string.Concat(name1, name2),
				ShippingNameKana = string.Concat(nameKana1, nameKana2),
				ShippingZip = StringUtility.ToEmpty(ship.ShipZipCode),
				ShippingAddr1 = StringUtility.ToEmpty(ship.ShipPrefecture),
				ShippingAddr2 = address[0],
				ShippingAddr3 = address[1],
				ShippingAddr4 = address[2],
				ShippingTel1 = StringUtility.ToPhoneNumber(ship.ShipPhoneNumber),
				ShippingTel2 = string.Empty,
				ShippingTel3 = string.Empty,
				ShippingFax = string.Empty,
				ShippingDate = ship.ShipRequestDate,
				ShippingTime = LohacoMappingManager.GetInstance().ConvertShippingTime(mallId, ship.ShipRequestTime),
				ShippingCheckNo = string.Empty,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				ShippingName1 = name1,
				ShippingName2 = name2,
				ShippingNameKana1 = nameKana1,
				ShippingNameKana2 = nameKana2,
				SenderName = string.Empty,
				SenderName1 = string.Empty,
				SenderName2 = string.Empty,
				SenderNameKana = string.Empty,
				SenderNameKana1 = string.Empty,
				SenderNameKana2 = string.Empty,
				SenderZip = string.Empty,
				SenderAddr1 = string.Empty,
				SenderAddr2 = string.Empty,
				SenderAddr3 = string.Empty,
				SenderAddr4 = string.Empty,
				SenderTel1 = string.Empty,
				WrappingPaperType = StringUtility.ToEmpty(ship.GiftWrapPaperType),
				WrappingPaperName = StringUtility.ToEmpty(ship.GiftWrapName),
				WrappingBagType = StringUtility.ToEmpty(ship.GiftWrapType),
				ShippingCompanyName = string.Empty,
				ShippingCompanyPostName = string.Empty,
				SenderCompanyName = string.Empty,
				SenderCompanyPostName = string.Empty,
				AnotherShippingFlg = (isAnotherShippingFlg) ? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID : Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID,
				ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS,
				DeliveryCompanyId = Constants.LOHACO_DEFAULT_SHIPPING_ID,
			};
			return shipping;
		}
		#endregion

		#region -CreateOrderItems 注文商品情報リスト作成
		/// <summary>
		/// 注文商品情報リスト作成
		/// </summary>
		/// <returns>注文商品情報リスト</returns>
		private static OrderItemModel[] CreateOrderItems(string orderId, List<OrderInfoResponse.Item> items)
		{
			if (items.Count == 0) throw new ImportOrderException(string.Format("注文ID'{0}'に商品が0件なので、注文生成に失敗しました。", orderId));

			var orderItemList = new List<OrderItemModel>();

			foreach (var item in items)
			{
				var product = GetProductByLohacoItemCode(StringUtility.ToEmpty(item.ItemCd));
				if (product == null) throw new ImportOrderException(string.Format("注文ID'{0}'の商品'{1}'が見つかりません。", orderId, StringUtility.ToEmpty(item.ItemCd)));

				var productPriceTax = TaxCalculationUtility.GetTaxPrice(
					item.UnitPrice,
					item.TaxRatio.Value,
					Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
					true);
				var itemPriceTax = TaxCalculationUtility.RoundTaxFraction(productPriceTax * item.Quantity);
				var productPrice = TaxCalculationUtility.GetPrescribedPrice(item.UnitPrice, productPriceTax, true);
				var productPriceOrgTax = TaxCalculationUtility.GetTaxPrice(
					item.OriginUnitPrice,
					item.TaxRatio.Value,
					Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
					true);
				var productPriceOrg = TaxCalculationUtility.GetPrescribedPrice(item.OriginUnitPrice, productPriceOrgTax, true);

				var orderItem = new OrderItemModel
				{
					OrderId = orderId,
					OrderItemNo = item.LineId,
					OrderShippingNo = 1,
					ShopId = Constants.CONST_DEFAULT_SHOP_ID,
					ProductId = product.ProductId,
					VariationId = product.VariationId,
					SupplierId = product.SupplierId,
					ProductName = ProductCommon.CreateProductJointName(product),
					ProductNameKana = product.NameKana,
					ProductPrice = productPrice,
					ProductPriceOrg = productPriceOrg,
					ProductTaxRate = item.TaxRatio.Value,
					ProductPricePretax = item.UnitPrice,
					ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag(),
					ItemQuantity = item.Quantity,
					ItemQuantitySingle = item.Quantity,
					ItemPrice = productPrice * item.Quantity,
					ItemPriceSingle = productPrice * item.Quantity,
					ItemPriceTax = itemPriceTax,
					ProductSetId = string.Empty,
					ItemMemo = StringUtility.ToEmpty(item.Title),
					ProductOptionTexts = string.Empty,
					BrandId = product.BrandId1,
					DownloadUrl = string.Empty,
					ProductsaleId = string.Empty,
					CooperationId1 = product.CooperationId1,
					CooperationId2 = product.CooperationId2,
					CooperationId3 = product.CooperationId3,
					CooperationId4 = product.CooperationId4,
					CooperationId5 = product.CooperationId5,
					CooperationId6 = product.CooperationId6,
					CooperationId7 = product.CooperationId7,
					CooperationId8 = product.CooperationId8,
					CooperationId9 = product.CooperationId9,
					CooperationId10 = product.CooperationId10,
					NoveltyId = string.Empty,
					RecommendId = string.Empty,
					ProductBundleId = string.Empty,
					ColumnForMallOrder = string.Empty,
					GiftWrappingId = string.Empty,
					GiftMessage = string.Empty,
					DateCreated = DateTime.Now,
					DateChanged = DateTime.Now,
					ShippingSizeKbn = product.ShippingSizeKbn,
					StockManagementKbn = product.StockManagementKbn,
				};
				orderItemList.Add(orderItem);
			}

			return orderItemList.ToArray();
		}
		#endregion

		#region -GetProductByLohacoItemCode Lohaco側の商品コードから商品情報取得
		/// <summary>
		/// Lohaco側の商品コードから商品情報取得
		/// </summary>
		/// <param name="itemCode">ロハコ側の商品コード</param>
		/// <returns>商品情報</returns>
		private static ProductModel GetProductByLohacoItemCode(string itemCode)
		{
			var product = new ProductService().GetProductByLohacoItemCode(Constants.CONST_DEFAULT_SHOP_ID, itemCode);
			return product;
		}
		#endregion

		#region -GetNewUserId 新ユーザーID発行
		/// <summary>
		/// 新ユーザーID発行
		/// </summary>
		/// <returns>新ユーザーID</returns>
		private static string GetNewUserId()
		{
			var userId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			return userId;
		}
		#endregion

		#region -AdjustFields
		/// <summary>
		/// 文字数オーバー可能項目の調整
		/// </summary>
		/// <param name="type">情報種類（ユーザ情報・注文者情報・配送先情報）</param>
		/// <param name="name1">氏名１</param>
		/// <param name="name2">氏名２</param>
		/// <param name="nameKana1">氏名かな１</param>
		/// <param name="nameKana2">氏名かな２</param>
		/// <param name="warningList">警告一覧</param>
		private static void AdjustFields(string type, ref string name1, ref string name2, ref string nameKana1, ref string nameKana2, ref List<string> warningList)
		{
			// 氏名１のチェック
			if (name1.Length > Constants.NAME1_MAX_LENGTH)
			{
				warningList.Add(string.Format("{0}の氏名１情報（{1}）が最大文字列数（{2}）を超えました。", type, name1, Constants.NAME1_MAX_LENGTH));
				name1 = string.Empty;
			}
			// 氏名２のチェック
			if (name2.Length > Constants.NAME2_MAX_LENGTH)
			{
				warningList.Add(string.Format("{0}の氏名２情報（{1}）が最大文字列数（{2}）を超えました。", type, name2, Constants.NAME2_MAX_LENGTH));
				name2 = string.Empty;
			}
			// 氏名かな１のチェック
			if (nameKana1.Length > Constants.NAME_KANA1_MAX_LENGTH)
			{
				warningList.Add(string.Format("{0}の氏名かな１情報（{1}）が最大文字列数（{2}）を超えました。", type, nameKana1, Constants.NAME_KANA1_MAX_LENGTH));
				nameKana1 = string.Empty;
			}
			// 氏名かな２のチェック
			if (nameKana2.Length > Constants.NAME_KANA2_MAX_LENGTH)
			{
				warningList.Add(string.Format("{0}の氏名かな２情報（{1}）が最大文字列数（{2}）を超えました。", type, nameKana2, Constants.NAME_KANA2_MAX_LENGTH));
				nameKana2 = string.Empty;
			}
		}
		#endregion
	}
}
