/*
=========================================================================================================
  Module      : 商品カテゴリサービスのインターフェース (IProductCategoryService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.IO;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.ProductCategory
{
	/// <summary>
	/// 商品カテゴリサービスのインターフェース
	/// </summary>
	public interface IProductCategoryService : IService
	{
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <returns>モデル</returns>
		ProductCategoryModel[] GetAll();

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		ProductCategoryModel Get(string categoryId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="categoryIds">カテゴリID</param>
		/// <returns>モデル</returns>
		ProductCategoryModel[] GetByCategoryIds(string[] categoryIds);

		/// <summary>
		/// Check Category By Fixed Purchase Member Flg
		/// </summary>
		/// <param name="categoryId">The category Id</param>
		/// <param name="fixedPurchaseMemberFlg">The Fixed Purchase Member Flg</param>
		/// <returns>Is Access (True: Allow/ Flase: Deny</returns>
		bool CheckCategoryByFixedPurchaseMemberFlg(string categoryId, string fixedPurchaseMemberFlg);

		/// <summary>
		/// 有効なカテゴリID存在チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productCategoryIdsList">商品カテゴリIDリスト</param>
		/// <returns>有効なカテゴリID</returns>
		string[] CheckValidProductCategory(
			string shopId,
			string[] productCategoryIdsList);

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

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		bool CheckFieldsForGetMaster(string sqlFieldNames, string shopId);
	}
}
