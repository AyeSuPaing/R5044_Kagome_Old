/*
=========================================================================================================
  Module      : Webメッセージクラス(WebMessages.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace w2.Cms.Manager.Codes.Common
{
	/// <summary>
	/// Webメッセージクラス
	/// </summary>
	public class WebMessages
	{
		/// <summary>エラーファイル最終更新日</summary>
		private static DateTime m_fileLastUpdate = new DateTime(0);
		/// <summary>エラーメッセージ格納ディクショナリ</summary>
		private static readonly Dictionary<string, string> m_dicMessages = new Dictionary<string, string>();
		/// <summary>ReaderWriterLockSlimオブジェクト</summary>
		private static readonly System.Threading.ReaderWriterLockSlim m_lock = new System.Threading.ReaderWriterLockSlim();

		/// <summary>システムエラー</summary>
		public static string SystemError { get { return Get("ERRMSG_MANAGER_SYSTEM_ERROR"); } }
		/// <summary>データベースエラー</summary>
		public static string DataBaseError { get { return Get("ERRMSG_DATABASE_ERROR"); } }
		/// <summary>HTTP404エラー</summary>
		public static string Http404Error { get { return Get("ERRMSG_404_ERROR"); } }
		/// <summary>システムバリデーションエラー</summary>
		public static string SystemValidationError { get { return Get("ERRMSG_SYSTEM_VALIDATION_ERROR"); } }

		/// <summary>一覧ヒットなしエラー</summary>
		public static string NoHitListError { get { return Get("ERRMSG_MANAGER_NO_HIT_LIST"); } }
		/// <summary>データ不整合エラー</summary>
		public static string InconsistencyError { get { return Get("ERRMSG_MANAGER_INCONSISTENCY_ERROR"); } }
		/// <summary>検索条件の日付指定時エラー</summary>
		public static string InvalidDateError { get { return Get("ERRMSG_MANAGER_INVALID_DATE_ERROR"); } }

		/// <summary>ログインエラー（ログイン失敗）</summary>
		public static string ShopOperatorLoginError { get { return Get("ERRMSG_MANAGER_SHOP_OPERATOR_LOGIN_ERROR"); } }
		/// <summary>ログインエラー（アクセス権なし、シングルサインオンで利用）</summary>
		public static string ShopOperatorUnaccessable { get { return Get("ERRMSG_MANAGER_LOGIN_UNACCESSABLE"); } }
		/// <summary>ログインエラー（試行カウントオーバー）</summary>
		public static string ShopOperatorLoginLimitedCountError { get { return Get("ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR"); } }
		/// <summary>未ログインエラー</summary>
		public static string ShopOperatorUnloggedIn { get { return Get("ERRMSG_MANAGER_NO_SHOP_ID"); } }
		/// <summary>パスワード変更エラー</summary>
		public static string ShopOperatorNoOperatorError { get { return Get("ERRMSG_MANAGER_SHOP_OPERATOR_NO_OPERATOR_ERROR"); } }
		/// <summary>Shop operator login session expired error</summary>
		public static string ShopOperatorLoginSessionExpiredError { get { return Get("ERRMSG_MANAGER_LOGIN_SESSION_EXPIRED"); } }

		/// <summary>SEOタイトルテキストエラー</summary>
		public static string ERRMSG_MANAGER_SEO_TITLE_TEXT_ERROR = "ERRMSG_MANAGER_SEO_TITLE_TEXT_ERROR";
		/// <summary>SEOキーワードテキストエラー</summary>
		public static string ERRMSG_MANAGER_SEO_KEYWORDS_TEXT_ERROR = "ERRMSG_MANAGER_SEO_KEYWORDS_TEXT_ERROR";
		/// <summary>SEOディスクリプションテキストエラー</summary>
		public static string ERRMSG_MANAGER_SEO_DESCRIPTION_TEXT_ERROR = "ERRMSG_MANAGER_SEO_DESCRIPTION_TEXT_ERROR";
		/// <summary>SEOコメントテキストエラー</summary>
		public static string ERRMSG_MANAGER_SEO_COMMENT_TEXT_ERROR = "ERRMSG_MANAGER_SEO_COMMENT_TEXT_ERROR";
		/// <summary>SEOタイトル開始タグ囲みエラー</summary>
		public static string ERRMSG_MANAGER_SEO_TITLE_START_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_TITLE_START_TAG_ENCLOSURE_ERROR";
		/// <summary>SEOキーワード開始タグ囲みエラー</summary>
		public static string ERRMSG_MANAGER_SEO_KEYWORDS_START_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_KEYWORDS_START_TAG_ENCLOSURE_ERROR";
		/// <summary>SEOディスクリプション開始タグ囲みエラー</summary>
		public static string ERRMSG_MANAGER_SEO_DESCRIPTION_START_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_DESCRIPTION_START_TAG_ENCLOSURE_ERROR";
		/// <summary>SEOコメント開始タグ囲みエラー</summary>
		public static string ERRMSG_MANAGER_SEO_COMMENT_START_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_COMMENT_START_TAG_ENCLOSURE_ERROR";
		/// <summary>SEOタイトル閉じタグ囲みエラー</summary>
		public static string ERRMSG_MANAGER_SEO_TITLE_END_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_TITLE_END_TAG_ENCLOSURE_ERROR";
		/// <summary>SEOキーワード閉じタグ囲みエラー</summary>
		public static string ERRMSG_MANAGER_SEO_KEYWORDS_END_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_KEYWORDS_END_TAG_ENCLOSURE_ERROR";
		/// <summary>SEOディスクリプション閉じタグ囲みエラー</summary>
		public static string ERRMSG_MANAGER_SEO_DESCRIPTION_END_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_DESCRIPTION_END_TAG_ENCLOSURE_ERROR";
		/// <summary>SEOコメント開始タグ閉じエラー</summary>
		public static string ERRMSG_MANAGER_SEO_COMMENT_END_TAG_ENCLOSURE_ERROR = "ERRMSG_MANAGER_SEO_COMMENT_END_TAG_ENCLOSURE_ERROR";


		/// <summary>コンテンツ管理</summary>
		public static string ContentsManagerFileAlreadyExists { get { return Get("ERRMSG_MANAGER_CONTENTSMANAGER_FILE_ALREADY_EXISTS"); } }
		public static string ContentsManagerFileOperationError { get { return Get("ERRMSG_MANAGER_CONTENTSMANAGER_FILE_OPERATION_ERROR"); } }
		public static string ContentsManagerImageSizeError { get { return Get("ERRMSG_MANAGER_CONTENTSMANAGER_IMAGESIZE_ERROR"); } }
		public static string ContentsManagerImageResizeError { get { return Get("ERRMSG_MANAGER_CONTENTSMANAGER_RESIZE_ERROR"); } }
		public static string ContentsManagerDeleteError { get { return Get("ERRMSG_MANAGER_CONTENTSMANAGER_DELETE_ERROR"); } }
		/// <summary>TreeView表示件数超過エラー</summary>
		public static string TreeViewMaxViewContentError { get { return Get("ERRMSG_MANAGER_TREEVIEW_MAX_VIEW_CONTENT_ERROR"); } }
		/// <summary>コーディネートページエラー</summary>
		public static string CoordinatePageMoveFileError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_MOVE_FILE_ERROR"); } }
		public static string CoordinatePageDeleteFileError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_DELETE_FILE_ERROR"); } }
		public static string CoordinatePageFileMaxError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_FILE_MAX_ERROR"); } }
		public static string CoordinatePageNotFileError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_NOT_FILE_ERROR"); } }
		public static string CoordinatePageVariationError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_VARIATION_ERROR"); } }
		public static string CoordinatePageDuplicationError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_DUPLICATION_ERROR"); } }
		public static string CoordinatePageCategoryError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_CATEGORY_ERROR"); } }
		public static string CoordinatePageStaffUnfindError { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_STAFF_UNFIND_ERROR"); } }
		public static string CoordinatePageDatetimeNoTerm { get { return Get("ERRMSG_MANAGER_COORDINATEPAGE_DATETIME_NO_TERM"); } }
		/// <summary>コーディネートカテゴリエラー</summary>
		public static string CoordinateCategoryDuplicateError { get { return Get("ERRMSG_MANAGER_COORDINATECATEGORY_DUPLICATE_ERROR"); } }
		/// <summary>XML構文エラー</summary>
		public static string ContentsManagerXmlFormatError { get { return Get("ERRMSG_MANAGER_CONTENTSMANAGER_XML_FORMAT_ERROT"); } }
		/// <summary>メニュー権限情報デフォルトページ選択エラー</summary>
		public static string MenuauthorityNoDefaltPage { get { return Get("ERRMSG_MANAGER_MENUAUTHORITY_NO_DEFALT_PAGE"); } }
		/// <summary>メニュー権限情報削除不可エラー</summary>
		public static string MenuauthorityDeleteImpossibleError { get { return Get("ERRMSG_MANAGER_MENUAUTHORITY_DELETE_IMPOSSIBLE_ERROR"); } }

		/// <summary>ファイル更新エラー</summary>
		public static string FileUpdateError { get { return Get("ERRMSG_MANAGER_FILE_UPDATE_ERROR"); } }
		/// <summary>ファイル削除エラー</summary>
		public static string FileDeleteError { get { return Get("ERRMSG_MANAGER_FILE_DELETE_ERROR"); } }
		/// <summary>ファイル非存在エラー</summary>
		public static string FileUnFindError { get { return Get("ERRMSG_MANAGER_FILE_UNFIND_ERROR"); } }
		/// <summary>ファイル読み取り専用エラー</summary>
		public static string FileReadOnlyError { get { return Get("ERRMSG_MANAGER_FILE_READONLY_ERROR"); } }

		/// <summary>マスタ出力未選択エラー</summary>
		public static string MasterexportSettingSettingIdNotSelected { get { return Get("ERRMSG_MASTEREXPORTSETTING_SETTING_ID_NOT_SELECTED"); } }
		/// <summary>マスタ出力フィールド不正</summary>
		public static string CsvOutputFieldError { get { return Get("ERRMSG_MANAGER_CSV_OUTPUT_FIELD_ERROR"); } }
		/// <summary>マスタ出力区分不正</summary>
		public static string CsvOutputKbnError { get { return Get("ERRMSG_MANAGER_CSV_OUTPUT_KBN_ERROR"); } }
		/// <summary>マスタ出力時データなし</summary>
		public static string MasterExportSettingNoData { get { return Get("ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA"); } }
		/// <summary>マスタ出力時件数超過</summary>
		public static string MasterExportExcelOverCapacity { get { return Get("ERRMSG_MANAGER_MASTEREXPORT_EXCEL_OVER_CAPACITY"); } }

		/// <summary>必須項目無しエラー</summary>
		public static string InputCheckNecessary { get { return Get("INPUTCHECK_NECESSARY"); } }
		/// <summary>入力チェック 日付</summary>
		public static string InputCheckDateRange { get { return Get("INPUTCHECK_DATERANGE"); } }
		/// <summary>タグマネージャー 出力ページ選択エラー</summary>
		public static string TagManagerSelectPageError { get { return Get("ERRMSG_MANAGER_AFFILIATE_TAG_SELECT_PAGE_ERROR"); } }
		/// <summary>タグマネージャー 出力ページ選択エラー</summary>
		public static string TagManagerSelectLandingPageError { get { return Get("ERRMSG_MANAGER_AFFILIATE_TAG_SELECT_LANDINGPAGE_ERROR"); } }
		/// <summary>タグマネージャー 削除エラー</summary>
		public static string TagManagerSelectCheckedError { get { return Get("ERRMSG_MANAGER_AFFILIATE_PRODUCT_TAG_CHECKED_ERROR"); } }

		/// <summary>タグマネージャー 商品タグ 更新対象なし</summary>
		public static string TagManagerProductTagSelectedError { get { return Get("ERRMSG_MANAGER_AFFILIATE_PRODUCT_TAG_SELECTED_ERROR"); } }
		/// <summary>タグマネージャー 商品タグ 削除不可</summary>
		public static string TagManagerProductTagDeleteError { get { return Get("ERRMSG_MANAGER_AFFILIATE_PRODUCT_TAG_DELETE_ERROR"); } }

		/// <summary>不正パラメータエラー</summary>
		public static string MasterExportSettingIrregularParameterError { get { return Get("ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR"); } }
		/// <summary>不正フィールドエラー</summary>
		public static string MasterExportSettingIrregularFieldsError { get { return Get("ERRMSG_MANAGER_MASTEREXPORTSETTING_FIELDS_ERROR"); } }

		/// <summary>重複エラー</summary>
		public static string InputCheckDuplication { get { return Get("INPUTCHECK_DUPLICATION"); } }
		/// <summary>削除不可エラー</summary>
		public static string InputCheckDeleteValid { get { return Get("INPUTCHECK_DELETE_VALID"); } }

		// マスタファイルアップロード
		public static string MasterUploadFileUnselected { get { return Get("ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNSELECTED"); } }
		public static string MasterUploadAlreadyExists { get { return Get("ERRMSG_MANAGER_MASTERUPLOAD_ALREADY_EXISTS"); } }
		public static string MasterUploadFileNotCsv { get { return Get("ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV"); } }
		public static string MasterUploadFileUnfind { get { return Get("ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNFIND"); } }
		public static string MasterUploadUnloadError { get { return Get("ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_ERROR"); } }

		/// <summary>商品無効</summary>
		public static string ProductInvalid { get { return Get("ERRMSG_MANAGER_PRODUCT_INVALID"); } }
		/// <summary>商品削除</summary>
		public static string ProductDelete { get { return Get("ERRMSG_MANAGER_PRODUCT_DELETE"); } }
		/// <summary>商品重複</summary>
		public static string ProductDuplication { get { return Get("ERRMSG_MANAGER_PRODUCT_DUPLICATION"); } }

		/// <summary>商品ランキング：商品ランキングID入力無しエラー </summary>
		public static string ProductRankingProductIdNoInput { get { return Get("ERRMSG_MANAGER_PRODUCTRANKING_ERRPR_PREFIX"); } }
		/// <summary>商品ランキング：カテゴリマスタ存在なしエラー </summary>
		public static string ProductRankingErrorPrefix { get { return Get("ERRMSG_MANAGER_PRODUCTRANKING_ERRPR_PREFIX"); } }
		/// <summary>商品ランキング：カテゴリマスタ存在なしエラー </summary>
		public static string ProductRankingProductCategoryUnFound { get { return Get("ERRMSG_MANAGER_PRODUCTRANKING_PRODUCT_CATEGORY_UNFOUND"); } }
		/// <summary>商品ランキング：カテゴリID矛盾エラー</summary>
		public static string ProductRankingProductCategoryConflictError { get { return Get("ERRMSG_MANAGER_PRODUCTRANKING_PRODUCT_CATEGORY_CONFLICT_ERROR"); } }
		/// <summary>商品ランキング：商品IDの重複エラー </summary>
		public static string ProductRankingProductIdDuplicationError { get { return Get("ERRMSG_MANAGER_PRODUCTRANKING_PRODUCT_ID_DUPLICATION_ERROR"); } }
		/// <summary>商品ランキング：商品IDデータなしエラー</summary>
		public static string ProductRankingProductIdUnfind { get { return Get("ERRMSG_MANAGER_PRODUCTRANKING_PRODUCT_ID_UNFIND"); } }

		/// <summary>商品グループ：開始日時 > 終了日時エラー</summary>
		public static string ProductGroupDaterangeError { get { return Get("ERRMSG_MANAGER_PRODUCTGROUP_DATERANGE_ERROR"); } }

		/// <summary>商品一覧表示設定：デフォルト指定誤り</summary>
		public static string ProductListDispSettingInconsistentError { get { return Get("ERRMSG_MANAGER_PRODUCTLISTDISPSETTING_INCONSISTENT_ERROR"); } }
		/// <summary>商品一覧表示設定：表示件数重複エラー</summary>
		public static string ProductListDispSettingDispCountRepeatError { get { return Get("ERRMSG_MANAGER_PRODUCTLISTDISPSETTING_DISPCOUNT_REPEAT_ERROR"); } }
		/// <summary>商品一覧表示設定：表示/非表示未設定エラー</summary>
		public static string ProductListDispSettingDisplayNoSettingError { get { return Get("ERRMSG_MANAGER_PRODUCTLISTDISPSETTING_DISPLAY_NO_SETTING_ERROR"); } }

		// ショートURL
		/// <summary>ショートURL形式エラー</summary>
		public static string ShorturlInputExtensionError { get { return Get("ERRMSG_MANAGER_SHORTURL_INPUT_EXTENSION_ERROR"); } }
		/// <summary>ショートURL更新対象なしエラー</summary>
		public static string ShorturlTargetNoSelectedError { get { return Get("ERRMSG_MANAGER_SHORTURL_TARGET_NO_SELECTED_ERROR"); } }

		// フロントアクセス
		/// <summary>基本認証エラー</summary>
		public static string FrontSiteUnAuthorizedError { get { return Get("ERRMSG_MANAGER_CMS_FRONT_SITE_UNAUTHORIZED_ERROR"); } }
		/// <summary>アクセスエラー</summary>
		public static string FrontSiteAccessError { get { return Get("ERRMSG_MANAGER_CMS_FRONT_SITE_ACCESS_ERROR"); } }

		// ページ管理
		/// <summary>デバイス利用チェック</summary>
		public static string PageDesignDeviceUseCheck { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_DEVICE_USE_CHECK"); } }
		/// <summary>ファイル名エラー</summary>
		public static string PageDesignFileNameError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_FILE_NAME_ERROR"); } }
		/// <summary>ファイル名の予約語による使用不可エラー</summary>
		public static string PageDesignFileNameUnusableError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_FILE_NAME_UNUSABLE_ERROR"); } }
		/// <summary>ファイル名の重複エラー</summary>
		public static string PageDesignFileNameDuplicateError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_FILE_NAME_DUPLICATE_ERROR"); } }
		/// <summary> 変更可能レイアウト範囲設定エラー </summary>
		public static string PageDesignFileEditLayoutRangeSettingError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_FILE_EDIT_LAYOUT_RANGE_SETTING_ERROR"); } }
		/// <summary> 変更可能範囲設定エラー </summary>
		public static string PageDesignFileEditRangeSettingError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_FILE_EDIT_RANGE_SETTING_ERROR"); } }
		/// <summary> 変更可能範囲未設定エラー </summary>
		public static string PageDesignFileEditRangeUpdateError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_FILE_EDIT_RANGE_UPDATE_ERROR"); } }
		/// <summary> ページ管理・パーツ管理 最大表示件数超過エラー </summary>
		public static string PageDesignPartsDesignMaxViewOrverError { get { return Get("ERRMSG_MANAGER_PAGEDESIGNPARTSDESIGN_MAX_VIEW_ORVER_ERROR"); } }
		/// <summary> ページ管理 タグ設定エラー </summary>
		public static string PageDesignTagCheckError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_TAG_CHECK_ERROR"); } }
		/// <summary> ページ管理 タグ設定重複エラー </summary>
		public static string PageDesignTagCheckDuplicateError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_TAG_CHECK_DUPLICATE_ERROR"); } }
		/// <summary> ページ管理 ファイル移動エラー </summary>
		public static string PageDesignMoveFileError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_MOVE_FILE_ERROR"); } }
		/// <summary> 変更可能レイアウト範囲更新エラー </summary>
		public static string PageDesignFileEditLayoutRangeUpdateError { get { return Get("ERRMSG_MANAGER_PAGEDESIGN_FILE_EDIT_LAYOUT_RANGE_UPDATE_ERROR"); } }

		// パーツ管理
		/// <summary>パーツのページ利用エラー</summary>
		public static string PartsDesignPartsUseError { get { return Get("ERRMSG_MANAGER_PARTSDESIGN_PARTS_USE_ERROR"); } }

		// 公開範囲
		/// <summary>公開期間エラー</summary>
		public static string ReleaseRangeSettingDateRangeError { get { return Get("ERRMSG_MANAGER_RELEASE_RANGE_SETTING_DATE_RANGE_ERROR"); } }

		// 特集ページ情報
		/// <summary>商品一覧未設定エラー</summary>
		public static string FeaturePageProductListNoSettingError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_PRODUCT_LIST_NO_SETTING_ERROR"); } }
		/// <summary>商品一覧複数設定エラー</summary>
		public static string FeaturePageMultiProductListError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_MULTI_PRODUCT_LIST_ERROR"); } }
		/// <summary>単品訴求の場合、商品は一つだけ設定エラー</summary>
		public static string FeaturePageSingleError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_SINGLE_ERROR"); } }
		/// <summary>カテゴリID重複エラー</summary>
		public static string FeaturePageCategoryDuplicateError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_CATEGORY_DUPLICATE_ERROR"); } }
		/// <summary>カテゴリID未入力エラー</summary>
		public static string FeaturePageCategoryEmptyError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_CATEGORY_EMPTY_ERROR"); } }
		/// <summary>カテゴリID桁数エラー</summary>
		public static string FeaturePageCategoryDigitError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_CATEGORY_DIGIT_ERROR"); } }
		/// <summary>カテゴリID型エラー</summary>
		public static string FeaturePageCategoryTypeError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_CATEGORY_TYPE_ERROR"); } }
		/// <summary>表示順未入力エラー</summary>
		public static string FeaturePageCategoryDisplayOrderEmptyError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_CATEGORY_DISPLAY_ORDER_EMPTY_ERROR"); } }
		/// <summary>表示順桁数エラー</summary>
		public static string FeaturePageCategoryDisplayOrderDigitError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_CATEGORY_DISPLAY_ORDER_DIGIT_ERROR"); } }
		/// <summary>表示順型エラー</summary>
		public static string FeaturePageCategoryDisplayOrderTypeError { get { return Get("ERRMSG_MANAGER_FEATUREPAGE_CATEGORY_DISPLAY_ORDER_TYPE_ERROR"); } }

		// ランディングページ管理
		/// <summary>商品設定なしエラー</summary>
		public static string LandingPageProductNoSelectError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_PRODUCT_NOSELECT"); } }
		/// <summary>頒布会コース設定なしエラー</summary>
		public static string LandingPageSubscriptionBoxCourseNoSelectError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_SUBSCRIPTIONBOXCOURSE_NOSELECT"); } }
		/// <summary>ファイル名重複エラー</summary>
		public static string LandingPageFileNameDuplicate { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_FILE_NAME_DUPLICATE"); } }
		/// <summary>定期購入不可エラー</summary>
		public static string ProductFixedPurchaseDisable { get { return Get("ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_DISABLE"); } }
		/// <summary>定期購入飲みエラー</summary>
		public static string ProductFixedPurchaseOnly { get { return Get("ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_ONLY"); } }
		/// <summary>不正ファイル名エラー</summary>
		public static string LandingPageFileNameInvalidChar { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_FILE_NAME_INVALID_CHAR"); } }
		/// <summary>商品設定数オーバーエラー</summary>
		public static string LandingPageProductOver { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_PRODUCT_OVER"); } }
		/// <summary>商品個数エラー</summary>
		public static string LandingPageProductCountError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_PRODUCT_COUNT"); } }
		/// <summary>商品セット有効な設定なしエラー</summary>
		public static string LandingPageProductSetNoValid { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_PRODUCT_SET_NOVALID"); } }
		/// <summary>消去画像なしエラー</summary>
		public static string LandingPageDeleteFileError { get { return Get("ERRMSG_MANAGER_LANDINGPAGE_DELETE_FILE_ERROR"); } }
		/// <summary>画像なしエラー</summary>
		public static string LandingPageNotFileError { get { return Get("ERRMSG_MANAGER_LANDINGPAGE_NOT_FILE_ERROR"); } }
		/// <summary>商品ID重複エラー</summary>
		public static string LandingPageDuplicationError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_DUPLICATION_ERROR"); } }
		/// <summary>登録最大件数オーバーエラー</summary>
		public static string LandingPageRegisterOverMaxCount { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_REGISTER_OVER_MAX_COUNT_ERROR"); } }
		/// <summary>複製して新規作成した時のOGP画像コピーエラー</summary>
		public static string LandingPageCopyOgpImageError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_COPY_OGP_IMAGE_ERROR"); } }

		// ABテスト
		/// <summary>LPページ設定なしエラー</summary>
		public static string AbTestLandingPageNoSelectError { get { return Get("ERRMSG_MANAGER_AB_TEST_LANDING_PAGE_NOSELECT"); } }
		/// <summary>LPページ設定数過多エラー</summary>
		public static string AbTestLandingPageTooManyError { get { return Get("ERRMSG_MANAGER_AB_TEST_LANDING_PAGE_TOO_MANY"); } }
		/// <summary>ID重複エラー</summary>
		public static string AbTestIdDuplicate { get { return Get("ERRMSG_MANAGER_AB_TEST_ID_DUPLICATE"); } }
		/// <summary>ID形式エラー</summary>
		public static string AbTestIdFormatError { get { return Get("ERRMSG_MANAGER_AB_TEST_ID_FORMAT_ERROR"); } }
		/// <summary>振り分け形式エラー</summary>
		public static string AbTestDistributionFormatError { get { return Get("ERRMSG_MANAGER_AB_TEST_DISTRIBUTION_FORMAT_ERROR"); } }
		/// <summary>振り分け比率エラー</summary>
		public static string AbTestDistributionRateError { get { return Get("ERRMSG_MANAGER_AB_TEST_DISTRIBUTION_RATE_ERROR"); } }

		/// <summary>利用不可決済種別設定エラー</summary>
		public static string LandingPagePaymentSettingError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_PAYMENT_SETTING_ERROR"); } }

		/// <summary>Landing page can not delete cart list page error</summary>
		public static string LandingPageCanNotDeleteCartListLpError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_CANNOT_DELETE_CART_LIST_LP_ERROR"); } }
		/// <summary>Register landing page error</summary>
		public static string LandingPageCanNotRegisterCartListLpError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_CANNOT_REGISTER_CART_LIST_LP_ERROR"); } }
		/// <summary>定期購入カート分離オプションがONの時、商品選択タイプを"一覧指定の商品のうち、複数の商品を選択可能"を選べないエラー</summary>
		public static string LandingPageChcekboxProductChooseTypeConfigurationError { get { return Get("ERRMSG_MANAGER_LANDING_PAGE_CHECKBOX_PRODUCT_CHOOSE_TYPE_CONFIGURATION_ERROR"); } }
		
		// サイトマップ設定
		/// <summary>サイトマップ設定XML解析に失敗</summary>
		public static string SitemapSettingSerializationFailed { get { return Get("ERRMSG_MANAGER_SITEMAP_SETTING_SERIALIZATION_FAILED"); } }

		// 特集エリア
		/// <summary>特集エリアバナー無しエラー</summary>
		public static string FeatureAreaBannerNotSelectedError { get { return Get("ERRMSG_MANAGER_FEATUREAREABANNER_NOTSELECTED_ERROR"); } }
		/// <summary>特集エリアタイプ削除エラー</summary>
		public static string FeatureAreaTypeNotDeletableError { get { return Get("ERRMSG_MANAGER_FEATUREAREATYPE_NOTDELETABLE_ERROR"); } }
		/// <summary>特集エリア画像名称重複エラー</summary>
		public static string FeatureAreaImageDulplicateNameError { get { return Get("ERRMSG_MANAGER_FEATUREAREA_IMAGEDUPLICATENAME_ERROR"); } }

		// サイト基本情報設定
		/// <summary>ID空白エラー</summary>
		public static string SiteInfomationIdEmptyError { get { return Get("ERRMSG_MANAGER_SITE_INFOMATION_ID_EMPTY_ERROR"); } }
		/// <summary>ID重複エラー</summary>
		public static string SiteInfomationIdDuplicateError { get { return Get("ERRMSG_MANAGER_SITE_INFOMATION_ID_DUPLICATE_ERROR"); } }
		/// <summary>IDフォーマットエラー(頭文字に数字を含む)</summary>
		public static string SiteInfomationIdFirstLetterError { get { return Get("ERRMSG_MANAGER_SITE_INFOMATION_ID_FIRST_LETTER_ERROR"); } }
		/// <summary>IDフォーマットエラー(半角英数以外)</summary>
		public static string SiteInfomationIdFormatError { get { return Get("ERRMSG_MANAGER_SITE_INFOMATION_ID_FORMAT_ERROR"); } }
		/// <summary>XML入力値エラー(半角英数以外)</summary>
		public static string SiteInfomationXmlTextError { get { return Get("ERRMSG_MANAGER_SITE_INFOMATION_XML_TEXT_ERROR"); } }
		/// <summary>削除実行エラー</summary>
		public static string SiteInfomationDeleteError { get { return Get("ERRMSG_MANAGER_SITE_INFOMATION_DELETE_ERROR"); } }

		/// <summary>ファイル更新エラー</summary>
		public static string FileOldError { get { return Get("ERRMSG_MANAGER_FILE_DATE_OLD_ERROR"); } }
		/// <summary>ファイル編集中エラー</summary>
		public static string FileOtherOperatorOpeningError { get { return Get("ERRMSG_MANAGER_FILE_OTHER_OPERATOR_OPENING_ERROR"); } }
		/// <summary>ファイル編集権限移譲エラー</summary>
		public static string FileOtherOperatorOpenError { get { return Get("ERRMSG_MANAGER_FILE_OTHER_OPERATOR_OPEN_ERROR"); } }
		/// <summary>ファイル名エラー</summary>
		public static string FileNameError { get { return Get("ERRMSG_MANAGER_FILE_NAME_ERROR"); } }
		/// <summary>CMSオプションOFFエラー</summary>
		public static string CmsOptionDisabledError { get { return Get("ERRMSG_MANAGER_CMS_OPTION_DISABLED"); } }

		/// <summary>Confirm delete message</summary>
		public static string ConfirmDeletedMessage { get { return Get("MSG_CONFIRM_DELETED"); } }
		/// <summary>Registered message</summary>
		public static string RegisteredMessage { get { return Get("MSG_REGISTERED"); } }
		/// <summary>Updated message</summary>
		public static string UpdatedMessage { get { return Get("MSG_UPDATED"); } }
		/// <summary>Deleted message</summary>
		public static string DeletedMessage { get { return Get("MSG_DELETED"); } }
		/// <summary>Copy to clipboard message</summary>
		public static string CopyToClipboardMessage { get { return Get("MSG_COPY_TO_THE_CLIPBOARD"); } }
		/// <summary>Preview is loading</summary>
		public static string PreviewIsLoadingMessage { get { return Get("MSG_PREVIEW_IS_LOADING"); } }
		/// <summary>Failed open page error</summary>
		public static string FailedOpenPageError { get { return Get("ERRMSG_MANAGER_FAILED_OPEN_PAGE_ERROR"); } }
		/// <summary>Scoring sale question item max error</summary>
		public static string ScoringsaleQuestionItemMaxError { get { return Get("ERRMSG_MANAGER_SCORINGSALE_QUESTION_ITEM_MAX_ERROR"); } }
		/// <summary>Scoring sale question not exist error</summary>
		public static string ScoringsaleQuestionNotExistError { get { return Get("ERRMSG_MANAGER_SCORINGSALE_QUESTION_NOT_EXIST_ERROR"); } }
		/// <summary>Scoring sale question page item no select error</summary>
		public static string ScoringsaleQuestionPageItemNoSelectError { get { return Get("ERRMSG_MANAGER_SCORINGSALE_QUESTION_PAGE_ITEM_NOSELECT"); } }
		/// <summary>Scoring sale question choice amount min error</summary>
		public static string ScoringSaleQuestionChoiceAmountMinError { get { return Get("SCORING_SALE_QUESTION_CHOICE_AMOUNT_MIN_ERROR"); } }
		/// <summary>Scoring sale question image duplicate name error</summary>
		public static string ScoringSaleQuestionImageDulplicateNameError { get { return Get("ERRMSG_MANAGER_SCORINGSALEQUESTION_IMAGEDUPLICATENAME_ERROR"); } }
		/// <summary>Scoring sale question set one or more setting error</summary>
		public static string ScoringSaleQuestionSetItemOneOrMoreItemError { get { return Get("ERRMSG_MANAGER_SCORINGSALEQUESTION_SET_ONE_OR_MORE_SETTING_ERROR"); } }
		/// <summary>Score axis set one setting error</summary>
		public static string ScoreAxisSetItemOneItemError { get { return Get("ERRMSG_MANAGER_SCOREAXIS_SET_ONE_SETTING_ERROR"); } }
		/// <summary>Score axis set one or more setting name error</summary>
		public static string ScoreAxisSetItemOneOrMoreItemError { get { return Get("ERRMSG_MANAGER_SCOREAXIS_SET_ONE_OR_MORE_SETTING_NAME_ERROR"); } }
		/// <summary>Scoring sale product set one or more item error</summary>
		public static string ScoringSaleProductSetOneOrMoreItemError { get { return Get("ERRMSG_MANAGER_SCORINGSALEPRODUCT_SET_ONE_OR_MORE_ITEM_ERROR"); } }
		/// <summary>Score axis addition value error</summary>
		public static string ScoreAxisAdditionValueError { get { return Get("ERRMSG_MANAGER_SCOREAXIS_ADDITION_VALUE_ERROR"); } }
		/// <summary>Score axis is not set error</summary>
		public static string ScoreAxisIsNotSetError { get { return Get("ERRMSG_MANAGER_SCOREAXIS_IS_NOT_SET_ERROR"); } }
		/// <summary>Scoring sale question answer type is not set error</summary>
		public static string ScoringSaleQuestionAnswerTypeIsNotSetError { get { return Get("ERRMSG_MANAGER_SCORINGSALEQUESTION_ANSWER_TYPE_IS_NOT_SET_ERROR"); } }

		/// <summary>Product list disp setting sort descriptions</summary>
		public static string[] ProductListDispSettingSortDescriptions
		{
			get
			{
				return new[]
				{
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_SORT_PRODUCT_NAME_KANA_ASC"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_SORT_CREATION_DATE_DESC"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_SORT_SELL_FROM_DATE_DESC"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_SORT_PRICE_ASC"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_SORT_PRICE_DESC"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_SORT_FAVORITE_COUNT_DESC"),
				};
			}
		}
		/// <summary>Product list disp setting image descriptions</summary>
		public static string[] ProductListDispSettingImgDescriptions
		{
			get
			{
				return new[]
				{
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_IMG_VERTICAL_ROW"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_IMG_SIDE_BY_SIDE"),
				};
			}
		}
		/// <summary>Product list disp setting stock descriptions</summary>
		public static string[] ProductListDispSettingStockDescriptions
		{
			get
			{
				return new[]
				{
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_STOCK_SHOW_ALL"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_STOCK_ONLY_IN_STOCK"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_STOCK_PRIORITY_IN_STOCK"),
					Get("MGS_MANAGER_PRODUCTLISTDISPSETTING_DESCRIPTION_STOCK_ONLY_OUT_STOCK"),
				};
			}
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="key">メッセージキー</param>
		/// <returns>エラーメッセージ</returns>
		public static string Get(string key)
		{
			// 必要あらばキャッシュ更新
			RefreshCache();

			// 取得
			m_lock.EnterReadLock();
			try
			{
				// エラーメッセージ取得
				if (m_dicMessages.ContainsKey(key))
				{
					return m_dicMessages[key];
				}
				else if (m_dicMessages.ContainsKey("ERRMSG_SYSTEM_ERROR"))
				{
					return m_dicMessages["ERRMSG_SYSTEM_ERROR"];
				}
			}
			finally
			{
				m_lock.ExitReadLock();
			}
			return "";
		}

		/// <summary>
		/// 更新されていればキャッシュが更新
		/// </summary>
		private static void RefreshCache()
		{
			var xmlFilePaths = Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Where(File.Exists).ToArray();
			var fileUpdateDate = xmlFilePaths.Max(fp => File.GetLastWriteTime(fp));
			if (m_fileLastUpdate < fileUpdateDate)
			{
				m_fileLastUpdate = fileUpdateDate;

				ReadMessagesXml(xmlFilePaths);
			}
		}

		/// <summary>
		/// メッセージをXMLより取得し、ディクショナリへ設定
		/// </summary>
		/// <param name="xmlFilePaths">読み込みXML物理パスのリスト</param>
		/// <returns>取得メッセージ</returns>
		public static void ReadMessagesXml(string[] xmlFilePaths)
		{
			m_lock.EnterWriteLock();
			try
			{
				var xdErrorMessages = new XmlDocument();
				foreach (var xmlPath in xmlFilePaths)
				{
					xdErrorMessages.Load(xmlPath);
					foreach (XmlNode errorMessageNode in xdErrorMessages.SelectSingleNode("ErrorMessages").ChildNodes)
					{
						if (errorMessageNode.NodeType == XmlNodeType.Comment) continue;
						m_dicMessages[errorMessageNode.Name] = errorMessageNode.InnerText;
					}
				}
			}
			finally
			{
				m_lock.ExitWriteLock();
			}
		}
	}
}
