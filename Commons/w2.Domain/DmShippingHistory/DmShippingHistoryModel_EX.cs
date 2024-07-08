/*
=========================================================================================================
  Module      : DM発送履歴モデル (DmShippingHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.DmShippingHistory
{
	/// <summary>
	/// DM発送履歴モデル
	/// </summary>
	public partial class DmShippingHistoryModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>有効期間</summary>
		public string ValidDate
		{
			get
			{
				 var validDate =string.Format(
					"{0} {1} {2}",
					(this.ValidDateFrom.HasValue) ? this.ValidDateFrom.Value.ToShortDateString() : string.Empty,
					(this.ValidDateFrom.HasValue || this.ValidDateTo.HasValue) ? "～" : string.Empty,
					(this.ValidDateTo.HasValue) ? this.ValidDateTo.Value.ToShortDateString() : string.Empty).Trim();

				return string.IsNullOrEmpty(validDate) ? "－" : validDate;
			}
		}
		/// <summary>代表者のユーザーID(統合用)</summary>
		public string RepresentativeUserId { get; set; }
		#endregion
	}
}
