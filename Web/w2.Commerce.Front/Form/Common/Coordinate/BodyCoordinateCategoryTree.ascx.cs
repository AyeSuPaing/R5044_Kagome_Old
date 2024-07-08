/*
=========================================================================================================
  Module      : コーディネートカテゴリツリー出力コントローラ処理(BodyProductCategoryTree.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.CoordinateCategory;
using w2.Domain.RealShop;
using w2.Domain.Staff;

public partial class Form_Common_Coordinate_BodyCoordinateCategoryTree : CoordinateUserControl
{
	#region ラップ済コントロール宣言
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
			// カテゴリツリー情報取得
			var categories = GetCoordinateCategoryFromCacheOrDb();

			// カテゴリツリー作成
			var coordinateCategoryTreeNodes = new List<CoordinateCategoryTreeNode>();
			var currentTree = coordinateCategoryTreeNodes;
			var stack = new Stack<List<CoordinateCategoryTreeNode>>();
			var beforeDepth = 0;		// 0～

			foreach (var category in categories)
			{
				// ツリーノード作成
				var cctn = new CoordinateCategoryTreeNode(category);

				// 深さが増したら、追加先リスト更新
				var currentDepth = (cctn.CategoryId.Length / Constants.CONST_CATEGORY_ID_LENGTH) - 1;
				if (currentDepth > beforeDepth)
				{
					stack.Push(currentTree);
					currentTree = currentTree[currentTree.Count - 1].Childs;
				}
				else
				{
					for (var loop = 0; loop < (beforeDepth - currentDepth); loop++)
					{
						currentTree = stack.Pop();
					}
				}

				// 追加して深さのパラメタ更新
				currentTree.Add(cctn);
				beforeDepth = currentDepth;
			}
			this.CategoryDataSource = coordinateCategoryTreeNodes;

			var staffList = DataCacheManager.GetInstance().GetData(
				"StaffAll",
				Constants.COORDINATELIST_CACHE_EXPIRE_MINUTES,
				new StaffService().GetAllForCoordinate);

			var realShopList = DataCacheManager.GetInstance().GetData(
				"RealShopAll",
				Constants.COORDINATELIST_CACHE_EXPIRE_MINUTES,
				new RealShopService().GetAll);

			this.StaffDataSource = (staffList != null)
				? staffList.Where(staff => staff.ValidFlg == Constants.FLG_STAFF_VALID_FLG_VALID).ToList()
				: new List<StaffModel>();
			this.RealShopDataSource = (realShopList != null)
				? realShopList.Where(shop => shop.ValidFlg == Constants.FLG_REALSHOP_VALID_FLG_VALID).ToList()
				: new List<RealShopModel>();

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報設定
				this.StaffDataSource = NameTranslationCommon.SetStaffTranslationData(this.StaffDataSource.ToArray()).ToList();
				this.CategoryDataSource = SetCoordinateCategoryTranslationData(this.CategoryDataSource.ToArray()).ToList();
				this.RealShopDataSource = NameTranslationCommon.SetRealShopTranslationData(this.RealShopDataSource.ToArray()).ToList();
			}

		}
		BasePage.DataBindByClass(this, "HeightList");
		this.WrCategoryList.DataSource = this.CategoryDataSource;
		this.WrStaffList.DataSource = this.StaffDataSource;
		this.WrRealShopList.DataSource = this.RealShopDataSource;
		this.WrCategoryList.DataBind();
		this.WrStaffList.DataBind();
		this.WrRealShopList.DataBind();
	}

	/// <summary>
	/// コーディネート一覧情報を取得（キャッシュorDBから）
	/// </summary>
	/// <returns>コーディネート一覧情報</returns>
	public CoordinateCategoryModel[] GetCoordinateCategoryFromCacheOrDb()
	{
		// キャッシュキー作成
		var cacheKey = "BodyCoordinateCategory " + string.Join(",", this.RequestParameter.Keys.Select(key => key + "=" + this.RequestParameter[key]).ToArray());

		// キャッシュまたはDBから取得
		var expire = Constants.COORDINATELIST_CACHE_EXPIRE_MINUTES;
		var data = DataCacheManager.GetInstance().GetData(
			cacheKey,
			expire,
			CreateCoordinateCategoryFromDb);
		return data;
	}

	/// <summary>
	/// DBからコーディネート一覧情報作成
	/// </summary>
	/// <returns>コーディネート一覧情報</returns>
	public virtual CoordinateCategoryModel[] CreateCoordinateCategoryFromDb()
	{
		// カテゴリツリー情報取得
		var ht = new Hashtable();
		for (var loop = 0; loop < (this.CoordinateCategoryId.Length / Constants.CONST_CATEGORY_ID_LENGTH); loop++)
		{
			var paramKey = "coordinate_category_id_like_escaped_" + (loop + 1);
			ht.Add(paramKey, StringUtility.SqlLikeStringSharpEscape(this.CoordinateCategoryId.Substring(0, Constants.CONST_CATEGORY_ID_LENGTH * (loop + 1))));
		}
		var categories = new CoordinateCategoryService().GetCategoryTree(ht);
		return categories;
	}

	/// <summary>
	/// コーディネートカテゴリ翻訳情報設定
	/// </summary>
	/// <param name="coordinateCategoryTrees">コーディネート情報リスト</param>
	/// <returns>翻訳後コーディネートリスト</returns>
	public static CoordinateCategoryTreeNode[] SetCoordinateCategoryTranslationData(params CoordinateCategoryTreeNode[] coordinateCategoryTrees)
	{
		var coordinateCategories = coordinateCategoryTrees.Select(
			tree => new CoordinateCategoryModel
			{
				CoordinateCategoryName = tree.CategoryName,
				CoordinateCategoryId = tree.CategoryId,
			}).ToArray();
		coordinateCategories = NameTranslationCommon.SetCoordinateCategoryTranslationData(coordinateCategories);
		coordinateCategoryTrees = coordinateCategories.Select(category => new CoordinateCategoryTreeNode(category)).ToArray();
		return coordinateCategoryTrees;
	}

	/// <summary>データソース</summary>
	public List<RealShopModel> RealShopDataSource
	{
		get { return (List<RealShopModel>)this.ViewState["RealShopDataSource"]; }
		set { this.ViewState["RealShopDataSource"] = value; }
	}
	/// <summary>データソース</summary>
	public List<StaffModel> StaffDataSource
	{
		get { return (List<StaffModel>)this.ViewState[" StaffDataSource"]; }
		set { this.ViewState[" StaffDataSource"] = value; }
	}
	/// <summary>データソース</summary>
	public List<CoordinateCategoryTreeNode> CategoryDataSource
	{
		get { return (List<CoordinateCategoryTreeNode>)this.ViewState["CoordinateCategoryTreeDataSource"]; }
		set { this.ViewState["CoordinateCategoryTreeDataSource"] = value; }
	}
}
