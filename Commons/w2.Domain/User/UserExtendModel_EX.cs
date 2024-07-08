/*
=========================================================================================================
  Module      : ユーザ拡張項目マスタモデル (UserExtendModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using w2.Common.Util;
using w2.Domain.User.Helper;
using w2.Domain.UserExtendSetting;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザ拡張項目マスタモデル
	/// </summary>
	public partial class UserExtendModel
	{
		#region メソッド
		/// <summary>
		/// DBから取得したユーザ拡張項目から、拡張項目IDを取得
		/// </summary>
		private void SetUserExtendColumns()
		{
			foreach (string key in this.DataSource.Keys)
			{
				switch (key)
				{
					case Constants.FIELD_USEREXTEND_USER_ID:
					case Constants.FIELD_USEREXTEND_DATE_CREATED:
					case Constants.FIELD_USEREXTEND_DATE_CHANGED:
					case Constants.FIELD_USEREXTEND_LAST_CHANGED:
						break;
					default:
						this.UserExtendColumns.Add(key);
						break;
				}
			}
		}

		/// <summary>
		/// DBから取得したユーザ拡張項目から、値を取得
		/// </summary>
		private void SetUserExtendDataValue()
		{
			// レコードが取得できた場合のみ初期化する
			if (this.DataSource.Count > 0)
			{
				var userExtendDataDict = this.DataSource
					.Cast<DictionaryEntry>()
					.ToDictionary(row =>
						(string)row.Key, row => StringUtility.ToEmpty(row.Value));
				foreach (var key in userExtendDataDict.Keys)
				{
					this.UserExtendDataValue[key] = userExtendDataDict[key];
				}

				// ユーザー拡張項目以外を削除する。
				this.UserExtendDataValue.Remove(Constants.FIELD_USEREXTEND_USER_ID);
				this.UserExtendDataValue.Remove(Constants.FIELD_USEREXTEND_DATE_CREATED);
				this.UserExtendDataValue.Remove(Constants.FIELD_USEREXTEND_DATE_CHANGED);
				this.UserExtendDataValue.Remove(Constants.FIELD_USEREXTEND_LAST_CHANGED);
			}
		}

		/// <summary>
		/// DBから取得したユーザ拡張項目から、選択値に紐づく表示名を設定
		/// </summary>
		/// <remarks>コンストラクタでは全項目を取得するが、各アプリでは表示項目のみ設定しなおす</remarks>
		private void SetUserExtendDataText()
		{
			// w2_UserExtendSetting にレコードが無い 場合には処理中止
			if (UserExtendSettings.Items.Count <= 0) return;

			foreach (var userExtendSetting in UserExtendSettings.Items)
			{
				string text = "";
				if (userExtendSetting.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT && this.UserExtendDataValue.ContainsKey(userExtendSetting.SettingId))
				{
					// 入力方法がテキストの場合はそのまま表示
					text = this.UserExtendDataValue[userExtendSetting.SettingId];
				}
				else
				{
					// 入力方法がテキスト以外の場合は表示名
					text = GetValueDisplayName(userExtendSetting);
				}

				// 設定項目IDに紐づく表示文言を追加
				if (this.UserExtendDataValue.ContainsKey(userExtendSetting.SettingId))
				{
					this.UserExtendDataText[userExtendSetting.SettingId] = text;
				}
			}
		}

		/// <summary>
		/// 表示名を取得する
		/// </summary>
		/// <param name="model">ユーザー拡張設定情報</param>
		/// <remarks>チェックボックスの場合は半角カンマ区切り</remarks>
		/// <returns>表示名</returns>
		private string GetValueDisplayName(UserExtendSettingModel model)
		{
			StringBuilder text = new StringBuilder();
			foreach (string keyValue in (model.InputDefault).Split(';'))
			{
				string[] item = keyValue.Split(',');
				if (item.Length < 2) continue;

				// セミコロンで区切って格納されている場合には、それぞれ対応する表示名にして表示
				switch (model.InputType)
				{
					case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
						if (this.UserExtendDataValue.ContainsKey(model.SettingId) && (this.UserExtendDataValue[model.SettingId] == item[0]))
						{
							text.Append(item[1]);
						}
						break;

					case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
						if (this.UserExtendDataValue.ContainsKey(model.SettingId) && (this.UserExtendDataValue[model.SettingId]).Split(',').Contains(item[0]))
						{
							text.Append((text.ToString() != "") ? "," : "");
							text.Append(item[1]);
						}
						break;

					default:
						// なにもしない
						break;
				}
			}
			return text.ToString();
		}

		/// <summary>
		/// 空のモデルを作成する
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="userExtendSettingList">ユーザー拡張項目設定リスト</param>
		/// <returns>ユーザ拡張項目マスタモデル</returns>
		public static UserExtendModel CreateEmpty(string userId, UserExtendSettingList userExtendSettingList)
		{
			// ユーザー拡張項目設定リストからUserExtendModelを作成するための空のDataSourceを作成する
			var dataSource = (userExtendSettingList == null)
				? new Hashtable()
				: new Hashtable(
					userExtendSettingList
					.Items
					.Select(item => item.SettingId)
					.ToDictionary(k => k, v => DBNull.Value));

			var userExtendModel = new UserExtendModel(dataSource, userExtendSettingList);
			userExtendModel.UserId = userId;
			userExtendModel.DateCreated = DateTime.Now;
			userExtendModel.DateChanged = DateTime.Now;
			userExtendModel.LastChanged = "user";

			return userExtendModel;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザ拡張項目の選択値 ※キーはユーザ拡張項目IDです</summary>
		public UserExtendData UserExtendDataValue { get; set; }
		/// <summary>ユーザ拡張項目の選択値に紐づく表示名 ※キーはユーザ拡張項目IDです　※CheckBoxはカンマ連結</summary>
		public UserExtendData UserExtendDataText { get; set; }
		/// <summary>ユーザ拡張項目のカラムID一覧</summary>
		public List<string> UserExtendColumns { get; set; }
		/// <summary>ユーザー拡張項目設定一覧</summary>
		public UserExtendSettingList UserExtendSettings { get; set; }
		#endregion

		/// <summary>
		/// ユーザー拡張項目クラス
		/// </summary>
		[Serializable]
		public class UserExtendData : Dictionary<string, string>
		{
			/// <summary>
			/// デフォルトコンストラクタ
			/// </summary>
			public UserExtendData():base() {}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="dictionary">辞書</param>
			public UserExtendData(Dictionary<string, string> dictionary)
			{
				foreach (var key in dictionary.Keys)
				{
					this[key] = dictionary[key];
				}
			}
			/// <summary>
			/// デシリアライズコンストラクタ
			/// </summary>
			/// <param name="info">SerializationInfo</param>
			/// <param name="context">StreamingContext</param>
			public UserExtendData(SerializationInfo info, StreamingContext context): base(info, context)
			{
			}

			/// <summary>CROSS POINT ユーザー拡張項目(店舗カード番号)</summary>
			public string CrossPointShopCardNo
			{
				get { return this[Constants.CROSS_POINT_USREX_SHOP_CARD_NO]; }
				set { this[Constants.CROSS_POINT_USREX_SHOP_CARD_NO] = value; }
			}
			/// <summary>CROSS POINT ユーザー拡張項目(店舗カードPIN)</summary>
			public string CrossPointShopCardPin
			{
				get { return this[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN]; }
				set { this[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN] = value; }
			}
			/// <summary>CrossPoint連携　ユーザー拡張項目(アプリ会員フラグ)</summary>
			public string CrossPointShopAppMemberIdFlag
			{
				get { return this[Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG]; }
				set { this[Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG] = value; }
			}
			/// <summary>CROSS POINT ユーザー拡張項目(登録店舗名)</summary>
			public string CrossPointAddShopName
			{
				get { return this[Constants.CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME]; }
				set { this[Constants.CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME] = value; }
			}
			/// <summary>CROSS POINT ユーザー拡張項目(郵便DM不要フラグ)</summary>
			public string CrossPointDm
			{
				get { return this[Constants.CROSS_POINT_USREX_DM]; }
				set { this[Constants.CROSS_POINT_USREX_DM] = value; }
			}
			/// <summary>CROSS POINT ユーザー拡張項目(メールDM不要フラグ)</summary>
			public string CrossPointMailFlg
			{
				get { return this[Constants.CROSS_POINT_USREX_MAIL_FLG]; }
				set { this[Constants.CROSS_POINT_USREX_MAIL_FLG] = value; }
			}
		}
	}
}
