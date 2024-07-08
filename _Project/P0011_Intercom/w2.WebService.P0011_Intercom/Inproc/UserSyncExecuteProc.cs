/*
=========================================================================================================
  Module      : 会員情報同期処理クラス(UserSyncExecuteProc.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : 会員情報同期の処理クラス
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using w2.Plugin.P0011_Intercom.WebService.Util;
using System.IO;

namespace w2.Plugin.P0011_Intercom.WebService.Inproc
{
	public class UserSyncExecuteProc : IWebServiceInProc 
	{
		public DataSet Execute(DataSet ds)
		{
			//引数DSのログ出力
			try
			{
				CommUtil._LogFileUtil().WriteReceiveDataLog(ds);
			}
			catch
			{
				//ログかけない場合
			}
			
			//戻り値用データセット
			DataSet rtnDS = new DataSet(WebServiceConst.DS_NAME_RETUNR);
			UsreUtil util = new UsreUtil();

			string proc = "";
			DataTable userTab = null;
					
			//引数データ分解
			try
			{
				if (ds.Tables.Contains(WebServiceConst.TAB_NAME_PROCTABLE) == false)
				{
					throw new Exception("データセットにテーブル【" + WebServiceConst.TAB_NAME_PROCTABLE + "】がありません");
				}

				if (ds.Tables.Contains(WebServiceConst.TAB_NAME_USERTABLE) == false)
				{
					throw new Exception("データセットにテーブル【" + WebServiceConst.TAB_NAME_USERTABLE + "】がありません");
				}
				
				//処理区分取出
				if (IsDBNullAndNullCheck(ds.Tables[WebServiceConst.TAB_NAME_PROCTABLE].Rows[0][WebServiceConst.COL_PROCTYPE]) == false)
				{
					proc = ds.Tables[WebServiceConst.TAB_NAME_PROCTABLE].Rows[0][WebServiceConst.COL_PROCTYPE].ToString();
				}

				//エラー時のログのためコピー
				userTab = ds.Tables[WebServiceConst.TAB_NAME_USERTABLE].Copy();
				
				//ログインID取り出し
				string loginID = (string)ds.Tables[WebServiceConst.TAB_NAME_USERTABLE].Rows[0][WebServiceConst.COL_LOGIN_ID];

				//パスワード取り出し
				string pass = (string)ds.Tables[WebServiceConst.TAB_NAME_USERTABLE].Rows[0][WebServiceConst.COL_PASSWORD];
							
				//EncPass
				ds.Tables[WebServiceConst.TAB_NAME_USERTABLE].Rows[0][WebServiceConst.COL_PASSWORD] = util.EncPass(pass);
		
				switch (proc)
				{
					case WebServiceConst.PROC_TYPE_USER_REGIST:

						//ログインIDの重複チェック
						if(util.ChkLoginIDDuplicate(loginID) == false)
						{
							//重複してるのでエラー
							throw new Exception("指定したログインIDはすでにDBに登録されています。:" + loginID);
						}

						//インターコムユーザID
						string icid = ds.Tables[WebServiceConst.TAB_NAME_USERTABLE].Rows[0][WebServiceConst.COL_ATTRIBUTE1].ToString();

						//インターコムユーザIDの空白チェック
						if (icid == "")
						{
							//空白なのでエラー
							throw new Exception("指定したインターコムユーザIDが空です。");
						}

						//インターコムユーザIDの重複チェック
						if (util.ChkICIDDuplicate(icid) == false)
						{
							//重複してるのでエラー
							throw new Exception("指定したインターコムユーザIDはすでにDBに登録されています。:" + icid);
						}
						
						//会員番号採番
						string newnum = util.CreateNewUserID();

						//採番した会員番号をセット
						ds.Tables[WebServiceConst.TAB_NAME_USERTABLE].Rows[0][WebServiceConst.COL_USER_ID] = newnum;

						//会員情報DBへ登録
						CommUtil._DBUtil().ExecuteSql(WebServiceConst.SQL_SECTION_INSERT_USERDATA, ds.Tables[WebServiceConst.TAB_NAME_USERTABLE]);

						//処理正常終了戻り
						rtnDS.Tables.Add(CreateResultDataTable(WebServiceConst.PROC_RESULT_SUCCESS, "正常終了"));
						
						//採番番号戻り
						rtnDS.Tables.Add(CreateUserIDTable(newnum));

						break;

					case WebServiceConst.PROC_TYPE_USER_MODIFY:

						//会員情報DBへ更新
						CommUtil._DBUtil().ExecuteSql(WebServiceConst.SQL_SECTION_UPDATE_USERDATA, ds.Tables[WebServiceConst.TAB_NAME_USERTABLE]);
					
						//処理結果戻り
						rtnDS.Tables.Add(CreateResultDataTable(WebServiceConst.PROC_RESULT_SUCCESS, "正常終了"));
						break;


					case WebServiceConst.PROC_TYPE_USER_DELETE:
						//会員情報DBへ更新
						CommUtil._DBUtil().ExecuteSql(WebServiceConst.SQL_SECTION_DELETE_USERDATA, ds.Tables[WebServiceConst.TAB_NAME_USERTABLE]);

						//会員退会
						rtnDS.Tables.Add(CreateResultDataTable(WebServiceConst.PROC_RESULT_SUCCESS, "正常終了"));

						break;

					default:
						//上記以外
						//エラー情報セット
						rtnDS.Tables.Add(CreateResultDataTable(WebServiceConst.PROC_RESULT_FAILED, "処理区分が不正です。指定処理区分→" + proc));
						break;
				}

			}
			catch (Exception ex)
			{
				//エラーのcsv出力
				try
				{
					CommUtil._LogFileUtil().WriteErrorCSV(userTab,proc);
				}
				catch
				{
				}

				//エラーログ出力
				try
				{
					CommUtil._LogFileUtil().WriteErrorLog(ds, ex);
				}
				catch
				{

				}

				if (rtnDS.Tables.Contains(WebServiceConst.TAB_NAME_RESULTTABLE))
				{
					//すでに処理結果テーブルが生成されていた場合
					if (rtnDS.Tables[WebServiceConst.TAB_NAME_RESULTTABLE].Rows.Count > 0)
					{
						//行がある場合
						rtnDS.Tables[WebServiceConst.TAB_NAME_RESULTTABLE].Rows[0][WebServiceConst.COL_PROCRESULT]
							= WebServiceConst.PROC_RESULT_FAILED; 
						
						
						rtnDS.Tables[WebServiceConst.TAB_NAME_RESULTTABLE].Rows[0][WebServiceConst.COL_PROCMESSAGE] 
							= string.Format("エラーが発生しました。【エラーメッセージ→】{0}【スタックトレース】{1}" , ex.Message, ex.StackTrace);
					}
					else
					{
						//行がない場合
						DataRow dr = rtnDS.Tables[WebServiceConst.TAB_NAME_RESULTTABLE].NewRow();
						dr[WebServiceConst.COL_PROCRESULT]
							= WebServiceConst.PROC_RESULT_FAILED;

						dr[WebServiceConst.COL_PROCMESSAGE]
							= string.Format("エラーが発生しました。【エラーメッセージ→】{0}【スタックトレース】{1}", ex.Message, ex.StackTrace);

					}
				}
				else
				{
					rtnDS.Tables.Add(CreateResultDataTable("Failed", "エラーが発生しました。エラーメッセージ→" + ex.Message));
				}
			}
			finally
			{

			}

			return rtnDS;
		}

		//##############################################################################################

		#region ヘルパ
				
		#region 処理結果テーブルの生成

		/// <summary>
		/// 処理結果テーブルの生成
		/// </summary>
		/// <param name="procResult"></param>
		/// <param name="procMessage"></param>
		/// <returns></returns>
		private DataTable CreateResultDataTable(string procResult, string procMessage)
		{

			DataTable resultDataTable = new DataTable(WebServiceConst.TAB_NAME_RESULTTABLE);

			resultDataTable.Columns.Add(WebServiceConst.COL_PROCRESULT);
			resultDataTable.Columns.Add(WebServiceConst.COL_PROCMESSAGE);

			DataRow dr1 = resultDataTable.NewRow();
			dr1[WebServiceConst.COL_PROCRESULT] = procResult;
			dr1[WebServiceConst.COL_PROCMESSAGE] = procMessage;

			resultDataTable.Rows.Add(dr1);

			return resultDataTable;
		}

		#endregion

		#region ユーザIDテーブルの生成

		/// <summary>
		/// ユーザIDテーブルの生成
		/// </summary>
		/// <param name="userid">生成結果に含めるユーザID</param>
		/// <returns></returns>
		private DataTable CreateUserIDTable(string userid)
		{

			DataTable userIDTable = new DataTable(WebServiceConst.TAB_NAME_USERIDTABLE);

			userIDTable.Columns.Add(WebServiceConst.COL_USERID);

			DataRow dr1 = userIDTable.NewRow();
			dr1[WebServiceConst.COL_USERID] = userid;

			userIDTable.Rows.Add(dr1);

			return userIDTable;

		}

		#endregion

		#region NullまたはDBNullかどうかをチェック

		/// <summary>
		/// NullまたはDBNullかどうかをチェック
		/// </summary>
		/// <param name="targetValue"></param>
		/// <returns></returns>
		private bool IsDBNullAndNullCheck(object targetValue)
		{
			if (targetValue == null)
			{
				return true;
			}

			if (targetValue == DBNull.Value)
			{
				return true;
			}

			return false;

		}

		#endregion

		#endregion
	}
}