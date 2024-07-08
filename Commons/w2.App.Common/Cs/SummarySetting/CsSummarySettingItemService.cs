/*
=========================================================================================================
  Module      : 集計区分アイテムサービス(CsSummarySettingItemService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.App.Common.Cs.SummarySetting
{
	/// <summary>
	/// 集計区分アイテムサービス
	/// </summary>
	public class CsSummarySettingItemService
	{
		/// <summary>レポジトリ</summary>
		private CsSummarySettingItemRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsSummarySettingItemService(CsSummarySettingItemRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +GetAllBySummarySettingNo 集計区分内アイテム全て取得
		/// <summary>
		/// 集計区分内アイテム全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summarySettingNo">集計区分番号</param>
		/// <returns>集計区分アイテムのリスト</returns>
		public CsSummarySettingItemModel[] GetAllBySummarySettingNo(string deptId, int summarySettingNo)
		{
			var dv = this.Repository.GetAllBySummarySettingNo(deptId, summarySettingNo);
			return (from DataRowView drv in dv select new CsSummarySettingItemModel(drv)).ToArray();
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分アイテムのリスト</returns>
		public CsSummarySettingItemModel[] GetAll(string deptId)
		{
			var dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsSummarySettingItemModel(drv)).ToArray();
		}
		#endregion

		#region +GetValidAll 有効なもの全て取得
		/// <summary>
		/// 有効なもの全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分アイテムのリスト</returns>
		public CsSummarySettingItemModel[] GetValidAll(string deptId)
		{
			var models = GetAll(deptId);
			return (from m in models where m.ValidFlg == Constants.FLG_CSSUMMARYSETTINGITEM_VALID_FLG_VALID select m).ToArray();
		}
		#endregion
	}
}
