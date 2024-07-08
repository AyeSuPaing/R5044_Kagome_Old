/*
=========================================================================================================
  Module      : 商品情報取込設定クラス(ImportSettingProduct.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingProduct : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_PRODUCT_SHOP_ID, Constants.FIELD_PRODUCT_PRODUCT_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_PRODUCT_DATE_CHANGED, Constants.FIELD_PRODUCT_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string>{ };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_PRODUCT_SHOP_ID, Constants.FIELD_PRODUCT_PRODUCT_ID };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_PRODUCT_SHOP_ID, Constants.FIELD_PRODUCT_PRODUCT_ID };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingProduct(string shopId)
			: base(
			shopId,
			Constants.TABLE_PRODUCT,
			Constants.TABLE_WORKPRODUCT,
			FIELDS_UPDKEY,
			FIELDS_EXCEPT,
			FIELDS_INCREASED_UPDATE,
			FIELDS_NECESSARY_FOR_INSERTUPDATE,
			FIELDS_NECESSARY_FOR_DELETE)
		{
			// 配送種別をセット
			this.ShopShippingsForCheck = GetShopShippings();
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			//------------------------------------------------------
			// データ変換
			//------------------------------------------------------
			foreach (string strFieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[strFieldName] = this.Data[strFieldName].ToString().Trim();

				// 半角変換
				switch (strFieldName)
				{
					case Constants.FIELD_PRODUCT_SHOP_ID:
					case Constants.FIELD_PRODUCT_PRODUCT_ID:
						this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
						break;
				}
			}

			if (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false)
			{
				this.Data[Constants.FIELD_PRODUCT_IMAGE_HEAD] = (string)this.Data[Constants.FIELD_PRODUCT_PRODUCT_ID];
			}

			// For case has fixed purchase next shipping item quantity field and value is empty
			if (this.Data.ContainsKey(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY)
				&& string.IsNullOrEmpty((string)this.Data[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY]))
			{
				this.Data[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY] = "0";
			}
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			//------------------------------------------------------
			// 入力チェック
			//------------------------------------------------------
			string strCheckKbn = null;
			List<string> lNecessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					strCheckKbn = "ProductInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "ProductDelete";
					lNecessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			StringBuilder sbErrorMessages = new StringBuilder();
			StringBuilder sbNecessaryFields = new StringBuilder();
			foreach (string strKeyField in lNecessaryFields)
			{
				if (this.HeadersCsv.Contains(strKeyField) == false)
				{
					sbNecessaryFields.Append((sbNecessaryFields.Length != 0) ? "," : "");
					sbNecessaryFields.Append(strKeyField);
				}
			}
			if (sbNecessaryFields.Length != 0)
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "");
				sbErrorMessages.Append("該当テーブルの更新にはフィールド「").Append(sbNecessaryFields.ToString()).Append("」が必須です。");
			}

			// 入力チェック
			string errorMessage = Validator.Validate(strCheckKbn, this.Data);
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCT_PRODUCT_ID);
			}

			// Check Input Product For Fixed Purchase Next Shipping Setting
			if (string.IsNullOrEmpty(errorMessage))
			{
				var productForFixedPurchaseNextShippingSettingErrorMessages = CheckInputProductForFixedPurchaseNextShippingSetting();
				if (productForFixedPurchaseNextShippingSettingErrorMessages.Length > 0)
				{
					sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : string.Empty)
						.Append(productForFixedPurchaseNextShippingSettingErrorMessages);
				}
			}

			// 配送種別存在チェック
			if (string.IsNullOrEmpty(errorMessage))
			{
				var shippingTypeErrorMessage = CheckShopShippingType();
				if (string.IsNullOrEmpty(shippingTypeErrorMessage) == false)
				{
					if (sbErrorMessages.Length != 0) sbErrorMessages.Append("\r\n");
					sbErrorMessages.Append(shippingTypeErrorMessage);
					this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCT_PRODUCT_ID);
				}
			}

			// 削除対象の商品が頒布会に含まれていないか
			if (string.IsNullOrEmpty(errorMessage) && (strCheckKbn == "ProductDelete"))
			{
				var includeSubscriptionBoxErrorMessage = CheckIncludSubscriptionBox();
				if (string.IsNullOrEmpty(includeSubscriptionBoxErrorMessage) == false)
				{
					if (sbErrorMessages.Length != 0) sbErrorMessages.Append("\r\n");
					sbErrorMessages.Append(includeSubscriptionBoxErrorMessage);
					this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCT_PRODUCT_ID);
				}
			}

			// 商品IDは既に「商品ID+バリエーションID」として使用されるかチェック
			if (string.IsNullOrEmpty(errorMessage) && (strCheckKbn == "ProductInsertUpdate"))
			{
				var errorMsg = CheckProductIdIsUsedAsVariationId();
				if (string.IsNullOrEmpty(errorMsg) == false)
				{
					if (sbErrorMessages.Length != 0) sbErrorMessages.Append("\r\n");
					sbErrorMessages.Append(errorMsg);
					this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCT_PRODUCT_ID);
				}
			}

			// エラーメッセージ格納
			if (sbErrorMessages.Length != 0)
			{
				this.ErrorMessages = sbErrorMessages.ToString();
			}
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";	// 整合性チェックしない
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			//------------------------------------------------------
			// 共通SQL文作成（※ImportSettingBaseで定義）
			//------------------------------------------------------
			base.CreateSql();
		}

		/// <summary>
		/// モール出品設定情報登録
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="htProductInput">モール出品設定情報を登録する商品情報</param>
		public int InsertMallExhibitsConfig(SqlAccessor sqlAccessor, Hashtable htProductInput)
		{
			// モール出品設定情報設定
			Hashtable htInput = new Hashtable();
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID] = htProductInput[Constants.FIELD_PRODUCT_SHOP_ID];
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID] = htProductInput[Constants.FIELD_PRODUCT_PRODUCT_ID];
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
			htInput[Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED] = Constants.IMPORT_LAST_CHANGED;

			//------------------------------------------------------
			// モール出品設定情報登録
			//------------------------------------------------------
			using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "InsertMallExhibitsConfig"))
			{
				return sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// モール出品設定情報削除
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="htProductInput">モール出品設定情報を削除する商品情報</param>
		public int DeleteMallExhibitsConfig(SqlAccessor sqlAccessor, Hashtable htInput)
		{
			//------------------------------------------------------
			// モール出品設定情報削除
			//------------------------------------------------------
			using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "DeleteMallExhibitsConfig"))
			{
				return sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// Check Input Product For Fixed Purchase Next Shipping Setting
		/// </summary>
		/// <returns>Error message</returns>
		private string CheckInputProductForFixedPurchaseNextShippingSetting()
		{
			var nextShippingProductChecker = new ProductFixedPurchaseNextShippingSettingChecker(
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_SHOP_ID]),
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_PRODUCT_ID]),
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_SHIPPING_TYPE]),
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID]),
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID]),
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY]),
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN]),
				StringUtility.ToEmpty(this.Data[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING]));

			var errorMessages = nextShippingProductChecker.Check();

			var result = string.Join("\r\n", errorMessages.Select(kv => kv.Value));

			return result;
		}

		/// <summary>
		/// 配送種別存在チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckShopShippingType()
		{
			if (string.IsNullOrEmpty((string)this.Data[Constants.FIELD_PRODUCT_SHIPPING_TYPE])) return "";

			if (this.ShopShippingsForCheck.ContainsKey((string)this.Data[Constants.FIELD_PRODUCT_SHIPPING_TYPE]) == false)
			{
				var errorMessage = MessageManager.GetMessages(CommerceMessages.ERRMSG_SHIPPING_TYPE_NOT_EXISTS)
					.Replace("@@ 1 @@", (string)this.Data[Constants.FIELD_PRODUCT_SHIPPING_TYPE]);
				return errorMessage;
			}

			return "";
		}

		/// <summary>
		/// 配送種別IDをキーとしたディクショナリで一括取得
		/// </summary>
		/// <returns>モデル</returns>
		private Dictionary<string, ShopShippingModel> GetShopShippings()
		{
			var shopShippings = new ShopShippingService().GetAll(this.ShopId);
			var result = shopShippings.ToDictionary(x => x.ShippingId, x => x);

			return result;
		}

		/// <summary>
		/// 削除対象の商品が頒布会に含まれていないか確認
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckIncludSubscriptionBox()
		{
			var result = new SubscriptionBoxService().GetSubscriptionItemByProductId(
				(string)this.Data[App.Common.Constants.FIELD_PRODUCT_SHOP_ID],
				(string)this.Data[Constants.FIELD_PRODUCT_PRODUCT_ID]);

			if (result.Length == 0) return "";
			var errorMessage = MessageManager.GetMessages(CommerceMessages.ERRMSG_DELETE_PRODUCT_INCLUDE_SUBSCRIPTION_BOX);
			return errorMessage;
		}

		/// <summary>
		/// 商品IDは既に「商品ID+バリエーションID」として使用されるかチェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckProductIdIsUsedAsVariationId()
		{
			var errorMessages = string.Empty;
			var product = new ProductService();

			// 商品IDが「商品ID＋バリエーションID」で使用されるかチェック
			var isUsed = product.CheckProductIdIsUsedAsVariationId(
				(string)this.Data[App.Common.Constants.FIELD_PRODUCT_SHOP_ID],
				(string)this.Data[Constants.FIELD_PRODUCT_PRODUCT_ID]);

			if (isUsed)
			{
				errorMessages = MessageManager.GetMessages(
					CommerceMessages.ERRMSG_MANAGER_PRODUCT_ID_IS_USED_AS_VARIATION_ID_ERROR);
			}

			return errorMessages;
		}

		/// <summary>配送種別IDをキーとしたディクショナリ</summary>
		private Dictionary<string, ShopShippingModel> ShopShippingsForCheck { get; set; }
	}
}
