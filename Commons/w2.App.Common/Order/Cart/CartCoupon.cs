/*
=========================================================================================================
  Module      : カートクーポンクラス(CartCoupon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Option;
using w2.Domain.Coupon.Helper;
using w2.Domain.Order;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カートクーポンクラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class CartCoupon
	{
		/// <summary>クーポン割引区分</summary>
		public enum CouponDiscountKbn
		{
			/// <summary>割引金額</summary>
			Price,
			/// <summary>割引率</summary>
			Rate,
			/// <summary>配送無料</summary>
			FreeShipping
		}
		/// <summary>クーポン有効期限・期間区分</summary>
		public enum CouponExpireKbn
		{
			/// <summary>有効期限</summary>
			Day,
			/// <summary>有効期間</summary>
			Term
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CartCoupon()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userCoupon">クーポン情報</param>
		public CartCoupon(UserCouponDetailInfo userCoupon)
		{
			this.UseTogetherFlg = Constants.FLG_COUPON_USE_TOGETHER_FLG_UNUSE;

			// データセット
			this.UpdateCoupon(userCoupon);
		}

		/// <summary>
		/// クーポン情報更新
		/// </summary>
		/// <param name="userCoupon">クーポン情報</param>
		public void UpdateCoupon(UserCouponDetailInfo userCoupon)
		{
			//------------------------------------------------------
			// クーポン情報セット
			//------------------------------------------------------
			this.DeptId = userCoupon.DeptId;
			this.CouponId = userCoupon.CouponId;
			this.CouponCode = userCoupon.CouponCode;
			this.CouponName = userCoupon.CouponName;
			this.CouponDispName = userCoupon.CouponDispName;
			this.CouponDispNameMobile = userCoupon.CouponDispNameMobile;
			this.CouponDiscription = userCoupon.CouponDiscription;
			this.CouponDiscriptionMobile = userCoupon.CouponDiscriptionMobile;
			this.CouponType = userCoupon.CouponType;
			this.PublishDateBgn = userCoupon.PublishDateBgn;
			this.PublishDateEnd = userCoupon.PublishDateEnd;

			// 配送無料クーポンの場合
			if (IsFreeShipping() || IsFreeShippingDiscountMoneyCouponUse())
			{
				this.DiscountKbn = CouponDiscountKbn.FreeShipping;
				this.DiscountPrice = 0;
				this.DiscountRate = 0;
			}
			// クーポン割引額指定の場合
			else if (userCoupon.DiscountPrice != null)
			{
				this.DiscountKbn = CouponDiscountKbn.Price;
				this.DiscountPrice = userCoupon.DiscountPrice.GetValueOrDefault();
				this.DiscountRate = 0;
			}
			// クーポン割引率指定の場合
			else if (userCoupon.DiscountRate != null)
			{
				this.DiscountKbn = CouponDiscountKbn.Rate;
				this.DiscountPrice = 0;
				this.DiscountRate = userCoupon.DiscountRate.GetValueOrDefault();
			}

			this.ExpireKbn = userCoupon.ExpireDay != null ? CouponExpireKbn.Day : CouponExpireKbn.Term;
			switch (this.ExpireKbn)
			{
				// 有効期限指定の場合
				case CouponExpireKbn.Day:
					this.ExpireDay = userCoupon.ExpireDay.GetValueOrDefault();
					this.ExpireDateBgn = DateTime.MinValue;
					this.ExpireDateEnd = DateTime.MinValue;
					break;

				// 有効期間指定の場合
				case CouponExpireKbn.Term:
					this.ExpireDay = 0;
					this.ExpireDateBgn = userCoupon.ExpireDateBgn.GetValueOrDefault();
					this.ExpireDateEnd = userCoupon.ExpireDateEnd.GetValueOrDefault();
					break;
			}

			this.ProductKbn = userCoupon.ProductKbn;
			this.ExceptionalProduct = userCoupon.ExceptionalProduct;
			this.ExceptionalIcon1 = userCoupon.ExceptionalIcon1 == Constants.FLG_COUPON_EXCEPTIONAL_ICON1;
			this.ExceptionalIcon2 = userCoupon.ExceptionalIcon2 == Constants.FLG_COUPON_EXCEPTIONAL_ICON2;
			this.ExceptionalIcon3 = userCoupon.ExceptionalIcon3 == Constants.FLG_COUPON_EXCEPTIONAL_ICON3;
			this.ExceptionalIcon4 = userCoupon.ExceptionalIcon4 == Constants.FLG_COUPON_EXCEPTIONAL_ICON4;
			this.ExceptionalIcon5 = userCoupon.ExceptionalIcon5 == Constants.FLG_COUPON_EXCEPTIONAL_ICON5;
			this.ExceptionalIcon6 = userCoupon.ExceptionalIcon6 == Constants.FLG_COUPON_EXCEPTIONAL_ICON6;
			this.ExceptionalIcon7 = userCoupon.ExceptionalIcon7 == Constants.FLG_COUPON_EXCEPTIONAL_ICON7;
			this.ExceptionalIcon8 = userCoupon.ExceptionalIcon8 == Constants.FLG_COUPON_EXCEPTIONAL_ICON8;
			this.ExceptionalIcon9 = userCoupon.ExceptionalIcon9 == Constants.FLG_COUPON_EXCEPTIONAL_ICON9;
			this.ExceptionalIcon10 = userCoupon.ExceptionalIcon10 == Constants.FLG_COUPON_EXCEPTIONAL_ICON10;

			this.UsablePriceFlg = (userCoupon.UsablePrice != null);
			this.UsablePrice = (this.UsablePriceFlg) ? userCoupon.UsablePrice.GetValueOrDefault() : 0;

			this.UseTogetherFlg = userCoupon.UseTogetherFlg;
			this.ValidFlg = userCoupon.ValidFlg;

			//------------------------------------------------------
			// ユーザクーポン情報
			//------------------------------------------------------
			if (userCoupon.UserId != null)
			{
				this.UserId = userCoupon.UserId;
				this.CouponNo = userCoupon.CouponNo.GetValueOrDefault();
				this.UseFlg = userCoupon.UseFlg;
			}
			else
			{
				this.UserId = null;
				this.CouponNo = 0;
				this.UseFlg = Constants.FLG_USERCOUPON_USE_FLG_UNUSE;
			}

			this.DateCreated = userCoupon.DateCreated;
			this.UserCouponCount = userCoupon.UserCouponCount;
			this.FreeShippingFlg = userCoupon.FreeShippingFlg;
		}

		/// <summary>
		/// 配送料無料クーポン？
		/// </summary>
		/// <returns>true:配送料無料クーポン</returns>
		public bool IsFreeShipping()
		{
			return CouponOptionUtility.IsFreeShipping(this.CouponType);
		}

		/// <summary>
		/// 配送料無料金額割引クーポン利用判定
		/// </summary>
		/// <returns>配送料無料利用フラグ</returns>
		public bool IsFreeShippingDiscountMoneyCouponUse()
		{
			return CouponOptionUtility.IsFreeShippingDiscountMoney(this.FreeShippingFlg, this.DiscountPrice, this.DiscountRate);
		}

		/// <summary>
		/// 発行者向け回数制限有りクーポン?
		/// </summary>
		/// <returns>True:制限有り、False：制限なし</returns>
		public bool IsCouponLimit()
		{
			return CouponOptionUtility.IsCouponLimit(this.CouponType);
		}

		/// <summary>
		/// 会員限定回数制限付きクーポン？
		/// </summary>
		/// <returns>会員限定回数制限付きクーポンならTrue</returns>
		public bool IsCouponLimitedForRegisteredUser()
		{
			return CouponOptionUtility.IsCouponLimitedForRegisteredUser(this.CouponType);
		}

		/// <summary>
		/// 全員向け回数制限有りクーポン？
		/// </summary>
		/// <returns>True:制限有り、False：制限なし</returns>
		public bool IsCouponAllLimit()
		{
			return CouponOptionUtility.IsCouponAllLimit(this.CouponType);
		}
		
		/// <summary>
		/// ブラックリスト型クーポン？
		/// </summary>
		/// <returns>true:ブラックリスト型クーポン</returns>
		public bool IsBlacklistCoupon()
		{
			return CouponOptionUtility.IsBlacklistCoupon(this.CouponType);
		}

		/// <summary>
		/// 注文クーポンモデル生成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="isPc">モバイル注文？</param>
		/// <returns>注文クーポンモデル</returns>
		public OrderCouponModel CreateModel(string orderId, bool isMobile)
		{
			var orderCoupon = new OrderCouponModel
			{
				DeptId = this.DeptId,
				OrderId = orderId,
				CouponId = this.CouponId,
				CouponCode = this.CouponCode,
				CouponName = this.CouponName,
				CouponDispName = isMobile
					? this.CouponDispNameMobile
					: this.CouponDispName,
				CouponType = this.CouponType,
				CouponNo = this.CouponNo,
				CouponDiscountPrice = this.DiscountPrice,
				CouponDiscountRate = this.DiscountRate,
				DateChanged = DateTime.Now,
				DateCreated = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_USER
			};
			return orderCoupon;
		}

		/// <summary>識別ID</summary>
		public string DeptId { get; set; }
		/// <summary>クーポンID</summary>
		public string CouponId { get; set; }
		/// <summary>クーポンコード</summary>
		public string CouponCode { get; set; }
		/// <summary>管理用クーポン名</summary>
		public string CouponName { get; set; }
		/// <summary>表示用クーポン名</summary>
		public string CouponDispName { get; set; }
		/// <summary>モバイル用表示用クーポン名</summary>
		public string CouponDispNameMobile { get; set; }
		/// <summary>クーポン説明文</summary>
		public string CouponDiscription { get; set; }
		/// <summary>モバイル用クーポン説明文</summary>
		public string CouponDiscriptionMobile { get; set; }
		/// <summary>クーポン種別</summary>
		public string CouponType { get; set; }
		/// <summary>クーポン発行期間(開始)</summary>
		public DateTime PublishDateBgn { get; set; }
		/// <summary>クーポン発行期間(終了)</summary>
		public DateTime PublishDateEnd { get; set; }
		/// <summary>クーポン割引区分</summary>
		public CouponDiscountKbn DiscountKbn { get; set; }
		/// <summary>クーポン割引額</summary>
		public decimal DiscountPrice { get; set; }
		/// <summary>クーポン割引率</summary>
		public decimal DiscountRate { get; set; }
		/// <summary>クーポン有効期限・期間区分</summary>
		public CouponExpireKbn ExpireKbn { get; set; }
		/// <summary>クーポン有効期限(日)</summary>
		public int ExpireDay { get; set; }
		/// <summary>クーポン有効期間(開始)</summary>
		public DateTime ExpireDateBgn { get; set; }
		/// <summary>クーポン有効期間(終了)</summary>
		public DateTime ExpireDateEnd { get; set; }
		/// <summary>クーポン対象商品区分</summary>
		public string ProductKbn { get; set; }
		/// <summary>クーポン例外商品</summary>
		public string ExceptionalProduct { get; set; }
		/// <summary>クーポン例外商品アイコン1-10</summary>
		public bool ExceptionalIcon1 { get; set; }
		public bool ExceptionalIcon2 { get; set; }
		public bool ExceptionalIcon3 { get; set; }
		public bool ExceptionalIcon4 { get; set; }
		public bool ExceptionalIcon5 { get; set; }
		public bool ExceptionalIcon6 { get; set; }
		public bool ExceptionalIcon7 { get; set; }
		public bool ExceptionalIcon8 { get; set; }
		public bool ExceptionalIcon9 { get; set; }
		public bool ExceptionalIcon10 { get; set; }
		/// <summary>クーポン利用最低購入金額フラグ</summary>
		public bool UsablePriceFlg { get; set; }
		/// <summary>クーポン利用最低購入金額</summary>
		public decimal UsablePrice { get; set; }
		/// <summary>クーポン併用フラグ</summary>
		public string UseTogetherFlg { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>枝番</summary>
		public int CouponNo { get; set; }
		/// <summary>利用フラグ</summary>
		public string UseFlg { get; set; }
		/// <summary>作成日</summary>
		public DateTime DateCreated { get; set; }
		/// <summary>ユーザークーポン利用回数</summary>
		public int? UserCouponCount { get; set; }
		/// <summary>配送料無料</summary>
		public string FreeShippingFlg { get; set; }
	}
}
