/*
=========================================================================================================
  Module      : ZcomAPI連携ファサード生成インターフェース (IZcomApiFacadeFactory.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// ZcomAPI連携ファサード生成インターフェース
	/// </summary>
	public interface IZcomApiFacadeFactory : IService
	{
		/// <summary>
		/// ファサード生成
		/// </summary>
		/// <param name="setting">設定</param>
		/// <returns>ファサード</returns>
		IZcomApiFacade CreateFacade(ZcomApiSetting setting);
	}
}
