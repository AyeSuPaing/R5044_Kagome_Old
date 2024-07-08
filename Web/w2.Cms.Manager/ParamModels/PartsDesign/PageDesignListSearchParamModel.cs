/*
=========================================================================================================
  Module      : パーツ管理 パーツ検索パラメータ(PageDesignListSearchParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Cms;
using w2.Common.Util;

namespace w2.Cms.Manager.ParamModels.PartsDesign
{
	/// <summary>
	/// パーツ管理 パーツ検索パラメータ
	/// </summary>
	public class PartsDesignListSearchParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsDesignListSearchParamModel()
		{
			this.Keyword = string.Empty;
			this.GroupId = string.Empty;
			this.UseType = string.Empty;

			this.Types = new[]
			{
				new CheckBoxModel(
					ValueText.GetValueText(
						Constants.TABLE_PAGEDESIGN,
						Constants.FIELD_PAGEDESIGN_PAGE_TYPE,
						Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL),
					Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL,
					true),
				new CheckBoxModel(
					ValueText.GetValueText(
						Constants.TABLE_PAGEDESIGN,
						Constants.FIELD_PAGEDESIGN_PAGE_TYPE,
						Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM),
					Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM,
					true),
			};
		}

		/// <summary>検索キーワード</summary>
		public string Keyword { get; set; }
		/// <summary>グループID</summary>
		public string GroupId { get; set; }
		/// <summary>パーツタイプ</summary>
		public CheckBoxModel[] Types { get; set; }
		/// <summary>ページ利用状態</summary>
		public string UseType { get; set; }
	}
}