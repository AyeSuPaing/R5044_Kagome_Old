/*
=========================================================================================================
  Module      : CSオペレータサービス(CsOperatorService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsOperatorService
	{
		/// <summary>レポジトリ</summary>
		private CsOperatorRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsOperatorService(CsOperatorRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>オペレータ情報</returns>
		public CsOperatorModel Get(string deptId, string operatorId)
		{
			DataView dv = this.Repository.Get(deptId, operatorId);
			return (dv.Count == 0) ? null : new CsOperatorModel(dv[0]);
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ情報のリスト</returns>
		public CsOperatorModel[] GetAll(string deptId)
		{
			DataView dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsOperatorModel(drv)).ToArray();
		}
		#endregion

		#region GetValidAll 有効なオペレータすべて取得
		/// <summary>
		/// 有効なオペレータすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ情報のリスト</returns>
		public CsOperatorModel[] GetValidAll(string deptId)
		{
			DataView dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv where ((string)drv[Constants.FIELD_SHOPOPERATOR_VALID_FLG] == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID) select new CsOperatorModel(drv)).ToArray();
		}
		#endregion

		#region +GetApprovalValidAll 承認可能で有効なオペレータすべて取得
		/// <summary>
		/// 承認可能で有効なオペレータすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ情報リスト</returns>
		public CsOperatorModel[] GetApprovalValidAll(string deptId)
		{
			DataView dv = this.Repository.GetApprovalValidAll(deptId);
			return (from DataRowView drv in dv select new CsOperatorModel(drv)).ToArray();
		}
		#endregion

		#region +GetMailSendableValidAll メール送信可能で有効なオペレータすべて取得
		/// <summary>
		/// メール送信可能で有効なオペレータすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ情報リスト</returns>
		public CsOperatorModel[] GetMailSendableValidAll(string deptId)
		{
			DataView dv = this.Repository.GetMailSendableValidAll(deptId);
			return (from DataRowView drv in dv select new CsOperatorModel(drv)).ToArray();
		}
		#endregion

		#region +RegisterUpdate 登録更新
		/// <summary>
		/// 登録更新
		/// </summary>
		/// <param name="model">登録用データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void RegisterUpdate(CsOperatorModel model, SqlAccessor accessor)
		{
			var modelBefore = this.Repository.Get(model.DeptId, model.OperatorId, accessor);
			if (modelBefore.Count == 0)
			{
				this.Repository.Register(model.DataSource, accessor);
			}
			else
			{
				this.Repository.Update(model.DataSource, accessor);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string operatorId, SqlAccessor accessor)
		{
			this.Repository.Delete(deptId, operatorId, accessor);
		}
		#endregion

		#region +DeleteCheck 削除チェック
		/// <summary>
		/// 削除チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:削除不可 false:削除可</returns>
		public bool DeleteCheck(string deptId, string operatorId, SqlAccessor accessor = null)
		{
			var dv = (accessor == null) ? Repository.DeleteCheck(deptId, operatorId) : Repository.DeleteCheck(deptId, operatorId, accessor);
			return dv.Count > 0;
		}
		#endregion
	}
}
