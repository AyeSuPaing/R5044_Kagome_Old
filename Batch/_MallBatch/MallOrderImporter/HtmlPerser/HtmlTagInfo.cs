/*
=========================================================================================================
  Module      : HTMLタグ情報クラス(HtmlTagInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.MallOrderImporter.HtmlPerser
{
	public class HtmlTagInfo : HtmlAttributeInfo
	{
		protected List<HtmlAttributeInfo> m_lAttributes;

		/// <summary>
		/// クローン取得
		/// </summary>
		/// <returns></returns>
		public override Object Clone()
		{
			HtmlTagInfo rtn = new HtmlTagInfo();
			for (int i = 0; i < m_lAttributes.Count; i++)
			{
				rtn.Add((HtmlAttributeInfo)this[i].Clone());
			}
			return rtn;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HtmlTagInfo()
			: base("", "")
		{
			m_lAttributes = new List<HtmlAttributeInfo>();
		}


		/// <summary>
		/// 追加
		/// </summary>
		/// <param name="aAttribute"></param>
		public void Add(HtmlAttributeInfo haAttribute)
		{
			m_lAttributes.Add(haAttribute);
		}

		/// <summary>
		/// クリア
		/// </summary>
		public void Clear()
		{
			m_lAttributes.Clear();
		}

		/// <summary>
		/// 属性セット
		/// </summary>
		/// <param name="strName"></param>
		/// <param name="strValue"></param>
		public void Set(string strName, string strValue)
		{
			if (strName == null)
			{
				return;
			}

			if (strValue == null)
			{
				strValue = "";
			}

			HtmlAttributeInfo a = this[strName];
			if (a == null)
			{
				this.Add(new HtmlAttributeInfo(strName, strValue));
			}
			else
			{
				a.Value = strValue;
			}
		}

		/// <summary>プロパティ：属性数</summary>
		public bool IsEmpty
		{
			get { return (m_lAttributes.Count == 0); }
		}
		/// <summary>プロパティ：属性数</summary>
		public int Count
		{
			get { return m_lAttributes.Count; }
		}
		/// <summary>プロパティ：属性リスト</summary>
		public List<HtmlAttributeInfo> List
		{
			get { return m_lAttributes; }
		}
		/// <summary>プロパティ：属性指定（インデックス）</summary>
		public HtmlAttributeInfo this[int iIndex]
		{
			get
			{
				if (iIndex < m_lAttributes.Count)
					return m_lAttributes[iIndex];
				else
					return null;
			}
		}
		/// <summary>プロパティ：属性指定（名称）</summary>
		public HtmlAttributeInfo this[string strName]
		{
			get
			{
				foreach (HtmlAttributeInfo attribute in this.List)
				{
					if (attribute.Name.ToLower() == strName.ToLower())
					{
						return attribute;
					}
				}

				return null;

			}
		}
	}
}
