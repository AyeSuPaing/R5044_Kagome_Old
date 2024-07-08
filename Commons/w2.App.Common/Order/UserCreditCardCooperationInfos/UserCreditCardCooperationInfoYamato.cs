/*
=========================================================================================================
  Module      :トークン取得向け決済情報（ヤマトKWC）(UserCreditCardCooperationInfoYamato.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// トークン取得向け決済情報（ヤマトKWC）
	/// </summary>
	public class UserCreditCardCooperationInfoYamato : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserCreditCardCooperationInfoYamato()
			: base(CreateCooperationId1(), CreateCooperationId2())
		{
			this.CheckSum = PaymentYamatoKwcCheckSumCreater.CreateForToken(this.CooperationId1, this.CooperationId2);
			this.AuthDiv = PaymentYamatoKwcCheckSumCreater.AuthDiv;
		}

		/// <summary>
		/// 連携ID1作成
		/// </summary>
		/// <returns>連携ID1</returns>
		private static string CreateCooperationId1()
		{
			// 会員IDは採番する
			var id1 = OrderCommon.CreateYamatoKwcMemberId(Constants.CONST_DEFAULT_SHOP_ID);
			return id1;
		}

		/// <summary>
		/// 連携ID2作成
		/// </summary>
		/// <returns>連携ID1</returns>
		private static string CreateCooperationId2()
		{
			// 認証キーは文字以内の半角英数字記号
			var id2 = DateTime.Now.ToString("HHmmss");
			return id2;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <returns>パラメタ</returns>
		public override string[] CreateParams()
		{
			return new[] { this.CooperationId1, this.CooperationId2, this.CheckSum, this.AuthDiv };
		}

		/// <summary>チェックサム</summary>
		public string CheckSum { get; set; }
		/// <summary>与信区分</summary>
		public string AuthDiv { get; set; }
	}
}
