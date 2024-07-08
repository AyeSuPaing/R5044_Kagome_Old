/*
=========================================================================================================
  Module      : コーディネートカテゴリビューモデル(CoordinateCategoryViewModel.cs)
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
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.CoordinateCategory;
using w2.Common.Util;
using w2.Domain.CoordinateCategory;
using w2.Domain.CoordinateCategory.Helper;

namespace w2.Cms.Manager.ViewModels.CoordinateCategory
{
	/// <summary>
	/// コーディネートカテゴリビューモデル
	/// </summary>
	[Serializable]
	public class CoordinateCategoryViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="model">モデル</param>
		/// <param name="pm">パラメタモデル</param>
		public CoordinateCategoryViewModel(
			ActionStatus actionStatus, 
			CoordinateCategoryModel model,
			CoordinateCategoryParamModel pm)
		{
			InitializeComponents(pm, actionStatus);

			this.ExportFiles =
				MasterExportHelper.CreateExportFilesDdlItems(
					Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_CATEGORY);
			
			switch (this.ActionStatus)
			{
				case ActionStatus.Detail:
				case ActionStatus.Update:
					var searchedModel = new CoordinateCategoryService().Get(pm.CoordinateCategoryId);
					if (searchedModel == null)
					{
						this.ErrorMessage = WebMessages.InconsistencyError;
						break;
					}
					SetModel(searchedModel);
					break;

				case ActionStatus.Insert:
					this.IsValid = true;
					break;

				case ActionStatus.Confirm:
					SetModel(model);
					break;
			}
		}

		/// <summary>
		/// 画面制御
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="actionStatus">アクションステータス</param>
		public void InitializeComponents(CoordinateCategoryParamModel pm, ActionStatus actionStatus)
		{
			this.ActionStatus = actionStatus;
			if (this.IsActionStatusUpdate) this.IsUpdate = true;
			this.ParamModel = pm;
			this.List = new CoordinateCategoryListSearchResult[0];
			if (string.IsNullOrEmpty(pm.CoordinateParentCategoryId) == false)
			{
				this.CoordinateParentCategoryId = pm.CoordinateParentCategoryId;
			}

			var nameList = new CoordinateCategoryService().GetTopCoordinateCategory();
			if (nameList != null)
			{
				this.TopCoordinateCategoryNameList =
					nameList.Select(l =>
							new SelectListItem
							{
								Text = l.CoordinateCategoryName,
								Value = l.CoordinateCategoryId
							}).ToList();
				this.TopCoordinateCategoryNameList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}
			else
			{
				this.TopCoordinateCategoryNameList = new List<SelectListItem>();
				this.TopCoordinateCategoryNameList.Insert(0, new SelectListItem { Text = "", Value = "" });
			}
		}

		/// <summary>
		/// カテゴリー情報セット
		/// </summary>
		/// <param name="model">モデル</param>
		private void SetModel(CoordinateCategoryModel model)
		{
			this.CoordinateCategoryId = model.CoordinateCategoryId;
			this.FormattedCoordinateCategoryId = this.CoordinateCategoryId.Substring(this.CoordinateCategoryId.Length - 3);
			this.CoordinateCategoryName = model.CoordinateCategoryName;
			this.CoordinateParentCategoryId = model.CoordinateParentCategoryId;
			this.DisplayOrder = model.DisplayOrder.ToString();
			this.SeoKeywords = model.SeoKeywords;
			this.IsValid = (model.ValidFlg == Constants.FLG_COORDINATECATEGORY_VALID_FLG_VALID);
			this.ValidFlg = ValueText.GetValueText(
				Constants.TABLE_COORDINATECATEGORY,
				Constants.FIELD_COORDINATECATEGORY_VALID_FLG,
				model.ValidFlg);
			if (this.IsActionStatusDetail)
			{
				this.DateCreated = DateTimeUtility.ToStringForManager(model.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
				this.DateChanged = DateTimeUtility.ToStringForManager(model.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
				this.LastChanged = model.LastChanged;
			}
		}

		/// <summary>カテゴリID</summary>
		public string CoordinateCategoryId { get; set; }
		/// <summary>整形されたカテゴリID</summary>
		public string FormattedCoordinateCategoryId { get; set; }
		/// <summary>カテゴリ名</summary>
		public string CoordinateCategoryName { get; set; }
		/// <summary>最上位カテゴリ名リスト</summary>
		public List <SelectListItem> TopCoordinateCategoryNameList { get; set; }
		/// <summary>親カテゴリID</summary>
		public string CoordinateParentCategoryId { get; set; }
		/// <summary>SEOキーワード</summary>
		public string SeoKeywords { get; set; }
		/// <summary>表示順</summary>
		public string DisplayOrder { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>有効か</summary>
		public bool IsValid { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>リスト一覧</summary>
		public CoordinateCategoryListSearchResult[] List { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>パラムモデル</summary>
		public CoordinateCategoryParamModel ParamModel { get; set; }
		/// <summary>更新か</summary>
		public bool IsUpdate { get; set; }
		/// <summary>選択肢群 マスタ出力</summary>
		public SelectListItem[] ExportFiles { get; private set; }
	}
}