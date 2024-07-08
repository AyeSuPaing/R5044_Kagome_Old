/*
=========================================================================================================
  Module      : Zcomキャンセルレスポンスデータ (ZcomCancelResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Cancel
{
	/// <summary>
	/// Zcomキャンセルレスポンスデータ
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "GlobalPayment_result", IsNullable = false, Namespace = "")]
	public class ZcomCancelResponse : BaseZcomResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZcomCancelResponse() : base() { }

		/// <summary>結果</summary>
		[XmlElement("result")]
		public Result[] Results { get; set; }

		/// <summary>
		/// 結果データ
		/// </summary>
		[Serializable]
		public class Result
		{
			/// <summary>決済結果</summary>
			[XmlAttribute("result")]
			public string result { get; set; }
			/// <summary>エラーコード</summary>
			[XmlAttribute("err_code")]
			public string ErrCode { get; set; }
			/// <summary>エラーメッセージ</summary>
			[XmlAttribute("err_detail")]
			public string ErrDetail { get; set; }
			/// <summary>売上金</summary>
			[XmlAttribute("sales_amount")]
			public string SalesAmount { get; set; }
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
		/// 決済結果取得
		/// </summary>
		/// <returns>決済結果</returns>
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

		/// <summary>
		/// 売上金取得
		/// </summary>
		/// <returns>売上金</returns>
		public string GetSalesAmountValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.SalesAmount) == false)
				{
					return res.SalesAmount;
				}
			}
			return "";
		}
	}
}
