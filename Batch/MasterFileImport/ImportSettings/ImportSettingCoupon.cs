/*
=========================================================================================================
  Module      : クーポン情報取込設定クラス(ImportSettingCoupon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Option;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Coupon;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	/// <summary>
	/// クーポン情報取込設定クラス
	/// </summary>
	public class ImportSettingCoupon : ImportSettingBase
	{
		/// <summary>
		/// 更新キーフィールド
		/// </summary>
		private static List<string> FIELDS_UPDKEY = new List<string>
		{
			Constants.FIELD_COUPON_DEPT_ID,
			Constants.FIELD_COUPON_COUPON_CODE
		};
		/// <summary>
		/// 更新禁止フィールド（SQL自動作成除外フィールド）
		/// </summary>
		private static List<string> FIELDS_EXCEPT = new List<string>
		{
			Constants.FIELD_COUPON_DATE_CHANGED,
			Constants.FIELD_COUPON_LAST_CHANGED
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
			Constants.FIELD_COUPON_DEPT_ID,
			Constants.FIELD_COUPON_COUPON_CODE
		};
		/// <summary>
		/// 必須フィールド（Delete用）
		/// </summary>
		private static List<string> FIELDS_NECESSARY_FOR_DELETE = new List<string>
		{
			Constants.FIELD_COUPON_DEPT_ID,
			Constants.FIELD_COUPON_COUPON_CODE
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportSettingCoupon()
			: base(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.TABLE_COUPON,
				Constants.TABLE_WORKCOUPON,
				FIELDS_UPDKEY,
				FIELDS_EXCEPT,
				FIELDS_INCREASED_UPDATE,
				FIELDS_NECESSARY_FOR_INSERTUPDATE,
				FIELDS_NECESSARY_FOR_DELETE)
		{
		}

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			// クーポンコードをTrimする
			this.Data[Constants.FIELD_COUPON_COUPON_CODE] = StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_CODE]).Trim();
			// クーポン利用可能回数の初期値設定
			if (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_COUNT]))) this.Data[Constants.FIELD_COUPON_COUPON_COUNT] = "0";
			// クーポン例外商品アイコンの初期値設定
			if (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_EXCEPTIONAL_ICON]))) this.Data[Constants.FIELD_COUPON_EXCEPTIONAL_ICON] = "0";
			// クーポン併用フラグの初期値設定
			if (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_USE_TOGETHER_FLG]))) this.Data[Constants.FIELD_COUPON_USE_TOGETHER_FLG] = "0";

			// クーポンID(取得できない場合は採番テーブルより採番する)
			var coupon = new CouponService().GetCouponFromCouponCodePerfectMatch(this.ShopId, StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_CODE]));
			this.Data[Constants.FIELD_COUPON_COUPON_ID] = (coupon != null) ? coupon.CouponId : CouponOptionUtility.CreateNewCouponId();

			// 更新の場合(既存クーポン情報が取得できた場合)、クーポン種別を取得したクーポンのクーポン種別で上書きする(クーポン種別は更新不可のため)
			this.Data[Constants.FIELD_COUPON_COUPON_TYPE] = (coupon != null) ? coupon.CouponType : this.Data[Constants.FIELD_COUPON_COUPON_TYPE];

			// 配送料無料クーポンの場合、割引額・割引率の入力値を削除
			if (CouponOptionUtility.IsFreeShipping(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_TYPE])))
			{
				this.Data[Constants.FIELD_COUPON_FREE_SHIPPING_FLG] = Constants.FLG_COUPON_FREE_SHIPPING_INVALID;
				this.Data[Constants.FIELD_COUPON_DISCOUNT_PRICE] = null;
				this.Data[Constants.FIELD_COUPON_DISCOUNT_RATE] = null;
			}
			// 利用回数制限クーポンでない場合、利用可能回数を0に設定
			else if ((StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_TYPE]) != Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT)
				&& (StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_TYPE]) != Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING))
			{
				this.Data[Constants.FIELD_COUPON_COUPON_COUNT] = "0";
			}

			// ヘッダフィールドに不足している項目を追加
			// クーポンID
			if (this.FieldNamesForUpdateDelete.Any(field => field == Constants.FIELD_COUPON_COUPON_ID) == false)
			{
				this.FieldNamesForUpdateDelete.Add(Constants.FIELD_COUPON_COUPON_ID);
			}
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			string checkKbn = null;
			var necessaryFields = new List<string>();
			switch (StringUtility.ToEmpty(this.Data[Constants.IMPORT_KBN]))
			{
				// Insert/Update
				case Constants.IMPORT_KBN_INSERT_UPDATE:
					checkKbn = "CouponInsertUpdate";
					necessaryFields = this.InsertUpdateNecessaryFields;
					break;

				// Delete
				case Constants.IMPORT_KBN_DELETE:
					checkKbn = "CouponDelete";
					necessaryFields = this.DeleteNecessaryFields;
					break;
			}

			// 必須フィールドチェック
			string errorMessages = CheckNecessaryFields(necessaryFields);

			if (StringUtility.ToEmpty(this.Data[Constants.IMPORT_KBN]) == Constants.IMPORT_KBN_DELETE)
			{
				// ユーザクーポン情報に設定されていたら削除不可
				var userCoupon = new CouponService().GetUserCouponFromCouponId
				(
					StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_DEPT_ID]),
					StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_ID])
				);
				if (userCoupon != null)
				{
					errorMessages += "\r\n" + MessageManager.GetMessages(Constants.INPUTCHECK_COUPON_DELETE_IMPOSSIBLE);
				}
			}
			else
			{
				// 配送料無料クーポン以外の場合のみ、割引額・割引率チェックを行う
				if (CouponOptionUtility.IsFreeShipping(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_COUPON_TYPE])) == false)
				{
					// クーポン割引額また割引率必須チェック
					if ((this.Data.ContainsKey(Constants.FIELD_COUPON_DISCOUNT_PRICE)
						|| this.Data.ContainsKey(Constants.FIELD_COUPON_DISCOUNT_RATE))
						|| this.Data.ContainsKey(Constants.FIELD_COUPON_FREE_SHIPPING_FLG))
					{
						if (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_DISCOUNT_PRICE]))
							&& string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_DISCOUNT_RATE]))
							&& StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_FREE_SHIPPING_FLG]) != Constants.FLG_COUPON_FREE_SHIPPING_VALID)
						{
							errorMessages += "\r\n" + MessageManager.GetMessages(Constants.INPUTCHECK_COUPON_DISCOUNT_NECESSARY);
						}
					}
					// クーポン割引額と割引率の両方入力あってもNG
					if ((string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_DISCOUNT_PRICE])) == false)
						&& (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_DISCOUNT_RATE])) == false))
					{
						errorMessages += "\r\n" + MessageManager.GetMessages(Constants.INPUTCHECK_COUPON_DISCOUNT_NECESSARY);
					}
				}

				// クーポン有効期限またはクーポン有効期間(開始、終了)必須チェック
				if (this.Data.ContainsKey(Constants.FIELD_COUPON_EXPIRE_DAY)
					|| this.Data.ContainsKey(Constants.FIELD_COUPON_EXPIRE_DATE_BGN)
					|| this.Data.ContainsKey(Constants.FIELD_COUPON_EXPIRE_DATE_END))
				{
					if (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_EXPIRE_DAY]))
						&& (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_EXPIRE_DATE_BGN]))
							|| string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_EXPIRE_DATE_END]))))
					{
						errorMessages += "\r\n" + MessageManager.GetMessages(Constants.INPUTCHECK_COUPON_EXPIREDAY_NECESSARY);
					}
				}
				// クーポン有効期限とクーポン有効期間(開始、終了)の両方入力あってもNG
				if ((string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_EXPIRE_DAY])) == false)
					&& ((string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_EXPIRE_DATE_BGN])) == false)
						|| (string.IsNullOrEmpty(StringUtility.ToEmpty(this.Data[Constants.FIELD_COUPON_EXPIRE_DATE_END])) == false)))
				{
					errorMessages += "\r\n" + MessageManager.GetMessages(Constants.INPUTCHECK_COUPON_EXPIREDAY_NECESSARY);
				}

				// クーポンIDがヘッダーに含まれている場合、エラーを返す
				if (this.HeadersCsv.Contains(Constants.FIELD_COUPON_COUPON_ID))
				{
					errorMessages += "\r\n" + MessageManager.GetMessages(Constants.INPUTCHECK_COUPON_COUPON_ID_NOT_SET);
				}
			}

			// 入力バリデーション
			string errorMessage = Validator.Validate(checkKbn, this.Data);
			this.ErrorOccurredIdInfo = "";

			if (errorMessage != "")
			{
				errorMessages += ((errorMessages.Length != 0) ? "\r\n" : "") + errorMessage;
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_COUPON_COUPON_ID);
				this.ErrorOccurredIdInfo += CreateIdString(Constants.FIELD_COUPON_COUPON_CODE);
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
	}
}
