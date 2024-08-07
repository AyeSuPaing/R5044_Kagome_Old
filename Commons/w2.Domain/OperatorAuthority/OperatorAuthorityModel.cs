/*
=========================================================================================================
  Module      : オペレータ権限モデル (OperatorAuthorityModel.cs)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OperatorAuthority
{
	/// <summary>
	/// オペレータ権限モデル
	/// </summary>
	[Serializable]
	public partial class OperatorAuthorityModel : ModelBase<OperatorAuthorityModel>
	{
		/// <summary>Const default condition type</summary>
		public const string CONST_DEFAULT_CONDITION_TYPE = "REAL_SHOP";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OperatorAuthorityModel()
		{
			ShopId = Constants.CONST_DEFAULT_SHOP_ID;
			OperatorId = string.Empty;
			ConditionType = CONST_DEFAULT_CONDITION_TYPE;
			Permission = Constants.FLG_ON;
			ConditionValue = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OperatorAuthorityModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OperatorAuthorityModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_OPERATORAUTHORITY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_OPERATORAUTHORITY_SHOP_ID] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return (string)this.DataSource[Constants.FIELD_OPERATORAUTHORITY_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_OPERATORAUTHORITY_OPERATOR_ID] = value; }
		}
		/// <summary>権限設定</summary>
		public string ConditionType
		{
			get { return (string)this.DataSource[Constants.FIELD_OPERATORAUTHORITY_CONDITION_TYPE]; }
			set { this.DataSource[Constants.FIELD_OPERATORAUTHORITY_CONDITION_TYPE] = value; }
		}
		/// <summary>許可区分</summary>
		public string Permission
		{
			get { return (string)this.DataSource[Constants.FIELD_OPERATORAUTHORITY_PERMISSION]; }
			set { this.DataSource[Constants.FIELD_OPERATORAUTHORITY_PERMISSION] = value; }
		}
		/// <summary>設定値</summary>
		public string ConditionValue
		{
			get { return (string)this.DataSource[Constants.FIELD_OPERATORAUTHORITY_CONDITION_VALUE]; }
			set { this.DataSource[Constants.FIELD_OPERATORAUTHORITY_CONDITION_VALUE] = value; }
		}
		#endregion
	}
}
