/*
=========================================================================================================
  Module      : Zcom実売上レスポンスデータ (ZcomSalesResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Sales
{
	/// <summary>
	/// Zcom実売上レスポンスデータ
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "GlobalPayment_result", IsNullable = false, Namespace = "")]
	public class ZcomSalesResponse : BaseZcomResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZcomSalesResponse() : base() { }

		/// <summary>結果</summary>
		[XmlElement("result")]
		public Result[] Results { get; set; }

		/// <summary>
		/// 結果データ
		/// </summary>
		[Serializable]
		public class Result
		{
			/// <summary>結果</summary>
			[XmlAttribute("result")]
			public string result { get; set; }
			/// <summary>エラーコード</summary>
			[XmlAttribute("err_code")]
			public string ErrCode { get; set; }
			/// <summary>エラーメッセージ</summary>
			[XmlAttribute("err_detail")]
			public string ErrDetail { get; set; }
		}

		/// <summary>
		/// 成功しているかどうか
		/// </summary>
		/// <returns>
		/// TRUE：成功（結果コードがOK）
		/// FALSE：失敗（結果コードがOK以外）
		/// </returns>
		public bool IsSuccessResult()
		{
			return (GetResultValue() == ZcomConst.RESULT_CODE_OK);
		}

		/// <summary>
		/// 結果取得
		/// </summary>
		/// <returns>結果</returns>
		public string GetResultValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.result) == false)
				{
					return res.result;
				}
			}
			return "";
		}

		/// <summary>
		/// エラーコード取得
		/// </summary>
		/// <returns>エラーコード</returns>
		public string GetErrorCodeValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.ErrCode) == false)
				{
					return res.ErrCode;
				}
			}
			return "";
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string GetErrorDetailValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.ErrDetail) == false)
				{
					return HttpUtility.UrlDecode(res.ErrDetail);
				}
			}
			return "";
		}
	}
}
