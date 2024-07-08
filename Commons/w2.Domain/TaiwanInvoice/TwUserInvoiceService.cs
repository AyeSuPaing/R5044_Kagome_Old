/*
=========================================================================================================
  Module      : ユーザ電子発票管理情報サービス (TwUserInvoiceService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.TwUserInvoice
{
	/// <summary>
	/// ユーザ電子発票管理情報サービス
	/// </summary>
	public class TwUserInvoiceService : ServiceBase, ITwUserInvoiceService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(string userId, int beginRowNum, int endRowNum)
		{
			using (var repository = new TwUserInvoiceRepository())
			{
				var count = repository.GetSearchHitCount(userId, beginRowNum, endRowNum);

				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>検索結果列</returns>
		public TwUserInvoiceModel[] Search(string userId, int beginRowNum, int endRowNum)
		{
			using (var repository = new TwUserInvoiceRepository())
			{
				var results = repository.Search(userId, beginRowNum, endRowNum);

				return results;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="twInvoiceNo">電子発票管理枝番</param>
		/// <returns>モデル</returns>
		public TwUserInvoiceModel Get(string userId, int twInvoiceNo)
		{
			using (var repository = new TwUserInvoiceRepository())
			{
				var model = repository.Get(userId, twInvoiceNo);

				return model;
			}
		}
		#endregion

		#region +Get All User Invoice By User Id
		/// <summary>
		/// Get All User Invoice By User Id
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>All User Invoice By User Id</returns>
		public TwUserInvoiceModel[] GetAllUserInvoiceByUserId(string userId)
		{
			using (var repository = new TwUserInvoiceRepository())
			{
				var models = repository.GetAllUserInvoiceByUserId(userId);

				return models;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">Accessor</param>
		public void Update(TwUserInvoiceModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			using (var repository = new TwUserInvoiceRepository())
			{
				repository.Update(model);
			}

			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, lastChanged, accessor);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="twInvoiceNo">電子発票管理枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">Sql Accessor</param>
		public void Delete(string userId,
			int twInvoiceNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			using (var repository = new TwUserInvoiceRepository(accessor))
			{
				repository.Delete(userId, twInvoiceNo);
			}

			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
		}
		#endregion

		#region -Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録されたユーザー配送先枝番</returns>
		public int Insert(TwUserInvoiceModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			using (var repository = new TwUserInvoiceRepository(accessor))
			{
				var invoiceNo = repository.GetNewInvoiceNo(model.UserId);
				model.TwInvoiceNo = invoiceNo;

				repository.Insert(model);

				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(model.UserId, lastChanged, accessor);
				}

				return invoiceNo;
			}
		}
		#endregion
	}
}
