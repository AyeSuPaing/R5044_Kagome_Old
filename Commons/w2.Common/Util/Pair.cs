/*
=========================================================================================================
  Module      : Pairクラスのジェネリクス版(Pair.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace w2.Common.Util
{
	public class Pair<T, U>
	{
		public Pair(T first, U second)
		{
			this.First = first;
			this.Second = second;
		}

		public T First { get; private set; }
		public U Second { get; private set; }
	}
}
