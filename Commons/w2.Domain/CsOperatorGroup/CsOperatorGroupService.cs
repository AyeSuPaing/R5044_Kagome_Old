/*
=========================================================================================================
  Module      : CSオペレータ所属グループサービス(CsOperatorGroupService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.CsOperatorGroup
{
	/// <summary>
	/// CSオペレータ所属グループサービス
	/// </summary>
	public class CsOperatorGroupService : ServiceBase
	{
		#region +GetGroups グループ取得（オペレータ指定）
		/// <summary>
		/// グループ取得（オペレータ指定）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>グループリスト</returns>
		public CsOperatorGroupModel[] GetGroups(string deptId, string operatorId)
		{
			using (var repository = new CsOperatorGroupRepository())
			{
				var result = repository.GetGroups(
					deptId,
					operatorId);
				return result;
			}
		}
		#endregion
	}
}
