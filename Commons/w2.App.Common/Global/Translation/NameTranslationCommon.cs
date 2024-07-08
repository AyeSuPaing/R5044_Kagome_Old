/*
=========================================================================================================
  Module      : 名称翻訳処理クラス (NameTranslationCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.Linq;
using w2.Domain.Coupon.Helper;
using w2.Domain.Helper.Attribute;
using w2.Domain.NameTranslationSetting;
using w2.Domain.News;
using w2.Domain.Payment;
using w2.Domain.ProductListDispSetting;
using w2.Domain.ProductStockMessage;
using w2.Domain.SetPromotion;
using w2.Domain.User.Helper;
using w2.Domain.UserExtendSetting;
using w2.App.Common.Global.Region;
using w2.Domain;
using w2.Domain.Coordinate;
using w2.Domain.Coordinate.Helper;
using w2.Domain.CoordinateCategory;
using w2.Domain.Product;
using w2.Domain.RealShop;
using w2.Domain.Staff;
using w2.Domain.Staff.Helper;

namespace w2.App.Common.Global.Translation
{
	/// <summary>
	/// 名称翻訳処理クラス
	/// </summary>
	public partial class NameTranslationCommon
	{
		#region +Translate 翻訳処理
		/// <summary>
		/// 翻訳処理
		/// </summary>
		/// <param name="translateDataSetTargetData">翻訳情報設定対象データ</param>
		/// <param name="translateTargetList">翻訳対象リスト</param>
		/// <returns>翻訳情報設定後データ</returns>
		/// <remarks>DataViewまたはDataRowViewのデータに設定する</remarks>
		public static object Translate(object translateDataSetTargetData, object[] translateTargetList)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return translateDataSetTargetData;

			// 翻訳設定情報取得
			string dataKbn;
			var translationSettings = GetTranslationSettings(translateTargetList, out dataKbn);

			// 対象が商品の場合は別処理で設定
			if (dataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT)
			{
				return TranslateProductAndVariationData(translateDataSetTargetData, translationSettings);
			}

			// 翻訳情報設定
			if (translateDataSetTargetData is DataView)
			{
				SetTranslationDataToDataView((DataView)translateDataSetTargetData, dataKbn, translationSettings);
			}
			else if (translateDataSetTargetData is DataRowView)
			{
				SetTranslationDataToDataRowView((DataRowView)translateDataSetTargetData, dataKbn, translationSettings);
			}

			return translateDataSetTargetData;
		}
		#endregion

		#region +SetTranslationDataToDataView DataViewに翻訳情報設定
		/// <summary>
		/// DataViewに翻訳情報設定
		/// </summary>
		/// <param name="data">設定対象データ</param>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳情報設定後データ</returns>
		public static DataView SetTranslationDataToDataView(DataView data, string dataKbn, NameTranslationSettingModel[] translationSettings)
		{
			if ((data.Count == 0) || (translationSettings.Any() == false)) return data;

			foreach (DataRowView drv in data)
			{
				SetTranslationDataToDataRowView(drv, dataKbn, translationSettings);
			}
			return data;
		}
		#endregion

		#region +SetTranslationDataToDataRowView DataRowViewに翻訳情報設定
		/// <summary>
		/// DataRowViewに翻訳情報設定
		/// </summary>
		/// <param name="datum">設定対象データ</param>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳情報設定後データ</returns>
		public static DataRowView SetTranslationDataToDataRowView(DataRowView datum, string dataKbn, NameTranslationSettingModel[] translationSettings)
		{
			if ((datum == null) || (translationSettings.Any() == false)) return datum;

			// 判定用のキー項目名を取得する
			var keyName = GetKeyColumnName(dataKbn);

			// 商品翻訳情報設定
			foreach (var setting in translationSettings)
			{
				if (setting.MasterId1 != (string)datum[keyName]) continue;
				datum = SetTranslationName(datum, setting);
			}
			return datum;
		}
		#endregion

		#region +SetTranslationDataToModel Modelに翻訳情報設定
		/// <summary>
		/// Modelに翻訳情報設定
		/// </summary>
		/// <param name="modelList">設定対象モデル配列</param>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳情報設定後データ</returns>
		public static IModel[] SetTranslationDataToModel(IModel[] modelList, string dataKbn, NameTranslationSettingModel[] translationSettings)
		{
			if ((modelList.Length == 0) || (translationSettings.Any() == false)) return modelList;

			foreach (var model in modelList)
			{
				if ((model == null) || (translationSettings.Any() == false)) continue;

				// 判定用のキー項目名を取得する
				var keyName = GetKeyColumnName(dataKbn);

				// 商品翻訳情報設定
				foreach (var setting in translationSettings)
				{
					if (setting.MasterId1 != (string)model.DataSource[keyName]) continue;
					SetTranslationName(model, setting);
				}
			}
			return modelList;
		}
		#endregion

		#region -SetTranslationName 翻訳名設定
		/// <summary>
		/// 翻訳名設定
		/// </summary>
		/// <param name="targetDatum">対象データ</param>
		/// <param name="translationSetting">翻訳設定情報</param>
		/// <returns>翻訳名設定後データ</returns>
		private static DataRowView SetTranslationName(DataRowView targetDatum, NameTranslationSettingModel translationSetting)
		{
			// 対象データにカラムが無ければ翻訳せず返す
			if (targetDatum.DataView.Table.Columns.Contains(translationSetting.TranslationTargetColumn) == false)
			{
				return targetDatum;
			}
			targetDatum[translationSetting.TranslationTargetColumn] = translationSetting.AfterTranslationalName;
			if (Constants.UseDisplayKbnColumn.Contains(translationSetting.TranslationTargetColumn))
			{
				var displayKbnFieldName = GetDisplayKbnFieldName(translationSetting.TranslationTargetColumn);
				targetDatum[displayKbnFieldName] = translationSetting.DisplayKbn;
			}
			return targetDatum;
		}
		#endregion

		#region -SetTranslationName 翻訳名設定(Modelに)
		/// <summary>
		/// 翻訳名設定(Modelに)
		/// </summary>
		/// <param name="model">対象モデル</param>
		/// <param name="translationSetting">翻訳設定情報</param>
		/// <returns>翻訳名設定後データ</returns>
		private static IModel SetTranslationName(IModel model, NameTranslationSettingModel translationSetting)
		{
			// 対象データにカラムが無ければ翻訳せず返す
			if (model.DataSource.Contains(translationSetting.TranslationTargetColumn) == false)
			{
				return model;
			}
			model.DataSource[translationSetting.TranslationTargetColumn] = translationSetting.AfterTranslationalName;
			if (Constants.UseDisplayKbnColumn.Contains(translationSetting.TranslationTargetColumn))
			{
				var displayKbnFieldName = GetDisplayKbnFieldName(translationSetting.TranslationTargetColumn);
				model.DataSource[displayKbnFieldName] = translationSetting.DisplayKbn;
			}
			return model;
		}
		#endregion

		#region -GetKeyColumnName キー項目名取得
		/// <summary>
		/// キー項目名取得
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>キー項目名</returns>
		/// <remarks>DataView,DataRowViewに翻訳情報を設定する際に使用するキー項目</remarks>
		private static string GetKeyColumnName(string dataKbn)
		{
			switch (dataKbn)
			{
				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT:
					return Constants.FIELD_PRODUCT_PRODUCT_ID;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY:
					return Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT:
					return Constants.FIELD_PAYMENT_PAYMENT_ID;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING:
					return Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION:
					return Constants.FIELD_SETPROMOTION_SETPROMOTION_ID;

				case Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET:
					return Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID;

			}
			return string.Empty;
		}
		#endregion

		#region -GetDisplayKbnFieldName 表示HTML区分フィールド名取得
		/// <summary>
		/// 表示HTML区分フィールド名取得
		/// </summary>
		/// <param name="displayField">表示フィールド</param>
		/// <returns>表示HTML区分フィールド名</returns>
		/// <remarks>HTML区分を利用する項目の場合、「HTML/TEXT」を設定するフィールド名を特定して取得する</remarks>
		private static string GetDisplayKbnFieldName(string displayField)
		{
			switch (displayField)
			{
				case Constants.FIELD_PRODUCT_OUTLINE:
					return Constants.FIELD_PRODUCT_OUTLINE_KBN;

				case Constants.FIELD_PRODUCT_DESC_DETAIL1:
					return Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1;

				case Constants.FIELD_PRODUCT_DESC_DETAIL2:
					return Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2;

				case Constants.FIELD_PRODUCT_DESC_DETAIL3:
					return Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3;

				case Constants.FIELD_PRODUCT_DESC_DETAIL4:
					return Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4;

				case Constants.FIELD_PRODUCTSET_DESCRIPTION:
					return Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN;
			}
			return string.Empty;
		}
		#endregion

		#region +TranslateProductAndVariationData 商品／商品バリエーション情報翻訳
		/// <summary>
		/// 商品／商品バリエーション情報翻訳
		/// </summary>
		/// <param name="translateDataSetTargetData">翻訳情報設定対象データ</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後商品／商品バリエーション情報</returns>
		public static object TranslateProductAndVariationData(object translateDataSetTargetData, NameTranslationSettingModel[] translationSettings)
		{
			// 翻訳情報設定
			if (translateDataSetTargetData is DataView)
			{
				SetProductAndVariationTranslationDataToDataView((DataView)translateDataSetTargetData, translationSettings);
			}
			else if (translateDataSetTargetData is DataRowView)
			{
				SetProductAndVariationTranslationDataToDataRowView((DataRowView)translateDataSetTargetData, translationSettings);
			}

			return translateDataSetTargetData;
		}
		#endregion

		#region +SetProductAndVariationTranslationDataToDataView DataViewに商品／商品バリエーション翻訳情報設定
		/// <summary>
		/// DataViewに商品／商品バリエーション翻訳情報設定
		/// </summary>
		/// <param name="productList">商品／商品バリエーション情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後商品／商品バリエーション情報</returns>
		public static DataView SetProductAndVariationTranslationDataToDataView(DataView productList, NameTranslationSettingModel[] translationSettings)
		{
			if ((translationSettings.Any() == false) || (productList.Count == 0)) return productList;

			foreach (DataRowView product in productList)
			{
				SetProductAndVariationTranslationDataToDataRowView(product, translationSettings);
			}
			return productList;
		}
		#endregion

		#region +SetProductAndVariationTranslationDataToDataRowView DataRowViewに商品／商品バリエーション翻訳情報設定
		/// <summary>
		/// DataRowViewに商品／商品バリエーション翻訳情報設定
		/// </summary>
		/// <param name="product">商品／商品バリエーション情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後商品／商品バリエーション情報</returns>
		public static DataRowView SetProductAndVariationTranslationDataToDataRowView(DataRowView product, NameTranslationSettingModel[] translationSettings)
		{
			if (translationSettings.Any() == false) return product;

			var productTranslationSettings = translationSettings.Where(
				setting => (setting.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT)).ToArray();
			var variationTranslationSetting = translationSettings.Where(
				setting => (setting.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION)).ToArray();

			// 商品翻訳情報設定
			foreach (var setting in productTranslationSettings)
			{
				if (setting.MasterId1 != (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID]) continue;
				product = SetTranslationName(product, setting);
			}

			// バリエーション未使用、またはバリエーション情報がDataRowViewに含まれていない場合はスキップ
			if ((product.DataView.Table.Columns.Contains(Constants.FIELD_PRODUCT_USE_VARIATION_FLG) == false)
				|| ((string)product[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] != Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
				|| (product.DataView.Table.Columns.Contains(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) == false)) return product;

			// バリエーション翻訳情報設定
			foreach (var setting in variationTranslationSetting)
			{
				if ((setting.MasterId1 != (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID])
					|| (setting.MasterId2 != (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])) continue;

				product = SetTranslationName(product, setting);
			}
			return product;
		}
		#endregion

		#region +SetProductStockMessageTranslationData 商品在庫文言翻訳情報設定
		/// <summary>
		/// 商品在庫文言翻訳情報設定
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="translationSettings">商品在庫文言翻訳情報</param>
		/// <returns>商品情報</returns>
		public static DataRowView SetProductStockMessageTranslationData(DataRowView product, ProductStockMessageModel translationSettings)
		{
			if (translationSettings != null)
			{
				var dictionary = translationSettings.GetType().GetProperties()
					.Where(property => Attribute.GetCustomAttribute(property, typeof(DbMapName)) != null)
					.ToDictionary(
						property => ((DbMapName)Attribute.GetCustomAttribute(property, typeof(DbMapName))).MapName,
						property => property.GetValue(translationSettings, null));

				foreach (var dic in dictionary)
				{
					if (product.DataView.Table.Columns.Contains(dic.Key) == false) continue;
					product[dic.Key] = dic.Value ?? DBNull.Value;
				}
			}
			return product;
		}
		/// <summary>
		/// 在庫文言翻訳情報設定
		/// </summary>
		/// <param name="productVariationStockInfos">商品バリエーション在庫情報</param>
		private static void SetProductStockMessageTranslationData(ref ProductVariationStockInfo[] productVariationStockInfos)
		{
			// 在庫文言の設定は商品情報に持つため、1つ目の情報を取得する
			var product = productVariationStockInfos[0];
			if (product.StockMessageDef == null) return;

			var translationSettings = GetProductStockMessageTranslationSettings(
				product.StockMessageId,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
			productVariationStockInfos = productVariationStockInfos
				.Select(info => SetProductStockMessageTranslationData(info, translationSettings)).ToArray();
		}
		/// <summary>
		/// 在庫文言翻訳情報設定
		/// </summary>
		/// <param name="productVariationStockInfo">商品バリエーション在庫情報</param>
		/// <param name="stockMessageGlobal">商品在庫翻訳文言</param>
		/// <returns>翻訳後在庫文言情報</returns>
		private static ProductVariationStockInfo SetProductStockMessageTranslationData(ProductVariationStockInfo productVariationStockInfo, ProductStockMessageModel stockMessageGlobal)
		{
			if (stockMessageGlobal == null) return productVariationStockInfo;

			// 表示しているカラム名から、設定先のプロパティを取得する
			var messageColumnName = (productVariationStockInfo.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
				? Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF
				: productVariationStockInfo.StockMessageDisplayColumnName;

			var stockMessageTranslationProperty = stockMessageGlobal.GetType().GetProperties()
				.Where(property => Attribute.GetCustomAttribute(property, typeof(DbMapName)) != null)
				.FirstOrDefault(property => ((DbMapName)Attribute.GetCustomAttribute(property, typeof(DbMapName))).MapName == messageColumnName);

			// 翻訳情報を設定
			var translationMessage = (string)stockMessageTranslationProperty.GetValue(stockMessageGlobal);
			if (string.IsNullOrEmpty(translationMessage) == false)
			{
				productVariationStockInfo.StockMessage = translationMessage;
			}

			if (string.IsNullOrEmpty(stockMessageGlobal.StockMessageDef) == false)
			{
				productVariationStockInfo.StockMessageDef = stockMessageGlobal.StockMessageDef;
			}
			return productVariationStockInfo;
		}
		#endregion

		#region SetCouponTranslationData クーポン翻訳情報設定
		/// <summary>
		/// クーポン翻訳情報設定
		/// </summary>
		/// <param name="coupons">クーポン情報リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後クーポン情報</returns>
		public static UserCouponDetailInfo[] SetCouponTranslationData(UserCouponDetailInfo[] coupons, string languageCode, string languageLocaleId)
		{
			// 翻訳情報取得
			var translationSettings = GetCouponTranslationSettings(coupons, languageCode, languageLocaleId);
			if (translationSettings.Any() == false) return coupons;

			// 翻訳情報設定
			coupons = coupons.Select(coupon => SetCouponTranslationData(coupon, translationSettings)).ToArray();
			return coupons;
		}
		/// <summary>
		/// クーポン翻訳情報設定
		/// </summary>
		/// <param name="coupon">クーポン情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後クーポン情報</returns>
		public static UserCouponDetailInfo SetCouponTranslationData(UserCouponDetailInfo coupon, NameTranslationSettingModel[] translationSettings)
		{
			// 表示用クーポン名
			var couponDispNameTranslationSetting = translationSettings.FirstOrDefault(
				setting => ((setting.MasterId1 == coupon.CouponId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_NAME)));

			coupon.CouponDispName = (couponDispNameTranslationSetting != null)
				? couponDispNameTranslationSetting.AfterTranslationalName
				: coupon.CouponDispName;

			// フロント表示用クーポン説明文
			var couponDispDiscriptionTranslationSetting = translationSettings.FirstOrDefault(
				setting => ((setting.MasterId1 == coupon.CouponId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_DISCRIPTION)));

			coupon.CouponDispDiscription = (couponDispDiscriptionTranslationSetting != null)
				? couponDispDiscriptionTranslationSetting.AfterTranslationalName
				: coupon.CouponDispDiscription;

			return coupon;
		}
		#endregion

		#region +SetProductListDispSettingTranslationData 商品一覧表示設定翻訳情報設定
		/// <summary>
		/// 商品一覧表示設定翻訳情報設定
		/// </summary>
		/// <param name="productListDispSettings">商品一覧表示設定リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後商品一覧表示設定</returns>
		public static ProductListDispSettingModel[] SetProductListDispSettingTranslationData(
			ProductListDispSettingModel[] productListDispSettings,
			string languageCode,
			string languageLocaleId)
		{
			// 翻訳設定情報取得
			var translationSettings = GetProductListDispSettingTranslationSettings(productListDispSettings, languageCode, languageLocaleId);

			// 翻訳情報設定
			productListDispSettings = productListDispSettings
				.Select(setting => SetProductListDispSettingTranslationData(setting, translationSettings)).ToArray();

			return productListDispSettings;
		}
		/// <summary>
		/// 商品一覧表示設定翻訳情報設定
		/// </summary>
		/// <param name="productListDispSetting">商品一覧表示設定</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後商品一覧表示設定</returns>
		public static ProductListDispSettingModel SetProductListDispSettingTranslationData(
			ProductListDispSettingModel productListDispSetting,
			NameTranslationSettingModel[] translationSettings)
		{
			var translationSetting = translationSettings.FirstOrDefault(
				setting =>
					(setting.MasterId1 == productListDispSetting.SettingId)
					&& (setting.MasterId2 == productListDispSetting.SettingKbn));

			// 翻訳前の情報が設定されていない場合は、設定しておく
			if (string.IsNullOrEmpty(productListDispSetting.SettingNameBeforeTranslation))
			{
				productListDispSetting.SettingNameBeforeTranslation = productListDispSetting.SettingName;
			}

			// 設定名
			if (translationSetting != null)
			{
				productListDispSetting.SettingName = translationSetting.AfterTranslationalName;
				productListDispSetting.SettingNameBeforeTranslation = translationSetting.ProductListDispSettingName;
			}
			else
			{
				// 翻訳情報が存在しない場合は、マスタに登録されている情報を表示
				productListDispSetting.SettingName = productListDispSetting.SettingNameBeforeTranslation;
			}

			return productListDispSetting;
		}
		#endregion

		#region +SetSetPromotionTranslationData セットプロモーション翻訳情報設定
		/// <summary>
		/// セットプロモーション翻訳情報設定
		/// </summary>
		/// <param name="setPromotionList">セットプロモーション情報リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳情報設定後セットプロモーション情報</returns>
		public static SetPromotionModel[] SetSetPromotionTranslationData(SetPromotionModel[] setPromotionList, string languageCode, string languageLocaleId)
		{
			if (setPromotionList == null) return null;
			// 翻訳情報を取得して、設定する
			var translationSettings = GetSetPromotionTranslationSettings(setPromotionList, languageCode, languageLocaleId);
			setPromotionList = setPromotionList
				.Select(setPromotion => SetSetPromotionTranslationData(setPromotion, translationSettings)).ToArray();
			return setPromotionList;
		}
		/// <summary>
		/// セットプロモーション翻訳情報設定
		/// </summary>
		/// <param name="setPromotion">セットプロモーション情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後セットプロモーション情報</returns>
		/// <remarks>フロント表示時、セップロ情報はキャッシュから取得しているため、翻訳前の情報を事前に保持して表示切替する必要がある</remarks>
		public static SetPromotionModel SetSetPromotionTranslationData(SetPromotionModel setPromotion, NameTranslationSettingModel[] translationSettings)
		{
			// 翻訳情報取得(※翻訳情報の登録が無い場合は空のインスタンスをセット)
			var setPromotionDispNameTranslationSetting =
				translationSettings.FirstOrDefault(d => ((d.MasterId1 == setPromotion.SetpromotionId)
					&& (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME)))
				?? new NameTranslationSettingModel { AfterTranslationalName = string.Empty };
			
			var descriptionTranslationSetting =
				translationSettings.FirstOrDefault(d => ((d.MasterId1 == setPromotion.SetpromotionId)
					&& (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_DESCRIPTION)))
				?? new NameTranslationSettingModel { AfterTranslationalName = string.Empty };

			// セットプロモーションマスタに登録されている翻訳前の情報を設定しておく(翻訳情報が取得できなかった場合に表示する)
			setPromotion.BeforeTranslationData = setPromotion.BeforeTranslationData
				?? new SetPromotionBeforeTranslationModel
				{
					SetPromotionDispName = setPromotion.SetpromotionDispName,
					Description = setPromotion.Description,
					DescriptionKbn = setPromotion.DescriptionKbn,
				};

			// 表示用セットプロモーション名
			if (string.IsNullOrEmpty(setPromotionDispNameTranslationSetting.AfterTranslationalName) == false)
			{
				setPromotion.SetpromotionDispName = setPromotionDispNameTranslationSetting.AfterTranslationalName;
				setPromotion.BeforeTranslationData.SetPromotionDispName = setPromotionDispNameTranslationSetting.SetPromotionDispName;
			}
			else
			{
				// 翻訳情報が存在しない場合は、マスタに登録されている情報を表示
				setPromotion.SetpromotionDispName = setPromotion.BeforeTranslationData.SetPromotionDispName;
			}

			// 表示用文言
			if (string.IsNullOrEmpty(descriptionTranslationSetting.AfterTranslationalName) == false)
			{
				setPromotion.Description = descriptionTranslationSetting.AfterTranslationalName;
				setPromotion.DescriptionKbn = descriptionTranslationSetting.DisplayKbn;

				setPromotion.BeforeTranslationData.Description = descriptionTranslationSetting.Description;
				setPromotion.BeforeTranslationData.DescriptionKbn = descriptionTranslationSetting.DescriptionKbn;
			}
			else
			{
				// 翻訳情報が存在しない場合は、マスタに登録されている情報を表示
				setPromotion.Description = setPromotion.BeforeTranslationData.Description;
				setPromotion.DescriptionKbn = setPromotion.BeforeTranslationData.DescriptionKbn;
			}
			return setPromotion;
		}
		#endregion

		#region +SetUserExtendSettingTranslationData ユーザ拡張項目設定翻訳情報設定
		/// <summary>
		/// ユーザ拡張項目設定翻訳情報設定
		/// </summary>
		/// <param name="userExtendSettingList">ユーザ拡張項目リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後ユーザ拡張項目設定リスト</returns>
		public static UserExtendSettingList SetUserExtendSettingTranslationData(UserExtendSettingList userExtendSettingList, string languageCode, string languageLocaleId)
		{
			// 翻訳情報を取得
			var translationSettings = GetUserExtendSettingTranslationSettings(userExtendSettingList, languageCode, languageLocaleId);

			var userExtendSettingListAfterTranslation = new UserExtendSettingList();
			userExtendSettingListAfterTranslation.Items.AddRange(userExtendSettingList.Items
				.Select(setting => SetUserExtendSettingTranslationData(setting, translationSettings)));

			return userExtendSettingListAfterTranslation;
		}
		/// <summary>
		/// ユーザ拡張項目設定翻訳情報設定
		/// </summary>
		/// <param name="userExtendSetting">ユーザ拡張項目設定</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳情報設定後ユーザ拡張項目設定</returns>
		/// <remarks>フロント表示時、ユーザ拡張項目設定情報はキャッシュから取得しているため、翻訳前の情報を事前に保持して表示切替する必要がある</remarks>
		public static UserExtendSettingModel SetUserExtendSettingTranslationData(UserExtendSettingModel userExtendSetting, NameTranslationSettingModel[] translationSettings)
		{
			// 翻訳情報取得(※翻訳情報の登録が無い場合は空のインスタンスをセット)
			var settingNameTranslationSetting =
				translationSettings.FirstOrDefault(d => ((d.MasterId1 == userExtendSetting.SettingId)
					&& (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_SETTING_NAME)))
				?? new NameTranslationSettingModel { AfterTranslationalName = string.Empty };

			var outlineTranslationSetting =
				translationSettings.FirstOrDefault(d => ((d.MasterId1 == userExtendSetting.SettingId)
					&& (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE)))
				?? new NameTranslationSettingModel { AfterTranslationalName = string.Empty };

			// ユーザ拡張項目設定マスタに登録されている翻訳前の情報を設定しておく(翻訳情報が取得できなかった場合に表示する)
			userExtendSetting.BeforeTranslationData = userExtendSetting.BeforeTranslationData ??
				new UserExtendSettingBeforeTranslationModel
				{
					SettingName = userExtendSetting.SettingName,
					OutlineKbn = userExtendSetting.OutlineKbn,
					Outline = userExtendSetting.Outline,
				};

			// 名称(ユーザー拡張項目)
			if (string.IsNullOrEmpty(settingNameTranslationSetting.AfterTranslationalName) == false)
			{
				userExtendSetting.SettingName = settingNameTranslationSetting.AfterTranslationalName;
				userExtendSetting.BeforeTranslationData.SettingName = settingNameTranslationSetting.SettingName;
			}
			else
			{
				// 翻訳情報が存在しない場合は、マスタに登録されている情報を表示
				userExtendSetting.SettingName = userExtendSetting.BeforeTranslationData.SettingName;
			}

			// ユーザ拡張項目概要
			if (string.IsNullOrEmpty(outlineTranslationSetting.AfterTranslationalName) == false)
			{
				userExtendSetting.Outline = outlineTranslationSetting.AfterTranslationalName;
				userExtendSetting.OutlineKbn = outlineTranslationSetting.DisplayKbn;

				userExtendSetting.BeforeTranslationData.Outline = outlineTranslationSetting.Outline;
				userExtendSetting.BeforeTranslationData.OutlineKbn = outlineTranslationSetting.OutlineKbn;
			}
			else
			{
				// 翻訳情報が存在しない場合は、マスタに登録されている情報を表示
				userExtendSetting.Outline = userExtendSetting.BeforeTranslationData.Outline;
				userExtendSetting.OutlineKbn = userExtendSetting.BeforeTranslationData.OutlineKbn;
			}

			return userExtendSetting;
		}
		#endregion

		#region +SetNewsTranslationData 新着情報翻訳情報設定
		/// <summary>
		/// 新着情報翻訳情報設定
		/// </summary>
		/// <param name="newsList">新着情報リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後新着情報</returns>
		public static NewsModel[] SetNewsTranslationData(NewsModel[] newsList, string languageCode, string languageLocaleId)
		{
			if ((newsList == null) || (newsList.Length == 0)) return null;

			// 翻訳情報取得
			var translationSettings = GetNewsTranslationSettings(newsList, languageCode, languageLocaleId);
			newsList = newsList.Select(news => SetNewsTranslationData(news, translationSettings)).ToArray();

			return newsList;
		}
		/// <summary>
		/// 新着情報翻訳情報設定
		/// </summary>
		/// <param name="news">新着情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後新着情報</returns>
		public static NewsModel SetNewsTranslationData(NewsModel news, NameTranslationSettingModel[] translationSettings)
		{
			// 翻訳情報取得(※翻訳情報の登録が無い場合は空のインスタンスをセット)
			var newsTextTranslationSetting = translationSettings.FirstOrDefault(d => (d.MasterId1 == news.NewsId))
				?? new NameTranslationSettingModel { AfterTranslationalName = string.Empty };

			// 新着情報マスタに登録されている翻訳前の情報を設定しておく(翻訳情報が取得できなかった場合に表示する)
			news.BeforeTranslationData = news.BeforeTranslationData
				?? new NewsBeforeTranslationModel
				{
					NewsText = news.NewsText,
					NewsTextKbn = news.NewsTextKbn,
				};

			// 本文
			if (string.IsNullOrEmpty(newsTextTranslationSetting.AfterTranslationalName) == false)
			{
				news.NewsText = newsTextTranslationSetting.AfterTranslationalName;
				news.NewsTextKbn = newsTextTranslationSetting.DisplayKbn;

				news.BeforeTranslationData.NewsText = newsTextTranslationSetting.NewsText;
				news.BeforeTranslationData.NewsTextKbn = newsTextTranslationSetting.NewsTextKbn;
			}
			else
			{
				// 翻訳情報が存在しない場合は、マスタに登録されている情報を表示
				news.NewsText = news.BeforeTranslationData.NewsText;
				news.NewsTextKbn = news.BeforeTranslationData.NewsTextKbn;
			}
			return news;
		}
		#endregion

		#region +SetPaymentTranslationData 決済種別翻訳情報設定
		/// <summary>
		/// 決済種別翻訳情報設定
		/// </summary>
		/// <param name="payments">決済種別情報リスト</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後決済種別情報リスト</returns>
		public static PaymentModel[] SetPaymentTranslationData(PaymentModel[] payments, string languageCode, string languageLocaleId)
		{
			var translationSettings = GetPaymentTranslationSettings(payments, languageCode, languageLocaleId);
			payments = payments.Select(payment => SetPaymentTranslationData(payment, translationSettings)).ToArray();

			return payments;
		}
		/// <summary>
		/// 決済種別翻訳情報設定
		/// </summary>
		/// <param name="payment">決済種別情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後決済種別</returns>
		public static PaymentModel SetPaymentTranslationData(PaymentModel payment, NameTranslationSettingModel[] translationSettings)
		{
			var paymentNameTranslationSetting = translationSettings.FirstOrDefault(
				setting => (setting.MasterId1 == payment.PaymentId));

			payment.PaymentName = (paymentNameTranslationSetting != null)
				? paymentNameTranslationSetting.AfterTranslationalName
				: payment.PaymentName;
		
			return payment;
		}
		#endregion

		#region -SetStockTranslationData 在庫文言翻訳情報設定
		/// <summary>
		/// 在庫文言翻訳情報設定
		/// </summary>
		/// <param name="productVariationStockInfos">商品バリエーション在庫情報</param>
		/// <param name="productId">商品Id</param>
		public static void SetStockTranslationData(ref ProductVariationStockInfo[] productVariationStockInfos, string productId)
		{
			if ((productVariationStockInfos == null) || (productVariationStockInfos.Any() == false)) return;

			// 商品バリエーション翻訳情報設定
			SetProductVariationTranslationData(ref productVariationStockInfos, productId);

			// そのあと在庫文言を設定
			SetProductStockMessageTranslationData(ref productVariationStockInfos);
		}
		#endregion

		#region -SetProductVariationTranslationData 商品バリエーション翻訳情報設定
		/// <summary>
		/// 商品バリエーション翻訳情報設定
		/// </summary>
		/// <param name="productVariationStockInfos">翻訳後商品バリエーション情報</param>
		/// <param name="productId">商品Id</param>
		private static void SetProductVariationTranslationData(ref ProductVariationStockInfo[] productVariationStockInfos, string productId)
		{
			// 翻訳設定情報取得
			var translationSettings = GetProductAndVariationTranslationSettingsByProductId(
				productId,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);

			// ProductVariationStockInfo[]に翻訳情報を設定する
			productVariationStockInfos = productVariationStockInfos
				.Select(info => SetProductVariationTranslationData(info, translationSettings)).ToArray();
		}
		/// <summary>
		/// 商品バリエーション翻訳情報設定
		/// </summary>
		/// <param name="product">商品バリエーション情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後商品バリエーション情報</returns>
		private static ProductVariationStockInfo SetProductVariationTranslationData(ProductVariationStockInfo product, NameTranslationSettingModel[] translationSettings)
		{
			if ((translationSettings == null) || (translationSettings.Any() == false)) return product;

			// 商品名翻訳情報設定
			var productNameTranslationSetting = translationSettings.FirstOrDefault(
				setting => ((setting.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME)
					&& (setting.MasterId1 == product.ProductId)));

			product.Name = (productNameTranslationSetting != null)
				? productNameTranslationSetting.AfterTranslationalName
				: product.Name;

			if (product.UseVariationFlg != Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE) return product;

			// バリエーション翻訳情報設定
			var variationTranslationSettings = translationSettings.Where(
				setting => ((setting.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION)
					&& (setting.MasterId1 == product.ProductId) && (setting.MasterId2 == product.VariationId))).ToList();

			if (variationTranslationSettings.Any() == false) return product;

			var variationName1TranslationSetting = variationTranslationSettings.FirstOrDefault(
				setting => (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME1));
			var variationName2TranslationSetting = variationTranslationSettings.FirstOrDefault(
				setting => (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME2));
			var variationName3TranslationSetting = variationTranslationSettings.FirstOrDefault(
				setting => (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME3));

			product.VariationName1 = (variationName1TranslationSetting != null) ? variationName1TranslationSetting.AfterTranslationalName : product.VariationName1;
			product.VariationName2 = (variationName2TranslationSetting != null) ? variationName2TranslationSetting.AfterTranslationalName : product.VariationName2;
			product.VariationName3 = (variationName3TranslationSetting != null) ? variationName3TranslationSetting.AfterTranslationalName : product.VariationName3;

			return product;
		}
		#endregion

		#region +SetCoordinateData コーディネート翻訳情報設定(子情報付)
		/// <summary>
		/// コーディネート翻訳情報設定(子情報付)
		/// </summary>
		/// <param name="searchCoordinates">コーディネート情報リスト</param>
		/// <returns>翻訳後コーディネートリスト</returns>
		public static CoordinateListSearchResult[] SetCoordinateTranslationDataWithChild(params CoordinateListSearchResult[] searchCoordinates)
		{
			var coordinates = searchCoordinates.Select(search => new CoordinateModel(search.DataSource)).ToArray();
			coordinates = coordinates.Select(SetCoordinateTranslationDataWithChild).ToArray();
			searchCoordinates = coordinates.Select(coordinate => new CoordinateListSearchResult(coordinate.DataSource)).ToArray();
			return searchCoordinates;
		}
		/// <summary>
		/// コーディネート翻訳情報設定(子情報付)
		/// </summary>
		/// <param name="coordinates">コーディネート情報リスト</param>
		/// <returns>翻訳後コーディネートリスト</returns>
		public static CoordinateModel[] SetCoordinateTranslationDataWithChild(params CoordinateModel[] coordinates)
		{
			coordinates = coordinates.Select(SetCoordinateTranslationDataWithChild).ToArray();
			return coordinates;
		}
		/// <summary>
		/// コーディネート翻訳情報設定(子情報付)
		/// </summary>
		/// <param name="coordinate">コーディネート情報</param>
		/// <returns>翻訳後コーディネート</returns>
		public static CoordinateModel SetCoordinateTranslationDataWithChild(CoordinateModel coordinate)
		{
			var languageCode = RegionManager.GetInstance().Region.LanguageCode;
			var languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
			coordinate = SetCoordinateTranslationData(
				GetCoordinateTranslationSettings(languageCode, languageLocaleId, coordinate),
				coordinate);
			coordinate = SetStaffTranslationData(
				GetStaffTranslationSettings(languageCode, languageLocaleId, new StaffModel{ StaffId = coordinate.StaffId}),
				coordinate);
			coordinate = SetRealShopTranslationData(
				GetRealShopTranslationSettings(languageCode, languageLocaleId, new RealShopModel{ RealShopId = coordinate.RealShopId}),
				coordinate);

			return coordinate;
		}
		/// <summary>
		/// コーディネート翻訳情報設定
		/// </summary>
		/// <param name="coordinate">コーディネート情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後コーディネート</returns>
		public static CoordinateModel SetCoordinateTranslationData(NameTranslationSettingModel[] translationSettings, CoordinateModel coordinate)
		{
			var coordinateTitleTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == coordinate.CoordinateId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATE_TITLE)));

			if (coordinateTitleTranslationSetting != null) coordinate.CoordinateTitle = coordinateTitleTranslationSetting.AfterTranslationalName;

			var coordinateSummaryTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == coordinate.CoordinateId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATE_SUMMARY)));

			if (coordinateSummaryTranslationSetting != null) coordinate.CoordinateSummary = coordinateSummaryTranslationSetting.AfterTranslationalName;
			return coordinate;
		}
		/// <summary>
		/// スタッフ翻訳情報設定（コーディネート内）
		/// </summary>
		/// <param name="coordinate">コーディネート情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後コーディネート</returns>
		public static CoordinateModel SetStaffTranslationData(NameTranslationSettingModel[] translationSettings, CoordinateModel coordinate)
		{
			var staffNameTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == coordinate.StaffId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_NAME)));

			if (staffNameTranslationSetting != null) coordinate.StaffName = staffNameTranslationSetting.AfterTranslationalName;

			var staffProfileTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == coordinate.StaffId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_PROFILE)));

			if (staffProfileTranslationSetting != null) coordinate.StaffProfile = staffProfileTranslationSetting.AfterTranslationalName;
			return coordinate;
		}
		/// <summary>
		/// リアル店舗翻訳情報設定（コーディネート内）
		/// </summary>
		/// <param name="coordinate">コーディネート情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後コーディネート</returns>
		public static CoordinateModel SetRealShopTranslationData(NameTranslationSettingModel[] translationSettings, CoordinateModel coordinate)
		{
			var nameTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == coordinate.RealShopId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_REALSHOP_NAME)));

			if (nameTranslationSetting != null) coordinate.RealShopName = nameTranslationSetting.AfterTranslationalName;

			return coordinate;
		}
		#endregion

		#region +SetCoordinateCategoryData コーディネートカテゴリ翻訳情報設定
		/// <summary>
		/// コーディネートカテゴリ翻訳情報設定
		/// </summary>
		/// <param name="coordinateCategories">コーディネートカテゴリ情報リスト</param>
		/// <returns>翻訳後コーディネートカテゴリリスト</returns>
		public static CoordinateCategoryModel[] SetCoordinateCategoryTranslationData(params CoordinateCategoryModel[] coordinateCategories)
		{
			var languageCode = RegionManager.GetInstance().Region.LanguageCode;
			var languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
			var translationSettings = GetCoordinateCategoryTranslationSettings(languageCode, languageLocaleId, coordinateCategories);
			coordinateCategories = coordinateCategories.Select(category => SetCoordinateCategoryTranslationData(translationSettings, category)).ToArray();

			return coordinateCategories;
		}
		/// <summary>
		/// コーディネートカテゴリ翻訳情報設定
		/// </summary>
		/// <param name="coordinateCategory">コーディネートカテゴリ情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後コーディネートカテゴリ</returns>
		public static CoordinateCategoryModel SetCoordinateCategoryTranslationData(NameTranslationSettingModel[] translationSettings, CoordinateCategoryModel coordinateCategory)
		{
			var coordinateTitleTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == coordinateCategory.CoordinateCategoryId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME)));

			if (coordinateTitleTranslationSetting != null) coordinateCategory.CoordinateCategoryName = coordinateTitleTranslationSetting.AfterTranslationalName;

			return coordinateCategory;
		}
		#endregion

		#region +SetStaffData スタッフ翻訳情報設定
		/// <summary>
		/// スタッフ翻訳情報設定
		/// </summary>
		/// <param name="searchStaffs">スタッフ情報リスト</param>
		/// <returns>翻訳後スタッフリスト</returns>
		public static StaffListSearchResult[] SetStaffTranslationData(params StaffListSearchResult[] searchStaffs)
		{
			var staffs = searchStaffs.Select(search => new StaffModel(search.DataSource)).ToArray();
			staffs = SetStaffTranslationData(staffs);
			searchStaffs = staffs.Select(staff => new StaffListSearchResult(staff.DataSource)).ToArray();
			return searchStaffs;
		}
		/// <summary>
		/// スタッフ翻訳情報設定
		/// </summary>
		/// <param name="staffs">スタッフ情報リスト</param>
		/// <returns>翻訳後スタッフリスト</returns>
		public static StaffModel[] SetStaffTranslationData(params StaffModel[] staffs)
		{
			var languageCode = RegionManager.GetInstance().Region.LanguageCode;
			var languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
			var translationSettings = GetStaffTranslationSettings(languageCode, languageLocaleId, staffs);
			staffs = staffs.Select(staff => SetStaffTranslationData(translationSettings, staff)).ToArray();

			return staffs;
		}
		/// <summary>
		/// スタッフ翻訳情報設定
		/// </summary>
		/// <param name="staff">スタッフ情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後スタッフ</returns>
		public static StaffModel SetStaffTranslationData(NameTranslationSettingModel[] translationSettings, StaffModel staff)
		{
			var staffNameTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == staff.StaffId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_NAME)));

			if (staffNameTranslationSetting != null) staff.StaffName = staffNameTranslationSetting.AfterTranslationalName;

			var staffProfileTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == staff.StaffId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_PROFILE)));

			if (staffProfileTranslationSetting != null) staff.StaffProfile = staffProfileTranslationSetting.AfterTranslationalName;

			return staff;
		}
		#endregion

		#region +SetRealShopTranslationData リアル店舗翻訳情報設定
		/// <summary>
		/// リアル店舗翻訳情報設定
		/// </summary>
		/// <param name="shops">リアル店舗リスト</param>
		/// <returns>翻訳後リアル店舗</returns>
		public static RealShopModel[] SetRealShopTranslationData(params RealShopModel[] shops)
		{
			var languageCode = RegionManager.GetInstance().Region.LanguageCode;
			var languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
			var translationSettings = GetRealShopTranslationSettings(languageCode, languageLocaleId, shops);
			shops = shops.Select(staff => SetRealShopTranslationData(translationSettings, staff)).ToArray();

			return shops;
		}
		/// <summary>
		/// リアル店舗情報設定
		/// </summary>
		/// <param name="shop">リアル店舗情報</param>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <returns>翻訳後リアル店舗</returns>
		public static RealShopModel SetRealShopTranslationData(NameTranslationSettingModel[] translationSettings, RealShopModel shop)
		{
			var nameTranslationSetting = translationSettings.FirstOrDefault((
				setting => (setting.MasterId1 == shop.RealShopId)
					&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_REALSHOP_NAME)));

			if (nameTranslationSetting != null) shop.Name = nameTranslationSetting.AfterTranslationalName;
			return shop;
		}
		#endregion
	}
}
