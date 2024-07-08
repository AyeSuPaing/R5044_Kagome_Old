/*
=========================================================================================================
  Module      : 回答例カテゴリサービス(CsAnswerTemplateCategoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.AnswerTemplate
{
	/// <summary>
	///回答例カテゴリサービス
	/// </summary>
	public class CsAnswerTemplateCategoryService
	{
		/// <summary>レポジトリ</summary>
		private CsAnswerTemplateCategoryRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsAnswerTemplateCategoryService(CsAnswerTemplateCategoryRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +SearchByCategoryId カテゴリID指定サーチ（指定がなければ全件）
		/// <summary>
		/// カテゴリID指定サーチ（指定がなければ全件）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">回答カテゴリID</param>
		/// <returns>回答例カテゴリリスト</returns>
		public CsAnswerTemplateCategoryModel[] SearchByCategoryId(string deptId, string categoryId)
		{
			var dv = this.Repository.SearchByCategoryId(deptId, categoryId);
			return (from DataRowView drv in dv select new CsAnswerTemplateCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件サーチ
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>回答例カテゴリリスト</returns>
		public CsAnswerTemplateCategoryModel[] GetAll(string deptId)
		{
			var dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsAnswerTemplateCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +GetValidAll 有効なもの全件取得
		/// <summary>
		/// 全件サーチ
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>回答例カテゴリリスト</returns>
		public CsAnswerTemplateCategoryModel[] GetValidAll(string deptId)
		{
			var models = GetAll(deptId);
			return models.Where(model => model.EX_RankedValidFlg == Constants.FLG_CSANSWERTEMPLATECATEGORY_VALID_FLG_VALID).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>回答例カテゴリ</returns>
		public CsAnswerTemplateCategoryModel Get(string deptId, string categoryId)
		{
			DataView dv = this.Repository.Get(deptId, categoryId);
			return (dv.Count == 0) ? null : new CsAnswerTemplateCategoryModel(dv[0]);
		}
		#endregion

		#region +HasChildCategories 子カテゴリ存在判定（削除チェック用）
		/// <summary>
		/// 子カテゴリ存在判定（削除チェック用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>子カテゴリが存在するかどうか</returns>
		public bool HasChildCategories(string deptId, string categoryId)
		{
			var dv = this.Repository.GetChildCategories(deptId, categoryId);
			return (dv.Count != 0);
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">登録用データ</param>
		/// <returns>登録したカテゴリID</returns>
		public string Register(CsAnswerTemplateCategoryModel model)
		{
			// Create new answer template category ID
			model.CategoryId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_ANSWER_CATEGORY_ID, 3);

			// トランザクション開始
			using(SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 登録
				this.Repository.Register(model.DataSource, accessor);

				accessor.CommitTransaction();

				return model.CategoryId;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">更新用データ</param>
		public void Update(CsAnswerTemplateCategoryModel input)
		{
			this.Repository.Update(input.DataSource);
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
