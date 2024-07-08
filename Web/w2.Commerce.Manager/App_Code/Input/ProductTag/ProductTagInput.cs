/*
=========================================================================================================
  Module      : 商品タグ入力クラス (ProductTagInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Input;
using w2.Domain.ProductTag;

/// <summary>
/// 商品タグ入力クラス
/// </summary>
[Serializable]
public class ProductTagInput : InputBase<ProductTagModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductTagInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductTagInput(ProductTagModel model)
		: this()
	{
		this.ProductId = model.ProductId;
		this.LastChanged = model.LastChanged;

		CreateProductTagIdsAndTagValuesFromSource(model.DataSource);
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductTagModel CreateModel()
	{
		var model = new ProductTagModel
		{
			ProductId = StringUtility.ToEmpty(this.ProductId),
			ProductTagIds = this.ProductTagIds,
			ProductTagValues = this.ProductTagValues,
			LastChanged = StringUtility.ToEmpty(this.LastChanged)
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <returns>Error messages as dictionary</returns>
	public Dictionary<string, string> Validate()
	{
		var errorMessagesContainer = new Dictionary<string, string>();
		for (var index = 0; index < this.ProductTagIds.Length; index++)
		{
			var productTagId = this.ProductTagIds[index];
			var errorMessage = Validator.CheckLengthMaxError(
				string.Format("{0}({1})", this.ProductTagNames[index], productTagId),
				this.ProductTagValues[index],
				100);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessagesContainer.Add(productTagId, errorMessage);
			}
		}

		return errorMessagesContainer;
	}

	/// <summary>
	/// Create product tag ids and tag values from source
	/// </summary>
	/// <param name="source">The source</param>
	private void CreateProductTagIdsAndTagValuesFromSource(Hashtable source)
	{
		var productTagIds = new List<string>();
		var productTagValues = new List<string>();
		foreach (DictionaryEntry item in source)
		{
			if (item.Key.ToString().StartsWith("tag_") == false) continue;

			productTagIds.Add(item.Key.ToString());
			productTagValues.Add(item.Value.ToString());
		}
		this.ProductTagIds = productTagIds.ToArray();
		this.ProductTagValues = productTagValues.ToArray();
	}
	#endregion

	#region プロパティ
	/// <summary>商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTTAG_PRODUCT_ID)]
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAG_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAG_PRODUCT_ID] = value; }
	}
	/// <summary>Product tag IDs</summary>
	[JsonProperty("product_tag_ids")]
	public string[] ProductTagIds
	{
		get { return (string[])this.DataSource["product_tag_ids"]; }
		set { this.DataSource["product_tag_ids"] = value; }
	}
	/// <summary>Product tag names</summary>
	[JsonProperty("product_tag_names")]
	public string[] ProductTagNames
	{
		get { return (string[])this.DataSource["product_tag_names"]; }
		set { this.DataSource["product_tag_names"] = value; }
	}
	/// <summary>Product tag values</summary>
	[JsonProperty("product_tag_values")]
	public string[] ProductTagValues
	{
		get { return (string[])this.DataSource["product_tag_values"]; }
		set { this.DataSource["product_tag_values"] = value; }
	}
	/// <summary>最終更新者</summary>
	[JsonProperty(Constants.FIELD_PRODUCTTAG_LAST_CHANGED)]
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAG_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAG_LAST_CHANGED] = value; }
	}
	#endregion
}