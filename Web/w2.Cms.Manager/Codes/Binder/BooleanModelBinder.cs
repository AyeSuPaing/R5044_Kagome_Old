/*
=========================================================================================================
  Module      : boolモデルバインダ(BooleanModelBinder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web.Mvc;

namespace w2.Cms.Manager.Codes.Binder
{
	/// <summary>
	/// boolモデルバインダ
	/// </summary>
	public class BooleanModelBinder : IModelBinder
	{
		/// <summary>
		/// モデルバインド
		/// </summary>
		/// <param name="controllerContext">ControllerContext</param>
		/// <param name="bindingContext">ModelBindingContext</param>
		/// <returns>バインド後の値</returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			// MVCのチェックボックスはチェックONだと以下の状態で来る
			if (valueResult != null)
			{
				if (valueResult.AttemptedValue == "true,false")
				{
					AddToModelState(bindingContext, valueResult);
					return true;
				}

				// TryParse
				bool parsed;
				if (bool.TryParse(valueResult.AttemptedValue, out parsed))
				{
					AddToModelState(bindingContext, valueResult);
					return parsed;
				}
			}

			return false;
		}

		/// <summary>
		/// モデルステートに追加
		/// </summary>
		/// <param name="bindingContext">ModelBindingContext</param>
		/// <param name="valueResult">valueResult</param>
		private static void AddToModelState(ModelBindingContext bindingContext, ValueProviderResult valueResult)
		{
			bindingContext.ModelState.Add(bindingContext.ModelName, new ModelState { Value = valueResult });
		}
	}
}