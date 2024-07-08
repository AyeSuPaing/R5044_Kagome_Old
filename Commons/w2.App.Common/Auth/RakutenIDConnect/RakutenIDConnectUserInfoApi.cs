/*
=========================================================================================================
  Module      : 楽天IDConnectユーザー情報取得APIクラス(RakutenIDConnectUserInfoApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnectユーザー情報取得APIクラス
	/// </summary>
	public class RakutenIDConnectUserInfoApi : RakutenIDConnectBaseApi
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessToken">発行されたAccess Token</param>
		public RakutenIDConnectUserInfoApi(string accessToken)
			: base(ApiType.UserInfo)
		{
			this.AccessToken = accessToken;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 実行
		/// </summary>
		/// <returns>ユーザー情報取得レスポンスデータ</returns>
		public RakutenIDConnectUserInfoResponseData Exec()
		{
			// ユーザー取得用URL作成インスタンス作成
			var urlCreator = RakutenIDConnectUrlCreator.CreateUrlCreatorForUserInfo();

			// リクエスト＆レスポンスデータ取得
			var responseString = PostHttpRequest(urlCreator);

			// レスポンスデータ返す
			return new RakutenIDConnectUserInfoResponseData(responseString);
		}
		#endregion
	}
}