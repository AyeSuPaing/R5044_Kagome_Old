/*
=========================================================================================================
  Module      : Zcom決済レスポンスデータ (ZcomDirectResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Direct
{
	/// <summary>
	/// Zcom決済レスポンスデータ
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "GlobalPayment_result", IsNullable = false, Namespace = "")]
	public class ZcomDirectResponse : BaseZcomResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZcomDirectResponse() : base() { }

		/// <summary>結果</summary>
		[XmlElement("result")]
		public Result[] Results { get; set; }

		[Serializable]
		public class Result
		{
			/// <summary>決済結果</summary>
			[XmlAttribute("result")]
			public string result { get; set; }

			/// <summary>トランザクションコード</summary>
			[XmlAttribute("trans_code")]
			public string TransCode { get; set; }

			/// <summary>エラーコード</summary>
			[XmlAttribute("err_code")]
			public string ErrCode { get; set; }

			/// <summary>エラーメッセージ</summary>
			[XmlAttribute("err_detail")]
			public string ErrDetail { get; set; }

			/// <summary>接続先URL</summary>
			[XmlAttribute("access_url")]
			public string AccessUrl { get; set; }

			/// <summary>決済コード</summary>
			[XmlAttribute("payment_code")]
			public string PaymentCode { get; set; }

			/// <summary>トランザクションコードハッシュ</summary>
			[XmlAttribute("trans_code_hash")]
			public string TransCodeHash { get; set; }

			/// <summary>モード</summary>
			[XmlAttribute("mode")]
			public string Mode { get; set; }

			/// <summary>取引ステータス</summary>
			[XmlAttribute("status")]
			public string Status { get; set; }
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
		/// トランザクションコード取得
		/// </summary>
		/// <returns>トランザクションコード</returns>
		public string GetTransCodeValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.TransCode) == false)
				{
					return res.TransCode;
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
		/// 接続先URL取得
		/// </summary>
		/// <returns>接続先URL</returns>
		public string GetAccessUrlValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.AccessUrl) == false)
				{
					return res.AccessUrl;
				}
			}
			return "";
		}

		/// <summary>
		/// 決済コード取得
		/// </summary>
		/// <returns>決済コード</returns>
		public string GetPaymentCodeValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.PaymentCode) == false)
				{
					return res.PaymentCode;
				}
			}
			return "";
		}

		/// <summary>
		/// トランザクションハッシュコード取得
		/// </summary>
		/// <returns>トランザクションハッシュコード</returns>
		public string GetTransCodeHashValue()
		{
			if (this.Results == null)
			{
				return "";
			}

			foreach (var res in this.Results)
			{
				if (string.IsNullOrEmpty(res.TransCodeHash) == false)
				{
					return res.TransCodeHash;
				}
			}
			return "";
		}

		/// <summary>
		/// Get mode value
		/// </summary>
		/// <returns>Mode value</returns>
		public string GetModeValue()
		{
			var item = this.Results.FirstOrDefault(result => (string.IsNullOrEmpty(result.Mode) == false));
			var mode = (item != null)
				? item.Mode
				: string.Empty;
			return mode;
		}
	}
}
