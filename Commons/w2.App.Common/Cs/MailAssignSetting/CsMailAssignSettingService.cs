/*
=========================================================================================================
  Module      : メール振分設定サービス(CsMailAssignSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;
using w2.App.Common.Cs.CsOperator;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.MailAssignSetting
{
	/// <summary>
	/// メール振分設定サービス
	/// </summary>
	public class CsMailAssignSettingService
	{
		/// <summary>レポジトリ</summary>
		private CsMailAssignSettingRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsMailAssignSettingService(CsMailAssignSettingRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region -Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <returns>モデル</returns>
		private CsMailAssignSettingModel Get(string deptId, string mailAssignId)
		{
			var dv = this.Repository.Get(deptId, mailAssignId);
			return (dv.Count == 0) ? null : new CsMailAssignSettingModel(dv[0]);
		}
		#endregion

		#region +GetWithItems アイテム含めて取得
		/// <summary>
		/// アイテム含めて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <returns>モデル</returns>
		public CsMailAssignSettingModel GetWithItems(string deptId, string mailAssignId)
		{
			var model = this.Get(deptId, mailAssignId);

			var itemService = new CsMailAssignSettingItemService(new CsMailAssignSettingItemRepository());
			model.Items = itemService.GetAll(deptId, model.MailAssignId);
			return model;
		}
		#endregion

		#region -GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル配列</returns>
		private CsMailAssignSettingModel[] GetAll(string deptId)
		{
			var dv = this.Repository.GetAll(deptId);
			return (from DataRowView drv in dv select new CsMailAssignSettingModel(drv)).ToArray();
		}
		/// <summary>
		/// Get all by mail assign ids
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignIds">Mail assign ids</param>
		/// <returns>モデル配列</returns>
		private CsMailAssignSettingModel[] GetAll(string deptId, string[] mailAssignIds)
		{
			var data = this.Repository.GetAll(deptId, mailAssignIds);
			return (from DataRowView row in data select new CsMailAssignSettingModel(row)).ToArray();
		}
		#endregion

		#region +GetAllWithItems アイテム含めて全件取得
		/// <summary>
		/// アイテム含めて全件取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル配列</returns>
		public CsMailAssignSettingModel[] GetAllWithItems(string deptId)
		{
			var models = this.GetAll(deptId);

			var itemService = new CsMailAssignSettingItemService(new CsMailAssignSettingItemRepository());
			foreach (var model in models)
			{
				model.Items = itemService.GetAll(deptId, model.MailAssignId);
			}
			return models;
		}
		/// <summary>
		/// Get all items by mail assign ids
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignIds">Mail assign ids</param>
		/// <returns>モデル配列</returns>
		public CsMailAssignSettingModel[] GetAllWithItems(string deptId, string[] mailAssignIds)
		{
			var models = this.GetAll(deptId, mailAssignIds);

			var itemService = new CsMailAssignSettingItemService(new CsMailAssignSettingItemRepository());
			foreach (var model in models)
			{
				model.Items = itemService.GetAll(deptId, model.MailAssignId);
			}
			return models;
		}
		#endregion

		#region -Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>登録したメール振分ID</returns>
		private string Insert(CsMailAssignSettingModel model, SqlAccessor accessor)
		{
			// 登録
			model.MailAssignId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_MAIL_ASSIGN_ID, 3);
			this.Repository.Register(model.DataSource, accessor);
			return model.MailAssignId;
		}
		#endregion

		#region +InsertWithItems アイテム含めて登録
		/// <summary>
		/// アイテム含めて登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録したメール振分ID</returns>
		public string InsertWithItems(CsMailAssignSettingModel model)
		{
			using(SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 登録
				var mailAssignId = this.Insert(model, accessor);

				// 詳細登録
				var itemService = new CsMailAssignSettingItemService(new CsMailAssignSettingItemRepository());
				foreach (var item in model.Items)
				{
					item.MailAssignId = mailAssignId;
					itemService.Register(item, accessor);
				}
				accessor.CommitTransaction();

				return mailAssignId;
			}
		}
		#endregion

		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void Update(CsMailAssignSettingModel model, SqlAccessor accessor)
		{
			this.Repository.Update(model.DataSource, accessor);
		}
		#endregion

		#region +UpdateWithItems アイテム含めて更新
		/// <summary>
		/// アイテム含めて更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void UpdateWithItems(CsMailAssignSettingModel model)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				this.Update(model, accessor);

				// 詳細更新
				var itemService = new CsMailAssignSettingItemService(new CsMailAssignSettingItemRepository());
				foreach (var item in model.Items)
				{
					itemService.Update(item, accessor);
				}
				itemService.DeleteAfter(model.DeptId, model.MailAssignId, model.Items.Length + 1, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateMailAssignSettingByRemoveOperator 移動になったオペレータでメール振り分け設定更新
		/// <summary>
		/// 移動になったオペレータでメール振り分け設定更新
		/// </summary>
		/// <param name="deptId">ログインオペレータ識別ID</param>
		/// <param name="groupId">拠点ID</param>
		/// <param name="removeOperators">移動になったオペレーター</param>
		public void UpdateMailAssignSettingByRemoveOperator(string deptId, string groupId, CsOperatorModel[] removeOperators)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				foreach (var removeOperator in removeOperators)
				{
					Repository.UpdateMailAssignSettingByRemoveOperator(deptId, groupId, removeOperator.OperatorId, accessor);
				}
				
				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +DeleteWithItems アイテム含めて削除
		/// <summary>
		/// アイテム含めて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="signatureId">メール振分設定ID</param>
		public void DeleteWithItems(string deptId, string mailAssignId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 削除
				this.Repository.Delete(deptId, mailAssignId, accessor);

				// 詳細削除
				var itemService = new CsMailAssignSettingItemService(new CsMailAssignSettingItemRepository());
				itemService.Delete(deptId, mailAssignId, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion
	}
}
