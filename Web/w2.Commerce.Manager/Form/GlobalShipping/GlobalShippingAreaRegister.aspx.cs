/*
=========================================================================================================
  Module      : 海外配送エリア登録ページ(GlobalShippingAreaRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.GlobalShipping;

/// <summary>
/// 海外配送料情報登録ページ
/// </summary>
public partial class Form_Shipping_ShippingRegister : BaseGlobalShippingPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// リクエスト取得＆ビューステート格納
			this.KeepintActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			InitializeComponents(this.KeepintActionStatus);

			// 新規
			if (this.KeepintActionStatus == Constants.ACTION_STATUS_INSERT)
			{

			}
			else if (this.KeepintActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// 編集
				var id = Request[Constants.REQUEST_KEY_GLOBAL_SHIPPING_AREA_ID];
				var sv = new GlobalShippingService();
				var model = sv.GetGlobalShippingAreaById(id);

				this.tbId.Text = model.GlobalShippingAreaId;
				this.litId.Text = model.GlobalShippingAreaId;
				this.tbName.Text = model.GlobalShippingAreaName;
				this.tbSortNo.Text = model.SortNo.ToString();
				this.chkValidFlg.Checked = (model.ValidFlg == GlobalShippingAreaModel.VALID_FLG_ON);
			}
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 入力データ
		var input = CreateInputByForm();

		// 検証
		var msg = (this.KeepintActionStatus == Constants.ACTION_STATUS_INSERT)
			? input.ValidateRegister()
			: input.ValidateUpdate();

		// 入力データをセッションに
		base.KeepingAreaInputData = input;

		//表示順半角数字のみチェック
		string strErrorMessages = null;
		strErrorMessages += Validator.CheckNecessaryError("表示順", this.KeepingAreaInputData.SortNo);
		strErrorMessages += Validator.CheckHalfwidthNumberError("表示順", this.KeepingAreaInputData.SortNo);

		if (strErrorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 画面遷移
		if (this.KeepintActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
			Response.Redirect(base.CreateInsertConfirmUrl());
		}
		else if (this.KeepintActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;
			Response.Redirect(base.CreateUpdateConfirmUrl());
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	private void InitializeComponents(string actionStatus)
	{
		// 新規登録
		if (actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trRegisterTop.Visible = true;
			tdGlobalShippingAreaIdEdit.Visible = true;
		}
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			// 編集
			trEditTop.Visible = true;
			tdShippingIdView.Visible = true;
		}
	}

	/// <summary>
	/// 入力データ取得
	/// </summary>
	/// <returns>入力データ</returns>
	public GlobalShippingAreaInput CreateInputByForm()
	{
		var input = new GlobalShippingAreaInput();
		input.GlobalShippingAreaId = this.tbId.Text;
		input.GlobalShippingAreaName = this.tbName.Text;
		input.SortNo = this.tbSortNo.Text;
		input.ValidFlg = this.chkValidFlg.Checked ? GlobalShippingAreaModel.VALID_FLG_ON : GlobalShippingAreaModel.VALID_FLG_OFF;
		input.LastChanged = base.LoginOperatorName;
		return input;
	}

	/// <summary>
	/// アクションステータス（ViewState保持）
	/// </summary>
	protected string KeepintActionStatus
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_ACTION_STATUS]); }
		set { ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = value; }
	}
}
