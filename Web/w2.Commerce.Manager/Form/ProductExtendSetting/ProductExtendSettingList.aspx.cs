/*
=========================================================================================================
  Module      : 商品拡張項目設定ページ処理(ProductExtendSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Form_ProductExtendSetting_ProductExtendSettingList : BasePage
{
	// 定数
	protected const int PRODUCT_EXTEND_COUNT = 140;	// 商品拡張項目数

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
			//------------------------------------------------------
			// 登録/更新完了メッセージ表示制御
			//------------------------------------------------------
			trExtendComplete.Visible = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_PARAM]) == Constants.ACTION_STATUS_COMPLETE;
			Session[Constants.SESSION_KEY_PARAM] = null;

			//------------------------------------------------------
			// 商品拡張項目情報データ取得
			//------------------------------------------------------
			DataView dvExtend = GetProductExtendSetting(this.LoginOperatorShopId);

			dvExtend.Sort = Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO;	// ソートキーとして拡張項目番号設定

			//------------------------------------------------------
			// リピータデータバインド用データ作成
			//------------------------------------------------------
			// Reperterループ用（後のforループで PRODUCT_EXTEND_COUNT 個つくられる）
			List<Hashtable> lExtendSettings = new List<Hashtable>();

			for (int iLoop = 1; iLoop <= PRODUCT_EXTEND_COUNT; iLoop++)
			{
				Hashtable htExtendSetting = new Hashtable();

				DataRowView[] drvs = dvExtend.FindRows(iLoop);
				if (drvs.Length != 0)
				{
					htExtendSetting.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME, (string)drvs[0][Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME]);
					htExtendSetting.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME + "_Escaped", ((string)drvs[0][Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME]).Replace(@"\", @"\\").Replace("\"", "\\\""));
					htExtendSetting.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION, (string)drvs[0][Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION]);
				}
				else
				{
					htExtendSetting.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME, "");
					htExtendSetting.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION, "");
				}

				lExtendSettings.Add(htExtendSetting);
			}

			//------------------------------------------------------
			// リピータデータバインド
			//------------------------------------------------------
			rExtendList.DataSource = lExtendSettings;
			rExtendList.DataBind();
		}

	}

	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		string strErrorMessages = "";
		Hashtable htIniputForValidate = new Hashtable();
		foreach (RepeaterItem ri in rExtendList.Items)
		{
			if (((TextBox)ri.FindControl("tbExtendName")).Text.Length != 0)
			{
				htIniputForValidate[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME] = ((TextBox)ri.FindControl("tbExtendName")).Text;

				string strErrorMessage = Validator.Validate("ProductExtendSetting", htIniputForValidate);

				if (strErrorMessage.Length != 0)
				{
					strErrorMessages += strErrorMessage.Replace("@@ 1 @@", ((HiddenField)ri.FindControl("hdnExtendNo")).Value);
				}
			}
		}
		if (strErrorMessages.Length != 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// DB登録
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				// リピータアイテムに対して繰り返し
				foreach (RepeaterItem ri in rExtendList.Items)
				{
					string strStatementName = null;

					//------------------------------------------------------
					// 発行SQL決定
					//------------------------------------------------------
					// INSERT
					if ((((TextBox)ri.FindControl("tbExtendName")).Text != "") && (((HiddenField)ri.FindControl("hdnExtendNameBefore")).Value == ""))
					{
						strStatementName = "InsertProductExtendSetting";
					}
					// DELETE
					else if ((((TextBox)ri.FindControl("tbExtendName")).Text == "") && (((HiddenField)ri.FindControl("hdnExtendNameBefore")).Value != ""))
					{
						strStatementName = "DeleteProductExtendSetting";
					}
					// UPDATE
					else if ((((TextBox)ri.FindControl("tbExtendName")).Text != ((HiddenField)ri.FindControl("hdnExtendNameBefore")).Value)
				   || (((TextBox)ri.FindControl("tbExtendDiscription")).Text != ((HiddenField)ri.FindControl("hdnExtendDiscriptionBefore")).Value))
					{
						strStatementName = "UpdateProductExtendSetting";
					}
					else
					{
						continue;
					}

					//------------------------------------------------------
					// SQL発行（拡張項目更新）
					//------------------------------------------------------
					using (SqlStatement sqlStatement = new SqlStatement("ProductExtendSetting", strStatementName))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_PRODUCTEXTENDSETTING_SHOP_ID, this.LoginOperatorShopId);
						htInput.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO, ((HiddenField)ri.FindControl("hdnExtendNo")).Value);
						htInput.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME, ((TextBox)ri.FindControl("tbExtendName")).Text);
						htInput.Add(Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION, ((TextBox)ri.FindControl("tbExtendDiscription")).Text);
						htInput.Add(Constants.FIELD_PRODUCTEXTENDSETTING_LAST_CHANGED, this.LoginOperatorName);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}

					//------------------------------------------------------
					// SQL発行（削除であれば値クリア）
					//------------------------------------------------------
					if (strStatementName == "DeleteProductExtendSetting")
					{
						using (SqlStatement sqlStatement = new SqlStatement("ProductExtendSetting", "ClearProductExtend"))
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_PRODUCTEXTENDSETTING_SHOP_ID, this.LoginOperatorShopId);

							sqlStatement.Statement = sqlStatement.Statement.Replace("@@ field @@", "extend" + ((HiddenField)ri.FindControl("hdnExtendNo")).Value);

							sqlStatement.ExecStatement(sqlAccessor, htInput);
						}
					}
				}

				// トランザクションコミット
				sqlAccessor.CommitTransaction();
			}
			catch
			{
				// トランザクションロールバック
				sqlAccessor.RollbackTransaction();

				throw;
			}

		}

		//------------------------------------------------------
		// 登録/更新完了メッセージ表示制御
		//------------------------------------------------------
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;

		//------------------------------------------------------
		// リダイレクト
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTEXTENDSETTING_LIST);
	}

	/// <summary>拡張項目リスト番号保持用</summary>
	protected int ExtendListNoTmp { get; set; }
}
