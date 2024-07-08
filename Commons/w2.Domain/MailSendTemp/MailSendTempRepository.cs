/*
=========================================================================================================
  Module      : メール配信　配信先テンポラリテーブルリポジトリ (MailSendTempRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.MailSendTemp
{
	/// <summary>
	/// メール配信　配信先テンポラリテーブルリポジトリ
	/// </summary>
	internal class MailSendTempRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MailSendTemp";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MailSendTempRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal MailSendTempRepository(SqlAccessor accessor) : base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="dataNo">枝番</param>
		/// <returns>モデル</returns>
		internal MailSendTempModel Get(string deptId, string masterId, long dataNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MAILSENDTEMP_DEPT_ID, deptId },
				{ Constants.FIELD_MAILSENDTEMP_MASTER_ID, masterId },
				{ Constants.FIELD_MAILSENDTEMP_DATA_NO, dataNo },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new MailSendTempModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(MailSendTempModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="dataNo">枝番</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string deptId, string masterId, long dataNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MAILSENDTEMP_DEPT_ID, deptId },
				{ Constants.FIELD_MAILSENDTEMP_MASTER_ID, masterId },
				{ Constants.FIELD_MAILSENDTEMP_DATA_NO, dataNo },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~DeleteAll 対象マスターIDのリストを全て削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteAll(string deptId, string masterId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MAILSENDTEMP_DEPT_ID, deptId },
				{ Constants.FIELD_MAILSENDTEMP_MASTER_ID, masterId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteAll", ht);
			return result;
		}
		#endregion
	}
}