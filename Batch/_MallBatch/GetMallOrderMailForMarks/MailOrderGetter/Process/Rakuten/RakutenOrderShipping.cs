/*
=========================================================================================================
  Module      : 楽天注文情報：配送先クラス (RakutenOrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Mall.Rakuten;
using w2.App.Common.Order;
using w2.Common.Util;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.User;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Rakuten
{
	/// <summary>
	/// 楽天注文情報：配送先クラス
	/// </summary>
	class RakutenOrderShipping : OrderShippingModel
	{
		/// <summary>デフォルト配送希望時間帯</summary>
		private const string DEFAULT_SHIPPING_TIME = "999";
		/// <summary>配送希望時間帯指定なし</summary>
		private const string SHIPPING_TIME_NONE = "指定無し";
		/// <summary>配送希望日時指定</summary>
		private const string SHIPPING_ORDER_DATE_TIME = "配送日時指定";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="owner">注文者</param>
		/// <param name="shopId">店舗ID</param>
		public RakutenOrderShipping(orderModel rakutenOrder, UserModel owner, string shopId)
		{
			var rakutenPackage = rakutenOrder.packageModel[0];
			var rakutenSender = rakutenPackage.senderModel;

			this.ShippingName1 = StringUtility.ToEmpty(rakutenSender.familyName);
			this.ShippingName2 = StringUtility.ToEmpty(rakutenSender.firstName);
			this.ShippingName = this.ShippingName1 + this.ShippingName2;
			SetNameKana(rakutenSender.familyNameKana, rakutenSender.firstNameKana);
			this.ShippingZip = string.Format("{0}-{1}",
				StringUtility.ToEmpty(rakutenSender.zipCode1),
				StringUtility.ToEmpty(rakutenSender.zipCode2));
			SetAddr(rakutenSender.city, rakutenSender.subAddress, rakutenSender.prefecture);
			this.ShippingTel1 = string.Format(
				"{0}-{1}-{2}",
				StringUtility.ToEmpty(rakutenSender.phoneNumber1),
				StringUtility.ToEmpty(rakutenSender.phoneNumber2),
				StringUtility.ToEmpty(rakutenSender.phoneNumber3));
			this.ExternalShippingCooperationId = StringUtility.ToEmpty(rakutenPackage.basketId);
			this.DeliveryCompanyId = GetDeliveryCompany();
			this.AnotherShippingFlg = GetAnotherShippingFlg(owner);
			this.ShippingTime = GetShippingTimeId(rakutenOrder);
			this.ShippingDate = rakutenOrder.wishDeliveryDateSpecified ? rakutenOrder.wishDeliveryDate : (DateTime?)null;
			this.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			this.ScheduledShippingDate = GetScheduledShippingDate(shopId);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 氏名(かな)セット
		/// </summary>
		/// <param name="familyNameKana">姓カナ</param>
		/// <param name="firstNameKana">名カナ</param>
		private void SetNameKana(string familyNameKana, string firstNameKana)
		{
			// 氏名（かな）、あるいは、氏名（カナ）は設定値により自動切替
			if (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
			{
				this.ShippingNameKana1 = StringUtility.ToZenkakuKatakana(familyNameKana);
				this.ShippingNameKana2 = StringUtility.ToZenkakuKatakana(firstNameKana);
			}
			else
			{
				this.ShippingNameKana1 = StringUtility.ToZenkakuHiragana(familyNameKana);
				this.ShippingNameKana2 = StringUtility.ToZenkakuHiragana(firstNameKana);
			}
			this.ShippingNameKana = this.ShippingNameKana1 + this.ShippingNameKana2;
		}

		/// <summary>
		/// 住所セット
		/// </summary>
		/// <param name="city">群市区</param>
		/// <param name="subAddress">それ以降の住所</param>
		/// <param name="prefecture">都道府県</param>
		private void SetAddr(string city, string subAddress, string prefecture)
		{
			var address = StringUtility.SplitAddress(city, subAddress);
			if (address == null)
			{
				Constants.DISP_ERROR_KBN = Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR;
				Constants.DISP_ERROR_MESSAGE = "配送先の住所情報が規定値を超えました。";
				throw new RakutenApiCoopException("配送先の住所情報が規定値を超えました。");
			}

			// 海外注文の場合、Addr1とAddr2を変換
			if (Constants.RAKUTEN_OVERSEA_ORDER_MAIL_ENABLED && (Properties.Resources.sKen.Contains(prefecture) == false))
			{
				this.ShippingAddr1 = Constants.ORDER_ADDR1_RAKUTEN_OVERSEA;
				this.ShippingAddr2 = StringUtility.ToEmpty(prefecture) + address[0];
			}
			else
			{
				this.ShippingAddr1 = StringUtility.ToEmpty(prefecture);
				this.ShippingAddr2 = address[0];
			}
			this.ShippingAddr3 = address[1];
			this.ShippingAddr4 = address[2];
		}

		/// <summary>
		/// 配送会社取得
		/// </summary>
		/// <returns></returns>
		private string GetDeliveryCompany()
		{
			var defaultCompany = new ShopShippingService().GetDefaultCompany(
					Constants.MALL_DEFAULT_SHIPPING_ID,
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);

			return defaultCompany.DeliveryCompanyId;
		}

		/// <summary>
		/// 別送フラグ取得
		/// </summary>
		/// <param name="owner">注文者情報</param>
		/// <returns>別送フラグ</returns>
		private string GetAnotherShippingFlg(UserModel owner)
		{
			var anotherShippingFlg = StringUtility.IsAnotherShippingFlagValid(
				owner.Name1,
				owner.Name2,
				owner.Zip,
				owner.Addr1,
				owner.Addr2,
				owner.Addr3,
				owner.Addr4,
				owner.Tel1,
				this.ShippingName1,
				this.ShippingName2,
				this.ShippingZip,
				this.ShippingAddr1,
				this.ShippingAddr2,
				this.ShippingAddr3,
				this.ShippingAddr4,
				this.ShippingTel1)
				? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID
				: Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID;

			return anotherShippingFlg;
		}

		/// <summary>
		/// 配送希望時間帯の取得
		/// </summary>
		/// <param name="noteContent">楽天APIで取得した備考欄の内容</param>
		/// <param name="isReserveOrder">予約購入か</param>
		/// <returns>配送希望時間帯</returns>
		private string GetShippingTerm(string noteContent, bool isReserveOrder)
		{
			// 配送日時指定の内容がなければ、ブランクで返す
			if (noteContent.Contains(SHIPPING_ORDER_DATE_TIME) == false) return "";

			// 文字列を改行ベースで配列化する
			var splitedContent = noteContent.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

			// 配送希望時間帯取得
			var shippingTerm = StringUtility.ToEmpty(splitedContent.FirstOrDefault(IsShippingTime));

			if (isReserveOrder == false) return shippingTerm.Trim();

			// 時間帯の箇所に日付があれば、日付を削除
			var matchDate = new Regex(@"([0-9]{4})年([0-9]{1,2})月([0-9]{1,2})日").Match(shippingTerm);
			if (matchDate.Success) shippingTerm = shippingTerm.Replace(matchDate.Value, "");
			return shippingTerm.Trim();
		}

		/// <summary>
		/// 配送希望時間帯か？
		/// </summary>
		/// <param name="content">備考欄内容の行単位の文字列</param>
		/// <returns>true:配送希望時間帯である, false:配送希望時間帯でない</returns>
		private bool IsShippingTime(string content)
		{
			// 配送希望日またはタイトルであればfalse
			if (Regex.Match(content, "[0-9]{4}-[0-9]{2}-[0-9]{2}").Success
				|| content.StartsWith("[")) return false;

			// 空でなければ配送希望時間帯とする
			if (string.IsNullOrEmpty(content) == false) return true;

			// それ以外はfalse
			return false;
		}

		/// <summary>
		/// 配送希望時間帯のID取得
		/// </summary>
		/// <param name="order">楽天注文情報</param>
		/// <returns>配送希望時間帯のID</returns>
		private string GetShippingTimeId(orderModel order)
		{
			var isReserveOrder = (order.orderType == Constants.RAKUTEN_API_RESERVE_ORDER_TYPE);
			// 配送希望時間帯の取得
			var shippingTerm = GetShippingTerm(order.option, isReserveOrder);

			// 空、指定なしの場合は空文字を返す
			if (string.IsNullOrEmpty(shippingTerm)
				|| (shippingTerm == SHIPPING_TIME_NONE)) return "";

			var shippingTimeId =
				new DeliveryCompanyService().SearchShippingTimeId(
					Constants.MALL_DEFAULT_SHIPPING_ID,
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS,
					shippingTerm);

			return (string.IsNullOrEmpty(shippingTimeId)) ? DEFAULT_SHIPPING_TIME : shippingTimeId;
		}

		/// <summary>
		/// 出荷予定日取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>出荷予定日</returns>
		private DateTime? GetScheduledShippingDate(string shopId)
		{
			var scheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
				shopId,
				this.ShippingDate,
				this.ShippingMethod,
				this.DeliveryCompanyId,
				this.ShippingCountryIsoCode,
				this.ShippingAddr1,
				this.ShippingZip);

			return scheduledShippingDate;
		}
		#endregion
	}
}
