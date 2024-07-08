/*
=========================================================================================================
  Module      : 外部ファイル取込一覧ページ処理(ExternalImportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using w2.Common.Web;
using w2.Domain;

public partial class Form_ExternalImport_ExternalImportList : BasePage
{
	const string REQUEST_KEY_EXTERNAL_TYPE = "etype";

	private string m_strShopDirectory = null;

	private string m_strUploadDirectory = null;

	/// <summary>Request key: external site id</summary>
	private string REQUEST_KEY_EXTERNAL_SITE_ID = "esid";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// ショップのディレクトリ決定
		//------------------------------------------------------
		m_strShopDirectory = Constants.PHYSICALDIRPATH_EXTERNAL_DIR + this.LoginOperatorShopId + @"\";

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// ディレクトリ名一覧取得（ドロップダウンセット）
			//------------------------------------------------------
			// XML読み込み
			XmlDocument objXml = new XmlDocument();
			objXml.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_EXTERNAL_IMPORT_SETTING);
			XmlNodeList xnSettings = objXml.SelectSingleNode("ExternalImportSetting").ChildNodes;

			// マスタ毎の設定取得
			foreach (XmlNode xnSetting in xnSettings)
			{
				if (xnSetting.NodeType == XmlNodeType.Comment)
				{
					continue;
				}

				ddlExternal.Items.Add(new ListItem(xnSetting.SelectSingleNode("Name").InnerText, xnSetting.SelectSingleNode("Directory").InnerText));
			}

			//------------------------------------------------------
			// ドロップダウンデフォルト選択
			//------------------------------------------------------
			// デフォルト選択（削除処理後）
			if (Request[REQUEST_KEY_EXTERNAL_TYPE] != null)
			{
				ddlExternal.SelectedIndex = -1;// 一度解除しないとエラー出ることがある
				ddlExternal.Items.FindByValue(Request[REQUEST_KEY_EXTERNAL_TYPE]).Selected = true;
			}
			// デフォルト選択（アップロード後）
			else if (Session[Constants.SESSION_KEY_PARAM] != null)
			{
				try
				{
					Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
					string strSelectedDirectory = (string)htParam[REQUEST_KEY_EXTERNAL_TYPE];
					if (strSelectedDirectory != null)
					{
						ddlExternal.SelectedIndex = -1;// 一度解除しないとエラー出ることがある
						ddlExternal.Items.FindByValue(strSelectedDirectory).Selected = true;
					}
				}
				catch
				{
					// 他の画面のSESSION_PARAMを取得した場合などは例外をスルーする
				}
			}

			// Set mall site information
			SetSiteInformation();
		}

		//------------------------------------------------------
		// アップロードディレクトリ設定
		//------------------------------------------------------
		m_strUploadDirectory = m_strShopDirectory + ddlExternal.SelectedValue + @"\" + Constants.DIRNAME_MASTERIMPORT_UPLOAD + @"\";

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 存在するファイル名取得＆各種表示設定
			//------------------------------------------------------
			if (Directory.Exists(m_strUploadDirectory))
			{
				string[] strExistedFiles = Directory.GetFiles(m_strUploadDirectory);
				if (strExistedFiles.Length != 0)
				{
					// アップロード不能に
					btnUpload.Enabled = false;

					// 存在するファイル名をデータソースへ設定
					rExistFiles.DataSource = strExistedFiles;

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
			ddlExternal.DataBind();
			rExistFiles.DataBind();
			ddlSiteInformation.DataBind();
		}
	}

	/// <summary>
	/// ファイルアップロードボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpload_Click(object sender, System.EventArgs e)
	{
		// Check mall site selected
		var isExternalTypeNewYahooOrder = (ddlExternal.SelectedValue == Constants.FLG_EXTERNALIMPORT_FILE_TYPE_NEW_YAHOO_ORDER);
		if (isExternalTypeNewYahooOrder
			&& string.IsNullOrEmpty(ddlSiteInformation.SelectedValue))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_EXTERNALIMPORT_SITE_UNSELECTED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// ファイル指定あり？
		if (fUpload.Value != "")
		{
			// ディレクトリが存在しなければ作成
			if (Directory.Exists(m_strUploadDirectory) == false)
			{
				Directory.CreateDirectory(m_strUploadDirectory);
			}

			// CSVファイルじゃなければエラー
			if (fUpload.PostedFile.FileName.EndsWith(".csv") == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// ファイルアップロード処理
			string strFilePath = m_strUploadDirectory + Path.GetFileName(fUpload.PostedFile.FileName);
			if (File.Exists(strFilePath))
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
						fUpload.PostedFile.SaveAs(strFilePath);
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
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_EXTERNALIMPORT_LIST)
				.AddParam(REQUEST_KEY_EXTERNAL_TYPE, ddlExternal.SelectedValue);
			if (isExternalTypeNewYahooOrder)
			{
				urlCreator.AddParam(REQUEST_KEY_EXTERNAL_SITE_ID, ddlSiteInformation.SelectedValue);
			}

			Response.Redirect(urlCreator.CreateUrl());
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
	protected void ddlExternal_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		var parameter = CreateParameter();
		Session[Constants.SESSION_KEY_PARAM] = parameter;

		// 自ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_EXTERNALIMPORT_LIST);
	}

	/// <summary>
	/// アップロードファイルリストリピータコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rExistFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var fileName = e.CommandArgument.ToString();
		var uploadFilePath = m_strUploadDirectory + fileName;

		if (e.CommandName == "delete")
		{
			if (File.Exists(uploadFilePath))
			{
				File.Delete(uploadFilePath);
			}

			// ページ更新のためリロード
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_EXTERNALIMPORT_LIST);
		}
		else if (e.CommandName == "import")
		{
			// 処理ファイルディレクトリ作成
			var activeDirectory = m_strShopDirectory + ddlExternal.SelectedValue + @"\" + Constants.DIRNAME_MASTERIMPORT_ACTIVE;
			if (Directory.Exists(activeDirectory) == false)
			{
				Directory.CreateDirectory(activeDirectory);
			}

			// 処理ファイルパス
			string strActiveFilePath = activeDirectory + @"\" + fileName;

			if (File.Exists(uploadFilePath))
			{
				// ファイル移動（プロセス生成に時間がかかることがあるため、移動後のファイルをバッチへ渡す。）
				File.Move(uploadFilePath, strActiveFilePath);

				// プロセス実行（移動後ファイルのフルパスを引数として渡す。）
				var arguments = String.Format(
					"\"{0}\" \"{1}\" \"{2}\" \"{3}\"",
					this.LoginOperatorShopId,
					ddlExternal.SelectedValue,
					fileName,
					ddlSiteInformation.SelectedValue);

				System.Diagnostics.Process.Start(Constants.PHYSICALDIRPATH_EXTERNAL_EXE, arguments);	// スペースが含まれても処理されるように「"」でくくる
			}

			// 取込実行完了画面へ遷移
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_EXTERNALIMPORT_COMPLETE);
		}
	}

	/// <summary>
	/// Site information selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSiteInformation_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		var parameter = CreateParameter();
		Session[Constants.SESSION_KEY_PARAM] = parameter;

		// 自ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_EXTERNALIMPORT_LIST);
	}

	/// <summary>
	/// Create parameter
	/// </summary>
	/// <returns>Parameter</returns>
	private Hashtable CreateParameter()
	{
		// ページの有効期限切れを防ぐため、自ページへ遷移
		var parameter = new Hashtable
		{
			{ REQUEST_KEY_EXTERNAL_TYPE, ddlExternal.SelectedValue },
		};
		if (ddlExternal.SelectedValue == Constants.FLG_EXTERNALIMPORT_FILE_TYPE_NEW_YAHOO_ORDER)
		{
			parameter.Add(REQUEST_KEY_EXTERNAL_SITE_ID, ddlSiteInformation.SelectedValue);
		}

		return parameter;
	}

	/// <summary>
	/// Set site information
	/// </summary>
	private void SetSiteInformation()
	{
		trSiteInformation.Visible = false;
		if (ddlExternal.SelectedValue != Constants.FLG_EXTERNALIMPORT_FILE_TYPE_NEW_YAHOO_ORDER) return;

		// Get yahoo mall site list
		var mallCooperationSettings = DomainFacade.Instance.MallCooperationSettingService.GetValidByMallKbn(
			this.LoginOperatorShopId,
			Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO);
		ddlSiteInformation.Items.AddRange(
			mallCooperationSettings
				.Select(item => new ListItem(string.Format(
					"{0} ({1})", item.MallName, item.MallId),
					item.MallId))
				.ToArray());

		trSiteInformation.Visible = true;

		// Check mall site id and set selected site id
		if (string.IsNullOrEmpty(Request[REQUEST_KEY_EXTERNAL_SITE_ID]) == false)
		{
			ddlSiteInformation.SelectedIndex = -1;
			if (ddlSiteInformation.Items.FindByValue(Request[REQUEST_KEY_EXTERNAL_SITE_ID]) != null)
			{
				ddlSiteInformation.Items.FindByValue(Request[REQUEST_KEY_EXTERNAL_SITE_ID]).Selected = true;
			}
		}
		// デフォルト選択（アップロード後）
		else if (Session[Constants.SESSION_KEY_PARAM] != null)
		{
			try
			{
				var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				var selectedSiteId = (string)param[REQUEST_KEY_EXTERNAL_SITE_ID];
				if (string.IsNullOrEmpty(selectedSiteId) == false)
				{
					ddlSiteInformation.SelectedIndex = -1;
					if (ddlSiteInformation.Items.FindByValue(selectedSiteId) != null)
					{
						ddlSiteInformation.Items.FindByValue(selectedSiteId).Selected = true;
					}
				}
			}
			catch
			{
				// 他の画面のSESSION_PARAMを取得した場合などは例外をスルーする
			}
		}
	}
}
