/*
=========================================================================================================
  Module      : ユーザポイント履歴モデル (UserPointHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.Domain.Point
{
	/// <summary>
	/// ユーザポイント履歴モデル
	/// </summary>
	public partial class UserPointHistoryModel
	{
		/// <summary>ポイント有効期限延長のデフォルト値</summary>
		public const string DEFAULT_POINT_EXP_EXTEND_STRING = "+000000";

		#region メソッド
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
		/// <summary>
		/// 拡張項目_ポイント種別
		/// </summary>
		public string PointTypeText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_TYPE, this.PointType);
			}
		}
		/// <summary>履歴グループ番号がセット済みか</summary>
		public bool IsHistoryGroupNoAlreadySet
		{
			get { return (this.HistoryGroupNo != Constants.CONST_USERPOINTHISTORY_DEFAULT_GROUP_NO); }
		}
		/// <summary>ポイント復元処理を実施済みか</summary>
		public bool IsRestored
		{
			get { return (this.RestoredFlg == Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_RESTORED); }
		}
		/// <summary>復元済フラグが利用可能か</summary>
		/// <remarks>V5.13以前の注文で、ポイントの再計算をR5044_Kagome.Developで実行されていない注文はこのプロパティがFALSEになる。</remarks>
		public bool IsRestoredFlgAvailable
		{
			get { return (this.RestoredFlg != Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NA); }
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
		/// <summary>本ポイントか</summary>
		public bool IsPointTypeComp
		{
			get { return (this.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP); }
		}
		/// <summary>購入時付与ポイントか</summary>
		public bool IsAddedPoint
		{
			get
			{
				return ((this.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_BUY)
					|| (this.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY));
			}
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return this.Kbn1; }
			set { this.Kbn1 = value; }
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return this.Kbn2; }
			set { this.Kbn2 = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return this.Kbn3; }
			set { this.Kbn3 = value; }
		}
		/// <summary>有効期限延長（符号）</summary>
		public string PointExpExtendSign
		{
			get
			{
				return (string.IsNullOrEmpty(this.PointExpExtend) == false)
					? this.PointExpExtend.Substring(0, 1)
					: string.Empty;
			}
		}
		/// <summary>有効期限延長（年）</summary>
		public int PointExpExtendYear
		{
			get
			{
				return (string.IsNullOrEmpty(this.PointExpExtend) == false)
					? int.Parse(this.PointExpExtend.Substring(1, 2))
					: 0;
			}
		}
		/// <summary>有効期限延長（月）</summary>
		public int PointExpExtendMonth
		{
			get
			{
				return (string.IsNullOrEmpty(this.PointExpExtend) == false)
					? int.Parse(this.PointExpExtend.Substring(3, 2))
					: 0;
			}
		}
		/// <summary>Shop name</summary>
		public string ShopName { get; set; }
		#endregion
	}
}