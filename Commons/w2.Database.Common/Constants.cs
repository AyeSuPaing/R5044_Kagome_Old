/*
=========================================================================================================
  Module      :  DB定数定義(共通)(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common;

namespace w2.Database.Common
{
	///*********************************************************************************************
	/// <summary>
	/// DB定数定義(共通)
	/// </summary>
	///*********************************************************************************************
	public partial class Constants : w2.Common.Constants
	{
		/// <summary>汎用フラグオン</summary>
		public const string FLG_ON = "1";
		/// <summary>汎用フラグオフ</summary>
		public const string FLG_OFF = "0";

		// 個別定義
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_EC = FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1;	// メニュー権限1
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_MP = FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2;	// メニュー権限2
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_CS = FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3;	// メニュー権限3
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_CMS = FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL4;	// メニュー権限4

		// 共通
		public const string FIELD_COMMON_BEFORE = "_before";	// 入力前用
		public const string FIELD_COMMON_CONF = "_conf";	// 入力確認用
		public const string FIELD_COMMON_BEGIN_NUM = "bgn_row_num";	// 取得開始番号（ページング用）
		public const string FIELD_COMMON_END_NUM = "end_row_num";	// 取得終了番号（ページング用）
		public const string FIELD_COMMON_ROW_COUNT = "row_count";	// 総件数（ページング用）

		// 商品マスタビュー
		public const string TABLE_PRODUCTVIEW = "w2_ProductView";

		// 為替レートマスタ
		public const string TABLE_EXCHANGERATE = "w2_ExchangeRate";                                 // 為替レートマスタ
		public const string FIELD_EXCHANGERATE_SRC_CURRENCY_CODE = "src_currency_code";             // 通貨コード（元）
		public const string FIELD_EXCHANGERATE_DST_CURRENCY_CODE = "dst_currency_code";             // 通貨コード（先）
		public const string FIELD_EXCHANGERATE_EXCHANGE_RATE = "exchange_rate";                     // 為替レート

		/// <summary>スーパーユーザー名</summary>
		public const string FLG_SUPERUSER_NAME = "スーパーユーザー";
		/// <summary>アクセス権限なしユーザー名</summary>
		public const string FLG_UNACCESSABLEUSER_NAME = "アクセス権限無し";
		/// <summary>メニュー権限レベル</summary>
		public const string FLG_CONDITION_MENU_ACCESS_LEVEL = "condition_menu_access_level";
		/// <summary>ソート順番</summary>
		public const string FLG_SHOPOPERATOR_SORT_KBN = "sort_kbn";
		/// <summary>LIKEエスケープ</summary>
		public const string FLG_LIKE_ESCAPED = "_like_escaped";
		/// <summary>権限なしValue値</summary>
		public const string FLG_SHOPOPERATOR_NO_AUTHORITY_VALUE= "999999";
		/// <summary>権限なし</summary>
		public const string FLG_NO_AUTHORITY_VALUE = "-1";

		/// <summary>オペレータID/昇順</summary>
		public const string FLG_SORT_OPERATOR_LIST_ID_ASC = "0";
		/// <summary>オペレータID/降順</summary>
		public const string FLG_SORT_OPERATOR_LIST_ID_DESC = "1";
		/// <summary>オペレータ名/昇順</summary>
		public const string FLG_SORT_OPERATOR_LIST_NAME_ASC = "2";
		/// <summary>オペレータ名/降順</summary>
		public const string FLG_SORT_OPERATOR_LIST_NAME_DESC = "3";
		/// <summary>不正パラメータ</summary>
		public const string FLG_ERROR_REQUEST_PRAMETER = "err_parameter";
	}
}
