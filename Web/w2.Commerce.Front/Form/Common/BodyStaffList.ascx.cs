/*
=========================================================================================================
  Module      : スタッフリスト出力コントローラ処理(BodyStaffList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Staff;
using w2.Domain.Staff.Helper;

/// <summary>
/// スタッフリスト出力コントローラー
/// </summary>
public partial class Form_Common_BodyStaffList : CoordinateUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrTopStaffList { get { return GetWrappedControl<WrappedRepeater>("rTopStaffList"); } }
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
			var searchCondition = new StaffListSearchCondition
			{
				StaffId = this.StaffId,
				StaffName = this.StaffName,
				HeightLowerLimit = this.HeightLowerLimit,
				HeightUpperLimit = this.HeightUpperLimit,
				BeginRowNumber = 1,
				EndRowNumber = this.MaxDispCount,
				ValidFlg = Constants.FLG_STAFF_VALID_FLG_VALID
			};

			var service = new StaffService();
			this.StaffCount = service.GetSearchHitCount(searchCondition);
			this.TopStaffList = service.Search(searchCondition);
			this.TopStaffListDataSource = this.TopStaffList;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報設定
				this.TopStaffListDataSource = NameTranslationCommon.SetStaffTranslationData(this.TopStaffListDataSource);
			}
		}
		this.WrTopStaffList.DataSource = this.TopStaffListDataSource;

		this.WrTopStaffList.DataBind();
	}

	/// <summary>総件数</summary>
	protected int StaffCount { get; set; }
	/// <summary>最大表示数</summary>
	protected int MaxDispCount { get; set; }
	/// <summary>スタッフID</summary>
	protected new string StaffId { get; set; }
	/// <summary>スタッフ名</summary>
	protected string StaffName { get; set; }
	/// <summary>身長下限</summary>
	protected new string HeightLowerLimit { get; set; }
	/// <summary>身長上限</summary>
	protected new string HeightUpperLimit { get; set; }
	/// <summary>スタッフリスト</summary>
	public StaffListSearchResult[] TopStaffList { get; set; }
	/// <summary>データソース</summary>
	public StaffListSearchResult[] TopStaffListDataSource
	{
		get { return (StaffListSearchResult[])this.ViewState["TopStaffListDataSource"]; }
		set { this.ViewState["TopStaffListDataSource"] = value; }
	}
}
