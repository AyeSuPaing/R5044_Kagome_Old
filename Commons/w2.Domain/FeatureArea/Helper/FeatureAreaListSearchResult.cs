/*
=========================================================================================================
  Module      : 特集エリアタイプ一覧検索のためのヘルパクラス (FeatureAreaListSearchResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.PartsDesign;

namespace w2.Domain.FeatureArea.Helper
{
	/// <summary>
	/// 特集エリア一覧検索結果
	/// </summary>
	[Serializable]
	public class FeatureAreaListSearchResult : FeatureAreaModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaListSearchResult(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaListSearchResult(Hashtable source)
			: base(source)
		{
			this.ThumbFileNames = new string[0];
		}
		#endregion

		#region プロパティ
		/// <summary>サムネイルファイル名(拡張子含む)</summary>
		public string[] ThumbFileNames { get; set; }
		/// <summary>バナー数</summary>
		public int BannerCount
		{
			get { return this.ThumbFileNames.Length; }
		}
		/// <summary>特集エリアタイプ名</summary>
		public string AreaTypeName{ get; set; }
		/// <summary>パーツモデル</summary>
		public PartsDesignModel PartsModel { get; set; }
		#endregion
	}
}
