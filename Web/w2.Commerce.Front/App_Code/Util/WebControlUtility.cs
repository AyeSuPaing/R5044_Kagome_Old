/*
=========================================================================================================
  Module      : Webコントロールユーティリティ(WebControlUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;

/// <summary>
/// WebControlUtility の概要の説明です
/// </summary>
public class WebControlUtility
{
	/// <summary>
	/// レコメンドエンジンユーザーコントロール取得
	/// </summary>
	/// <param name="control">検索対象コントロール</param>
	/// <returns>item1:商品レコメンドエンジンユーザーコントロール
	/// item2:カテゴリレコメンドエンジンユーザーコントロール</returns>
	public static
		Tuple<List<ProductRecommendByRecommendEngineUserControl>, List<CategoryRecommendByRecommendEngineUserControl>>
		GetRecommendEngineUserControls(Control control)
	{
		var userControls = GetAllUserControls(control);
		return new Tuple
			<List<ProductRecommendByRecommendEngineUserControl>, List<CategoryRecommendByRecommendEngineUserControl>>(
			userControls.Where(c => c is ProductRecommendByRecommendEngineUserControl).Cast<ProductRecommendByRecommendEngineUserControl>().ToList(),
			userControls.Where(c => c is CategoryRecommendByRecommendEngineUserControl).Cast<CategoryRecommendByRecommendEngineUserControl>().ToList());
	}
	
	/// <summary>
	/// 配下すべてのユーザーコントロール取得
	/// </summary>
	/// <param name="control"></param>
	/// <returns></returns>
	public static IEnumerable<Control> GetAllUserControls(Control control)
	{
		var all = GetAllControls(control);
		return all.Where(c => c is UserControl);
	}

	/// <summary>
	/// 配下すべてのコントロール取得（再帰）
	/// </summary>
	/// <param name="control"></param>
	/// <returns></returns>
	public static IEnumerable<Control> GetAllControls(Control control)
	{
		yield return control;
		foreach (var ctrl in control.Controls.Cast<Control>().SelectMany(GetAllControls))
		{
			yield return ctrl;
		}
	}
}