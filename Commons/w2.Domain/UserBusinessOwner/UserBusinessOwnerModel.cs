/*
=========================================================================================================
  Module      : User Business Owner Model(UserBusinessOwnerModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.UserBusinessOwner
{
	public class UserBusinessOwnerModel : ModelBase<UserBusinessOwnerModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserBusinessOwnerModel()
		{
			this.ShopCustomerId = string.Empty;
			this.OwnerName1 = string.Empty;
			this.OwnerName2 = string.Empty;
			this.OwnerNameKana1 = string.Empty;
			this.OwnerNameKana2 = string.Empty;
			this.CreditStatus = string.Empty;
			this.Birth = null;
			this.RequestBudget = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserBusinessOwnerModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserBusinessOwnerModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		/// <summary>
		/// ビジネスオーナーの変更
		/// </summary>
		/// <param name="userBusinessOwner">ビジネスオーナーのモデル</param>
		/// <returns>変更有り</returns>
		public bool CheckChange(UserBusinessOwnerModel userBusinessOwner)
		{
			if ((this.OwnerName1 != userBusinessOwner.OwnerName1) ||
				(this.OwnerName2 != userBusinessOwner.OwnerName2) ||
				(this.OwnerNameKana1 != userBusinessOwner.OwnerNameKana1) ||
				(this.OwnerNameKana2 != userBusinessOwner.OwnerNameKana2) ||
				(this.Birth != userBusinessOwner.Birth) ||
				(this.RequestBudget != userBusinessOwner.RequestBudget))
			{
				return true;
			}

			return false;
		}

		#region Field
		/// <summary>ユーザID</summary>
		[UpdateData(1, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_USER_ID] = value; }
		}
		/// <summary>GMO店舗の顧客ID</summary>
		[UpdateData(2, "gmo_shop_customer_id")]
		public string ShopCustomerId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_SHOP_CUSTOMER_ID]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_SHOP_CUSTOMER_ID] = value; }
		}
		/// <summary>オーナー名1</summary>
		[UpdateData(3, "owner_name1")]
		public string OwnerName1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME1]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME1] = value; }
		}
		/// <summary>オーナー名2</summary>
		[UpdateData(4, "owner_name2")]
		public string OwnerName2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME2]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME2] = value; }
		}
		/// <summary>オーナー名（カナ）1</summary>
		[UpdateData(5, "owner_name_kana1")]
		public string OwnerNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1] = value; }
		}
		/// <summary>オーナー名（カナ）2</summary>
		[UpdateData(6, "owner_name_kana2")]
		public string OwnerNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2] = value; }
		}
		/// <summary>生年月日</summary>
		[UpdateData(7, "birth")]
		public DateTime? Birth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH];
			}
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH] = value; }
		}
		/// <summary>要求限度額予算</summary>
		[UpdateData(8, "request_budget")]
		public int RequestBudget
		{
			get 
			{
				string val = this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET].ToString();
				if (string.IsNullOrWhiteSpace(val))
				{
					return 0;
				}
				return int.Parse(val);
			}
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET] = value; }
		}
		/// <summary>与信状況</summary>
		[UpdateData(9, "credit_status")]
		public string CreditStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(10, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(11, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_DATE_CHANGED] = value; }
		}
		#endregion
	}
}