/*
=========================================================================================================
  Module      : Awoo API リクエスト値列挙体 (AwooApiResponseValues.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Helper.Attribute;

namespace w2.App.Common.Awoo
{
	/// <summary>
	/// 商品並び替えキー
	/// </summary>
	public enum ProductSortType
	{
		/// <summary>FinalPriceAsc</summary>
		[EnumTextName("1")]
		FinalPriceAsc,
		/// <summary>FinalPriceDesc</summary>
		[EnumTextName("2")]
		FinalPriceDesc,
		/// <summary>PopularityAsc</summary>
		[EnumTextName("3")]
		PopularityAsc,
		/// <summary>PopularityDesc</summary>
		[EnumTextName("4")]
		PopularityDesc,
		/// <summary>FavoriteAsc</summary>
		[EnumTextName("5")]
		FavoriteAsc,
		/// <summary>FavoriteDesc</summary>
		[EnumTextName("6")]
		FavoriteDesc,
		/// <summary>SalesVolumeAsc</summary>
		[EnumTextName("7")]
		SalesVolumeAsc,
		/// <summary>SalesVolumDesc</summary>
		[EnumTextName("8")]
		SalesVolumDesc,
		/// <summary>ReleaseTimeAsc</summary>
		[EnumTextName("11")]
		ReleaseTimeAsc,
		/// <summary>ReleaseTimeDesc</summary>
		[EnumTextName("12")]
		ReleaseTimeDesc,
		/// <summary>UpdateTimeAsc</summary>
		[EnumTextName("13")]
		UpdateTimeAsc,
		/// <summary>UpdateTimeDesc</summary>
		[EnumTextName("14")]
		UpdateTimeDesc,
		/// <summary>ReviewsAsc</summary>
		[EnumTextName("15")]
		ReviewsAsc,
		/// <summary>ReviewsDesc</summary>
		[EnumTextName("16")]
		ReviewsDesc,
		/// <summary>RatingAsc</summary>
		[EnumTextName("17")]
		RatingAsc,
		/// <summary>RatingDesc</summary>
		[EnumTextName("18")]
		RatingDesc,
		/// <summary>ProductCreateTimeAsc</summary>
		[EnumTextName("20")]
		ProductCreateTimeAsc,
		/// <summary>ProductCreateTimeDesc</summary>
		[EnumTextName("21")]
		ProductCreateTimeDesc
	}

	/// <summary>
	/// 商品追加取得情報種別
	/// </summary>
	public enum SelectAdditionalProductFieldType
	{
		/// <summary>商品ディスクリプション情報</summary>
		[EnumTextName("productDescription")]
		ProductDescription,
		/// <summary>在庫情報</summary>
		[EnumTextName("productAvailability")]
		ProductAvailability,
	}

	/// <summary>
	/// おすすめ情報選択種別
	/// </summary>
	public enum SelectRecommendInfoType
	{
		/// <summary>おすすめタグ情報</summary>
		[EnumTextName("tags")]
		Tags,
		/// <summary>在庫情報</summary>
		[EnumTextName("products")]
		Products,
	}

	/// <summary>
	/// 商品レコメンド種別
	/// </summary>
	public enum RecommendDirectionType
	{
		/// <summary>同じカテゴリー内でのレコメンド</summary>
		[EnumTextName("v")]
		Vertical,
		/// <summary>カテゴリーをまたいでのレコメンド</summary>
		[EnumTextName("h")]
		Horizontal,
	}
}
