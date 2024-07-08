/*
=========================================================================================================
  Module      : Product Catalog Action(ProductCatalogAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Facebook;
using w2.App.Common.MallCooperation;
using w2.App.Common.Option;
using w2.Commerce.Batch.LiaiseFacebookMall.Settings;
using w2.Domain.MallCooperationSetting;

namespace w2.Commerce.Batch.LiaiseFacebookMall.Actions
{
	/// <summary>
	/// Product Catalog Action
	/// </summary>
	public class ProductCatalogAction : ActionBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="facebookCatalogApiFacade">Facebook Catalog Api Facade</param>
		/// <param name="mallSetting">Mall setting</param>
		public ProductCatalogAction(
			FacebookCatalogApiFacade facebookCatalogApiFacade,
			MallCooperationSettingModel mallSetting)
			: base(facebookCatalogApiFacade, mallSetting)
		{
		}

		/// <summary>
		/// 開始時処理
		/// </summary>
		public override void OnStart()
		{
			// モール監視ログ登録
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_FACEBOOK_MALL,
				this.MallSetting.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
				"Facebookショッピング自動連携を開始しました。");
		}

		/// <summary>
		/// 実行
		/// </summary>
		public override void Exec()
		{
			// Create requests
			var exhibitsFlgName = MallOptionUtility.GetExhibitsConfigField(this.MallSetting.MallExhibitsConfig);
			var requestFields = FacebookExternalSettingManager.GetInstance().GetApiRequests(exhibitsFlgName);
			var request = new FacebookCatalogRequestApi
			{
				AccessToken = this.MallSetting.FacebookAccessToken,
				Requests = new FacebookCatalogRequestFieldApi[0],
			};

			// Execute send request
			for (var index = 0; index < requestFields.Length; index = (index + Constants.FACEBOOK_CATALOG_API_MAX_REQUEST_COUNT))
			{
				var items = requestFields
					.Skip(index)
					.Take(Constants.FACEBOOK_CATALOG_API_MAX_REQUEST_COUNT);
				request.Requests = items.ToArray();
				var result = this.FacebookCatalogApiFacade.CallApiFacebook(request, this.MallSetting.FacebookCatalogId);
				if (result.IsSuccess)
				{
					this.Result.RequestSuccessCount += request.Requests.Length;
				}
				else
				{
					this.Result.RequestErrorCount += request.Requests.Length;
					this.Result.RequestErrorMessage += result.Response.Error.Message;
				}
			}
		}

		/// <summary>
		/// 終了時処理
		/// </summary>
		public override void OnComplete()
		{
			if(this.Result.RequestErrorCount > 0)
			{
				new MallWatchingLogManager().Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_FACEBOOK_MALL,
					this.MallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
					Constants.FACEBOOK_CATALOG_API_RESPONSE_ERROR_LOG_MESSAGE,
					this.Result.RequestErrorMessage);
			}
			else
			{
				// モール監視ログ登録
				new MallWatchingLogManager().Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_FACEBOOK_MALL,
					this.MallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					string.Format(
						"Facebookショッピング自動連携を完了しました。(成功件数：{0}件)",
						this.Result.RequestSuccessCount.ToString()));
			}
		}

		/// <summary>
		/// 異常時処理
		/// </summary>
		public override void OnError()
		{
			// モール監視ログ登録
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_FACEBOOK_MALL,
				this.MallSetting.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
				"Facebookショッピング自動連携に例外エラーが発生しました。システム管理者にお問い合わせください。");
		}
	}
}