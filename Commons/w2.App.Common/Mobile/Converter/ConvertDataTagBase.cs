/*
=========================================================================================================
  Module      : データタグ変換基底クラス(ConvertDataTagBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace w2.App.Common.Mobile.Converter
{
	public abstract class ConvertDataTagBase
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected ConvertDataTagBase()
		{
			// 何もしない
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="html">変換対象文字列</param>
		/// <param name="data">データ</param>
		// HACK:paramsのobjectで渡し各クラスで引数の内容を判断がよいかもしれない
		protected ConvertDataTagBase(StringBuilder html, Hashtable data)
		{
			this.Html = html;
			this.Data = data;
		}

		/// <summary>
		/// タグ置換
		/// </summary>
		public void Convert()
		{
			ConvertTag();
		}

		/// <summary>
		/// タグ変換
		/// </summary>
		/// <remarks>各タグ変換クラスで実装必須</remarks>
		protected abstract void ConvertTag();

		/// <summary>
		/// タグパターンマッチコレクション取得
		/// </summary>
		/// <param name="tagHead">タグ先頭</param>
		/// <param name="tagFoot">タグ終端</param>
		/// <returns>タグパターンマッチコレクション取得</returns>
		protected MatchCollection GetTagMatches(string tagHead, string tagFoot)
		{
			return Regex.Matches(this.Html.ToString(), tagHead + ".*?" + tagFoot, RegexOptions.Singleline | RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// タグ内部属性文字取得
		/// </summary>
		/// <param name="tagData">対象タグデータ</param>
		/// <param name="tagHead">タグ先頭</param>
		/// <param name="tagFoot">タグ終端</param>
		/// <returns>内部属性</returns>
		protected string GetTagInner(string tagData, string tagHead, string tagFoot)
		{
			// 先頭 or 終端にマッチするパターン
			StringBuilder pattern = new StringBuilder("(").Append(tagHead).Append(")|(").Append(tagFoot).Append(")");

			// タグ部分を削除（大文字小文字区別しない）
			return Regex.Replace(tagData, pattern.ToString(), "", RegexOptions.IgnoreCase);
		}

		/// <summary>HTML文字列</summary>
		protected StringBuilder Html { get; set; }
		/// <summary>データ</summary>
		protected Hashtable Data { get; set; }
	}
}
