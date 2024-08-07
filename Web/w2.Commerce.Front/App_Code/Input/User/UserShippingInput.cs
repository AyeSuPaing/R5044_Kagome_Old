/*
=========================================================================================================
  Module      : ユーザ配送先情報入力クラス (UserShippingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Global;
using w2.App.Common.Input;
using w2.Domain.UserShipping;

/// <summary>
/// ユーザ配送先情報入力クラス
/// </summary>
[Serializable]
public class UserShippingInput : InputBase<UserShippingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public UserShippingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public UserShippingInput(UserShippingModel model)
		: this()
	{
		this.UserId = model.UserId;
		this.ShippingNo = model.ShippingNo.ToString();
		this.Name = model.Name;
		this.ShippingName = model.ShippingName;
		this.ShippingNameKana = model.ShippingNameKana;
		this.ShippingZip = model.ShippingZip;
		this.ShippingZip1 = model.ShippingZip1;
		this.ShippingZip2 = model.ShippingZip2;
		this.ShippingAddr1 = model.ShippingAddr1;
		this.ShippingAddr2 = model.ShippingAddr2;
		this.ShippingAddr3 = model.ShippingAddr3;
		this.ShippingAddr4 = model.ShippingAddr4;
		this.ShippingTel1 = model.ShippingTel1;
		this.ShippingTel1_1 = model.ShippingTel1_1;
		this.ShippingTel1_2 = model.ShippingTel1_2;
		this.ShippingTel1_3 = model.ShippingTel1_3;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.ShippingName1 = model.ShippingName1;
		this.ShippingName2 = model.ShippingName2;
		this.ShippingNameKana1 = model.ShippingNameKana1;
		this.ShippingNameKana2 = model.ShippingNameKana2;
		this.ShippingCompanyName = model.ShippingCompanyName;
		this.ShippingCompanyPostName = model.ShippingCompanyPostName;
		this.ShippingCountryIsoCode = model.ShippingCountryIsoCode;
		this.ShippingCountryName = model.ShippingCountryName;
		this.ShippingAddr5 = model.ShippingAddr5;
		this.ShippingReceivingStoreFlg = model.ShippingReceivingStoreFlg;
		this.ShippingReceivingStoreId = model.ShippingReceivingStoreId;
		this.ShippingReceivingStoreType = model.ShippingReceivingStoreType;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override UserShippingModel CreateModel()
	{
		var model = new UserShippingModel
		{
			UserId = this.UserId,
			ShippingNo = int.Parse(this.ShippingNo),
			Name = this.Name,
			ShippingName = this.ShippingName,
			ShippingNameKana = this.ShippingNameKana,
			ShippingZip = this.ShippingZip,
			ShippingAddr1 = this.ShippingAddr1,
			ShippingAddr2 = this.ShippingAddr2,
			ShippingAddr3 = this.ShippingAddr3,
			ShippingAddr4 = this.ShippingAddr4,
			ShippingTel1 = this.ShippingTel1,
			ShippingName1 = this.ShippingName1,
			ShippingName2 = this.ShippingName2,
			ShippingNameKana1 = this.ShippingNameKana1,
			ShippingNameKana2 = this.ShippingNameKana2,
			ShippingCompanyName = this.ShippingCompanyName,
			ShippingCompanyPostName = this.ShippingCompanyPostName,
			ShippingCountryIsoCode = this.ShippingCountryIsoCode,
			ShippingCountryName = this.ShippingCountryName,
			ShippingAddr5 = this.ShippingAddr5,
			ShippingReceivingStoreFlg = this.ShippingReceivingStoreFlg,
			ShippingReceivingStoreId = this.ShippingReceivingStoreId,
			ShippingReceivingStoreType = StringUtility.ToEmpty(this.ShippingReceivingStoreType)
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public Dictionary<string, string> Validate()
	{
		// Set value for zip code
		this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP]
			= StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP]);

		// Set value for telephone
		this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1]
			= StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1]);

		var validationName = this.IsShippingAddrJp
			? "UserShippingRegist"
			: "UserShippingRegistGlobal";
		var errorMessages = Validator.ValidateAndGetErrorContainer(
			validationName,
			this.DataSource,
			this.ShippingCountryIsoCode);
		return errorMessages;
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_USER_ID]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_USER_ID] = value; }
	}
	/// <summary>配送先枝番</summary>
	public string ShippingNo
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NO]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NO] = value; }
	}
	/// <summary>配送先名</summary>
	public string Name
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_NAME]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_NAME] = value; }
	}
	/// <summary>配送先氏名</summary>
	public string ShippingName
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME] = value; }
	}
	/// <summary>配送先氏名かな</summary>
	public string ShippingNameKana
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA] = value; }
	}
	/// <summary>郵便番号</summary>
	public string ShippingZip
	{
		get
		{
			if ((string.IsNullOrEmpty(this.ShippingZip1) == false) && (string.IsNullOrEmpty(this.ShippingZip2) == false))
			{
				return string.Join("-", this.ShippingZip1, this.ShippingZip2);
			}
			return this.ShippingZipInner;
		}
		set { this.ShippingZipInner = value; }
	}
	/// <summary>郵便番号（内部用）</summary>
	public string ShippingZipInner
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP] = value; }
	}
	/// <summary>郵便番号1</summary>
	public string ShippingZip1
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP + "_1"]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP + "_1"] = value; }
	}
	/// <summary>郵便番号2</summary>
	public string ShippingZip2
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP + "_2"]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP + "_2"] = value; }
	}
	/// <summary>住所1</summary>
	public string ShippingAddr1
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1] = value; }
	}
	/// <summary>住所2</summary>
	public string ShippingAddr2
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2] = value; }
	}
	/// <summary>住所3</summary>
	public string ShippingAddr3
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3] = value; }
	}
	/// <summary>住所4</summary>
	public string ShippingAddr4
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4] = value; }
	}
	/// <summary>電話番号1</summary>
	public string ShippingTel1
	{
		get
		{
			if ((string.IsNullOrEmpty(this.ShippingTel1_1) == false)
				&& (string.IsNullOrEmpty(this.ShippingTel1_2) == false)
				&& (string.IsNullOrEmpty(this.ShippingTel1_3) == false))
			{
				return string.Join("-", this.ShippingTel1_1, this.ShippingTel1_2, this.ShippingTel1_3);
			}
			return m_shippingTel1;
		}
		set { m_shippingTel1 = value; }
	}
	private string m_shippingTel1
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1] = value; }
	}
	/// <summary>電話番号1_1</summary>
	public string ShippingTel1_1
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1 + "_1"]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1 + "_1"] = value; }
	}
	/// <summary>電話番号1_2</summary>
	public string ShippingTel1_2
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1 + "_2"]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1 + "_2"] = value; }
	}
	/// <summary>電話番号1_3</summary>
	public string ShippingTel1_3
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1 + "_3"]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1 + "_3"] = value; }
	}
	/// <summary>電話番号2</summary>
	public string ShippingTel2
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL2]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL2] = value; }
	}
	/// <summary>電話番号3</summary>
	public string ShippingTel3
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL3]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL3] = value; }
	}
	/// <summary>ＦＡＸ</summary>
	public string ShippingFax
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_FAX]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_FAX] = value; }
	}
	/// <summary>配送業者</summary>
	public string ShippingCompany
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CHANGED] = value; }
	}
	/// <summary>配送先氏名1</summary>
	public string ShippingName1
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME1]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME1] = value; }
	}
	/// <summary>配送先氏名2</summary>
	public string ShippingName2
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME2]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME2] = value; }
	}
	/// <summary>配送先氏名かな1</summary>
	public string ShippingNameKana1
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1] = value; }
	}
	/// <summary>配送先氏名かな2</summary>
	public string ShippingNameKana2
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2] = value; }
	}
	/// <summary>企業名</summary>
	public string ShippingCompanyName
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME] = value; }
	}
	/// <summary>部署名</summary>
	public string ShippingCompanyPostName
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME] = value; }
	}
	/// <summary>配送先国ISOコード</summary>
	public string ShippingCountryIsoCode
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE] = value; }
	}
	/// <summary>配送先国名</summary>
	public string ShippingCountryName
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_NAME]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_NAME] = value; }
	}
	/// <summary>住所5</summary>
	public string ShippingAddr5
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5] = value; }
	}
	/// <summary>配送先住所は日本か</summary>
	public bool IsShippingAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.ShippingCountryIsoCode); }
	}
	/// <summary>店舗受取フラグ</summary>
	public string ShippingReceivingStoreFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]; }
		set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG] = value; }
	}
	/// <summary>店舗受取店舗ID</summary>
	public string ShippingReceivingStoreId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID]; }
		set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID] = value; }
	}
	/// <summary>コンビニ受取：受取方法</summary>
	public string ShippingReceivingStoreType
	{
		get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE]; }
		set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = value; }
	}
	#endregion
}
