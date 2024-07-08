/*
=========================================================================================================
  Module      : CSオペレータ権限サービス(CsOperatorAuthorityService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsOperatorAuthorityService
	{
		/// <summary>レポジトリ</summary>
		private CsOperatorAuthorityRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsOperatorAuthorityService(CsOperatorAuthorityRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorAuthorityId">オペレータ権限ID</param>
		/// <returns>オペレータ権限情報</returns>
		public CsOperatorAuthorityModel Get(string deptId, string operatorAuthorityId)
		{
			DataView dv = this.Repository.Get(deptId, operatorAuthorityId);
			return (dv.Count == 0) ? null : new CsOperatorAuthorityModel(dv[0]);
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ権限情報のリスト</returns>
		public CsOperatorAuthorityModel[] GetAll(string deptId)
		{
			DataView dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsOperatorAuthorityModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">登録用データ</param>
		public void Register(CsOperatorAuthorityModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				this.Repository.Register(model.DataSource, accessor);
				accessor.CloseConnection();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">更新用データ</param>
		public void Update(CsOperatorAuthorityModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				this.Repository.Update(model.DataSource, accessor);
				accessor.CloseConnection();
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorAuthorityId">オペレータ権限ID</param>
		public void Delete(string deptId, string operatorAuthorityId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				this.Repository.Delete(deptId, operatorAuthorityId, accessor);
				accessor.CloseConnection();
			}
		}
		#endregion
	}
}
