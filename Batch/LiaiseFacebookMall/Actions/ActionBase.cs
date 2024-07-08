/*
=========================================================================================================
  Module      : アクション基底クラス(ActionBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Facebook;
using w2.Domain.MallCooperationSetting;

namespace w2.Commerce.Batch.LiaiseFacebookMall.Actions
{
	/// <summary>
	/// アクション基底クラス
	/// </summary>
	public abstract class ActionBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="facebookCatalogApiFacade">Facebook Catalog Api Facade</param>
		/// <param name="mallSetting">Mall setting</param>
		public ActionBase(
			FacebookCatalogApiFacade facebookCatalogApiFacade,
			MallCooperationSettingModel mallSetting)
		{
			this.FacebookCatalogApiFacade = facebookCatalogApiFacade;
			this.MallSetting = mallSetting;
			this.Result = new ActionResult();
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

		/// <summary>Facebook catalog api facade</summary>
		protected FacebookCatalogApiFacade FacebookCatalogApiFacade { get; set; }
		/// <summary>Mall setting</summary>
		protected MallCooperationSettingModel MallSetting { get; set; }
		/// <summary>実行結果</summary>
		protected ActionResult Result { get; set; }
	}
}