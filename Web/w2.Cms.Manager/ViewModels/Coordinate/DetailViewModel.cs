/*
=========================================================================================================
  Module      : 詳細ビューデル(DetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Domain.ContentsTag;
using w2.Domain.Coordinate;
using w2.Domain.CoordinateCategory;
using w2.Domain.RealShop;
using w2.Domain.Staff;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// コーディネートカテゴリビューモデル
	/// </summary>
	[Serializable]
	public class DetailViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailViewModel()
		{
			InitializeComponent();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailViewModel(string coordinateId, string shopId)
			:this()
		{
			this.Input = new CoordinateInput();
			this.ImageModalViewModel = new ImageModalViewModel();
			SetModel(coordinateId, shopId);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void InitializeComponent()
		{
			// 公開区分リスト
			this.DisplayKbns =
				ValueText.GetValueItemArray(Constants.TABLE_COORDINATE, Constants.FIELD_COORDINATE_DISPLAY_KBN)
					.Select(l =>
						new SelectListItem
						{
							Text = l.Text,
							Value = l.Value
						}).ToList();

			// カテゴリ
			var categoryList = new CoordinateCategoryService().GetAll();
			if (categoryList != null)
			{
				foreach (var category in categoryList)
				{
					if (string.IsNullOrEmpty(category.CoordinateCategoryName) == false) this.Categorys += category.CoordinateCategoryName + ',';
				}
			}

			// タグ
			var tagList = new ContentsTagService().GetAll();
			if (tagList != null)
			{
				foreach (var tag in tagList)
				{
					if (string.IsNullOrEmpty(tag.ContentsTagName) == false) this.Tags += tag.ContentsTagName + ',';
				}
			}

			// スタッフリスト
			var staffList = new StaffService().GetAllForCoordinate();
			this.StaffList = (staffList != null)
				? staffList.Select(
					m => new SelectListItem
					{
						Text = m.StaffName,
						Value = m.StaffId
					}).ToList()
				: new List<SelectListItem>();

			// リアル店舗リスト
			var realShopList = new RealShopService().GetAll();
			this.RealShopList = (realShopList != null)
				? realShopList.Where(
					s => s.ValidFlg == Constants.FLG_REALSHOP_VALID_FLG_VALID).Select(
					m => new SelectListItem
					{
						Text = m.Name,
						Value = m.RealShopId
					}).ToList()
				: new List<SelectListItem>();
		}

		/// <summary>
		/// モデルをセット
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="shopId">店舗ID</param>
		public void SetModel(string coordinateId, string shopId)
		{
			var model = new CoordinateService().GetWithChilds(coordinateId, shopId);
			if (model == null)
			{
				this.CoordinateId = coordinateId;
				this.ActionStatus = ActionStatus.Insert;
				return;
			}
			var displayDate =DateTimeUtility.ToStringForManager(
				model.DisplayDate,
				DateTimeUtility.FormatType.ShortDateHourMinute2Letter)
				.Split(new[] { " " }, StringSplitOptions.None);
			this.CoordinateId = model.CoordinateId;
			this.CoordinateTitle = model.CoordinateTitle;
			this.CoordinateUrl = model.CoordinateUrl;
			this.CoordinateSummary = model.CoordinateSummary;
			this.InternalMemo = model.InternalMemo;
			this.StaffId = model.StaffId;
			this.RealShopId = model.RealShopId;
			this.DisplayKbn = model.DisplayKbn;
			if (model.DisplayDate != null)
			{
				this.DisplayDate1 = displayDate[0];
				this.DisplayDate2 = displayDate[1];
			}
			this.HtmlTitle = model.HtmlTitle;
			this.MetadataDesc = model.MetadataDesc;

			foreach (var tag in model.TagList)
			{
				if(string.IsNullOrEmpty(tag.ContentsTagName) == false) this.ContentsTagNames += tag.ContentsTagName + ',';
			}
			foreach (var category in model.CategoryList)
			{
				if (string.IsNullOrEmpty(category.CoordinateCategoryName) == false) this.CoordinateCategoryNames += category.CoordinateCategoryName + ',';
			}
			foreach (var product in model.ProductList)
			{
				if (string.IsNullOrEmpty(product.ProductId) == false) this.ProductIds += product.ProductId + ',';
				if (string.IsNullOrEmpty(product.VariationId) == false) this.VariationIds += product.VariationId + ',';
			}
			this.ActionStatus = ActionStatus.Update;
		}

		/// <summary>画像リストから選択</summary>
		public ImageModalViewModel ImageModalViewModel { get; set; }
		/// <summary>入力値</summary>
		public CoordinateInput Input { get; set; }
		/// <summary>コーディネートID</summary>
		public string CoordinateId { get; set; }
		/// <summary>コーディネートタイトル</summary>
		public string CoordinateTitle { get; set; }
		/// <summary>コーディネートURL</summary>
		public string CoordinateUrl { get; set; }
		/// <summary>コーディネート概要</summary>
		public string CoordinateSummary { get; set; }
		/// <summary>内部用メモ</summary>
		public string InternalMemo { get; set; }
		/// <summary>スタッフID</summary>
		public string StaffId { get; set; }
		/// <summary>リアル店舗ID</summary>
		public string RealShopId { get; set; }
		/// <summary>表示区分</summary>
		public string DisplayKbn { get; set; }
		/// <summary>公開日(日付)</summary>
		public string DisplayDate1 { get; set; }
		/// <summary>公開日（時間）</summary>
		public string DisplayDate2 { get; set; }
		/// <summary>カテゴリ名（カンマ区切り）</summary>
		public string CoordinateCategoryNames { get; set; }
		/// <summary>タグ名（カンマ区切り）</summary>
		public string ContentsTagNames { get; set; }
		/// <summary>商品ID（カンマ区切り）</summary>
		public string ProductIds { get; set; }
		/// <summary>商品ID（カンマ区切り）</summary>
		public string VariationIds { get; set; }
		/// <summary>タイトル</summary>
		public string HtmlTitle { get; set; }
		/// <summary>ディスクリプション</summary>
		public string MetadataDesc { get; set; }
		/// <summary>表示区分リスト</summary>
		public List<SelectListItem> DisplayKbns { get; set; }
		/// <summary>リアル店舗リスト</summary>
		public List<SelectListItem> RealShopList { get; set; }
		/// <summary>スタッフリスト</summary>
		public List<SelectListItem> StaffList { get; set; }
		/// <summary>カテゴリリスト（カンマ区切り）</summary>
		public string Categorys { get; set; }
		/// <summary>タグリスト（カンマ区切り）</summary>
		public string Tags { get; set; }
		/// <summary>作成日</summary>
		public DateTime DateCreated { get; set; }
		/// <summary>更新日</summary>
		public DateTime DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}