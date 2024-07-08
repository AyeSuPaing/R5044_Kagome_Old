/*
=========================================================================================================
  Module      : クーポン利用ユーザー情報取込設定クラス(ImportSettingCouponUseUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// クーポン利用ユーザー情報取込設定クラス
	/// </summary>
	public class ImportSettingCouponUseUser
		: ImportSettingBase
	{
		// 更新キーフィールド
		private static readonly List<string> FIELDS_UPDKEY = new List<string>
		{
			Constants.FIELD_COUPONUSEUSER_COUPON_ID,
			Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER
		};
		// 更新禁止フィールド（SQL自動作成除外フィールド）
		private static readonly List<string> FIELDS_EXCEPT = new List<string>
		{
			Constants.FIELD_COUPON_COUPON_CODE,
			Constants.FIELD_COUPONUSEUSER_LAST_CHANGED
		};
		// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		private static readonly List<string> FIELDS_INCREASED_UPDATE = new List<string>
		{
		};
		// 必須フィールド（Insert/Update用）
		// ※更新キーフィールドも含めること
		private static readonly List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string>
		{
			Constants.FIELD_COUPON_COUPON_CODE,
			Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER
		};
		// 必須フィールド（Delete用）
		private static readonly List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string>
		{
			Constants.FIELD_COUPON_COUPON_CODE,
			Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER
		};
		// カラム存在チェック除外フィールド
		private static readonly List<string> FIELDS_EXCLUSION = new List<string>
		{
			Constants.FIELD_COUPON_COUPON_CODE
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingCouponUseUser()
			: base(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.TABLE_COUPONUSEUSER,
				Constants.TABLE_WORKCOUPONUSEUSER,
				FIELDS_UPDKEY,
				FIELDS_EXCEPT,
				FIELDS_INCREASED_UPDATE,
				FIELDS_NECESSARY_FOR_INSERTUPDATE,
				FIELDS_NECESSARY_FOR_DELETE)
		{
			this.ExclusionFields = FIELDS_EXCLUSION;
		}
		
		///<summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		///</summary>
		protected override void ConvertData()
		{
			// ユーザークーポン履歴用データの追加
			// 識別ID（「0」固定 ※店舗IDから取得）
			this.Data[Constants.FIELD_COUPON_DEPT_ID] = this.ShopId;
			// クーポンID
			var coupon = new CouponService().GetCouponsFromCouponCode(this.ShopId, StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_CODE])).FirstOrDefault();
			this.Data[Constants.FIELD_COUPONUSEUSER_COUPON_ID] = (coupon != null) ? coupon.CouponId : string.Empty;
			// ユーザーID
			this.Data[Constants.FIELD_USER_USER_ID] = (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
				? GetUserIdFromOrderOrFixedPurchase()
				: (string)this.Data[Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER];

			// ヘッダフィールドに不足している項目を追加
			// クーポンID
			if (this.FieldNamesForUpdateDelete.Any(field => field == Constants.FIELD_COUPONUSEUSER_COUPON_ID) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_COUPONUSEUSER_COUPON_ID);
			}
		}

		/// <summary>
		/// 注文情報または定期購入情報からユーザーID取得
		/// </summary>
		/// <returns>ユーザーID</returns>
		private string GetUserIdFromOrderOrFixedPurchase()
		{
			// 注文IDかつ定期購入IDが未入力の場合はユーザー情報と紐づけない
			var orderId = StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPONUSEUSER_ORDER_ID]);
			var fixedPurchaseId = StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID]);
			if (string.IsNullOrEmpty(orderId) && string.IsNullOrEmpty(fixedPurchaseId))
			{
				return Constants.COUPONUSEUSER_DEFAULT_BLACKLISTCOUPON_USER;
			}

			// 注文IDがあれば、優先として注文情報からユーザーID取得
			if (string.IsNullOrEmpty(orderId) == false)
			{
				var order = new OrderService().GetOrderByOrderIdAndCouponUseUser(
					orderId,
					StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER]),
					Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE);
				return (order != null) ? order.UserId : Constants.COUPONUSEUSER_DEFAULT_BLACKLISTCOUPON_USER;
			}

			// 注文IDが未入力、かつ定期購入IDが入力の場合、定期台帳からユーザーID取得
			var fixedPurchase = new FixedPurchaseService().GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser(
				fixedPurchaseId,
				StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER]),
				Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE);
			var userId = (fixedPurchase != null) ? fixedPurchase.UserId : Constants.COUPONUSEUSER_DEFAULT_BLACKLISTCOUPON_USER;
			return userId;
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			// 必須チェック
			var errorMessages = new List<string>();
			var necessaryField = (this.Data[Constants.IMPORT_KBN].ToString() == Constants.IMPORT_KBN_INSERT_UPDATE)
				? FIELDS_NECESSARY_FOR_INSERTUPDATE
				: FIELDS_NECESSARY_FOR_DELETE;
			var missingField = necessaryField.Where(field => this.Data.ContainsKey(field) == false);
			errorMessages.Add(missingField.Any() 
				? string.Format("該当テーブルの更新にはフィールド「{0}」が必須です。", string.Join(", ", missingField.ToArray()))
				: string.Empty);
			
			// 入力チェック
			var emptyField = necessaryField.Where(field => ((this.Data.ContainsKey(field) == false) || (string.IsNullOrEmpty(this.Data[field].ToString()))));
			errorMessages.Add(emptyField.Any()
				? string.Format("フィールド「{0}」は空文字にできません。", string.Join(",", emptyField.ToArray()))
				: string.Empty);

			// クーポンコード存在チェック
			if ((this.Data.ContainsKey(Constants.FIELD_COUPON_COUPON_CODE) && (string.IsNullOrEmpty(this.Data[Constants.FIELD_COUPON_COUPON_CODE].ToString())))
				&& (this.Data.ContainsKey(Constants.FIELD_COUPONUSEUSER_COUPON_ID) == false) || (string.IsNullOrEmpty(this.Data[Constants.FIELD_COUPONUSEUSER_COUPON_ID].ToString())))
			{
				errorMessages.Add(string.Format("「{0}」は登録されていないクーポンコードです。", this.Data[Constants.FIELD_COUPON_COUPON_CODE].ToString()));
			}

			
			this.ErrorOccurredIdInfo = string.Empty;
			if (errorMessages.Any(message => string.IsNullOrEmpty(message) == false))
			{
				this.ErrorMessages = string.Join(Environment.NewLine, errorMessages.Where(message => string.IsNullOrEmpty(message) == false).ToArray());
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_COUPON_COUPON_CODE);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER);
			}
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns></returns>
		public override string CheckDataConsistency()
		{
			// 処理なし
			return string.Empty;
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			base.CreateSql();
		}

		/// <summary>
		/// ユーザークーポン履歴の登録
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		public void InsertUserCouponHistory(SqlAccessor accessor, Hashtable couponUseUser)
		{
			var orderId = StringUtility.ToEmpty(couponUseUser[Constants.FIELD_COUPONUSEUSER_ORDER_ID]);
			var fixedPurchaseId = StringUtility.ToEmpty(couponUseUser[Constants.FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID]);
			new CouponService().InsertUserCouponHistory(
				(string)couponUseUser[Constants.FIELD_USER_USER_ID],
				orderId,
				(string)couponUseUser[Constants.FIELD_COUPON_DEPT_ID],
				(string)couponUseUser[Constants.FIELD_COUPONUSEUSER_COUPON_ID],
				(string)couponUseUser[Constants.FIELD_COUPON_COUPON_CODE],
				(string.IsNullOrEmpty(orderId) && (string.IsNullOrEmpty(fixedPurchaseId) == false))
					? Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE
					: Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE,
				Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_OPERATOR,
				-1,
				0,
				Constants.IMPORT_LAST_CHANGED,
				accessor,
				fixedPurchaseId);
		}
	}
}
