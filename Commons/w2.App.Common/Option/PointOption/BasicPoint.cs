/*
=========================================================================================================
  Module      : 通常ポイントオブジェクト(BasicPoint.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Domain.Point;

namespace w2.App.Common.Option.PointOption
{
	/// <summary>
	/// 通常ポイントオブジェクト
	/// </summary>
	[Serializable]
	public class BasicPoint
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BasicPoint()
		{
			this.PointComp = 0;
			this.PointTemp = 0;
			this.PointCompExpiryDate = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BasicPoint(UserPointModel[] model)
		{
			this.PointComp = model.Where(upm => (upm.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)).Sum(upm => upm.Point);
			this.PointTemp = model.Where(upm => (upm.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP)).Sum(upm => upm.Point);
			this.PointCompExpiryDate = model
				.Where(upm => (upm.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
				.Max(upm => upm.PointExp);
		}

		/// <summary>本ポイント</summary>
		public decimal PointComp { get; set; }
		/// <summary>仮ポイント</summary>
		public decimal PointTemp { get; set; }
		/// <summary>本ポイントの有効期限</summary>
		public DateTime? PointCompExpiryDate { get; set; }
	}
}
