/*
=========================================================================================================
  Module      : 自動翻訳設定一覧検索のためのヘルパクラス (AutoTranslationWordListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.AutoTranslationWord.Helper
{
	/// <summary>
	/// 自動翻訳設定一覧検索のためのヘルパクラス
	/// </summary>
	public class AutoTranslationWordListSearchCondition : BaseDbMapModel
	{
		/// <summary>翻訳元ワード</summary>
		public string WordBefore { get; set; }
		/// <summary>検索自動翻訳元ワード（SQL LIKEエスケープ済）</summary>
		[DbMapName("word_before_like_escaped")]
		public string WordBeforeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.WordBefore); }
		}
		/// <summary>言語コード</summary>
		public string LanguageCode { get; set; }
		/// <summary>検索言語コード（SQL LIKEエスケープ済）</summary>
		[DbMapName("language_code_like_escaped")]
		public string LanguageCodeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.LanguageCode); }
		}
		/// <summary>開始行</summary>
		[DbMapName("bgn_row_num")]
		public int BgnRowNum { get; set; }
		/// <summary>終了行</summary>
		[DbMapName("end_row_num")]
		public int EndRowNum { get; set; }
	}

	#region +自動翻訳検索結果クラス
	/// <summary>
	/// 自動翻訳検索結果クラス
	/// ※ AutoTranslationWordModelを拡張
	/// </summary>
	[Serializable]
	public class AutoTranslationWordListSearchResult : AutoTranslationWordModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AutoTranslationWordListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion
	}
	#endregion
}
