/*
=========================================================================================================
  Module      : ZcomAPI連携ファサード生成クラス (ZcomApiFacadeFactory.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// 外部APIファサード生成
	/// </summary>
	internal class ZcomApiFacadeFactory : IZcomApiFacadeFactory
	{
		/// <summary>
		/// ファサード生成
		/// </summary>
		/// <param name="setting">設定</param>
		/// <returns>ファサード</returns>
		public IZcomApiFacade CreateFacade(ZcomApiSetting setting)
		{
			var facade = setting == null
				? new ZcomApiFacade()
				: new ZcomApiFacade(setting);
			return facade;
		}
	}
}
