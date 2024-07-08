/*
=========================================================================================================
  Module      : 共有情報既読管理リポジトリ(CsShareInfoReadRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報既読管理リポジトリ
	/// </summary>
	public class CsShareInfoReadRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsShareInfoRead";

		#region +SearchWithShareInfo 検索（共有情報も取得）
		/// <summary>
		/// 検索（共有情報も取得）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="infoText">共有情報テキスト</param>
		/// <param name="senderId">送信元オペレータID</param>
		/// <param name="importance">重要度</param>
		/// <param name="infoKbn">区分</param>
		/// <param name="readFlg">既読フラグ</param>
		/// <param name="pinnedFlg">ピンフラグ</param>
		/// <param name="sortKbn">ソート区分</param>
		/// <param name="pageNo">ページ番号</param>
		/// <returns>結果</returns>
		public DataView SearchWithShareInfo(string deptId, string operatorId, string infoText, string senderId, string importance, string infoKbn, string readFlg, string pinnedFlg, string sortKbn, int pageNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "SearchWithShareInfo"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSHAREINFO_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_OPERATOR_ID, operatorId);
				ht.Add(Constants.FIELD_CSSHAREINFO_INFO_TEXT + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(infoText));
				ht.Add(Constants.FIELD_CSSHAREINFO_SENDER, senderId);
				ht.Add(Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE, (importance != "") ? importance : null);
				ht.Add(Constants.FIELD_CSSHAREINFO_INFO_KBN, infoKbn);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_READ_FLG, readFlg);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_PINNED_FLG, pinnedFlg);
				ht.Add("sort_kbn", sortKbn);
				ht.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNo - 1) + 1);
				ht.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNo);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>結果</returns>
		public DataView Get(string deptId, long infoNo, string operatorId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_INFO_NO, infoNo);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_OPERATOR_ID, operatorId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>結果</returns>
		public DataView GetAll(string deptId, long infoNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_INFO_NO, infoNo);
				return statement.SelectSingleStatement(accessor, ht);
			}
		}
		#endregion

		#region +SearchByReadFlg 既読フラグを指定して検索
		/// <summary>
		/// 既読フラグを指定して検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="readFlg">既読フラグ</param>
		/// <returns>共有情報リスト</returns>
		public DataView SearchByReadFlg(string deptId, string operatorId, string readFlg)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "SearchByReadFlg"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSHAREINFO_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_OPERATOR_ID, operatorId);
				ht.Add(Constants.FIELD_CSSHAREINFOREAD_READ_FLG, readFlg);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +UpdateReadFlg 既読フラグ更新
		/// <summary>
		/// 既読フラグ更新
		/// </summary>
		/// <param name="ht">更新用データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateReadFlg(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateReadFlg"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +UpdatePinnedFlg ピン止めフラグ更新
		/// <summary>
		/// ピン止めフラグ更新
		/// </summary>
		/// <param name="ht">更新用データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdatePinnedFlg(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdatePinnedFlg"))
			{
				statement.ExecStatementWithOC(accessor, ht);
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

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +DeleteAll すべて削除
		/// <summary>
		/// すべて削除
		/// </summary>
		/// <param name="ht">情報</param>
		public void DeleteAll(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAll"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion
	}
}
