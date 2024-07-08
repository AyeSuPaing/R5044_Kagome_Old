/*
=========================================================================================================
  Module      : ターゲットリストアップロードページ処理(TargetListUpload.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using w2.App.Common.TargetList;
using w2.Common.Util;

public partial class Form_TargetList_TargetListUpload : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponent(this.ActionStatus);

			// 画面表示・リダイレクト制御
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_UPLOAD_CONFIRM:
					DisplayUploadInfo();
					break;

				case Constants.ACTION_STATUS_UPLOAD:
					if (Directory.GetFiles(this.MasterUploadDirectory).Length != 0) RedirectToTargetListUploadConfirmPage();
					break;

				case Constants.ACTION_STATUS_COMPLETE:
					break;

				default:
					RedirectToTargetListUploadPage();
					break;
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent(string actionKbn)
	{
		switch (actionKbn)
		{
			case Constants.ACTION_STATUS_UPLOAD:
				tblUpload.Visible = true;
				btnUpload.Visible = true;
				tblUploadConfirm.Visible = false;
				// マスタアップロードディレクトリ作成
				if (Directory.Exists(this.MasterUploadDirectory) == false) Directory.CreateDirectory(this.MasterUploadDirectory);
				break;

			case Constants.ACTION_STATUS_UPLOAD_CONFIRM:
				tblUploadConfirm.Visible = true;
				btnRegisterTop.Visible = true;
				btnDeleteTop.Visible = true;
				break;

			case Constants.ACTION_STATUS_COMPLETE:
				tblComplete.Visible = true;
				tblUpload.Visible = false;
				tblUploadConfirm.Visible = false;
				tblTargetListName.Visible = false;
				divActionTop.Visible = false;
				divActionBottom.Visible = false;
				divActionBottomComplete.Visible = true;
				tblExplain.Visible = false;
				break;
		}
	}

	/// <summary>
	/// アップロード情報表示
	/// </summary>
	private void DisplayUploadInfo()
	{
		// ファイルが存在しない場合はアップロードページへリダイレクト
		var existedFiles = Directory.GetFiles(this.MasterUploadDirectory);
		if (existedFiles.Length == 0) RedirectToTargetListUploadPage();

		// 各種表示設定
		var dataCount = GetCsvFileDataLineCount(existedFiles[0]);
		lblFileUploadName.Text = WebSanitizer.HtmlEncode(Path.GetFileName(existedFiles[0]));
		hfFileUploadName.Value = Path.GetFileName(existedFiles[0]);
		lblFileDataCount.Text = dataCount.ToString();
		tbTargetListName.Text = (Session[Constants.SESSION_KEY_PARAM] is Hashtable) ? StringUtility.ToEmpty(((Hashtable)Session[Constants.SESSION_KEY_PARAM])[Constants.FIELD_TARGETLIST_TARGET_NAME]) : string.Empty;
	}

	/// <summary>
	/// アップロードボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpload_Click(object sender, EventArgs e)
	{
		// エラーチェック
		ValidateFile();

		// ファイルアップロード
		FileUpload();

		// 確認画面ヘリダイレクト
		var inputParams = new Hashtable { { Constants.FIELD_TARGETLIST_TARGET_NAME, tbTargetListName.Text } };
		Session[Constants.SESSION_KEY_PARAM] = inputParams;
		RedirectToTargetListUploadConfirmPage();
	}

	/// <summary>
	/// ファイルチェック
	/// </summary>
	private void ValidateFile()
	{
		// ファイル指定あり？
		if (fUpload.Value == "")
		{
			// 「ファイルを選択してください」エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNSELECTED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// CSVファイルじゃなければエラー
		if (fUpload.PostedFile.FileName.ToLower().EndsWith(".csv") == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// ファイルアップロード
	/// </summary>
	private void FileUpload()
	{
		// ファイルアップロード処理
		var filePath = Path.Combine(this.MasterUploadDirectory, Path.GetFileName(fUpload.PostedFile.FileName));
		// ファイルが既に存在していたらエラーページへ
		if (Directory.GetFiles(this.MasterUploadDirectory).Length > 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_ALREADY_EXISTS);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		// ファイル存在チェック
		if (fUpload.PostedFile.InputStream.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNFIND);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// ファイルアップロード実行
		fUpload.PostedFile.SaveAs(filePath);
	}

	/// <summary>
	/// ファイルのデータ行数を取得
	/// </summary>
	/// <param name="filePath">ファイルパス</param>
	/// <returns>データ行数</returns>
	protected int GetCsvFileDataLineCount(string filePath)
	{
		var count = 0;
		using (var reader = new StreamReader(filePath))
		{
			while (reader.ReadLine() != null)
			{
				count++;
			}
		}
		return count - 1;
	}

	/// <summary>
	/// アップロードファイルのヘッダーチェック
	/// </summary>
	/// <param name="filePath">ファイルパス</param>
	protected void CheckCsvFileHeader(string filePath)
	{
		using (var fileReader = new StreamReader(filePath))
		{
			var headers = StringUtility.SplitCsvLine(fileReader.ReadLine() ?? string.Empty);
			if (headers[0].Trim() == Constants.FIELD_USER_USER_ID) return;
			// 先頭列がuser_idでなければエラー
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_FIELDS_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// ターゲットリストアップロード画面へ遷移
	/// </summary>
	protected void RedirectToTargetListUploadPage()
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_UPLOAD);
		url.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPLOAD);
		Response.Redirect(url.ToString());
	}

	/// <summary>
	/// ターゲットリストアップロード確認画面へ遷移
	/// </summary>
	protected void RedirectToTargetListUploadConfirmPage()
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_UPLOAD);
		url.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPLOAD_CONFIRM);
		Response.Redirect(url.ToString());
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteTop_Click(object sender, EventArgs e)
	{
		var uploadFilePath = Path.Combine(this.MasterUploadDirectory, hfFileUploadName.Value);
		if (File.Exists(uploadFilePath)) File.Delete(uploadFilePath);
		RedirectToTargetListUploadPage();
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterTop_Click(object sender, EventArgs e)
	{
		var fileName = hfFileUploadName.Value;
		var uploadFilePath = Path.Combine(this.MasterUploadDirectory, fileName);

		// データの入力チェックします。
		CheckInputData();

		// アップロードファイルのヘッダーチェック
		CheckCsvFileHeader(uploadFilePath);

		// アクティブディレクトリへファイル移動
		var activeDirectoryPath = Path.Combine(
			Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR,
			this.LoginOperatorShopId,
			Constants.TABLE_TARGETLISTDATA,
			Constants.DIRNAME_MASTERIMPORT_ACTIVE);
		if (Directory.Exists(activeDirectoryPath) == false) Directory.CreateDirectory(activeDirectoryPath);

		var activeFilePath = Path.Combine(activeDirectoryPath, Path.GetFileNameWithoutExtension(uploadFilePath) + ".csv");
		if (File.Exists(activeFilePath) == false)
		{
			File.Move(uploadFilePath, activeFilePath);
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages
				.GetMessages(WebMessages.ERRMSG_MANAGER_CSV_UPLOADED)
				.Replace("@@ 1 @@", Path.GetFileNameWithoutExtension(uploadFilePath));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		ExecuteBatch("\"" + activeFilePath + "\" " + this.LoginOperatorShopId + " " + this.LoginOperatorDeptId + " \"" + this.LoginOperatorName + "\" \"" + tbTargetListName.Text + "\"");

		// 取込実行完了画面へ遷移
		RedirectToConmpletePage();
	}

	/// <summary>
	/// アップロード ページに行く
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackToUpload_Click(object sender, EventArgs e)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_UPLOAD);
		url.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPLOAD);
		Response.Redirect(url.ToString());
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackBottom_Click(object sender, EventArgs e)
	{
		var uploadFilePath = Path.Combine(this.MasterUploadDirectory, hfFileUploadName.Value);
		if (File.Exists(uploadFilePath)) File.Delete(uploadFilePath);
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_UPLOAD:
				Response.Redirect(Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST);
				break;
			case Constants.ACTION_STATUS_UPLOAD_CONFIRM:
				RedirectToTargetListUploadPage();
				break;
		}
	}

	/// <summary>
	/// 取込実行完了画面へ遷移
	/// </summary>
	private void RedirectToConmpletePage()
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_UPLOAD);
		url.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_COMPLETE);
		Response.Redirect(url.ToString());
	}

	/// <summary>
	/// データの入力チェックします。
	/// </summary>
	private void CheckInputData()
	{
		// // エラーチェック
		var errorMessage = new StringBuilder();
		errorMessage.Append(Validator.CheckNecessaryError(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_TARGET_LIST_NAME),
			tbTargetListName.Text));
		errorMessage.Append(Validator.CheckByteLengthMaxError(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_TARGET_LIST_NAME),
			tbTargetListName.Text, 60));

		// エラーがあれば画面遷移
		if (errorMessage.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// バッチを実行
	/// </summary>
	/// <param name="args">パラメーター</param>
	/// <returns>成功または失敗</returns>
	public void ExecuteBatch(string args)
	{
		string batchFilePath = Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE;
		if (File.Exists(batchFilePath) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTER_FILE_NOT_FOUND);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		var exeProcess = new Process { StartInfo = { FileName = batchFilePath, Arguments = args } };
		exeProcess.Start();
	}

	///<summary>マスタアップロードディレクトリ</summary>
	private string MasterUploadDirectory { get { return Path.Combine(Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR, this.LoginOperatorShopId, Constants.TABLE_TARGETLISTDATA, Constants.DIRNAME_MASTERIMPORT_UPLOAD); } }
	///<summary>処理ファイルディレクトリ作成</summary>
	private string ActiveDirectory { get { return Path.Combine(Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR, this.LoginOperatorShopId, Constants.TABLE_TARGETLISTDATA, Constants.DIRNAME_MASTERIMPORT_ACTIVE); } }

}