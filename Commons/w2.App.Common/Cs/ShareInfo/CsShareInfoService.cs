/*
=========================================================================================================
  Module      : 共有情報サービス(CsShareInfoService.cs)
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

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報サービス
	/// </summary>
	public class CsShareInfoService
	{
		/// <summary>レポジトリ</summary>
		private CsShareInfoRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsShareInfoService(CsShareInfoRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <returns>モデル</returns>
		public CsShareInfoModel Get(string deptId, long infoNo)
		{
			var ht = new Hashtable();
			ht.Add(Constants.FIELD_CSSHAREINFO_DEPT_ID, deptId);
			ht.Add(Constants.FIELD_CSSHAREINFO_INFO_NO, infoNo);

			var dv = this.Repository.Get(ht);
			return (dv.Count == 0) ? null : new CsShareInfoModel(dv[0]);
		}
		#endregion

		#region +Search 全件検索
		/// <summary>
		/// 全件検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <param name="infoText">共有情報テキスト</param>
		/// <param name="infoKbn">共有情報区分</param>
		/// <param name="infoImportance">共有情報重要度</param>
		/// <param name="operatorId">送信元オペレータID</param>
		/// <param name="sortKbn">ソート区分</param>
		/// <param name="pageNo">表示開始ページ番号</param>
		/// <returns>共有情報リスト</returns>
		public CsShareInfoModel[] Search(string deptId, long? infoNo, string infoText, string infoKbn, string infoImportance, string operatorId, string sortKbn, int pageNo)
		{
			DataView dv = this.Repository.Search(deptId, infoNo, infoText, infoKbn, infoImportance, operatorId, sortKbn, pageNo);
			return (from DataRowView drv in dv select new CsShareInfoModel(drv)).ToArray();
		}
		#endregion

		#region -Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録した共有情報NO</returns>
		public long Register(CsShareInfoModel model)
		{
			// Create new share info NO
			model.InfoNo = NumberingUtility.CreateNewNumber(model.DeptId, Constants.NUMBER_KEY_CS_SHARE_INFO_NO);

			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 登録
				this.Repository.Register(model.DataSource, accessor);

				accessor.CommitTransaction();

				return model.InfoNo;
			}
		}
		#endregion

		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void Update(CsShareInfoModel model)
		{
			this.Repository.Update(model.DataSource);
		}
		#endregion

		#region +DeleteWithReads 既読情報含めて削除
		/// <summary>
		/// 既読情報含めて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <returns>モデル</returns>
		public void DeleteWithReads(string deptId, long infoNo)
		{
			// 削除
			Delete(deptId, infoNo);

			// 既読情報削除
			var readService = new CsShareInfoReadService(new CsShareInfoReadRepository());
			readService.DeleteAll(deptId, infoNo);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <returns>モデル</returns>
		public void Delete(string deptId, long infoNo)
		{
			var ht = new Hashtable();
			ht.Add(Constants.FIELD_CSSHAREINFO_DEPT_ID, deptId);
			ht.Add(Constants.FIELD_CSSHAREINFO_INFO_NO, infoNo);

			this.Repository.Delete(ht);
		}
		#endregion
	}
}
