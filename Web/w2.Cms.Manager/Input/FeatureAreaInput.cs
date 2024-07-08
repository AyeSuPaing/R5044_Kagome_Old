/*
=========================================================================================================
  Module      : 特集エリアタイプ入力クラス(FeatureAreaImput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Web;
using w2.App.Common.Input;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.FeatureArea;
using w2.Domain.FeatureImage;
using w2.Domain.TargetList;
using Validator = w2.Cms.Manager.Codes.Common.Validator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 特集エリアタイプ入力クラス
	/// </summary>
	public class FeatureAreaInput : InputBase<FeatureAreaModel>
	{
		/// <summary>バナー（ダミー）ファイルのありか</summary>
		private const string DUMMY_BANNER_FILE_PATH = "Contents/ImagesPkg/bnr_top980b.jpg";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureAreaInput()
		{
			// 初期値は使いやすそうな値を固定で指定
			this.AreaId = "";
			this.SideMaxCount = "3";
			this.SideTurn = "1";
			this.SliderArrow = "1";
			this.SliderCount = "1";
			this.SliderDot = "1";
			this.SliderScrollAuto = "1";
			this.SliderScrollCount = "1";
			this.SliderScrollInterval = "4";
			this.AreaTypeId = "";
			this.BannerInput = new FeatureAreaBannerInput[0];
			this.UseType = "";
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override FeatureAreaModel CreateModel()
		{
			var model = new FeatureAreaModel
			{
				AreaId = this.AreaId,
				AreaName = this.AreaName,
				InternalMemo = this.InternalMemo,
				AreaTypeId = this.AreaTypeId,
				ActionType = this.ActionType,
				SideMaxCount = this.SideMaxCount,
				SideTurn = this.SideTurn,
				SliderArrow = this.SliderArrow,
				SliderCount = this.SliderCount,
				SliderDot = this.SliderDot,
				SliderScrollAuto = this.SliderScrollAuto,
				SliderScrollCount = this.SliderScrollCount,
				SliderScrollInterval = this.SliderScrollInterval,
				Banners = this.BannerInput.Select(banner => banner.CreateModel()).ToArray(),
				UseType = this.UseType
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="isUpdate">更新か</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(bool isUpdate)
		{
			var errorMessage = Validator.Validate(
				isUpdate ? "FeatureAreaModify" : "FeatureAreaRegister",
				this.CreateModel().DataSource);

			if (this.BannerInput.Length == 0)
			{
				errorMessage += WebMessages.FeatureAreaBannerNotSelectedError;
			}

			foreach (var banner in this.BannerInput)
			{
				errorMessage += banner.Validate();
			}

			// 画像リストから選択されていない且つアップロードファイルがある
			var imageFileNameList = this.BannerInput
				.Where(data =>
					(data.BannerImageInput.ImageId == 0)
					&& (data.BannerImageInput.UploadFile != null))
				.Select(data => data.FileName).ToArray();
			var distinctImageFileNameList = imageFileNameList.Distinct().ToArray();

			// 新規ファイルがアップロード場合には既存ファイルと同名かを判定
			var count = distinctImageFileNameList
				.Sum(fileName => new FeatureImageService().GetImageCountByFileName(fileName));

			if ((imageFileNameList.Length != distinctImageFileNameList.Length) || (count > 0))
			{
				errorMessage += WebMessages.FeatureAreaImageDulplicateNameError;
			}

			return errorMessage;
		}

		/// <summary>
		/// ダミーデータ作成
		/// </summary>
		/// <param name="actionType">動作タイプ</param>
		/// <param name="bannerLength">バナー数</param>
		/// <returns>ダミーデータ</returns>
		public static FeatureAreaInput CreateDummy(string actionType, int bannerLength = 3)
		{
			// 名前が一致しないように名前を変更する
			var areaId = "__" + actionType;
			var previewInput = new FeatureAreaInput()
			{
				AreaId = areaId,
				AreaName = "preview",
				ActionType = actionType,
			};
			var targetList = new TargetListService().GetAll(Constants.CONST_DEFAULT_SHOP_ID);
			var banners = Enumerable.Range(1, bannerLength).Select(
				i => new FeatureAreaBannerInput(targetList)
				{
					Text = "PreviewSample" + i,
					AltText = "PreviewSample" + i,
					AreaId = areaId,
					BannerNo = i,
					WindowType = Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_POPUP,
					FileName = DUMMY_BANNER_FILE_PATH,
					ReleaseRangeSettingInput = new ReleaseRangeSettingInput(),
					Publish = true,
				}).ToArray();
			previewInput.BannerInput = banners;

			return previewInput;
		}

		/// <summary> 特集エリアID </summary>
		public string AreaId { get; set; }
		/// <summary> 特集エリア名</summary>
		public string AreaName { get; set; }
		/// <summary> 内部メモ </summary>
		public string InternalMemo { get; set; }
		/// <summary> 特集エリアタイプID </summary>
		public string AreaTypeId { get; set; }
		/// <summary> 特集エリアタイプ動作タイプ </summary>
		public string ActionType { get; set; }
		/// <summary> 横並び数 </summary>
		public string SideMaxCount { get; set; }
		/// <summary> 折り返し </summary>
		public string SideTurn { get; set; }
		/// <summary> スライド矢印表示 </summary>
		public string SliderArrow { get; set; }
		/// <summary> スライド数 </summary>
		public string SliderCount { get; set; }
		/// <summary> スライド下部ドット表示 </summary>
		public string SliderDot { get; set; }
		/// <summary> 自動スクロール </summary>
		public string SliderScrollAuto { get; set; }
		/// <summary> スライドスクロール数 </summary>
		public string SliderScrollCount { get; set; }
		/// <summary> 自動スクロール間隔 </summary>
		public string SliderScrollInterval { get; set; }
		/// <summary> 更新日 </summary>
		public DateTime DateChanged { get; set; }
		/// <summary> バナー </summary>
		public FeatureAreaBannerInput[] BannerInput { get; set; }
		/// <summary>更新日</summary>
		public string DisplayDateChanged
		{
			get
			{
				return DateTimeUtility.ToStringForManager(
					this.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			}
		}
		/// <summary>端末タイプ</summary>
		public string UseType { get; set; }
	}

	/// <summary>
	/// 特集エリアバナー入力クラス
	/// </summary>
	public class FeatureAreaBannerInput : InputBase<FeatureAreaBannerModel>
	{
		/// <summary>部品に渡す親名称のフォーマット</summary>
		private const string PARENT_NAME_FORMAT = "bannerInputs[{0}].{1}";
		/// <summary>部品に渡す親名称のフォーマット</summary>
		private const string IMAGE_BASE_NAME = "BannerImageInput";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureAreaBannerInput()
			: this(targetList: null)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="targetList">ターゲットリストモデル 設定がある場合に利用 無い場合はメソッド内で取得</param>
		public FeatureAreaBannerInput(TargetListModel[] targetList)
		{
			this.WindowType = Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_NONPOPUP;
			this.Index = "___index";
			this.IsWindowPopup = false;

			this.BannerImageInput = new ImageInput(string.Format(PARENT_NAME_FORMAT, this.Index, IMAGE_BASE_NAME), ImageType.Area);
			this.ReleaseRangeSettingInput = new ReleaseRangeSettingInput(
				this.Index,
				string.Format(PARENT_NAME_FORMAT, this.Index, "ReleaseRangeSettingInput"),
				new ReleaseRangeSettingModel()
				{
					ConditionMemberOnlyType = Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL,
					ConditionTargetListType = Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_OR
				},
				targetListModels:targetList);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="targetList">ターゲットリストモデル 設定がある場合に利用 無い場合はメソッド内で取得</param>
		public FeatureAreaBannerInput(FeatureAreaBannerModel model, TargetListModel[] targetList)
			: this(model, 0, targetList)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="index">作成するバナーのIndex</param>
		/// <param name="targetList">ターゲットリストモデル 設定がある場合に利用 無い場合はメソッド内で取得</param>
		public FeatureAreaBannerInput(FeatureAreaBannerModel model, string index, TargetListModel[] targetList)
			: this(model, int.Parse(index), targetList)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="index">作成するバナーのIndex</param>
		/// <param name="targetList">ターゲットリストモデル 設定がある場合に利用 無い場合はメソッド内で取得</param>
		public FeatureAreaBannerInput(FeatureAreaBannerModel model, int index, TargetListModel[] targetList)
			: this(targetList)
		{
			this.WindowType = Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_NONPOPUP;
			this.Index = (index).ToString();
			this.IsWindowPopup = false;

			this.BannerImageInput = new ImageInput(string.Format(PARENT_NAME_FORMAT, index, IMAGE_BASE_NAME), ImageType.Area)
			{
				RealFileName = model.FileName,
				FileName = HttpUtility.UrlEncode(model.FileName).Replace("+", "%20"),
			};

			this.ReleaseRangeSettingInput = new ReleaseRangeSettingInput(
				this.Index,
				string.Format(PARENT_NAME_FORMAT, index, "ReleaseRangeSettingInput"),
				new ReleaseRangeSettingModel()
				{
					ConditionMemberOnlyType = model.ConditionMemberOnlyType,
					ConditionMemberRankId = model.ConditionMemberRankId,
					ConditionTargetListType = model.ConditionTargetListType,
					ConditionTargetListIds = model.ConditionTargetListIds,
					ConditionPublishDateFrom = model.ConditionPublishDateFrom,
					ConditionPublishDateTo = model.ConditionPublishDateTo
				},
				targetListModels: targetList);
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override FeatureAreaBannerModel CreateModel()
		{
			var releaseRangeSettingModel = this.ReleaseRangeSettingInput.CreateModel();
			var model = new FeatureAreaBannerModel()
			{
				AreaId = this.AreaId,
				BannerNo = this.BannerNo,
				FileName = this.FileName,
				FileDirPath = this.FileDirPath,
				AltText = this.AltText,
				Text = this.Text,
				LinkUrl = this.LinkUrl,
				WindowType = this.WindowType,
				Publish = this.Publish ? Constants.FLG_FEATUREAREABANNER_PUBLISH_PUBLIC : Constants.FLG_FEATUREAREABANNER_PUBLISH_PRIVATE,
				ConditionPublishDateFrom = releaseRangeSettingModel.ConditionPublishDateFrom,
				ConditionPublishDateTo = releaseRangeSettingModel.ConditionPublishDateTo,
				ConditionMemberOnlyType = releaseRangeSettingModel.ConditionMemberOnlyType,
				ConditionMemberRankId = releaseRangeSettingModel.ConditionMemberRankId,
				ConditionTargetListIds = releaseRangeSettingModel.ConditionTargetListIds,
				ConditionTargetListType = releaseRangeSettingModel.ConditionTargetListType,
				PreviewBinary = this.BannerImageInput.PreviewBinary
			};

			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var msgs = new Validator.ErrorMessageList();
			var errorMessages = w2.Common.Util.Validator.Validate(
				"FeatureAreaBanner",
				this.CreateModel().DataSource,
				Constants.GLOBAL_OPTION_ENABLE ? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE : "");
			errorMessages.ForEach(
				errorMsg => msgs.Add(errorMsg.Key, string.Format("バナー[{0}]：{1}", this.BannerNo + 1, errorMsg.Value)));
			var errorMessage = Validator.ChangeToDisplay(msgs);
			errorMessage += this.ReleaseRangeSettingInput.Validate();

			return errorMessage;
		}

		#region プロパティ
		/// <summary>特集エリアID</summary>
		public string AreaId { get; set; }
		/// <summary>特集エリアバナーNo</summary>
		public int BannerNo { get; set; }
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName { get; set; }
		/// <summary>ディレクトリパス</summary>
		public string FileDirPath { get; set; }
		/// <summary>代替テキスト</summary>
		public string AltText { get; set; }
		/// <summary>テキスト</summary>
		public string Text { get; set; }
		/// <summary>リンク</summary>
		public string LinkUrl { get; set; }
		/// <summary>ウィンドウタイプ</summary>
		public string WindowType { get; set; }
		/// <summary>公開状態</summary>
		public bool Publish { get; set; }
		/// <summary>画像情報</summary>
		public ImageInput BannerImageInput { get; set; }
		/// <summary>公開範囲</summary>
		public ReleaseRangeSettingInput ReleaseRangeSettingInput { get; set; }
		/// <summary>表示番号</summary>
		public string Index { get; set; }
		/// <summary>ポップアップか</summary>
		public bool IsWindowPopup
		{
			get { return this.WindowType == Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_POPUP; }
			set
			{
				this.WindowType = value
					? Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_POPUP
					: Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_NONPOPUP;
			}
		}
		#endregion
	}
}
