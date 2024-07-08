/*
=========================================================================================================
  Module      : 名称翻訳設定リポジトリ (NameTranslationSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.NameTranslationSetting.Helper;

namespace w2.Domain.NameTranslationSetting
{
	/// <summary>
	/// 名称翻訳設定リポジトリ
	/// </summary>
	internal class NameTranslationSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "NameTranslationSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal NameTranslationSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal NameTranslationSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetAllTranslationSettingsByDataKbn 翻訳設定情報全件取得(データ区分ごと)
		/// <summary>
		/// 翻訳設定情報全件取得(データ区分ごと)
		/// </summary>
		/// <returns>翻訳設定情報(全件)</returns>
		internal NameTranslationSettingModel[] GetAllTranslationSettingsByDataKbn(string dataKbn)
		{
			var queryName = GetExecuteQueryNameForAllTranslationSettingsByDataKbn(dataKbn);
			var ht = new Hashtable {};
			var dv = Get(XML_KEY_NAME, queryName, ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetProductAndVariationTranslationSettings 商品、バリエーションの翻訳設定情報取得
		/// <summary>
		/// 商品、バリエーションの翻訳設定情報取得
		/// </summary>
		/// <returns>商品、商品バリエーション翻訳情報</returns>
		internal NameTranslationSettingModel[] GetProductAndVariationTranslationSettings(NameTranslationSettingSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetProductAndVariationTranslationSettings", condition.CreateHashtableParams());
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTranslationSettingsByMultipleMasterId1 複数のマスタID1を指定して翻訳設定情報取得
		/// <summary>
		/// 複数のマスタID1を指定して翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetTranslationSettingsByMultipleMasterId1(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetTranslationSettingsByMultipleMasterId1", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTranslationSettingsByDataKbn データ区分から翻訳設定情報取得
		/// <summary>
		/// データ区分から翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetTranslationSettingsByDataKbn(NameTranslationSettingSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetTranslationSettingsByDataKbn", condition.CreateHashtableParams());
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTranslationSettingsByMasterId マスタIDから翻訳設定情報取得
		/// <summary>
		/// マスタIDから翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetTranslationSettingsByMasterId(NameTranslationSettingSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetTranslationSettingsByMasterId", condition.CreateHashtableParams());
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTranslationSettingsByMasterIdAndLanguageCode マスタID／言語コードから翻訳設定情報取得
		/// <summary>
		/// マスタID／言語コードから翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetTranslationSettingsByMasterIdAndLanguageCode(NameTranslationSettingSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetTranslationSettingsByMasterIdAndLanguageCode", condition.CreateHashtableParams());
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetAfterTranslationalNameSetting 翻訳後名称設定取得
		/// <summary>
		/// 翻訳後名称設定取得
		/// </summary>
		/// <returns>翻訳後名称設定</returns>
		internal NameTranslationSettingModel GetAfterTranslationalNameSetting(NameTranslationSettingSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetAfterTranslationalName", condition.CreateHashtableParams());
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new NameTranslationSettingModel(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetSetPromotionTranslationSettingsForFrontDisplay フロント表示用セットプロモーション翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用セットプロモーション翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>セットプロモーション翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetSetPromotionTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetSetPromotionTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetProductListDispSettingTranslationSettingsForFrontDisplay フロント表示用商品一覧表示設定翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用商品一覧表示設定翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>商品一覧表示設定翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetProductListDispSettingTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetProductListDispSettingTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetUserExtendSettingTranslationSettingsForFrontDisplay フロント表示用ユーザ拡張項目翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用ユーザ拡張項目翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>ユーザ拡張項目翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetUserExtendSettingTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetUserExtendSettingTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetNewsTranslationSettingsForFrontDisplay フロント表示用新着情報翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用新着情報翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>新着情報翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetNewsTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetNewsTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCoordinateTranslationSettings コーディネートの翻訳設定情報取得
		/// <summary>
		/// コーディネートの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>コーディネート翻訳情報</returns>
		internal NameTranslationSettingModel[] GetCoordinateTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetCoordinateTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCoordinateCategoryTranslationSettings コーディネートカテゴリの翻訳設定情報取得
		/// <summary>
		/// コーディネートカテゴリの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>コーディネートカテゴリ翻訳情報</returns>
		internal NameTranslationSettingModel[] GetCoordinateCategoryTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetCoordinateCategoryTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetStaffTranslationSettings スタッフの翻訳設定情報取得
		/// <summary>
		/// スタッフの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>スタッフ翻訳情報</returns>
		internal NameTranslationSettingModel[] GetStaffTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetStaffTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetRealShopTranslationSettings リアル店舗の翻訳設定情報取得
		/// <summary>
		/// リアル店舗の翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>リアル店舗翻訳情報</returns>
		internal NameTranslationSettingModel[] GetRealShopTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			if (condition.MasterId1List.Any() == false) return new NameTranslationSettingModel[0];

			var masterId1s = string.Join(",", condition.MasterId1ArrayEscapedSingleQuotation.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", masterId1s);

			var dv = Get(XML_KEY_NAME, "GetRealShopTranslationSettingsForFrontDisplay", condition.CreateHashtableParams(), replaces: replace);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTranslationSettingsByListPage 翻訳設定情報取得(各一覧ページから)
		/// <summary>
		/// 翻訳設定情報取得(各一覧ページから)
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="exportTargetIdList">出力対象IDリスト</param>
		/// <returns>翻訳設定情報</returns>
		internal NameTranslationSettingModel[] GetTranslationSettingsByListPage(string dataKbn, string[] exportTargetIdList)
		{
			var queryName = GetExecuteQueryNameForTranslationSettingsByListPage(dataKbn);
			var ht = new Hashtable{};

			// IDリストが1件もない場合は、IN句に空を指定する
			var idList = (exportTargetIdList.Any())
				? string.Join(",", exportTargetIdList.Select(id => id.Replace("'", "''")).Select(id => string.Format("'{0}'", id)))
				: "''";
			var replace = new KeyValuePair<string, string>("@@ master_id1_where @@", idList);

			var dv = Get(XML_KEY_NAME, queryName, ht, replaces: replace);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetBeforeTranslationName 翻訳前名称取得
		/// <summary>
		/// 翻訳前名称取得
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <returns>処理件数</returns>
		internal NameTranslationSettingModel GetBeforeTranslationName(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2)
		{
			var condition = new NameTranslationSettingModel
			{
				DataKbn = dataKbn,
				TranslationTargetColumn = translationTargetColumn,
				MasterId1 = masterId1,
				MasterId2 = masterId2,
			};
			var dv = Get(XML_KEY_NAME, "GetBeforeTranslationName", condition.DataSource);
			if (dv.Count == 0) return null;
			return new NameTranslationSettingModel(dv[0]);
		}
		#endregion

		#region ~GetNameTranslationSettingWithLocales 名称翻訳設定情報取得(ロケール分)
		/// <summary>
		/// 名称翻訳設定情報取得(ロケール分)
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>処理件数</returns>
		internal NameTranslationSettingModel[] GetNameTranslationSettingWithLocales(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2,
			string languageCode = null,
			string languageLocaleId = null)
		{
			var condition = new NameTranslationSettingModel
			{
				DataKbn = dataKbn,
				TranslationTargetColumn = translationTargetColumn,
				MasterId1 = masterId1,
				MasterId2 = masterId2,
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var dv = Get(XML_KEY_NAME, "GetNameTranslationSettingWithLocales", condition.DataSource);
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCountNameTranslationSettingForListSearch 翻訳設定一覧件数取得
		/// <summary>
		/// 翻訳設定一覧情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定一覧情報</returns>
		internal int GetCountNameTranslationSettingForListSearch(NameTranslationSettingListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetCountNameTranslationSettingForListSearch", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetNameTranslationSettingForListSearch 翻訳設定一覧情報取得
		/// <summary>
		/// 翻訳設定一覧情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定一覧情報</returns>SqlStatementDataReader
		internal NameTranslationSettingModel[] GetNameTranslationSettingForListSearch(NameTranslationSettingListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetNameTranslationSettingForListSearch", condition.CreateHashtableParams());
			if (dv.Count == 0) return new NameTranslationSettingModel[0];
			return dv.Cast<DataRowView>()
				.Select(drv => new NameTranslationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(NameTranslationSettingModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~DeleteNameTranslationSettingForRegister 名称翻訳設定情報削除(登録画面用)
		/// <summary>
		/// 名称翻訳設定情報削除(登録画面用)
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <returns>処理件数</returns>
		internal int DeleteNameTranslationSettingForRegister(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2)
		{
			var condition = new NameTranslationSettingModel
			{
				DataKbn = dataKbn,
				TranslationTargetColumn = translationTargetColumn,
				MasterId1 = masterId1,
				MasterId2 = masterId2,
			};
			var result = Exec(XML_KEY_NAME, "DeleteNameTranslationSettingForRegister", condition.DataSource);
			return result;
		}
		#endregion

		#region リポジトリ内で使用するメソッド
		#region -GetExecuteQueryNameForAllTranslationSettingsByDataKbn 実行クエリ名取得(翻訳情報全件取得用)
		/// <summary>
		/// 実行クエリ名取得(翻訳情報全件取得用)
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>実行クエリ名</returns>
		private string GetExecuteQueryNameForAllTranslationSettingsByDataKbn(string dataKbn)
		{
			switch (dataKbn)
			{
				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT:
					return "GetProductAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION:
					return "GetProductVariationAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY:
					return "GetProductCategoryAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET:
					return "GetProductSetAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON:
					return "GetCouponAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK:
					return "GetMemberRankAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION:
					return "GetSetPromotionAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT:
					return "GetPaymentAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING:
					return "GetUserExtendSettingAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND:
					return "GetBrandAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY:
					return "GetNoveltyAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS:
					return "GetNewsAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING:
					return "GetOrderMemoSettingAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON:
					return "GetFixedPurchaseCancelReasonAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING:
					return "GetProductDispSettingAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION:
					return "GetSiteInformationAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING:
					return "GetProductTagSettingAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE:
					return "GetCoordinateAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY:
					return "GetCoordinateCategoryAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF:
					return "GetStaffAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP:
					return "GetRealShopAllTranslationSettings";
			}
			return string.Empty;
		}
		#endregion

		#region -GetExecuteQueryNameForTranslationDataByListPage 実行クエリ名取得(各一覧ページから翻訳設定情報取得用)
		/// <summary>
		/// 実行クエリ名取得(各一覧ページから翻訳設定情報取得用)
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>実行クエリ名</returns>
		private string GetExecuteQueryNameForTranslationSettingsByListPage(string dataKbn)
		{
			switch (dataKbn)
			{
				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT:
					return "GetTranslationSettingsFromProductList";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY:
					return "GetTranslationSettingsFromProductCategoryRegister";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION:
					return "GetTranslationSettingsFromSetPromotionList";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET:
					return "GetTranslationSettingsFromProductSetList";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY:
					return "GetTranslationSettingsFromNoveltyList";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS:
					return "GetTranslationSettingsFromNewsList";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING:
					return "GetTranslationSettingsFromOrderMemoList";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT:
					// 決済種別設定一覧は検索条件がないため、全件出力
					return "GetPaymentAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING:
					// ユーザー拡張項目設定一覧は検索条件がないため、全件出力
					return "GetUserExtendSettingAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING:
					// 商品一覧表示設定一覧は検索条件がないため、全件出力
					return "GetProductDispSettingAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION:
					return "GetSiteInformationAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON:
					return "GetTranslationSettingsFromCouponList";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK:
					// 会員ランク設定は検索条件がないため、全件出力
					return "GetMemberRankAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON:
					// 定期解約理由区分は検索条件がないため、全件出力
					return "GetFixedPurchaseCancelReasonAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND:
					// 商品ブランドは検索条件がないため、全件出力
					return "GetBrandAllTranslationSettings";

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING:
					// 商品タグは検索条件がないため、全件出力
					return "GetTagSettingAllTranslationSettings";
			}
			return string.Empty;
		}
		#endregion
		#endregion
	}
}
