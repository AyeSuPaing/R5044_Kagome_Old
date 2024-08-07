/*
=========================================================================================================
  Module      : 決済種別マスタモデル (PaymentModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Payment
{
	/// <summary>
	/// 決済種別マスタモデル
	/// </summary>
	[Serializable]
	public partial class PaymentModel : ModelBase<PaymentModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PaymentModel()
		{
			this.PaymentGroupId = "";
			this.PaymentAltId = "";
			this.PaymentPriceKbn = Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR;
			this.PaymentPrice = 0;
			this.MobileDispFlg = Constants.FLG_PAYMENT_MOBILE_DISP_FLG_BOTH_PC_AND_MOBILE;
			this.DisplayOrder = 1;
			this.ValidFlg = Constants.FLG_PAYMENT_VALID_FLG_VALID;
			this.DelFlg = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PaymentModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PaymentModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_SHOP_ID] = value; }
		}
		/// <summary>決済種別グループID</summary>
		public string PaymentGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_GROUP_ID] = value; }
		}
		/// <summary>決済種別ID</summary>
		public string PaymentId
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_ID]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_ID] = value; }
		}
		/// <summary>連携用決済ID</summary>
		public string PaymentAltId
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_ALT_ID]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_ALT_ID] = value; }
		}
		/// <summary>決済種別名</summary>
		public string PaymentName
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME] = value; }
		}
		/// <summary>モバイル用決済種別名</summary>
		public string PaymentNameMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME_MOBILE] = value; }
		}
		/// <summary>決済手数料区分</summary>
		public string PaymentPriceKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN] = value; }
		}
		/// <summary>決済手数料</summary>
		public decimal PaymentPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_PRICE]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_PRICE] = value; }
		}
		/// <summary>利用可能金額(下限)</summary>
		public decimal? UsablePriceMin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN];
			}
			set { this.DataSource[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN] = value; }
		}
		/// <summary>利用可能金額(上限)</summary>
		public decimal? UsablePriceMax
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX];
			}
			set { this.DataSource[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX] = value; }
		}
		/// <summary>モバイル表示フラグ</summary>
		public string MobileDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_MOBILE_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_MOBILE_DISP_FLG] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_PAYMENT_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PAYMENT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PAYMENT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_LAST_CHANGED] = value; }
		}
		/// <summary>利用不可ユーザー管理レベル</summary>
		public string UserManagementLevelNotUse
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_USER_MANAGEMENT_LEVEL_NOT_USE]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_USER_MANAGEMENT_LEVEL_NOT_USE] = value; }
		}
		/// <summary>利用不可注文者区分</summary>
		public string OrderOwnerKbnNotUse
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_ORDER_OWNER_KBN_NOT_USE]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_ORDER_OWNER_KBN_NOT_USE] = value; }
		}
		#endregion
	}
}
