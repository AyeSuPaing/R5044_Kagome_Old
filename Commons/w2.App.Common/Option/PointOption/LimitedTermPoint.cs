/*
=========================================================================================================
  Module      : 期間限定ポイントオブジェクト(LimitedTermPoint.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.Point;

namespace w2.App.Common.Option.PointOption
{
	/// <summary>
	/// 期間限定ポイントオブジェクト
	/// </summary>
	[Serializable]
	public class LimitedTermPoint
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LimitedTermPoint()
		{
			this.PointKbnNo = 1;
			this.Point = 0;
			this.PointType = Constants.FLG_USERPOINT_POINT_TYPE_COMP;
			this.EffectiveDate = null;
			this.ExpiryDate = null;
			this.PointRuleId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ユーザーポイント</param>
		public LimitedTermPoint(UserPointModel model)
		{
			this.PointKbnNo = model.PointKbnNo;
			this.Point = model.Point;
			this.PointType = model.PointType;
			this.EffectiveDate = model.EffectiveDate;
			this.ExpiryDate = model.PointExp;
			this.PointRuleId = model.PointRuleId;
		}

		/// <summary>ポイント枝番</summary>
		public int PointKbnNo { get; set; }
		/// <summary>ポイント</summary>
		public decimal Point { get; set; }
		/// <summary>ポイント種別（仮・本ポイント）</summary>
		public string PointType { get; set; }
		/// <summary>ポイント利用可能開始日</summary>
		public DateTime? EffectiveDate { get; set; }
		/// <summary>有効期限</summary>
		public DateTime? ExpiryDate { get; set; }
		/// <summary>発行ルールID</summary>
		public string PointRuleId { get; set; }
		/// <summary>注文利用可能か</summary>
		public bool IsUsableForOrder
		{
			get
			{
				if (this.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP)
				{
					return false;
				}
				return ((this.EffectiveDate <= DateTime.Now) && (DateTime.Now <= this.ExpiryDate));
			}
		}
		/// <summary>本ポイントか？</summary>
		public bool IsPointTypeComp
		{
			get { return (this.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP); }
		}
		/// <summary>仮ポイントか？</summary>
		public bool IsPointTypeTemp
		{
			get { return (this.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP); }
		}
	}
}
