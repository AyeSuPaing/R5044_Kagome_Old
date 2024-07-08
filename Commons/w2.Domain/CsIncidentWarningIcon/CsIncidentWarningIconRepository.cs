/*
=========================================================================================================
  Module      : CSインシデント警告アイコンリポジトリ (CsIncidentWarningIconRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.CsIncidentWarningIcon
{
	/// <summary>
	/// CSインシデント警告アイコンリポジトリ
	/// </summary>
	internal class CsIncidentWarningIconRepository : RepositoryBase
	{
		/// <returns>XML名</returns>
		private const string XML_KEY_NAME = "CsIncidentWarningIcon";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal CsIncidentWarningIconRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal CsIncidentWarningIconRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}

		/// <summary>
		/// オペレータIDによる一括取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>モデル配列</returns>
		internal CsIncidentWarningIconModel[] GetByOperatorId(string deptId, string operatorId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_CSINCIDENTWARNINGICON_DEPT_ID, deptId },
				{ Constants.FIELD_CSINCIDENTWARNINGICON_OPERATOR_ID, operatorId },
			};
			var dv = Get(XML_KEY_NAME, "GetByOperatorId", ht);
			if (dv.Count == 0) return null;

			var models = dv.Cast<DataRowView>()
				.Select(drv => new CsIncidentWarningIconModel(drv))
				.ToArray();
			return models;
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Insert(CsIncidentWarningIconModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}

		/// <summary>
		/// オペレータIDによる一括削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteByOperatorId(string deptId, string operatorId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_CSINCIDENTWARNINGICON_DEPT_ID, deptId },
				{ Constants.FIELD_CSINCIDENTWARNINGICON_OPERATOR_ID, operatorId }
			};
			var result = Exec(XML_KEY_NAME, "DeleteByOperatorId", ht);
			return result;
		}
	}
}
