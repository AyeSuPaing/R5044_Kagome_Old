/*
=========================================================================================================
  Module      : クーポン入力情報(CouponInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Input;
using w2.Domain.Coupon;
using w2.Domain.Product;

namespace Input.Coupon
{
	/// <summary>
	/// クーポン入力情報
	/// </summary>
	[Serializable]
	public class CouponInput : InputBase<CouponModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CouponInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public CouponInput(CouponModel model)
			: this()
		{
			if (model != null)
			{
				this.DeptId = model.DeptId;
				this.CouponId = model.CouponId;
				this.CouponCode = model.CouponCode;
				this.CouponName = model.CouponName;
				this.CouponDispName = model.CouponDispName;
				this.CouponDispNameMobile = model.CouponDispNameMobile;
				this.CouponDiscription = model.CouponDiscription;
				this.CouponDiscriptionMobile = model.CouponDiscriptionMobile;
				this.CouponType = model.CouponType;
				this.CouponCount = model.CouponCount.ToString();
				this.PublishDateBgn = model.PublishDateBgn.ToString();
				this.PublishDateEnd = model.PublishDateEnd.ToString();
				this.DiscountPrice = (model.DiscountPrice != null) ? model.DiscountPrice.ToString() : null;
				this.DiscountRate = (model.DiscountRate != null) ? model.DiscountRate.ToString() : null;
				this.ExpireDay = (model.ExpireDay != null) ? model.ExpireDay.ToString() : null;
				this.ExpireDateBgn = (model.ExpireDateBgn != null) ? model.ExpireDateBgn.ToString() : null;
				this.ExpireDateEnd = (model.ExpireDateEnd != null) ? model.ExpireDateEnd.ToString() : null;
				this.ProductKbn = model.ProductKbn;
				this.ExceptionalProduct = model.ExceptionalProduct;
				this.ExceptionalIcon = model.ExceptionalIcon.ToString();
				this.ExceptionalIcon1 = model.ExceptionalIcon1.ToString();
				this.ExceptionalIcon2 = model.ExceptionalIcon2.ToString();
				this.ExceptionalIcon3 = model.ExceptionalIcon3.ToString();
				this.ExceptionalIcon4 = model.ExceptionalIcon4.ToString();
				this.ExceptionalIcon5 = model.ExceptionalIcon5.ToString();
				this.ExceptionalIcon6 = model.ExceptionalIcon6.ToString();
				this.ExceptionalIcon7 = model.ExceptionalIcon7.ToString();
				this.ExceptionalIcon8 = model.ExceptionalIcon8.ToString();
				this.ExceptionalIcon9 = model.ExceptionalIcon9.ToString();
				this.ExceptionalIcon10 = model.ExceptionalIcon10.ToString();
				this.UsablePrice = (model.UsablePrice != null) ? model.UsablePrice.ToString() : null;
				this.UseTogetherFlg = model.UseTogetherFlg;
				this.ValidFlg = model.ValidFlg;
				this.DateCreated = model.DateCreated.ToString();
				this.DateChanged = model.DateChanged.ToString();
				this.LastChanged = model.LastChanged;
				this.DispFlg = model.DispFlg;
				this.CouponDispDiscription = model.CouponDispDiscription;
				this.FreeShippingFlg = model.FreeShippingFlg;
				this.ExceptionalBrandIds = model.ExceptionalBrandIds;
				this.ExceptionalProductCategoryIds = model.ExceptionalProductCategoryIds;
			}
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override CouponModel CreateModel()
		{
			return new CouponModel()
			{
				DeptId = this.DeptId,
				CouponId = this.CouponId,
				CouponCode = this.CouponCode,
				CouponName = this.CouponName,
				CouponDispName = this.CouponDispName,
				CouponDispNameMobile = this.CouponDispNameMobile,
				CouponDiscription = this.CouponDiscription,
				CouponDiscriptionMobile = this.CouponDiscriptionMobile,
				CouponType = this.CouponType,
				CouponCount = int.Parse(this.CouponCount),
				PublishDateBgn = DateTime.Parse(this.PublishDateBgn),
				PublishDateEnd = DateTime.Parse(this.PublishDateEnd),
				DiscountPrice = string.IsNullOrEmpty(this.DiscountPrice) ? (decimal?)null : decimal.Parse(this.DiscountPrice),
				DiscountRate = string.IsNullOrEmpty(this.DiscountRate) ? (decimal?)null : decimal.Parse(this.DiscountRate),
				ExpireDay = string.IsNullOrEmpty(this.ExpireDay) ? (int?)null : int.Parse(this.ExpireDay),
				ExpireDateBgn = string.IsNullOrEmpty(this.ExpireDateBgn) ? (DateTime?)null : DateTime.Parse(this.ExpireDateBgn),
				ExpireDateEnd = string.IsNullOrEmpty(this.ExpireDateEnd) ? (DateTime?)null : DateTime.Parse(this.ExpireDateEnd),
				ProductKbn = this.ProductKbn,
				ExceptionalProduct = this.ExceptionalProduct,
				ExceptionalIcon = int.Parse(this.ExceptionalIcon),
				UsablePrice = string.IsNullOrEmpty(this.UsablePrice) ? (decimal?)null : decimal.Parse(this.UsablePrice),
				UseTogetherFlg = this.UseTogetherFlg,
				ValidFlg = this.ValidFlg,
				LastChanged = this.LastChanged,
				DispFlg = this.DispFlg,
				CouponDispDiscription = this.CouponDispDiscription,
				FreeShippingFlg = this.FreeShippingFlg,
				ExceptionalBrandIds = this.ExceptionalBrandIds,
				ExceptionalProductCategoryIds = this.ExceptionalProductCategoryIds,
			};
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="validatorKey">バリデータのKey</param>
		/// <returns></returns>
		public string Validate(string validatorKey)
		{
			var errorMessages = new StringBuilder();
			errorMessages.Append(Validator.Validate(validatorKey, this.DataSource));
			if (Validator.CheckDateRange(this.PublishDateBgn, this.PublishDateEnd) == false)
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE).Replace("@@ 1 @@", "発行期間")).Append("<br />");
			}
			if (((string.IsNullOrEmpty(this.ExpireDateBgn) == false) && (string.IsNullOrEmpty(this.ExpireDateEnd) == false))
				&& (Validator.CheckDateRange(this.ExpireDateBgn, this.ExpireDateEnd) == false))
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE).Replace("@@ 1 @@", "有効期間")).Append("<br />");
			}

			// 「対象商品を限定する(または)」指定する場合、商品IDとキャンペーンアイコンを指定しているかをチェック
			if (this.IsProductUnTarget
				&& string.IsNullOrEmpty(this.ExceptionalProduct)
				&& (this.IsSelectedProductIcon == false))
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_CHECK)).Append("<br />");
			}

			// 「対象商品を限定する(かつ)」指定する場合、商品IDとキャンペーンアイコンを指定しているかをチェック
			if (this.IsProductUnTargetByLogicalAnd
				&& string.IsNullOrEmpty(this.ExceptionalBrandIds)
				&& string.IsNullOrEmpty(this.ExceptionalProductCategoryIds)
				&& (this.IsSelectedProductIcon == false))
			{
				errorMessages.Append(
					WebMessages.GetMessages(
						Constants.PRODUCT_BRAND_ENABLED
							? WebMessages.ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_BY_LOGICAL_AND_CHECK_BRAND
							: WebMessages.ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_BY_LOGICAL_AND_CHECK))
					.Append("<br />");
			}

			return errorMessages.ToString();
		}

		/// <summary>
		/// 商品存在チェック
		/// </summary>
		/// <param name="exceptionalProduct">クーポン例外商品</param>
		/// <returns></returns>
		public string CheckProductIds(string exceptionalProduct)
		{
			var errorMessages = new StringBuilder();

			foreach (var exceptionalProductId in exceptionalProduct.Split(','))
			{
				var product = new ProductService().Get(Constants.CONST_DEFAULT_SHOP_ID, exceptionalProductId);
				if (product == null)
				{
					errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_PRODUCT_ID_NOT_EXISTS)).Replace("@@ 1 @@", exceptionalProductId);
					continue;
				}

				if (product.ValidFlg == Constants.FLG_OFF)
				{
					errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_PRODUCT_INVALID).Replace("@@ 1 @@", exceptionalProductId));
				}
			}

			return errorMessages.ToString();
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_COUPON_DEPT_ID] = value; }
		}

		/// <summary>クーポンID</summary>
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_ID] = value; }
		}

		/// <summary>クーポンコード</summary>
		public string CouponCode
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_CODE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_CODE] = value; }
		}

		/// <summary>管理用クーポン名</summary>
		public string CouponName
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_NAME]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_NAME] = value; }
		}

		/// <summary>表示用クーポン名</summary>
		public string CouponDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME] = value; }
		}

		/// <summary>モバイル用表示用クーポン名</summary>
		public string CouponDispNameMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME_MOBILE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME_MOBILE] = value; }
		}

		/// <summary>クーポン説明文</summary>
		public string CouponDiscription
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION] = value; }
		}

		/// <summary>モバイル用クーポン説明文</summary>
		public string CouponDiscriptionMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION_MOBILE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION_MOBILE] = value; }
		}

		/// <summary>クーポン種別</summary>
		public string CouponType
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_TYPE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_TYPE] = value; }
		}

		/// <summary>クーポン利用可能回数</summary>
		public string CouponCount
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_COUNT]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_COUNT] = value; }
		}

		/// <summary>クーポン発行期間(開始)</summary>
		public string PublishDateBgn
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_BGN]; }
			set { this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_BGN] = value; }
		}

		/// <summary>クーポン発行期間(終了)</summary>
		public string PublishDateEnd
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_END]; }
			set { this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_END] = value; }
		}

		/// <summary>クーポン商品割引額</summary>
		public string DiscountPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DISCOUNT_PRICE]; }
			set { this.DataSource[Constants.FIELD_COUPON_DISCOUNT_PRICE] = value; }
		}

		/// <summary>クーポン商品割引率</summary>
		public string DiscountRate
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DISCOUNT_RATE]; }
			set { this.DataSource[Constants.FIELD_COUPON_DISCOUNT_RATE] = value; }
		}

		/// <summary>クーポン有効期限(日)</summary>
		public string ExpireDay
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXPIRE_DAY]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXPIRE_DAY] = value; }
		}

		/// <summary>クーポン有効期間(開始)</summary>
		public string ExpireDateBgn
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_BGN]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_BGN] = value; }
		}

		/// <summary>クーポン有効期間(終了)</summary>
		public string ExpireDateEnd
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_END]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_END] = value; }
		}

		/// <summary>クーポン対象商品区分</summary>
		public string ProductKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_PRODUCT_KBN]; }
			set { this.DataSource[Constants.FIELD_COUPON_PRODUCT_KBN] = value; }
		}

		/// <summary>クーポン例外商品</summary>
		public string ExceptionalProduct
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT] = value; }
		}

		/// <summary>クーポン例外商品アイコン</summary>
		public string ExceptionalIcon
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON] = value; }
		}

		/// <summary>クーポン利用最低購入金額</summary>
		public string UsablePrice
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_USABLE_PRICE]; }
			set { this.DataSource[Constants.FIELD_COUPON_USABLE_PRICE] = value; }
		}

		/// <summary>クーポン併用フラグ</summary>
		public string UseTogetherFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_USE_TOGETHER_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPON_USE_TOGETHER_FLG] = value; }
		}

		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPON_VALID_FLG] = value; }
		}

		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COUPON_DATE_CREATED] = value; }
		}

		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUPON_DATE_CHANGED] = value; }
		}

		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUPON_LAST_CHANGED] = value; }
		}

		/// <summary>フロント表示フラグ</summary>
		public string DispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPON_DISP_FLG] = value; }
		}

		/// <summary>クーポン説明文(ユーザ表示用)</summary>
		public string CouponDispDiscription
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_DISCRIPTION] = value; }
		}
		/// <summary>クーポン配送料無料フラグ</summary>
		public string FreeShippingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_FREE_SHIPPING_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPON_FREE_SHIPPING_FLG] = value; }
		}

		/// <summary>クーポン例外ブランドID</summary>
		public string ExceptionalBrandIds
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_BRAND_IDS]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_BRAND_IDS] = value; }
		}

		/// <summary>クーポン例外商品カテゴリID</summary>
		public string ExceptionalProductCategoryIds
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT_CATEGORY_IDS]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT_CATEGORY_IDS] = value; }
		}
		#endregion

		#region 拡張プロパティ
		/// <summary>古いクーポンコード</summary>
		public string OldCouponCode
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_CODE + "_old"]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_CODE + "_old"] = value; }
		}

		/// <summary>クーポン有効期限・期間</summary>
		public string Expire
		{
			get { return (string)this.DataSource["expire"]; }
			set { this.DataSource["expire"] = value; }
		}

		/// <summary>クーポン例外商品アイコン1</summary>
		public string ExceptionalIcon1
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "1"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "1"] = value; }
		}

		/// <summary>クーポン例外商品アイコン2</summary>
		public string ExceptionalIcon2
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "2"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "2"] = value; }
		}

		/// <summary>クーポン例外商品アイコン3</summary>
		public string ExceptionalIcon3
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "3"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "3"] = value; }
		}

		/// <summary>クーポン例外商品アイコン4</summary>
		public string ExceptionalIcon4
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "4"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "4"] = value; }
		}

		/// <summary>クーポン例外商品アイコン5</summary>
		public string ExceptionalIcon5
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "5"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "5"] = value; }
		}

		/// <summary>クーポン例外商品アイコン6</summary>
		public string ExceptionalIcon6
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "6"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "6"] = value; }
		}

		/// <summary>クーポン例外商品アイコン7</summary>
		public string ExceptionalIcon7
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "7"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "7"] = value; }
		}

		/// <summary>クーポン例外商品アイコン8</summary>
		public string ExceptionalIcon8
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "8"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "8"] = value; }
		}

		/// <summary>クーポン例外商品アイコン9</summary>
		public string ExceptionalIcon9
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "9"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "9"] = value; }
		}

		/// <summary>クーポン例外商品アイコン10</summary>
		public string ExceptionalIcon10
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "10"]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON + "10"] = value; }
		}

		/// <summary>「対象商品を限定する(または)」指定するか</summary>
		public bool IsProductUnTarget
		{
			get { return (this.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET); }
		}

		/// <summary>「対象商品を限定する(かつ)」指定するか</summary>
		public bool IsProductUnTargetByLogicalAnd
		{
			get { return (this.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND); }
		}

		/// <summary>キャンペーンアイコンが指定されているか</summary>
		public bool IsSelectedProductIcon
		{
			get
			{
				int exceptionalIcon;
				if (int.TryParse(this.ExceptionalIcon, out exceptionalIcon) == false) return false;
				return (exceptionalIcon > 0);
			}
		}
		#endregion
	}
}