/*
=========================================================================================================
  Module      : ゼウスAPI基底クラス(ZeusApiBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using w2.App.Common.Order.Payment.Zeus.Helper;
using w2.App.Common.Web;
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Order.Payment.Zeus
{
	/// <summary>
	/// ゼウスAPI基底クラス
	/// </summary>
	[Serializable]
	public abstract class ZeusApiBase
	{
		/// <summary>通過区分：円</summary>
		protected const string KBN_SEND_YEN = "mall";
		/// <summary>通過区分：ドル</summary>
		protected const string KBN_SEND_DOLLAR = "";
		/// <summary>レスポンス時にオーダ番号を取得するか：する</summary>
		protected const string KBN_PRINTORD_YES = "yes";
		/// <summary>CGIコール停止：する</summary>
		protected const string KBN_PUBSEC_YES = "yes";
		/// <summary>CGIコール停止：しない</summary>
		protected const string KBN_PUBSEC_NON = "non";
		/// <summary>レスポンス時にエラーコードを取得するか</summary>
		protected const string KBN_RPERRCODE_YED = "yes";

		/// <summary>Default encoding</summary>
		protected Encoding _defaultEncoding = Encoding.GetEncoding("Shift_JIS");

		/// <summary>期間種別</summary>
		protected enum PeriodType
		{
			/// <summary>非会員</summary>
			[EnumTextName("01")]
			NotUser,
			/// <summary>この取引中に作成</summary>
			[EnumTextName("02")]
			Now,
			/// <summary> 30日未満</summary>
			[EnumTextName("03")]
			OneMonthLess,
			/// <summary>30～60日</summary>
			[EnumTextName("04")]
			TwoMonthsLess,
			/// <summary>60日を超える期間</summary>
			[EnumTextName("05")]
			TwoMonthsMore
		}

		/// <summary>出荷方法種別</summary>
		protected enum ShippingIndicatorType
		{
			/// <summary>ユーザーの請求先住所に出荷する</summary>
			[EnumTextName("01")]
			UserAddress,
			/// <summary>アドレス帳に登録済みの他の住所に出荷する</summary>
			[EnumTextName("02")]
			InAddressBook,
			/// <summary>ユーザーの請求先住所と異なる住所に出荷する</summary>
			[EnumTextName("03")]
			NotInAddressBook,
			/// <summary>店舗へ出荷</summary>
			[EnumTextName("04")]
			ToStore,
			/// <summary>デジタル商品</summary>
			[EnumTextName("05")]
			DigitalProduct,
			/// <summary>出荷されない旅行およびイベントのチケット</summary>
			[EnumTextName("06")]
			EventTicket,
			/// <summary>その他</summary>
			[EnumTextName("07")]
			Other
		}

		/// <summary>ISO 3166-1の3桁の国コード種別</summary>
		protected enum CountryIsoCode
		{
			/// <summary>日本のISO 3166-1の3桁の国コード</summary>
			[EnumTextName("392")]
			Japan,
			/// <summary>台湾のISO 3166-1の3桁の国コード</summary>
			[EnumTextName("158")]
			Taiwan,
			/// <summary>米国のISO 3166-1の3桁の国コード</summary>
			[EnumTextName("840")]
			America,
			/// <summary>その他</summary>
			[EnumTextName("")]
			Other
		}

		/// <summary>ゼウスのフラグ種別</summary>
		protected enum ZeusFlag
		{
			/// <summary>フラグON</summary>
			[EnumTextName("1")]
			On,
			/// <summary>フラグOFF</summary>
			[EnumTextName("0")]
			Off
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="isCvsPayment">Is CVS payment</param>
		protected ZeusApiBase(string url, bool isCvsPayment = false)
		{
			this.ServerUrl = url;
			this.ClientIP = isCvsPayment
				? Constants.PAYMENT_CVS_ZUES_CLIENT_IP
				: SessionManager.UsePaymentTabletZeus
					? Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP_OFFLINE
					: Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP;
		}

		/// <summary>
		/// GET実行
		/// </summary>
		/// <param name="url">URL</param>
		/// <returns>レスポンス</returns>
		protected string GetResponse(string url)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			var responseText = GetResponseString(webRequest.GetResponse());
			return responseText;
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="param">リクエスト文字列</param>
		/// <returns>戻り文字列</returns>
		protected string PostHttpRequest(Dictionary<string, string> param)
		{
			var paramString = string.Join(
				"&",
				param.Where(kvp => kvp.Value != null)
					.Select(kvp =>
						string.Format(
							"{0}={1}",
							kvp.Key,
							HttpUtility.UrlEncode(kvp.Value, _defaultEncoding))));
			var postData = _defaultEncoding.GetBytes(paramString);

			var webRequest = (HttpWebRequest)WebRequest.Create(this.ServerUrl);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = @"application/x-www-form-urlencoded";
			webRequest.ContentLength = postData.Length;

			// レスポンス取得
			string responseText = null;
			try
			{
				// 送信データの書き込み
				var postStream = webRequest.GetRequestStream();
				// 送信するデータを書き込む
				postStream.Write(postData, 0, postData.Length);
				postStream.Close();

				responseText = GetResponseString(webRequest.GetResponse());
				return responseText;
			}
			catch (WebException ex)
			{
				responseText = GetResponseString(ex.Response);
				return responseText;
			}
		}

		/// <summary>
		/// Get response string
		/// </summary>
		/// <param name="response">The web response</param>
		/// <returns>A response as string</returns>
		private string GetResponseString(WebResponse response)
		{
			using (var responseStream = response.GetResponseStream())
			using (var sr = new StreamReader(responseStream, _defaultEncoding))
			{
				return sr.ReadToEnd();
			}
		}

		/// <summary>
		/// 文字の切り詰め
		/// </summary>
		/// <param name="source">変換元</param>
		/// <param name="size">文字数</param>
		/// <returns>返還後</returns>
		protected string ReduceString(string source, int size)
		{
			return (source.Length > size) ? source.Substring(0, size) : source;
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="kbn">区分（決済の種類）</param>
		/// <param name="result">結果（なければnull）</param>
		/// <param name="infos">情報</param>
		public void WriteLog(PaymentFileLogger.PaymentProcessingType kbn, bool? result, params KeyValuePair<string, string>[] infos)
		{
			PaymentZeusLogger.WriteLog(GetType().Name, kbn, result, infos);
		}

		/// <summary>サーバURL</summary>
		public string ServerUrl { get; private set; }
		/// <summary>加盟店IP</summary>
		public string ClientIP { get; private set; }
	}
}
