/*
=========================================================================================================
  Module      : OMotion Api Setting(OMotionApiSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using System.Text;

namespace w2.App.Common.User.OMotion
{
	/// <summary>
	/// O-MOTION api setting
	/// </summary>
	public class OMotionApiSetting
	{
		/// <summary>Api encoding</summary>
		public Encoding ApiEncoding { get; set; }
		/// <summary>Method</summary>
		public string Method { get; set; }
		/// <summary>Http time out milli second</summary>
		public int HttpTimeOutMilliSecond { get; set; }
		/// <summary>Content type</summary>
		public string ContentType { get; set; }
		/// <summary>On before request</summary>
		public Action<BaseOMotionRequest> OnBeforeRequest { get; set; }
		/// <summary>On after request</summary>
		public Action<BaseOMotionRequest, string> OnAfterRequest { get; set; }
		/// <summary>On request error</summary>
		public Action<BaseOMotionRequest, string, Exception> OnRequestError { get; set; }
		/// <summary>On extend web request</summary>
		public Action<HttpWebRequest> OnExtendWebRequest { get; set; }
	}
}
