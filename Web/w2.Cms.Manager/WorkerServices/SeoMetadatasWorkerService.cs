/*
=========================================================================================================
  Module      : SEOタグ設定ワーカーサービス(SeoMetadatasWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.SeoMetadatas;
using w2.Common.Util;
using w2.Domain.SeoMetadatas;
using Constants = w2.App.Common.Constants;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// SEOタグ設定ワーカーサービス
	/// </summary>
	public class SeoMetadatasWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 編集画面用ビューモデル作成
		/// </summary>
		/// <returns>ビュー</returns>
		public ModifyViewModel CreateViewModelForModify(string pageLayout)
		{
			var seoMetadatas = new SeoMetadatasService().GetAll();
			var registerViewModel = new ModifyViewModel
			{
				InputForDefault = new SeoMetadatasInput(
					seoMetadatas.FirstOrDefault(sm => (sm.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_DEFAULT_SETTING))),
				InputForProductList = new SeoMetadatasInput(
					seoMetadatas.FirstOrDefault(sm => (sm.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST))),
				InputForProductDetail = new SeoMetadatasInput(
					seoMetadatas.FirstOrDefault(sm => (sm.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_DETAIL))),
				InputForCoordinateTop = new SeoMetadatasInput(
					seoMetadatas.FirstOrDefault(sm => (sm.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_TOP))),
				InputForCoordinateList = new SeoMetadatasInput(
					seoMetadatas.FirstOrDefault(sm => (sm.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_LIST))),
				InputForCoordinateDetail = new SeoMetadatasInput(
					seoMetadatas.FirstOrDefault(sm => (sm.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_DETAIL))),
				PageLayout = pageLayout
			};

			return registerViewModel;
		}

		/// <summary>
		/// UPDATE実行（DBにデータがなければINSERT）
		/// </summary>
		/// <param name="inputForDefault">全体設定用インプット</param>
		/// <param name="inputForProductList">商品一覧用インプット</param>
		/// <param name="inputForProductDetail">商品詳細用インプット</param>
		/// <param name="inputForCoordinateTop">コーディネートトップインプット</param>
		/// <param name="inputForCoordinateList">コーディネート一覧インプット</param>
		/// <param name="inputForCoordinateDetail">コーディネート詳細インプット</param>
		/// <returns>エラーメッセージ</returns>
		public string Update(
			SeoMetadatasInput inputForDefault,
			SeoMetadatasInput inputForProductList,
			SeoMetadatasInput inputForProductDetail,
			SeoMetadatasInput inputForCoordinateTop,
			SeoMetadatasInput inputForCoordinateList,
			SeoMetadatasInput inputForCoordinateDetail)
		{
			inputForDefault.DataKbn = Constants.FLG_SEOMETADATAS_DATA_KBN_DEFAULT_SETTING;
			inputForProductList.DataKbn = Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST;
			inputForProductDetail.DataKbn = Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_DETAIL;
			inputForCoordinateTop.DataKbn = Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_TOP;
			inputForCoordinateList.DataKbn = Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_LIST;
			inputForCoordinateDetail.DataKbn = Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_DETAIL;

			var service = new SeoMetadatasService();
			var oldMetadatas = service.GetAll();

			// 入力チェック
			var inputs = new[]
			{
				inputForDefault,
				inputForProductList,
				inputForProductDetail,
				inputForCoordinateTop,
				inputForCoordinateList,
				inputForCoordinateDetail,
			};
			var errorMessages = inputs
				.Select(
					input =>
						input
							.Validate((oldMetadatas.Any(m => (m.DataKbn == input.DataKbn)) == false))
							.Replace(
								"@@ 1 @@",
								ValueText.GetValueText(
									Constants.TABLE_SEOMETADATAS,
									Constants.FIELD_SEOMETADATAS_DATA_KBN,
									input.DataKbn)))
				.ToArray();

			// メッセージあれば返却
			if (errorMessages.All(string.IsNullOrEmpty) == false)
			{
				return string.Join(Environment.NewLine, errorMessages).Trim();
			}

			// INSERT / UPDATE
			foreach (var input in inputs)
			{
				input.LastChanged = this.SessionWrapper.LoginOperatorName;
				if (oldMetadatas.Any(m => (m.DataKbn == input.DataKbn)))
				{
					service.Update(input.CreateModel());
				}
				else
				{
					service.Insert(input.CreateModel());
				}
			}

			// キャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.SeoMetadatas).CreateUpdateRefreshFile();

			return string.Empty;
		}
	}
}	