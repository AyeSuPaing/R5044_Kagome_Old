/*
=========================================================================================================
  Module      : 定期購入配送先情報入力クラス (FixedPurchaseShippingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.App.Common.Global;
using w2.App.Common.Input;
using w2.App.Common.Order.Payment.ECPay;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;

/// <summary>
/// 定期購入配送先情報入力クラス
/// </summary>
[Serializable]
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
		this.ShippingCountryIsoCode = shippingContainer.ShippingCountryIsoCode;
		this.ShippingCountryName = shippingContainer.ShippingCountryName;
		this.ShippingZip = shippingContainer.ShippingZip;
		this.ShippingAddr1 = shippingContainer.ShippingAddr1;
		this.ShippingAddr2 = shippingContainer.ShippingAddr2;
		this.ShippingAddr3 = shippingContainer.ShippingAddr3;
		this.ShippingAddr4 = shippingContainer.ShippingAddr4;
		this.ShippingAddr5 = shippingContainer.ShippingAddr5;
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
		this.DeliveryCompanyId = shippingContainer.DeliveryCompanyId;
		this.ShippingReceivingStoreFlg = shippingContainer.ShippingReceivingStoreFlg;
		this.ShippingReceivingStoreId = shippingContainer.ShippingReceivingStoreId;
		this.ShippingReceivingStoreType = shippingContainer.ShippingReceivingStoreType;
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
			ShippingCountryIsoCode = this.ShippingCountryIsoCode,
			ShippingCountryName = this.ShippingCountryName,
			ShippingZip = this.ShippingZip,
			ShippingAddr1 = this.ShippingAddr1,
			ShippingAddr2 = this.ShippingAddr2,
			ShippingAddr3 = this.ShippingAddr3,
			ShippingAddr4 = this.ShippingAddr4,
			ShippingAddr5 = this.ShippingAddr5,
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
			ShippingReceivingStoreId = StringUtility.ToEmpty(this.ShippingReceivingStoreId),
			ShippingReceivingStoreFlg = StringUtility.ToEmpty(this.ShippingReceivingStoreFlg),
			ShippingReceivingStoreType = StringUtility.ToEmpty(this.ShippingReceivingStoreType),
		};
		// 商品リスト
		model.Items = this.Items.Select(i => i.CreateModel()).ToArray();
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="orderPaymentKbn">支払い区分</param>
	/// <param name="isUpdateShipping">Is Update Shipping</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(string orderPaymentKbn, bool isUpdateShipping)
	{
		var validationName = this.IsShippingAddrJp
			? (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				? "FixedPurchaseAmazonModifyInput" : "FixedPurchaseModifyInput"
			: "FixedPurchaseModifyInputGlobal";

		var dataSource = (Hashtable)this.DataSource.Clone();
		if (isUpdateShipping == false)
		{
			dataSource.Remove(Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME1);
			dataSource.Remove(Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME2);
			dataSource.Remove(Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP);
			dataSource.Remove(Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR2);

			// Remove key shipping tel1 when the delivery destination is 7-Eleven
			if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (this.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				&& ECPayUtility.CheckShippingReceivingStoreType7Eleven(this.ShippingReceivingStoreType))
			{
				dataSource.Remove(Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1);
			}
		}

		return Validator.Validate(validationName, dataSource, this.ShippingCountryIsoCode);
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="checkKbn">対象チェック区分</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(string checkKbn)
	{
		return Validator.Validate(checkKbn, this.DataSource);
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
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_METHOD]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_METHOD] = value; }
	}
	/// <summary>配送会社ID</summary>
	public string DeliveryCompanyId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_DELIVERY_COMPANY_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_DELIVERY_COMPANY_ID] = value; }
	}
	/// <summary>商品リスト</summary>
	public FixedPurchaseItemInput[] Items
	{
		get { return (FixedPurchaseItemInput[])this.DataSource["Items"]; }
		set { this.DataSource["Items"] = value; }
	}
	/// <summary>国ISOコード</summary>
	public string ShippingCountryIsoCode
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_ISO_CODE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_ISO_CODE] = value; }
	}
	/// <summary>国名</summary>
	public string ShippingCountryName
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
		get { return GlobalAddressUtil.IsCountryJp(this.ShippingCountryIsoCode); }
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
	/// <summary>User Shipping Kbn</summary>
	public string UserShippingKbn
	{
		get { return (string)this.DataSource["UserShippingKbn"]; }
		set { this.DataSource["UserShippingKbn"] = value; }
	}
	/// <summary>コンビニ受取：受取方法</summary>
	public string ShippingReceivingStoreType
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_TYPE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = value; }
	}
	#endregion
}