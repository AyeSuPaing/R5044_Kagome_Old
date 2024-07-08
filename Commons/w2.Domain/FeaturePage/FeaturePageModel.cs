/*
=========================================================================================================
  Module      : 特集ページ情報モデル (FeaturePageModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeaturePage
{
	/// <summary>
	/// 特集ページ情報モデル
	/// </summary>
	[Serializable]
	public partial class FeaturePageModel : ModelBase<FeaturePageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageModel()
		{
			this.ManagementTitle = string.Empty;
			this.PageType = Constants.FLG_FEATUREPAGE_GROUP;
			this.FileDirPath = string.Empty;
			this.UseType = Constants.FLG_FEATUREPAGE_USE_TYPE_ALL;
			this.HtmlPageTitle = string.Empty;
			this.MetadataDesc = string.Empty;
			this.Publish = Constants.FLG_FEATUREPAGE_PUBLISH_PUBLIC;
			this.ConditionPublishDateFrom = null;
			this.ConditionPublishDateTo = null;
			this.ConditionMemberOnlyType = Constants.FLG_FEATUREPAGE_MEMBER_ONLY_TYPE_ALL;
			this.ConditionMemberRankId = null;
			this.ConditionTargetListType = Constants.FLG_FEATUREPAGE_CONDITION_TARGET_LIST_TYPE_OR;
			this.ConditionTargetListIds = null;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeaturePageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeaturePageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>特集ページID</summary>
		public long FeaturePageId
		{
			get { return (long)this.DataSource[Constants.FIELD_FEATUREPAGE_FEATURE_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_FEATURE_PAGE_ID] = value; }
		}
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_MANAGEMENT_TITLE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_MANAGEMENT_TITLE] = value; }
		}
		/// <summary>ページタイプ</summary>
		public string PageType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_PAGE_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_PAGE_TYPE] = value; }
		}
		/// <summary>特集ページカテゴリID</summary>
		public string CategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_CATEGORY_ID] = value; }
		}
		/// <summary>ブランドID</summary>
		public string PermittedBrandIds
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_PERMITTED_BRAND_IDS]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_PERMITTED_BRAND_IDS] = value; }
		}
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_FILE_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_FILE_NAME] = value; }
		}
		/// <summary>ディレクトリパス</summary>
		public string FileDirPath
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_FILE_DIR_PATH]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_FILE_DIR_PATH] = value; }
		}
		/// <summary>ページ順序</summary>
		public int PageSortNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_FEATUREPAGE_PAGE_SORT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_PAGE_SORT_NUMBER] = value; }
		}
		/// <summary>HTMLページタイトル</summary>
		public string HtmlPageTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_HTML_PAGE_TITLE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_HTML_PAGE_TITLE] = value; }
		}
		/// <summary>SEOディスクリプション</summary>
		public string MetadataDesc
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_METADATA_DESC]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_METADATA_DESC] = value; }
		}
		/// <summary>ページの利用状態</summary>
		public string UseType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_USE_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_USE_TYPE] = value; }
		}
		/// <summary>ページの公開状態</summary>
		public string Publish
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_PUBLISH]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_PUBLISH] = value; }
		}
		/// <summary>公開範囲:公開開始日</summary>
		public DateTime? ConditionPublishDateFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_FROM] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_FROM];
			}
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_FROM] = value; }
		}
		/// <summary>公開範囲:公開終了日</summary>
		public DateTime? ConditionPublishDateTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_TO];
			}
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_PUBLISH_DATE_TO] = value; }
		}
		/// <summary>公開範囲:会員限定コンテンツ</summary>
		public string ConditionMemberOnlyType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_MEMBER_ONLY_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_MEMBER_ONLY_TYPE] = value; }
		}
		/// <summary>公開範囲:一部会員に公開する場合の会員ランク</summary>
		public string ConditionMemberRankId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_MEMBER_RANK_ID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_MEMBER_RANK_ID];
			}
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_MEMBER_RANK_ID] = value; }
		}
		/// <summary>公開範囲:ターゲットリスト検索条件</summary>
		public string ConditionTargetListType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_TARGET_LIST_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_TARGET_LIST_TYPE] = value; }
		}
		/// <summary>公開範囲:対象ターゲットリスト</summary>
		public string ConditionTargetListIds
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_TARGET_LIST_IDS] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_TARGET_LIST_IDS];
			}
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_CONDITION_TARGET_LIST_IDS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREPAGE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREPAGE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}