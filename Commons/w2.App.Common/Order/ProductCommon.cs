/*
=========================================================================================================
  Module      : 商品共通処理クラス(ProductCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.App.Common.ProductDefaultSetting;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.ProductSale;
using w2.Domain.SetPromotion;
using w2.Domain.SubscriptionBox;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// 商品共通処理クラス
	/// </summary>
	///*********************************************************************************************
	public partial class ProductCommon
	{
		/// <summary>検索モード</summary>
		public enum SearchMode
		{
			Perfect,
			Suffix,
			Prefix,
			Internal,
			// TODO:数値検索は未対応
			Numeric
		}

		/// <summary>検索用演算子</summary>
		public enum SearchOperator
		{
			Equal,
			GreaterThan,
			LessThan,
			GreaterThanNotEqual,
			LessThanNotEqual
		}

		/// <summary>検索オブジェクト</summary>
		public class SearchObject
		{
			/// <summary>検索モード</summary>
			public SearchMode SearchMode { get; set; }
			/// <summary>検索演算子</summary>
			public SearchOperator SearchOperator { get; set; }
			/// <summary>検索フィールド</summary>
			public string SearchField { get; set; }
		}

		public const string URL_KEY_CATEGORY_NAME = "category_name";
		public const string URL_KEY_BRAND_NAME = "brand_name";
		public const string TAG_GREATER_THAN = "_gt";
		public const string TAG_LESS_THAN = "_lt";
		public const string TAG_GREATER_THAN_NOT_EQUAL = "_gtne";
		public const string TAG_LESS_THAN_NOT_EQUAL = "_ltne";
		private static Dictionary<string, SearchObject> m_searchModes = new Dictionary<string, SearchObject>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		static ProductCommon()
		{
			// PC・Mobileサイト以外でProductCommonを利用する場合は詳細検索設定XMLファイルがないので飛ばす
			if ((Directory.Exists(Constants.PHYSICALDIRPATH_CONTENTS) == false) ||
				(File.Exists(Constants.PHYSICALDIRPATH_CONTENTS + Constants.FILE_ADVANCEDSEARCHSETTING) == false)) return;

			// 検索モード作成
			CreateAdvancedSearchModeSetting();

			// 監視起動(検索モード作成)
			FileUpdateObserver.GetInstance().AddObservation(
				Constants.PHYSICALDIRPATH_CONTENTS,
				Constants.FILE_ADVANCEDSEARCHSETTING,
				CreateAdvancedSearchModeSetting);
		}

		/// <summary>
		/// 商品データ取得（表示条件考慮しない）
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID(なしの場合、商品ID)</param>
		/// <param name="strMemberRankId">会員ランクID</param>
		/// <param name="productSaleId">商品セールID（闇市用）</param>
		/// <returns>商品データ</returns>
		public static DataView GetProductVariationInfo(
			string strShopId,
			string strProductId,
			string strVariationId,
			string strMemberRankId,
			string productSaleId = "")
		{
			DataView dvProductVariation = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductVariation"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_CART_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_CART_PRODUCT_ID, strProductId);
				htInput.Add(Constants.FIELD_CART_VARIATION_ID, strVariationId);
				htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, strMemberRankId);
				htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, productSaleId);

				dvProductVariation = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			return dvProductVariation;
		}

		/// <summary>
		/// 商品データ取得（スコアリング機能）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品情報</returns>
		public static DataView GetScoringSaleResultProduct(string shopId, string productId, string memberRankId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				using (SqlStatement sqlStatement = new SqlStatement("Product", "GetScoringSaleResultProduct"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, shopId);
					htInput.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, productId);
					htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId);

					return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}
			}
		}

		/// <summary>
		/// 商品データ取得（表示条件考慮しない）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID(なしの場合、商品ID)</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <param name="productSaleId">商品セールID（闇市用）</param>
		/// <returns>商品データ</returns>
		public static DataView GetProductVariationInfo(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			SqlAccessor accessor,
			string productSaleId = "")
		{
			using (var statement = new SqlStatement("Product", "GetProductVariation"))
			{
				var input = new Hashtable()
				{
					{ Constants.FIELD_CART_SHOP_ID, shopId },
					{ Constants.FIELD_CART_PRODUCT_ID, productId },
					{ Constants.FIELD_CART_VARIATION_ID, variationId },
					{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId },
					{ Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, productSaleId }
				};

				var data = statement.SelectSingleStatement(accessor, input);

				return ((data.Count > 0) ? data : null);
			}
		}

		/// <summary>
		/// 商品＋バリエーション名作成
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>バリエーション名</returns>
		/// <remarks>バリエーションありなしを自動判定します。</remarks>
		public static string CreateProductJointName(object product)
		{
			return GetKeyValue(product, Constants.FIELD_PRODUCT_NAME) + CreateVariationName(product);
		}
		/// <summary>
		/// 商品＋バリエーション名作成
		/// </summary>
		/// <param name="strProductName">商品名</param>
		/// <param name="strVariationName1">バリエーション名１</param>
		/// <param name="strVariationName2">バリエーション名２</param>
		/// <param name="variationName3">バリエーション名3</param>
		/// <returns>バリエーション名</returns>
		public static string CreateProductJointName(
			string strProductName,
			string strVariationName1,
			string strVariationName2,
			string variationName3 = "")
		{
			return strProductName + CreateVariationName(strVariationName1, strVariationName2, variationName3);
		}

		/// <summary>
		/// バリエーション名作成
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>バリエーション名</returns>
		/// <remarks>バリエーションありなしを自動判定します。</remarks>
		public static string CreateVariationName(object product)
		{
			var name1 = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1));
			var name2 = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2));
			var name3 = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3));

			if ((string.IsNullOrEmpty(name1) == false) || (string.IsNullOrEmpty(name2) == false) || (string.IsNullOrEmpty(name3) == false))
			{
				return CreateVariationName(name1, name2, name3);
			}
			return "";
		}
		/// <summary>
		/// バリエーション名作成
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="strParenthesisLeft">左括弧</param>
		/// <param name="strParenthesisRight">右括弧</param>
		/// <param name="strPunctuation">区切り文字</param>
		/// <returns>バリエーション名</returns>
		/// <remarks>バリエーションありなしを自動判定します。</remarks>
		public static string CreateVariationName(DataRowView drvProduct, string strParenthesisLeft, string strParenthesisRight, string strPunctuation)
		{
			if (HasVariation(drvProduct))
			{
				return CreateVariationName(
					(string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
					(string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
					(string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
					strParenthesisLeft,
					strParenthesisRight,
					strPunctuation);
			}

			return "";
		}
		/// <summary>
		/// バリエーション名作成
		/// </summary>
		/// <param name="strVariationName1">バリエーション名１</param>
		/// <param name="strVariationName2">バリエーション名２</param>
		/// <param name="variationName3">バリエーション名3</param>
		/// <returns>バリエーション名</returns>
		public static string CreateVariationName(string strVariationName1, string strVariationName2, string variationName3)
		{
			return CreateVariationName(
				strVariationName1,
				strVariationName2,
				variationName3,
				Constants.CONST_PRODUCTVARIATIONNAME_PARENTHESIS_LEFT,
				Constants.CONST_PRODUCTVARIATIONNAME_PARENTHESIS_RIGHT,
				Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);
		}
		/// <summary>
		/// バリエーション名作成
		/// </summary>
		/// <param name="strVariationName1">バリエーション名１</param>
		/// <param name="strVariationName2">バリエーション名２</param>
		/// <param name="variationName3">バリエーション名3</param>
		/// <param name="strParenthesisLeft">左括弧</param>
		/// <param name="strParenthesisRight">右括弧</param>
		/// <param name="strPunctuation">区切り文字</param>
		/// <returns>バリエーション名</returns>
		public static string CreateVariationName(string strVariationName1, string strVariationName2, string variationName3, string strParenthesisLeft, string strParenthesisRight, string strPunctuation)
		{
			StringBuilder sbResult = new StringBuilder();
			if ((strVariationName1 != string.Empty) || (strVariationName2 != string.Empty) || (variationName3 != string.Empty))
			{
				sbResult.Append(strParenthesisLeft);
				sbResult.Append(strVariationName1);
				if ((strVariationName1 != string.Empty) && (strVariationName2 != string.Empty))
				{
					sbResult.Append(strPunctuation);
				}
				sbResult.Append(strVariationName2);
				if (((strVariationName1 != string.Empty) && (variationName3 != string.Empty))
					|| ((strVariationName2 != string.Empty) && (variationName3 != string.Empty)))
				{
					sbResult.Append(strPunctuation);
				}
				sbResult.Append(variationName3);
				sbResult.Append(strParenthesisRight);
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// バリエーションがあるか
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>バリエーションがあるか</returns>
		public static bool HasVariation(object product)
		{
			return (((string)GetKeyValue(product, Constants.FIELD_PRODUCT_USE_VARIATION_FLG) == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE));
		}

		/// <summary>
		/// 商品価格数値取得
		/// </summary>
		/// <param name="objProduct">商品情報</param>
		/// <param name="blTargetVariation">バリエーション対象</param>
		/// <returns>商品価格</returns>
		public static string GetProductPriceNumeric(object objProduct, bool blTargetVariation = false)
		{
			var price = (blTargetVariation == false)
				? StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCT_DISPLAY_PRICE))
				: StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_PRICE));

			price = StringUtility.ToNumeric(price.ToPriceString());

			return price;
		}

		/// <summary>
		/// 商品セール期間取得(改行)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <returns>セール期間</returns>
		/// <remarks>
		/// 商品セール期間(改行)取得個別デザインのため
		/// </remarks>
		public static string GetProductSaleTermBr(string shopId, string productSaleId = "", string linefeed = "～\r\n")
		{
			if (string.IsNullOrEmpty(productSaleId)) return string.Empty;

			var productSale = new ProductSaleService().Get(shopId, productSaleId);
			var beginDate = DateTimeUtility.ToStringFromRegion(productSale.DateBgn, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var endDate = DateTimeUtility.ToStringFromRegion(productSale.DateEnd, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var productSaleTerm = beginDate + linefeed + endDate;
			return productSaleTerm;
		}

		/// <summary>
		/// Lpページセール期間（開始日）取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <returns>セール期間（開始日）</returns>
		/// <remarks>
		/// Lpページ商品セール期間（開始日）取得個別デザインのため
		/// </remarks>
		public static string GetProductSaleTermBrLpBeginDate(string shopId, string productSaleId = "")
		{
			if (string.IsNullOrEmpty(productSaleId)) return string.Empty;

			var productSale = new ProductSaleService().Get(shopId, productSaleId);
			var beginDate = DateTimeUtility.ToStringFromRegion(productSale.DateBgn, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			return beginDate;
		}

		/// <summary>
		/// Lpページ商品セール期間（終了日）取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <returns>セール期間（終了日）</returns>
		/// <remarks>
		/// Lpページ商品セール期間（終了日）取得個別デザインのため
		/// </remarks>
		public static string GetProductSaleTermBrLpEndDate(string shopId, string productSaleId = "")
		{
			if (string.IsNullOrEmpty(productSaleId)) return string.Empty;

			var productSale = new ProductSaleService().Get(shopId, productSaleId);
			var endDate = DateTimeUtility.ToStringFromRegion(productSale.DateEnd, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			return endDate;
		}

		/// <summary>
		/// セットプロモーション期間(改行)取得
		/// </summary>
		/// <param name="setPromotionId">セットプロモーションID</param>
		/// <returns>セットプロモーション期間</returns>
		/// <remarks>
		/// セットプロモーション期間(改行)取得個別デザインのため
		/// </remarks>
		public static string GetSetPromotionTermBr(string setPromotionId = "", string linefeed = "～\r\n")
		{
			if (string.IsNullOrEmpty(setPromotionId)) return string.Empty;
			var setPromotion = new SetPromotionService().Get(setPromotionId);
			var beginDate = DateTimeUtility.ToStringFromRegion(setPromotion.BeginDate, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var endDate = DateTimeUtility.ToStringFromRegion(setPromotion.EndDate, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var setPromotionTerm = beginDate + linefeed + endDate;
			return setPromotionTerm;
		}

		/// <summary>
		/// データ取り出しメソッド
		/// </summary>
		/// <param name="objSrc"></param>
		/// <param name="strKey"></param>
		/// <returns></returns>
		public static object GetKeyValue(object objSrc, string strKey)
		{
			if (objSrc is DataRowView)
			{
				return ((DataRowView)objSrc).Row.Table.Columns.Contains(strKey) ? ((DataRowView)objSrc)[strKey] : null;
			}
			else if (objSrc is IDictionary)
			{
				return ((IDictionary)objSrc)[strKey];
			}
			else if (objSrc is w2.App.Common.Order.CartProduct)
			{
				switch (strKey)
				{
					case Constants.FIELD_PRODUCT_SHOP_ID:
						return ((w2.App.Common.Order.CartProduct)objSrc).ShopId;

					case Constants.FIELD_PRODUCT_PRODUCT_ID:
						return ((w2.App.Common.Order.CartProduct)objSrc).ProductId;

					case Constants.FIELD_PRODUCTVARIATION_VARIATION_ID:
						return ((w2.App.Common.Order.CartProduct)objSrc).VariationId;

					case Constants.FIELD_PRODUCT_IMAGE_HEAD:
					case Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD:
						return ((w2.App.Common.Order.CartProduct)objSrc).ProductVariationImageHead;

					case Constants.FIELD_PRODUCT_NAME:
						return ((w2.App.Common.Order.CartProduct)objSrc).ProductName;

					case Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1:
						return ((w2.App.Common.Order.CartProduct)objSrc).VariationName1;

					case Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2:
						return ((w2.App.Common.Order.CartProduct)objSrc).VariationName2;

					case Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3:
						return ((w2.App.Common.Order.CartProduct)objSrc).VariationName3;

					case Constants.FIELD_ORDERITEM_PRODUCT_NAME:
						return ((w2.App.Common.Order.CartProduct)objSrc).ProductJointName;

					case Constants.FIELD_ORDERITEM_PRODUCT_PRICE:
						return ((w2.App.Common.Order.CartProduct)objSrc).Price;

					case Constants.FIELD_ORDERITEM_ITEM_QUANTITY:
						return ((w2.App.Common.Order.CartProduct)objSrc).Count;

					case Constants.FIELD_PRODUCT_BRAND_ID1:
						return ((w2.App.Common.Order.CartProduct)objSrc).BrandId;

					case Constants.FIELD_PRODUCT_BRAND_ID2:
						return ((w2.App.Common.Order.CartProduct)objSrc).BrandId2;

					case Constants.FIELD_PRODUCT_BRAND_ID3:
						return ((w2.App.Common.Order.CartProduct)objSrc).BrandId3;

					case Constants.FIELD_PRODUCT_BRAND_ID4:
						return ((w2.App.Common.Order.CartProduct)objSrc).BrandId4;

					case Constants.FIELD_PRODUCT_BRAND_ID5:
						return ((w2.App.Common.Order.CartProduct)objSrc).BrandId5;

					case Constants.FIELD_PRODUCT_CATEGORY_ID1:
						return ((w2.App.Common.Order.CartProduct)objSrc).CategoryId1;

					case Constants.FIELD_PRODUCT_CATEGORY_ID2:
						return ((w2.App.Common.Order.CartProduct)objSrc).CategoryId2;

					case Constants.FIELD_PRODUCT_CATEGORY_ID3:
						return ((w2.App.Common.Order.CartProduct)objSrc).CategoryId3;

					case Constants.FIELD_PRODUCT_CATEGORY_ID4:
						return ((w2.App.Common.Order.CartProduct)objSrc).CategoryId4;

					case Constants.FIELD_PRODUCT_CATEGORY_ID5:
						return ((w2.App.Common.Order.CartProduct)objSrc).CategoryId5;

					case Constants.FIELD_PRODUCT_ICON_FLG1:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[0];

					case Constants.FIELD_PRODUCT_ICON_TERM_END1:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[0];

					case Constants.FIELD_PRODUCT_ICON_FLG2:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[1];

					case Constants.FIELD_PRODUCT_ICON_TERM_END2:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[1];

					case Constants.FIELD_PRODUCT_ICON_FLG3:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[2];

					case Constants.FIELD_PRODUCT_ICON_TERM_END3:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[2];

					case Constants.FIELD_PRODUCT_ICON_FLG4:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[3];

					case Constants.FIELD_PRODUCT_ICON_TERM_END4:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[3];

					case Constants.FIELD_PRODUCT_ICON_FLG5:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[4];

					case Constants.FIELD_PRODUCT_ICON_TERM_END5:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[4];

					case Constants.FIELD_PRODUCT_ICON_FLG6:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[5];

					case Constants.FIELD_PRODUCT_ICON_TERM_END6:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[5];

					case Constants.FIELD_PRODUCT_ICON_FLG7:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[6];

					case Constants.FIELD_PRODUCT_ICON_TERM_END7:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[6];

					case Constants.FIELD_PRODUCT_ICON_FLG8:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[7];

					case Constants.FIELD_PRODUCT_ICON_TERM_END8:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[7];

					case Constants.FIELD_PRODUCT_ICON_FLG9:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[8];

					case Constants.FIELD_PRODUCT_ICON_TERM_END9:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[8];

					case Constants.FIELD_PRODUCT_ICON_FLG10:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconFlg[9];

					case Constants.FIELD_PRODUCT_ICON_TERM_END10:
						return ((w2.App.Common.Order.CartProduct)objSrc).IconTermEnd[9];
				}
			}
			else if (objSrc is UserCreditCard)
			{
				return ((UserCreditCard)objSrc).DataSource[strKey];
			}
			else if (objSrc is IModel)
			{
				return ((IModel)objSrc).DataSource[strKey];
			}
			return null;
		}

		/// <summary>
		/// データ取り出しメソッド（DBNullをnull扱いにする）
		/// </summary>
		/// <param name="src">ソース</param>
		/// <param name="key">キー</param>
		/// <returns>データ</returns>
		public static object GetKeyValueToNull(object src, string key)
		{
			return GetKeyValue(src, key) != DBNull.Value ? GetKeyValue(src, key) : null;
		}

		#region キーがあるか判定（非推奨）
		/// <summary>
		/// キーがあるか判定
		/// </summary>
		/// <param name="objSrc"></param>
		/// <param name="strKey"></param>
		/// <returns></returns>
		[Obsolete("[V5.3] 使用しないのであれば削除します")]
		public static bool HasKey(object objSrc, string strKey)
		{
			if (objSrc is DataRowView)
			{
				return ((DataRowView)objSrc).Row.Table.Columns.Contains(strKey);
			}
			else if (objSrc is IDictionary)
			{
				return ((IDictionary)objSrc).Contains(strKey);
			}

			throw new ArgumentException("パラメタエラー: objSrc is [" + objSrc.GetType().ToString() + "]");
		}
		#endregion

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>商品情報</returns>
		public static DataView GetProductInfoUnuseMemberRankPrice(string shopId, string productId)
		{
			return GetProductInfo(shopId, productId, "", string.Empty);
		}
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="memberRankId">会員ランク順位</param>
		/// <param name="fixedPurchaseMemberFlag">ログインユーザー定期会員フラグ</param>
		/// <returns>商品情報</returns>
		public static DataView GetProductInfo(string shopId, string productId, string memberRankId, string fixedPurchaseMemberFlag = null)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProduct"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, shopId);
					htInput.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, productId);
					htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId);
					htInput.Add(Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, fixedPurchaseMemberFlag);

					return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}
			}
		}

		/// <summary>
		/// 商品情報取得（ダミー商品）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品情報</returns>
		public static DataView GetDummyProductInfo(string shopId)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Product", "GetDummyProductInfo"))
			{
				sqlAccessor.OpenConnection();
				var input = new Hashtable
				{
					{Constants.FIELD_PRODUCT_SHOP_ID, shopId}
				};

				return sqlStatement.SelectSingleStatement(sqlAccessor, input);
			}
		}

		/// <summary>
		/// 商品情報取得（複数商品ID指定）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productIds">商品IDリスト配列</param>
		/// <returns>商品情報取得</returns>
		public static DataView GetProductsInfo(string shopId, string[] productIds)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Product", "GetProducts"))
			{
				var input = new Hashtable
				{
					{Constants.FIELD_PRODUCT_SHOP_ID, shopId}
				};
				// 商品IDをパラメータに追加
				var productIdWhere = new StringBuilder();
				var no = 1;
				foreach (var productId in productIds)
				{
					var paramKey = Constants.FIELD_PRODUCT_PRODUCT_ID + no.ToString();
					input.Add(paramKey, productId);
					statement.AddInputParameters(paramKey, SqlDbType.NVarChar, 30);
					if (productIdWhere.Length != 0) productIdWhere.Append(",");
					productIdWhere.Append("@" + paramKey);
					no++;
				}

				var where = (productIds.Length != 0)
					? "AND w2_Product.product_id IN (" + productIdWhere.ToString() + ")"
					: "AND 1 = 0";
				statement.Statement = statement.Statement.Replace("@@ where @@", where.ToString());

				return statement.SelectSingleStatementWithOC(accessor, input);
			}
		}

		/// <summary>
		/// 商品情報取得(多言語考慮)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="memberRankId">会員ランク順位</param>
		/// <param name="fixedPurchaseMemberFlag">ログインユーザー定期会員フラグ</param>
		/// <returns>商品情報</returns>
		public static DataView GetProductInfoWithTranslatedName(string shopId, string productId, string memberRankId, string fixedPurchaseMemberFlag = null)
		{
			var products= GetProductInfo(shopId, productId, memberRankId, fixedPurchaseMemberFlag);
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報設定
				var productTranslationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettingsByProductId(
					productId,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId);
				products = NameTranslationCommon.SetProductAndVariationTranslationDataToDataView(
					products,
					productTranslationSettings);
			}
			return products;
		}

		/// <summary>
		/// 商品一覧遷移URL作成
		/// </summary>
		/// <param name="objShopId">店舗ID</param>
		/// <param name="objCategoryId">カテゴリID</param>
		/// <param name="objSearchWord">検索文字列</param>
		/// <param name="objProductGroupId">商品グループID</param>
		/// <param name="objCampaignIcon">キャンペーンアイコン</param>
		/// <param name="objMinPrice">最小価格</param>
		/// <param name="objMaxPrice">最大価格</param>
		/// <param name="objSortKbn">ソート区分</param>
		/// <param name="objBrandId">ブランドID</param>
		/// <param name="objDispImageKbn">画像表示区分</param>
		/// <param name="objDispOnlySpPrice"></param>
		/// <param name="strCategoryName">カテゴリ名（URL表示用）</param>
		/// <param name="strBrandName">ブランド名</param>
		/// <param name="strUndisplayNostock">在庫無し有無</param>
		/// <param name="fixedPurchaseFilter">定期購入フィルタ</param>
		/// <param name="iPageNumber">ページ番号(-1のとき指定なし)</param>
		/// <param name="iDisplayCount">表示件数(-1のとき指定なし)</param>
		/// <param name="productColorId">商品カラーID</param>
		/// <param name="saleFilter">セール対象フィルタ</param>
		/// <param name="subscriptionBoxSearchWord">頒布会検索文字列</param>
		/// <returns>作成URL</returns>
		public static string CreateProductListUrl(
			object objShopId,
			object objCategoryId,
			object objSearchWord,
			object objProductGroupId,
			object objCampaignIcon,
			object objMinPrice,
			object objMaxPrice,
			object objSortKbn,
			object objBrandId,
			object objDispImageKbn,
			object objDispOnlySpPrice,
			string strCategoryName,
			string strBrandName,
			string strUndisplayNostock,
			string fixedPurchaseFilter,
			int iPageNumber,
			int iDisplayCount = -1,
			string productColorId = "",
			string saleFilter = "",
			object subscriptionBoxSearchWord = null)
		{
			var requestParameter = new Dictionary<string, string>
			{
				{ Constants.REQUEST_KEY_SHOP_ID,
					(string.IsNullOrEmpty(StringUtility.ToEmpty(objShopId)) == false) ? objShopId.ToString() : Constants.CONST_DEFAULT_SHOP_ID },
				{ Constants.REQUEST_KEY_CATEGORY_ID, StringUtility.ToEmpty(objCategoryId) },
				{ Constants.REQUEST_KEY_BRAND_ID, StringUtility.ToEmpty(objBrandId) },
				{ Constants.REQUEST_KEY_SEARCH_WORD, StringUtility.ToEmpty(objSearchWord) },
				{ Constants.REQUEST_KEY_DISP_IMG_KBN,
					(string.IsNullOrEmpty(StringUtility.ToEmpty(objDispImageKbn)) == false) ? objDispImageKbn.ToString() : ProductListDispSettingUtility.DispImgKbnDefault },
				{ Constants.REQUEST_KEY_SORT_KBN,
					(string.IsNullOrEmpty(StringUtility.ToEmpty(objSortKbn)) == false) ? objSortKbn.ToString() : ProductListDispSettingUtility.SortDefault },
				{ Constants.REQUEST_KEY_CAMPAINGN_ICOM, StringUtility.ToEmpty(objCampaignIcon) },
				{ Constants.REQUEST_KEY_MIN_PRICE, StringUtility.ToEmpty(objMinPrice) },
				{ Constants.REQUEST_KEY_MAX_PRICE, StringUtility.ToEmpty(objMaxPrice) },
				{ Constants.REQUEST_KEY_DISP_PRODUCT_COUNT, (iDisplayCount > 0) ? iDisplayCount.ToString() : string.Empty },
				{ Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT,
					(string.IsNullOrEmpty(strUndisplayNostock) == false) ? strUndisplayNostock : ProductListDispSettingUtility.UndisplayNostockProductDefault },
				{ Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER,
					(string.IsNullOrEmpty(fixedPurchaseFilter) == false) ? fixedPurchaseFilter : Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL },
				{ Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE, StringUtility.ToEmpty(objDispOnlySpPrice) },
				{ URL_KEY_CATEGORY_NAME, strCategoryName },
				{ URL_KEY_BRAND_NAME, strBrandName },
				{ Constants.REQUEST_KEY_PRODUCT_GROUP_ID,StringUtility.ToEmpty(objProductGroupId) },
				{ Constants.REQUEST_KEY_PAGE_NO, ((iPageNumber > 0) ? iPageNumber : 1).ToString() },
				{ Constants.REQUEST_KEY_PRODUCT_COLOR_ID, StringUtility.ToEmpty(productColorId) },
				{ Constants.REQUEST_KEY_PRODUCT_SALE_FILTER, StringUtility.ToEmpty(saleFilter) },
				{ Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD, StringUtility.ToEmpty(subscriptionBoxSearchWord) },
			};
			var productListUrl = CreateProductListUrl(requestParameter);
			return productListUrl;
		}
		/// <summary>
		/// 商品一覧遷移URL作成
		/// </summary>
		/// <param name="requestParam">リクエストパラメーター</param>
		/// <param name="targetUrl">遷移先Url</param>
		/// <returns>作成URL</returns>
		public static string CreateProductListUrl(Dictionary<string, string> requestParam, string targetUrl = "")
		{
			var urlCreator = CreateUrlCreatorForProductListUrl(requestParam, targetUrl);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_PRODUCT_GROUP_ID);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_CAMPAINGN_ICOM);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_DISP_PRODUCT_COUNT);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_DISP_IMG_KBN);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_MAX_PRICE);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_MIN_PRICE);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_SORT_KBN);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_SEARCH_WORD);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_PRODUCT_COLOR_ID);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_PRODUCT_SALE_FILTER);
			AddProductListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD);

			// 詳細検索項目追加(アルファベット順)
			foreach (var param in requestParam.Where(i => i.Key.IndexOf("_") == 0).OrderBy(i => i.Key))
			{
				if (IsProductUrlParameterOmit(param.Value) == false)
				{
					urlCreator.AddParam(param.Key, param.Value);
				}
			}

			// ページ番号は一番最後
			if (requestParam.ContainsKey(Constants.REQUEST_KEY_PAGE_NO))
			{
				var pageNo = (int.Parse(requestParam[Constants.REQUEST_KEY_PAGE_NO]) > 0) ? requestParam[Constants.REQUEST_KEY_PAGE_NO] : "1";
				urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNo);
			}

			return urlCreator.CreateUrl();
		}

		/// <summary>
		/// 商品リストURLパラメーター追加
		/// </summary>
		/// <param name="urlCreator">URLクエリ</param>
		/// <param name="requestParam">リクエストパラメーター</param>
		/// <param name="paramName">パラメーター名</param>
		private static void AddProductListUrlParam(
			UrlCreator urlCreator,
			Dictionary<string, string> requestParam,
			string paramName)
		{
			if (IsProductUrlParameterOmit(requestParam[paramName]) == false)
			{
				urlCreator.AddParam(paramName, requestParam[paramName]);
			}
		}

		/// <summary>
		/// 商品一覧遷移URL作成用の<see cref="w2.Common.Web.UrlCreator"/>生成
		/// </summary>
		/// <param name="requestParam">リクエストパラメータ</param>
		/// <param name="targetUrl">遷移先Url</param>
		/// <returns><see cref="w2.Common.Web.UrlCreator"/></returns>
		private static UrlCreator CreateUrlCreatorForProductListUrl(Dictionary<string, string> requestParam, string targetUrl)
		{
			// 遷移先設定時
			if (string.IsNullOrEmpty(targetUrl) == false)
			{
				var urlCreator = new UrlCreator(Constants.PATH_ROOT + targetUrl)
					.AddParam(Constants.REQUEST_KEY_SHOP_ID, requestParam[Constants.REQUEST_KEY_SHOP_ID])
					.AddParam(Constants.REQUEST_KEY_CATEGORY_ID, requestParam[Constants.REQUEST_KEY_CATEGORY_ID]);
				if ((requestParam.ContainsKey(Constants.REQUEST_KEY_BRAND_SEARCH_ALL) && string.IsNullOrEmpty(StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_BRAND_ID])))
					|| (requestParam.ContainsKey(Constants.REQUEST_KEY_BRAND_ID) && (string.IsNullOrEmpty(StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_BRAND_ID])) == false)))
				{
					urlCreator.AddParam(Constants.REQUEST_KEY_BRAND_ID, requestParam[Constants.REQUEST_KEY_BRAND_ID]);
				}
				return urlCreator;
			}

			// フレンドリーURL使用時
			if ((Constants.FRIENDLY_URL_ENABLED)
				&& (string.IsNullOrEmpty(requestParam[URL_KEY_CATEGORY_NAME]) == false))
			{
				// ブランド使用時
				if ((Constants.PRODUCT_BRAND_ENABLED)
					&& (string.IsNullOrEmpty(requestParam[URL_KEY_BRAND_NAME]) == false)
					&& requestParam.ContainsKey(Constants.REQUEST_KEY_BRAND_ID))
				{
					var url = string.Format(
						"{0}{1}-{2}/brandcategory/{3}/{4}/{5}/",
						Constants.PATH_ROOT,
						HttpUtility.UrlEncode(EscapeFriendlyName(requestParam[URL_KEY_BRAND_NAME])),
						HttpUtility.UrlEncode(EscapeFriendlyName(requestParam[URL_KEY_CATEGORY_NAME])),
						HttpUtility.UrlEncode(StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_BRAND_ID])),
						HttpUtility.UrlEncode(requestParam[Constants.REQUEST_KEY_SHOP_ID]),
						HttpUtility.UrlEncode(requestParam[Constants.REQUEST_KEY_CATEGORY_ID]));
					return new UrlCreator(url);
				}
				// ブランド未使用時
				else
				{
					var url = string.Format(
						"{0}{1}/category/{2}/{3}/",
						Constants.PATH_ROOT,
						HttpUtility.UrlEncode(EscapeFriendlyName(requestParam[URL_KEY_CATEGORY_NAME])),
						HttpUtility.UrlEncode(requestParam[Constants.REQUEST_KEY_SHOP_ID]),
						HttpUtility.UrlEncode(requestParam[Constants.REQUEST_KEY_CATEGORY_ID]));

					// For case search brand has category
					if ((requestParam.ContainsKey(Constants.REQUEST_KEY_BRAND_SEARCH_ALL)
						&& (string.IsNullOrEmpty(StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_BRAND_ID])) == false)))
					{
						url = string.Format("{0}?{1}={2}", url, Constants.REQUEST_KEY_BRAND_ID, StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_BRAND_ID]));
					}

					return new UrlCreator(url);
				}
			}
			// フレンドリーURL未使用時
			else
			{
				var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCT_LIST)
					.AddParam(Constants.REQUEST_KEY_SHOP_ID, requestParam[Constants.REQUEST_KEY_SHOP_ID])
					.AddParam(Constants.REQUEST_KEY_CATEGORY_ID, requestParam[Constants.REQUEST_KEY_CATEGORY_ID]);
				if ((requestParam.ContainsKey(Constants.REQUEST_KEY_BRAND_SEARCH_ALL) && string.IsNullOrEmpty(StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_BRAND_ID])))
					|| (requestParam.ContainsKey(Constants.REQUEST_KEY_BRAND_ID) && (string.IsNullOrEmpty(StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_BRAND_ID])) == false)))
				{
					urlCreator.AddParam(Constants.REQUEST_KEY_BRAND_ID, requestParam[Constants.REQUEST_KEY_BRAND_ID]);
				}
				return urlCreator;
			}
		}

		/// <summary>
		/// 商品詳細URL作成(カテゴリIDをDBのCATEGORY_ID1から取得)
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="brandId"></param>
		/// <returns>商品詳細URL</returns>
		public static string CreateProductDetailUrlUseProductCategoryx(object product, string variationId, string currentBrandId)
		{
			var brandId = string.IsNullOrEmpty(currentBrandId) ? (string)GetKeyValue(product, Constants.FIELD_PRODUCT_BRAND_ID1) : currentBrandId;
			var brandName = ProductBrandUtility.GetProductBrandName(brandId);
			var productName = string.IsNullOrEmpty(variationId) ? (string)GetKeyValue(product, Constants.FIELD_PRODUCT_NAME) : CreateProductJointName(product);

			return CreateProductDetailUrl(
				(string)GetKeyValue(product, Constants.FIELD_PRODUCT_SHOP_ID),
				(string)GetKeyValue(product, Constants.FIELD_PRODUCT_CATEGORY_ID1),
				brandId,
				"",
				(string)GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID),
				variationId,
				productName,
				brandName);
		}

		/// <summary>
		/// 商品詳細URL作成
		/// </summary>
		/// <param name="objProduct">商品情報</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="strCategoryId">カテゴリID</param>
		/// <param name="strBrandId">ブランドID</param>
		/// <param name="strSearchWord">検索文字列</param>
		/// <param name="strBrandName">ブランド名</param>
		/// <returns>商品詳細URL</returns>
		public static string CreateProductDetailUrl(object objProduct, string strVariationId, string strCategoryId, string strBrandId, string strSearchWord, string strBrandName)
		{
			// フレンドリーURL名（商品名or商品バリエーション名）取得
			string strFriendlyUrlName = (string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_NAME);
			if (string.IsNullOrEmpty(strVariationId) == false)
			{
				strFriendlyUrlName += ProductCommon.CreateVariationName(
					(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1),
					(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2),
					(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3));
			}

			// 商品詳細URL作成
			return CreateProductDetailUrl(
				(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_SHOP_ID),
				strCategoryId,
				strBrandId,
				strSearchWord,
				(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_PRODUCT_ID),
				strVariationId,
				strFriendlyUrlName,
				strBrandName);
		}
		/// <summary>
		/// 商品詳細URL作成
		/// </summary>
		/// <param name="objShopId">店舗ID</param>
		/// <param name="objCategoryId">カテゴリID</param>
		/// <param name="objBrandId">ブランドID</param>
		/// <param name="objSearchWord">検索文字列</param>
		/// <param name="objProductId">商品ID</param>
		/// <param name="strProductName">商品名</param>
		/// <param name="strBrandName">ブランド名</param>
		/// <returns>商品詳細URL</returns>
		public static string CreateProductDetailUrl(
			object objShopId,
			object objCategoryId,
			object objBrandId,
			object objSearchWord,
			object objProductId,
			string strProductName,
			string strBrandName)
		{
			return CreateProductDetailUrl(objShopId, objCategoryId, objBrandId, objSearchWord, objProductId, "", strProductName, strBrandName);
		}
		/// <summary>
		/// 商品詳細URL作成
		/// </summary>
		/// <param name="objShopId">店舗ID</param>
		/// <param name="objCategoryId">カテゴリID</param>
		/// <param name="objBrandId">ブランドID</param>
		/// <param name="objSearchWord">検索文字列</param>
		/// <param name="objProductId">商品ID</param>
		/// <param name="objVariationId">商品バリエーションID</param>
		/// <param name="strProductName">商品（バリエーション）名</param>
		/// <param name="strBrandName">ブランド名</param>
		/// <param name="previewPageNo">商品プレビューのページ番号</param>
		/// <returns>商品詳細URL</returns>
		public static string CreateProductDetailUrl(
			object objShopId,
			object objCategoryId,
			object objBrandId,
			object objSearchWord,
			object objProductId,
			object objVariationId,
			string strProductName,
			string strBrandName,
			string previewPageNo = "")
		{
			StringBuilder sbUrl = new StringBuilder();
			var urlCreator = new UrlCreator("");

			//------------------------------------------------------
			// フレンドリーURL
			//------------------------------------------------------
			if ((Constants.FRIENDLY_URL_ENABLED)
				&& (string.IsNullOrEmpty(strProductName) == false))
			{
				if (Constants.PRODUCT_BRAND_ENABLED && (string.IsNullOrEmpty(strBrandName) == false))
				{
					// ブランドオプション:ON で、ブランド指定したアクセス
					sbUrl.Append(Constants.PATH_ROOT);
					sbUrl.Append(HttpUtility.UrlEncode(EscapeFriendlyName(strBrandName)));
					sbUrl.Append("-");
					sbUrl.Append(HttpUtility.UrlEncode(EscapeFriendlyName(strProductName)));
					sbUrl.Append("/brandproduct");
					sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objBrandId)));
					sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objShopId)));
					sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objProductId)));
					if (string.IsNullOrEmpty((string)objVariationId) == false)
					{
						sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objVariationId)));
					}
					sbUrl.Append("/");
				}
				else
				{
					// ブランドオプション:OFF
					// ブランドオプション:ON だが、ブランド無しアクセス（デフォルトブランドは設定なし）
					sbUrl.Append(Constants.PATH_ROOT);
					sbUrl.Append(HttpUtility.UrlEncode(EscapeFriendlyName(strProductName)));
					sbUrl.Append("/product");
					sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objShopId)));
					sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objProductId)));
					if (string.IsNullOrEmpty((string)objVariationId) == false)
					{
						sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objVariationId)));
					}
					sbUrl.Append("/");
				}
				urlCreator = new UrlCreator(sbUrl.ToString());
			}
			//------------------------------------------------------
			// フレンドリーURL未使用時
			//------------------------------------------------------
			else
			{
				sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PRODUCT_DETAIL);
				urlCreator = new UrlCreator(sbUrl.ToString());
				AddProductDetailUrlParam(urlCreator, Constants.REQUEST_KEY_SHOP_ID, StringUtility.ToEmpty(objShopId));
				AddProductDetailUrlParam(urlCreator, Constants.REQUEST_KEY_PRODUCT_ID, StringUtility.ToEmpty(objProductId));
				AddProductDetailUrlParam(urlCreator, Constants.REQUEST_KEY_VARIATION_ID, StringUtility.ToEmpty(objVariationId));
				if (string.IsNullOrEmpty(StringUtility.ToEmpty(objBrandId)) == false)
				{
					urlCreator.AddParam(Constants.REQUEST_KEY_BRAND_ID, StringUtility.ToEmpty(objBrandId));
				}
			}

			AddProductDetailUrlParam(urlCreator, Constants.REQUEST_KEY_CATEGORY_ID, StringUtility.ToEmpty(objCategoryId));
			AddProductDetailUrlParam(urlCreator, Constants.REQUEST_KEY_SEARCH_WORD, StringUtility.ToEmpty(objSearchWord));

			// 商品プレビューのページ番号が設定されているときは、パラメータに追加する
			if (string.IsNullOrEmpty(previewPageNo) == false)
			{
				AddProductDetailUrlParam(urlCreator, Constants.REQUEST_KEY_PAGE_NO, previewPageNo);
			}

			return urlCreator.CreateUrl();
		}

		/// <summary>
		/// 商品詳細URLパラメーター追加
		/// </summary>
		/// <param name="urlCreator">URLクエリ</param>
		/// <param name="paramName">パラメーター名</param>
		/// <param name="value">パラメーター値</param>
		private static void AddProductDetailUrlParam(UrlCreator urlCreator, string paramName, string value)
		{
			if (IsProductUrlParameterOmit(value)) return;

			urlCreator.AddParam(paramName, value);
		}

		/// <summary>
		/// 商品URL空パラメーター追加判定
		/// </summary>
		/// <param name="value">URLパラメーター値</param>
		/// <returns>true：除外する、false：除外しない</returns>
		private static bool IsProductUrlParameterOmit(string value)
		{
			var isParameterOmit = (Constants.FRONT_PRODUCTURL_OMIT_EMPTY_QUERYPARAMETER && string.IsNullOrEmpty(value));
			return isParameterOmit;
		}

		/// <summary>
		/// ブランドトップページ遷移URL作成
		/// </summary>
		/// <param name="objBrandId">ブランドID</param>
		/// <param name="strBrandName">ブランド名</param>
		/// <returns></returns>
		public static string CreateBrandTopPageUrl(object objBrandId, string strBrandName = null)
		{
			StringBuilder sbUrl = new StringBuilder();

			//------------------------------------------------------
			// フレンドリーURL（ブランド使用時）
			//------------------------------------------------------
			if ((Constants.FRIENDLY_URL_ENABLED)
				&& (Constants.PRODUCT_BRAND_ENABLED)
				&& (string.IsNullOrEmpty(strBrandName) == false))
			{
				sbUrl.Append(Constants.PATH_ROOT);
				sbUrl.Append(HttpUtility.UrlEncode(EscapeFriendlyName(strBrandName)));
				sbUrl.Append("/brandtop");
				sbUrl.Append("/").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objBrandId)));
				sbUrl.Append("/");
			}
			//------------------------------------------------------
			// フレンドリーURL未使用時
			//------------------------------------------------------
			else
			{
				sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_DEFAULT_BRAND_TOP);
				if (StringUtility.ToEmpty(objBrandId) != "")
				{
					sbUrl.Append("?").Append(Constants.REQUEST_KEY_BRAND_ID).Append("=").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(objBrandId)));
				}
			}

			return sbUrl.ToString();
		}

		/// <summary>
		/// 商品セット情報取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductSetId">商品セットID</param>
		/// <returns></returns>
		public static DataView GetProductSetInfo(string strShopId, string strProductSetId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "GetProductSet"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTSET_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID, strProductSetId);

				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 商品サブ画像設定取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <returns>商品サブ画像設定</returns>
		public static DataView GetProductSubImageSettingList(string strShopId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				using (SqlStatement sqlStatement = new SqlStatement("ProductSubImageSetting", "GetProductSubImageSettingList"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID, strShopId);
					htInput.Add(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO, Constants.PRODUCTSUBIMAGE_MAXCOUNT);

					return sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
				}
			}
		}

		/// <summary>
		/// 在庫文言作成
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="blIsPC">PCフラグ</param>
		/// <returns>在庫文言</returns>
		public static string CreateProductStockMessage(DataRowView drvProduct, bool blIsPC)
		{
			// 在庫管理している場合
			if ((string)drvProduct[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
			{
				if (drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] != DBNull.Value)
				{
					if (drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7] != DBNull.Value)
					{
						if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] <= (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE7];
						}
					}
					if (drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6] != DBNull.Value)
					{
						if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] <= (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE6];
						}
					}
					if (drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5] != DBNull.Value)
					{
						if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] <= (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE5];
						}
					}
					if (drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4] != DBNull.Value)
					{
						if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] <= (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE4];
						}
					}
					if (drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3] != DBNull.Value)
					{
						if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] <= (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE3];
						}
					}
					if (drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2] != DBNull.Value)
					{
						if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] <= (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE2];
						}
					}
					if (drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1] != DBNull.Value)
					{
						if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] <= (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE1];
						}
						else if ((int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK] > (int)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1])
						{
							return blIsPC ? (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] : (string)drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF_MOBILE];
						}
					}
				}
			}

			return StringUtility.ToEmpty((blIsPC ? drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] : drvProduct[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF_MOBILE]));
		}

		/// <summary>
		/// クロスセル情報取得(Mobile用)
		/// </summary>
		/// <param name="drvProduct">対象商品マスタ</param>
		/// <param name="strUserId">ユーザーID（デフォルトNULL）</param>
		/// <returns>クロスセル情報</returns>
		public static DataView GetProductCrossSellProducts(DataRowView drvProduct, string strUserId = null)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				return GetProductCrossSellProducts(drvProduct, sqlAccessor, Constants.SHOW_OUT_OF_STOCK_ITEMS, strUserId);
			}
		}
		/// <summary>
		/// クロスセル情報取得
		/// </summary>
		/// <param name="drvProduct">対象商品マスタ</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="showOutOfStock">在庫切れ商品を表示するかどうか</param>
		/// <param name="strUserId">ユーザーID（デフォルトNULL）</param>
		/// <returns>クロスセル情報</returns>
		public static DataView GetProductCrossSellProducts(DataRowView drvProduct, SqlAccessor sqlAccessor, bool showOutOfStock, string strUserId = null)
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, drvProduct[Constants.FIELD_PRODUCT_SHOP_ID]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5]);
			htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, MemberRankOptionUtility.GetMemberRankId(strUserId)); // 会員ランク
			htInput.Add(Constants.KEY_SHOW_OUT_OF_STOCK_ITEMS, showOutOfStock ? Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_VALID : Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_INVALID);

			return CreateProductUpCrossSellProducts("GetCrossSellProducts", htInput, sqlAccessor);
		}
		/// <summary>
		/// アップセル情報取得(Mobile用)
		/// </summary>
		/// <param name="drvProduct">対象商品マスタ</param>
		/// <param name="strUserId">ユーザーID（デフォルトNULL）</param>
		/// <returns>アップセル情報</returns>
		public static DataView GetProductUpSellProducts(DataRowView drvProduct, string strUserId = null)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				return GetProductUpSellProducts(drvProduct, sqlAccessor, Constants.SHOW_OUT_OF_STOCK_ITEMS, strUserId);
			}
		}
		/// <summary>
		/// アップセル情報取得
		/// </summary>
		/// <param name="drvProduct">対象商品マスタ</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="showOutOfStock">在庫切れ商品を表示するかどうか</param>
		/// <param name="strUserId">ユーザーID（デフォルトNULL）</param>
		/// <returns>アップセル情報</returns>
		public static DataView GetProductUpSellProducts(DataRowView drvProduct, SqlAccessor sqlAccessor, bool showOutOfStock, string strUserId = null)
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, drvProduct[Constants.FIELD_PRODUCT_SHOP_ID]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4]);
			htInput.Add(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5, drvProduct[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5]);
			htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, MemberRankOptionUtility.GetMemberRankId(strUserId)); // 会員ランク
			htInput.Add(Constants.KEY_SHOW_OUT_OF_STOCK_ITEMS, showOutOfStock ? Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_VALID : Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_INVALID);

			return CreateProductUpCrossSellProducts("GetUpSellProducts", htInput, sqlAccessor);
		}
		/// <summary>
		/// 関連商品情報取得
		/// </summary>
		/// <param name="strStatementName">ステートメント名</param>
		/// <param name="htInput">SQL発行用Hashtable</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ループデータ</returns>
		private static DataView CreateProductUpCrossSellProducts(string strStatementName, Hashtable htInput, SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement("Product", strStatementName))
			{
				return sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// モバイル商品一覧URL(フルパス)取得
		/// </summary>
		/// <param name="strSiteDomain">サイトドメイン</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strCategoryId">カテゴリID</param>
		/// <param name="strSearchText">検索文字列</param>
		/// <param name="strCampaignIcon">キャンペーンアイコン</param>
		/// <param name="strMinPrice">最小価格</param>
		/// <param name="strMaxPrice">最大価格</param>
		/// <param name="strSortKbn">ソート区分</param>
		/// <param name="strDispImage">画像表示区分</param>
		/// <param name="strBrandId">ブランドID</param>
		/// <returns>モバイル商品一覧URL(フルパス)</returns>
		public static string CreateMobileProductListUrl(
			string strSiteDomain,
			string strShopId,
			string strCategoryId,
			string strSearchText,
			string strCampaignIcon,
			string strMinPrice,
			string strMaxPrice,
			string strSortKbn,
			string strDispImage,
			string strBrandId)
		{
			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(Constants.PROTOCOL_HTTP).Append(strSiteDomain);
			sbUrl.Append(Constants.PATH_ROOT_FRONT_MOBILE);
			sbUrl.Append(CreateMobileProductListUrl(
				strShopId,
				strCategoryId,
				strSearchText,
				strCampaignIcon,
				strMinPrice,
				strMaxPrice,
				strSortKbn,
				strDispImage,
				strBrandId));

			return sbUrl.ToString();
		}
		/// <summary>
		/// モバイル商品一覧URL取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strCategoryId">カテゴリID</param>
		/// <param name="strSearchText">検索文字列</param>
		/// <param name="strCampaignIcon">キャンペーンアイコン</param>
		/// <param name="strMinPrice">最小価格</param>
		/// <param name="strMaxPrice">最大価格</param>
		/// <param name="strSortKbn">ソート区分</param>
		/// <param name="strDispImage">画像表示区分</param>
		/// <param name="strBrandId">ブランドID</param>
		/// <returns>モバイル商品一覧URL</returns>
		public static string CreateMobileProductListUrl(
			string strShopId,
			string strCategoryId,
			string strSearchText,
			string strCampaignIcon,
			string strMinPrice,
			string strMaxPrice,
			string strSortKbn,
			string strDispImage,
			string strBrandId)
		{
			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(Constants.PAGE_MFRONT_PRODUCT_LIST);
			sbUrl.Append("?").Append(Constants.REQUEST_KEY_MFRONT_PAGE_ID).Append("=").Append(Constants.PAGEID_MFRONT_PRODUCT_LIST);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_SHOP_ID).Append("=").Append(strShopId);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_CATEGORY_ID).Append("=").Append(strCategoryId);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_SEARCH_TEXT).Append("=").Append(strSearchText);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_CAMPAINGN_ICOM).Append("=").Append(strCampaignIcon);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MIN_PRICE).Append("=").Append(strMinPrice);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MAX_PRICE).Append("=").Append(strMaxPrice);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_SORT_KBN).Append("=").Append(strSortKbn);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_DISP_IMAGE).Append("=").Append(strDispImage);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_BRAND_ID).Append("=").Append(strBrandId);

			return sbUrl.ToString();
		}

		/// <summary>
		/// モバイル商品詳細URL(フルパス)取得
		/// </summary>
		/// <param name="strSiteDomain">サイトドメイン</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProdcutId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <returns>モバイル商品詳細URL(フルパス)</returns>
		public static string CreateMobileProductDetailUrl(string strSiteDomain, string strShopId, string strProdcutId, string strVariationId, string strBrandId)
		{
			StringBuilder sbMobileUrl = new StringBuilder();
			sbMobileUrl.Append(Constants.PROTOCOL_HTTP).Append(strSiteDomain);
			sbMobileUrl.Append(Constants.PATH_ROOT_FRONT_MOBILE);
			sbMobileUrl.Append(CreateMobileProductDetailUrl(strShopId, strProdcutId, strVariationId, strBrandId));

			return sbMobileUrl.ToString();
		}
		/// <summary>
		/// モバイル商品詳細URL取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProdcutId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <returns>モバイル商品詳細URL</returns>
		public static string CreateMobileProductDetailUrl(string strShopId, string strProdcutId, string strVariationId, string strBrandId)
		{
			StringBuilder sbMobileUrl = new StringBuilder();
			sbMobileUrl.Append(Constants.PAGE_MFRONT_PRODUCT_DETAIL);
			sbMobileUrl.Append("?").Append(Constants.REQUEST_KEY_MFRONT_PAGE_ID).Append("=").Append(Constants.PAGEID_MFRONT_PRODUCT_DETAIL);
			sbMobileUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_SHOP_ID).Append("=").Append(strShopId);
			sbMobileUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_PRODUCT_ID).Append("=").Append(strProdcutId);
			sbMobileUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_VARIATION_ID).Append("=").Append(strVariationId);
			sbMobileUrl.Append("&").Append(Constants.REQUEST_KEY_BRAND_ID).Append("=").Append(strBrandId);

			return sbMobileUrl.ToString();
		}

		/// <summary>
		/// ルートカテゴリから指定カテゴリまでのカテゴリ情報を取得
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strCurrentCategoryId">基準となるカテゴリID</param>
		/// <param name="onlyFixedPurchaseMemberFlg">Only fixed purchase member flag</param>
		/// <returns>親カテゴリ一覧（TOPカテゴリから順に格納されている）</returns>
		public static DataView GetParentAndCurrentCategories(string strShopId, string strCurrentCategoryId, string onlyFixedPurchaseMemberFlg)
		{
			DataView dvParentCategories = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetParentCategories"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, strCurrentCategoryId);
				htInput.Add(Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, onlyFixedPurchaseMemberFlg);

				dvParentCategories = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			return dvParentCategories;
		}

		/// <summary>
		/// フレンドリー名エスケープ処理
		/// </summary>
		/// <returns>フレンドリー名</returns>
		private static string EscapeFriendlyName(string strFriendlyName)
		{
			StringBuilder sbFriendlyName = new StringBuilder(strFriendlyName);

			// 「/」を「-」にエスケープ
			sbFriendlyName.Replace("/", "-");

			// 「<」、「>」をそれぞれ全角にエスケープ
			sbFriendlyName.Replace("<", "＜").Replace(">", "＞");

			// 「\」を「-」にエスケープ
			sbFriendlyName.Replace("\\", "-");

			// 「&」を「＆」全角にエスケープ
			sbFriendlyName.Replace("&", "＆");

			// 「?」を「？」全角にエスケープ
			sbFriendlyName.Replace("?", "？");

			// 「:」を「：」全角にエスケープ
			sbFriendlyName.Replace(":", "：");

			// 「%」を「％」全角にエスケープ
			sbFriendlyName.Replace("%", "％");

			// 「*」を「＊」全角にエスケープ
			sbFriendlyName.Replace("*", "＊");

			// 「.」を「．」全角にエスケープ
			sbFriendlyName.Replace(".", "．");

			return sbFriendlyName.ToString();
		}

		/// <summary>
		/// 商品マスタのカラム名取得
		/// </summary>
		/// <returns>商品マスタの全カラムリスト</returns>
		public static List<string> GetProductColumns()
		{
			List<string> productColumns = new List<string>();

			//------------------------------------------------------
			// 商品マスタ項目取得
			//------------------------------------------------------
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductColumns"))
			{
				foreach (DataColumn productMasterColumn in sqlStatement.SelectSingleStatementWithOC(sqlAccessor).Table.Columns)
				{
					productColumns.Add(productMasterColumn.ToString());
				}
			}

			return productColumns;
		}
		/// <summary>
		/// 商品タグマスタのカラム名取得
		/// </summary>
		/// <param name="productColumns">商品マスタのカラムリスト</param>
		/// <returns>商品タグマスタのカラムリスト（商品マスタと重複するカラム除く）</returns>
		public static List<string> GetProductTagColumns(List<string> productColumns)
		{
			List<string> productTagColumns = new List<string>();

			//------------------------------------------------------
			// 商品タグマスタ項目取得
			//------------------------------------------------------;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductTagColumns"))
			{
				DataView productTagMaster = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
				// 商品マスタにあるカラムは除外する
				foreach (string removeColumn in productColumns)
				{
					if (productTagMaster.Table.Columns.IndexOf(removeColumn) >= 0)
					{
						productTagMaster.Table.Columns.Remove(removeColumn);
					}
				}
				foreach (DataColumn productTagColumn in productTagMaster.Table.Columns)
				{
					productTagColumns.Add(productTagColumn.ToString());
				}
			}

			return productTagColumns;
		}

		/// <summary>
		/// 商品詳細検索条件作成
		/// </summary>
		/// <param name="requetParam">パラメーター</param>
		/// <param name="productMasterColumns">商品マスタカラム</param>
		/// <returns>商品詳細検索条件</returns>
		public static StringBuilder CreateAdvancedSearchWhere(Dictionary<string, string> requetParam, List<string> productMasterColumns)
		{
			var searchObjectList = new Dictionary<string, SearchObject>(m_searchModes); // ループ中に検索モードオブジェクトがXMLから変更する場合あるためインスタンス生成

			// 詳細検索条件
			var advancedSearchWhere = new StringBuilder();
			foreach (string tagKey in requetParam.Keys)
			{
				// カラム存在チェック
				if ((searchObjectList.ContainsKey(tagKey)) && (productMasterColumns.Contains(searchObjectList[tagKey].SearchField)))
				{
					// 検索演算子
					switch (searchObjectList[tagKey].SearchOperator)
					{
						case ProductCommon.SearchOperator.GreaterThan:
							advancedSearchWhere.Append(" AND w2_Product.").Append(searchObjectList[tagKey].SearchField).Append(" >= ");
							advancedSearchWhere.Append("'").Append(StringUtility.SqlLikeStringSharpEscape(requetParam[tagKey].Replace("'", "''"))).Append("'");
							break;

						case ProductCommon.SearchOperator.LessThan:
							advancedSearchWhere.Append(" AND w2_Product.").Append(searchObjectList[tagKey].SearchField).Append(" <= ");
							advancedSearchWhere.Append("'").Append(StringUtility.SqlLikeStringSharpEscape(requetParam[tagKey].Replace("'", "''"))).Append("'");
							break;

						case ProductCommon.SearchOperator.GreaterThanNotEqual:
							advancedSearchWhere.Append(" AND w2_Product.").Append(searchObjectList[tagKey].SearchField).Append(" > ");
							advancedSearchWhere.Append("'").Append(StringUtility.SqlLikeStringSharpEscape(requetParam[tagKey].Replace("'", "''"))).Append("'");
							break;

						case ProductCommon.SearchOperator.LessThanNotEqual:
							advancedSearchWhere.Append(" AND w2_Product.").Append(searchObjectList[tagKey].SearchField).Append(" < ");
							advancedSearchWhere.Append("'").Append(StringUtility.SqlLikeStringSharpEscape(requetParam[tagKey].Replace("'", "''"))).Append("'");
							break;

						case ProductCommon.SearchOperator.Equal:
							string prefixString = "";
							string suffixString = "";
							// 検索モード
							switch (searchObjectList[tagKey].SearchMode)
							{
								// 完全一致
								case ProductCommon.SearchMode.Perfect:
									prefixString = "";
									suffixString = "";
									break;

								// 部分一致
								case ProductCommon.SearchMode.Internal:
									prefixString = "%";
									suffixString = "%";
									break;

								// 前方一致
								case ProductCommon.SearchMode.Prefix:
									prefixString = "";
									suffixString = "%";
									break;

								// 後方一致
								case ProductCommon.SearchMode.Suffix:
									prefixString = "%";
									suffixString = "";
									break;
							}
							advancedSearchWhere.Append(" AND (");
							string values = requetParam[tagKey];
							int cnt = 0;
							foreach (string value in values.Split(','))
							{
								advancedSearchWhere.Append(cnt != 0 ? " OR " : "").Append("w2_Product.").Append(searchObjectList[tagKey].SearchField).Append(" LIKE ");
								advancedSearchWhere.Append("'").Append(prefixString).Append(StringUtility.SqlLikeStringSharpEscape(value.Replace("'", "''"))).Append(suffixString).Append("' ESCAPE '#'");
								cnt++;
							}
							advancedSearchWhere.Append(")");
							break;
					}
				}
			}
			return advancedSearchWhere;
		}

		/// <summary>
		/// 検索オブジェクト作成
		/// </summary>
		/// <param name="searchMode">検索モード</param>
		/// <param name="searchField">検索フィールド</param>
		/// <returns>検索オブジェクト</returns>
		public static SearchObject CreateSearchObject(string searchMode, string searchField)
		{
			SearchObject searchObject = new SearchObject();

			Regex regexGreaterThan = new Regex(ProductCommon.TAG_GREATER_THAN + "$");
			Regex regexLessThan = new Regex(ProductCommon.TAG_LESS_THAN + "$");
			Regex regexGreaterThanNotEqual = new Regex(ProductCommon.TAG_GREATER_THAN_NOT_EQUAL + "$");
			Regex regexLessThanNotEqual = new Regex(ProductCommon.TAG_LESS_THAN_NOT_EQUAL + "$");

			// 検索モード
			SearchMode sm;
			if (Enum.TryParse(searchMode, true, out sm) == false)
			{
				// 変換できない場合はデフォルト（Perfectモード）
				sm = SearchMode.Perfect;
			}
			searchObject.SearchMode = sm;

			// 検索カラム名、検索演算子取得
			if (regexGreaterThan.IsMatch(searchField))
			{
				searchObject.SearchField = regexGreaterThan.Replace(searchField, "");
				searchObject.SearchOperator = SearchOperator.GreaterThan;
			}
			else if (regexLessThan.IsMatch(searchField))
			{
				searchObject.SearchField = regexLessThan.Replace(searchField, "");
				searchObject.SearchOperator = SearchOperator.LessThan;
			}
			else if (regexGreaterThanNotEqual.IsMatch(searchField))
			{
				searchObject.SearchField = regexGreaterThanNotEqual.Replace(searchField, "");
				searchObject.SearchOperator = SearchOperator.GreaterThanNotEqual;
			}
			else if (regexLessThanNotEqual.IsMatch(searchField))
			{
				searchObject.SearchField = regexLessThanNotEqual.Replace(searchField, "");
				searchObject.SearchOperator = SearchOperator.LessThanNotEqual;
			}
			else
			{
				searchObject.SearchField = searchField;
				searchObject.SearchOperator = SearchOperator.Equal;
			}

			return searchObject;
		}

		/// <summary>
		/// 検索モード作成
		/// </summary>
		/// <remarks>詳細検索設定XMLを読み取り、検索モードオブジェクトを作成</remarks>
		private static void CreateAdvancedSearchModeSetting()
		{
			const string XML_NODE_SETTING = "Setting";
			const string XML_ATTRIBUTE_REQUESTKEY = "RequestKey";
			const string XML_ATTRIBUTE_SEARCHFIELD = "SearchField";
			const string XML_ATTRIBUTE_SEARCHMODE = "SearchMode";
			Dictionary<string, SearchObject> searchObjectList = new Dictionary<string, SearchObject>();

			try
			{
				// 詳細検索設定XMLを読み取り
				var settingList =
					from xdoc in XDocument.Load(Constants.PHYSICALDIRPATH_CONTENTS + Constants.FILE_ADVANCEDSEARCHSETTING).Descendants(XML_NODE_SETTING)
					where ((xdoc.Attribute(XML_ATTRIBUTE_REQUESTKEY) != null) && (xdoc.Attribute(XML_ATTRIBUTE_REQUESTKEY).Value != ""))
					select new
					{
						RequestKey = "_" + xdoc.Attribute(XML_ATTRIBUTE_REQUESTKEY).Value,
						SearchField = (xdoc.Attribute(XML_ATTRIBUTE_SEARCHFIELD) != null) ? xdoc.Attribute(XML_ATTRIBUTE_SEARCHFIELD).Value : "",
						SearchMode = (xdoc.Attribute(XML_ATTRIBUTE_SEARCHMODE) != null) ? xdoc.Attribute(XML_ATTRIBUTE_SEARCHMODE).Value : ""
					};

				// 検索モードオブジェクトを作成
				foreach (var setting in settingList)
				{
					if (searchObjectList.ContainsKey(setting.RequestKey)) continue;	// 重複は追加せず先勝ち

					searchObjectList.Add(setting.RequestKey, CreateSearchObject(setting.SearchMode, setting.SearchField));
				}
				m_searchModes = searchObjectList;
			}
			catch (Exception ex)
			{
				// XML構成エラーは詳細検索させないことで検知可能にする
				m_searchModes = new Dictionary<string, SearchObject>();
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// 商品グループ検索条件作成
		/// </summary>
		/// <returns>商品グループ検索条件</returns>
		public static string CreateProductGroupWhere()
		{
			var productGroupWhere = new StringBuilder();
			productGroupWhere.Append("AND EXISTS ( SELECT w2_ProductGroup.* FROM w2_ProductGroup ");
			productGroupWhere.Append(" INNER JOIN w2_ProductGroupItem ON ( w2_ProductGroup.product_group_id = w2_ProductGroupItem.product_group_id )");
			productGroupWhere.Append(" WHERE w2_ProductGroup.product_group_id = @product_group_id ");
			productGroupWhere.Append(" AND w2_ProductGroupItem.item_type = 'PRODUCT' ");
			productGroupWhere.Append(" AND w2_ProductGroupItem.shop_id = w2_Product.shop_id ");
			productGroupWhere.Append(" AND w2_ProductGroupItem.master_id = w2_Product.product_id ");
			productGroupWhere.Append(" AND w2_ProductGroup.begin_date <= GETDATE() ");
			productGroupWhere.Append(" AND ISNULL(w2_ProductGroup.end_date, GETDATE()) >= GETDATE() ");
			productGroupWhere.Append(" AND w2_ProductGroup.valid_flg = '1' )");

			return productGroupWhere.ToString();
		}

		/// <summary>
		/// 商品グループアイテム表示順ORDER BY作成
		/// </summary>
		/// <returns>商品グループアイテム表示順ORDER BY</returns>
		public static string CreateProductGroupItemOrderBy()
		{
			var productGroupItemOrderBy = new StringBuilder();
			productGroupItemOrderBy.Append("( SELECT w2_ProductGroupItem.item_no FROM w2_ProductGroupItem ");
			productGroupItemOrderBy.Append(" INNER JOIN w2_ProductGroup ON ");
			productGroupItemOrderBy.Append(" ( w2_ProductGroup.product_group_id = w2_ProductGroupItem.product_group_id ");
			productGroupItemOrderBy.Append(" AND w2_ProductGroup.begin_date <= GETDATE() ");
			productGroupItemOrderBy.Append(" AND ISNULL(w2_ProductGroup.end_date, GETDATE()) >= GETDATE() ");
			productGroupItemOrderBy.Append(" AND w2_ProductGroup.valid_flg = '1' ) ");
			productGroupItemOrderBy.Append(" WHERE w2_ProductGroupItem.product_group_id = @product_group_id ");
			productGroupItemOrderBy.Append(" AND w2_ProductGroupItem.item_type = 'PRODUCT' ");
			productGroupItemOrderBy.Append(" AND w2_ProductGroupItem.shop_id = w2_Product.shop_id ");
			productGroupItemOrderBy.Append(" AND  w2_ProductGroupItem.master_id = w2_Product.product_id ) ASC , ");

			return productGroupItemOrderBy.ToString();
		}

		/// <summary>
		/// お気に入り登録数ORDER BY作成
		/// </summary>
		/// <returns>お気に入り数ORDER BY</returns>
		public static string CreateFavariteCountOrderBy()
		{
			var favoriteCountOrderBy = new StringBuilder();
			favoriteCountOrderBy.Append(" ( SELECT COUNT(w2_Favorite.product_id) as favorite_cnt ");
			favoriteCountOrderBy.Append(" FROM w2_Favorite ");
			favoriteCountOrderBy.Append(" WHERE w2_Favorite.shop_id = w2_Product.shop_id ");
			favoriteCountOrderBy.Append(" AND w2_Favorite.product_id = w2_Product.product_id ");
			favoriteCountOrderBy.Append(" GROUP BY w2_Favorite.shop_id, ");
			favoriteCountOrderBy.Append(" w2_Favorite.product_id ) DESC, ");

			return favoriteCountOrderBy.ToString();
		}

		/// <summary>
		/// SEO用検索条件作成
		/// </summary>
		/// <param name="requetParam">パラメーター</param>
		/// <param name="productMasterColumns">商品マスタカラム</param>
		/// <returns>商品詳細検索条件</returns>
		public static StringBuilder CreateAdvancedSearchWhereForSeo(
			Dictionary<string, string> requetParam,
			List<string> productMasterColumns)
		{
			var searchObjectList = new Dictionary<string, SearchObject>(m_searchModes);

			// 詳細検索条件
			var advancedSearchWhere = new StringBuilder();
			var tags = new List<string>();
			foreach (var tagKey in requetParam.Keys)
			{
				// カラム存在チェック
				if ((searchObjectList.ContainsKey(tagKey))
					&& (productMasterColumns.Contains(searchObjectList[tagKey].SearchField))
					&& (searchObjectList[tagKey].SearchOperator == SearchOperator.Equal))
				{
					tags.Add("'" + searchObjectList[tagKey].SearchField + "'");
				}
			}

			if (tags.Count > 0)
			{
				var str = "AND tag_id IN ({0})";
				advancedSearchWhere.Append(string.Format(str, string.Join(",", tags)));
			}
			return advancedSearchWhere;
		}

		/// <summary>
		/// 販売期間内？
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>販売期間内：true、販売期間外：false</returns>
		public static bool IsSellTerm(object product)
		{
			return (((DateTime)GetKeyValue(product, Constants.FIELD_PRODUCT_SELL_FROM) <= DateTime.Now)
				&& ((GetKeyValue(product, Constants.FIELD_PRODUCT_SELL_TO) == DBNull.Value) ? true : (DateTime.Now <= (DateTime)GetKeyValue(product, Constants.FIELD_PRODUCT_SELL_TO))));
		}

		/// <summary>
		/// 販売前？
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>販売前：true、販売期間前以外：false</returns>
		public static bool IsSellBefore(object product)
		{
			return ((DateTime)GetKeyValue(product, Constants.FIELD_PRODUCT_SELL_FROM) > DateTime.Now);
		}

		/// <summary>
		/// 販売後？
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>販売後：true、販売後以外：false</returns>
		public static bool IsSellAfter(object product)
		{
			return ((IsSellBefore(product) == false)
				&& ((GetKeyValue(product, Constants.FIELD_PRODUCT_SELL_TO) == DBNull.Value) ? false : (DateTime.Now > (DateTime)GetKeyValue(product, Constants.FIELD_PRODUCT_SELL_TO))));
		}

		/// <summary>
		/// Create product detail url for send mail
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation id</param>
		/// <param name="productName">Product name</param>
		/// <returns>Product detail url</returns>
		public static string CreateProductDetailUrlForSendMail(
			string shopId,
			string productId,
			string variationId,
			string productName)
		{
			// ブランドオプション:OFF
			// ブランドオプション:ON だが、ブランド無しアクセス（デフォルトブランドは設定なし）
			if ((Constants.FRIENDLY_URL_ENABLED)
				&& (string.IsNullOrEmpty(productName) == false))
			{
				var urlBuilder = new StringBuilder(Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC)
					.Append(HttpUtility.UrlEncode(EscapeFriendlyName(productName)))
					.AppendFormat(
						"/product/{0}/{1}",
						HttpUtility.UrlEncode(shopId),
						HttpUtility.UrlEncode(productId));
				if (string.IsNullOrEmpty(variationId) == false)
				{
					urlBuilder.AppendFormat("/{0}", HttpUtility.UrlEncode(variationId));
				}
				urlBuilder.Append("/");
				var url = new UrlCreator(urlBuilder.ToString())
					.CreateUrl();
				return url;
			}

			var urlCreator = new UrlCreator(Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_PRODUCT_DETAIL);
			AddProductDetailUrlParam(
				urlCreator,
				Constants.REQUEST_KEY_SHOP_ID,
				shopId);
			AddProductDetailUrlParam(
				urlCreator,
				Constants.REQUEST_KEY_PRODUCT_ID,
				productId);
			AddProductDetailUrlParam(
				urlCreator,
				Constants.REQUEST_KEY_VARIATION_ID,
				variationId);
			return urlCreator.CreateUrl();
		}
	}
}
