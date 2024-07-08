/*
=========================================================================================================
  Module      : ユーザー統合ユーザ情報入力クラス (UserIntegrationUserInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.UserIntegration;
using w2.Domain.UserIntegration.Helper;

/// <summary>
/// ユーザー統合ユーザ情報入力クラス
/// </summary>
[Serializable]
public class UserIntegrationUserInput : InputBase<UserIntegrationUserModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public UserIntegrationUserInput()
	{
		this.Histories = new UserIntegrationHistoryInput[0];
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public UserIntegrationUserInput(UserIntegrationUserContainer container)
		: this()
	{
		this.UserIntegrationNo = container.UserIntegrationNo.ToString();
		this.UserId = container.UserId;
		this.RepresentativeFlg = container.RepresentativeFlg;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		this.UserKbn = container.UserKbn;
		this.MallId = container.MallId;
		this.MallName = container.MallName;
		this.Name = container.Name;
		this.Name1 = container.Name1;
		this.Name2 = container.Name2;
		this.NameKana = container.NameKana;
		this.NameKana1 = container.NameKana1;
		this.NameKana2 = container.NameKana2;
		this.MailAddr = container.MailAddr;
		this.Zip = container.Zip;
		this.Addr = container.Addr;
		this.Addr1 = container.Addr1;
		this.Addr2 = container.Addr2;
		this.Addr3 = container.Addr3;
		this.Addr4 = container.Addr4;
		this.Addr5 = container.Addr5;
		this.AddrCountryName = container.AddrCountryName;
		this.AddrCountryIsoCode = container.AddrCountryIsoCode;
		this.Tel1 = container.Tel1;
		this.Tel2 = container.Tel2;
		this.Sex = container.Sex;
		this.Birth = container.Birth;
		this.UserDateCreated = container.UserDateCreated;
		this.UserDateChanged = container.UserDateChanged;
		this.DateLastLoggedin = container.DateLastLoggedin;
		this.HasCreditCards = container.HasCreditCards;
		// 履歴リスト
		this.Histories = container.Histories.Select(s => new UserIntegrationHistoryInput(s)).ToArray();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override UserIntegrationUserModel CreateModel()
	{
		var model = new UserIntegrationUserModel
		{
			UserIntegrationNo = this.UserIntegrationNo != null ? long.Parse(this.UserIntegrationNo) : 0,
			UserId = this.UserId,
			RepresentativeFlg = this.RepresentativeFlg,
			LastChanged = this.LastChanged,
		};
		model.Histories = this.Histories.Select(h => h.CreateModel()).ToArray();
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		return "";
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザー統合No</summary>
	public string UserIntegrationNo
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_INTEGRATION_NO]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_INTEGRATION_NO] = value; }
	}
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_ID]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_USER_ID] = value; }
	}
	/// <summary>代表フラグ</summary>
	public string RepresentativeFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_REPRESENTATIVE_FLG]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_REPRESENTATIVE_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONUSER_LAST_CHANGED] = value; }
	}
	/// <summary>代表フラグが「代表である」か</summary>
	public bool IsOnRepresentativeFlg
	{
		get { return (this.RepresentativeFlg == Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_ON); }
	}
	/// <summary>顧客区分</summary>
	public string UserKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_USER_KBN]; }
		set { this.DataSource[Constants.FIELD_USER_USER_KBN] = value; }
	}
	/// <summary>顧客区分テキスト</summary>
	public string UserKbnText
	{
		get { return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, this.UserKbn); }
	}
	/// <summary>
	/// 会員か判定
	/// </summary>
	public bool IsMember
	{
		get
		{
			switch (this.UserKbn)
			{
				case Constants.FLG_USER_USER_KBN_PC_USER:
				case Constants.FLG_USER_USER_KBN_MOBILE_USER:
				case Constants.FLG_USER_USER_KBN_SMARTPHONE_USER:
				case Constants.FLG_USER_USER_KBN_OFFLINE_USER:
					return true;

				default:
					return false;
			}
		}
	}
	/// <summary>モールID</summary>
	public string MallId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MALL_ID]; }
		set { this.DataSource[Constants.FIELD_USER_MALL_ID] = value; }
	}
	/// <summary>モール名</summary>
	public string MallName
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]; }
		set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME] = value; }
	}
	/// <summary>氏名</summary>
	public string Name
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_NAME] = value; }
	}
	/// <summary>氏名1</summary>
	public string Name1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME1]; }
		set { this.DataSource[Constants.FIELD_USER_NAME1] = value; }
	}
	/// <summary>氏名2</summary>
	public string Name2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME2]; }
		set { this.DataSource[Constants.FIELD_USER_NAME2] = value; }
	}
	/// <summary>氏名かな</summary>
	public string NameKana
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA]; }
		set { this.DataSource[Constants.FIELD_USER_NAME_KANA] = value; }
	}
	/// <summary>氏名かな1</summary>
	public string NameKana1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA1]; }
		set { this.DataSource[Constants.FIELD_USER_NAME_KANA1] = value; }
	}
	/// <summary>氏名かな2</summary>
	public string NameKana2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA2]; }
		set { this.DataSource[Constants.FIELD_USER_NAME_KANA2] = value; }
	}
	/// <summary>メールアドレス</summary>
	public string MailAddr
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR]; }
		set { this.DataSource[Constants.FIELD_USER_MAIL_ADDR] = value; }
	}
	/// <summary>メールアドレス2</summary>
	public string MailAddr2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR2]; }
		set { this.DataSource[Constants.FIELD_USER_MAIL_ADDR2] = value; }
	}
	/// <summary>郵便番号</summary>
	public string Zip
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ZIP]; }
		set { this.DataSource[Constants.FIELD_USER_ZIP] = value; }
	}
	/// <summary>住所</summary>
	public string Addr
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR] = value; }
	}
	/// <summary>住所1</summary>
	public string Addr1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR1]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR1] = value; }
	}
	/// <summary>住所2</summary>
	public string Addr2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR2]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR2] = value; }
	}
	/// <summary>住所3</summary>
	public string Addr3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR3]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR3] = value; }
	}
	/// <summary>住所4</summary>
	public string Addr4
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR4]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR4] = value; }
	}
	/// <summary>住所5</summary>
	public string Addr5
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR5]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR5] = value; }
	}
	/// <summary>住所国名</summary>
	public string AddrCountryName
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_NAME] = value; }
	}
	/// <summary>住所国ISOコード</summary>
	public string AddrCountryIsoCode
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE] = value; }
	}
	/// <summary>電話番号1</summary>
	public string Tel1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL1]; }
		set { this.DataSource[Constants.FIELD_USER_TEL1] = value; }
	}
	/// <summary>電話番号2</summary>
	public string Tel2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL2]; }
		set { this.DataSource[Constants.FIELD_USER_TEL2] = value; }
	}
	/// <summary>性別</summary>
	public string Sex
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_SEX]; }
		set { this.DataSource[Constants.FIELD_USER_SEX] = value; }
	}
	/// <summary>
	/// 性別テキスト
	/// </summary>
	public string SexText
	{
		get
		{
			return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, this.Sex);
		}
	}
	/// <summary>生年月日</summary>
	public DateTime? Birth
	{
		get { return (DateTime?)this.DataSource[Constants.FIELD_USER_BIRTH]; }
		set { this.DataSource[Constants.FIELD_USER_BIRTH] = value; }
	}
	/// <summary>作成日</summary>
	public DateTime UserDateCreated
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CREATED + "_" + Constants.TABLE_USER]; }
		set { this.DataSource[Constants.FIELD_USER_DATE_CREATED + "_" + Constants.TABLE_USER] = value; }
	}
	/// <summary>更新日</summary>
	public DateTime UserDateChanged
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CHANGED + "_" + Constants.TABLE_USER]; }
		set { this.DataSource[Constants.FIELD_USER_DATE_CHANGED + "_" + Constants.TABLE_USER] = value; }
	}
	/// <summary>最終ログイン日時</summary>
	public DateTime? DateLastLoggedin
	{
		get { return (DateTime?)this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN]; }
		set { this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN] = value; }
	}
	/// <summary>カード連携情報が存在する?</summary>
	public bool HasCreditCards
	{
		get { return (bool)this.DataSource["HasCreditCards"]; }
		set { this.DataSource["HasCreditCards"] = value; }
	}
	/// <summary>履歴リスト</summary>
	public UserIntegrationHistoryInput[] Histories
	{
		get { return (UserIntegrationHistoryInput[])this.DataSource["Histories"]; }
		set { this.DataSource["Histories"] = value; }
	}
	/// <summary>住所が日本か</summary>
	public bool IsAddrJp
	{
		get { return ((Constants.GLOBAL_OPTION_ENABLE == false)); }
	}
	#endregion
}
