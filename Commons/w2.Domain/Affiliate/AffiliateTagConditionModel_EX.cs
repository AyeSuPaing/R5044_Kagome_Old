/*
=========================================================================================================
  Module      : アフィリエイトタグの出力条件管理モデル (AffiliateTagConditionModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトタグの出力条件管理モデル
	/// </summary>
	public partial class AffiliateTagConditionModel
	{
		#region メソッド
		/// <summary>
		/// 変更可能な設置箇所条件か判定
		/// </summary>
		/// <param name="modifiablePages">変更対象の設置箇所配列</param>
		/// <returns>変更可能な設置箇所か</returns>
		public bool IsModifiablePageCondition(IEnumerable<string> modifiablePages)
		{
			var isUnchangePage = (this.IsPageCondition && modifiablePages.Contains(this.ConditionValue));
			return isUnchangePage;
		}
		#endregion

		#region プロパティ
		/// <summary>設置箇所条件か</summary>
		public bool IsPageCondition
		{
			get { return (this.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE); }
		}
		#endregion
	}
}
