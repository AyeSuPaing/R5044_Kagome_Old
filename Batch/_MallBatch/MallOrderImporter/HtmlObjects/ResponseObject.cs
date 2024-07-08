/*
=========================================================================================================
  Module      : レスポンスオブジェクトクラス(ResponseObject.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.MallOrderImporter.HtmlObjects
{
	public class ResponseObject
	{
		string m_strUrl = null;
		string m_strHtml = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strUrl"></param>
		/// <param name="strHtml"></param>
		public ResponseObject(string strUrl, string strHtml)
		{
			m_strUrl = strUrl;
			m_strHtml = strHtml;
		}

		/// <summary>プロパティ：アドレス</summary>
		public string Url
		{
			get { return m_strUrl; }
		}
		/// <summary>プロパティ：返却HTML</summary>
		public string Html
		{
			get { return m_strHtml; }
		}
	}
}
