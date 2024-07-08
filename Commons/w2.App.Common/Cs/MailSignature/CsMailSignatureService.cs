/*
=========================================================================================================
  Module      : メール署名サービス(CsMailSignatureService.cs)
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

namespace w2.App.Common.Cs.MailSignature
{
	public class CsMailSignatureService
	{
		/// <summary>レポジトリ</summary>
		private CsMailSignatureRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsMailSignatureService(CsMailSignatureRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Search 一覧取得
		/// <summary>
		/// 一覧取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="bgnRow">開始行</param>
		/// <param name="endRow">終了行</param>
		/// <returns>メール署名データリスト</returns>
		public CsMailSignatureModel[] Search(string deptId, string operatorId, int bgnRow, int endRow)
		{
			DataView dv = this.Repository.Search(deptId, operatorId, bgnRow, endRow);
			return (from DataRowView drv in dv select new CsMailSignatureModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="signatureId">メール署名ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>メール署名データ</returns>
		public CsMailSignatureModel Get(string deptId, string signatureId, string operatorId)
		{
			DataView dv = this.Repository.Get(deptId, signatureId, operatorId);
			return (dv.Count == 0) ? null : new CsMailSignatureModel(dv[0]);
		}
		#endregion

		#region +GetUsableAll 指定オペレータにとって使用可能なものを取得
		/// <summary>
		/// 有効なものを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>メール署名データリスト</returns>
		public CsMailSignatureModel[] GetUsableAll(string deptId, string operatorId)
		{
			DataView dv = this.Repository.GetUsableAll(deptId, operatorId);
			return (from DataRowView drv in dv select new CsMailSignatureModel(drv)).ToArray();
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">メール署名データ</param>
		public void Register(CsMailSignatureModel model)
		{
			// Create new mail signature ID
			model.MailSignatureId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_MAIL_SIGNATURE_ID, 3);

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
		/// <param name="model">メール署名データ</param>
		public void Update(CsMailSignatureModel model)
		{
			this.Repository.Update(model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="signatureId">メール署名ID</param>
		public void Delete(string deptId, string signatureId)
		{
			this.Repository.Delete(deptId, signatureId);
		}
		#endregion
	}
}
