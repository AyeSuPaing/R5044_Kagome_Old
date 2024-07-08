/*
=========================================================================================================
  Module      : ポイント操作のヘルパクラス (PointOperation.cs)
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
	/// ポイント操作のヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class PointOperation
	{
		#region ~Execute オペレータによるポイント調整を実施
		/// <summary>
		/// オペレータによるポイント調整を実施
		/// </summary>
		/// <param name="operation">ポイント操作内容</param>
		/// <param name="service">ポイントサービスクラス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>
		/// 操作件数
		/// 取得⇒更新を行うため同時実行の楽観ロックを入れており
		/// 取得後に更新があった場合は0を返す
		/// </returns>
		internal int Execute(
			PointOperationContents operation,
			PointService service,
			UpdateHistoryAction updateHistoryAction)
		{
			// ポイントマスタ
			var master = service.GetPointMaster().FirstOrDefault(m => (m.PointKbn == operation.PointKbn));
			if (master == null) throw new Exception("ポイントマスタの取得に失敗したためポイント調整は行えませんでした。");

			var shouldUpdate = false;
			var userPointModel = new UserPointModel();
			if (operation.PointKbnNo != 0)
			{
				// ユーザーのポイントを取得
				userPointModel = service.GetUserPoint(operation.UserId, string.Empty)
					.FirstOrDefault(p => (p.PointKbnNo == operation.PointKbnNo));

				// UPDATEするかINSERTするかを決めておく
				shouldUpdate = (userPointModel != null);
			}

			if (shouldUpdate)
			{
				// UPDATE用
				userPointModel.Point += operation.AddPoint;
				userPointModel.LastChanged = operation.OperatorName;
				userPointModel.PointExp = (userPointModel.PointExp.HasValue && master.IsValidPointExpKbn)
					? userPointModel.PointExp.Value.AddMonths(operation.AddExpMonths).AddDays(operation.AddExpDays)
					: (DateTime?)null;
			}
			else
			{
				// INSERT用
				userPointModel = new UserPointModel
				{
					DeptId = operation.DeptId,
					UserId = operation.UserId,
					PointKbn = operation.PointKbn,
					PointKbnNo = (userPointModel != null) ? service.IssuePointKbnNoForUser(operation.UserId) : 1,
					PointRuleId = string.Empty,
					PointRuleKbn = string.Empty,
					PointType = Constants.FLG_USERPOINT_POINT_TYPE_COMP,
					PointIncKbn = string.Empty,
					Point = operation.AddPoint,
					// １年後 ＋ オペレーターによるの指定月と指定日
					PointExp = master.IsValidPointExpKbn
						? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, 997)
								.AddYears(1)
								.AddMonths(operation.AddExpMonths)
								.AddDays(operation.AddExpDays)
						: (DateTime?)null,
					Kbn1 = string.Empty,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					LastChanged = operation.OperatorName,
					DateChanged = DateTime.Now,
					EffectiveDate = null
				};
			}

			// 履歴作成
			var history = new UserPointHistoryModel
			{
				UserId = userPointModel.UserId,
				DeptId = userPointModel.DeptId,
				PointRuleId = userPointModel.PointRuleId,
				PointRuleKbn = userPointModel.PointRuleKbn,
				PointKbn = userPointModel.PointKbn,
				PointType = userPointModel.PointType,
				PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_OPERATOR,
				PointInc = operation.AddPoint,
				// ポイント有効期限延長は+00XXXXの形にする（月、日のみ指定）
				PointExpExtend = UserPointHistoryModel.GetPointExpExtendFormtString(0, operation.AddExpMonths, operation.AddExpDays),
				UserPointExp = userPointModel.PointExp,
				Kbn1 = userPointModel.Kbn1,
				Kbn2 = userPointModel.Kbn2,
				Kbn3 = userPointModel.Kbn3,
				Kbn4 = userPointModel.Kbn4,
				Kbn5 = userPointModel.Kbn5,
				Memo = string.Empty,
				LastChanged = userPointModel.LastChanged,
				EffectiveDate = null
			};

			var updated = 0;
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				// 物理削除を行う条件
				var shouldDelete = (userPointModel.IsLimitedTermPoint && (userPointModel.Point == 0));

				accessor.BeginTransaction();
				// INSERT / UPDATE / DELETE
				updated += shouldUpdate
					? (shouldDelete)
						? service.DeleteUserPoint(userPointModel, UpdateHistoryAction.DoNotInsert, accessor)
						: service.UpdateUserPoint(userPointModel, UpdateHistoryAction.DoNotInsert, accessor)
					: service.RegisterUserPoint(userPointModel, UpdateHistoryAction.DoNotInsert, accessor)
						? 1
						: 0; // なんでこのメソッドだけboolなんだ…

				// 楽観ロックによる更新失敗
				if (updated == 0)
				{
					accessor.RollbackTransaction();
					return updated;
				}

				// 履歴登録
				service.RegisterHistory(history);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userPointModel.UserId, operation.OperatorName, accessor);
				}
				accessor.CommitTransaction();
			}

			return updated;
		}
		#endregion
	}

	/// <summary>
	/// ポイント操作内容
	/// </summary>
	[Serializable]
	public class PointOperationContents
	{
		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId { get; set; }
		/// <summary>ポイント操作対象のユーザーID</summary>
		public string UserId { get; set; }
		/// <summary>ポイント区分</summary>
		public string PointKbn { get; set; }
		/// <summary>ポイント枝番</summary>
		public int PointKbnNo { get; set; }
		/// <summary>ポイント増減数（プラスは加算・マイナスは減算）</summary>
		public Decimal AddPoint { get; set; }
		/// <summary>有効期限増減月（プラスは加算・マイナスは減算）</summary>
		public int AddExpMonths { get; set; }
		/// <summary>有効期限増減日（プラスは加算・マイナスは減算）</summary>
		public int AddExpDays { get; set; }
		/// <summary>実行するオペレーター名（最終更新者）</summary>
		public string OperatorName { get; set; }
		#endregion
	}
}
