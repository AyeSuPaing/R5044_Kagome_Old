/*
=========================================================================================================
  Module      : ツリービュー(TreeView.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;

namespace w2.Cms.Manager.Codes.TreeView
{
	/// <summary>  
	/// アイテムのコレクションで再帰的にHtml木を作成
	/// </summary>  
	public class TreeView<T> : IHtmlString
	{
		/// <summary>Htmlヘルパー</summary>
		private readonly HtmlHelper m_html;
		/// <summary>コレクション</summary>
		private readonly IEnumerable<T> m_items = Enumerable.Empty<T>();
		/// <summary>表示プロパティ</summary>
		private Func<T, string> m_displayProperty = item => item.ToString();
		/// <summary>子プロパティ</summary>
		private Func<T, IEnumerable<T>> m_childrenProperty;
		/// <summary>空コンテンツ</summary>
		private string m_emptyContent = "No children";
		/// <summary>Html属性</summary>
		private IDictionary<string, object> m_htmlAttributes = new Dictionary<string, object>();
		/// <summary>子Html属性</summary>
		private IDictionary<string, object> m_childHtmlAttributes = new Dictionary<string, object>();
		/// <summary>テンプレート</summary>
		private Func<T, HelperResult> m_itemTemplate;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="html">ヘルパー</param>
		/// <param name="items">コレクション</param>
		public TreeView(HtmlHelper html, IEnumerable<T> items)
		{
			if (html == null) throw new ArgumentNullException("html");
			m_html = html;
			m_items = items;
			m_itemTemplate = item => new HelperResult(writer => writer.Write(m_displayProperty(item)));
		}

		/// <summary>
		/// 各項目にレンダリングされたテキストを表示するプロパティ
		/// </summary>
		/// <param name="selector">セレクタ</param>
		public TreeView<T> ItemText(Func<T, string> selector)
		{
			if (selector == null) throw new ArgumentNullException("selector");
			m_displayProperty = selector;
			return this;
		}


		/// <summary>
		/// 各項目をレンダリングするときに使うテンプレート
		/// </summary>
		/// <param name="itemTemplate">テンプレート</param>
		public TreeView<T> ItemTemplate(Func<T, HelperResult> itemTemplate)
		{
			if (itemTemplate == null) throw new ArgumentNullException("itemTemplate");
			m_itemTemplate = itemTemplate;
			return this;
		}


		/// <summary>
		/// 子項目を返す
		/// </summary>
		/// <param name="selector">セレクタ</param>
		public TreeView<T> Children(Func<T, IEnumerable<T>> selector)
		{
			m_childrenProperty = selector;
			return this;
		}

		/// <summary>
		/// リストが空だった時に表示するコンテンツ
		/// </summary>
		/// <param name="emptyContent">コンテンツ</param>
		public TreeView<T> EmptyContent(string emptyContent)
		{
			if (emptyContent == null) throw new ArgumentNullException("emptyContent");
			m_emptyContent = emptyContent;
			return this;
		}

		/// <summary>
		/// ルートul属性に追加するhtml属性
		/// </summary>
		/// <param name="htmlAttributes">html属性</param>
		public TreeView<T> HtmlAttributes(object htmlAttributes)
		{
			HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
			return this;
		}
		/// <summary>
		/// ルートul属性に追加するhtml属性
		/// </summary>
		/// <param name="htmlAttributes">html属性</param>
		public TreeView<T> HtmlAttributes(IDictionary<string, object> htmlAttributes)
		{
			if (htmlAttributes == null) throw new ArgumentNullException("htmlAttributes");
			m_htmlAttributes = htmlAttributes;
			return this;
		}

		/// <summary>
		/// 子項目に追加するhtml属性
		/// </summary>
		/// <param name="htmlAttributes">html属性</param>
		public TreeView<T> ChildrenHtmlAttributes(object htmlAttributes)
		{
			ChildrenHtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
			return this;
		}
		/// <summary>
		/// 子項目に追加するhtml属性
		/// </summary>
		/// <param name="htmlAttributes">html属性</param>
		public TreeView<T> ChildrenHtmlAttributes(IDictionary<string, object> htmlAttributes)
		{
			if (htmlAttributes == null) throw new ArgumentNullException("htmlAttributes");
			m_childHtmlAttributes = htmlAttributes;
			return this;
		}

		/// <summary>
		/// Html文字列化
		/// </summary>
		/// <returns>文字列</returns>
		public string ToHtmlString()
		{
			return ToString();
		}

		/// <summary>
		/// レンダリング
		/// </summary>
		public void Render()
		{
			var writer = m_html.ViewContext.Writer;
			using (var textWriter = new HtmlTextWriter(writer))
			{
				textWriter.Write(ToString());
			}
		}

		/// <summary>
		/// 設定の検証
		/// </summary>
		private void ValidateSettings()
		{
			if (m_childrenProperty == null)
			{
				return;
			}
		}

		/// <summary>
		/// 文字列化
		/// </summary>
		/// <returns>Htmlテキスト</returns>
		public override string ToString()
		{
			ValidateSettings();
			var listItems = new List<T>();
			if (m_items != null)
			{
				listItems = m_items.ToList();
			}


			var ul = new TagBuilder("ul");
			ul.MergeAttributes(m_htmlAttributes);
			var li = new TagBuilder("li")
			{
				InnerHtml = m_emptyContent
			};
			li.MergeAttribute("id", "-1");

			if (listItems.Count > 0)
			{
				var innerUl = new StringBuilder();
				AddUlTag(innerUl);
				foreach (var item in listItems)
				{
					BuildNestedTag(innerUl, item, m_childrenProperty);
				}
				innerUl.Append("</ul>");
				li.InnerHtml += innerUl.ToString();
			}
			ul.InnerHtml += li.ToString();

			return ul.ToString();
		}

		/// <summary>
		/// 子項目を追加
		/// </summary>
		/// <param name="parentTag">親タグ</param>
		/// <param name="parentItem">親項目</param>
		/// <param name="childrenProperty">子プロパティ</param>
		private void AppendChildren(StringBuilder parentTag, T parentItem, Func<T, IEnumerable<T>> childrenProperty)
		{
			if (childrenProperty == null)
			{
				return;
			}
			var children = childrenProperty(parentItem).ToList();
			if (children.Any() == false)
			{
				return;
			}

			var innerUl = new StringBuilder();
			AddUlTag(innerUl);
			foreach (var item in children)
			{
				BuildNestedTag(innerUl, item, childrenProperty);
			}
			parentTag.Append(innerUl).Append("</ul>");
		}

		/// <summary>
		/// ネストタグ作成
		/// </summary>
		/// <param name="parentTag">親タグ</param>
		/// <param name="parentItem">親アイテム</param>
		/// <param name="childrenProperty">子プロパティ</param>
		private void BuildNestedTag(StringBuilder parentTag, T parentItem, Func<T, IEnumerable<T>> childrenProperty)
		{
			var li = GetLi(parentItem);
			var innerLi = new StringBuilder();
			innerLi.Append(li.InnerHtml);

			parentTag.Append(li.ToString(TagRenderMode.StartTag));
			AppendChildren(innerLi, parentItem, childrenProperty);
			parentTag.Append(innerLi).Append(li.ToString(TagRenderMode.EndTag));
		}

		/// <summary>
		/// liタグ作成
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <returns>liタグ</returns>
		private TagBuilder GetLi(T item)
		{
			var li = new TagBuilder("li")
			{
				InnerHtml = m_itemTemplate(item).ToHtmlString()
			};
			var myType = item.GetType();
			var props = new List<PropertyInfo>(myType.GetProperties());
			foreach (var prop in props)
			{
				if (prop.Name.ToLower() == "id")
					li.MergeAttribute("id", prop.GetValue(item, null).ToString());
				if (prop.Name.ToLower() == "sortorder")
					li.MergeAttribute("priority", prop.GetValue(item, null).ToString());
			}
			return li;
		}

		/// <summary>
		/// ulタグを使いするメソッド
		/// NOTE : TagManagerを使わなかった理由は、TagManagerだと高速化が難しかったため
		/// </summary>
		/// <param name="tag">Tagを扱うStringBuilder(副作用注意)</param>
		private void AddUlTag(StringBuilder tag)
		{
			tag.Append("<ul ");
			tag.Append(MergeAttributes(m_childHtmlAttributes));
		}

		/// <summary>
		/// Attributesをタグに追加する
		/// </summary>
		/// <param name="htmlAttributes">HTMLのAttributesの辞書</param>
		/// <returns>文字列(終端の">"つき)</returns>
		private string MergeAttributes(IDictionary<string, object> htmlAttributes)
		{
			var list = htmlAttributes.Select(x => x.Key + " = \"" + (string)x.Value + "\"");
			var attributes = string.Join(" ", list);
			var result = attributes + " >";
			return result;
		}
	}
}