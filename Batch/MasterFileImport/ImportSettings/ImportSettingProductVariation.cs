/*
=========================================================================================================
  Module      : 商品バリエーション情報取込設定クラス(ImportSettingProductVariation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.App.Common;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingProductVariation : ImportSettingBase
	{
		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string> { Constants.FIELD_PRODUCTVARIATION_SHOP_ID, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID };
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string> { Constants.FIELD_PRODUCTVARIATION_DATE_CHANGED, Constants.FIELD_PRODUCTVARIATION_LAST_CHANGED };
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string> { };
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string> { Constants.FIELD_PRODUCTVARIATION_SHOP_ID, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID };
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string> { Constants.FIELD_PRODUCTVARIATION_SHOP_ID, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingProductVariation(string shopId)
			: base(
			shopId,
			Constants.TABLE_PRODUCTVARIATION,
			Constants.TABLE_WORKPRODUCTVARIATION,
			FIELDS_UPDKEY,
			FIELDS_EXCEPT,
			FIELDS_INCREASED_UPDATE,
			FIELDS_NECESSARY_FOR_INSERTUPDATE,
			FIELDS_NECESSARY_FOR_DELETE)
		{
			// 何もしない
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
					case Constants.FIELD_PRODUCTVARIATION_SHOP_ID:
					case Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID:
					case Constants.FIELD_PRODUCTVARIATION_VARIATION_ID:
						this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
						break;
				}
			}

			if (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false)
			{
				this.Data[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] =
					(string)this.Data[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID]
					+ "_var"
					+ ((string)this.Data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
					.Replace((string)this.Data[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID], "");
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
					strCheckKbn = "ProductVariationInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "ProductVariationDelete";
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

			// バリエーションIDチェック
			errorMessage += CheckVariationId(errorMessage);

			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID);
			}

			// 削除対象の商品が頒布会に含まれていないか
			if (string.IsNullOrEmpty(errorMessage) && (strCheckKbn == "ProductVariationDelete"))
			{
				var includSubscriptionBoxErrorMessage = CheckIncludSubscriptionBox();
				if (string.IsNullOrEmpty(includSubscriptionBoxErrorMessage) == false)
				{
					if (sbErrorMessages.Length != 0) sbErrorMessages.Append("\r\n");
					sbErrorMessages.Append(includSubscriptionBoxErrorMessage);
					this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCT_PRODUCT_ID);
				}
			}

			// 入力された商品ID+バリエーションIDが使用されるかチェック
			if (string.IsNullOrEmpty(errorMessage) && (strCheckKbn == "ProductVariationInsertUpdate"))
			{
				var errorMsg = CheckVariationIdIsUsed();
				if (string.IsNullOrEmpty(errorMsg) == false)
				{
					if (sbErrorMessages.Length != 0) sbErrorMessages.Append("\r\n");
					sbErrorMessages.Append(errorMsg);
					this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCT_PRODUCT_ID);
					this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_PRODUCT_VARIATION_ID);
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
		/// 削除対象の商品が頒布会に含まれていないか確認
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckIncludSubscriptionBox()
		{
			var result = new SubscriptionBoxService().GetSubscriptionItemByProductVariationId(
				(string)this.Data[App.Common.Constants.FIELD_PRODUCT_SHOP_ID],
				(string)this.Data[Constants.FIELD_PRODUCT_PRODUCT_ID],
				(string)this.Data[Constants.FIELD_PRODUCT_VARIATION_ID]);

			if (result.Length == 0) return "";
			var errorMessage = MessageManager.GetMessages(CommerceMessages.ERRMSG_DELETE_PRODUCT_INCLUDE_SUBSCRIPTION_BOX);
			return errorMessage;
		}

		/// <summary>
		/// 入力された商品ID+バリエーションIDが使用されるかチェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckVariationIdIsUsed()
		{
			var errorMessages = string.Empty;
			var product = new ProductService();

			// 入力された商品ID＋バリエーションIDは既に「商品ID」として使用されるかチェック
			errorMessages += product.CheckVariationIdIsUsedAsProductId(
				(string)this.Data[Constants.FIELD_PRODUCTVARIATION_SHOP_ID],
				(string)this.Data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
				? MessageManager.GetMessages(
					CommerceMessages.ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_IS_USED_AS_PRODUCT_ID_ERROR)
				: "";

			// 新規バリエーションID追加の場合
			// 入力された商品ID＋バリエーションIDは既に「商品ID+バリエーショID」として使用されるかチェック
			if (product.CountProductView(
					(string)this.Data[Constants.FIELD_PRODUCTVARIATION_SHOP_ID],
					(string)this.Data[Constants.FIELD_PRODUCT_PRODUCT_ID],
					(string)this.Data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]) == 0)
			{
				errorMessages += product.CheckVariationIdIsUsedAsVariationId(
					(string)this.Data[Constants.FIELD_PRODUCTVARIATION_SHOP_ID],
					(string)this.Data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
					? MessageManager.GetMessages(
						CommerceMessages.ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_IS_USED_AS_VARIATION_ID_ERROR)
					: "";
			}

			return errorMessages;
		}
	}
}
