/*
=========================================================================================================
  Module      : CSグループサービス(CsGroupService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsGroupService
	{
			/// <summary>レポジトリ</summary>
		private CsGroupRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsGroupService(CsGroupRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">CSグループID</param>
		/// <returns>CSグループモデル</returns>
		public CsGroupModel Get(string deptId, string groupId)
		{
			DataView dv = this.Repository.Get(deptId, groupId);
			return (dv.Count == 0) ? null : new CsGroupModel(dv[0]);
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>CSグループモデルの配列</returns>
		public CsGroupModel[] GetAll(string deptId)
		{
			DataView dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsGroupModel(drv)).ToArray();
		}
		#endregion

		#region +GetValidAll 有効なグループすべて取得
		/// <summary>
		/// 有効なグループすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>CSグループモデルの配列</returns>
		public CsGroupModel[] GetValidAll(string deptId)
		{
			DataView dv = this.Repository.GetValidAll(deptId);
			return (from DataRowView drv in dv select new CsGroupModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">登録用データ</param>
		/// <returns>登録したグループID</returns>
		public string Register(CsGroupModel model)
		{
			// Create new group ID
			model.CsGroupId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_GROUP_ID, 8);

			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 登録
				this.Repository.Register(model.DataSource, accessor);
				
				accessor.CommitTransaction();
			}

			return model.CsGroupId;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">更新用データ</param>
		public void Update(CsGroupModel model)
		{
			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				
				// 更新
				this.Repository.Update(model.DataSource, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">CSグループID</param>
		public void Delete(string deptId, string groupId)
		{
			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 削除
				this.Repository.Delete(deptId, groupId, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +GetValidAllWithValidOperators 有効なグループすべてとそれに所属する有効なオペレータ取得
		/// <summary>
		/// 有効なグループすべてとそれに所属する有効なオペレータ取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>CSグループモデルの配列（所属オペレータも含めて）</returns>
		public CsGroupModel[] GetValidAllWithValidOperators(string deptId)
		{
			var service = new CsOperatorGroupService(new CsOperatorGroupRepository());
			var models = GetValidAll(deptId);
			foreach (CsGroupModel model in models)
			{
				model.Ex_Operators = service.GetOperators(deptId, model.CsGroupId).Where(p => p.EX_ValidFlag == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID).ToArray();
			}
			return models;
		}
		#endregion
		#region +GetValidAllWithOperators 有効なグループすべてとそれに所属するオペレータ取得
		/// <summary>
		/// 有効なグループすべてとそれに所属するオペレータ取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>CSグループモデルの配列（所属オペレータも含めて）</returns>
		public CsGroupModel[] GetValidAllWithOperators(string deptId)
		{
			var service = new CsOperatorGroupService(new CsOperatorGroupRepository());
			var models = GetValidAll(deptId);
			foreach (CsGroupModel model in models)
			{
				model.Ex_Operators = service.GetOperators(deptId, model.CsGroupId);
			}
			return models;
		}
		#endregion
		#region +GetAllWithOperators グループすべてとそれに所属するオペレータ取得
		/// <summary>
		/// グループすべてとそれに所属するオペレータ取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>CSグループモデルの配列（所属オペレータも含めて）</returns>
		public CsGroupModel[] GetAllWithOperators(string deptId)
		{
			var service = new CsOperatorGroupService(new CsOperatorGroupRepository());
			var models = GetAll(deptId);
			SetOperators(models);
			return models;
		}
		#endregion

		#region -SetOperators オペレータセット
		/// <summary>
		/// オペレータセット
		/// </summary>
		/// <param name="models">グループモデル配列</param>
		private void SetOperators(CsGroupModel[] models)
		{
			var service = new CsOperatorGroupService(new CsOperatorGroupRepository());
			models.ToList().ForEach(m => m.Ex_Operators = service.GetOperators(m.DeptId, m.CsGroupId));
		}
		#endregion
	}
}
