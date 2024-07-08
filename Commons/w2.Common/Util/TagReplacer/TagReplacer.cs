/*
=========================================================================================================
  Module      : タグ置換クラス(TagReplacer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.Common.Logger;

namespace w2.Common.Util.TagReplacer
{
	/* Class説明
		 
	【 簡易 】
	XMLから設定を取得して、置換タグと、それに対応する値を設定し、
	文字列の置換処理を行うClass。
	
	【 詳細 】
	Root以下の要素を再帰的に取得し、要素名を結合して
	置換タグを生成する。
	置換タグの構成は、Root以下の階層ごとにピリオド区切りで生成。
	
	【 設定XMLサンプル 】
	──────────────────────────────────────
	<Root>
	  <A1>
		<B1>
		  <attribute key="C1" value="1" />
		  <attribute key="C2" value="2" />
		</B1>
		<B2>
		  <attribute key="C1" value="3" />
		  <attribute key="C2" value="4" />
		</B2>
	  </A1>
	  <A2>
		・・・
	  </A2>
	</Root>
	──────────────────────────────────────
	置換タグ      ： 置換後の値
	@@A1.B1.C1@@ ： 1
	@@A1.B1.C2@@ ： 2
	@@A1.B2.C1@@ ： 3
	@@A1.B2.C2@@ ： 4
	@@A2・・・
	──────────────────────────────────────
	※ 同一ブロック内では要素の名称が一意である必要があります。
	
	【 利用方法 】
	──────────────────────────────────────
	TagReplacer trReplacer = TagReplacer.GetInstance[機能名]();
	string strReplacedString = trReplacer.Replace((string)置換対象文字列);
	──────────────────────────────────────
	*/
	public class TagReplacer
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private TagReplacer()
		{
			// プロパティ初期化
			this.TagReplaceSettings = new Dictionary<string, string>();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strReplaceTagSettingPath">置換設定が記述されているファイルパス</param>
		private TagReplacer(string strReplaceTagSettingPath)
			: this()
		{
			// 置換タグ一覧の取得・設定
			var xdReplaceTags = XDocument.Load(strReplaceTagSettingPath);
			var dic = CreateReplaceKeyPairs(xdReplaceTags.Root);
			foreach (var kvpReplaceKeyPair in dic)
			{
				var keys = kvpReplaceKeyPair.Key.Split(' ');
				var key = string.Format("@@{0}@@ {1} {2}", keys[0], keys[1], keys[2]);

				this.TagReplaceSettings.Add(key, kvpReplaceKeyPair.Value);
			}
		}

		/// <summary>
		/// DataSchema置換設定のインスタンス取得
		/// </summary>
		/// <returns>DataSchema置換設定インスタンス</returns>
		public static TagReplacer GetInstanceDataSchemaSetting()
		{
			return new TagReplacer(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"\DataSchema\ReplaceTagSetting.xml");
		}

		/// <summary>
		/// 置換タグのキーペア生成
		/// </summary>
		/// <param name="xcContainer">XML要素群</param>
		/// <returns>スキーマ名.列名.属性名, 置換後の値</returns>
		private Dictionary<string, string> CreateReplaceKeyPairs(XContainer xcContainer)
		{
			var dicReplaceKeyValuePairs = new Dictionary<string, string>();

			//------------------------------------------------------
			// 現在の要素が子要素を持っている場合再帰呼び出しし
			// 全要素を取得する
			//------------------------------------------------------
			foreach (var xeChildElement in xcContainer.Elements())
			{
				var languageLocaleId = (xeChildElement.Attribute("languageLocaleId") != null)
					? xeChildElement.Attribute("languageLocaleId").Value
					: "";
				var countryIsoCode = (xeChildElement.Attribute("countryIsoCode") != null)
					? xeChildElement.Attribute("countryIsoCode").Value.ToLower()
					: "";
				if (xeChildElement.HasElements)
				{
					// 子要素のキーペアを取得する
					var dic = CreateReplaceKeyPairs(xeChildElement);
					foreach (var kvpChildElement in dic)
					{
						var dicKey = string.Format(
							"{0}.{1} {2} {3}",
							xeChildElement.Name.LocalName,
							kvpChildElement.Key,
							languageLocaleId,
							countryIsoCode);
						dicReplaceKeyValuePairs[dicKey] = kvpChildElement.Value;
					}
				}
				else
				{
					var dicKey = string.Format(
						"{0} {1} {2}",
						xeChildElement.Attribute("key").Value,
						languageLocaleId,
						countryIsoCode);
					// 現在の要素名と値のキーペアを追加する
					dicReplaceKeyValuePairs[dicKey] = xeChildElement.Attribute("value").Value;
				}
			}

			return dicReplaceKeyValuePairs;
		}

		/// <summary>
		/// タグに対応する置換後の値を取得
		/// </summary>
		/// <param name="keyTag">タグ（@@～@@）</param>
		/// <param name="languageLocaleId">言語コード</param>
		/// <param name="countryIsoCode">国ISOコード（配送先の国によって切り替わるときに利用）</param>
		/// <returns>タグに対応する値(存在しないKeyの場合はnullを返す)</returns>
		public string GetValue(
			string keyTag,
			string languageLocaleId = "",
			string countryIsoCode = "")
		{
			var dicKeys = new List<string>();
			var languageLocaleIdTmps = new[] { languageLocaleId, "" }.Distinct().ToArray();
			var countryIsoCodeTmps = new[] { StringUtility.ToEmpty(countryIsoCode).ToLower(), "" }.Distinct().ToArray();	// nullで渡ってくるところがいくつかあるためToEmptyする
			var keyTags = new[] { CreateGlobalTagName(keyTag, Constants.GLOBAL_OPTION_ENABLE ? "globalAddress" : ""), keyTag }.Distinct().ToArray();
			// languageLocaleIdは空のものでもかまわないが、countryIsoCodeがあればそれを優先したい
			foreach (var keyTagInner in keyTags)
			{
				foreach (var countryIsoCodeTmp in countryIsoCodeTmps)
				{
					foreach (var languageLocaleIdTmp in languageLocaleIdTmps)
					{
						if (string.IsNullOrEmpty(countryIsoCodeTmp) && string.IsNullOrEmpty(languageLocaleIdTmp)) continue;
						dicKeys.Add(string.Format("{0} {1} {2}", keyTagInner, languageLocaleIdTmp, countryIsoCodeTmp));
					}
				}
			}
			foreach (var keyTagInner in keyTags)
			{
				if ((string.IsNullOrEmpty(languageLocaleId) || (languageLocaleId == Constants.LANGUAGE_LOCALE_ID_JAJP))
					&& keyTagInner.Contains(".globalAddress.")) continue;
				dicKeys.Add(string.Format("{0}  ", keyTagInner));
			}

			foreach (var dicKey in dicKeys)
			{
				if (this.TagReplaceSettings.ContainsKey(dicKey))
				{
					return this.TagReplaceSettings[dicKey];
				}
			}

			return "";
		}

		/// <summary>
		/// テキスト全置換処理（Validatorなどまとめて置換する際に利用）
		/// </summary>
		/// <param name="targetString">置換対象文字列リスト</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <returns>置換後の文字列</returns>
		public string ReplaceTextAll(
			string targetString,
			string languageLocaleId = "",
			string countryIsoCode = "")
		{
			var languageLocaleIdTmps = new[] { languageLocaleId, "" }.Distinct().ToArray();
			var countryIsoCodeTmps = new[] { StringUtility.ToEmpty(countryIsoCode).ToLower(), "" }.Distinct().ToArray();	// nullで渡ってくるところがいくつかあるためToEmptyする

			var targetStringTmp = targetString;
			var maches = Regex.Matches(targetStringTmp, @"(@@[^.\s@]+(\.[^.\s@]+)+?@@)");	// @@User.birth.name[*(繰り返し)]@@ 形式のみ対象
			foreach (Match match in maches)
			{
				// 組み合わせを作成
				var dicKeys = new List<string>();
				foreach (var matchValue in new[]
				{
					CreateGlobalTagName(match.Value, Constants.GLOBAL_OPTION_ENABLE ? "globalAddress" : ""),
					match.Value,
					match.Value.Replace(".globalAddress.", "."),
				}.Distinct())
				{
					foreach (var countryIsoCodeTmp in countryIsoCodeTmps)
					{
						foreach (var languageLocaleIdTmp in languageLocaleIdTmps)
						{
							dicKeys.Add(
								string.Format("{0} {1} {2}", matchValue, languageLocaleIdTmp, countryIsoCodeTmp));
						}
					}
				}

				// 含んでいれば置換
				foreach (var dicKey in dicKeys)
				{
					if (this.TagReplaceSettings.ContainsKey(dicKey))
					{
						targetStringTmp = targetStringTmp.Replace(match.Value, this.TagReplaceSettings[dicKey]);
					}
				}
			}

			return targetStringTmp;
		}

		/// <summary>
		/// グローバルタグ名作成
		/// </summary>
		/// <param name="targetString">置換対象文字列リスト</param>
		/// <param name="globalTagPart">グローバル部分タグ</param>
		/// <returns>グローバルタグ名</returns>
		private string CreateGlobalTagName(string targetString, string globalTagPart)
		{
			if (targetString.Contains(string.Format(".{0}.", globalTagPart))) return targetString;
			if (Validator.IsHalfwidthNumber(targetString.Replace("@@", "").Trim())) return targetString;

			var lastIndexOfDot = targetString.LastIndexOf('.');
			// 空ぶったらそのまま返却
			if (lastIndexOfDot < 0) { return targetString; }

			var targetStringGlobal = targetString.Substring(0, lastIndexOfDot) + "." + globalTagPart
				+ targetString.Substring(lastIndexOfDot);
			return targetStringGlobal;
		}

		/// <summary>置換タグ設定</summary>
		protected Dictionary<string, string> TagReplaceSettings { get; set; }
	}
}
