/*
=========================================================================================================
  Module      : クーポン設定一覧ページ処理(CouponList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.Common.Web;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;

public partial class Form_Coupon_CouponList : CouponPage
{
	// 定数
	public const string FIELD_COUPON_PUBLISH_DATE = "publish_date";

	public const string FLG_COUPON_PUBLISH_DATE_BEFORE = "0";	// 発行期間前
	public const string FLG_COUPON_PUBLISH_DATE_TERM = "1";		// 発行期間中
	public const string FLG_COUPON_PUBLISH_DATE_AFTER = "2";	// 発行期間後

	#region -Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			// 登録系のセッションをクリア
			Session[Constants.SESSIONPARAM_KEY_COUPON_INFO] = null;

			// クーポン情報一覧表示
			ViewCouponList();

			// クーポン検索情報をセッションへ格納
			Session[Constants.SESSIONPARAM_KEY_COUPON_SEARCH_INFO] = GetSearchInfo();
		}
	}
	#endregion

	#region -ViewCouponList クーポン情報一覧表示(DataGridにDataView(クーポン情報)を設定)
	/// <summary>
	/// クーポン情報一覧表示(DataGridにDataView(クーポン情報)を設定)
	/// </summary>
	private void ViewCouponList()
	{
		// 変数宣言
		Hashtable paramList = new Hashtable();

		//------------------------------------------------------
		// 画面制御
		//------------------------------------------------------
		InitializeComponents();

		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		paramList = GetParameters(Request);
		// 不正パラメータが存在した場合
		if ((bool)paramList[Constants.ERROR_REQUEST_PRAMETER])
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		var searchCond = GetSearchSqlCond(paramList);
		int currentPageNumber = (int)paramList[Constants.REQUEST_KEY_PAGE_NO];

		if (this.IsNotSearchDefault) return;

		//------------------------------------------------------
		// クーポン一覧
		//------------------------------------------------------
		int totalCouponCounts = 0;	// ページング可能総商品数
		// クーポンデータ取得
		var couponList = GetCouponList(searchCond, currentPageNumber);
		if (couponList != null && couponList.Length > 0)
		{
			totalCouponCounts = couponList[0].RowCount;
			// エラー非表示制御
			trListError.Visible = false;
		}
		else
		{
			totalCouponCounts = 0;
			// 一覧非表示・エラー表示制御
			trListError.Visible = true;

			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		// データソースセット
		rList.DataSource = couponList;
		this.CouponIdListOfDisplayedData = couponList.Select(c => c.CouponId).ToArray();

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string nextUrl = CreateCouponListUrl(paramList);
		lbPager.Text = WebPager.CreateDefaultListPager(totalCouponCounts, currentPageNumber, nextUrl);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}
	#endregion

	#region -InitializeComponents コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// クーポン種別
		ddlCouponType.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_COUPON, Constants.FIELD_COUPON_COUPON_TYPE))
		{
			if ((Constants.INTRODUCTION_COUPON_OPTION_ENABLED == false)
				&& ((li.Value == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON)
					|| (li.Value == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON)
					|| (li.Value == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION)))
			{
				continue;
			}

			ddlCouponType.Items.Add(li);
		}

		// 発行ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_COUPON, FIELD_COUPON_PUBLISH_DATE))
		{
			rblPublishDate.Items.Add(li);
		}

		// 有効フラグ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_COUPON, Constants.FIELD_COUPON_VALID_FLG))
		{
			rblValidFlg.Items.Add(li);
		}
	}
	#endregion

	#region #GetParameters クーポン一覧パラメタ取得
	/// <summary>
	/// クーポン一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">クーポン一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	protected Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable result = new Hashtable();
		string sortKbn = String.Empty;
		int currentPageNumber = 1;
		bool paramError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// クーポンコード
		try
		{
			result.Add(Constants.REQUEST_KEY_COUPON_COUPON_CODE, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COUPON_COUPON_CODE]));
			tbCouponCode.Text = hrRequest[Constants.REQUEST_KEY_COUPON_COUPON_CODE];
		}
		catch
		{
			paramError = true;
		}
		// クーポン種別
		try
		{
			result.Add(Constants.REQUEST_KEY_COUPON_COUPON_TYPE, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COUPON_COUPON_TYPE]));
			foreach (ListItem li in ddlCouponType.Items)
			{
				li.Selected = (li.Value == hrRequest[Constants.REQUEST_KEY_COUPON_COUPON_TYPE]);
			}
		}
		catch
		{
			paramError = true;
		}
		// 管理用クーポン名
		try
		{
			result.Add(Constants.REQUEST_KEY_COUPON_COUPON_NAME, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COUPON_COUPON_NAME]));
			tbCouponName.Text = hrRequest[Constants.REQUEST_KEY_COUPON_COUPON_NAME];
		}
		catch
		{
			paramError = true;
		}
		// 発行ステータス
		try
		{
			result.Add(Constants.REQUEST_KEY_COUPON_PUBLISH_DATE, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COUPON_PUBLISH_DATE]));
			foreach (ListItem li in rblPublishDate.Items)
			{
				li.Selected = (li.Value == hrRequest[Constants.REQUEST_KEY_COUPON_PUBLISH_DATE]);
			}
			rblPublishDate.SelectedIndex = (rblPublishDate.SelectedIndex != -1) ? rblPublishDate.SelectedIndex : 0;
		}
		catch
		{
			paramError = true;
		}
		// 有効フラグ
		try
		{
			result.Add(Constants.REQUEST_KEY_COUPON_VALID_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COUPON_VALID_FLG]));
			foreach (ListItem li in rblValidFlg.Items)
			{
				li.Selected = (li.Value == hrRequest[Constants.REQUEST_KEY_COUPON_VALID_FLG]);
			}
			rblValidFlg.SelectedIndex = (rblValidFlg.SelectedIndex != -1) ? rblValidFlg.SelectedIndex : 0;
		}
		catch
		{
			paramError = true;
		}
		// ソート区分
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_COUPON_LIST_DATE_CREATED_ASC:			// 作成日/昇順
				case Constants.KBN_SORT_COUPON_LIST_DATE_CREATED_DESC:			// 作成日/降順
				case Constants.KBN_SORT_COUPON_LIST_DATE_CHANGED_ASC:			// 更新日/昇順
				case Constants.KBN_SORT_COUPON_LIST_DATE_CHANGED_DESC:			// 更新日/降順
					sortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					sortKbn = Constants.KBN_SORT_COUPON_LIST_DEFAULT;		// 更新日/降順がデフォルト
					break;
				default:
					paramError = true;
					break;
			}
			result.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);
			foreach (ListItem li in ddlSortKbn.Items)
			{
				li.Selected = (li.Value == sortKbn);
			}
		}
		catch
		{
			paramError = true;
		}
		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				currentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			paramError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		result.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);
		result.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

		return result;
	}
	#endregion

	#region -CouponListSearchCondition 検索値取得
	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="searchParams">検索情報</param>
	/// <returns>検索条件情報</returns>
	private CouponListSearchCondition GetSearchSqlCond(Hashtable searchParams)
	{
		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		return new CouponListSearchCondition()
		{
			DeptId = this.LoginOperatorDeptId,
			// クーポン種別
			CouponType = StringUtility.ToEmpty(searchParams[Constants.REQUEST_KEY_COUPON_COUPON_TYPE]),
			// クーポンコード
			CouponCode = StringUtility.SqlLikeStringSharpEscape(searchParams[Constants.REQUEST_KEY_COUPON_COUPON_CODE]),
			// 管理用クーポン名
			CouponName = StringUtility.SqlLikeStringSharpEscape(searchParams[Constants.REQUEST_KEY_COUPON_COUPON_NAME]),
			// 発行ステータス
			PublishDate = StringUtility.ToEmpty(searchParams[Constants.REQUEST_KEY_COUPON_PUBLISH_DATE]),
			// 有効フラグ
			ValidFlg = StringUtility.ToEmpty(searchParams[Constants.REQUEST_KEY_COUPON_VALID_FLG]),
			// ソート区分
			SortKbn = StringUtility.ToEmpty(searchParams[Constants.REQUEST_KEY_SORT_KBN])
		};
	}
	#endregion

	#region -GetSearchInfo 各検索コントロールから検索情報取得
	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfo()
	{
		// 変数宣言
		Hashtable searchInfo = new Hashtable();

		searchInfo.Add(Constants.REQUEST_KEY_COUPON_COUPON_CODE, tbCouponCode.Text.Trim());							// クーポンコード
		searchInfo.Add(Constants.REQUEST_KEY_COUPON_COUPON_TYPE, ddlCouponType.SelectedValue);						// クーポン種別
		searchInfo.Add(Constants.REQUEST_KEY_COUPON_COUPON_NAME, tbCouponName.Text.Trim());							// 管理用クーポン名
		searchInfo.Add(Constants.REQUEST_KEY_COUPON_PUBLISH_DATE, rblPublishDate.SelectedValue);						// 発行ステータス
		searchInfo.Add(Constants.REQUEST_KEY_COUPON_VALID_FLG, rblValidFlg.SelectedValue);							// 有効フラグ
		searchInfo.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);										// ソート区分

		return searchInfo;
	}
	#endregion

	#region -GetCouponList クーポン一覧データビューを表示分だけ取得
	/// <summary>
	/// クーポン一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="searchCond">検索条件情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <returns>クーポン情報一覧</returns>
	private CouponListSearchResult[] GetCouponList(CouponListSearchCondition searchCond, int pageNumber)
	{
		// 開始行～終了行の検索条件を設定
		int begin = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNumber - 1) + 1;
		int end = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNumber;

		searchCond.BgnRowNumber = begin;
		searchCond.EndRowNumber = end;

		// クーポン情報取得
		return new CouponService().SearchCouponList(searchCond);
	}
	#endregion

	#region -CreateCouponListUrl クーポン一覧遷移URL作成
	/// <summary>
	/// クーポン一覧遷移URL作成
	/// </summary>
	/// <param name="searchParams">検索情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <returns>クーポン一覧遷移URL</returns>
	private string CreateCouponListUrl(Hashtable searchParams, int pageNumber)
	{
		StringBuilder url = new StringBuilder();

		url.Append(CreateCouponListUrl(searchParams));
		url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(pageNumber.ToString());

		return url.ToString();
	}
	#endregion

	#region -CreateCouponUseUserListUrl クーポン利用ユーザー一覧画面遷移URL作成
	/// <summary>
	/// クーポン利用ユーザー一覧画面遷移URL作成
	/// </summary>
	/// <param name="couponId"></param>
	/// <returns></returns>
	protected string CreateCouponUseUserListUrl(string couponId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_COUPONUSEUSERLIST)
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID, couponId)
			.CreateUrl();
		return url;
	}
	#endregion

	#region -IsBlacklistCoupon ブラックリスト型クーポンか
	/// <summary>
	/// ブラックリスト型クーポンか
	/// </summary>
	/// <param name="couponType">クーポン種別</param>
	/// <returns>ブラックリスト型クーポンならtrue</returns>
	protected bool IsBlacklistCoupon(string couponType)
	{
		return CouponOptionUtility.IsBlacklistCoupon(couponType);
	}
	#endregion

	#region #DisplayPublishDate クーポン発行期間取得
	/// <summary>
	/// クーポン発行期間取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン発行期間</returns>
	protected string DisplayPublishDate(CouponListSearchResult coupon)
	{
		string result = null;
		DateTime dtNow = DateTime.Now;

		// 日付が格納されている場合
		if (coupon.PublishDateBgn != null && coupon.PublishDateEnd != null)
		{
			// 発行期間前
			if ((DateTime)coupon.PublishDateBgn > dtNow)
			{
				// 「発行期間前」
				result = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_COUPON_LIST,
					Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
					Constants.VALUETEXT_PARAM_COUPON_LIST_BEFORE_ISSUE_PERIOD);
			}
			// 発行期間中
			else if (coupon.PublishDateBgn <= dtNow && coupon.PublishDateEnd >= dtNow)
			{
				// 「発行期間中」
				result = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_COUPON_LIST,
					Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
					Constants.VALUETEXT_PARAM_COUPON_LIST_DURING_ISSUE_PERIOD);
			}
			// 発行期間後
			else if (coupon.PublishDateEnd < dtNow)
			{
				// 「発行期間後」
				result = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_COUPON_LIST,
					Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
					Constants.VALUETEXT_PARAM_COUPON_LIST_AFTER_ISSUE_PERIOD);
			}
		}

		return StringUtility.ToEmpty(result);
	}
	#endregion

	#region #DisplayDiscount クーポン割引(割引額 OR 割引率)取得
	/// <summary>
	/// クーポン割引(割引額 OR 割引率)取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン割引</returns>
	protected string DisplayDiscount(CouponListSearchResult coupon)
	{
		// 配送料無料
		var freeShippingMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING);
		var discountPriceMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE);

		// クーポン割引額に値がある場合
		if (coupon.DiscountPrice != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + coupon.DiscountPrice.ToPriceString(true);
			return coupon.DiscountPrice.ToPriceString(true);
		}
		// クーポン割引率に値がある場合
		if (coupon.DiscountRate != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(coupon.DiscountRate) + "%";
			return StringUtility.ToNumeric(coupon.DiscountRate) + "%";
		}
		if (CouponOptionUtility.IsFreeShipping(coupon.CouponType) || (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
		{
			return freeShippingMessage;
		}

		return "-";
	}
	#endregion

	#region #DisplayExpire クーポン有効期限・期間取得
	/// <summary>
	/// クーポン有効期限・期間取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン有効期限・期間</returns>
	protected string DisplayExpire(CouponListSearchResult coupon)
	{
		string result = null;

		// クーポン有効期限に値がある場合
		if (coupon.ExpireDay != null)
		{
			result = string.Format(
				// 「発行から {0} 日」
				ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_EXPIRE_DAY, "ExpireDay"),
				coupon.ExpireDay);
		}
		// クーポン有効期間に値がある場合
		else if (coupon.ExpireDateBgn != null)
		{
			result = string.Format(
				"{0}～{1}",
				DateTimeUtility.ToStringForManager(
					coupon.ExpireDateBgn,
					DateTimeUtility.FormatType.ShortDate2Letter),
				DateTimeUtility.ToStringForManager(
					coupon.ExpireDateEnd,
					DateTimeUtility.FormatType.ShortDate2Letter));
		}

		return StringUtility.ToEmpty(result);
	}
	#endregion

	#region #btnSearch_Click 検索ボタンクリック
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// クーポン一覧へ
		var url = CreateCouponListUrl(GetSearchInfo(), 1);
		Response.Redirect((url));
	}
	#endregion

	#region #btnInsertTop_Click 新規ボタンクリック
	/// <summary>
	/// 新規ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertTop_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
	#endregion

	#region #DisplayCouponCount クーポン利用可能回数の表示
	/// <summary>
	/// クーポン利用可能回数の表示
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン利用可能回数</returns>
	protected string DisplayCouponCount(CouponListSearchResult coupon)
	{
		string result = null;

		// 回数制限のある場合
		if (CouponOptionUtility.IsCouponAllLimit(coupon.CouponType))
		{
			result = StringUtility.ToNumeric(coupon.CouponCount);
		}
		else
		{
			result = "－";
		}

		return result;
	}
	#endregion

	#region #CreateSearchParams クーポンマスタ出力用の検索ハッシュテーブル生成
	/// <summary>
	/// クーポンマスタ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>クーポンマスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	private Hashtable CreateSearchParams()
	{
		var searchCond = GetSearchSqlCond(GetParameters(Request)).CreateHashtableParams();
		return searchCond;
	}
	#endregion

	#region #lbExportTranslationData_OnClick 翻訳データ出力リンククリック
	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_OnClick(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = this.CouponIdListOfDisplayedData;
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
		
	}
	#endregion

	/// <summary>画面表示されているクーポンIDリスト</summary>
	private string[] CouponIdListOfDisplayedData
	{
		get { return (string[])ViewState["couponid_list_of_displayed_data"]; }
		set { ViewState["couponid_list_of_displayed_data"] = value; }
	}
}
