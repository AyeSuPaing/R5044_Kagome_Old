/*
=========================================================================================================
  Module      : 共有情報リポジトリ(CsShareInfoRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報リポジトリ
	/// </summary>
	public class CsShareInfoRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsShareInfo";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="ht">情報</param>
		/// <returns>既読管理情報データビュー</returns>
		public DataView Get(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Search 全件検索
		/// <summary>
		/// 全件検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <param name="infoText">共有情報テキスト</param>
		/// <param name="infoKbn">共有情報区分</param>
		/// <param name="importance">共有情報重要度</param>
		/// <param name="operatorId">送信元オペレータID</param>
		/// <param name="sortKbn">ソート区分</param>
		/// <param name="pageNo">ページ番号</param>
		/// <returns>共有情報一覧データビュー</returns>
		public DataView Search(string deptId, long? infoNo, string infoText, string infoKbn, string importance, string operatorId, string sortKbn, int pageNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Search"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSHAREINFO_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSSHAREINFO_INFO_NO, infoNo);
				ht.Add(Constants.FIELD_CSSHAREINFO_INFO_TEXT + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(infoText));
				ht.Add(Constants.FIELD_CSSHAREINFO_INFO_KBN, infoKbn);
				ht.Add(Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE, (importance != "") ? importance : null);
				ht.Add(Constants.FIELD_CSSHAREINFO_SENDER, operatorId);
				ht.Add("sort_kbn", sortKbn);
				ht.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNo - 1) + 1);
				ht.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNo);

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Register(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Register"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="ht">情報</param>
		public void Update(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="ht">情報</param>
		public void Delete(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion
	}
}
