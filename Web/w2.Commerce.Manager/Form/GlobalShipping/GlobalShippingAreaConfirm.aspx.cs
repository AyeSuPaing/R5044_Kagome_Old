/*
=========================================================================================================
  Module      : 海外配送エリア確認ページ(GlobalShippingAreaConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global;
using w2.Domain.CountryLocation;
using w2.Domain.GlobalShipping;

/// <summary>
/// 海外配送エリア詳細ページ
/// </summary>
public partial class Form_Global_Shipping_Area_Confirm : BaseGlobalShippingPage
{
	/// <summary>
	/// 拡張構成条件を使うかどうか
	/// true：使う（住所3、住所2、郵便番号の構成条件が指定可能となる）
	/// false：使わない（PKGではデータ登録が煩雑になるためデフォルト使わない）
	/// </summary>
	protected static bool USE_EXTEND_COMPONENT_CONDITION = false;

	/// <summary>海外配送エリアモデル</summary>
	protected GlobalShippingAreaModel m_areaData = new GlobalShippingAreaModel();

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			this.KeepintActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(this.KeepintActionStatus);

			if (this.KeepintActionStatus == Constants.ACTION_STATUS_INSERT || this.KeepintActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// 登録・更新画面確認
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				var model = base.KeepingAreaInputData.CreateModel();
				model.DateChanged = DateTime.Now;
				model.DateCreated = DateTime.Now;
				m_areaData = model;
			}
			else if (this.KeepintActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// 詳細表示
				var id = Request[Constants.REQUEST_KEY_GLOBAL_SHIPPING_AREA_ID];
				var sv = new GlobalShippingService();
				var model = sv.GetGlobalShippingAreaById(id);
				m_areaData = model;
				this.KeepingEditId = id;

				// 構成条件国ドロップダウン
				var countrySv = new CountryLocationService();
				var countryModels = countrySv.GetShippingAvailableCountry();
				ddlConditionCountry.Items.AddRange(countryModels.Select(x => new ListItem(x.CountryIsoCode, x.CountryIsoCode)).ToArray());

				if (ddlConditionCountry.Items.Count > 0)
				{
					ddlConditionCountry.SelectedValue = ddlConditionCountry.Items[0].Value;
				}
				
				ddlConditionCountry.DataBind();

				var compone = sv.GetAreaComponentByAreaId(this.KeepingEditId);
				this.repAreaComponent.DataSource = compone;
				this.repAreaComponent.DataBind();

			}
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			this.DataBind();
		}
	}
	#endregion

	#region #btnEdit_Click 編集ボタンクリック
	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(base.CreateEditUrl(this.KeepingEditId));
	}
	#endregion

	#region #btnInsert_Click 登録するボタンクリック
	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		var model = base.KeepingAreaInputData.CreateModel();
		model.DateChanged = DateTime.Now;
		model.DateCreated = DateTime.Now;

		// 登録
		var sv = new GlobalShippingService();
		sv.RegisterGlobalShippingArea(model);

		// 確認画面へ
		Response.Redirect(base.CreateDetailUrl(model.GlobalShippingAreaId));
	}
	#endregion

	#region #btnUpdate_Click 更新ボタンクリック
	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		var model = base.KeepingAreaInputData.CreateModel();
		model.DateChanged = DateTime.Now;
		model.DateCreated = DateTime.Now;

		// 登録
		var sv = new GlobalShippingService();
		sv.UpdateGlobalShippingArea(model);

		// 確認画面へ
		Response.Redirect(base.CreateDetailUrl(model.GlobalShippingAreaId));
	}
	#endregion

	#region #btnBackToListTop_Click 一覧へ戻るボタンクリック
	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnBackToListTop_Click(object sender, EventArgs e)
	{
		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_AREA_LIST);
	}
	#endregion

	#region #btnAddCondition_Click 構成条件追加ボタン押下
	/// <summary>
	/// 構成条件追加ボタン押下
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnAddCondition_Click(object sender, EventArgs e)
	{
		// 国を選択していなかった場合、エラーページへ遷移する
		if (string.IsNullOrEmpty(ddlConditionCountry.SelectedValue))
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_GLOBAL_SHIPPING_AREA_EMPTY_ERROR);
			this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		var model = new GlobalShippingAreaComponentModel
		{
			GlobalShippingAreaId = this.KeepingEditId,
			CountryIsoCode = this.ddlConditionCountry.SelectedValue,
			ConditionAddr5 = this.GetInputValueAddr5Condition(),
			ConditionAddr4 = this.tbConditionAddr4.Text,
			ConditionAddr3 = this.tbConditionAddr3.Text,
			ConditionAddr2 = this.tbConditionAddr2.Text,
			ConditionZip = this.tbConditionZip.Text,
			LastChanged = base.LoginOperatorName
		};
		var sv = new GlobalShippingService();
		sv.RegisterGlobalShippingAreaComponent(model);

		var compone = sv.GetAreaComponentByAreaId(this.KeepingEditId);
		this.repAreaComponent.DataSource = compone;
		this.repAreaComponent.DataBind();
	}
	#endregion

	#region #repAreaComponent_ItemCommand 構成リピータイベント
	/// <summary>
	/// 構成リピータイベント
	/// </summary>
	/// <param name="source">イベントの発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void repAreaComponent_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "delCondition")
		{
			this.DelCondition(e);
		}
	}
	#endregion

	#region #ddlConditionCountry_SelectedIndexChanged 国ドロップダウン変更
	/// <summary>
	/// 国ドロップダウン変更
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void ddlConditionCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		// USだったらStateドロップダウン表示、address5テキスト非表示
		// US以外はStateドロップダウン非表示、address5テキスト表示

		if (UseStateddl())
		{
			this.tbConditionAddr5.Visible = false;
			this.ddlUsState.Visible = true;
			this.StateddlSetData();
		}
		else
		{
			this.tbConditionAddr5.Visible = true;
			this.ddlUsState.Visible = false;
		}

		var sv = new GlobalShippingService();
		var compone = sv.GetAreaComponentByAreaId(this.KeepingEditId);
		this.repAreaComponent.DataSource = compone;
		this.repAreaComponent.DataBind();
	}
	#endregion

	#region -InitializeComponents コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	private void InitializeComponents(string actionStatus)
	{
		if (actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			// 新規登録
			btnInsertTop.Visible = true;
			trDetailTop.Visible = false;
			trConfirmTop.Visible = true;
			btnHistoryBackTop.Visible = true;
			btnBackToListTop.Visible = false;

		}
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			// 更新
			btnUpdateTop.Visible = true;
			trDetailTop.Visible = false;
			trConfirmTop.Visible = true;
			btnHistoryBackTop.Visible = true;
			btnBackToListTop.Visible = false;

		}
		else if (actionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			// 詳細
			btnEditTop.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetailTop.Visible = true;
			btnHistoryBackTop.Visible = false;
			btnBackToListTop.Visible = true;
		}
	}
	#endregion
	
	/// <summary>
	/// 構成条件削除
	/// </summary>
	/// <param name="e">リピーターイベント引数</param>
	private void DelCondition(RepeaterCommandEventArgs e)
	{
		var delSeq = int.Parse(e.CommandArgument.ToString());
		var sv = new GlobalShippingService();
		sv.DeleteGlobalShippingAreaComponent(delSeq);

		var compone = sv.GetAreaComponentByAreaId(this.KeepingEditId);
		this.repAreaComponent.DataSource = compone;
		this.repAreaComponent.DataBind();
	}

	/// <summary>
	/// State用のドロップダウンを使うかどうか
	/// </summary>
	/// <returns>True：使う、False：使わない</returns>
	protected virtual bool UseStateddl()
	{
		if (GlobalAddressUtil.IsCountryUs(this.ddlConditionCountry.SelectedValue)) { return true; }
		return false;
	}

	/// <summary>
	/// /入力している住所5条件取得
	/// </summary>
	/// <returns>入力している値</returns>
	protected virtual string GetInputValueAddr5Condition()
	{
		return UseStateddl()
			? this.ddlUsState.SelectedValue
			: this.tbConditionAddr5.Text;
	}

	/// <summary>
	/// State用のドロップダウンリストのデータセット＆バインド
	/// </summary>
	protected virtual void StateddlSetData()
	{
		var stateList = Constants.US_STATES_LIST.Select(s => new ListItem(s, s)).ToList();
		stateList.Insert(0, new ListItem("", ""));
		ddlUsState.DataSource = stateList;
		ddlUsState.DataBind();
	}

	/// <summary>
	/// 編集する海外エリアID（ViewSatteで保持）
	/// </summary>
	protected string KeepingEditId
	{
		get { return StringUtility.ToEmpty(ViewState["edit_id"]); }
		set { ViewState["edit_id"] = value; }
	}

	/// <summary>
	/// アクションステータス（ViewStateで保持）
	/// </summary>
	protected string KeepintActionStatus
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_ACTION_STATUS]); }
		set { ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = value; }
	}
}
