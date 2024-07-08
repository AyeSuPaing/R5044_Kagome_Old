/*
=========================================================================================================
  Module      : 商品グループ入力クラス (ProductGroupInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.ProductGroup;

/// <summary>
/// 商品グループ入力クラス
/// </summary>
public class ProductGroupInput : InputBase<ProductGroupModel>
{
	#region 列挙体
	/// <summary>入力チェック区分</summary>
	public enum EnumProductGroupInputValidationKbn
	{
		/// <summary>登録</summary>
		Register = 0,
		/// <summary>更新</summary>
		Update = 1
	};
	#endregion

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductGroupInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductGroupInput(ProductGroupModel model)
		: this()
	{
		this.ProductGroupId = model.ProductGroupId;
		this.ProductGroupName = model.ProductGroupName;
		this.BeginDate = (model.BeginDate != null) ? model.BeginDate.ToString() : null;
		this.EndDate = (model.EndDate != null) ? model.EndDate.ToString() : null;
		this.ValidFlg = model.ValidFlg;
		this.ProductGroupPageContentsKbn = model.ProductGroupPageContentsKbn;
		this.ProductGroupPageContents = model.ProductGroupPageContents;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductGroupModel CreateModel()
	{
		var model = new ProductGroupModel
		{
			ProductGroupId = this.ProductGroupId,
			ProductGroupName = this.ProductGroupName,
			BeginDate = DateTime.Parse(this.BeginDate),
			EndDate = (this.EndDate != null) ? DateTime.Parse(this.EndDate) : (DateTime?)null,
			ValidFlg = this.ValidFlg,
			ProductGroupPageContentsKbn = this.ProductGroupPageContentsKbn,
			ProductGroupPageContents = this.ProductGroupPageContents,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="validationKbn">入力チェック区分</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(EnumProductGroupInputValidationKbn validationKbn)
	{
		var errorMessage = Validator.Validate("ProductGroupRegister", this.DataSource);
		if ((errorMessage.Length == 0) && (validationKbn == EnumProductGroupInputValidationKbn.Register))
		{
			errorMessage = CheckDupulicationProductGroupId();
		}
		 return errorMessage;
	}

	/// <summary>
	/// 商品グループID重複チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string CheckDupulicationProductGroupId()
	{
		string errorMessage = string.Empty;

		if (new ProductGroupService().CheckDupulicationProductGroupId(this.ProductGroupId) == false)
		{
			errorMessage = WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", "商品グループID");
		}
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>商品グループID</summary>
	public string ProductGroupId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID] = value; }
	}
	/// <summary>商品グループ名</summary>
	public string ProductGroupName
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_NAME]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_NAME] = value; }
	}
	/// <summary>開始日時</summary>
	public string BeginDate
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_BEGIN_DATE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_BEGIN_DATE] = value; }
	}
	/// <summary>終了日時</summary>
	public string EndDate
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_END_DATE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_END_DATE] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_VALID_FLG] = value; }
	}
	/// <summary>商品グループページ表示内容HTML区分</summary>
	public string ProductGroupPageContentsKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN] = value; }
	}
	/// <summary>商品グループページ表示内容</summary>
	public string ProductGroupPageContents
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTGROUP_LAST_CHANGED] = value; }
	}
	#endregion
}
