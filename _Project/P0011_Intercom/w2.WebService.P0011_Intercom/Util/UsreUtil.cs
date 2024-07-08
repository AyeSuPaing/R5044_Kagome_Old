/*
=========================================================================================================
  Module      : ユーザーユーティリティクラス(UsreUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : ユーザー関連のユーティリティクラス
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;


namespace w2.Plugin.P0011_Intercom.WebService.Util
{
	/// <summary>
	/// ユーザ関連のユーティリティクラス
	/// </summary>
	public class UsreUtil
	{
		public const string CONST_USER_ID_HEADER = "N";
		private RijndaelCrypto cry_ = null;
		public const string DEPT_ID = "0";
		public const string NUMBER_CODE = "UserId";


		//Selet with UpdLock
		const string SELECT = "SELECT	number \n"
							+ "	 FROM	w2_Number WITH (UPDLOCK) \n"
							+ "	WHERE	dept_id = @dept_id \n"
							+ "	  AND	number_code = @number_code \n";

		const string UPDATE = "UPDATE	w2_Number \n"
							+ "	  SET	number = @number, \n"
							+ "			date_changed = getdate()"
							+ " WHERE	dept_id = @dept_id \n"
							+ "   AND	number_code = @number_code \n";

		public UsreUtil()
		{
			byte[] key = Convert.FromBase64String(WebConfigurationManager.AppSettings["key"].ToString());
			byte[] iv = Convert.FromBase64String(WebConfigurationManager.AppSettings["iv"].ToString());

			cry_ = new RijndaelCrypto(key, iv);
		}

		/// <summary>
		/// パスワード暗号化
		/// </summary>
		/// <returns></returns>
		public string EncPass(string target)
		{
			return cry_.Encrypt(target);
		}

		/// <summary>
		/// ログインIDの重複チェック
		/// </summary>
		/// <returns>
		/// True：重複なし
		/// False：重複している。すでにDBに登録あり。
		/// </returns>
		public bool ChkLoginIDDuplicate(string targetStr)
		{
			bool chkVal = false;

			//接続文字
			string connStr = CommUtil._DBUtil().GetConnectionString();

			string sqlStr = "SELECT COUNT(login_id) as cnt FROM w2_USER WHERE login_id = @login_id ";

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				//SqlCommand
				SqlCommand selCmd = new SqlCommand(sqlStr, conn);

				//SettingParam
				selCmd.Parameters.AddWithValue("@login_id", targetStr);
				
				//connectionOpen
				selCmd.Connection.Open();
				
				int getcnt = (int)selCmd.ExecuteScalar();

				if(getcnt == 0)
				{
					chkVal = true;
				}
			}

			return chkVal;
		}

		/// <summary>
		/// 会員番号採番
		/// </summary>
		/// <returns></returns>
		public string CreateNewUserID()
		{
			//自前でトランザクション
			//接続文字
			string connStr = CommUtil._DBUtil().GetConnectionString();

			long returnval = 0;
			
			using (SqlConnection conn = new SqlConnection(connStr))
			{							
				//SqlCommand
				SqlCommand selCmd = new SqlCommand(SELECT, conn);

				//SettingParam
				selCmd.Parameters.AddWithValue("@dept_id", DEPT_ID);
				selCmd.Parameters.AddWithValue("@number_code", NUMBER_CODE);

				//connectionOpen
				selCmd.Connection.Open();
				//BeginTransaction
				SqlTransaction tran = selCmd.Connection.BeginTransaction(IsolationLevel.Serializable);

				selCmd.Transaction = tran;
				

				Int64 getnum = (Int64)selCmd.ExecuteScalar();


				returnval = getnum + 1;

				//SqlCommand
				SqlCommand updCmd = new SqlCommand(UPDATE, conn);

				
				//SettingParam
				updCmd.Parameters.AddWithValue("@number", returnval);
				updCmd.Parameters.AddWithValue("@dept_id", DEPT_ID);
				updCmd.Parameters.AddWithValue("@number_code", NUMBER_CODE);

				updCmd.Transaction = tran;

				updCmd.ExecuteNonQuery();

				tran.Commit();
				
				conn.Close();
				
			}
			return CONST_USER_ID_HEADER + returnval.ToString();
		}

		/// <summary>
		/// インターコム採番ユーザIDの重複チェック
		/// </summary>
		/// <returns>
		/// True：重複なし
		/// False：重複している。すでにDBに登録あり。
		/// 削除フラグが立っていないもののみを対象として重複チェック
		/// </returns>
		public bool ChkICIDDuplicate(string targetStr)
		{
			//デフォルトチェックNGとしておく
			bool chkVal = false;

			//接続文字
			string connStr = CommUtil._DBUtil().GetConnectionString();

			string sqlStr = "SELECT COUNT(attribute1) as cnt FROM w2_USER WHERE attribute1 = @attribute1 and del_flg = @del_flg ";

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				//SqlCommand
				SqlCommand selCmd = new SqlCommand(sqlStr, conn);

				//SettingParam
				selCmd.Parameters.AddWithValue("@attribute1", targetStr);
				//削除していないもののみ対象
				selCmd.Parameters.AddWithValue("@del_flg", "0");

				//connectionOpen
				selCmd.Connection.Open();

				int getcnt = (int)selCmd.ExecuteScalar();

				if (getcnt == 0)
				{
					//ゼロ件ならチェックOK
					chkVal = true;
				}
			}

			return chkVal;
		}
	}
}