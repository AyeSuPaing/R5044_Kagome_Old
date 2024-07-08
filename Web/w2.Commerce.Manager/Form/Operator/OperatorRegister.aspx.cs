/*
=========================================================================================================
  Module      : オペレータ情報登録ページ処理(OperatorRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.MenuAuthority;
using w2.Domain.RealShop;

public partial class Form_Operator_OperatorRegister : BasePage
{
	protected Hashtable m_htOperator = new Hashtable();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
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
			// 入れる?
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				// Display real shop area if back from confirm
				if (Constants.REALSHOP_OPTION_ENABLED)
				{
					if (Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] != null)
					{
						m_htOperator = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];
						var listRealShop = (List<string>)m_htOperator[Constants.FLG_LIST_REAL_SHOP_ID];
						Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = null;
						if ((listRealShop == null) || (listRealShop.Count == 0))
						{
							this.cbRealShopOperatorRegister.Checked = false;
							Page.ClientScript.RegisterStartupScript(this.GetType(), "change", "javascript:change(false);", true);
							return;
						}
					}
					this.cbRealShopOperatorRegister.Checked = true;
				}
			}
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

				ViewState.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, m_htOperator[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]);

				tbMailAddress.Text = (string)m_htOperator[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR];

				if (Constants.REALSHOP_OPTION_ENABLED)
				{
					var realShopOperatorRegisterChecked = false;
					foreach (var ri in rArea.Items.Cast<RepeaterItem>())
					{
						var ckRealShop = (CheckBoxList)ri.FindControl("ckRealShop");
						var ckArea = (CheckBox)ri.FindControl("ckArea");
						var operatorAuthoritys = DomainFacade.Instance.OperatorAuthorityService.Get(
							this.LoginOperatorShopId,
							(string)m_htOperator[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]);

						if (operatorAuthoritys != null)
						{
							var realShopIds = operatorAuthoritys.Select(ope => ope.ConditionValue).ToArray();
							SetSearchCheckBoxValue(ckRealShop, realShopIds);
							if (realShopIds.Length > 0)
							{
								realShopOperatorRegisterChecked = true;
							}
						}

						ckArea.Checked = ckRealShop.Items.Cast<ListItem>().All(shop => shop.Selected == true);
					}
					// Display real shop area if DB or session of list of real shop id has value
					this.cbRealShopOperatorRegister.Checked = realShopOperatorRegisterChecked;
					if (m_htOperator[Constants.FLG_LIST_REAL_SHOP_ID] == null)
					{
						if (realShopOperatorRegisterChecked) return;
					}
					else if (((List<string>)m_htOperator[Constants.FLG_LIST_REAL_SHOP_ID]).Count > 0)
					{
						return;
					}
					Page.ClientScript.RegisterStartupScript(this.GetType(), "change", "javascript:change(false);", true);
				}
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
		ddlMenuAccessLevel.Items.Add(new ListItem(Constants.STRING_UNACCESSABLEUSER_NAME, Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER));
		if (base.LoginOperatorMenuAccessLevel == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER) // スーパーユーザはスーパーユーザの時のみ表示
		{
			ddlMenuAccessLevel.Items.Add(new ListItem(Constants.STRING_SUPERUSER_NAME, Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER));
		}
		ddlMenuAccessLevel.Items.AddRange(
			menuAuthorities.Select(ma => new ListItem(ma.MenuAuthorityName, ma.MenuAuthorityLevel.ToString())).ToArray());

		if (Constants.REALSHOP_OPTION_ENABLED)
		{
			// Get real shop
			var realShop = new RealShopService().GetAll();
			if (realShop != null)
			{
				var areaExist = realShop.Select(area => area.AreaId).Distinct();

				var areas = DataCacheControllerFacade
					.GetRealShopAreaCacheController()
					.GetRealShopAreaList()
					.Where(area => areaExist.Contains(area.AreaId));

				rArea.DataSource = areas;
				rArea.DataBind();
			}
		}

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
		// w2Commerceの権限設定
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

		// Get selected real shop
		if (Constants.REALSHOP_OPTION_ENABLED)
		{
			var realShopSelect = new List<string>();
			var areaRealShopNameSelect = new StringBuilder();
			if (this.cbRealShopOperatorRegister.Checked)
			{
				foreach (RepeaterItem ri in rArea.Items)
				{
					var ckRealShop = (CheckBoxList)ri.FindControl("ckRealShop");
					var areaRealShop = ckRealShop.Items
						.Cast<ListItem>()
						.Where(li => li.Selected)
						.Select(li => li.Value)
						.ToArray();

					if (areaRealShop.Length > 0)
					{
						var ckArea = (CheckBox)ri.FindControl("ckArea");
						areaRealShopNameSelect.AppendFormat("{0}: ", ckArea.Text);

						var areaRealShopName = ckRealShop.Items
							.Cast<ListItem>()
							.Where(li => li.Selected)
							.Select(li => li.Text)
							.ToArray();

						areaRealShopNameSelect.AppendFormat("{0}<br />", string.Join(", ", areaRealShopName));
					}

					realShopSelect.AddRange(areaRealShop);
				}
			}
			htInput[Constants.FLG_LIST_REAL_SHOP_ID] = realShopSelect;
			htInput[Constants.FLG_AREA_REAL_SHOP_NAME] = areaRealShopNameSelect.ToString();
		}

		// セッションへパラメタセット
		Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO] = htInput;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

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
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 確認画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_CONFIRM + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}

	/// <summary>
	/// Get real shop by area
	/// </summary>
	/// <param name="areaId">Area id</param>
	/// <returns>List real shop</returns>
	public List<ListItem> GetRealShopByArea(string areaId)
	{
		var shopOperator = (Hashtable)Session[Constants.SESSIONPARAM_KEY_OPERATOR_INFO];
		var realShopList = new RealShopService().GetAll();

		var realShopArea = realShopList
			.Where(shop => (shop.AreaId == areaId))
			.Select(item => new ListItem(item.Name, item.RealShopId))
			.ToList();

		return realShopArea;
	}
}
