/*
=========================================================================================================
  Module      : コーディネート検索出力コントローラ処理(BodySearchCoordinate.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.CoordinateCategory;
using w2.Domain.RealShop;
using w2.Domain.Staff;

public partial class Form_Common_Coordinate_BodySearchCoordinate : CoordinateUserControl
{
	#region ラップ済コントロール宣言
	protected WrappedTextBox WtbSearchCoordinate { get { return GetWrappedControl<WrappedTextBox>("tbSearchCoordinate"); } }
	protected WrappedButton WlbSearch { get { return GetWrappedControl<WrappedButton>("lbSearch"); } }
	WrappedRepeater WrCategoryList { get { return GetWrappedControl<WrappedRepeater>("rCategoryList"); } }
	WrappedRepeater WrStaffList { get { return GetWrappedControl<WrappedRepeater>("rStaffList"); } }
	WrappedRepeater WrRealShopList { get { return GetWrappedControl<WrappedRepeater>("rRealShopList"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsPostBack)
		{
			this.WtbSearchCoordinate.Text = this.SearchKeyword;
		}

		// リストをセット
		var staffList = new StaffService().GetAllForCoordinate();
		var realShopList = new RealShopService().GetAll();
		var categoryList = new CoordinateCategoryService().GetTopCoordinateCategory();
		this.StaffListDataSource = (staffList != null)
			? staffList.Where(staff => staff.ValidFlg == Constants.FLG_STAFF_VALID_FLG_VALID).ToList()
			: new List<StaffModel>();
		this.RealShopListDataSource = (realShopList != null)
			? realShopList.Where(shop => shop.ValidFlg == Constants.FLG_REALSHOP_VALID_FLG_VALID).ToList()
			: new List<RealShopModel>();
		this.CoordinateCategoryListDataSource = (categoryList != null)
			? categoryList.Where(cat => cat.ValidFlg == Constants.FLG_COORDINATECATEGORY_VALID_FLG_VALID).ToList()
			: new List<CoordinateCategoryModel>();

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 翻訳情報設定
			this.StaffListDataSource = 
				NameTranslationCommon.SetStaffTranslationData(this.StaffListDataSource.ToArray()).ToList();
			this.CoordinateCategoryListDataSource = 
				NameTranslationCommon.SetCoordinateCategoryTranslationData(this.CoordinateCategoryListDataSource.ToArray()).ToList();
			this.RealShopListDataSource =
				NameTranslationCommon.SetRealShopTranslationData(this.RealShopListDataSource.ToArray()).ToList();
		}

		this.WrCategoryList.DataSource = this.CoordinateCategoryListDataSource;
		this.WrStaffList.DataSource = this.StaffListDataSource;
		this.WrRealShopList.DataSource = this.RealShopListDataSource;
		this.WrCategoryList.DataBind();
		this.WrStaffList.DataBind();
		this.WrRealShopList.DataBind();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearch_Click(object sender, EventArgs e)
	{
		var url = CoordinatePage.CreateCoordinateListUrl(
			Constants.REQUEST_KEY_COORDINATE_KEYWORD,
			this.WtbSearchCoordinate.Text.Trim());
		this.Response.Redirect(url);
	}

	/// <summary>データソース</summary>
	public List<RealShopModel> RealShopListDataSource
	{
		get { return (List<RealShopModel>)this.ViewState["RealShopListDataSource"]; }
		set { this.ViewState["RealShopListDataSource"] = value; }
	}
	/// <summary>データソース</summary>
	public List<StaffModel> StaffListDataSource
	{
		get { return (List<StaffModel>)this.ViewState[" StaffListDataSource"]; }
		set { this.ViewState[" StaffListDataSource"] = value; }
	}
	/// <summary>データソース</summary>
	public List<CoordinateCategoryModel> CoordinateCategoryListDataSource
	{
		get { return (List<CoordinateCategoryModel>)this.ViewState["CoordinateCategoryListDataSource"]; }
		set { this.ViewState["CoordinateCategoryListDataSource"] = value; }
	}
}
