/*
=========================================================================================================
  Module      : 定期解約理由区分設定モデル (FixedPurchaseCancelReasonModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期解約理由区分設定モデル
	/// </summary>
	public partial class FixedPurchaseCancelReasonModel
	{
		#region 列挙体
		/// <summary>表示区分</summary>
		public enum DisplayKbnValue
		{
			/// <summary>PC・スマフォ</summary>
			PC,
			/// <summary>EC管理</summary>
			EC,
		}
		#endregion

		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>表示区分：PC利用</summary>
		public bool IsValidDisplayKbnPc
		{
			get
			{
				foreach (var value in this.DisplayKbn.Split(','))
				{
					if (value == FixedPurchaseCancelReasonModel.DisplayKbnValue.PC.ToString()) return true;
				}
				return false;
			}
		}
		/// <summary>表示区分：EC利用</summary>
		public bool IsValidDisplayKbnEc
		{
			get
			{
				foreach (var value in this.DisplayKbn.Split(','))
				{
					if (value == FixedPurchaseCancelReasonModel.DisplayKbnValue.EC.ToString()) return true;
				}
				return false;
			}
		}
		#endregion
	}
}
