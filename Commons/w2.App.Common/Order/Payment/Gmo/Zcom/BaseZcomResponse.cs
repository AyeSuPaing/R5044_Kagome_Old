/*
=========================================================================================================
  Module      : Zcom基底レスポンスクラス (BaseZcomResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// Zcom基底レスポンスクラス
	/// </summary>
	public abstract class BaseZcomResponse : IHttpApiResponseData
	{
		/// <summary>
		/// レスポンス文字生成
		/// </summary>
		/// <returns>レスポンス文字</returns>
		public string CreateResponseString()
		{
			return SerializeHelper.Serialize(this);
		}
	}
}
