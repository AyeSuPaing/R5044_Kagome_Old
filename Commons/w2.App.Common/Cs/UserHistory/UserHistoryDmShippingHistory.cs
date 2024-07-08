/*
=========================================================================================================
  Module      : ユーザー履歴（DM発送履歴）クラス(UserHistoryDmShippingHistory.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.DmShippingHistory;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー履歴（DM発送履歴）クラス
	/// </summary>
	[Serializable]
	public class UserHistoryDmShippingHistory : UserHistoryBase
	{
		private const string KBN_STRING = "DM";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		public UserHistoryDmShippingHistory(DmShippingHistoryModel info)
			: base(info.DataSource)
		{
			this.DmShippingHistory = info;
		}

		/// <summary>
		/// 情報セット
		/// </summary>
		protected override void SetInfo()
		{
			// DM発送日をセット
			this.DateTime = this.DmShippingDate;
			this.KbnString = KBN_STRING;
		}

		#region プロパティ
		/// <summary>DM発送履歴情報</summary>
		public DmShippingHistoryModel DmShippingHistory { get; private set; }
		/// <summary>DM発送日</summary>
		public DateTime DmShippingDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DM_SHIPPING_DATE]; }
		}
		#endregion
	}
}