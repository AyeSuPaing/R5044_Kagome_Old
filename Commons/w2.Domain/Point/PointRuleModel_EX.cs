/*
=========================================================================================================
  Module      : ポイントルールマスタモデル (PointRuleModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントルールマスタモデル
	/// </summary>
	public partial class PointRuleModel
	{
		/// <summary>ポイント有効期限延長のデフォルト値</summary>
		public const string DEFAULT_POINT_EXP_EXTEND_STRING = "+000000";
		/// <summary>期間限定ポイント：有効期限を設定する</summary>
		public const string LIMTIED_TERM_POINT_EXPIRE_KBN_RELATIVE = "ExpireRelative";
		/// <summary>期間限定ポイント：有効期間を設定する</summary>
		public const string LIMTIED_TERM_POINT_EXPIRE_KBN_ABSOLUTE = "ExpireAbsolute";

		#region メソッド
		/// <summary>
		/// 利用可能開始日を算出する
		/// </summary>
		/// <param name="forceCalculation">NULLになるパターンを考慮せずに強制的に計算を実行する</param>
		/// <returns>ポイント発効日</returns>
		public DateTime? CalculateEffectiveDatetime(bool forceCalculation = false)
		{
			if (forceCalculation == false)
			{
				// ポイント区分が通常
				// OR 期間限定ポイントで仮ポイントを使用するものはNULL
				// （本ポイント移行時に有効期限を計算するので）
				if (this.IsBasePoint
					|| (this.IsLimitedTermPoint && (this.UseTempFlg == Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID)))
				{
					return null;
				}
			}

			var dt = DateTime.Now.Date;
			switch (this.LimitedTermPointExpireKbn)
			{
				case LIMTIED_TERM_POINT_EXPIRE_KBN_ABSOLUTE:
					if (this.PeriodBegin.HasValue == false) break;
					dt = this.PeriodBegin.Value;
					break;

				case LIMTIED_TERM_POINT_EXPIRE_KBN_RELATIVE:
					if ((this.EffectiveOffset.HasValue == false)
						|| string.IsNullOrEmpty(this.EffectiveOffsetType)) break;

					switch (this.EffectiveOffsetType)
					{
						case Constants.FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_DAY:
							dt = dt.AddDays((int)this.EffectiveOffset);
							break;

						case Constants.FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_MONTH:
							dt = dt.AddMonths((int)this.EffectiveOffset);
							break;

						case Constants.FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_MONTH_FIRST_DAY:
							dt = dt.AddMonths((int)this.EffectiveOffset);
							dt = new DateTime(dt.Year, dt.Month, 1);
							break;
					}
					break;
			}

			return dt;
		}

		/// <summary>
		/// 有効期限を算出する
		/// </summary>
		/// <param name="forceCalculation">NULLになるパターンを考慮せずに強制的に計算を実行する</param>
		/// <returns>
		/// PointExpExtendと現在日時を元に算出した有効期限
		/// </returns>
		/// <remarks>期間限定ポイントで仮ポイントはNULL返却</remarks>
		public DateTime? CalculateExpExtendDatetime(bool forceCalculation = false)
		{
			if (forceCalculation == false)
			{
				if ((this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT)
					&& (this.UseTempFlg == Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID))
				{
					return null;
				}
			}

			var dt = DateTime.Now;
			var sign = (this.PointExpExtendSign == "+")
				? 1
				: (this.PointExpExtendSign == "-")
					? -1
					: 0;

			// 符号がおかしければ計算できない
			if (sign == 0) return null;

			var year = int.Parse(this.PointExpExtendYear) * sign;
			var month = int.Parse(this.PointExpExtendMonth) * sign;
			var day = int.Parse(this.PointExpExtendDay) * sign;

			dt = dt.AddYears(year).AddMonths(month).AddDays(day);

			dt = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 997);
			return dt;
		}

		/// <summary>
		/// 期間限定ポイントの有効期限を計算する
		/// </summary>
		/// <param name="forceCalculation">NULLになるパターンを考慮せずに強制的に計算を実行する</param>
		/// <returns>有効期限</returns>
		/// <remarks>期間限定ポイントで仮ポイントはNULL返却</remarks>
		public DateTime? CalculateExpiryDatetimeForLimitedTermPoint(bool forceCalculation = false)
		{
			if (forceCalculation == false)
			{
				if (this.IsLimitedTermPoint
					&& (this.UseTempFlg == Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID))
				{
					return null;
				}
			}

			var dt = DateTime.Now;
			switch (this.LimitedTermPointExpireKbn)
			{
				// 有効期間を指定する場合
				case LIMTIED_TERM_POINT_EXPIRE_KBN_ABSOLUTE:
					if (this.PeriodEnd.HasValue == false) break;

					dt = this.PeriodEnd.Value;
					dt = new DateTime(dt.Year, dt.Month, dt.Day);
					break;

				// 発行日から期限を指定する
				case LIMTIED_TERM_POINT_EXPIRE_KBN_RELATIVE:
					if ((this.Term.HasValue == false) || string.IsNullOrEmpty(this.TermType)) break;

					dt = CalculateEffectiveDatetime(forceCalculation) ?? DateTime.Now.Date;
					switch (this.TermType)
					{
						case Constants.FLG_POINTRULE_TERM_TYPE_DAY:
							dt = dt.AddDays((int)this.Term);
							break;

						case Constants.FLG_POINTRULE_TERM_TYPE_MONTH:
							dt = dt.AddMonths((int)this.Term);
							break;
					}

					dt = dt.AddDays(-1);
					break;
			}

			dt = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 997);
			return dt;
		}

		/// <summary>
		/// ポイント有効期限延長フォーマットの文字列取得
		/// </summary>
		/// <param name="year">延長年</param>
		/// <param name="month">延長月</param>
		/// <param name="day">延長日</param>
		/// <returns></returns>
		public static string GetPointExpExtendFormtString(int year, int month, int day)
		{
			if (year == 0 && month == 0 && day == 0)
			{
				return DEFAULT_POINT_EXP_EXTEND_STRING;
			}

			string sign = "+";
			if (year < 0 || month < 0 || day < 0)
			{
				sign = "-";
			}

			return string.Format("{0}{1}{2}{3}",
				sign,
				string.Format("{0:D2}", year).Replace("-", ""),
				string.Format("{0:D2}", month).Replace("-", ""),
				string.Format("{0:D2}", day).Replace("-", ""));
		}
		#endregion

		#region プロパティ
		/// <summary>ポイントルール日付</summary>
		public PointRuleDateModel[] RuleDate
		{
			get { return (PointRuleDateModel[])this.DataSource["EX_RuleDate"]; }
			set { this.DataSource["EX_RuleDate"] = value; }
		}
		/// <summary>ポイント有効期限延長の符号（+ or - PointExpExtendの頭1ケタ目)</summary>
		public string PointExpExtendSign { get { return this.PointExpExtend.Substring(0, 1); } }
		/// <summary>ポイント有効期限延長の年（頭2～3ケタ目 * 符号)</summary>
		public string PointExpExtendYear { get { return this.PointExpExtend.Substring(1, 2); } }
		/// <summary>ポイント有効期限延長の月（頭4～5ケタ目 * 符号)</summary>
		public string PointExpExtendMonth { get { return this.PointExpExtend.Substring(3, 2); } }
		/// <summary>ポイント有効期限延長の日（頭6～7ケタ目 * 符号)</summary>
		public string PointExpExtendDay { get { return this.PointExpExtend.Substring(5, 2); } }
		/// <summary>ポイント加算区分がスケジュール利用できるか？</summary>
		public bool UseScheduleIncKbnPointRule
		{
			get
			{
				return ((this.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
					|| (this.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT));
			}
		}
		/// <summary>期間限定ポイント：ポイントルールが有効期限の設定か有効期間の設定か</summary>
		public string LimitedTermPointExpireKbn
		{
			get
			{
				return (this.PeriodBegin != null)
					? LIMTIED_TERM_POINT_EXPIRE_KBN_ABSOLUTE
					: (this.Term != null)
						? LIMTIED_TERM_POINT_EXPIRE_KBN_RELATIVE
						: string.Empty;
			}
		}
		/// <summary>通常ポイントか</summary>
		public bool IsBasePoint
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE); }
		}
		/// <summary>期間限定ポイントか</summary>
		public bool IsLimitedTermPoint
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT); }
		}
		/// <summary>仮ポイントを使用するか</summary>
		public bool NeedsToUseTempPoint
		{
			get { return (this.UseTempFlg == Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID); }
		}
		/// <summary>定期購入基本ルールポイント設定あるかどうか</summary>
		public bool FixedPurchasePointValid
		{
			get
			{
				return ((this.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM) || (this.IncFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_RATE));
			}
		}
		#endregion
	}
}
