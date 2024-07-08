/*
=========================================================================================================
  Module      : ページ管理 検索条件 (PageDesignListSearch.cs)
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

namespace w2.Domain.PageDesign.Helper
{
	/// <summary>
	/// ページ管理 検索条件
	/// </summary>
	[Serializable]
	public class PageDesignListSearch : BaseDbMapModel
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
		[DbMapName("page_type_like_escaped")]
		public string PageTypesLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(string.Join(",", this.PageTypes)); }
		}
		/// <summary>ページタイプ</summary>
		public string[] PageTypes { get; set; }
		/// <summary>ページ利用状態（SQL LIKEエスケープ済）</summary>
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
			var pageTypesReplace = new KeyValuePair<string, string>(
				"@@ parts_type @@",
				string.Join(",", this.PageTypes.Select(id => string.Format("'{0}'", id))));
			var useTypesReplace = new KeyValuePair<string, string>(
				"@@ use_type @@",
				string.Join(",", this.UseType.Select(id => string.Format("'{0}'", id))));
			var result = new[] { pageTypesReplace, useTypesReplace };
			return result;
		}
	}

	/// <summary>
	/// ページ管理 検索結果 ページ単位
	/// </summary>
	[Serializable]
	public class PageDesignListSearchResult : PageDesignModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PageDesignListSearchResult()
		{
			this.ManagementTitle = "";
			this.PageType = Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM;
			this.FileName = "";
			this.FileDirPath = "";
			this.GroupId = 0;
			this.PageSortNumber = 0;
			this.UseType = Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP;
			this.Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PUBLIC;
			this.ConditionPublishDateFrom = null;
			this.ConditionPublishDateTo = null;
			this.ConditionMemberOnlyType = Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL;
			this.ConditionMemberRankId = "";
			this.ConditionTargetListType = Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_OR;
			this.ConditionTargetListIds = "";
			this.LastChanged = "";
			this.MetadataDesc = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PageDesignListSearchResult(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PageDesignListSearchResult(Hashtable source) : this()
		{
			this.DataSource = source;
		}
	}

	/// <summary>
	/// ページ管理 検索結果 グループ単位
	/// </summary>
	[Serializable]
	public class PageDesignListSearchGroupResult : PageDesignGroupModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PageDesignListSearchGroupResult()
		{
			this.GroupName = "";
			this.GroupSortNumber = 0;
			this.LastChanged = "";
			this.PageList = new PageDesignListSearchResult[] { };
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PageDesignListSearchGroupResult(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PageDesignListSearchGroupResult(Hashtable source) : this()
		{
			this.DataSource = source;
		}

		/// <summary>ページ管理 検索結果 ページ単位</summary>
		public PageDesignListSearchResult[] PageList { get; set; }
	}
}