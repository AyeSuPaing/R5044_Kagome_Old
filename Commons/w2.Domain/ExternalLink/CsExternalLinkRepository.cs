/*
=========================================================================================================
  Module      : 外部リンク設定リポジトリ(CsExternalLinkRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ExternalLink
{
	/// <summary>
	/// 外部リンクリポジトリ
	/// </summary>
	internal class CsExternalLinkRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		private const string XML_KEY_NAME = "CsExternalLink";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal CsExternalLinkRepository()
		{
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="linkId">リンクID</param>
		/// <returns>該当例リンク</returns>
		internal CsExternalLinkModel Get(string deptId, string linkId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CSEXTERNALLINK_DEPT_ID, deptId },
				{ Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_ID, linkId },
			};
			var dv = Get(XML_KEY_NAME, "Get", input);
			if (dv.Count == 0) return null;
			return new CsExternalLinkModel(dv[0]);
		}

		/// <summary>
		/// フラグオンリンク取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <returns>フラグ付けリスト</returns>
		internal CsExternalLinkModel[] GetValidFlg(string deptId, string validFlg)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CSEXTERNALLINK_DEPT_ID, deptId },
				{ Constants.FIELD_CSEXTERNALLINK_VALID_FLG, validFlg },
			};
			var dv = Get(XML_KEY_NAME, "GetValidFlg", input);
			return dv.Cast<DataRowView>().Select(drv => new CsExternalLinkModel(drv)).ToArray();
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="externalLinkId">外部リンク設定ID</param>
		/// <param name="externalLinkTitle">外部リンク設定タイトル</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="beginRow">開始NO</param>
		/// <param name="endRow">終了NO</param>
		/// <returns>検索値数</returns>
		internal CsExternalLinkModel[] Search(
			string deptId,
			string externalLinkId,
			string externalLinkTitle,
			string validFlg,
			int beginRow,
			int endRow)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CSEXTERNALLINK_DEPT_ID, deptId },
				{ Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_ID, externalLinkId },
				{ Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_TITLE, externalLinkTitle },
				{ Constants.FIELD_CSEXTERNALLINK_VALID_FLG, validFlg },
				{ "bgn_row_num", beginRow },
				{ "end_row_num", endRow },
			};
			var dv = Get(XML_KEY_NAME, "Search", input);
			var result = dv.Cast<DataRowView>()
				.Select(drv => new CsExternalLinkModel(drv))
				.ToArray();
			return result;
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="linkData">外部リンク情報</param>
		internal int Register(CsExternalLinkModel linkData)
		{
			return Exec(XML_KEY_NAME, "Register", linkData.DataSource);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="linkData">外部リンク情報</param>
		internal int Update(CsExternalLinkModel linkData)
		{
			return Exec(XML_KEY_NAME, "Update", linkData.DataSource);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="linkData">外部リンク情報</param>
		internal int Delete(CsExternalLinkModel linkData)
		{
			return Exec(XML_KEY_NAME, "Delete", linkData.DataSource);
		}
	}
}
