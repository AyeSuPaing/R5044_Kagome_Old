/*
=========================================================================================================
  Module      : 更新履歴前後検索のためのヘルパクラス (UpdateHistoryBeforeAndAfterSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.UpdateHistory.Helper
{
	#region +更新履歴前後検索条件クラス
	/// <summary>
	/// 更新履歴前後検索条件クラス
	/// </summary>
	[Serializable]
	public class UpdateHistoryBeforeAndAfterSearchCondition : BaseDbMapModel
	{
		/// <summary>更新履歴No</summary>
		[DbMapName("update_history_no")]
		public long UpdateHistoryNo { get; set; }
		/// <summary>更新履歴区分</summary>
		[DbMapName("update_history_kbn")]
		public string UpdateHistoryKbn { get; set; }
		/// <summary>ユーザーID</summary>
		[DbMapName("user_id")]
		public string UserId { get; set; }
		/// <summary>マスタID</summary>
		[DbMapName("master_id")]
		public string MasterId { get; set; }
	}
	#endregion

	#region +更新履歴前後検索結果クラス
	/// <summary>
	/// 更新履歴前後検索結果クラス
	/// </summary>
	[Serializable]
	public class UpdateHistoryBeforeAndAfterSearchResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dv">検索結果</param>
		public UpdateHistoryBeforeAndAfterSearchResult(DataView dv)
		{
			if (dv.Count == 1)
			{
				this.BeforeUpdateHistory = new UpdateHistoryModel();
				this.AfterUpdateHistory = new UpdateHistoryModel(dv[0]);
			}
			else if (dv.Count == 2)
			{
				this.BeforeUpdateHistory = new UpdateHistoryModel(dv[1]);
				this.AfterUpdateHistory = new UpdateHistoryModel(dv[0]);
			}
		}
		#endregion

		#region プロパティ
		/// <summary>更新履歴情報（前）</summary>
		public UpdateHistoryModel BeforeUpdateHistory { get; private set; }
		/// <summary>更新履歴情報（後）</summary>
		public UpdateHistoryModel AfterUpdateHistory { get; private set; }
		#endregion
	}
	#endregion
}