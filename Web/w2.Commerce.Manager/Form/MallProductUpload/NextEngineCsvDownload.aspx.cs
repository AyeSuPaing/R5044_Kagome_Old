/*
=========================================================================================================
  Module      : モール連携 ネクストエンジン モール商品CSVダウンロードページ(NextEngineCsvDownload.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.Domain.MallCooperationSetting;

/// <summary>
/// モール連携 ネクストエンジン モール商品CSVダウンロードページ
/// </summary>
public partial class Form_MallProductUpload_NextEngineCsvDownload : BasePage
{
	/// <summary>モール商品CSV ヘッダー 固定</summary>
	private const string MALL_PRODUCT_CSV_HEADER = "商品コード,商品名,売価";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (Session[Constants.SESSIONPARAM_KEY_MALLPRODUCTUPLOAD_SEARCH_INFO] == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(
					Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR
					+ "?" + Constants.REQUEST_KEY_WINDOW_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
			}

			this.Input = (Hashtable)Session[Constants.SESSIONPARAM_KEY_MALLPRODUCTUPLOAD_SEARCH_INFO];
		}
	}

	/// <summary>
	/// モール商品CSVボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNextEngineCsv_OnClick(object sender, EventArgs e)
	{
		GetCsv(this.Input);
	}

	/// <summary>
	/// モール商品CSVの取得
	/// </summary>
	/// <param name="input">入力内容</param>
	private void GetCsv(Hashtable input)
	{
		var nextEngineMall = new MallCooperationSettingService()
			.GetValidByMallKbn(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE)
			.FirstOrDefault(
				m => (m.ValidFlg == Constants.FLG_MALLCOOPERATIONSETTING_VALID_FLG_VALID));

		if (nextEngineMall == null) return;

		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("MallCooperationUpdateLog", "GetTargetNextEngineMallProductCsv"))
		{
			statement.ReplaceStatement(
				"## EXHIBITS_FLG ##",
				(string.IsNullOrEmpty(nextEngineMall.MallExhibitsConfig) == false)
					? string.Format(
						" AND {0}.{1} = {2} ",
						Constants.TABLE_MALLEXHIBITSCONFIG,
						MallOptionUtility.GetExhibitsConfigField(nextEngineMall.MallExhibitsConfig),
						Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)
					: string.Empty);
			var dv = statement.SelectSingleStatementWithOC(accessor, input);
			CsvExport(dv);
		}
	}

	/// <summary>
	/// CSVエクスポート
	/// </summary>
	/// <param name="exportTargetData">エクスポート対象データ</param>
	private void CsvExport(DataView exportTargetData)
	{
		Response.ContentEncoding = Encoding.GetEncoding("Shift-jis");
		Response.AppendHeader("Content-type", "application/x-download");
		Response.AppendHeader(
			"Content-Disposition",
			"attachment; filename=" + "next_engine_mall_product_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
		Response.Write((MALL_PRODUCT_CSV_HEADER) + Environment.NewLine);
		Response.Write(
			string.Join(
				Environment.NewLine,
				exportTargetData.Cast<DataRowView>().Select(
					drv => string.Format(
						"{0},\"{1}\",{2}",
						drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString(),
						drv[Constants.FIELD_PRODUCT_NAME].ToString(),
						drv[Constants.FIELD_PRODUCTVARIATION_PRICE].ToPriceString())).ToArray()));
		Response.Flush();
		Response.End();
	}

	/// <summary>入力内容</summary>
	private Hashtable Input
	{
		get { return (Hashtable)ViewState["Input"]; }
		set { ViewState["Input"] = value; }
	}
}