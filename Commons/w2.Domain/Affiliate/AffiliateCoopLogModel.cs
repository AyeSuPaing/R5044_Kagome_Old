/*
=========================================================================================================
  Module      : アフィリエイト連携ログモデル (AffiliateCoopLog.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイト連携ログモデル
	/// </summary>
	[Serializable]
	public partial class AffiliateCoopLogModel : ModelBase<AffiliateCoopLogModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AffiliateCoopLogModel()
		{
			this.AffiliateKbn = "";
			this.MasterId = "";
			this.CoopStatus = "WAIT";
			this.CoopData1 = "";
			this.CoopData2 = "";
			this.CoopData3 = "";
			this.CoopData4 = "";
			this.CoopData5 = "";
			this.CoopData6 = "";
			this.CoopData7 = "";
			this.CoopData8 = "";
			this.CoopData9 = "";
			this.CoopData10 = "";
			this.LastChanged = "";
			this.CoopData11 = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateCoopLogModel(DataRowView source)
			:this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateCoopLogModel(Hashtable source)
			:this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ログNo</summary>
		public int LogNo
		{
			get { return (int)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_LOG_NO]; }
		}
		/// <summary>アフィリエイト区分</summary>
		public string AffiliateKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN] = value; }
		}
		/// <summary>マスタID</summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID] = value; }
		}
		/// <summary>連携ステータス</summary>
		public string CoopStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_STATUS]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_STATUS] = value; }
		}
		/// <summary>連携データ1</summary>
		public string CoopData1
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1] = value; }
		}
		/// <summary>連携データ2</summary>
		public string CoopData2
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA2]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA2] = value; }
		}
		/// <summary>連携データ3</summary>
		public string CoopData3
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA3]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA3] = value; }
		}
		/// <summary>連携データ4</summary>
		public string CoopData4
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA4]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA4] = value; }
		}
		/// <summary>連携データ5</summary>
		public string CoopData5
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA5]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA5] = value; }
		}
		/// <summary>連携データ6</summary>
		public string CoopData6
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA6]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA6] = value; }
		}
		/// <summary>連携データ7</summary>
		public string CoopData7
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA7]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA7] = value; }
		}
		/// <summary>連携データ8</summary>
		public string CoopData8
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA8]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA8] = value; }
		}
		/// <summary>連携データ9</summary>
		public string CoopData9
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA9]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA9] = value; }
		}
		/// <summary>連携データ10</summary>
		public string CoopData10
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA10]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA10] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_LAST_CHANGED] = value; }
		}
		/// <summary>連携データ11</summary>
		public int? CoopData11
		{
			get
			{
				if (this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11];
			}
			set { this.DataSource[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11] = value; }
		}
		#endregion
	}
}
