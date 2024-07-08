/*
=========================================================================================================
  Module      : ポイントルールマスタモデル (PointRuleModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントルールマスタモデル
	/// </summary>
	[Serializable]
	public partial class PointRuleModel : ModelBase<PointRuleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PointRuleModel()
		{
			this.PointRuleKbn = Constants.FLG_USERPOINTHISTORY_POINT_RULE_KBN_BASE;
			this.PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE;
			this.UseTempFlg = Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID;
			this.Term = 0;
			this.PeriodBegin = new DateTime(0);
			this.PeriodEnd = new DateTime(0);
			this.IncNum = 0;
			this.IncRate = 0;
			this.PointExpExtend = DEFAULT_POINT_EXP_EXTEND_STRING;
			this.Priority = 100;
			this.ValidFlg = Constants.FLG_POINTRULE_VALID_FLG_VALID;
			this.RuleDate = new PointRuleDateModel[] { };
			this.EffectiveOffset = null;
			this.EffectiveOffsetType = null;
			this.Term = null;
			this.TermType = null;
			this.PeriodBegin = null;
			this.PeriodEnd = null;
			this.AllowDuplicateApplyFlg = Constants.FLG_POINTRULE_DUPLICATE_APPLY_DISALLOW;
			this.IncFixedPurchaseNum = 0;
			this.IncFixedPurchaseRate = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PointRuleModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PointRuleModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_DEPT_ID] = value; }
		}
		/// <summary>ポイントルールID</summary>
		public string PointRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_ID] = value; }
		}
		/// <summary>ポイントルール名</summary>
		public string PointRuleName
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_NAME]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_NAME] = value; }
		}
		/// <summary>ポイントルール区分</summary>
		public string PointRuleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_KBN] = value; }
		}
		/// <summary>対象ポイント区分</summary>
		public string PointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_KBN] = value; }
		}
		/// <summary>仮ポイント使用フラグ</summary>
		public string UseTempFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_USE_TEMP_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_USE_TEMP_FLG] = value; }
		}
		/// <summary>ポイント加算区分</summary>
		public string PointIncKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_INC_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_INC_KBN] = value; }
		}
		/// <summary>ポイント加算方法</summary>
		public string IncType
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_INC_TYPE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_INC_TYPE] = value; }
		}
		/// <summary>ポイント加算数</summary>
		public decimal IncNum
		{
			get { return (decimal)this.DataSource[Constants.FIELD_POINTRULE_INC_NUM]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_INC_NUM] = value; }
		}
		/// <summary>ポイント加算率</summary>
		public decimal IncRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_POINTRULE_INC_RATE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_INC_RATE] = value; }
		}
		/// <summary>ポイント有効期限延長</summary>
		public string PointExpExtend
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_EXP_EXTEND]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_EXP_EXTEND] = value; }
		}
		/// <summary>有効期間（開始）</summary>
		public DateTime? ExpBgn
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_EXP_BGN] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_POINTRULE_EXP_BGN];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_EXP_BGN] = value; }
		}
		/// <summary>有効期間（終了）</summary>
		public DateTime? ExpEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_EXP_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_POINTRULE_EXP_END];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_EXP_END] = value; }
		}
		/// <summary>キャンペーン期間区分</summary>
		public string CampaignTermKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_KBN] = value; }
		}
		/// <summary>キャンペーン期間値</summary>
		public string CampaignTermValue
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_VALUE]); }
			set { this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_VALUE] = value; }
		}
		/// <summary>優先順位</summary>
		public int Priority
		{
			get { return (int)this.DataSource[Constants.FIELD_POINTRULE_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_PRIORITY] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_VALID_FLG] = value; }
		}
		/// <summary>予備区分1</summary>
		public string Kbn1
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN1]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN1] = value; }
		}
		/// <summary>予備区分2</summary>
		public string Kbn2
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN2]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN2] = value; }
		}
		/// <summary>予備区分3</summary>
		public string Kbn3
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN3]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN3] = value; }
		}
		/// <summary>予備区分4</summary>
		public string Kbn4
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN4]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN4] = value; }
		}
		/// <summary>予備区分5</summary>
		public string Kbn5
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN5]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN5] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_POINTRULE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_POINTRULE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_LAST_CHANGED] = value; }
		}
		/// <summary>期間限定ポイント発効オフセット</summary>
		public int? EffectiveOffset
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET] = value; }
		}
		/// <summary>期間限定ポイント発効オフセット種別</summary>
		public string EffectiveOffsetType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE] = value; }
		}
		/// <summary>期間限定ポイント有効期間（相対）</summary>
		public int? Term
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_TERM] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_POINTRULE_TERM];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_TERM] = value; }
		}
		/// <summary>期間限定ポイント有効期間（相対）種別</summary>
		public string TermType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_TERM_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_POINTRULE_TERM_TYPE];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_TERM_TYPE] = value; }
		}
		/// <summary>期間限定ポイント有効期間（絶対）開始日</summary>
		public DateTime? PeriodBegin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_PERIOD_BEGIN] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_POINTRULE_PERIOD_BEGIN];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_PERIOD_BEGIN] = value; }
		}
		/// <summary>期間限定ポイント有効期間（絶対）終了日</summary>
		public DateTime? PeriodEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_PERIOD_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_POINTRULE_PERIOD_END];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_PERIOD_END] = value; }
		}
		/// <summary>キャンペーン：基本ルールとの二重適用を許可するか</summary>
		public string AllowDuplicateApplyFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_ALLOW_DUPLICATE_APPLY_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_ALLOW_DUPLICATE_APPLY_FLG] = value; }
		}
		/// <summary>定期ポイント加算方法</summary>
		public string IncFixedPurchaseType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE] = value; }
		}
		/// <summary>定期ポイント加算数</summary>
		public decimal? IncFixedPurchaseNum
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_NUM] == DBNull.Value) return null; 
				return (decimal)this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_NUM];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_NUM] = value; }
		}
		/// <summary>定期ポイント加算率</summary>
		public decimal? IncFixedPurchaseRate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_RATE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_RATE];
			}
			set { this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_RATE] = value; }
		}
		#endregion
	}
}
