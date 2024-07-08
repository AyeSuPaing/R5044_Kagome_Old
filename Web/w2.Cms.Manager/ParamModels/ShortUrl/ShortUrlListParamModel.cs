/*
=========================================================================================================
  Module      : ショートURLパラメタモデル(ShortUrlListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Input;

namespace w2.Cms.Manager.ParamModels.ShortUrl
{
	/// <summary>
	/// ショートURLパラメタモデル
	/// </summary>
	[Serializable]
	public class ShortUrlListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShortUrlListParamModel()
		{
			this.ShortUrl = "";
			this.SortKbn = "1";
			this.LongUrl = "";
			this.ProtocolAndDomain = "";
			this.DataExportType = "";
			this.PagerNo = 1;
			this.BulkTargetInput = new ShortUrlInput[0];
			this.RegisterInput = new ShortUrlInput();
			this.DelSurlNo = "";
			this.PageLayout = Constants.LAYOUT_PATH_DEFAULT;
		}

		/// <summary>ショートURL（検索条件）</summary>
		public string ShortUrl { get; set; }
		/// <summary>並び順（検索条件）</summary>
		public string SortKbn { get; set; }
		/// <summary>ロングURL（検索条件）</summary>
		public string LongUrl { get; set; }
		/// <summary>プロトコル＆ドメイン</summary>
		public string ProtocolAndDomain { get; set; }
		/// <summary>データ出力タイプ</summary>
		public string DataExportType { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
		/// <summary>更新対象の入力データ</summary>
		public ShortUrlInput[] BulkTargetInput { get; set; }
		/// <summary>登録対象の入力データ</summary>
		public ShortUrlInput RegisterInput { get; set; }
		/// <summary>削除対象のショートURLNO</summary>
		public string DelSurlNo { get; set; }
		/// <summary>ページレイアウト</summary>
		public string PageLayout { get; set; }
	}
}