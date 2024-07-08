/*
=========================================================================================================
  Module      : ウェブサービス内処理用インタフェース(IWebServiceInProc.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : ウェブサービス内処理用のインタフェース。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace w2.Plugin.P0011_Intercom.WebService.Inproc
{
	interface IWebServiceInProc
	{
		DataSet Execute(DataSet ds);
	}
}
