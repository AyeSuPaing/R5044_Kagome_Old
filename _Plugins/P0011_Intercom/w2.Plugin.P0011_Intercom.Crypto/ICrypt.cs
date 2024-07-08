/*
=========================================================================================================
  Module      : 暗号・複合化処理のインタフェース(ICrypt.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Crypto
{
	public interface ICrypt
	{
		string Encrypt(string targetvalue, string password);
		string Decrypt(string targetvalue,string password);
	}
}
