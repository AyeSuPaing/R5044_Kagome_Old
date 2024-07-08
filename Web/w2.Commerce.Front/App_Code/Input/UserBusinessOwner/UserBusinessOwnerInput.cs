/*
=========================================================================================================
  Module      : User Business Owner Input(UserBusinessOwnerInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Input;
using w2.Domain.UserBusinessOwner;

/// <summary>
/// ビジネスオーナーの入力
/// </summary>
[Serializable]
public class UserBusinessOwnerInput :InputBase<UserBusinessOwnerModel>
{
	#region 定数
	/// <summary>
	/// ユーザー入力チェック列挙体
	/// </summary>
	public enum EnumUserInputValidationKbn
	{
		Regist = 0,
		Modify = 1,
	}
	#endregion

	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public UserBusinessOwnerInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="user">与信ステータス</param>
	public UserBusinessOwnerInput(UserBusinessOwnerModel user)
		: this()
	{
		this.OwnerName1 = user.OwnerName1;
		this.OwnerName2 = user.OwnerName2;
		this.OwnerNameKana1 = user.OwnerNameKana1;
		this.OwnerNameKana2 = user.OwnerNameKana2;
		this.RequestBudget = user.RequestBudget.ToString();
		this.CreditStatus = user.CreditStatus;
		this.Birth = (user.Birth != null) ? user.Birth.ToString() : null;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>与信ステータス</returns>
	public override UserBusinessOwnerModel CreateModel()
	{
		var user = new UserBusinessOwnerModel
		{
			OwnerName1 = this.OwnerName1,
			OwnerName2 = this.OwnerName2,
			OwnerNameKana1 = this.OwnerNameKana1,
			OwnerNameKana2 = this.OwnerNameKana2,
			CreditStatus = this.CreditStatus == null ? string.Empty : this.CreditStatus,
			Birth = (this.Birth != null) ? DateTime.Parse(this.Birth) : (DateTime?)null,
		};

		var requestBudget = 0;
		int.TryParse(this.RequestBudget, out requestBudget);
		user.RequestBudget = requestBudget;

		return user;
	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <param name="validateKbn">入力チェック区分</param>
	/// <param name="excludeList">チェック除外項目</param>
	/// <returns>成功:空/失敗:エラーメッセージ</returns>
	public Dictionary<string, string> Validate(EnumUserInputValidationKbn validateKbn, List<string> excludeList = null)
	{
		var input = (Hashtable)this.DataSource.Clone();
		Dictionary<string, string> errorMessages = new Dictionary<string, string>();

		// 入力チェック
		switch (validateKbn)
		{
			case EnumUserInputValidationKbn.Regist:
				errorMessages = Validator.ValidateAndGetErrorContainer("UserRegist", input, (string)input[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]);
				break;

			case EnumUserInputValidationKbn.Modify:
				errorMessages = Validator.ValidateAndGetErrorContainer("UserBusinessOwner", input, (string)input[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]);
				break;

			default:
				break;
		}

		// エラーメッセージがなければ成功
		return errorMessages;
	}
	#endregion

	#region GMO
	/// <summary>GMO店舗の顧客ID</summary>
	public string ShopCustomerId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_SHOP_CUSTOMER_ID]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_SHOP_CUSTOMER_ID] = value; }
	}
	/// <summary>オーナー名1</summary>
	public string OwnerName1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME1]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME1] = value; }
	}
	/// <summary>オーナー名2</summary>
	public string OwnerName2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME2]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME2] = value; }
	}
	/// <summary>オーナー名（カナ）1</summary>
	public string OwnerNameKana1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1] = value; }
	}
	/// <summary>オーナー名（カナ）2</summary>
	public string OwnerNameKana2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2] = value; }
	}
	/// <summary>生年月日（年）</summary>
	public string BirthYear
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH_YEAR]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH_YEAR] = value; }
	}
	/// <summary>生年月日（月）</summary>
	public string BirthMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH_MONTH]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH_MONTH] = value; }
	}
	/// <summary>生年月日（日）</summary>
	public string BirthDay
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH_DAY]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH_DAY] = value; }
	}
	/// <summary>生年月日</summary>
	public string Birth
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH] = value; }
	}
	/// <summary>要求限度額予算</summary>
	public string RequestBudget
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET] = value; }
	}
	/// <summary>与信状況</summary>
	public string CreditStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS]; }
		set { this.DataSource[Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS] = value; }
	}
	#endregion
}