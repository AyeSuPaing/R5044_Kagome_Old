/*
=========================================================================================================
  Module      : Operator Authority Service Interface (IOperatorAuthorityService.cs)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.Domain.OperatorAuthority
{
	/// <summary>
	/// Interface operator authority service
	/// </summary>
	public interface IOperatorAuthorityService : IService
	{
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="operatorId">Operator id</param>
		/// <returns>List operator authority </returns>
		List<OperatorAuthorityModel> Get(string shopId, string operatorId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(OperatorAuthorityModel model);

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="operatorId">Operator Id</param>
		void Delete(string shopId, string operatorId);
	}
}
