/*
=========================================================================================================
  Module      : アクション基底クラス(ActionBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using AmazonMarketPlaceWebService;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Amazon
{
	/// <summary>
	/// アクション基底クラス
	/// </summary>
	public abstract class ActionBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="amazonApi">AmazonAPIサービスクラス</param>
		/// <param name="mallId">モールID</param>
		public ActionBase(AmazonApiService amazonApi, string mallId)
		{
			this.AmazonApi = amazonApi;
			this.MallId = mallId;
			this.ActionResult = new ActionResult();
		}

		/// <summary>
		/// 開始時処理
		/// </summary>
		public abstract void OnStart();

		/// <summary>
		/// 実行
		/// </summary>
		public abstract void Exec();

		/// <summary>
		/// 終了時処理
		/// </summary>
		public abstract void OnComplete();

		/// <summary>
		/// 異常時処理
		/// </summary>
		public abstract void OnError();

		#region プロパティ
		/// <summary>AmazonAPIサービス</summary>
		protected AmazonApiService AmazonApi { get; set; }
		/// <summary>モールID</summary>
		protected string MallId { get; set; }
		/// <summary>実行結果</summary>
		protected ActionResult ActionResult { get; set; }
		#endregion
	}
}
