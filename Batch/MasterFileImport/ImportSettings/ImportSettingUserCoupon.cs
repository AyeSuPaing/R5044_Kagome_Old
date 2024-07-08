/*
=========================================================================================================
  Module      : ユーザクーポン情報取込設定クラス(ImportSettingUserCoupon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Coupon;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// ユーザクーポン情報取込設定クラス
	/// </summary>
	class ImportSettingUserCoupon : ImportSettingBase
	{
		/// <summary>
		/// 更新キーフィールド
		/// </summary>
		private static List<string> FIELDS_UPDKEY = new List<string>
		{
			Constants.FIELD_USERCOUPON_USER_ID,
			Constants.FIELD_USERCOUPON_DEPT_ID,
			Constants.FIELD_USERCOUPON_COUPON_ID,
			Constants.FIELD_USERCOUPON_COUPON_NO
		};
		/// <summary>
		/// 更新禁止フィールド（SQL自動作成除外フィールド）
		/// </summary>
		private static List<string> FIELDS_EXCEPT = new List<string>
		{
			Constants.FIELD_COUPON_COUPON_CODE,
			Constants.FIELD_USERCOUPON_DATE_CHANGED,
			Constants.FIELD_USERCOUPON_LAST_CHANGED
		};
		/// <summary>
		/// 差分更新フィールド（「"add_" + 実フィールド名」がヘッダとして送られる）
		/// </summary>
		private static List<string> FIELDS_INCREASED_UPDATE = new List<string>
		{
		};
		/// <summary>
		/// 必須フィールド（Insert/Update用）
		/// ※更新キーフィールドも含めること
		/// </summary>
		private static List<string> FIELDS_NECESSARY_FOR_INSERTUPDATE = new List<string>
		{
			Constants.FIELD_USERCOUPON_USER_ID,
			Constants.FIELD_USERCOUPON_DEPT_ID,
			Constants.FIELD_USERCOUPON_COUPON_NO
		};
		/// <summary>
		/// 必須フィールド（Delete用）
		/// </summary>
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string>
		{
			Constants.FIELD_USERCOUPON_USER_ID,
			Constants.FIELD_USERCOUPON_DEPT_ID,
			Constants.FIELD_USERCOUPON_COUPON_NO
		};
		/// <summary>
		/// カラム存在チェック除外フィールド
		/// </summary>
		private static List<string> FIELDS_EXCLUSION = new List<string>
		{
			Constants.FIELD_COUPON_COUPON_CODE
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingUserCoupon()
			: base(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.TABLE_USERCOUPON,
				Constants.TABLE_WORKUSERCOUPON,
				FIELDS_UPDKEY,
				FIELDS_EXCEPT,
				FIELDS_INCREASED_UPDATE,
				FIELDS_NECESSARY_FOR_INSERTUPDATE,
				FIELDS_NECESSARY_FOR_DELETE)
		{
			this.ExclusionFields = FIELDS_EXCLUSION;
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			// クーポンID
			var coupon = new CouponService().GetCouponsFromCouponCode(this.ShopId, StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_CODE])).FirstOrDefault();
			this.Data[Constants.FIELD_USERCOUPON_COUPON_ID] = (coupon != null) ? coupon.CouponId : string.Empty;
			// クーポン金額
			if (coupon != null) this.Data[Constants.FIELD_USERCOUPONHISTORY_COUPON_PRICE] = (coupon.DiscountPrice != null) ? coupon.DiscountPrice : 0;

			// ヘッダフィールドに不足している項目を追加
			// クーポンID
			if (this.FieldNamesForUpdateDelete.Any(field => field == Constants.FIELD_USERCOUPON_COUPON_ID) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_USERCOUPON_COUPON_ID);
			}
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			string checkKbn = null;
			var necessaryFields = new List<string>();
			switch (this.Data[Constants.IMPORT_KBN].ToString())
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "UserCouponInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "UserCouponDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			string errorMessages = CheckNecessaryFields(necessaryFields);

			// 発行可能クーポンチェック
			if (this.Data[Constants.IMPORT_KBN].ToString() == Constants.IMPORT_KBN_INSERT_UPDATE)
			{
				var coupon = new CouponService().GetPublishCouponsById(this.ShopId, this.Data[Constants.FIELD_USERCOUPON_COUPON_ID].ToString());
				if (coupon == null) errorMessages += "\r\n" + "[" + this.ShopId + "," + this.Data[Constants.FIELD_USERCOUPON_COUPON_ID].ToString() + "]" + MessageManager.GetMessages(Constants.INPUTCHECK_COUPON_NOT_PUBLISH_COUPON);
			}

			// 入力バリデーション
			string errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = "";

			if (errorMessage != "")
			{
				errorMessages += ((errorMessages.Length != 0) ? "\r\n" : "") + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_USERCOUPON_USER_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_USERCOUPON_COUPON_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_USERCOUPON_COUPON_NO);
			}
			// エラーメッセージ格納
			if (errorMessages.Length != 0)
			{
				this.ErrorMessages = errorMessages.ToString();
			}
		}

		/// <summary>
		/// 必須フィールドチェック
		/// </summary>
		/// <param name="fields">必須フィールド</param>
		/// <returns>エラーメッセージ</returns>
		protected string CheckNecessaryFields(List<string> fields)
		{
			var errorMessages = new StringBuilder();
			var necessaryFields = new StringBuilder();
			foreach (string keyField in fields)
			{
				if (this.HeadersCsv.Contains(keyField) == false)
				{
					necessaryFields.Append((necessaryFields.Length != 0) ? "," : "");
					necessaryFields.Append(keyField);
				}
			}
			if (necessaryFields.Length != 0)
			{
				errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
				errorMessages.Append("該当テーブルの更新にはフィールド「").Append(necessaryFields.ToString()).Append("」が必須です。");
			}
			return errorMessages.ToString();
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";	
		}

		/// <summary>
		/// SQL文作成
		/// </summary>
		public override void CreateSql()
		{
			base.CreateSql();
		}

		/// <summary>
		/// Insert/Update文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>Insert/Update文</returns>
		protected override string CreateInsertUpdateSql(string strTableName)
		{
			var sbResult = new StringBuilder();
			var sbSqlSelect = new StringBuilder();

			// Select文作成
			if (this.UpdateKeys.Count != 0)
			{
				// Select文組み立て
				sbSqlSelect.Append(" SELECT @SELECT_COUNTS = COUNT(*)");
				sbSqlSelect.Append(" FROM ").Append(strTableName);
				sbSqlSelect.Append(CreateWhere());
			}

			// Insert/Update文組み立て
			sbResult.Append(" DECLARE @SELECT_COUNTS int");
			sbResult.Append(sbSqlSelect.ToString());
			sbResult.Append(" IF @SELECT_COUNTS = 0 ");
			sbResult.Append("   BEGIN ");
			sbResult.Append(CreateSelectNextCouponNo(strTableName));
			sbResult.Append(CreateInsertSqlRaw(strTableName));
			sbResult.Append("   END ");
			sbResult.Append(" ELSE ");
			sbResult.Append("   BEGIN ");
			sbResult.Append(CreateUpdateSql(strTableName));
			sbResult.Append("   END ");

			return sbResult.ToString();
		}

		/// <summary>
		/// 枝番採番用SELECT文作成
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <returns>SELECT文</returns>
		private string CreateSelectNextCouponNo(string strTableName)
		{
			var sbResult = new StringBuilder();

			sbResult.Append(" SELECT @coupon_no = ISNULL(MAX(coupon_no), 0) + 1 ");
			sbResult.Append(" FROM ").Append(strTableName);
			sbResult.Append(" WHERE user_id = @user_id ");
			sbResult.Append(" AND dept_id = @dept_id ");

			return sbResult.ToString();
		}

		/// <summary>
		/// ユーザークーポン履歴の登録
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="userCoupon">ユーザークーポン情報</param>
		/// <param name="flgDeleteHistory">削除履歴フラグ</param>
		public void InsertUserCouponHistory(SqlAccessor accessor, Hashtable userCoupon, bool flgDeleteHistory)
		{
			new CouponService().InsertUserCouponHistory(
				(string)userCoupon[Constants.FIELD_USERCOUPONHISTORY_USER_ID],
				StringUtility.ToEmpty(userCoupon[Constants.FIELD_USERCOUPONHISTORY_ORDER_ID]),
				(string)userCoupon[Constants.FIELD_USERCOUPONHISTORY_DEPT_ID],
				(string)userCoupon[Constants.FIELD_USERCOUPONHISTORY_COUPON_ID],
				(string)userCoupon[Constants.FIELD_USERCOUPONHISTORY_COUPON_CODE],
				Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH,
				Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_MASTER_UPLOAD,
				(flgDeleteHistory) ? -1 : 1,
				(flgDeleteHistory) ? (decimal)userCoupon[Constants.FIELD_USERCOUPONHISTORY_COUPON_PRICE] * -1 : (decimal)userCoupon[Constants.FIELD_USERCOUPONHISTORY_COUPON_PRICE],
				Constants.IMPORT_LAST_CHANGED,
				accessor
			);
		}
	}
}
