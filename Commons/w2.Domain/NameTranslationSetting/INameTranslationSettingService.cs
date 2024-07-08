/*
=========================================================================================================
  Module      : 名称翻訳設定サービスのインタフェース(INameTranslationSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.NameTranslationSetting.Helper;

namespace w2.Domain.NameTranslationSetting
{
	/// <summary>
	/// 名称翻訳設定サービスのインタフェース
	/// </summary>
	public interface INameTranslationSettingService : IService
	{
		/// <summary>
		/// 翻訳設定情報全件取得(データ区分ごと)
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>翻訳設定情報</returns>
		NameTranslationSettingModel[] GetAllTranslationSettingsByDataKbn(string dataKbn);

		/// <summary>
		/// 翻訳設定情報取得(各一覧ページから)
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="exportTargetIdList">出力対象IDリスト</param>
		/// <returns>翻訳設定情報</returns>
		NameTranslationSettingModel[] GetTranslationSettingsByListPage(string dataKbn, string[] exportTargetIdList);

		/// <summary>
		/// データ区分から翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		NameTranslationSettingModel[] GetTranslationSettingsByDataKbn(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// 商品IDから商品、バリエーションの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>商品、商品バリエーション翻訳設定情報</returns>
		NameTranslationSettingModel[] GetProductAndVariationTranslationSettings(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// 複数のマスタID1を指定して翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>翻訳設定情報</returns>
		NameTranslationSettingModel[] GetTranslationSettingsByMultipleMasterId1(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// マスタIDから翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		NameTranslationSettingModel[] GetTranslationSettingsByMasterId(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// マスタID／言語コードから翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		NameTranslationSettingModel[] GetTranslationSettingsByMasterIdAndLanguageCode(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// 翻訳後名称設定取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳後名称設定</returns>
		NameTranslationSettingModel GetAfterTranslationalNameSetting(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用セットプロモーション翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>翻訳設定情報</returns>
		NameTranslationSettingModel[] GetSetPromotionTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用商品一覧表示設定翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>商品一覧表示設定翻訳設定情報</returns>
		NameTranslationSettingModel[] GetProductListDispSettingTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用ユーザ拡張項目翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>ユーザ拡張項目翻訳設定情報</returns>
		NameTranslationSettingModel[] GetUserExtendSettingTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用新着情報翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>新着情報翻訳設定情報</returns>
		NameTranslationSettingModel[] GetNewsTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用コーディネートの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>コーディネート翻訳設定情報</returns>
		NameTranslationSettingModel[] GetCoordinateTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用コーディネートカテゴリの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>コーディネートカテゴリ翻訳設定情報</returns>
		NameTranslationSettingModel[] GetCoordinateCategoryTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用スタッフの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>スタッフ翻訳設定情報</returns>
		NameTranslationSettingModel[] GetStaffTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// フロント表示用リアル店舗の翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>リアル店舗設定情報</returns>
		NameTranslationSettingModel[] GetRealShopTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition);

		/// <summary>
		/// 翻訳前名称取得
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <returns></returns>
		NameTranslationSettingContainer GetBeforeTranslationName(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2);

		/// <summary>
		/// 名称翻訳設定情報取得(ロケール分)
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <returns>名称翻訳設定情報(ロケール分)</returns>
		NameTranslationSettingContainer GetNameTranslationSettingForRegister(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2);

		/// <summary>
		/// 翻訳設定一覧件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定一覧件数</returns>
		int GetCountNameTranslationSettingForListSearch(NameTranslationSettingListSearchCondition condition);

		/// <summary>
		/// 翻訳設定一覧情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		NameTranslationSettingContainer[] GetNameTranslationSettingForListSearch(
			NameTranslationSettingListSearchCondition condition);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void Insert(NameTranslationSettingModel model, SqlAccessor accessor);

		/// <summary>
		/// 名称翻訳設定情報削除(登録画面用)
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeleteNameTranslationSettingForRegister(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2,
			SqlAccessor accessor);
	}
}