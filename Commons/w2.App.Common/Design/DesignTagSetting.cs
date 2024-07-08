/*
=========================================================================================================
  Module      : ページ・パーツ管理 内部で利用できるタグ説明(TagSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Xml.Serialization;

namespace w2.App.Common.Design
{
	/// <summary>
	/// ページ・パーツ管理 内部で利用できるタグ説明
	/// </summary>
	[Serializable]
	public class DesignTagSetting
	{
		/// <summary>タグ内容</summary>
		[XmlElement("Raw")]
		public string Raw { get; set; }
		/// <summary>説明</summary>
		[XmlElement("Description")]
		public string Description { get; set; }
		/// <summary>タグ内容 head用</summary>
		[XmlElement("RawHead")]
		public string RawHead { get; set; }
		/// <summary>タグ内容 foot用</summary>
		[XmlElement("RawFoot")]
		public string RawFoot { get; set; }
		/// <summary>タグ内容 開始タグ</summary>
		[XmlElement("RawBgn")]
		public string RawBgn { get; set; }
		/// <summary>タグ内容 終了タグ</summary>
		[XmlElement("RawEnd")]
		public string RawEnd { get; set; }
		/// <summary>タグ内容 タグ内アイテム 開始タグ</summary>
		[XmlElement("RawItemBgn")]
		public string RawItemBgn { get; set; }
		/// <summary>タグ内容 タグ内アイテム 終了タグ</summary>
		[XmlElement("RawItemEnd")]
		public string RawItemEnd { get; set; }
	}
}