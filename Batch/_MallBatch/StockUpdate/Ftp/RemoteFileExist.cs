/*
=========================================================================================================
  Module      : FTPファイル通信例外エラー (RemoteFileExist.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Commerce.MallBatch.StockUpdate.Ftp
{
	///**************************************************************************************
	/// <summary>
	///  FTPファイル通信例外エラークラス
	/// </summary>
	///**************************************************************************************
	public class RemoteFileExist : Exception
	{
		private string m_strMsg = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <remarks>エラーメッセージを書き換える</remarks>
		public RemoteFileExist(string strMsg)
		{
			m_strMsg = strMsg;
		}

		/// <summary>メッセージ</summary>
		public override string Message
		{
			get { return m_strMsg; }
		}
	}
}
