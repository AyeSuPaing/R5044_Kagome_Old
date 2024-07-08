/*
=========================================================================================================
  Module      : HTMLパーサ既定クラス(HtmlParserBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.MallOrderImporter.HtmlPerser
{
	public class HtmlParserBase : HtmlTagInfo
	{
		private string m_strSource;
		private int m_iIndex;
		private char m_chParseDelim;
		private string m_strParseName;
		private string m_strParseValue;
		public string m_strTag;

		/// <summary>
		/// 先頭からホワイスペースを削除
		/// </summary>
		public void EatWhiteSpace()
		{
			while (!Eof())
			{
				if (IsWhiteSpace(GetCurrentChar()) == false)
				{
					return;
				}
				m_iIndex++;
			}
		}

		/// <summary>
		/// ホワイトスペース判定
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		public static bool IsWhiteSpace(char chTarget)
		{
			return ("\r\n ".IndexOf(chTarget) != -1);
		}

		/// <summary>
		/// EOF判定
		/// </summary>
		/// <returns></returns>
		public bool Eof()
		{
			return (m_iIndex >= m_strSource.Length);
		}

		/// <summary>
		/// 属性名称をパース
		/// </summary>
		public void ParseAttributeName()
		{
			EatWhiteSpace();

			// 名称を格納
			while (!Eof())
			{
				if (IsWhiteSpace(GetCurrentChar()) || (GetCurrentChar() == '=') || (GetCurrentChar() == '>'))
				{
					break;
				}

				m_strParseName += GetCurrentChar();
				m_iIndex++;
			}

			EatWhiteSpace();
		}

		/// <summary>
		/// 属性値をパース
		/// </summary>
		public void ParseAttributeValue()
		{
			if (m_chParseDelim != 0)
			{
				return;
			}

			if (GetCurrentChar() == '=')
			{
				m_iIndex++;

				EatWhiteSpace();

				if ((GetCurrentChar() == '\'') || (GetCurrentChar() == '"'))
				{
					m_chParseDelim = GetCurrentChar();
					m_iIndex++;
					while (GetCurrentChar() != m_chParseDelim)
					{
						m_strParseValue += GetCurrentChar();
						m_iIndex++;
					}
					m_iIndex++;
				}
				else
				{
					while (!Eof() &&
					  !IsWhiteSpace(GetCurrentChar()) &&
					  (GetCurrentChar() != '>'))
					{
						m_strParseValue += GetCurrentChar();
						m_iIndex++;
					}
				}

				EatWhiteSpace();
			}
		}

		/// <summary>
		/// 属性追加
		/// </summary>
		public void AddAttribute()
		{
			Add(new HtmlAttributeInfo(m_strParseName, m_strParseValue, m_chParseDelim));
		}

		/// <summary>
		/// 先頭文字取得
		/// </summary>
		/// <returns></returns>
		public char GetCurrentChar()
		{
			return GetCurrentChar(0);
		}
		/// <summary>
		/// 先頭文字取得
		/// </summary>
		/// <param name="iPeek"></param>
		/// <returns></returns>
		public char GetCurrentChar(int iPeek)
		{
			if ((m_iIndex + iPeek) < m_strSource.Length)
			{
				return m_strSource[m_iIndex + iPeek];
			}
			else
			{
				return (char)0;
			}
		}

		/// <summary>
		/// 先頭文字取得＆カウンタを進める
		/// </summary>
		/// <returns>The next character</returns>
		public char GetCharAndIncCounter()
		{
			return m_strSource[m_iIndex++];
		}

		/// <summary>
		/// カウンタを進める
		/// </summary>
		public void IncCounter()
		{
			m_iIndex++;
		}
		/// <summary>一番最近みつけた属性名</summary>
		public string ParseName
		{
			get { return m_strParseName; }
			set { m_strParseName = value; }
		}
		/// <summary>一番最近みつけた属性値</summary>
		public string ParseValue
		{
			get { return m_strParseValue; }
			set { m_strParseValue = value; }
		}
		/// <summary>一番最近みつけた属性のデリミタ</summary>
		public char ParseDelim
		{
			get { return m_chParseDelim; }
			set { m_chParseDelim = value; }
		}
		/// <summary>パースされる文字列</summary>
		public string Source
		{
			get { return m_strSource; }
			set { m_strSource = value; }
		}
	}
}
