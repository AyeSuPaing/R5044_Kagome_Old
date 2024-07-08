/*
=========================================================================================================
  Module      : DB定数定義Cms部分(Constants_Cms.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Database.Common
{
	///*********************************************************************************************
	/// <summary>
	/// DB定数定義CustomerSupport部分
	/// </summary>
	///*********************************************************************************************
	public partial class Constants : w2.Common.Constants
	{
		#region テーブル・フィールド定数

		// スタッフ
		public const string TABLE_STAFF = "w2_Staff";                                               // スタッフ
		public const string FIELD_STAFF_STAFF_ID = "staff_id";                                      // スタッフID
		public const string FIELD_STAFF_STAFF_NAME = "staff_name";                                  // 氏名
		public const string FIELD_STAFF_STAFF_PROFILE = "staff_profile";                            // プロフィールテキスト
		public const string FIELD_STAFF_STAFF_HEIGHT = "staff_height";                              // 身長
		public const string FIELD_STAFF_STAFF_INSTAGRAM_ID = "staff_instagram_id";                  // インスタグラムID
		public const string FIELD_STAFF_STAFF_SEX = "staff_sex";                                    // 性別
		public const string FIELD_STAFF_OPERATOR_ID = "operator_id";                                // オペレータID
		public const string FIELD_STAFF_REAL_SHOP_ID = "real_shop_id";                              // リアル店舗ID
		public const string FIELD_STAFF_VALID_FLG = "valid_flg";                                    // 有効フラグ
		public const string FIELD_STAFF_MODEL_FLG = "model_flg";                                  // モデルフラグ
		public const string FIELD_STAFF_DATE_CREATED = "date_created";                              // 作成日
		public const string FIELD_STAFF_DATE_CHANGED = "date_changed";                              // 更新日
		public const string FIELD_STAFF_LAST_CHANGED = "last_changed";                              // 最終更新者

		// コーディネートマスタ
		public const string TABLE_COORDINATE = "w2_Coordinate";                                     // コーディネートマスタ
		public const string FIELD_COORDINATE_COORDINATE_ID = "coordinate_id";                       // コーディネートID
		public const string FIELD_COORDINATE_COORDINATE_TITLE = "coordinate_title";                 // コーディネートタイトル
		public const string FIELD_COORDINATE_COORDINATE_URL = "coordinate_url";                     // コーディネートURL
		public const string FIELD_COORDINATE_COORDINATE_SUMMARY = "coordinate_summary";             // コーディネート概要
		public const string FIELD_COORDINATE_INTERNAL_MEMO = "internal_memo";                       // 内部用メモ
		public const string FIELD_COORDINATE_STAFF_ID = "staff_id";                                 // スタッフID
		public const string FIELD_COORDINATE_REAL_SHOP_ID = "real_shop_id";                         // リアル店舗ID
		public const string FIELD_COORDINATE_HTML_TITLE = "html_title";                             // タイトル
		public const string FIELD_COORDINATE_METADATA_KEYWORDS = "metadata_keywords";                 // キーワード
		public const string FIELD_COORDINATE_METADATA_DESC = "metadata_desc";                       // ディスクリプション
		public const string FIELD_COORDINATE_DISPLAY_KBN = "display_kbn";                           // 表示区分
		public const string FIELD_COORDINATE_DISPLAY_DATE = "display_date";                         // 表示用更新日
		public const string FIELD_COORDINATE_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_COORDINATE_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_COORDINATE_LAST_CHANGED = "last_changed";                         // 最終更新者

		// コーディネートアイテム
		public const string TABLE_COORDINATEITEM = "w2_CoordinateItem";                               // コーディネートアイテム
		public const string FIELD_COORDINATEITEM_COORDINATE_ID = "coordinate_id";                    // コーディネートID
		public const string FIELD_COORDINATEITEM_ITEM_NO = "item_no";                                // アイテムNO
		public const string FIELD_COORDINATEITEM_ITEM_KBN = "item_kbn";                              // アイテム区分
		public const string FIELD_COORDINATEITEM_ITEM_ID = "item_id";                                // アイテムID
		public const string FIELD_COORDINATEITEM_ITEM_ID2 = "item_id2";                              // アイテムID2
		public const string FIELD_COORDINATEITEM_ITEM_NAME = "item_name";                            // アイテム名
		public const string FIELD_COORDINATEITEM_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_COORDINATEITEM_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_COORDINATEITEM_LAST_CHANGED = "last_changed";                      // 最終更新者

		// コーディネートカテゴリ
		public const string TABLE_COORDINATECATEGORY = "w2_CoordinateCategory";                     // コーディネートカテゴリ
		public const string FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID = "coordinate_category_id";// カテゴリID
		public const string FIELD_COORDINATECATEGORY_COORDINATE_PARENT_CATEGORY_ID = "coordinate_parent_category_id";// 親カテゴリID
		public const string FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME = "coordinate_category_name";// カテゴリ名
		public const string FIELD_COORDINATECATEGORY_DISPLAY_ORDER = "display_order";                 // SEOキーワード
		public const string FIELD_COORDINATECATEGORY_SEO_KEYWORDS = "seo_keywords";                 // SEOキーワード
		public const string FIELD_COORDINATECATEGORY_VALID_FLG = "valid_flg";                       // 有効フラグ
		public const string FIELD_COORDINATECATEGORY_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_COORDINATECATEGORY_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_COORDINATECATEGORY_LAST_CHANGED = "last_changed";                 // 最終更新者

		// 特集画像グループマスタ
		public const string TABLE_FEATUREIMAGEGROUP = "w2_FeatureImageGroup";                         // 特集画像グループマスタ
		public const string FIELD_FEATUREIMAGEGROUP_FEATURE_IMAGE_GROUP_ID = "feature_image_group_id";              // 特集画像グループIDID
		public const string FIELD_FEATUREIMAGEGROUP_GROUP_NAME = "group_name";                        // グループ名
		public const string FIELD_FEATUREIMAGEGROUP_GROUP_SORT_NUMBER = "group_sort_number";          // グループ順序
		public const string FIELD_FEATUREIMAGEGROUP_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_FEATUREIMAGEGROUP_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_FEATUREIMAGEGROUP_LAST_CHANGED = "last_changed";                    // 最終更新者

		// 特集画像マスタ
		public const string TABLE_FEATUREIMAGE = "w2_FeatureImage";                                   // ページデザイン ページ管理
		public const string FIELD_FEATUREIMAGE_IMAGE_ID = "image_id";                                 // 画像ID
		public const string FIELD_FEATUREIMAGE_FILE_NAME = "file_name";                               // ファイル名(拡張子含む)
		public const string FIELD_FEATUREIMAGE_FILE_DIR_PATH = "file_dir_path";                       // ディレクトリパス
		public const string FIELD_FEATUREIMAGE_GROUP_ID = "feature_image_group_id";                   // 特集画像グループIDID
		public const string FIELD_FEATUREIMAGE_IMAGE_SORT_NUMBER = "image_sort_number";               // グループ内画像順序
		public const string FIELD_FEATUREIMAGE_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_FEATUREIMAGE_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_FEATUREIMAGE_LAST_CHANGED = "last_changed";                         // 最終更新者

		// 特集カテゴリマスタ
		public const string TABLE_FEATURECATEGORY = "w2_FeatureCategory";                                   // 特集カテゴリマスタ
		public const string FIELD_FEATURECATEGORY_CATEGORY_ID = "feature_category_id";                      // 特集カテゴリID
		public const string FIELD_FEATURECATEGORY_PARENT_CATEGORY_ID = "feature_parent_category_id";        // 特集親カテゴリID
		public const string FIELD_FEATURECATEGORY_CATEGORY_NAME = "feature_category_name";                  // 特集カテゴリ名

		// 特集エリアマスタ
		public const string TABLE_FEATUREAREA = "w2_FeatureArea";                                   // 特集エリアマスタ
		public const string FIELD_FEATUREAREA_AREA_ID = "area_id";                                  // 特集エリアID
		public const string FIELD_FEATUREAREA_AREA_NAME = "area_name";                              // 特集エリア名
		public const string FIELD_FEATUREAREA_AREA_TYPE_ID = "area_type_id";                        // 特集エリアタイプ
		public const string FIELD_FEATUREAREA_INTERNAL_MEMO = "internal_memo";                      // 内部用メモ
		public const string FIELD_FEATUREAREA_SIDE_MAX_COUNT = "side_max_count";                    // 横並び最大数
		public const string FIELD_FEATUREAREA_SIDE_TURN = "side_turn";                              // 折り返し設定
		public const string FIELD_FEATUREAREA_SLIDER_COUNT = "slider_count";                        // スライド数
		public const string FIELD_FEATUREAREA_SLIDER_SCROLL_COUNT = "slider_scroll_count";          // スクロールのスライド数
		public const string FIELD_FEATUREAREA_SLIDER_SCROLL_AUTO = "slider_scroll_auto";            // 自動スクロール
		public const string FIELD_FEATUREAREA_SLIDER_SCROLL_INTERVAL = "slider_scroll_interval";    // 自動スクロールの間隔
		public const string FIELD_FEATUREAREA_SLIDER_ARROW = "slider_arrow";                        // 矢印表示
		public const string FIELD_FEATUREAREA_SLIDER_DOT = "slider_dot";                            // ドット表示
		public const string FIELD_FEATUREAREA_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_FEATUREAREA_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_FEATUREAREA_LAST_CHANGED = "last_changed";                        // 最終更新者

		// 特集エリアバナー
		public const string TABLE_FEATUREAREABANNER = "w2_FeatureAreaBanner";                       // 特集エリアバナー
		public const string FIELD_FEATUREAREABANNER_AREA_ID = "area_id";                            // 特集エリアID
		public const string FIELD_FEATUREAREABANNER_BANNER_NO = "banner_no";                        // 特集エリアバナーNo
		public const string FIELD_FEATUREAREABANNER_FILE_NAME = "file_name";                        // ファイル名(拡張子含む)
		public const string FIELD_FEATUREAREABANNER_FILE_DIR_PATH = "file_dir_path";                // ディレクトリパス
		public const string FIELD_FEATUREAREABANNER_ALT_TEXT = "alt_text";                          // 代替テキスト
		public const string FIELD_FEATUREAREABANNER_TEXT = "text";                                  // テキスト
		public const string FIELD_FEATUREAREABANNER_LINK_URL = "link_url";                          // リンク
		public const string FIELD_FEATUREAREABANNER_WINDOW_TYPE = "window_type";                    // ウィンドウタイプ
		public const string FIELD_FEATUREAREABANNER_PUBLISH = "publish";                            // 公開状態
		public const string FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_FROM = "condition_publish_date_from";// 公開範囲:公開開始日
		public const string FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_TO = "condition_publish_date_to";// 公開範囲:公開終了日
		public const string FIELD_FEATUREAREABANNER_CONDITION_MEMBER_ONLY_TYPE = "condition_member_only_type";// 公開範囲:会員限定コンテンツ
		public const string FIELD_FEATUREAREABANNER_CONDITION_MEMBER_RANK_ID = "condition_member_rank_id";// 公開範囲:一部会員に公開する場合の会員ランク
		public const string FIELD_FEATUREAREABANNER_CONDITION_TARGET_LIST_TYPE = "condition_target_list_type";// 公開範囲:ターゲットリスト検索条件
		public const string FIELD_FEATUREAREABANNER_CONDITION_TARGET_LIST_IDS = "condition_target_list_ids";// 公開範囲:対象ターゲットリスト
		public const string FIELD_FEATUREAREABANNER_DATE_CREATED = "date_created";                  // 作成日

		// 特集エリアタイプマスタ
		public const string TABLE_FEATUREAREATYPE = "w2_FeatureAreaType";                           // 特集エリアタイプマスタ
		public const string FIELD_FEATUREAREATYPE_AREA_TYPE_ID = "area_type_id";                    // 特集エリアタイプID
		public const string FIELD_FEATUREAREATYPE_AREA_TYPE_NAME = "area_type_name";                // 特集エリアタイプ名
		public const string FIELD_FEATUREAREATYPE_ACTION_TYPE = "action_type";                      // 動作タイプ
		public const string FIELD_FEATUREAREATYPE_FILE_NAME = "file_name";                          // ファイル名(拡張子含む)
		public const string FIELD_FEATUREAREATYPE_INTERNAL_MEMO = "internal_memo";                  // 内部用メモ
		public const string FIELD_FEATUREAREATYPE_PC_START_TAG = "pc_start_tag";                    // PC開始タグ
		public const string FIELD_FEATUREAREATYPE_PC_REPEAT_TAG = "pc_repeat_tag";                  // PC繰り返しタグ
		public const string FIELD_FEATUREAREATYPE_PC_END_TAG = "pc_end_tag";                        // PC終了タグ
		public const string FIELD_FEATUREAREATYPE_PC_SCRIPT_TAG = "pc_script_tag";                  // PCスクリプトタグ
		public const string FIELD_FEATUREAREATYPE_SP_START_TAG = "sp_start_tag";                    // SP開始タグ
		public const string FIELD_FEATUREAREATYPE_SP_REPEAT_TAG = "sp_repeat_tag";                  // SP繰り返しタグ
		public const string FIELD_FEATUREAREATYPE_SP_END_TAG = "sp_end_tag";                        // SP終了タグ
		public const string FIELD_FEATUREAREATYPE_SP_SCRIPT_TAG = "sp_script_tag";                  // SPスクリプトタグ
		public const string FIELD_FEATUREAREATYPE_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_FEATUREAREATYPE_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_FEATUREAREATYPE_LAST_CHANGED = "last_changed";                    // 最終更新者

		// ページデザイン グループマスタ
		public const string TABLE_PAGEDESIGNGROUP = "w2_PageDesignGroup";                           // ページデザイン グループマスタ
		public const string FIELD_PAGEDESIGNGROUP_GROUP_ID = "group_id";                            // 識別ID
		public const string FIELD_PAGEDESIGNGROUP_GROUP_NAME = "group_name";                      // グループ名
		public const string FIELD_PAGEDESIGNGROUP_GROUP_SORT_NUMBER = "group_sort_number";          // グループ順序
		public const string FIELD_PAGEDESIGNGROUP_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_PAGEDESIGNGROUP_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_PAGEDESIGNGROUP_LAST_CHANGED = "last_changed";                    // 最終更新者

		// ページデザイン ページ管理
		public const string TABLE_PAGEDESIGN = "w2_PageDesign";                                     // ページデザイン ページ管理
		public const string FIELD_PAGEDESIGN_PAGE_ID = "page_id";                                   // 識別ID
		public const string FIELD_PAGEDESIGN_MANAGEMENT_TITLE = "management_title";                 // 管理用タイトル
		public const string FIELD_PAGEDESIGN_PAGE_TYPE = "page_type";                               // タイプ
		public const string FIELD_PAGEDESIGN_FILE_NAME = "file_name";                               // ファイル名(拡張子含む)
		public const string FIELD_PAGEDESIGN_PC_FILE_DIR_PATH = "pc_file_dir_path";                 // PCディレクトリパス
		public const string FIELD_PAGEDESIGN_GROUP_ID = "group_id";                                 // グループ識別ID
		public const string FIELD_PAGEDESIGN_PAGE_SORT_NUMBER = "page_sort_number";                 // グループ内ページ順序
		public const string FIELD_PAGEDESIGN_USE_TYPE = "use_type";                                 // ページの利用状態
		public const string FIELD_PAGEDESIGN_PUBLISH = "publish";                                   // ページの公開状態
		public const string FIELD_PAGEDESIGN_CONDITION_PUBLISH_DATE_FROM = "condition_publish_date_from";// 公開範囲:公開開始日
		public const string FIELD_PAGEDESIGN_CONDITION_PUBLISH_DATE_TO = "condition_publish_date_to";// 公開範囲:公開終了日
		public const string FIELD_PAGEDESIGN_CONDITION_MEMBER_ONLY_TYPE = "condition_member_only_type";// 公開範囲:会員限定コンテンツ
		public const string FIELD_PAGEDESIGN_CONDITION_MEMBER_RANK_ID = "condition_member_rank_id"; // 公開範囲:一部会員に公開する場合の会員ランク
		public const string FIELD_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE = "condition_target_list_type";// 公開範囲:ターゲットリスト検索条件
		public const string FIELD_PAGEDESIGN_CONDITION_TARGET_LIST_IDS = "condition_target_list_ids";// 公開範囲:対象ターゲットリスト
		public const string FIELD_PAGEDESIGN_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_PAGEDESIGN_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_PAGEDESIGN_LAST_CHANGED = "last_changed";                         // 最終更新者
		public const string FIELD_PAGEDESIGN_METADATA_DESC = "metadata_desc";                         // ディスクリプション

		// コンテンツログ
		public const string TABLE_CONTENTSLOG = "w2_ContentsLog";                                   // コンテンツログ
		public const string FIELD_CONTENTSLOG_LOG_NO = "log_no";                                    // ログNO
		public const string FIELD_CONTENTSLOG_DATE = "date";                                        // 日付
		public const string FIELD_CONTENTSLOG_REPORT_TYPE = "report_type";                          // レポートタイプ
		public const string FIELD_CONTENTSLOG_ACCESS_KBN = "access_kbn";                            // アクセス区分
		public const string FIELD_CONTENTSLOG_CONTENTS_TYPE = "contents_type";                      // コンテンツタイプ
		public const string FIELD_CONTENTSLOG_CONTENTS_ID = "contents_id";                          // コンテンツID
		public const string FIELD_CONTENTSLOG_PRICE = "price";                                      // 金額
		public const string FIELD_CONTENTSLOG_ORDER_ID = "order_id";                                //注文ID

		// コンテンツ解析
		public const string TABLE_CONTENTSSUMMARYANALYSIS = "w2_ContentsSummaryAnalysis";           // コンテンツ解析
		public const string FIELD_CONTENTSSUMMARYANALYSIS_DATA_NO = "data_no";                      // データNO
		public const string FIELD_CONTENTSSUMMARYANALYSIS_DATE = "date";                            // 日付
		public const string FIELD_CONTENTSSUMMARYANALYSIS_TGT_YEAR = "tgt_year";                    // 対象年
		public const string FIELD_CONTENTSSUMMARYANALYSIS_TGT_MONTH = "tgt_month";                  // 対象月
		public const string FIELD_CONTENTSSUMMARYANALYSIS_TGT_DAY = "tgt_day";                      // 対象日
		public const string FIELD_CONTENTSSUMMARYANALYSIS_REPORT_TYPE = "report_type";              // レポートタイプ
		public const string FIELD_CONTENTSSUMMARYANALYSIS_ACCESS_KBN = "access_kbn";                // アクセス区分
		public const string FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_TYPE = "contents_type";          // コンテンツタイプ
		public const string FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_ID = "contents_id";              // コンテンツID
		public const string FIELD_CONTENTSSUMMARYANALYSIS_COUNT = "count";                          // カウント
		public const string FIELD_CONTENTSSUMMARYANALYSIS_PRICE = "price";                          // 金額


		// パーツデザイン グループマスタ
		public const string TABLE_PARTSDESIGNGROUP = "w2_PartsDesignGroup";                         // パーツデザイン グループマスタ
		public const string FIELD_PARTSDESIGNGROUP_GROUP_ID = "group_id";                           // 識別ID
		public const string FIELD_PARTSDESIGNGROUP_GROUP_NAME = "group_name";                       // グループ名
		public const string FIELD_PARTSDESIGNGROUP_GROUP_SORT_NUMBER = "group_sort_number";         // グループ順序
		public const string FIELD_PARTSDESIGNGROUP_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_PARTSDESIGNGROUP_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_PARTSDESIGNGROUP_LAST_CHANGED = "last_changed";                   // 最終更新者

		// パーツデザイン パーツ管理
		public const string TABLE_PARTSDESIGN = "w2_PartsDesign";                                   // パーツデザイン パーツ管理
		public const string FIELD_PARTSDESIGN_PARTS_ID = "parts_id";                                // 識別ID
		public const string FIELD_PARTSDESIGN_MANAGEMENT_TITLE = "management_title";                // 管理用タイトル
		public const string FIELD_PARTSDESIGN_PARTS_TYPE = "parts_type";                            // タイプ
		public const string FIELD_PARTSDESIGN_FILE_NAME = "file_name";                              // ファイル名(拡張子含む)
		public const string FIELD_PARTSDESIGN_PC_FILE_DIR_PATH = "pc_file_dir_path";                // PCディレクトリパス
		public const string FIELD_PARTSDESIGN_GROUP_ID = "group_id";                                // グループ識別ID
		public const string FIELD_PARTSDESIGN_PARTS_SORT_NUMBER = "parts_sort_number";              // グループ内パーツ順序
		public const string FIELD_PARTSDESIGN_USE_TYPE = "use_type";                                // パーツの利用状態
		public const string FIELD_PARTSDESIGN_PUBLISH = "publish";                                  // パーツの公開状態
		public const string FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_FROM = "condition_publish_date_from";// 公開範囲:公開開始日
		public const string FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_TO = "condition_publish_date_to";// 公開範囲:公開終了日
		public const string FIELD_PARTSDESIGN_CONDITION_MEMBER_ONLY_TYPE = "condition_member_only_type";// 公開範囲:会員限定コンテンツ
		public const string FIELD_PARTSDESIGN_CONDITION_MEMBER_RANK_ID = "condition_member_rank_id";// 公開範囲:一部会員に公開する場合の会員ランク
		public const string FIELD_PARTSDESIGN_CONDITION_TARGET_LIST_TYPE = "condition_target_list_type";// 公開範囲:ターゲットリスト検索条件
		public const string FIELD_PARTSDESIGN_CONDITION_TARGET_LIST_IDS = "condition_target_list_ids";// 公開範囲:対象ターゲットリスト
		public const string FIELD_PARTSDESIGN_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_PARTSDESIGN_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_PARTSDESIGN_LAST_CHANGED = "last_changed";                        // 最終更新者
		public const string FIELD_PARTSDESIGN_AREA_ID = "area_id";                                  // 特集エリアID

		/// <summary>特集ページ情報</summary>
		public const string TABLE_FEATUREPAGE = "w2_FeaturePage";
		/// <summary>特集ページID</summary>
		public const string FIELD_FEATUREPAGE_FEATURE_PAGE_ID = "feature_page_id";
		/// <summary>管理用タイトル</summary>
		public const string FIELD_FEATUREPAGE_MANAGEMENT_TITLE = "management_title";
		/// <summary>特集ページタイプ</summary>
		public const string FIELD_FEATUREPAGE_PAGE_TYPE = "page_type";
		/// <summary>特集ページカテゴリID</summary>
		public const string FIELD_FEATUREPAGE_CATEGORY_ID = "category_id";
		/// <summary>ブランドID</summary>
		public const string FIELD_FEATUREPAGE_PERMITTED_BRAND_IDS = "permitted_brand_ids";
		/// <summary>ファイル名(拡張子含む)</summary>
		public const string FIELD_FEATUREPAGE_FILE_NAME = "file_name";
		/// <summary>PCディレクトリパス</summary>
		public const string FIELD_FEATUREPAGE_FILE_DIR_PATH = "file_dir_path";
		/// <summary>ページ順序</summary>
		public const string FIELD_FEATUREPAGE_PAGE_SORT_NUMBER = "page_sort_number";
		/// <summary>ページの利用状態</summary>
		public const string FIELD_FEATUREPAGE_USE_TYPE = "use_type";
		/// <summary>ページの公開状態</summary>
		public const string FIELD_FEATUREPAGE_PUBLISH = "publish";
		/// <summary>ページタイトル</summary>
		public const string FIELD_FEATUREPAGE_HTML_PAGE_TITLE = "html_page_title";
		/// <summary>ディスクリプション</summary>
		public const string FIELD_FEATUREPAGE_METADATA_DESC = "metadata_desc";
		/// <summary>公開範囲:公開開始日</summary>
		public const string FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_FROM = "condition_publish_date_from";
		/// <summary>公開範囲:公開終了日</summary>
		public const string FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_TO = "condition_publish_date_to";
		/// <summary>公開範囲:会員限定コンテンツ</summary>
		public const string FIELD_FEATUREPAGE_CONDITION_MEMBER_ONLY_TYPE = "condition_member_only_type";
		/// <summary>公開範囲:一部会員に公開する場合の会員ランク</summary>
		public const string FIELD_FEATUREPAGE_CONDITION_MEMBER_RANK_ID = "condition_member_rank_id";
		/// <summary>公開範囲:ターゲットリスト検索条件</summary>
		public const string FIELD_FEATUREPAGE_CONDITION_TARGET_LIST_TYPE = "condition_target_list_type";
		/// <summary>公開範囲:対象ターゲットリスト</summary>
		public const string FIELD_FEATUREPAGE_CONDITION_TARGET_LIST_IDS = "condition_target_list_ids";
		/// <summary>作成日</summary>
		public const string FIELD_FEATUREPAGE_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_FEATUREPAGE_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_FEATUREPAGE_LAST_CHANGED = "last_changed";

		/// <summary>特集ページコンテンツ</summary>
		public const string TABLE_FEATUREPAGECONTENTS = "w2_FeaturePageContents";
		/// <summary>特集ページID</summary>
		public const string FIELD_FEATUREPAGECONTENTS_FEATURE_PAGE_ID = "feature_page_id";
		/// <summary>PCか</summary>
		public const string FIELD_FEATUREPAGECONTENTS_CONTENTS_KBN = "contents_kbn";
		/// <summary>コンテンツタイプ</summary>
		public const string FIELD_FEATUREPAGECONTENTS_CONTENTS_TYPE = "contents_type";
		/// <summary>コンテンツ表示順序</summary>
		public const string FIELD_FEATUREPAGECONTENTS_CONTENTS_SORT_NUMBER = "contents_sort_number";
		/// <summary>ページタイトル</summary>
		public const string FIELD_FEATUREPAGECONTENTS_PAGE_TITLE = "page_title";
		/// <summary>代替テキスト</summary>
		public const string FIELD_FEATUREPAGECONTENTS_ALT_TEXT = "alt_text";
		/// <summary>商品グループID</summary>
		public const string FIELD_FEATUREPAGECONTENTS_PRODUCT_GROUP_ID = "product_group_id";
		/// <summary>商品一覧タイトル</summary>
		public const string FIELD_FEATUREPAGECONTENTS_PRODUCT_LIST_TITLE = "product_list_title";
		/// <summary>表示件数</summary>
		public const string FIELD_FEATUREPAGECONTENTS_DISPLAY_NUMBER = "display_number";
		/// <summary>ページ送り</summary>
		public const string FIELD_FEATUREPAGECONTENTS_PAGINATION_FLG = "pagination_flg";
		/// <summary>作成日</summary>
		public const string FIELD_FEATUREPAGECONTENTS_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_FEATUREPAGECONTENTS_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_FEATUREPAGECONTENTS_LAST_CHANGED = "last_changed";

		/// <summary>特集ページカテゴリマスタ</summary>
		public const string TABLE_FEATURE_PAGE_CATEGORY = "w2_FeaturePageCategory";
		/// <summary>店舗ID</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID = "shop_id";
		/// <summary>カテゴリID</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID = "category_id";
		/// <summary>親カテゴリID</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID = "parent_category_id";
		/// <summary>カテゴリ名</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_NAME = "category_name";
		/// <summary>有効フラグ</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_VALID_FLG = "valid_flg";
		/// <summary>表示順</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER = "display_order";
		/// <summary>カテゴリ説明文</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE = "category_outline";
		/// <summary>作成日</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_DATE_CHANGED = "date_changed";
		/// <summary>変更前カテゴリID</summary>
		public const string FIELD_FEATURE_PAGE_CATEGORY_BEFORE_CATEGORY_ID = "before_category_id";

		// コンテンツタグ
		public const string TABLE_CONTENTSTAG = "w2_ContentsTag";                                   // コンテンツタグ
		public const string FIELD_CONTENTSTAG_CONTENTS_TAG_ID = "contents_tag_id";                  // コンテンツタグID
		public const string FIELD_CONTENTSTAG_CONTENTS_TAG_NAME = "contents_tag_name";              // コンテンツタグ名
		public const string FIELD_CONTENTSTAG_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_CONTENTSTAG_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_CONTENTSTAG_LAST_CHANGED = "last_changed";                        // 最終更新者

		// ワークスタッフ
		public const string TABLE_WORKSTAFF = "w2_WorkStaff";                                               // スタッフ
		public const string FIELD_WORKSTAFF_STAFF_ID = "staff_id";                                      // スタッフID
		public const string FIELD_WORKSTAFF_STAFF_NAME = "staff_name";                                  // 氏名
		public const string FIELD_WORKSTAFF_STAFF_PROFILE = "staff_profile";                            // プロフィールテキスト
		public const string FIELD_WORKSTAFF_STAFF_HEIGHT = "staff_height";                              // 身長
		public const string FIELD_WORKSTAFF_STAFF_INSTAGRAM_ID = "staff_instagram_id";                  // インスタグラムID
		public const string FIELD_WORKSTAFF_STAFF_SEX = "staff_sex";                                    // 性別
		public const string FIELD_WORKSTAFF_OPERATOR_ID = "operator_id";                                // オペレータID
		public const string FIELD_WORKSTAFF_REAL_SHOP_ID = "real_shop_id";                              // リアル店舗ID
		public const string FIELD_WORKSTAFF_VALID_FLG = "valid_flg";                                    // 有効フラグ
		public const string FIELD_WORKSTAFF_MODEL_FLG = "model_flg";                                  // モデルフラグ
		public const string FIELD_WORKSTAFF_DATE_CREATED = "date_created";                              // 作成日
		public const string FIELD_WORKSTAFF_DATE_CHANGED = "date_changed";                              // 更新日
		public const string FIELD_WORKSTAFF_LAST_CHANGED = "last_changed";                              // 最終更新者

		// ワークコーディネートマスタ
		public const string TABLE_WORKCOORDINATE = "w2_WorkCoordinate";                                     // コーディネートマスタ
		public const string FIELD_WORKCOORDINATE_COORDINATE_ID = "coordinate_id";                       // コーディネートID
		public const string FIELD_WORKCOORDINATE_COORDINATE_TITLE = "coordinate_title";                 // コーディネートタイトル
		public const string FIELD_WORKCOORDINATE_COORDINATE_URL = "coordinate_url";                     // コーディネートURL
		public const string FIELD_WORKCOORDINATE_COORDINATE_SUMMARY = "coordinate_summary";             // コーディネート概要
		public const string FIELD_WORKCOORDINATE_INTERNAL_MEMO = "internal_memo";                       // 内部用メモ
		public const string FIELD_WORKCOORDINATE_STAFF_ID = "staff_id";                                 // スタッフID
		public const string FIELD_WORKCOORDINATE_REAL_SHOP_ID = "real_shop_id";                         // リアル店舗ID
		public const string FIELD_WORKCOORDINATE_HTML_TITLE = "html_title";                             // タイトル
		public const string FIELD_WORKCOORDINATE_METADATA_KEYWORDS = "metadata_keywords";                 // キーワード
		public const string FIELD_WORKCOORDINATE_METADATA_DESC = "metadata_desc";                       // ディスクリプション
		public const string FIELD_WORKCOORDINATE_DISPLAY_KBN = "display_kbn";                           // 表示区分
		public const string FIELD_WORKCOORDINATE_DISPLAY_DATE = "display_date";                         // 表示用更新日
		public const string FIELD_WORKCOORDINATE_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_WORKCOORDINATE_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_WORKCOORDINATE_LAST_CHANGED = "last_changed";                         // 最終更新者

		// ワークコーディネートアイテム
		public const string TABLE_WORKCOORDINATEITEM = "w2_WorkCoordinateItem";                               // コーディネートアイテム
		public const string FIELD_WORKCOORDINATEITEM_COORDINATE_ID = "coordinate_id";                    // コーディネートID
		public const string FIELD_WORKCOORDINATEITEM_ITEM_NO = "item_no";                                // アイテムNO
		public const string FIELD_WORKCOORDINATEITEM_ITEM_KBN = "item_kbn";                              // アイテム区分
		public const string FIELD_WORKCOORDINATEITEM_ITEM_ID = "item_id";                                // アイテムID
		public const string FIELD_WORKCOORDINATEITEM_ITEM_ID2 = "item_id2";                              // アイテムID2
		public const string FIELD_WORKCOORDINATEITEM_ITEM_NAME = "item_name";                            // アイテム名
		public const string FIELD_WORKCOORDINATEITEM_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_WORKCOORDINATEITEM_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_WORKCOORDINATEITEM_LAST_CHANGED = "last_changed";                      // 最終更新者

		// ワークコーディネートカテゴリ
		public const string TABLE_WORKCOORDINATECATEGORY = "w2_WorkCoordinateCategory";                     // コーディネートカテゴリ
		public const string FIELD_WORKCOORDINATECATEGORY_COORDINATE_CATEGORY_ID = "coordinate_category_id";// カテゴリID
		public const string FIELD_WORKCOORDINATECATEGORY_COORDINATE_PARENT_CATEGORY_ID = "coordinate_parent_category_id";// 親カテゴリID
		public const string FIELD_WORKCOORDINATECATEGORY_COORDINATE_CATEGORY_NAME = "coordinate_category_name";// カテゴリ名
		public const string FIELD_WORKCOORDINATECATEGORY_DISPLAY_ORDER = "display_order";                 // SEOキーワード
		public const string FIELD_WORKCOORDINATECATEGORY_SEO_KEYWORDS = "seo_keywords";                 // SEOキーワード
		public const string FIELD_WORKCOORDINATECATEGORY_VALID_FLG = "valid_flg";                       // 有効フラグ
		public const string FIELD_WORKCOORDINATECATEGORY_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_WORKCOORDINATECATEGORY_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_WORKCOORDINATECATEGORY_LAST_CHANGED = "last_changed";                 // 最終更新者

		// OGPタグ全体設定
		public const string TABLE_OGPTAGSETTING = "w2_OgpTagSetting";                               // OGPタグ全体設定
		public const string FIELD_OGPTAGSETTING_DATA_KBN = "data_kbn";                              // データ区分
		public const string FIELD_OGPTAGSETTING_SITE_TITLE = "site_title";                          // サイト名
		public const string FIELD_OGPTAGSETTING_PAGE_TITLE = "page_title";                          // ページ名
		public const string FIELD_OGPTAGSETTING_DESCRIPTION = "description";                        // ディスクリプション
		public const string FIELD_OGPTAGSETTING_IMAGE_URL = "image_url";                            // 画像URL
		public const string FIELD_OGPTAGSETTING_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_OGPTAGSETTING_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_OGPTAGSETTING_LAST_CHANGED = "last_changed";                      // 最終更新者

		#endregion

		#region フィールドフラグ定数

		// スタッフ：有効フラグ
		public const string FLG_STAFF_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_STAFF_VALID_FLG_INVALID = "0";	// 無効

		// スタッフ: モデルフラグ
		public const string FLG_STAFF_MODEL_FLG_VALID = "1";	// 有効
		public const string FLG_STAFF_MODEL_FLG_INVALID = "0";	// 無効

		// スタッフ：性別
		public const string FLG_STAFF_SEX_MALE = "MALE";		// 男
		public const string FLG_STAFF_SEX_FEMALE = "FEMALE";	// 女
		public const string FLG_STAFF_SEX_UNKNOWN = "UNKNOWN";		// 不明

		// コーディネートカテゴリ：有効フラグ
		public const string FLG_COORDINATECATEGORY_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_COORDINATECATEGORY_VALID_FLG_INVALID = "0";	// 無効

		// コーディネートカテゴリ：ルートカテゴリ
		public const string FLG_COORDINATECATEGORY_ROOT = "ROOT";

		// コーディネートカテゴリ：整形されたカテゴリID
		public const string FLG_COORDINATECATEGORY_FORMATTED_COORDINATE_CATEGORY_ID = "formatted_coordinate_category_id";

		// コーディネート：ページの公開状態
		public const string FLG_COORDINATE_DISPLAY_KBN_PUBLIC = "PUBLIC";	// 公開
		public const string FLG_COORDINATE_DISPLAY_KBN_PRIVATE = "PRIVATE";	// 非公開
		public const string FLG_COORDINATE_DISPLAY_KBN_DRAFT = "DRAFT";	// 下書き

		// コーディネート：紐づけ種類
		public const string FLG_COORDINATE_ITEM_KBN_PRODUCT = "PDT";	// 公開
		public const string FLG_COORDINATE_ITEM_KBN_TAG = "TAG";	// 非公開
		public const string FLG_COORDINATE_ITEM_KBN_CATEGORY = "CTG";	// 下書き

		// コーディネート:リアル店舗名
		public const string FLG_COORDINATE_REAL_SHOP_NAME = "real_shop_name";

		// コーディネート:タグ名
		public const string FLG_COORDINATE_CONTENTS_TAG_NAMES = "contents_tag_names";

		// コーディネート:カテゴリIDs
		public const string FLG_COORDINATE_COORDINATE_CATEGORY_NAMES = "coordinate_category_names";

		// コーディネート:商品IDs
		public const string FLG_COORDINATE_COORDINATE_PRODUCT_IDS = "product_ids";

		// コーディネート:バリエーションIDs
		public const string FLG_COORDINATE_COORDINATE_VARIATION_IDS = "variation_ids";

		// コーディネート：公開日区分
		public const string FLG_COORDINATE_DISPLAY_DATE_KBN = "display_date_kbn";

		// 特集エリア：折り返し
		public const string FLG_FEATUREAREA_SIDE_TURN_VALID = "1";	// 折り返す
		public const string FLG_FEATUREAREA_SIDE_TURN_INVALID = "0";	// 折り返さない

		// 特集エリア：自動スクロール
		public const string FLG_FEATUREAREA_SLIDER_SCROLL_AUTO_VALID = "1";	// 有効
		public const string FLG_FEATUREAREA_SLIDER_SCROLL_AUTO_INVALID = "0";	// 無効

		// 特集エリア：矢印
		public const string FLG_FEATUREAREA_SLIDER_ARROW_VALID = "1";	// 有効
		public const string FLG_FEATUREAREA_SLIDER_ARROW_INVALID = "0";	// 無効

		// 特集エリア：ドット表示
		public const string FLG_FEATUREAREA_SLIDER_DOT_VALID = "1";	// 有効
		public const string FLG_FEATUREAREA_SLIDER_DOT_INVALID = "0";	// 無効

		// 特集エリアバナー：ページの公開状態
		public const string FLG_FEATUREAREABANNER_PUBLISH_PUBLIC = "PUBLIC";	// 公開
		public const string FLG_FEATUREAREABANNER_PUBLISH_PRIVATE = "PRIVATE";	// 非公開

		// 特集エリアバナー：ウィンドウ表示
		public const string FLG_FEATUREAREABANNER_WINDOW_TYPE_POPUP = "POPUP";	// 別ウィンドウで表示
		public const string FLG_FEATUREAREABANNER_WINDOW_TYPE_NONPOPUP = "NONPOPUP ";	// 同一画面で表示

		// 特集エリアタイプ
		public const string FLG_FEATUREAREATYPE_ACTION_TYPE_VERTICAL = "VERTICAL";	// 縦並び
		public const string FLG_FEATUREAREATYPE_ACTION_TYPE_SIDE = "SIDE";	// 横並び
		public const string FLG_FEATUREAREATYPE_ACTION_TYPE_SLIDER = "SLIDER";	// スライダー
		public const string FLG_FEATUREAREATYPE_ACTION_TYPE_RANDOM = "RANDOM";	// ランダム
		public const string FLG_FEATUREAREATYPE_ACTION_TYPE_OTHER = "OTHER";	// その他

		// ページデザイン ページ管理:タイプ
		public const string FLG_PAGEDESIGN_PAGE_TYPE_NORMAL = "NORMAL";	// 標準ページ
		public const string FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM = "CUSTOM";	// カスタムページ
		public const string FLG_PAGEDESIGN_PAGE_TYPE_HTML = "HTML";	// HTMLページ

		// ページデザイン ページ管理:ページの利用状況
		public const string FLG_PAGEDESIGN_USE_TYPE_PC = "PC";	// PCのみ
		public const string FLG_PAGEDESIGN_USE_TYPE_SP = "SP";	// SPのみ
		public const string FLG_PAGEDESIGN_USE_TYPE_PC_SP = "PC_SP";	// PC SP の両方

		// ページデザイン ページ管理:ページの公開状態
		public const string FLG_PAGEDESIGN_PUBLISH_PUBLIC = "PUBLIC";	// 公開
		public const string FLG_PAGEDESIGN_PUBLISH_PRIVATE = "PRIVATE";	// 非公開

		// ページデザイン ページ管理:会員限定コンテンツ
		public const string FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL = "ALL";	// 全ユーザ
		public const string FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_MEMBER_ONLY = "MEMBER_ONLY";	// 会員限定
		public const string FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_PARTIAL_MEMBER_ONLY = "PARTIAL_MEMBER_ONLY";	// 一部会員

		// ページデザイン ページ管理:ターゲットリスト検索条件
		public const string FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_OR = "OR";	// OR条件
		public const string FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_AND = "AND";	// AND条件

		// ページデザイン ページ管理:「その他」グループID
		public const long FLG_PAGEDESIGNGROUP_GROUP_ID_OTHER_ID = 0; //その他」グループID

		// コンテンツログ/コンテンツ解析
		// レポートタイプ
		public const string FLG_CONTENTSLOG_REPORT_TYPE_PV = "PV";	// ページビュー
		public const string FLG_CONTENTSLOG_REPORT_TYPE_CV = "CV";	// コンバージョン
		public const string FLG_CONTENTSLOG_REPORT_TYPE_ODREXCCV = "ODREXCCV";	//注文実行時コンバージョン
		public const string FLG_CONTENTSLOG_REPORT_TYPE_ODRCOMPCV = "ODRCOMPCV";	// 注文完了時コンバージョン

		// アクセス区分
		public const string FLG_CONTENTSLOG_ACCESS_KBN_PC = "PC";	// PC
		public const string FLG_CONTENTSLOG_ACCESS_KBN_SP = "SP";	// スマフォ

		// コンテンツタイプ
		public const string FLG_CONTENTSLOG_CONTENTS_TYPE_COORDINATE = "CDNT";	// コーディネート
		public const string FLG_CONTENTSLOG_CONTENTS_TYPE_LANDINGCART = "LPCT";	// LPカート
		//public const string FLG_CONTENTSLOG_CONTENTS_TYPE_ARTICLE = "ATCL";	// 記事
		public const string FLG_CONTENTSLOG_CONTENTS_TYPE_FEATURE = "FETR";	// 特集
		public const string FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST = "ABTEST";	// ABテスト
		/// <summary>Flag contents log contents type scoring sale</summary>
		public const string FLG_CONTENTSLOG_CONTENTS_TYPE_SCORINGSALE = "SCRG";

		// ページデザイン パーツ管理:タイプ
		public const string FLG_PARTSDESIGN_PARTS_TYPE_NORMAL = "NORMAL";	// 標準
		public const string FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM = "CUSTOM";	// カスタム

		// ページデザイン パーツ管理:パーツの利用状況
		public const string FLG_PARTSDESIGN_USE_TYPE_PC = "PC";	// PCのみ
		public const string FLG_PARTSDESIGN_USE_TYPE_SP = "SP";	// SPのみ
		public const string FLG_PARTSDESIGN_USE_TYPE_PC_SP = "PC_SP";	// PC SP の両方

		// ページデザイン パーツ管理:パーツの公開状態
		public const string FLG_PARTSDESIGN_PUBLISH_PUBLIC = "PUBLIC";	// 公開
		public const string FLG_PARTSDESIGN_PUBLISH_PRIVATE = "PRIVATE";	// 非公開

		// ページデザイン パーツ管理:会員限定コンテンツ
		public const string FLG_PARTSDESIGN_MEMBER_ONLY_TYPE_ALL = "ALL";	// 全ユーザ
		public const string FLG_PARTSDESIGN_MEMBER_ONLY_TYPE_MEMBER_ONLY = "MEMBER_ONLY";	// 会員限定
		public const string FLG_PARTSDESIGN_MEMBER_ONLY_TYPE_PARTIAL_MEMBER_ONLY = "PARTIAL_MEMBER_ONLY";	// 一部会員

		// ページデザイン パーツ管理:ターゲットリスト検索条件
		public const string FLG_PARTSDESIGN_CONDITION_TARGET_LIST_TYPE_OR = "OR";	// OR条件
		public const string FLG_PARTSDESIGN_CONDITION_TARGET_LIST_TYPE_AND = "AND";	// AND条件

		// ページデザイン パーツ管理:「その他」グループID
		public const long FLG_PARTSDESIGNGROUP_GROUP_ID_OTHER_ID = 0; //その他」グループID

		/// <summary>商品一覧訴求</summary>
		public const string FLG_FEATUREPAGE_GROUP = "GROUP";
		/// <summary>複数商品一覧訴求</summary>
		public const string FLG_FEATUREPAGE_MULTI_GROUP = "MULTIGROUP";
		/// <summary>単品訴求</summary>
		public const string FLG_FEATUREPAGE_SINGLE = "SINGLE";

		/// <summary>公開</summary>
		public const string FLG_FEATUREPAGE_PUBLISH_PUBLIC = "PUBLIC";	// 公開
		/// <summary>非公開</summary>
		public const string FLG_FEATUREPAGE_PUBLISH_PRIVATE = "PRIVATE";	// 非公開

		/// <summary>PCのみ</summary>
		public const string FLG_FEATUREPAGE_USE_TYPE_PC = "PC";
		/// <summary>SPのみ</summary>
		public const string FLG_FEATUREPAGE_USE_TYPE_SP = "SP";
		/// <summary>全て利用</summary>
		public const string FLG_FEATUREPAGE_USE_TYPE_ALL = "";

		/// <summary>全ユーザ</summary>
		public const string FLG_FEATUREPAGE_MEMBER_ONLY_TYPE_ALL = "ALL";
		/// <summary>会員限定</summary>
		public const string FLG_FEATUREPAGE_MEMBER_ONLY_TYPE_MEMBER_ONLY = "MEMBER_ONLY";
		/// <summary>一部会員</summary>
		public const string FLG_FEATUREPAGE_MEMBER_ONLY_TYPE_PARTIAL_MEMBER_ONLY = "PARTIAL_MEMBER_ONLY";

		/// <summary>OR条件</summary>
		public const string FLG_FEATUREPAGE_CONDITION_TARGET_LIST_TYPE_OR = "OR";
		/// <summary>AND条件</summary>
		public const string FLG_FEATUREPAGE_CONDITION_TARGET_LIST_TYPE_AND = "AND";

		/// <summary>ページタイトル(テキスト)</summary>
		public const string FLG_FEATUREPAGECONTENTS_TYPE_PAGE_TITLE = "PAGE_TITLE";
		/// <summary>ヘッダーバナー</summary>
		public const string FLG_FEATUREPAGECONTENTS_TYPE_HEADER_BANNER = "HEADER_BANNER";
		/// <summary>コンテンツエリア上部</summary>
		public const string FLG_FEATUREPAGECONTENTS_TYPE_UPPER_CONTENTS_AREA = "UPPER_CONTENTS_AREA";
		/// <summary>商品一覧</summary>
		public const string FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST = "PRODUCT_LIST";
		/// <summary>コンテンツエリア下部</summary>
		public const string FLG_FEATUREPAGECONTENTS_TYPE_LOWER_CONTENTS_AREA = "LOWER_CONTENTS_AREA";

		/// <summary>PC</summary>
		public const string FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC = "1";
		/// <summary>SP</summary>
		public const string FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP = "0";

		/// <summary>あり</summary>
		public const string FLG_FEATUREPAGECONTENTS_PAGINATION_FLG_ON = "1";
		/// <summary>なし</summary>
		public const string FLG_FEATUREPAGECONTENTS_PAGINATION_FLG_OFF = "0";

		// OGPタグ設定:データ区分
		public const string FLG_OGPTAGSETTING_DATA_KBN_DEFAULT_SETTING = "default_setting";				// 全体設定

		// OGPメタデータ：タイプ
		public const string FLG_OGPTAGSETTING_TYPE_WEBSITE = "website";				// ウェブサイト
		public const string FLG_OGPTAGSETTING_TYPE_ARTICLE = "article";				// 記事

		// 並び替えタイプ
		public const string FLG_SORT_TYPE_ASC = "0";	// 昇順
		public const string FLG_SORT_TYPE_DESC = "1";	// 降順

		//ABテスト：ページの公開状態
		public const string FLG_ABTEST_PUBLISH_PUBLIC = "PUBLIC";	// 公開
		public const string FLG_ABTEST_PUBLISH_PRIVATE = "PRIVATE";	// 非公開

		/// <summary>最上位カテゴリ</summary>
		public const string FLG_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY = "root";
		/// <summary>親カテゴリなし</summary>
		public const string FLG_FEATURE_PAGE_CATEGORY_NOT_PARENT_CATEGORY = "親カテゴリなし";
		/// <summary>有効フラグ：有効</summary>
		public const string FLG_FEATURE_PAGE_CATEGORY_VALID_FLG_ON = "1";
		/// <summary>有効フラグ：無効</summary>
		public const string FLG_FEATURE_PAGE_CATEGORY_VALID_FLG_OFF = "0";
		#endregion
	}
}