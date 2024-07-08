/*
=========================================================================================================
  Module      : マスタ出力設定のユーティリティ(MasterExportSettingUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.App.Common.Global.Config;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.AdvCode;
using w2.Domain.Affiliate;
using w2.Domain.Coordinate;
using w2.Domain.CoordinateCategory;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.OperatorAuthority;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.ProductCategory;
using w2.Domain.ProductReview;
using w2.Domain.ProductSale;
using w2.Domain.ProductStock;
using w2.Domain.ProductTaxCategory;
using w2.Domain.RealShop;
using w2.Domain.RealShopProductStock;
using w2.Domain.SerialKey;
using w2.Domain.ShopOperator;
using w2.Domain.ShortUrl;
using w2.Domain.Staff;
using w2.Domain.StockOrder;
using w2.Domain.TargetList;
using w2.Domain.User;
using w2.Domain.UserProductArrivalMail;

namespace w2.App.Common.MasterExport
{
	public class MasterExportSettingUtility
	{
		#region 新規追加時はDomainのを利用する
		/// <summary>XMLファイルルート名</summary>
		private const string ROOT_XML = "MasterExportSetting";

		/// <summary>
		/// マスタ出力設定削除
		/// </summary>
		/// <param name="masterExportSetting">マスタ出力設定</param>
		/// <remarks>設定IDが抜け番になるため削除後に、既存レコードにID振り直し</remarks>
		public static void DeleteMasterExportSetting(Hashtable masterExportSetting)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();
				try
				{
					using (SqlStatement sqlStatement = new SqlStatement(ROOT_XML, "DeleteMasterExportSetting"))
					{
						sqlStatement.ExecStatement(sqlAccessor, masterExportSetting);
					}

					using (SqlStatement sqlStatement = new SqlStatement(ROOT_XML, "UpdateMasterExportSettingId"))
					{
						sqlStatement.ExecStatement(sqlAccessor, masterExportSetting);
					}

					// トランザクションコミット
					sqlAccessor.CommitTransaction();
				}
				catch (Exception)
				{
					// ロールバック
					sqlAccessor.RollbackTransaction();

					// 例外をスローする
					throw;
				}
			}
		}

		/// <summary>
		/// マスタ出力設定更新
		/// </summary>
		/// <param name="masterExportSetting">マスタ出力設定</param>
		public static void UpdateMasterExportSetting(Hashtable masterExportSetting)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(ROOT_XML, "UpdateMasterExportSetting"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, masterExportSetting);
			}
		}

		/// <summary>
		/// マスタ出力設定挿入
		/// </summary>
		/// <param name="masterExportSetting">マスタ出力設定</param>
		public static void InsertMasterExportSetting(Hashtable masterExportSetting)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(ROOT_XML, "InsertMasterExportSetting"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, masterExportSetting);
			}
		}

		/// <summary>
		/// フィールド群が正しい値かチェック
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// <param name="fieldsCsv">対象フィールド群</param>
		/// <param name="param">パラメータ</param>
		/// <returns>フィールド群の成否</returns>
		public static bool CheckSerialKeyFields(string statement, string fieldsCsv, Hashtable param)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(ROOT_XML, statement))
			{
				try
				{
					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ fields @@", fieldsCsv);
					sqlStatement.SelectSingleStatementWithOC(sqlAccessor, param);
				}
				catch (Exception ex)
				{
					AppLogger.WriteWarn(ex);
					return false;
				}
			}

			return true;
		}
		#endregion

		/// <summary>
		/// マスタ出力定義設定XMLから指定マスタのHashtableリスト取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="masterKbnDataBind">マスタ区分：データ結合マスタ</param>
		/// <returns>マスタ出力セッティングファイル</returns>
		public static List<Hashtable> GetMasterExportSettingFieldList(string masterKbn, string masterKbnDataBind = null)
		{
			// データ結合マスタの場合フィールド名にプレフィックスをつける
			var baseName = masterKbnDataBind == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING
				? Constants.FIELD_USER_ATTRIBUTE_CONVERTING_NAME
				: string.Empty;
			var baseJName = masterKbnDataBind == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING
				? "ユーザー拡張項目："
				: string.Empty;

			var result =
				GetFieldElements(masterKbn)
					.Where(
						element =>
							(element.Attribute(Constants.MASTEREXPORTSETTING_XML_OPTION) == null)
								|| OptionEnabled(element.Attribute(Constants.MASTEREXPORTSETTING_XML_OPTION).Value))
					.Select(
						element => new Hashtable
						{
							{ Constants.MASTEREXPORTSETTING_XML_NAME, baseName + element.Attribute(Constants.MASTEREXPORTSETTING_XML_NAME).Value },
							{ Constants.MASTEREXPORTSETTING_XML_J_NAME, baseJName + element.Attribute(Constants.MASTEREXPORTSETTING_XML_J_NAME).Value },
							{ Constants.MASTEREXPORTSETTING_XML_FIELD, element.Attribute(Constants.MASTEREXPORTSETTING_XML_FIELD).Value
							+ ((masterKbnDataBind == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING)
								? " as " + baseName + element.Attribute(Constants.MASTEREXPORTSETTING_XML_NAME).Value
								: string.Empty) }
						})
					.ToList();
			return result;
		}

		/// <summary>
		/// オプションが有効か
		/// </summary>
		/// <param name="optionName">オプション名</param>
		/// <returns>有効か</returns>
		private static bool OptionEnabled(string optionName)
		{
			switch (optionName)
			{
				case "cpm":
					return Constants.CPM_OPTION_ENABLED;

				case "global":
					return Constants.GLOBAL_OPTION_ENABLE;

				case "scheduledshippingdate":
					return GlobalConfigUtil.UseLeadTime();

				case "receipt":
					return Constants.RECEIPT_OPTION_ENABLED;

				case "mobile":
					return Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED;

				case "product_stock":
					return Constants.PRODUCT_STOCK_OPTION_ENABLE;

				case "product_stock_message_setting":
					return Constants.PRODUCT_STOCK_OPTION_ENABLE;

				case "realstock":
					return Constants.REALSTOCK_OPTION_ENABLED;

				case "demand":
					return Constants.DEMAND_OPTION_ENABLE;

				case "user_attribute":
					return Constants.USER_ATTRIBUTE_OPTION_ENABLE;
				
				case "item_discounted_price":
					return Constants.ORDER_ITEM_DISCOUNTED_PRICE_ENABLE;

				case "point":
					return Constants.W2MP_POINT_OPTION_ENABLED;

				case "coupon":
					return Constants.W2MP_COUPON_OPTION_ENABLED;

				case "member_rank":
					return Constants.MEMBER_RANK_OPTION_ENABLED;

				case "subscription_box":
					return Constants.SUBSCRIPTION_BOX_OPTION_ENABLED;

				case "store_pickup":
					return Constants.REALSHOP_OPTION_ENABLED && Constants.STORE_PICKUP_OPTION_ENABLED;

				case "fixed_purchase":
					return Constants.FIXEDPURCHASE_OPTION_ENABLED;

				case "setpromotion":
					return Constants.SETPROMOTION_OPTION_ENABLED;

				case "mall_cooperation":
					return Constants.MALLCOOPERATION_OPTION_ENABLED;

				case "product_tag":
					return Constants.PRODUCT_TAG_OPTION_ENABLE;

				case "affiliate":
					return Constants.W2MP_AFFILIATE_OPTION_ENABLED;

				case "gift":
					return Constants.GIFTORDER_OPTION_ENABLED;

				case "digital_contents":
					return Constants.DIGITAL_CONTENTS_OPTION_ENABLED;

				case "display_corporation":
					return Constants.DISPLAY_CORPORATION_ENABLED;

				case "product_set":
					return Constants.PRODUCT_SET_OPTION_ENABLED;

				case "product_sale":
					return Constants.PRODUCT_SALE_OPTION_ENABLED;

				case "novelty":
					return Constants.NOVELTY_OPTION_ENABLED;

				case "recommend":
					return Constants.RECOMMEND_OPTION_ENABLED;

				case "brand":
					return Constants.PRODUCT_BRAND_ENABLED;

				case "user_integration":
					return Constants.USERINTEGRATION_OPTION_ENABLED;

				case "gmo_post":
					return Constants.PAYMENT_GMO_POST_ENABLED;
				case "free_shipping_fee":
					return Constants.FREE_SHIPPING_FEE_OPTION_ENABLED;

				case "shipment_forecast_report":
					return Constants.SHIPTMENT_FORECAST_BY_DAYS_ENABLED;
			}
			throw new Exception("不明なオプション：" + optionName);
		}

		/// <summary>
		/// マスタ出力定義設定XMLから指定マスタのHashtable取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="masterKbnDataBind">マスタ区分：データ結合マスタ</param>
		/// <returns>フィールドのみ取得</returns>
		public static Hashtable GetMasterExportSettingFields(string masterKbn, string masterKbnDataBind = null)
		{
			var baseName = masterKbnDataBind == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING
				? Constants.FIELD_USER_ATTRIBUTE_CONVERTING_NAME
				: string.Empty;

			var result = new Hashtable();
			foreach (var item in GetFieldElements(
				(masterKbnDataBind == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING
				&& Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING == masterKbn) 
					? masterKbnDataBind
					: masterKbn))
			{
				result[baseName + item.Attribute(Constants.MASTEREXPORTSETTING_XML_NAME).Value] =
					item.Attribute(Constants.MASTEREXPORTSETTING_XML_FIELD).Value;
			}
			return result;
		}

		/// <summary>
		/// マスタ出力定義設定XMLから指定マスタのHashtable取得(Type)
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>タイプのみ取得</returns>
		public static Hashtable GetMasterExportSettingTypes(string masterKbn)
		{
			var temp = GetFieldElements(GetMasterKbnException(masterKbn))
				.ToDictionary(
					key => key.Attribute(Constants.MASTEREXPORTSETTING_XML_NAME).Value,
					value => (value.Attribute(Constants.MASTEREXPORTSETTING_XML_TYPE) != null)
						? value.Attribute(Constants.MASTEREXPORTSETTING_XML_TYPE).Value
						: "");

			return new Hashtable(temp);
		}

		/// <summary>
		/// マスタ出力定義設定XMLから指定マスタのノード取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>マスタ出力セッティングファイル(Fieldノード要素)</returns>
		private static IEnumerable<XElement> GetFieldElements(string masterKbn)
		{
			var result = XElement
						.Load(GetPath(masterKbn) + Constants.FILE_XML_MASTEREXPORTSETTING_SETTING)
						.Elements(masterKbn)
						.Elements("Field")
						.Distinct();
			return result;
		}

		/// <summary>
		/// マスタ区分でパス変更
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>パス</returns>
		private static string GetPath(string masterKbn)
		{
			var path = AppDomain.CurrentDomain.BaseDirectory;
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE:
					path = Constants.PHYSICALDIRPATH_COMMERCE_MANAGER;
					break;
			}
			return path;
		}

		/// <summary>
		/// フィールド列文字列のエスケープ
		/// </summary>
		/// <param name="fields">フィールド列</param>
		/// <returns>エスケープ後フィールド列文字列</returns>
		public static string GetFieldsEscape(string fields)
		{
			return fields.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("　", "");
		}

		/// <summary>
		/// マスタ区分取得（例外用）
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>マスタ区分</returns>
		/// <remarks>ワークフローの場合マスタ定義は注文と同一情報を利用するためここで変換</remarks>
		public static string GetMasterKbnException(string masterKbn)
		{
			switch (masterKbn)
			{
				// 注文マスタ表示（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER;
					break;

				// 注文商品マスタ表示（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM;
					break;

				// 注文セットプロモーションマスタ（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION;
					break;

				// 定期購入（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE;
					break;

				// 定期購入商品（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM;
					break;

				// データ結合マスタ（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING;
					break;
			}
			return masterKbn;
		}

		/// <summary>
		/// 注文系マスタ向けフィールドセット
		/// </summary>
		/// <param name="fields">フィールド格納ハッシュ（こちらに格納されます）</param>
		/// <param name="masterKbn">マスタ区分</param>
		public static void SetFieldInfoForOrder(Hashtable fields, string masterKbn = null)
		{
			// データ結合マスタの場合フィールド名にプレフィックスをつける
			var baseName = masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING
				? Constants.FIELD_ORDER_CONVERTING_NAME
				: string.Empty;
			// 注文拡張ステータス項目設定
			for (var i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				fields.Add(
					baseName + Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i.ToString(),
					"w2_Order." + Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i.ToString() + " as " + baseName + Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i.ToString());
				fields.Add(
					baseName + Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + i.ToString(),
					"w2_Order." + Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + i.ToString() + " as " + baseName + Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + i.ToString());
			}

			if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING)
			{
				var taxFieldNamesDataBinding = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
				var regexDataBinding = new Regex("^(.*_by_rate)_(\\d+)$");
				foreach (var taxFieldName in taxFieldNamesDataBinding)
				{
					var match = regexDataBinding.Match(taxFieldName);
					if (match.Success == false) continue;

					var rateString = match.Groups[2].Value.PadLeft(4, '0');
					var rate = decimal.Parse(rateString.Substring(0, 2) + "." + rateString.Substring(2, 2));

					fields.Remove(taxFieldName);
					fields.Add(baseName + taxFieldName, "");

					fields[baseName + taxFieldName] =
						string.Format(
							"ISNULL((SELECT {0} FROM w2_OrderPriceByTaxRate WHERE order_id = w2_Order.order_id AND key_tax_rate = {1}), 0) AS [{2}]",
							match.Groups[1].Value,
							rate,
							"order_" + taxFieldName);
				}
			}
			else
			{
				// 税率毎価格情報項目設定
				var taxFieldNames = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
				var regex = new Regex("^(.*_by_rate)_(\\d+)$");
				foreach (var taxFieldName in taxFieldNames)
				{
					var match = regex.Match(taxFieldName);
					if (match.Success == false) continue;

					var rateString = match.Groups[2].Value.PadLeft(4, '0');
					var rate = decimal.Parse(rateString.Substring(0, 2) + "." + rateString.Substring(2, 2));
					fields[taxFieldName] =
						string.Format(
							"ISNULL((SELECT {0} FROM w2_OrderPriceByTaxRate WHERE order_id = w2_Order.order_id AND key_tax_rate = {1}), 0) AS [{2}]",
							match.Groups[1].Value,
							rate,
							taxFieldName);
				}
			}
		}

		/// <summary>
		/// フィールドチェック
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="sqlFieldNames">SQLフィールド名</param>
		/// <returns>チェックOKか</returns>
		public static bool CheckFieldsExists(string shopId, string masterKbn, string sqlFieldNames)
		{
			var input = new Hashtable();
			switch (masterKbn)
			{
				// EC ユーザー
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING: // ユーザ配送先マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE: // ユーザー電子発票管理マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER: // ユーザーマスタ表示
					return new UserService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn);
				// EC 商品
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT: // 商品マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION: // 商品価格
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE: // 商品バリエーション表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG: // 商品タグ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND: // 商品拡張項目表示
					return new ProductService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn, shopId);
				// EC 注文
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER: // 注文マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM: // 注文商品マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION: // 注文セットプロモーションマスタ表示
					return new OrderService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn, shopId);
				// EC 定期
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE: // 定期購入マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM: // 定期購入商品マスタ表示
					return new FixedPurchaseService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn, shopId);
				// 商品在庫表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK:
					return new ProductStockService().CheckFieldsForGetMaster(sqlFieldNames, shopId);
				// 商品カテゴリ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY:
					return new ProductCategoryService().CheckFieldsForGetMaster(sqlFieldNames, shopId);
				// 商品セール価格
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE:
					return new ProductSaleService().CheckFieldsForGetMaster(sqlFieldNames, shopId);
				// ショートURL
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL:
					return new ShortUrlService().CheckFieldsForGetMaster(sqlFieldNames, shopId);
				// シリアルキー
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY:
					return new SerialKeyService().CheckFieldsForGetMaster(sqlFieldNames);
				// リアル店舗
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP:
					return new RealShopService().CheckFieldsForGetMaster(sqlFieldNames);
				// 発注マスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM:
					return new StockOrderService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn, shopId);
				// 入荷通知メールマスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL:
					return new UserProductArrivalMailService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn, shopId);
				// 商品レビュー
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW:
					return new ProductReviewService().CheckFieldsForGetMaster(sqlFieldNames, shopId);
				// リアル店舗商品在庫
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK:
					return new RealShopProductStockService().CheckFieldsForGetMaster(sqlFieldNames);
				// オペレータマスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR:
					return new ShopOperatorService().CheckFieldsForGetMaster(sqlFieldNames);
				// データ結合マスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING:
					return new OrderService().CheckFieldsForGetMasterDataBinding(sqlFieldNames, masterKbn, shopId);

				// MP
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG: // アフィリエイト連携ログ表示
					return new AffiliateService().CheckAffiliateCoopLogFieldsForGetMaster(sqlFieldNames);
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA:
					return new TargetListService().CheckTargetListDataFieldsForGetMaster(sqlFieldNames);
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE: // 広告コード
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE: // 広告媒体区分マスタ
					return new AdvCodeService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn);
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL: // ポイント情報(詳細)
					return new PointService().CheckUserPointDetailFieldsForGetMaster(sqlFieldNames);
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON: // クーポンマスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON: // ユーザクーポンマスタ
					return new CouponService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn, input);
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER: // クーポン利用ユーザー情報
					input.Add(
						Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE,
						Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE);
					return new CouponService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn, input);

				// CMS
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE: // コーディネートマスタ表示
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM: // コーディネートアイテムマスタ表示
					return new CoordinateService().CheckFieldsForGetMaster(sqlFieldNames, masterKbn);
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_CATEGORY: // コーディネートカテゴリマスタ表示
					return new CoordinateCategoryService().CheckFieldsForGetMaster(sqlFieldNames);
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STAFF: // スタッフマスタ表示
					return new StaffService().CheckFieldsForGetMaster(sqlFieldNames);
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}
	}
}
