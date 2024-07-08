/*
=========================================================================================================
  Module      : パーツ管理 検索条件 (PartsDesignListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.PartsDesign.Helper
{
	/// <summary>
	/// パーツ管理 検索条件
	/// </summary>
	[Serializable]
	public class PartsDesignListSearch : BaseDbMapModel
	{
		/// <summary>検索キーワード（SQL LIKEエスケープ済）</summary>
		[DbMapName("keyword_like_escaped")]
		public string KeywordLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.Keyword); }
		}
		/// <summary>検索キーワード</summary>
		public string Keyword { get; set; }
		/// <summary>グループID</summary>
		[DbMapName("group_id")]
		public long? GroupId { get; set; }
		/// <summary></summary>
		[DbMapName("parts_type_like_escaped")]
		public string PartsTypesLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(string.Join(",", this.PartsTypes)); }
		}
		/// <summary>パーツタイプ</summary>
		public string[] PartsTypes { get; set; }
		/// <summary>パーツ利用状態（SQL LIKEエスケープ済）</summary>
		[DbMapName("use_type_like_escaped")]
		public string UseTypeLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(string.Join(",", this.UseType)); }
		}
		/// <summary>パーツ利用状態</summary>
		[DbMapName("use_type")]
		public string[] UseType { get; set; }

		/// <summary>
		/// SQL置換処理
		/// </summary>
		/// <returns>置換内容</returns>
		public KeyValuePair<string, string>[] ReplaceList()
		{
			var partsTypesReplace = new KeyValuePair<string, string>(
				"@@ parts_type @@",
				string.Join(",", this.PartsTypes.Select(id => string.Format("'{0}'", id))));
			var useTypesReplace = new KeyValuePair<string, string>(
				"@@ use_type @@",
				string.Join(",", this.UseType.Select(id => string.Format("'{0}'", id))));
			var result = new[] { partsTypesReplace, useTypesReplace };
			return result;
		}
	}

	/// <summary>
	/// パーツ管理 検索結果 パーツ単位
	/// </summary>
	[Serializable]
	public class PartsDesignListSearchResult : PartsDesignModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PartsDesignListSearchResult()
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
		public PartsDesignListSearchResult(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PartsDesignListSearchResult(Hashtable source) : this()
		{
			this.DataSource = source;
		}
	}

	/// <summary>
	/// パーツ管理 検索結果 グループ単位
	/// </summary>
	[Serializable]
	public class PartsDesignListSearchGroupResult : PartsDesignGroupModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PartsDesignListSearchGroupResult()
		{
			this.GroupName = "";
			this.GroupSortNumber = 0;
			this.LastChanged = "";
			this.PartsList = new PartsDesignListSearchResult[] { };
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PartsDesignListSearchGroupResult(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PartsDesignListSearchGroupResult(Hashtable source) : this()
		{
			this.DataSource = source;
		}

		/// <summary>パーツ管理 検索結果 パーツ単位</summary>
		public PartsDesignListSearchResult[] PartsList { get; set; }
	}
}