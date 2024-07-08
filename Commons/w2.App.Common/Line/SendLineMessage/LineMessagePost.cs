/*
=========================================================================================================
  Module      : LINE API　送信情報送信ボディモデル (LineMessagePost.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.SendLineMessage
{
	/// <summary>
	/// LINE API 送信情報ボディモデル
	/// </summary>
	public class LineMessagePost : PostBodyBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LineMessagePost()
		{
			this.TemplateId = 0;
			this.TelNo = null;
			this.Payload = null;
		}

		/// <summary>テンプレートID</summary>
		[JsonProperty("templateId")]
		public int TemplateId { get; set; }
		/// <summary>電話番号</summary>
		[JsonProperty("telNo")]
		public string TelNo { get; set; }
		/// <remarks>未使用</remarks>
		[JsonProperty("payload")]
		public string Payload { get; set; }
		/// <summary>構造体</summary>
		[JsonProperty("value")]
		public OrderParam Value { get; set; }
	}
}
