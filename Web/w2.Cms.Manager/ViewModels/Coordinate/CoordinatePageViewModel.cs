/*
=========================================================================================================
  Module      : コーディネートページビューモデル(CoordinatePageViewModel.cs)
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
using w2.Domain.CoordinateCategory;
using w2.Domain.RealShop;
using w2.Domain.Staff;
using Constants = w2.App.Common.Constants;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// コーディネートカテゴリビューモデル
	/// </summary>
	[Serializable]
	public class CoordinatePageViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CoordinatePageViewModel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void InitializeComponent()
		{
			// カテゴリリスト
			var categoryList = new CoordinateCategoryService().GetAll();
			if (categoryList != null)
			{
				this.CategoryList =
					categoryList.Select(l =>
						new SelectListItem
						{
							Text = l.CoordinateCategoryName,
							Value = l.CoordinateCategoryId
						}).ToList();
				this.CategoryList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}
			else
			{
				this.CategoryList = new List<SelectListItem>();
				this.CategoryList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}

			// スタッフリスト
			var staffList = new StaffService().GetAllForCoordinate();
			if (staffList != null)
			{
				this.StaffList =
					staffList.Select(l =>
						new SelectListItem
						{
							Text = l.StaffName,
							Value = string.Format("{0}:{1}", l.StaffId, l.StaffName)
						}).ToList();
				this.StaffList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}
			else
			{
				this.StaffList = new List<SelectListItem>();
				this.StaffList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}

			// リアル店舗リスト
			var realShopList = new RealShopService().GetAll();
			if (realShopList != null)
			{
				this.RealShopList =
					realShopList.Select(l =>
						new SelectListItem
						{
							Text = l.Name,
							Value = string.Format("{0}:{1}", l.RealShopId, l.Name)
						}).ToList();
				this.RealShopList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}
			else
			{
				this.RealShopList = new List<SelectListItem>();
				this.RealShopList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}

			// タグリスト
			var tagList = new ContentsTagService().GetAll();
			if (tagList != null)
			{
				this.TagList =
					tagList.Select(l =>
						new SelectListItem
						{
							Text = l.ContentsTagName,
							Value = l.ContentsTagId.ToString()
						}).ToList();
				this.TagList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}
			else
			{
				this.TagList = new List<SelectListItem>();
				this.TagList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}

			// 公開日リスト
			this.DisplayDateKbnList =
				ValueText.GetValueItemArray(Constants.TABLE_COORDINATE, Constants.FLG_COORDINATE_DISPLAY_DATE_KBN)
					.Select(l =>
						new SelectListItem
						{
							Text = l.Text,
							Value = l.Value
						}).ToList();

			this.ExportFiles = MasterExportHelper.CreateExportFilesDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE)
					.Concat(MasterExportHelper.CreateExportFilesDdlItems(
						Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM))
					.ToArray();
		}

		/// <summary>リアル店舗リスト</summary>
		public List<SelectListItem> RealShopList { get; set; }
		/// <summary>スタッフリスト</summary>
		public List<SelectListItem> StaffList { get; set; }
		/// <summary>カテゴリリスト</summary>
		public List<SelectListItem> CategoryList { get; set; }
		/// <summary>タグリスト</summary>
		public List<SelectListItem> TagList { get; set; }
		/// <summary>公開日リスト</summary>
		public List<SelectListItem> DisplayDateKbnList { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>パラムモデル</summary>
		public CoordinateParamModel ParamModel { get; set; }
		/// <summary>選択肢群 マスタ出力</summary>
		public SelectListItem[] ExportFiles { get; private set; }
	}
}