/*
=========================================================================================================
  Module      : 商品一覧表示設定サービス (ProductListDispSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;

namespace w2.Domain.ProductListDispSetting
{
	/// <summary>
	/// 商品一覧表示設定サービス
	/// </summary>
	public class ProductListDispSettingService : ServiceBase, IProductListDispSettingService
	{
		#region +GetUsable 利用可能なもの取得
		/// <summary>
		/// 利用可能なもの取得
		/// </summary>
		/// <returns>モデル列</returns>
		public ProductListDispSettingModel[] GetUsable()
		{
			var models = GetAll();
			return models.Where(m => m.DispEnable == Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON
				|| m.IsDefaultDispFlgOn)
				.OrderBy(m => m.DispNo)
				.ThenBy(m => m.SettingId).ToArray();
		}
		#endregion
		#region +GetUsable 利用可能なもの取得（設定区分指定）
		/// <summary>
		/// 利用可能なもの取得（設定区分指定）
		/// </summary>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>モデル配列</returns>
		public ProductListDispSettingModel[] GetUsable(string settingKbn)
		{
			var models = GetUsable().Where(data => data.SettingKbn == settingKbn).ToArray();
			return models;
		}
		#endregion

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <returns>モデル列</returns>
		public ProductListDispSettingModel[] GetAll()
		{
			using (var repository = new ProductListDispSettingRepository())
			{
				var models = repository.GetAll();
				return models;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(ProductListDispSettingModel model)
		{
			using (var repository = new ProductListDispSettingRepository())
			{
				if (model.IsCountSetting) return 0;

				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +InsertCountSetting 件数登録
		/// <summary>
		/// 件数登録
		/// </summary>
		/// <param name="models">モデル配列</param>
		public int InsertCountSetting(ProductListDispSettingModel[] models)
		{
			using (var repository = new ProductListDispSettingRepository())
			{
				repository.DeleteBySettingKbn(Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_COUNT);
				var result = models.Select(model => repository.Insert(model)).Sum();
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>影響を受けた数</returns>
		public int Delete(string settingId, string settingKbn)
		{
			using (var repository = new ProductListDispSettingRepository())
			{
				var result = repository.Delete(settingId, settingKbn);
				return result;
			}
		}
		#endregion

		#region +DeleteBySettingKbn 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>影響を受けた数</returns>
		public int DeleteBySettingKbn(string settingKbn)
		{
			using (var repository = new ProductListDispSettingRepository())
			{
				var result = repository.DeleteBySettingKbn(settingKbn);
				return result;
			}
		}
		#endregion
	}
}
