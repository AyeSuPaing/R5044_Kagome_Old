/*
=========================================================================================================
  Module      : メール振分設定モデルのパーシャルクラス(CsMailAssignSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Extensions;
using w2.Common.Util;
using w2.App.Common.Cs.CsOperator;

namespace w2.App.Common.MailAssignSetting
{
	/// <summary>
	/// メール振分設定モデルのパーシャルクラス
	/// </summary>
	public partial class CsMailAssignSettingModel : ModelBase<CsMailAssignSettingModel>
	{
		/// <summary>
		/// メール振分設定アイテムリスト
		/// </summary>
		public CsMailAssignSettingItemModel[] Items { get; set; }

		/// <summary>
		/// 総件数
		/// </summary>
		public int EX_SearchCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>
		/// 振分先インシデントカテゴリ名
		/// </summary>
		public string EX_AssignIncidentCategoryName
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME])).ConvertIfNullEmpty("－"); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME] = value; }
		}
		/// <summary>
		/// 振分後担当オペレータ名
		/// </summary>
		public string EX_AssignOperatorName
		{
			get
			{
				return ((string)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID] == Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID_CLEAR) ?
					Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_CLEAR_TEXT : (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME])).ConvertIfNullEmpty("－");
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME] = value; }
		}
		/// <summary>
		/// 振分後担当CSグループ名
		/// </summary>
		public string EX_AssignCsGroupName
		{
			get
			{
				return ((string)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_CS_GROUP_ID] == Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_GROUP_ID_CLEAR) ?
					Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_CLEAR_TEXT : (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_NAME])).ConvertIfNullEmpty("－");
			}
			set { this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_NAME] = value; }
		}
		/// <summary>
		/// 振分後重要度名
		/// </summary>
		public string EX_AssignImportanceName
		{
			get
			{
				return (ValueText.GetValueText(Constants.TABLE_CSINCIDENT,
					Constants.FIELD_CSINCIDENT_IMPORTANCE, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_IMPORTANCE])).ConvertIfNullEmpty("－");
			}
		}
		/// <summary>
		/// 振分後ステータス名
		/// </summary>
		public string EX_AssignStatusName
		{
			get
			{
				return (ValueText.GetValueText(Constants.TABLE_CSINCIDENT,
					Constants.FIELD_CSINCIDENT_STATUS, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_STATUS])).ConvertIfNullEmpty("－");
			}
		}
		/// <summary>
		/// 論理演算子名
		/// </summary>
		public string EX_LogicalOperationName
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTING,
					Constants.FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR]);
			}
		}
		/// <summary>
		/// 論理演算子名（シンプル）
		/// </summary>
		public string EX_LogicalOperationName_Short
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTING,
					Constants.FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR + "_short", this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR]);
			}
		}
		/// <summary>
		/// オートレスポンス表示
		/// </summary>
		public string EX_AutoResponseName
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTING,
					Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE]);
			}
		}
		/// <summary>
		/// ごみ箱表示
		/// </summary>
		public string EX_TrashName
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTING,
					Constants.FIELD_CSMAILASSIGNSETTING_TRASH, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_TRASH]);
			}
		}
		/// <summary>
		/// 振分け停止表示
		/// </summary>
		public string EX_StopFiltering
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTING,
					Constants.FIELD_CSMAILASSIGNSETTING_STOP_FILTERING, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_STOP_FILTERING]);
			}
		}

		/// <summary>
		/// このメール振り分け設定に、別のメール振り分け設定を統合します。
		/// 値が設定されている項目だけが上書きされます。
		/// </summary>
		public void Merge(CsMailAssignSettingModel settingModel)
		{
			// 振分けルール
			if (settingModel.AssignStatus != "") this.AssignStatus = settingModel.AssignStatus;
			if (settingModel.AssignIncidentCategoryId != "") this.AssignIncidentCategoryId = settingModel.AssignIncidentCategoryId;
			if (settingModel.AssignImportance != "") this.AssignImportance = settingModel.AssignImportance;
			if (settingModel.AssignOperatorId != "") this.AssignOperatorId = settingModel.AssignOperatorId;
			if (settingModel.AssignCsGroupId != "") this.AssignCsGroupId = settingModel.AssignCsGroupId;
			if (settingModel.Trash != Constants.FLG_CSMAILASSIGNSETTING_TRASH_INVALID) this.Trash = settingModel.Trash;

			// オートレスポンス
			this.AutoResponse = settingModel.AutoResponse;
			this.AutoResponseFrom = settingModel.AutoResponseFrom;
			this.AutoResponseCc = settingModel.AutoResponseCc;
			this.AutoResponseBcc = settingModel.AutoResponseBcc;
			this.AutoResponseSubject = settingModel.AutoResponseSubject;
			this.AutoResponseBody = settingModel.AutoResponseBody;
		}

		/// <summary>既存インシデント紐付時にマッチする振分設定かどうか</summary>
		public bool EX_IsMatchOnBind
		{
			get { return (this.MailAssignId == Constants.CONST_MAIL_ASSIGN_ID_MATCH_ON_BIND); }
		}

		/// <summary>すべての受信メールにマッチする振分設定かどうか</summary>
		public bool EX_IsMatchAnything
		{
			get { return (this.MailAssignId == Constants.CONST_MAIL_ASSIGN_ID_MATCH_ANYTHING); }
		}

		/// <summary>個別作成した振分け設定（=システム既定の振分設定ではない）かどうか</summary>
		public bool EX_IsMatchUserSetting
		{
			get { return (EX_IsMatchOnBind == false) && (EX_IsMatchAnything == false); }
		}

		/// <summary>振分後担当オペレータの有効フラグ</summary>
		private string EX_AssignOperatorValidFlg
		{
			get { return StringUtility.ToNull(this.DataSource["shop_operator_" + Constants.FIELD_SHOPOPERATOR_VALID_FLG]); }
		}

		/// <summary>振分後担当オペレータが存在するかどうか</summary>
		public bool EX_AssignOperatorExists
		{
			get { return this.EX_AssignOperatorValidFlg != null; }
		}

		/// <summary>振分後担当オペレータが有効かどうか</summary>
		public bool EX_AssignOperatorValid
		{
			get { return EX_AssignOperatorExists && (this.EX_AssignOperatorValidFlg == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID); }
		}

		/// <summary>振分後担当CSグループの有効フラグ</summary>
		public string EX_AssignCsGroupValidFlg
		{
			get { return StringUtility.ToNull(this.DataSource["cs_group_" + Constants.FIELD_SHOPOPERATOR_VALID_FLG]); }
		}

		/// <summary>振分後担当CSグループが存在するかどうか</summary>
		public bool EX_AssignCsGroupExists
		{
			get { return this.EX_AssignCsGroupValidFlg != null; }
		}

		/// <summary>振分後担当CSグループが有効かどうか</summary>
		public bool EX_AssignCsGroupValid
		{
			get { return EX_AssignCsGroupExists && (this.EX_AssignCsGroupValidFlg == Constants.FLG_CSGROUP_VALID_FLG_VALID); }
		}
	}
}
