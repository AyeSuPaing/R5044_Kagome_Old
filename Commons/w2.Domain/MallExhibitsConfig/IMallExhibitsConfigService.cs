/*
=========================================================================================================
  Module      : モール出品設定サービスのインターフェース (IMallExhibitsConfigService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.IO;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.MallExhibitsConfig
{
	/// <summary>
	/// モール出品設定サービスのインターフェース
	/// </summary>
	public interface IMallExhibitsConfigService : IService
	{
		/// <summary>
		/// CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice);

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice);

		/// Is exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>True if mall exhibits config is existed, otherwise false</returns>
		bool IsExist(
			string shopId,
			string productId,
			SqlAccessor accessor = null);

		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Insert(MallExhibitsConfigModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Delete(string shopId, string productId, SqlAccessor accessor = null);
	}
}
