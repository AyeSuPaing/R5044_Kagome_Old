/*
=========================================================================================================
  Module      : メールヘッダ解析 (MailHeader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using w2.Common.Net.Mail;

namespace w2.Commerce.MallBatch.MailOrderGetter.MailAnalyze
{
	///**************************************************************************************
    /// <summary>
    /// メールヘッダ解析クラス
    /// </summary>
	///**************************************************************************************
    public class MailHeader
    {
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailHeader()
		{
			// プロパティ初期化
			this.HeaderParts = new Hashtable();
		}
        /// <summary>
        /// コンストラクタ
        /// </summary>
		/// <param name="strHeader">ヘッダ文字列</param>
		/// <remarks>ヘッダを構成する</remarks>
		public MailHeader(string strHeader) : this()
		{
			InitMailHeader(strHeader);
		}
        /// <summary>
        /// ヘッダを構成する
        /// </summary>
		/// <param name="strHeader">ヘッダ文字列</param>
		private void InitMailHeader(string strHeader)
        {
			string strTmpHeader = "\r\n" + strHeader;

            // ヘッダパートの正規表現
            Regex regex = new Regex("\\r\\n[^\\s]*:");
			MatchCollection matchCollection = regex.Matches(strTmpHeader);

            // ハッシュに分解して格納
            int iLastIndex = 0;
			this.HeaderParts = new Hashtable();
			for (int iLoop = 0; iLoop < matchCollection.Count; iLoop++)
            {
				int iStartIndex = (iLoop == matchCollection.Count - 1) ? strTmpHeader.Length : matchCollection[iLoop + 1].Index;
				string strSingleLow = strTmpHeader.Substring(iLastIndex, iStartIndex - iLastIndex);
				if (strSingleLow.Trim().Length == 0)
				{
					continue;
				}

                // key,valueに分解
				string strKey = strSingleLow.Substring(1, strSingleLow.IndexOf(':') - 1).Trim();
				string strValue = strSingleLow.Substring(strSingleLow.IndexOf(':') + 1);

                // 配列が無ければ作る
				if (this.HeaderParts[strKey.ToLower()] == null)
                {
					this.HeaderParts[strKey.ToLower()] = new List<string>();
                }

                // 投入
				string strDecoded = Regex.Replace(strValue, @"=\?([^\?]+)\?([qQbB])\?([^\?]+)\?=", new MatchEvaluator(HeaderReplacer));
				((List<string>)this.HeaderParts[strKey.ToLower()]).Add(strDecoded);

				if (iLoop < matchCollection.Count - 1)
				{
					iLastIndex = matchCollection[iLoop + 1].Index;
				}
            }

			// マルチパートメッセージ設定
			this.MultiPart = CheckMultiPart();
			this.MultiPartBoundary = GetMultiPartBoundary();
        }

		/// <summary>
		/// ヘッダ置換用メソッド
		/// </summary>
		/// <param name="match">マッチング情報</param>
		/// <returns>置換後ヘッダ</returns>
		private static string HeaderReplacer(Match match)
		{
			// "ISO-2022-JP"等の文字コード部を取得
			var charSet = match.Groups[1].ToString();
			var enc = Encoding.GetEncoding(charSet);
			var src = match.Groups[3].ToString();

			// Qエンコード（Quoted-Printableとは別物）／Bエンコード（Base64と同じ）変換
			if (match.Groups[2].ToString().ToLower() == "q")
			{
				return EncodeHelper.DecodeQEncode(enc, src);
			}
			else
			{
				return EncodeHelper.DecodeBase64(enc, src);
			}
		}

		/// <summary>
		/// マルチパートメッセージであればtrueを返却する
		/// </summary>
		/// <returns>マルチパートメッセージ有無</returns>
		private bool CheckMultiPart()
		{
			if (this.HeaderParts["content-type"] == null)
			{
				return false;
			}

			List<string> lHeaderParts = (List<string>)this.HeaderParts["content-type"];
			int iMultipartFind = ((string)lHeaderParts[0]).ToLower().IndexOf("multipart");
			return ((0 < iMultipartFind) && (iMultipartFind < ((string)lHeaderParts[0]).Length));
		}

		/// <summary>
		/// マルチパートメッセージの区切り文字列を返却する
		/// </summary>
		/// <returns>マルチパートメッセージ文字列</returns>
		public string GetMultiPartBoundary()
		{
			if (this.MultiPart)
			{
				string strBoundary = "boundary=\"";
				List<string> lHeaderParts = (List<string>)this.HeaderParts["content-type"];
				int iBoundaryHead = ((string)lHeaderParts[0]).ToLower().IndexOf(strBoundary) + strBoundary.Length;

				if ((0 < iBoundaryHead) && (iBoundaryHead < ((string)lHeaderParts[0]).Length))
				{
					int iBoundaryTail = ((string)lHeaderParts[0]).IndexOf("\"", iBoundaryHead);
					return "--" + ((string)lHeaderParts[0]).Substring(iBoundaryHead, iBoundaryTail - iBoundaryHead);
				}
				else
				{
					// マルチパートでないためnullを返却
					return null;
				}
			}
			else
			{
				// マルチパートでないためnullを返却
				return null;
			}
		}

		/// <summary>ヘッダ　※キーは小文字で格納される</summary>
		public Hashtable HeaderParts { get; private set; }

		/// <summary>マルチパートメッセージ有無</summary>
		public bool MultiPart { get; private set; }

		/// <summary>マルチパートメッセージの区切り文字列を返却します</summary>
		public string MultiPartBoundary { get; private set; }
	}
}
