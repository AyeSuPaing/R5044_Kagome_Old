/*
=========================================================================================================
  Module      : リアル店舗情報モデル (RealShopModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.RealShop
{
	/// <summary>
	/// リアル店舗情報モデル
	/// </summary>
	public partial class RealShopModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>有効か</summary>
		public bool IsValid
		{
			get { return this.ValidFlg == Constants.FLG_ON; }
		}
		#endregion
	}
}
