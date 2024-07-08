/*
 =========================================================================================================
  Module      : レコメンド設定ボタン画像アップロードページ処理(RecommendButtonImageFileUpload.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Recommend;

public partial class Form_Recommend_RecommendButtonImageFileUpload : RecommendPage
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
			// 表示コンポーネント初期化
			InitializeComponents();

			// ボタン画像ディレクトリ作成＆古い（作成日が1日以上経った）一時ボタン画像ファイルを削除
			var buttonImageOperator = new RecommendButtonImageOperator();
			buttonImageOperator.CreateRecommendButtonImageDirectory();
			buttonImageOperator.DeleteOldTempRecommendButtonImageFile();
		}

		// 完了メッセージ非表示
		divComp.Visible = false;

		// 親ウィンドウをポストバックさせるためフラグをリセット
		hfWindowOpenerPostBack.Value = string.Empty;
	}

	/// <summary>
	/// ファイルアップロードボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnButtonImageFileUpload_Click(object sender, System.EventArgs e)
	{
		// エラーメッセージ非表示
		trRecommendErrorMessagesTitle.Visible =
			trRecommendErrorMessages.Visible = false;
		lbRecommendErrorMessages.Text = string.Empty;

		// ファイル指定チェック
		var isSuccess = true;
		if (isSuccess && (fuButtonImage.HasFile == false))
		{
			lbRecommendErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_BOTTONIMAGE_FILE_UNSELECTED);
			isSuccess = false;
		}
		// ファイル存在チェック
		if (isSuccess && (fuButtonImage.PostedFile.InputStream.Length == 0))
		{
			lbRecommendErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_BOTTONIMAGE_FILE_UNFIND);
			isSuccess = false;
		}
		if (isSuccess == false)
		{
			// エラーメッセージ表示
			trRecommendErrorMessagesTitle.Visible =
				trRecommendErrorMessages.Visible = true;
			return;
		}

		// 一時保存中のボタン画像ファイル削除
		ButtonImageType buttonImageType;
		Enum.TryParse(this.RequestButtonImageType, out buttonImageType);
		var buttonImageOperator = new RecommendButtonImageOperator(buttonImageType);
		buttonImageOperator.DeleteTempRecommendButtonImageFile(this.RequestTempRecommendId);

		// ボタン画像ファイル名＆ファイル拡張子取得
		var fileName = buttonImageOperator.GetRecommendButtonImageFileNameWithoutExtension(this.RequestTempRecommendId);
		var fileExtension = Path.GetExtension(fuButtonImage.FileName);

		// ボタン画像ファイル保存
		var filePath =
			buttonImageOperator.GetRecommendButtonImagePhysicalFilePath(this.RequestTempRecommendId, fileExtension, true);
		fuButtonImage.PostedFile.SaveAs(filePath);

		// 完了メッセージ表示
		divComp.Visible = true;

		// 親ウィンドウをポストバックさせるためフラグセット
		hfWindowOpenerPostBack.Value = "1";
	}

	#region メソッド
	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// ボタン画像名表示
		lButtonName.Text = ValueText.GetValueText(Constants.TABLE_RECOMMEND, "button_image_type", this.RequestButtonImageType);
	}
	#endregion

	#region プロパティ
	/// <summary>リクエスト：レコメンドID（※ボタン画像保存用）</summary>
	protected string RequestTempRecommendId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TEMP_RECOMMEND_ID]).Trim(); }
	}
	/// <summary>リクエスト：ボタン画像種別</summary>
	protected string RequestButtonImageType
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RECOMMEND_BUTTONIMAGE_TYPE]).Trim(); }
	}
	#endregion
}