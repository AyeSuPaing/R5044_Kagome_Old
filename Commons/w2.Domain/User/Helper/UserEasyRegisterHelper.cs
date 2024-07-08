/*
=========================================================================================================
  Module      : かんたん会員登録ヘルパー (UserEasyRegisterHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// かんたん会員登録ヘルパー
	/// </summary>
	public class UserEasyRegisterHelper
	{
		/// <summary>
		/// 入力チェック項目取得
		/// </summary>
		/// <param name="itemId">アイテムID</param>
		/// <returns>入力チェック項目</returns>
		public List<string> GetValidaterItemList(string itemId)
		{
			var result = new List<string>();
			switch (itemId)
			{
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR1:
					result.Add(Constants.FIELD_USER_ADDR1);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR2:
					result.Add(Constants.FIELD_USER_ADDR2);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR3:
					result.Add(Constants.FIELD_USER_ADDR3);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR4:
					result.Add(Constants.FIELD_USER_ADDR4);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_BIRTH:
					result.Add(Constants.FIELD_USER_BIRTH);
					result.Add(Constants.FIELD_USER_BIRTH_YEAR);
					result.Add(Constants.FIELD_USER_BIRTH_MONTH);
					result.Add(Constants.FIELD_USER_BIRTH_DAY);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_NAME:
					result.Add(Constants.FIELD_USER_COMPANY_NAME);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_POST_NAME:
					result.Add(Constants.FIELD_USER_COMPANY_POST_NAME);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_ADDR2:
					result.Add(Constants.FIELD_USER_MAIL_ADDR2);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_FLG:
					result.Add(Constants.FIELD_USER_MAIL_FLG);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME:
					result.Add(Constants.FIELD_USER_NAME);
					result.Add(Constants.FIELD_USER_NAME1);
					result.Add(Constants.FIELD_USER_NAME2);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME_KANA:
					result.Add(Constants.FIELD_USER_NAME_KANA);
					result.Add(Constants.FIELD_USER_NAME_KANA1);
					result.Add(Constants.FIELD_USER_NAME_KANA2);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NICK_NAME:
					result.Add(Constants.FIELD_USER_NICK_NAME);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_SEX:
					result.Add(Constants.FIELD_USER_SEX);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL1:
					result.Add(Constants.FIELD_USER_TEL1);
					result.Add(Constants.FIELD_USER_TEL1_1);
					result.Add(Constants.FIELD_USER_TEL1_2);
					result.Add(Constants.FIELD_USER_TEL1_3);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL2:
					result.Add(Constants.FIELD_USER_TEL2);
					result.Add(Constants.FIELD_USER_TEL2_1);
					result.Add(Constants.FIELD_USER_TEL2_2);
					result.Add(Constants.FIELD_USER_TEL2_3);
					break;

				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ZIP:
					result.Add(Constants.FIELD_USER_ZIP);
					result.Add(Constants.FIELD_USER_ZIP1);
					result.Add(Constants.FIELD_USER_ZIP2);
					break;
			}

			return result;
		}
	}
}
