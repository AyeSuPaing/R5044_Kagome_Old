/*
=========================================================================================================
  Module      : ユーザーポイント情報取込設定クラス(ImportSettingUserPoint.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;
using w2.Domain.Point;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingUserPoint : ImportSettingBase
	{
		// 有効期限(日単位で追加)
		private const string USERPOINT_ADD_POINT_EXP_DAY = "add_point_exp_day";
		// 有効期限(月単位で追加)
		private const string USERPOINT_ADD_POINT_EXP_MONTH = "add_point_exp_month";

		// 更新キーフィールド
		private static List<string> FIELDS_UPDKEY = new List<string>
		{
			Constants.FIELD_USERPOINT_USER_ID,
			Constants.FIELD_USERPOINT_POINT_KBN,
			Constants.FIELD_USERPOINT_POINT_TYPE,
			Constants.FIELD_USERPOINT_KBN1
		};
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static List<string> FIELDS_EXCEPT = new List<string>
		{
			Constants.FIELD_USERPOINT_DATE_CHANGED,
			Constants.FIELD_USERPOINT_LAST_CHANGED,
			USERPOINT_ADD_POINT_EXP_DAY,
			USERPOINT_ADD_POINT_EXP_MONTH,
		};
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string>
		{
			"add_" + Constants.FIELD_USERPOINT_POINT
		};
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string>
		{
			Constants.FIELD_USERPOINT_USER_ID,
			Constants.FIELD_USERPOINT_POINT_TYPE,
		};
		// 必須フィールド（Delete用）
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string>
		{
			Constants.FIELD_USERPOINT_USER_ID
		};
		// カラム存在チェック除外フィールド
		private static readonly List<string> FIELDS_EXCLUSION = new List<string>
		{
			USERPOINT_ADD_POINT_EXP_DAY,
			USERPOINT_ADD_POINT_EXP_MONTH,
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingUserPoint(string shopId)
			: base(
				shopId,
				Constants.TABLE_USERPOINT,
				Constants.TABLE_WORKUSERPOINT,
				FIELDS_UPDKEY,
				FIELDS_EXCEPT,
				FIELDS_INCREASED_UPDATE,
				FIELDS_NECESSARY_FOR_INSERTUPDATE,
				FIELDS_NECESSARY_FOR_DELETE)
		{
			// = FIELDS_EXCLUSION;
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			//------------------------------------------------------
			// データ変換
			//------------------------------------------------------
			foreach (string strFieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[strFieldName] = this.Data[strFieldName].ToString().Trim();

				// 半角変換
				switch (strFieldName)
				{
					case Constants.FIELD_USERPOINT_USER_ID:
					case Constants.FIELD_USERPOINT_POINT:
					case Constants.FIELD_USERPOINT_POINT_EXP:
						this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
						break;
				}

				// ポイントの利用開始期限の時分秒を固定（00:00:00.000)に変換する
				if ((strFieldName == Constants.FIELD_USERPOINT_EFFECTIVE_DATE) && (Validator.IsDate(this.Data[strFieldName])))
				{
					this.Data[strFieldName] = DateTime.Parse(this.Data[strFieldName].ToString()).ToString("yyyy/MM/dd 00:00:00.000");
				}

				// ポイントの有効期限の時分秒を固定（23:59:59.997)に変換する
				if ((strFieldName == Constants.FIELD_USERPOINT_POINT_EXP) && (Validator.IsDate(this.Data[strFieldName])))
				{
					this.Data[strFieldName] = DateTime.Parse(this.Data[strFieldName].ToString()).ToString("yyyy/MM/dd 23:59:59.997");
				}
			}

			//------------------------------------------------------
			// 固定値設定
			// ※ヘッダフィールドに結合フィールドが存在しない場合、追加する
			//------------------------------------------------------
			// 識別ID（「0」固定 ※店舗IDから取得）
			this.Data[Constants.FIELD_USERPOINT_DEPT_ID] = this.ShopId;
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_DEPT_ID) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_DEPT_ID);
			}
			// ポイント区分（省略の場合通常ポイント）
			this.Data[Constants.FIELD_USERPOINT_POINT_KBN] = this.Data[Constants.FIELD_USERPOINT_POINT_KBN] ?? Constants.FLG_USERPOINT_POINT_KBN_BASE;
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_KBN) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_POINT_KBN);
			}
			// ポイントルールID
			this.Data[Constants.FIELD_USERPOINT_POINT_RULE_ID] = this.Data[Constants.FIELD_USERPOINT_POINT_RULE_ID] ?? "";
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_RULE_ID) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_POINT_RULE_ID);
			}
			// ポイントルール区分（「」固定）
			this.Data[Constants.FIELD_USERPOINT_POINT_RULE_KBN] = "";
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_RULE_KBN) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_POINT_RULE_KBN);
			}
			// ポイント種別
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_TYPE) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_POINT_TYPE);
			}
			// ポイント加算区分（「97：マスタアップロードポイント調整」固定）
			this.Data[Constants.FIELD_USERPOINT_POINT_INC_KBN] = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_OPERATOR_MASTER_UPLOAD;
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_INC_KBN) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_POINT_INC_KBN);
			}
			// 予備区分1(注文ID)
			this.Data[Constants.FIELD_USERPOINT_KBN1] = this.Data[Constants.FIELD_USERPOINT_KBN1] ?? "";
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_KBN1) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_KBN1);
			}

			//------------------------------------------------------
			// ※ユーザーポイント履歴用
			//------------------------------------------------------
			// 枝番（insert時に自動で採番されるため、ここではデフォルトで1を入れる）
			this.Data[Constants.FIELD_USERPOINTHISTORY_HISTORY_NO] = "1";
			// ポイント加算数
			// addの場合そのままでOK、add_じゃない場合は現在のポイントとの差分
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_ADD_POINT))
			{
				this.Data[Constants.FIELD_USERPOINTHISTORY_POINT_INC] = this.Data[Constants.FIELD_USERPOINT_ADD_POINT];
			}
			else if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT))
			{
				this.Data[Constants.FIELD_USERPOINTHISTORY_POINT_INC] = CalcPointInc();
			}

			if ((string.IsNullOrEmpty((string)this.Data[Constants.FIELD_USERPOINT_POINT_EXP]) == false)
				&& ((string.IsNullOrEmpty((string)this.Data[USERPOINT_ADD_POINT_EXP_MONTH]) == false)
					|| (string.IsNullOrEmpty((string)this.Data[USERPOINT_ADD_POINT_EXP_DAY]) == false)))
			{
				this.ErrorMessages = "「有効期限」と「有効期限(日単位で追加) or 有効期限(月単位で追加)」が入力されています。どちらか一つを入力してください。";
			}

			if (string.IsNullOrEmpty((string)this.Data[Constants.FIELD_USERPOINT_POINT_EXP]))
			{
				this.Data[Constants.FIELD_USERPOINT_POINT_EXP] = CalcPointExp();
			}
			// ユーザ最新有効期限（＝ユーザーポイント情報の有効期限）
			this.Data[Constants.FIELD_USERPOINTHISTORY_USER_POINT_EXP] = this.Data[Constants.FIELD_USERPOINT_POINT_EXP];
			// メモ（「マスタアップロード」固定）
			this.Data[Constants.FIELD_USERPOINTHISTORY_MEMO] = "マスタアップロード";
		}

		/// <summary>
		/// 有効期間増減数の計算
		/// </summary>
		/// <returns>計算済み日付文字列</returns>
		private string CalcPointExp()
		{
			var addExpMonths = 0;
			var addExpDays = 0;
			int.TryParse((string)this.Data[USERPOINT_ADD_POINT_EXP_MONTH], out addExpMonths);
			int.TryParse((string)this.Data[USERPOINT_ADD_POINT_EXP_DAY], out addExpDays);

			var master = new PointService().GetPointMaster().FirstOrDefault(i =>
				(i.DeptId == (string)this.Data[Constants.FIELD_USERPOINT_DEPT_ID])
				&& (i.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE));
			if (master == null) this.ErrorMessages = "ポイントマスタの取得に失敗したためポイント調整は行えませんでした。";

			DateTime? pointExp;
			var userPoints = new PointService().GetUserPoint((string)this.Data[Constants.FIELD_USERPOINT_USER_ID], string.Empty);

			if ((userPoints == null) || (userPoints.Sum(r => r.Point) == 0)
				&& (addExpMonths == 0)
				&& (addExpDays == 0))
			{
				pointExp = new DateTime(
					DateTime.Now.Year,
					DateTime.Now.Month,
					DateTime.Now.Day,
					23,
					59,
					59,
					997).AddMonths(Constants.CALC_POINT_EXP_ADDMONTH);
			}
			else if ((userPoints == null)
				|| userPoints.Any(p => p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP) == false)
			{
				// 本ポイントがない場合は一年後+指定月+指定日時
				pointExp = master.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID
					? (DateTime?)new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, 997)
						.AddYears(1)
						.AddMonths(addExpMonths)
						.AddDays(addExpDays)
					: null;
			}
			else
			{
				pointExp = userPoints.FirstOrDefault(p => p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP).PointExp;
				// 有効期限延長（月、日の順）
				pointExp = master.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID
					? pointExp.HasValue
						? (DateTime?)pointExp.Value.AddMonths(addExpMonths).AddDays(addExpDays) 
						: null
					: null;
			}
			return StringUtility.ToEmpty(pointExp);
		}

		/// <summary>
		/// ポイント増減数の計算
		/// </summary>
		/// <returns>ポイント増減数</returns>
		/// <remarks>期間限定ポイントは常にINSERTを行うため、通常ポイント数は考慮しない</remarks>
		private string CalcPointInc()
		{
			// Remarksの通り
			if (this.IsLimitedTermPoint)
			{
				return (string)this.Data[Constants.FIELD_USERPOINT_POINT];
			}

			var userPointModels = new PointService().GetUserPoint((string)this.Data[Constants.FIELD_USERPOINT_USER_ID], string.Empty)
				.Where(up => up.IsBasePoint)
				.ToArray();

			var point = 0m;
			var pointType = (string)this.Data[Constants.FIELD_USERPOINT_POINT_TYPE];
			if (pointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
			{
				point = userPointModels
					.Where(userPointModel => (userPointModel.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
					.Sum(userPointModel => userPointModel.Point);
			}
			else if (pointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP)
			{
				point = userPointModels
					.Where(userPointModel => (userPointModel.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP)
						&& (userPointModel.Kbn1 == this.Data[Constants.FIELD_USERPOINT_KBN1].ToString()))
					.Sum(userPointModel => userPointModel.Point);
			}
			if (point == 0)
			{
				return this.Data[Constants.FIELD_USERPOINT_POINT].ToString();
			}
			else
			{
				var diff = (Convert.ToInt32(this.Data[Constants.FIELD_USERPOINT_POINT]) - point).ToString();
				return diff;
			}
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			//------------------------------------------------------
			// 入力チェック
			//------------------------------------------------------
			string strCheckKbn = null;
			List<string> lNecessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					strCheckKbn = "UserPointInsertUpdate";
					lNecessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					strCheckKbn = "UserPointDelete";
					lNecessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			StringBuilder sbErrorMessages = new StringBuilder();
			StringBuilder sbNecessaryFields = new StringBuilder();
			foreach (string strKeyField in lNecessaryFields)
			{
				if (this.HeadersCsv.Contains(strKeyField) == false)
				{
					sbNecessaryFields.Append((sbNecessaryFields.Length != 0) ? "," : "");
					sbNecessaryFields.Append(strKeyField);
				}
			}
			if (sbNecessaryFields.Length != 0)
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "");
				sbErrorMessages.Append("該当テーブルの更新にはフィールド「").Append(sbNecessaryFields.ToString()).Append("」が必須です。");
			}

			// 入力チェック
			string errorMessage = Validator.Validate(strCheckKbn, this.Data);
			this.ErrorOccurredIdInfo = "";
			if (errorMessage != "")
			{
				sbErrorMessages.Append((sbErrorMessages.Length != 0) ? "\r\n" : "").Append(errorMessage);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_USERPOINT_USER_ID);
			}

			// エラーメッセージ格納
			if (sbErrorMessages.Length != 0)
			{
				this.ErrorMessages = sbErrorMessages.ToString();
			}
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			// ユーザー存在チェック
			var errorMessage = CheckUserExist();

			// 受注存在チェック
			if (this.IsPointTypeTemp)
			{
				errorMessage = CheckOrderExist(errorMessage);
			}

			// ポイントルールチェック
			// 発行理由となるポイントルールを設定しておかないと本ポイント移行時に有効期限を計算できない
			// (ポイントルールを取得できずにぬるりで落ちる。物理削除を行わない設計のため無いなんてことは想定していない。)
			if (this.IsPointTypeTemp && this.IsLimitedTermPoint)
			{
				errorMessage = CheckPointRuleValidity(errorMessage);
			}

			// 有効期間などの整合性チェック
			errorMessage = CheckUserPointPeriodValididy(errorMessage);

			return errorMessage;
		}

		/// <summary>
		/// ユーザー存在チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckUserExist()
		{
			var userModel = new UserService().Get(this.Data[Constants.FIELD_USERPOINT_USER_ID].ToString());
			if (userModel == null)
			{
				return "ユーザ情報:" + this.Data[Constants.FIELD_USERPOINT_USER_ID] + " が見つかりませんでした";
			}
			return "";
		}

		/// <summary>
		/// 受注存在チェック(仮ポイントの場合のみ行う)
		/// ・注文IDが入力されているかチェックする。
		/// ・注文IDに紐づく注文情報が存在するか、キャンセルされていないかチェックする。
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckOrderExist(string errorMessage)
		{
			// 必須チェック
			if (string.IsNullOrEmpty(this.Data[Constants.FIELD_USERPOINT_KBN1].ToString()))
			{
				errorMessage += ((errorMessage.Length != 0) ? "\r\n" : "");
				errorMessage += "注文ID(kbn1)が空です。仮ポイントの場合は注文ID(kbn1)を必ず入力してください。";
			}
			else
			{
				// 受注存在・ステータスチェック
				var orderModel = new OrderService().GetOrderInfoByOrderId(this.Data[Constants.FIELD_USERPOINT_KBN1].ToString());
				if (orderModel == null)
				{
					errorMessage += ((errorMessage.Length != 0) ? "\r\n" : "");
					errorMessage += ("注文情報:" + this.Data[Constants.FIELD_USERPOINT_KBN1] + " が見つかりませんでした。");
				}
				else if ((orderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
					|| (orderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
				{
					errorMessage += ((errorMessage.Length != 0) ? "\r\n" : "");
					errorMessage += ("注文情報:" + this.Data[Constants.FIELD_USERPOINT_KBN1] + " はすでにキャンセルされています。");
				}
				else if (orderModel.UserId != this.Data[Constants.FIELD_USERPOINT_USER_ID].ToString())
				{
					errorMessage += ((errorMessage.Length != 0) ? "\r\n" : "");
					errorMessage += ("注文情報:　対象ユーザーID(" + this.Data[Constants.FIELD_USERPOINT_USER_ID]+ ")は注文者ID(" + orderModel.UserId + ")と一致していません。");
				}
			}
			return errorMessage;
		}

		/// <summary>
		/// ポイントルールチェック（期間限定ポイント・仮ポイントの場合のみ行う）
		/// ・ポイントルールルールID必須チェック
		/// ・ポイントルールが存在するかチェックする
		/// ・ポイントルールのポイント区分が期間限定ポイントであるかチェックする
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckPointRuleValidity(string errorMessage)
		{
			var pointRuleId = StringUtility.ToEmpty(this.Data[Constants.FIELD_USERPOINT_POINT_RULE_ID]);
			// 必須チェック
			if (string.IsNullOrEmpty(pointRuleId))
			{
				errorMessage += (errorMessage.Length != 0) ? Environment.NewLine : string.Empty;
				errorMessage += MessageManager.GetMessages(Constants.INPUTCHECK_USERPOINT_POINTRULE_ID_EMPTY);
				return errorMessage;
			}

			// ルール存在チェック
			var pointRule = new PointService().GetPointRule(Constants.CONST_DEFAULT_DEPT_ID, pointRuleId);
			if (pointRule == null)
			{
				errorMessage += (errorMessage.Length != 0) ? Environment.NewLine : string.Empty;
				errorMessage += MessageManager.GetMessages(Constants.INPUTCHECK_USERPOINT_POINTRULE_ID_NOT_EXIST)
					.Replace("@@ 1 @@", pointRuleId);
				return errorMessage;
			}

			// 期間限定ポイントのルールか？
			if (pointRule.IsLimitedTermPoint == false)
			{
				errorMessage += (errorMessage.Length != 0) ? Environment.NewLine : string.Empty;
				errorMessage += ((pointRule.PointRuleKbn == Constants.FLG_POINTRULE_POINT_RULE_KBN_BASE)
						? MessageManager.GetMessages(Constants.INPUTCHECK_USERPOINT_POINTRULE_BASERULE_NOT_FOR_LIMITED_TERM_POINT)
						: MessageManager.GetMessages(Constants.INPUTCHECK_USERPOINT_POINTRULE_CAMPAIGN_NOT_FOR_LIMITED_TERM_POINT))
							.Replace("@@ 1 @@", pointRule.PointRuleName);
			}

			return errorMessage;
		}

		/// <summary>
		/// 有効期間などの整合性チェック
		/// ・通常ポイントにeffective_dateが設定されていないか
		/// ・期間限定ポイント（本ポイント）にeffective_dateは設定されているか
		/// ・利用可能期間開始日が有効期限を過ぎていないか
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckUserPointPeriodValididy(string errorMessage)
		{
			// 期間限定ポイントで利用期間が設定されていないものはダメ
			if (this.IsLimitedTermPoint
				&& (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_USERPOINT_EFFECTIVE_DATE]))
					|| string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_USERPOINT_POINT_EXP]))))
			{
				// 仮ポイントは利用期間未設定でもいいが、本ポイントの場合は必須
				if (this.IsPointTypeComp)
				{
					errorMessage += ((errorMessage.Length != 0) ? Environment.NewLine : string.Empty);;
					errorMessage += MessageManager.GetMessages(Constants.INPUTCHECK_USERPOINT_MISSING_PERIOD);
					return errorMessage;
				}
			}

			// 通常ポイントに利用可能期間開始日は指定不可
			if (this.IsBasePoint
				&& (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_USERPOINT_EFFECTIVE_DATE])) == false))
			{
				errorMessage += ((errorMessage.Length != 0) ? Environment.NewLine : string.Empty);
				errorMessage += MessageManager.GetMessages(Constants.INPUTCHECK_USERPOINT_UNACCEPTABLE_EFFECTIVE_DATE);
				return errorMessage;
			}

			// 利用可能期間が開始日＜終了日となっているか
			if (this.IsLimitedTermPoint && this.IsPointTypeComp)
			{
				// ここまで来てればInvalidCastExceptionにはならないはず
				var effectiveDate = DateTime.Parse((string)this.Data[Constants.FIELD_USERPOINT_EFFECTIVE_DATE]);
				var exipiryDate = DateTime.Parse((string)this.Data[Constants.FIELD_USERPOINT_POINT_EXP]);
				if (exipiryDate < effectiveDate)
				{
					errorMessage += ((errorMessage.Length != 0) ? Environment.NewLine : string.Empty);
					errorMessage += MessageManager.GetMessages(Constants.INPUTCHECK_USERPOINT_ILLEGAL_PERIOD);
					return errorMessage;
				}
			}
			return errorMessage;
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			//------------------------------------------------------
			// 共通SQL文作成（※ImportSettingBaseで定義）
			//------------------------------------------------------
			base.CreateSql();

			//------------------------------------------------------
			// ユーザーポイント在庫履歴SQL文
			//------------------------------------------------------
			this.UserPointHistoryInsertSql = CreateUserPointHistoryInsertSql();
			this.UserPointHistoryDeleteSql = CreateUserPointHistoryDeleteSql();
		}

		/// <summary>
		/// Insert/Update文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert/Update文</returns>
		protected override string CreateInsertUpdateSql(string strTableName)
		{
			var result = new StringBuilder();
			var sqlSelect = new StringBuilder();

			//------------------------------------------------------
			// Insert/Update文作成
			//------------------------------------------------------
			// Select文作成
			if (this.UpdateKeys.Count != 0)
			{
				// Select文組み立て
				sqlSelect.Append(" SELECT @SELECT_COUNTS = COUNT(*)");
				sqlSelect.Append(" FROM ").Append(strTableName);
				sqlSelect.Append(CreateWhere());
			}

			if (this.FieldNamesForInsert.Contains(Constants.FIELD_USERPOINT_POINT_EXP) == false)
			{
				this.FieldNamesForInsert.Add(Constants.FIELD_USERPOINT_POINT_EXP);
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_EXP) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERPOINT_POINT_EXP);
			}

			// Insert/Update文組み立て
			result.Append(" DECLARE @point_kbn_no int");
			result.Append(" DECLARE @SELECT_COUNTS int");
			result.Append(" DECLARE @point_kbn_temp NVARCHAR(10)");
			result.AppendFormat(" SET @point_kbn_temp = '{0}'", (string)this.Data[Constants.FIELD_USERPOINT_POINT_KBN]);
			result.Append(sqlSelect.ToString());
			result.Append(" IF @SELECT_COUNTS = 0 OR @point_kbn_temp = '02'"); // 期間限定ポイントは強制的にINSERTを行う
			result.Append("   BEGIN ");
			result.Append(CreateSelectNextPointKbnNo(strTableName));
			result.Append(CreateInsertSqlRaw(strTableName));
			result.Append("   END ");
			result.Append(" ELSE ");
			result.Append("   BEGIN ");
			result.Append(CreateSelectTargetPointKbnNo(strTableName));
			result.Append(CreateUpdateSql(strTableName));
			result.AppendFormat("  AND {0} = @{0}", Constants.FIELD_USERPOINT_POINT_KBN_NO);
			result.Append("   END ");

			return result.ToString();
		}

		/// <summary>
		/// Insert文作成
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <returns>Insert文</returns>
		protected override string CreateInsertSqlRaw(string tableName)
		{
			var result = new StringBuilder();
			var sqlFields = new StringBuilder();
			var sqlValues = new StringBuilder();

			// Insert文作成
			foreach (var fieldName in this.FieldNamesForInsert)
			{
				if (fieldName.Length == 0) continue;

				// 更新禁止フィールド？
				if (this.ExceptionFields.Contains(fieldName)) continue;

				// フィールドの区切り記号を追加
				if (sqlFields.Length != 0)
				{
					sqlFields.Append(",");
					sqlValues.Append(",");
				}

				// 差分更新フィールド？
				if (this.IncreaseFields.Contains(fieldName))
				{
					var realField = fieldName.Substring("add_".Length);
					sqlFields.Append(realField);
					sqlValues.Append("@").Append(fieldName);
				}
				else
				{
					sqlFields.Append(fieldName);
					sqlValues.Append("@").Append(fieldName);
				}
			}

			// 更新者設定（※更新禁止フィールドに"last_changed"があれば更新）
			if (this.ExceptionFields.Contains(Constants.FIELD_IMPORT_LAST_CHANGED))
			{
				sqlFields.AppendFormat(",{0}", Constants.FIELD_IMPORT_LAST_CHANGED);
				sqlValues.AppendFormat(",@{0}", Constants.FIELD_IMPORT_LAST_CHANGED);
			}

			// 「point_kbn_no」の採番設定
			sqlFields.AppendFormat(",{0}", Constants.FIELD_USERPOINT_POINT_KBN_NO);
			sqlValues.AppendFormat(",@{0}", Constants.FIELD_USERPOINT_POINT_KBN_NO);

			// Insert文組み立て
			result.Append("INSERT ").Append(tableName);
			result.Append(" (");
			result.Append(sqlFields);
			result.Append(" ) VALUES ( ");
			result.Append(sqlValues);
			result.Append(" ) ");

			return result.ToString();
		}

		/// <summary>
		/// 枝番採番用SELECT文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>SELECT文</returns>
		private string CreateSelectNextPointKbnNo(string strTableName)
		{
			var result = new StringBuilder();

			result.Append(" SELECT @point_kbn_no = ISNULL(MAX(point_kbn_no), 0) + 1 ");
			result.Append(" FROM ").Append(strTableName);
			result.Append(" WHERE user_id = @user_id ");

			return result.ToString();
		}

		/// <summary>
		/// 更新対象枝番取得SELECT文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>SELECT文</returns>
		private string CreateSelectTargetPointKbnNo(string strTableName)
		{
			var result = new StringBuilder();

			result.Append(" SELECT @point_kbn_no = MAX(point_kbn_no)");
			result.Append(" FROM ").Append(strTableName);
			result.Append(" WHERE user_id = @user_id ");
			result.Append(" AND point_type = @point_type ");
			result.Append(" AND point_kbn = @point_kbn ");
			if (this.IsPointTypeTemp)
			{
				result.Append(" AND kbn1 = @kbn1 ");
			}

			return result.ToString();
		}

		/// <summary>
		/// ユーザーポイント履歴Insert文作成
		/// </summary>
		/// <returns>ユーザーポイント履歴Insert文</returns>
		private string CreateUserPointHistoryInsertSql()
		{
			//------------------------------------------------------
			// Select文作成
			//------------------------------------------------------
			StringBuilder sbSqlSelect = new StringBuilder();
			sbSqlSelect.Append(" SELECT @SELECT_COUNTS = COUNT(*)");
			sbSqlSelect.Append(" FROM ").Append(Constants.TABLE_USERPOINTHISTORY);
			sbSqlSelect.Append(" WHERE ");
			sbSqlSelect.Append(Constants.FIELD_USERPOINTHISTORY_USER_ID).Append(" = @").Append(Constants.FIELD_USERPOINTHISTORY_USER_ID);
			sbSqlSelect.Append(" AND ");
			sbSqlSelect.Append(Constants.FIELD_USERPOINTHISTORY_HISTORY_NO).Append(" = @").Append(Constants.FIELD_USERPOINTHISTORY_HISTORY_NO);

			//------------------------------------------------------
			// Insert文作成
			//------------------------------------------------------
			StringBuilder sbFields = new StringBuilder();
			StringBuilder sbValues = new StringBuilder();

			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_DEPT_ID))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_DEPT_ID).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_DEPT_ID).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_RULE_ID))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_RULE_ID).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_POINT_RULE_ID).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_RULE_KBN))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_RULE_KBN).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_POINT_RULE_KBN).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_KBN))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_KBN).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_POINT_KBN).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_TYPE))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_TYPE).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_POINT_TYPE).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_INC_KBN))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_INC).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_POINT_INC).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_ADD_POINT))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_INC).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_POINT_INC).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINT_POINT_EXP))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_USER_POINT_EXP).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_USER_POINT_EXP).Append(",");
			}
			if (this.FieldNamesForUpdateDelete.Contains(Constants.FIELD_USERPOINTHISTORY_POINT_EXP_EXTEND))
			{
				sbFields.Append(Constants.FIELD_USERPOINTHISTORY_POINT_EXP_EXTEND).Append(",");
				sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_USER_POINT_EXP).Append(",");
			}

			if (sbFields.Length == 0)
			{
				return null;
			}

			StringBuilder sbSqlInsert = new StringBuilder();
			sbFields.Append(Constants.FIELD_USERPOINTHISTORY_USER_ID).Append(",");
			sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_USER_ID).Append(",");

			sbFields.Append(Constants.FIELD_USERPOINTHISTORY_HISTORY_NO).Append(",");
			sbValues.Append(" (SELECT ISNULL(MAX(w2_UserPointHistory.history_no), 0) + 1 FROM w2_UserPointHistory WHERE w2_UserPointHistory.user_id = @user_id)").Append(",");

			sbFields.Append(Constants.FIELD_USERPOINTHISTORY_HISTORY_GROUP_NO).Append(",");
			sbValues.Append(" (SELECT ISNULL(MAX(w2_UserPointHistory.history_group_no), 0) + 1 FROM w2_UserPointHistory WHERE w2_UserPointHistory.user_id = @user_id)").Append(",");

			sbFields.Append(Constants.FIELD_USERPOINTHISTORY_MEMO).Append(",");
			sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_MEMO).Append(",");

			sbFields.Append(Constants.FIELD_USERPOINTHISTORY_KBN1).Append(",");
			sbValues.Append("@").Append(Constants.FIELD_USERPOINTHISTORY_KBN1).Append(",");

			sbFields.Append(Constants.FIELD_USERPOINTHISTORY_DATE_CREATED).Append(",");
			sbValues.Append("GETDATE(),");

			sbFields.Append(Constants.FIELD_USERPOINTHISTORY_LAST_CHANGED);
			sbValues.Append("'").Append(Constants.IMPORT_LAST_CHANGED).Append("'");

			sbSqlInsert.Append(" INSERT INTO " + Constants.TABLE_USERPOINTHISTORY);
			sbSqlInsert.Append("(").Append(sbFields.ToString()).Append(")");
			sbSqlInsert.Append(" VALUES ");
			sbSqlInsert.Append("(").Append(sbValues.ToString()).Append(")");

			//------------------------------------------------------
			// Insert文作成
			//------------------------------------------------------
			StringBuilder sbResult = new StringBuilder();
			sbResult.Append(" DECLARE @SELECT_COUNTS int");
			sbResult.Append(sbSqlSelect.ToString());
			sbResult.Append(sbSqlInsert.ToString());

			return sbResult.ToString();
		}

		/// <summary>
		/// ユーザーポイント履歴Delete文作成
		/// </summary>
		/// <returns>ユーザーポイント履歴Delete文</returns>
		private string CreateUserPointHistoryDeleteSql()
		{
			StringBuilder sbResult = new StringBuilder();

			sbResult.Append("DELETE ").Append(Constants.TABLE_USERPOINTHISTORY);
			sbResult.Append(" WHERE ").Append(Constants.FIELD_USERPOINTHISTORY_USER_ID).Append(" = ").Append("@").Append(Constants.FIELD_USERPOINTHISTORY_USER_ID);
			sbResult.Append("   AND ").Append(Constants.FIELD_USERPOINTHISTORY_HISTORY_NO).Append(" = ").Append("@").Append(Constants.FIELD_USERPOINTHISTORY_HISTORY_NO);

			return sbResult.ToString();
		}

		/// <summary>ユーザーポイント履歴Insert文</summary>
		public string UserPointHistoryInsertSql { get; set; }
		/// <summary>ユーザーポイント履歴Delete文</summary>
		public string UserPointHistoryDeleteSql { get; set; }
		/// <summary>期間限定ポイント？</summary>
		private bool IsLimitedTermPoint
		{
			get
			{
				return ((string)this.Data[Constants.FIELD_USERPOINT_POINT_KBN] == Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT);
			}
		}
		/// <summary>通常ポイント？</summary>
		private bool IsBasePoint
		{
			get
			{
				return ((string)this.Data[Constants.FIELD_USERPOINT_POINT_KBN] == Constants.FLG_USERPOINT_POINT_KBN_BASE);
			}
		}
		/// <summary>本ポイント？</summary>
		private bool IsPointTypeComp
		{
			get
			{
				return ((string)this.Data[Constants.FIELD_USERPOINT_POINT_TYPE] == Constants.FLG_USERPOINT_POINT_TYPE_COMP);
			}
		}
		/// <summary>仮ポイント？</summary>
		private bool IsPointTypeTemp
		{
			get
			{
				return ((string)this.Data[Constants.FIELD_USERPOINT_POINT_TYPE] == Constants.FLG_USERPOINT_POINT_TYPE_TEMP);
			}
		}
	}
}
