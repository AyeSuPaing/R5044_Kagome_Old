/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.Commerce.Batch.ImportPublicity
  BaseVersion : V5.0
  Author      : M.Yoshimoto
  email       : product@w2solution.co.jp
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  URL         : http://www.w2solution.co.jp/
=========================================================================================================
PKG-V5.0[PF0118] 2011/04/25 M.Yoshimoto     P0001_NatureLab：パブリシティ取込機能対応
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.Batch.ImportPublicity
{
	class Constants : w2.App.Common.Constants
	{
		//========================================================================
		// ディレクトリ・パス系
		//========================================================================
		public static string PC_PHYSICALDIRPATH_IMAGE_FILE = "";
		public static string PC_PHYSICALDIRPATH_TEMPLATE_FILE = "";
		public static string PC_PHYSICALDIRPATH_PUBLICITY_FILE = "";
		public static string SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE = "";
		public static string SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE = "";
		public static string SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE = "";
		public static string MOBILE_PHYSICALDIRPATH_IMAGE_FILE = "";
		public static string PUBLICITY_SETTING_SITE_ID = "";
		public static string PUBLICITY_SETTING_ACCESS_UPDATED_DATE_URL = "";
		public static string PUBLICITY_SETTING_ACCESS_PUBLICITY_XML_URL = "";

		//========================================================================
		// モバイルページID
		//========================================================================
		public static string MOBILE_PUBLICITY_VIEW_TEMPLATE_PAGE_ID = "";
		public static string MOBILE_PUBLICITY_ARCHIVE_TEMPLATE_PAGE_ID = "";
		public static string MOBILE_PUBLICITY_BACKNUMBER_TEMPLATE_PAGE_ID = "";
		public static string MOBILE_PUBLICITY_VIEW_PAGE_ID = "";
		public static string MOBILE_PUBLICITY_ARCHIVE_PAGE_ID = "";
		public static string MOBILE_SITETOP_PAGE_ID = "";
		public static string MOBILE_PUBLICITY_TOP_TEMPLATE_PAGE_ID = "";

		//========================================================================
		// キー
		//========================================================================
		// XML要素キー
		public const string XML_ELEMENT_KEY_DETAIL = "detail";
		public const string XML_ELEMENT_KEY_ID_PUBLICITY = "id_publicity";
		public const string XML_ELEMENT_KEY_PUBLISH_DATE = "pub_date";
		public const string XML_ELEMENT_KEY_DIVISION = "division";
		public const string XML_ELEMENT_KEY_FIXITY_FLG = "fixity_flg";
		public const string XML_ELEMENT_KEY_TITLE = "title";
		public const string XML_ELEMENT_KEY_BODY = "body";
		public const string XML_ELEMENT_KEY_ID_ATTACHMENT1 = "id_attachment1";
		public const string XML_ELEMENT_KEY_THUMNAIL1 = "thumbnail1";
		public const string XML_ELEMENT_KEY_FILE_NAME1 = "file_name1";
		public const string XML_ELEMENT_KEY_FILE_NAME_THUM1 = "file_name_thum1";
		public const string XML_ELEMENT_KEY_ID_ATTACHMENT2 = "id_attachment2";
		public const string XML_ELEMENT_KEY_THUMNAIL2 = "thumbnail2";
		public const string XML_ELEMENT_KEY_FILE_NAME2 = "file_name2";
		public const string XML_ELEMENT_KEY_FILE_NAME_THUM2 = "file_name_thum2";
		public const string XML_ELEMENT_KEY_ANCHOR_TITLE1 = "anchor_title1";
		public const string XML_ELEMENT_KEY_ANCHOR_LINK1 = "anchor_link1";
		public const string XML_ELEMENT_KEY_ANCHOR_TITLE2 = "anchor_title2";
		public const string XML_ELEMENT_KEY_ANCHOR_LINK2 = "anchor_link2";
		public const string XML_ELEMENT_KEY_ANCHOR_TITLE3 = "anchor_title3";
		public const string XML_ELEMENT_KEY_ANCHOR_LINK3 = "anchor_link3";
		public const string XML_ELEMENT_KEY_ANCHOR_TITLE4 = "anchor_title4";
		public const string XML_ELEMENT_KEY_ANCHOR_LINK4 = "anchor_link4";
		public const string XML_ELEMENT_KEY_STATUS_FLG = "status_flg";
		public const string XML_ELEMENT_KEY_DEL_FLG = "del_flg";
		public const string XML_ELEMENT_KEY_CREATED = "created";

		//========================================================================
		// 各区分値
		//========================================================================
		// 新着情報区分
		public const string KBN_PUBLICITY_DIVISION_NEW_ITEM = "1";			// 新商品
		public const string KBN_PUBLICITY_DIVISION_MEDIA = "2";				// メディア
		public const string KBN_PUBLICITY_DIVISION_CAMPAIGN = "3";			// キャンペーン
		public const string KBN_PUBLICITY_DIVISION_ANNOUNCEMENT = "4";		// お知らせ
		public const string KBN_PUBLICITY_DIVISION_OTHER = "5";				// その他

		// 固定項目決定フラグ
		public const string KBN_PUBLICITY_FIXITY_FLG_UNFIXED = "0";			// 固定しない
		public const string KBN_PUBLICITY_FIXITY_FLG_FIXED = "1";			// 固定

		// 削除フラグ
		public const string KBN_PUBLICITY_DELETE_FLG_NOT_DELETED = "0";		// 現行
		public const string KBN_PUBLICITY_DELETE_FLG_DELETED = "1";			// 削除済み

	}
}
