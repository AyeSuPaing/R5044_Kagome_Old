/*
=========================================================================================================
  Module      : 頒布会コース設定ページ処理(SubscriptionBoxList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web;
using w2.Domain.SubscriptionBox;

public partial class Form_SubscriptionBox_SubscriptionBoxList : BasePage
{
	private string m_strSortKbn = null;			// Get type of subscription to sort

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// 登録系のセッションをクリア
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = null;
			Session[Constants.SESSIONPARAM_KEY_PRODUCTVARIATION_INFO] = null;
			Session[Constants.SESSIONPARAM_KEY_PRODUCTEXTEND_INFO] = null;
			Session[Constants.SESSIONPARAM_KEY_PRODUCTMALLEXHIBITS_INFO] = null;
			var service = new SubscriptionBoxService();
			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParameters();

			if (this.IsNotSearchDefault) return;

			// 検索条件をセッションに保存
			Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO] = htParam;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable htSqlParam = GetSearchSqlInfo(htParam);

			//------------------------------------------------------
			// 商品該当件数取得
			//------------------------------------------------------
			int totalProductCount = 0;	// ページング可能総商品数

			var total = service.GetCount(htSqlParam);
			totalProductCount = (int)total;

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool displayPager = true;
			StringBuilder errorMessage = new StringBuilder();
			// 上限件数より多い？
			if (totalProductCount > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				errorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(totalProductCount));
				errorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				displayPager = false;
			}
			// 該当件数なし？
			else if (totalProductCount == 0)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = errorMessage.ToString();
			trListError.Visible = (errorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// 商品一覧情報表示
			//------------------------------------------------------
			DataView productList = null;
			if (trListError.Visible == false)
			{
				htSqlParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1);
				htSqlParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);
				productList = service.SearchSubscriptionBoxesAtDataView(htSqlParam);
				// データバインド
				rList.DataSource = productList;
				rList.DataBind();
			}

			this.ProductIdListOfDisplayedData = (productList != null)
				? productList.Cast<DataRowView>().Select(drv => (string)drv[Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID]).ToArray()
				: new string[0];

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (displayPager)
			{
				lbPager1.Text = WebPager.CreateDefaultListPager(
					totalProductCount, this.CurrentPageNo, CreateProductListUrlWithoutPageNo(htParam));
			}

		}
	}

	/// <summary>
	/// Create url for list subscription box course
	/// </summary>
	protected static string CreateProductListUrlWithoutPageNo(Hashtable htParam)
	{
		var urlCreator = new w2.Common.Web.UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_LIST);
		if (htParam == null) return urlCreator.CreateUrl();

		urlCreator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, (string)htParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID])
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME, (string)htParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, (string)htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]);
			
		Enumerable.Range(1, Constants.COOPERATION_ID_COLUMNS_COUNT).ToList().ForEach(i =>
		{
			var key = Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COOPERATION_ID_HEAD + i.ToString();
			urlCreator.AddParam(key, (string)htParam[key]);
		});
		urlCreator.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)htParam[Constants.REQUEST_KEY_SORT_KBN])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, (string)htParam[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(string productId)
	{
		return CreateProductDetailUrl(productId, false);
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="isPopupAction">ポップアップか？</param>
	/// <returns></returns>
	protected string CreateProductDetailUrl(string productId, bool isPopupAction)
	{
		StringBuilder sbProductDetailUrl = new StringBuilder();
		sbProductDetailUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_REGISTER);
		sbProductDetailUrl.Append("?").Append(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID).Append("=").Append(HttpUtility.UrlEncode(productId));
		sbProductDetailUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);
		if (isPopupAction)
		{
			sbProductDetailUrl.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
		}

		return sbProductDetailUrl.ToString();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 有効フラグ
		dllValidFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_SUBSCRIPTIONBOX, Constants.FIELD_SUBSCRIPTIONBOX_VALID_FLG))
		{
			dllValidFlg.Items.Add(li);
		}
	}

	/// <summary>
	/// 商品一覧パラメタ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		Hashtable htResult = new Hashtable();
		try
		{
			// 商品ID
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
			tbProductId.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID];

			// 頒布会名
			htResult.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME]));
			tbSubscriptionBoxName.Text = (string)htResult[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME];

			// 頒布会コースID
			htResult.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID]));
			tbSubscriptionBoxCourseId.Text = (string)htResult[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID];

			htResult.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_DISPLAY_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_DISPLAY_KBN]));
			dllValidFlg.SelectedValue = (string)Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_DISPLAY_KBN];

			htResult.Add(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]));
			dllValidFlg.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_VALID_FLG];
			// ソート
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_ID_ASC:			// 商品ID/昇順
				case Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_ID_DESC:			// 商品ID/降順
				case Constants.KBN_SORT_PRODUCT_LIST_NAME_ASC:					// 商品名/昇順
				case Constants.KBN_SORT_PRODUCT_LIST_NAME_DESC:					// 商品名/降順
				case Constants.KBN_SORT_SUBSCRIPTION_BOX_VALID_FLAG:					// valid flag
					htResult.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
					break;
				default:
					htResult.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_SUBSCRIPTION_BOX_DEFAULT);
					break;
			}
			ddlSortKbn.SelectedValue = (string)htResult[Constants.REQUEST_KEY_SORT_KBN];

			// ページ番号（ページャ動作時のみもちまわる）
			int iPageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iPageNo) == false)
			{
				iPageNo = 1;
			}
			htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iPageNo.ToString());
			this.CurrentPageNo = iPageNo;
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return htResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htParam)
	{
		var htResult = new Hashtable();

		// 店舗ID
		htResult.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
		// 商品ID
		htResult.Add(Constants.FIELD_PRODUCT_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
		// 商品名
		htResult.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME]));
		// サプライヤID
		htResult.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID]));
		// 有効フラグ
		htResult.Add(Constants.FIELD_PRODUCT_VALID_FLG, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]));
		// ソート区分
		switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_SUBSCRIPTION_BOX_NAME_ASC:
			case Constants.KBN_SORT_SUBSCRIPTION_BOX_NAME_DESC:
			case Constants.KBN_SORT_SUBSCRIPTION_BOX_ID_ASC:
			case Constants.KBN_SORT_SUBSCRIPTION_BOX_ID_DESC:
			case Constants.KBN_SORT_SUBSCRIPTION_BOX_VALID_FLAG:
				m_strSortKbn = Request[Constants.REQUEST_KEY_SORT_KBN];
				break;
			default:
				m_strSortKbn = Constants.KBN_SORT_SUBSCRIPTION_BOX_DEFAULT;
				break;
		}
		htResult.Add("sort", m_strSortKbn);

		return htResult;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		Hashtable htSearch = new Hashtable();
		htSearch.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, tbSubscriptionBoxCourseId.Text.Trim());
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, tbProductId.Text.Trim());
		htSearch.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_NAME, tbSubscriptionBoxName.Text.Trim());
		htSearch.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, dllValidFlg.SelectedValue);
		return htSearch;
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 入力チェック
		Hashtable htSearch = GetSearchInfoFromControl();
		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateProductListUrlWithoutPageNo(htSearch));
	}

	/// <summary>
	/// 新規ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// セッション初期化
		//------------------------------------------------------
		Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = null;
		Session[Constants.SESSIONPARAM_KEY_PRODUCTVARIATION_INFO] = null;
		Session[Constants.SESSIONPARAM_KEY_PRODUCTEXTEND_INFO] = null;

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(
			CreateSubscriptionBoxRegisterUrl("", Constants.ACTION_STATUS_INSERT));
	}

	/// <summary>
	/// 頒布会登録画面のURLを作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>頒布会登録画面URL</returns>
	protected string CreateSubscriptionBoxRegisterUrl(string productId, string actionStatus)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID).Append("=").Append(HttpUtility.UrlEncode(productId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(actionStatus);

		return sbResult.ToString();
	}

	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_PAGE_NO]; }
		set { ViewState[Constants.REQUEST_KEY_PAGE_NO] = value; }
	}
	/// <summary>画面に表示されている商品IDリスト</summary>
	protected string[] ProductIdListOfDisplayedData
	{
		get { return (string[])ViewState["productid_list_of_displayed_data"]; }
		set { ViewState["productid_list_of_displayed_data"] = value; }
	}
}