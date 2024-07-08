/*
=========================================================================================================
  Module      : 名称翻訳設定サービス (NameTranslationSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;
using w2.Domain.NameTranslationSetting.Helper;

namespace w2.Domain.NameTranslationSetting
{
	/// <summary>
	/// 名称翻訳設定サービス
	/// </summary>
	public class NameTranslationSettingService : ServiceBase, INameTranslationSettingService
	{
		#region +GetAllTranslationSettingsByDataKbn 翻訳設定情報全件取得(データ区分ごと)
		/// <summary>
		/// 翻訳設定情報全件取得(データ区分ごと)
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetAllTranslationSettingsByDataKbn(string dataKbn)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetAllTranslationSettingsByDataKbn(dataKbn);
				return data;
			}
		}
		#endregion

		#region +GetTranslationSettingsByListPage 翻訳設定情報取得(各一覧ページから)
		/// <summary>
		/// 翻訳設定情報取得(各一覧ページから)
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="exportTargetIdList">出力対象IDリスト</param>
		/// <returns>翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetTranslationSettingsByListPage(string dataKbn, string[] exportTargetIdList)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetTranslationSettingsByListPage(dataKbn, exportTargetIdList);
				return data;
			}
		}
		#endregion

		#region +GetTranslationSettingsByDataKbn データ区分から翻訳設定情報取得
		/// <summary>
		/// データ区分から翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetTranslationSettingsByDataKbn(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetTranslationSettingsByDataKbn(condition);
				return data;
			}
		}
		#endregion

		#region +GetProductAndVariationTranslationSettings 商品、バリエーションの翻訳設定情報取得
		/// <summary>
		/// 商品IDから商品、バリエーションの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>商品、商品バリエーション翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetProductAndVariationTranslationSettings(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetProductAndVariationTranslationSettings(condition);
				return data;
			}
		}
		#endregion

		#region +GetTranslationSettingsByMultipleMasterId1 複数のマスタID1を指定して翻訳設定情報取得
		/// <summary>
		/// 複数のマスタID1を指定して翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetTranslationSettingsByMultipleMasterId1(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetTranslationSettingsByMultipleMasterId1(condition);
				return data;
			}
		}
		#endregion

		#region +GetTranslationSettingsByMasterId マスタIDから翻訳設定情報取得
		/// <summary>
		/// マスタIDから翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetTranslationSettingsByMasterId(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var translationSettings = repository.GetTranslationSettingsByMasterId(condition);
				return translationSettings;
			}
		}
		#endregion

		#region +GetTranslationSettingsByMasterIdAndLanguageCode マスタID／言語コードから翻訳設定情報取得
		/// <summary>
		/// マスタID／言語コードから翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetTranslationSettingsByMasterIdAndLanguageCode(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var translationSettings = repository.GetTranslationSettingsByMasterIdAndLanguageCode(condition);
				return translationSettings;
			}
		}
		#endregion

		#region +GetAfterTranslationalNameSetting 翻訳後名称設定取得
		/// <summary>
		/// 翻訳後名称設定取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳後名称設定</returns>
		public NameTranslationSettingModel GetAfterTranslationalNameSetting(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var afterTrasnlationalNameSetting = repository.GetAfterTranslationalNameSetting(condition);
				return afterTrasnlationalNameSetting;
			}
		}
		#endregion

		#region +GetSetPromotionTranslationSettingsForFrontDisplay フロント表示用セットプロモーション翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用セットプロモーション翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetSetPromotionTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var translationSettings = repository.GetSetPromotionTranslationSettingsForFrontDisplay(condition);
				return translationSettings;
			}
		}
		#endregion

		#region +GetProductListDispSettingTranslationSettingsForFrontDisplay フロント表示用商品一覧表示設定翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用商品一覧表示設定翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>商品一覧表示設定翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetProductListDispSettingTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var translationSettings = repository.GetProductListDispSettingTranslationSettingsForFrontDisplay(condition);
				return translationSettings;
			}
		}
		#endregion

		#region +GetUserExtendSettingTranslationSettingsForFrontDisplay フロント表示用ユーザ拡張項目翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用ユーザ拡張項目翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>ユーザ拡張項目翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetUserExtendSettingTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var translationSettings = repository.GetUserExtendSettingTranslationSettingsForFrontDisplay(condition);
				return translationSettings;
			}
		}
		#endregion

		#region +GetNewsTranslationSettingsForFrontDisplay フロント表示用新着情報翻訳設定情報取得(翻訳前情報も取得)
		/// <summary>
		/// フロント表示用新着情報翻訳設定情報取得(翻訳前情報も取得)
		/// </summary>
		/// <param name="condition">検索情報</param>
		/// <returns>新着情報翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetNewsTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var translationSettings = repository.GetNewsTranslationSettingsForFrontDisplay(condition);
				return translationSettings;
			}
		}
		#endregion

		#region +GetCoordinateTranslationSettingsForFrontDisplay フロント表示用コーディネートの翻訳設定情報取得
		/// <summary>
		/// フロント表示用コーディネートの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>コーディネート翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetCoordinateTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetCoordinateTranslationSettingsForFrontDisplay(condition);
				return data;
			}
		}
		#endregion

		#region +GetCoordinateCategoryTranslationSettingsForFrontDisplay フロント表示用コーディネートカテゴリの翻訳設定情報取得
		/// <summary>
		/// フロント表示用コーディネートカテゴリの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>コーディネートカテゴリ翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetCoordinateCategoryTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetCoordinateCategoryTranslationSettingsForFrontDisplay(condition);
				return data;
			}
		}
		#endregion

		#region +GetStaffTranslationSettingsForFrontDisplay フロント表示用スタッフの翻訳設定情報取得
		/// <summary>
		/// フロント表示用スタッフの翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>スタッフ翻訳設定情報</returns>
		public NameTranslationSettingModel[] GetStaffTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetStaffTranslationSettingsForFrontDisplay(condition);
				return data;
			}
		}
		#endregion

		#region +GetRealShopTranslationSettingsForFrontDisplay フロント表示用リアル店舗の翻訳設定情報取得
		/// <summary>
		/// フロント表示用リアル店舗の翻訳設定情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>リアル店舗設定情報</returns>
		public NameTranslationSettingModel[] GetRealShopTranslationSettingsForFrontDisplay(NameTranslationSettingSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetRealShopTranslationSettingsForFrontDisplay(condition);
				return data;
			}
		}
		#endregion

		#region +GetBeforeTranslationName 翻訳前名称取得
		/// <summary>
		/// 翻訳前名称取得
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <returns></returns>
		public NameTranslationSettingContainer GetBeforeTranslationName(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var data = repository.GetBeforeTranslationName(
					dataKbn,
					translationTargetColumn,
					masterId1,
					masterId2);
				return new NameTranslationSettingContainer
				{
					DataKbn = data.DataKbn,
					TranslationTargetColumn = data.TranslationTargetColumn,
					MasterId1 = data.MasterId1,
					MasterId2 = data.MasterId2,
					BeforeTranslationalName = data.BeforeTranslationalName
				};
			}
		}
		#endregion

		#region +GetNameTranslationSettingForRegister 名称翻訳設定情報取得(登録画面用)
		/// <summary>
		/// 名称翻訳設定情報取得(ロケール分)
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <returns>名称翻訳設定情報(ロケール分)</returns>
		public NameTranslationSettingContainer GetNameTranslationSettingForRegister(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var condition = new NameTranslationSettingModel
				{
					DataKbn = dataKbn,
					TranslationTargetColumn = translationTargetColumn,
					MasterId1 = masterId1,
					MasterId2 = masterId2,
				};
				var data = GetNameTranslationSettingWithLocales(repository, condition);
				return data;
			}
		}
		#endregion

		#region +GetCountNameTranslationSettingForListSearch 翻訳設定一覧件数取得
		/// <summary>
		/// 翻訳設定一覧件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定一覧件数</returns>
		public int GetCountNameTranslationSettingForListSearch(NameTranslationSettingListSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var count = repository.GetCountNameTranslationSettingForListSearch(condition);
				return count;
			}
		}
		#endregion

		#region +GetNameTranslationSettingForListSearch 翻訳設定一覧情報取得
		/// <summary>
		/// 翻訳設定一覧情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		public NameTranslationSettingContainer[] GetNameTranslationSettingForListSearch(
			NameTranslationSettingListSearchCondition condition)
		{
			using (var repository = new NameTranslationSettingRepository())
			{
				var list = repository.GetNameTranslationSettingForListSearch(condition);
				var listWithLocales = list.Select(data => GetNameTranslationSettingWithLocales(repository, data)).ToArray();
				return listWithLocales;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(NameTranslationSettingModel model, SqlAccessor accessor)
		{
			using (var repository = new NameTranslationSettingRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +DeleteNameTranslationSettingForRegister 名称翻訳設定情報削除(登録画面用)
		/// <summary>
		/// 名称翻訳設定情報削除(登録画面用)
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteNameTranslationSettingForRegister(
			string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2,
			SqlAccessor accessor)
		{
			using (var repository = new NameTranslationSettingRepository(accessor))
			{
				repository.DeleteNameTranslationSettingForRegister(dataKbn, translationTargetColumn, masterId1, masterId2);
			}
		}
		#endregion

		#region -GetNameTranslationSettingWithLocales 翻訳設定情報取得（ロケール分）
		/// <summary>
		/// 翻訳設定情報取得（ロケール分）
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		/// <param name="condition">検索条件</param>
		/// <returns>翻訳設定情報</returns>
		private static NameTranslationSettingContainer GetNameTranslationSettingWithLocales(
			NameTranslationSettingRepository repository,
			NameTranslationSettingModel condition)
		{
			var languages = repository.GetNameTranslationSettingWithLocales(
				condition.DataKbn,
				condition.TranslationTargetColumn,
				condition.MasterId1,
				condition.MasterId2,
				condition.LanguageCode,
				condition.LanguageLocaleId);

			var firstLanguage = languages.FirstOrDefault();
			var beforeTranslationalName = (firstLanguage == null) ? null : firstLanguage.BeforeTranslationalName;

			return new NameTranslationSettingContainer
			{
				DataKbn = condition.DataKbn,
				TranslationTargetColumn = condition.TranslationTargetColumn,
				MasterId1 = condition.MasterId1,
				MasterId2 = condition.MasterId2,
				BeforeTranslationalName = beforeTranslationalName,
				Languages = languages
			};
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// 対象データ区分に紐づく、翻訳名称項目のValueTextのキー
		/// </summary>
		public static Dictionary<string, string> ValueTextKeyForTranslationTargetColumn
		{
			get
			{
				var dict = new Dictionary<string, string>
				{
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT, "translation_target_column_product"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION, "translation_target_column_product_variation"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY, "translation_target_column_product_category"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION, "translation_target_column_set_promotion"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET, "translation_target_column_product_set"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON, "translation_target_column_coupon"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK, "translation_target_column_member_rank"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT, "translation_target_column_payment"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING, "translation_target_column_user_extend_setting"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND, "translation_target_column_product_brand"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY, "translation_target_column_novelty"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS, "translation_target_column_news"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING, "translation_target_column_order_memo_setting"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON, "translation_target_column_fixed_purchase_cancel_reason"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING, "translation_target_column_product_list_disp_setting"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION, "translation_target_column_site_information"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING, "translation_target_column_product_tag_setting"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE, "translation_target_column_coordinate"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY, "translation_target_column_coordinate_category"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF, "translation_target_column_staff"　},
					{　Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP, "translation_target_column_real_shop"　},
				};
				return dict;
			}
		}
		#endregion
	}
}
