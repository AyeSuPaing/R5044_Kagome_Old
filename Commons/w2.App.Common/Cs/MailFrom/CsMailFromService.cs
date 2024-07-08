/*
=========================================================================================================
  Module      : メール送信元サービス(CsMailFromService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.MailFrom
{
	public class CsMailFromService
	{
		/// <summary>レポジトリ</summary>
		private CsMailFromRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsMailFromService(CsMailFromRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Search 一覧取得
		/// <summary>
		/// 一覧取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beginRow">開始行</param>
		/// <param name="endRow">終了行</param>
		/// <returns>メール送信元モデルリスト</returns>
		public CsMailFromModel[] Search(string deptId, int beginRow, int endRow)
		{
			DataView dv = this.Repository.Search(deptId, beginRow, endRow);
			return (from DataRowView drv in dv select new CsMailFromModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fromId">メール送信元ID</param>
		/// <returns>メール送信元モデル</returns>
		public CsMailFromModel Get(string deptId, string fromId)
		{
			DataView dv = this.Repository.Get(deptId, fromId);
			return (dv.Count == 0) ? null : new CsMailFromModel(dv[0]);
		}
		#endregion

		#region +GetValidAll 有効なものを取得
		/// <summary>
		/// 有効なものを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>メール送信元モデルリスト</returns>
		public CsMailFromModel[] GetValidAll(string deptId)
		{
			DataView dv = this.Repository.GetValidAll(deptId);
			return (from DataRowView drv in dv select new CsMailFromModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">メール送信元データ</param>
		public void Register(CsMailFromModel model)
		{
			// Create new mail from ID
			model.MailFromId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_MAIL_FROM_ID, 3);

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
		/// <param name="model">メール送信元データ</param>
		public void Update(CsMailFromModel model)
		{
			this.Repository.Update(model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fromId">メール送信元ID</param>
		public void Delete(string deptId, string fromId)
		{
			this.Repository.Delete(deptId, fromId);
		}
		#endregion
	}
}
