/*
=========================================================================================================
  Module      : 注文LP入力フロープロセス(OrderLandingInputProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web;
using System.Web.UI;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// OrderLandingInputProcess の概要の説明です
/// </summary>
public class OrderFormLpInputProcess : OrderLandingInputProcess
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public OrderFormLpInputProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>カートリストリピータ</summary>
	public override WrappedRepeater WrCartList { get { return GetWrappedControl<WrappedRepeater>("rCartList"); } }
}