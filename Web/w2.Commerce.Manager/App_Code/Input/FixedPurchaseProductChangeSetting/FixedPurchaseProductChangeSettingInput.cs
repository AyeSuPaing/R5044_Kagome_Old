/*
=========================================================================================================
  Module      : 定期商品変更設定入力クラス (FixedPurchaseProductChangeSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Domain.FixedPurchaseProductChangeSetting;
using w2.Domain.FixedPurchaseProductChangeSetting.Helper;

/// <summary>
/// 定期商品変更設定入力クラス
/// </summary>
[Serializable]
public class FixedPurchaseProductChangeSettingInput : InputBase<FixedPurchaseProductChangeSettingModel>
{
	// 定期変更商品ID：エラーメッセージ出力用
	const string CONST_REPLACE_NAME_FIXED_PURCHASE_PRODUCT_CHANGE_ID = "定期変更商品ID";
	// 変更元商品設定：エラーメッセージ出力用
	const string CONST_REPLACE_NAME_BEFORE_ITEMS_SETTING = "変更元商品設定";
	// 変更後商品設定：エラーメッセージ出力用
	const string CONST_REPLACE_NAME_AFTER_ITEMS_SETTING = "変更後商品設定";

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public FixedPurchaseProductChangeSettingInput()
	{
	}
	public FixedPurchaseProductChangeSettingInput(FixedPurchaseProductChangeSettingContainer model)
	{
		this.FixedPurchaseProductChangeId = model.FixedPurchaseProductChangeId;
		this.FixedPurchaseProductChangeName = model.FixedPurchaseProductChangeName;
		this.Priority = model.Priority.ToString();
		this.ValidFlg = model.ValidFlg;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.BeforeChangeItems = model.BeforeChangeItemContainers.Select(beforeChangeItem => new FixedPurchaseBeforeChangeItemInput(beforeChangeItem)).ToList();
		this.AfterChangeItems = model.AfterChangeItemContainers.Select(affterChangeItem => new FixedPurchaseAfterChangeItemInput(affterChangeItem)).ToList();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>定期商品変更設定モデル</returns>
	public override FixedPurchaseProductChangeSettingModel CreateModel()
	{
		return new FixedPurchaseProductChangeSettingContainer
		{
			FixedPurchaseProductChangeId = this.FixedPurchaseProductChangeId,
			FixedPurchaseProductChangeName = this.FixedPurchaseProductChangeName,
			Priority = int.Parse(this.Priority),
			ValidFlg = this.ValidFlg,
			BeforeChangeItems = this.BeforeChangeItems.Select(
				beforeChangeItem =>
				{
					var model = beforeChangeItem.CreateModel();
					model.FixedPurchaseProductChangeId = this.FixedPurchaseProductChangeId;
					return model;
				}).ToArray(),
			AfterChangeItems = this.AfterChangeItems.Select(
				afterChangeItem =>
				{
					var model = afterChangeItem.CreateModel();
					model.FixedPurchaseProductChangeId = this.FixedPurchaseProductChangeId;
					return model;
				}).ToArray(),
			LastChanged = this.LastChanged,
		};
	}

	/// <summary>
	/// 入力値チェック
	/// </summary>
	/// <returns></returns>
	public bool Validate(bool isInsert)
	{
		var errorMessages = new StringBuilder();
		errorMessages.Append(Validator.Validate("FixedPurchaseProductChangeSetting", this.DataSource));

		// 重複チェック
		if (isInsert && new FixedPurchaseProductChangeSettingService().Get(this.FixedPurchaseProductChangeId) != null)
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION)
				.Replace("@@ 1 @@", CONST_REPLACE_NAME_FIXED_PURCHASE_PRODUCT_CHANGE_ID)).Append("<br />");
		}

		// 定期変更元商品の個数チェック
		if ((this.BeforeChangeItems == null) || (this.BeforeChangeItems.Count < 1))
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY).Replace("@@ 1 @@", CONST_REPLACE_NAME_BEFORE_ITEMS_SETTING)).Append("<br />");
		}
		else
		{
			// 定期変更元商品の重複チェック
			var isBeforeChangeItemsDuplicate = this.BeforeChangeItems
				.GroupBy(
					beforeChangeItem => new
					{
						beforeChangeItem.ShopId,
						beforeChangeItem.ProductId,
						beforeChangeItem.VariationId,
						beforeChangeItem.ItemUnitType
					})
				.Any(beforeChangeItem => beforeChangeItem.Count() > 1);
			if (isBeforeChangeItemsDuplicate)
			{
				errorMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_BEFORE_CHANGE_ITEMS_DUPLICATION)).Append("<br />");
			}
		}

		var afterItemShippingType = string.Empty;
		// 定期変更後商品の個数チェック
		if ((this.AfterChangeItems == null) || (this.AfterChangeItems.Count < 1))
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY).Replace("@@ 1 @@", CONST_REPLACE_NAME_AFTER_ITEMS_SETTING));
		}
		else
		{
			// 定期変更元商品の重複チェック
			var isAfterChangeItemsDuplicate = this.AfterChangeItems
				.GroupBy(
					afterChangeItem => new
					{
						afterChangeItem.ShopId,
						afterChangeItem.ProductId,
						afterChangeItem.VariationId,
						afterChangeItem.ItemUnitType
					})
				.Any(afterChangeItem => afterChangeItem.Count() > 1);

			if (isAfterChangeItemsDuplicate)
			{
				errorMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_AFTER_CHANGE_ITEMS_DUPLICATION)).Append("<br />");
			}
		}

		// 配送種別チェック
		if ((this.BeforeChangeItems != null) && (this.AfterChangeItems != null))
		{
			var shippingTypes = this.BeforeChangeItems.Select(beforeChangeItem => beforeChangeItem.ShippingType).ToList();
			shippingTypes.AddRange(this.AfterChangeItems.Select(afterChangeItem => afterChangeItem.ShippingType));
			if (shippingTypes.GroupBy(shipipngtype => shipipngtype).Count() != 1)
			{
				errorMessages.AppendLine(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_SHIPPIING_TYPE_ERROR)).Append("<br />");
			}
		}

		// エラーメッセージをセット
		this.ErrorMessages = errorMessages.ToString();
		return this.ErrorMessages.Length == 0;
	}
	#endregion

	#region プロパティ
	/// <summary>定期商品変更ID</summary>
	public string FixedPurchaseProductChangeId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID] = value; }
	}
	/// <summary>定期商品変更名</summary>
	public string FixedPurchaseProductChangeName
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME] = value; }
	}
	/// <summary>適用優先順</summary>
	public string Priority
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_PRIORITY]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_PRIORITY] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_LAST_CHANGED] = value; }
	}
	/// <summary>定期変更元商品</summary>
	public List<FixedPurchaseBeforeChangeItemInput> BeforeChangeItems { get; set; }
	/// <summary>定期変更後商品</summary>
	public List<FixedPurchaseAfterChangeItemInput> AfterChangeItems { get; set; }
	/// <summary>エラーメッセージ</summary>
	public string ErrorMessages { get; set; }
	#endregion
}
