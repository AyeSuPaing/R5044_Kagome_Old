/*
=========================================================================================================
  Module      : ワーカーサービスインターフェース(IWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// ワーカーサービスインターフェース
	/// </summary>
	public interface IWorkerService
	{
		/// <summary>セッションラッパー</summary>
		SessionWrapper SessionWrapper { get; set; }
	}
}