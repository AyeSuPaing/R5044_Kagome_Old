/*
=========================================================================================================
  Module      : ユーザー統合情報サービス (UserIntegrationService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.User;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.Order;
using w2.Domain.FixedPurchase;
using w2.Domain.CsIncident;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.MailSendLog;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;
using w2.Domain.UserIntegration.Helper;
using w2.Domain.Coupon;
using w2.Domain.DmShippingHistory;

namespace w2.Domain.UserIntegration
{
	/// <summary>
	/// ユーザー統合情報サービス
	/// </summary>
	public class UserIntegrationService : ServiceBase
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(UserIntegrationListSearchCondition condition)
		{
			using (var repository = new UserIntegrationRepository())
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
		public UserIntegrationListSearchResult[] Search(UserIntegrationListSearchCondition condition)
		{
			using (var repository = new UserIntegrationRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <returns>モデル</returns>
		public UserIntegrationModel Get(long userIntegrationNo)
		{
			using (var repository = new UserIntegrationRepository())
			{
				var model = repository.Get(userIntegrationNo);
				if (model == null) return null;

				// ユーザー統合ユーザ情報&ユーザー統合履歴情報セット
				model.Users = repository.GetUserAll(userIntegrationNo);
				foreach (var user in model.Users)
				{
					user.Histories = repository.GetHistoryAll(userIntegrationNo, user.UserId);
				}
				
				return model;
			}
		}
		#endregion

		#region +GetContainer 取得（表示用）
		/// <summary>
		/// 取得（表示用）
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <returns>モデル</returns>
		public UserIntegrationContainer GetContainer(long userIntegrationNo)
		{
			using (var repository = new UserIntegrationRepository())
			{
				var container = repository.GetContainer(userIntegrationNo);
				foreach (var user in container.Users)
				{
					// ユーザーポイントが移行されている場合は、ユーザポイント履歴情報をセット
					if (user.Histories.Any(h => h.TableName == Constants.TABLE_USERPOINT))
					{
						var userPointHistores = new PointService().GetUserPointHistories(user.UserId);
						foreach (var history in user.Histories.Where(h => h.TableName == Constants.TABLE_USERPOINT))
						{
							history.UserPointHistory = userPointHistores.FirstOrDefault(h => h.HistoryNo == int.Parse(history.PrimaryKey2));
						}
					}
				}
				return container;
			}
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>モデル列</returns>
		public UserIntegrationModel[] GetAll()
		{
			using (var repository = new UserIntegrationRepository())
			{
				return repository.GetAll();
			}
		}
		#endregion

		#region +GetUnintegratedUsers ユーザー統合解除情報取得
		/// <summary>
		/// ユーザー統合解除情報取得
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <returns>モデル列</returns>
		public UserIntegrationModel[] GetUnintegratedUsers(long userIntegrationNo)
		{
			using (var repository = new UserIntegrationRepository())
			{
				return repository.GetUnintegratedUsers(userIntegrationNo);
			}
		}
		#endregion

		#region +GetUserIntegrationByUserId ユーザーIDからユーザー統合情報取得
		/// <summary>
		/// ユーザーIDからユーザー統合情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル列</returns>
		public UserIntegrationModel[] GetUserIntegrationByUserId(string userId)
		{
			using (var repository = new UserIntegrationRepository())
			{
				return repository.GetUserIntegrationByUserId(userId);
			}
		}
		#endregion

		#region +GetIntegratedUserId 統合先ユーザーID取得
		/// <summary>
		/// 統合先ユーザーID取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>統合先ユーザーID</returns>
		public string GetIntegratedUserId(string userId)
		{
			using (var repository = new UserIntegrationRepository())
			{
				return repository.GetIntegratedUserId(userId);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(UserIntegrationModel model, SqlAccessor accessor)
		{
			using (var repository = new UserIntegrationRepository(accessor))
			{
				// 登録（ユーザー統合Noをセット）
				model.UserIntegrationNo = repository.Insert(model);
				foreach (var user in model.Users)
				{
					// ユーザー統合ユーザ情報
					user.UserIntegrationNo = model.UserIntegrationNo;
					repository.InsertUser(user);
					foreach (var history in user.Histories)
					{
						// ユーザー統合履歴情報
						history.UserIntegrationNo = model.UserIntegrationNo;
						repository.InsertHistory(history);
					}
				}
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserIntegrationModel model, SqlAccessor accessor)
		{
			var result = 0;
			using (var repository = new UserIntegrationRepository(accessor))
			{
				// 更新
				result = repository.Update(model);

				// ユーザー統合ユーザ情報&ユーザー統合履歴情報（DELETE => INSERT）
				repository.DeleteUser(model.UserIntegrationNo);
				repository.DeleteHistory(model.UserIntegrationNo);
				foreach (var user in model.Users)
				{
					repository.InsertUser(user);

					foreach (var history in user.Histories)
					{
						repository.InsertHistory(history);
					}
				}
			}

			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(long userIntegrationNo, SqlAccessor accessor)
		{
			using (var repository = new UserIntegrationRepository(accessor))
			{
				var result = repository.Delete(userIntegrationNo);
				repository.DeleteUser(userIntegrationNo);
				repository.DeleteHistory(userIntegrationNo);
				return result;
			}
		}
		#endregion

		#region +ユーザー統合操作関連
		/// <summary>
		/// ステータスを「統合未確定」に更新
		/// ※現在のステータスが「統合済み」の場合、統合解除処理（ユーザ紐づけ解除）を実行
		/// </summary>
		/// <param name="userIntegration">ユーザー統合モデル</param>
		/// <param name="register">登録するか？</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isBlackListCouponJudgedByUserId">UserIdによってブラックリストクーポン判定しているか(TRUE:ユーザーID、FALSE:メールアドレス)</param>
		public void CancelUserIntegration(
			UserIntegrationModel userIntegration,
			bool register,
			string deptId,
			string lastChanged,
			bool isBlackListCouponJudgedByUserId)
		{
			// 変更前のステータス取得
			var tempStatus = userIntegration.Status;

			// ステータスを「統合未確定」、最終更新者をセット
			userIntegration.Status = Constants.FLG_USERINTEGRATION_STATUS_NONE;
			userIntegration.LastChanged = lastChanged;

			// 代表ユーザーID取得
			var representativeUserId = userIntegration.Users.FirstOrDefault(u => u.IsOnRepresentativeFlg).UserId;

			var userService = new UserService();
			var couponService = new CouponService();
			var representativeUserOld = userService.Get(representativeUserId);

			// 代表ユーザーは定期会員か
			var isFixedPurchaseMember =
				(representativeUserOld.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON);

			// 代表ユーザーは通常会員になるかの判定
			var fixedPurchaseService = new FixedPurchaseService();
			var nonRepresentativeFixedPurchaseIds =
				userIntegration.Users.Where(u => u.IsOnRepresentativeFlg == false).SelectMany(h => h.Histories)
				.Where(h => h.TableName == Constants.TABLE_FIXEDPURCHASE).Select(f => f.PrimaryKey1).ToList();
			var becomeNormalMember = 
				(fixedPurchaseService.HasActiveFixedPurchaseInfo(representativeUserId, nonRepresentativeFixedPurchaseIds) == false);

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 未登録の場合は登録
				if (register)
				{
					this.Insert(userIntegration, accessor);
				}

				// 現在のステータスが「統合済み」の場合、統合解除処理（ユーザ紐づけ解除）を実行
				if (tempStatus == Constants.FLG_USERINTEGRATION_STATUS_DONE)
				{
					// 非代表ユーザーでループ
					foreach (var user in userIntegration.Users.Where(u => u.IsOnRepresentativeFlg == false))
					{
						// 注文情報：ユーザーIDを元に戻す
						var orderService = new OrderService();
						foreach (var order in user.Histories.Where(h => h.TableName == Constants.TABLE_ORDER))
						{
							// 注文情報（返品交換含む）更新
							var relatedOrders = orderService.GetRelatedOrders(order.PrimaryKey1, accessor);
							foreach (var relatedOrder in relatedOrders)
							{
								orderService.UpdateUserId(relatedOrder.OrderId, user.UserId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);

								// 更新履歴登録
								new UpdateHistoryService().InsertForOrder(relatedOrder.OrderId, lastChanged, null, accessor);
								new UpdateHistoryService().InsertForOrder(relatedOrder.OrderId, lastChanged, representativeUserId, accessor);
							}
						}

						// ユーザーポイント情報：ユーザーにポイントを戻す
						var pointService = new PointService();
						var a = user.Histories.Where(h => h.TableName == Constants.TABLE_USERPOINT);
						foreach (var userPointHistory in a)
						{
							var contents = new PointIntegrationCancelContents()
							{
								RepresentativeUserId = representativeUserId,
								TargetUserPointHistory = new PointService().GetUserPointHistories(userPointHistory.UserId, accessor).FirstOrDefault(h => h.HistoryNo == int.Parse(userPointHistory.PrimaryKey2)),
								OperatorName = lastChanged
							};
							pointService.CancelPointIntegration(contents, UpdateHistoryAction.DoNotInsert, accessor);
							pointService.DeleteUserPointHistory(contents.TargetUserPointHistory);
						}

						// ユーザーポイント履歴を戻す
						var userPointHistoies = user.Histories.Where(h => h.TableName == Constants.TABLE_USERPOINTHISTORY).ToArray();
						var historyNo = pointService.GetNextHistoryNo(user.UserId, accessor);
						var historyGroupNo = pointService.IssueHistoryGroupNoForUser(user.UserId, accessor);
						// ポイント履歴グループ番号は1から採番
						var checkHistoryGroupNo = userPointHistoies.Any() ? int.Parse(userPointHistoies.First().PrimaryKey2) : 1;
						foreach (var userPointHistory in userPointHistoies)
						{
							// 非代表ユーザーの履歴グループ番号が異なる場合に履歴グループ番号を更新
							if (checkHistoryGroupNo != int.Parse(userPointHistory.PrimaryKey2))
							{
								historyGroupNo++;
								checkHistoryGroupNo = int.Parse(userPointHistory.PrimaryKey2);
							}

							// 代表ユーザーのポイント履歴No取得
							var representativeUserHistoryNo = userPointHistory.PrimaryKey1;
							pointService.UpdateUserPointHistoryForIntegration(
								user.UserId,
								representativeUserId,
								historyNo,
								int.Parse(representativeUserHistoryNo),
								historyGroupNo,
								accessor);

							historyNo++;
						}

						// 定期購入情報：ユーザーIDを元に戻す
						foreach (var fixedPurchase in user.Histories.Where(h => h.TableName == Constants.TABLE_FIXEDPURCHASE))
						{
							fixedPurchaseService.UpdateUserIdForIntegration(fixedPurchase.PrimaryKey1, user.UserId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);

							// 更新履歴登録
							new UpdateHistoryService().InsertForFixedPurchase(fixedPurchase.PrimaryKey1, lastChanged, null, accessor);
							new UpdateHistoryService().InsertForFixedPurchase(fixedPurchase.PrimaryKey1, lastChanged, representativeUserId, accessor);
						}

						var fixedPurchases = fixedPurchaseService.GetFixedPurchasesByUserId(
							user.UserId,
							accessor);
						foreach (var userCouponIntegrationHistory in user.Histories
							.Where(h => h.TableName == Constants.TABLE_USERCOUPON))
						{
							var couponId = userCouponIntegrationHistory.PrimaryKey1;
							var oldCouponNo = int.Parse(userCouponIntegrationHistory.PrimaryKey2);
							var newCouponNo = int.Parse(userCouponIntegrationHistory.PrimaryKey3);
							new OrderService().UpdateCouponForIntegration(
								user.UserId,
								couponId,
								newCouponNo.ToString(),
								oldCouponNo.ToString(),
								lastChanged,
								accessor);

							var fixedPurchaseUseUserCouponList = fixedPurchases
								.Where(fixedPurchase =>
									((fixedPurchase.NextShippingUseCouponId == couponId)
										&& (fixedPurchase.NextShippingUseCouponNo == oldCouponNo)));
							foreach (var fixedPurchase in fixedPurchaseUseUserCouponList)
							{
								fixedPurchaseService.UpdateNextShippingUseCoupon(
									fixedPurchase.FixedPurchaseId,
									fixedPurchase.NextShippingUseCouponId,
									newCouponNo,
									lastChanged,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}

							couponService.UpdateUserCouponForIntegration(
								user.UserId,
								representativeUserId,
								couponId,
								newCouponNo.ToString(),
								oldCouponNo.ToString(),
								accessor);
						}

						// ブラックリストクーポン情報統合解除
						var judgeValueRepresentative = isBlackListCouponJudgedByUserId
							? representativeUserId
							: representativeUserOld.MailAddr;
						foreach (var couponUseUserHistory in user.Histories
							.Where(history => history.TableName == Constants.TABLE_COUPONUSEUSER))
						{
							couponService.DeleteCouponUseUser(couponUseUserHistory.PrimaryKey1, judgeValueRepresentative, accessor);
						}

						// クーポン履歴情報統合解除
						foreach (var userCouponHistoryIntegrationHistory in user.Histories
							.Where(h => h.TableName == Constants.TABLE_USERCOUPONHISTORY))
						{
							var historyNoOld = userCouponHistoryIntegrationHistory.PrimaryKey1;
							var historyNoNew = userCouponHistoryIntegrationHistory.PrimaryKey2;
							couponService.UpdateUserCouponHistoryForIntegration(
								user.UserId,
								representativeUserId,
								historyNoNew,
								historyNoOld,
								accessor);
						}

						// DM発送履歴を元に戻す
						var dmShippingHistoryService = new DmShippingHistoryService();
						var dmShippingHistories = user.Histories.Where(h => h.TableName == Constants.TABLE_DMSHIPPINGHISTORY);
						foreach (var dmShippingHistory in dmShippingHistories)
						{
							var dmShipingHostoryModel = new DmShippingHistoryModel()
							{
								RepresentativeUserId = dmShippingHistory.PrimaryKey1,
								DmCode = dmShippingHistory.PrimaryKey2,
								UserId = representativeUserId,
							};
							dmShippingHistoryService.UpdateForUserIntegration(dmShipingHostoryModel, accessor);
						}

						// クレジットカード情報：ユーザーIDを元に戻す。
						// 注文情報：クレジット枝番を元に戻す。
						// 定期情報：クレジット枝番を元に戻す
						var creditService = new UserCreditCardService();
						var creditCards = creditService.GetByUserId(representativeUserId);
						var orders = orderService.GetOrdersByUserId(user.UserId, accessor);
						foreach (var creditBranchInfo in user.Histories.Where(h => (h.TableName == Constants.TABLE_USERCREDITCARD)).OrderBy(history => history.PrimaryKey1))
						{
							var card = creditCards.FirstOrDefault(c => (c.BranchNo.ToString() == creditBranchInfo.PrimaryKey2));
							if (card == null) continue;
							var updateOrders = orders.Where(o => (o.CreditBranchNo == card.BranchNo)).ToArray();
							var updateFixedPurchases = fixedPurchases.Where(f => (f.CreditBranchNo == card.BranchNo)).ToArray();

							creditService.Delete(representativeUserId, card.BranchNo, accessor);

							updateOrders.ToList().ForEach(
								uo => orderService.UpdateCreditBranchNo(
									uo.OrderId,
									Convert.ToInt32(creditBranchInfo.PrimaryKey1),
									lastChanged,
									UpdateHistoryAction.DoNotInsert,
									accessor));

							updateFixedPurchases.ToList().ForEach(
								uf => fixedPurchaseService.UpdateOrderPayment(
									uf.FixedPurchaseId,
									uf.OrderPaymentKbn,
									Convert.ToInt32(creditBranchInfo.PrimaryKey1),
									uf.CardInstallmentsCode,
									uf.ExternalPaymentAgreementId,
									lastChanged,
									UpdateHistoryAction.DoNotInsert,
									accessor));
						}

						// インシデント：ユーザーIDを元に戻す
						var csIncidenService = new CsIncidentService();
						foreach (var csIncident in user.Histories.Where(h => h.TableName == Constants.TABLE_CSINCIDENT))
						{
							csIncidenService.UpdateUserId(csIncident.PrimaryKey1, csIncident.PrimaryKey2, user.UserId, lastChanged, accessor);
						}

						// メール送信ログ：ユーザーIDを元に戻す
						var mailSendLogService = new MailSendLogService();
						foreach (var mailSendLog in user.Histories.Where(h => h.TableName == Constants.TABLE_MAILSENDLOG))
						{
							mailSendLogService.UpdateUserId(long.Parse(mailSendLog.PrimaryKey1), user.UserId, accessor);
						}

						// ユーザーアクティビティ:ユーザーIDを元に戻す
						foreach (var useractivity in user.Histories.Where(h => h.TableName == Constants.TABLE_USERACTIVITY))
						{
							var model = new UserActivityModel
							{
								UserId = representativeUserId,
								MasterKbn = useractivity.PrimaryKey1,
								MasterId = useractivity.PrimaryKey2
							};
							userService.DeleteUserActivity(model.UserId, model.MasterKbn, model.MasterId, accessor);
							model.UserId = useractivity.UserId;
							userService.InsertUserActivity(model, accessor);
						}

						// ユーザー情報：ユーザー統合フラグを「通常」に更新
						userService.UpdateOffIntegratedFlg(user.UserId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);

						var orderCountOld = userService.Get(user.UserId, accessor).OrderCountOld;
						// ユーザーの受注情報の累計購入回数を振り直しとユーザーリアルタイム累計購入回数更新
						UpdateOrderCountByCancelConfirmUserIntegration(user.UserId, orderCountOld, accessor);

						// 更新履歴登録（非代表ユーザー）
						new UpdateHistoryService().InsertForUser(user.UserId, lastChanged, accessor);

						// ユーザー統合履歴を削除
						user.Histories = new UserIntegrationHistoryModel[0];
					}

					// 代表ユーザーは定期会員で、生きている定期注文情報が存在しない場合、定期会員フラグをOFFに更新する
					if (isFixedPurchaseMember && becomeNormalMember)
					{
						userService.UpdateFixedPurchaseMemberFlg(
							representativeUserId,
							Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF,
							lastChanged,
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}

					var orderCountOldRepresent = userService.Get(representativeUserId, accessor).OrderCountOld;
					// ユーザーの受注情報の累計購入回数を振り直しとユーザーリアルタイム累計購入回数更新
					UpdateOrderCountByCancelConfirmUserIntegration(representativeUserId, orderCountOldRepresent, accessor);

					// 最終誕生日クーポン発行年更新
					userService.UpdateUserBirthdayCouponPublishYearByCouponInfo(representativeUserId, lastChanged, accessor);

					// 更新履歴登録（代表ユーザー）
					new UpdateHistoryService().InsertForUser(representativeUserId, lastChanged, accessor);
				}

				// 更新
				if (register == false)
				{
					this.Update(userIntegration, accessor);
				}
				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// ステータスを「保留」に更新
		/// </summary>
		/// <param name="userIntegration">ユーザー統合モデル</param>
		/// <param name="register">登録するか？</param>
		/// <param name="lastChanged">最終更新者</param>
		public void SuspendUserIntegration(UserIntegrationModel userIntegration, bool register, string lastChanged)
		{
			// ステータスを「保留」、最終更新者をセット
			userIntegration.Status = Constants.FLG_USERINTEGRATION_STATUS_SUSPEND;
			userIntegration.LastChanged = lastChanged;

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				// 未登録の場合は登録
				if (register)
				{
					this.Insert(userIntegration, accessor);
				}
				// 更新
				else
				{
					this.Update(userIntegration, accessor);
				}
				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// ステータスを「統合確定」に更新
		/// ユーザー統合処理（ユーザー紐づけ）を実行
		/// </summary>
		/// <param name="userIntegration">ユーザー統合モデル</param>
		/// <param name="register">登録するか？</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="systemUserItems">システムで利用しているユーザー拡張項目</param>
		/// <param name="isBlackListCouponJudgedByUserId">UserIdによってブラックリストクーポン判定しているか(TRUE:ユーザーID、FALSE:メールアドレス)</param>
		/// <param name="isZeus">ゼウスか</param>
		public void ExecuteUserIntegration(
			UserIntegrationModel userIntegration,
			bool register,
			string deptId,
			string lastChanged,
			string[] systemUserItems,
			bool isZeus,
			bool isBlackListCouponJudgedByUserId)
		{
			// ステータスを「統合確定」、最終更新者をセット
			userIntegration.Status = Constants.FLG_USERINTEGRATION_STATUS_DONE;
			userIntegration.LastChanged = lastChanged;

			// 代表ユーザーID取得
			var representativeUserId = userIntegration.Users.FirstOrDefault(u => u.IsOnRepresentativeFlg).UserId;

			var userService = new UserService();
			var couponService = new CouponService();
			var fixedPurchaseService = new FixedPurchaseService();
			var representativeUserOld = userService.Get(representativeUserId);
			var dmShippingHistoryService = new DmShippingHistoryService();

			// 表示ユーザーは定期会員になるかどうかの判定
			var isNormalMember =
				(representativeUserOld.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF);
			// ユーザー統合対象に関する生きている定期注文情報が存在すれば、定期会員になる判定
			var becomeFixedPurchaseMember = false;
			foreach (var user in userIntegration.Users)
			{
				becomeFixedPurchaseMember = fixedPurchaseService.HasActiveFixedPurchaseInfo(user.UserId);
				if (becomeFixedPurchaseMember) break;
			}

			// データ移行しない分購入回数
			var orderCountOld = 0;
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 未登録の場合は登録
				if (register)
				{
					this.Insert(userIntegration, accessor);
				}

				// 非代表ユーザーでループ
				foreach (var user in userIntegration.Users.Where(u => u.IsOnRepresentativeFlg == false))
				{
					var userModel = userService.Get(user.UserId, accessor);
					var histores = new List<UserIntegrationHistoryModel>();
					orderCountOld += userModel.OrderCountOld;
					var orderService = new OrderService();

					// クレジットカード情報：代表ユーザーのユーザーIDに更新
					// 注文情報:クレジット枝番を更新
					// 定期情報;クレジット枝番を更新
					var creditService = new UserCreditCardService();
					var orders = orderService.GetOrdersByUserId(user.UserId, accessor);
					var fixedPurcahses = fixedPurchaseService.GetFixedPurchasesByUserId(user.UserId, accessor);
					foreach (var card in creditService.GetByUserId(user.UserId).OrderBy(card => card.BranchNo))
					{
						var updateOrders = orders.Where(o => (o.CreditBranchNo == card.BranchNo)).ToArray();
						var updateFixedPurchases = fixedPurcahses.Where(f => (f.CreditBranchNo == card.BranchNo)).ToArray();
						if ((card.CooperationType == Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_CREDITCARD) && isZeus)
						{
							card.CooperationId2 = string.IsNullOrEmpty(card.CooperationId2)
								? new UserCreditCardService().GetCooperationId2ForZeus(card.UserId, card.BranchNo)
								: card.CooperationId2;
						}

						card.UserId = representativeUserId;
						var oldBranchNo = card.BranchNo.ToString();
						var newBranchNo = creditService.Insert(card, UpdateHistoryAction.DoNotInsert, accessor);
						histores.Add(
							CreateHistoryModel(
								userIntegration,
								user.UserId,
								histores.Count,
								Constants.TABLE_USERCREDITCARD,
								new string[]
								{
									oldBranchNo,
									newBranchNo.ToString()
								}));

						updateOrders.ToList().ForEach(
							uo => orderService.UpdateCreditBranchNo(
								uo.OrderId,
								newBranchNo,
								lastChanged,
								UpdateHistoryAction.DoNotInsert,
								accessor));

						updateFixedPurchases.ToList().ForEach(
							uf => fixedPurchaseService.UpdateOrderPayment(
								uf.FixedPurchaseId,
								uf.OrderPaymentKbn,
								newBranchNo,
								uf.CardInstallmentsCode,
								uf.ExternalPaymentAgreementId,
								lastChanged,
								UpdateHistoryAction.DoNotInsert,
								accessor));
					}

					// ユーザークーポン情報統合
					var userCoupons = couponService.GetUserCoupons(
						Constants.CONST_DEFAULT_DEPT_ID,
						user.UserId,
						accessor);
					var userCouponHistoryList = couponService.GetHistoiresAll(
						user.UserId, 
						accessor);
					foreach (var userCoupon in userCoupons)
					{
						var couponNoOld = userCoupon.CouponNo;
						var couponNoNew = couponService.GetUserCouponNo(
							Constants.CONST_DEFAULT_DEPT_ID,
							representativeUserId,
							userCoupon.CouponId,
							accessor);

						new OrderService().UpdateCouponForIntegration(
							user.UserId,
							userCoupon.CouponId,
							couponNoNew.ToString(),
							couponNoOld.ToString(),
							lastChanged,
							accessor);

						var fixedPurhcaseUseUserCouponList = fixedPurcahses
							.Where(fixedPurcahse => 
								((fixedPurcahse.NextShippingUseCouponId == userCoupon.CouponId)
									&& (fixedPurcahse.NextShippingUseCouponNo == userCoupon.CouponNo)));
						foreach (var fixedPurchase in fixedPurhcaseUseUserCouponList)
						{
							new FixedPurchaseService().UpdateNextShippingUseCoupon(
								fixedPurchase.FixedPurchaseId,
								fixedPurchase.NextShippingUseCouponId,
								couponNoNew,
								lastChanged,
								UpdateHistoryAction.DoNotInsert,
								accessor);
						}

						couponService.UpdateUserCouponForIntegration(
							representativeUserId,
							user.UserId,
							userCoupon.CouponId,
							couponNoNew.ToString(),
							couponNoOld.ToString(),
							accessor);
						histores.Add(
							CreateHistoryModel(
								userIntegration,
								user.UserId,
								histores.Count,
								Constants.TABLE_USERCOUPON,
								new string[] 
								{
									userCoupon.CouponId,
									couponNoNew.ToString(),
									couponNoOld.ToString(),
									userCoupon.CouponCode
								}));
					}

					// ブラックリストクーポン情報統合
					var judgeValueRepresentative = isBlackListCouponJudgedByUserId
						? representativeUserId
						: representativeUserOld.MailAddr;
					var judgeValue = isBlackListCouponJudgedByUserId
						? user.UserId
						: userModel.MailAddr;
					var couponUseUserListRepresentative = couponService
						.GetCouponUseUserByCouponUseUser(
							judgeValueRepresentative,
							accessor);
					var couponUseUserList = couponService
						.GetCouponUseUserByCouponUseUser(
							judgeValue,
							accessor);
					var couponUseUserListNeedUpdate = couponUseUserList
						.Where(couponUseUser => (couponUseUserListRepresentative
							.Any(couponUseUserRepresentative => 
								couponUseUserRepresentative.CouponUseUser == couponUseUser.CouponUseUser) == false));
					foreach (var couponUseUser in couponUseUserListNeedUpdate)
					{
						if (couponService.CheckUsedCoupon(
							couponUseUser.CouponId,
							couponUseUser.CouponUseUser)) continue;

						if (couponUseUser.LastChanged != userIntegration.LastChanged)
						{
							couponUseUser.LastChanged = userIntegration.LastChanged;
						}
						couponUseUser.CouponUseUser = judgeValueRepresentative;
						couponService.InsertCouponUseUser(
							couponUseUser,
							accessor);
						histores.Add(
							CreateHistoryModel(
								userIntegration,
								user.UserId,
								histores.Count,
								Constants.TABLE_COUPONUSEUSER,
								new string[]
								{
									couponUseUser.CouponId
								}));
					}

					// クーポン履歴情報統合
					foreach (var userCouponHistory in userCouponHistoryList)
					{
						var historyNo = couponService.GetNextHistoryNo(representativeUserId, accessor);
						couponService.UpdateUserCouponHistoryForIntegration(
							representativeUserId,
							user.UserId,
							historyNo.ToString(),
							userCouponHistory.HistoryNo.ToString(),
							accessor);
						histores.Add(
							CreateHistoryModel(
								userIntegration,
								user.UserId,
								histores.Count,
								Constants.TABLE_USERCOUPONHISTORY,
								new string[]
								{
									historyNo.ToString(),
									userCouponHistory.HistoryNo.ToString()
								}));
					}

					// DM発送履歴の統合
					var dmShippingHistories = dmShippingHistoryService.GetDmShippingHistoriesByUserIdForUserIntegration(user.UserId, accessor);
					var dmRepresentativeUserShippingHistories = dmShippingHistoryService.GetDmShippingHistoriesByUserIdForUserIntegration(representativeUserId, accessor);
					foreach (var dmHistory in dmShippingHistories)
					{
						if (dmRepresentativeUserShippingHistories.Any(history => (history.DmCode == dmHistory.DmCode))) continue;

						dmHistory.RepresentativeUserId = representativeUserId;
						dmShippingHistoryService.UpdateForUserIntegration(dmHistory, accessor);
						histores.Add(
							CreateHistoryModel(
								userIntegration,
								user.UserId,
								histores.Count,
								Constants.TABLE_DMSHIPPINGHISTORY,
								new string[]
								{
									dmHistory.UserId,
									dmHistory.DmCode,
								}));
					}

					// 注文情報：代表ユーザーのユーザーIDに更新
					foreach (var order in orderService.GetOrdersByUserId(user.UserId, accessor))
					{
						orderService.UpdateUserId(order.OrderId, representativeUserId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);
						// 元注文の場合ユーザー統合履歴に残す
						if (order.IsOriginalOrder)
						{
							histores.Add(
								CreateHistoryModel(userIntegration, user.UserId, histores.Count, Constants.TABLE_ORDER, new string[] { order.OrderId }));
						}

						// 更新履歴用に注文情報＆非代表のカード情報取得
						var orderForUpdateHistory = new OrderService().Get(order.OrderId, accessor);
						UserCreditCardModel userCreditCard = null;
						if (orderForUpdateHistory.UseUserCreditCard)
						{
							userCreditCard = new UserCreditCardService().Get(user.UserId, order.CreditBranchNo.Value, accessor);
						}

						// 更新履歴登録
						new UpdateHistoryService().InsertForOrder(orderForUpdateHistory, userCreditCard, lastChanged, null, accessor);
						new UpdateHistoryService().InsertForOrder(orderForUpdateHistory, userCreditCard, lastChanged, user.UserId, accessor);
					}

					// ユーザーポイント履歴情報統合
					var pointService = new PointService();
					var userPointHistories = pointService.GetUserPointHistories(user.UserId, accessor);
					var representativeUserHistoryNo = pointService.GetNextHistoryNo(representativeUserId, accessor);
					var representativeUserHistoryGroupNo = pointService.IssueHistoryGroupNoForUser(representativeUserId, accessor);
					// ポイント履歴グループ番号は1から採番
					var checkHistoryGroupNo = userPointHistories.Any() ? userPointHistories.First().HistoryGroupNo : 1;
					foreach (var userPointHistory in userPointHistories)
					{
						// 非代表ユーザーの履歴グループ番号が異なる場合に履歴グループ番号を更新
						if (checkHistoryGroupNo != userPointHistory.HistoryGroupNo)
						{
							representativeUserHistoryGroupNo++;
							checkHistoryGroupNo = userPointHistory.HistoryGroupNo;
						}

						pointService.UpdateUserPointHistoryForIntegration(
							representativeUserId,
							user.UserId,
							representativeUserHistoryNo,
							userPointHistory.HistoryNo,
							representativeUserHistoryGroupNo,
							accessor);
						histores.Add(
							CreateHistoryModel(
								userIntegration,
								user.UserId,
								histores.Count,
								Constants.TABLE_USERPOINTHISTORY,
								new string[]
								{
									representativeUserHistoryNo.ToString(),
									userPointHistory.HistoryGroupNo.ToString()
								}));

						representativeUserHistoryNo++;
					}

					// ユーザーポイント情報：代表ユーザーにポイントを移行
					var userPoints = pointService.GetUserPoint(user.UserId, string.Empty, accessor);
					foreach (var userPoint in userPoints)
					{
						var contents = new PointIntegrationContents()
						{
							RepresentativeUserId = representativeUserId,
							TargetUserPoint = userPoint,
							OperatorName = lastChanged
						};
						var historyNo = pointService.ExecutePointIntegration(contents, UpdateHistoryAction.DoNotInsert, lastChanged, accessor);
						histores.Add(
							CreateHistoryModel(userIntegration, user.UserId, histores.Count, Constants.TABLE_USERPOINT, new string[] { userPoint.UserId, historyNo.ToString() }));
					}

					// 定期購入情報：代表ユーザーのユーザーIDに更新
					foreach (var fixedPurchase in fixedPurchaseService.GetFixedPurchasesByUserId(user.UserId, accessor))
					{
						fixedPurchaseService.UpdateUserIdForIntegration(fixedPurchase.FixedPurchaseId, representativeUserId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);
						histores.Add(
							CreateHistoryModel(userIntegration, user.UserId, histores.Count, Constants.TABLE_FIXEDPURCHASE, new string[] { fixedPurchase.FixedPurchaseId }));

						// 更新履歴登録
						new UpdateHistoryService().InsertForFixedPurchase(fixedPurchase.FixedPurchaseId, lastChanged, null, accessor);
						new UpdateHistoryService().InsertForFixedPurchase(fixedPurchase.FixedPurchaseId, lastChanged, user.UserId, accessor);
					}

					// 定期購入継続分析情報：代表ユーザーのユーザーIDに更新
					var repeatAnalysisService = new FixedPurchaseRepeatAnalysisService();
					foreach (var repeatAnalysis in repeatAnalysisService.GetRepeatAnalysisByUserId(user.UserId, accessor))
					{
						repeatAnalysis.UserId = representativeUserId;
						repeatAnalysisService.Update(repeatAnalysis, accessor);
						histores.Add(
							CreateHistoryModel(
								userIntegration,
								user.UserId,
								histores.Count,
								Constants.TABLE_FIXEDPURCHASEREPEATANALYSIS,
								new string[] { repeatAnalysis.DataNo.ToString() }));
					}

					// インシデント：代表ユーザーのユーザーIDに更新
					var csIncidenService = new CsIncidentService();
					foreach (var csIncident in csIncidenService.GetCsIncidentByUserId(deptId, user.UserId, accessor))
					{
						csIncidenService.UpdateUserId(csIncident.DeptId, csIncident.IncidentId, representativeUserId, lastChanged, accessor);
						histores.Add(
							CreateHistoryModel(userIntegration, user.UserId, histores.Count, Constants.TABLE_CSINCIDENT, new string[] { csIncident.DeptId, csIncident.IncidentId }));
					}

					// メール送信ログ：代表ユーザーのユーザーIDに更新
					var mailSendLogService = new MailSendLogService();
					foreach (var mailSendLog in mailSendLogService.GetMailSendLogByUserId(user.UserId, accessor))
					{
						mailSendLogService.UpdateUserId(mailSendLog.LogNo.Value, representativeUserId, accessor);
						histores.Add(
							CreateHistoryModel(userIntegration, user.UserId, histores.Count, Constants.TABLE_MAILSENDLOG, new string[] { mailSendLog.LogNo.ToString() }));
					}

					// ユーザー拡張項目のシステム利用項目を除去
					var userExtend = userService.GetUserExtend(user.UserId);
					foreach (var systemUsedItem in systemUserItems)
					{
						if (userExtend.UserExtendColumns.Contains(systemUsedItem))
						{
							userExtend.UserExtendDataValue[systemUsedItem] = string.Empty;
						}
					}
					userService.UpdateUserExtend(userExtend, user.UserId, lastChanged, UpdateHistoryAction.DoNotInsert);

					// ユーザーアクティビティ:代表ユーザーのユーザーIDに更新
					var userActivities = userService.GetUserActivityByUserId(user.UserId, accessor);
					foreach (var userActivity in userActivities)
					{
						var representativeUserActivities = userService.GetUserActivityByUserId(representativeUserId, accessor);
						if (representativeUserActivities.Any(
							rua => (rua.MasterKbn == userActivity.MasterKbn) && (rua.MasterId == userActivity.MasterId)) == false)
						{
							userService.DeleteUserActivity(userActivity.UserId, userActivity.MasterKbn, userActivity.MasterId, accessor);
							userActivity.UserId = representativeUserId;
							userService.InsertUserActivity(userActivity, accessor);
							histores.Add(
								CreateHistoryModel(userIntegration, user.UserId, histores.Count, Constants.TABLE_USERACTIVITY, new string[] { userActivity.MasterKbn, userActivity.MasterId }));
						}
					}

					// ユーザー情報：ユーザー統合フラグを「統合済み」に更新
					userService.UpdateOnIntegratedFlg(user.UserId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);

					// 更新履歴登録（非代表ユーザー）
					new UpdateHistoryService().InsertForUser(user.UserId, lastChanged, accessor);

					// ユーザー統合履歴をセット
					user.Histories = histores.ToArray();
				}

				// 代表ユーザーは通常会員で、生きている定期注文情報が存在する場合、定期会員フラグをONに更新する
				if (isNormalMember && becomeFixedPurchaseMember)
				{
					userService.UpdateFixedPurchaseMemberFlg(
						representativeUserId,
						Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}

				var orderCountOldRepresent = userService.Get(representativeUserId, accessor).OrderCountOld;
				// ユーザーの受注情報の累計購入回数を振り直しとユーザーリアルタイム累計購入回数更新
				UpdateOrderCountByCancelConfirmUserIntegration(representativeUserId, orderCountOldRepresent + orderCountOld, accessor);

				// 最終誕生日クーポン発行年更新
				userService.UpdateUserBirthdayCouponPublishYearByCouponInfo(representativeUserId, lastChanged, accessor);

				// 更新履歴登録（代表ユーザー）
				new UpdateHistoryService().InsertForUser(representativeUserId, lastChanged, accessor);

				// 更新（統合履歴がセットされるため、必ず更新する）
				this.Update(userIntegration, accessor);
				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// ステータスを「除外」に更新
		/// </summary>
		/// <param name="userIntegration">ユーザー統合モデル</param>
		/// <param name="register">登録するか？</param>
		/// <param name="lastChanged">最終更新者</param>
		public void	ExcludedUserIntegration(UserIntegrationModel userIntegration, bool register, string lastChanged)
		{
			// ステータスを「除外」、最終更新者をセット
			userIntegration.Status = Constants.FLG_USERINTEGRATION_STATUS_EXCLUDED;
			userIntegration.LastChanged = lastChanged;

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				// 未登録の場合は登録
				if (register)
				{
					this.Insert(userIntegration, accessor);
				}
				// 更新
				else
				{
					this.Update(userIntegration, accessor);
				}
				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// 履歴モデル作成
		/// </summary>
		/// <param name="model">ユーザー統合モデル</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">枝番</param>
		/// <param name="tableName">テーブル名</param>
		/// <param name="primaryKeys">主キー1～5</param>
		/// <returns>履歴モデル</returns>
		private UserIntegrationHistoryModel CreateHistoryModel(UserIntegrationModel model, string userId, int branchNo, string tableName, string[] primaryKeys)
		{
			return new UserIntegrationHistoryModel
			{
				UserIntegrationNo = model.UserIntegrationNo,
				UserId = userId,
				BranchNo = branchNo,
				TableName = tableName,
				PrimaryKey1 = (primaryKeys.Length >= 1) ? primaryKeys[0] : "",
				PrimaryKey2 = (primaryKeys.Length >= 2) ? primaryKeys[1] : "",
				PrimaryKey3 = (primaryKeys.Length >= 3) ? primaryKeys[2] : "",
				PrimaryKey4 = (primaryKeys.Length >= 4) ? primaryKeys[3] : "",
				PrimaryKey5 = (primaryKeys.Length >= 5) ? primaryKeys[4] : "",
				LastChanged = model.LastChanged,
			};
		}
		#endregion

		#region +RegisterUserIntegrationAfterDuplicateIdentification 名寄せを行いユーザー統合情報の登録・更新・削除を行う
		/// <summary>
		/// 名寄せを行いユーザー統合情報の登録・更新・削除を行う
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <param name="parallelWorkerThreads">並列処理ワーカースレッド数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>実行結果メッセージ</returns>
		public string RegisterUserIntegrationAfterDuplicateIdentification(DateTime targetStart, DateTime targetEnd, int parallelWorkerThreads, string lastChanged)
		{
			var userIntegrationRegister = new UserIntegrationRegister();
			return userIntegrationRegister.RegisterUserIntegrationAfterDuplicateIdentification(targetStart, targetEnd, parallelWorkerThreads, lastChanged);
		}
		#endregion

		#region +GetStatusCount ユーザー統合ステータス件数取得
		/// <summary>
		/// 検索
		/// </summary>
		/// <returns>検索結果列</returns>
		public UserIntegrationStatusCount[] GetStatusCount()
		{
			using (var repository = new UserIntegrationRepository())
			{
				var results = repository.GetStatusCount();
				return results;
			}
		}
		#endregion

		#region +UpdateOrderCountByCancelConfirmUserIntegration ユーザー統合・解除により購入回数更新処理
		/// <summary>
		/// 累計購入回数を振り直し
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderCountOld">データ移行しない分</param>
		/// <param name="accessor">アクセサー</param>
		public void UpdateOrderCountByCancelConfirmUserIntegration(string userId, int orderCountOld = 0, SqlAccessor accessor = null)
		{
			// データ移行しない分を取得
			orderCountOld = ((orderCountOld == 0) ? new UserService().Get(userId, accessor).OrderCountOld : orderCountOld);
			// ユーザーの受注情報の累計購入回数を振り直し
			new OrderService().UpdateOrderCountByUserId(userId, orderCountOld, accessor);
			// ユーザーリアルタイム累計購入回数更新
			var orderCount = new OrderService().GetUncancelledOrdersByUserId(userId, accessor).Where(o => o.IsNotReturnExchangeOrder).ToArray().Count();
			new UserService().UpdateRealTimeOrderCount(userId, (orderCount + orderCountOld), accessor);
		}
		#endregion

		#region +GetBeforeIntegrationUserByOrderId 注文IDより統合前のユーザ情報を取得
		/// <summary>
		/// 注文IDより統合前のユーザ情報を取得
		/// </summary>
		/// <param name="primaryKey1">注文ID</param>
		/// <returns>ユーザモデル</returns>
		public UserModel GetBeforeIntegrationUserByOrderId(string primaryKey1)
		{
			using (var repository = new UserIntegrationRepository())
			{
				var model = repository.GetBeforeIntegrationUserByOrderId(primaryKey1);
				return model;
			}
		}
		#endregion
	}
}
