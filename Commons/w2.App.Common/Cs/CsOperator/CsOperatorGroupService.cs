/*
=========================================================================================================
  Module      : CSオペレータ所属グループサービス(CsOperatorGroupService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsOperatorGroupService
	{
		/// <summary>レポジトリ</summary>
		private CsOperatorGroupRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsOperatorGroupService(CsOperatorGroupRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ所属グループリスト</returns>
		public CsOperatorGroupModel[] GetAll(string deptId)
		{
			DataView dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsOperatorGroupModel(drv)).ToArray();
		}
		#endregion

		#region +GetGroups グループ取得（オペレータ指定）
		/// <summary>
		/// グループ取得（オペレータ指定）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>グループリスト</returns>
		public CsGroupModel[] GetGroups(string deptId, string operatorId)
		{
			DataView dv = this.Repository.GetGroups(deptId, operatorId);
			return (from DataRowView drv in dv select new CsGroupModel(drv)).ToArray();
		}
		#endregion

		#region +GetOperators オペレータ取得（グループ指定）
		/// <summary>
		/// オペレータ取得（グループ指定）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">グループID</param>
		/// <returns>オペレータリスト</returns>
		public CsOperatorModel[] GetOperators(string deptId, string groupId)
		{
			DataView dv = this.Repository.GetOperators(deptId, groupId);
			CsOperatorModel[] models = (from DataRowView drv in dv select new CsOperatorModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetValidOperators 有効なオペレータ取得（グループ指定）
		/// <summary>
		/// 有効なオペレータ取得（グループ指定）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">グループID</param>
		/// <returns>オペレータリスト</returns>
		public CsOperatorModel[] GetValidOperators(string deptId, string groupId)
		{
			return this.GetOperators(deptId, groupId).Where(ope => ope.EX_ValidFlag == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID).ToArray();
		}
		#endregion

		#region +GetValidGroupOperatorIdsByOperatorId 有効なグループ内オペレータID取得（オペレータ指定）
		/// <summary>
		/// 有効なグループ内オペレータID取得（オペレータ指定）
		/// 指定オペレータと同じグループに所属するオペレータの一覧を返します。
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>オペレータIDリスト</returns>
		public List<string> GetValidGroupOperatorIdsByOperatorId(string deptId, string operatorId)
		{
			List<string> operatorIds = new List<string>();
			CsGroupModel[] groups = this.GetGroups(deptId, operatorId);
			foreach (CsGroupModel group in groups.Where(group => group.ValidFlg == Constants.FLG_CSGROUP_VALID_FLG_VALID))
			{
				operatorIds.AddRange(this.GetValidOperators(deptId, group.CsGroupId)
					.Where(ope => operatorIds.Contains(ope.OperatorId) == false)
					.Select(ope => ope.OperatorId));
			}
			return operatorIds;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">グループID（削除対象）</param>
		/// <param name="models">オペレータ所属グループ情報</param>
		public void Update(string deptId, string groupId, CsOperatorGroupModel[] models)
		{
			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 指定されたグループに所属するすべてのオペレータ削除
				this.Repository.Delete(deptId, groupId, accessor);

				// 登録する
				foreach (CsOperatorGroupModel model in models)
				{
					this.Repository.Register(model.DataSource, accessor);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion
	}
}
