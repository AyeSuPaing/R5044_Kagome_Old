/*
=========================================================================================================
  Module      : 楽天IDConnectトークンAPIクラス(RakutenIDConnectTokenApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnectトークンAPIクラス
	/// </summary>
	public class RakutenIDConnectTokenApi : RakutenIDConnectBaseApi
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RakutenIDConnectTokenApi()
			: base(ApiType.Token)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="code">Authorization Code</param>
		/// <returns>トークンレスポンスデータ</returns>
		public RakutenIDConnectTokenResponseData Exec(string code)
		{
			// トークン用URL作成インスタンス取得
			var urlCreator = RakutenIDConnectUrlCreator.CreateUrlCreatorForToken(code);

			// リクエスト＆レスポンスデータ取得
			var responseString = PostHttpRequest(urlCreator);

			// レスポンスデータ返す
			return new RakutenIDConnectTokenResponseData(responseString);
		}
		#endregion
	}
}