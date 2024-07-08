/*
=========================================================================================================
  Module      : ユーザー統合ステータス件数取得のためのヘルパクラス (UserIntegrationStatusCount.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UserIntegration.Helper
{
	#region +ユーザー統合ステータス件数クラス
	/// <summary>
	/// ユーザー統合ステータス件数クラス
	/// </summary>
	public class UserIntegrationStatusCount : ModelBase<UserIntegrationStatusCount>
	{
		#region +コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserIntegrationStatusCount()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationStatusCount(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationStatusCount(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region +プロパティ
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_STATUS]; }
			set { this.DataSource[Constants.FIELD_USERINTEGRATION_STATUS] = value; }
		}
		/// <summary>件数</summary>
		public int Count
		{
			get { return (int)this.DataSource["count"]; }
			set { this.DataSource["count"] = value; }
		}
		#endregion
	}
	#endregion
}
