using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Plugin.P0011_Intercom.CooperationWebService
{
	public class CooperationWebServiceConst
	{

		#region 処理区分定義
		/// <summary>処理区分_会員登録</summary>
		public const string PROC_TYPE_USER_REGIST = "UserRegist";
		/// <summary>処理区分_会員変更</summary>
		public const string PROC_TYPE_USER_MODIFY = "UserModify";
		/// <summary>処理区分_会員退会</summary>
		public const string PROC_TYPE_USER_DELETE = "UserDelete";
		/// <summary>処理区分_シリアルチェック</summary>
		public const string PROC_TYPE_SERIAL_CHECK = "SerialCheck";
		/// <summary>処理区分_シリアル抹消</summary>
		public const string PROC_TYPE_SERIAL_DELETE = "SerialDelete";
		/// <summary>処理区分_レコメンドイベント</summary>
		public const string PROC_TYPE_RECOMMEND_EVENT = "RecommendEvent";
		/// <summary>処理区分_レコメンド商品</summary>
		public const string PROC_TYPE_RECOMMEND_PRODUCT = "RecommendProduct";
		#endregion

		#region 処理結果区分

		/// <summary>処理結果区分_正常終了</summary>
		public const string PROC_RESULT_SUCCESS = "Success";
		/// <summary>処理結果区分_異常狩猟</summary>
		public const string PROC_RESULT_FAILED = "Failed";

		#endregion

		#region 連携用データテーブル定義

		//##############################################################################################
		//テーブル名
		//##############################################################################################

		public const string TAB_PROC = "ProcTable"; //処理区分用データテーブル名
		public const string TAB_RESULT = "ResultTable"; //処理結果用データテーブル名
		public const string TAB_USERID = "UserIDTable"; //ユーザーIDデータテーブル名
		public const string TAB_SERIAL = "SerialTable"; //シリアルNoデータテーブル名
		public const string TAB_USER = "UserTable"; //会員情報テーブル名
		public const string TAB_RECOMMEND = "RecommendTable"; //レコメンドデータテーブル名


		//##############################################################################################
		//カラム名
		//##############################################################################################

		public const string COL_PROCTYPE = "ProcType"; //処理区分カラム名
		public const string COL_PROCRESULT = "ProcResult"; //処理結果区分カラム名
		public const string COL_PROCMSG = "ProcMessage"; //処理メッセージカラム名


		#endregion

		#region 送受信方向

		public const string WAY_SEND = "送信";
		public const string WAY_RECEIVE = "受信";

		#endregion

	}
}
