/*
=========================================================================================================
  Module      : 特集エリア詳細ビューモデル(DetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Menu;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Domain.FeatureArea;
using w2.Domain.FeatureArea.Helper;
using w2.Domain.PartsDesign;
using w2.Domain.TargetList;

namespace w2.Cms.Manager.ViewModels.FeatureArea
{
	/// <summary>
	/// 特集エリアタイプ詳細ビューモデル
	/// </summary>
	public class DetailViewModel : ViewModelBase
	{
		/// <summary>選択肢の最大値</summary>
		private const int MAX_RANGE = 9;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public DetailViewModel(FeatureAreaModel model)
		{
			this.SideMaxCounts = this.SliderCounts = this.SliderScrollCounts = this.SliderScrollIntervals =
				Enumerable.Range(1, MAX_RANGE).Select(data => new SelectListItem()
				{
					Text = data.ToString(),
					Value = data.ToString()
				}).ToArray();
			this.AreaTypes = new FeatureAreaService().GetFeatureAreaTypeList();
			this.SideTurns = ValueTextItemArray(Constants.FIELD_FEATUREAREA_SIDE_TURN);
			this.SliderScrollAutos = ValueTextItemArray(Constants.FIELD_FEATUREAREA_SLIDER_SCROLL_AUTO);
			this.SliderArrows = ValueTextItemArray(Constants.FIELD_FEATUREAREA_SLIDER_ARROW);
			this.SliderDots = ValueTextItemArray(Constants.FIELD_FEATUREAREA_SLIDER_DOT);

			this.UseTypeList = new[]{
				new SelectListItem
				{
					Text = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_CMS_COMMON,
						Constants.VALUE_TEXT_FIELD_USE_TYPE,
						Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP),
					Value = Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP,
					Selected = true
				},
				new SelectListItem
				{
					Text = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_CMS_COMMON,
						Constants.VALUE_TEXT_FIELD_USE_TYPE,
						Constants.FLG_PAGEDESIGN_USE_TYPE_PC),
					Value = Constants.FLG_PAGEDESIGN_USE_TYPE_PC,
				},
				new SelectListItem
				{
					Text = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_CMS_COMMON,
						Constants.VALUE_TEXT_FIELD_USE_TYPE,
						Constants.FLG_PAGEDESIGN_USE_TYPE_SP),
					Value = Constants.FLG_PAGEDESIGN_USE_TYPE_SP
				},
			};

			this.ImageModalViewModel = new ImageModalViewModel(ImageType.Area);

			var targetList = new TargetListService().GetAll(Constants.CONST_DEFAULT_SHOP_ID);
			if (model == null)
			{
				this.Input = new FeatureAreaInput
				{
					BannerInput = new[] { new FeatureAreaBannerInput(new FeatureAreaBannerModel(), targetList), },
					AreaTypeId = Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_VERTICAL,
					ActionType = Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_VERTICAL,
				};
				this.ActionStatus = ActionStatus.Insert;
				return;
			}

			this.Input = new FeatureAreaInput
			{
				AreaId = model.AreaId,
				AreaName = model.AreaName,
				InternalMemo = model.InternalMemo,
				AreaTypeId = model.AreaTypeId,
				ActionType = model.ActionType,
				SideMaxCount = model.SideMaxCount,
				SideTurn = model.SideTurn,
				SliderArrow = model.SliderArrow,
				SliderCount = model.SliderCount,
				SliderDot = model.SliderDot,
				SliderScrollAuto = model.SliderScrollAuto,
				SliderScrollCount = model.SliderScrollCount,
				SliderScrollInterval = model.SliderScrollInterval,
				DateChanged = model.DateChanged,
				BannerInput = model.Banners.Select(
					(banner, index) => new FeatureAreaBannerInput(banner, index, targetList)
					{
						AreaId = banner.AreaId,
						BannerNo = banner.BannerNo,
						FileName = banner.FileName,
						FileDirPath = banner.FileDirPath,
						AltText = banner.AltText,
						Text = banner.Text,
						LinkUrl = banner.LinkUrl,
						WindowType = banner.WindowType,
						Publish = (banner.Publish == Constants.FLG_FEATUREAREABANNER_PUBLISH_PUBLIC),
					}).ToArray(),
				UseType = Constants.FLG_PARTSDESIGN_USE_TYPE_PC
			};

			var partsModel = new PartsDesignService().GetPartsByAreaId(model.AreaId);
			if (partsModel != null)
			{
				this.Input.UseType = partsModel.UseType;
				this.PartsId = partsModel.PartsId;
			}

			this.ActionStatus = ActionStatus.Update;
		}

		/// <summary>
		/// ValueTextから情報を取得する
		/// </summary>
		/// <param name="field">フィールド名</param>
		/// <returns>ValueText情報</returns>
		private SelectListItem[] ValueTextItemArray(string field)
		{
			return ValueText.GetValueItemArray(Constants.TABLE_FEATUREAREA, field).Select(
				data => new SelectListItem()
				{
					Text = data.Text,
					Value = data.Value,
					Selected = (data.Value == @"1")
				}).ToArray();
		}

		/// <summary>選択肢群 エリアタイプ</summary>
		public FeatureAreaTypeListSearchResult[] AreaTypes { get; private set; }
		/// <summary>選択肢群 横サイズ</summary>
		public SelectListItem[] SideMaxCounts { get; private set; }
		/// <summary>選択肢群 横折り返し</summary>
		public SelectListItem[] SideTurns { get; private set; }
		/// <summary>選択肢群 スライダ数</summary>
		public SelectListItem[] SliderCounts { get; private set; }
		/// <summary>選択肢群 スライド数</summary>
		public SelectListItem[] SliderScrollCounts { get; private set; }
		/// <summary>選択肢群 自動スクロール</summary>
		public SelectListItem[] SliderScrollAutos { get; private set; }
		/// <summary>選択肢群 自動スクロール間隔</summary>
		public SelectListItem[] SliderScrollIntervals { get; private set; }
		/// <summary>選択肢群 矢印</summary>
		public SelectListItem[] SliderArrows { get; private set; }
		/// <summary>選択肢群 ドット表示</summary>
		public SelectListItem[] SliderDots { get; private set; }
		/// <summary>画像リストから選択</summary>
		public ImageModalViewModel ImageModalViewModel { get; set; }
		/// <summary>入力クラス</summary>
		public FeatureAreaInput Input { get; set; }
		/// <summary>端末タイプ(表示用)</summary>
		public string UseTypeForDisp {
			get
			{
				return (string.IsNullOrEmpty(this.Input.UseType) == false)
					? ValueText.GetValueText(Constants.VALUE_TEXT_KEY_CMS_COMMON, Constants.VALUE_TEXT_FIELD_USE_TYPE, this.Input.UseType)
					: "";
			}
		}
		/// <summary>パーツを開くボタンを表示するか</summary>
		public bool CanUsePartsDesign
		{
			get
			{
				return this.IsActionStatusUpdate
					&& this.PartsId != null
					&& ManagerMenuCache.Instance.HasOperatorMenuAuthority(Constants.PATH_ROOT + Constants.CONTROLLER_W2CMS_MANAGER_PARTS_DESIGN + "/");
			}
		}
		/// <summary>個別プロパティが有効になるか</summary>
		public bool IsPropValid
		{
			get
			{
				return (this.Input.ActionType == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SIDE)
					|| (this.Input.ActionType == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SLIDER);
			}
		}
		/// <summary>横並びか</summary>
		public bool IsAreaTypeSide
		{
			get { return (this.Input.ActionType == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SIDE); }
		}
		/// <summary>スライダーか</summary>
		public bool IsAreaTypeSlider
		{
			get { return (this.Input.ActionType == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_SLIDER); }
		}
		/// <summary>選択肢群 端末タイプ</summary>
		public SelectListItem[] UseTypeList { get; private set; }
		/// <summary>紐づくパーツID</summary>
		public long? PartsId { get; private set; }
	}
}
