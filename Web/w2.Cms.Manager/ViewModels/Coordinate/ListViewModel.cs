/*
=========================================================================================================
  Module      : 一覧ビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.Coordinate;
using w2.Common.Util;
using w2.Domain.ContentsTag;
using w2.Domain.Coordinate.Helper;
using w2.Domain.CoordinateCategory;
using w2.Domain.RealShop;
using w2.Domain.Staff;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// コーディネートカテゴリビューモデル
	/// </summary>
	[Serializable]
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void InitializeComponent()
		{
			this.List = new List<CoordinateListSearchResult>();

			// 公開区分リスト
			this.DisplayKbns =
				ValueText.GetValueItemArray(Constants.TABLE_COORDINATE, Constants.FIELD_COORDINATE_DISPLAY_KBN)
					.Select(l =>
						new SelectListItem
						{
							Text = l.Text,
							Value = l.Value
						}).ToList();

			// カテゴリリスト
			var categoryList = new CoordinateCategoryService().GetAll();
			this.CategoryList = (categoryList != null) ?
				categoryList.Select(l =>
					new SelectListItem
					{
						Text = l.CoordinateCategoryName,
						Value = l.CoordinateCategoryId
					}).ToList()
					: this.CategoryList = new List<SelectListItem>();
			this.CategoryList.Insert(0, new SelectListItem { Text = "", Value = "" });
			

			// スタッフリスト
			var staffList = new StaffService().GetAllForCoordinate();
			this.StaffList = (staffList != null) ?
				staffList.Select(l =>
					new SelectListItem
					{
						Text = l.StaffName,
						Value = l.StaffId
					}).ToList()
			: this.StaffList = new List<SelectListItem>();
			this.StaffList.Insert(0, new SelectListItem { Text = "", Value = "" });

			// リアル店舗リスト
			var realShopList = new RealShopService().GetAll();
			this.RealShopList = (realShopList != null) ?
				realShopList.Select(l =>
					new SelectListItem
					{
						Text = l.Name,
						Value = l.RealShopId
					}).ToList()
			: this.RealShopList = new List<SelectListItem>();
			this.RealShopList.Insert(0, new SelectListItem { Text = "", Value = "" });

			// タグリスト
			var tagList = new ContentsTagService().GetAll();
			this.TagList = (tagList != null) ?
				tagList.Select(l =>
					new SelectListItem
					{
						Text = l.ContentsTagName,
						Value = l.ContentsTagId.ToString()
					}).ToList()
					: this.TagList = new List<SelectListItem>();
			this.TagList.Insert(0, new SelectListItem { Text = "", Value = "" });
		}

		/// <summary>表示区分リスト</summary>
		public List<SelectListItem> DisplayKbns { get; set; }
		/// <summary>リアル店舗リスト</summary>
		public List<SelectListItem> RealShopList { get; set; }
		/// <summary>スタッフリスト</summary>
		public List<SelectListItem> StaffList { get; set; }
		/// <summary>カテゴリリスト</summary>
		public List<SelectListItem> CategoryList { get; set; }
		/// <summary>リアル店舗リスト</summary>
		public List<SelectListItem> TagList { get; set; }
		/// <summary>リスト一覧</summary>
		public List<CoordinateListSearchResult> List { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>パラムモデル</summary>
		public CoordinateParamModel ParamModel { get; set; }
	}
}