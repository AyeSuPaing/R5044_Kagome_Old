/*
=========================================================================================================
  Module      : 商品共通ページ 商品初期設定部分(ProductPage_DefaultSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Product;
using w2.App.Common.ProductDefaultSetting;
using System.Linq;

/// <summary>
/// ProductPage_DefaultSetting の概要の説明です
/// </summary>
public partial class ProductPage : BasePage
{
	/// <summary>
	/// 商品初期設定の初期値を取得
	/// </summary>
	protected Hashtable GetProductDefaultSettingValue()
	{
		var param = new Hashtable();
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var defaultSetting = this.ProductDefaultSetting.Product;

			// 基本情報
			// 商品ID
			param.Add(
				Constants.FIELD_PRODUCT_PRODUCT_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_PRODUCT_ID));
			// 備考
			param.Add(
				Constants.FIELD_PRODUCT_NOTE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_NOTE));
			// サプライヤID
			param.Add(
				Constants.FIELD_PRODUCT_SUPPLIER_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SUPPLIER_ID));
			// 商品連携ID1
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID1));
			// 商品連携ID2
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID2));
			// 商品連携ID3
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID3));
			// 商品連携ID4
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID4));
			// 商品連携ID5
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID5));
			// 商品連携ID6
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID6,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID6));
			// 商品連携ID7
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID7,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID7));
			// 商品連携ID8
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID8,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID8));
			// 商品連携ID9
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID9,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID9));
			// 商品連携ID10
			param.Add(
				Constants.FIELD_PRODUCT_COOPERATION_ID10,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_COOPERATION_ID10));
			// モール拡張商品ID
			param.Add(
				Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID));
			// モール出品設定
			param.Add(
				Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG,
				defaultSetting.GetDefault(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG));
			// 外部レコメンド利用
			param.Add(
				Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG));
			// 商品名
			param.Add(
				Constants.FIELD_PRODUCT_NAME,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_NAME));
			// 商品名(フリガナ)
			param.Add(
				Constants.FIELD_PRODUCT_NAME_KANA,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_NAME_KANA));
			// SEOキーワード
			param.Add(
				Constants.FIELD_PRODUCT_SEO_KEYWORDS,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SEO_KEYWORDS));
			// キャッチコピー
			param.Add(
				Constants.FIELD_PRODUCT_CATCHCOPY,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CATCHCOPY));
			// 検索キーワード
			param.Add(
				Constants.FIELD_PRODUCT_SEARCH_KEYWORD,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SEARCH_KEYWORD));
			// 商品概要(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_OUTLINE_KBN,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_OUTLINE_KBN));
			// 商品概要
			param.Add(
				Constants.FIELD_PRODUCT_OUTLINE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_OUTLINE));
			// 商品詳細説明1(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1));
			// 商品詳細説明1
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL1));
			// 商品詳細説明2(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2));
			// 商品詳細説明2
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL2));
			// 商品詳細説明3(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3));
			// 商品詳細説明3
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL3));
			// 商品詳細説明4(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4));
			// 商品詳細説明4
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL4));
			// 返品交換文言
			param.Add(
				Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE));
			// 紹介URL
			param.Add(
				Constants.FIELD_PRODUCT_URL,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_URL));
			// 問い合わせ用メールアドレス
			param.Add(
				Constants.FIELD_PRODUCT_INQUIRE_EMAIL,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_INQUIRE_EMAIL));
			// 問い合わせ用電話番号
			param.Add(
				Constants.FIELD_PRODUCT_INQUIRE_TEL,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_INQUIRE_TEL));
			// 販売価格
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_PRICE,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCT_DISPLAY_PRICE)
					&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_PRICE) == null))
				? string.Empty
				: defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_PRICE));
			// 特別価格
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE));
			// 付与ポイント区分1
			param.Add(
				Constants.FIELD_PRODUCT_POINT_KBN1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_POINT_KBN1));
			// 付与ポイント1
			param.Add(
				Constants.FIELD_PRODUCT_POINT1,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCT_POINT1)
					&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCT_POINT1) == null))
				? string.Empty
				: defaultSetting.GetDefault(Constants.FIELD_PRODUCT_POINT1));
			// 付与ポイント区分2
			param.Add(
				Constants.FIELD_PRODUCT_POINT_KBN2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_POINT_KBN2));
			// 付与ポイント2
			param.Add(
				Constants.FIELD_PRODUCT_POINT2,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCT_POINT2)
					&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCT_POINT2) == null))
				? string.Empty
				: defaultSetting.GetDefault(Constants.FIELD_PRODUCT_POINT2));
			// 配送種別
			param.Add(
				Constants.FIELD_PRODUCT_SHIPPING_TYPE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SHIPPING_TYPE));
			// 配送サイズ区分
			param.Add(
				Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN));
			// 配送料複数個無料フラグ
			param.Add(
				Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG));
			// 商品カラーID（商品単位）
			param.Add(
				Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID));
			// 税区分
			param.Add(
				Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG));
			// 販売期間(FROM)
			param.Add(
				Constants.FIELD_PRODUCT_SELL_FROM,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SELL_FROM));
			// 販売期間(TO)
			param.Add(
				Constants.FIELD_PRODUCT_SELL_TO,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SELL_TO));
			// 表示期間(FROM)
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_FROM,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_FROM));
			// 表示期間(TO)
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_TO,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_TO));
			// 販売期間表示フラグ
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG));
			// 表示優先順
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_PRIORITY,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY));
			// 販売可能数量
			param.Add(
				Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY));
			// 在庫管理
			param.Add(
				Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN));
			// 在庫文言
			param.Add(
				Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID));
			// 定期購入フラグ
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG));
			// 頒布会フラグ
			param.Add(
				Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG));
			// 年齢制限フラグ
			param.Add(
				Constants.FIELD_PRODUCT_AGE_LIMIT_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG));
			// ギフト購入フラグ
			param.Add(
				Constants.FIELD_PRODUCT_GIFT_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_GIFT_FLG));
			// カート投入URL制限フラグ
			param.Add(
				Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG));
			// 有効フラグ
			param.Add(
				Constants.FIELD_PRODUCT_VALID_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_VALID_FLG));
			// 商品表示区分
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_KBN,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_KBN));
			// 会員ランク割引対象フラグ
			param.Add(
				Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG));
			// 閲覧可能会員ランク
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK));
			// 購入可能会員ランク
			param.Add(
				Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK));
			// Googleショッピング連携
			param.Add(
				Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG));
			// 再入荷通知メール有効フラグ
			param.Add(
				Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG));
			// 販売開始通知メール有効フラグ
			param.Add(
				Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG));
			// 再販売通知メール有効フラグ
			param.Add(
				Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG));
			// 商品バリエーション選択方法
			param.Add(
				Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN));
			// デジタルコンテンツ商品フラグ
			param.Add(
				Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG));
			// ダウンロードURL
			param.Add(
				Constants.FIELD_PRODUCT_DOWNLOAD_URL,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DOWNLOAD_URL));
			// Product limited payment
			param.Add(
				Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS));
			// 通常商品購入制限チェック
			param.Add(
				Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG));
			// 定期商品購入制限チェック
			param.Add(
				Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG));
			// 定期購入初回購入価格
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE));
			// 定期購入価格
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE));
			// 商品税率カテゴリ
			param.Add(
				Constants.FIELD_PRODUCT_TAX_CATEGORY_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID));
			// 同梱商品明細表示フラグ
			param.Add(
				Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE));
			// 商品区分
			param.Add(
				Constants.FIELD_PRODUCT_PRODUCT_TYPE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_PRODUCT_TYPE));
			// ＆mallの販売方式
			param.Add(
				Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG));
			// 解約可能回数
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)
					&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) == null))
				? string.Empty
				: defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT));
			// 定期購入利用不可ユーザー管理レベル
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS));
			// 商品重量(g）
			param.Add(
				Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM)
					&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM) == null))
				? string.Empty
				: defaultSetting.GetDefault(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM));

			// 商品付帯情報
			var productOptionSettingKeys = ProductOptionSettingHelper.GetAllProductOptionSettingKeys();
			foreach (var posKey in productOptionSettingKeys)
			{
				param.Add(posKey, defaultSetting.GetDefault(posKey));
			}

			// カテゴリ
			//カテゴリ1
			param.Add(
				Constants.FIELD_PRODUCT_CATEGORY_ID1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CATEGORY_ID1));
			//カテゴリ2
			param.Add(
				Constants.FIELD_PRODUCT_CATEGORY_ID2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CATEGORY_ID2));
			//カテゴリ3
			param.Add(
				Constants.FIELD_PRODUCT_CATEGORY_ID3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CATEGORY_ID3));
			//カテゴリ4
			param.Add(
				Constants.FIELD_PRODUCT_CATEGORY_ID4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CATEGORY_ID4));
			//カテゴリ5
			param.Add(
				Constants.FIELD_PRODUCT_CATEGORY_ID5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CATEGORY_ID5));

			// ブランド
			// ブランドID1
			param.Add(
				Constants.FIELD_PRODUCT_BRAND_ID1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_BRAND_ID1));
			// ブランドID2
			param.Add(
				Constants.FIELD_PRODUCT_BRAND_ID2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_BRAND_ID2));
			// ブランドID3
			param.Add(
				Constants.FIELD_PRODUCT_BRAND_ID3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_BRAND_ID3));
			// ブランドID4
			param.Add(
				Constants.FIELD_PRODUCT_BRAND_ID4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_BRAND_ID4));
			// ブランドID5
			param.Add(
				Constants.FIELD_PRODUCT_BRAND_ID5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_BRAND_ID5));

			// クロスセル
			// 商品関連ID1（クロスセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1));
			// 商品関連ID2（クロスセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2));
			// 商品関連ID3（クロスセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3));
			// 商品関連ID4（クロスセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4));
			// 商品関連ID5（クロスセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5));

			// アップセル
			// 商品関連ID1（アップセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1));
			// 商品関連ID2（アップセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2));
			// 商品関連ID3（アップセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3));
			// 商品関連ID4（アップセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4));
			// 商品関連ID5（アップセル）
			param.Add(
				Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5));

			// 商品画像
			param.Add(
				Constants.FIELD_PRODUCT_IMAGE_HEAD,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_IMAGE_HEAD));

			// キャンペーンアイコン
			// アイコン表示フラグ1
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG1));
			// アイコン表示期限1
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END1));
			// アイコン表示フラグ2
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG2));
			// アイコン表示期限2
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END2));
			// アイコン表示フラグ3
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG3));
			// アイコン表示期限3
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END3));
			// アイコン表示フラグ4
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG4));
			// アイコン表示期限4
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END4));
			// アイコン表示フラグ5
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG5));
			// アイコン表示期限5
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END5));
			// アイコン表示フラグ6
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG6,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG6));
			// アイコン表示期限6
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END6,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END6));
			// アイコン表示フラグ7
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG7,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG7));
			// アイコン表示期限7
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END7,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END7));
			// アイコン表示フラグ8
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG8,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG8));
			// アイコン表示期限8
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END8,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END8));
			// アイコン表示フラグ9
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG9,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG9));
			// アイコン表示期限9
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END9,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END9));
			//アイコン表示フラグ10
			param.Add(
				Constants.FIELD_PRODUCT_ICON_FLG10,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_FLG10));
			//アイコン表示期限10
			param.Add(
				Constants.FIELD_PRODUCT_ICON_TERM_END10,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_ICON_TERM_END10));

			// モバイル
			// モバイルキャッチコピー
			param.Add(
				Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE));
			// モバイル商品概要(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE));
			// モバイル商品概要
			param.Add(
				Constants.FIELD_PRODUCT_OUTLINE_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_OUTLINE_MOBILE));
			// モバイル商品詳細説明1(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE));
			// モバイル商品詳細説明1
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE));
			// モバイル商品詳細説明2(TEXT OR HTML)
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE));
			// モバイル商品詳細説明2
			param.Add(
				Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE));
			// モバイル返品交換文言
			param.Add(
				Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE));
			// モバイル商品画像
			param.Add(
				Constants.FIELD_PRODUCT_IMAGE_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_IMAGE_MOBILE));
			// モバイル表示フラグ
			param.Add(
				Constants.FIELD_PRODUCT_MOBILE_DISP_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_MOBILE_DISP_FLG));
			// モバイル用商品バリエーション選択方法
			param.Add(
				Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE));

			// Flag fixed purchase member
			// Display only fixed purchase member
			param.Add(
				Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG));
			// Sell only fixed purchase member
			param.Add(
				Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG));

			// Fixed Purchase Next Shipping Setting
			// 定期購入2回目以降配送商品ID
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID));
			// 定期購入2回目以降配送商品バリエーションID
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID));
			// 定期購入2回目以降配送商品注文個数
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY)
						&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY) == null))
					? string.Empty
					: defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY));

			// 定期購入スキップ制限回数
			param.Add(
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)
						&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) == null))
					? string.Empty
					: defaultSetting.GetDefault(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT));

			// Product size factor
			param.Add(
				Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR));

			//会員ランクポイント付与率除外フラグ
			param.Add(
				Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG));
			// 店舗受取可能フラグ
			param.Add(
				Constants.FIELD_PRODUCT_STOREPICKUP_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_STOREPICKUP_FLG));
			// 配送料無料適用外フラグ
			param.Add(
				Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG));
		}

		return param;
	}

	/// <summary>
	/// 商品バリエーション初期設定の初期値を取得
	/// </summary>
	protected Hashtable GetProductVariationDefaultSettingValue()
	{
		var param = new Hashtable();
		if (this.ProductDefaultSetting.Tables.ContainsKey(Constants.TABLE_PRODUCTVARIATION))
		{
			var defaultSetting = this.ProductDefaultSetting.Tables[Constants.TABLE_PRODUCTVARIATION];
			param.Add(Constants.FIELD_PRODUCTVARIATION_V_ID, string.Empty);
			// バリエーション名1
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1));
			// バリエーション名2
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2));
			// バリエーション名3
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3));
			// 商品カラーID
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID));
			// 表示順
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER));
			// 販売価格
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_PRICE,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCTVARIATION_PRICE)
					&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_PRICE) == null))
				? string.Empty
				: defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_PRICE));
			// 特別価格
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE));
			// 商品バリエーション連携ID1
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1));
			// 商品バリエーション連携ID2
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2));
			// 商品バリエーション連携ID3
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3));
			// 商品バリエーション連携ID4
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4));
			// 商品バリエーション連携ID5
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5));
			// 商品バリエーション連携ID6
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6));
			// 商品バリエーション連携ID7
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7));
			// 商品バリエーション連携ID8
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8));
			// 商品バリエーション連携ID9
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9));
			// 商品バリエーション連携ID10
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10));
			// ダウンロードURL
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL));
			// モールバリエーションID1
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1));
			// モールバリエーションID2
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2));
			// モールバリエーション種別
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE));
			// 商品画像ヘッダ
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD));
			// モバイルバリエーション画像名
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE));
			// 定期初回購入価格
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE));
			// 定期購入価格
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE));
			// ＆mallの予約商品フラグ
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG));
			// 商品バリエーション重量（g）
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM,
				(defaultSetting.GetDisplay(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM)
					&& (defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM) == null))
				? string.Empty
				: defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM));
			// カート投入URL制限フラグ
			param.Add(
				Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG,
				defaultSetting.GetDefault(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG));
		}
		return param;
	}

	/// <summary>
	/// 商品付帯情報表示可否取得
	/// </summary>
	/// <returns>表示可否</returns>
	protected bool GetDisplayProductOptionValueArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var productOptionSettingKeys = ProductOptionSettingHelper.GetAllProductOptionSettingKeys();
			var isDisplayProductOptionSetting = productOptionSettingKeys.Any(
				posKey => GetProductDefaultSettingDisplayField(posKey));
			return isDisplayProductOptionSetting;
		}
		return true;
	}

	/// <summary>
	/// カテゴリ領域表示可否取得
	/// </summary>
	/// <returns>表示可否</returns>
	protected bool GetDisplayCategoryArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			return (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID1)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID2)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID3)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID4)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID5));
		}
		return true;
	}

	/// <summary>
	/// ブランド領域表示可否取得
	/// </summary>
	/// <returns>表示可否</returns>
	protected bool GetDisplayBrandArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			return (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID1)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID2)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID3)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID4)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID5));
		}
		return true;
	}

	/// <summary>
	/// クロスセル領域表示可否取得
	/// </summary>
	/// <returns>表示可否</returns>
	protected bool GetDisplayCsArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			return GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1);
		}
		return true;
	}

	/// <summary>
	/// アップセル領域表示可否取得
	/// </summary>
	/// <returns>表示可否</returns>
	protected bool GetDisplayUsArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			return GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1);
		}
		return true;
	}

	/// <summary>
	/// 商品画像の指定領域表示可否取得
	/// </summary>
	/// <returns>表示可否</returns>
	protected bool GetDisplayImageHeadArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_IMAGE_HEAD);
		}
		return true;
	}

	/// <summary>
	/// アイコン領域表示可否取得
	/// </summary>
	/// <returns>表示可否</returns>
	protected bool GetDisplayIconArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			return (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG1)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG2)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG3)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG4)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG5)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG6)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG7)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG8)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG9)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG10));
		}
		return true;
	}

	/// <summary>
	/// 商品初期設定マスタの項目行の表示可否を取得
	/// </summary>
	/// <param name="tableName">テーブルのキー</param>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>表示可否</returns>
	protected bool GetDefaultSettingDisplayField(string tableName, string fieldName)
	{
		if (this.ProductDefaultSetting.Tables.ContainsKey(tableName))
		{
			return (this.ProductDefaultSetting.Tables[tableName].GetDisplay(fieldName));
		}
		return true;
	}

	/// <summary>
	/// 商品初期設定マスタの項目コメント値取得
	/// </summary>
	/// <param name="tableName">テーブルのキー</param>
	/// <param name="fieldName">項目のキー</param>
	protected string GetDefaultSettingComment(string tableName, string fieldName)
	{
		if (this.ProductDefaultSetting.Tables.ContainsKey(tableName))
		{
			return this.ProductDefaultSetting.Tables[tableName].GetComment(fieldName);
		}
		return string.Empty;
	}

	/// <summary>
	/// Get default setting field value
	/// </summary>
	/// <param name="tableName">テーブルのキー</param>
	/// <param name="fieldName">項目のキー</param>
	protected string GetDefaultSettingFieldValue(string tableName, string fieldName)
	{
		if (this.ProductDefaultSetting.Tables.ContainsKey(tableName))
		{
			return this.ProductDefaultSetting.Tables[tableName].GetDefault(fieldName);
		}
		return null;
	}

	/// <summary>
	/// Has default setting field value
	/// </summary>
	/// <param name="tableName">テーブルのキー</param>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>True: If the field has a default setting value</returns>
	protected bool HasDefaultSettingFieldValue(string tableName, string fieldName)
	{
		if (this.ProductDefaultSetting.Tables.ContainsKey(tableName) == false) return false;

		return (this.ProductDefaultSetting.Tables[tableName].GetDefault(fieldName) != null);
	}

	/// <summary>
	/// Get default date
	/// </summary>
	/// <param name="table">Table</param>
	/// <param name="fieldName">Field name</param>
	/// <param name="format">Date format</param>
	/// <param name="defaultValue">Default value</param>
	/// <returns>Date time</returns>
	protected DateTime? GetDefaultDate(
		ProductDefaultSettingTable table,
		string fieldName,
		DateTime? defaultValue = null,
		string format = "yyyy/MM/dd HH:mm:ss")
	{
		if (table.Fields.ContainsKey(fieldName) == false) return defaultValue;

		var date = StringUtility.ToDateString(table.GetDefault(fieldName), format);
		return string.IsNullOrEmpty(date) ? defaultValue : DateTime.Parse(date);
	}

	/// <summary>
	/// Get product default setting display field
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>表示可否</returns>
	protected bool GetProductDefaultSettingDisplayField(string fieldName)
	{
		return GetDefaultSettingDisplayField(Constants.TABLE_PRODUCT, fieldName);
	}

	/// <summary>
	/// Get variation default setting display field 
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>表示可否</returns>
	protected bool GetVariationDefaultSettingDisplayField(string fieldName)
	{
		return GetDefaultSettingDisplayField(Constants.TABLE_PRODUCTVARIATION, fieldName);
	}

	/// <summary>
	/// Has product default setting field value
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>True: If the field has a default setting value</returns>
	protected bool HasProductDefaultSettingFieldValue(string fieldName)
	{
		return HasDefaultSettingFieldValue(Constants.TABLE_PRODUCT, fieldName);
	}

	/// <summary>
	/// Has variation default setting field value
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>True: If the field has a default setting value</returns>
	protected bool HasVariationDefaultSettingFieldValue(string fieldName)
	{
		return HasDefaultSettingFieldValue(Constants.TABLE_PRODUCTVARIATION, fieldName);
	}

	/// <summary>
	/// Get product default setting comment
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>Product default setting comment</returns>
	protected string GetProductDefaultSettingComment(string fieldName)
	{
		return GetDefaultSettingComment(Constants.TABLE_PRODUCT, fieldName);
	}

	/// <summary>
	/// Has product default setting comment
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>True: Product default setting has set comment</returns>
	protected bool HasProductDefaultSettingComment(string fieldName)
	{
		return (string.IsNullOrEmpty(GetDefaultSettingComment(Constants.TABLE_PRODUCT, fieldName)) == false);
	}

	/// <summary>
	/// Get variation default setting comment
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>Variation default setting comment</returns>
	protected string GetVariationDefaultSettingComment(string fieldName)
	{
		return GetDefaultSettingComment(Constants.TABLE_PRODUCTVARIATION, fieldName);
	}

	/// <summary>
	/// Has variation default setting comment
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>True: Variation default setting has set comment</returns>
	protected bool HasVariationDefaultSettingComment(string fieldName)
	{
		return (string.IsNullOrEmpty(GetDefaultSettingComment(Constants.TABLE_PRODUCTVARIATION, fieldName)) == false);
	}

	/// <summary>
	/// Get product default setting field value
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>Product default setting field value</returns>
	protected string GetProductDefaultSettingFieldValue(string fieldName)
	{
		return GetDefaultSettingFieldValue(Constants.TABLE_PRODUCT, fieldName);
	}

	/// <summary>
	/// Get variation default setting field value
	/// </summary>
	/// <param name="fieldName">項目のキー</param>
	/// <returns>Variation default setting field value</returns>
	protected string GetVariationDefaultSettingFieldValue(string fieldName)
	{
		return GetDefaultSettingFieldValue(Constants.TABLE_PRODUCTVARIATION, fieldName);
	}

	/// <summary>
	/// Get display product inquire area
	/// </summary>
	/// <returns>True: Show display product inquire area</returns>
	protected bool GetDisplayProductInquireArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var result = (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_URL)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_EMAIL)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_TEL));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display product cooperation ids area
	/// </summary>
	/// <returns>True: Show display product cooperation ids area</returns>
	protected bool GetDisplayProductCooperationIdsArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var result = (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID1)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID2)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID3)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID4)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID5)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID6)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID7)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID8)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID9)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID10));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display product notification settings area
	/// </summary>
	/// <returns>True: Show display product notification settings area</returns>
	protected bool GetDisplayProductNotificationSettingsArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var result = (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display fixed purchase area
	/// </summary>
	/// <returns>True: Show display fixed purchase area</returns>
	protected bool GetDisplayFixedPurchaseArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var result = (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS)
				|| (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
					&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG))
				|| (Constants.FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE
					&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG))
				|| (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED
					&& GetProductDefaultSettingDisplayField(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING))
				|| (Constants.PRODUCT_ORDER_LIMIT_ENABLED
					&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG)));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display limited option area
	/// </summary>
	/// <returns>True: Show display limited option area</returns>
	protected bool GetDisplayLimitedOptionArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var result = (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS)
				|| (Constants.MEMBER_RANK_OPTION_ENABLED
					&& (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK)
						|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG)
						|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK)
						|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG)))
				|| (Constants.PRODUCT_ORDER_LIMIT_ENABLED
					&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG))
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display mall cooperation area
	/// </summary>
	/// <returns>True: Show display mall cooperation area</returns>
	protected bool GetDisplayMallCooperationArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var result = (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display digital content area
	/// </summary>
	/// <returns>True: Show display digital content area</returns>
	protected bool GetDisplayDigitalContentArea()
	{
		if (this.ProductDefaultSetting.HasProductSetting)
		{
			var result = (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DOWNLOAD_URL));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display variation cooperation id area
	/// </summary>
	/// <returns>True: Show display variation cooperation id area</returns>
	protected bool GetDisplayVariationCooperationIdArea()
	{
		if (this.ProductDefaultSetting.HasProductVariationSetting)
		{
			var result = (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10));
			return result;
		}
		return true;
	}

	/// <summary>
	/// Get display variation mall area
	/// </summary>
	/// <returns>True: Show display variation mall area</returns>
	protected bool GetDisplayVariationMallArea()
	{
		if (this.ProductDefaultSetting.HasProductVariationSetting)
		{
			var result = (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG)
				|| GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1));
			return result;
		}
		return true;
	}

	/// <summary>商品デフォルト設定</summary>
	protected ProductDefaultSetting ProductDefaultSetting
	{
		get
		{
			if (this._productDefaultSetting == null)
			{
				this._productDefaultSetting = new ProductDefaultSetting();
				this._productDefaultSetting.LoadSetting(this.LoginOperatorShopId);
			}
			return this._productDefaultSetting;
		}
	}
	private ProductDefaultSetting _productDefaultSetting = null;
}
