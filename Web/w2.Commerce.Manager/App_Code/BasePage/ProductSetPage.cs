/*
=========================================================================================================
  Module      : 商品セット共通ページ(ProductSetPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// ProductSetPage の概要の説明です
/// </summary>
public class ProductSetPage : ProductPage
{
	/// <summary>
	/// 商品セット取得
	/// </summary>
	/// <param name="strProductSetId">商品セットID</param>
	/// <returns>商品セット</returns>
	protected Hashtable GetProductSet(string strProductSetId)
	{
		Hashtable htResult = new Hashtable();

		DataView dvProductSet = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "GetProductSet"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSET_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID, strProductSetId);

			dvProductSet = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		if (dvProductSet.Count != 0)
		{
			foreach (DataColumn dc in dvProductSet.Table.Columns)
			{
				htResult.Add(dc.ColumnName, dvProductSet[0][dc.ColumnName]);
			}
		}

		return htResult;
	}

	/// <summary>
	/// 商品セットアイテム取得
	/// </summary>
	/// <param name="strProductSetId">商品セットID</param>
	/// <returns>商品セット</returns>
	protected List<ProductSetItem> GetProductSetItem(string strProductSetId)
	{
		List<ProductSetItem> ltResult = new List<ProductSetItem>();

		DataView dvProductSetItem = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "GetProductSetItem"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSET_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID, strProductSetId);

			dvProductSetItem = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		foreach (DataRowView drvProductSetItem in dvProductSetItem)
		{
			string strProductId = (string)drvProductSetItem[Constants.FIELD_PRODUCTSETITEM_PRODUCT_ID];
			string strVId = ((string)drvProductSetItem[Constants.FIELD_PRODUCTSETITEM_VARIATION_ID]).Substring(strProductId.Length);

			ProductSetItem psi = new ProductSetItem(
				strProductId,
				strVId,
				CreateProductAndVariationName(drvProductSetItem),
				StringUtility.ToEmpty(drvProductSetItem[Constants.FIELD_PRODUCTVARIATION_PRICE]),
				StringUtility.ToEmpty(drvProductSetItem[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN]),
				StringUtility.ToEmpty(drvProductSetItem[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX]),
				StringUtility.ToEmpty(drvProductSetItem[Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE]),
				StringUtility.ToEmpty(drvProductSetItem[Constants.FIELD_PRODUCTSETITEM_FAMILY_FLG]),
				StringUtility.ToEmpty(drvProductSetItem[Constants.FIELD_PRODUCTSETITEM_DISPLAY_ORDER]));

			ltResult.Add(psi);
		}

		return ltResult;
	}

	///**************************************************************************************
	/// <summary>
	/// セット商品アイテムクラス
	/// </summary>
	///**************************************************************************************
	[Serializable]
	public class ProductSetItem
	{
		string m_strProductId = null;
		string m_strVId = null;
		string m_strName = null;
		string m_strPrice = null;
		string m_strSetItemPrice = null;
		string m_strCountMin = null;
		string m_strCountMax = null;
		string m_strFamilyFlg = null;
		string m_strDisplayOrder = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strProductId"></param>
		/// <param name="strVId"></param>
		/// <param name="strName"></param>
		/// <param name="strPrice"></param>
		/// <param name="strCountMin"></param>
		/// <param name="strCountMax"></param>
		/// <param name="strSetItemPrice"></param>
		/// <param name="strFamilyFlg"></param>
		/// <param name="strDisplayOrder"></param>
		public ProductSetItem(
			string strProductId,
			string strVId,
			string strName,
			string strPrice,
			string strCountMin,
			string strCountMax,
			string strSetItemPrice,
			string strFamilyFlg,
			string strDisplayOrder
			)
		{
			m_strProductId = strProductId;
			m_strVId = strVId;
			m_strName = strName;
			m_strPrice = strPrice;
			m_strCountMin = strCountMin;
			m_strCountMax = strCountMax;
			m_strSetItemPrice = strSetItemPrice;
			m_strFamilyFlg = strFamilyFlg;
			m_strDisplayOrder = strDisplayOrder;
		}

		/// <summary>
		/// 登録用ハッシュテーブル取得
		/// </summary>
		/// <returns></returns>
		public Hashtable GetHashtable()
		{
			Hashtable htInputProduct = new Hashtable();
			htInputProduct.Add(Constants.FIELD_PRODUCTSETITEM_PRODUCT_ID, this.ProductId);
			htInputProduct.Add(Constants.FIELD_PRODUCTSETITEM_VARIATION_ID, this.ProductId + this.VId);
			htInputProduct.Add(Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE, this.SetItemPrice);
			htInputProduct.Add(Constants.FIELD_PRODUCTSETITEM_COUNT_MIN, this.CountMin);
			htInputProduct.Add(Constants.FIELD_PRODUCTSETITEM_COUNT_MAX, this.CountMax);
			htInputProduct.Add(Constants.FIELD_PRODUCTSETITEM_FAMILY_FLG, this.FamilyFlg);
			htInputProduct.Add(Constants.FIELD_PRODUCTSETITEM_DISPLAY_ORDER, this.DisplayOrder);
			htInputProduct.Add("name", this.Name);	// エラーメッセージ表示用に使用

			return htInputProduct;
		}

		/// <summary>プロパティ：商品ID</summary>
		public string ProductId
		{
			get { return m_strProductId; }
		}
		/// <summary>プロパティ：バリエーションID</summary>
		public string VId
		{
			get { return m_strVId; }
		}
		/// <summary>プロパティ：商品名</summary>
		public string Name
		{
			get { return m_strName; }
		}
		/// <summary>プロパティ：商品価格</summary>
		public string Price
		{
			get { return m_strPrice; }
		}
		/// <summary>プロパティ：商品セット価格</summary>
		public string SetItemPrice
		{
			get { return m_strSetItemPrice; }
			set { m_strSetItemPrice = value; }
		}
		/// <summary>プロパティ：商品数下限</summary>
		public string CountMin
		{
			get { return m_strCountMin; }
			set { m_strCountMin = value; }
		}
		/// <summary>プロパティ：商品数上限</summary>
		public string CountMax
		{
			get { return m_strCountMax; }
			set { m_strCountMax = value; }
		}
		/// <summary>プロパティ：親子フラグ</summary>
		public string FamilyFlg
		{
			get { return m_strFamilyFlg; }
			set { m_strFamilyFlg = value; }
		}
		/// <summary>プロパティ：表示順</summary>
		public string DisplayOrder
		{
			get { return m_strDisplayOrder; }
			set { m_strDisplayOrder = value; }
		}
	}
}
