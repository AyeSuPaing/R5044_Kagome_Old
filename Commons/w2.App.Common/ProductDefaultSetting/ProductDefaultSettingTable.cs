/*
=========================================================================================================
  Module      : 商品初期設定テーブル格納クラス(ProductDefaultSettingTable.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;

namespace w2.App.Common.ProductDefaultSetting
{
	/// <summary>
	/// 商品初期設定テーブル格納クラス
	/// </summary>
	public class ProductDefaultSettingTable
	{
		/// <summary>Field product relations</summary>
		private readonly string[] FIELD_PRODUCT_RELATIONS = new[]
		{
			Constants.FIELD_PRODUCT_BRAND_ID1,
			Constants.FIELD_PRODUCT_BRAND_ID2,
			Constants.FIELD_PRODUCT_BRAND_ID3,
			Constants.FIELD_PRODUCT_BRAND_ID4,
			Constants.FIELD_PRODUCT_BRAND_ID5,
			Constants.FIELD_PRODUCT_CATEGORY_ID1,
			Constants.FIELD_PRODUCT_CATEGORY_ID2,
			Constants.FIELD_PRODUCT_CATEGORY_ID3,
			Constants.FIELD_PRODUCT_CATEGORY_ID4,
			Constants.FIELD_PRODUCT_CATEGORY_ID5,
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1,
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2,
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3,
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4,
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5,
			Constants.FIELD_PRODUCT_IMAGE_HEAD,
		};
		/// <summary>Field product icons</summary>
		private static readonly string[] FIELD_PRODUCT_ICONS = new[]
		{
			Constants.FIELD_PRODUCT_ICON_FLG1,
			Constants.FIELD_PRODUCT_ICON_TERM_END1,
			Constants.FIELD_PRODUCT_ICON_FLG2,
			Constants.FIELD_PRODUCT_ICON_TERM_END2,
			Constants.FIELD_PRODUCT_ICON_FLG3,
			Constants.FIELD_PRODUCT_ICON_TERM_END3,
			Constants.FIELD_PRODUCT_ICON_FLG4,
			Constants.FIELD_PRODUCT_ICON_TERM_END4,
			Constants.FIELD_PRODUCT_ICON_FLG5,
			Constants.FIELD_PRODUCT_ICON_TERM_END5,
			Constants.FIELD_PRODUCT_ICON_FLG6,
			Constants.FIELD_PRODUCT_ICON_TERM_END6,
			Constants.FIELD_PRODUCT_ICON_FLG7,
			Constants.FIELD_PRODUCT_ICON_TERM_END8,
			Constants.FIELD_PRODUCT_ICON_FLG9,
			Constants.FIELD_PRODUCT_ICON_TERM_END9,
			Constants.FIELD_PRODUCT_ICON_FLG10,
			Constants.FIELD_PRODUCT_ICON_TERM_END10,
		};
		/// <summary>Field product display to for check</summary>
		public const string FIELD_PRODUCT_DISPLAY_TO_FOR_CHECK = Constants.FIELD_PRODUCT_DISPLAY_TO + "_check";
		/// <summary>Field product sell to for check</summary>
		public const string FIELD_PRODUCT_SELL_TO_FOR_CHECK = Constants.FIELD_PRODUCT_SELL_TO + "_check";
		/// <summary>Field product brand id 1 for check</summary>
		public const string FIELD_PRODUCT_BRAND_ID1_FOR_CHECK = Constants.FIELD_PRODUCT_BRAND_ID1 + "_for_check";
		/// <summary>Field product for check list</summary>
		public static readonly string[] FIELD_PRODUCT_FOR_CHECK_LIST = new[]
		{
			FIELD_PRODUCT_DISPLAY_TO_FOR_CHECK,
			FIELD_PRODUCT_SELL_TO_FOR_CHECK,
			FIELD_PRODUCT_BRAND_ID1_FOR_CHECK,
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		public ProductDefaultSettingTable(string tableName)
		{
			this.Name = tableName;
			this.Fields = new Dictionary<string, ProductDefaultSettingField>();
		}

		/// <summary>
		/// 特定フィールドのデフォルト取得
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="defaultObject">デフォルト値</param>
		/// <param name="comment">項目メモ</param>
		/// <param name="display">項目表示有無</param>
		public void Add(string fieldName, object defaultObject, string comment, bool display)
		{
			this.Fields.Add(
				fieldName,
				new ProductDefaultSettingField(
					fieldName,
					(defaultObject != null) ? defaultObject.ToString() : null,
					StringUtility.ToEmpty(comment),
					display));
		}

		/// <summary>
		/// 特定フィールドのデフォルト取得
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>デフォルト値</returns>
		public string GetDefault(string fieldName)
		{
			if (this.Fields.ContainsKey(fieldName))
			{
				return this.Fields[fieldName].Default;
			}

			return null;
		}

		/// <summary>
		/// 特定フィールドの表示状態取得(取得できなければ表示）
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>項目表示有無</returns>
		public bool GetDisplay(string fieldName)
		{
			if (this.Fields.ContainsKey(fieldName))
			{
				return this.Fields[fieldName].Display;
			}

			return true;
		}

		/// <summary>
		/// 特定フィールドのコメント取得
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>項目メモ</returns>
		public string GetComment(string fieldName)
		{
			if (this.Fields.ContainsKey(fieldName))
			{
				return this.Fields[fieldName].Comment;
			}

			return string.Empty;
		}

		/// <summary>
		/// フィールドのデフォルト値をDictionary&lt;string, string&gt;形式で取得
		/// </summary>
		/// <param name="isRemoveRelationAndIconField">Is remove relation and icon field</param>
		/// <returns>Field default values</returns>
		public Dictionary<string, string> GetFieldDefaultValues(bool isRemoveRelationAndIconField = false)
		{
			var fieldDefaultValues = this.Fields.Values.ToDictionary(pdsf => pdsf.Name, pdsf => pdsf.Default);
			if (isRemoveRelationAndIconField)
			{
				fieldDefaultValues = fieldDefaultValues
					.Where(pdsf => ((FIELD_PRODUCT_RELATIONS.Contains(pdsf.Key) == false)
						&& (FIELD_PRODUCT_ICONS.Contains(pdsf.Key) == false)))
					.ToDictionary(item => item.Key, item => item.Value);
			}
			return fieldDefaultValues;
		}

		/// <summary>
		/// Get product relation field default values
		/// </summary>
		/// <returns>Relation field default values</returns>
		public Dictionary<string, string> GetProductRelationFieldDefaultValues()
		{
			var fieldDefaultValues = GetFieldDefaultValues()
				.Where(pdsf => FIELD_PRODUCT_RELATIONS.Contains(pdsf.Key))
				.ToDictionary(item => item.Key, item => item.Value);
			return fieldDefaultValues;
		}

		/// <summary>
		/// Get product icon flag field default values
		/// </summary>
		/// <returns>Product icon flag field default values</returns>
		public Dictionary<string, string> GetProductIconFlagFieldDefaultValues()
		{
			var fieldDefaultValues = GetFieldDefaultValues()
				.Where(pdsf => FIELD_PRODUCT_ICONS.Contains(pdsf.Key))
				.ToDictionary(item => item.Key, item => item.Value);
			return fieldDefaultValues;
		}

		/// <summary>テーブル名</summary>
		public string Name { get; set; }
		/// <summary>フィールド配列</summary>
		public Dictionary<string, ProductDefaultSettingField> Fields { get; set; }
	}
}
