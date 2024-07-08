/*
=========================================================================================================
  Module      : ユーザポイントマスタモデル (UserPointModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Domain.Point
{
	/// <summary>
	/// ユーザポイントマスタモデル
	/// </summary>
	public partial class UserPointModel
	{
		#region メソッド
		/// <summary>
		/// デフォルト有効期限を取得<br />
		/// ポイントルールに基づかない発行ではデフォルトを1年後とする
		/// </summary>
		/// <returns>有効期限</returns>
		public static DateTime GetDefaultExp()
		{
			var result = DateTime.Now.AddYears(1).AddDays(1).AddMilliseconds(-3);
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>仮ポイント？</summary>
		public bool IsPointTypeTemp
		{
			get { return (this.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP); }
		}
		/// <summary>本ポイント？</summary>
		public bool IsPointTypeComp
		{
			get { return (this.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP); }
		}
		/// <summary>通常ポイント？</summary>
		public bool IsBasePoint
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE); }
		}
		/// <summary>期間限定ポイント？</summary>
		public bool IsLimitedTermPoint
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT); }
		}
		/// <summary>注文利用可能？</summary>
		public bool IsUsableForOrder
		{
			get
			{
				if (this.IsPointTypeTemp) return false;

				if (this.IsBasePoint)
				{
					if (this.PointExp == null) return true;
					return (DateTime.Now <= this.PointExp);
				}
				if (this.IsLimitedTermPoint)
				{
					return ((this.EffectiveDate <= DateTime.Now) && (DateTime.Now <= this.PointExp));
				}

				return false;
			}
		}
		/// <summary>(拡張プロパティ)ポイントに紐付いている注文ID</summary>
		public string OrderId
		{
			get { return this.Kbn1; }
			set { this.Kbn1 = value; }
		}
		/// <summary>(拡張プロパティ)ポイントに紐付いている定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return this.Kbn2; }
			set { this.Kbn2 = value; }
		}
		#endregion
	}
}
