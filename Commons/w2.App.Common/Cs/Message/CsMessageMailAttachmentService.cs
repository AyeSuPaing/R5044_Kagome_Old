/*
=========================================================================================================
  Module      : メッセージメール添付ファイルサービス(CsMessageMailAttachmentService.cs)
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
	/// メッセージメール添付ファイルサービス
	/// </summary>
	public class CsMessageMailAttachmentService
	{
		/// <summary>レポジトリ</summary>
		private CsMessageMailAttachmentRepository Repository;

		#region +コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsMessageMailAttachmentService(CsMessageMailAttachmentRepository repository)
		{
			this.Repository = repository;
		}

		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="fileNo">ファイルNO</param>
		/// <returns>メッセージメール添付ファイル</returns>
		public CsMessageMailAttachmentModel Get(string deptId, string mailId, int fileNo)
		{
			var dv = this.Repository.Get(deptId, mailId, fileNo);
			if (dv.Count == 0) return null;

			return new CsMessageMailAttachmentModel(dv[0]);
		}
		#endregion

		#region +GetAll メールに紐づくものすべて取得
		/// <summary>
		/// メールに紐づくものすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <returns>リスト</returns>
		public CsMessageMailAttachmentModel[] GetAll(string deptId, string mailId)
		{
			var dv = this.Repository.GetAll(deptId, mailId);
			return (from DataRowView drv in dv select new CsMessageMailAttachmentModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// メール添付ファイル登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>メール添付ファイルNO</returns>
		public int Register(CsMessageMailAttachmentModel model, SqlAccessor accessor)
		{
			var fileNo = this.Repository.GetRegisterFileNo(model.DeptId, model.MailId, accessor);
			model.FileNo = fileNo;

			this.Repository.Register(model.DataSource, accessor);

			return fileNo;
		}
		#endregion
		#region +Register 登録
		/// <summary>
		/// メール添付ファイル登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>メール添付ファイルNO</returns>
		public int Register(CsMessageMailAttachmentModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				int fileNo = this.Register(model, accessor);

				accessor.CommitTransaction();

				return fileNo;
			}
		}
		#endregion

		#region +UpdateTempIdToFormalId 仮IDを正式IDに更新
		/// <summary>
		/// 仮IDを正式IDに更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateTempIdToFormalId(CsMessageMailAttachmentModel model, SqlAccessor accessor)
		{
			this.Repository.UpdateTempIdToFormalId(model.DataSource, accessor);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="fileNo">ファイルNO</param>
		public void Delete(string deptId, string mailId, int fileNo)
		{
			this.Repository.Delete(deptId, mailId, fileNo);
		}
		#endregion

		#region +DeleteAll メールに紐づくものすべて取得
		/// <summary>
		/// メールに紐づくものすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAll(string deptId, string mailId, SqlAccessor accessor)
		{
			this.Repository.DeleteAll(deptId, mailId, accessor);
		}
		#endregion
	}
}
