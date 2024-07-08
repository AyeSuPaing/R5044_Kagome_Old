/*
=========================================================================================================
  Module      : ButtonAttribute(ButtonAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Reflection;
using System.Web.Mvc;

namespace w2.Cms.Manager.Codes.Binder
{
	/// <summary>
	/// ButtonAttribute
	/// </summary>
	public class ButtonAttribute : ActionMethodSelectorAttribute
	{
		/// <summary>
		/// IsValidForRequest
		/// </summary>
		/// <param name="controllerContext">ControllerContext</param>
		/// <param name="methodInfo">MethodInfo</param>
		/// <returns>bool</returns>
		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			// 設定したボタン名と同名のデータが存在するかチェック（Requestで返ってきているか）
			return controllerContext.Controller.ValueProvider.GetValue(this.ButtonName) != null;
		}

		// アクションメソッド付加時に設定したボタン名を保存
		public string ButtonName { get; set; }
	}
}