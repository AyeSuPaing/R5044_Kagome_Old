/*
=========================================================================================================
  Module      : 集計区分サービス(CsSummarySettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.SummarySetting
{
	/// <summary>
	/// 集計区分サービス
	/// </summary>
	public class CsSummarySettingService
	{
		/// <summary>レポジトリ</summary>
		private CsSummarySettingRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsSummarySettingService(CsSummarySettingRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分のリスト</returns>
		public CsSummarySettingModel[] Search(string deptId)
		{
			var dv = this.Repository.Search(deptId);
			return (from DataRowView drv in dv select new CsSummarySettingModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summarySettingNo">集計区分番号</param>
		/// <returns>集計区分データ</returns>
		public CsSummarySettingModel Get(string deptId, int summarySettingNo)
		{
			var dv = this.Repository.Get(deptId, summarySettingNo);
			return (dv.Count == 0) ? null : new CsSummarySettingModel(dv[0]);
		}
		#endregion

		#region +GetWithItems 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summarySettingNo">集計区分番号</param>
		/// <returns>集計区分のリスト</returns>
		public CsSummarySettingModel GetWithItems(string deptId, int summarySettingNo)
		{
			var model = Get(deptId, summarySettingNo);
			if (model == null) return null;

			var itemService = new CsSummarySettingItemService(new CsSummarySettingItemRepository());
			var items = itemService.GetAllBySummarySettingNo(deptId, summarySettingNo);
			model.EX_Items = (from item in items select item).ToArray();

			return model;
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分のリスト</returns>
		public CsSummarySettingModel[] GetAll(string deptId)
		{
			var dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsSummarySettingModel(drv)).ToArray();
		}
		#endregion

		#region +GetValidAll 有効なもの全て取得
		/// <summary>
		/// 有効なもの全て取得
		/// </summary>
		/// <param name="deptId"></param>
		/// <returns>集計区分のリスト</returns>
		public CsSummarySettingModel[] GetValidAll(string deptId)
		{
			var models = GetAll(deptId);
			return (from m in models where m.ValidFlg == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID select m).ToArray();
		}
		#endregion

		#region +GetAllWithItems 全て取得（アイテムも）
		/// <summary>
		/// 全て取得（アイテムも）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分のリスト</returns>
		public CsSummarySettingModel[] GetAllWithItems(string deptId)
		{
			var itemService = new CsSummarySettingItemService(new CsSummarySettingItemRepository());

			var settings = this.GetAll(deptId);
			var items = itemService.GetAll(deptId);

			foreach (var s in settings)
			{
				s.EX_Items = (from item in items
									where ((s.DeptId == item.DeptId) && (s.SummarySettingNo == item.SummarySettingNo))
									select item).ToArray();
			}
			return settings;
		}
		#endregion

		#region +GetValidAllWithValidItems 有効なもの全て取得（アイテムも）
		/// <summary>
		/// 有効なもの全て取得（アイテムも）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分のリスト</returns>
		public CsSummarySettingModel[] GetValidAllWithValidItems(string deptId)
		{
			var allSettings = GetAllWithItems(deptId);
			var validSettings = (from s in allSettings where s.ValidFlg == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID select s).ToArray();
			foreach (var s in validSettings)
			{
				s.EX_Items = (from i in s.EX_Items where i.ValidFlg == Constants.FLG_CSSUMMARYSETTINGITEM_VALID_FLG_VALID select i).ToArray();
			}
			return validSettings;
		}
		#endregion

		#region -Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録した集計番号</returns>
		private int Register(CsSummarySettingModel model, SqlAccessor accessor)
		{
			// 登録
			model.SummarySettingNo = (int)NumberingUtility.CreateNewNumber(model.DeptId, Constants.NUMBER_KEY_CS_SUMMARY_SETTING_NO);
			this.Repository.Register(model.DataSource, accessor);

			return model.SummarySettingNo;
		}
		#endregion

		#region +RegisterWithItems アイテムも全て登録
		/// <summary>
		/// アイテムも全て登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void RegisterWithItems(CsSummarySettingModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				int settingNo = Register(model, accessor);

				var itemRepository = new CsSummarySettingItemRepository();
				foreach (var item in model.EX_Items)
				{
					item.SummarySettingNo = settingNo;
					itemRepository.Register(item.DataSource, accessor);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void Update(CsSummarySettingModel model, SqlAccessor accessor)
		{
			this.Repository.Update(model.DataSource, accessor);
		}
		#endregion

		#region +UpdateWithItems アイテムも全て更新
		/// <summary>
		/// アイテムも全て更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateWithItems(CsSummarySettingModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				Update(model, accessor);

				var itemRepository = new CsSummarySettingItemRepository();
				itemRepository.DeleteAll(model.DataSource, accessor);

				foreach (var item in model.EX_Items)
				{
					itemRepository.Register(item.DataSource, accessor);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion
	}
}
