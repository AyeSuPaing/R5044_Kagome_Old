/*
=========================================================================================================
  Module      : コーディネート一覧画面処理(CoordinateList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Coordinate;
using w2.Domain.Coordinate.Helper;

public partial class Form_Coordinate_CoordinateList : CoordinatePage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	#region ラップ済コントロール宣言
	WrappedRepeater WrCoordinateList { get { return GetWrappedControl<WrappedRepeater>("rCoordinateList"); } }
	WrappedLabel WrPager1 { get { return GetWrappedControl<WrappedLabel>("lPager1"); } }
	WrappedLabel WrPager2 { get { return GetWrappedControl<WrappedLabel>("lPager2"); } }
	WrappedLabel WrAlertMessage { get { return GetWrappedControl<WrappedLabel>("lAlertMessage"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!this.IsPostBack)
		{
			// 表示データ取得
			var info = CreateCoordinateListInfoFromDb();
			var total = info.Item1;
			this.CoordinateList = info.Item2;

			// ページャ作成（コーディネート一覧処理で総件数を取得）
			var parameter = this.RequestParameter;
			// パラメーター削除
			parameter.Remove(Constants.REQUEST_KEY_PAGE_NO);
			this.WrPager1.Text = this.WrPager2.Text = WebPager.CreateDefaultListPager(total, this.PageNumber, CreateCoordinateListUrl(parameter));

			// コーディネートが０個であれば情報表示
			if (this.CoordinateList.Count == 0) this.WrAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COORDINATE_NO_ITEM);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.CoordinateList =
					NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.CoordinateList.ToArray()).ToList();
			}

			// コーディネート一覧をデータバインド
			this.CoordinateListDataSource = this.CoordinateList;
			this.WrCoordinateList.DataSource = this.CoordinateListDataSource;
			this.WrCoordinateList.DataBind();
		}
	}

	/// <summary>
	/// DBからコーディネート一覧情報作成
	/// </summary>
	/// <returns>item1：総件数
	/// item2:コーディネート一覧</returns>
	private Tuple<int, List<CoordinateListSearchResult>> CreateCoordinateListInfoFromDb()
	{
		var searchCondition = new CoordinateListSearchCondition
		{
			Staff = this.StaffId,
			RealShop = this.RealShopId,
			BeginRowNumber = (this.PageNumber - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
			EndRowNumber = this.PageNumber * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
			Category = this.CoordinateCategoryId,
			HeightUpperLimit = this.HeightUpperLimit,
			HeightLowerLimit = this.HeightLowerLimit,
			DisplayKbn = Constants.FLG_COORDINATE_DISPLAY_KBN_PUBLIC,
			DisplayDateKbn = "UNSELECTED",
			ConsiderDisplayDate = "1"
		};

		if (string.IsNullOrEmpty(this.SearchKeyword) == false)
		{
			char[] delimiterChars = { ' ', '　' }; // 半角全角スペース
			var keywords = this.SearchKeyword.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
			if (keywords.Length > 0) searchCondition.Keyword0 = keywords[0];
			if (keywords.Length > 1) searchCondition.Keyword1 = keywords[1];
			if (keywords.Length > 2) searchCondition.Keyword2 = keywords[2];
			if (keywords.Length > 3) searchCondition.Keyword3 = keywords[3];
			if (keywords.Length > 4) searchCondition.Keyword4 = keywords[4];
			if (keywords.Length > 5)
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COORDINATESEARCH_WORDS_NUMOVER);
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		var list = new CoordinateService().Search(searchCondition).GroupBy(u => u.CoordinateId).Where(u => u.Any())
			.Distinct().Select(u => u.FirstOrDefault()).ToList();
		var count = new CoordinateService().GetSearchHitCount(searchCondition);
		var info = new Tuple<int, List<CoordinateListSearchResult>>(count, list);
		return info;
	}

	/// <summary>コーディネートリスト</summary>
	protected List<CoordinateListSearchResult> CoordinateList { get; set; }
	/// <summary>コーディネートリスト（データソース）</summary>
	protected List<CoordinateListSearchResult> CoordinateListDataSource
	{
		get { return (List<CoordinateListSearchResult>)this.ViewState["CoordinateListDataSource"]; }
		private set { this.ViewState["CoordinateListDataSource"] = value; }
	}
}