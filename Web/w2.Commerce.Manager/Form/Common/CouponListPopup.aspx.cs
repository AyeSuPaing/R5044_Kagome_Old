/*
=========================================================================================================
  Module      : クーポンリストポップアップページ処理 (CouponListPopup.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.Common.Web;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;

public partial class Form_Common_CouponListPopup : BasePage
{
	/// <summary>フィールド名：発行ステータス</summary>
	private const string FIELD_COUPON_PUBLISH_DATE = "publish_date";
	/// <summary>発行期間前</summary>
	private const string FLG_COUPON_PUBLISH_DATE_BEFORE = "0";
	/// <summary>発行期間中</summary>
	private const string FLG_COUPON_PUBLISH_DATE_TERM = "1";
	/// <summary>発行期間後</summary>
	private const string FLG_COUPON_PUBLISH_DATE_AFTER = "2";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponents();

			// 検索パラメータ取得
			var parameters = GetParameters();

			// 検索値を画面にセット
			SetParameters(parameters);

			if (this.IsNotSearchDefault) return;

			// クーポン一覧取得
			var couponList = GetCouponList(parameters);
			if (couponList.Length > 0)
			{
				rCouponList.DataSource = couponList;

				// ページャセット
				lbPager.Text = WebPager.CreateDefaultListPager(couponList[0].RowCount, this.CurrentPageNo, GetUrlByParameters(parameters));
				trListError.Visible = false;
			}
			else
			{
				// 0件だったらエラーメッセージ表示
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				trListError.Visible = true;
			}

			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
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

	/// <summary>
	/// デフォルト検索値以外に検索パラメータが存在するか
	/// </summary>
	private bool HasSearchParameter()
	{
		if (Request.QueryString.AllKeys.Any() == false) return false;

		foreach (var key in Request.QueryString.AllKeys.Where(
			key => key != Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG))
		{
			if (Request.QueryString[key] != string.Empty) return true;
		}

		return false;
	}

	/// <summary>
	/// 検索パラメータ取得
	/// </summary>
	/// <param name="hrRequest">クーポン一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		var parameters = new Hashtable
		{
			// クーポンコード
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE]) },
			// 管理用クーポン名
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME]) },
			// 発行ステータス
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE]) },
			// 有効フラグ
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG]) },
		};

		// 並び順
		switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_COUPON_LIST_DATE_CREATED_ASC:
			case Constants.KBN_SORT_COUPON_LIST_DATE_CREATED_DESC:
			case Constants.KBN_SORT_COUPON_LIST_DATE_CHANGED_ASC:
			case Constants.KBN_SORT_COUPON_LIST_DATE_CHANGED_DESC:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
				break;

			default:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_COUPON_LIST_DEFAULT);
				break;
		}

		// ページ番号
		int pageNo;
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageNo) == false)
		{
			pageNo = 1;
		}
		parameters.Add(Constants.REQUEST_KEY_PAGE_NO, pageNo);
		this.CurrentPageNo = pageNo;

		return parameters;
	}

	/// <summary>
	/// 検索値を画面にセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetParameters(Hashtable parameters)
	{
		tbCouponCode.Text = (string)parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE];
		tbCouponName.Text = (string)parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME];
		ddlSortKbn.SelectedValue = (string)parameters[Constants.REQUEST_KEY_SORT_KBN];
		rblPublishDate.SelectedValue = (string)parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE];
		rblValidFlg.SelectedValue = (string)parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG];
	}

	/// <summary>
	/// クーポン一覧取得
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>クーポン情報一覧</returns>
	private CouponListSearchResult[] GetCouponList(Hashtable parameters)
	{
		var begin = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1;
		var end = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo;

		var searchCondition = new CouponListSearchCondition
		{
			DeptId = this.LoginOperatorDeptId,
			CouponType = string.Empty,
			CouponCode = StringUtility.SqlLikeStringSharpEscape(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE]),
			CouponName = StringUtility.SqlLikeStringSharpEscape(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME]),
			PublishDate = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE]),
			ValidFlg = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG]),
			SortKbn = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SORT_KBN]),
			BgnRowNumber = begin,
			EndRowNumber = end,
		};

		var couponList = new CouponService().SearchCouponList(searchCondition);
		return couponList;
	}

	/// <summary>
	/// 検索ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		var url = GetUrlByParameters(GetSearchInfoFromControl());
		Response.Redirect(url);
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		var htSerch = new Hashtable
		{
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE, tbCouponCode.Text.Trim() },
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME, tbCouponName.Text.Trim() },
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE, rblPublishDate.SelectedValue },
			{ Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG, rblValidFlg.Text.Trim() },
			{ Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue },
		};
		return htSerch;
	}

	/// <summary>
	/// パラメータから検索用URLを作成
	/// </summary>
	/// <param name="parameters">パラメータ</param>
	/// <returns>検索用URL</returns>
	private string GetUrlByParameters(Hashtable parameters)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_COUPON_LIST_POPUP)
			.AddParam(
				Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE]))
			.AddParam(
				Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME]))
			.AddParam(
				Constants.REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE]))
			.AddParam(
				Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG]))
			.AddParam(
				Constants.REQUEST_KEY_SORT_KBN,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SORT_KBN]))
			.CreateUrl();
		return url;
	}

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
			{
				return WebSanitizer.HtmlEncodeChangeToBr(freeShippingMessage + "\n" + discountPriceMessage + coupon.DiscountPrice.ToPriceString(withSymbol: true));
			}
			return coupon.DiscountPrice.ToPriceString(withSymbol: true);
		}
		// クーポン割引率に値がある場合
		if (coupon.DiscountRate != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return WebSanitizer.HtmlEncodeChangeToBr(freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(coupon.DiscountRate) + "%");
			return WebSanitizer.HtmlEncode(StringUtility.ToNumeric(coupon.DiscountRate) + "%");
		}
		if (CouponOptionUtility.IsFreeShipping(coupon.CouponType) || (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
		{
			return WebSanitizer.HtmlEncode(freeShippingMessage);
		}

		return WebSanitizer.HtmlEncode("-");
	}

	/// <summary>
	/// クーポン発行期間取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン発行期間</returns>
	protected string DisplayPublishDate(CouponListSearchResult coupon)
	{
		string result = null;
		var dtNow = DateTime.Now;

		// 発行期間前
		if (coupon.PublishDateBgn > dtNow)
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

		return WebSanitizer.HtmlEncode(StringUtility.ToEmpty(result));
	}

	/// <summary>
	/// クーポン有効期限・期間取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン有効期限・期間</returns>
	protected string DisplayExpire(CouponListSearchResult coupon)
	{
		var result = string.Empty;

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

		return WebSanitizer.HtmlEncode(result);
	}

	/// <summary>ページ番号</summary>
	private int CurrentPageNo { get; set; }
	/// <summary>デフォルトの検索結果を表示しないか</summary>
	protected override bool IsNotSearchDefault
	{
		get { return (HasSearchParameter() == false) && (Constants.DISPLAY_NOT_SEARCH_DEFAULT); }
	}
}
