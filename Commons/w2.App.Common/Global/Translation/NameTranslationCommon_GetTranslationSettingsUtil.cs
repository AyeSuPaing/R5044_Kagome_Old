/*
=========================================================================================================
  Module      : 名称翻訳処理クラス(翻訳設定情報取得ユーティリティ) (NameTranslationCommon_GetTranslationSettingsUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Global.Region;
using w2.Domain.Coordinate;
using w2.Domain.CoordinateCategory;
using w2.Domain.Coupon.Helper;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.News;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.ProductCategory;
using w2.Domain.ProductListDispSetting;
using w2.Domain.ProductStockMessage;
using w2.Domain.RealShop;
using w2.Domain.SetPromotion;
using w2.Domain.Staff;
using w2.Domain.User.Helper;

namespace w2.App.Common.Global.Translation
{
	/// <summary>
	/// 名称翻訳処理クラス(翻訳設定情報取得ユーティリティ)
	/// </summary>
	public partial class NameTranslationCommon
	{
		#region +GetTranslationSettings 翻訳設定情報取得
		/// <summary>
		/// 翻訳設定情報取得
		/// </summary>
		/// <param name="translationSettingGetTargetData">翻訳設定情報取得対象オブジェクト</param>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetTranslationSettings(object[] translationSettingGetTargetData, out string dataKbn)
		{
			dataKbn = string.Empty;
			if (Constants.GLOBAL_OPTION_ENABLE == false) return new NameTranslationSettingModel[0];

			var languageCode = RegionManager.GetInstance().Region.LanguageCode;
			var languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;

			if (translationSettingGetTargetData is NewsModel[])
			{
				// 新着情報
				dataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS;
				return GetNewsTranslationSettings((NewsModel[])translationSettingGetTargetData, languageCode, languageLocaleId);
			}
			else if (translationSettingGetTargetData is ProductListDispSettingModel[])
			{
				// 商品一覧表示設定
				dataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING;
				return GetProductListDispSettingTranslationSettings(
					(ProductListDispSettingModel[])translationSettingGetTargetData,
					languageCode,
					languageLocaleId);
			}
			else if (translationSettingGetTargetData is SetPromotionModel[])
			{
				// セットプロモーション
				dataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION;
				return GetSetPromotionTranslationSettings(
					(SetPromotionModel[])translationSettingGetTargetData,
					languageCode,
					languageLocaleId);
			}
			else if (translationSettingGetTargetData is ProductModel[])
			{
				// 商品情報
				dataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT;
				return GetProductAndVariationTranslationSettings(
					(ProductModel[])translationSettingGetTargetData,
					languageCode,
					languageLocaleId);
			}
			else if (translationSettingGetTargetData is UserCouponDetailInfo[])
			{
				// クーポン情報
				dataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON;
				return GetCouponTranslationSettings(
					(UserCouponDetailInfo[])translationSettingGetTargetData,
					languageCode,
					languageLocaleId);
			}
			else if (translationSettingGetTargetData is PaymentModel[])
			{
				// 決済種別
				dataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT;
				return GetPaymentTranslationSettings(
					(PaymentModel[])translationSettingGetTargetData,
					languageCode,
					languageLocaleId);
			}
			else if (translationSettingGetTargetData is ProductCategoryModel[])
			{
				// 商品カテゴリ
				dataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY;
				return GetProductCategoryTranslationSettings(
					(ProductCategoryModel[])translationSettingGetTargetData,
					languageCode,
					languageLocaleId);
			}
			
			return new NameTranslationSettingModel[0];
		}
		#endregion

		#region +GetSetPromotionTranslationSettings セットプロモーション翻訳情報取得
		/// <summary>
		/// セットプロモーション翻訳情報取得
		/// </summary>
		/// <param name="setPromotionList">セットプロモーション</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>セットプロモーション翻訳情報</returns>
		public static NameTranslationSettingModel[] GetSetPromotionTranslationSettings(SetPromotionModel[] setPromotionList, string languageCode, string languageLocaleId)
		{
			if (setPromotionList == null) return null;
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION,
				MasterId1List = setPromotionList.Select(setPromotion => setPromotion.SetpromotionId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};

			var translationSettings = new NameTranslationSettingService().GetSetPromotionTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetProductStockMessageTranslationData 商品在庫文言翻訳情報取得
		/// <summary>
		/// 商品在庫文言翻訳情報取得
		/// </summary>
		/// <param name="productStockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品在庫文言翻訳情報</returns>
		public static ProductStockMessageModel GetProductStockMessageTranslationSettings(string productStockMessageId, string languageCode, string languageLocaleId)
		{
			var stockMessageTranslationSetting = new ProductStockMessageService().GetProductStockMessageTranslationSetting(
				Constants.CONST_DEFAULT_SHOP_ID,
				productStockMessageId,
				languageCode,
				languageLocaleId);
			return stockMessageTranslationSetting;
		}
		#endregion

		#region +GetCouponTranslationSettings クーポン翻訳設定情報取得
		/// <summary>
		/// クーポン翻訳設定情報取得
		/// </summary>
		/// <param name="coupons">クーポンリスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>クーポン翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetCouponTranslationSettings(UserCouponDetailInfo[] coupons, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON,
				MasterId1List = coupons.Select(coupon => coupon.CouponId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetProductListDispSettingTranslationSettings 商品一覧表示設定翻訳設定情報取得
		/// <summary>
		/// 商品一覧表示設定翻訳設定情報取得
		/// </summary>
		/// <param name="productListDispSettings">商品一覧表示設定</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品一覧表示設定翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetProductListDispSettingTranslationSettings(
			ProductListDispSettingModel[] productListDispSettings,
			string languageCode,
			string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING,
				MasterId1List = productListDispSettings.Select(setting => setting.SettingId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetProductListDispSettingTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetProductTranslationSettings 商品情報翻訳設定情報取得
		/// <summary>
		/// 商品情報翻訳設定情報取得
		/// </summary>
		/// <param name="products">商品情報</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品情報翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetProductTranslationSettings(ProductModel[] products, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT,
				MasterId1List = products.Select(product => product.ProductId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetPaymentTranslationSettings 決済種別翻訳設定情報取得
		/// <summary>
		/// 決済種別翻訳設定情報取得
		/// </summary>
		/// <param name="payments">決済種別情報</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>決済種別翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetPaymentTranslationSettings(PaymentModel[] payments, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
				MasterId1List = payments.Select(payment => payment.PaymentId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetProductCategoryTranslationSettings 商品カテゴリ翻訳設定情報取得
		/// <summary>
		/// 商品カテゴリ翻訳設定情報取得
		/// </summary>
		/// <param name="categories">カテゴリ情報</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品カテゴリ翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetProductCategoryTranslationSettings(ProductCategoryModel[] categories, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY,
				MasterId1List = categories.Select(category => category.CategoryId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetUserExtendSettingTranslationSettings ユーザ拡張項目設定翻訳設定情報取得
		/// <summary>
		/// ユーザ拡張項目設定翻訳設定情報取得
		/// </summary>
		/// <param name="userExtendSettingList">ユーザ拡張項目リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>ユーザ拡張項目設定翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetUserExtendSettingTranslationSettings(UserExtendSettingList userExtendSettingList, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING,
				MasterId1List = userExtendSettingList.Items.Select(setting => setting.SettingId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetUserExtendSettingTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetProductAndVariationTranslationSettingsByVariationId 商品IDとバリエーションIDで商品／商品バリエーション翻訳情報取得
		/// <summary>
		/// 商品IDとバリエーションIDで商品／商品バリエーション翻訳情報取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品／商品バリエーション翻訳情報</returns>
		public static NameTranslationSettingModel[] GetProductAndVariationTranslationSettingsByVariationId(
			string productId,
			string variationId,
			string languageCode,
			string languageLocaleId)
		{
			var settings = GetProductAndVariationTranslationSettingsByProductId(
				productId,
				languageCode,
				languageLocaleId);

			var result = settings
				.Where(setting => ((setting.DataKbn != Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION)
					|| ((productId + setting.MasterId2) == variationId)))
				.ToArray();

			return result;
		}
		#endregion

		#region +GetProductAndVariationTranslationSettingsByProductId 商品IDで商品／商品バリエーション翻訳情報取得
		/// <summary>
		/// 商品IDで商品／商品バリエーション翻訳情報取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品／商品バリエーション翻訳情報</returns>
		public static NameTranslationSettingModel[] GetProductAndVariationTranslationSettingsByProductId(string productId, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				MasterId1 = productId,
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var productAndVariationTranslationSettings = new NameTranslationSettingService().GetProductAndVariationTranslationSettings(searchCondition);
			return productAndVariationTranslationSettings;
		}
		#endregion

		#region +GetProductAndVariationTranslationSettings 商品／商品バリエーション翻訳設定情報取得
		/// <summary>
		/// 商品／商品バリエーション翻訳設定情報取得
		/// </summary>
		/// <param name="products">商品情報リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品／商品バリエーション翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetProductAndVariationTranslationSettings(ProductModel[] products, string languageCode, string languageLocaleId)
		{
			// 先に商品の翻訳情報を取得
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT,
				MasterId1List = products.Select(product => product.ProductId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var productTranslationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

			// 商品バリエーションの翻訳情報を取得
			searchCondition.DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION;
			var variationTranslationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

			var translationSettings = productTranslationSettings.Concat(variationTranslationSettings).ToArray();
			return translationSettings;
		}
		#endregion

		#region +GetNewsTranslationSettings 新着情報翻訳設定情報取得
		/// <summary>
		/// 新着情報翻訳設定情報取得
		/// </summary>
		/// <param name="newsList">新着情報リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>新着情報翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetNewsTranslationSettings(NewsModel[] newsList, string languageCode, string languageLocaleId)
		{
			if ((newsList == null) || (newsList.Length == 0)) return new NameTranslationSettingModel[0];
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS,
				MasterId1List = newsList.Select(news => news.NewsId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetNewsTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetProductSetTranslationSettings 商品セット翻訳設定情報取得
		/// <summary>
		/// 商品セット翻訳設定情報取得
		/// </summary>
		/// <param name="productSetIdList">商品セットIDリスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>商品セット翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetProductSetTranslationSettings(string[] productSetIdList, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET,
				MasterId1List = productSetIdList.ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetNewsTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetOrderMemoSettingTranslationSettings 注文メモ設定翻訳設定情報取得
		/// <summary>
		/// 注文メモ設定翻訳設定情報取得
		/// </summary>
		/// <param name="orderMemoIdList">注文メモIDリスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>注文メモ設定翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetOrderMemoSettingTranslationSettings(string[] orderMemoIdList, string languageCode, string languageLocaleId)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING,
				MasterId1List = orderMemoIdList.ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetNewsTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetCoordinateTranslationSettings コーディネート設定翻訳設定情報取得
		/// <summary>
		/// コーディネート設定翻訳設定情報取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="coordinates">コーディネートモデルリスト</param>
		/// <returns>コーディネート設定翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetCoordinateTranslationSettings(string languageCode, string languageLocaleId, params CoordinateModel[] coordinates)
		{
			if ((coordinates.Any() == false)) return new NameTranslationSettingModel[0];

			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE,
				MasterId1List = coordinates.Select(coordinate => coordinate.CoordinateId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetCoordinateTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetCoordinateCategoryTranslationSettings コーディネートカテゴリ設定翻訳設定情報取得
		/// <summary>
		/// コーディネートカテゴリ設定翻訳設定情報取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="coordinateCategories">コーディネートカテゴリ</param>
		/// <returns>コーディネートカテゴリ設定翻訳設定情報</returns>
		public static NameTranslationSettingModel[] GetCoordinateCategoryTranslationSettings(string languageCode, string languageLocaleId, params CoordinateCategoryModel[] coordinateCategories)
		{
			if ((coordinateCategories.Any() == false)) return new NameTranslationSettingModel[0];

			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY,
				MasterId1List = coordinateCategories.Select(category => category.CoordinateCategoryId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetCoordinateCategoryTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetStaffTranslationSettings スタッフ設定翻訳設定情報取得
		/// <summary>
		/// スタッフ設定翻訳設定情報取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="staffs">スタッフ</param>
		/// <returns>スタッフ設定翻訳設定情報取得</returns>
		public static NameTranslationSettingModel[] GetStaffTranslationSettings(string languageCode, string languageLocaleId, params StaffModel[] staffs)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF,
				MasterId1List = staffs.Select(staff => staff.StaffId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetStaffTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion

		#region +GetRealShopTranslationSettings リアル店舗設定翻訳設定情報取得
		/// <summary>
		/// リアル店舗設定翻訳設定情報取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="shops">リアル店舗</param>
		/// <returns>リアル店舗設定翻訳設定情報取得</returns>
		public static NameTranslationSettingModel[] GetRealShopTranslationSettings(string languageCode, string languageLocaleId, params RealShopModel[] shops)
		{
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP,
				MasterId1List = shops.Select(shop => shop.RealShopId).ToList(),
				LanguageCode = languageCode,
				LanguageLocaleId = languageLocaleId,
			};
			var translationSettings = new NameTranslationSettingService().GetRealShopTranslationSettingsForFrontDisplay(searchCondition);
			return translationSettings;
		}
		#endregion
	}
}
