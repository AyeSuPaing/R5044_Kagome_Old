/*
=========================================================================================================
  Module      : リアル店舗商品在庫一覧出力コントローラ処理(BodyRealShopProductStockList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_Product_BodyRealShopProductStockList : ProductUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrRealShopList { get { return GetWrappedControl<WrappedRepeater>("rRealShopList"); } }
	# endregion

	// リアル店舗商品在庫情報の更新日時
	protected const string FIELD_REALSHOP_DATE_CHANGED_REAL_STOCK = Constants.FIELD_REALSHOP_DATE_CHANGED + "_real_stock";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		//------------------------------------------------------
		// リアル店舗商品在庫情報取得
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("RealShop", "GetRealShopList"))
		{
			// 店舗ID、商品ID、商品バリエーションID（URLパラメータから取得）
			Hashtable param = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, this.ShopId },
				{ Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, this.ProductId },
				{ Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID, this.VariationId },
				{ "in_stock", this.InStock ? "1" : "" }
			};

			// プロパティ指定
			StringBuilder where = new StringBuilder();
			if (string.IsNullOrEmpty(this.RealShopId) == false)
			{
				where.Append(GetWhereIn(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_REAL_SHOP_ID, this.RealShopId, param, sqlStatement));
			}
			else
			{
				where.Append("1 = 1");
			}
			if (where.Length > 0) where.Append(" AND ");
			if (string.IsNullOrEmpty(this.Zip) == false)
			{
				where.Append(GetWhereIn(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_ZIP, this.Zip, param, sqlStatement));
			}
			else
			{
				where.Append("1 = 1");
			}			
			if (where.Length > 0) where.Append(" AND ");
			if (string.IsNullOrEmpty(this.Addr1) == false)
			{
				where.Append(GetWhereIn(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_ADDR1, this.Addr1, param, sqlStatement));
			}
			else
			{
				where.Append("1 = 1");
			}
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@where@@", where.Length != 0 ? where.ToString() : "1 = 1");
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@order_by@@", GetSort());

			this.RealShop = sqlStatement.SelectSingleStatement(sqlAccessor, param);
		}

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		if (this.RealShop.Count != 0)
		{
			this.OldestDateChanged = (DateTime)this.RealShop.Table.Select("", FIELD_REALSHOP_DATE_CHANGED_REAL_STOCK + " ASC")[0][FIELD_REALSHOP_DATE_CHANGED_REAL_STOCK];
			this.NewestDateChanged = (DateTime)this.RealShop.Table.Select("", FIELD_REALSHOP_DATE_CHANGED_REAL_STOCK + " DESC")[0][FIELD_REALSHOP_DATE_CHANGED_REAL_STOCK];
			WrRealShopList.DataSource = this.RealShop;
			WrRealShopList.DataBind();
		}
		else
		{
			WrRealShopList.Visible = false;
		}		
	}

	/// <summary>
	/// リアル店舗説明取得(Text,Html判定）
	/// </summary>
	/// <param name="realShop">リアル店舗情報</param>
	/// <param name="field">フィールド名</param>
	/// <returns>リアル店舗説明</returns>
	protected string GetRealShopDataHtml(object realShop, string field)
	{
		string kbnField = null;
		switch (field)
		{
			case Constants.FIELD_REALSHOP_DESC1_PC:
				kbnField = Constants.FIELD_REALSHOP_DESC1_KBN_PC;
				break;
			case Constants.FIELD_REALSHOP_DESC2_PC:
				kbnField = Constants.FIELD_REALSHOP_DESC2_KBN_PC;
				break;
			case Constants.FIELD_REALSHOP_DESC1_SP:
				kbnField = Constants.FIELD_REALSHOP_DESC1_KBN_SP;
				break;
			case Constants.FIELD_REALSHOP_DESC2_SP:
				kbnField = Constants.FIELD_REALSHOP_DESC2_KBN_SP;
				break;
			case Constants.FIELD_REALSHOP_DESC1_MB:
				kbnField = Constants.FIELD_REALSHOP_DESC1_KBN_MB;
				break;
			case Constants.FIELD_REALSHOP_DESC2_MB:
				kbnField = Constants.FIELD_REALSHOP_DESC2_KBN_MB;
				break;
		}

		return ProductPage.GetProductDescHtml(
			(string)GetKeyValue(realShop, kbnField),
			(string)GetKeyValue(realShop, field));
	}

	/// <summary>
	/// 条件文（IN = 含む）取得
	/// </summary>
	/// <param name="table">テーブル名</param>
	/// <param name="field">フィールド名</param>
	/// <param name="valueString">値</param>
	/// <param name="param">パラメタ</param>
	/// <param name="sqlStatement">SQLステートメント</param>
	/// <returns>条件文（IN = 含む）</returns>
	private string GetWhereIn(string table, string field, string valueString, Hashtable param, SqlStatement sqlStatement)
	{
		StringBuilder result = new StringBuilder();

		if (string.IsNullOrEmpty(valueString) == false)
		{
			string[] values = valueString.Split(',');

			int i = 1;
			StringBuilder where = new StringBuilder();
			foreach (string value in values)
			{
				string key = field + "_" + i.ToString();
				param.Add(key, value);
				sqlStatement.AddInputParameters(key, SqlDbType.VarChar, 3000);

				if (i  > 1) where.Append(", ");
				where.Append("@" + key);

				i++;
			}
			result.Append(table).Append(".").Append(field).Append(" IN (").Append(where).Append(")");
		}

		return result.ToString();
	}

	/// <summary>
	/// 条件文（ソート）取得
	/// </summary>
	/// <returns>条件文（ソート）</returns>
	private string GetSort()
	{
		StringBuilder result = new StringBuilder();

		// ソート対象フィールド取得（デフォルト：表示順）
		string sort = Constants.FIELD_REALSHOP_DISPLAY_ORDER;
		switch (this.SortBy)
		{
			case Constants.FIELD_REALSHOP_REAL_SHOP_ID:
			case Constants.FIELD_REALSHOP_NAME:
			case Constants.FIELD_REALSHOP_NAME_KANA:
			case Constants.FIELD_REALSHOP_DESC1_KBN_PC:
			case Constants.FIELD_REALSHOP_DESC1_PC:
			case Constants.FIELD_REALSHOP_DESC2_KBN_PC:
			case Constants.FIELD_REALSHOP_DESC2_PC:
			case Constants.FIELD_REALSHOP_DESC1_KBN_SP:
			case Constants.FIELD_REALSHOP_DESC1_SP:
			case Constants.FIELD_REALSHOP_DESC2_KBN_SP:
			case Constants.FIELD_REALSHOP_DESC2_SP:
			case Constants.FIELD_REALSHOP_DESC1_KBN_MB:
			case Constants.FIELD_REALSHOP_DESC1_MB:
			case Constants.FIELD_REALSHOP_DESC2_KBN_MB:
			case Constants.FIELD_REALSHOP_DESC2_MB:
			case Constants.FIELD_REALSHOP_ZIP:
			case Constants.FIELD_REALSHOP_ZIP1:
			case Constants.FIELD_REALSHOP_ZIP2:
			case Constants.FIELD_REALSHOP_ADDR:
			case Constants.FIELD_REALSHOP_ADDR1:
			case Constants.FIELD_REALSHOP_ADDR2:
			case Constants.FIELD_REALSHOP_ADDR3:
			case Constants.FIELD_REALSHOP_ADDR4:
			case Constants.FIELD_REALSHOP_TEL:
			case Constants.FIELD_REALSHOP_TEL_1:
			case Constants.FIELD_REALSHOP_TEL_2:
			case Constants.FIELD_REALSHOP_TEL_3:
			case Constants.FIELD_REALSHOP_FAX:
			case Constants.FIELD_REALSHOP_FAX_1:
			case Constants.FIELD_REALSHOP_FAX_2:
			case Constants.FIELD_REALSHOP_FAX_3:
			case Constants.FIELD_REALSHOP_URL:
			case Constants.FIELD_REALSHOP_MAIL_ADDR:
			case Constants.FIELD_REALSHOP_OPENING_HOURS:
			case Constants.FIELD_REALSHOP_DISPLAY_ORDER:
			case Constants.FIELD_REALSHOP_VALID_FLG:
			case Constants.FIELD_REALSHOP_DEL_FLG:
			case Constants.FIELD_REALSHOP_DATE_CREATED:
			case Constants.FIELD_REALSHOP_DATE_CHANGED:
			case Constants.FIELD_REALSHOP_LAST_CHANGED:
				sort = this.SortBy;
				break;
		}

		// 昇順・降順取得（デフォルト：昇順）
		string sortType = (this.SortType.ToUpper() == "ASC" || this.SortType.ToUpper() == "DESC") ? this.SortType.ToUpper() : "ASC";

		return result.Append(Constants.TABLE_REALSHOP).Append(".").Append(sort).Append(" ").Append(sortType).ToString();
	}

	#region プロパティ

	/// <summary>リアル店舗商品在庫情報</summary>
	public DataView RealShop { get; private set; }
	/// <summary>店舗商品在庫情報の更新日時（最も古い）</summary>
	public DateTime OldestDateChanged { get; private set; }
	/// <summary>店舗商品在庫情報の更新日時（最も新しい）</summary>
	public DateTime NewestDateChanged { get; private set; }

	#region 抽出条件

	/// <summary>リアル店舗ID</summary>
	public string RealShopId
	{
		get { return m_realShopId; }
		set { m_realShopId = value; }
	}
	private string m_realShopId = string.Empty;
	/// <summary>郵便番号</summary>
	public string Zip
	{
		get { return m_zip; }
		set { m_zip = value; }
	}
	private string m_zip = string.Empty;
	/// <summary>都道府県</summary>
	public string Addr1
	{
		get { return m_addr1; }
		set { m_addr1 = value; }
	}
	private string m_addr1 = string.Empty;
	/// <summary>表示順序</summary>
	public string SortBy
	{
		get { return m_sortby; }
		set { m_sortby = value; }
	}
	private string m_sortby = Constants.FIELD_REALSHOP_DISPLAY_ORDER;
	/// <summary>昇順・降順</summary>
	public string SortType
	{
		get { return m_sortType; }
		set { m_sortType = value; }
	}
	private string m_sortType = "ASC";
	/// <summary>在庫有の店舗のみかどうか？</summary>
	public bool InStock
	{
		get { return m_inStock; }
		set { m_inStock = value; }
	}
	private bool m_inStock = false;

	#endregion

	#endregion
}