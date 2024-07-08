/*
=========================================================================================================
  Module      : インシデントVOCサービス(CsIncidentVocService.cs)
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

namespace w2.App.Common.Cs.IncidentVoc
{
	/// <summary>
	/// インシデントVOCサービス
	/// </summary>
	public class CsIncidentVocService
	{
		/// <summary>レポジトリ</summary>
		private CsIncidentVocRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsIncidentVocService(CsIncidentVocRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Search 一覧取得
		/// <summary>
		/// 一覧取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="bgnRow">開始行番号</param>
		/// <param name="endRow">終了行番号</param>
		/// <returns>VOCモデル配列</returns>
		public CsIncidentVocModel[] Search(string deptId, int bgnRow, int endRow)
		{
			DataView dv = this.Repository.Search(deptId, bgnRow, endRow);
			return (from DataRowView drv in dv select new CsIncidentVocModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="vocId">VOCID</param>
		/// <returns>VOCモデル</returns>
		public CsIncidentVocModel Get(string deptId, string vocId)
		{
			DataView dv = this.Repository.Get(deptId, vocId);
			return (dv.Count == 0) ? null : new CsIncidentVocModel(dv[0]);
		}
		#endregion

		#region +GetValidAll 有効なものすべて取得
		/// <summary>
		/// 有効なものすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>VOCモデル配列</returns>
		public CsIncidentVocModel[] GetValidAll(string deptId)
		{
			DataView dv = this.Repository.GetValidAll(deptId);
			return (from DataRowView model in dv select new CsIncidentVocModel(model)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">登録用データ</param>
		public void Register(CsIncidentVocModel input)
		{
			// Create new incident Voc ID
			input.VocId = NumberingUtility.CreateKeyId(input.DeptId, Constants.NUMBER_KEY_CS_INCIDENT_VOC_ID, 3);

			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				this.Repository.Register(input.DataSource, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">更新用データ</param>
		public void Update(CsIncidentVocModel input)
		{
			this.Repository.Update(input.DataSource);
		}
		#endregion

		#region +CheckDeletable 削除チェック
		/// <summary>
		/// 削除チェック
		/// </summary>
		/// <param name="input">削除データ</param>
		/// <returns>削除可能かどうか</returns>
		public bool CheckDeletable(CsIncidentVocModel input)
		{
			return this.Repository.CheckDeletable(input.DeptId, input.VocId);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="input">削除データ</param>
		public void Delete(CsIncidentVocModel input)
		{
			this.Repository.Delete(input.DeptId, input.VocId);
		}
		#endregion
	}
}
