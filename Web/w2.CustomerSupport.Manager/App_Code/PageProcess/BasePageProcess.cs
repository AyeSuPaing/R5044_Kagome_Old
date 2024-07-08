/*
=========================================================================================================
  Module      : Base page process(BasePageProcess.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using System.Web.UI;
using w2.App.Common.Web.Process;

/// <summary>
/// Base page process
/// </summary>
public class BasePageProcess : CommonPageProcess
{
	/// <summary>
	/// Base page process
	/// </summary>
	/// <param name="caller">Caller</param>
	/// <param name="viewState">View state</param>
	/// <param name="context">Context</param>
	public BasePageProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState)
	{
		this.Context = context;
	}

	/// <summary>Http context</summary>
	protected HttpContext Context { get; set; }
}