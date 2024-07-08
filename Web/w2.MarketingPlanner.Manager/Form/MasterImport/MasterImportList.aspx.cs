/*
=========================================================================================================
  Module      : マスタアップロードページ処理(MasterImportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class Form_MasterImport_MasterImportInput : BasePage
{
	private string m_shopDirectory = null;

	private string m_masterUploadDirectory = null;

	const string REQUEST_KEY_EXTERNAL_TYPE = "etype";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ショップのディレクトリ決定
		//------------------------------------------------------
		m_shopDirectory = Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR + this.LoginOperatorShopId + @"\";

		if (!IsPostBack)
		{
			// マスタ種別取得
			foreach (ListItem item in GetMasterItemFromXml(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MASTER_UPLOAD_SETTING))
			{
				ddlMaster.Items.Add(item);
			}

			//------------------------------------------------------
			// ドロップダウンデフォルト選択
			//------------------------------------------------------
			// デフォルト選択（削除処理後）
			if (Request[Constants.REQUEST_KEY_SELECTED_MASTER] != null)
			{
				ddlMaster.SelectedIndex = -1;// 一度解除しないとエラー出ることがある
				ddlMaster.Items.FindByValue(Request[Constants.REQUEST_KEY_SELECTED_MASTER]).Selected = true;
			}
			// デフォルト選択（アップロード後）
			else if (Session[Constants.SESSION_KEY_PARAM] != null)
			{
				try
				{
					Hashtable param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
					string selectedDirectory = (string)param[Constants.REQUEST_KEY_SELECTED_MASTER];
					if (selectedDirectory != null)
					{
						ddlMaster.SelectedIndex = -1;// 一度解除しないとエラー出ることがある
						ddlMaster.Items.FindByValue(selectedDirectory).Selected = true;
					}
				}
				catch
				{
					// 他の画面のSESSION_PARAMを取得した場合などは例外をスルーする
				}
			}
		}

		//------------------------------------------------------
		// 選択中マスタでのファイルの実アップロードディレクトリ決定
		//  （ドロップダウンデフォルト選択処理の後におく必要がある）
		//------------------------------------------------------
		m_masterUploadDirectory = m_shopDirectory + ddlMaster.SelectedValue + @"\" + Constants.DIRNAME_MASTERIMPORT_UPLOAD + @"\";

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 存在するファイル名取得＆各種表示設定
			//------------------------------------------------------
			if (Directory.Exists(m_masterUploadDirectory))
			{
				string[] existedFiles = Directory.GetFiles(m_masterUploadDirectory);
				if (existedFiles.Length != 0)
				{
					// アップロード不能に
					btnUpload.Enabled = false;

					// 存在するファイル名をデータソースへ設定
					rExistFiles.DataSource = existedFiles;

					// 表示制御
					trEnableUploadMessage.Visible = false;		// 「アップロード可能です」文言非表示
					trDisableUploadMessage.Visible = true;		// 「アップロードできません」文言表示
					rExistFiles.Visible = true;					// 存在ファイルリスト表示
				}
				else
				{
					// アップロード可能に
					btnUpload.Enabled = true;

					// 表示制御
					trEnableUploadMessage.Visible = true;		// 「アップロード可能です」文言表示
					trDisableUploadMessage.Visible = false;		// 「アップロードできません」文言非表示
					rExistFiles.Visible = false;				// 存在ファイルリスト非表示
				}
			}
			else
			{
				// アップロード可能に
				btnUpload.Enabled = true;

				// 表示制御
				trEnableUploadMessage.Visible = true;		// 「アップロード可能です」文言表示
				trDisableUploadMessage.Visible = false;		// 「アップロードできません」文言非表示
				rExistFiles.Visible = false;				// 存在ファイルリスト非表示
			}

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			ddlMaster.DataBind();
			rExistFiles.DataBind();
		}
	}

	/// <summary>
	/// マスター種別をXMLから読み込んで返却する
	/// </summary>
	/// <param name="xmlPath">ファイルパス</param>
	/// <returns>マスター種別</returns>
	private List<ListItem> GetMasterItemFromXml(string xmlPath)
	{
		XDocument xDoc = XDocument.Load(xmlPath);

		var infoList = from xe in xDoc.Descendants("Master")
					   select new
					   {
						   Name = xe.Element("Name").Value,
						   Directory = xe.Element("Directory").Value
					   };

		List<ListItem> result = new List<ListItem>();
		foreach (var info in infoList)
		{
			// マスタの種別によって判断する
			switch (info.Directory)
			{
				case Constants.TABLE_ADVCODE:
				case Constants.TABLE_ADVCODEMEDIATYPE:
					if (Constants.W2MP_AFFILIATE_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Directory));
					}
					break;

				case Constants.TABLE_COUPON:
				case Constants.TABLE_USERCOUPON:
				case Constants.TABLE_COUPONUSEUSER:
					if (Constants.W2MP_COUPON_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Directory));
					}
					break;

				case Constants.TABLE_USERPOINT:
					if (Constants.W2MP_POINT_OPTION_ENABLED && (Constants.CROSS_POINT_OPTION_ENABLED == false))
					{
						result.Add(new ListItem(info.Name, info.Directory));
					}
					break;

				case Constants.TABLE_DMSHIPPINGHISTORY:
					if (Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Directory));
					}
					break;

				default:
					result.Add(new ListItem(info.Name, info.Directory));
					break;
			}
		}
		return result;
	}

	/// <summary>
	/// ファイルアップロードボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpload_Click(object sender, System.EventArgs e)
	{
		// ファイル指定あり？
		if (fUpload.Value != "")
		{
			// ディレクトリが存在しなければ作成
			if (Directory.Exists(m_masterUploadDirectory) == false)
			{
				Directory.CreateDirectory(m_masterUploadDirectory);
			}

			// CSVファイルじゃなければエラー
			if (fUpload.PostedFile.FileName.EndsWith(".csv") == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// ファイルアップロード処理
			string filePath = m_masterUploadDirectory + Path.GetFileName(fUpload.PostedFile.FileName);
			if (File.Exists(filePath))
			{
				// ファイルが既に存在していたらエラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_ALREADY_EXISTS);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			else
			{
				// ファイル存在チェック
				if (fUpload.PostedFile.InputStream.Length == 0)
				{
					// ファイルなしエラー
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNFIND);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				else
				{
					try
					{
						// ファイルアップロード実行
						fUpload.PostedFile.SaveAs(filePath);
					}
					catch (System.UnauthorizedAccessException ex)
					{
						// ファイルアップロード権限エラー（ログにも記録）
						AppLogger.WriteError(ex);
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_ERROR);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
				}
			}

			//------------------------------------------------------
			// 一覧ページへ（選択マスタ保持しておく）
			//------------------------------------------------------
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_LIST + "?" + REQUEST_KEY_EXTERNAL_TYPE + "=" + ddlMaster.SelectedValue);
		}
		else
		{
			// 「ファイルを選択してください」エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNSELECTED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// ドロップダウンリスト変更時処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlMaster_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// ページの有効期限切れを防ぐため、自ページへ遷移
		Hashtable param = new Hashtable();
		param.Add(Constants.REQUEST_KEY_SELECTED_MASTER, ddlMaster.SelectedValue);
		Session[Constants.SESSION_KEY_PARAM] = param;

		// 自ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_LIST);
	}

	/// <summary>
	/// アップロードファイルリストリピータコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rExistFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		string fileName = e.CommandArgument.ToString();
		string uploadFilePath = m_masterUploadDirectory + fileName;

		if (e.CommandName == "delete")
		{

			if (File.Exists(uploadFilePath))
			{
				File.Delete(uploadFilePath);
			}

			// ページ更新のためリロード
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_LIST);
		}
		else if (e.CommandName == "import")
		{
			// 処理ファイルディレクトリ作成
			string activeDirectory = m_shopDirectory + ddlMaster.SelectedValue + @"\" + Constants.DIRNAME_MASTERIMPORT_ACTIVE;
			if (Directory.Exists(activeDirectory) == false)
			{
				Directory.CreateDirectory(activeDirectory);
			}

			// 処理ファイルパス
			string activeFilePath = activeDirectory + @"\" + fileName;

			if (File.Exists(uploadFilePath))
			{
				// ファイル移動（プロセス生成に時間がかかることがあるため、移動後のファイルをバッチへ渡す。）
				File.Move(uploadFilePath, activeFilePath);

				// プロセス実行（移動後ファイルのフルパスを引数として渡す。）
				System.Diagnostics.Process.Start(Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE, "\"" + activeFilePath + "\"");	// スペースが含まれても処理されるように「"」でくくる
			}

			// 取込実行完了画面へ遷移
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_COMPLETE);
		}
	}

	/// <summary>アップロード可能なファイルの最大サイズ（メガバイト）</summary>
	protected int MaxRequestLength
	{
		get
		{
			HttpRuntimeSection httpRuntimeSection = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
			int maxRequestLength = httpRuntimeSection.MaxRequestLength;
			return maxRequestLength / 1024;
		}
	}
}
