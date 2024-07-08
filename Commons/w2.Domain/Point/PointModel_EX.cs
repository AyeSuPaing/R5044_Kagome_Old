/*
=========================================================================================================
  Module      : ポイントマスタモデル (PointModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントマスタモデル
	/// </summary>
	public partial class PointModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>通常ポイント？</summary>
		public bool IsPointKbnBase
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE); }
		}
		/// <summary>期間限定ポイント？</summary>
		public bool IsPointKbnLimitedTermPoint
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT); }
		}
		/// <summary>有効期限設定有効？</summary>
		/// <remarks>期間限定ポイントの場合この設定は考慮しない</remarks>
		public bool IsValidPointExpKbn
		{
			get { return (this.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID); }
		}
		#endregion
	}
}
