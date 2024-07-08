/*
=========================================================================================================
  Module      : リアル店舗在庫情報一覧ページ処理(RealShopProductStockList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.RealShop;

public partial class Form_RealShopProductStock_RealShopProductStockList : RealShopProductStockPage
{
	private const string REQUEST_KEY_INSERT_SUCCESS = "success";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable param = GetParameters();

			if (this.IsNotSearchDefault) return;

			// 不正パラメータが存在した場合
			if ((bool)param[Constants.ERROR_REQUEST_PRAMETER])
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// 検索情報保持(編集で利用)
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_REALSHOPPRODUCTSTOCK_SEARCH_INFO] = param;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable sqlParam = GetSearchSqlInfo(param);
			int currentPageNumber = (int)param[Constants.REQUEST_KEY_PAGE_NO];

			//------------------------------------------------------
			// リアル店舗商品在庫該当件数取得
			//------------------------------------------------------
			int totalRealShopProductStockCounts = 0;	// ページング可能総商品数
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("RealShopProductStock", "GetRealShopProductStockCount"))
			{
				DataView realShopProductStockCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
				if (realShopProductStockCount.Count != 0)
				{
					totalRealShopProductStockCounts = (int)realShopProductStockCount[0]["row_count"];
				}
			}

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool displayPager = true;
			StringBuilder errorMessage = new StringBuilder();
			// 上限件数より多い？
			if (totalRealShopProductStockCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				errorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(totalRealShopProductStockCounts));
				errorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				displayPager = false;
			}
			// 該当件数なし？
			else if (totalRealShopProductStockCounts == 0)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdListErrorMessage.InnerHtml = errorMessage.ToString();
			trListError.Visible = (errorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// リアル店舗商品在庫覧情報表示
			//------------------------------------------------------
			if (trListError.Visible == false)
			{
				// リアル店舗商品在庫覧情報取得
				DataView realShopProductStockList = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("RealShopProductStock", "GetRealShopProductStockList"))
				{
					sqlParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (currentPageNumber - 1) + 1);
					sqlParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * currentPageNumber);
					realShopProductStockList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
				}

				// Redirect to last page when current page no don't have any data
				CheckRedirectToLastPage(
					realShopProductStockList.Count,
					totalRealShopProductStockCounts,
					CreateRealShopProductStockListUrl(sqlParam));

				// データバインド
				rList.DataSource = realShopProductStockList;
				rList.DataBind();
			}
			btnStockUpdateTop.Visible = btnStockUpdateBottom.Visible = (rList.Items.Count > 0);

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (displayPager)
			{
				lbPager1.Text = WebPager.CreateDefaultListPager(totalRealShopProductStockCounts, currentPageNumber, CreateRealShopProductStockListUrl(param));
			}

			//------------------------------------------------------
			// エンターキーでのSubmitを無効とする領域を設定する
			// ※RepeaterがBindされていないと正常に動作しない
			//------------------------------------------------------
			// KeyEventをキャンセルするスクリプトを設定
			new InnerTextBoxList(divStockEdit).SetKeyPressEventCancelEnterKey();
			new InnerTextBoxList(divStockAddProduct).SetKeyPressEventCancelEnterKey();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 並び順
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOPPRODUCTSTOCK, Constants.REQUEST_KEY_SORT_KBN))
		{
			ddlSortKbn.Items.Add(li);
			if (li.Value == Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DEFAULT) li.Selected = true;
		}

		// 在庫数検索
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOPPRODUCTSTOCK, Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK + "_type"))
		{
			ddlSearchProductCountType.Items.Add(li);
			if (li.Value == Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_DEFAULT) li.Selected = true;
		}

		// リアル店舗名選択用
		var realShopAll = RealShop.GetRealShopAll();
		rShopIdAutoComplete.DataSource = realShopAll;
		rShopIdAutoComplete.DataBind();
		rShopNameAutoComplete.DataSource = realShopAll;
		rShopNameAutoComplete.DataBind();

		divStockRegistError.Visible = false;
		trRegistMessage.Visible = divStockRegistComplete.Visible = (StringUtility.ToEmpty(Request[REQUEST_KEY_INSERT_SUCCESS]) == "1");
	}

	/// <summary>
	/// リアル店舗商品在庫一覧パラメタ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	protected Hashtable GetParameters()
	{
		Hashtable result = new Hashtable();
		int currentPageNumber = 1;
		decimal searchStockCount = 0;
		bool paramError = false;

		try
		{
			// リアル店舗ID
			result.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID]));
			tbRealShopId.Text = Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID];
			// 店舗名
			result.Add(Constants.REQUEST_KEY_REALSHOP_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_NAME]));
			tbRealShopName.Text = Request[Constants.REQUEST_KEY_REALSHOP_NAME];
			// 商品ID
			result.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID]));
			tbProductId.Text = Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID];
			// 商品バリエーションID
			result.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID]));
			tbVariationId.Text = Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID];
			// 商品名
			result.Add(Constants.REQUEST_KEY_PRODUCT_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_NAME]));
			tbProductName.Text = Request[Constants.REQUEST_KEY_PRODUCT_NAME];
			// リアル店舗商品在庫数
			if (decimal.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK]), out searchStockCount) == false)
			{
				result.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, "");
				tbSearchStockCount.Text = "";
			}
			else
			{
				result.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK]));
				tbSearchStockCount.Text = Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK];
			}
			// 在庫数検索
			result.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE]));
			ddlSearchProductCountType.SelectedValue = Request[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE];
			// ソート区分
			string sortKbn = null;
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CHANGED_ASC:				// 更新日/昇順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CHANGED_DESC:			// 更新日/降順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CREATED_ASC:				// 作成日/昇順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CREATED_DESC:			// 作成日/降順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_ASC:						// 氏名/昇順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_DESC:					// 氏名/降順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_KANA_ASC:				// 氏名(かな)/昇順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_KANA_DESC:				// 氏名(かな)/降順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_PRODUCT_ID_ASC:				// 商品ID/昇順
				case Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_PRODUCT_ID_DESC:				// 商品ID/降順
					sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					sortKbn = Constants.KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DEFAULT;			// 商品ID/昇順がデフォルト
					break;
				default:
					paramError = true;
					break;
			}
			result.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);
			ddlSortKbn.SelectedValue = sortKbn;

			// ページ番号（ページャ動作時のみもちまわる）
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				currentPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			paramError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		result.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);
		result.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

		return result;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="search">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable search)
	{
		// 変数宣言
		Hashtable result = new Hashtable();

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		// リアル店舗ID
		result.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID]));
		// 店舗名
		result.Add("real_shop_" + Constants.FIELD_REALSHOP_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_NAME]));
		// 商品ID
		result.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID]));
		// ソート区分
		result.Add(Constants.REQUEST_KEY_SORT_KBN + "_kbn", StringUtility.ToEmpty(search[Constants.REQUEST_KEY_SORT_KBN]));
		// 商品バリエーションID
		result.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID]));
		// 商品名
		result.Add("product_" + Constants.FIELD_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_PRODUCT_NAME]));
		// リアル店舗商品在庫数
		result.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, StringUtility.ToEmpty(search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK]));
		// 在庫数検索
		result.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK + "_type", StringUtility.ToEmpty(search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE]));
		return result;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfo()
	{
		// 変数宣言
		Hashtable searchInfo = new Hashtable();
		searchInfo.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, tbRealShopId.Text.Trim());							// リアル店舗ID
		searchInfo.Add(Constants.REQUEST_KEY_REALSHOP_NAME, tbRealShopName.Text.Trim());											// 店舗名
		searchInfo.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID, tbProductId.Text.Trim());								// 商品ID
		searchInfo.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);													// ソート区分
		searchInfo.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID, tbVariationId.Text.Trim());							// 商品バリエーションID
		searchInfo.Add(Constants.REQUEST_KEY_PRODUCT_NAME, tbProductName.Text.Trim());												// 商品氏名
		searchInfo.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, tbSearchStockCount.Text.Trim());					// リアル店舗商品在庫数
		searchInfo.Add(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE, ddlSearchProductCountType.SelectedValue);	// 在庫数検索
		return searchInfo;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		Response.Redirect(CreateRealShopProductStockListUrl(GetSearchInfo(), 1));
	}

	/// <summary>
	/// このページの一括更新クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStockUpdate_Click(object sender, System.EventArgs e)
	{
		var result = new List<Hashtable>();

		// 在庫数入力チェック
		CheckInputStockNumber();

		for (var i = 0; i < rList.Items.Count; i++)
		{
			var input = new Hashtable();
			bool delete = ((CheckBox)(rList.Items[i].FindControl("cbDelete"))).Checked;
			var realShopId = ((HiddenField)(rList.Items[i].FindControl("hfRealShopId"))).Value;
			var productId = ((HiddenField)(rList.Items[i].FindControl("hfProductId"))).Value;
			var variationId = ((HiddenField)(rList.Items[i].FindControl("hfVariationId"))).Value;
			var stockNumberInput = ((TextBox)(rList.Items[i].FindControl("tbStockNumber"))).Text.Trim();
			decimal stockNumber;
			decimal.TryParse(stockNumberInput, out stockNumber);

			input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, realShopId);
			input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, productId);
			input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID, variationId);
			input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, stockNumber);

			// 削除？
			if (delete)
			{
				input["result"] = DeleteRealShopProductStock(realShopId, productId, variationId);
				result.Add(input);
			}
			// 更新？
			else if ((delete == false) && (stockNumber != 0))
			{
				input["result"] = UpdateRealShopProductStock(realShopId, productId, variationId, stockNumber);
				result.Add(input);
			}
		}
		if (result.Count > 0)
		{
			rListComplete.DataSource = result;
			rListComplete.DataBind();
			divStockComplete.Visible = true;
			divStockEdit.Visible = tbdyRegist.Visible = false;
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTSTOCK_TARGET_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 在庫数入力チェック
	/// </summary>
	private void CheckInputStockNumber()
	{
		StringBuilder errorMessages = new StringBuilder();
		for (var i = 0; i < rList.Items.Count; i++)
		{
			// 削除対象の場合、入力チェックを行わない
			if (((CheckBox)(rList.Items[i].FindControl("cbDelete"))).Checked) continue;

			var stockNumberInput = ((TextBox)(rList.Items[i].FindControl("tbStockNumber"))).Text.Trim();
			var productName = ((HiddenField)(rList.Items[i].FindControl("hfProductName"))).Value;
			if (Validator.IsHalfwidthDecimal(stockNumberInput) == false)
			{
				var input = new Hashtable();
				input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, stockNumberInput);
				errorMessages.Append(productName+"の" + Validator.Validate("RealShopProductStockModify", input));
			}
		}
		if (errorMessages.Length > 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// リアル店舗在庫更新
	/// </summary>
	/// <param name="realShopId">リアル店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">商品バリエーションID</param>
	/// <param name="stockNumber">在庫数</param>
	private bool UpdateRealShopProductStock(string realShopId, string productId, string variationId, decimal stockNumber)
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, realShopId);
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, productId);
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID, variationId);
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, stockNumber);
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_LAST_CHANGED, this.LoginOperatorName);
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("RealShopProductStock", "UpdateRealShopProductStock"))
		{
			return (sqlStatement.ExecStatementWithOC(sqlAccessor, input) > 0);
		}
	}

	/// <summary>
	/// リアル店舗在庫削除
	/// </summary>
	/// <param name="realShopId">リアル店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">商品バリエーションID</param>
	private bool DeleteRealShopProductStock(string realShopId, string productId, string variationId)
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, realShopId);
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, productId);
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID, variationId);
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("RealShopProductStock", "DeleteRealShopProductStock"))
		{
			return (sqlStatement.ExecStatementWithOC(sqlAccessor, input) > 0);
		}
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegist_Click(object sender, EventArgs e)
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, tbRealShopIdRegist.Text.Trim());
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, tbProductIdRegist.Text.Trim());
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID, (tbVariationIdRegist.Text.Trim() == string.Empty) ? tbProductIdRegist.Text.Trim() : tbProductIdRegist.Text.Trim() + tbVariationIdRegist.Text.Trim());
		input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, tbStockRegist.Text.Trim());
		string errorMessage = Validator.Validate("RealShopProductStockRegist", input);
		if (errorMessage.Length != 0)
		{
			lbMessageError.Text = errorMessage;
			divStockRegistError.Visible = trRegistMessage.Visible = true;
		}
		else if (CheckInputProductStockRegist() == false)
		{
			trRegistMessage.Visible = divStockRegistError.Visible = true;
			divStockRegistComplete.Visible = false;
		}
		else
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("RealShopProductStock", "InsertRealShopProductStock"))
			{
				var inserted = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
				if (inserted < 1) throw new Exception("登録0件です");
			}

			int pageno;
			int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageno);
			Response.Redirect(CreateRealShopProductStockListUrl(GetSearchInfo(), (pageno == 0) ? 1 : pageno) + "&" + REQUEST_KEY_INSERT_SUCCESS + "=1");
		}
	}

	/// <summary>
	/// 続けて編集を行うボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btRedirectEdit_Click(object sender, System.EventArgs e)
	{
		int pageno;
		int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageno);
		string url = CreateRealShopProductStockListUrl(GetSearchInfo(), (pageno == 0) ? 1 : pageno);

		// 在庫編集表示
		Response.Redirect(url);
	}

	/// <summary>
	/// 商品のデータ取得
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetInfo_Click(object sender, EventArgs e)
	{
		lbProductName.Text = hfProductName.Value;
	}

	/// <summary>
	/// 商品在庫登録入力チェック
	/// </summary>
	/// <returns>TRUE:正常 FALSE:エラー</returns>
	protected bool CheckInputProductStockRegist()
	{
		lbMessageError.Text = string.Empty;
		bool result = true;
		if (CheckValidRealShopId(tbRealShopIdRegist.Text.Trim()) == false)
		{
			lbMessageError.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REALSHOP_REAL_SHOP_ID_NOT_EXIST);
			result = false;
		}

		if (CheckExistProductAndVariationId(tbProductIdRegist.Text.Trim(), tbVariationIdRegist.Text.Trim()) == false)
		{
			lbMessageError.Text += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_OR_PRODUCTVARIATION_NOT_EXIST);
			result = false;
		}

		if (CheckExistRealShopProductStock(tbRealShopIdRegist.Text.Trim(), tbProductIdRegist.Text.Trim(), tbVariationIdRegist.Text.Trim()))
		{
			lbMessageError.Text += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REALSHOPPRODUCTSTOCK_EXIST);
			result = false;
		}
		return result;
	}

	/// <summary>
	/// リアル店舗ID存在かどうかチェック
	/// </summary>
	/// <param name="realShopId">リアル店舗ID</param>
	/// <returns>TRUE:正常 FALSE:エラー</returns>
	protected bool CheckValidRealShopId(string realShopId)
	{
		Hashtable sqlParam = new Hashtable();
		sqlParam.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, realShopId);
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("RealShop", "CheckValidRealShopId"))
		{
			DataView realShopIdCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
			return ((int)realShopIdCount[0]["count_row"] != 0);
		}
	}

	/// <summary>
	/// 商品ID存在かどうかチェック
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>TRUE:正常 FALSE:エラー</returns>
	protected bool CheckExistProductAndVariationId(string productId, string variationId)
	{
		Hashtable sqlParam = new Hashtable();
		sqlParam.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
		sqlParam.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, productId);
		sqlParam.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId);
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Product", "CheckValidProduct"))
		{
			DataView realProductIdCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
			return ((int)realProductIdCount[0]["count_row"] != 0);
		}
	}

	/// <summary>
	/// リアル店舗商品在庫存在かどうかチェックする
	/// </summary>
	/// <param name="realShopId">リアル店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>TRUE:正常 FALSE:エラー</returns>
	public bool CheckExistRealShopProductStock(string realShopId, string productId, string variationId)
	{
		//------------------------------------------------------
		// リアル店舗商品在庫該当件数取得
		//------------------------------------------------------
		int totalRealShopProductStockCounts = 0;	// ページング可能総商品数
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("RealShopProductStock", "CheckExistRealShopProductStock"))
		{
			Hashtable sqlParam = new Hashtable();
			sqlParam.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, realShopId);
			sqlParam.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, productId);
			sqlParam.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID, variationId);
			DataView realShopProductStockCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
			totalRealShopProductStockCounts = (int)realShopProductStockCount[0]["count"];
		}
		return (totalRealShopProductStockCounts != 0);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParameters());
	}
}