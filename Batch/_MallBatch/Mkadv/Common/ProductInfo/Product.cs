/*
=========================================================================================================
  Module      : 商品情報クラス(Product.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Logger;

namespace w2.Commerce.MallBatch.Mkadv.Common.ProductInfo
{
	///**************************************************************************************
	/// <summary>
	/// 商品情報クラス
	/// </summary>
	///**************************************************************************************
	public class Product
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Product()
		{
			// プロパティ初期化
			this.Products = new List<Hashtable>();
			this.HtYhoProductVariationInfo = new Hashtable();
			this.HtRtnCategoryInfo = new Hashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strShopId">ショップID</param>
		/// <param name="strTargetName">ターゲット名</param>
		/// <param name="strExtractionPatternId">引き抜きパターンID</param>
		/// <param name="blNeedYhoVariationInfo">Yahoo!バリエーション商品情報有無</param>
		/// <param name="blNeedRtnCategoryInfo">楽天カテゴリ商品情報有無</param>
		/// <param name="blIsTmpTableUse">テンポラリテーブル仕様有無</param>
		/// <param name="strMallId">モールID</param>
		/// <remarks>モール毎の商品商品情報を各モールのクラスへ設定する</remarks>
		public Product(
			SqlAccessor sqlAccessor,
			string strShopId,
			string strTargetName,
			string strExtractionPatternId,
			bool blIsNeedYhoVariationInfo,
			bool blIsNeedRtnCategoryInfo,
			bool blIsTmpTableUse,
			string strMallId) : this()
		{
			InitProduct(
				sqlAccessor,
				strShopId,
				strTargetName,
				strExtractionPatternId,
				blIsNeedYhoVariationInfo,
				blIsNeedRtnCategoryInfo,
				blIsTmpTableUse,
				strMallId);
		}

		/// <summary>
		/// モール毎の商品商品情報を各モールのクラスへ設定する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strShopId">ショップID</param>
		/// <param name="strTargetName">ターゲット名</param>
		/// <param name="strExtractionPatternId">引き抜きパターンID</param>
		/// <param name="blNeedYhoVariationInfo">Yahoo!バリエーション商品情報有無</param>
		/// <param name="blNeedRtnCategoryInfo">楽天カテゴリ商品情報有無</param>
		/// <param name="blIsTmpTableUse">テンポラリテーブル仕様有無</param>
		/// <param name="strMallId">モールID</param>
		private void InitProduct(
			SqlAccessor sqlAccessor, 
			string strShopId, 
			string strTargetName, 
			string strExtractionPatternId,  
			bool blIsNeedYhoVariationInfo, 
			bool blIsNeedRtnCategoryInfo, 
			bool blIsTmpTableUse, 
			string strMallId)
		{
			DataView dvProducts = new DataView();

			// 楽天カテゴリ用商品情報
			if (blIsNeedRtnCategoryInfo)
			{
				// 楽天カテゴリ情報取得
				DataView dvRtnCategory = GetProduct(sqlAccessor, strShopId, "RakutenCategory", strExtractionPatternId, strMallId);

				foreach (DataRowView drvRtnCategory in dvRtnCategory)
				{
					string[] strRtnCategorys = new string[5];
					strRtnCategorys[0] = StringUtility.ToEmpty(drvRtnCategory[Constants.FIELD_PRODUCTEXTEND_EXTEND112]);
					strRtnCategorys[1] = StringUtility.ToEmpty(drvRtnCategory[Constants.FIELD_PRODUCTEXTEND_EXTEND113]);
					strRtnCategorys[2] = StringUtility.ToEmpty(drvRtnCategory[Constants.FIELD_PRODUCTEXTEND_EXTEND114]);
					strRtnCategorys[3] = StringUtility.ToEmpty(drvRtnCategory[Constants.FIELD_PRODUCTEXTEND_EXTEND115]);
					strRtnCategorys[4] = StringUtility.ToEmpty(drvRtnCategory[Constants.FIELD_PRODUCTEXTEND_EXTEND116]);

					foreach (string strRtnCategory in strRtnCategorys)
					{
						if (strRtnCategory != "")
						{
							Hashtable htParam = new Hashtable();

							// カラムごとに処理
							for (int iLoop = 0; iLoop < drvRtnCategory.Row.Table.Columns.Count; iLoop++)
							{
								htParam[drvRtnCategory.Row.Table.Columns[iLoop].ColumnName] = drvRtnCategory[iLoop];
							}
							// 楽天カテゴリ名を追加
							htParam[Constants.PRODUCTEXTEND_RAKUTEN_CATEGORY] = strRtnCategory;

							// 行を格納
							this.Products.Add(htParam);
						}
					}
				}
			}
			else
			{
				// 商品情報取得・設定
				dvProducts = GetProduct(sqlAccessor, strShopId, strTargetName, strExtractionPatternId, strMallId);
				SetProduct(dvProducts);
			}
			
			// Yahoo用商品バリエーション情報必要時以下を実行
			if (blIsNeedYhoVariationInfo)
			{
				// 商品アップロード判別
				string strStatement = (blIsTmpTableUse) ? "YahooVariationTmpTableUse" : "YahooVariation";

				// 商品バリエーション情報取得
				DataView dvProductView = GetProduct(sqlAccessor, strShopId, strStatement, strExtractionPatternId, strMallId);
				foreach (DataRowView drvProduct in dvProducts)
				{
					if (this.HtYhoProductVariationInfo.Contains((string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]))
					{
						continue;
					}

					// 商品ID毎に商品バリエーション情報を取得
					dvProductView.RowFilter = Constants.FIELD_PRODUCT_PRODUCT_ID + "='" + (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID] + "'";

					// 商品ID毎に商品バリエーション情報を設定
					YahooVariation yahooVariation = new YahooVariation();
					foreach (DataRowView drvProductView in dvProductView)
					{
						// モールバリエーション種別に"s"が設定されていなければサブコード設定
						if ((Constants.FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_DROPDOWN != (string)drvProductView[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE])
							&& (Constants.FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_CHECKBOX != (string)drvProductView[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE]))
						{
							if ((string)drvProductView[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
							{
								StringBuilder sbSubCode = new StringBuilder();
								if (StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND49]) != "")
								{
									sbSubCode.Append(StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND49]));
									sbSubCode.Append(":");
									sbSubCode.Append((string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]);
								}
								else if ((string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] != "")
								{
									FileLogger.WriteWarn("商品拡張項目のNo.49に値が設定されていない為、バリエーション情報は出力されません。(variation_id:" + (string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] + ")");
								}
								if (StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND50]) != "")
								{
									sbSubCode.Append("#");
									sbSubCode.Append(StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND50]));
									sbSubCode.Append(":");
									sbSubCode.Append((string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]);
									sbSubCode.Append("=");
									sbSubCode.Append((string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
								}
								else if ((string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] != "")
								{
									FileLogger.WriteWarn("商品拡張項目のNo.50に値が設定されていない為、バリエーション情報は出力されません。(variation_id:" + (string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] + ")");
								}
								else if ((string)drvProductView[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1] != "")
								{
									sbSubCode.Append("=");
									sbSubCode.Append((string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
								}

								if (sbSubCode.ToString() != "")
								{
									if (yahooVariation.SubCode == null)
									{
										yahooVariation.SubCode = sbSubCode.ToString();
									}
									else
									{
										// SubCodeを「&」で文字列連結する
										sbSubCode.Insert(0, "&");
										sbSubCode.Insert(0, yahooVariation.SubCode);
										yahooVariation.SubCode = sbSubCode.ToString();
									}
								}
							}
							// オプション１の項目名が空でなければ、オプション１追加
							if (StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND49]) != "")
							{
								yahooVariation.SetOptions(
									StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND49]), 
									(string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]);
							}
							// オプション２の項目名が空でなければ、オプション２追加
							if (StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND50]) != "")
							{
								yahooVariation.SetOptions(
									StringUtility.ToEmpty(drvProductView[Constants.FIELD_PRODUCTEXTEND_EXTEND50]), 
									(string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]);
							}
						}
						else
						{
							yahooVariation.SetOptions(
								(string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
								(string)drvProductView[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]);
						}
					}

					this.HtYhoProductVariationInfo.Add((string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], yahooVariation);
				}
			}
		}

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strShopId">ショップID（一時テーブル名）</param>
		/// <param name="strTargetName">ターゲット名</param>
		/// <param name="strExtractionPatternId">引き抜きパターンID</param>
		/// <param name="strMallId">モールID</param>
		private DataView GetProduct(
			SqlAccessor sqlAccessor, 
			string strShopId, 
			string strTargetName, 
			string strExtractionPatternId, 
			string strMallId)
		{
			//------------------------------------------------------
			// ステートメント決定
			//------------------------------------------------------
			string strStatement = null;
			switch (strTargetName)
			{
				case "w2_ProductView":
					strStatement = "GetProductView";
					break;
				case "w2_ProductExtend":
					strStatement = "GetProductExtend";
					break;
				case "w2_Product":
					strStatement = "GetProduct";
					break;
				case "w2_ProductUpdate":
					strStatement = "ProductUpdate";
					break;
				case "w2_ProductStockUpdate":
					strStatement = "ProductStockUpdate";
					break;
				case "w2_ProductViewStockUpdate":
					strStatement = "ProductViewStockUpdate";
					break;
				case "w2_ProductViewStockUpdateSku":
					strStatement = "ProductViewStockUpdateSku";
					break;
				case "YahooVariation":
					strStatement = "GetProductViewYahooValiation";
					break;
				case "YahooVariationTmpTableUse":
					strStatement = "GetProductViewYahooValiationTmpUse";
					break;
				case "RakutenCategory":
					strStatement = "GetRakutenCategory";
					break;
			}

			//------------------------------------------------------
			// 抽出条件設定
			//------------------------------------------------------
			StringBuilder sbWhere = new StringBuilder();
			switch (strExtractionPatternId)
			{
				case "1":
					sbWhere.Append(" w2_Product.valid_flg = 1 ");
					break;

				case "2":
					sbWhere.Append(" w2_Product.display_from <= CURRENT_TIMESTAMP ");
					sbWhere.Append(" AND ");
					sbWhere.Append(" ((w2_Product.display_to IS NULL) OR (w2_Product.display_to >= CURRENT_TIMESTAMP)) ");
					sbWhere.Append(" AND ");
					sbWhere.Append(" w2_Product.display_kbn = '0' ");
					sbWhere.Append(" AND ");
					sbWhere.Append(" w2_Product.mobile_disp_flg IN ('0','1') ");
					sbWhere.Append(" AND ");
					sbWhere.Append(" w2_Product.valid_flg='1' ");
					break;

				default:
					sbWhere.Append(" 1 = 1 ");
					break;
			}

			//------------------------------------------------------
			// データセット
			//------------------------------------------------------
			DataView dvProductInfos = null;
			using (SqlStatement sqlStatement = new SqlStatement("Product", strStatement))
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ extractionPattern @@", sbWhere.ToString());

				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, strMallId);

				dvProductInfos = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			return dvProductInfos;
		}

		/// <summary>
		/// 商品情報設定
		/// </summary>
		/// <param name="dvProductInfos">商品情報</param>
		private void SetProduct(DataView dvProductInfos)
		{
			// データセット
			foreach (DataRowView drvProductInfos in dvProductInfos)
			{
				Hashtable htParam = new Hashtable();

				// カラムごとに処理
				for (int iLoop = 0; iLoop < drvProductInfos.Row.Table.Columns.Count; iLoop++)
				{
					htParam[drvProductInfos.Row.Table.Columns[iLoop].ColumnName] = drvProductInfos[iLoop];
				}

				// 行を格納
				this.Products.Add(htParam);
			}
		}

		/// <summary>商品情報</summary>
		public List<Hashtable> Products { get; private set; }

		/// <summary>ヤフー商品バリエーション情報</summary>
		public Hashtable HtYhoProductVariationInfo { get; private set; }

		/// <summary>楽天商品バリエーション情報</summary>
		public Hashtable HtRtnCategoryInfo { get; private set; }
	}
}
