/*
=========================================================================================================
  Module      : パスワード変更入力ページ処理(OperatorPasswordChangeInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Form_OperatorPasswordChange_OperatorPasswordChangeInput : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnChange_Click(object sender, System.EventArgs e)
	{

		// 変数宣言
		Hashtable htParam = new Hashtable();

		htParam.Add(Constants.FIELD_SHOPOPERATOR_PASSWORD + "_old", tbPassowrdOld.Text);		// 現在のパスワード
		htParam.Add(Constants.FIELD_SHOPOPERATOR_PASSWORD, tbPassowrd.Text);					// 新しいパスワード
		htParam.Add(Constants.FIELD_SHOPOPERATOR_PASSWORD + "_conf", tbPassowrdConfirm.Text);	// 新しいパスワード(確認)

		// 整合性チェック
		string strErrorMessages = Validator.Validate("ShopOperatorPasswordChange", htParam);
		if (strErrorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 旧パスワードチェック
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopOperator", "GetOperatorPassWord"))
		{
			Hashtable htInput = new Hashtable();

			htInput.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, this.LoginOperatorId);	// オペレータID
			htInput.Add(Constants.FIELD_SHOPOPERATOR_PASSWORD,tbPassowrdOld.Text);			// 旧パスワード
			htInput.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);	// 店舗ID

			// SQL発行
			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
			DataView dv = ds.Tables["Table"].DefaultView;

			// オペレータが存在しない場合
			if (dv.Count == 0)
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = 
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOP_OPERATOR_NO_OPERATOR_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		// 更新
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopOperator", "ChangeOperatorPassword"))
		{
			Hashtable htInput = new Hashtable();

			// オペレータID
			htInput.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, this.LoginOperatorId);
			// 旧パスワード
			htInput.Add(Constants.FIELD_SHOPOPERATOR_PASSWORD, tbPassowrd.Text);
			// 店舗ID
			htInput.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);
			// 最終更新者
			htInput.Add(Constants.FIELD_SHOPOPERATOR_LAST_CHANGED, this.LoginOperatorName);

			// 更新
			int iUpdated = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);

			// 完了ページへ
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_PASSWORD_CHANGE_COMPLETE);
		}
	}
}

