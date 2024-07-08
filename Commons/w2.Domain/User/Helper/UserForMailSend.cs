/*
=========================================================================================================
  Module      : メール送信のためユーザ情報 (UserForMailSend.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// メール送信のためユーザ情報
	/// </summary>
	[Serializable]
	public class UserForMailSend : ModelBase<UserForMailSend>
	{
		#region 定数
		/// <summary>期間限定本ポイント合計（利用可能なポイントのみ）</summary>
		public const string FIELD_USERFORMAILSEND_LIMITED_TERM_POINT = "limited_term_point";
		/// <summary>利用可能ポイント合計</summary>
		public const string FIELD_USERFORMAILSEND_POINT_USABLE = "point_usable";
		#endregion

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserForMailSend()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserForMailSend(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserForMailSend(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		#endregion
	}
}
