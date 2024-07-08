/*
=========================================================================================================
  Module      : 基底ページヘルパ(BasePageHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// 基底ページヘルパ
/// </summary>
public class BasePageHelper
{
	/// <summary>
	/// 親リピータ内のコントロールを取得（再帰メソッド）
	/// </summary>
	/// <param name="control">検索の基準になるコントロール</param>
	/// <param name="targetControlId">検索するコントロールのID</param>
	/// <returns>検索するコントロール</returns>
	public static dynamic GetParentRepeaterItemControl(Control control, string targetControlId)
	{
		if (control.Parent == null)
		{
			return null;
		}

		if ((control.Parent is RepeaterItem) && control.Parent.FindControl(targetControlId) != null)
		{
			return control.Parent.FindControl(targetControlId);
		}
		else
		{
			return GetParentRepeaterItemControl(control.Parent, targetControlId);
		}
	}
}