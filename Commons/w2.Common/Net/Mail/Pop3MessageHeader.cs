/*
=========================================================================================================
  Module      : POP3メッセージヘッダ(Pop3MessageHeader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace w2.Common.Net.Mail
{
	/// <summary>
	/// POP3メッセージヘッダ
	/// </summary>
	public class Pop3MessageHeader
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public Pop3MessageHeader(string source)
		{
			this.Values = new NameValueCollection();

			// ソースをプロパティにセット
			this.Source = source;

			// パース
			Parse(source);

			// デコード
			Decode();

			this.ContentType = GetContentsType();
			this.ContentDisposition = GetContentDisposition();
		}
		#endregion

		#region -Parse パース
		/// <summary>
		/// パース
		/// </summary>
		/// <param name="source">ソース</param>
		private void Parse(string source)
		{
			// 値をセット
			var currentHeaderName = "";
			using (var reader = new StringReader(source))
			{
				while (reader.Peek() >= 0)
				{
					string line = reader.ReadLine();

					// ボディ判定
					if (line == "") return;

					// 先頭がスペースやタブなら、前の行とつなげる
					if (line.StartsWith(" ") || line.StartsWith("\t"))
					{
						this.Values[currentHeaderName] += " " + line.Trim();
					}
					// 不正メール対策（「From: "choda\r\n" <jlprdxfdyignsihmra@softbank.ne.jp>」というようなデータに対応）
					// 「:」が含まれないヘッダ行は場合、前の行とつなげる
					else if (line.Contains(":") == false)
					{
						this.Values[currentHeaderName] += line;
					}
					else
					{
						// 「:」区切りでヘッダの値を取得
						string[] val = line.Split(new Char[] { ':' }, 2);
						currentHeaderName = val[0].Trim();
						this.Values.Add(currentHeaderName, val[1].Trim());
					}
				}
			}
		}
		#endregion

		#region -Decode デコード
		/// <summary>
		/// デコード
		/// </summary>
		private void Decode()
		{
			foreach (string key in this.Values.AllKeys)
			{
				var values = this.Values.GetValues(key);
				this.Values.Remove(key);

				foreach (var value in values)
				{
					// ※複数行にわたるメールヘッダのうち、デコードされた文字列間の空白は無視する（RFC 2047）
					var decoded = Regex.Replace(value, @"=\?([^\?]+)\?([qQbB])\?([^\?]+)\?=\s*", DecodeHeaderValue);
					this.Values.Add(key, decoded);
				}
			}
		}
		#endregion

		#region -DecodeHeaderValue ヘッダ値デコード
		/// <summary>
		/// ヘッダ値デコード
		/// </summary>
		/// <param name="match">マッチング情報</param>
		/// <returns>デコード後文字列</returns>
		private string DecodeHeaderValue(Match match)
		{
			// "ISO-2022-JP"等の文字コード部を取得
			var charSet = match.Groups[1].ToString();
			var enc = GetEncoding(charSet);
			var src = match.Groups[3].ToString();

			// Base64/Quoted-Printable等変換
			if (match.Groups[2].ToString().ToLower() == "q")
			{
				return EncodeHelper.DecodeQEncode(enc, src);
			}
			else
			{
				return EncodeHelper.DecodeBase64(enc, src);
			}
		}
		#endregion

		#region -GetEncoding  未対応/非標準も含めてcharset文字列を解釈する
		/// <summary>
		/// 未対応/非標準も含めてcharset文字列を解釈する
		/// </summary>
		/// <param name="charset">charset文字列</param>
		/// <returns>エンコーディング</returns>
		public static Encoding GetEncoding(string charset)
		{
			try
			{
				return Encoding.GetEncoding(charset);
			}
			catch (Exception)
			{
				// .NET未対応
				if (string.Equals(charset, "Windows-31J", StringComparison.OrdinalIgnoreCase))
				{
					return Encoding.GetEncoding(932);
				}

				// 非標準 CPxxx
				var matchCodePage = Regex.Match(charset ?? "", @"^CP(\d+)$", RegexOptions.IgnoreCase);
				if (matchCodePage.Success)
				{
					return Encoding.GetEncoding(int.Parse(matchCodePage.Groups[1].Value));
				}

				return null;
			}
		}
		#endregion

		#region -GetContentsType コンテンツタイプを取得
		/// <summary>
		/// コンテンツタイプを取得
		/// </summary>
		/// <returns>ContentTypeクラス</returns>
		private ContentType GetContentsType()
		{
			var contentTypeString = this.Values["content-type"];
			if (contentTypeString == null)
			{
				// this.BodyDecodedに文字を格納したいためtext/plain指定
				return new ContentType("text/plain");
			}

			// nameに日本語が入っていると ContentType コンストラクタでエラーとなるため nameだけ分ける
			// 例：Content-Type: application/octet-stream; name="=?ISO-2022-JP?B?GyRCJF4kJCRhGyhCLnR4dA==?="
			// 例：Content-Type: application/octet-stream;name="=?ISO-2022-JP?B?GyRCJF4kJCRhGyhCLnR4dA==?="
			string nameLower = null;
			foreach (var key in this.Values)
			{
				foreach (var tmp in this.Values[(string)key].Split(';'))
				{
					if (tmp.Trim().ToLower().StartsWith("name=")
						|| tmp.Trim().ToLower().StartsWith("filename="))
					{
						nameLower = tmp.Trim().ToLower();
						contentTypeString = contentTypeString.Replace(tmp, "");
						// ;;があるとnew ContentType(contentTypeString)で異常が投げられるため、;に変更
						contentTypeString = contentTypeString.Replace(";;", ";");
					}
				}
			}

			// ContentType の boundary は「"」で括られていないとContentType コンストラクタでエラーとなるため 「"」で括る
			if (contentTypeString.ToLower().Contains(" boundary=")
				&& (contentTypeString.ToLower().Contains(" boundary=\"") == false))
			{
				var boundary = " boundary=";
				var boundaryEndPos = contentTypeString.IndexOf(boundary, StringComparison.CurrentCultureIgnoreCase) + boundary.Length;
				contentTypeString = contentTypeString.Substring(0, boundaryEndPos) + "\"" + contentTypeString.Substring(boundaryEndPos) + "\"";
			}

			// ContentTypeクラス生成
			ContentType contentType = null;
			try
			{
				contentType = new ContentType(contentTypeString);
			}
			catch (Exception)
			{
				// 不正メール対策（「text/html charset="Windows-1252"」というデータが来たときの対応）
				try
				{
					// セミコロンが抜けている場合は付与
					var splitted = contentTypeString.Split(' ');
					if (splitted[0].EndsWith(";") == false)
					{
						splitted[0] = splitted[0] + ";";
						contentType = new ContentType(string.Join(" ", splitted));
					}
				}
				catch (Exception) { }

				// 変換できていなかったらデフォルトの値をセット
				if (contentType == null) contentType = new ContentType("text/plain; charset=\"ISO-2022-JP\"");
			}
			if (nameLower != null)
			{
				contentType.Name = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(nameLower.Replace("\"", "").Replace("filename=", "").Replace("name=", "")));
			}

			return contentType;
		}
		#endregion

		#region -GetContentDisposition ContentDispositionを取得
		/// <summary>
		/// ContentDispositionを取得
		/// </summary>
		/// <returns>ContentDispositionクラス</returns>
		private ContentDisposition GetContentDisposition()
		{
			var contentDispositionString = this.Values["content-disposition"];

			if (contentDispositionString == null)
			{
				return new ContentDisposition();
			}

			string fileName = null;
			foreach (var tmp in contentDispositionString.Split(';'))
			{
				if (tmp.Trim().ToLower().StartsWith("filename="))
				{
					fileName = tmp;
					contentDispositionString = contentDispositionString.Replace(tmp + ";", "");
					contentDispositionString = contentDispositionString.Replace(tmp, "");
				}
			}

			var contentDispition = new ContentDisposition(contentDispositionString);

			if (fileName != null)
			{
				contentDispition.FileName = fileName.Replace("\"", "").Replace("filename=", "");
			}

			return contentDispition;
		}
		#endregion

		#region +GetBodyReadEncoding ボディ読み込みエンコーディング取得
		/// <summary>
		/// ボディ読み込みエンコーディング取得
		/// </summary>
		/// <returns>ボディ読み込みエンコード（判定できない場合はnull）</returns>
		/// <remarks>
		/// SPAMではTransferEncodingなしでShift_JISなどが指定されていたりするので
		/// 読み込みエンコードをヘッダから判定して読み込む
		/// </remarks>
		public Encoding GetBodyReadEncoding()
		{
			// Transfer-Encodingで符号化されている場合はASCII
			string transferEncoding = this.Values["content-transfer-encoding"] ?? "";
			switch (transferEncoding.Trim().ToLower())
			{
				case "quoted-printable":
				case "base64":
					return Encoding.ASCII;
			}

			// CharSet変換できればそれを返す
			try
			{
				return Encoding.GetEncoding(this.ContentType.CharSet);
			}
			catch (ArgumentException) { }

			// 判定できなかった場合はnull
			return null;
		}
		#endregion

		#region プロパティ
		/// <summary>ソース</summary>
		public string Source { get; private set; }
		/// <summary>値</summary>
		public NameValueCollection Values { get; private set; }
		/// <summary>コンテンツタイプ</summary>
		public ContentType ContentType { get; private set; }
		/// <summary>ContentDisposition</summary>
		public ContentDisposition ContentDisposition { get; private set; }
		#endregion
	}
}
