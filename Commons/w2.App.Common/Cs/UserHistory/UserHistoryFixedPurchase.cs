/*
=========================================================================================================
  Module      : ユーザー履歴（定期購入情報）クラス(UserHistoryFixedPurchase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain.FixedPurchase.Helper;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー履歴（定期購入情報）クラス
	/// </summary>
	public class UserHistoryFixedPurchase : UserHistoryBase
	{
		private const string KBN_STRING = "定期情報";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		public UserHistoryFixedPurchase(UserFixedPurchaseListSearchResult info)
			: base(info.DataSource)
		{
			this.FixedPurchase = info;
		}

		/// <summary>
		/// 情報セット
		/// </summary>
		protected override void SetInfo()
		{
			this.DateTime = this.DateCreated;
			this.KbnString = KBN_STRING;
		}

		#region プロパティ
		/// <summary>定期購入情報</summary>
		public UserFixedPurchaseListSearchResult FixedPurchase { get; private set; }
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED]; }
		}
		#endregion
	}
}
