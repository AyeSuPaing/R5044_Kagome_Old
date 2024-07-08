/*
=========================================================================================================
  Module      : 定期商品変更設定拡張モデル (FixedPurchaseProductChangeSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.FixedPurchaseProductChangeSetting
{
	/// <summary>
	/// 定期商品変更設定拡張モデル
	/// </summary>
	public partial class FixedPurchaseProductChangeSettingModel
	{
		#region プロパティ
		/// <summary>変更元定期商品</summary>
		public FixedPurchaseBeforeChangeItemModel[] BeforeChangeItems { get; set; }
		/// <summary>変更後定期商品</summary>
		public FixedPurchaseAfterChangeItemModel[] AfterChangeItems { get; set; }
		#endregion
	}
}
