/*
=========================================================================================================
  Module      : メッセージメールデータサービス(CsMessageMailDataService.cs)
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

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージメールデータサービス
	/// </summary>
	public class CsMessageMailDataService
	{
		/// <summary>レポジトリ</summary>
		private CsMessageMailDataRepository Repository;

		#region +コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsMessageMailDataService(CsMessageMailDataRepository repository)
		{
			this.Repository = repository;
		}

		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Register(CsMessageMailDataModel model, SqlAccessor accessor)
		{
			this.Repository.Register(model.DataSource, accessor);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(CsMessageMailDataModel model, SqlAccessor accessor)
		{
			this.Repository.Update(model.DataSource, accessor);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string mailId, SqlAccessor accessor)
		{
			this.Repository.Delete(deptId, mailId, accessor);
		}
		#endregion
	}
}
