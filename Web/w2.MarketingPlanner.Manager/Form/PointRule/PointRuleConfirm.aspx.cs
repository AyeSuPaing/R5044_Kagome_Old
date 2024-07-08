/*
=========================================================================================================
  Module      : ポイント基本ルール確認ページ処理(PointRuleConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using Input.Point;
using w2.App.Common.RefreshFileManager;
using w2.Common.Util;
using w2.Domain.Point;

public partial class Form_PointRule_PointRuleConfirm : BasePage
{
	#region #Page_Load ページロード
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
			// 画面設定処理
			//------------------------------------------------------
			// 登録・コピー登録・更新画面確認？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_COPY_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_UPDATE
				)
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				Input = MpSessionWrapper.PointRuleInput;
			}
			// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{

				// ポイントルールID取得
				string pointRuleId = Request[Constants.REQUEST_KEY_POINTRULE_ID];
				var sv = new PointService();
				var model = sv.GetPointRule(this.LoginOperatorDeptId, pointRuleId);
				if (model == null)
				{
					// 該当データ無しの場合
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
				}

				Input = new PointRuleInput(model);
			}
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}

			// 画面制御
			InitializeComponents(strActionStatus);

			// 購入時ポイント発行の場合は表示
			trIncType.Visible = (Input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY);

			// URLクリックポイント発行のURL作成
			if ((Input.IsClickPoint) && strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				this.Url = Input.ClikPointUrlCreator();
				trUrl.Visible = (Input.IsClickPoint);
			}
			// データバインド
			DataBind();
		}
	}
	#endregion

	#region -InitializeComponents コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規・コピー新規？
		if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
			strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 更新？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 詳細
		else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			if (new PointService().GetPointRuleScheduleByPointRuleId(Request[Constants.REQUEST_KEY_POINTRULE_ID]).Length == 0)
			{
				btnDeleteTop.Visible = true;
				btnDeleteBottom.Visible = true;
			}
			trPointRuleId.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetail.Visible = true;
		}

		// ポイント有効期限延長
		trPointExpEntend.Visible = GetPointExpKbn(Constants.FLG_USERPOINT_POINT_KBN_BASE);
		trPointExpire.Visible = (this.Input.UseScheduleIncKbnPointRule == false);
	}
	#endregion

	#region -GetPointExpKbn ポイント有効期限設定の有無
	/// <summary>
	/// ポイント有効期限設定の有無
	/// </summary>
	/// <param name="pointKbn">ポイント区分</param>
	/// <returns>ポイント有効期限設定の有無(True:有 False:無)</returns>
	private bool GetPointExpKbn(string pointKbn)
	{
		// 変数宣言
		bool blResult = true;

		var sv = new PointService();
		var res = sv.GetPointMaster();
		var point = res.FirstOrDefault(i => i.DeptId == this.LoginOperatorDeptId && i.PointKbn == pointKbn);

		if (point != null)
		{
			// ポイント有効期限設定取得
			blResult = point.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID;
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPPOINT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		return blResult;
	}
	#endregion

	#region #btnEditTop_Click 編集ボタンクリック
	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEditTop_Click(object sender, System.EventArgs e)
	{
		// ポイント基本ルール情報をそのままセッションへセット
		MpSessionWrapper.PointRuleInput = Input;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}
	#endregion

	#region #btnCopyInsertTop_Click コピー新規登録するボタンクリック
	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsertTop_Click(object sender, System.EventArgs e)
	{
		// コピー新規の場合はIDを空にしてセッションへセット
		Input.PointRuleId = string.Empty;
		MpSessionWrapper.PointRuleInput = Input;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}
	#endregion

	#region #btnDeleteTop_Click 削除するボタンクリック
	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteTop_Click(object sender, System.EventArgs e)
	{
		var sv = new PointService();
		sv.DeletePointRule(Input.CreateModel());

		// 各サイトのポイントルール情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.PointRules).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_LIST);
	}
	#endregion

	#region #btnInsertTop_Click 登録するボタンクリック
	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertTop_Click(object sender, System.EventArgs e)
	{
		// 入力値を元にポイント登録
		var sv = new PointService();
		Input.PointRuleId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_MP_POINTRULE_ID, 10);
		sv.RegisterPointRule(Input.CreateModel());

		// 各サイトのポイントルール情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.PointRules).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_LIST);
	}
	#endregion

	#region #btnUpdateTop_Click 更新ボタンクリック
	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTop_Click(object sender, System.EventArgs e)
	{
		// 入力値を元にポイント更新
		var sv = new PointService();
		sv.UpdatePointRule(Input.CreateModel());

		// 各サイトのポイントルール情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.PointRules).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_LIST);
	}
	#endregion

	/// <summary> ポイントルール入力値クラス</summary>
	protected PointRuleInput Input
	{
		get { return (PointRuleInput)ViewState[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT]; }
		set { ViewState[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT] = value; }
	}
	//クリックポイント発行URL
	protected string Url { get; set; }
}
