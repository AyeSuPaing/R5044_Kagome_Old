/*
=========================================================================================================
  Module      : メール振分設定アイテムサービス(CsMailAssignSettingItemService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.App.Common.MailAssignSetting
{
	/// <summary>
	/// メール振分設定アイテムリポジトリ
	/// </summary>
	public class CsMailAssignSettingItemService
	{
		/// <summary>レポジトリ</summary>
		private CsMailAssignSettingItemRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsMailAssignSettingItemService(CsMailAssignSettingItemRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振り分け設定ID</param>
		/// <returns>モデル配列</returns>
		public CsMailAssignSettingItemModel[] GetAll(string deptId, string mailAssignId)
		{
			DataView dv = this.Repository.GetAll(deptId, mailAssignId);
			return (from DataRowView drv in dv select new CsMailAssignSettingItemModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">登録用モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Register(CsMailAssignSettingItemModel model, SqlAccessor accessor)
		{
			model.ItemNo = this.Repository.GetRegistItemNo(model.DeptId, model.MailAssignId, accessor);
			this.Repository.Register(model.DataSource, accessor);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">更新用モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(CsMailAssignSettingItemModel model, SqlAccessor accessor)
		{
			int count = this.Repository.Update(model.DataSource, accessor);
			if (count == 0) this.Repository.Register(model.DataSource, accessor);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string mailAssignId, SqlAccessor accessor)
		{
			this.Repository.Delete(deptId, mailAssignId, accessor);
		}
		#endregion

		#region +DeleteAfter 以降削除
		/// <summary>
		/// 以降削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振り分け設定ID</param>
		/// <param name="mailAssignItemNo">アイテム番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAfter(string deptId, string mailAssignId, int mailAssignItemNo, SqlAccessor accessor)
		{
			this.Repository.DeleteAfter(deptId, mailAssignId, mailAssignItemNo, accessor);
		}
		#endregion
	}
}
