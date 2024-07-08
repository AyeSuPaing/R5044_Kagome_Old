/*
=========================================================================================================
  Module      : ワンタイムパスワード発行処理クラス(OnetimePasswordProc.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : ワンタイムパスワード発行の処理クラス
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using w2.Plugin.P0011_Intercon.WebService.Util;
using System.IO;

namespace w2.Plugin.P0011_Intercon.WebService.Inproc
{
	public class OnetimePasswordProc : IWebServiceInProc 
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

			DataSet rtnDS = new DataSet();

			DataTable inputdt = null;

			//引数データ分解
			try
			{
				if (ds.Tables.Contains(WebServiceConst.TAB_NAME_USERTABLE) == false)
				{
					throw new Exception("データセットにテーブル【" + WebServiceConst.TAB_NAME_USERTABLE + "】がありません");
				}

				//エラー時のログのためコピー
				inputdt = ds.Tables[WebServiceConst.TAB_NAME_USERTABLE].Copy();

				DataTable dt = ds.Tables[WebServiceConst.TAB_NAME_USERTABLE];

				//w2ユーザID取り出し
				string w2ID = (string)dt.Rows[0][WebServiceConst.COL_W2_USERID];

				//インターコムユーザID取り出し
				string icID = (string)dt.Rows[0][WebServiceConst.COL_INTERCOM_USERID];
				
				//ワンタイムパスワード生成
				PasswordGenerator passUtil = new PasswordGenerator();
				string pass = passUtil.GenerateOnetimePass();

				//ワンタイムパスワードをDBへ登録
				//パラメータ用のデータテーブル作成
				DataTable paramDataTable = CreateParamTable(w2ID, icID, pass);
				CommUtil._DBUtil().ExecuteSql(
					WebServiceConst.SQL_SECTION_UPDATE_ONETIMEPASS, paramDataTable); 

				//ワンタイムパステーブル
				rtnDS.Tables.Add(CreateOnetimepassTable(pass));

				//処理結果テーブル
				rtnDS.Tables.Add(CreateResultDataTable("Success", "正常終了"));

			}
			catch (Exception ex)
			{
				//エラーログ出力
				try
				{
					CommUtil._LogFileUtil().WriteErrorLog(ds, ex);
				}
				catch
				{

				}

				if (rtnDS.Tables.Contains("ResultData"))
				{
					//すでに処理結果テーブルが生成されていた場合
					rtnDS.Tables["ResultData"].Rows[0]["ProcResult"] = "Failed";
					rtnDS.Tables["ResultData"].Rows[0]["ProcMessage"] = "エラーが発生しました。エラーメッセージ→" + ex.Message;
				}
				else
				{
					rtnDS.Tables.Add(CreateResultDataTable("Failed", "エラーが発生しました。エラーメッセージ→" + ex.Message));
				}
			}
			finally
			{
				//特になし

			}

			return rtnDS;
		}

		#region ヘルパ

		/// <summary>
		/// ワンタイムパスワードテーブルの生成
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		private DataTable CreateOnetimepassTable(string pass)
		{
			DataTable resultDataTable = new DataTable("OnetimePasswordTable");

			resultDataTable.Columns.Add("OnetimePassword");

			DataRow dr1 = resultDataTable.NewRow();
			dr1["OnetimePassword"] = pass;

			resultDataTable.Rows.Add(dr1);

			return resultDataTable;
		}

		/// <summary>
		/// SQLパラメータ用テーブルの生成
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		private DataTable CreateParamTable(string w2ID,string icID,string pass)
		{
			DataTable paramDataTable = new DataTable("ParamTable");

			paramDataTable.Columns.Add("w2user_id");
			paramDataTable.Columns.Add("icuser_id");
			paramDataTable.Columns.Add("onetimepass");

			DataRow dr1 = paramDataTable.NewRow();
			dr1["w2user_id"] = w2ID;
			dr1["icuser_id"] = icID;
			dr1["onetimepass"] = pass;

			paramDataTable.Rows.Add(dr1);

			return paramDataTable;
		}

		/// <summary>
		/// 処理結果テーブルの生成
		/// </summary>
		/// <param name="procResult"></param>
		/// <param name="procMessage"></param>
		/// <returns></returns>
		private DataTable CreateResultDataTable(string procResult, string procMessage)
		{

			DataTable resultDataTable = new DataTable("ResultData");

			resultDataTable.Columns.Add("ProcResult");
			resultDataTable.Columns.Add("ProcMessage");

			DataRow dr1 = resultDataTable.NewRow();
			dr1["ProcResult"] = procResult;
			dr1["ProcMessage"] = procMessage;

			resultDataTable.Rows.Add(dr1);

			return resultDataTable;
		}

		/// <summary>
		/// ユーザIDテーブルの生成
		/// </summary>
		/// <returns>ユーザIDはyyyyMMddHHmmssfffとする</returns>
		private DataTable CreateUserIDTable()
		{
			return CreateUserIDTable(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
		}
		/// <summary>
		/// ユーザIDテーブルの生成
		/// </summary>
		/// <param name="userid">生成結果に含めるユーザID</param>
		/// <returns></returns>
		private DataTable CreateUserIDTable(string userid)
		{

			DataTable userIDTable = new DataTable("UserIDTable");

			userIDTable.Columns.Add("UserID");

			DataRow dr1 = userIDTable.NewRow();
			dr1["UserID"] = userid;

			userIDTable.Rows.Add(dr1);

			return userIDTable;

		}

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
	}
}