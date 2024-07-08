/*
=========================================================================================================
  Module      :  サポートサイトコンテンツゲッター(SupportSiteContentsGetter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace w2.App.Common.SupportSite
{
	/// <summary>
	/// サポートサイトコンテンツライター
	/// </summary>
	public class SupportSiteContentsGetter
	{
		/// <summary>お知らせデータ</summary>
		static readonly SupportSiteContents m_infoData = new SupportSiteContents(Constants.SUPPORT_SITE_URL + "wp-json/wp/v2/posts?al=on&categories=15");
		/// <summary>最新の投稿データ</summary>
		static readonly SupportSiteContents m_newData = new SupportSiteContents(Constants.SUPPORT_SITE_URL + "wp-json/wp/v2/posts?al=on");

		/// <summary>
		/// コンテンツ取得
		/// </summary>
		/// <param name="infoKbn">情報区分</param>
		public string Get(string infoKbn)
		{
			switch (infoKbn)
			{
				// お知らせ
				case "info":
					return m_infoData.GetData();

				// 最新の投稿
				case "new":
					return m_newData.GetData();
			}
			throw new Exception("未対応のinfoKbn:" + infoKbn);
		}
	}
}
