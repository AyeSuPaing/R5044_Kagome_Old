/*
=========================================================================================================
  Module      : 特集エリアバナーモデル (FeatureAreaBannerModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアバナーモデル
	/// </summary>
	[Serializable]
	public partial class FeatureAreaBannerModel : ModelBase<FeatureAreaBannerModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FeatureAreaBannerModel()
		{
			this.AreaId = "";
			this.FileName = "";
			this.FileDirPath = "";
			this.AltText = "";
			this.Text = "";
			this.LinkUrl = "";
			this.WindowType = Constants.FLG_FEATUREAREABANNER_WINDOW_TYPE_NONPOPUP;
			this.Publish = Constants.FLG_FEATUREAREABANNER_PUBLISH_PUBLIC;
			this.ConditionPublishDateFrom = null;
			this.ConditionPublishDateTo = null;
			this.ConditionMemberOnlyType = Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL;
			this.ConditionMemberRankId = "";
			this.ConditionTargetListType = Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_OR;
			this.ConditionTargetListIds = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaBannerModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaBannerModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>特集エリアID</summary>
		public string AreaId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_AREA_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_AREA_ID] = value; }
		}
		/// <summary>特集エリアバナーNo</summary>
		public int BannerNo
		{
			get { return (int)this.DataSource[Constants.FIELD_FEATUREAREABANNER_BANNER_NO]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_BANNER_NO] = value; }
		}
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_FILE_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_FILE_NAME] = value; }
		}
		/// <summary>ディレクトリパス</summary>
		public string FileDirPath
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_FILE_DIR_PATH]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_FILE_DIR_PATH] = value; }
		}
		/// <summary>代替テキスト</summary>
		public string AltText
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_ALT_TEXT]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_ALT_TEXT] = value; }
		}
		/// <summary>テキスト</summary>
		public string Text
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_TEXT]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_TEXT] = value; }
		}
		/// <summary>リンク</summary>
		public string LinkUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_LINK_URL]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_LINK_URL] = value; }
		}
		/// <summary>ウィンドウタイプ</summary>
		public string WindowType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_WINDOW_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_WINDOW_TYPE] = value; }
		}
		/// <summary>公開状態</summary>
		public string Publish
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_PUBLISH]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_PUBLISH] = value; }
		}
		/// <summary>公開範囲:公開開始日</summary>
		public DateTime? ConditionPublishDateFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_FROM] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_FROM];
			}
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_FROM] = value; }
		}
		/// <summary>公開範囲:公開終了日</summary>
		public DateTime? ConditionPublishDateTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_TO];
			}
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_PUBLISH_DATE_TO] = value; }
		}
		/// <summary>公開範囲:会員限定コンテンツ</summary>
		public string ConditionMemberOnlyType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_MEMBER_ONLY_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_MEMBER_ONLY_TYPE] = value; }
		}
		/// <summary>公開範囲:一部会員に公開する場合の会員ランク</summary>
		public string ConditionMemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_MEMBER_RANK_ID] = value; }
		}
		/// <summary>公開範囲:ターゲットリスト検索条件</summary>
		public string ConditionTargetListType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_TARGET_LIST_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_TARGET_LIST_TYPE] = value; }
		}
		/// <summary>公開範囲:対象ターゲットリスト</summary>
		public string ConditionTargetListIds
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_TARGET_LIST_IDS]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_CONDITION_TARGET_LIST_IDS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREAREABANNER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREABANNER_DATE_CREATED] = value; }
		}
		#endregion
	}
}
