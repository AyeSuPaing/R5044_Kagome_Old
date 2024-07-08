/*
=========================================================================================================
  Module      : 特集エリアユーザコントロール(FeatureAreaUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Cms;
using w2.App.Common.DataCacheController;
using w2.App.Common.Preview;
using w2.Domain.ShortUrl.Helper;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.FeatureArea;

/// <summary>
/// 特集エリアユーザコントロール
/// </summary>
public class FeatureAreaUserControl : BaseUserControl
{
	#region ラップ済コントロール宣言
	/// <summary>特集エリアリピーター</summary>
	public WrappedRepeater WrFeatureArea { get { return GetWrappedControl<WrappedRepeater>("rFeatureArea"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitComponents();
		}
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	private void InitComponents()
	{
		// プレビューモードチェック
		bool previewMode = false;
		if (Request[Constants.REQUEST_KEY_PREVIEW_HASH] != null)
		{
			// ハッシュチェック
			if (Request[Constants.REQUEST_KEY_PREVIEW_HASH] == FeatureAreaPreview.CreateFeatureAreaHash())
			{
				previewMode = true;

				// プレビューの場合、キャッシュを残さない
				Response.AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate");
				Response.AddHeader("Pragma", "no-cache");
				Response.Cache.SetAllowResponseInBrowserHistory(false);
			}
		}

		this.FeatureArea = previewMode
			? new FeatureAreaModel()
			{
				DataSource = FeatureAreaPreview.GetFeatureAreaPreview(this.FeatureAreaId)
			}
			: DataCacheControllerFacade.GetFeatureAreaBannerCacheController().CacheData
				.FirstOrDefault(m => (m.AreaId == this.FeatureAreaId));

		var modelSource = previewMode
			? FeatureAreaPreview.GetFeatureAreaBannerPreview(this.FeatureAreaId).Select(
				data => new FeatureAreaBannerModel()
				{
					DataSource = data
				}).ToArray()
				: (this.FeatureArea != null) ? this.FeatureArea.Banners : new FeatureAreaBannerModel[0];

		// アクセスユーザ情報取得
		var accessUser = new ReleaseRangeAccessUser
		{
			Now = this.ReferenceDateTime,
			MemberRankInfo = this.LoginMemberRankInfo,
			IsLoggedIn = this.IsLoggedIn,
			HitTargetListId = this.LoginUserHitTargetListIds
		};

		if (previewMode)
		{
			var relatedParts = DataCacheControllerFacade.GetPartsDesignCacheController().CacheData
				.FirstOrDefault(partsDesign => partsDesign.AreaId == this.FeatureAreaId);

			if (relatedParts != null)
			{
				var releaseRangeResult = new ReleaseRangePartsDesign(relatedParts).Check(accessUser);

				if (releaseRangeResult.IsReleaseRangeOut)
				{
					this.WrFeatureArea.Visible = false;
					return;
				}
			}
		}

		// 公開範囲条件の確認
		var modelPublishSource = modelSource
			.Where(model => new ReleaseRangeFeatureAreaBanner(model).IsPublish(accessUser)).ToArray();
		var source = modelPublishSource.Select(model => new FeatureAreaBannerInput(model)).ToArray();

		// ランダムの場合は任意のデータを選択する
		if ((this.FeatureArea != null)
			&& (this.FeatureArea.AreaTypeId == Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_RANDOM)
			&& (source.Length != 0))
		{
			source = new[] { source[new Random().Next(0, source.Length)] };
		}

		this.WrFeatureArea.DataSource = source;
		this.WrFeatureArea.DataBind();
		this.WrFeatureArea.Visible = (source.Length != 0);
	}

	/// <summary>特集エリアID</summary>
	public string FeatureAreaId { get; set; }
	/// <summary>特集エリアID</summary>
	public FeatureAreaModel FeatureArea { get; private set; }
	/// <summary>横並び数</summary>
	public string SideMaxCount
	{
		get { return this.FeatureArea.SideMaxCount; }
	}
	/// <summary>折り返し設定</summary>
	public bool IsSideTurn
	{
		get
		{
			return (this.FeatureArea != null)
				&& (this.FeatureArea.SideTurn == Constants.FLG_FEATUREAREA_SIDE_TURN_VALID);
		}
	}
	/// <summary>矢印</summary>
	public string SliderArrow
	{
		get
		{
			return ((this.FeatureArea != null)
				&& (this.FeatureArea.SliderArrow == Constants.FLG_FEATUREAREA_SLIDER_ARROW_VALID)).ToString().ToLower();
		}
	}
	/// <summary>スライド数</summary>
	public string SliderCount
	{
		get { return (this.FeatureArea != null) ? this.FeatureArea.SliderCount : "1"; }
	}
	/// <summary>ドット表示</summary>
	public string SliderDot
	{
		get
		{
			return ((this.FeatureArea != null)
				&& (this.FeatureArea.SliderDot == Constants.FLG_FEATUREAREA_SLIDER_DOT_VALID)).ToString().ToLower();
		}
	}
	/// <summary>自動スクロール</summary>
	public string SliderScrollAuto
	{
		get
		{
			return ((this.FeatureArea != null)
				&& (this.FeatureArea.SliderScrollAuto == Constants.FLG_FEATUREAREA_SLIDER_SCROLL_AUTO_VALID)).ToString().ToLower();
		}
	}
	/// <summary>スクロールのスライド数</summary>
	public string SliderScrollCount
	{
		get { return (this.FeatureArea != null) ? this.FeatureArea.SliderScrollCount : "1"; }
	}
	/// <summary>スライド間隔</summary>
	public string SliderScrollInterval
	{
		get { return (this.FeatureArea != null) ? this.FeatureArea.SliderScrollInterval : "1"; }
	}

	/// <summary>
	/// 表示用特集エリアバナークラス
	/// </summary>
	public class FeatureAreaBannerInput : FeatureAreaBannerModel
	{
		/// <summary>コンストラクタ</summary>
		public FeatureAreaBannerInput()
		{
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="model">特集エリアバナーモデル</param>
		public FeatureAreaBannerInput(FeatureAreaBannerModel model)
		{
			this.DataSource = model.DataSource;
		}

		/// <summary>表示用URL</summary>
		public string DisplayLinkUrl
		{
			get
			{
				if (string.IsNullOrEmpty(this.LinkUrl)) return "";
				return (Regex.IsMatch(this.LinkUrl, @"^https://") || Regex.IsMatch(this.LinkUrl, @"^http://"))
					? this.LinkUrl
					: UrlUtility.AddProtocolAndDomain(this.LinkUrl);
			}
		}
		/// <summary>画像ファイルパス</summary>
		public string ImageFilePath
		{
			get { return (string.IsNullOrEmpty(this.PreviewBinary)) ? (Constants.PATH_ROOT + HttpUtility.UrlEncode(this.FilePath).Replace("+", "%20")) : this.PreviewBinary; }
		}
	}

}