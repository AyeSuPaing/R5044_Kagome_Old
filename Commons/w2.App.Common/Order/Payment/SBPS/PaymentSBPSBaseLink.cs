/*
=========================================================================================================
  Module      : ソフトバンクペイメント リンク式基底クラス(PaymentSBPSBaseLink.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント リンク式基底クラス
	/// </summary>
	public abstract class PaymentSBPSBaseLink : PaymentSBPSBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="hashEncoding">ハッシュエンコーディング</param>
		public PaymentSBPSBaseLink(
			PaymentSBPSSetting settings,
			Encoding hashEncoding)
			: base(settings, hashEncoding)
		{
		}
	}
}
