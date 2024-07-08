/*
=========================================================================================================
  Module      : クーポンテーブルモデル (CouponModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Coupon
{
	/// <summary>
	/// クーポンテーブルモデル
	/// </summary>
	public partial class CouponModel
	{
		#region プロパティ
		/// <summary>クーポン例外商品アイコン1</summary>
		public int ExceptionalIcon1
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "1"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "1"] = value; }
		}

		/// <summary>クーポン例外商品アイコン2</summary>
		public int ExceptionalIcon2
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "2"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "2"] = value; }
		}

		/// <summary>クーポン例外商品アイコン3</summary>
		public int ExceptionalIcon3
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "3"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "3"] = value; }
		}

		/// <summary>クーポン例外商品アイコン4</summary>
		public int ExceptionalIcon4
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "4"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "4"] = value; }
		}

		/// <summary>クーポン例外商品アイコン5</summary>
		public int ExceptionalIcon5
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "5"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "5"] = value; }
		}

		/// <summary>クーポン例外商品アイコン6</summary>
		public int ExceptionalIcon6
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "6"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "6"] = value; }
		}

		/// <summary>クーポン例外商品アイコン7</summary>
		public int ExceptionalIcon7
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "7"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "7"] = value; }
		}

		/// <summary>クーポン例外商品アイコン8</summary>
		public int ExceptionalIcon8
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "8"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "8"] = value; }
		}

		/// <summary>クーポン例外商品アイコン9</summary>
		public int ExceptionalIcon9
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "9"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "9"] = value; }
		}

		/// <summary>クーポン例外商品アイコン10</summary>
		public int ExceptionalIcon10
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "10"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "10"] = value; }
		}
		#endregion
	}
}
