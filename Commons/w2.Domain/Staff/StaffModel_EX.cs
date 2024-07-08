/*
=========================================================================================================
  Module      : スタッフモデル (StaffModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Staff
{
	/// <summary>
	/// スタッフモデル
	/// </summary>
	public partial class StaffModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>リアル店舗名</summary>
		public string RealShopName
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_NAME]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_NAME] = value; }
		}
		/// <summary>モデルか</summary>
		public bool IsModel
		{
			get { return (this.ModelFlg == Constants.FLG_STAFF_MODEL_FLG_VALID); }
		}
		#endregion
	}
}
