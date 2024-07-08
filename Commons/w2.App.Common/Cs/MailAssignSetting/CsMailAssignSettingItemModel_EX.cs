/*
=========================================================================================================
  Module      : メール振分設定アイテムモデルのパーシャルクラス(CsMailAssignSettingItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.App.Common.MailAssignSetting
{
	/// <summary>
	/// メール振分設定アイテムモデルのパーシャルクラス
	/// </summary>
	public partial class CsMailAssignSettingItemModel : ModelBase<CsMailAssignSettingItemModel>
	{
		/// <summary>振マッチング対象名</summary>
		public string EX_AssignItemMatchingTargetName
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTINGITEM,
					Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET]);
			}
		}
		/// <summary>大文字小文字非区別名</summary>
		public string EX_IgnoreCaseName
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTINGITEM,
					Constants.FIELD_CSMAILASSIGNSETTINGITEM_IGNORE_CASE, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_IGNORE_CASE]);
			}
		}
		/// <summary>マッチング種別名</summary>
		public string EX_MatchingTypeName
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSMAILASSIGNSETTINGITEM,
					Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE, this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE]);
			}
		}
	}
}
