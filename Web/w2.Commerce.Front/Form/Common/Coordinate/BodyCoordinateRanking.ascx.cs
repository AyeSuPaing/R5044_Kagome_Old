/*
=========================================================================================================
  Module      : コーディネートランキング出力コントローラ処理(BodyCoordinateRanking.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Coordinate;

public partial class Form_Common_Coordinate_BodyCoordinateRanking : CoordinateUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrCoordinateList { get { return GetWrappedControl<WrappedRepeater>("rRankingList"); } }
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
			this.CoordinateList = new CoordinateModel[this.MaxDispCount];
			var data = GetCoordinateRankingFromCacheOrDb();
			if (data == null) return;

			// 表示データ取得
			this.CoordinateList = CreateCoordinateListForDisplay(data, this.MaxDispCount);
			this.CoordinateRankingDataSource = this.CoordinateList;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報設定
				this.CoordinateRankingDataSource = NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.CoordinateRankingDataSource);
			}
		}
		this.WrCoordinateList.DataSource = this.CoordinateRankingDataSource;

		this.WrCoordinateList.DataBind();
	}

	/// <summary>
	/// コーディネートランキングを取得
	/// </summary>
	/// <returns>コーディネートランキング</returns>
	private CoordinateModel[] GetCoordinateRankingFromCacheOrDb()
	{
		// キャッシュキー作成
		var cacheKey = "BodyCoordinateRanking " + string.Join(",", this.SummaryClass, this.CountDays).ToArray();

		// キャッシュまたはDBから取得
		var expire = Constants.COORDINATELIST_CACHE_EXPIRE_MINUTES;
		var data = DataCacheManager.GetInstance().GetData(
			cacheKey,
			expire,
			CreateCoordinateRankingList);
		return data;
	}

	/// <summary>
	/// コーディネートランキングを作成
	/// </summary>
	/// <returns>コーディネートランキング</returns>
	private CoordinateModel[] CreateCoordinateRankingList()
	{
		var service = new CoordinateService();
		return this.SummaryClass == "LIKE"
			? service.GetLikeRankingList(this.CountDays)
			: service.GetContentsSummaryRankingList(this.SummaryClass, this.CountDays);
	}

	/// <summary>最大表示数</summary>
	protected int MaxDispCount { get; set; }
	/// <summary>集計日数</summary>
	protected int CountDays { get; set; }
	/// <summary>集計区分</summary>
	protected string SummaryClass { get; set; }
	/// <summary>コーディネートリスト</summary>
	public CoordinateModel[] CoordinateList { get; set; }
	/// <summary>データソース</summary>
	public CoordinateModel[] CoordinateRankingDataSource
	{
		get { return (CoordinateModel[])this.ViewState["CoordinateRankingDataSource"]; }
		set { this.ViewState["CoordinateRankingDataSource"] = value; }
	}
}
