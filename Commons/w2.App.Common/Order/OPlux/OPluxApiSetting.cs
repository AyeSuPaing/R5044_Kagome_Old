/*
=========================================================================================================
  Module      : OPlux Api Setting(OPluxApiSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// O-PLUX api setting
	/// </summary>
	public class OPluxApiSetting
	{
		/// <summary>Api encoding</summary>
		public Encoding ApiEncoding { get; set; }
		/// <summary>Method</summary>
		public string Method { get; set; }
		/// <summary>Http time out miri second</summary>
		public int HttpTimeOutMiriSecond { get; set; }
		/// <summary>Content type</summary>
		public string ContentType { get; set; }
		/// <summary>On before request</summary>
		public Action<IHttpApiRequestData> OnBeforeRequest { get; set; }
		/// <summary>On after request</summary>
		public Action<IHttpApiRequestData, string> OnAfterRequest { get; set; }
		/// <summary>On request error</summary>
		public Action<IHttpApiRequestData, string, Exception> OnRequestError { get; set; }
		/// <summary>On extend web request</summary>
		public Action<HttpWebRequest> OnExtendWebRequest { get; set; }
	}
}
