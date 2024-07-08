/*
=========================================================================================================
  Module      : コーディネートカテゴリ入力クラス (CoordinateCategoryInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.CoordinateCategory;

/// <summary>
/// コーディネートカテゴリ入力クラス
/// </summary>
public class CoordinateCategoryInput : InputBase<CoordinateCategoryModel>
{
	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override CoordinateCategoryModel CreateModel()
	{
		var model = new CoordinateCategoryModel
		{
			CoordinateParentCategoryId = this.CoordinateParentCategoryId,
			CoordinateCategoryId = this.CoordinateCategoryId,
			CoordinateCategoryName = this.CoordinateCategoryName,
			DisplayOrder = Int32.Parse(this.DisplayOrder),
			SeoKeywords = this.SeoKeywords,
			ValidFlg = this.ValidFlg
				? Constants.FLG_COORDINATECATEGORY_VALID_FLG_VALID
				: Constants.FLG_COORDINATECATEGORY_VALID_FLG_INVALID,
			LastChanged = this.LastChanged
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="isUpdate">更新か</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(bool isUpdate)
	{
		var errorMessage = Validator.Validate(
			(isUpdate)
				? "CoordinateCategoryModify"
				: "CoordinateCategoryRegister",
			this.DataSource);

		if (isUpdate)
		{
			var service = new CoordinateCategoryService();
			var count = service.GetAll().Count(c => c.CoordinateCategoryName == this.CoordinateCategoryName);
			if(service.Get(this.CoordinateCategoryId).CoordinateCategoryName == this.CoordinateCategoryName) count--;
			if (count > 0) errorMessage += WebMessages.CoordinateCategoryDuplicateError;
		}

		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>カテゴリID</summary>
	public string CoordinateCategoryId
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID] = value; }
	}
	/// <summary>親カテゴリID</summary>
	public string CoordinateParentCategoryId
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_PARENT_CATEGORY_ID]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_PARENT_CATEGORY_ID] = value; }
	}
	/// <summary>カテゴリ名</summary>
	public string CoordinateCategoryName
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME] = value; }
	}
	/// <summary>表示順</summary>
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_DISPLAY_ORDER] = value; }
	}
	/// <summary>SEOキーワード</summary>
	public string SeoKeywords
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_SEO_KEYWORDS]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_SEO_KEYWORDS] = value; }
	}
	/// <summary>有効フラグ</summary>
	public bool ValidFlg
	{
		get { return (bool) (this.DataSource[Constants.FIELD_COORDINATECATEGORY_VALID_FLG] ?? false); }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_VALID_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_LAST_CHANGED] = value; }
	}
	/// <summary>整形されたカテゴリID</summary>
	public string FormattedCoordinateCategoryId
	{
		get { return (string)this.DataSource[Constants.FLG_COORDINATECATEGORY_FORMATTED_COORDINATE_CATEGORY_ID]; }
		set { this.DataSource[Constants.FLG_COORDINATECATEGORY_FORMATTED_COORDINATE_CATEGORY_ID] = value; }
	}
	#endregion
}
