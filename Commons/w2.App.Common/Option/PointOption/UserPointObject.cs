/*
=========================================================================================================
  Module      : ユーザーポイントオブジェクトクラス(UserPointObject.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Option.PointOption;
using w2.Domain.Point;

namespace w2.App.Common.Option
{
	/// <summary>
	/// ユーザーポイントオブジェクトクラス
	/// 通常ポイントと期間限定ポイントを包括して管理する。
	/// </summary>
	[Serializable]
	public class UserPointObject
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointObject()
		{
			this.BasicPoint = new BasicPoint();
			this.LimitedTermPoint = new LimitedTermPoint[] { };
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userPoint">ポイントモデル</param>
		/// <remarks>
		/// そのユーザーが保有するポイントをすべて渡すこと
		/// ポイント区分、ポイント種別での振り分けは内部で行う
		/// </remarks>
		public UserPointObject(UserPointModel[] userPoint)
		{
			// ポイント区分でふりわける
			// 通常ポイント
			this.BasicPoint = new BasicPoint(
				userPoint
					.Where(p => p.IsBasePoint)
					.ToArray());

			// 期間限定ポイント
			this.LimitedTermPoint = userPoint
				.Where(p => p.IsLimitedTermPoint)
				.Select(p => new LimitedTermPoint(p))
				.ToArray();
		}

		/// <summary>通常ポイント</summary>
		public BasicPoint BasicPoint { get; set; }
		/// <summary>期間限定ポイント（DBのレコードには対応していない）</summary>
		public LimitedTermPoint[] LimitedTermPoint { get; set; }
		/// <summary>利用可能ポイント合計</summary>
		public decimal PointUsable
		{
			get
			{
				var pointComp = ((this.BasicPoint.PointCompExpiryDate == null)
					|| (this.BasicPoint.PointCompExpiryDate >= DateTime.Now))
						? this.BasicPoint.PointComp
						: 0;
				var limitedTermPoint = this.LimitedTermPoint
					.Where(p => ((p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
						&& (p.EffectiveDate <= DateTime.Now)
						&& (DateTime.Now <= p.ExpiryDate)))
					.Sum(p => p.Point);
				return (pointComp + limitedTermPoint);
			}
		}
		/// <summary>本ポイント合計</summary>
		public decimal PointComp
		{
			get
			{
				return this.BasicPoint.PointComp
					+ this.LimitedTermPoint
						.Where(p => (p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
						.Sum(p => p.Point);
			}
		}
		/// <summary>仮ポイント合計</summary>
		public decimal PointTemp
		{
			get
			{
				return this.BasicPoint.PointTemp
					+ this.LimitedTermPoint
						.Where(p => (p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP))
						.Sum(p => p.Point);
			}
		}
		/// <summary>ポイント合計（期間限定、通常、仮、本ポイントを考慮しない全ポイント）</summary>
		public decimal PointTotal
		{
			get
			{
				return this.BasicPoint.PointComp
					+ this.BasicPoint.PointTemp
					+ this.LimitedTermPoint
						.Sum(p => p.Point);
			}
		}
	}
}
