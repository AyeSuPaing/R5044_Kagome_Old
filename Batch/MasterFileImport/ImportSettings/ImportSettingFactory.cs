using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public static class ImportSettingFactory
	{
		/// <summary>
		/// ImportSettingのインスタンスを作成
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="shopId">店舗ID（デフォルト"0"）</param>
		/// <returns>ImportSettingインスタンス</returns>
		public static ImportSettingBase CreateInstance(string tableName, string shopId = Constants.CONST_DEFAULT_SHOP_ID)
		{
			switch (tableName)
			{
				// 商品情報
				case Constants.TABLE_PRODUCT:
					return new ImportSettingProduct(shopId);

				// 商品バリエーション情報
				case Constants.TABLE_PRODUCTVARIATION:
					return new ImportSettingProductVariation(shopId);

				// 商品在庫情報
				case Constants.TABLE_PRODUCTSTOCK:
					return new ImportSettingProductStock(shopId);

				// 商品タグ情報
				case Constants.TABLE_PRODUCTTAG:
					return new ImportSettingProductTag();

				// 商品カテゴリ情報
				case Constants.TABLE_PRODUCTCATEGORY:
					return new ImportSettingProductCategory(shopId);

				// 商品拡張項目情報
				case Constants.TABLE_PRODUCTEXTEND:
					return new ImportSettingProductExtend(shopId);

				// 商品画像削除
				case Constants.ACTION_KBN_DELETE_PRODUCT_IMAGE:
					return new ImportSettingDeleteProductImage(shopId);

				// 商品レビュー情報
				case Constants.TABLE_PRODUCTREVIEW:
					return new ImportSettingProductReview(shopId);

				// ユーザー情報
				case Constants.TABLE_USER:
					return new ImportSettingUser(shopId);

				// ユーザー拡張情報
				case Constants.TABLE_USEREXTEND:
					return new ImportSettingUserExtend();

				// ユーザー情報(チェック不要)
				case Constants.TABLE_USERNOTVALIDATOR:
					return new ImportSettingUser(shopId, false);

				// ユーザーポイント情報
				case Constants.TABLE_USERPOINT:
					return new ImportSettingUserPoint(shopId);

				// ユーザー配送先情報
				case Constants.TABLE_USERSHIPPING:
					return new ImportSettingUserShipping(shopId);

				// ショートURL
				case Constants.TABLE_SHORTURL:
					return new ImportSettingShortUrl(shopId);

				// シリアルキー情報
				case Constants.TABLE_SERIALKEY:
					return new ImportSettingSerialKey(shopId);

				// 商品価格マスタ
				case Constants.TABLE_PRODUCTPRICE:
					return new ImportSettingProductPrice(shopId);

				// 商品セール価格マスタ
				case Constants.TABLE_PRODUCTSALEPRICE:
					return new ImportSettingProductSalePrice(shopId);

				// ターゲットリストデータ情報
				case Constants.TABLE_TARGETLISTDATA:
					return new ImportSettingTargetListData();

				// リアル店舗
				case Constants.TABLE_REALSHOP:
					return new ImportSettingRealShop(shopId);

				// リアル店舗商品在庫
				case Constants.TABLE_REALSHOPPRODUCTSTOCK:
					return new ImportSettingRealShopProductStock(shopId);

				// 広告コード
				case Constants.TABLE_ADVCODE:
					return new ImportSettingAdvCode();

				// Advertisement Code Media Type
				case Constants.TABLE_ADVCODEMEDIATYPE:
					return new ImportSettingAdvCodeMediaType();

				// クーポンマスタ
				case Constants.TABLE_COUPON:
					return new ImportSettingCoupon();

				// ユーザクーポンマスタ
				case Constants.TABLE_USERCOUPON:
					return new ImportSettingUserCoupon();

				// クーポン利用ユーザー情報
				case Constants.TABLE_COUPONUSEUSER:
					return new ImportSettingCouponUseUser();

				// 名称翻訳設定マスタ
				case Constants.TABLE_NAMETRANSLATIONSETTING:
					return new ImportSettingNameTranslationSetting();

				// DM発送履歴
				case Constants.TABLE_DMSHIPPINGHISTORY:
					return new ImportSettingDmShippingHistory();

				// スタッフ
				case Constants.TABLE_STAFF:
					return new ImportSettingStaff();

				// コーディネートカテゴリ
				case Constants.TABLE_COORDINATECATEGORY:
					return new ImportSettingCoordinateCategory();

				// コーディネート
				case Constants.TABLE_COORDINATE:
					return new ImportSettingCoordinate();

				// コーディネートアイテム
				case Constants.TABLE_COORDINATEITEM:
					return new ImportSettingCoordinateItem();

				// User credit card
				case Constants.TABLE_USERCREDITCARD:
					return new ImportSettingUserCreaditCard();

				// Order
				case Constants.TABLE_ORDER:
					return new ImportSettingOrder();

				// Fixed purchase
				case Constants.TABLE_FIXEDPURCHASE:
					return new ImportSettingFixedPurchase();

				// Fixed purchase history
				case Constants.TABLE_FIXEDPURCHASEHISTORY:
					return new ImportSettingFixedPurchaseHistory();

				// Fixed purchase item
				case Constants.TABLE_FIXEDPURCHASEITEM:
					return new ImportSettingFixedPurchaseItem();

				// Fixed purchase shipping
				case Constants.TABLE_FIXEDPURCHASESHIPPING:
					return new ImportSettingFixedPurchaseShipping();

				// CS incident
				case Constants.TABLE_CSINCIDENT:
					return new ImportSettingCsIncident();

				// CS message
				case Constants.TABLE_CSMESSAGE:
					return new ImportSettingCsMessage();

				// 店舗管理者マスタ
				case Constants.TABLE_SHOPOPERATOR:
					return new ImportSettingShopOperator(shopId);

				// 上記以外
				default:
					throw new w2Exception("テーブル「" + tableName + "」はマスタアップロードできません。");
			}
		}
	}
}
