/*
=========================================================================================================
  Module      : 定期購入配送先情報入力クラス (FixedPurchaseShippingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Global;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.App.Common.Input;

/// <summary>
/// 定期購入配送先情報入力クラス
/// </summary>
public class FixedPurchaseShippingInput : InputBase<FixedPurchaseShippingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public FixedPurchaseShippingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="shippingContainer">表示用定期購入情報（配送先）</param>
	public FixedPurchaseShippingInput(FixedPurchaseShippingContainer shippingContainer)
		: this()
	{
		this.FixedPurchaseId = shippingContainer.FixedPurchaseId;
		this.FixedPurchaseShippingNo = shippingContainer.FixedPurchaseShippingNo.ToString();
		this.ShippingName = shippingContainer.ShippingName;
		this.ShippingNameKana = shippingContainer.ShippingNameKana;
		this.ShippingZip = shippingContainer.ShippingZip;
		this.ShippingAddr1 = shippingContainer.ShippingAddr1;
		this.ShippingAddr2 = shippingContainer.ShippingAddr2;
		this.ShippingAddr3 = shippingContainer.ShippingAddr3;
		this.ShippingAddr4 = shippingContainer.ShippingAddr4;
		this.ShippingTel1 = shippingContainer.ShippingTel1;
		this.ShippingTime = shippingContainer.ShippingTime;
		this.DateCreated = shippingContainer.DateCreated.ToString();
		this.DateChanged = shippingContainer.DateChanged.ToString();
		this.ShippingName1 = shippingContainer.ShippingName1;
		this.ShippingName2 = shippingContainer.ShippingName2;
		this.ShippingNameKana1 = shippingContainer.ShippingNameKana1;
		this.ShippingNameKana2 = shippingContainer.ShippingNameKana2;
		this.ShippingCompanyName = shippingContainer.ShippingCompanyName;
		this.ShippingCompanyPostName = shippingContainer.ShippingCompanyPostName;
		this.ShippingMethod = shippingContainer.ShippingMethod;
		this.ShippingAddrCountryIsoCode = shippingContainer.ShippingCountryIsoCode;
		this.ShippingAddrCountryName = shippingContainer.ShippingCountryName;
		this.ShippingAddr5 = shippingContainer.ShippingAddr5;
		this.ShippingReceivingStoreFlg = shippingContainer.ShippingReceivingStoreFlg;
		this.ShippingReceivingStoreId = shippingContainer.ShippingReceivingStoreId;
		this.ShippingReceivingStoreType = shippingContainer.ShippingReceivingStoreType;
		this.DeliveryCompanyId = shippingContainer.DeliveryCompanyId;
		// 商品リスト
		this.Items = shippingContainer.Items.Select(i => new FixedPurchaseItemInput(i)).ToArray();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override FixedPurchaseShippingModel CreateModel()
	{
		var model = new FixedPurchaseShippingModel
		{
			FixedPurchaseId = this.FixedPurchaseId,
			FixedPurchaseShippingNo = int.Parse(this.FixedPurchaseShippingNo),
			ShippingName = this.ShippingName,
			ShippingNameKana = this.ShippingNameKana,
			ShippingZip = this.ShippingZip,
			ShippingAddr1 = this.ShippingAddr1,
			ShippingAddr2 = this.ShippingAddr2,
			ShippingAddr3 = this.ShippingAddr3,
			ShippingAddr4 = this.ShippingAddr4,
			ShippingTel1 = this.ShippingTel1,
			ShippingTime = this.ShippingTime,
			ShippingName1 = this.ShippingName1,
			ShippingName2 = this.ShippingName2,
			ShippingNameKana1 = this.ShippingNameKana1,
			ShippingNameKana2 = this.ShippingNameKana2,
			ShippingCompanyName = this.ShippingCompanyName,
			ShippingCompanyPostName = this.ShippingCompanyPostName,
			ShippingMethod = this.ShippingMethod,
			DeliveryCompanyId = this.DeliveryCompanyId,
			ShippingCountryIsoCode = this.ShippingAddrCountryIsoCode,
			ShippingCountryName = this.ShippingAddrCountryName,
			ShippingAddr5 = this.ShippingAddr5,
			ShippingReceivingStoreId = this.ShippingReceivingStoreId,
			ShippingReceivingStoreFlg = this.ShippingReceivingStoreFlg,
			ShippingReceivingStoreType = StringUtility.ToEmpty(this.ShippingReceivingStoreType)
		};
		// 商品リスト
		model.Items = this.Items.Select(i => i.CreateModel()).ToArray();
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="excludeList">入力チェック除外項目</param>
	/// <returns>エラーメッセージ</returns>
	/// <remarks>stringではなく、Dictionaryで返す。管理サイトとは異なる。</remarks>
	public Dictionary<string, string> Validate(List<string> excludeList = null)
	{
		// Set value for zip
		this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP]
			= StringUtility.ToEmpty(this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP]);

		// Set value for tel
		this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1]
			= StringUtility.ToEmpty(this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1]);
		var dataSource = (Hashtable)this.DataSource.Clone();

		foreach (var item in excludeList)
		{
			dataSource.Remove(item);
		}

		var validationName = this.IsShippingAddrJp
			? "FixedPurchaseModifyInput"
			: "FixedPurchaseModifyInputGlobal";
		return Validator.ValidateAndGetErrorContainer(validationName, dataSource, this.ShippingAddrCountryIsoCode);
	}
	#endregion

	#region プロパティ
	/// <summary>定期購入ID</summary>
	public string FixedPurchaseId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_ID] = value; }
	}
	/// <summary>定期購入配送先枝番</summary>
	public string FixedPurchaseShippingNo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO] = value; }
	}
	/// <summary>配送先氏名</summary>
	public string ShippingName
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME] = value; }
	}
	/// <summary>配送先氏名かな</summary>
	public string ShippingNameKana
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA] = value; }
	}
	/// <summary>郵便番号</summary>
	public string ShippingZip
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP] = value; }
	}
	/// <summary>郵便番号（ハイフンなし）</summary>
	public string HyphenlessShippingZip
	{
		get { return this.ShippingZip.Replace("-", ""); }
	}
	/// <summary>郵便番号1</summary>
	public string ShippingZip1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP + "_1"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP + "_1"] = value; }
	}
	/// <summary>郵便番号2</summary>
	public string ShippingZip2
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP + "_2"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP + "_2"] = value; }
	}
	/// <summary>住所1</summary>
	public string ShippingAddr1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR1]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR1] = value; }
	}
	/// <summary>住所2</summary>
	public string ShippingAddr2
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR2]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR2] = value; }
	}
	/// <summary>住所3</summary>
	public string ShippingAddr3
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR3]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR3] = value; }
	}
	/// <summary>住所４</summary>
	public string ShippingAddr4
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR4]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR4] = value; }
	}
	/// <summary>電話番号1</summary>
	public string ShippingTel1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1] = value; }
	}
	/// <summary>電話番号1_1</summary>
	public string ShippingTel1_1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1 + "_1"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1 + "_1"] = value; }
	}
	/// <summary>電話番号1_2</summary>
	public string ShippingTel1_2
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1 + "_2"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1 + "_2"] = value; }
	}
	/// <summary>電話番号1_3</summary>
	public string ShippingTel1_3
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1 + "_3"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1 + "_3"] = value; }
	}
	/// <summary>配送希望時間帯</summary>
	public string ShippingTime
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TIME]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TIME] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_DATE_CHANGED] = value; }
	}
	/// <summary>配送先氏名1</summary>
	public string ShippingName1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME1]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME1] = value; }
	}
	/// <summary>配送先氏名2</summary>
	public string ShippingName2
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME2]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME2] = value; }
	}
	/// <summary>配送先氏名かな1</summary>
	public string ShippingNameKana1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA1]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA1] = value; }
	}
	/// <summary>配送先氏名かな2</summary>
	public string ShippingNameKana2
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA2]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA2] = value; }
	}
	/// <summary>配送先企業名</summary>
	public string ShippingCompanyName
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COMPANY_NAME]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COMPANY_NAME] = value; }
	}
	/// <summary>配送先部署名</summary>
	public string ShippingCompanyPostName
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COMPANY_POST_NAME]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COMPANY_POST_NAME] = value; }
	}
	/// <summary>配送方法</summary>
	public string ShippingMethod
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]; }
		set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD] = value; }
	}
	/// <summary>配送会社ID</summary>
	public string DeliveryCompanyId
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID] = value; }
	}
	/// <summary>商品リスト</summary>
	public FixedPurchaseItemInput[] Items
	{
		get { return (FixedPurchaseItemInput[])this.DataSource["Items"]; }
		set { this.DataSource["Items"] = value; }
	}
	/// <summary>住所国ISOコード</summary>
	public string ShippingAddrCountryIsoCode
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_ISO_CODE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_ISO_CODE] = value; }
	}
	/// <summary>住所国名</summary>
	public string ShippingAddrCountryName
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_NAME]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_NAME] = value; }
	}
	/// <summary>住所5</summary>
	public string ShippingAddr5
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR5]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR5] = value; }
	}
	/// <summary>配送先住所は日本か</summary>
	public bool IsShippingAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>店舗受取フラグ</summary>
	public string ShippingReceivingStoreFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_FLG]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_FLG] = value; }
	}
	/// <summary>店舗受取店舗ID</summary>
	public string ShippingReceivingStoreId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_ID] = value; }
	}
	/// <summary>コンビニ受取：受取方法</summary>
	public string ShippingReceivingStoreType
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_TYPE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = value; }
	}
	#endregion
}