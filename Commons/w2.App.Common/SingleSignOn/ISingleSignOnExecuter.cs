/*
=========================================================================================================
  Module      : シングルサインオン実行インタフェース(ISingleSignOnExecuter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.SingleSignOn
{
	/// <summary>シングルサインオン実行インタフェース</summary>
	public interface ISingleSignOnExecuter
	{
		/// <returns>シングルサインオン実行</returns>
		SingleSignOnResult Execute();
	}
}