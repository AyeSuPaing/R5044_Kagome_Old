/*
=========================================================================================================
  Module      : HTMLパーサクラス(HtmlParser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.MallOrderImporter.HtmlPerser
{
	public class HtmlParser : HtmlParserBase
	{
		/// <summary>
		/// タグ取得
		/// </summary>
		/// <returns></returns>
		public HtmlTagInfo GetTag()
		{
			HtmlTagInfo alTag = new HtmlTagInfo();
			alTag.Name = m_strTag;

			foreach (HtmlAttributeInfo aChild in this.List)
			{
				alTag.Add((HtmlAttributeInfo)aChild.Clone());
			}

			return alTag;
		}


		/// <summary>
		/// タグをパース
		/// </summary>
		protected void ParseTag()
		{
			m_strTag = "";
			Clear();

			// コメント？
			if ((GetCurrentChar() == '!') && (GetCurrentChar(1) == '-') && (GetCurrentChar(2) == '-'))
			{
				while (!Eof())
				{
					if ((GetCurrentChar() == '-') && (GetCurrentChar(1) == '-') && (GetCurrentChar(2) == '>'))
					{
						break;
					}

					if (GetCurrentChar() != '\n')
					{
						m_strTag += GetCurrentChar();
					}
					IncCounter();
				}
				m_strTag += "--";
				IncCounter();
				IncCounter();
				IncCounter();

				this.ParseDelim = (char)0;
				return;
			}

			// タグ名を探索
			while (!Eof())
			{
				if (IsWhiteSpace(GetCurrentChar()) || (GetCurrentChar() == '>'))
				{
					break;
				}
				m_strTag += GetCurrentChar();
				IncCounter();
			}

			EatWhiteSpace();

			// 属性取得
			while (GetCurrentChar() != '>')
			{
				ParseName = "";
				ParseValue = "";
				ParseDelim = (char)0;

				ParseAttributeName();

				if (GetCurrentChar() == '>')
				{
					AddAttribute();
					break;
				}

				// 値取得
				ParseAttributeValue();
				AddAttribute();
			}
			IncCounter();
		}

		/// <summary>
		/// パース
		/// </summary>
		/// <returns></returns>
		public char Parse()
		{
			var ch = GetCurrentChar();
			if (ch == '<')
			{
				IncCounter();
				ch = char.ToUpper(GetCurrentChar());
				if ((ch >= 'A') && (ch <= 'Z') || (ch == '!') || (ch == '/'))
				{
					ParseTag();
					return (char)0;
				}
				return (GetCharAndIncCounter());
			}
			return (GetCharAndIncCounter());
		}
	}
}
