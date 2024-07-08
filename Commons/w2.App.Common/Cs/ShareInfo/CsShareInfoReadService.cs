/*
=========================================================================================================
  Module      : 共有情報既読管理サービス(CsShareInfoReadService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報既読管理サービス
	/// </summary>
	public class CsShareInfoReadService
	{
		/// <summary>レポジトリ</summary>
		private CsShareInfoReadRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsShareInfoReadService(CsShareInfoReadRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +SearchWithShareInfo 検索（共有情報も取得）
		/// <summary>
		/// 検索（共有情報も取得）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="infoText">共有情報テキスト</param>
		/// <param name="senderId">送信元オペレータID</param>
		/// <param name="importance">重要度</param>
		/// <param name="infoKbn">区分</param>
		/// <param name="readFlg">既読フラグ</param>
		/// <param name="pinnedFlg">ピンフラグ</param>
		/// <param name="sortKbn">ソート区分</param>
		/// <param name="pageNo">ページ番号</param>
		/// <returns>共有情報リスト</returns>
		public CsShareInfoModel[] SearchWithShareInfo(string deptId, string operatorId, string infoText, string senderId, string importance, string infoKbn, string readFlg, string pinnedFlg, string sortKbn, int pageNo)
		{
			DataView dv = this.Repository.SearchWithShareInfo(deptId, operatorId, infoText, senderId, importance, infoKbn, readFlg, pinnedFlg, sortKbn, pageNo);
			return (from DataRowView drv in dv select new CsShareInfoModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>モデル</returns>
		public CsShareInfoReadModel Get(string deptId, long infoNo, string operatorId)
		{
			var dv = this.Repository.Get(deptId, infoNo, operatorId);
			return (dv.Count == 0) ? null : new CsShareInfoReadModel(dv[0]);
		}
		#endregion

		#region +GetAll 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <returns>モデル配列</returns>
		public CsShareInfoReadModel[] GetAll(string deptId, long infoNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				return this.GetAll(deptId, infoNo, accessor);
			}
		}
		#endregion
		#region +GetAll 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル配列</returns>
		private CsShareInfoReadModel[] GetAll(string deptId, long infoNo, SqlAccessor accessor)
		{
			var dv = this.Repository.GetAll(deptId, infoNo, accessor);
			return (from DataRowView drv in dv select new CsShareInfoReadModel(drv)).ToArray();
		}
		#endregion

		#region +GetOperatorUnreadCount オペレータ未読件数カウント
		/// <summary>
		/// オペレータ未読件数カウント取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>未読件数</returns>
		public int GetOperatorUnreadCount(string deptId, string operatorId)
		{
			return this.Repository.SearchByReadFlg(deptId, operatorId, Constants.FLG_CSSHAREINFOREAD_READ_FLG_UNREAD).Count;
		}
		#endregion

		#region +UpdateReadFlg 既読フラグ更新
		/// <summary>
		/// 既読フラグ更新
		/// </summary>
		/// <param name="model">共有情報既読管理情報</param>
		public void UpdateReadFlg(CsShareInfoReadModel model)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				this.Repository.UpdateReadFlg(model.DataSource, sqlAccessor);
			}
		}
		#endregion

		#region +UpdatePinnedFlg ピン止めフラグ更新
		/// <summary>
		/// ピン止めフラグ更新
		/// </summary>
		/// <param name="model">共有情報既読管理情報</param>
		public void UpdatePinnedFlg(CsShareInfoReadModel model)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				this.Repository.UpdatePinnedFlg(model.DataSource, sqlAccessor);
			}
		}
		#endregion

		#region -RegisterDelete 登録・削除
		/// <summary>
		/// 登録・削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo"></param>
		/// <param name="models">登録モデル配列</param>
		public void RegisterDelete(string deptId, long infoNo, CsShareInfoReadModel[] regiserModels)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 既存取得
				var existModels = GetAll(deptId, infoNo, accessor);

				// 既存に無ければ登録
				foreach (var regModel in regiserModels)
				{
					if (Array.Exists(existModels, e => e.OperatorId == regModel.OperatorId) == false) Register(regModel, accessor);
				}

				// なくなっていれば削除
				foreach (var extModel in existModels)
				{
					if (Array.Exists(regiserModels, e => e.OperatorId == extModel.OperatorId) == false) Delete(extModel, accessor);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region -Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void Register(CsShareInfoReadModel model, SqlAccessor accessor)
		{
			this.Repository.Register(model.DataSource, accessor);
		}
		#endregion

		#region -Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void Delete(CsShareInfoReadModel model, SqlAccessor accessor)
		{
			this.Repository.Delete(model.DataSource, accessor);
		}
		#endregion

		#region +DeleteAll 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="infoNo">共有情報NO</param>
		/// <returns>モデル</returns>
		public void DeleteAll(string deptId, long infoNo)
		{
			Hashtable ht = new Hashtable();
			ht.Add(Constants.FIELD_CSSHAREINFOREAD_DEPT_ID, deptId);
			ht.Add(Constants.FIELD_CSSHAREINFOREAD_INFO_NO, infoNo);

			this.Repository.DeleteAll(ht);
		}
		#endregion
	}
}
