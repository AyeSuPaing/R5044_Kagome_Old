/*
=========================================================================================================
  Module      : パーツデザイン パーツ管理モデル (PartsDesignModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.PartsDesign
{
	/// <summary>
	/// パーツデザイン パーツ管理モデル
	/// </summary>
	[Serializable]
	public partial class PartsDesignModel : ModelBase<PartsDesignModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PartsDesignModel()
		{
			this.ManagementTitle = "";
			this.PartsType = Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM;
			this.FileName = "";
			this.FileDirPath = "";
			this.GroupId = 0;
			this.PartsSortNumber = 0;
			this.UseType = Constants.FLG_PARTSDESIGN_USE_TYPE_PC_SP;
			this.Publish = Constants.FLG_PARTSDESIGN_PUBLISH_PUBLIC;
			this.ConditionPublishDateFrom = null;
			this.ConditionPublishDateTo = null;
			this.ConditionMemberOnlyType = Constants.FLG_PARTSDESIGN_MEMBER_ONLY_TYPE_ALL;
			this.ConditionMemberRankId = "";
			this.ConditionTargetListType = Constants.FLG_PARTSDESIGN_CONDITION_TARGET_LIST_TYPE_OR;
			this.ConditionTargetListIds = "";
			this.AreaId = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PartsDesignModel(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PartsDesignModel(Hashtable source) : this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public long PartsId
		{
			get { return (long)this.DataSource[Constants.FIELD_PARTSDESIGN_PARTS_ID]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_PARTS_ID] = value; }
		}
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_MANAGEMENT_TITLE]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_MANAGEMENT_TITLE] = value; }
		}
		/// <summary>タイプ</summary>
		public string PartsType
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_PARTS_TYPE]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_PARTS_TYPE] = value; }
		}
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_FILE_NAME]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_FILE_NAME] = value; }
		}
		/// <summary>PCディレクトリパス</summary>
		public string FileDirPath
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_PC_FILE_DIR_PATH]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_PC_FILE_DIR_PATH] = value; }
		}
		/// <summary>グループ識別ID</summary>
		public long GroupId
		{
			get { return (long)this.DataSource[Constants.FIELD_PARTSDESIGN_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_GROUP_ID] = value; }
		}
		/// <summary>グループ内パーツ順序</summary>
		public int PartsSortNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_PARTSDESIGN_PARTS_SORT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_PARTS_SORT_NUMBER] = value; }
		}
		/// <summary>パーツの利用状態</summary>
		public string UseType
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_USE_TYPE]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_USE_TYPE] = value; }
		}
		/// <summary>パーツの公開状態</summary>
		public string Publish
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_PUBLISH]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_PUBLISH] = value; }
		}
		/// <summary>公開範囲:公開開始日</summary>
		public DateTime? ConditionPublishDateFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_FROM] == DBNull.Value)
					return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_FROM];
			}
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_FROM] = value; }
		}
		/// <summary>公開範囲:公開終了日</summary>
		public DateTime? ConditionPublishDateTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_TO];
			}
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_PUBLISH_DATE_TO] = value; }
		}
		/// <summary>公開範囲:会員限定コンテンツ</summary>
		public string ConditionMemberOnlyType
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_MEMBER_ONLY_TYPE]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_MEMBER_ONLY_TYPE] = value; }
		}
		/// <summary>公開範囲:一部会員に公開する場合の会員ランク</summary>
		public string ConditionMemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_MEMBER_RANK_ID] = value; }
		}
		/// <summary>公開範囲:ターゲットリスト検索条件</summary>
		public string ConditionTargetListType
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_TARGET_LIST_TYPE]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_TARGET_LIST_TYPE] = value; }
		}
		/// <summary>公開範囲:対象ターゲットリスト</summary>
		public string ConditionTargetListIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_TARGET_LIST_IDS]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_CONDITION_TARGET_LIST_IDS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PARTSDESIGN_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PARTSDESIGN_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_LAST_CHANGED] = value; }
		}
		/// <summary>特集エリアID</summary>
		public string AreaId
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_AREA_ID]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_AREA_ID] = value; }
		}
		#endregion
	}
}