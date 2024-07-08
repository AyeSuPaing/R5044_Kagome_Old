/*
=========================================================================================================
  Module      : 決済カード連携マスタモデル (UserCreditCardModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.UserCreditCard
{
	/// <summary>
	/// 決済カード連携マスタモデル
	/// </summary>
	public partial class UserCreditCardModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>カード枝番（文字列より変換）</summary>
		/// <remarks>更新履歴の表示では文字列になっているため、こちらから取得する</remarks>
		public int BranchNoFromString
		{
			get
			{
				var branchNoString = StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USERCREDITCARD_BRANCH_NO]);
				return int.Parse(branchNoString);
			}
		}
		/// <summary>
		/// 拡張項目_表示フラグONか判定
		/// </summary>
		public bool IsDisp
		{
			get { return (this.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON); }
		}
		/// <summary>登録アクション区分が注文系か</summary>
		public bool IsRegisterActionKbnOrderSomething
		{
			get
			{
				return ((this.RegisterActionKbn == Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_REGISTER)
					|| (this.RegisterActionKbn == Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_MODIFY)
					|| (this.RegisterActionKbn == Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_RETURN_EXCHANGE));
			}
		}
		/// <summary>登録アクション区分が定期変更か</summary>
		public bool IsRegisterActionKbnFixedPurchaseModify
		{
			get { return (this.RegisterActionKbn == Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_FIXEDPURCHASE_MODIFY); }
		}
		/// <summary>登録済ステータスが通常か</summary>
		public bool IsRegisterdStatusNormal
		{
			get { return (this.RegisterStatus == Constants.FLG_USERCREDITCARD_REGISTER_STATUS_NORMAL); }
		}
		/// <summary>登録済ステータスが未登録か</summary>
		public bool IsRegisterdStatusUnregisterd
		{
			get { return (this.RegisterStatus == Constants.FLG_USERCREDITCARD_REGISTER_STATUS_UNREGISTERED); }
		}
		/// <summary>登録済ステータスが未与信か</summary>
		public bool IsRegisterdStatusUnauthed
		{
			get { return (this.RegisterStatus == Constants.FLG_USERCREDITCARD_REGISTER_STATUS_UNAUTHED); }
		}
		/// <summary>登録済ステータスが与信エラーか</summary>
		public bool IsRegisterdStatusdAuthError
		{
			get { return (this.RegisterStatus == Constants.FLG_USERCREDITCARD_REGISTER_STATUS_AUTHERROR); }
		}
		#endregion
	}
}
