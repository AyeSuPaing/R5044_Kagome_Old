/*
=========================================================================================================
  Module      : 更新対象格納インターフェース(IUpdated.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.User.UpdateAddressOfUserandFixedPurchase
{
	/// <summary>
	/// 更新対象格納インターフェース
	/// </summary>
	public interface IUpdated
	{
		/// <summary>
		/// URL生成
		/// </summary>
		/// <returns>URL</returns>
		string CreateUrl();

		/// <summary>更新対象ID</summary>
		string Id { get; set; }
		/// <summary>更新対象区分</summary>
		UpdatedKbn UpdatedKbn { get; }
	}
}
