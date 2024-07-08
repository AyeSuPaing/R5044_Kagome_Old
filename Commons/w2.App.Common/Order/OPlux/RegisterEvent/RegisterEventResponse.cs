/*
=========================================================================================================
  Module      : Register Event Response(RegisterEventResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.OPlux.RegisterEvent
{
	/// <summary>
	/// Register event response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	public class RegisterEventResponse : BaseOPluxResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RegisterEventResponse()
			: base()
		{
		}

		/// <summary>処理時間</summary>
		[XmlElement("time")]
		public string Time { get; set; }
		/// <summary>処理結果</summary>
		[XmlElement("result")]
		public string Result { get; set; }
		/// <summary>エラータグ</summary>
		[XmlElement("errors")]
		public ErrorsElement Errors { get; set; }
		/// <summary>イベント</summary>
		[XmlElement("event")]
		public EventElement Event { get; set; }

		/// <summary>
		/// Errors element
		/// </summary>
		public class ErrorsElement
		{
			/// <summary>エラー詳細</summary>
			[XmlElement("error")]
			public ErrorElement[] Error { get; set; }
		}

		/// <summary>
		/// Error element
		/// </summary>
		public class ErrorElement
		{
			/// <summary>エラーコード</summary>
			[XmlElement("code")]
			public string Code { get; set; }
			/// <summary>エラーメッセージ</summary>
			[XmlElement("message")]
			public string Message { get; set; }
		}

		/// <summary>
		/// Event element
		/// </summary>
		public class EventElement
		{
			/// <summary>イベントID</summary>
			[XmlElement("id")]
			public string Id { get; set; }
			/// <summary>審査結果コード</summary>
			[XmlElement("result")]
			public string Result { get; set; }
			/// <summary>スキップフラグ</summary>
			[XmlElement("skipped")]
			public string Skipped { get; set; }
			/// <summary>スコア合計</summary>
			[XmlElement("score")]
			public ScoreElement Score { get; set; }
			/// <summary>発動ルール</summary>
			[XmlElement("rules")]
			public RulesElement Rules { get; set; }
			/// <summary>ルールグループ</summary>
			[XmlElement("ruleGroup")]
			public string RuleGroup { get; set; }
			/// <summary>関連イベント</summary>
			[XmlElement("similars")]
			public SimilarsElement Similars { get; set; }
		}

		/// <summary>
		/// Score element
		/// </summary>
		public class ScoreElement
		{
			/// <summary>OKスコア合計</summary>
			[XmlElement("ok")]
			public string Ok { get; set; }
			/// <summary>NGスコア合計</summary>
			[XmlElement("ng")]
			public string Ng { get; set; }
			/// <summary>HOLDスコア合計</summary>
			[XmlElement("hold")]
			public string Hold { get; set; }
		}

		/// <summary>
		/// Rules element
		/// </summary>
		public class RulesElement
		{
			/// <summary>ルール</summary>
			[XmlElement("rule")]
			public RuleElement[] Rule { get; set; }
		}

		/// <summary>
		/// Rule element
		/// </summary>
		public class RuleElement
		{
			/// <summary>ルールコード</summary>
			[XmlElement("code")]
			public string Code { get; set; }
			/// <summary>OKスコア</summary>
			[XmlElement("ok")]
			public string Ok { get; set; }
			/// <summary>NGスコア</summary>
			[XmlElement("ng")]
			public string Ng { get; set; }
			/// <summary>HOLDスコア</summary>
			[XmlElement("hold")]
			public string Hold { get; set; }
			/// <summary>タッチポイント</summary>
			[XmlElement("touchpoint")]
			public string Touchpoint { get; set; }
			/// <summary>ルール説明文</summary>
			[XmlElement("description")]
			public string Description { get; set; }
		}

		/// <summary>
		/// Similars element
		/// </summary>
		public class SimilarsElement
		{
			/// <summary>関連イベント情報</summary>
			[XmlElement("similar")]
			public SimilarElement[] Similar { get; set; }
		}

		/// <summary>
		/// Similar element
		/// </summary>
		public class SimilarElement
		{
			/// <summary>関連イベントID</summary>
			[XmlElement("event_id")]
			public string EventId { get; set; }
			/// <summary>マッチングルールコード</summary>
			[XmlElement("rule_code")]
			public string RuleCode { get; set; }
			/// <summary>タッチポイント</summary>
			[XmlElement("touchpoints")]
			public string Touchpoints { get; set; }
		}
	}
}
