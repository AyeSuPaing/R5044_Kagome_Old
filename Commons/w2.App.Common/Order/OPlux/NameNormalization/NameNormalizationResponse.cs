/*
=========================================================================================================
  Module      : Name Normalization Response(NameNormalizationResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.OPlux.NameNormalization
{
	/// <summary>
	/// Name normalization response
	/// </summary>
	public class NameNormalizationResponse
	{
		/// <summary>処理時間</summary>
		[JsonProperty("response")]
		public Response ResponseObject { get; set; }

		/// <summary>
		/// 処理時間
		/// </summary>
		public class Response
		{
			/// <summary>正規化名前</summary>
			[JsonProperty("firstName")]
			public FirstName FirstName { get; set; }
			/// <summary>正規化苗字</summary>
			[JsonProperty("lastName")]
			public LastName LastName { get; set; }
			/// <summary>結果</summary>
			[JsonProperty("result")]
			public string Result { get; set; }
			/// <summary>文字数カウント</summary>
			[JsonProperty("letterCount")]
			public LetterCount LetterCount { get; set; }
			/// <summary>処理時間</summary>
			[JsonProperty("time")]
			public int Time { get; set; }
			/// <summary>エラータグ</summary>
			[JsonProperty("errors")]
			public Errors Errors { get; set; }
		}

		/// <summary>
		/// 正規化名前
		/// </summary>
		public class FirstName
		{
			/// <summary>正規化名前アルファベット</summary>
			[JsonProperty("alphabet")]
			public string Alphabet { get; set; }
			/// <summary>正規化名前読み</summary>
			[JsonProperty("reading")]
			public string Reading { get; set; }
			/// <summary>正規化名前書き</summary>
			[JsonProperty("writing")]
			public string Writing { get; set; }
		}

		/// <summary>
		/// 正規化苗字
		/// </summary>
		public class LastName
		{
			/// <summary>正規化苗字アルファベット</summary>
			[JsonProperty("alphabet")]
			public string Alphabet { get; set; }
			/// <summary>苗字辞書存在フラグ</summary>
			[JsonProperty("existed")]
			public string Existed { get; set; }
			/// <summary>正規化苗字読み</summary>
			[JsonProperty("reading")]
			public string Reading { get; set; }
			/// <summary>正規化苗字書き</summary>
			[JsonProperty("writing")]
			public string Writing { get; set; }
		}

		/// <summary>
		/// 文字数カウント
		/// </summary>
		public class LetterCount
		{
			/// <summary>氏名アルファベット数</summary>
			[JsonProperty("alphabetCountInName")]
			public int AlphabetCountInName { get; set; }
			/// <summary>氏名ひらがな数</summary>
			[JsonProperty("hiraganaCountInName")]
			public int HiraganaCountInName { get; set; }
			/// <summary>氏名漢字数</summary>
			[JsonProperty("kanjiCountInName")]
			public int KanjiCountInName { get; set; }
			/// <summary>氏名カタカナ数</summary>
			[JsonProperty("katakanaCountInName")]
			public int KatakanaCountInName { get; set; }
			/// <summary>氏名文字数</summary>
			[JsonProperty("nameLength")]
			public int NameLength { get; set; }
			/// <summary>氏名その他文字数</summary>
			[JsonProperty("otherCountInName")]
			public int OtherCountInName { get; set; }
		}

		/// <summary>
		/// エラータグ
		/// </summary>
		public class Errors
		{
			/// <summary>エラー詳細</summary>
			[JsonProperty("error")]
			public Error Error { get; set; }
		}

		/// <summary>
		/// エラー詳細
		/// </summary>
		public class Error
		{
			/// <summary>エラーコード</summary>
			[JsonProperty("code")]
			public string Code { get; set; }
			/// <summary>エラーメッセージ</summary>
			[JsonProperty("message")]
			public string Message { get; set; }
		}
	}
}
