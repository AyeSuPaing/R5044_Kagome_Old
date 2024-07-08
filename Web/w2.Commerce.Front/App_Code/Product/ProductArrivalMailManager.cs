/*
=========================================================================================================
  Module      : 入荷通知メール登録機能共通処理(ProductArrivalMailManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using w2.Domain.User;
using w2.App.Common;
using w2.App.Common.Global.Region;
using w2.App.Common.Order;
using w2.App.Common.UserProductArrivalMail;

/// <summary>
/// 入荷通知メール登録機能共通処理用マネージャークラス
/// </summary>
[Serializable]
public class ProductArrivalMailManager
{
	/// <summary>
	/// 存在しない入荷通知メール区分ならばエラー画面に遷移
	/// </summary>
	public void CheckArrivalMailKbn()
	{
		switch (this.ArrivalMailKbn)
		{
			case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL:
			case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE:
			case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE:
				break;

			default:
				HttpContext.Current.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR);
				HttpContext.Current.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				break;
		}
	}

	/// <summary>
	/// 商品情報を取得
	/// </summary>
	public void SetProductInfo()
	{
		if (string.IsNullOrEmpty(this.VariationId)) this.VariationId = this.ProductId;

		var productVariation = ProductCommon.GetProductVariationInfo(Constants.CONST_DEFAULT_SHOP_ID, this.ProductId, this.VariationId, null);
		if (productVariation.Count > 0)
		{
			this.ProductName = (string)productVariation[0][Constants.FIELD_PRODUCT_NAME];
			this.HasVariation = ProductCommon.HasVariation(productVariation[0]);
			this.VariationName1 = (string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
			this.VariationName2 = (string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
			this.VariationName3 = (string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
			this.BrandId1 = (string)productVariation[0][Constants.FIELD_PRODUCT_BRAND_ID1];
			this.BrandId2 = (string)productVariation[0][Constants.FIELD_PRODUCT_BRAND_ID2];
			this.BrandId3 = (string)productVariation[0][Constants.FIELD_PRODUCT_BRAND_ID3];
			this.BrandId4 = (string)productVariation[0][Constants.FIELD_PRODUCT_BRAND_ID4];
			this.BrandId5 = (string)productVariation[0][Constants.FIELD_PRODUCT_BRAND_ID5];
		}
		else
		{
			// 商品が見つからないときはエラー画面に遷移
			HttpContext.Current.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			HttpContext.Current.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN + "=" + Constants.KBN_REQUEST_ERRORPAGE_GOTOP);
		}
	}

	/// <summary>
	/// 入荷通知情報をセット
	/// </summary>
	public void SetArrivalInfo()
	{
		// 入荷通知情報セット
		this.IsPcAddrRegistered = false;
		this.IsMbAddrRegistered = false;
		this.IsOtherAddrRegistered = false;
		var productArrivalMailInfo = UserProductArrivalMailCommon.GetUserProductArrivalMailInfo(this.LoginUserId, this.ProductId, this.VariationId);
		foreach (DataRowView row in productArrivalMailInfo)
		{
			if ((DateTime)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED] < DateTime.Now) continue; // 期限切れはスキップ
			if ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN] != this.ArrivalMailKbn) continue; // 異なる区分はスキップ

			if ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR] != "")
			{
				// その他メールアドレスで登録済み
				this.IsOtherAddrRegistered = true;
				continue;
			}

			switch ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN])
			{
				// PCメールアドレスで通知登録済み
				case Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC:
					this.IsPcAddrRegistered = true;
					break;

				// MBメールアドレスで通知登録済み
				case Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE:
					this.IsMbAddrRegistered = true;
					break;
			}
		}
	}

	/// <summary>
	/// メールアドレスの区分選択ラジオボタンセット
	/// </summary>
	/// <returns>ラジオボタンリスト</returns>
	public ListItem[] SetMailAddrCheckBox()
	{
		var buttonList = new List<ListItem>();
		buttonList.Add(new ListItem(UserProductArrivalMailCommon.CreateMailAddressKbnNameFromTagReplacer(Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC), Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC));
		if (Constants.MOBILEOPTION_ENABLED)
		{
			buttonList.Add(new ListItem(UserProductArrivalMailCommon.CreateMailAddressKbnNameFromTagReplacer(Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE), Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE));
		}
		return buttonList.ToArray();
	}

	/// <summary>
	/// 通知登録&amp;メール送信のデータ準備
	/// </summary>
	/// <param name="isPcAddrChecked">PCメールアドレスチェック状態</param>
	/// <param name="isMbAddrChecked">MBメールアドレスチェック状態</param>
	/// <param name="isOtherChecked">その他チェック状態</param>
	/// <param name="selectValue">その他ラジオボタンインデックス</param>
	/// <param name="addr">その他メールアドレス</param>
	/// <returns>エラーメッセージ</returns>
	public string StanbyMailInfo(bool isPcAddrChecked, bool isMbAddrChecked, bool isOtherChecked,string selectValue, string addr)
	{
		// 入力チェック
		var errorMessage = "";
		if (isOtherChecked)
		{
			errorMessage = this.ValidationMailKbn(
				StringUtility.ToHankaku(addr.Trim()),
				selectValue);
			if (errorMessage != "") return errorMessage;
		}

		this.MailInfoList = new List<ArrivalMailInfo>();

		if (this.IsLoggedIn)
		{
			// 会員
			if (isPcAddrChecked && (this.IsPcAddrRegistered == false))
			{
				if (this.MailInfoList == null) this.MailInfoList = new List<ArrivalMailInfo>();
				this.MailInfoList.Add(new ArrivalMailInfo(this.LoginUserId, Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC, ""));
				this.HasPcMailKbnResult = true;
			}
			if (isMbAddrChecked && (this.IsMbAddrRegistered == false))
			{
				if (this.MailInfoList == null) this.MailInfoList = new List<ArrivalMailInfo>();
				this.MailInfoList.Add(new ArrivalMailInfo(this.LoginUserId, Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE, ""));
				this.HasMbMailKbnResult = true;
			}
			if (isOtherChecked)
			{
				if (this.MailInfoList == null) this.MailInfoList = new List<ArrivalMailInfo>();
				this.MailInfoList.Add(new ArrivalMailInfo(this.LoginUserId, selectValue, StringUtility.ToHankaku(addr.Trim())));
				this.HasOtherMailKbnResult = true;
			}

			// 入力チェック
			if ((this.IsPcAddrRegistered == false) || (this.IsMbAddrRegistered == false))
			{
				errorMessage = this.ValidationMailAddr();
			}
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				SetRegisterFailedInfo();
				return errorMessage;
			}
		}
		else
		{
			// ゲスト
			if (isOtherChecked)
			{
				if (this.MailInfoList == null) this.MailInfoList = new List<ArrivalMailInfo>();
				this.MailInfoList.Add(new ArrivalMailInfo(Constants.FLG_USERPRODUCTARRIVALMAIL_USER_ID_GUEST, selectValue, StringUtility.ToHankaku(addr.Trim())));
			}
		}
		return "";
	}

		/// <summary>
	/// メール区分チェック
	/// </summary>
	/// <param name="addr">メールアドレス</param>
	/// <param name="mailKbn">メール区分</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidationMailKbn(string addr, string mailKbn)
	{
		var errorMessage = Validator.Validate(
			"ProductArrivalMail",
			new Hashtable
			{
				{Constants.FIELD_USER_MAIL_ADDR, addr},
				{"mail_addr_kbn", mailKbn }
			});
		return errorMessage;
	}

	/// <summary>
	/// メールアドレスのチェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string ValidationMailAddr()
	{
		var errorMesasge = Validator.Validate(
			"ProductArrivalMail",
			new Hashtable
			{
				{"mail_addr_check", new string('*', this.MailInfoList.Count)}
			});
		return errorMesasge;
	}

	/// <summary>
	/// メール登録失敗時の結果情報の初期化
	/// </summary>
	private void SetRegisterFailedInfo()
	{
		this.HasPcMailKbnResult = false;
		this.HasMbMailKbnResult = false;
		this.HasOtherMailKbnResult = false;
	}

	/// <summary>
	/// メール送信と通知メール登録処理
	/// </summary>
	public void SendMailAndRegisterArival()
	{
		var mailId = GetMailTemplateId(this.ArrivalMailKbn);
		//メール送信準備
		var inputParameter = StanbySendMail();

		// 処理実行
		foreach (var info in this.MailInfoList)
		{
			// 入荷通知登録処理
			UserProductArrivalMailCommon.RegistUserProductArrivalMail(
				info.UserId,
				Constants.CONST_DEFAULT_SHOP_ID,
				this.ProductId,
				this.VariationId,
				info.PcMbKbn,
				this.ArrivalMailKbn,
				info.GuestAddr);

			// PCフラグ格納
			var isPc = (info.PcMbKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC);

			// メール送信処理
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				mailId,
				this.LoginUserId,
				inputParameter,
				isPc,
				Constants.MailSendMethod.Auto,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId,
				(info.GuestAddr != "") ? info.GuestAddr : this.PcAddr))
			{
				// 送信先の設定（空でなければguest_addr宛）
				mailSender.AddTo((info.GuestAddr != "") ? info.GuestAddr : (isPc ? this.PcAddr : this.MbAddr));

				// 送信
				if (mailSender.SendMail() == false)
				{
					// エラーログ出力
					AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + mailSender.MailSendException.ToString());
				}
			}
		}
	}

	/// <summary>
	/// 通知メール区分より、メールテンプレートID取得
	/// </summary>
	/// <param name="arrivalMailKbn">通知メール区分</param>
	/// <returns>メールテンプレートID</returns>
	private string GetMailTemplateId(string arrivalMailKbn)
	{
		// メールテンプレートID格納
		var mailId = "";
		switch (arrivalMailKbn)
		{
			case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL:
				mailId = Constants.CONST_MAIL_ID_ACCEPT_PRODUCT_ARRIVAL;
				break;

			case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE:
				mailId = Constants.CONST_MAIL_ID_ACCEPT_PRODUCT_RELEASE;
				break;

			case Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE:
				mailId = Constants.CONST_MAIL_ID_ACCEPT_PRODUCT_RESALE;
				break;
		}
		return mailId;
	}

	/// <summary>
	/// メール送信準備
	/// </summary>
	private Hashtable StanbySendMail()
	{
		// 商品情報を取得
		SetProductInfo();

		// 入力パラメータ格納
		var inputParameter = new Hashtable();
		var productName = ProductCommon.CreateProductJointName(this.ProductName, this.VariationName1, this.VariationName2, this.VariationName3);
		inputParameter.Add(Constants.FIELD_SHOP_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID);	// shop_id
		inputParameter.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, this.ProductId);	// product_id
		inputParameter.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, this.VariationId);	// variation_id
		inputParameter.Add(Constants.FIELD_PRODUCT_PRODUCT_ID + "_urlenc", HttpUtility.UrlEncode(this.ProductId));	// product_id (URLエンコード済み)
		inputParameter.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID + "_urlenc", HttpUtility.UrlEncode(this.VariationId));	// variation_id (URLエンコード済み)
		inputParameter.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME, productName);	// product_name
		inputParameter.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, this.VariationName1);	// variation_name1
		inputParameter.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, this.VariationName2);	// variation_name2
		inputParameter.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3, this.VariationName3);	// variation_name3
		inputParameter.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED, this.ExpiredDate);	// date_expired	> 2012/09/30 23:59:59
		inputParameter.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED + "_yyyy", this.ExpiredDate.ToString("yyyy"));	// date_expired_yyyy	> 2012
		inputParameter.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED + "_mm", this.ExpiredDate.ToString("MM"));	// date_expired_mm		> 09
		inputParameter.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED + "_m", this.ExpiredDate.ToString("%M"));	// date_expired_m		> 9
		inputParameter.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED + "_dd", this.ExpiredDate.ToString("dd"));	// date_expired_dd		> 30
		inputParameter.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED + "_d", this.ExpiredDate.ToString("%d"));	// date_expired_d		> 30
		inputParameter.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED + "_time", this.ExpiredDate.ToString("HH:mm:ss"));	// date_expired_time	> 23:59:59
		inputParameter.Add(Constants.FIELD_PRODUCT_BRAND_ID1, this.BrandId1);	// ブランドID1
		inputParameter.Add(Constants.FIELD_PRODUCT_BRAND_ID2, this.BrandId2);	// ブランドID2
		inputParameter.Add(Constants.FIELD_PRODUCT_BRAND_ID3, this.BrandId3);	// ブランドID3
		inputParameter.Add(Constants.FIELD_PRODUCT_BRAND_ID4, this.BrandId4);	// ブランドID4
		inputParameter.Add(Constants.FIELD_PRODUCT_BRAND_ID5, this.BrandId5);	// ブランドID5

		inputParameter.Add(
			Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID,
			this.IsLoggedIn ? this.LoginUserId : Constants.FLG_USERPRODUCTARRIVALMAIL_USER_ID_GUEST);	// 会員/ゲストの振り分けタグ

		var user = new UserService().Get(this.LoginUserId);
		inputParameter.Add(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, (user == null) ? string.Empty : user.UserManagementLevelId);
		inputParameter.Add(Constants.FIELD_ORDER_ADVCODE_FIRST, (user == null) ? string.Empty : user.AdvcodeFirst);	// 初回広告コード

		return inputParameter;
	}

	/// <summary>入荷通知区分</summary>
	public string ArrivalMailKbn { get; set; }

	/// <summary>ログインしているか</summary>
	public bool IsLoggedIn { get; set; }
	/// <summary>ユーザーID</summary>
	public string LoginUserId { get; set; }

	/// <summary>商品ID</summary>
	public string ProductId { get; set; }
	/// <summary>バリエーションID</summary>
	public string VariationId { get; set; }
	/// <summary>商品名</summary>
	public string ProductName { get; set; }
	/// <summary>バリエーションを持っているか</summary>
	public bool HasVariation { get; set; }
	/// <summary>バリエーション表示名1</summary>
	public string VariationName1 { get; set; }
	/// <summary>バリエーション表示名2</summary>
	public string VariationName2 { get; set; }
	/// <summary>バリエーション表示名3</summary>
	public string VariationName3 { get; set; }
	/// <summary>ブランドID1</summary>
	public string BrandId1 { get; set; }
	/// <summary>ブランドID2</summary>
	public string BrandId2 { get; set; }
	/// <summary>ブランドID3</summary>
	public string BrandId3 { get; set; }
	/// <summary>ブランドID4</summary>
	public string BrandId4 { get; set; }
	/// <summary>ブランドID5</summary>
	public string BrandId5 { get; set; }

	/// <summary>PCメールアドレス</summary>
	public string PcAddr { get; set; }
	/// <summary>MBメールアドレス</summary>
	public string MbAddr { get; set; }
	/// <summary>PCメールアドレスの有無</summary>
	public bool HasPcAddr { get { return (this.PcAddr != ""); } }
	/// <summary>MBメールアドレスの有無</summary>
	public bool HasMbAddr { get { return (this.MbAddr != ""); } }

	/// <summary>入荷通知メールの有効期限</summary>
	public DateTime ExpiredDate
	{
		get { return UserProductArrivalMailCommon.GetExpiredDate(this.ArrivalMailKbn); }
	}

	/// <summary>PCメールアドレスが入荷通知登録済みかどうか</summary>
	public bool IsPcAddrRegistered { get; set; }
	/// <summary>MBメールアドレスが入荷通知登録済みかどうか</summary>
	public bool IsMbAddrRegistered { get; set; }
	/// <summary>その他メールアドレスが入荷通知登録済みかどうか</summary>
	public bool IsOtherAddrRegistered { get; set; }

	/// <summary>登録したものにPCメール区分は含まれているか</summary>
	public bool HasPcMailKbnResult { get; set; }
	/// <summary>登録したものにMBメール区分は含まれているか</summary>
	public bool HasMbMailKbnResult { get; set; }
	/// <summary>登録したものにその他メール区分は含まれているか</summary>
	public bool HasOtherMailKbnResult { get; set; }


	/// <summary>通知メール情報</summary>
	private List<ArrivalMailInfo> MailInfoList { get; set; }

	/// <summary>
	/// 入荷通知情報
	/// </summary>
	private class ArrivalMailInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailKbn">PC/モバイル区分</param>
		/// <param name="addr">ゲストユーザーのメールアドレス</param>
		public ArrivalMailInfo(string userId, string mailKbn, string addr)
		{
			this.UserId = userId;
			this.PcMbKbn = mailKbn;
			this.GuestAddr = addr;
		}

		/// <summary>ユーザーID</summary>
		public string UserId { get; set; }
		/// <summary>PC/モバイル区分</summary>
		public string PcMbKbn { get; set; }
		/// <summary>ゲストユーザーのメールアドレス</summary>
		public string GuestAddr { get; set; }
	}
}
