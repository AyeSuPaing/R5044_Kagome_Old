/*
=========================================================================================================
  Module      : 商品一覧表示設定ユーティリティ(ProductListDispSettingUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.DataCacheController;
using w2.Domain.ProductListDispSetting;

namespace w2.App.Common.ProductDefaultSetting
{
	/// <summary>
	/// 商品一覧表示設定ユーティリティ
	/// </summary>
	public class ProductListDispSettingUtility
	{
		#region メソッド
		/// <summary>
		/// 在庫設定を取得（翻訳後用）
		/// </summary>
		/// <param name="models">商品一覧表示設定</param>
		/// <returns>在庫設定の商品一覧表示設定</returns>
		public static ProductListDispSettingModel[] GetStockSetting(ProductListDispSettingModel[] models)
		{
			var stockSetting = models
				.Where(setting => setting.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK
					&& setting.DispEnable == Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON)
				.ToArray();
			return stockSetting;
		}

		/// <summary>
		/// 表示件数を取得（翻訳後用）
		/// </summary>
		/// <param name="models">商品一覧表示設定</param>
		/// <returns>表示件数の商品一覧表示設定</returns>
		public static int[] GetCountSetting(ProductListDispSettingModel[] models)
		{
			var counts = models
				.Where(setting => setting.IsCountSetting
					&& setting.DispEnable == Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON)
				.Select(setting => setting.DispCount ?? 0)
				.ToArray();
			return (counts.Length == 0) ? Constants.NUMBER_DISPLAY_LINKS_PRODUCT_LIST.ToArray() : counts;
		}

		/// <summary>
		/// 表示形式を取得（翻訳後用）
		/// </summary>
		/// <param name="models">商品一覧表示設定</param>
		/// <returns>表示形式の商品一覧表示設定</returns>
		public static ProductListDispSettingModel[] GetImgSetting(ProductListDispSettingModel[] models)
		{
			var imgs = models
				.Where(setting => setting.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG
					&& setting.DispEnable == Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON)
				.ToArray();
			return imgs;
		}

		/// <summary>
		/// ソート形式を取得（翻訳後用）
		/// </summary>
		/// <param name="models">商品一覧表示設定</param>
		/// <returns>ソート形式の商品一覧表示設定</returns>
		public static ProductListDispSettingModel[] GetSortSetting(ProductListDispSettingModel[] models)
		{
			var sorts = models
				.Where(setting => setting.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT
					&& setting.DispEnable == Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON)
				.ToArray();
			return sorts;
		}
		#endregion

		#region プロパティ
		/// <summary>ソート形式設定（DB & Config）</summary>
		public static string SortDefault
		{
			get
			{
				return (SortSetting.Length == 0)
					? Constants.KBN_SORT_PRODUCT_LIST_DEFAULT
					: SortSetting.First(setting => setting.IsDefaultDispFlgOn).SettingId;
			}
		}
		/// <summary>在庫表示設定（DB & Config）</summary>
		public static string UndisplayNostockProductDefault
		{
			get
			{
				return (StockSetting.Length == 0)
					? Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DEFAULT
					: StockSetting.First(setting => setting.IsDefaultDispFlgOn).SettingId;
			}
		}
		/// <summary>件数設定（DB & Config）</summary>
		public static int CountDispContentsImgOn
		{
			get
			{
				if (CountSetting.Length == 0) return Constants.CONST_DISP_CONTENTS_PRODUCT_LIST_IMG_ON;
				return CountSetting.First(setting => setting.IsDefaultDispFlgOn).DispCount
					?? Constants.CONST_DISP_CONTENTS_PRODUCT_LIST_IMG_ON;
			}
		}
		/// <summary>件数設定（DB & Config）</summary>
		public static int CountDispContentsWindowShopping
		{
			get
			{
				if (CountSetting.Length == 0) return Constants.CONST_DISP_CONTENTS_PRODUCT_LIST_WINDOWSHOPPING;
				return CountSetting.First(setting => setting.IsDefaultDispFlgOn).DispCount
					?? Constants.CONST_DISP_CONTENTS_PRODUCT_LIST_WINDOWSHOPPING;
			}
		}
		/// <summary>形式初期設定（DB & Config）</summary>
		public static string DispImgKbnDefault
		{
			get
			{
				return (ImgSetting.Length == 0)
					? Constants.KBN_REQUEST_DISP_IMG_KBN_DEFAULT
					: ImgSetting.First(setting => setting.IsDefaultDispFlgOn).SettingId;
			}
		}
		/// <summary>在庫表示設定（DB）</summary>
		public static ProductListDispSettingModel[] StockSetting
		{
			get
			{
				return DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData
					.Where(setting => setting.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK)
					.ToArray();
			}
		}
		/// <summary>件数設定（DB）</summary>
		public static ProductListDispSettingModel[] CountSetting
		{
			get
			{
				return DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData
					.Where(setting => setting.IsCountSetting)
					.ToArray();
			}
		}
		/// <summary>表示形式設定（DB）</summary>
		public static ProductListDispSettingModel[] ImgSetting
		{
			get
			{
				return DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData
					.Where(setting => setting.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG)
					.ToArray();
			}
		}
		/// <summary>表示形式設定（DB）</summary>
		public static ProductListDispSettingModel[] SortSetting
		{
			get
			{
				return DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData
					.Where(setting => setting.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT)
					.ToArray();
			}
		}
		#endregion
	}
}
