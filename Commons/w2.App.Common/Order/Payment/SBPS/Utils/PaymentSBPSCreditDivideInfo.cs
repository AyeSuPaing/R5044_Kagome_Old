/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット分割情報クラス(PaymentSBPSCreditDivideInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント クレジット分割情報クラス
	/// </summary>
	public class PaymentSBPSCreditDivideInfo
	{
		/// <summary>取引区分：一括</summary>
		public const string DEALING_TYPES_ONCE = "10";
		/// <summary>取引区分：ボーナス一括</summary>
		public const string DEALING_TYPES_BONUS1 = "21";
		/// <summary>取引区分：分割</summary>
		public const string DEALING_TYPES_DIVIDE = "61";
		/// <summary>取引区分：リボ</summary>
		public const string DEALING_TYPES_REVO = "80";

		/// <summary>分割タイプ</summary>
		public enum DivideTypes
		{
			/// <summary>一括</summary>
			Once,
			/// <summary>ボーナス一括</summary>
			Bonus1,
			/// <summary>分割</summary>
			Divide,
			/// <summary>リボ</summary>
			Revo
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="divideType">分割タイプ</param>
		/// <param name="divideTimes">分割回数（分割以外はnull）</param>
		internal PaymentSBPSCreditDivideInfo(
			DivideTypes divideType,
			int? divideTimes = null)
		{
			this.DivideType = divideType;
			this.DivideTimes = divideTimes;
		}

		/// <summary>
		/// 「取引区分」文字列取得
		/// </summary>
		/// <returns>「取引区分」文字列</returns>
		internal string GetDealingsTypeString()
		{
			switch (this.DivideType)
			{
				case PaymentSBPSCreditDivideInfo.DivideTypes.Once:
					return DEALING_TYPES_ONCE;

				case PaymentSBPSCreditDivideInfo.DivideTypes.Bonus1:
					return DEALING_TYPES_BONUS1;

				case PaymentSBPSCreditDivideInfo.DivideTypes.Divide:
					return DEALING_TYPES_DIVIDE;

				case PaymentSBPSCreditDivideInfo.DivideTypes.Revo:
					return DEALING_TYPES_REVO;
			}
			return DEALING_TYPES_ONCE;
		}

		/// <summary>分割タイプ</summary>
		internal DivideTypes DivideType { get; private set; }
		/// <summary>分割回数（分割以外はnull）</summary>
		internal int? DivideTimes { get; private set; }
	}
}
