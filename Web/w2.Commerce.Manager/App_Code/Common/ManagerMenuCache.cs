/*
=========================================================================================================
  Module      : 管理画面メニューキャッシュ(ManagerMenuCache.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Global.Config;
using w2.App.Common.Manager.Menu;

/// <summary>
/// 管理画面メニューキャッシュ
/// </summary>
public class ManagerMenuCache : ManagerMenuCacheBase<ManagerMenuCache>
{
	/// <summary>
	/// オプションステータス取得
	/// </summary>
	/// <returns>メニュー有効状態</returns>
	protected override Dictionary<string, bool> GetOptionStatus()
	{
		return new Dictionary<string, bool>
		{
			{"logistics", Constants.REALSTOCK_OPTION_ENABLED}, // ロジスティクスオプション用
			{"mobile", Constants.MOBILEOPTION_ENABLED},	// モバイルオプション用
			{
				"userintegration",
				(Constants.USERINTEGRATION_OPTION_ENABLED
					&& (Constants.CROSS_POINT_OPTION_ENABLED == false))
			}, // ユーザー統合オプション用
			{"product_review", Constants.PRODUCTREVIEW_ENABLED}, // 商品レビュー機能用
			{"product_set", Constants.PRODUCT_SET_OPTION_ENABLED}, // セット商品オプション用
			{"product_sale", Constants.PRODUCT_SALE_OPTION_ENABLED}, // 商品セールオプション用
			{"setpromotion", Constants.SETPROMOTION_OPTION_ENABLED}, // セットプロモーションオプション用
			{"external_import", Constants.EXTERNAL_IMPORT_OPTION_ENABLED}, // 外部ファイル取込オプション用
			{"fixedpurchase", Constants.FIXEDPURCHASE_OPTION_ENABLED}, // 定期購入オプション用
			{"orderregist", Constants.ORDERREGIST_OPTION_ENABLED}, // 注文登録（電話注文）オプション用
			{"mallcooperation", Constants.MALLCOOPERATION_OPTION_ENABLED}, // モール連携オプション用
			{"product_brand", Constants.PRODUCT_BRAND_ENABLED}, // ブランドオプション用
			{"digitalcontents", Constants.DIGITAL_CONTENTS_OPTION_ENABLED}, // デジタルコンテンツオプション用
			{"support", Constants.COOPERATION_SUPPORT_SITE},	// サポートサイト用
			{"realshop", Constants.REALSHOP_OPTION_ENABLED},	// リアル店舗用
			{"updatepointerrormail", Constants.UPDATE_POINT_ERROR_MAIL_OPTION_ENABLED},	// エラーメールポイント更新
			{"novelty", Constants.NOVELTY_OPTION_ENABLED},	// ノベルティオプション用
			{"recommend", Constants.RECOMMEND_OPTION_ENABLED},	// レコメンド設定オプション用
			{"product_bundle", Constants.PRODUCTBUNDLE_OPTION_ENABLED},	// 商品同梱設定オプション用
			{"memberrank", Constants.MEMBER_RANK_OPTION_ENABLED},	// Member Rank Option
			{"invoicecsv", Constants.INVOICECSV_ENABLED},	// Invoice CSV Option
			{"pdfoutputorderstatement", Constants.PDF_OUTPUT_ORDERSTATEMENT_ENABLED},	// Pdf Output Order Statement Enabled
			{"productgroup", Constants.PRODUCTGROUP_OPTION_ENABLED},	// 商品グループオプション用
			{"order_combine", Constants.ORDER_COMBINE_OPTION_ENABLED}, // 注文同梱設定オプション用
			{"global", Constants.GLOBAL_OPTION_ENABLE},	// グローバルオプション用
			{"translation", GlobalConfigUtil.GlobalTranslationEnabled()},	// 自動翻訳APIオプション用
			{
				"fixedpurchase_combine", 
				(Constants.ORDER_COMBINE_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.FIXED_PURCHASE_COMBINE_OPTION_ENABLED)
			}, // 定期台帳同梱設定オプション用
			{ "fixedpurchase_productchange", Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED }, // 定期商品変更オプション用
			{"receipt", Constants.RECEIPT_OPTION_ENABLED},	// 領収書オプション用
			{"invoice", Constants.TWINVOICE_ENABLED},// Taiwan Invoice Option
			{"global_multi_languages", Constants.GLOBAL_OPTION_ENABLE && (Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Count > 1)}, // 複数言語対応があるオプション用
			{"product_ranking", Constants.PRODUCTRANKING_OPTION_ENABLED}, // 商品ランキング設定用
			{"user_product_arrival_mail", Constants.USERPRODUCTARRIVALMAIL_OPTION_ENABLED}, // 入荷通知メール管理用
			{"product_list_disp_setting", Constants.PRODUCTLISTDISPSETTING_OPTION_ENABLED}, // 商品一覧表示設定用
			{"news_list_disp_setting", Constants.NEWSLISTDISPSETTING_OPTION_ENABLED}, // お知らせ設定用
			{"seo_tag_disp_setting", Constants.SEOTAGDISPSETTING_OPTION_ENABLED}, // SEO設定用
			{"usermanagementlevel_setting", Constants.USERMANAGEMENTLEVELSETTING_OPTION_ENABLED}, // ユーザ管理レベル設定用
			{"usereazyregister_setting", Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED}, // かんたん会員登録設定用
			{"orderextendstatus_setting", Constants.ORDEREXTENDSTATUSSETTING_OPTION_ENABLED}, // 注文拡張ステータス設定用
			{"orderworkflow", Constants.ORDERWORKFLOW_OPTION_ENABLED}, // 受注ワークフロー用
			{"userextend_setting", Constants.USEREXTENDSETTING_OPTION_ENABLED}, // ユーザ拡張項目設定用
			{"targetlist_setting", Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE}, // ターゲットリスト情報設定用
			{"product_stock",Constants.PRODUCT_STOCK_OPTION_ENABLE}, // 在庫情報設定
			{"product_stock_message_setting",Constants.PRODUCT_STOCK_OPTION_ENABLE}, // 在庫文言設定用
			{"product_category",Constants.PRODUCT_CTEGORY_OPTION_ENABLE}, // 商品カテゴリ設定用
			{"product_tag_setting", Constants.PRODUCT_TAG_OPTION_ENABLE}, // 商品タグ設定用
			{"product_subimage_setting", Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE}, // 商品サブ画像設定
			{"shorturl_setting",Constants.SHORTURL_OPTION_ENABLE}, // ショートURL設定
			{"orderextend_setting",Constants.ORDER_EXTEND_OPTION_ENABLED}, // 注文拡張項目
			{"subscriptionbox",Constants.SUBSCRIPTION_BOX_OPTION_ENABLED}, // 頒布会
			{
				"data_migration_setting",
				(Constants.DATAMIGRATION_OPTION_ENABLED
					&& (DateTime.Now <= Constants.DATAMIGRATION_END_DATETIME))
			}, // データ移行設定
			{"option",Constants.OPTIONAPPEAL_ENABLED}, // オプション訴求
			{ "store_pickup_order_list", Constants.STORE_PICKUP_OPTION_ENABLED },
			{ "databinding_master", Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE },
		};
	}

	/// <summary>機能一覧URL</summary>
	public override string PageIndexListUrl
	{
		get { return Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAGE_INDEX_LIST; }
	}
	/// <summary>ログインオペレータメニュー</summary>
	public override IEnumerable<MenuLarge> LoginOperatorMenus
	{
		get { return  (List<MenuLarge>)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
}
