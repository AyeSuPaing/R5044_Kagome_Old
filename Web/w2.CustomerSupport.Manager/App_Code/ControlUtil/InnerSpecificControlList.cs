/*
=========================================================================================================
  Module      : InnerSpecificControlListクラス(InnerSpecificControlList.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// InnerSpecificControlListクラス
///   指定したControlが含んでいる、特定のControlのみを抽出したリストを表す。
///   （Controlが持つControlをすべて検索して抽出する）
/// </summary>
/// <typeparam name="ControlType">Controlを継承しているClassを設定</typeparam>
/// <remarks>
/// Commerce.ManagerとMarketingPlanner.Managerの双方に同一のClassがあります
/// </remarks>
public class InnerSpecificControlList<ControlType> : List<ControlType>
{
	/// <summary>
	/// コンストラクタ
	///   (指定したControlが含んでいる、特定のControlのみを抽出したリストを返す)
	/// </summary>
	/// <param name="cRootControl">ROOTとするControl</param>
	public InnerSpecificControlList(Control cRootControl)
	{
		this.AddRange(GetSpecificControls(GetInnerControls(cRootControl)));
	}
	/// <summary>
	/// コンストラクタ
	///   (指定したControlが含んでいる、特定のControlのみを抽出したリストを返す)
	/// </summary>
	/// <param name="lcRootControl">ROOTとするControlのリスト</param>
	public InnerSpecificControlList(List<Control> lcRootControls)
	{
		this.AddRange(GetSpecificControls(GetInnerSpecificControls(lcRootControls)));
	}

	/// <summary>
	/// 指定したControl内の特定のコントロールを取得する
	/// </summary>
	/// <param name="lcRootControl">検索対象とするControlのリスト</param>
	/// <returns>指定したコントロール内に含まれる特定のコントロールリスト</returns>
	private List<Control> GetInnerSpecificControls(List<Control> lcRootControls)
	{
		List<Control> lcResult = new List<Control>();
		lcRootControls.ForEach((cTarget) => { lcResult.AddRange(GetInnerControls(cTarget)); });
		return lcResult;
	}

	/// <summary>
	/// ControlのListに含まれる特定のControlのリストを取得する
	/// </summary>
	/// <param name="lcTargets">検索対象のリスト</param>
	/// <returns>指定したコントロール内に含まれる特定のControlのリスト</returns>
	private List<ControlType> GetSpecificControls(List<Control> lcTargets)
	{
		return new List<ControlType>(lcTargets.FindAll((cTarget) => { return cTarget is ControlType; })
				.Cast<ControlType>());
	}

	/// <summary>
	/// 指定したControlリスト内の全Controlを取得する
	/// </summary>
	/// <param name="cRootControl">検索対象とするControl</param>
	/// <returns>指定したコントロール内に含まれる全Controlのリスト</returns>
	private List<Control> GetInnerControls(Control cRootControl)
	{
		List<Control> lResult = new List<Control>();
		foreach (Control cControl in cRootControl.Controls)
		{
			lResult.Add(cControl);
			lResult.AddRange(GetInnerControls(cControl));
		}
		return lResult;
	}
}