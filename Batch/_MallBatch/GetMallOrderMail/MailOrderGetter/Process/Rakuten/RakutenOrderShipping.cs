/*
=========================================================================================================
  Module      : 楽天注文情報：配送先クラス (RakutenOrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Mall.Rakuten;
using w2.App.Common.Mall.RakutenApi;
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

		/// <summary>配送日時指定</summary>
		private const string SHIPPING_MEMO_TITLE = "配送日時指定:";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="owner">注文者</param>
		/// <param name="shopId">店舗ID</param>
		public RakutenOrderShipping(RakutenApiOrder rakutenOrder, UserModel owner, string shopId)
		{
			var rakutenPackage = rakutenOrder.PackageModelList[0];
			var rakutenSender = rakutenPackage.SenderModel;

			this.ShippingName = StringUtility.ToEmpty(rakutenSender.FamilyName) + StringUtility.ToEmpty(rakutenSender.FirstName);
			this.ShippingName1 = StringUtility.ToEmpty(rakutenSender.FamilyName);
			this.ShippingName2 = StringUtility.ToEmpty(rakutenSender.FirstName);
			SetNameKana(rakutenSender.FamilyNameKana, rakutenSender.FirstNameKana);
			this.ShippingZip = string.Format("{0}-{1}",
				StringUtility.ToEmpty(rakutenSender.ZipCode1),
				StringUtility.ToEmpty(rakutenSender.ZipCode2));
			SetAddr(rakutenSender.City, rakutenSender.SubAddress, rakutenSender.Prefecture);
			this.ShippingTel1 = string.Format(
				"{0}-{1}-{2}",
				StringUtility.ToEmpty(rakutenSender.PhoneNumber1),
				StringUtility.ToEmpty(rakutenSender.PhoneNumber2),
				StringUtility.ToEmpty(rakutenSender.PhoneNumber3));
			this.ExternalShippingCooperationId = StringUtility.ToEmpty(rakutenPackage.BasketId);
			this.DeliveryCompanyId = GetDeliveryCompany();
			this.AnotherShippingFlg = GetAnotherShippingFlg(owner);
			this.ShippingDate = rakutenOrder.DeliveryDate;
			this.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			this.ScheduledShippingDate = GetScheduledShippingDate(shopId);
			GetRemarksInfo(rakutenOrder.Remarks);
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
				this.ShippingNameKana = StringUtility.ToZenkakuKatakana(familyNameKana + firstNameKana);
				this.ShippingNameKana1 = StringUtility.ToZenkakuKatakana(familyNameKana);
				this.ShippingNameKana2 = StringUtility.ToZenkakuKatakana(firstNameKana);
			}
			else
			{
				this.ShippingNameKana = StringUtility.ToZenkakuHiragana(familyNameKana + firstNameKana);
				this.ShippingNameKana1 = StringUtility.ToZenkakuHiragana(familyNameKana);
				this.ShippingNameKana2 = StringUtility.ToZenkakuHiragana(firstNameKana);
			}
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
		/// 備考の解析
		/// </summary>
		/// <param name="remarks">備考</param>
		private void GetRemarksInfo(string remarks)
		{
			if (string.IsNullOrEmpty(remarks) || (remarks.Contains("[") == false) || (remarks.Contains("]") == false)) return;

			var remarksTmp = remarks.StartsWith("[") ? remarks.Substring(1) : remarks;
			var remarkParts = remarksTmp.Split(new[] { "\r\n[" }, StringSplitOptions.None);

			// ブロック単位に分割
			var blocks = new Hashtable();
			foreach (var block in remarkParts)
			{
				int iIndex = block.IndexOf("]");
				blocks[block.Substring(0, iIndex)] = block.Substring(iIndex + 1);
			}

			// 配送希望時間帯
			if (blocks.Contains(SHIPPING_MEMO_TITLE))
			{
				GetShippingTimeFromRemarks((string)blocks[SHIPPING_MEMO_TITLE]);
			}
		}

		/// <summary>
		/// 配送希望時間帯取得
		/// </summary>
		/// <param name="remarks">備考</param>
		private void GetShippingTimeFromRemarks(string remarks)
		{
			var shippingOrderTimes = remarks.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
			var timeText = "";

			// メモ内容解析（配送希望日部分）
			foreach (var times in shippingOrderTimes.Select(
				(value, index) => new { value, index }))
			{
				if (Regex.Match(times.value, "[0-9]{4}-[0-9]{2}-[0-9]{2}").Success)
				{
					// 日付部分は別途取得しているため、何もしない
					// this.ShippingDate = DateTime.Parse(times);
				}
				else if ((times.value.Length != 0) && (string.IsNullOrEmpty(timeText)))
				{
					timeText = times.value;
				}
				// 2行目が希望時間帯、以降は要望備考のコメントなので処理中断
				if (times.index > 1) break;
			}

			// 配送希望時間帯ID取得
			this.ShippingTime = GetShippingTime(timeText);
		}

		/// <summary>
		/// 配送希望時間帯取得
		/// </summary>
		/// <param name="shippingTerm">お届け時間帯</param>
		/// <returns>配送希望時間帯</returns>
		private string GetShippingTime(string shippingTerm)
		{
			// 指定がないまたは設定ファイルで指定されている文字列の場合、空文字を返す
			if (string.IsNullOrEmpty(shippingTerm)
				|| Constants.RAKUTEN_API_SHIPPING_TERM_NONE_COMMENT.Contains(shippingTerm)) return "";

		// マッチング文言を取得する
			var shippingTimeMatchings =
				new DeliveryCompanyService().GetShippingTimeMatching(
					Constants.MALL_DEFAULT_SHIPPING_ID,
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);

			// マッチングできたらIDを返す
			var shippingTime = shippingTimeMatchings
				.GetShippingTimeMatchingList()
				.Where(kvp => kvp.Value.Contains(shippingTerm))
				.Select(kvp => kvp.Key)
				.FirstOrDefault();
			return (string.IsNullOrEmpty(shippingTime)) ? DEFAULT_SHIPPING_TIME : shippingTime;
		}

		/// <summary>
		/// 配送希望時間帯文言取得
		/// </summary>
		/// <param name="shippingTerm">お届け希望時間帯</param>
		/// <returns>配送希望時間帯文言</returns>
		private string GetShippingTimeMsg(string shippingTerm)
		{
			// 空、null、指定なし、その他の場合は空文字を返す
			if (string.IsNullOrEmpty(shippingTerm)
				|| (shippingTerm == Constants.RAKUTEN_API_SHIPPING_TERM_NONE)
				|| (shippingTerm == Constants.RAKUTEN_API_SHIPPING_TERM_OTHER)) return "";

			if (shippingTerm == Constants.RAKUTEN_API_SHIPPING_TERM_AM)
			{
				return "午前";
			}
			else if (shippingTerm == Constants.RAKUTEN_API_SHIPPING_TERM_PM)
			{
				return "午後";
			}
			else
			{
				var shippingTimeFrom = shippingTerm.Substring(0, shippingTerm.Length - 2);
				var shippingTimeTo = int.Parse(shippingTerm.Substring(shippingTerm.Length - 2)).ToString();

				return string.Format("{0}:00～{1}:00", shippingTimeFrom, shippingTimeTo);
			}
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
