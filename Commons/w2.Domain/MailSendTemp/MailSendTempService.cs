/*
=========================================================================================================
  Module      : メール配信　配信先テンポラリテーブルサービス (MailSendTempService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.MailSendTemp
{
	/// <summary>
	/// メール配信　配信先テンポラリテーブルサービス
	/// </summary>
	public class MailSendTempService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="dataNo">枝番</param>
		/// <returns>モデル</returns>
		public MailSendTempModel Get(string deptId, string masterId, long dataNo)
		{
			using (var repository = new MailSendTempRepository())
			{
				var model = repository.Get(deptId, masterId, dataNo);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(MailSendTempModel model, SqlAccessor accessor = null)
		{
			using (var repository = new MailSendTempRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="dataNo">枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string deptId, string masterId, long dataNo, SqlAccessor accessor = null)
		{
			using (var repository = new MailSendTempRepository(accessor))
			{
				var result = repository.Delete(deptId, masterId, dataNo);
				return result;
			}
		}
		#endregion

		#region +DeleteAll 対象マスターIDのリストを全て削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteAll(string deptId, string masterId, SqlAccessor accessor = null)
		{
			using (var repository = new MailSendTempRepository(accessor))
			{
				var result = repository.DeleteAll(deptId, masterId);
				return result;
			}
		}
		#endregion
	}
}