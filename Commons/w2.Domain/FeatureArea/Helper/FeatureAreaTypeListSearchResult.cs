/*
=========================================================================================================
  Module      : 特集エリアタイプ一覧検索のためのヘルパクラス (FeatureAreaTypeListSearchResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;

namespace w2.Domain.FeatureArea.Helper
{

	/// <summary>
	/// 特集エリアタイプ一覧検索結果
	/// </summary>
	[Serializable]
	public class FeatureAreaTypeListSearchResult : FeatureAreaTypeModel
	{
		/// <summary>参照数</summary>
		const string FIELD_REFERENCE_COUNT = "ReferenceCount";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaTypeListSearchResult(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaTypeListSearchResult(Hashtable source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>参照数</summary>
		public int ReferenceCount
		{
			get { return (int)this.DataSource[FIELD_REFERENCE_COUNT]; }
			set { this.DataSource[FIELD_REFERENCE_COUNT] = value; }
		}
		#endregion
	}
}
