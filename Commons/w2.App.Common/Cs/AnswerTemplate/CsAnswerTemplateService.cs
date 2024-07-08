/*
=========================================================================================================
  Module      : 回答例サービス(CsAnswerTemplateService.cs)
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

namespace w2.App.Common.Cs.AnswerTemplate
{
	/// <summary>
	/// 回答例リポジトリ
	/// </summary>
	public class CsAnswerTemplateService
	{
		/// <summary>レポジトリ</summary>
		private CsAnswerTemplateRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsAnswerTemplateService(CsAnswerTemplateRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="answerTitle">回答例タイトル</param>
		/// <param name="answerMailTitle">回答例件名</param>
		/// <param name="answerText">回答例テキスト</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="beginRow">開始NO</param>
		/// <param name="endRow">終了NO</param>
		/// <returns>回答例モデル配列</returns>
		public CsAnswerTemplateModel[] Search(
			string deptId,
			string categoryId,
			string answerTitle,
			string answerMailTitle,
			string answerText,
			string validFlg,
			int beginRow,
			int endRow)
		{
			var dv = this.Repository.Search(
				deptId,
				categoryId,
				answerTitle,
				answerMailTitle,
				answerText,
				validFlg,
				beginRow,
				endRow);
			var result = dv.Cast<DataRowView>()
				.Select(drv => new CsAnswerTemplateModel(drv))
				.ToArray();
			return result;
		}
		#endregion

		#region +SearchValid 有効なものから検索
		/// <summary>
		/// 有効なものから検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="keyword">キーワード</param>
		/// <returns>回答例モデル配列</returns>
		public CsAnswerTemplateModel[] SearchValid(string deptId, string categoryId, string keyword)
		{
			var dv = this.Repository.SearchValid(deptId, categoryId, keyword);
			return (from DataRowView drv in dv select new CsAnswerTemplateModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="answerId">回答例ID</param>
		/// <returns>回答例モデル</returns>
		public CsAnswerTemplateModel Get(string deptId, string answerId)
		{
			DataView dv = this.Repository.Get(deptId, answerId);
			return (dv.Count == 0) ? null : new CsAnswerTemplateModel(dv[0]);
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">登録用データ</param>
		public void Register(CsAnswerTemplateModel model)
		{
			// Create new answer template ID
			model.AnswerId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_ANSWER_ID, 3);

			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				this.Repository.Register(model.DataSource, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">更新用データ</param>
		public void Update(CsAnswerTemplateModel model)
		{
			this.Repository.Update(model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="model">削除用データ</param>
		public void Delete(CsAnswerTemplateModel model)
		{
			this.Repository.Delete(model.DeptId, model.AnswerId);
		}
		#endregion
	}
}
