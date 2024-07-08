/*
=========================================================================================================
  Module      : 更新履歴一覧検索のためのヘルパクラス (UpdateHistoryListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.UpdateHistory.Helper
{
	#region +更新履歴一覧検索条件クラス
	/// <summary>
	/// 更新履歴一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class UpdateHistoryListSearchCondition : BaseDbMapModel
	{
		/// <summary>更新履歴区分1</summary>
		[DbMapName("update_history_kbn1")]
		public string UpdateHistoryKbn1 { get; set; }
		/// <summary>更新履歴区分2</summary>
		[DbMapName("update_history_kbn2")]
		public string UpdateHistoryKbn2 { get; set; }
		/// <summary>更新履歴区分3</summary>
		[DbMapName("update_history_kbn3")]
		public string UpdateHistoryKbn3 { get; set; }
		/// <summary>ユーザーID</summary>
		[DbMapName("user_id")]
		public string UserId { get; set; }
		/// <summary>マスタID</summary>
		[DbMapName("master_id")]
		public string MasterId { get; set; }
		/// <summary>ユーザID（SQL LIKEエスケープ済）</summary>
		[DbMapName("master_id_like_escaped")]
		public string MasterIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.MasterId); }
		}
	}
	#endregion

	#region +更新履歴一覧検索結果クラス
	/// <summary>
	/// 更新履歴一覧検索結果クラス
	/// ※UpdateHistoryModelを拡張
	/// </summary>
	[Serializable]
	public class UpdateHistoryListSearchResult : UpdateHistoryModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UpdateHistoryListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		#endregion
	}
	#endregion
}