/*
=========================================================================================================
  Module      : デフォルトJAXオプション(DefaultAjaxOptions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc.Ajax;

namespace w2.Cms.Manager.Codes.View
{
	/// <summary>
	/// デフォルトJAXオプション
	/// </summary>
	public class DefaultAjaxOptions : AjaxOptions
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DefaultAjaxOptions()
		{
			this.HttpMethod = "POST";
			this.OnFailure = "handleError";
		}
	}
}