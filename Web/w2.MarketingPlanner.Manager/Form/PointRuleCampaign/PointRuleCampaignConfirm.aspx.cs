/*
=========================================================================================================
  Module      : ポイントキャンペーンルール確認ページ処理(PointRuleCampaignConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI;
using Input.Point;
using w2.App.Common.RefreshFileManager;
using w2.Domain.Point;
using w2.Common.Util;
using Calendar = System.Web.UI.WebControls.Calendar;

public partial class Form_PointRuleCampaign_PointRuleCampaignConfirm : BasePage
{
	//=========================================================================================
	// カレンダー用定数
	//=========================================================================================
	protected const string CAMPAIGN_CALENDAR_DATE = "campaign_calendar_date";
	protected const string CAMPAIGN_CALENDAR_DATE_NO = "campaign_calendar_date_no";

	protected ArrayList m_alCampaign = new ArrayList();					// カレンダーリピータデータバインド用

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// 指定した言語ロケールIDにより、カルチャーを変換する
		if (Constants.GLOBAL_OPTION_ENABLE && string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
		}

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
			InitializeComponents( strActionStatus );

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・コピー登録・更新画面確認？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT
				|| strActionStatus == Constants.ACTION_STATUS_COPY_INSERT
				|| strActionStatus == Constants.ACTION_STATUS_UPDATE
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
				string strPointRuleId = Request[Constants.REQUEST_KEY_POINTRULE_ID];
				var sv = new PointService();
				var model = sv.GetPointRule(this.LoginOperatorDeptId, strPointRuleId);

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

			// データバインド
			DataBind();

			// カレンダー描画
			CreateCampaignCalendar(DateTime.Parse(Input.ExpBgn), DateTime.Parse(Input.ExpEnd));
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
		else if(strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
		}
			// 詳細
		else if(strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
			trPointRuleId.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetail.Visible = true;
		}

		// ポイント有効期限延長
		trPointExpEntend.Visible = GetPointExpKbn(Constants.FLG_USERPOINT_POINT_KBN_BASE);
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

		if(point != null)
		{
			// ポイント有効期限設定取得
			blResult = point.PointExpKbn  == Constants.FLG_POINT_POINT_EXP_KBN_VALID;
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

	#region -CreateCampaignCalendar カレンダー再描画
	/// <summary>
	/// カレンダー再描画
	/// </summary>
	/// <param name="dtExpBgnTmp">キャンペーン有効期間（開始）</param>
	/// <param name="dtExpEndTmp">キャンペーン有効期間（終了）</param>
	private void CreateCampaignCalendar(DateTime dtExpBgnTmp, DateTime dtExpEndTmp)
	{
		// 変数宣言
		int iCalendarNo = 1;

		// キャンペーン有効期間取得
		DateTime dtExpBgn = DateTime.Parse(dtExpBgnTmp.ToString("yyyy/MM/01"));
		DateTime dtExpEnd = DateTime.Parse(dtExpEndTmp.ToString("yyyy/MM/01"));

		// キャンペーン有効期間(開始) < キャンペーン有効期間(終了)ループ
		while (dtExpBgn <= dtExpEnd)
		{
			Hashtable htInput = new Hashtable();

			htInput.Add(CAMPAIGN_CALENDAR_DATE, 
				DateTime.Parse(dtExpBgn.Year.ToString() + "/" + dtExpBgn.Month.ToString() + "/01"));	// カレンダー日付
			htInput.Add(CAMPAIGN_CALENDAR_DATE_NO, iCalendarNo);										// カレンダーNo

			// カレンダー情報を格納
			m_alCampaign.Add(htInput);

			dtExpBgn = dtExpBgn.AddMonths(1);	// 1月単位で追加
			iCalendarNo++;
		}

		// データバインド
		rCampaign.DataBind();
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
		// ポイントキャンペーンルール情報をそのままセッションへセット
		MpSessionWrapper.PointRuleInput = Input;
		
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_REGISTER + "?" + 
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
		// ポイントキャンペーンルール情報をそのままセッションへセット
		MpSessionWrapper.PointRuleInput = Input;
		
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_REGISTER + "?" + 
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
		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_LIST );
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
		var sv = new PointService();
		Input.PointRuleId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_MP_POINTRULE_ID, 10);
		sv.RegisterPointRule(Input.CreateModel());
		
		// 各サイトのポイントルール情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.PointRules).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_LIST );
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
		var sv = new PointService();
		sv.UpdatePointRule(Input.CreateModel());

		// 各サイトのポイントルール情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.PointRules).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_LIST);
	}
	#endregion

	#region +cldCampaign1_DayRender カレンダー日付描画毎に呼び出されるイベント
	/// <summary>
	/// カレンダー日付描画毎に呼び出されるイベント 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cldCampaign1_DayRender(object sender, System.Web.UI.WebControls.DayRenderEventArgs e)
	{
		// 変数宣言
		string strDay = String.Empty;

		// カレンダー選択情報取得
		var alCalendarInfo = Input.RuleDate.Select(i => DateTime.Parse(i.TgtDate)).ToArray();
		
		// カレンダー取得
		Calendar cldCampaign = (Calendar)sender;
		
		strDay = ((LiteralControl)e.Cell.Controls[0]).Text;
		LiteralControl hcDay = new LiteralControl(strDay);
		// 同じ月の場合
		e.Cell.BackColor = Color.FromArgb(0xee, 0xee, 0xee);
		if (cldCampaign.VisibleDate.Month == e.Day.Date.Month)
		{
			// カレンダー選択情報に対象セルの日付が含まれている場合
			if (alCalendarInfo.Contains(e.Day.Date))
			{
				e.Cell.BackColor = Color.FromArgb(0xbe, 0xd2, 0xff);
				e.Cell.Font.Bold = true;
				e.Cell.ForeColor = Color.FromArgb(0x08, 0x64, 0xAA);
			}
		}
		e.Cell.Controls.Clear();
		e.Cell.Controls.Add(hcDay);
	}
	#endregion

	/// <summary> ポイントルール入力値クラス</summary>
	protected PointRuleInput Input
	{
		get { return (PointRuleInput)ViewState[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT]; }
		set { ViewState[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT] = value; }
	}
}
