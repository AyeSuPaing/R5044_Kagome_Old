/*
=========================================================================================================
  Module      : ValidatorXml1項目作成モジュール(ValidatorXmlColumn.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml;

namespace w2.Common.Util
{
	/// <summary>
	/// ValidatorXmlColumnクラス
	/// </summary>
	[Serializable]
	public class ValidatorXmlColumn
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ValidatorXmlColumn()
		{
			this.IsComment = false;
			InitializeEmptyData();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="columnName">カラム名</param>
		public ValidatorXmlColumn(string columnName)
			: this()
		{
			this.IsComment = false;
			this.Name = columnName;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="columnNode">カラムXmlノード</param>
		public ValidatorXmlColumn(XmlNode columnNode)
		{
			// コメント判別
			if (columnNode.NodeType == XmlNodeType.Comment)
			{
				// 空データで初期化
				InitializeEmptyData();
				// コメントノードとして格納
				this.Comment = (XmlComment)columnNode;
				this.IsComment = true;
			}
			else
			{
				this.IsComment = false;

				// カラムノードの名称を取得
				if (columnNode.Attributes["name"] != null)
				{
					this.Name = columnNode.Attributes["name"].Value;
				}

				// InnerTextをロード
				this.DisplayName = LoadInnerText(columnNode, "name");
				this.Necessary = LoadInnerText(columnNode, "necessary");
				this.Type = LoadInnerText(columnNode, "type");
				this.Accept = LoadInnerText(columnNode, "accept");
				this.Prohibit = LoadInnerText(columnNode, "prohibit");
				this.Length = LoadInnerText(columnNode, "length");
				this.LengthMax = LoadInnerText(columnNode, "length_max");
				this.LengthMin = LoadInnerText(columnNode, "length_min");
				this.ByteLength = LoadInnerText(columnNode, "byte_length");
				this.ByteLengthMax = LoadInnerText(columnNode, "byte_length_max");
				this.ByteLengthMin = LoadInnerText(columnNode, "byte_length_min");
				this.NumberMax = LoadInnerText(columnNode, "number_max");
				this.NumberMin = LoadInnerText(columnNode, "number_min");
				this.ExceptSjis = LoadInnerText(columnNode, "except_sjis");
				this.ExceptJis = LoadInnerText(columnNode, "except_jis");
				this.Regexp = LoadInnerText(columnNode, "regexp");
				this.Confirm = LoadInnerText(columnNode, "confirm");
				this.Equivalence = LoadInnerText(columnNode, "equivalence");
				this.DifferentValue = LoadInnerText(columnNode, "different_value");
				this.Duplication = LoadInnerText(columnNode, "duplication");
				this.DuplicationDaterange = LoadInnerText(columnNode, "duplication_daterange");

				// Attributeをロード
				this.RegexpOutline = LoadElementAttribute(columnNode, "regexp", "ptn");
				this.EquivalenceTargetName = LoadElementAttribute(columnNode, "equivalence", "target_name");
				this.DifferentValueTargetName = LoadElementAttribute(columnNode, "different_value", "target_name");
				this.DuplicationPage = LoadElementAttribute(columnNode, "duplication", "page");
				this.DuplicationStatement = LoadElementAttribute(columnNode, "duplication", "statement");
				this.DuplicationDaterangePage = LoadElementAttribute(columnNode, "duplication_daterange", "page");
				this.DuplicationDaterangeStatement = LoadElementAttribute(columnNode, "duplication_daterange", "statement");
				this.ExceptSjisName = LoadElementAttribute(columnNode, "except_sjis", "exept_name");
				this.ExceptJisName = LoadElementAttribute(columnNode, "except_jis", "exept_name");
			}
		}

		/// <summary>
		/// プロパティ初期化
		/// </summary>
		private void InitializeEmptyData()
		{
			this.Name = "";
			this.DisplayName = "";
			this.Necessary = "";
			this.Type = "";
			this.Accept = "";
			this.Prohibit = "";
			this.Length = "";
			this.LengthMax = "";
			this.LengthMin = "";
			this.ByteLength = "";
			this.ByteLengthMax = "";
			this.ByteLengthMin = "";
			this.NumberMax = "";
			this.NumberMin = "";
			this.ExceptSjis = "";
			this.ExceptJis = "";
			this.Regexp = "";
			this.Confirm = "";
			this.Equivalence = "";
			this.DifferentValue = "";
			this.Duplication = "";
			this.DuplicationDaterange = "";
			this.RegexpOutline = "";
			this.EquivalenceTargetName = "";
			this.DifferentValueTargetName = "";
			this.DuplicationPage = "";
			this.DuplicationStatement = "";
			this.DuplicationDaterangePage = "";
			this.DuplicationDaterangeStatement = "";
			this.ExceptSjisName = "";
			this.ExceptJisName = "";
		}

		/// <summary>
		/// 各要素のInnerTextロード処理
		/// </summary>
		/// <param name="columnNode">対象ノード</param>
		/// <param name="elementName">要素名</param>
		/// <returns>InnerTextの内容</returns>
		private string LoadInnerText(XmlNode columnNode, string elementName)
		{
			string result = "";

			if (columnNode.SelectSingleNode(elementName) != null)
			{
				result = columnNode.SelectSingleNode(elementName).InnerText;
			}

			return result;
		}

		/// <summary>
		/// 各要素のAttributeロード処理
		/// </summary>
		/// <param name="columnNode">対象ノード</param>
		/// <param name="elementName">要素名</param>
		/// <param name="attributeName">Attribute名称</param>
		/// <returns></returns>
		private string LoadElementAttribute(XmlNode columnNode, string elementName, string attributeName)
		{
			string result = "";
			if ((columnNode.SelectSingleNode(elementName) != null) && (columnNode.SelectSingleNode(elementName).Attributes[attributeName] != null))
			{
				result = columnNode.SelectSingleNode(elementName).Attributes[attributeName].Value;
			}
			return result;
		}

		/// <summary>
		/// Validator用XmlDocument生成
		/// </summary>
		/// <returns>Validator用XmlDocument</returns>
		public XmlDocument CreateColumnXml()
		{
			XmlDocument resultDocument = new XmlDocument();

			// カラム用ノード作成
			XmlElement columnNode = resultDocument.CreateElement("Column");
			columnNode.SetAttribute("name", this.Name);
			resultDocument.AppendChild(columnNode);

			AppendElement(columnNode, "name", this.DisplayName, null);
			AppendElement(columnNode, "necessary", this.Necessary, null);
			AppendElement(columnNode, "type", this.Type, null);
			AppendElement(columnNode, "length", this.Length, null);
			AppendElement(columnNode, "length_max", this.LengthMax, null);
			AppendElement(columnNode, "length_min", this.LengthMin, null);
			AppendElement(columnNode, "byte_length", this.ByteLength, null);
			AppendElement(columnNode, "byte_length_max", this.ByteLengthMax, null);
			AppendElement(columnNode, "byte_length_min", this.ByteLengthMin, null);
			AppendElement(columnNode, "number_max", this.NumberMax, null);
			AppendElement(columnNode, "number_min", this.NumberMin, null);
			AppendElement(columnNode, "confirm", this.Confirm, null);

			Dictionary<string, string> regexpAttr = new Dictionary<string, string>();
			regexpAttr.Add("ptn",this.RegexpOutline);
			AppendElement(columnNode, "regexp", this.Regexp, regexpAttr);

			Dictionary<string, string> equivalenceAtrr = new Dictionary<string, string>();
			equivalenceAtrr.Add("target_name", this.EquivalenceTargetName);
			AppendElement(columnNode, "equivalence", this.Equivalence, equivalenceAtrr);

			Dictionary<string, string> differentValueAttr = new Dictionary<string, string>();
			differentValueAttr.Add("target_name", this.DifferentValueTargetName);
			AppendElement(columnNode, "different_value", this.DifferentValue, differentValueAttr);

			Dictionary<string, string> duplicationAttr = new Dictionary<string, string>();
			duplicationAttr.Add("page", this.DuplicationPage);
			duplicationAttr.Add("statement", this.DuplicationStatement);
			AppendElement(columnNode, "duplication", this.Duplication, duplicationAttr);

			Dictionary<string, string> duplicationDaterangeAttr = new Dictionary<string, string>();
			duplicationDaterangeAttr.Add("page", this.DuplicationDaterangePage);
			duplicationDaterangeAttr.Add("statement", this.DuplicationDaterangeStatement);
			AppendElement(columnNode, "duplication_daterange", this.DuplicationDaterange, duplicationDaterangeAttr);

			AppendElement(columnNode, "accept", this.Accept, null);
			AppendElement(columnNode, "prohibit", this.Prohibit, null);

			Dictionary<string, string> exceptSjisAttr = new Dictionary<string, string>();
			exceptSjisAttr.Add("exept_name", this.ExceptSjisName);
			AppendElement(columnNode, "except_sjis", this.ExceptSjis, exceptSjisAttr);

			Dictionary<string, string> exceptJisAttr = new Dictionary<string, string>();
			exceptJisAttr.Add("exept_name", this.ExceptJisName);
			AppendElement(columnNode, "except_jis", this.ExceptJis, exceptJisAttr);

			return resultDocument;
		}

		/// <summary>
		/// 各要素追加
		/// </summary>
		/// <param name="columnNode">要素追加先Node</param>
		/// <param name="elementName">要素名</param>
		/// <param name="innerText">InnerText</param>
		/// <param name="attrList">Attribute</param>
		private void AppendElement(XmlNode columnNode, string elementName, string innerText, Dictionary<string,string> attrList)
		{
			// InnerTextが空であれば追加は行わない
			// 最低限、nameの属性とノードは必要
			if ((innerText != "") || (elementName == "name"))
			{
				// Element作成
				XmlElement element = columnNode.OwnerDocument.CreateElement(elementName);
				element.InnerText = innerText;

				// Attribute設定
				if ((attrList != null) && (attrList.Count > 0))
				{
					foreach (KeyValuePair<string, string> attr in attrList)
					{
						element.SetAttribute(attr.Key, attr.Value);
					}
				}

				// 要素追加先ノードに追加
				columnNode.AppendChild(element);
			}
		}

		#region プロパティ

		# region 属性
		/// <summary>カラム名</summary>
		public string Name { get; set; }
		/// <summary>表示名</summary>
		public string DisplayName { get; set; }
		/// <summary>必須チェック有無</summary>
		public string Necessary { get; set; }
		/// <summary>チェックタイプ</summary>
		public string Type { get; set; }
		/// <summary>入力許可文字列</summary>
		public string Accept { get; set; }
		/// <summary>入力禁止文字</summary>
		public string Prohibit { get; set; }
		/// <summary>文字列長</summary>
		public string Length { get; set; }
		/// <summary>最大文字列長</summary>
		public string LengthMax { get; set; }
		/// <summary>最小文字列長</summary>
		public string LengthMin { get; set; }
		/// <summary>バイト長</summary>
		public string ByteLength { get; set; }
		/// <summary>最大バイト長</summary>
		public string ByteLengthMax { get; set; }
		/// <summary>最小バイト長</summary>
		public string ByteLengthMin { get; set; }
		/// <summary>最大値（数値）</summary>
		public string NumberMax { get; set; }
		/// <summary>最小値（数値）</summary>
		public string NumberMin { get; set; }
		/// <summary>SJISチェック有無</summary>
		public string ExceptSjis { get; set; }
		public string ExceptSjisName { get; set; }
		/// <summary>JISチェック有無</summary>
		public string ExceptJis { get; set; }
		public string ExceptJisName { get; set; }
		/// <summary>正規表現</summary>
		public string Regexp { get; set; }
		/// <summary>正規表現（説明用）</summary>
		public string RegexpOutline { get; set; }
		/// <summary>確認用入力チェック</summary>
		public string Confirm { get; set; }
		/// <summary>同値チェック</summary>
		public string Equivalence { get; set; }
		/// <summary>同値チェック対象名（表示用）</summary>
		public string EquivalenceTargetName { get; set; }
		/// <summary>異値チェック</summary>
		public string DifferentValue { get; set; }
		/// <summary>異値チェック対象名（表示用）</summary>
		public string DifferentValueTargetName { get; set; }
		/// <summary>重複チェック</summary>
		public string Duplication { get; set; }
		public string DuplicationPage { get; set; }
		public string DuplicationStatement { get; set; }
		/// <summary>重複チェック（同一期間内の同一項目重複チェック）</summary>
		public string DuplicationDaterange { get; set; }
		public string DuplicationDaterangePage { get; set; }
		public string DuplicationDaterangeStatement { get; set; }
		# endregion

		/// <summary>コメントフラグ</summary>
		public bool IsComment { get; set; }
		/// <summary>コメント</summary>
		public XmlComment Comment { get; set; }
		#endregion
	}
}
