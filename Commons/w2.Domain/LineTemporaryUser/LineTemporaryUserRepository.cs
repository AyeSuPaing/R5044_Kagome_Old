/*
=========================================================================================================
  Module      : LINE仮会員テーブルリポジトリ (LineTemporaryUserRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.LineTemporaryUser
{
	/// <summary>
	/// LINE仮会員テーブルリポジトリ
	/// </summary>
	internal class LineTemporaryUserRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "LineTemporaryUser";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal LineTemporaryUserRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal LineTemporaryUserRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="lineUserId">LINEユーザーID</param>
		/// <returns>モデル</returns>
		internal LineTemporaryUserModel Get(string lineUserId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_LINETEMPORARYUSER_LINE_USER_ID, lineUserId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);

			if (dv.Count == 0) return null;
			return new LineTemporaryUserModel(dv[0]);
		}
		#endregion

		#region ~GetByUserId ユーザーIDから取得
		/// <summary>
		/// ユーザーIDから取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		internal LineTemporaryUserModel GetByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_LINETEMPORARYUSER_TEMPORARY_USER_ID, userId },
			};
			var dv = Get(XML_KEY_NAME, "GetByUserId", ht);

			if (dv.Count == 0) return null;
			return new LineTemporaryUserModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(LineTemporaryUserModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(LineTemporaryUserModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="lineUserId">LINEユーザーID</param>
		internal int Delete(string lineUserId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_LINETEMPORARYUSER_LINE_USER_ID, lineUserId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

	}
}
