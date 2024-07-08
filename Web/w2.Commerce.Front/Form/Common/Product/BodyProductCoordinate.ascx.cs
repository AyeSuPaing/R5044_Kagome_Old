/*
=========================================================================================================
  Module      : 商品コーディネート出力コントローラー(BodyProductCoordinate.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Coordinate;

public partial class Form_Common_Product_BodyProductCoordinate : ProductUserControl
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
			// リクエストよりパラメタ取得
			GetParameters();

			// 同じ商品のコーディネートリスト
			this.CoordinateList = new CoordinateModel[this.MaxDispCount];
			var list = new CoordinateService().GetCoordinateListByProductId(this.ProductId, this.ShopId);
			if (list != null)
			{
				this.CoordinateList = list.Where(
					c => (c.DisplayKbn == "PUBLIC") && ((c.DisplayDate != null) && ((DateTime)c.DisplayDate < DateTime.Now))).ToArray();
				this.CoordinateList = CoordinateUserControl.CreateCoordinateListForDisplay(this.CoordinateList, this.MaxDispCount);
				this.CoordinateListDataSource = this.CoordinateList;

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					// 翻訳情報設定
					this.CoordinateListDataSource = NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.CoordinateListDataSource);
				}
			}
			else
			{
				 ProductCoordinateList.Visible = false;
			}
		}
		this.WrCoordinateList.DataSource = this.CoordinateListDataSource;
		this.WrCoordinateList.DataBind();
	}

	/// <summary>
	/// 最大表示数追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void AddMaxDispCount_Click(object sender, System.EventArgs e)
	{
		this.MaxDispCount += this.AddDispCount;

		// 同じ商品のコーディネートリスト
		this.CoordinateList = new CoordinateModel[this.MaxDispCount];
		var list = new CoordinateService().GetCoordinateListByProductId(this.ProductId, this.ShopId);
		if (list != null)
		{
			this.CoordinateList = list.Where(
				c => (c.DisplayKbn == "PUBLIC") && ((c.DisplayDate != null) && ((DateTime)c.DisplayDate < DateTime.Now))).ToArray();
			this.CoordinateList = CoordinateUserControl.CreateCoordinateListForDisplay(this.CoordinateList, this.MaxDispCount);
			this.CoordinateListDataSource = this.CoordinateList;
			this.WrCoordinateList.DataSource = this.CoordinateListDataSource;
			this.WrCoordinateList.DataBind();
		}
	}

	/// <summary>最大表示件数</summary>
	protected int MaxDispCount { get; set; }
	/// <summary>増加表示件数</summary>
	protected int AddDispCount { get; set; }
	/// <summary>コーディネートリスト</summary>
	public CoordinateModel[] CoordinateList
	{
		get { return (CoordinateModel[])this.ViewState["CoordinateList"]; }
		set { this.ViewState["CoordinateList"] = value; }
	}
	/// <summary>データソース</summary>
	public CoordinateModel[] CoordinateListDataSource
	{
		get { return (CoordinateModel[])this.ViewState["CoordinateListDataSource"]; }
		set { this.ViewState["CoordinateListDataSource"] = value; }
	}
}
