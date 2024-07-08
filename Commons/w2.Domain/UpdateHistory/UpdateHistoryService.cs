/*
=========================================================================================================
  Module      : 更新履歴情報サービス (UpdateHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.Coupon.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UpdateHistory.Helper.UpdateData;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserShipping;

namespace w2.Domain.UpdateHistory
{
	/// <summary>
	/// 更新履歴情報サービス
	/// </summary>
	public class UpdateHistoryService : ServiceBase, IUpdateHistoryService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(UpdateHistoryListSearchCondition condition)
		{
			using (var repository = new UpdateHistoryRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public UpdateHistoryListSearchResult[] Search(UpdateHistoryListSearchCondition condition)
		{
			using (var repository = new UpdateHistoryRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion

		#region +BeforeAfterSearch 検索（変更前後）
		/// <summary>
		/// 取得（変更前後）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public UpdateHistoryBeforeAndAfterSearchResult BeforeAfterSearch(UpdateHistoryBeforeAndAfterSearchCondition condition)
		{
			using (var repository = new UpdateHistoryRepository())
			{
				var result = repository.BeforeAfterSearch(condition);
				return result;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="updateHistoryNo">更新履歴No</param>
		/// <returns>モデル</returns>
		public UpdateHistoryModel Get(long updateHistoryNo)
		{
			using (var repository = new UpdateHistoryRepository())
			{
				var model = repository.Get(updateHistoryNo);
				return model;
			}
		}
		#endregion

		#region +InsertAllForOrder 注文IDから全て登録
		/// <summary>
		/// 注文IDから全て登録
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertAllForOrder(
			string orderId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			var order = new OrderService().Get(orderId, accessor);
			InsertForFixedPurchase(order.FixedPurchaseId, lastChanged, accessor);
			InsertForOrder(order, lastChanged, null, accessor);
			InsertForUser(order.UserId, lastChanged, accessor);
		}
		#endregion

		#region +InsertForUser 登録（ユーザー）
		/// <summary>
		/// 登録（ユーザー）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		public decimal InsertForUser(
			string userId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			if (string.IsNullOrEmpty(userId)) return 0;

			var userRelationData = new UserService().GetWithUserRelationDatas(userId, accessor);

			if (string.IsNullOrEmpty(userRelationData.User.UserId)) return 0;

			var historyNo = InsertForUser(
				userRelationData.User,
				userRelationData.UserShipping.ToArray(),
				userRelationData.UserPoint.ToArray(),
				userRelationData.UserCouponDetail.ToArray(),
				userRelationData.UserInvoice.ToArray(),
				lastChanged,
				accessor);
			return historyNo;
		}
		#endregion
		#region -InsertForUser 登録（ユーザー）
		/// <summary>
		/// 登録（ユーザー）
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <param name="userShippings">ユーザー配送先リスト</param>
		/// <param name="userPoints">ユーザーポイント</param>
		/// <param name="userCoupons">ユーザークーポン</param>
		/// <param name="userInvoice">User Invoice</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		private decimal InsertForUser(
			UserModel user,
			UserShippingModel[] userShippings,
			UserPointModel[] userPoints,
			UserCouponDetailInfo[] userCoupons,
			TwUserInvoiceModel[] userInvoice,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			if (user == null) return 0;

			var updateData = new UpdateDataUser(user, userShippings, userPoints, userCoupons, userInvoice);
			var model = CreateUpdateHistoryModel(
				Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_USER,
				user.UserId,
				user.UserId,
				updateData,
				lastChanged);
			var historyNo = Register(model, accessor);
			return historyNo;
		}
		#endregion

		#region +InsertForOrder 登録（注文）
		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		public decimal InsertEmptyForOrder(
			string orderId,
			string userId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			if (string.IsNullOrEmpty(orderId)) return 0;

			var order = new OrderService().Get(orderId, accessor) ?? new OrderModel
			{
				OrderId = orderId,
				UserId = userId,
				IsRollbackHistoryInfo = true	// 存在しない場合はロールバック履歴とみなす
			};
			var logNo = InsertForOrder(order, lastChanged, null, accessor);
			return logNo;
		}
		#endregion

		#region +InsertForOrder 登録（注文）
		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		public decimal InsertForOrder(
			string orderId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			var logNo = InsertForOrder(orderId, lastChanged, null, accessor);
			return logNo;
		}
		#endregion
		#region +InsertForOrder 登録（注文）
		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		public decimal InsertForOrder(
			string orderId,
			string lastChanged,
			string updateUserId,
			SqlAccessor accessor = null)
		{
			if (string.IsNullOrEmpty(orderId)) return 0;

			var order = new OrderService().Get(orderId, accessor);
			if (order == null) return 0;
			var logNo = InsertForOrder(order, lastChanged, updateUserId, accessor);
			return logNo;
		}
		#endregion
		#region +InsertForOrder 登録（注文）
		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		private decimal InsertForOrder(
			OrderModel order,
			string lastChanged,
			string updateUserId,
			SqlAccessor accessor)
		{
			var userCreditCard = order.UseUserCreditCard
				? new UserCreditCardService().Get(order.UserId, order.CreditBranchNo.Value, accessor)
				: null;
			return InsertForOrder(order, userCreditCard, lastChanged, updateUserId, accessor);
		}
		#endregion
		#region +InsertForOrder 登録（注文）
		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="userCreditCard">ユーザークレジットカードモデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		public decimal InsertForOrder(
			OrderModel order,
			UserCreditCardModel userCreditCard,
			string lastChanged,
			string updateUserId = null,
			SqlAccessor accessor = null)
		{
			var updateData = order.IsRollbackHistoryInfo
				? new UpdateDataOrder()
				: new UpdateDataOrder(order, userCreditCard);
			var model = CreateUpdateHistoryModel(
				Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER,
				string.IsNullOrEmpty(updateUserId) ? order.UserId : updateUserId,
				order.OrderId,
				updateData,
				lastChanged);
			return Register(model, accessor);
		}
		#endregion

		#region +InsertForFixedPurchase 登録（定期）
		/// <summary>
		/// 登録（定期）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		public decimal InsertForFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			var logNo = InsertForFixedPurchase(fixedPurchaseId, lastChanged, null, accessor);
			return logNo;
		}
		#endregion
		#region +InsertForFixedPurchase 登録（定期）
		/// <summary>
		/// 登録（定期）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		public decimal InsertForFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			string updateUserId,
			SqlAccessor accessor = null)
		{
			if ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false) || string.IsNullOrEmpty(fixedPurchaseId)) return 0;

			var fixedPurchase = new FixedPurchaseService().GetContainer(fixedPurchaseId, accessor: accessor);
			if (fixedPurchase == null) return 0;

			var userCreditCard = fixedPurchase.UseUserCreditCard
				? new UserCreditCardService().Get(fixedPurchase.UserId, fixedPurchase.CreditBranchNo.Value, accessor)
				: null;
			var logNo = InsertForFixedPurchase(fixedPurchase, userCreditCard, lastChanged, updateUserId, accessor);
			return logNo;
		}
		#endregion
		#region -InsertForFixedPurchase 登録（定期）
		/// <summary>
		/// 登録（定期）
		/// </summary>
		/// <param name="fixedPurchase">定期購入モデル</param>
		/// <param name="userCreditCard">ユーザークレジットカードモデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		private decimal InsertForFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			UserCreditCardModel userCreditCard,
			string lastChanged,
			string updateUserId,
			SqlAccessor accessor)
		{
			if ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false) || (fixedPurchase == null)) return 0;

			var updateData = new UpdateDataFixedPurchase(fixedPurchase, userCreditCard);
			var model = CreateUpdateHistoryModel(
				Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE,
				string.IsNullOrEmpty(updateUserId) ? fixedPurchase.UserId : updateUserId,
				fixedPurchase.FixedPurchaseId,
				updateData,
				lastChanged);
			return Register(model, accessor);
		}
		#endregion

		#region -Register 登録（同じものが登録されていなければ）
		/// <summary>
		///  登録（同じものが登録されていなければ）
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		private decimal Register(UpdateHistoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new UpdateHistoryRepository(accessor))
			{
				var historyNo = repository.Register(model);
				return historyNo;
			}
		}
		#endregion

		#region -CreateUpdateHistoryModel モデル作成
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="updateHistoryKbn">更新履歴区分</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="updateData">更新データ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>モデル</returns>
		private UpdateHistoryModel CreateUpdateHistoryModel(
			string updateHistoryKbn,
			string userId,
			string masterId,
			IUpdateData updateData,
			string lastChanged)
		{
			var serializedAndHashString = updateData.SerializeAndCreateHashString();
			var model = new UpdateHistoryModel
			{
				UpdateHistoryKbn = updateHistoryKbn,
				UserId = userId,
				MasterId = masterId,
				UpdateHistoryAction = CreateUpdateHistoryAction(updateHistoryKbn, Constants.UPDATEHISTORY_APPLICATIONI_TYPE),
				UpdateData = serializedAndHashString.Item1,
				UpdateDataHash = serializedAndHashString.Item2,
				LastChanged = lastChanged,
			};
			return model;
		}
		#endregion

		#region -CreateUpdateHistoryAction 更新履歴アクション作成
		/// <summary>
		/// 更新履歴アクション作成
		/// </summary>
		/// <param name="updateHistoryKbn">更新履歴区分</param>
		/// <param name="applicationType">更新アプリケーション種別</param>
		/// <returns>更新履歴アクション</returns>
		private string CreateUpdateHistoryAction(string updateHistoryKbn, ApplicationType applicationType)
		{
			var languageCode = Constants.GLOBAL_OPTION_ENABLE
				? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
				: "";

			var masterNameReplaceTag = "";
			switch (updateHistoryKbn)
			{
				case Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_USER:
					masterNameReplaceTag = "@@DispText.master_kbn.user@@";
					break;

				case Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER:
					masterNameReplaceTag = "@@DispText.master_kbn.order@@";
					break;

				case Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE:
					masterNameReplaceTag = "@@DispText.master_kbn.fixedPurchase@@";
					break;

				default:
					return "";
			}
			var master = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(masterNameReplaceTag, languageCode);

			var applicationNameReplaceTag = "";
			switch (applicationType)
			{
				case ApplicationType.Front:
					applicationNameReplaceTag = "@@DispText.application_name.front@@";
					break;

				case ApplicationType.Manager:
					applicationNameReplaceTag = "@@DispText.application_name.manager@@";
					break;

				case ApplicationType.Batch:
					applicationNameReplaceTag = "@@DispText.application_name.batch@@";
					break;
			}
			var application = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(applicationNameReplaceTag, languageCode);

			return string.Format(
				Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
					"@@DispText.update_history.history_action.text@@",
					languageCode),
				master,
				application);
		}
		#endregion
	}
}