/*
=========================================================================================================
  Module      : 注文返品交換仮ポイント入力クラス (OrderReturnExchangePointInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Input.Order.OrderReturnExchange
{
	/// <summary>
	/// 注文返品仮ポイント入力クラス
	/// </summary>
	[Serializable]
	public class OrderReturnExchangeTempPointInput : InputBaseDto
	{
		#region プロパティ
		/// <summary>ポイント数</summary>
		public string OrderPointAdd
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT] = value; }
		}
		/// <summary>枝番</summary>
		public string PointKbnNo
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN_NO]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN_NO] = value; }
		}
		/// <summary>ポイント区分</summary>
		public string PointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN] = value; }
		}
		/// <summary>ポイント加算区分</summary>
		public string PointIncKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_INC_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_INC_KBN] = value; }
		}
		/// <summary>ポイント数（更新前）</summary>
		public string OrderPointAddBefore
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT + Constants.FIELD_COMMON_BEFORE]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT + Constants.FIELD_COMMON_BEFORE] = value; }
		}
		#endregion
	}
}