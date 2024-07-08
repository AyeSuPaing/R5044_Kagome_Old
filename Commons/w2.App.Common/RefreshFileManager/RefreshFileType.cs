/*
=========================================================================================================
  Module      : リフレッシュファイルタイプ列挙体(RefreshFileType.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.RefreshFileManager
{
	/// <summary>
	/// リフレッシュファイルタイプ
	/// </summary>
	public enum RefreshFileType
	{
		/// <summary>商品表示</summary>
		DisplayProduct,
		/// <summary>会員ランク</summary>
		MemberRank,
		/// <summary>新着情報</summary>
		News,
		/// <summary>ポイントルール</summary>
		PointRules,
		/// <summary>セットプロモーション</summary>
		SetPromotion,
		/// <summary>ノベルティ</summary>
		Novelty,
		/// <summary>レコメンド</summary>
		Recommend,
		/// <summary>SEO設定</summary>
		SeoMetadatas,
		/// <summary>商品一覧表示設定</summary>
		ProductListDispSetting,
		/// <summary>ユーザー拡張項目設定</summary>
		UserExtendSetting,
		/// <summary>ショートURL</summary>
		ShortUrl,
		/// <summary>モール連携設定</summary>
		MallCooperationSetting,
		/// <summary>定期購入キャンセル理由</summary>
		FixedPurchaseCancelReason,
		/// <summary>商品グループ設定</summary>
		ProductGroup,
		/// <summary>商品カラー</summary>
		ProductColor,
		/// <summary>自動翻訳API</summary>
		AutoTranslationWord,
		/// <summary>国ISOコード</summary>
		CountryLocation,
		/// <summary>アフィリエイトタグ設定</summary>
		AffiliateTagSetting,
		/// <summary>広告コード設定</summary>
		AdvCode,
		/// <summary>ページ管理</summary>
		PageDesign,
		/// <summary>パーツ管理</summary>
		PartsDesign,
		/// <summary>特集ページ情報</summary>
		FeaturePage,
		/// <summary>特集エリアバナー</summary>
		FeatureAreaBanner,
		/// <summary>OGPタグ設定</summary>
		OgpTagSetting,
		/// <summary>ランディングページデザイン</summary>
		LandingPageDesign,
		/// <summary>配送情報</summary>
		ShopShipping,
		/// <summary>サブ画像設定</summary>
		ProductSubImageSetting,
		/// <summary>決済種別</summary>
		Payment,
		/// <summary>注文拡張項目</summary>
		OrderExtendSetting,
		/// <summary>Real shop area</summary>
		RealShopArea,
		/// <summary>ABテスト</summary>
		AbTest,
		/// <summary>メンテナンス</summary>
		Maintenance,
		/// <summary>Scoring sale</summary>
		ScoringSale,
		/// <summary>頒布会コース</summary>
		SubscriptionBox,
		/// <summary>為替レート</summary>
		ExchangeRate,
		/// <summary>リアル店舗</summary>
		RealShop,
	}
}
