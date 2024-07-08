/*
=========================================================================================================
  Module      : オペレータ情報確認ページ処理(OperatorConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Domain.ShopOperator;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.MailFrom;
using w2.Common.Web;
using w2.Domain.CsIncidentWarningIcon;
using w2.Domain.MenuAuthority;

public partial class Form_Operator_OperatorConfirm : BasePage
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

			string strOperatorId = Request[Constants.REQUEST_KEY_OPERATOR_ID];
			ViewState.Add(Constants.REQUEST_KEY_OPERATOR_ID, strOperatorId);
			
			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			Initialize(strActionStatus);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・画面確認？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT
				|| strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				// Csオペレータモデルが取れたか否かによってCSオペレータとして登録するか否かを判断
				this.IsCsOperator = (this.CsOperatorModel != null);
				if (IsCsOperator)
				{
					ShowIncidentWarningIconSettings();
				}
			}
			// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				var shopOperatorService = new ShopOperatorService();
				this.ShopOperatorModel = shopOperatorService.Get(this.LoginOperatorShopId, strOperatorId);
				if (this.ShopOperatorModel == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] =
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// 登録・更新用のCSオペレータモデル
				this.CsOperatorModel = GetCsOperatorModel(this.LoginOperatorDeptId, strOperatorId);

				// Csオペレータモデルが取れたか否かによってCSオペレータか判断
				this.IsCsOperator = (this.CsOperatorModel != null);

			}
			// 該当なし？
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize(string strActionStatus)
	{
		// 詳細表示？
		if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditBottom.Visible = true;
			btnEditTop.Visible = true;

			// 作成日・更新日・最終更新者表示
			trOperatorId.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;

			// 削除ボタン表示
			btnDeleteBottom.Visible = true;
			btnDeleteTop.Visible = true;
		}
		else
		{
			if (Session[Constants.SESSION_KEY_OPERATOR_INFO] == null)
			{
				// オペーレータ登録の後にブラウザバックを行うとシステムエラーになるため、
				// オペーレータ一覧ページに遷移させる
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
			}

			// 新規登録？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				btnInsertBottom.Visible = true;
				btnInsertTop.Visible = true;
			}
			// 編集確認？
			else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				trOperatorId.Visible = true;
				btnUpdateBottom.Visible = true;
				btnUpdateTop.Visible = true;
			}
		}
	}

	/// <summary>
	/// CSオペレータモデル取得
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="operatorId">オペレータID</param>
	/// <returns>CSオペレータモデル</returns>
	private CsOperatorModel GetCsOperatorModel(string deptId, string operatorId)
	{
		CsOperatorService service = new CsOperatorService(new CsOperatorRepository());
		CsOperatorModel model = service.Get(deptId, operatorId);
		if (model != null)
		{
			// メール送信元を取得
			CsMailFromService mailFromService = new CsMailFromService(new CsMailFromRepository());
			CsMailFromModel mailFromModel = mailFromService.Get(deptId, model.MailFromId);
			if ((mailFromModel != null) && (mailFromModel.ValidFlg == Constants.FLG_CSMAILFROM_VALID_FLG_VALID)) model.EX_MailFromDisplayName = mailFromModel.EX_DisplayAddress;

			// 表示用メニュー権限名取得
			if (this.ShopOperatorModel.MenuAccessLevel3.ToString()
				== Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER)
			{
				model.EX_MenuAuthorityName = Constants.STRING_UNACCESSABLEUSER_NAME;
			}
			else if (this.ShopOperatorModel.MenuAccessLevel3.ToString()
				== Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER)
			{
				model.EX_MenuAuthorityName = Constants.STRING_SUPERUSER_NAME;
			}
			else
			{
				var menuAuthorityService = new MenuAuthorityService();
				model.EX_MenuAuthorityName = menuAuthorityService
					.GetNameByLevel(this.ShopOperatorModel.MenuAccessLevel3.Value);

			}
			var incidentWarningIconService = new CsIncidentWarningIconService();
			this.IncidentWarningIconModels = incidentWarningIconService.GetByOperatorId(deptId, operatorId);
			ShowIncidentWarningIconSettings();
		}
		return model;
	}

	/// <summary>
	/// インシデント警告アイコン設定情報を画面に反映
	/// </summary>
	private void ShowIncidentWarningIconSettings()
	{
		if (this.IncidentWarningIconModels == null) return;

		var noneIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_NONE))
			.ToArray();
		if (noneIcon.Length > 0) lIncidentWarningIconNoneValid.Text = HtmlSanitizer.HtmlEncode("○");
		foreach (var model in noneIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconNoneOrange.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconNoneRed.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;
			}
		}

		var activeIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_ACTIVE))
			.ToArray();
		if (activeIcon.Length > 0) lIncidentWarningIconActiveValid.Text = HtmlSanitizer.HtmlEncode("○");
		foreach (var model in activeIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconActiveOrange.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconActiveRed.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;
			}
		}

		var suspendIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_SUSPEND))
			.ToArray();
		if (suspendIcon.Length > 0) lIncidentWarningIconSuspendValid.Text = HtmlSanitizer.HtmlEncode("○");
		foreach (var model in suspendIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconSuspendOrange.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconSuspendRed.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;
			}
		}

		var urgentIcon = this.IncidentWarningIconModels
			.Where(model => (model.IncidentStatus == Constants.FLG_CSINCIDENT_STATUS_URGENT))
			.ToArray();
		if (urgentIcon.Length > 0) lIncidentWarningIconUrgentValid.Text = HtmlSanitizer.HtmlEncode("○");
		foreach (var model in urgentIcon)
		{
			switch (model.WarningLevel)
			{
				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_ORANGE:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconUrgentOrange.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;

				case Constants.FLG_CSINCIDENT_WARNING_LEVEL_RED:
					if (model.Term.HasValue == false) break;
					lIncidentWarningIconUrgentRed.Text =
						HtmlSanitizer.HtmlEncode(ConvertTermAndSetLiteral(model.Term.Value));
					break;
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
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// 更新ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		using (var sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			var shopOperatorService = new ShopOperatorService();
			if (string.IsNullOrEmpty(this.ShopOperatorModel.Password))
			{
				// 更新用にパスワード取得
				this.ShopOperatorModel.Password = shopOperatorService.Get(
					this.ShopOperatorModel.ShopId,
					this.ShopOperatorModel.OperatorId,
					sqlAccessor
				).Password;
			}

			shopOperatorService.Modify(
				this.ShopOperatorModel.ShopId,
				this.ShopOperatorModel.OperatorId,
				model =>
				{
					model.Name = this.ShopOperatorModel.Name;
					model.MenuAccessLevel3 = this.ShopOperatorModel.MenuAccessLevel3;
					model.LoginId = this.ShopOperatorModel.LoginId;
					model.Password = this.ShopOperatorModel.Password;
					model.MailAddr = this.ShopOperatorModel.MailAddr;
					model.ValidFlg = this.ShopOperatorModel.ValidFlag;
					model.LastChanged = this.LoginOperatorName;
				},
				sqlAccessor);

			// CSオペレータ登録更新or削除
			if (this.CsOperatorModel != null)
			{
				RegisterUpdateCsOperator(this.ShopOperatorModel.OperatorId, sqlAccessor);
			}
			else
			{
				var csOperatorService = new CsOperatorService(new CsOperatorRepository());
				csOperatorService.Delete(this.LoginOperatorDeptId, this.ShopOperatorModel.OperatorId, sqlAccessor);
				var incidentWarningIconService = new CsIncidentWarningIconService();
				incidentWarningIconService.DeleteByOperatorId(
					this.LoginOperatorDeptId,
					this.ShopOperatorModel.OperatorId,
					sqlAccessor);
			}
			sqlAccessor.CommitTransaction();
		}
		Session[Constants.SESSION_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
	}

	/// <summary>
	/// 登録ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録オペレータID
		string registOperatorId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_SHOP_OPERATOR_ID, Constants.CONST_CS_OPERATOR_AUTHORITY_ID_LENGTH);
		this.ShopOperatorModel.OperatorId = registOperatorId;

		// 登録
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			new ShopOperatorService().Insert(this.ShopOperatorModel, sqlAccessor);

			// CSオペレータ登録
			if (this.CsOperatorModel != null) RegisterUpdateCsOperator(registOperatorId, sqlAccessor);

			sqlAccessor.CommitTransaction();
		}
		Session[Constants.SESSION_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
	}

	/// <summary>
	/// CSオペレータ登録更新
	/// </summary>
	/// <param name="operatorId">登録するオペレータID</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	private void RegisterUpdateCsOperator(string operatorId, SqlAccessor sqlAccessor)
	{
		// オペレータIDセット
		this.CsOperatorModel.OperatorId = operatorId;

		// CSオペレータ情報の登録更新
		CsOperatorService service = new CsOperatorService(new CsOperatorRepository());
		service.RegisterUpdate(this.CsOperatorModel, sqlAccessor);
		// インシデント警告アイコンアップサート
		if (this.IncidentWarningIconModels != null)
		{
			foreach (var model in this.IncidentWarningIconModels)
			{
				model.OperatorId = operatorId;
			}
		}

		new CsIncidentWarningIconService().Modify(
			this.LoginOperatorDeptId,
			this.ShopOperatorModel.OperatorId,
			this.IncidentWarningIconModels,
			sqlAccessor);
			// 自分の情報であればセッション情報更新
		if (operatorId == this.LoginOperatorId) this.LoginOperatorCsInfo = this.CsOperatorModel;
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// 削除
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			// CSオペレータ存在チェック
			if (this.CsOperatorModel == null)
			{
				new ShopOperatorService().Delete(
					this.LoginOperatorShopId,
					this.ShopOperatorModel.OperatorId,
					sqlAccessor);
				sqlAccessor.CommitTransaction();
				Session[Constants.SESSION_KEY_OPERATOR_INFO] = null;
				// 一覧画面へ戻る
				Dispose();
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
			}

			var csOperatorService = new CsOperatorService(new CsOperatorRepository());
			// CSオペレータ削除チェック
			if (csOperatorService.DeleteCheck(this.LoginOperatorDeptId, this.CsOperatorModel.OperatorId, sqlAccessor))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSOPERATOR_DELETE_IMPOSSIBLE_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// インシデント警告アイコンテーブル削除
			if (this.IncidentWarningIconModels != null)
			{
				new CsIncidentWarningIconService().DeleteByOperatorId(
					this.LoginOperatorDeptId,
					this.ShopOperatorModel.OperatorId,
					sqlAccessor);
			}
			csOperatorService.Delete(this.LoginOperatorDeptId, this.CsOperatorModel.OperatorId, sqlAccessor);
			new ShopOperatorService().Delete(
				this.LoginOperatorShopId,
				this.ShopOperatorModel.OperatorId,
				sqlAccessor);
			sqlAccessor.CommitTransaction();
		}
		Session[Constants.SESSION_KEY_OPERATOR_INFO] = null;

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST);
	}

	/// <summary>
	/// 戻るボタンURL取得
	/// </summary>
	/// <returns>URL</returns>
	protected string GetBackButtonUrl()
	{
		var result = "";
		if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			result = Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST;
		}
		else if(this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			result = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_REGISTER)
				.AddParam(Constants.REQUEST_KEY_OPERATOR_ID, this.ShopOperatorModel.OperatorId)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
				.CreateUrl();
		}
		else
		{
			result = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_REGISTER)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT)
				.AddParam(Constants.REQUEST_KEY_RETURN_FLAG, Constants.RETURN_FLAG_TRUE)
				.CreateUrl();
		}
		return result;
	}

	/// <summary>
	/// 数値型分を時分に変換した文字列を返す
	/// </summary>
	/// <param name="minutes">分</param>
	/// <returns>時間文字列</returns>
	private string ConvertTermAndSetLiteral(int minutes)
	{
		var timeSpan = new TimeSpan(0, 0, minutes, 0);
		var result = (minutes >= 60)
			? string.Format("{0}時間{1}分", Math.Floor(timeSpan.TotalHours), timeSpan.Minutes)
			: string.Format("{0}分", timeSpan.Minutes);
		return result;
	}

	#region プロパティ
	/// <summary>Csオペレータとして登録するか否か</summary>
	protected bool IsCsOperator { get; set; }
	/// <summary>ショップオペレータモデル</summary>
	protected ShopOperatorModel ShopOperatorModel
	{
		get { return (ShopOperatorModel)Session[Constants.SESSION_KEY_OPERATOR_INFO]; }
		private set { Session[Constants.SESSION_KEY_OPERATOR_INFO] = value; }
	}
	/// <summary>Csオペレータモデル</summary>
	protected CsOperatorModel CsOperatorModel
	{
		get { return (CsOperatorModel)Session[Constants.SESSION_KEY_CSOPERATOR_INFO]; }
		private set { Session[Constants.SESSION_KEY_CSOPERATOR_INFO] = value; }
	}
	/// <summary>インシデント警告アイコンモデル配列</summary>
	private CsIncidentWarningIconModel[] IncidentWarningIconModels
	{
		get { return (CsIncidentWarningIconModel[])Session[Constants.SESSION_KEY_INCIDENT_WARNING_ICON]; }
		set { Session[Constants.SESSION_KEY_INCIDENT_WARNING_ICON] = value; }
	}
	#endregion
}

