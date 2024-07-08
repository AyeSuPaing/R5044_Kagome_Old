/*
=========================================================================================================
  Module      : 商品タグ設定ページ処理(ProductTagSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

public partial class Form_ProductTagSetting_ProductTagSettingList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 初期表示
		if (!IsPostBack)
		{
			List<Hashtable> tagSettingList = new List<Hashtable>();
			if (Session[SESSIONPARAM_KEY_PRODUCTTAGSETTING_INFO] == null)
			{
				//------------------------------------------------------
				// 商品タグ設定の情報取得、データ作成
				//------------------------------------------------------
				foreach (DataRowView drv in GetProductTagSetting())
				{
					Hashtable tagSetting = new Hashtable();
					tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NO, StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO]));
					tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_ID, (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
					tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG, (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG]);
					tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME, (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME]);
					tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG + TAG_OLD, (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG]);
					tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME + TAG_OLD, (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME]);
					tagSettingList.Add(tagSetting);
				}
			}
			else
			{
				tagSettingList = (List<Hashtable>)Session[SESSIONPARAM_KEY_PRODUCTTAGSETTING_INFO];
			}
			this.ProductTagSettingCount = tagSettingList.Count;

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind(tagSettingList);
		}
	}

	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		List<Hashtable> tagSettingList = GetInputProductTag();
		string errorMessages = "";

		//------------------------------------------------------
		// 入力チェック & エラーページ遷移
		//------------------------------------------------------
		foreach (Hashtable tagSetting in tagSettingList)
		{
			// 新規登録のタグのみチェック
			if ((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] != "")
			{	
				errorMessages += Validator.Validate("ProductTagSetting", tagSetting).Replace("@@ 1 @@", (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
			}
		}
		if (errorMessages.Length != 0) RedirectErrorPage(errorMessages);

		//------------------------------------------------------
		// 重複チェック & エラーページ遷移
		//------------------------------------------------------
		List<string> lTagList = new List<string>();
		foreach (Hashtable tagSetting in tagSettingList)
		{
			if ((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] != "")
			{
				string tagId = GetTagId(tagSetting);
				if (lTagList.Contains(tagId))
				{
					errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTTAG_SETTING_TAG_ID_DUPLICATED)
						.Replace("@@ 1 @@", tagId);
				}
				else
				{
					lTagList.Add(tagId);
				}
			}
		}
		if (errorMessages.Length != 0) RedirectErrorPage(errorMessages);

		//------------------------------------------------------
		// アクション判定と商品タグ設定マスタの追加・削除・編集
		//------------------------------------------------------
		bool blSuccess = ExecutionSqlStatement();
		if (blSuccess == false) RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));

		//------------------------------------------------------
		// 正常遷移
		//------------------------------------------------------
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTTAGSETTING_LIST);
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		// データ作成
		List<Hashtable> tagSettingList = GetInputProductTag();
		Hashtable tagSetting = new Hashtable();
		tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NO, "");
		tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_ID, "");
		tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG, Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_VALID);
		tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME, "");
		tagSetting.Add(FIELD_DELETE_VALIF_FLG, "");
		tagSettingList.Add(tagSetting);
		this.ProductTagSettingCount += 1;

		// コンポーネント初期化
		InitializeComponents();

		// データバインド
		DataBind(tagSettingList);
	}

	/// <summary>
	/// キャンセルボタンクリック ※DB未登録情報をHashtableから削除
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancel_Click(object sender, EventArgs e)
	{
		// 該当情報の削除
		List<Hashtable> tagSettingList = GetInputProductTag();
		int iIndex = 0;
		if (int.TryParse(StringUtility.ToEmpty((((LinkButton)sender).CommandArgument)), out iIndex))
		{
			tagSettingList.Remove(tagSettingList[iIndex]);
			this.ProductTagSettingCount -= 1;
		}

		// コンポーネント初期化
		InitializeComponents();

		// データバインド
		DataBind(tagSettingList);
	}

	/// <summary>
	/// アクション判定と商品タグ設定マスタの追加・削除・編集
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private bool ExecutionSqlStatement()
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();
			string transaction = "";
			try
			{
				string strAction = "";
				List<string> addTagSetting = new List<string>();
				List<string> deleteTagSetting = new List<string>();
				List<string> resultTagSetting = new List<string>();
				foreach (Hashtable tagSetting in GetInputProductTag())
				{
					string strStatement = null;
					//------------------------------------------------------
					// アクション判定
					//------------------------------------------------------
					// マスタ再定義対象かどうか(タグNOが空ではなく かつ 削除フラグがOFFのとき)
					if (((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] != "") && ((string)tagSetting[FIELD_DELETE_VALIF_FLG] == FLG_DELETE_INVALID))
					{
						resultTagSetting.Add((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
					}
					// 新規タグかどうか(タグNOが空 かつ タグIDに値があるとき)
					if (((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] == "") && ((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] != ""))
					{
						strStatement = "Insert";
						strAction = Constants.ACTION_STATUS_INSERT;

						addTagSetting.Add(TAG_ID_INITIALS + (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
					}
					// 削除対象のタグかどうか(タグNoが空ではなく かつ 削除フラグがONのとき)
					else if (((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] != "") && ((string)tagSetting[FIELD_DELETE_VALIF_FLG] == FLG_DELETE_VALID))
					{
						strStatement = "Delete";
						strAction = Constants.ACTION_STATUS_DELETE;

						deleteTagSetting.Add((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
					}
					// 更新対象のタグかどうか(タグ名称、有効フラグが変更されているとき)
					else if (((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME] != ((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME + "_old"])
						|| ((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG] != ((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG + "_old"]))))
					{
						strStatement = "Update";
						strAction = Constants.ACTION_STATUS_UPDATE;
					}
					else
					{
						continue;
					}

					// INSERTなら、タグIDに頭文字を付ける
					tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] = (strAction == Constants.ACTION_STATUS_INSERT) ? (TAG_ID_INITIALS + (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]) : (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID];
					tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_LAST_CHANGED, this.LoginOperatorName);

					//------------------------------------------------------
					// SQL発行(w2_ProductTagSettingに対して)
					//------------------------------------------------------
					transaction = "1.商品タグ設定マスタへのINSERT/DELETE/UPDATE";
					ExecuteStatement(sqlAccessor, strStatement, tagSetting);
				}

				//------------------------------------------------------
				// 追加・削除ならw2_productTagの再定義 ※削除->追加の順で行う
				//------------------------------------------------------
				if ((deleteTagSetting.Count != 0) || (addTagSetting.Count != 0))
				{
					
					transaction = "2-1.商品タグマスタの再定義 コピーテーブル作成";
					ExecuteStatement(sqlAccessor, "TableCopy");
					
					foreach (string delTag in deleteTagSetting)
					{
						transaction = "2-2.商品タグマスタの再定義 (コピーテーブルに対して)列の削除";
						ExecuteStatement(sqlAccessor, "RemoveColumn", delTag);
					}

					foreach (string addTag in addTagSetting)
					{
						transaction = "2-3.商品タグマスタの再定義 (コピーテーブルに対して)列の追加";
						ExecuteStatement(sqlAccessor, "AddColumn", addTag);
					}

					foreach (string resultTag in resultTagSetting)
					{
						transaction = "2-4.商品タグマスタの再定義 (可変の)既存カラムの制約を再定義";
						ExecuteStatement(sqlAccessor, "ReDefinitionColumn", resultTag);
					}

					transaction = "2-5.商品タグマスタの再定義 元テーブルのDROP、固定カラムに制約を追加、コピーテーブルを元テーブルにリネーム";
					ExecuteStatement(sqlAccessor, "ReDefinition");
				}

				// トランザクションコミット
				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				sqlAccessor.RollbackTransaction();
				AppLogger.Write(AppLogger.LOGKBN_ERROR, (transaction + ex));
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// ステートメント実行
	/// </summary>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <param name="statementName">ステートメント名</param>
	private static void ExecuteStatement(SqlAccessor sqlAccessor, string statementName)
	{
		using (SqlStatement sqlStatement = new SqlStatement("ProductTagSetting", statementName))
		{
			sqlStatement.ExecStatement(sqlAccessor);
		}
	}

	/// <summary>
	/// ステートメント実行
	/// </summary>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <param name="statementName">ステートメント名</param>
	/// <param name="inputParam">タグ設定情報</param>
	private static void ExecuteStatement(SqlAccessor sqlAccessor, string statementName, Hashtable inputParam)
	{
		using (SqlStatement sqlStatement = new SqlStatement("ProductTagSetting", statementName))
		{
			sqlStatement.ExecStatement(sqlAccessor, inputParam);
		}
	}

	/// <summary>
	/// ステートメント実行
	/// </summary>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <param name="statementName">ステートメント名</param>
	/// <param name="Tag">タグID</param>
	private static void ExecuteStatement(SqlAccessor sqlAccessor, string statementName, string Tag)
	{
		using (SqlStatement sqlStatement = new SqlStatement("ProductTagSetting", statementName))
		{
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ field @@", Tag);
			sqlStatement.ExecStatement(sqlAccessor);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 追加可能な最大件数未満なら表示
		btnAddTop.Visible = btnAddBottom.Visible = (this.ProductTagSettingCount < Constants.PRODUCTTAGSETTING_MAXCOUNT);

		// 設定情報が1件以上なら表示
		btnAllUpdateTop.Visible = btnAllUpdateBottom.Visible = (this.ProductTagSettingCount != 0);

		// 登録/更新完了メッセージ表示制御
		dvTagComplete.Visible = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ACTION_STATUS]) == Constants.ACTION_STATUS_COMPLETE;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = null;

		Session[SESSIONPARAM_KEY_PRODUCTTAGSETTING_INFO] = null;
	}

	/// <summary>
	/// データバインド
	/// </summary>
	/// <param name="tagSettingList">商品タグ設定情報のリスト</param>
	private void DataBind(List<Hashtable> tagSettingList)
	{
		rTagList.DataSource = tagSettingList;
		rTagList.DataBind();
	}

	/// <summary>
	/// 画面からデータ取得
	/// </summary>
	/// <returns>商品タグ設定情報</returns>
	protected List<Hashtable> GetInputProductTag()
	{
		List<Hashtable> tagSettingList = new List<Hashtable>();
		foreach (RepeaterItem ri in rTagList.Items)
		{
			Hashtable tagSetting = new Hashtable();
			tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NO, ((HiddenField)ri.FindControl("hdnTagNo")).Value.Trim());
			tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_ID, (((HiddenField)ri.FindControl("hdnTagNo")).Value != "") ? ((HiddenField)ri.FindControl("hdnTagId")).Value.ToLower() : ((TextBox)ri.FindControl("tbTagId")).Text.Trim().ToLower());
			tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG, (((CheckBox)ri.FindControl("cbValidFlg")).Checked) ? Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_VALID : Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_INVALID);
			tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG + TAG_OLD, (((HiddenField)ri.FindControl("hdnValidFlg_old")).Value));
			tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME, ((TextBox)ri.FindControl("tbTagName")).Text);
			tagSetting.Add(Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME + TAG_OLD, (((HiddenField)ri.FindControl("hdnTagName_old")).Value));
			tagSetting.Add(FIELD_DELETE_VALIF_FLG, (((CheckBox)ri.FindControl("cbDeleteFlg")).Checked) ? FLG_DELETE_VALID : FLG_DELETE_INVALID);

			tagSettingList.Add(tagSetting);
		}
		return tagSettingList;
	}

	/// <summary>
	/// タグIDを取得
	/// </summary>
	/// <param name="tagSetting">タグ設定</param>
	/// <return>タグID（新規登録の場合は頭文字付与状態で返す）</return>
	private string GetTagId(Hashtable tagSetting)
	{
		return ((string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] != "")
			? (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]
			: TAG_ID_INITIALS + (string)tagSetting[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID];
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void RedirectErrorPage(string errorMessage)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
		Session[SESSIONPARAM_KEY_PRODUCTTAGSETTING_INFO] = GetInputProductTag();
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>商品タグ設定の件数</summary>
	protected int ProductTagSettingCount
	{
		get { return (int)ViewState["ProductTagSettingCount"]; }
		private set { ViewState["ProductTagSettingCount"] = value; }
	}
	/// <summary>削除フラグ：フィールド名</summary>
	protected const string FIELD_DELETE_VALIF_FLG = "del_flg";
	/// <summary>削除フラグ：有効</summary>
	protected const string FLG_DELETE_VALID = "1";
	/// <summary>削除フラグ：無効</summary>
	protected const string FLG_DELETE_INVALID = "0";
	/// <summary>タグIDの頭文字</summary>
	protected const string TAG_ID_INITIALS = "tag_";
	protected const string TAG_OLD = "_old";
	/// <summary>セッションキー：商品タグ設定情報</summary>
	protected const string SESSIONPARAM_KEY_PRODUCTTAGSETTING_INFO = "producttagsetting_info";
}