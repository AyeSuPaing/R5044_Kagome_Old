/*
=========================================================================================================
  Module      : メール配信送信済ユーザサービス (MailDistSentUserService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.MailDistSentUser
{
	/// <summary>
	/// メール配信送信済ユーザサービス
	/// </summary>
	public class MailDistSentUserService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="maildistId">メール配信設定ID</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		public MailDistSentUserModel Get(string maildistId, string userId)
		{
			using (var repository = new MailDistSentUserRepository())
			{
				var model = repository.Get(maildistId, userId);
				return model;
			}
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
		public int GetDuplicateExceptCount(
			string deptId,
			string masterId,
			string maildistId,
			string conditionForExceptMobile)
		{
			using (var repository = new MailDistSentUserRepository())
			{
				var model = repository.GetDuplicateExceptCount(
					deptId,
					masterId,
					maildistId,
					conditionForExceptMobile);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(MailDistSentUserModel model)
		{
			using (var repository = new MailDistSentUserRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="maildistId">メール配信設定ID</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>影響件数</returns>
		public int Delete(string maildistId, string userId)
		{
			using (var repository = new MailDistSentUserRepository())
			{
				var result = repository.Delete(maildistId, userId);
				return result;
			}
		}
		#endregion

		#region +DeleteByMaildistId メール配信設定IDで一括削除
		/// <summary>
		/// メール配信設定IDで一括削除
		/// </summary>
		/// <param name="maildistId">メール配信設定ID</param>
		/// <returns>影響件数</returns>
		public int DeleteByMaildistId(string maildistId)
		{
			using (var repository = new MailDistSentUserRepository())
			{
				var result = repository.DeleteByMaildistId(maildistId);
				return result;
			}
		}
		#endregion
	}
}
