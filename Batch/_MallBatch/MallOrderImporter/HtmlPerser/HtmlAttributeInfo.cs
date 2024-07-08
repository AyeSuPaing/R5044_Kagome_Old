/*
=========================================================================================================
  Module      : HTML属性情報クラス(HtmlAttributeInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.MallOrderImporter.HtmlPerser
{
	public class HtmlAttributeInfo : ICloneable
	{
		private string m_strName;
		private string m_strValue;
		private char m_chDelim;

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public HtmlAttributeInfo()
			: this("", "", (char)0)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strName"></param>
		/// <param name="strValue"></param>
		public HtmlAttributeInfo(string strName, string strValue)
			: this(strName, strValue, (char)0)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strName"></param>
		/// <param name="strValue"></param>
		/// <param name="delim"></param>
		public HtmlAttributeInfo(string strName, string strValue, char chDelim)
		{
			m_strName = strName;
			m_strValue = strValue;
			m_chDelim = chDelim;
		}

		/// <summary>
		/// クローン
		/// </summary>
		/// <returns></returns>
		#region ICloneable Members
		public virtual object Clone()
		{
			return new HtmlAttributeInfo(m_strName, m_strValue, m_chDelim);
		}
		#endregion

		/// <summary>プロパティ：デリミタ</summary>
		public char Delim
		{
			get { return m_chDelim; }
			set { m_chDelim = value; }
		}
		/// <summary>プロパティ：属性名</summary>
		public string Name
		{
			get { return m_strName; }
			set { m_strName = value; }
		}
		/// <summary>プロパティ：属性値</summary>
		public string Value
		{
			get { return m_strValue; }
			set { m_strValue = value; }
		}
	}
}
