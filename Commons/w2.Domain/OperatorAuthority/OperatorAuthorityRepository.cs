/*
=========================================================================================================
  Module      : オペレータ権限リポジトリ (OperatorAuthorityRepository.cs)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OperatorAuthority
{
	/// <summary>
	/// オペレータ権限リポジトリ
	/// </summary>
	internal class OperatorAuthorityRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OperatorAuthority";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OperatorAuthorityRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal OperatorAuthorityRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="operatorId">Operator id</param>
		/// <returns>List operator authority </returns>
		internal List<OperatorAuthorityModel> Get(string shopId, string operatorId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_OPERATORAUTHORITY_SHOP_ID, shopId},
				{Constants.FIELD_OPERATORAUTHORITY_OPERATOR_ID, operatorId},
			};
			var data = Get(XML_KEY_NAME, "Get", input);
			if (data.Count == 0) return null;

			return data.Cast<DataRowView>().Select(row => new OperatorAuthorityModel(row)).ToList();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal void Insert(OperatorAuthorityModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="operatorId">Operator id</param>
		internal void Delete(string shopId, string operatorId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_OPERATORAUTHORITY_SHOP_ID, shopId},
				{Constants.FIELD_OPERATORAUTHORITY_OPERATOR_ID, operatorId},
			};
			Exec(XML_KEY_NAME, "Delete", input);
		}
		#endregion
	}
}
