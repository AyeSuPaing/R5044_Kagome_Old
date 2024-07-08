/*
=========================================================================================================
  Module      : 置換タグ管理クラス(ReplaceTagManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace w2.App.Common.Affiliate
{
	/// <summary>置換タグ管理クラス</summary>
	public class ReplaceTagManager
	{
		/// <summary>置換タグデータの取得キー</summary>
		public enum ReplaceTagKey : int
		{
			/// <summary>商品タグ置換タグ</summary>
			PRODUCT,

			/// <summary>商品ID</summary>
			PRODUCT_ID,
			/// <summary>商品バリエーションID</summary>
			VARIATION_ID,
			/// <summary>商品名</summary>
			PRODUCT_NAME,
			/// <summary>商品金額</summary>
			PRODUCT_PRICE,
			/// <summary>商品金額(税抜)</summary>
			PRODUCT_PRICE_TAX_EXCLUDE,
			/// <summary>商品数量</summary>
			QUANTITY,
			/// <summary>商品総額</summary>
			PRODUCT_PRICE_TOTAL,

			/// <summary>[カート]ループ開始</summary>
			CART_LOOP_START,
			/// <summary>[カート]ループ終了</summary>
			CART_LOOP_END,
			/// <summary>[カート]小計</summary>
			CART_PRICE_SUB_TOTAL,
			/// <summary>[カート]小計(税抜)</summary>
			CART_PRICE_SUB_TOTAL_TAX_EXCLUDED,
			/// <summary>[カート]小計 税額</summary>
			CART_PRICE_SUB_TOTAL_TAX,
			/// <summary>[カート]商品数量 合計</summary>
			CART_ITEM_QUANTITY,

			/// <summary>[注文]注文ID</summary>
			ORDER_ID,
			/// <summary>[注文]小計</summary>
			ORDER_PRICE_SUB_TOTAL,
			/// <summary>[注文]小計(税抜)</summary>
			ORDER_PRICE_SUB_TOTAL_EXCLUDED,
			/// <summary>[注文]小計 税額</summary>
			ORDER_PRICE_SUB_TOTAL_TAX,
			/// <summary>[注文]商品数量 合計</summary>
			ORDER_ITEM_QUANTITY,
			/// <summary>[注文]ループ開始</summary>
			ORDER_LOOP_START,
			/// <summary>[注文]ループ終了</summary>
			ORDER_LOOP_END,

			/// <summary>注文者 メールアドレス</summary>
			OWNER_USER_EMAIL,
			/// <summary>注文者 電話番号(国番号あり、ハイフンなし)</summary>
			OWNER_USER_TEL1_WITH_COUNTRY_CODE,
			/// <summary>注文者 性別</summary>
			OWNER_USER_SEX,
			/// <summary>注文者 姓</summary>
			OWNER_USER_FAMILY_NAME,
			/// <summary>注文者 名</summary>
			OWNER_USER_FIRST_NAME,
			/// <summary>注文者 姓(かな)</summary>
			OWNER_USER_FAMILY_NAME_KANA,
			/// <summary>注文者 名(かな)</summary>
			OWNER_USER_FIRST_NAME_KANA,
			/// <summary>注文者 誕生日</summary>
			OWNER_USER_BIRTH_DAY,
			/// <summary>注文者 年齢</summary>
			OWNER_USER_AGE,
			/// <summary>注文者 郵便番号</summary>
			OWNER_USER_ZIP,
			/// <summary>注文者 都道府県</summary>
			OWNER_USER_PREF,
			/// <summary>配送先 姓</summary>
			SHIPPING_USER_FAMILY_NAME,
			/// <summary>配送先 名</summary>
			SHIPPING_USER_FIRST_NAME,
			/// <summary>配送先 姓(かな)</summary>
			SHIPPING_USER_FAMILY_NAME_KANA,
			/// <summary>配送先 名(かな)</summary>
			SHIPPING_USER_FIRST_NAME_KANA,
			/// <summary>配送先 郵便番号</summary>
			SHIPPING_USER_ZIP,
			/// <summary>配送先 都道府県</summary>
			SHIPPING_USER_PREF,

			/// <summary>ログインユーザ ユーザID</summary>
			LOGIN_USER_ID,
			/// <summary>ログインユーザ メールアドレス</summary>
			LOGIN_USER_EMAIL,
			/// <summary>ログインユーザ 電話番号(国番号あり、ハイフンなし)</summary>
			LOGIN_USER_TEL1_WITH_COUNTRY_CODE,
			/// <summary>ログインユーザ 性別</summary>
			LOGIN_USER_SEX,
			/// <summary>ログインユーザ 姓</summary>
			LOGIN_USER_FAMILY_NAME,
			/// <summary>ログインユーザ 名</summary>
			LOGIN_USER_FIRST_NAME,
			/// <summary>ログインユーザ 姓(かな)</summary>
			LOGIN_USER_FAMILY_NAME_KANA,
			/// <summary>ログインユーザ 名(かな)</summary>
			LOGIN_USER_FIRST_NAME_KANA,
			/// <summary>ログインユーザ 誕生日</summary>
			LOGIN_USER_BIRTH_DAY,
			/// <summary>ログインユーザ 年齢</summary>
			LOGIN_USER_AGE,
			/// <summary>ログインユーザ 郵便番号</summary>
			LOGIN_USER_ZIP,
			/// <summary>ログインユーザ 都道府県</summary>
			LOGIN_USER_PREF,

			/// SEO設定
			/// <summary> HTMLタイトル </summary>
			HTML_TITLE,
			/// <summary> ルート親カテゴリ名 </summary>
			ROOT_PARENT_CATEGORY_NAME,
			/// <summary> 親カテゴリ名 </summary>
			PARENT_CATEGORY_NAME,
			/// <summary> カテゴリ名 </summary>
			CATEGORY_NAME,
			/// <summary> 直下の子カテゴリ </summary>
			CHILD_CATEGORY_TOP,
			/// <summary> バリエーション名1 </summary>
			VARIATION_NAME1,
			/// <summary> バリエーション名2 </summary>
			VARIATION_NAME2,
			/// <summary> バリエーション名3 </summary>
			VARIATION_NAME3,
			/// <summary> カラー </summary>
			PRODUCT_COLOR,
			/// <summary> 商品タグ </summary>
			PRODUCT_TAG,
			/// <summary> 商品名（カナ） </summary>
			PRODUCT_NAME_KANA,
			/// <summary> ブランドタイトル </summary>
			BRAND_TITLE,
			/// <summary> ブランドSEOキーワード </summary>
			BRAND_SEO_KEYWORD,
			/// <summary> SEOキーワード </summary>
			SEO_KEYWORDS,
			/// <summary> SEOタイトル </summary>
			SEO_TITLE,
			/// <summary> SEOディスクリプション </summary>
			SEO_DESCRIPTION,
			/// <summary> アイコン1 </summary>
			PRODUCT_ICON1,
			/// <summary> アイコン2 </summary>
			PRODUCT_ICON2,
			/// <summary> アイコン3 </summary>
			PRODUCT_ICON3,
			/// <summary> アイコン4 </summary>
			PRODUCT_ICON4,
			/// <summary> アイコン5 </summary>
			PRODUCT_ICON5,
			/// <summary> アイコン6 </summary>
			PRODUCT_ICON6,
			/// <summary> アイコン7 </summary>
			PRODUCT_ICON7,
			/// <summary> アイコン8 </summary>
			PRODUCT_ICON8,
			/// <summary> アイコン9 </summary>
			PRODUCT_ICON9,
			/// <summary> アイコン10 </summary>
			PRODUCT_ICON10,
			/// <summary> フリーワード </summary>
			FREE_WORD,
			/// <summary> デフォルト文言 </summary>
			DEFAULT_TEXT,

			/// SEO設定
			/// <summary> コーディネート名 </summary>
			TITLE,
			/// <summary> 商品名(カンマ区切り) </summary>
			PRODUCT_NAMES,

			/// <summary> アフィリエイト成果報告用パラメータ </summary>
			AFFILIATE_REPORT,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReplaceTagManager()
		{
			SetCartReplaceTag();
			SetCartOrderProductReplaceTag();
			SetOrderReplaceTag();
			SetLoginUserReplaceTag();
			SetSeoAllReplaceTag();
			SetSeoProductListReplaceTag();
			SetSeoProductDetailReplaceTag();
			SetCoordinateListReplaceTag();
			SetCoordinateDetailReplaceTag();
		}

		/// <summary>
		/// カート用置換タグの設定
		/// </summary>
		private void SetCartReplaceTag()
		{
			this.CartReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>
			{
				{ ReplaceTagKey.CART_LOOP_START, new ReplaceTag(ReplaceTagKey.CART_LOOP_START, "CART_LOOP_START") },
				{ ReplaceTagKey.CART_LOOP_END, new ReplaceTag(ReplaceTagKey.CART_LOOP_END, "CART_LOOP_END") },
				{
					ReplaceTagKey.CART_PRICE_SUB_TOTAL,
					new ReplaceTag(ReplaceTagKey.CART_PRICE_SUB_TOTAL, "CART_PRICE_SUB_TOTAL")
				},
				{
					ReplaceTagKey.CART_PRICE_SUB_TOTAL_TAX_EXCLUDED,
					new ReplaceTag(ReplaceTagKey.CART_PRICE_SUB_TOTAL_TAX_EXCLUDED, "CART_PRICE_SUB_TOTAL_TAX_EXCLUDED")
				},
				{
					ReplaceTagKey.CART_PRICE_SUB_TOTAL_TAX,
					new ReplaceTag(ReplaceTagKey.CART_PRICE_SUB_TOTAL_TAX, "CART_PRICE_SUB_TOTAL_TAX")
				},
				{
					ReplaceTagKey.CART_ITEM_QUANTITY,
					new ReplaceTag(ReplaceTagKey.CART_ITEM_QUANTITY, "CART_ITEM_QUANTITY")
				},
				{ ReplaceTagKey.OWNER_USER_EMAIL, new ReplaceTag(ReplaceTagKey.OWNER_USER_EMAIL, "OWNER_USER_EMAIL") },
				{
					ReplaceTagKey.OWNER_USER_TEL1_WITH_COUNTRY_CODE,
					new ReplaceTag(
						ReplaceTagKey.OWNER_USER_TEL1_WITH_COUNTRY_CODE,
						"OWNER_USER_TEL1_WITH_COUNTRY_CODE")
				},
				{ ReplaceTagKey.OWNER_USER_SEX, new ReplaceTag(ReplaceTagKey.OWNER_USER_SEX, "OWNER_USER_SEX") },
				{
					ReplaceTagKey.OWNER_USER_FAMILY_NAME,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FAMILY_NAME, "OWNER_USER_FAMILY_NAME")
				},
				{
					ReplaceTagKey.OWNER_USER_FIRST_NAME,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FIRST_NAME, "OWNER_USER_FIRST_NAME")
				},
				{
					ReplaceTagKey.OWNER_USER_FAMILY_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FAMILY_NAME_KANA, "OWNER_USER_FAMILY_NAME_KANA")
				},
				{
					ReplaceTagKey.OWNER_USER_FIRST_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FIRST_NAME_KANA, "OWNER_USER_FIRST_NAME_KANA")
				},
				{
					ReplaceTagKey.OWNER_USER_BIRTH_DAY,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_BIRTH_DAY, "OWNER_USER_BIRTH_DAY")
				},
				{
					ReplaceTagKey.OWNER_USER_AGE,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_AGE, "OWNER_USER_AGE")
				},
				{ ReplaceTagKey.OWNER_USER_ZIP, new ReplaceTag(ReplaceTagKey.OWNER_USER_ZIP, "OWNER_USER_ZIP") },
				{ ReplaceTagKey.OWNER_USER_PREF, new ReplaceTag(ReplaceTagKey.OWNER_USER_PREF, "OWNER_USER_PREF") },
				{
					ReplaceTagKey.SHIPPING_USER_FAMILY_NAME,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FAMILY_NAME, "SHIPPING_USER_FAMILY_NAME")
				},
				{
					ReplaceTagKey.SHIPPING_USER_FIRST_NAME,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FIRST_NAME, "SHIPPING_USER_FIRST_NAME")
				},
				{
					ReplaceTagKey.SHIPPING_USER_FAMILY_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FAMILY_NAME_KANA, "SHIPPING_USER_FAMILY_NAME_KANA")
				},
				{
					ReplaceTagKey.SHIPPING_USER_FIRST_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FIRST_NAME_KANA, "SHIPPING_USER_FIRST_NAME_KANA")
				},
				{
					ReplaceTagKey.SHIPPING_USER_ZIP,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_ZIP, "SHIPPING_USER_ZIP")
				},
				{
					ReplaceTagKey.SHIPPING_USER_PREF,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_PREF, "SHIPPING_USER_PREF")
				},

				{ ReplaceTagKey.PRODUCT, new ReplaceTag(ReplaceTagKey.PRODUCT, "PRODUCT") },
			};
		}

		/// <summary>
		/// カート用商品置換タグの設定
		/// </summary>
		private void SetCartOrderProductReplaceTag()
		{
			this.CartProductReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>
			{
				{ ReplaceTagKey.PRODUCT_ID, new ReplaceTag(ReplaceTagKey.PRODUCT_ID, "PRODUCT_ID") },
				{ ReplaceTagKey.VARIATION_ID, new ReplaceTag(ReplaceTagKey.VARIATION_ID, "VARIATION_ID") },
				{ ReplaceTagKey.PRODUCT_NAME, new ReplaceTag(ReplaceTagKey.PRODUCT_NAME, "PRODUCT_NAME") },
				{ ReplaceTagKey.PRODUCT_PRICE, new ReplaceTag(ReplaceTagKey.PRODUCT_PRICE, "PRODUCT_PRICE") },
				{
					ReplaceTagKey.PRODUCT_PRICE_TAX_EXCLUDE,
					new ReplaceTag(ReplaceTagKey.PRODUCT_PRICE_TAX_EXCLUDE, "PRODUCT_PRICE_TAX_EXCLUDE")
				},
				{ ReplaceTagKey.QUANTITY, new ReplaceTag(ReplaceTagKey.QUANTITY, "QUANTITY") },
				{ ReplaceTagKey.PRODUCT_PRICE_TOTAL, new ReplaceTag(ReplaceTagKey.PRODUCT_PRICE_TOTAL, "PRODUCT_PRICE_TOTAL") },
			};
		}

		/// <summary>
		/// 注文用置換タグの設定
		/// </summary>
		private void SetOrderReplaceTag()
		{
			this.OrderReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>
			{
				{ ReplaceTagKey.ORDER_LOOP_START, new ReplaceTag(ReplaceTagKey.ORDER_LOOP_START, "ORDER_LOOP_START") },
				{ ReplaceTagKey.ORDER_LOOP_END, new ReplaceTag(ReplaceTagKey.ORDER_LOOP_END, "ORDER_LOOP_END") },
				{ ReplaceTagKey.ORDER_ID, new ReplaceTag(ReplaceTagKey.ORDER_ID, "ORDER_ID") },
				{
					ReplaceTagKey.ORDER_PRICE_SUB_TOTAL,
					new ReplaceTag(ReplaceTagKey.ORDER_PRICE_SUB_TOTAL, "ORDER_PRICE_SUB_TOTAL")
				},
				{
					ReplaceTagKey.ORDER_PRICE_SUB_TOTAL_EXCLUDED,
					new ReplaceTag(ReplaceTagKey.ORDER_PRICE_SUB_TOTAL_EXCLUDED, "ORDER_PRICE_SUB_TOTAL_EXCLUDED")
				},
				{
					ReplaceTagKey.ORDER_PRICE_SUB_TOTAL_TAX,
					new ReplaceTag(ReplaceTagKey.ORDER_PRICE_SUB_TOTAL_TAX, "ORDER_PRICE_SUB_TOTAL_TAX")
				},
				{
					ReplaceTagKey.ORDER_ITEM_QUANTITY,
					new ReplaceTag(ReplaceTagKey.ORDER_ITEM_QUANTITY, "ORDER_ITEM_QUANTITY")
				},
				{
					ReplaceTagKey.AFFILIATE_REPORT,
					new ReplaceTag(ReplaceTagKey.AFFILIATE_REPORT, "AFFILIATE_REPORT")
				},
				{ ReplaceTagKey.OWNER_USER_EMAIL, new ReplaceTag(ReplaceTagKey.OWNER_USER_EMAIL, "OWNER_USER_EMAIL") },
				{
					ReplaceTagKey.OWNER_USER_TEL1_WITH_COUNTRY_CODE,
					new ReplaceTag(
						ReplaceTagKey.OWNER_USER_TEL1_WITH_COUNTRY_CODE,
						"OWNER_USER_TEL1_WITH_COUNTRY_CODE")
				},
				{ ReplaceTagKey.OWNER_USER_SEX, new ReplaceTag(ReplaceTagKey.OWNER_USER_SEX, "OWNER_USER_SEX") },
				{
					ReplaceTagKey.OWNER_USER_FAMILY_NAME,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FAMILY_NAME, "OWNER_USER_FAMILY_NAME")
				},
				{
					ReplaceTagKey.OWNER_USER_FIRST_NAME,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FIRST_NAME, "OWNER_USER_FIRST_NAME")
				},
				{
					ReplaceTagKey.OWNER_USER_FAMILY_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FAMILY_NAME_KANA, "OWNER_USER_FAMILY_NAME_KANA")
				},
				{
					ReplaceTagKey.OWNER_USER_FIRST_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_FIRST_NAME_KANA, "OWNER_USER_FIRST_NAME_KANA")
				},
				{
					ReplaceTagKey.OWNER_USER_BIRTH_DAY,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_BIRTH_DAY, "OWNER_USER_BIRTH_DAY")
				},
				{
					ReplaceTagKey.OWNER_USER_AGE,
					new ReplaceTag(ReplaceTagKey.OWNER_USER_AGE, "OWNER_USER_AGE")
				},
				{ ReplaceTagKey.OWNER_USER_ZIP, new ReplaceTag(ReplaceTagKey.OWNER_USER_ZIP, "OWNER_USER_ZIP") },
				{ ReplaceTagKey.OWNER_USER_PREF, new ReplaceTag(ReplaceTagKey.OWNER_USER_PREF, "OWNER_USER_PREF") },
				{
					ReplaceTagKey.SHIPPING_USER_FAMILY_NAME,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FAMILY_NAME, "SHIPPING_USER_FAMILY_NAME")
				},
				{
					ReplaceTagKey.SHIPPING_USER_FIRST_NAME,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FIRST_NAME, "SHIPPING_USER_FIRST_NAME")
				},
				{
					ReplaceTagKey.SHIPPING_USER_FAMILY_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FAMILY_NAME_KANA, "SHIPPING_USER_FAMILY_NAME_KANA")
				},
				{
					ReplaceTagKey.SHIPPING_USER_FIRST_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_FIRST_NAME_KANA, "SHIPPING_USER_FIRST_NAME_KANA")
				},
				{
					ReplaceTagKey.SHIPPING_USER_ZIP,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_ZIP, "SHIPPING_USER_ZIP")
				},
				{
					ReplaceTagKey.SHIPPING_USER_PREF,
					new ReplaceTag(ReplaceTagKey.SHIPPING_USER_PREF, "SHIPPING_USER_PREF")
				},

				{ ReplaceTagKey.PRODUCT, new ReplaceTag(ReplaceTagKey.PRODUCT, "PRODUCT") },
			};
		}

		/// <summary>
		/// ログインユーザ用置換タグの設定
		/// </summary>
		private void SetLoginUserReplaceTag()
		{
			this.LoginUserReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>
			{
				{ ReplaceTagKey.LOGIN_USER_ID, new ReplaceTag(ReplaceTagKey.LOGIN_USER_ID, "LOGIN_USER_ID") },
				{ ReplaceTagKey.LOGIN_USER_EMAIL, new ReplaceTag(ReplaceTagKey.LOGIN_USER_EMAIL, "LOGIN_USER_EMAIL") },
				{
					ReplaceTagKey.LOGIN_USER_TEL1_WITH_COUNTRY_CODE,
					new ReplaceTag(
						ReplaceTagKey.LOGIN_USER_TEL1_WITH_COUNTRY_CODE,
						"LOGIN_USER_TEL1_WITH_COUNTRY_CODE")
				},
				{ ReplaceTagKey.LOGIN_USER_SEX, new ReplaceTag(ReplaceTagKey.LOGIN_USER_SEX, "LOGIN_USER_SEX") },
				{
					ReplaceTagKey.LOGIN_USER_FAMILY_NAME,
					new ReplaceTag(ReplaceTagKey.LOGIN_USER_FAMILY_NAME, "LOGIN_USER_FAMILY_NAME")
				},
				{
					ReplaceTagKey.LOGIN_USER_FIRST_NAME,
					new ReplaceTag(ReplaceTagKey.LOGIN_USER_FIRST_NAME, "LOGIN_USER_FIRST_NAME")
				},
				{
					ReplaceTagKey.LOGIN_USER_FAMILY_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.LOGIN_USER_FAMILY_NAME_KANA, "LOGIN_USER_FAMILY_NAME_KANA")
				},
				{
					ReplaceTagKey.LOGIN_USER_FIRST_NAME_KANA,
					new ReplaceTag(ReplaceTagKey.LOGIN_USER_FIRST_NAME_KANA, "LOGIN_USER_FIRST_NAME_KANA")
				},
				{
					ReplaceTagKey.LOGIN_USER_BIRTH_DAY,
					new ReplaceTag(ReplaceTagKey.LOGIN_USER_BIRTH_DAY, "LOGIN_USER_BIRTH_DAY")
				},
				{
					ReplaceTagKey.LOGIN_USER_AGE,
					new ReplaceTag(ReplaceTagKey.LOGIN_USER_AGE, "LOGIN_USER_AGE")
				},
				{ ReplaceTagKey.LOGIN_USER_ZIP, new ReplaceTag(ReplaceTagKey.LOGIN_USER_ZIP, "LOGIN_USER_ZIP") },
				{ ReplaceTagKey.LOGIN_USER_PREF, new ReplaceTag(ReplaceTagKey.LOGIN_USER_PREF, "LOGIN_USER_PREF") },
			};
		}

		/// <summary>
		/// SEO全体設定置換タグの設定
		/// </summary>
		private void SetSeoAllReplaceTag()
		{
			this.SeoAllReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>
			{
				{ ReplaceTagKey.HTML_TITLE, new ReplaceTag(ReplaceTagKey.HTML_TITLE, "html_title", false, true) },
			};
		}

		/// <summary>
		/// SEO商品一覧置換タグの設定
		/// </summary>
		private void SetSeoProductListReplaceTag()
		{
			this.SeoProductListReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>
			{
				{ ReplaceTagKey.ROOT_PARENT_CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.ROOT_PARENT_CATEGORY_NAME, "root_parent_category_name", false, true) },
				{ ReplaceTagKey.PARENT_CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.PARENT_CATEGORY_NAME, "parent_category_name", false, true) },
				{ ReplaceTagKey.CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.CATEGORY_NAME, "category_name", false, true) },
				{ ReplaceTagKey.CHILD_CATEGORY_TOP, new ReplaceTag(ReplaceTagKey.CHILD_CATEGORY_TOP, "child_category_top", false, true) },
				{ ReplaceTagKey.PRODUCT_PRICE, new ReplaceTag(ReplaceTagKey.PRODUCT_PRICE, "product_price", false, true) },
				{ ReplaceTagKey.PRODUCT_COLOR, new ReplaceTag(ReplaceTagKey.PRODUCT_COLOR, "product_color", false, true) },
				{ ReplaceTagKey.PRODUCT_TAG, new ReplaceTag(ReplaceTagKey.PRODUCT_TAG, "product_tag", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON1, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON1, "product_icon1", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON2, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON2, "product_icon2", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON3, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON3, "product_icon3", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON4, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON4, "product_icon4", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON5, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON5, "product_icon5", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON6, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON6, "product_icon6", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON7, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON7, "product_icon7", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON8, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON8, "product_icon8", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON9, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON9, "product_icon9", false, true) },
				{ ReplaceTagKey.PRODUCT_ICON10, new ReplaceTag(ReplaceTagKey.PRODUCT_ICON10, "product_icon10", false, true) },
				{ ReplaceTagKey.FREE_WORD, new ReplaceTag(ReplaceTagKey.FREE_WORD, "free_word", false, true) },
				{ ReplaceTagKey.DEFAULT_TEXT, new ReplaceTag(ReplaceTagKey.DEFAULT_TEXT, "default_text", false, true) },
			};

			if (Constants.PRODUCT_BRAND_ENABLED)
			{
				this.SeoProductListReplaceTags.Add(ReplaceTagKey.BRAND_TITLE, new ReplaceTag(ReplaceTagKey.BRAND_TITLE, "brand_title", false, true));
				this.SeoProductListReplaceTags.Add(ReplaceTagKey.BRAND_SEO_KEYWORD, new ReplaceTag(ReplaceTagKey.BRAND_SEO_KEYWORD, "brand_seo_keyword", false, true));
			}
		}

		/// <summary>
		/// SEO商品詳細置換タグの設定
		/// </summary>
		private void SetSeoProductDetailReplaceTag()
		{
			this.SeoProductDetailReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>
			{
				{ ReplaceTagKey.PRODUCT_NAME, new ReplaceTag(ReplaceTagKey.PRODUCT_NAME, "product_name", false, true) },
				{ ReplaceTagKey.PRODUCT_NAME_KANA, new ReplaceTag(ReplaceTagKey.PRODUCT_NAME_KANA, "product_name_kana", false, true) },
				{ ReplaceTagKey.VARIATION_NAME1, new ReplaceTag(ReplaceTagKey.VARIATION_NAME1, "variation_name1", false, true) },
				{ ReplaceTagKey.VARIATION_NAME2, new ReplaceTag(ReplaceTagKey.VARIATION_NAME2, "variation_name2", false, true) },
				{ ReplaceTagKey.VARIATION_NAME3, new ReplaceTag(ReplaceTagKey.VARIATION_NAME3, "variation_name3", false, true) },
				{ ReplaceTagKey.PRODUCT_PRICE, new ReplaceTag(ReplaceTagKey.PRODUCT_PRICE, "product_price", false, true) },
				{ ReplaceTagKey.ROOT_PARENT_CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.ROOT_PARENT_CATEGORY_NAME, "root_parent_category_name", false, true) },
				{ ReplaceTagKey.PARENT_CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.PARENT_CATEGORY_NAME, "parent_category_name", false, true) },
				{ ReplaceTagKey.CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.CATEGORY_NAME, "category_name", false, true) },
				{ ReplaceTagKey.SEO_KEYWORDS, new ReplaceTag(ReplaceTagKey.SEO_KEYWORDS, "seo_keywords", false, true) },
			};

			if (Constants.PRODUCT_BRAND_ENABLED)
			{
				this.SeoProductDetailReplaceTags.Add(ReplaceTagKey.BRAND_TITLE, new ReplaceTag(ReplaceTagKey.BRAND_TITLE, "brand_title", false, true));
				this.SeoProductDetailReplaceTags.Add(ReplaceTagKey.BRAND_SEO_KEYWORD, new ReplaceTag(ReplaceTagKey.BRAND_SEO_KEYWORD, "brand_seo_keyword", false, true));
			}
		}

		/// <summary>
		/// コーディネート一覧置換タグの設定
		/// </summary>
		private void SetCoordinateListReplaceTag()
		{
			this.CoordinateListReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>()
			{
				{ ReplaceTagKey.PARENT_CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.PARENT_CATEGORY_NAME, "parent_category_name", false, true) },
				{ ReplaceTagKey.CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.CATEGORY_NAME, "category_name", false, true) },
				{ ReplaceTagKey.CHILD_CATEGORY_TOP, new ReplaceTag(ReplaceTagKey.CHILD_CATEGORY_TOP, "child_category_top", false, true) },
			};
		}

		/// <summary>
		/// コーディネート詳細置換タグの設定
		/// </summary>
		private void SetCoordinateDetailReplaceTag()
		{
			this.CoordinateDetailReplaceTags = new Dictionary<ReplaceTagKey, ReplaceTag>()
			{
				{ ReplaceTagKey.TITLE, new ReplaceTag(ReplaceTagKey.TITLE, "title", false, true) },
				{ ReplaceTagKey.PRODUCT_NAMES, new ReplaceTag(ReplaceTagKey.PRODUCT_NAMES, "product_names", false, true) },
				{ ReplaceTagKey.PARENT_CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.PARENT_CATEGORY_NAME, "parent_category_name", false, true) },
				{ ReplaceTagKey.CATEGORY_NAME, new ReplaceTag(ReplaceTagKey.CATEGORY_NAME, "category_name", false, true) },
				{ ReplaceTagKey.SEO_TITLE, new ReplaceTag(ReplaceTagKey.SEO_TITLE, "seo_title", false, true) },
				{ ReplaceTagKey.SEO_DESCRIPTION, new ReplaceTag(ReplaceTagKey.SEO_DESCRIPTION, "seo_description", false, true) },
			};
		}
		/// <summary>
		/// 選択したページから置換タグリストを取得
		/// </summary>
		/// <param name="pages">選択したページリスト</param>
		/// <returns>置換タグリスト</returns>
		public List<string> SelectPageReplaceTagList(List<string> pages)
		{
			var actionType = "";

			if (pages.Count > 0)
			{
				var actionTypeList = pages.Select(
						p => TagSetting.GetInstance().Setting.TargetPages.FirstOrDefault(i => i.Path == p)
							.ActionType)
					.Distinct().ToList();
				actionType = (actionTypeList.Count > 1)
					? TagSetting.ACTION_TYPE_MIX
					: actionTypeList.FirstOrDefault();
			}

			var tags = SelectPageReplaceTagList(actionType);

			return tags;
		}

		/// <summary>
		/// アクションタイプから置換タグリストを取得
		/// </summary>
		/// <param name="actionType">タグのアクションタイプ</param>
		/// <returns>置換タグリスト</returns>
		public List<string> SelectPageReplaceTagList(string actionType)
		{
			var tags = new List<string>();
			switch (actionType)
			{
				case TagSetting.ACTION_TYPE_SESSION_ONLY:
					tags.AddRange(
						this.LoginUserReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					tags.AddRange(this.CartReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_ORDER:
					tags.AddRange(
						this.LoginUserReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					tags.AddRange(this.OrderReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_CART_PRODUCT:
					tags.AddRange(
						this.CartProductReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_MIX:
					tags.AddRange(
						this.LoginUserReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_ALL:
					tags.AddRange(
						this.LoginUserReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_LANDING_CART:
					tags.AddRange(
						this.LoginUserReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					tags.AddRange(this.CartReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_SEO_ALL:
					tags.AddRange(
						this.SeoAllReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_SEO_PRODUCT_LIST:
					tags.AddRange(
						this.SeoProductListReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_SEO_PRODUCT_DETAIL:
					tags.AddRange(
						this.SeoProductDetailReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_COORDINATE_LIST:
					tags.AddRange(
						this.CoordinateListReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				case TagSetting.ACTION_TYPE_COORDINATE_DETAIL:
					tags.AddRange(
						this.CoordinateDetailReplaceTags.Where(i => i.Value.Valid).Select(i => i.Value.Tag).ToList());
					break;

				default:
					break;
			}

			return tags;
		}

		/// <summary>
		/// 商品タグ区切り文字に連番(1～)を入れる
		/// ※ 区切り文字中の[X]を置換
		/// </summary>
		/// <param name="products">商品内容リスト</param>
		/// <param name="delimiter">区切り文字</param>
		/// <returns></returns>
		protected string ProductTagDelimiterSetSerialNumber(List<string> products, string delimiter)
		{
			var serialNumber = 1;
			var result = string.Empty;
			foreach (var product in products)
			{
				result += product + ((products.Count == serialNumber) ? "" : delimiter.Replace("[X]", serialNumber.ToString()));
				serialNumber++;
			}
			return result;
		}

		/// <summary>
		/// 誕生日から年齢を算出
		/// </summary>
		/// <param name="birthDay">誕生日</param>
		/// <returns>年齢</returns>
		protected int UserAge(DateTime birthDay)
		{
			var age = (DateTime.Today >= birthDay)
				? (new DateTime((DateTime.Today - birthDay).Ticks).Year - 1)
				: 0;
			return age;
		}

		/// <summary>カート用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> CartReplaceTags { get; private set; }
		/// <summary>カート用商品置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> CartProductReplaceTags { get; private set; }
		/// <summary>注文用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> OrderReplaceTags { get; private set; }
		/// <summary>ログインユーザ用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> LoginUserReplaceTags { get; private set; }
		/// <summary>SEO全体設定用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> SeoAllReplaceTags { get; private set; }
		/// <summary>SEO商品一覧用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> SeoProductListReplaceTags { get; private set; }
		/// <summary>SEO商品詳細用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> SeoProductDetailReplaceTags { get; private set; }
		/// <summary>コーディネート一覧用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> CoordinateListReplaceTags { get; private set; }
		/// <summary>コーディネート詳細用置換タグリスト</summary>
		protected Dictionary<ReplaceTagKey, ReplaceTag> CoordinateDetailReplaceTags { get; private set; }
		
	}

	/// <summary>
	/// 置換タグクラス
	/// </summary>
	public class ReplaceTag
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="replaceTagkey">置換タグデータの取得キー</param>
		/// <param name="tagKey">置換する文字列</param>
		/// <param name="includeData">置換タグにデータを含めるかどうか</param>
		/// <param name="isSeoReplaceTags">SEO設定の置換タグかどうか</param>
		public ReplaceTag(ReplaceTagManager.ReplaceTagKey replaceTagkey, string tagKey, bool includeData = false, bool isSeoReplaceTags = false)
		{
			var tagFormat = isSeoReplaceTags ? "@@ {0} @@" : "@@{0}@@";

			this.ReplaceTagkey = replaceTagkey;
			this.TagKey = tagKey;
			this.IncludeDate = includeData;
			this.Tag = string.Format(tagFormat, this.TagKey + ((this.IncludeDate) ? "[]" : ""));
			this.Valid = (TagSetting.GetInstance().Setting.NotUsedReplaceTags
				.Any(t => t == replaceTagkey.ToString()) == false);
		}

		/// <summary>
		/// データを含む場合 一致内容の取得
		/// </summary>
		/// <param name="value">検証データ</param>
		/// <returns>一致内容</returns>
		public List<Match> GetDataTagMatch(string value)
		{
			if (this.IncludeDate == false) return null;

			var pattern = string.Format(@"(@@{0}[)(?<tag>.+?)(]@@)", this.TagKey);
			var tagMatch = Regex.Matches(value, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase)
				.Cast<Match>().ToList();
			return tagMatch;
		}

		/// <summary>
		/// データを含む場合 一致内容からタグに設定されたデータを取得
		/// </summary>
		/// <param name="match">一致内容</param>
		/// <returns>タグに設定されたデータ</returns>
		public string GetData(Match match)
		{
			if (this.IncludeDate == false) return string.Empty;

			var delimiter = match.Groups["tag"].ToString();
			return delimiter;
		}

		/// <summary>置換タグデータの取得キー</summary>
		public ReplaceTagManager.ReplaceTagKey ReplaceTagkey { get; private set; }
		/// <summary>実際に置換する文字列</summary>
		public string Tag { get; private set; }
		/// <summary>タグの有効性</summary>
		public bool Valid { get; private set; }
		/// <summary>タグ内にデータが含まれるかどうか</summary>
		public bool IncludeDate { get; private set; }
		/// <summary>置換する文字列</summary>
		private string TagKey { get; set; }
	}
}