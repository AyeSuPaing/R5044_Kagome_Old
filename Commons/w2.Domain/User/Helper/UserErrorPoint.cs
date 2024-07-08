/*
=========================================================================================================
  Module      : ユーザーのエラーポイントクラス (UserErrorPoint.cs)
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
	/// ユーザーの検索結果クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class UserErrorPoint : ModelBase<UserErrorPoint>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private UserErrorPoint()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserErrorPoint(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserErrorPoint(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>エラーポイント</summary>
		public int ErrorPoint
		{
			get
			{
				if (this.DataSource.Contains(Constants.FIELD_MAILERRORADDR_ERROR_POINT)
					&& this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT] != DBNull.Value)
				{
					return (int)this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT];
				}
				else
				{
					return 0;
				}
			}
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT] = value; }
		}

		/// <summary>エラーポイント2</summary>
		public int ErrorPoint2
		{
			get
			{
				if (this.DataSource.Contains(Constants.FIELD_MAILERRORADDR_ERROR_POINT + "2")
					&& this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT + "2"] != DBNull.Value)
				{
					return (int)this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT + "2"];
				}
				else
				{
					return 0;
				}
			}
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT + "2"] = value; }
		}
		#endregion
	}
}
