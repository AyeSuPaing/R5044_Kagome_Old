/*
=========================================================================================================
  Module      : CustomModelBinder(CustomModelBinder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace w2.Cms.Manager.Codes.Binder
{
	/// <summary>
	/// カスタムモデルバインダー
	/// </summary>
	public class CustomModelBinder : DefaultModelBinder
	{
		/// <summary>
		/// プロパティの値を取得する
		/// </summary>
		/// <param name="controllerContext">コントローラーコンテキスト</param>
		/// <param name="bindingContext">バインディングコンテキスト</param>
		/// <param name="propertyDescriptor">プロパティ抽象化</param>
		/// <param name="propertyBinder">プロパティバインダー</param>
		/// <returns>プロパティの値</returns>
		protected override object GetPropertyValue(
			ControllerContext controllerContext,
			ModelBindingContext bindingContext,
			PropertyDescriptor propertyDescriptor,
			IModelBinder propertyBinder)
		{
			// バインドするときに空文字をnullに変更しない
			bindingContext.ModelMetadata.ConvertEmptyStringToNull = false;
			var result = base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
			return result;
		}

		/// <summary>
		/// モデルプロパティ取得
		/// </summary>
		/// <param name="controllerContext">コントローラーコンテキスト</param>
		/// <param name="bindingContext">バインディングコンテキスト</param>
		/// <returns>モデルプロパティ</returns>
		protected override PropertyDescriptorCollection GetModelProperties(
			ControllerContext controllerContext,
			ModelBindingContext bindingContext)
		{
			var toReturn = base.GetModelProperties(controllerContext, bindingContext);
			var additional = new List<PropertyDescriptor>();

			//now look for any aliasable properties in here
			foreach (var p in this.GetTypeDescriptor(controllerContext, bindingContext).GetProperties()
				.Cast<PropertyDescriptor>())
			{
				foreach (var attr in p.Attributes.OfType<BindAliasAttribute>())
				{
					additional.Add(new AliasedPropertyDescriptor(attr.Alias, p));

					if (bindingContext.PropertyMetadata.ContainsKey(p.Name))
					{
						bindingContext.PropertyMetadata.Add(attr.Alias, bindingContext.PropertyMetadata[p.Name]);
					}
				}
			}

			var result = new PropertyDescriptorCollection(toReturn.Cast<PropertyDescriptor>().Concat(additional).ToArray());
			return result;
		}
	}
}