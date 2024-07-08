/*
=========================================================================================================
  Module      : 特集エリアワーカーサービス(FeatureAreaWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Controllers;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureAreaType;
using w2.Cms.Manager.ViewModels.FeatureAreaType;
using w2.Domain.FeatureArea;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 特集エリアワーカーサービス
	/// </summary>
	public class FeatureAreaTypeWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(FeatureAreaTypeListParamModel pm)
		{
			// 検索条件から対象エリア取得
			var list = new FeatureAreaService().GetFeatureAreaTypeList();

			if (pm.DateChanged != Constants.DateSelectType.Unselected)
			{
				var conditionDate = DateTime.Now;
				switch (pm.DateChanged)
				{
					case Constants.DateSelectType.Day:
						conditionDate = DateTime.Now.AddDays(-1);
						break;

					case Constants.DateSelectType.Week:
						conditionDate = DateTime.Now.AddDays(-7);
						break;

					case Constants.DateSelectType.Month:
						conditionDate = DateTime.Now.AddMonths(-1);
						break;

					case Constants.DateSelectType.ThreeMonth:
						conditionDate = DateTime.Now.AddMonths(-3);
						break;

					case Constants.DateSelectType.AfterThreeMonth:
						conditionDate = DateTime.Now.AddMonths(-3);
						break;
				}

				list = (Constants.DateSelectType.AfterThreeMonth == pm.DateChanged)
					? list.Where(data => conditionDate >= data.DateChanged).ToArray()
					: list.Where(data => conditionDate <= data.DateChanged).ToArray();
			}
			var resultList = list.Select(data => new FeatureAreaTypeListSearchResultViewModel(data.DataSource)).ToArray();

			if (string.IsNullOrEmpty(pm.FreeWord) == false)
			{
				resultList = resultList.Where(
					data => (data.AreaTypeId.Contains(pm.FreeWord)
						|| (data.AreaTypeName.Contains(pm.FreeWord))
						|| (data.InternalMemo.Contains(pm.FreeWord))))
					.ToArray();
			}

			return new ListViewModel
			{
				ParamModel = pm,
				List = resultList,
			};
		}

		/// <summary>
		/// 詳細ビューモデル作成
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <returns>ビューモデル</returns>
		public DetailViewModel CreateDetailVm(string areaTypeId)
		{
			var featureAreaType = new FeatureAreaService().GetFeatureAreaType(areaTypeId);
			var model = new DetailViewModel(featureAreaType);
			return model;
		}

		/// <summary>
		/// 詳細画面登録
		/// </summary>
		/// <param name="input">特集エリアタイプ</param>
		public void InsertFeatureAreaType(FeatureAreaTypeInput input)
		{
			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			new FeatureAreaService().InsertFeatureAreaType(model);
		}

		/// <summary>
		/// 詳細画面更新
		/// </summary>
		/// <param name="input">特集エリアタイプ</param>
		public void UpdateFeatureAreaType(FeatureAreaTypeInput input)
		{
			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			new FeatureAreaService().UpdateFeatureAreaType(model);
		}

		/// <summary>
		/// 詳細画面削除
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		public string DeleteFeatureAreaType(string areaTypeId)
		{
			var service = new FeatureAreaService();
			if (service.GetFeatureAreaTypeCount(areaTypeId) > 0)
			{
				var errorMessage = WebMessages.FeatureAreaTypeNotDeletableError;
				return errorMessage;
			}
			service.DeleteFeatureAreaType(areaTypeId);
			return null;
		}

		/// <summary>
		/// 詳細画面削除
		/// </summary>
		/// <param name="input">特集エリアタイプ</param>
		/// <param name="devieType">デバイスタイプ</param>
		/// <returns>Json</returns>
		public string Preview(FeatureAreaTypeInput input, DesignCommon.DeviceType devieType)
		{
			// プレビュー時、対応するデータは存在しないのでダミーを入れる
			var previewInput = FeatureAreaInput.CreateDummy(input.ActionType);

			// プレビュー画面生成 URL返却
			var url = new FeatureAreaController().PreviewUrl(previewInput, devieType, false, input);
			return url;
		}

		/// <summary>
		/// サムネイルをアクションタイプ名で取得
		/// </summary>
		/// <param name="actionType">アクションタイプ</param>
		/// <returns>サムネイル</returns>
		public static string GetThumbnailByActionType(string actionType)
		{
			string fileName;
			switch (actionType)
			{
				case Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_VERTICAL:
					fileName = Constants.FEATUREAREATYPEIMAGE_VERTICAL;
					break;

				case Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SIDE:
					fileName = Constants.FEATUREAREATYPEIMAGE_SIDE;
					break;

				case Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SLIDER:
					fileName = Constants.FEATUREAREATYPEIMAGE_SLIDER;
					break;

				case Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_RANDOM:
					fileName = Constants.FEATUREAREATYPEIMAGE_RANDOM;
					break;

				case Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_OTHER:
					fileName = Constants.FEATUREAREATYPEIMAGE_OTHER;
					break;

				default:
					fileName = Constants.FEATUREAREATYPEIMAGE_OTHER;
					break;
			}

			fileName = Constants.PATH_FEATUREAREA_ICON_IMAGE + fileName;
			return fileName;
		}
	}
}