/*
=========================================================================================================
  Module      : デフォルト注文方法設定情報入力パラメータークラス(UserDefaultOrderSettingInputParameter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// デフォルト注文方法設定情報入力パラメータークラス
/// </summary>
[Serializable]
public class UserDefaultOrderSettingInputParameter
{
	/// <summary>選択中の配送先方法が注文者情報の住所であるかの判定</summary>
	public bool IsSelectedDefaultOwnerShippingOn { get; set; }
	/// <summary>選択中の配送先方法があるかの判定</summary>
	public bool IsSelectedDefaultShippingOn { get; set; }
	/// <summary>選択中の支払方法があるかの判定</summary>
	public bool IsSelectedDefaultPaymentOn { get; set; }
	/// <summary>決済種別ID</summary>
	public string PaymentId { get; set; }
	/// <summary>決済種別名</summary>
	public string PaymentName { get; set; }
	/// <summary>配送先枝番</summary>
	public string ShippingNo { get; set; }
	/// <summary>配送先名</summary>
	public string ShippingName { get; set; }
	/// <summary>配送先名1</summary>
	public string ShippingName1 { get; set; }
	/// <summary>配送先名2</summary>
	public string ShippingName2 { get; set; }
	/// <summary>配送先名(かな1)</summary>
	public string ShippingNameKana1 { get; set; }
	/// <summary>配送先名(かな2)</summary>
	public string ShippingNameKana2 { get; set; }
	/// <summary>配送先名</summary>
	public string ShippingZip { get; set; }
	/// <summary>配送先住所1</summary>
	public string ShippingAddress1 { get; set; }
	/// <summary>配送先住所2</summary>
	public string ShippingAddress2 { get; set; }
	/// <summary>配送先住所3</summary>
	public string ShippingAddress3 { get; set; }
	/// <summary>配送先住所4</summary>
	public string ShippingAddress4 { get; set; }
	/// <summary>配送先住所5</summary>
	public string ShippingAddress5 { get; set; }
	/// <summary>配送先国名</summary>
	public string ShippingCountryName { get; set; }
	/// <summary>配送先国ISOコード</summary>
	public string ShippingCountryIsoCode { get; set; }
	/// <summary>配送先企業名</summary>
	public string ShippingCompanyName { get; set; }
	/// <summary>配送先部署名</summary>
	public string ShippingCompanyPostName { get; set; }
	/// <summary>配送先電話番号</summary>
	public string ShippingTel1 { get; set; }
	/// <summary>クレジットカード枝番</summary>
	public string UserCreditCardBranchNo { get; set; }
	/// <summary>クレジットカード登録名</summary>
	public string UserCreditCardCartDispName { get; set; }
	/// <summary>クレジットカード会社名</summary>
	public string UserCreditCardCompanyName { get; set; }
	/// <summary>クレジットカード番号下四桁</summary>
	public string UserCreditCardLastFourDigit { get; set; }
	/// <summary>クレジットカード有効期限(月)</summary>
	public string UserCreditCardExpirationMonth { get; set; }
	/// <summary>クレジットカード有効期限(年)</summary>
	public string UserCreditCardExpirationYear { get; set; }
	/// <summary>クレジットカードカード名義</summary>
	public string UserCreditCardAuthorName { get; set; }
	/// <summary>Invoice No</summary>
	public string InvoiceNo { get; set; }
	/// <summary>Invoice Name</summary>
	public string InvoiceName { get; set; }
	/// <summary>Invoice Name</summary>
	public string UniformInvoiceInformation { get; set; }
	/// <summary>Invoice Name</summary>
	public string CarryTypeInformation { get; set; }
	/// <summary>Uniform Invoice Type Option 1</summary>
	public string UniformInvoiceTypeOption1 { get; set; }
	/// <summary>Uniform Invoice Type Option 2</summary>
	public string UniformInvoiceTypeOption2 { get; set; }
	/// <summary>楽天コンビニ前払い支払いコンビニ名称</summary>
	public string RakutenCvsTypeName { get; set; }
	/// <summary>楽天コンビニ前払い支払いコンビニ</summary>
	public string RakutenCvsType { get; set; }
	/// <summary>コンビニ前払い支払いコンビニ名称</summary>
	public string CvsTypeName { get; set; }
	/// <summary>コンビニ前払い支払いコンビニ</summary>
	public string CvsType { get; set; }
}