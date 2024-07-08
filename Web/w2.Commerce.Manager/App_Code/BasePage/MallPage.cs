/*
=========================================================================================================
  Module      : モール共通ページ(MallPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;

///**************************************************************************************
/// <summary>
/// モール共通ページ
/// </summary>
///**************************************************************************************
public class MallPage : BasePage
{
	/// <summary>
	/// モール連携設定取得
	/// </summary>
	public static DataRowView GetMallCooperationSetting(string strShopId, string strMallId)
	{
		DataView dvMallCooperationSetting = null;
		using (SqlAccessor sqlAccesser = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSetting"))
		{
			Hashtable htInput = new Hashtable();
			htInput[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID] = strShopId;
			htInput[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] = strMallId;

			dvMallCooperationSetting = sqlStatement.SelectSingleStatementWithOC(sqlAccesser, htInput);
		}

		return dvMallCooperationSetting[0];
	}

	/// <summary>
	/// モール連携設定取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <returns>モール連携設定</returns>
	public static DataView GetMallCooperationSettingAll(string strShopId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSettingAll"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, strShopId);

			return sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// モール連携設定情報更新処理
	/// </summary>
	/// <param name="htInput">更新内容</param>
	protected void UpdateMallCooperationSetting(Hashtable htInput)
	{
		// モール連携設定情報更新
		using (SqlAccessor sqlAccesser = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "UpdateMallCooperation"))
		{
			sqlStatement.ExecStatementWithOC(sqlAccesser, htInput);
		}

		// モール出品設定リセット
		ResetMallExhibitsConfig();
	}

	/// <summary>
	/// モール出品設定リセット
	/// </summary>
	protected void ResetMallExhibitsConfig()
	{
		// モール出品設定リセット
		Hashtable htMallExhibitsConfig = new Hashtable();
		DataView dvMallCooperationSettings = MallPage.GetMallCooperationSettingAll(this.LoginOperatorShopId);
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
		{
			htMallExhibitsConfig.Add(li.Value.Replace("EXH", "exhibits_flg"), Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON);
			foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSettings)
			{
				if ((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG] == li.Value)
				{
					htMallExhibitsConfig[li.Value.Replace("EXH", "exhibits_flg")] = "";
					break;
				}
			}
		}

		// 更新処理
		using (SqlAccessor sqlAccesser = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "UpdateMallExhibitsConfigReset"))
		{
			htMallExhibitsConfig.Add(Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID, this.LoginOperatorShopId);
			htMallExhibitsConfig.Add(Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED, this.LoginOperatorId);

			sqlStatement.ExecStatementWithOC(sqlAccesser, htMallExhibitsConfig);
		}
	}

	/// <summary>
	/// モール連携設定画面遷移URL作成
	/// </summary>
	/// <param name="strMallId">モールＩＤ</param>
	/// <returns>モール連携設定画面遷移URL</returns>
	protected string CreateMallConfigUrl(string strMallId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_MALL_CONFIG);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_MALL_ID).Append("=").Append(HttpUtility.UrlEncode(strMallId));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbResult.ToString();
	}

	/// <summary>
	/// 商品コンバータ一覧遷移URL作成
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品コンバータ一覧遷移URL</returns>
	protected static string CreateProductConverterListUrl(int iPageNumber)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_LIST);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(iPageNumber.ToString());

		return sbResult.ToString();
	}
}
