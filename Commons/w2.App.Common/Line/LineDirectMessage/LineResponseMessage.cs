/*
=========================================================================================================
  Module      : LINEレスポンスメッセージクラス(LineResponseMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace w2.App.Common.Line.LineDirectMessage
{
	/// <summary>
	/// LINEレスポンスメッセージクラス
	/// </summary>
	public class LineResponseMessage
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">LINE送信結果のステータスコード</param>
		/// <param name="jsonObj">LINE送信結果のJSONオブジェクト</param>
		public LineResponseMessage(HttpStatusCode statusCode,JObject jsonObj)
		{
			this.StatusCode = statusCode;
			this.Message = (jsonObj["message"] != null) ? jsonObj["message"].ToString() : string.Empty;
			var details =  (jsonObj["details"] != null) ? jsonObj["details"].AsJEnumerable() : null;
			if (details == null)
			{
				this.Details = new[]
				{
					new Detail
					{
						Message = string.Empty,
						Property = string.Empty
					}
				};
			}
			else
			{
				this.Details = details.Select(
						detail => new Detail
						{
							Message = (detail["message"] != null) ? detail["message"].ToString() : string.Empty,
							Property = (detail["property"] != null) ? detail["property"].ToString() : string.Empty
						}).ToArray();
			}
		}

		/// <summary> ステータスコード </summary>
		public HttpStatusCode StatusCode { get; set; }
		/// <summary> エラー情報 </summary>
		public string Message { get; set; }
		/// <summary> エラー詳細 </summary>
		public Detail[] Details { get; set; }
	}

	/// <summary>
	/// LINEレスポンスメッセージエラー詳細クラス
	/// </summary>
	public class Detail
	{
		/// <summary> エラー詳細 </summary>
		public string Message { get; set; }
		/// <summary> エラー発生箇所 </summary>
		public string Property { get; set; }
	}
}
