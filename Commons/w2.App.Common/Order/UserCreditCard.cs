/*
=========================================================================================================
  Module      : ユーザークレジットカード情報(UserCreditCard.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ユーザークレジットカード情報(UserCreditCardModel継承)
	/// </summary>
	[Serializable]
	public class UserCreditCard : UserCreditCardModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">クレジットカードモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserCreditCard(UserCreditCardModel model, SqlAccessor accessor = null)
			: base(model.DataSource)
		{
			Initialize(accessor);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		private void Initialize(SqlAccessor accessor = null)
		{
			this.CooperationInfo = new UserCardCooperationInfo(this, accessor);
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCard Get(string userId, int branchNo, SqlAccessor accessor = null)
		{
			var model = new UserCreditCardService().Get(userId, branchNo, accessor);
			if (model == null) return null;
			return new UserCreditCard(model);
		}

		/// <summary>
		/// 利用可能か未登録のものを取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCard[] GetUsableOrUnregisterd(string userId)
		{
			var models = new UserCreditCardService().GetUsableOrUnregisterd(userId);
			return models.Select(m => new UserCreditCard(m)).ToArray();
		}

		/// <summary>
		/// 利用可能か枝番から取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCard[] GetUsableOrByBranchno(string userId, int branchNo)
		{
			var models = new UserCreditCardService().GetUsableOrByBranchno(userId, branchNo);
			return models.Select(m => new UserCreditCard(m)).ToArray();
		}

		/// <summary>
		/// 利用可能なもの取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCard[] GetUsable(string userId)
		{
			var models = new UserCreditCardService().GetUsable(userId);
			return models.Select(m => new UserCreditCard(m)).ToArray();
		}
		
		/// <summary>
		/// 連携ID更新
		/// </summary>
		/// <param name="cooperationId">連携ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新 成功/失敗</returns>
		public bool UpdateCooperationId(string cooperationId, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			var result = new UserCreditCardService().UpdateCooperationId(
				this.UserId,
				this.BranchNo,
				cooperationId,
				lastChanged,
				updateHistoryAction);
			if (result)
			{
				this.CooperationId = cooperationId;
			}
			return result;
		}

		/// <summary>
		/// 表示フラグ更新
		/// </summary>
		/// <param name="dispFlg">表示フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新 成功/失敗</returns>
		public bool UpdateDispFlg(bool dispFlg, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor = null)
		{
			var result = new UserCreditCardService().UpdateDispFlg(
				this.UserId,
				this.BranchNo,
				dispFlg,
				lastChanged,
				updateHistoryAction,
				accessor);
			if (result)
			{
				this.DispFlg = dispFlg ? Constants.FLG_USERCREDITCARD_DISP_FLG_ON : Constants.FLG_USERCREDITCARD_DISP_FLG_OFF;
			}
			return result;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>削除成功したか</returns>
		public bool Delete(string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			var result = new UserCreditCardService().Delete(
				this.UserId,
				this.BranchNo,
				lastChanged,
				updateHistoryAction);
			return result;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>削除成功したか</returns>
		public bool Delete(UpdateHistoryAction updateHistoryAction)
		{
			var result = new UserCreditCardService().Delete(
				this.UserId,
				this.BranchNo,
				this.LastChanged,
				updateHistoryAction);
			return result;
		}

		/// <summary>表示フラグ（デザインの差を吸収するため残す）</summary>
		public bool DispFlag
		{
			get { return (this.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON); }
			set { this.DispFlg = value ? Constants.FLG_USERCREDITCARD_DISP_FLG_ON : Constants.FLG_USERCREDITCARD_DISP_FLG_OFF; }
		}
		/// <summary>連携情報</summary>
		public UserCardCooperationInfo CooperationInfo
		{
			get { return (UserCardCooperationInfo)this.DataSource["CooperationInfo"]; }
			set { this.DataSource["CooperationInfo"] = value; }
		}
		/// <summary>カード会社名</summary>
		public string CompanyName
		{
			get { return ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName, this.CompanyCode); }
		}

		#region 内部クラス
		/// <summary>
		/// ユーザーカード連携情報
		/// </summary>
		[Serializable]
		public class UserCardCooperationInfo
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="userCreditcard">ユーザークレジットカード</param>
			/// <param name="accessor">SQLアクセサ</param>
			public UserCardCooperationInfo(UserCreditCardModel userCreditcard, SqlAccessor accessor = null) : this(
				userCreditcard.UserId,
				userCreditcard.BranchNoFromString,
				userCreditcard.CooperationType,
				userCreditcard.CooperationId,
				userCreditcard.CooperationId2,
				accessor)
			{
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="userId">ユーザーID</param>
			/// <param name="branchNo">カード枝番</param>
			/// <param name="cooperationType">連携種別</param>
			/// <param name="cooperationId1">連携ID1</param>
			/// <param name="cooperationId2">連携ID2</param>
			/// <param name="accessor">SQLアクセサ</param>
			internal UserCardCooperationInfo(
				string userId,
				int branchNo,
				string cooperationType,
				string cooperationId1,
				string cooperationId2,
				SqlAccessor accessor = null)
			{
				this.CooperationId1 = "";
				this.CooperationId2 = "";

				switch (cooperationType)
				{
					case Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_CREDITCARD:
						switch (Constants.PAYMENT_CARD_KBN)
						{
							case Constants.PaymentCard.Zeus:
								this.CooperationId1 = this.ZeusTelNo = cooperationId1;
								this.CooperationId2 = this.ZeusSendId = string.IsNullOrEmpty(cooperationId2)
									? new UserCreditCardService().GetCooperationId2ForZeus(userId, branchNo)
									: cooperationId2;
								break;

							case Constants.PaymentCard.SBPS:
								this.CooperationId1 = this.SBPSCustCode = cooperationId1;
								break;

							case Constants.PaymentCard.YamatoKwc:
								this.CooperationId1 = this.YamatoKwcMemberId = cooperationId1;
								this.CooperationId2 = this.YamatoKwcAuthenticationKey = cooperationId2;
								break;

							case Constants.PaymentCard.Gmo:
								this.CooperationId1 = this.GMOMemberId = string.IsNullOrEmpty(cooperationId1)
									? new UserCreditCardCooperationInfoGmo(userId, accessor).CooperationId1
									: cooperationId1;
								break;

							case Constants.PaymentCard.Zcom:
								this.CooperationId1 = string.IsNullOrEmpty(cooperationId1)
									? new UserCreditCardCooperationInfoZcom(userId).CooperationId1
									: cooperationId1;
								break;

							case Constants.PaymentCard.EScott:
								this.CooperationId1 = string.IsNullOrEmpty(cooperationId1)
									? new UserCreditCardCooperationInfoEScott(userId, branchNo.ToString()).CooperationId1
									: cooperationId1;
								break;

							case Constants.PaymentCard.VeriTrans:
								this.CooperationId1 = string.IsNullOrEmpty(cooperationId1)
									? new UserCreditCardCooperationInfoVeritrans(userId).CooperationId1
									: cooperationId1;
								break;
						}
						break;

					case Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_PAYPAL:
						this.CooperationId1 = this.PayPalCustomerId = cooperationId1;
						this.CooperationId2 = this.PayPalDeviceData = cooperationId2;
						break;
				}
			}
			/// <summary>連携ID1</summary>
			public string CooperationId1 { get; internal set; }
			/// <summary>連携ID2</summary>
			public string CooperationId2 { get; internal set; }
			/// <summary>ゼウス向け電話番号</summary>
			public string ZeusTelNo { get; internal set; }
			/// <summary>ゼウス向けユニークID</summary>
			public string ZeusSendId { get; internal set; }
			/// <summary>SBPS向け顧客ID</summary>
			public string SBPSCustCode { get; internal set; }
			/// <summary>GMO向け会員ID</summary>
			public string GMOMemberId { get; internal set; }
			/// <summary>ヤマトKWC向け会員ID</summary>
			public string YamatoKwcMemberId { get; internal set; }
			/// <summary>ヤマトKWC向け認証キー</summary>
			public string YamatoKwcAuthenticationKey { get; internal set; }
			/// <summary>PayPal向けCustomerId</summary>
			public string PayPalCustomerId { get; internal set; }
			/// <summary>PayPal向けDeviceData</summary>
			public string PayPalDeviceData { get; internal set; }
		}
		#endregion
	}
}
