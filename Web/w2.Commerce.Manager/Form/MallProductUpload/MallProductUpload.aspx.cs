/*
=========================================================================================================
  Module      : モール連携 商品アップロードページ処理(MallProductUpload.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using w2.App.Common.Util;
using System.Collections.Generic;

public partial class Form_MallProductUpload_MallProductUpload : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent();

			//------------------------------------------------------
			// 商品/バリエーションの最終登録/更新ログを取得＆画面セット
			//------------------------------------------------------
			DataView dvLastInsertLog = null;
			DataView dvLastUpdateLog = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationUpdateLog", "GetLastLog"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID, this.LoginOperatorShopId);

				DataSet dsLastLog = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
				dvLastInsertLog = dsLastLog.Tables[0].DefaultView;
				dvLastUpdateLog = dsLastLog.Tables[1].DefaultView;
			}

			if (dvLastInsertLog.Count != 0)
			{
				ucProductInsertDatePeriod.SetStartDate((DateTime)dvLastInsertLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED]);
			}

			if (dvLastUpdateLog.Count != 0)
			{
				ucProductUpdateDatePeriod.SetStartDate((DateTime)dvLastUpdateLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED]);
			}

			//------------------------------------------------------
			// 検索実行
			//------------------------------------------------------
			ProductSearch(sender, e);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		this.ValidNextEngine = false;
		// モール出品先
		DataView dvMallCooperationSettings = MallPage.GetMallCooperationSettingAll(this.LoginOperatorShopId);
		foreach (DataRowView drv in dvMallCooperationSettings)
		{
			// 有効フラグOFFまたはモール区分=Amazon・Facebook・Lohaco・＆mallであれば表示しない
			if ((drv[Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG].ToString() != Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)
				|| (drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN].ToString() == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON)
				|| (drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN].ToString() == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO)
				|| (drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN].ToString() == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL)
				|| (drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN].ToString() == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK))
			{
				continue;
			}

			var item = new ListItem((string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME], (string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]);
			item.Selected = true;
			cblProductInsertMalls.Items.Add(item);

			if (drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN].ToString() == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE)
			{
				this.ValidNextEngine = true;
				continue;
			}

			cblProductUpdateMalls.Items.Add(item);
		}
	}

	/// <summary>
	/// 商品検索
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ProductSearch(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 商品のアップロード対象件数取得
		//------------------------------------------------------
		DataView dvProductInsert = null;
		DataView dvProductUpdate = null;
		Hashtable htInput = GetSearchInfo();
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallCooperationUpdateLog", "GetTarget"))
		{
			DataSet dsTarget = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
			dvProductInsert = dsTarget.Tables[0].DefaultView;
			dvProductUpdate = dsTarget.Tables[1].DefaultView;
		}

		//------------------------------------------------------
		// 画面表示
		//------------------------------------------------------
		lProductInsert.Text = StringUtility.ToNumeric(dvProductInsert[0][0]);
		lProductUpdate.Text = StringUtility.ToNumeric(dvProductUpdate[0][0]);

		btnSendInsertData.Enabled = ((int)dvProductInsert[0][0] > 0);
		btnSendUpdateData.Enabled = ((int)dvProductUpdate[0][0] > 0);

		tbdyList.Visible = true;

		//------------------------------------------------------
		// ポップアップ商品一覧用に検索条件をセッションに格納
		//------------------------------------------------------
		Session[Constants.SESSIONPARAM_KEY_MALLPRODUCTUPLOAD_SEARCH_INFO] = htInput;
	}

	/// <summary>
	/// 新規登録分送信
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendInsertData_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// モール連携更新ログ登録
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallCooperationUpdateLog", "CreateProductInsertLog"))
		{
			Hashtable htInput = GetSearchInfo();
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ mall_ids @@", "'" + String.Join("','", GetCheckedMallIds(cblProductInsertMalls)) + "'"); // 送信先モールIDの絞り込み

			int iUpdate = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		//------------------------------------------------------
		// 画面遷移(元のページへ）
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_PRODUCT_UPLOAD);
	}

	/// <summary>
	/// 更新分送信
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendUpdateData_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// モール連携更新ログ登録
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallCooperationUpdateLog", "CreateProductUpdateLog"))
		{
			Hashtable htInput = GetSearchInfo();
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ mall_ids @@", "'" + String.Join("','", GetCheckedMallIds(cblProductUpdateMalls)) + "'"); // 送信先モールIDの絞り込み

			int iUpdate = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		//------------------------------------------------------
		// 画面遷移(元のページへ）
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_PRODUCT_UPLOAD);
	}

	/// <summary>
	/// チェック値取得
	/// </summary>
	/// <param name="cbl">チェックボックスリスト</param>
	/// <returns>チェックされた値の集合</returns>
	private List<string> GetCheckedMallIds(CheckBoxList cbl)
	{
		List<string> mallIds = new List<string>();
		foreach (ListItem liMallExhibitsConfig in cbl.Items)
		{
			if (liMallExhibitsConfig.Selected) mallIds.Add(liMallExhibitsConfig.Value);
		}
		return mallIds;
	}

	/// <summary>
	/// 検索用Hashtable取得
	/// </summary>
	/// <returns></returns>
	private Hashtable GetSearchInfo()
	{
		Hashtable htResult = new Hashtable();

		//------------------------------------------------------
		// 日付取得
		//------------------------------------------------------
		// 末日補正処理
		if (string.IsNullOrEmpty(ucProductInsertDatePeriod.StartDateString) == false)
		{
			var productInsertDateStart = DateTime.Parse(ucProductInsertDatePeriod.StartDateString);
			if (DateTimeUtility.IsLastDayOfMonth(
				productInsertDateStart.Year,
				productInsertDateStart.Month,
				productInsertDateStart.Day))
			{
				var productInsertDateStartDay = DateTimeUtility.GetLastDayOfMonth(
					productInsertDateStart.Year,
					productInsertDateStart.Month);
				ucProductInsertDatePeriod.SetStartDate(productInsertDateStart.AddDays(productInsertDateStartDay));
			}
		}

		string strDateTimeTemp = null;
		DateTime? dtInsertFrom = null;
		strDateTimeTemp = ucProductInsertDatePeriod.StartDateTimeString;
		if (Validator.IsDate(strDateTimeTemp))
		{
			dtInsertFrom = DateTime.Parse(strDateTimeTemp);
		}

		// 末日補正処理
		if (string.IsNullOrEmpty(ucProductInsertDatePeriod.EndDateString) == false)
		{
			var productInsertDateEnd = DateTime.Parse(ucProductInsertDatePeriod.EndDateString);
			if (DateTimeUtility.IsLastDayOfMonth(productInsertDateEnd.Year,
				productInsertDateEnd.Month,
				productInsertDateEnd.Day))
			{
				var productInsertDateEndDay = DateTimeUtility.GetLastDayOfMonth(
					productInsertDateEnd.Year,
					productInsertDateEnd.Month);
				ucProductInsertDatePeriod.SetEndDate(productInsertDateEnd.AddDays(productInsertDateEndDay));
			}
		}

		DateTime? dtInsertTo = null;
		strDateTimeTemp = ucProductInsertDatePeriod.EndDateTimeString;
		if (Validator.IsDate(strDateTimeTemp))
		{
			dtInsertTo = DateTime.Parse(strDateTimeTemp).AddSeconds(1);
		}

		// 末日補正処理
		if (string.IsNullOrEmpty(ucProductUpdateDatePeriod.StartDateTimeString) == false)
		{
			var productUpdateDateStart = DateTime.Parse(ucProductUpdateDatePeriod.StartDateTimeString);
			if (DateTimeUtility.IsLastDayOfMonth(
				productUpdateDateStart.Year,
				productUpdateDateStart.Month,
				productUpdateDateStart.Day))
			{
				var productUpdateDateStartDay = DateTimeUtility.GetLastDayOfMonth(
					productUpdateDateStart.Year,
					productUpdateDateStart.Month);
				ucProductUpdateDatePeriod.SetStartDate(productUpdateDateStart.AddDays(productUpdateDateStartDay));
			}
		}

		DateTime? dtUpdateFrom = null;
		strDateTimeTemp = ucProductUpdateDatePeriod.StartDateTimeString;
		if (Validator.IsDate(strDateTimeTemp))
		{
			dtUpdateFrom = DateTime.Parse(strDateTimeTemp);
		}

		// 末日補正処理
		if (string.IsNullOrEmpty(ucProductUpdateDatePeriod.EndDateTimeString) == false)
		{
			var productUpdateDateEnd = DateTime.Parse(ucProductUpdateDatePeriod.EndDateTimeString);
			if (DateTimeUtility.IsLastDayOfMonth(
				productUpdateDateEnd.Year,
				productUpdateDateEnd.Month,
				productUpdateDateEnd.Day))
			{
				var productUpdateDateEndDay = DateTimeUtility.GetLastDayOfMonth(
					productUpdateDateEnd.Year,
					productUpdateDateEnd.Month);
				ucProductUpdateDatePeriod.SetEndDate(productUpdateDateEnd.AddDays(productUpdateDateEndDay));
			}
		}

		DateTime? dtUpdateTo = null;
		strDateTimeTemp = ucProductUpdateDatePeriod.EndDateTimeString;
		if (Validator.IsDate(strDateTimeTemp))
		{
			dtUpdateTo = DateTime.Parse(strDateTimeTemp).AddSeconds(1);
		}

		htResult.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
		htResult.Add(Constants.FIELD_PRODUCT_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbProductId.Text.Trim()));
		htResult.Add(Constants.FIELD_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbName.Text.Trim()));
		htResult.Add("date_insert_from", dtInsertFrom);
		htResult.Add("date_insert_to", dtInsertTo);
		htResult.Add("date_update_from", dtUpdateFrom);
		htResult.Add("date_update_to", dtUpdateTo);
		htResult.Add(Constants.FIELD_PRODUCT_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_PRODUCT_VALID_FLG_VALID : "");

		return htResult;
	}

	/// <summary>
	/// 区分:ネクストエンジン モール設定の有効無効
	/// </summary>
	protected bool ValidNextEngine
	{
		get { return (bool)ViewState["ValidNextEngine"]; }
		set { ViewState["ValidNextEngine"] = value; }
	}
}
