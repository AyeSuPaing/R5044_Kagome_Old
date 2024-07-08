/*
=========================================================================================================
  Module      : ダウンロード待機ページ処理(DownloadWait.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Form_Download_DownloadWait : BasePage
{
	//=========================================================================================
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	//=========================================================================================
	protected void Page_Load(object sender, EventArgs e)
	{
		// 呼び出し元でファイルを削除するタイムラグを考慮して初回はファイル非表示とする
		if (ViewState["popupWait"] == null)
		{
			ViewState["popupWait"] = 1;
			ViewState["isCreating"] = true;
			var url = Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.PATH_CONTENTS + "Invoice/";
			var pdfFiles = Directory.GetFiles(url, "*.pdf");
			foreach (var pdfFile in pdfFiles)
			{
				File.Delete(pdfFile);
			}
		}
		else if ((int)ViewState["popupWait"] == 1)
		{
			RefreshList();
		}
	}

	/// <summary>
	/// 一覧更新
	/// </summary>
	private void RefreshList()
	{
		List<Hashtable> lFiles = new List<Hashtable>();

		//------------------------------------------------------
		// 作成完了のファイル取得
		//------------------------------------------------------
		var strTargetUrl = StringUtility.ToEmpty(Request["targaturl"]);
		var strTargetDirPath = Server.MapPath(strTargetUrl);
		foreach (var strFilePath in Directory.GetFiles(strTargetDirPath))
		{
			var htFileInfo = new Hashtable
			{
				{ "creationtime", File.GetCreationTime(strFilePath) },
				{ "filename", Path.GetFileName(strFilePath) },
				{ "filepath", strFilePath },
				{ "fileurl", strTargetUrl + Path.GetFileName(strFilePath) },
				{ "complete", true }
			};

			lFiles.Add(htFileInfo);
		}

		// 作成中のファイル取得
		var strTempDirPath = strTargetDirPath + @"\Tmp";
		var strSessionId = StringUtility.ToEmpty(Request["sid"]);
		var creatingFiles = Directory.GetFiles(strTempDirPath, "*." + strSessionId);

		trNoList.Visible = (lFiles.Count == 0);
		rList.DataSource = lFiles;
		rList.DataBind();

		if (lFiles.Count != 0)
		{
			ViewState["isCreating"] = creatingFiles.Any();
		}
	}
}
