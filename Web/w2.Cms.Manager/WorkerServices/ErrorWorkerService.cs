/*
=========================================================================================================
  Module      : エラーワーカーサービス(ErrorWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// エラーワーカーサービス
	/// </summary>
	public class ErrorWorkerService : BaseWorkerService
	{
		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="errorKbn">エラー区分</param>
		/// <param name="tempDataErrorMessage">TempDataのエラーメッセージ</param>
		/// <returns>エラーメッセージ</returns>
		public string GetErrorMessage(string errorKbn, string tempDataErrorMessage)
		{
			switch (errorKbn)
			{
				// 制御文字入力エラー
				case Constants.REQUEST_KBN_ERROR_KBN_SYTEM_VALIDATION_ERROR:
					return WebMessages.SystemValidationError;

				// システムエラー（集約エラーハンドラ内ではセッションが使えないこともあるので）
				case Constants.REQUEST_KBN_ERROR_KBN_SYSTEM_ERROR:
					return WebMessages.SystemError;

				// 404エラー
				case Constants.REQUEST_KBN_ERROR_KBN_404_ERROR:
					return WebMessages.Http404Error;

				// 未ログインエラー
				case Constants.REQUEST_KBN_ERROR_KBN_UNLOGGEDIN_ERROR:
					return WebMessages.ShopOperatorUnloggedIn;

				// CMSオプションOFFエラー
				case Constants.REQUEST_KBN_ERROR_KBN_UNCMS_OPTION_DISABLED_ERROR:
					return WebMessages.CmsOptionDisabledError;

				// その他エラー
				default:
					return tempDataErrorMessage ?? WebMessages.SystemError;
			}
		}

		/// <summary>
		/// Clear Cache of Browser
		/// </summary>
		/// <param name="reposense">レスポンス</param>
		public void ClearBrowserCache(HttpResponseBase reposense)
		{
			reposense.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
			reposense.Cache.SetCacheability(HttpCacheability.NoCache);
			reposense.Cache.SetNoStore();
		}
	}
}