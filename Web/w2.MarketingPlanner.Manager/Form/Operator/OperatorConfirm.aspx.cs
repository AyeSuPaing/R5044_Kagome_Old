/*
=========================================================================================================
  Module      : オペレータ情報確認ページ処理(OperatorConfirm.aspx.cs)
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
using w2.App.Common.Cs.CsOperator;
using w2.Common.Util;

public partial class Form_Operator_OperatorConfirm : BasePage
{
	protected Hashtable m_htOperator = new Hashtable();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			string strOperatorId = Request[Constants.REQUEST_KEY_OPERATOR_ID];
			ViewState.Add(Constants.REQUEST_KEY_OPERATOR_ID, strOperatorId);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・画面確認？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_UPDATE
				)
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				m_htOperator = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];
			}
			// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				DataRow drOperator = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatements = new SqlStatement("ShopOperator", "GetOperator"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);
					htInput.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, strOperatorId);

					DataSet dsOperator = sqlStatements.SelectStatementWithOC(sqlAccessor, htInput);

					// 該当データが有りの場合
					if (dsOperator.Tables["Table"].DefaultView.Count != 0)
					{
						drOperator = dsOperator.Tables["Table"].Rows[0];
					}
					// 該当データ無しの場合
					else
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);

					}
				}

				// Hashtabe格納
				foreach (DataColumn dcValue in drOperator.Table.Columns)
				{
					m_htOperator.Add(dcValue.ColumnName, drOperator[dcValue.ColumnName]);
				}
				// 「スーパーユーザー」名設定
				if (m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG].ToString() == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER)
				{
					m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG + "_name"] = Constants.STRING_SUPERUSER_NAME;
				}
				// 「権限なしユーザ」名設定
				if (m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG].ToString() == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER)
				{
					m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG + "_name"] = Constants.STRING_UNACCESSABLEUSER_NAME;
				}
			}
			// 該当なし？
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}

			// オペレータ情報にパスワード情報も含まれているため
			// ViewStateではなく、Sessionに格納しておく
			Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = m_htOperator;

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 詳細表示？
		if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEdit.Visible = true;
			btnEdit2.Visible = true;

			// 作成日・更新日・最終更新者表示
			trOperatorId.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;

			// 削除ボタン表示
			btnDelete.Visible = true;
			btnDelete2.Visible = true;
		}
		else
		{
			if (Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] == null)
			{
				// オペーレータ登録の後にブラウザバックを行うとシステムエラーになるため、
				// オペーレータ一覧ページに遷移させる
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
			}

			// 新規登録？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				btnInsert.Visible = true;
				btnInsert2.Visible = true;
			}
			// 編集確認？
			else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				trOperatorId.Visible = true;
				btnUpdate.Visible = true;
				btnUpdate2.Visible = true;
			}
		}
	}


	/// <summary>
	/// 編集ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPERATOR_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}



	/// <summary>
	/// 更新ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopOperator", "UpdateOperator"))
		{
			Hashtable htInput = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];

			// パスワード変更を行わない場合はNullを設定
			if ((string)htInput[Constants.FIELD_SHOPOPERATOR_PASSWORD] == "")
			{
				htInput[Constants.FIELD_SHOPOPERATOR_PASSWORD] = DBNull.Value;
			}

			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}
		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPERATOR_LIST);
	}


	/// <summary>
	/// 登録ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// オペレータ情報取得
		Hashtable input = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];
		input[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID] = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_SHOP_OPERATOR_ID, Constants.CONST_SHOPOPERATOR_ID_LENGTH);

		// 登録
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			using (SqlStatement sqlStatement = new SqlStatement("ShopOperator", "InsertOperator"))
			{
				// 登録
				int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}
		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPERATOR_LIST);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		var htInput = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];

		var service = new CsOperatorService(new CsOperatorRepository());
		if (service.DeleteCheck(this.LoginOperatorDeptId, (string)htInput[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSOPERATOR_DELETE_IMPOSSIBLE_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 削除
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopOperator", "DeleteOperator"))
		{
			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}
		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPERATOR_LIST);
	}
}