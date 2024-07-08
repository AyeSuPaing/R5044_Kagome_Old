/*
=========================================================================================================
  Module      : セットプロモーションマスタモデル (SetPromotionModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.SetPromotion
{
	/// <summary>
	/// セットプロモーションマスタモデル
	/// </summary>
	[Serializable]
	public partial class SetPromotionModel : ModelBase<SetPromotionModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SetPromotionModel()
		{
			this.ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF;
			this.ShippingChargeFreeFlg = Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF;
			this.PaymentChargeFreeFlg = Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF;
			this.DescriptionKbn = Constants.FLG_SETPROMOTION_DESCRIPTION_KBN_TEXT;
			this.DescriptionKbnMobile = Constants.FLG_SETPROMOTION_DESCRIPTION_KBN_MOBILE_TEXT;
			this.DisplayOrder = 1;
			this.ValidFlg = Constants.FLG_SETPROMOTION_VALID_FLG_VALID;
			this.Items = new SetPromotionItemModel[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SetPromotionModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SetPromotionModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>セットプロモーションID</summary>
		public string SetpromotionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_ID]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_ID] = value; }
		}
		/// <summary>管理用セットプロモーション名</summary>
		public string SetpromotionName
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_NAME]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_NAME] = value; }
		}
		/// <summary>表示用セットプロモーション名</summary>
		public string SetpromotionDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME] = value; }
		}
		/// <summary>モバイル用表示用セットプロモーション名</summary>
		public string SetpromotionDispNameMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME_MOBILE] = value; }
		}
		/// <summary>商品金額割引フラグ</summary>
		public string ProductDiscountFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG] = value; }
		}
		/// <summary>配送料無料フラグ</summary>
		public string ShippingChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>決済手数料無料フラグ</summary>
		public string PaymentChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>商品割引区分</summary>
		public string ProductDiscountKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_KBN] = value; }
		}
		/// <summary>商品割引設定</summary>
		public decimal? ProductDiscountSetting
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING];
			}
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING] = value; }
		}
		/// <summary>表示用文言HTML区分</summary>
		public string DescriptionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN] = value; }
		}
		/// <summary>表示用文言</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION] = value; }
		}
		/// <summary>モバイル表示用文言HTML区分</summary>
		public string DescriptionKbnMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN_MOBILE] = value; }
		}
		/// <summary>モバイル表示用文言</summary>
		public string DescriptionMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_MOBILE] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime BeginDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SETPROMOTION_BEGIN_DATE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_BEGIN_DATE] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime? EndDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SETPROMOTION_END_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SETPROMOTION_END_DATE];
			}
			set { this.DataSource[Constants.FIELD_SETPROMOTION_END_DATE] = value; }
		}
		/// <summary>適用会員ランク</summary>
		public string TargetMemberRank
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_MEMBER_RANK]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_MEMBER_RANK] = value; }
		}
		/// <summary>適用注文区分</summary>
		public string TargetOrderKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_ORDER_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_ORDER_KBN] = value; }
		}
		/// <summary>URL</summary>
		public string Url
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_URL]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_URL] = value; }
		}
		/// <summary>モバイルURL</summary>
		public string UrlMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_URL_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_URL_MOBILE] = value; }
		}
		/// <summary>表示優先順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_SETPROMOTION_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_LAST_CHANGED] = value; }
		}
		/// <summary>ターゲットリストID</summary>
		public string TargetIds
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_TARGET_IDS]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_TARGET_IDS] = value; }
		}
		/// <summary>適用優先順</summary>
		public int ApplyOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_SETPROMOTION_APPLY_ORDER]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_APPLY_ORDER] = value; }
		}
		#endregion
	}
}
