/*
=========================================================================================================
  Module      : メール配信送信済ユーザリポジトリ (MailDistSentUserRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using w2.Common.Sql;

namespace w2.Domain.MailDistSentUser
{
	/// <summary>
	/// メール配信送信済ユーザリポジトリ
	/// </summary>
	internal class MailDistSentUserRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MailDistSentUser";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MailDistSentUserRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MailDistSentUserRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="maildistId">メール配信設定ID</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		internal MailDistSentUserModel Get(string maildistId, string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILDISTSENTUSER_MAILDIST_ID, maildistId},
				{Constants.FIELD_MAILDISTSENTUSER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count > 0)
				? new MailDistSentUserModel(dv[0])
				: null;
		}
		#endregion

		#region +GetDuplicateExceptCount 送信済除外件数取得
		/// <summary>
		/// 配信済除外件数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="maildistId">メール配信設定ID</param>
		/// <param name="conditionForExceptMobile">モバイルメール排除用条件</param>
		/// <returns>送信済除外件数</returns>
		/// <remarks>エラーポイント上限、排除リストを除外した上で配信済チェックを行っている</remarks>
		internal int GetDuplicateExceptCount(
			string deptId,
			string masterId,
			string maildistId,
			string conditionForExceptMobile)
		{
			var replaceKeyValues = new KeyValuePair<string, string>(
				"@@ where_except_mobile @@",
				conditionForExceptMobile);

			var ht = new Hashtable
			{
				{ Constants.FIELD_TARGETLISTDATA_DEPT_ID, deptId },
				{ Constants.FIELD_TARGETLISTDATA_MASTER_ID, masterId },
				{ Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, maildistId}
			};

			var result = (int)Get(XML_KEY_NAME, "GetDuplicateExceptCount", ht, null, replaceKeyValues)[0][0];
			return result;
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(MailDistSentUserModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="maildistId">メール配信設定ID</param>
		/// <param name="userId">ユーザID</param>
		internal int Delete(string maildistId, string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MAILDISTSENTUSER_MAILDIST_ID, maildistId },
				{ Constants.FIELD_MAILDISTSENTUSER_USER_ID, userId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~DeleteByMaildistId メール配信設定IDで一括削除
		/// <summary>
		/// メール配信設定IDで一括削除
		/// </summary>
		/// <param name="maildistId">メール配信設定ID</param>
		/// <returns>影響件数</returns>
		internal int DeleteByMaildistId(string maildistId)
		{
			var result = Exec(
				XML_KEY_NAME,
				"DeleteByMaildistId",
				new Hashtable
				{
					{ Constants.FIELD_MAILDISTSENTUSER_MAILDIST_ID, maildistId }
				});

			return result;
		}
		#endregion
	}
}