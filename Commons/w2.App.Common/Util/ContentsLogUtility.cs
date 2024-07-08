/*
=========================================================================================================
  Module      : コンテンツログユーティリティ(ContentsLogUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.ContentsLog;

namespace w2.App.Common.Util
{
	/// <summary>
	/// コンテンツログユーティリティ
	/// </summary>
	public class ContentsLogUtility
	{
		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static ContentsLogUtility()
		{
			// なにもしない
		}

		/// <summary>
		/// コンテンツログ登録（PV）
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsId">コンテンツID</param>
		/// <param name="isSmartPhone">スマフォか</param>
		public static void InsertPageViewContentsLog(string contentsType, string contentsId, bool isSmartPhone)
		{
			var key = contentsType + contentsId;
			var value = CookieManager.GetValue(key);
			DateTime dateTime;

			if (DateTime.TryParse(value, out dateTime) == false)
			{
				dateTime = DateTime.Now.AddDays(-1).Date;
			}

			if (dateTime < DateTime.Now.Date)
			{
				var model = new ContentsLogModel
				{
					AccessKbn = (isSmartPhone ? Constants.FLG_CONTENTSLOG_ACCESS_KBN_SP : Constants.FLG_CONTENTSLOG_ACCESS_KBN_PC),
					ReportType = Constants.FLG_CONTENTSLOG_REPORT_TYPE_PV,
					ContentsId = contentsId,
					ContentsType = contentsType,
					Date = DateTime.Now,
					Price = 0,
				};
				new ContentsLogService().Insert(model);

				CookieManager.SetCookie(key, DateTime.Now.ToString(), Constants.PATH_ROOT);
			}
		}
	}
}
