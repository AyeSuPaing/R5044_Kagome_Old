/*
=========================================================================================================
  Module      : 注文者情報モデル (OrderOwnerModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Util;
using w2.Domain.User.Helper;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文者情報モデル
	/// </summary>
	public partial class OrderOwnerModel
	{
		#region メソッド
		/// <summary>
		/// 住所項目を結合（国名は含めない）
		/// </summary>
		/// <returns>結合した住所</returns>
		public string ConcatenateAddressWithoutCountryName()
		{
			var address = AddressHelper.ConcatenateAddressWithoutCountryName(
				this.OwnerAddr1,
				this.OwnerAddr2,
				this.OwnerAddr3,
				this.OwnerAddr4);

			return address;
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// 拡張項目_注文者区分
		/// </summary>
		public string OwnerKbnText
		{
			get { return ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_KBN, this.OwnerKbn); }
		}
		#endregion
	}
}
