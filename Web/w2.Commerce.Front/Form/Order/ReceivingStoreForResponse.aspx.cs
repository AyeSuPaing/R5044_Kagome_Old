/*
=========================================================================================================
  Module      : ReceivingStoreForResponse (ReceivingStoreForResponse.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

public partial class Form_Order_ReceivingStoreForResponse : BasePage
{
	/// <summary>コンビニの種別</summary>
	private enum EStoreCategory
	{
		/// <summary>コンビニ種別 - OK</summary>
		[EnumTextName("TOK")]
		OK,
		/// <summary>コンビニ種別 - LIFE</summary>
		[EnumTextName("TLF")]
		LIFE,
		/// <summary>コンビニ種別 - FAMILY</summary>
		[EnumTextName("TFM")]
		FAMILY,
	}
	/// <summary>コンビニの前頭コード</summary>
	private enum EStoreCodePrefix
	{
		/// <summary>コンビニ前頭コード - OK</summary>
		[EnumTextName("K")]
		OK,
		/// <summary>コンビニ前頭コード - LIFE</summary>
		[EnumTextName("L")]
		LIFE,
		/// <summary>コンビニ前頭コード - FAMILY</summary>
		[EnumTextName("F")]
		FAMILY,
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.StoreCategory = Request.Form[Constants.RECEIVING_STORE_STORE_KEY_CATEGORY];
		this.StoreCode = CreateStoreCode(Request.Form[Constants.RECEIVING_STORE_STORE_KEY_CODE], this.StoreCategory);
		this.StoreName = Request.Form[Constants.RECEIVING_STORE_STORE_KEY_NAME];
		this.StoreAddr = Request.Form[Constants.RECEIVING_STORE_STORE_KEY_ADDR];
		this.StoreTel = Request.Form[Constants.RECEIVING_STORE_STORE_KEY_TEL];
	}

	/// <summary>
	/// コンビニの番号を、TwPelicanAllCvs.xmlに記載されてるの番号に合わせる
	/// </summary>
	/// <param name="srcStoreCode">コンビニ支店の番号</param>
	/// <param name="srcStoreCategory">コンビニの種別</param>
	private string CreateStoreCode(string srcStoreCode, string srcStoreCategory)
	{
		var prefix = string.Empty;

		// コンビニ前頭に種類のコードを判断する
		if (srcStoreCategory == EStoreCategory.OK.ToText())
		{
			prefix = EStoreCodePrefix.OK.ToText();
		}
		else if (srcStoreCategory == EStoreCategory.LIFE.ToText())
		{
			prefix = EStoreCodePrefix.LIFE.ToText();
		}
		else if (srcStoreCategory == EStoreCategory.FAMILY.ToText())
		{
			prefix = EStoreCodePrefix.FAMILY.ToText();
		}

		var storeCode = prefix + srcStoreCode.PadLeft(6, '0');
		return storeCode;
	}

	/// <summary>コンビニの種別</summary>
	protected string StoreCategory { get; set; }
	/// <summary>コンビニ支店の番号</summary>
	protected string StoreCode { get; set; }
	/// <summary>コンビニ支店の名前</summary>
	protected string StoreName { get; set; }
	/// <summary>コンビニ支店の住所</summary>
	protected string StoreAddr { get; set; }
	/// <summary>コンビニ支店の電話</summary>
	protected string StoreTel { get; set; }
}