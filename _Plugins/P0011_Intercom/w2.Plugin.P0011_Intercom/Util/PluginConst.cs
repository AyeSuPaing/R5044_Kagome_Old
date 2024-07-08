/*
=========================================================================================================
  Module      : プラグイン用定数クラス(PluginConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : プラグインで利用する定数を宣言する。
=========================================================================================================
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Plugin.P0011_Intercom.Util
{
	/// <summary>
	/// プラグイン用定数クラス
	/// </summary>
	class PluginConst
	{
		/// <summary>DBコネクション定義ConfigSection名</summary>
		public const string CONNECTION_KEY = "w2Database";
		/// <summary>SQL定義XMLファイル名</summary>
		public const string SQLXML_FILENAME = "SqlSettings.xml";
		/// <summary>プラグイン設定XMLファイル名</summary>
		public const string SETTINGXM_FILENAME = "P0011_IntercomPluginConfig.xml";

		#region 処理区分定義

		/// <summary>処理区分_会員登録</summary>
		public const string PROC_TYPE_USER_REGIST = "UserRegist";
		/// <summary>処理区分_会員変更</summary>
		public const string PROC_TYPE_USER_MODIFY = "UserModify";
		/// <summary>処理区分_会員退会</summary>
		public const string PROC_TYPE_USER_DELETE = "UserDelete";
		/// <summary>処理区分_シリアルチェック</summary>
		public const string PROC_TYPE_SERIAL_CHECK = "SerialCheck";
		/// <summary>処理区分_シリアル商品チェック</summary>
		public const string PROC_TYPE_SERIAL_PRODUCT_CHECK = "SerialProductCheck";
		/// <summary>処理区分_シリアル抹消</summary>
		public const string PROC_TYPE_SERIAL_DELETE = "SerialDelete";
		/// <summary>処理区分_レコメンド</summary>
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

		#region SQLセクション定義

		/// <summary>SQLセクション_会員登録</summary>
		public const string SQL_SECTION_ID_USER_REGIST = "IUserRegisteredPlugin"; 
		/// <summary>SQLセクション_会員登録後反映</summary>
		public const string SQL_SECTION_ID_USER_REGIST_AFTER_UPDATE = "IUserRegisteredPlugin_AfterUpdate";
		/// <summary>SQLセクション_会員変更</summary>
		public const string SQL_SECTION_ID_USER_MODIFY = "IUserModifiedPlugin"; 
		/// <summary>SQLセクション_会員退会</summary>
		public const string SQL_SECTION_ID_USER_DELETE = "IUserWithdrawedPlugin";
		//SQLセクション_シリアルチェック
		public const string SQL_SECTION_ID_SERIAL_CHECK = "IOrderValidatingPlugin";
		//SQLセクション_シリアル抹消
		public const string SQL_SECTION_ID_SERIAL_DELETE = "IOrderCompletedPlugin";
		//SQLセクション_ログイン時
		public const string SQL_SECTION_ID_LOGIN = "IUserLoggedInPlugin";
		//SQLセクション_ログイン時のカート情報削除
		public const string SQL_SECTION_ID_LOGIN_CART_DELETE = "IUserLoggedInPluginCartDelete";
		//SQLセクション_注文完了時のシリアル番号更新
		public const string SQL_SECTION_ID_ORDER_COMP_SERIAL_UPDATE = "IOrderCompletePlugin";

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
		public const string TAB_RECOMMEND = "EventTable"; //レコメンドデータテーブル名


		//##############################################################################################
		//カラム名
		//##############################################################################################

		public const string COL_PROCTYPE = "ProcType"; //処理区分カラム名
		public const string COL_PROCRESULT = "ProcResult"; //処理結果区分カラム名
		public const string COL_PROCMSG = "ProcMessage"; //処理メッセージカラム名

		public const string COL_EVENTID = "EventID"; //レコメンドイベントID
		public const string COL_SERIALFLAG = "SerialFlag"; //レコメンドイベントシリアル管理フラグ
		public const string COL_DISPORDER = "DisplayOrder"; //レコメンドイベント並び順


		public const string COL_SERIALNO = "SerialNo"; //シリアルNoのハッシュキー
		public const string COL_EVENT_PRODUCT = "EventProduct"; //イベント商品情報のハッシュキー

		public const string COL_ORDER_ID = "order_id"; //注文番号カラム名
		public const string COL_RELATION_MEMO = "relation_memo"; //外部連携メモ
		
			
		#endregion

		#region プラグインXMLキー

		/// <summary>エラー時メール送信先</summary>
		public const string SETTING_KEY_ERROR_TOLIST = "ErrorToList";
		/// <summary>エラー時メールCC</summary>
		public const string SETTING_KEY_ERROR_CCLIST = "ErrorCcList";
		/// <summary>エラー時メールBCC</summary>
		public const string SETTING_KEY_ERROR_BCCLIST = "ErrorBccList";
		/// <summary>エラー時件名</summary>
		public const string SETTING_KEY_ERROR_SUBJECT = "ErrorSubject";
		/// <summary>エラー時メール本文</summary>
		public const string SETTING_KEY_ERROR_BODYTEMPLATE = "ErrorBodyTemplate";
		/// <summary>エラー時メール送信元</summary>
		public const string SETTING_KEY_ERROR_FROM = "ErrorFrom";

		/// <summary>エラー時メールALL送信先</summary>
		public const string SETTING_KEY_ERROR_TOLIST_ALL = "ErrorToAllList";
		/// <summary>エラー時メールCCALL</summary>
		public const string SETTING_KEY_ERROR_CCLIST_ALL = "ErrorCcAllList";
		/// <summary>エラー時メールBCCALL</summary>
		public const string SETTING_KEY_ERROR_BCCLIST_ALL = "ErrorBccAllList";
		/// <summary>エラー時件名ALL</summary>
		public const string SETTING_KEY_ERROR_SUBJECT_ALL = "ErrorSubjectAll";
		/// <summary>エラー時メール本文ALL</summary>
		public const string SETTING_KEY_ERROR_BODYTEMPLATE_ALL = "ErrorBodyTemplateAll";
		/// <summary>エラー時メール送信元ALL</summary>
		public const string SETTING_KEY_ERROR_FROM_ALL = "ErrorFromAll";
		
		/// <summary>退会時メール送信先</summary>
		public const string SETTING_KEY_USERDELSYNC_TOLIST = "UserDelSyncToList";
		/// <summary>退会時メールCC</summary>
		public const string SETTING_KEY_USERDELSYNC_CCLIST = "UserDelSyncCcList";
		/// <summary>退会時メールBCC</summary>
		public const string SETTING_KEY_USERDELSYNC_BCCLIST = "UserDelSyncBccList";
		/// <summary>退会時件名</summary>
		public const string SETTING_KEY_USERDELSYNC_SUBJECT = "UserDelSyncSubject";
		/// <summary>退会時メール本文</summary>
		public const string SETTING_KEY_USERDELSYNC_BODYTEMPLATE = "UserDelSyncBodyTemplate";
		/// <summary>退会時メール送信元</summary>
		public const string SETTING_KEY_USERDELSYNC_FROM = "UserDelSyncFrom";
		
		/// <summary>メールアドレス区切り文字</summary>
		public const string SETTING_KEY_MAIL_SPLIT_STR = "MailSplitStr";
		/// <summary>ウェブサービス利用する、しないのフラグ</summary>
		public const string SETTING_KEY_CALL_WEBSRV_FLG = "CallWebServiceFlag";
		/// <summary>送受信データログディレクトリ</summary>
		public const string SETTING_KEY_SENDDATA_LOGDIR = "SendDataLogDir";
		/// <summary>エラーCSVログディレクトリ</summary>
		public const string SETTING_KEY_ERR_CSV_LOGDIR = "ErrCsvLogDir";
		/// <summary>インターコム用のSSO用URL</summary>
		public const string SETTINF_KEY_IC_SSO_URL = "IntercomSSOUrl";
		/// <summary>インターコム用のSSO用URL付与文字列</summary>
		public const string SETTING_KEY_IC_SSO_URL_ADD_HEAD = "IntercomSSOUrl";
		/// <summary>w2⇒Intercomのキー</summary>
		public const string SETTING_KEY_ENC = "EncryptKey";
		/// <summary>Intercom⇒w2のキー</summary>
		public const string SETTING_KEY_DEC = "DecryptKey";
		/// <summary>w2キー</summary>
		public const string SETTING_W2_KEY = "w2Key";
		/// <summary>w2IV</summary>
		public const string SETTING_W2_IV = "w2IV";
		/// <summary>webServiceAuth</summary>
		public const string SETTING_W2_WEBSERVICE_AUTH = "webServiceAuth";
		/// <summary>webServiceID</summary>
		public const string SETTING_W2_WEBSERVICE_ID = "webServiceID";
		/// <summary>webServicePA</summary>
		public const string SETTING_W2_WEBSERVICE_PA = "webServicePA";
		/// <summary>webServiceDO</summary>
		public const string SETTING_W2_WEBSERVICE_DO = "webServiceDO";
	

		/// <summary>デバッグフラグ</summary>
		public const string SETTING_KEY_DEBUG_FLAG = "DebugFlag";

		
		#endregion

		#region キャッシュキー

		/// <summary>Configユーティリティ用のキャッシュキー</summary>
		public const string CACHE_KEY_CONFIGUTIL = "ConfigUtil";
		/// <summary>DBユーティリティ用のキャッシュキー</summary>
		public const string CACHE_KEY_DBUTIL = "DbUtil";

		#endregion

		#region セッション情報

		/// <summary>param_data用セッションキー</summary>
		public const string SESSION_KEY_PARAM_DATA = "param_data";

		/// <summary>カートリストのセッションキー</summary>
		public const string SESSION_KEY_CART_LIST = "w2cFront_cart_list";

		/// <summary>HOST_DATAのメールアドレス格納用KEY</summary>
		public const string HOST_DATA_KEY_MAIL_ADR = "mail_addr";

		/// <summary>w2ユーザID格納用KEY</summary>
		public const string PLUGIN_SESSION_KEY_W2ID = "w2cFront_login_user_id";

		/// <summary>HOST_DATAの注文番号格納用KEY</summary>
		public const string HOST_DATA_KEY_ORDER_ID = "order_id";

		/// <summary>HOST_DATAのOrderProductList格納用KEY</summary>
		public const string HOST_DATA_KEY_ORDER_PRODUCT_LIST = "OrderProductList";


		/*
		 * 
		 *プラグインで設定するセッションのKEY
		 *プラグイン独自の仕様
		 * 
		 * 
		*/

		

		/// <summary>インターコムユーザID格納用KEY</summary>
		public const string PLUGIN_SESSION_KEY_ICID = "PLUGIN_SESSION_KEY_ICID";
		/// <summary>レコメンド情報格納用KEY</summary>
		public const string PLUGIN_SESSION_KEY_RECOMMEND = "PLUGIN_SESSION_KEY_RECOMMEND";
		/// <summary>レコメンドイベント情報が正常に取得できたかのフラグ</summary>
		public const string PLUGIN_SESSION_KEY_RECOMMEND_FLAG = "PLUGIN_SESSION_KEY_RECOMMEND_FLAG";

		#endregion


	}
}
