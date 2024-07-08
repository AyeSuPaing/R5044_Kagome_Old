/*
=========================================================================================================
  Module      : ユーザー履歴基底モデル基底クラス(UserHistoryBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー履歴モデル基底クラス
	/// </summary>
	[Serializable]
	public abstract class UserHistoryBase : ModelBase<UserHistoryBase>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private UserHistoryBase()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		protected UserHistoryBase(DataRowView info)
			: this(info.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		protected UserHistoryBase(Hashtable info)
			: this()
		{
			this.DataSource = info;

			SetInfo();
		}

		/// <summary>
		/// 情報セット
		/// </summary>
		protected abstract void SetInfo();

		/// <summary>日付</summary>
		public DateTime DateTime { get; protected set; }
		/// <summary>区分文字列</summary>
		public string KbnString { get; protected set; }
		/// <summary>緊急フラグ（メッセージ用）</summary>
		public string MessageUrgencyFlg { get; protected set; }
		/// <summary>URL</summary>
		public string Url { get; set; }
	}
}