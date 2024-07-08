/*
=========================================================================================================
  Module      : インシデントカテゴリマスタサービス(CsIncidentCategoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.IncidentCategory
{
	/// <summary>
	/// インシデントカテゴリマスタサービス
	/// </summary>
	public class CsIncidentCategoryService
	{
		/// <summary>レポジトリ</summary>
		private CsIncidentCategoryRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsIncidentCategoryService(CsIncidentCategoryRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +SearchByCategoryId カテゴリID指定サーチ（指定がなければ全件）
		/// <summary>
		/// カテゴリID指定サーチ（指定がなければ全件）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>インシデントカテゴリリスト</returns>
		public CsIncidentCategoryModel[] SearchByCategoryId(string deptId, string categoryId)
		{
			var dv = this.Repository.SearchByCategoryId(deptId, categoryId);
			return (from DataRowView drv in dv select new CsIncidentCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +GetValidAll 有効なもの取得（オペレータ画面のドロップダウンリスト用）
		/// <summary>
		/// 有効なもの取得（オペレータ画面のドロップダウンリスト用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>インシデントカテゴリリスト</returns>
		public CsIncidentCategoryModel[] GetValidAll(string deptId)
		{
			var dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv where ((new CsIncidentCategoryModel(drv)).EX_RankedValidFlg == Constants.FLG_CSINCIDENTCATEGORY_VALID_FLG_VALID) select new CsIncidentCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +GetAll 全件取得（インシデントカテゴリ設定画面のドロップダウンリスト用）
		/// <summary>
		/// 全件取得（システム設定系画面のドロップダウンリスト用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>インシデントカテゴリリスト</returns>
		public CsIncidentCategoryModel[] GetAll(string deptId)
		{
			var dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsIncidentCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>インシデントカテゴリ</returns>
		public CsIncidentCategoryModel Get(string deptId, string categoryId)
		{
			var dv = this.Repository.Get(deptId, categoryId);
			return (dv.Count == 0) ? null : new CsIncidentCategoryModel(dv[0]);
		}
		#endregion

		#region +GetCategoryTrailList 親カテゴリリスト取得（ルートから指定カテゴリまで）
		/// <summary>
		/// カテゴリ階層リスト（ルートカテゴリまでのリスト）取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>インシデントカテゴリリスト</returns>
		public CsIncidentCategoryModel[] GetCategoryTrailList(string deptId, string categoryId)
		{
			var dv = this.Repository.GetCategoryTrailList(deptId, categoryId);
			return (from DataRowView drv in dv select new CsIncidentCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">登録用データ</param>
		public void Register(CsIncidentCategoryModel input)
		{
			// Create new incident category ID
			input.CategoryId = NumberingUtility.CreateKeyId(input.DeptId, Constants.NUMBER_KEY_CS_INCIDENT_CATEGORY_ID, 3);

			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 登録
				this.Repository.Register(input.DataSource, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">更新用データ</param>
		public void Update(CsIncidentCategoryModel input)
		{
			this.Repository.Update(input.DataSource);
		}
		#endregion

		#region +HasIncident 利用中インシデントがあるかどうか判定
		/// <summary>
		/// 利用中インシデントがあるかどうか判定
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>利用インシデントの有無</returns>
		public bool HasIncident(string deptId, string categoryId)
		{
			return (this.Repository.GetIncidents(deptId, categoryId).Count != 0);
		}
		#endregion

		#region +HasChildCategory 子カテゴリが存在するかどうか判定
		/// <summary>
		/// 子カテゴリが存在するかどうか判定
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>子カテゴリの有無</returns>
		public bool HasChildCategory(string deptId, string categoryId)
		{
			return (this.Repository.GetChildCategories(deptId, categoryId).Count != 0);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		public void Delete(string deptId, string categoryId)
		{
			this.Repository.Delete(deptId, categoryId);
		}
		#endregion
	}
}
