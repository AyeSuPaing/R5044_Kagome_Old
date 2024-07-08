/*
=========================================================================================================
  Module      : BinderConfig(BinderConfig.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;

namespace w2.Cms.Manager.Codes.Binder
{
	/// <summary>
	/// BinderConfig
	/// </summary>
	public class BinderConfig
	{
		/// <summary>
		/// RegisterBinders
		/// </summary>
		/// <param name="binders">バインダー</param>
		public static void RegisterBinders(ModelBinderDictionary binders)
		{
			binders.DefaultBinder = new CustomModelBinder();
		}
	}
}