/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 処理区分(PaymentYamatoKaProcessDiv.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 処理区分
	/// </summary>
	public enum PaymentYamatoKaProcessDiv
	{
		/// <summary>新規登録</summary>
		Entry = 0,
		/// <summary>変更登録</summary>
		Update = 1,
		/// <summary>他社配送分の新規登録又は変更登録</summary>
		InsertUpdate = 2,
		/// <summary>取消</summary>
		Cancel = 9,
	}
}
