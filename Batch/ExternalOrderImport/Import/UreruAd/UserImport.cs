/*
=========================================================================================================
  Module      : つくーるAPI連携：ユーザー情報登録 (UserImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.User;
using w2.App.Common.Option;
using w2.App.Common.User;
using w2.Commerce.Batch.ExternalOrderImport.Entity;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalOrderImport.Import.UreruAd
{
	/// <summary>
	/// つくーるAPI連携：ユーザー情報登録
	/// </summary>
	public class UserImport : UreruAdImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseData">レスポンスデータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserImport(UreruAdResponseDataItem responseData, SqlAccessor accessor)
			: base(responseData, accessor)
		{
			this.UserCreditCardImport = new UserCreditCardImport(responseData, accessor);
		}

		/// <summary>
		/// 登録・更新
		/// </summary>
		public override void Import()
		{
			var registerUser = CreateImportData();

			UserModel correctionUserFromResponse;
			// this.ResponseData.User は注文情報を登録することに利用される
			// 一方で、ここで新しく作られたユーザーや一部更新すべき情報を更新したユーザーは登録・更新しなくてはならない
			if (this.IsNewUser)
			{
				// 新規ユーザーは補正情報で登録する必要がある
				correctionUserFromResponse = UpdateByImportResponse(registerUser);
				new UserService().Insert(correctionUserFromResponse, UpdateHistoryAction.DoNotInsert, this.Accessor);
			}
			else
			{
				// 既存ユーザーは一部データのみ更新した後、注文用に情報を更新する
				new UserService().Update(registerUser, UpdateHistoryAction.DoNotInsert, this.Accessor);
				correctionUserFromResponse = UpdateByImportResponse(registerUser);
			}
			this.ResponseData.User = correctionUserFromResponse;

			this.UserCreditCardImport.Import();

			if (this.IsNewUser)
			{
				var pointRules = PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER);
				foreach (var pointRule in pointRules)
				{
					new PointOptionUtility().InsertUserRegisterUserPoint(
						this.ResponseData.User.UserId,
						pointRule.PointRuleId,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.DoNotInsert);
				}
			}

			// 更新履歴登録
			new UpdateHistoryService().InsertForUser(this.ResponseData.User.UserId, Constants.FLG_LASTCHANGED_BATCH, this.Accessor);
		}

		/// <summary>
		/// インポートデータ生成
		/// </summary>
		/// <returns>ユーザー情報</returns>
		private UserModel CreateImportData()
		{
			var user = new UserService().GetUserByMailAddr(this.ResponseData.Email);
			this.IsNewUser = ((user == null)
				|| string.IsNullOrEmpty(this.ResponseData.Email)
				|| (user.Name != this.ResponseData.Name));

			var userModel = (this.IsNewUser == false) ? new UserModel(user.DataSource) : new UserModel();
			userModel.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
			userModel.DateLastLoggedin = this.ResponseData.Created;

			//定期注文が入金済みかどうかで判断するかどうかで制御が変わる
			if (userModel.FixedPurchaseMemberFlg != Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON)
			{
				if (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE)
				{
					// 入金済みを考慮する場合
					// 新規会員の場合、定期注文であれば定期会員フラグは寝かせる
					userModel.FixedPurchaseMemberFlg = Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF;
				}
				else
				{
					// 定期注文であれば定期会員フラグを立てる
					userModel.FixedPurchaseMemberFlg = this.ResponseData.IsFixedPurchase
						? Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON
						: Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF;
				}
			}

			var mailFlg = ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, this.ResponseData.MailOptinFlg);
			userModel.MailFlg = string.IsNullOrEmpty(mailFlg) ? Constants.FLG_USER_MAILFLG_OK : mailFlg;
			if (string.IsNullOrEmpty(userModel.AdvcodeFirst)) userModel.AdvcodeFirst = this.ResponseData.GetAdvCode();

			// これより先は新規顧客用のデータ
			if (this.IsNewUser == false) return userModel;

			userModel.LoginId = ((Constants.URERU_AD_IMPORT_DEFAULT_USER_KBN == Constants.FLG_URERU_AD_IMPORT_DEFAULT_USER_KBN_USER)
				&& Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
				? this.ResponseData.Email
				: string.Empty;
			userModel.MallId = Constants.FLG_USER_MALL_ID_URERU_AD;
			userModel.UserId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			userModel.MailAddr2 = string.Empty;
			userModel.Sex = ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, this.ResponseData.Sex);
			userModel.NickName = string.Empty;
			userModel.Tel2 = string.Empty;
			userModel.Tel2_1 = string.Empty;
			userModel.Tel2_2 = string.Empty;
			userModel.Tel2_3 = string.Empty;
			userModel.Tel3 = string.Empty;
			userModel.Tel3_1 = string.Empty;
			userModel.Tel3_2 = string.Empty;
			userModel.Tel3_3 = string.Empty;
			userModel.Fax = string.Empty;
			userModel.Fax_1 = string.Empty;
			userModel.Fax_2 = string.Empty;
			userModel.Fax_3 = string.Empty;
			userModel.CompanyName = string.Empty;
			userModel.CompanyPostName = string.Empty;
			userModel.CompanyExectiveName = string.Empty;
			userModel.Attribute1 = string.Empty;
			userModel.Attribute2 = string.Empty;
			userModel.Attribute3 = string.Empty;
			userModel.Attribute4 = string.Empty;
			userModel.Attribute5 = string.Empty;
			userModel.Attribute6 = string.Empty;
			userModel.Attribute7 = string.Empty;
			userModel.Attribute8 = string.Empty;
			userModel.Attribute9 = string.Empty;
			userModel.Attribute10 = string.Empty;
			userModel.Password = string.Empty;
			userModel.Question = string.Empty;
			userModel.Answer = string.Empty;
			userModel.CareerId = string.Empty;
			userModel.MobileUid = string.Empty;
			userModel.UserMemo = string.Empty;
			userModel.DelFlg = Constants.FLG_USER_DELFLG_UNDELETED;
			userModel.MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank();
			userModel.RecommendUid = string.Empty;
			userModel.UserManagementLevelId = Constants.FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL;
			userModel.IntegratedFlg = Constants.FLG_USER_INTEGRATED_FLG_NONE;
			userModel.EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL;

			// 広告コードより補正
			UserUtility.CorrectUserByAdvCode(userModel);

			return userModel;
		}

		/// <summary>
		/// レスポンス情報に基づいてユーザー情報を補正する
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <returns>補正後のユーザー</returns>
		private UserModel UpdateByImportResponse(UserModel user)
		{
			// 氏名の読み仮名判定(ひらがな/カタカナ)
			string nameKana, nameKana1, nameKana2;
			switch (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana.type@@"))
			{
				case Validator.STRTYPE_FULLWIDTH_HIRAGANA:
					nameKana = this.ResponseData.Kana;
					nameKana1 = this.ResponseData.FamilyKana;
					nameKana2 = this.ResponseData.GivenKana;
					break;

				case Validator.STRTYPE_FULLWIDTH_KATAKANA:
					nameKana = this.ResponseData.Katakana;
					nameKana1 = this.ResponseData.FamilyKatakana;
					nameKana2 = this.ResponseData.GivenKatakana;
					break;

				default:
					nameKana = this.ResponseData.Kana;
					nameKana1 = this.ResponseData.FamilyKana;
					nameKana2 = this.ResponseData.GivenKana;
					break;
			}

			if (this.IsNewUser)
			{
				user.UserKbn = ValueText.GetValueText(
					Constants.TABLE_USER,
					(Constants.URERU_AD_IMPORT_DEFAULT_USER_KBN == Constants.FLG_URERU_AD_IMPORT_DEFAULT_USER_KBN_USER)
						? Constants.FIELD_USER_USER_KBN
						: Constants.FIELD_USER_USER_KBN + "_guest",
						this.ResponseData.Type.ToUpper());
			}
			else
			{
				user.UserKbn = ValueText.GetValueText(
					Constants.TABLE_USER,
					Constants.FIELD_USER_USER_KBN,
					this.ResponseData.Type.ToUpper());
			}

			user.Name = this.ResponseData.Name;
			user.Name1 = this.ResponseData.FamilyName;
			user.Name2 = this.ResponseData.GivenName;
			user.NameKana = nameKana;
			user.NameKana1 = nameKana1;
			user.NameKana2 = nameKana2;
			user.MailAddr = this.ResponseData.Email;
			user.Zip = this.ResponseData.ZipFullHyphen;
			user.Zip1 = this.ResponseData.Zip1;
			user.Zip2 = this.ResponseData.Zip2;
			user.Addr = this.ResponseData.AddressFull;
			user.Addr1 = this.ResponseData.Prefecture;
			user.Addr2 = this.ResponseData.Address1Zenkaku;
			user.Addr3 = this.ResponseData.Address2Zenkaku;
			user.Addr4 = this.ResponseData.Address3Zenkaku;
			user.Tel1 = this.ResponseData.TelNoFullHyphen;
			user.Tel1_1 = this.ResponseData.TelNo1;
			user.Tel1_2 = this.ResponseData.TelNo2;
			user.Tel1_3 = this.ResponseData.TelNo3;
			user.Birth = this.ResponseData.Birthday;
			user.BirthYear = this.ResponseData.Birthday.HasValue ? this.ResponseData.Birthday.Value.ToString("yyyy") : string.Empty;
			user.BirthMonth = this.ResponseData.Birthday.HasValue ? this.ResponseData.Birthday.Value.ToString("MM") : string.Empty;
			user.BirthDay = this.ResponseData.Birthday.HasValue ? this.ResponseData.Birthday.Value.ToString("dd") : string.Empty;
			user.LoginId = this.ResponseData.Email;
			user.RemoteAddr = this.ResponseData.IpAddress;

			user.Sex = ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, this.ResponseData.Sex);

			return user;
		}

		#region プロパティ
		/// <summary>新規ユーザー？</summary>
		private bool IsNewUser { get; set; }
		/// <summary>ユーザークレジットカード情報登録クラス</summary>
		private UserCreditCardImport UserCreditCardImport { get; set; }
		#endregion
	}
}