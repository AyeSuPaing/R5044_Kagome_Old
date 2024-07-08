/*
=========================================================================================================
  Module      : マスタアップロードページ処理(MasterImportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class Form_MasterUpload_MasterUploadInput : BasePage
{
	private string m_strShopDirectory = null;

	private string m_strMasterUploadDirectory = null;

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
		m_strShopDirectory = Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR + this.LoginOperatorShopId + @"\";

		if (!IsPostBack)
		{
			// マスタ種別取得
			foreach(ListItem item in GetMasterItemFromXml(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MASTER_UPLOAD_SETTING))
			{
				// 商品カテゴリオプション
				if ((Constants.PRODUCT_CTEGORY_OPTION_ENABLE == false) && (item.Value == Constants.TABLE_PRODUCTCATEGORY)) 
					continue;
				
				// 商品タグオプション
				if((Constants.PRODUCT_TAG_OPTION_ENABLE == false) && (item.Value == Constants.TABLE_PRODUCTTAG))
					continue;

				// 商品在庫オプション
				if((Constants.PRODUCT_STOCK_OPTION_ENABLE == false) && (item.Value == Constants.TABLE_PRODUCTSTOCK))
					continue;

				// オペレータマスタ
				if ((Constants.REALSHOP_OPTION_ENABLED == false) && (item.Value == Constants.TABLE_SHOPOPERATOR))
					continue;

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
					Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
					string strSelectedDirectory = (string)htParam[Constants.REQUEST_KEY_SELECTED_MASTER];
					if (strSelectedDirectory != null)
					{
						ddlMaster.SelectedIndex = -1;// 一度解除しないとエラー出ることがある
						ddlMaster.Items.FindByValue(strSelectedDirectory).Selected = true;
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
		m_strMasterUploadDirectory = m_strShopDirectory + ddlMaster.SelectedValue + @"\" + Constants.DIRNAME_MASTERIMPORT_UPLOAD + @"\";

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 存在するファイル名取得＆各種表示設定
			//------------------------------------------------------
			if (Directory.Exists(m_strMasterUploadDirectory))
			{
				string[] strExistedFiles = Directory.GetFiles(m_strMasterUploadDirectory);
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
			ddlMaster.DataBind();
			rExistFiles.DataBind();
		}
	}

	/// <summary>
	/// マスター種別をXMLから読み込んで返却する
	/// </summary>
	/// <returns></returns>
	private List<ListItem> GetMasterItemFromXml(string xmlPath)
	{
		var xdoc = XDocument.Load(xmlPath);

		var infolist = xdoc.Descendants(Constants.XML_MASTERUPLOADSETTING_MASTER_ELEMENT)
			.Select(item =>
				new
				{
					Name = item.Element(Constants.XML_MASTERUPLOADSETTING_NAME_ELEMENT).Value,
					Dir = item.Element(Constants.XML_MASTERUPLOADSETTING_DIRECTORY_ELEMENT).Value
				});

		var result = new List<ListItem>();
		foreach (var info in infolist)
		{			
			// マスタの種別によって判断する
			switch (info.Dir)
			{
				case Constants.TABLE_USER:
				case Constants.TABLE_USEREXTEND:
					if (Constants.MASTERUPLOAD_USER_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;

				case Constants.TABLE_SERIALKEY:
					if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;

				case Constants.TABLE_PRODUCTPRICE:
					// 商品価格マスタは会員ランクOPONのときのみ使用する（会員価格でしか使用しないため）
					if (Constants.MEMBER_RANK_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;

				case Constants.TABLE_PRODUCTEXTEND:
					if (Constants.MALLCOOPERATION_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;

				case Constants.TABLE_PRODUCTSALEPRICE:
					if (Constants.PRODUCT_SALE_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;

				case Constants.TABLE_REALSHOP:
					if (Constants.REALSHOP_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;

				case Constants.TABLE_REALSHOPPRODUCTSTOCK:
					if (Constants.REALSHOP_OPTION_ENABLED)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;

				case Constants.TABLE_NAMETRANSLATIONSETTING:
					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						result.Add(new ListItem(info.Name, info.Dir));
					}
					break;
					
				default:
					result.Add(new ListItem(info.Name, info.Dir));
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
			if (Directory.Exists(m_strMasterUploadDirectory) == false)
			{
				Directory.CreateDirectory(m_strMasterUploadDirectory);
			}

			// CSVファイルじゃなければエラー
			if (fUpload.PostedFile.FileName.EndsWith(".csv") == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// ファイルアップロード処理
			string strFilePath = m_strMasterUploadDirectory + Path.GetFileName(fUpload.PostedFile.FileName);
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
		Hashtable htParam = new Hashtable();
		htParam.Add(Constants.REQUEST_KEY_SELECTED_MASTER, ddlMaster.SelectedValue);
		Session[Constants.SESSION_KEY_PARAM] = htParam;

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
		string strFileName = e.CommandArgument.ToString();
		string strUploadFilePath = m_strMasterUploadDirectory + strFileName;

		if (e.CommandName == "delete")
		{

			if (File.Exists(strUploadFilePath))
			{
				File.Delete(strUploadFilePath);
			}

			// ページ更新のためリロード
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_LIST);
		}
		else if (e.CommandName == "import")
		{
			// 処理ファイルディレクトリ作成
			string strActiveDirectory = m_strShopDirectory + ddlMaster.SelectedValue + @"\" + Constants.DIRNAME_MASTERIMPORT_ACTIVE;
			if (Directory.Exists(strActiveDirectory) == false)
			{
				Directory.CreateDirectory(strActiveDirectory);
			}

			// 処理ファイルパス
			string strActiveFilePath = strActiveDirectory + @"\" + strFileName;

			if (File.Exists(strUploadFilePath))
			{
				// ファイル移動（プロセス生成に時間がかかることがあるため、移動後のファイルをバッチへ渡す。）
				File.Move(strUploadFilePath, strActiveFilePath);

				// プロセス実行（移動後ファイルのフルパスを引数として渡す。）
				System.Diagnostics.Process.Start(Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE, "\"" + strActiveFilePath + "\"");	// スペースが含まれても処理されるように「"」でくくる
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
