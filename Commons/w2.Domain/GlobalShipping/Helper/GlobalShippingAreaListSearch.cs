/*
=========================================================================================================
  Module      : 海外配送エリア構成モデル (GlobalShippingAreaListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;
using w2.Common.Extensions;

namespace w2.Domain.GlobalShipping.Helper
{
	internal class GlobalShippingAreaListSearch
	{
		#region ~GetSearchCount 検索件数取得
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="cond">検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal int GetSearchCount(GlobalShippingAreaListSearchCondition cond)
		{
			using (var rep = new GlobalShippingRepository())
			{
				return rep.GetSearchGlobalShippingAreaListCount(cond);
			}
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="cond">検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal GlobalShippingAreaListSearchResult[] Search(GlobalShippingAreaListSearchCondition cond)
		{
			using (var rep = new GlobalShippingRepository())
			{
				return rep.SearchGlobalShippingAreaList(cond);
			}
		}
		#endregion
	}

	/// <summary>
	/// 海外配送エリア一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class GlobalShippingAreaListSearchCondition : BaseDbMapModel
	{
		/// <summary>
		/// 開始行番号
		/// </summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>
		/// 終了行番号
		/// </summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
	}

	/// <summary>
	/// ユーザーポイントの検索結果クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class GlobalShippingAreaListSearchResult :GlobalShippingAreaModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private GlobalShippingAreaListSearchResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GlobalShippingAreaListSearchResult(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GlobalShippingAreaListSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>行番号</summary>
		public int RowNum
		{
			get { return (int)this.DataSource["row_num"]; }
		}
		/// <summary>件数</summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		#endregion
	}

}
