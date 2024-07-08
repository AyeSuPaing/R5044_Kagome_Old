/*
=========================================================================================================
  Module      : オペレータ情報登録ページ処理(OperatorRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.ShopOperator;
using w2.Common.Web;
using w2.Domain.CsIncidentWarningIcon;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.MailFrom;

public partial class Form_Operator_OperatorRegister : BasePage
{
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
			Initialize(strActionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			// 新規？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				if (this.ShopOperatorModel != null)
				{
					RestorationShopOperator();
				}

				if (Request[Constants.REQUEST_KEY_RETURN_FLAG] == Constants.RETURN_FLAG_TRUE)
				{
					// 戻るボタンで遷移してきた場合
					DisplayControlForUpdateCsOperator();
				}
				else
				{
					DisplayControlForRegistCsOperator();
				}
			}
			// 編集？
			else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				lbOperatorId.Text = this.ShopOperatorModel.OperatorId;
				tbMailAddress.Text = this.ShopOperatorModel.MailAddr;

				ViewState.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, this.ShopOperatorModel.OperatorId);
				RestorationShopOperator();
				// CSオペレータの場合の表示制御
				DisplayControlForUpdateCsOperator();
			}
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void Initialize(string strActionStatus)
	{
		//------------------------------------------------------
		// メニュー権限一覧取得・ドロップダウン作成
		//------------------------------------------------------
		// メニュー権限ドロップダウンの値セット
		ddlMenuAccessLevel.Items.AddRange(MenuUtility.CreateMenuAuthorityList(this.LoginOperatorShopId, this.LoginOperatorMenuAccessLevel));

		// オペレータ権限ドロップダウンの値セット
		CsOperatorAuthorityService authService = new CsOperatorAuthorityService(new CsOperatorAuthorityRepository());
		ddlOperatorAuthority.Items.Add(new ListItem(Constants.STRING_UNACCESSABLEUSER_NAME, ""));
		ddlOperatorAuthority.Items.AddRange(authService.GetAll(this.LoginOperatorDeptId).Select(p => new ListItem(p.OperatorAuthorityName, p.OperatorAuthorityId)).ToArray());

		// メール送信元ドロップダウンの値セット
		CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
		ddlMailFrom.Items.Add(new ListItem("", ""));
		ddlMailFrom.Items.AddRange(service.GetValidAll(base.LoginOperatorDeptId).Select(p => new ListItem(p.EX_DisplayAddress, p.MailFromId)).ToArray());

		// 表示順ドロップダウンの値セット
		for (int i = 1; i <= 100; i++)
		{
			ListItem li = new ListItem(i.ToString(), i.ToString());
			ddlDisplayOrder.Items.Add(li);
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
	/// ショップオペレータ入力欄を復元
	/// </summary>
	private void RestorationShopOperator()
	{
		tbOperatorName.Text = this.ShopOperatorModel.Name;
		var count = 0;
		foreach (ListItem li in ddlMenuAccessLevel.Items)
		{
			var menuAccessLevel =
				(this.ShopOperatorModel.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG]
				?? string.Empty).ToString();
			if (li.Value == menuAccessLevel)
			{
				ddlMenuAccessLevel.SelectedIndex = count;
				break;
			}

			count++;
		}
		tbLoginId.Text = this.ShopOperatorModel.LoginId;
		tbPassWord.Text = this.ShopOperatorModel.Password;
		tbMailAddress.Text = this.ShopOperatorModel.MailAddr;
		cbValid.Checked = (this.ShopOperatorModel.ValidFlg == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID);
	}

	/// <summary>
	/// CSオペレータ登録の場合の表示制御
	/// </summary>
	private void DisplayControlForRegistCsOperator()
	{
		chkIsCsOperator.Checked = true;
	}

	/// <summary>
	/// CSオペレータ更新の場合の表示制御
	/// </summary>
	private void DisplayControlForUpdateCsOperator()
	{
		var model = (Session[Constants.SESSION_KEY_CSOPERATOR_INFO] != null)
			? (CsOperatorModel)Session[Constants.SESSION_KEY_CSOPERATOR_INFO]
			: null;
		if (model != null)
		{
			chkIsCsOperator.Checked = true;
			ddlOperatorAuthority.SelectedValue = (ddlOperatorAuthority.Items.FindByValue(model.OperatorAuthorityId) != null ? model.OperatorAuthorityId : "");
			ddlMailFrom.SelectedValue = (ddlMailFrom.Items.FindByValue(model.MailFromId) != null ? model.MailFromId : "");
			chkNoticeMail.Checked = (model.NotifyInfoFlg == Constants.FLG_CSOPERATOR_NOTIFY_INFO_FLG_VALID);
			chkWarningMail.Checked = (model.NotifyWarnFlg == Constants.FLG_CSOPERATOR_NOTIFY_WARN_FLG_VALID);
			tbMailAddr.Text = model.MailAddr;
			ddlDisplayOrder.SelectedValue = model.DisplayOrder.ToString();
		}

		// インシデント警告アイコン表示制御
		var incidentWarningIconModels = (this.IncidentWarningIconModels != null)
			? (CsIncidentWarningIconModel[])Session[Constants.SESSION_KEY_INCIDENT_WARNING_ICON]
			: null;
		DisplayControlForUpdateIncidentWarningIcon(incidentWarningIconModels);
	}

	/// <summary>
	/// インシデント警告アイコン表示制御
	/// </summary>
	/// <param name="models">インシデント警告アイコンモデル配列</param>
	private void DisplayControlForUpdateIncidentWarningIcon(CsIncidentWarningIconModel[] models)
	{
		if (models == null) return;

		var noneIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_NONE))
			.ToArray();
		if (noneIcon.Length > 0)
		{
			cbNone.Checked = true;
			tbNoneOrangeHours.Enabled = true;
			tbNoneOrangeMinutes.Enabled = true;
			tbNoneRedHours.Enabled = true;
			tbNoneRedMinutes.Enabled = true;
		}
		foreach (var model in noneIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					tbNoneOrangeHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbNoneOrangeMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					tbNoneRedHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbNoneRedMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;
			}
		}

		var activeIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_ACTIVE))
			.ToArray();
		if (activeIcon.Length > 0)
		{
			cbActive.Checked = true;
			tbActiveOrangeHours.Enabled = true;
			tbActiveOrangeMinutes.Enabled = true;
			tbActiveRedHours.Enabled = true;
			tbActiveRedMinutes.Enabled = true;
		}
		foreach (var model in activeIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					tbActiveOrangeHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbActiveOrangeMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					tbActiveRedHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbActiveRedMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;
			}
		}

		var suspendIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_SUSPEND))
			.ToArray();
		if (suspendIcon.Length > 0)
		{
			cbSuspend.Checked = true;
			tbSuspendOrangeHours.Enabled = true;
			tbSuspendOrangeMinutes.Enabled = true;
			tbSuspendRedHours.Enabled = true;
			tbSuspendRedMinutes.Enabled = true;
		}
		foreach (var model in suspendIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					tbSuspendOrangeHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbSuspendOrangeMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					tbSuspendRedHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbSuspendRedMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;
			}
		}

		var urgentIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_URGENT))
			.ToArray();
		if (urgentIcon.Length > 0)
		{
			cbUrgent.Checked = true;
			tbUrgentOrangeHours.Enabled = true;
			tbUrgentOrangeMinutes.Enabled = true;
			tbUrgentRedHours.Enabled = true;
			tbUrgentRedMinutes.Enabled = true;
		}
		foreach (var model in urgentIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					tbUrgentOrangeHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbUrgentOrangeMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					tbUrgentRedHours.Text = HtmlSanitizer.HtmlEncode((model.Term.Value / 60).ToString());
					tbUrgentRedMinutes.Text = HtmlSanitizer.HtmlEncode((model.Term.Value % 60).ToString());
					break;
			}
		}
	}

	/// <summary>
	/// 確認するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		var shopOperatorInput = new ShopOperatorInput
		{
			ShopId = this.LoginOperatorShopId,
			Name = tbOperatorName.Text,
			LoginId = tbLoginId.Text,
			Password = tbPassWord.Text,
			ValidFlg = (cbValid.Checked
				? Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID
				: Constants.FLG_SHOPOPERATOR_VALID_FLG_INVALID),
			DelFlg = Constants.FLG_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG_VALID,
		};

		if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED)
		{
			shopOperatorInput.MailAddr = tbMailAddress.Text;
		}

		shopOperatorInput.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG] =
			(ddlMenuAccessLevel.SelectedValue != string.Empty) ? ddlMenuAccessLevel.SelectedValue : null;

		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			// 更新の場合既存の編集不可情報を格納
			shopOperatorInput.OperatorId = (string)ViewState[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID];
		}

		// 入力チェック
		string strErrorMessages = null;
		// 新規登録？
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT)
		{
			this.ShopOperatorModel = new ShopOperatorModel();
			strErrorMessages = shopOperatorInput.Validate("ShopOperatorRegist");
		}
		// 編集確認？
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			strErrorMessages = shopOperatorInput.Validate("ShopOperatorModify");
		}

		// CSオペレータの検証
		if (chkIsCsOperator.Checked)
		{
			strErrorMessages += GetCsOperatorInput().Validate();
		}
		// CS権限無し かつ メニュー権限ユーザーが権限無しでない場合はエラー（CS権限無しは画面を見せたくない）
		else if (ddlMenuAccessLevel.SelectedValue != Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER)
		{
			strErrorMessages += WebMessages.GetMessages(WebMessages.ERROR_MANAGER_WHEN_SETTING_MENU_AUTHORITY)
				.Replace("@@ 1 @@", Environment.NewLine);
		}

		if (string.IsNullOrEmpty(strErrorMessages) == false)
		{
			lErrorMessage.Text = HtmlSanitizer.HtmlEncodeChangeToBr(strErrorMessages);
			return;
		}

		// CSオペレータの入力モデル取得
		CsOperatorModel csOperatorModel = CreateCsOperatorModel();

		var shopOperatorModel = shopOperatorInput.CreateModel();
		shopOperatorModel.LastChanged = this.LoginOperatorName;

		this.ShopOperatorModel = shopOperatorModel;
		this.CsOperatorModel = csOperatorModel;
		this.IncidentWarningIconModels = CreateIncidentWarningIconModels();

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		// 確認画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_CONFIRM + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}

	/// <summary>
	/// 入力値を元にCSオペレータモデルを生成
	/// </summary>
	private CsOperatorModel CreateCsOperatorModel()
	{
		// CSオペレータとしない場合はNULL返却
		if (chkIsCsOperator.Checked == false) return null;

		var model = GetCsOperatorInput().CreateModel();
		model.OperatorId = string.Empty;	// オペレータIDはこの時点では確定しない。ShopOperator登録・更新後に確定する
		model.MailAddr = (model.MailAddr != null) ? model.MailAddr : string.Empty;
		model.LastChanged = this.LoginOperatorName;
		model.EX_OperatorAuthorityName = ddlOperatorAuthority.SelectedItem.Text;
		model.EX_MailFromDisplayName = ddlMailFrom.SelectedItem.Text;
		model.EX_MenuAuthorityName = ddlMenuAccessLevel.SelectedItem.Text;
		model.LastChanged = this.LoginOperatorName;
		return model;
	}

	/// <summary>
	/// CSオペレータインプット取得
	/// </summary>
	/// <returns>CSオペレータインプット</returns>
	private CsOperatorInput GetCsOperatorInput()
	{
		var input = new CsOperatorInput
	{
			DeptId = this.LoginOperatorDeptId,
			OperatorAuthorityId = ddlOperatorAuthority.SelectedValue,
			MailFromId = ddlMailFrom.SelectedValue,
			NotifyInfoFlg = chkNoticeMail.Checked
				? Constants.FLG_CSOPERATOR_NOTIFY_INFO_FLG_VALID
				: Constants.FLG_CSOPERATOR_NOTIFY_INFO_FLG_INVALID,
			NotifyWarnFlg = chkWarningMail.Checked
				? Constants.FLG_CSOPERATOR_NOTIFY_WARN_FLG_VALID
				: Constants.FLG_CSOPERATOR_NOTIFY_WARN_FLG_INVALID,
			DisplayOrder = ddlDisplayOrder.SelectedValue,
			MailAddr = (chkNoticeMail.Checked || chkWarningMail.Checked)
				? tbMailAddr.Text
				: null
		};
		return input;
	}

	/// <summary>
	/// インシデント警告アイコン切り替え時間モデル配列取得
	/// </summary>
	/// <returns>モデル配列化した切替時間テーブル</returns>
	private CsIncidentWarningIconModel[] CreateIncidentWarningIconModels()
	{
		var models = new List<CsIncidentWarningIconModel>();
		// インシデントステータス：未対応
		if (cbNone.Checked)
		{
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_NONE,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE,
					Term = ConvertToMinutes(tbNoneOrangeHours.Text, tbNoneOrangeMinutes.Text)
				});
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_NONE,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED,
					Term = ConvertToMinutes(tbNoneRedHours.Text, tbNoneRedMinutes.Text)
				});
		}
		// インシデントステータス：対応中
		if (cbActive.Checked)
		{
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_ACTIVE,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE,
					Term = ConvertToMinutes(tbActiveOrangeHours.Text, tbActiveOrangeMinutes.Text)
				});
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_ACTIVE,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED,
					Term = ConvertToMinutes(tbActiveRedHours.Text, tbActiveRedMinutes.Text)
				});
		}
		// インシデントステータス：保留
		if (cbSuspend.Checked)
		{
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_SUSPEND,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE,
					Term = ConvertToMinutes(tbSuspendOrangeHours.Text, tbSuspendOrangeMinutes.Text)
				});
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_SUSPEND,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED,
					Term = ConvertToMinutes(tbSuspendRedHours.Text, tbSuspendRedMinutes.Text)
				});
		}
		// インシデントステータス：緊急
		if (cbUrgent.Checked)
		{
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_URGENT,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE,
					Term = ConvertToMinutes(tbUrgentOrangeHours.Text, tbUrgentOrangeMinutes.Text)
				});
			models.Add(
				new CsIncidentWarningIconModel
				{
					IncidentStatus = Constants.FLG_CSINCIDENT_STATUS_URGENT,
					WarningLevel = Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED,
					Term = ConvertToMinutes(tbUrgentRedHours.Text, tbUrgentRedMinutes.Text)
				});
		}

		// 共通項目を格納
		foreach (var model in models)
		{
			model.DeptId = this.LoginOperatorDeptId;
			model.LastChanged = this.LoginOperatorName;
		}
		var result = (models.Count > 0)
			? models.ToArray()
			: null;
		return result;
	}

	/// <summary>
	/// 時分文字列を数値型の分単位に変換
	/// </summary>
	/// <param name="hours">時間</param>
	/// <param name="minutes">分</param>
	/// <returns>分（無効値または空文字はnull）</returns>
	private int? ConvertToMinutes(string hours, string minutes)
	{
		int intHours, intMinutes;
		var result = int.TryParse(hours, out intHours) | int.TryParse(minutes, out intMinutes)
			? (int?)new TimeSpan(intHours, intMinutes, 0).TotalMinutes
			: null;
		return result;
	}

	#region プロパティ
	/// <summary>ショップオペレータモデル</summary>
	private ShopOperatorModel ShopOperatorModel
	{
		get { return (ShopOperatorModel)Session[Constants.SESSION_KEY_OPERATOR_INFO]; }
		set { Session[Constants.SESSION_KEY_OPERATOR_INFO] = value; }
	}
	/// <summary>Csオペレータモデル</summary>
	private CsOperatorModel CsOperatorModel
	{
		set { Session[Constants.SESSION_KEY_CSOPERATOR_INFO] = value; }
	}
	/// <summary>インシデント警告アイコンモデル配列</summary>
	private CsIncidentWarningIconModel[] IncidentWarningIconModels
	{
		get { return (CsIncidentWarningIconModel[])Session[Constants.SESSION_KEY_INCIDENT_WARNING_ICON]; }
		set { Session[Constants.SESSION_KEY_INCIDENT_WARNING_ICON] = value; }
	}
	/// <summary>戻るボタンURL</summary>
	protected string BackButtonUrl
	{
		get
	{
			var result = (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
				? Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST
				: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_CONFIRM)
					.AddParam(Constants.REQUEST_KEY_OPERATOR_ID, this.ShopOperatorModel.OperatorId)
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
					.CreateUrl();
			return result;
		}
	}
	#endregion
}

