/*
=========================================================================================================
  Module      : ユーザー配送先情報モデル (UserShippingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.UserShipping
{
	/// <summary>
	/// ユーザー配送先情報モデル
	/// </summary>
	public partial class UserShippingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>郵便番号1</summary>
		public string ShippingZip1 { get { return (this.ShippingZip + "-").Split('-')[0]; } }
		/// <summary>郵便番号2</summary>
		public string ShippingZip2 { get { return (this.ShippingZip + "-").Split('-')[1]; } }
		/// <summary>郵便番号1-1</summary>
		public string ShippingTel1_1 { get { return (this.ShippingTel1 + "--").Split('-')[0]; } }
		/// <summary>郵便番号1-2</summary>
		public string ShippingTel1_2 { get { return (this.ShippingTel1 + "--").Split('-')[1]; } }
		/// <summary>郵便番号1-3</summary>
		public string ShippingTel1_3 { get { return (this.ShippingTel1 + "--").Split('-')[2]; } }
		#endregion
	}
}