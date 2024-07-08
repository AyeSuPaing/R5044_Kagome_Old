/*
=========================================================================================================
  Module      : 商品初期値設定クラス(ProductDefaultSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using w2.App.Common.Web.Process;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.ProductDefaultSetting;

namespace w2.App.Common.ProductDefaultSetting
{
	/// <summary>
	/// 商品初期設定格納クラス
	/// </summary>
	public class ProductDefaultSetting
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductDefaultSetting()
		{
			this.Tables = new Dictionary<string, ProductDefaultSettingTable>();
		}

		/// <summary>
		/// Validate product
		/// </summary>
		/// <returns>Error message</returns>
		public string ValidateProduct()
		{
			var errorMessageList = Validator.Validate("ProductRegist", this.Product.GetFieldDefaultValues(true));
			var errorMessage = string.Join(
				Environment.NewLine,
				errorMessageList.Select(kvp => kvp.Value.Replace("@@ 1 @@", string.Empty)));
			return errorMessage;
		}

		/// <summary>
		/// Validate product relations
		/// </summary>
		/// <returns>Error message</returns>
		public string ValidateProductRelations()
		{
			var errorMessageList = Validator.Validate("ProductRegist", this.Product.GetProductRelationFieldDefaultValues());
			var errorMessage = string.Join(
				Environment.NewLine,
				errorMessageList.Select(kvp => kvp.Value.Replace("@@ 1 @@", string.Empty)));
			return errorMessage;
		}

		/// <summary>
		/// Validate product icons
		/// </summary>
		/// <returns>Error message</returns>
		public string ValidateProductIcons()
		{
			var errorMessageList = Validator.Validate("ProductRegist", this.Product.GetProductIconFlagFieldDefaultValues());
			var errorMessage = string.Join(
				Environment.NewLine,
				errorMessageList.Select(kvp => kvp.Value.Replace("@@ 1 @@", string.Empty)));
			return errorMessage;
		}

		/// <summary>
		/// Validate product variation
		/// </summary>
		/// <returns>Error message</returns>
		public string ValidateProductVariation()
		{
			var errorMessageList = Validator.Validate("ProductVariationRegist", this.ProductVariation.GetFieldDefaultValues());
			var variationErrorMessage = string.Format(
				CommonPageProcess.ReplaceTagByLocaleId("@@DispText.product_error_message.variation@@", Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE),
				1);
			var errorMessage = string.Join(
				Environment.NewLine,
				errorMessageList.Select(kvp => kvp.Value.Replace("@@ 1 @@", variationErrorMessage)));
			return errorMessage;
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="shopId">ショップID</param>
		public void LoadSetting(string shopId)
		{
			// 商品デフォルト設定情報取得
			var setting = DomainFacade.Instance.ProductDefaultSettingService.GetByShopId(shopId);

			// XML構造格納
			if (setting == null) return;

			// 項目各データをセッションへ格納
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(setting.InitData);

			foreach (XmlNode xnTable in xmlDoc.SelectSingleNode("ProductDefaultSetting").ChildNodes)
			{
				if (xnTable.NodeType == XmlNodeType.Comment) continue;

				var pdstTable = new ProductDefaultSettingTable(xnTable.Name);
				foreach (XmlNode xnField in xnTable.SelectNodes("Field"))
				{
					var pdsfField = new ProductDefaultSettingField(
						xnField.Attributes["Name"].Value,
						(xnField["Default"] == null) ? (string)null : xnField["Default"].InnerText,
						xnField["Comment"].InnerText,
						(xnField["Display"].InnerText == "1"));

					pdstTable.Fields.Add(xnField.Attributes["Name"].Value, pdsfField);
				}

				this.Tables.Add(pdstTable.Name, pdstTable);
			}
		}

		/// <summary>
		/// 商品デフォルト設定更新
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="operatorName">オペレータ名</param>
		public void UpdateProductDefaultSetting(string shopId, string operatorName)
		{
			// 商品デフォルト設定XML作成
			var stringWriter = new StringWriter();
			var xmlTextWriter = new XmlTextWriter(stringWriter);

			xmlTextWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");
			xmlTextWriter.WriteStartElement("ProductDefaultSetting");

			foreach (var table in this.Tables.Values)
			{
				xmlTextWriter.WriteStartElement(table.Name);

				foreach (var field in table.Fields.Values)
				{
					// Remove field settings for check
					if (ProductDefaultSettingTable.FIELD_PRODUCT_FOR_CHECK_LIST.Contains(field.Name)) continue;

					xmlTextWriter.WriteStartElement("Field");
					{
						xmlTextWriter.WriteStartAttribute("Name");
						xmlTextWriter.WriteString(field.Name);
						xmlTextWriter.WriteEndAttribute();
					}

					if (field.Default != null)
					{
						xmlTextWriter.WriteStartElement("Default");
						xmlTextWriter.WriteCData(field.Default);
						xmlTextWriter.WriteEndElement();
					}

					xmlTextWriter.WriteStartElement("Comment");
					xmlTextWriter.WriteCData(field.Comment);
					xmlTextWriter.WriteEndElement();

					xmlTextWriter.WriteStartElement("Display");
					xmlTextWriter.WriteString(field.Display ? "1" : "0");
					xmlTextWriter.WriteEndElement();

					xmlTextWriter.WriteEndElement();
				}

				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();

			// 登録/更新
			var setting = new ProductDefaultSettingModel
			{
				ShopId = shopId,
				InitData = stringWriter.ToString(),
				LastChanged = operatorName
			};
			DomainFacade.Instance.ProductDefaultSettingService.Upsert(setting);
		}

		/// <summary>商品初期設定格納一覧</summary>
		public Dictionary<string, ProductDefaultSettingTable> Tables { get; set; }
		/// <summary>商品マスタデフォルト設定</summary>
		public ProductDefaultSettingTable Product
		{
			get { return (ProductDefaultSettingTable)this.Tables[Constants.TABLE_PRODUCT]; }
			set { this.Tables[Constants.TABLE_PRODUCT] = value; }
		}
		/// <summary>Has product setting</summary>
		public bool HasProductSetting
		{
			get { return this.Tables.ContainsKey(Constants.TABLE_PRODUCT); }
		}
		/// <summary>商品バリエーションマスタデフォルト設定</summary>
		public ProductDefaultSettingTable ProductVariation
		{
			get { return (ProductDefaultSettingTable)this.Tables[Constants.TABLE_PRODUCTVARIATION]; }
			set { this.Tables[Constants.TABLE_PRODUCTVARIATION] = value; }
		}
		/// <summary>Has product variation setting</summary>
		public bool HasProductVariationSetting
		{
			get { return this.Tables.ContainsKey(Constants.TABLE_PRODUCTVARIATION); }
		}
	}
}
