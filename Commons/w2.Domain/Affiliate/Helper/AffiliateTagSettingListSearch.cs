/*
=========================================================================================================
  Module      : アフィリエイトタグ検索のためのヘルパクラス (AffiliateTagSettingListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Affiliate.Helper
{
	/// <summary>
	/// アフィリエイトタグ検索のためのヘルパクラス
	/// </summary>
	[Serializable]
	public class AffiliateTagSettingListSearchCondition : BaseDbMapModel
	{
		/// <summary>フィールド名:対象ページ</summary>
		public const string FIELD_PAGE = "page";
		/// <summary>フィールド名:商品ID</summary>
		public const string FIELD_PRODUCT_ID = "product_id";
		/// <summary>フィールド名:広告コード</summary>
		public const string FIELD_ADVERTISEMENT_CODE = "advertisement_code";
		/// <summary>フィールド名:広告媒体区分</summary>
		public const string FIELD_ADVMEDIA_TYPE_ID = "advcode_media_type_id";
		/*
		* 検索条件となるものをプロパティで持つ
		* 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		*/

		#region プロパティ
		/// <summary>アフィリエイトタグ名 部分一致</summary>
		[DbMapName(Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_NAME + "_like_escaped")]
		public string AffiliateNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.AffiliateName); }
		}

		/// <summary>アフィリエイトタグ名</summary>
		[DbMapName(Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_NAME)]
		public string AffiliateName { get; set; }

		/// <summary>アフィリエイト区分</summary>
		[DbMapName(Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_KBN)]
		public string AffiliateKbn { get; set; }

		/// <summary>有効フラグ</summary>
		[DbMapName(Constants.FIELD_AFFILIATETAGSETTING_VALID_FLG)]
		public string ValidFlg { get; set; }

		/// <summary>対象ページ</summary>
		[DbMapName(FIELD_PAGE)]
		public string Page { get; set; }

		/// <summary>条件 商品ID 部分一致</summary>
		[DbMapName(FIELD_PRODUCT_ID + "_like_escaped")]
		public string ProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductId); }
		}

		/// <summary>条件 商品ID</summary>
		[DbMapName(FIELD_PRODUCT_ID)]
		public string ProductId { get; set; }

		/// <summary>条件 広告コード 部分一致</summary>
		[DbMapName(FIELD_ADVERTISEMENT_CODE + "_like_escaped")]
		public string AdvertisementCodeLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.AdvertisementCod); }
		}

		/// <summary>条件 広告コード</summary>
		[DbMapName(FIELD_ADVERTISEMENT_CODE)]
		public string AdvertisementCod { get; set; }

		/// <summary>条件 広告媒体区分</summary>
		[DbMapName(FIELD_ADVMEDIA_TYPE_ID)]
		public string AdvcodeMediaTypeId { get; set; }

		/// <summary>開始 行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }

		/// <summary>終了 行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		#endregion
	}

	/// <summary>
	/// アフィリエイトタグ検索 結果
	/// </summary>
	[Serializable]
	public class AffiliateTagSettingListSearchResult : AffiliateTagSettingModel
	{
		/// <summary>フィールド名:対象ページ</summary>
		private const string FIELD_PAGE_RESULT = "PAGE";
		/// <summary>フィールド名:商品ID</summary>
		private const string FIELD_PRODUCT_ID = "PRODUCT_ID";
		/// <summary>フィールド名:広告コード</summary>
		private const string FIELD_ADVERTISEMENT_CODE = "ADVERTISEMENT_CODE";
		/// <summary>フィールド名:広告媒体区分</summary>
		private const string FIELD_ADVMEDIA_TYPE_ID = "ADCODE_MEDIA_TYPE";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AffiliateTagSettingListSearchResult(DataRowView source) : base(source)
		{
		}
		#endregion

		#region プロパティ(Modelに実装している以外）
		/// <summary>画面条件</summary>
		public string PageConditions
		{
			get { return StringUtility.ToEmpty(this.DataSource[FIELD_PAGE_RESULT]); }
			set { this.DataSource[FIELD_PAGE_RESULT] = value; }
		}
		/// <summary>広告媒体条件</summary>
		public string AdcodeMediaTypeConditions
		{
			get { return StringUtility.ToEmpty(this.DataSource[FIELD_ADVMEDIA_TYPE_ID]); }
			set { this.DataSource[FIELD_ADVMEDIA_TYPE_ID] = value; }
		}
		/// <summary>広告コード条件</summary>
		public string AdvertisementCodeConditions
		{
			get { return StringUtility.ToEmpty(this.DataSource[FIELD_ADVERTISEMENT_CODE]); }
			set { this.DataSource[FIELD_ADVERTISEMENT_CODE] = value; }
		}
		/// <summary>商品ID条件</summary>
		public string ProductIdConditions
		{
			get { return StringUtility.ToEmpty(this.DataSource[FIELD_PRODUCT_ID]); }
			set { this.DataSource[FIELD_PRODUCT_ID] = value; }
		}
		/// <summary>タグ名称</summary>
		public string TagName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME]); }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME] = value; }
		}
		/// <summary>タグ内容</summary>
		public string TagContent
		{
			get
			{
				return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_CONTENT]);
			}
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_CONTENT] = value; }
		}
		/// <summary>区切り文字</summary>
		public string TagDelimiter
		{
			get
			{
				return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_DELIMITER]);
			}
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_DELIMITER] = value; }
		}
		#endregion
	}
}