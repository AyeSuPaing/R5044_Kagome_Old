/*
=========================================================================================================
  Module      : シングルサインオン結果(SingleSignOnResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain.User;

namespace w2.App.Common.SingleSignOn
{
	#region 列挙対
	/// <summary>
	/// シングルサインオン結果詳細
	/// </summary>
	public enum SingleSignOnDetailTypes
	{
		/// <summary>成功</summary>
		Success,
		/// <summary>失敗</summary>
		Failure,
		/// <summary>何もしない</summary>
		None,
	}
	#endregion

	/// <summary>シングルサインオン結果クラス</summary>
	public class SingleSignOnResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="resultDetail">シングルサインオン結果詳細</param>
		/// <param name="user">ユーザー情報</param>
		/// <param name="nextUrl">ログイン後遷移先URL</param>
		/// <param name="messages">メッセージ</param>
		public SingleSignOnResult(
			SingleSignOnDetailTypes resultDetail,
			UserModel user,
			string nextUrl = "",
			string messages = "")
		{
			this.SingleSignOnDetail = resultDetail;
			this.User = user;
			this.NextUrl = nextUrl;
			this.Messages = messages;
		}
		#endregion

		#region プロパティ
		/// <summary>成功したか？</summary>
		public bool IsSuccess
		{
			get
			{
				return (this.SingleSignOnDetail == SingleSignOnDetailTypes.Success);
			}
		}
		/// <summary>失敗したか？</summary>
		public bool IsFailure
		{
			get
			{
				return (this.SingleSignOnDetail == SingleSignOnDetailTypes.Failure);
			}
		}
		/// <summary>何もしなかったか？</summary>
		public bool IsNone
		{
			get
			{
				return (this.SingleSignOnDetail == SingleSignOnDetailTypes.None);
			}
		}
		/// <summary>結果詳細</summary>
		public SingleSignOnDetailTypes SingleSignOnDetail { get; private set; }
		/// <summary>ユーザー情報</summary>
		public UserModel User { get; private set; }
		/// <summary>ログイン後遷移先URL</summary>
		public string NextUrl { get; private set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; private set; }
		#endregion
	}
}