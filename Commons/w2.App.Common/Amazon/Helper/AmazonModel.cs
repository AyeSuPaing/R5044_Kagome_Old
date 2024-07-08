/*
=========================================================================================================
  Module      : Amazon Amazonモデル(AmazonModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using Amazon.Pay.API.WebStore.Buyer;
using Amazon.Pay.API.WebStore.Types;
using w2.App.Common.Amazon.Responses;

namespace w2.App.Common.Amazon.Helper
{
	/// <summary>
	/// Amazonモデル
	/// </summary>
	[Serializable]
	public class AmazonModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AmazonModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="token">トークン</param>
		/// <param name="res">レスポンス</param>
		public AmazonModel(string token, GetUserInfoResponse res)
		{
			this.Token = token;
			this.UserId = res.UserId;
			this.Name = res.Name;
			this.Email = res.Email;
			this.TransitionCallbackSourcePath = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="token">トークン</param>
		/// <param name="buyer">アマゾンBuyer</param>
		public AmazonModel(string token, BuyerResponse buyer)
		{
			this.Token = token;
			this.UserId = buyer.BuyerId;
			this.Name = buyer.Name;
			this.Email = buyer.Email;
			this.TransitionCallbackSourcePath = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="token">トークン</param>
		/// <param name="buyer">アマゾンBuyer</param>
		public AmazonModel(string token, Buyer buyer)
		{
			this.Token = token;
			this.UserId = buyer.BuyerId;
			this.Name = buyer.Name;
			this.Email = buyer.Email;
			this.TransitionCallbackSourcePath = string.Empty;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 姓
		/// </summary>
		/// <returns>姓</returns>
		public string GetName1()
		{
			if (this.Name.Contains(" ") || this.Name.Contains("　"))
			{
				this.Name = this.Name.Replace('　', ' ');
				return this.Name.Split(' ')[0];
			}
			else
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// 名
		/// </summary>
		/// <returns>名</returns>
		public string GetName2()
		{
			if (this.Name.Contains(" ") || this.Name.Contains("　"))
			{
				this.Name = this.Name.Replace('　', ' ');
				return this.Name.Split(' ')[1];
			}
			else
			{
				return string.Empty;
			}
		}
		#endregion

		#region プロパティ
		/// <summary>トークン</summary>
		public string Token { get; set; }
		/// <summary>ユーザーID</summary>
		public string UserId { get; set; }
		/// <summary>氏名</summary>
		public string Name { get; set; }
		/// <summary>メールアドレス</summary>
		public string Email { get; set; }
		/// <summary>遷移元コールバックパス</summary>
		public string TransitionCallbackSourcePath { get; set; }
		#endregion
	}
}
