/*
=========================================================================================================
  Module      : クーポン利用ユーザテーブル入力クラス (CouponUseUserInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;

/// <summary>
/// クーポン利用ユーザテーブル入力クラス
/// </summary>
public class CouponUseUserInput : InputBase<CouponUseUserModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public CouponUseUserInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public CouponUseUserInput(CouponUseUserModel model)
		: this()
	{
		this.CouponId = model.CouponId;
		this.OrderId = model.OrderId;
		this.CouponUseUser = model.CouponUseUser;
		this.DateCreated = model.DateCreated.ToString();
		this.LastChanged = model.LastChanged;
		this.FixedPurchaseId = model.FixedPurchaseId;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override CouponUseUserModel CreateModel()
	{
		var model = new CouponUseUserModel
		{
			CouponId = this.CouponId,
			OrderId = this.OrderId,
			CouponUseUser = this.CouponUseUser,
			LastChanged = this.LastChanged,
			FixedPurchaseId = this.FixedPurchaseId,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var errorMessages = new List<string>();
		var usedUserJudgeType = ValueText.GetValueText(
			Constants.TABLE_COUPONUSEUSER,
			Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE,
			Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE);
		if (string.IsNullOrEmpty(this.CouponUseUser))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY).Replace("@@ 1 @@", usedUserJudgeType));
		}
		else if (new CouponService().CheckUsedCoupon(this.CouponId, this.CouponUseUser))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPONUSEUSER_BLACKLISTCOUPON_IS_USED));
		}
		if ((string.IsNullOrEmpty(this.OrderId) == false)
			&& (new OrderService().GetOrderByOrderIdAndCouponUseUser(this.OrderId, this.CouponUseUser, Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE) == null))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPONUSEUSER_IRREGULAR_ORDER_OWNER));
		}
		if ((string.IsNullOrEmpty(this.FixedPurchaseId) == false)
			&& (new FixedPurchaseService().GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser(
				this.FixedPurchaseId,
				this.CouponUseUser,
				Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE) == null))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPONUSEUSER_IRREGULAR_FIXEDPURCHASE_OWNER));
		}
		
		return string.Join("<br />", errorMessages.Where(message => string.IsNullOrEmpty(message) == false).ToArray());
	}
	#endregion

	#region プロパティ
	/// <summary>クーポンID</summary>
	public string CouponId
	{
		get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_ID]; }
		set { this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_ID] = value; }
	}
	/// <summary>クーポン利用ユーザー</summary>
	public string CouponUseUser
	{
		get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER]; }
		set { this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER] = value; }
	}
	/// <summary>注文ID</summary>
	public string OrderId
	{
		get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_ORDER_ID]; }
		set { this.DataSource[Constants.FIELD_COUPONUSEUSER_ORDER_ID] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_COUPONUSEUSER_DATE_CREATED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_COUPONUSEUSER_LAST_CHANGED] = value; }
	}
	/// <summary>定期購入ID</summary>
	public string FixedPurchaseId
	{
		get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID]; }
		set { this.DataSource[Constants.FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID] = value; }
	}
	#endregion
}
