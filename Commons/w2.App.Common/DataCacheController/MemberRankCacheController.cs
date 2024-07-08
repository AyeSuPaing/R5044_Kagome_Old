/*
=========================================================================================================
  Module      : 会員ランクキャッシュコントローラ(MemberRankCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.RefreshFileManager;
using w2.Common.Sql;
using w2.Domain;
using w2.Domain.MemberRank;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 会員ランクキャッシュコントローラ
	/// </summary>
	public class MemberRankCacheController : DataCacheControllerBase<MemberRankModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MemberRankCacheController()
			: base(RefreshFileType.MemberRank)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.MemberRankService.GetMemberRankList();
		}
	}
}
