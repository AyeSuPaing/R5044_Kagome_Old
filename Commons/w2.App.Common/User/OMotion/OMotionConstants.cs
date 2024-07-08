/*
=========================================================================================================
  Module      : OMotion Constants(OMotionConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.User.OMotion
{
	/// <summary>
	/// O-MOTION const
	/// </summary>
	public static class OMotionConstants
	{
		/// <summary>Default web request content type</summary>
		public const string DEFAULT_WEB_REQUEST_CONTENT_TYPE = "application/json";
		/// <summary>Request header key signature</summary>
		public const string REQUEST_HEADER_KEY_SIGNATURE = "X-Omotion-Signature";
		/// <summary>Request header key useridhashed</summary>
		public const string REQUEST_HEADER_KEY_USERIDHASHED = "X-Omotion-Useridhashed";
		/// <summary>Request header key methodoverride</summary>
		public const string REQUEST_HEADER_KEY_METHODOVERRIDE = "X-Http-Method-Override";
		/// <summary>api log file name prefix</summary>
		public const string LOGFILE_NAME_PREFIX = "OMotionApi";
		/// <summary>log file extension</summary>
		public const string LOGFILE_EXTENSION = "log";
		/// <summary>log file encoding</summary>
		public const string LOGFILE_ENCODING = "shift_jis";
		/// <summary>log file threshold（MB）</summary>
		public const int LOGFILE_THRESHOLD = 100;
	}
}
