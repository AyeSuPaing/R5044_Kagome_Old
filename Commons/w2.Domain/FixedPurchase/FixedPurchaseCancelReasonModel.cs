/*
=========================================================================================================
  Module      : 定期解約理由区分設定モデル (FixedPurchaseCancelReasonModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期解約理由区分設定モデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseCancelReasonModel : ModelBase<FixedPurchaseCancelReasonModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseCancelReasonModel()
		{
			this.DisplayOrder = 1;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseCancelReasonModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseCancelReasonModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>解約理由区分ID</summary>
		public string CancelReasonId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_ID] = value; }
		}
		/// <summary>解約理由区分名</summary>
		public string CancelReasonName
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_ORDER] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_KBN] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CHANGED] = value; }
		}
		/// <summary>更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
