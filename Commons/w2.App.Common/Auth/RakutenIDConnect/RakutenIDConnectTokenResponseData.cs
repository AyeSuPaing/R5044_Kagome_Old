/*
=========================================================================================================
  Module      : 楽天IDConnectトークンレスポンスデータクラス(RakutenIDConnectTokenResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using w2.App.Common.Order.Payment;
using w2.Common.Helper;
using w2.Common.Util;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnectトークンレスポンスデータクラス
	/// </summary>
	[Serializable]
	public class RakutenIDConnectTokenResponseData : RakutenIDConnectBaseResponseData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal RakutenIDConnectTokenResponseData(string responseString)
			: base(responseString)
		{
			// プロパティセット
			SetProperty();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// プロパティセット
		/// </summary>
		private void SetProperty()
		{
			this.OpenId = "";

			try
			{
				// id_tokenのペイロード部取得
				var payload = this.IdToken.Split('.')[1];

				// Base64Urlデコードを行いユーザー識別子セット
				var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
				var payloadData = SerializeHelper.DeserializeJson<Dictionary<string, string>>(payloadJson);
				this.OpenId = payloadData["openid_id"];
				this.Nonce = payloadData["nonce"];
			}
			catch (Exception ex)
			{
				// エラーログ出力
				RakutenIDConnectLogger.WriteErrorLog("token_decode_error", ex.ToString(), PaymentFileLogger.PaymentProcessingType.Unknown);
			}
		}

		/// <summary>
		///  Base64Urlデコード
		/// </summary>
		/// <param name="payload">ペイロード</param>
		/// <returns>バイト配列</returns>
		private byte[] Base64UrlDecode(string payload)
		{
			var output = payload;
			output = output.Replace('-', '+'); // 62nd char of encoding
			output = output.Replace('_', '/'); // 63rd char of encoding
			switch (output.Length % 4) // Pad with trailing '='s
			{
				case 0: break; // No pad chars in this case
				case 2: output += "=="; break; // Two pad chars
				case 3: output += "="; break; // One pad char
				default: throw new Exception("Illegal base64url string!");
			}
			var converted = Convert.FromBase64String(output);
			return converted;
		}
		#endregion

		#region プロパティ
		/// <summary>発行されたAccess Token</summary>
		public string AccessToken
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("access_token")); }
		}
		/// <summary>トークン種別（固定値）</summary>
		public string TokenType
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("token_type")); }
		}
		/// <summary>Access Tokenの有効期限（秒）</summary>
		public string ExpiresIn
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("expires_in")); }
		}
		/// <summary>JWTフォーマットでエンコードされたユーザー情報を含むトークン</summary>
		public string IdToken
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("id_token")); }
		}
		/// <summary>ユーザー識別子（ユーザーの特定に利用）</summary>
		public string OpenId { get; private set; }
		/// <summary>ノンス（Authorization Endpointで渡されたnonce値）</summary>
		public string Nonce { get; private set; }
		#endregion
	}
}