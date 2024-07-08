/*
=========================================================================================================
  Module      : マスタファイル出力(MasterFileExport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.App.Common.MasterExport;
using w2.App.Common.Order;
using w2.Domain.ProductTaxCategory;

/// <summary>
/// MasterFileExport
/// </summary>
public static class MasterFileExport
{
	/// <summary>検索抽出条件</summary>
	private const string SQLSTATEMENT_WHERE = "@@ where @@";	

	/// <summary>
	/// エクスポートデータ抽出
	/// </summary>
	/// <param name="masterKbn">マスター区分</param>
	/// <param name="sqlPageName">SQLページ名</param>
	/// <param name="sqlStatmentName">SQLステートメント名</param>
	/// <param name="param">パラメータ</param>
	/// <param name="sqlFieldNames">フィールド</param>
	/// <returns></returns>
	public static DataView ExtractExportData(
		string masterKbn,
		string sqlPageName,
		string sqlStatmentName,
		Hashtable param,
		string sqlFieldNames)
	{
		var types = MasterExportSettingUtility.GetMasterExportSettingTypes(masterKbn);
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER)
		{
			// ユーザー属性
			var userAttributeTypes =
				MasterExportSettingUtility.GetMasterExportSettingTypes(
					Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE);
			userAttributeTypes.Keys.Cast<string>().ToList().ForEach(key => types[key] = userAttributeTypes[key]);
		}

		var taxFieldNames = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
		foreach (var taxFieldName in taxFieldNames)
		{
			types[taxFieldName] = "price";
		}

		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement(sqlPageName, sqlStatmentName))
		{
			// @@ fields @@をフィールド列に置換
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ fields @@", sqlFieldNames);
			// @@ where @@を条件文に置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				"@@ where @@",
				StringUtility.ToEmpty(param[SQLSTATEMENT_WHERE]));
			// @@ orderby @@を条件文に置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				"@@ orderby @@",
				OrderCommon.GetOrderSearchOrderByStringForOrderItemListAndWorkflow(
					((string)param["sort_kbn"]),
					sqlPageName,
					sqlStatmentName));
			// @@ user_extend_field_name @@を条件文に置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				"@@ user_extend_field_name @@",
				string.Format(
					"{0}.{1}",
					Constants.TABLE_USEREXTEND,
					string.IsNullOrEmpty((string)param["user_extend_name"])
						? Constants.FIELD_USEREXTEND_USER_ID
						: (string)param["user_extend_name"]));

			// Replace @@ multi_order_id @@ with conditional statement
			sqlStatement.ReplaceStatement("@@ multi_order_id @@", OrderCommon.GetOrderSearchMultiOrderId(param));
			sqlStatement.ReplaceStatement(
				"@@ order_shipping_addr1 @@",
				StringUtility.ToEmpty(param[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));

			// 条件文はパラメータではないので削除
			param.Remove(SQLSTATEMENT_WHERE);

			var dvSource = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, param);
			return dvSource;
		}
	}
}