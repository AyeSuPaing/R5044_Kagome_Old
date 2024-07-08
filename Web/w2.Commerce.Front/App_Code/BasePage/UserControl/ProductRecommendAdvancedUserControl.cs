/*
=========================================================================================================
  Module      : おすすめ商品ユーザコントロール(ProductRecommendAdvancedUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/// <summary>
/// BodyProductRecommendAdvanced の概要の説明です
/// </summary>
public class ProductRecommendAdvancedUserControl : ProductRecommendBaseUserControl
{
	/// <summary>誤設定</summary>
	private bool m_settingNotCorrect = false;

	/// <summary>
	/// パラメタ取得
	/// </summary>
	protected override void GetParams()
	{
		// 対象カテゴリ決定（リクエストカテゴリ利用なしであればascx上で指定したものを利用）
		if (this.UseCategory)
		{
			var val = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CATEGORY_ID]);
			this.CategoryId = (val.Length > this.CategoryIdLength) ? val.Substring(0, this.CategoryIdLength) : val;
		}
	}

	/// <summary>
	/// キャッシュキー取得
	/// </summary>
	/// <returns>キャッシュキー</returns>
	protected override string GetCacheKey()
	{
		return string.Format("Campaign='{0}';BrandId='{1}';CategoryId='{2}';MemberRankId='{3}';UserFixedPurchaseMemberFlg='{4}';", this.CampaignIcons, this.BrandId, this.CategoryId, this.MemberRankId, this.UserFixedPurchaseMemberFlg)
			+ string.Format("ProductTag_1='{0}';ProductTag_2='{1}';ProductTag_3='{2}';ProductTag_4='{3}';ProductTag_5='{4}';SellTimeFrom='{5}';SellTimeTo='{6}';", this.ProductTag_1, this.ProductTag_2, this.ProductTag_3, this.ProductTag_4, this.ProductTag_5, this.SellTimeFrom, this.SellTimeTo).GetHashCode();
	}

	/// <summary>
	/// キャッシング時間を取得（分。「0」はキャッシングしない）
	/// </summary>
	/// <returns>キャッシング時間</returns>
	protected override int GetCacheExpireMinutes()
	{
		return Constants.PRODUCT_RECOMMEND_ADVANCED_CACHE_EXPIRE_MINUTES;
	}

	/// <summary>
	/// DBからデータ取得（おすすめ商品（詳細設定可）取得）
	/// </summary>
	/// <returns>おすすめ商品</returns>
	protected override DataView GetDatasFromDbInner()
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ProductDisplay", "GetProductRecommendAdvanced"))
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DISPPRODUCTINFO_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID},
				{Constants.FIELD_PRODUCTBRAND_BRAND_ID, this.BrandId},
				{Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, this.CategoryId},
				{Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId},
				{Constants.KEY_SHOW_OUT_OF_STOCK_ITEMS, this.ShowOutOfStock ? Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_VALID : Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_INVALID},
				{Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg},
			};
			SetSellTimeConditions(ht);
			SetCampaignIconConditions(ht);
			SetProductTagConditions(statement, ht);
			statement.Statement = statement.Statement.Replace("@@orderby@@", GetSortExpression(this.DbSortBy, this.DbSortType, "RAND()"));
			var dv = m_settingNotCorrect ? (new DataTable()).DefaultView : statement.SelectSingleStatement(accessor, ht);
			return dv;
		}
	}

	/// <summary>
	/// ソート（商品対象リストをランダムに並び替え）
	/// </summary>
	/// <param name="products">商品リスト</param>
	/// <returns>商品リスト</returns>
	/// <remarks>
	/// 共有リソースのDataViewを書き換えるのではなく、複製したものを書き換えたい
	/// </remarks>
	protected override DataView SortProducts(DataView products)
	{
		var sortExpression = GetSortExpression(this.SortBy, this.SortType);
		var selectTopRow = this.MaxDispCount;

		products.Table.AcceptChanges();
		var productListTable = products.Table;
		if (productListTable.Rows.Count > 0)
		{
			if (selectTopRow > 0)
			{
				// 並べ替え指定あり？
				if (string.IsNullOrEmpty(sortExpression) == false)
				{
					// 指定キー並び替え
					productListTable.DefaultView.Sort = sortExpression;
				}
				else
				{
					// ランダム並び替え
					productListTable = products.Table.AsEnumerable().OrderBy(index => Guid.NewGuid()).Cast<DataRow>().Take(selectTopRow).CopyToDataTable();
				}
				productListTable = productListTable.DefaultView.ToTable();
			}
			else
			{
				productListTable = products.Table.Clone();
			}
		}
		return productListTable.DefaultView;
	}

	/// <summary>
	/// 販売時期
	/// </summary>
	/// <param name="outParams"></param>
	private void SetSellTimeConditions(Hashtable outParams)
	{
		if (m_settingNotCorrect)
		{
			return;
		}
		outParams.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_f", DBNull.Value);
		outParams.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_t", DBNull.Value);
		outParams.Add(Constants.FIELD_PRODUCT_SELL_TO + "_f", DBNull.Value);
		outParams.Add(Constants.FIELD_PRODUCT_SELL_TO + "_t", DBNull.Value);

		// 販売時期 (特別)
		var dateFormatLong = "yyyyMMddHHmmss";
		var dateFormatShort = "yyyyMMdd";
		var dateTemplateLong = "{0}000000";
		var sellTimeFromSetting = SellTimeFrom.Trim();
		var sellTimeToSetting = SellTimeTo.Trim();
		var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
		var firstDayOfPreviousMonth = firstDayOfMonth.AddMonths(-1);
		var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);
		var firstDayOfWeek = DateTime.Now.AddDays(-(Convert.ToInt32(DateTime.Now.DayOfWeek)));
		var lastDayOfWeek = firstDayOfWeek.AddDays(6);
		var firstDayPreviousWeek = firstDayOfWeek.AddDays(-7);
		var lastDayPreviousWeek = firstDayOfWeek.AddDays(-1);
		var settingThisMonth = string.Format("{0}-{1}", firstDayOfMonth.ToString(dateFormatShort), lastDayOfMonth.ToString(dateFormatShort));
		var settingPrevMonth = string.Format("{0}-{1}", firstDayOfPreviousMonth.ToString(dateFormatShort), lastDayOfPreviousMonth.ToString(dateFormatShort));
		var settingThisWeek = string.Format("{0}-{1}", firstDayOfWeek.ToString(dateFormatShort), lastDayOfWeek.ToString(dateFormatShort));
		var settingPrevWeek = string.Format("{0}-{1}", firstDayPreviousWeek.ToString(dateFormatShort), lastDayPreviousWeek.ToString(dateFormatShort));
		switch (sellTimeFromSetting)
		{
			case Constants.SETTING_DATE_THISMONTH:
				sellTimeFromSetting = settingThisMonth;
				break;

			case Constants.SETTING_DATE_PREVIOUSMONTH:
				sellTimeFromSetting = settingPrevMonth;
				break;

			case Constants.SETTING_DATE_THISWEEK:
				sellTimeFromSetting = settingThisWeek;
				break;

			case Constants.SETTING_DATE_PREVIOUSWEEK:
				sellTimeFromSetting = settingPrevWeek;
				break;
		}
		switch (sellTimeToSetting)
		{
			case Constants.SETTING_DATE_THISMONTH:
				sellTimeToSetting = settingThisMonth;
				break;

			case Constants.SETTING_DATE_PREVIOUSMONTH:
				sellTimeToSetting = settingPrevMonth;
				break;

			case Constants.SETTING_DATE_THISWEEK:
				sellTimeToSetting = settingThisWeek;
				break;

			case Constants.SETTING_DATE_PREVIOUSWEEK:
				sellTimeToSetting = settingPrevWeek;
				break;
		}

		// 販売時期 (From)
		if ((sellTimeFromSetting.IndexOf("-", StringComparison.Ordinal) >= 0) || (string.IsNullOrEmpty(sellTimeFromSetting) == false))
		{
			var sellFromPeriods = sellTimeFromSetting.Split('-');
			try
			{
				if (string.IsNullOrEmpty(sellFromPeriods[0].Trim()) == false)
				{
					outParams[Constants.FIELD_PRODUCT_SELL_FROM + "_f"] = DateTime.ParseExact(string.Format(dateTemplateLong, sellFromPeriods[0]), dateFormatLong, null);
				}
				if (string.IsNullOrEmpty(sellFromPeriods[1].Trim()) == false)
				{
					outParams[Constants.FIELD_PRODUCT_SELL_FROM + "_t"] = DateTime.ParseExact(string.Format(dateTemplateLong, sellFromPeriods[1]), dateFormatLong, null).AddDays(1);
				}
			}
			catch
			{
				m_settingNotCorrect = true;
			}
		}

		//販売時期 (To)
		if ((sellTimeToSetting.IndexOf("-", StringComparison.Ordinal) >= 0) || (string.IsNullOrEmpty(sellTimeToSetting) == false))
		{
			var sellToPeriods = sellTimeToSetting.Split('-');
			try
			{
				if (string.IsNullOrEmpty(sellToPeriods[0].Trim()) == false)
				{
					outParams[Constants.FIELD_PRODUCT_SELL_TO + "_f"] = DateTime.ParseExact(string.Format(dateTemplateLong, sellToPeriods[0]), dateFormatLong, null);
				}
				if (string.IsNullOrEmpty(sellToPeriods[1].Trim()) == false)
				{
					outParams[Constants.FIELD_PRODUCT_SELL_TO + "_t"] = DateTime.ParseExact(string.Format(dateTemplateLong, sellToPeriods[1]), dateFormatLong, null).AddDays(1);
				}
			}
			catch
			{
				m_settingNotCorrect = true;
			}
		}
	}

	/// <summary>
	/// 表示区分
	/// </summary>
	/// <param name="outParams"></param>
	private void SetCampaignIconConditions(Hashtable outParams)
	{
		if (m_settingNotCorrect)
		{
			return;
		}
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG1, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG2, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG3, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG4, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG5, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG6, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG7, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG8, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG9, string.Empty);
		outParams.Add(Constants.FIELD_PRODUCT_ICON_FLG10, string.Empty);
		if (string.IsNullOrEmpty(this.CampaignIcons) == false)
		{
			m_settingNotCorrect = true;
			foreach (var campaignIcon in this.CampaignIcons.Split(','))
			{
				if (campaignIcon.StartsWith("IC"))
				{
					string fieldIcon = string.Format("icon_flg{0}", campaignIcon.Trim().Substring(2));
					var iconFlags = new List<string>
						{
							Constants.FIELD_PRODUCT_ICON_FLG1,
							Constants.FIELD_PRODUCT_ICON_FLG2,
							Constants.FIELD_PRODUCT_ICON_FLG3,
							Constants.FIELD_PRODUCT_ICON_FLG4,
							Constants.FIELD_PRODUCT_ICON_FLG5,
							Constants.FIELD_PRODUCT_ICON_FLG6,
							Constants.FIELD_PRODUCT_ICON_FLG7,
							Constants.FIELD_PRODUCT_ICON_FLG8,
							Constants.FIELD_PRODUCT_ICON_FLG9,
							Constants.FIELD_PRODUCT_ICON_FLG10
						};
					if (iconFlags.Contains(fieldIcon))
					{
						outParams[fieldIcon] = "1";
						m_settingNotCorrect = false;
					}
				}
			}
		}
	}

	/// <summary>
	/// 商品タグ 1～ 5
	/// </summary>
	/// <param name="sqlStatement"> </param>
	/// <param name="outParams"> </param>
	private void SetProductTagConditions(SqlStatement sqlStatement, Hashtable outParams)
	{
		if (m_settingNotCorrect)
		{
			return;
		}
		var dynamicSqlProductTagWhere = string.Empty;
		var listCorrectTag = GetProductTagSetting();
		var listProductTagSetting = new List<string> 
		{
			this.ProductTag_1,
			this.ProductTag_2,
			this.ProductTag_3,
			this.ProductTag_4,
			this.ProductTag_5
		};
		const string sqlTagFieldTemplate = " (@{0} <> '' AND {1}.{2} = @{0}) OR";
		const string sqlTagSettingTemplate = " AND ( {0} )";
		const string dynamicProductTagWhere = "@@DYNAMIC_PRODUCT_TAG_WHERE@@";
		var settingNumber = 0;
		foreach (var tagSetting in listProductTagSetting)
		{
			settingNumber++;
			if (string.IsNullOrEmpty(tagSetting.Trim()))
			{
				continue;
			}
			else if (tagSetting.IndexOf("=", StringComparison.Ordinal) < 0)
			{
				m_settingNotCorrect = true;
				break;
			}
			var sqlTagSetting = string.Empty;
			m_settingNotCorrect = true;
			foreach (var tagField in tagSetting.Split(','))
			{
				if (tagField.IndexOf("=", StringComparison.Ordinal) < 0)
				{
					continue;
				}
				var sqlTagField = string.Empty;
				var productTagName = tagField.Split('=')[0].Trim();
				var productTagValue = tagField.Split('=')[1].Trim();
				var sqlParamKey = productTagName + "_" + settingNumber;
				while (outParams[sqlParamKey] != null)
				{
					sqlParamKey += "_" + settingNumber;
				}
				if (listCorrectTag.Contains(productTagName) == false)
				{
					continue;
				}
				sqlTagField = string.Format(sqlTagFieldTemplate, sqlParamKey, Constants.TABLE_PRODUCTTAG, productTagName);
				outParams[sqlParamKey] = productTagValue;
				sqlStatement.AddInputParameters(sqlParamKey, SqlDbType.NVarChar, 100);
				sqlTagSetting += sqlTagField;
				m_settingNotCorrect = false;
			}
			if (sqlTagSetting.EndsWith("OR"))
			{
				dynamicSqlProductTagWhere += Environment.NewLine + string.Format(sqlTagSettingTemplate, sqlTagSetting.Substring(0, sqlTagSetting.Length - 2));
			}
		}
		sqlStatement.Statement = sqlStatement.Statement.Replace(dynamicProductTagWhere, dynamicSqlProductTagWhere);
	}

	/// <summary>
	/// 表示順序
	/// </summary>
	/// <returns></returns>
	private string GetSortExpression(string sortBy, string sortType, string defaultSort = "")
	{
		string sortbyResult = string.Empty;
		string sortTypeValue = sortType.ToUpper() == "DESC" ? "DESC" : "ASC";
		var uniqueList = new List<string>();
		foreach (string item in sortBy.Split(','))
		{
			// 一意の列を並べ替える
			if (uniqueList.Contains(item.Trim()))
			{
				continue;
			}
			uniqueList.Add(item.Trim());

			switch (item.Trim())
			{
				case Constants.FIELD_PRODUCT_SELL_FROM:
				case Constants.FIELD_PRODUCT_PRODUCT_ID:
				case Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID:
					sortbyResult += string.Format("{0} {1},", item.Trim(), sortTypeValue);
					break;

				case "product_" + Constants.FIELD_PRODUCT_NAME:
				case "product_" + Constants.FIELD_PRODUCT_NAME_KANA:
					sortbyResult += string.Format("{0} {1},", item.Trim().Replace("product_", string.Empty), sortTypeValue);
					break;

				case "category_" + Constants.FIELD_PRODUCTCATEGORY_NAME:
				case "category_" + Constants.FIELD_PRODUCTCATEGORY_NAME_KANA:
					sortbyResult += string.Format("{0}_{1} {2},", Constants.TABLE_PRODUCTCATEGORY, item.Trim().Replace("category_", string.Empty), sortTypeValue);
					break;
			}
		}
		sortbyResult = string.IsNullOrEmpty(sortbyResult) ? defaultSort : sortbyResult.Substring(0, sortbyResult.Length - 1);
		return sortbyResult;
	}

	/// <summary>
	/// 商品タグ設定取得 
	/// </summary>
	/// <returns></returns>
	private List<string> GetProductTagSetting()
	{
		var productTags = new List<string>();
		DataView productSettings = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductTag", "GetProductTagSetting"))
		{
			productSettings = sqlStatement.SelectSingleStatement(sqlAccessor);
		}
		foreach (DataRowView rowView in productSettings)
		{
			productTags.Add(rowView[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID].ToString());
		}
		return productTags;
	}

	#region プロパティ
	/// <summary>商品カテゴリー</summary>
	/// <remarks>
	/// UserControlの呼び出し元から指定できるようにするため、
	/// Page_Initで行うGetPramaetersの影響を受けないようnewで新たに定義する
	/// </remarks>
	public new string CategoryId
	{
		get { return m_categoryId; }
		set { m_categoryId = value; }
	}
	private string m_categoryId = string.Empty;
	/// <summary>商品タグ 1～ 5</summary>
	public string ProductTag_1
	{
		get { return m_productTag_1; }
		set { m_productTag_1 = value; }
	}
	private string m_productTag_1 = string.Empty;
	/// <summary>商品タグ 1～ 5</summary>
	public string ProductTag_2
	{
		get { return m_productTag_2; }
		set { m_productTag_2 = value; }
	}
	private string m_productTag_2 = string.Empty;
	/// <summary>商品タグ 1～ 5</summary>
	public string ProductTag_3
	{
		get { return m_productTag_3; }
		set { m_productTag_3 = value; }
	}
	private string m_productTag_3 = string.Empty;
	/// <summary>商品タグ 1～ 5</summary>
	public string ProductTag_4
	{
		get { return m_productTag_4; }
		set { m_productTag_4 = value; }
	}
	private string m_productTag_4 = string.Empty;
	/// <summary>商品タグ 1～ 5</summary>
	public string ProductTag_5
	{
		get { return m_productTag_5; }
		set { m_productTag_5 = value; }
	}
	private string m_productTag_5 = string.Empty;
	/// <summary>販売時期</summary>
	public string SellTimeFrom
	{
		get { return m_sellTimeFrom; }
		set { m_sellTimeFrom = value; }
	}
	private string m_sellTimeFrom = string.Empty;
	/// <summary>販売時期</summary>
	public string SellTimeTo
	{
		get { return m_sellTimeTo; }
		set { m_sellTimeTo = value; }
	}
	private string m_sellTimeTo = string.Empty;
	/// <summary>表示順序</summary>
	public string SortBy
	{
		get { return m_sortby; }
		set { m_sortby = value; }
	}
	private string m_sortby = string.Empty;
	/// <summary>昇順・降順</summary>
	public string SortType
	{
		get { return m_sortType; }
		set { m_sortType = value; }
	}
	private string m_sortType = string.Empty;
	/// <summary> 表示区分</summary>
	public string CampaignIcons
	{
		get { return m_campaignIcons; }
		set { m_campaignIcons = value; }
	}
	private string m_campaignIcons = string.Empty;
	/// <summary>DB表示順序</summary>
	public string DbSortBy
	{
		get { return m_db_sortby; }
		set { m_db_sortby = value; }
	}
	private string m_db_sortby = string.Empty;
	/// <summary>DB昇順・降順</summary>
	public string DbSortType
	{
		get { return m_db_sortType; }
		set { m_db_sortType = value; }
	}
	private string m_db_sortType = string.Empty;
	#endregion
}
