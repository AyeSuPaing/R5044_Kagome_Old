/*
=========================================================================================================
  Module      : マスタアップロードエラーログ情報出力ページ処理(MasterImportOutputLog.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using w2.Common.Util;

public partial class Form_MasterImport_MasterImportOutputLog : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// サニタイズしてエラーログの内容を出力
		string strOutput;
		if (GetErrorLog(out strOutput))
		{
			lContent.Text = WebSanitizer.HtmlEncode(strOutput);
		}
		// エラーはエラー画面で表示
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strOutput;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// エラーログの内容を得る。改行->Brへの変換等は行っていない。
	/// </summary>
	/// <param name="strOut">エラーログを出力する。エラーがあれば、エラーメッセージを出力する。</param>
	/// <returns>正常終了でtrue。エラーあればfalse。</returns>
	protected bool GetErrorLog(out string strOut)
	{
		const bool CONST_NO_ERROR = true;	// 正常終了
		const bool CONST_ERROR = false;		// エラー発生

		//------------------------------------------------------
		// 各パラメータ情報取得
		// ※ディレクトリトラバーサル防止のため、
		//   店舗ID、マスタ種別（ディレクトリにあたる）は".."、"\"、"/"を削除。
		//   ファイル名は"\"、"/"を削除。（".."はファイル名に含めることが可能なため）
		//------------------------------------------------------
		// 店舗ID
		string strShopId =
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MASTERIMPORTOUTPUTLOG_SHOP_ID]).Replace(@"\", "").Replace(@"/", "").Replace(@"..", "");
		// マスタ種別
		string strMaster =
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MASTERIMPORTOUTPUTLOG_MASTER]).Replace(@"\", "").Replace(@"/", "").Replace(@"..", "");
		// ファイル名
		string strLogFileName =
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MASTERIMPORTOUTPUTLOG_FILE_NAME]).Replace(@"\", "").Replace(@"/", "");

		//------------------------------------------------------
		// マスタ種別チェック
		//------------------------------------------------------
		bool blOutput;
		// ユーザー・ユーザポイント情報の場合、マスタアップロードのユーザー移行利用有無により出力制御
		switch (strMaster)
		{
			case Constants.TABLE_USER:
				blOutput = Constants.MASTERUPLOAD_USER_ENABLED;
				break;

			case Constants.TABLE_USERPOINT:
				blOutput = (Constants.MASTERUPLOAD_USER_ENABLED && Constants.W2MP_POINT_OPTION_ENABLED);	// ポイントオプション利用有無を考慮
				break;

			default:
				blOutput = true;
				break;
		}
		if (blOutput == false)
		{
			strOut = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTER_TYPE_INVALID);
			return CONST_ERROR;
		}

		//------------------------------------------------------
		// エラーログファイル指定チェック
		//------------------------------------------------------
		if (string.IsNullOrEmpty(strShopId)
			|| string.IsNullOrEmpty(strMaster)
			|| string.IsNullOrEmpty(strLogFileName))
		{
			strOut = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FILE_LOG_DONT_SPECIFIED);
			return CONST_ERROR;
		}

		//------------------------------------------------------
		// エラーログファイル存在チェック
		//------------------------------------------------------
		string strLogFilePath =
			Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR + @"\" + strShopId + @"\" + strMaster + @"\complete\" + strLogFileName;
		if (File.Exists(strLogFilePath) == false)
		{
			strOut = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FILE_LOG_DONT_EXIST);
			return CONST_ERROR;
		}

		//------------------------------------------------------
		// エラーログファイル出力
		//------------------------------------------------------
		using (StreamReader srReader = new StreamReader(strLogFilePath, Encoding.UTF8))
		{
			strOut = srReader.ReadToEnd();
		}

		return CONST_NO_ERROR;
	}
}
