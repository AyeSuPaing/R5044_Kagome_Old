/*
=========================================================================================================
  Module      : CSインシデント警告アイコンサービス (CsIncidentWarningIconService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.CsIncidentWarningIcon
{
	/// <summary>
	/// CSインシデント警告アイコンサービス
	/// </summary>
	public class CsIncidentWarningIconService : ServiceBase
	{
		/// <summary>
		/// オペレータIDによる一括取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>モデル配列</returns>
		public CsIncidentWarningIconModel[] GetByOperatorId(string deptId, string operatorId)
		{
			using (var repository = new CsIncidentWarningIconRepository())
			{
				var models = repository.GetByOperatorId(deptId, operatorId);
				return models;
			}
		}

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="models">モデル配列</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(string deptId, string operatorId, CsIncidentWarningIconModel[] models)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = DeleteByOperatorId(deptId, operatorId, accessor);
				if (models != null)
				{
					result += Insert(models, accessor);
				}
				accessor.CommitTransaction();
				return result;
			}
		}
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(string deptId, string operatorId, CsIncidentWarningIconModel[] models, SqlAccessor accessor)
		{
			var result = DeleteByOperatorId(deptId, operatorId, accessor);
			if (models != null)
			{
				result += Insert(models, accessor);
			}
			return result;
		}

		/// <summary>
		/// 複数件一括登録
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(CsIncidentWarningIconModel[] models, SqlAccessor accessor = null)
		{
			using (var repository = new CsIncidentWarningIconRepository(accessor))
			{
				var result = 0;
				foreach (var model in models)
				{
					result += repository.Insert(model);
				}
				return result;
			}
		}

		/// <summary>
		/// オペレータIDによる一括削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int DeleteByOperatorId(string deptId, string operatorId, SqlAccessor accessor = null)
		{
			using (var repository = new CsIncidentWarningIconRepository(accessor))
			{
				var result = repository.DeleteByOperatorId(deptId, operatorId);
				return result;
			}
		}
	}
}
