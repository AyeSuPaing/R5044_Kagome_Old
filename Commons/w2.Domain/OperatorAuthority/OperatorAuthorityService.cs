/*
=========================================================================================================
  Module      : オペレータ権限ルサービス (OperatorAuthorityService.cs)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.Domain.OperatorAuthority
{
	/// <summary>
	/// オペレータ権限ルサービス
	/// </summary>
	public class OperatorAuthorityService : ServiceBase, IOperatorAuthorityService
	{
		#region +Get
		/// <summary>
		/// Get all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="operatorId">Operator id</param>
		/// <returns>List operator authority </returns>
		public List<OperatorAuthorityModel> Get(string shopId, string operatorId)
		{
			using (var repository = new OperatorAuthorityRepository())
			{
				return repository.Get(shopId, operatorId);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(OperatorAuthorityModel model)
		{
			using (var repository = new OperatorAuthorityRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">Operator Id</param>
		public void Delete(string shopId, string operatorId)
		{
			using (var repository = new OperatorAuthorityRepository())
			{
				repository.Delete(shopId, operatorId);
			}
		}
		#endregion
	}
}
