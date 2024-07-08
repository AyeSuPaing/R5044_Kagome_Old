/*
=========================================================================================================
  Module      : シルバーエッグレポート リクエストデータ(SilvereggAigentReportRequestData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Web;

/// <summary>
/// SilvereggAigentReportRequestData の概要の説明です
/// </summary>
public class SilvereggAigentReportRequestData : IHttpApiRequestData
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public SilvereggAigentReportRequestData()
	{
	}

	/// <summary>
	/// POSTデータ生成
	/// </summary>
	/// <returns>POSTデータ</returns>
	public string CreatePostString()
	{
		var parameters = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("m", this.Merchant),
			new KeyValuePair<string, string>("spec", this.Spec),
			new KeyValuePair<string, string>("from", this.From),
			new KeyValuePair<string, string>("to", this.To),
			new KeyValuePair<string, string>("date", this.Date)
		};
		var postString = string.Join("&",
			parameters.Select(param =>
				string.Format("{0}={1}", param.Key, param.Value)));
		return postString;
	}

	/// <summary>マーチャントID</summary>
	public string Merchant { get; set; }
	/// <summary>レコメンドID</summary>
	public string Spec { get; set; }
	/// <summary>集計開始日・月（YYYYMMDD / YYYYMM）</summary>
	public string From { get; set; }
	/// <summary>集計終了日・月（YYYYMMDD / YYYYMM）</summary>
	public string To { get; set; }
	/// <summary>集計日・月（YYYYMMDD / YYYYMM）</summary>
	public string Date { get; set; }
}