/*
=========================================================================================================
  Module      : 共通列挙体定義(Enums.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common
{
	///**************************************************************************************
	/// <summary>
	/// SMTP認証タイプ
	/// </summary>
	///**************************************************************************************
	public enum SmtpAuthType
	{
		/// <summary>NORMAL</summary>
		Normal,
		/// <summary>POP Before SMTP</summary>
		PopBeforeSmtp,
		/// <summary>SMTP AUTH</summary>
		SmtpAuth
	}

	///**************************************************************************************
	/// <summary>
	/// POPタイプ
	/// </summary>
	///**************************************************************************************
	public enum PopType
	{
		/// <summary>POP</summary>
		Pop,
		/// <summary>APOP</summary>
		Apop
	}
}
