/*
=========================================================================================================
  Module      : 個別変換セット(ConvertSet.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Commerce.MallBatch.Mkadv.Common.Convert
{
	///**************************************************************************************
	/// <summary>
	/// 個別変換のセットを表記する
	/// </summary>
	///**************************************************************************************
	public class ConvertSet
	{
		/// <summary>
		/// 個別変換設定を構築する
		/// </summary>
		/// <param name="iTarget">対象出力フィールドID</param>
		/// <param name="strFrom">変換前文字列</param>
		/// <param name="strTo">変換後文字列</param>
		public ConvertSet(int iTarget, string strFrom, string strTo)
		{
			this.Target = iTarget;
			this.From = strFrom;
			this.To = strTo;
		}

		/// <summary>ターゲット</summary>
		public int Target { get; private set; }

		/// <summary>変換前文字</summary>
		public string From { get; private set; }

		/// <summary>変換後文字</summary>
		public string To { get; private set; }
	}
}
