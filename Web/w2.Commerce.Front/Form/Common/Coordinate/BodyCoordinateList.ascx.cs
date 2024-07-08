/*
=========================================================================================================
  Module      : コーディネートリスト出力コントローラ処理(BodyCoordinateList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Coordinate;
using w2.Domain.Coordinate.Helper;

public partial class Form_Common_Coordinate_BodyCoordinateList : CoordinateUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrCoordinateList { get { return GetWrappedControl<WrappedRepeater>("rCoordinateList"); } }
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
			var coordinateListinfo = GetCoordinateListInfoFromCacheOrDb("BodyCoordinateList ");
			this.CoordinateCount = coordinateListinfo.Item1;
			this.CoordinateList = coordinateListinfo.Item2;
			var array = this.CoordinateList;

			this.CoordinateList = CreateCoordinateListForDisplay(array, this.MaxDispCount);
			this.CoordinateListDataSource = this.CoordinateList;
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 翻訳情報設定
			this.CoordinateListDataSource = NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.CoordinateListDataSource);
		}
		this.WrCoordinateList.DataSource = this.CoordinateListDataSource;

		this.WrCoordinateList.DataBind();
	}

	/// <summary>
	/// DBからコーディネート一覧情報作成
	/// </summary>
	/// <returns>item1：総件数
	/// item2:コーディネート一覧</returns>
	public override Tuple<int, CoordinateListSearchResult[]> CreateCoordinateListInfoFromDb()
	{
		var searchCondition = new CoordinateListSearchCondition
		{
			Staff = this.StaffId,
			RealShop = this.RealShopId,
			BeginRowNumber = 1,
			EndRowNumber = this.MaxDispCount,
			Category = this.CoordinateCategoryId,
			DisplayKbn = Constants.FLG_COORDINATE_DISPLAY_KBN_PUBLIC,
			HeightUpperLimit = this.HeightUpperLimit,
			HeightLowerLimit = this.HeightLowerLimit,
			DisplayDateKbn = "UNSELECTED",
			ConsiderDisplayDate = "1"
		};

		var list = new CoordinateService().Search(searchCondition).GroupBy(u => u.CoordinateId).Where(u => u.Any())
			.Distinct().Select(u => u.FirstOrDefault()).ToArray();
		var count = new CoordinateService().GetSearchHitCount(searchCondition);
		var info = new Tuple<int, CoordinateListSearchResult[]>(count, list);

		return info;
	}

	/// <summary>総件数</summary>
	protected int CoordinateCount { get; set; }
	/// <summary>最大表示数</summary>
	protected int MaxDispCount { get; set; }
	/// <summary>スタッフID</summary>
	protected new string StaffId { get; set; }
	/// <summary>カテゴリID</summary>
	protected new string CoordinateCategoryId { get; set; }
	/// <summary>店舗ID</summary>
	protected new string RealShopId { get; set; }
	/// <summary>コーディネートリスト</summary>
	public CoordinateListSearchResult[] CoordinateList { get; set; }
	/// <summary>データソース</summary>
	public CoordinateListSearchResult[] CoordinateListDataSource
	{
		get { return (CoordinateListSearchResult[])this.ViewState["CoordinateListDataSource"]; }
		set { this.ViewState["CoordinateListDataSource"] = value; }
	}

}
