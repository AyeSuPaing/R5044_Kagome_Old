/*
=========================================================================================================
  Module      : CSオペレータ所属グループリポジトリ(CsOperatorGroupRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.CsOperatorGroup
{
	/// <summary>
	/// CSオペレータ所属グループリポジトリ
	/// </summary>
	public class CsOperatorGroupRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsOperatorGroup";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsOperatorGroupRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CsOperatorGroupRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetGroups グループ取得（オペレータ指定）
		/// <summary>
		/// グループ取得（オペレータ指定）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>グループリスト</returns>
		internal CsOperatorGroupModel[] GetGroups(string deptId, string operatorId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetGroups",
				new Hashtable
				{
					{ Constants.FIELD_CSOPERATORGROUP_DEPT_ID, deptId },
					{ Constants.FIELD_CSOPERATORGROUP_OPERATOR_ID, operatorId },
				});
			var results = dv.Cast<DataRowView>()
				.Select(drv => new CsOperatorGroupModel(drv))
				.ToArray();
			return results;
		}
		#endregion
	}
}
