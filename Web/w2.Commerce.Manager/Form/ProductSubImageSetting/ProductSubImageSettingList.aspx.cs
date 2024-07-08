/*
=========================================================================================================
  Module      : 商品サブ画像設定ページ(ProductSubImageSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････


vld
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;

public partial class Form_ProductSubImageSetting_ProductSubImageSettingList : BasePage
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
			//------------------------------------------------------
			// 商品サブ画像設定一覧取得
			//------------------------------------------------------
			DataView dvExtend = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductSubImageSetting", "GetProductSubImageSettingList"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID, this.LoginOperatorShopId);
				htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO, Constants.PRODUCTSUBIMAGE_MAXCOUNT);
				dvExtend = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			// 商品サブ画像設定番号ソート用
			dvExtend.Sort = Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO;

			//------------------------------------------------------
			// データ作成
			//------------------------------------------------------
			List<string> lExtendSettings = new List<string>();
			for (int iLoop = 1; iLoop <= Constants.PRODUCTSUBIMAGE_MAXCOUNT; iLoop++)
			{
				DataRowView[] drvs = dvExtend.FindRows(iLoop);
				Hashtable htExtendSetting = new Hashtable();
				string strExtendSetting;

				if (drvs.Length != 0)
				{
					// データベースより取得
					strExtendSetting = (string)drvs[0][Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME];
				}
				else
				{
					// 空で作成
					strExtendSetting = "";
				}

				lExtendSettings.Add(strExtendSetting);
			}

			// データバインド
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
		StringBuilder sbErrorMessages = new StringBuilder();

		// 商品サブ画像設定の数だけ
		foreach (RepeaterItem ri in rExtendList.Items)
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME, ((TextBox)ri.FindControl("tbSubImageName")).Text);

			string strErr = Validator.Validate("ProductSubImageSetting", htInput);
			string strNo = (ri.ItemIndex + 1).ToString();

			// メッセージ追加
			sbErrorMessages.Append(strErr.Replace("@@ 1 @@", strNo));
		}

		// エラーがあったら
		if (sbErrorMessages.Length != 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// SQL開始
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// トランザクション開始
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// 商品サブ画像設定全削除
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("ProductSubImageSetting", "DeleteProductSubImageSettingList"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID, this.LoginOperatorShopId);

					sqlStatement.ExecStatement(sqlAccessor, htInput);
				}

				//------------------------------------------------------
				// 商品サブ画像設定追加
				//------------------------------------------------------
				foreach (RepeaterItem ri in rExtendList.Items)
				{
					string strName = ((TextBox)ri.FindControl("tbSubImageName")).Text;
					int iIndex = ri.ItemIndex + 1;

					// 名前があったら
					if (strName.Length != 0)
					{
						using (SqlStatement sqlStatement = new SqlStatement("ProductSubImageSetting", "InsertProductSubImageSettingList"))
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID, this.LoginOperatorShopId);
							htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO, iIndex.ToString());
							htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME, strName);
							htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_LAST_CHANGED, this.LoginOperatorName);

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
	}
}
