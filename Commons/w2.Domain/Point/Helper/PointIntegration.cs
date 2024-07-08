/*
=========================================================================================================
  Module      : ポイント統合のヘルパクラス (PointIntegration.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Point.Helper
{
	/// <summary>
	/// ポイント統合のヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class PointIntegration
	{
		#region ~非代表ユーザーのポイントを代表ユーザーのポイントに統合
		/// <summary>
		/// 非代表ユーザーのポイントを代表ユーザーのポイントに統合
		/// </summary>
		/// <param name="contents">ポイント統合内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザーポイント履歴枝番</returns>
		internal int Execute(
			PointIntegrationContents contents,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var pointService = new PointService();

			// 非代表ユーザーポイント、代表ユーザーポイント取得
			var userPoint = contents.TargetUserPoint;
			var representativeUserPoints = pointService.GetUserPoint(contents.RepresentativeUserId,string.Empty, accessor);

			// ポイントマスタ取得
			var pointMaster = pointService.GetPointMaster().FirstOrDefault(i => i.DeptId == userPoint.DeptId && i.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE);
			if (pointMaster == null) throw new Exception("ポイントマスタの取得に失敗したためユーザー統合は行えませんでした。");

			// 代表ユーザーに本ポイントが存在しない or 非代表ユーザーのポイントが仮ポイント or 期間限定ポイントの場合は登録
			// それ以外は更新を行う
			var register = (((representativeUserPoints.Any(p => p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)) == false)
					|| (userPoint.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP)
					|| userPoint.IsLimitedTermPoint);

			// 代表ユーザーのユーザーポイントモデル作成
			var model = CreateRepresentativeUserPointForExecute(contents, pointMaster, representativeUserPoints, userPoint, register, accessor);

			// 代表ユーザーのポイントを登録・更新
			if (register)
			{
				pointService.RegisterUserPoint(model, UpdateHistoryAction.DoNotInsert, accessor);
			}
			else
			{
				pointService.UpdateUserPoint(model, UpdateHistoryAction.DoNotInsert, accessor);
			}

			// 非代表ユーザーのポイントを削除（履歴も登録）
			pointService.DeleteUserPoint(userPoint, UpdateHistoryAction.DoNotInsert, accessor);
			var historyNo = pointService.RegisterHistory(CreateUserPointHistoryForExecute(contents, userPoint), accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userPoint.UserId, contents.OperatorName, accessor);
				new UpdateHistoryService().InsertForUser(model.UserId, contents.OperatorName, accessor);
			}
			return historyNo;
		}

		/// <summary>
		/// 代表ユーザーのユーザーポイントモデル作成（統合時用）
		/// </summary>
		/// <param name="contents">ポイント統合内容</param>
		/// <param name="pointMaster">ポイント情報</param>
		/// <param name="representativeUserPoints">代表ユーザーポイント</param>
		/// <param name="userPoint">非代表ユーザーポイント</param>
		/// <param name="register">登録 or 更新</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザーポイントモデル</returns>
		private UserPointModel CreateRepresentativeUserPointForExecute(PointIntegrationContents contents, PointModel pointMaster, UserPointModel[] representativeUserPoints, UserPointModel userPoint, bool register, SqlAccessor accessor = null)
		{
			UserPointModel result;

			// 代表ユーザーに本ポイントが存在しない
			// or 非代表ユーザーのポイントが仮ポイントの場合
			// or 期間限定ポイント
			if (register)
			{
				result = new UserPointModel()
				{
					UserId = contents.RepresentativeUserId,
					PointKbn = userPoint.PointKbn,
					PointKbnNo = new PointService().IssuePointKbnNoForUser(contents.RepresentativeUserId, accessor),
					DeptId = userPoint.DeptId,
					PointRuleId = userPoint.PointRuleId,
					PointRuleKbn = userPoint.PointRuleKbn,
					PointType = userPoint.PointType,
					PointIncKbn = userPoint.PointIncKbn,
					Point = userPoint.Point,
					PointExp = userPoint.PointExp,
					Kbn1 = userPoint.Kbn1,
					Kbn2 = userPoint.Kbn2,
					Kbn3 = userPoint.Kbn3,
					Kbn4 = userPoint.Kbn4,
					Kbn5 = userPoint.Kbn5,
					EffectiveDate = userPoint.EffectiveDate,
					LastChanged = contents.OperatorName
				};
			}
			// 代表ユーザーの本ポイントが存在する場合
			else
			{
				// ポイント数（加算）、有効期限、最終更新者をセット
				result = representativeUserPoints.FirstOrDefault(p => p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP);
				result.Point += userPoint.Point;
				if (pointMaster.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID)
				{
					if (result.PointExp.HasValue && userPoint.PointExp.HasValue)
					{
						result.PointExp = (userPoint.PointExp.Value > result.PointExp.Value) ? userPoint.PointExp.Value : result.PointExp.Value;
					}
				}
				else
				{
					result.PointExp = null;
				}
				result.LastChanged = contents.OperatorName;
			}

			return result;
		}

		/// <summary>
		/// 代表ユーザーのユーザーポイント履歴モデル作成（統合時用）
		/// </summary>
		/// <param name="representativeUserPoint">代表ユーザーポイント</param>
		/// <param name="userPoint">非代表ユーザーポイント</param>
		/// <returns>ユーザーポイント履歴モデル</returns>
		private UserPointHistoryModel CreateRepresentativeUserPointHistoryForExecute(UserPointModel representativeUserPoint, UserPointModel userPoint)
		{
			return
				new UserPointHistoryModel
				{
					UserId = representativeUserPoint.UserId,
					DeptId = representativeUserPoint.DeptId,
					PointRuleId = representativeUserPoint.PointRuleId,
					PointRuleKbn = representativeUserPoint.PointRuleKbn,
					PointKbn = representativeUserPoint.PointKbn,
					PointType = representativeUserPoint.PointType,
					PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_USERINTEGRATION,
					PointInc = userPoint.Point,
					// ポイント有効期限延長は+000000の形にする（日指定のみ）
					PointExpExtend = UserPointHistoryModel.GetPointExpExtendFormtString(0, 0, 0),
					UserPointExp = representativeUserPoint.PointExp,
					Kbn1 = representativeUserPoint.Kbn1,
					Kbn2 = representativeUserPoint.Kbn2,
					Kbn3 = representativeUserPoint.Kbn3,
					Kbn4 = representativeUserPoint.Kbn4,
					Kbn5 = representativeUserPoint.Kbn5,
					Memo = "統合のためポイント追加",
					LastChanged = representativeUserPoint.LastChanged,
					EffectiveDate = representativeUserPoint.EffectiveDate
				};
		}

		/// <summary>
		/// 非代表ユーザーのユーザーポイント履歴モデル作成（統合時用）
		/// </summary>
		/// <param name="contents">ポイント統合内容</param>
		/// <param name="userPoint">非代表ユーザーポイント</param>
		/// <returns>ユーザーポイント履歴モデル</returns>
		private UserPointHistoryModel CreateUserPointHistoryForExecute(PointIntegrationContents contents, UserPointModel userPoint)
		{
			return
				new UserPointHistoryModel
				{
					UserId = userPoint.UserId,
					DeptId = userPoint.DeptId,
					PointRuleId = userPoint.PointRuleId,
					PointRuleKbn = userPoint.PointRuleKbn,
					PointKbn = userPoint.PointKbn,
					PointType = userPoint.PointType,
					PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_USERINTEGRATION,
					PointInc = userPoint.Point * -1,
					UserPointExp = userPoint.PointExp,
					Kbn1 = userPoint.Kbn1,
					Kbn2 = userPoint.Kbn2,
					Kbn3 = userPoint.Kbn3,
					Kbn4 = userPoint.Kbn4,
					Kbn5 = userPoint.Kbn5,
					Memo = "統合のためポイント削除",
					LastChanged = contents.OperatorName,
					EffectiveDate = userPoint.EffectiveDate
				};
		}

		#endregion

		#region ~ユーザー統合時に代表ユーザーに統合したポイントを元に戻す
		/// <summary>
		/// ユーザー統合時に代表ユーザーに統合したポイントを元に戻す
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		internal int Cancel(
			PointIntegrationCancelContents contents,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var pointService = new PointService();

			// 非代表ユーザーポイント履歴、代表ユーザーポイント取得
			var userPointHistory = contents.TargetUserPointHistory;
			var representativeUserPoints = pointService.GetUserPoint(contents.RepresentativeUserId, string.Empty, accessor);

			// ポイントマスタ取得
			var pointMaster = pointService.GetPointMaster()
				.FirstOrDefault(i => (i.DeptId == userPointHistory.DeptId) && (i.PointKbn == userPointHistory.PointKbn));
			if (pointMaster == null) throw new Exception("ポイントマスタの取得に失敗したためユーザー統合は行えませんでした。");

			var representativeUserLimitedTermPointDeleted = false;

			// 代表ユーザーポイントの本・仮ポイントを戻す
			if ((userPointHistory.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
			{
				if (userPointHistory.IsBasePoint)
				{
					RollBackRepresentativeUserPoint(contents, representativeUserPoints, userPointHistory, UpdateHistoryAction.DoNotInsert, accessor);
				}
				else if (userPointHistory.IsLimitedTermPoint)
				{
					representativeUserLimitedTermPointDeleted
						= RollBackRepresentativeUserLimitedTermPoint(contents, representativeUserPoints, userPointHistory, UpdateHistoryAction.DoNotInsert, accessor);
				}
			}
			else
			{
				RollBackRepresentativeUserTempPoint(contents, representativeUserPoints, userPointHistory, UpdateHistoryAction.DoNotInsert, accessor);
			}

			// 非代表ユーザーポイントの本・仮ポイントを戻す
			if ((userPointHistory.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
			{
				if (userPointHistory.IsBasePoint)
				{
					RollBackUserPoint(contents, userPointHistory, UpdateHistoryAction.DoNotInsert, accessor);
				}
				else if (userPointHistory.IsLimitedTermPoint && representativeUserLimitedTermPointDeleted)
				{
					RollBackUserLimitedTermPoint(contents, userPointHistory, UpdateHistoryAction.DoNotInsert, accessor);
				}
			}
			else
			{
				RollBackUserTempPoint(contents, userPointHistory, UpdateHistoryAction.DoNotInsert, accessor);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(contents.RepresentativeUserId, contents.OperatorName, accessor);
				new UpdateHistoryService().InsertForUser(userPointHistory.UserId, contents.OperatorName, accessor);
			}
			return 1;
		}

		/// <summary>
		/// 代表ユーザーの本ポイントを戻す
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="representativeUserPoints">代表ユーザーポイント</param>
		/// <param name="userPointHistory">非代表ユーザーポイント履歴</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void RollBackRepresentativeUserPoint(
			PointIntegrationCancelContents contents,
			UserPointModel[] representativeUserPoints,
			UserPointHistoryModel userPointHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 代表ユーザーの通常本ポイントが存在する場合は更新
			var register = true;
			UserPointModel model;
			if (representativeUserPoints.Any(p => (p.IsBasePoint && p.IsPointTypeComp)))
			{
				// ポイント数（減算）、有効期限、最終更新者をセット
				model = representativeUserPoints.FirstOrDefault(p => p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP);
				model.Point += userPointHistory.PointInc;
				model.LastChanged = contents.OperatorName;

				register = false;
			}
			// 代表ユーザーの通常本ポイントが存在しない
			// or 期間限定ポイントの場合は登録
			else
			{
				model = new UserPointModel()
				{
					UserId = contents.RepresentativeUserId,
					PointKbn = userPointHistory.PointKbn,
					PointKbnNo = new PointService().IssuePointKbnNoForUser(contents.RepresentativeUserId, accessor),
					DeptId = userPointHistory.DeptId,
					PointRuleId = userPointHistory.PointRuleId,
					PointRuleKbn = userPointHistory.PointRuleKbn,
					PointType = userPointHistory.PointType,
					PointIncKbn = userPointHistory.PointIncKbn,
					Point = userPointHistory.PointInc,
					PointExp = userPointHistory.UserPointExp,
					Kbn1 = userPointHistory.Kbn1,
					Kbn2 = userPointHistory.Kbn2,
					Kbn3 = userPointHistory.Kbn3,
					Kbn4 = userPointHistory.Kbn4,
					Kbn5 = userPointHistory.Kbn5,
					LastChanged = contents.OperatorName,
					EffectiveDate = userPointHistory.EffectiveDate
				};
			}

			var pointService = new PointService();

			// 代表ユーザーのポイントを登録・更新（履歴も登録）
			if (register)
			{
				pointService.RegisterUserPoint(model, UpdateHistoryAction.DoNotInsert, accessor);
			}
			else
			{
				pointService.UpdateUserPoint(model, UpdateHistoryAction.DoNotInsert, accessor);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, model.LastChanged, accessor);
			}
		}

		/// <summary>
		/// （統合解除時）
		/// 代表ユーザーの期間限定ポイントを削除
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="representativeUserPoints">代表ユーザーポイント</param>
		/// <param name="userPointHistory">非代表ユーザーポイント履歴</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除処理が行われたか</returns>
		private bool RollBackRepresentativeUserLimitedTermPoint(
			PointIntegrationCancelContents contents,
			UserPointModel[] representativeUserPoints,
			UserPointHistoryModel userPointHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 代表ユーザーの期間限定ポイントを削除
			var userPoint = representativeUserPoints
				.FirstOrDefault(
					p => (p.PointKbn == userPointHistory.PointKbn)
						&& (p.EffectiveDate == userPointHistory.EffectiveDate
						&& (p.PointExp == userPointHistory.UserPointExp)
						&& (p.Point == (userPointHistory.PointInc * -1))));

			if (userPoint == null) return false;

			var service = new PointService();
			service.DeleteUserPoint(userPoint, UpdateHistoryAction.DoNotInsert, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userPoint.UserId, contents.OperatorName, accessor);
			}

			return true;
		}

		/// <summary>
		/// 代表ユーザーの仮ポイントを戻す
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="representativeUserPoints">代表ユーザーポイント</param>
		/// <param name="userPointHistory">非代表ユーザーポイント履歴</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void RollBackRepresentativeUserTempPoint(
			PointIntegrationCancelContents contents,
			UserPointModel[] representativeUserPoints,
			UserPointHistoryModel userPointHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 仮ポイントが存在する場合は登録
			var userPoints = representativeUserPoints.Where(p => (p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP) && (p.Kbn1 == userPointHistory.Kbn1)).ToArray();
			if (userPoints.Any())
			{
				// 代表ユーザーのポイントを削除（履歴も登録）
				var pointService = new PointService();
				var userPoint = userPoints[0];
				pointService.DeleteUserPoint(userPoint, UpdateHistoryAction.DoNotInsert, accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userPoint.UserId, contents.OperatorName, accessor);
				}
			}
		}

		/// <summary>
		/// 非代表ユーザーの本ポイントを戻す
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="userPointHistory">非代表ユーザーポイント履歴</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void RollBackUserPoint(
			PointIntegrationCancelContents contents,
			UserPointHistoryModel userPointHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 非代表ユーザーの通常本ポイントが存在する場合は更新
			var register = true;
			UserPointModel model;
			var userPoints = new PointService().GetUserPoint(userPointHistory.UserId, string.Empty, accessor);
			if (userPoints.Any(p => (p.IsBasePoint && p.IsPointTypeComp)))
			{
				// ポイント数（加算）、有効期限、最終更新者をセット
				model = userPoints.FirstOrDefault(p => p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP);
				model.Point += (userPointHistory.PointInc*-1);
				model.LastChanged = contents.OperatorName;

				register = false;
			}
			// 非代表ユーザーの通常本ポイントが存在しない場合は登録
			else
			{
				model = new UserPointModel()
				{
					UserId = userPointHistory.UserId,
					PointKbn = userPointHistory.PointKbn,
					PointKbnNo = new PointService().IssuePointKbnNoForUser(userPointHistory.UserId, accessor),
					DeptId = userPointHistory.DeptId,
					PointRuleId = userPointHistory.PointRuleId,
					PointRuleKbn = userPointHistory.PointRuleKbn,
					PointType = userPointHistory.PointType,
					PointIncKbn = userPointHistory.PointIncKbn,
					Point = (userPointHistory.PointInc * -1),
					PointExp = userPointHistory.UserPointExp,
					Kbn1 = userPointHistory.Kbn1,
					Kbn2 = userPointHistory.Kbn2,
					Kbn3 = userPointHistory.Kbn3,
					Kbn4 = userPointHistory.Kbn4,
					Kbn5 = userPointHistory.Kbn5,
					LastChanged = contents.OperatorName,
					EffectiveDate = userPointHistory.EffectiveDate
				};
			}

			var pointService = new PointService();
			// 非代表ユーザーのポイントを登録・更新（履歴も登録）
			if (register)
			{
				pointService.RegisterUserPoint(model, UpdateHistoryAction.DoNotInsert, accessor);
			}
			else
			{
				pointService.UpdateUserPoint(model, UpdateHistoryAction.DoNotInsert, accessor);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, contents.OperatorName, accessor);
			}
		}

		/// <summary>
		/// 非代表ユーザーの仮ポイントを戻す
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="userPointHistory">非代表ユーザーポイント履歴</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void RollBackUserTempPoint(
			PointIntegrationCancelContents contents,
			UserPointHistoryModel userPointHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 非代表ユーザーのポイントを登録（履歴も登録）
			var model = new UserPointModel()
			{
				UserId = userPointHistory.UserId,
				PointKbn = userPointHistory.PointKbn,
				PointKbnNo = new PointService().IssuePointKbnNoForUser(userPointHistory.UserId, accessor),
				DeptId = userPointHistory.DeptId,
				PointRuleId = userPointHistory.PointRuleId,
				PointRuleKbn = userPointHistory.PointRuleKbn,
				PointType = userPointHistory.PointType,
				PointIncKbn = userPointHistory.PointIncKbn,
				Point = (userPointHistory.PointInc * -1),
				PointExp = userPointHistory.UserPointExp,
				Kbn1 = userPointHistory.Kbn1,
				Kbn2 = userPointHistory.Kbn2,
				Kbn3 = userPointHistory.Kbn3,
				Kbn4 = userPointHistory.Kbn4,
				Kbn5 = userPointHistory.Kbn5,
				LastChanged = contents.OperatorName
			};
			var pointService = new PointService();
			pointService.RegisterUserPoint(model, UpdateHistoryAction.DoNotInsert, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, contents.OperatorName, accessor);
			}
		}
		#endregion

		/// <summary>
		/// （統合解除時）
		/// 非代表ユーザーに期間限定ポイントを再付与
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="userPointHistory">非代表ユーザーポイント履歴</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void RollBackUserLimitedTermPoint(
			PointIntegrationCancelContents contents,
			UserPointHistoryModel userPointHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var userPoint = new UserPointModel
			{
				UserId = userPointHistory.UserId,
				PointKbn = userPointHistory.PointKbn,
				PointKbnNo = 1,
				DeptId = userPointHistory.DeptId,
				PointRuleId = userPointHistory.PointRuleId,
				PointRuleKbn = userPointHistory.PointRuleKbn,
				PointType = userPointHistory.PointType,
				PointIncKbn = userPointHistory.PointIncKbn,
				Point = (userPointHistory.PointInc * -1),
				PointExp = userPointHistory.UserPointExp,
				Kbn1 = userPointHistory.Kbn1,
				Kbn2 = userPointHistory.Kbn2,
				Kbn3 = userPointHistory.Kbn3,
				Kbn4 = userPointHistory.Kbn4,
				Kbn5 = userPointHistory.Kbn5,
				LastChanged = contents.OperatorName,
				EffectiveDate = userPointHistory.EffectiveDate
			};

			var service = new PointService();
			service.RegisterUserPoint(userPoint, UpdateHistoryAction.DoNotInsert, accessor);
			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userPoint.UserId, contents.OperatorName, accessor);
			}
		}
	}

	/// <summary>
	/// ポイント統合内容
	/// </summary>
	[Serializable]
	public class PointIntegrationContents
	{
		#region プロパティ
		/// <summary>代表ユーザーID</summary>
		public string RepresentativeUserId { get; set; }
		/// <summary>非代表ユーザーポイント</summary>
		public UserPointModel TargetUserPoint { get; set; }
		/// <summary>実行するオペレーター名（最終更新者）</summary>
		public string OperatorName { get; set; }
		#endregion
	}

	/// <summary>
	/// ポイント統合解除内容
	/// </summary>
	[Serializable]
	public class PointIntegrationCancelContents
	{
		#region プロパティ
		/// <summary>代表ユーザーID</summary>
		public string RepresentativeUserId { get; set; }
		/// <summary>非代表ユーザーポイント履歴</summary>
		public UserPointHistoryModel TargetUserPointHistory { get; set; }
		/// <summary>実行するオペレーター名（最終更新者）</summary>
		public string OperatorName { get; set; }
		#endregion
	}
}