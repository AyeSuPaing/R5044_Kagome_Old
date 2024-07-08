/*
=========================================================================================================
  Module      : オペレータ情報登録ページ処理(OperatorRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.MenuAuthority;

public partial class Form_Operator_OperatorRegister : BasePage
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

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			// 新規？
			if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// 
			}
			// 編集？
			else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// セッションよりオペレータデータ取得
				m_htOperator = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];

				// オペレータID
				lbOperatorId.Text = (string)m_htOperator[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID];

				// オペレータ名
				tbOperatorName.Text = (string)m_htOperator[Constants.FIELD_SHOPOPERATOR_NAME];

				// メニュー権限が削除されている場合も考慮し、メニュー権限はここで選択
				foreach (ListItem li in ddlMenuAccessLevel.Items)
				{
					if (li.Value == (m_htOperator[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG]).ToString())
					{
						li.Selected = true;
						break;
					}
				}

				// ログインＩＤ
				tbLoginId.Text = (string)m_htOperator[Constants.FIELD_SHOPOPERATOR_LOGIN_ID];

				// パスワード
				tbPassWord.Text = (string)m_htOperator[Constants.FIELD_SHOPOPERATOR_PASSWORD];

				// 有効フラグ
				cbValid.Checked = ((string)m_htOperator[Constants.FIELD_SHOPOPERATOR_VALID_FLG] == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID);

				//メールアドレス
				tbMailAddress.Text = (string)m_htOperator[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR];

				ViewState.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, m_htOperator[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]);
			}
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		//------------------------------------------------------
		// メニュー権限一覧取得・ドロップダウン作成
		//------------------------------------------------------
		var menuAuthorities = new MenuAuthorityService().GetAllByPkgKbn(
			this.LoginOperatorShopId,
			Constants.ManagerSiteType);

		// ドロップダウン作成
		ddlMenuAccessLevel.Items.Add(new ListItem(Constants.STRING_UNACCESSABLEUSER_NAME, Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER));
		if (base.LoginOperatorMenuAccessLevel == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER) // スーパーユーザはスーパーユーザの時のみ表示
		{
			ddlMenuAccessLevel.Items.Add(new ListItem(Constants.STRING_SUPERUSER_NAME, Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER));
		}
		ddlMenuAccessLevel.Items.AddRange(
			menuAuthorities.Select(ma => new ListItem(ma.MenuAuthorityName, ma.MenuAuthorityLevel.ToString())).ToArray());

		//------------------------------------------------------
		// 画面制御
		//------------------------------------------------------
		// 新規登録？
		if (strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			// メニュー権限はデフォルト値設定
			ddlMenuAccessLevel.SelectedIndex = 0;
		}
		// 編集？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trOperatorId.Visible = true;
		}
	}

	/// <summary>
	/// 確認するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// セッションへ値格納
		Hashtable htInput = new Hashtable();

		// 入力データ
		htInput.Add(Constants.FIELD_SHOPOPERATOR_NAME, tbOperatorName.Text);

		// w2MartetingPlannerの権限設定
		if (ddlMenuAccessLevel.SelectedValue != "")
		{
			htInput.Add(Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG, ddlMenuAccessLevel.SelectedValue);
		}
		else
		{
			htInput.Add(Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG, System.DBNull.Value);
		}
		htInput.Add(Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG + "_name", ddlMenuAccessLevel.SelectedItem.Text);	// 確認画面表示用
		htInput.Add(Constants.FIELD_SHOPOPERATOR_LOGIN_ID, tbLoginId.Text);
		htInput.Add(Constants.FIELD_SHOPOPERATOR_PASSWORD, tbPassWord.Text);
		htInput.Add(Constants.FIELD_SHOPOPERATOR_VALID_FLG,
			(cbValid.Checked ? Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID : Constants.FLG_SHOPOPERATOR_VALID_FLG_INVALID));

		// 追加データ
		htInput.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);
		htInput.Add(Constants.FIELD_SHOPOPERATOR_LAST_CHANGED, this.LoginOperatorName);
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			// 更新の場合は対象オペレータID格納
			htInput.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, ViewState[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]);
		}

		if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED)
		{
			htInput.Add(Constants.FIELD_SHOPOPERATOR_MAIL_ADDR, tbMailAddress.Text);
		}

		// 入力チェック
		string strErrorMessages = null;
		// 新規登録？
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT)
		{
			strErrorMessages = Validator.Validate("ShopOperatorRegist", htInput);
		}
		// 編集確認？
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			strErrorMessages = Validator.Validate("ShopOperatorModify", htInput);
		}
		// エラーページへ遷移？
		if (strErrorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// セッションへパラメタセット
		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = htInput;

		// 確認画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPERATOR_CONFIRM + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}
}

