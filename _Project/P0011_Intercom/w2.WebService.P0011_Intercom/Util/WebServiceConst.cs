/*
=========================================================================================================
  Module      : w2WebService用定数クラス(WebServiceConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : w2WebServiceで利用する定数を宣言する。
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace w2.Plugin.P0011_Intercom.WebService.Util
{
	public class WebServiceConst
	{
		public const string CONNECTION_KEY = "w2Database"; //コネクション定義名
		public const string DS_NAME_RETUNR = "ReturnData"; //戻り値用データセット名

		#region AppSettingsKey

		public const string APP_SETTINGS_KEY_RECEIVE_DATA_LOG_DIR = "ReceiveDataLogDir";
		public const string APP_SETTINGS_KEY_ERROR_CSV_LOG_DIR = "ErrorCsvLogDir";
		public const string APP_SETTINGS_KEY_SQL_FILEPATH = "SqlFilePath";
		
		#endregion

		#region 処理結果区分

		public const string PROC_RESULT_SUCCESS = "Success";
		public const string PROC_RESULT_FAILED = "Failed";
		
		#endregion

		#region 処理区分定義

		public const string PROC_TYPE_USER_REGIST = "UserRegist"; //処理区分_会員登録
		public const string PROC_TYPE_USER_MODIFY = "UserModify"; //処理区分_会員変更
		public const string PROC_TYPE_USER_DELETE = "UserDelete"; //処理区分_会員退会
	
		#endregion

		#region SQLセクション定義

		public const string SQL_SECTION_INSERT_USERDATA = "InsertUserData"; //SQLセクション_会員登録
		public const string SQL_SECTION_UPDATE_USERDATA = "UpdateUserData"; //SQLセクション_会員変更
		public const string SQL_SECTION_DELETE_USERDATA = "DeleteUserData"; //SQLセクション_会員退会
		public const string SQL_SECTION_UPDATE_ONETIMEPASS= "UpdateOnetimePass"; //SQLセクション_ワンタイムパスワードUpdate
		
		#endregion

		#region データテーブル定義

		public const string TAB_NAME_PROCTABLE = "ProcTable";
		public const string TAB_NAME_USERTABLE = "UserTable";
		public const string TAB_NAME_RESULTTABLE = "ResultTable";
		public const string TAB_NAME_USERIDTABLE = "UserIDTable";
		public const string TAB_NAME_ONETIMEPASSWORD = "OnetimePasswordTable";

		public const string COL_PROCRESULT = "ProcResult";
		public const string COL_PROCMESSAGE = "ProcMessage";
		public const string COL_USERID = "UserID";
		public const string COL_LINKEDUSERID = "LinkedUserID";
		public const string COL_ONETIMEPASSWORD = "OnetimePassword";
		public const string COL_PROCTYPE = "ProcType";

		public const string COL_USER_ID = "user_id";
		public const string COL_USER_KBN = "user_kbn";
		public const string COL_MALL_ID = "mall_id";
		public const string COL_NAME = "name";
		public const string COL_NAME1 = "name1";
		public const string COL_NAME2 = "name2";
		public const string COL_NAME_KANA = "name_kana";
		public const string COL_NAME_KANA1 = "name_kana1";
		public const string COL_NAME_KANA2 = "name_kana2";
		public const string COL_NICK_NAME = "nick_name";
		public const string COL_MAIL_ADDR = "mail_addr";
		public const string COL_MAIL_ADDR2 = "mail_addr2";
		public const string COL_ZIP = "zip";
		public const string COL_ZIP1 = "zip1";
		public const string COL_ZIP2 = "zip2";
		public const string COL_ADDR = "addr";
		public const string COL_ADDR1 = "addr1";
		public const string COL_ADDR2 = "addr2";
		public const string COL_ADDR3 = "addr3";
		public const string COL_ADDR4 = "addr4";
		public const string COL_TEL1 = "tel1";
		public const string COL_TEL1_1 = "tel1_1";
		public const string COL_TEL1_2 = "tel1_2";
		public const string COL_TEL1_3 = "tel1_3";
		public const string COL_SEX = "sex";
		public const string COL_BIRTH = "birth";
		public const string COL_BIRTH_YEAR = "birth_year";
		public const string COL_BIRTH_MONTH = "birth_month";
		public const string COL_BIRTH_DAY = "birth_day";
		public const string COL_COMPANY_NAME = "company_name";
		public const string COL_COMPANY_POST_NAME = "company_post_name";
		public const string COL_ADVCODE_FIRST = "advcode_first";
		public const string COL_ATTRIBUTE1 = "attribute1";
		public const string COL_LOGIN_ID = "login_id";
		public const string COL_PASSWORD = "password";
		public const string COL_QUESTION = "question";
		public const string COL_ANSWER = "answer";
		public const string COL_CAREER_ID = "career_id";
		public const string COL_MOBILE_UID = "mobile_uid";
		public const string COL_REMOTE_ADDR = "remote_addr";
		public const string COL_MAIL_FLG = "mail_flg";
		public const string COL_USER_MEMO = "user_memo";
		public const string COL_DEL_FLG = "del_flg";
		public const string COL_LAST_CHANGED = "last_changed";
		public const string COL_MEMBER_RANK_ID = "member_rank_id";

		public const string COL_W2_USERID = "UserID";
		public const string COL_INTERCOM_USERID = "LinkedUserID";



		#endregion

	}
}