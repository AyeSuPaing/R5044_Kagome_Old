/*
=========================================================================================================
  Module      : クーポン推移レポート一覧ページ処理(CouponTransitionReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Web.UI.WebControls;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;

public partial class Form_CouponTransitionReport_CouponTransitionReportList : BasePage
{
	protected int m_currentYear = DateTime.Now.Year;
	protected int m_currentMonth = DateTime.Now.Month;

	const string REQUEST_KEY_DATE_TYPE = "dtype";
	const string REQUEST_KEY_COUPON_TYPE = "ctype";
	const string REQUEST_KEY_COUPON_ID = "cpid";
	const string REQUEST_KEY_COUPON_CODE = "cpcd";

	protected const string KBN_DISP_MONTHLY_REPORT = "1";
	protected const string KBN_DISP_DAILY_REPORT = "0";

	// クーポンドロップダウン表示
	private bool m_displayCouponDropdown = true;

	// クーポン検索ボックス表示切替閾値
	private const int COUPON_DROPDOWN_MAX = 30;

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// 年月取得
		try
		{
			if (ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] != null)
			{
				m_currentYear = (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR];
				m_currentMonth = (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH];
			}
			else
			{
				m_currentYear = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_YEAR]);
				m_currentMonth = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_MONTH]);
			}
		}
		catch
		{
			m_currentYear = DateTime.Now.Year;
			m_currentMonth = DateTime.Now.Month;
		}
		// 年月はビューステートへ格納
		ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_currentYear;
		ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_currentMonth;

		if (!IsPostBack)
		{
			// ラジオボタン選択状態取得・設定
			try
			{
				int checkedFlg = int.Parse(Request[REQUEST_KEY_DATE_TYPE]);
				foreach (ListItem li in rblReportType.Items)
				{
					li.Selected = (li.Value == checkedFlg.ToString());
				}
			}
			catch
			{
			}

			tbCouponId.Text = StringUtility.ToEmpty(Request[REQUEST_KEY_COUPON_CODE]);
			// 全てクーポン情報を取得
			CouponService couponService = new CouponService();
			var couponCount = couponService.GetAllCouponCount(this.LoginOperatorDeptId);

			// 存在しない場合クーポン指定をできないようにする
			if (couponCount == 0)
			{
				m_displayCouponDropdown = true;

				rbAllCoupon.Checked = true;
				rbTargetCoupon.Enabled = true;
				ddlCoupon.Enabled = true;

				tbCouponId.Visible = false;
				btnSearchCoupon.Visible = false;
			}
			// 一定数以下はクーポンドロップダウン表示
			else if ((couponCount >= 1) && (couponCount <= COUPON_DROPDOWN_MAX))
			{
				m_displayCouponDropdown = true;

				var couponInfoList = couponService.GetAllCoupons(this.LoginOperatorDeptId);
				if (couponInfoList != null && couponInfoList.Length > 0)
				{
					ddlCoupon.Items.Add(new ListItem ("",""));
					// 発行可能クーポン設定
					foreach (CouponModel coupon in couponInfoList)
					{
						ListItem li = new ListItem("[" + coupon.CouponCode + "]" + coupon.CouponName, coupon.CouponId);
						if (StringUtility.ToEmpty(Request[REQUEST_KEY_COUPON_ID]) == coupon.CouponId)
						{
							li.Selected = true;
						}
						ddlCoupon.Items.Add(li);
					}
				}
			}
			// 一定数を超えたらドロップダウン表示しない
			else
			{
				m_displayCouponDropdown = false;

				ddlCoupon.Visible = false;

			}

			// クーポン指定状態取得
			if (StringUtility.ToEmpty(Request[REQUEST_KEY_COUPON_TYPE]) == "select")
			{
				rbTargetCoupon.Checked = true;
			}
			else
			{
				rbAllCoupon.Checked = true;
			}

			ViewState["DisplayCouponDropdown"] = m_displayCouponDropdown;
		}
		else
		{
			m_displayCouponDropdown = (bool)ViewState["DisplayCouponDropdown"];

			divDaily.Visible = false;
			divMonthly.Visible = false;
			divNoHitCoupon.Visible = false;
			divOneHitCoupon.Visible = false;
			divMoreThanOneHitCoupon.Visible = false;
		}

		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		bool couponHitFlg = Initialize();

		//------------------------------------------------------
		// 集計データ取得＆データバインド
		//------------------------------------------------------
		if (couponHitFlg)
		{
			rDataList.DataSource = GetData();

			// エラー非表示
			trListError.Visible = false;
		}
		else
		{
			rDataList.DataSource = null;

			// エラー表示
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		rDataList.DataBind();
	}
	#endregion

	#region -Initialize コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <returns>該当クーポンありなし(全てのクーポンの場合はあり）</returns>
	private bool Initialize()
	{
		//------------------------------------------------------
		// カレンダ設定
		//------------------------------------------------------
		var paramForCurrent = REQUEST_KEY_DATE_TYPE + "=" + rblReportType.SelectedValue;
		// 特定クーポンの場合
		if (rbTargetCoupon.Checked)
		{
			paramForCurrent += "&" + REQUEST_KEY_COUPON_TYPE + "=select";
			paramForCurrent += "&" + REQUEST_KEY_COUPON_ID + "=" + HttpUtility.UrlEncode(ddlCoupon.SelectedValue);
			paramForCurrent += "&" + REQUEST_KEY_COUPON_CODE + "=" + HttpUtility.UrlEncode(tbCouponId.Text.Trim());
		}
		// 基準カレンダ設定
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_currentYear, m_currentMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_TRANSITION_REPORT_LIST, paramForCurrent, Constants.REQUEST_KEY_CURRENT_YEAR, Constants.REQUEST_KEY_CURRENT_MONTH);

		// 表示情報設定
		if ((rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT) && rbAllCoupon.Checked)
		{
			divDaily.Visible = true;
		}
		else if ((rblReportType.SelectedValue == KBN_DISP_MONTHLY_REPORT) && rbAllCoupon.Checked)
		{
			divMonthly.Visible = true;
		}

		//------------------------------------------------------
		// クーポン指定設定
		//------------------------------------------------------
		var couponHitFlg = false;

		// 全てクーポンの場合
		if (rbAllCoupon.Checked)
		{
			couponHitFlg = true;

			ddlCoupon.Enabled = false;
			tbCouponId.Enabled = false;
			btnSearchCoupon.Enabled = false;
		}
		else if (rbTargetCoupon.Checked)
		{
			ddlCoupon.Enabled = true;
			tbCouponId.Enabled = true;
			btnSearchCoupon.Enabled = true;

			var couponId   = ddlCoupon.SelectedValue;
			var couponCode = tbCouponId.Text.Trim();

			// クーポン数取得
			var condition = new CouponListSearchCondition()
			{
				DeptId = this.LoginOperatorDeptId,
				CouponId = couponId,
				CouponCode = couponCode
			};
			var couponService = new CouponService();
			var couponCount = couponService.GetCouponCount(condition);

			if (couponCount == 0)
			{
				couponHitFlg = false;
				divNoHitCoupon.Visible = true;
			}
			else if (couponCount == 1)
			{
				couponHitFlg = true;
				CouponModel[] couponInfo = null;
				if (couponId.Length != 0)
				{
					// クーポン名など取得（クーポンIDを使用）
					couponInfo = new CouponModel[] { couponService.GetCoupon(this.LoginOperatorDeptId, couponId) };
				}
				else
				{
					// クーポン名など取得 (クーポンコードを使用)
					couponInfo = couponService.GetCouponsFromCouponCode(this.LoginOperatorDeptId, couponCode);
				}

				if (couponInfo != null && couponInfo.Length > 0)
				{
					divOneHitCoupon.Visible = true;
					this.HitCouponCode = couponInfo[0].CouponCode;
					this.HitCouponName = couponInfo[0].CouponName;
				}
			}
			else
			{
				couponHitFlg = true;
				divMoreThanOneHitCoupon.Visible = true;
				this.HitCouponCount = StringUtility.ToNumeric(couponCount);
			}
		}

		return couponHitFlg;
	}
	#endregion

	#region -GetData 検索結果データ取得
	/// <summary>
	/// 検索結果データ取得
	/// </summary>
	/// <returns>表示データ</returns>
	private CouponTransitionReportResult[] GetData()
	{
		// 検索条件設定
		var condition = new CouponTransitionReportCondition()
		{
			Year = m_currentYear.ToString(),
			Month = m_currentMonth.ToString(),
			DeptId = this.LoginOperatorDeptId,
			CouponId = rbTargetCoupon.Checked ? ddlCoupon.SelectedValue : "",
			CouponCodeLikeEscaped = rbTargetCoupon.Checked ? StringUtility.SqlLikeStringSharpEscape(tbCouponId.Text.Trim()) : "",
			ReportType = (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT) ? ReportType.Day : ReportType.Month,
			CurrencyLocaleId = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId,
			CurrencyLocaleFormat = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleFormat
		};
		return new CouponService().SearchCouponTransitionReport(condition);
	}
	#endregion

	#region プロパティ
	/// <summary>選択された月</summary>
	protected string CurrentMonth
	{
		get
		{
			return DateTimeUtility.ToStringForManager(
				new DateTime(m_currentYear, m_currentMonth, 1),
				DateTimeUtility.FormatType.LongYearMonth);
		}
	}
	/// <summary>選択された年</summary>
	protected string CurrentYear
	{
		get { return m_currentYear.ToString(); }
	}
	/// <summary>検索されたクーポンコード</summary>
	protected string HitCouponCode { get; set; }
	/// <summary>検索されたクーポン名</summary>
	protected string HitCouponName { get; set; }
	/// <summary>検索されたクーポン数</summary>
	protected string HitCouponCount { get; set; }
	#endregion
}
