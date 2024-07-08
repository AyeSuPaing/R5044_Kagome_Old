/*
=========================================================================================================
  Module      : ソフトバンクペイメント Webコンビニタイプ(PaymentSBPSWebCvsType.cs)
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
	public class PaymentSBPSTypes
	{
		/// <summary>Webコンビニタイプ</summary>
		public enum WebCvsTypes
		{
			/// <summary>セブンイレブン</summary>
			SevenEleven,
			/// <summary>ローソン</summary>
			Lowson,
			/// <summary>ミニストップ</summary>
			MiniStop,
			/// <summary>デイリーヤマザキ</summary>
			DailyYamazaki,
			/// <summary>ファミリーマート</summary>
			FamilyMart,
			/// <summary>セイコーマート</summary>
			Seicomart,
		}

		/// <summary>支払い方法</summary>
		public enum PayMethodTypes
		{
			/// <summary>クレジット</summary>
			credit,
			/// <summary>クレジット(3Dセキュア1.0)</summary>
			credit3d,
			/// <summary>クレジット(3Dセキュア2.0)</summary>
			credit3d2,
			/// <summary>Webコンビニ</summary>
			webcvs,
			/// <summary>ペイジー</summary>
			payeasy,
			/// <summary>ドコモケータイ払い</summary>
			docomo,
			/// <summary>auかんたん決済</summary>
			auone,
			/// <summary>S!まとめて支払い</summary>
			softbank,
			/// <summary>リクルートかんたん支払い</summary>
			recruit,
			/// <summary>ペイパル</summary>
			paypal,
			/// <summary>楽天ペイ</summary>
			rakuten,
			/// <summary>楽天PayV2</summary>
			rakutenv2,
			/// <summary>ソフトバンクまとめて支払い</summary>
			//mysoftbank,
			/// <summary>ソフトバンク・ワイモバイルまとめて支払い</summary>
			softbank2,
			/// <summary>Paypay</summary>
			paypay,
		}
	}
}
