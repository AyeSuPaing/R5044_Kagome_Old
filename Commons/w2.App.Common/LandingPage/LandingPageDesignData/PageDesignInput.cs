/*
=========================================================================================================
  Module      : LPページデザイン入力クラス(PageDesignInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.LandingPage.LandingPageDesignData
{
	/// <summary>
	/// LPページデザイン入力クラス
	/// </summary>
	[Serializable]
	public class PageDesignInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageDesignInput() { }

		/// <summary>ページID</summary>
		public string PageId { get; set; }
		/// <summary>デザインタイプ</summary>
		public string DesignType { get; set; }
		/// <summary>ブロック入力データ</summary>
		public BlockDesignInput[] BlockSettings { get; set; }
	}

	/// <summary>
	/// LPブロックデザイン入力クラス
	/// </summary>
	[Serializable]
	public class BlockDesignInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BlockDesignInput() { }

		/// <summary>
		/// ブロック捜索
		/// </summary>
		/// <param name="targetPraceHolderName">対象のプレースホルダー名</param>
		/// <returns>プレースホルダーに指定したブロック内の要素</returns>
		public BlockElementInput FindBlock(string targetPraceHolderName)
		{
			return this.Elements.First(ele => ele.ElementPlaceHolderName == targetPraceHolderName);
		}

		/// <summary>
		/// 属性値取得
		/// </summary>
		/// <param name="targetPraceHolderName">対象のプレースホルダー名</param>
		/// <param name="attributeName">対象の属性</param>
		/// <returns>指定した属性の値</returns>
		public string GetAttributeValue(string targetPraceHolderName, string attributeName)
		{
			// 見つからない場合は空
			if (this.Elements.Any(ele => ele.ElementPlaceHolderName == targetPraceHolderName) == false) return "";

			var result = ImagePathAddDomain(attributeName, FindBlock(targetPraceHolderName).GetValue(attributeName));
			return result;
		}

		/// <summary>
		/// 属性値取得（Style形式、「属性：属性値；」の形式）
		/// </summary>
		/// <param name="targetPraceHolderName">対象のプレースホルダー名</param>
		/// <param name="attributeName">対象の属性</param>
		/// <returns>属性値</returns>
		public string GetAttributeValueStyleFormat(string targetPraceHolderName, string attributeName)
		{
			// 空の場合は空で返す
			var atrVal = this.GetAttributeValue(targetPraceHolderName, attributeName);
			if (string.IsNullOrEmpty(atrVal)) return "";

			atrVal = ImagePathAddDomain(attributeName, atrVal);
			// 空ではない場合はStyle形式で返す
			return string.Format("{0}:{1};", attributeName, atrVal);
		}

		/// <summary>
		/// 属性値取得（Style形式、「属性：属性値；」の形式）
		/// </summary>
		/// <param name="targetPraceHolderName">対象のプレースホルダー名</param>
		/// <param name="attributeNames">対象の属性（複数可）</param>
		/// <returns>属性値</returns>
		public string GetAttributeValueStyleFormat(string targetPraceHolderName, IEnumerable<string> attributeNames)
		{
			return string.Join("", attributeNames.Select(attr => this.GetAttributeValueStyleFormat(targetPraceHolderName, attr)).ToArray());
		}

		/// <summary>
		/// 要素取得
		/// </summary>
		/// <param name="targetPraceHolderName">対象のプレースホルダー名</param>
		/// <returns>指定プレースホルダー名をもつ要素</returns>
		public BlockElementInput[] GetListElementsByPlaceHolder(string targetPraceHolderName)
		{
			return this.Elements
				.Where(ele => ele.ElementPlaceHolderName == targetPraceHolderName)
				.OrderBy(ele => int.Parse(ele.ElementIndex))
				.ToArray();
		}

		/// <summary>
		/// 属性値取得
		/// </summary>
		/// <param name="targetPraceHolderName">対象のプレースホルダー名</param>
		/// <param name="attributeName">対象の属性</param>
		/// <param name="index">対象のインデックス</param>
		/// <returns>属性値</returns>
		public string GetAttributeValue(string targetPraceHolderName, string attributeName, int index)
		{
			var elements = this.GetListElementsByPlaceHolder(targetPraceHolderName);
			if ((elements == null) || (elements.Length == 0)) return "";

			var result = ImagePathAddDomain(attributeName, elements[index].GetValue(attributeName));
			return result;
		}

		/// <summary>
		/// 属性値取得（Style形式、「属性：属性値；」の形式）
		/// </summary>
		/// <param name="matchingPraceHolderName">対象のプレースホルダー名</param>
		/// <param name="attributeName">対象の属性</param>
		/// <param name="index">対象のインデックス</param>
		/// <returns>属性値</returns>
		public string GetAttributeValueStyleFormat(string matchingPraceHolderName, string attributeName, int index)
		{
			var atrVal = this.GetAttributeValue(matchingPraceHolderName, attributeName, index);
			if (string.IsNullOrEmpty(atrVal)) return "";
			// 空ではない場合はStyle形式で返す
			return string.Format("{0}:{1};", attributeName, atrVal);
		}

		/// <summary>
		/// 属性値取得（Style形式、「属性：属性値；」の形式）
		/// </summary>
		/// <param name="matchingPraceHolderName">対象のプレースホルダー名</param>
		/// <param name="attributeNames">対象の属性（複数可）</param>
		/// <param name="index">対象のインデックス</param>
		/// <returns>属性値</returns>
		public string GetAttributeValueStyleFormat(string matchingPraceHolderName, IEnumerable<string> attributeNames, int index)
		{
			return string.Join("", attributeNames.Select(attr => this.GetAttributeValueStyleFormat(matchingPraceHolderName, attr, index)).ToArray());
		}

		/// <summary>
		/// 画像パスにドメインを追加
		/// </summary>
		/// <param name="attributeName">要素名</param>
		/// <param name="value">要素内容</param>
		/// <returns>追加後の画像パス</returns>
		private string ImagePathAddDomain(string attributeName, string value)
		{
			// 処理回数削減 及び 他のアトリビュートに影響を与えないために画像パス系以外は処理をスキップ
			if ((attributeName != Constants.FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_SRC)
				&& (attributeName != Constants.FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_BACKGROUND_IMAGE)) return value;

			if ((string.IsNullOrEmpty(value)) || (value.Contains(Constants.PROTOCOL_HTTPS))) return value;

			var result = value;
			switch (attributeName)
			{
				case Constants.FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_SRC:
					result = Constants.PROTOCOL_HTTPS
						+ Constants.SITE_DOMAIN
						+ Constants.PATH_ROOT_FRONT_PC
						+ result;
					break;

				case Constants.FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_BACKGROUND_IMAGE:
					result = result
						.Replace("url(\"", "url(\"" + Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC);
					break;

				default:
					break;
			}
			return result;
		}

		/// <summary>ブロックClass名</summary>
		public string BlockClassName { get; set; }
		/// <summary>ブロックインデックス</summary>
		public string BlockIndex { get; set; }
		/// <summary>ブロック要素データ</summary>
		public BlockElementInput[] Elements { get; set; }
	}

	/// <summary>
	/// ブロック要素入力クラス
	/// </summary>
	[Serializable]
	public class BlockElementInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BlockElementInput() { }

		/// <summary>
		/// 要素内属性捜索
		/// </summary>
		/// <param name="attributeName">対象の属性</param>
		/// <returns>属性（Key、Value）</returns>
		public BlockKeyValue FindAttribute(string attributeName)
		{
			return this.Attributes.First(a => a.Attribute == attributeName);
		}

		/// <summary>
		/// 属性値取得
		/// </summary>
		/// <param name="attributeName">対象の属性</param>
		/// <returns>属性値</returns>
		public string GetValue(string attributeName)
		{
			// 見つからない場合は空
			if (this.Attributes.Any(a => a.Attribute == attributeName) == false) return "";

			return this.FindAttribute(attributeName).Value;
		}

		/// <summary>
		/// 属性値取得（Style形式、「属性：属性値；」の形式）
		/// </summary>
		/// <param name="attributeName">対象の属性</param>
		/// <returns>属性値</returns>
		public string GetValueStyleFormat(string attributeName)
		{
			var val = this.GetValue(attributeName);
			if (string.IsNullOrEmpty(val)) return "";

			// 空ではない場合はStyle形式で返す
			return string.Format("{0}:{1};", attributeName, val);
		}

		/// <summary>要素のプレースホルダー名</summary>
		public string ElementPlaceHolderName { get; set; }
		/// <summary>要素のインデックス</summary>
		public string ElementIndex { get; set; }
		/// <summary>要素の属性</summary>
		public BlockKeyValue[] Attributes { get; set; }
	}

	/// <summary>
	/// ブロック属性用KeyValue
	/// </summary>
	[Serializable]
	public class BlockKeyValue
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BlockKeyValue() { }

		/// <summary>属性</summary>
		public string Attribute { get; set; }
		/// <summary>属性値</summary>
		public string Value { get; set; }
	}
}
