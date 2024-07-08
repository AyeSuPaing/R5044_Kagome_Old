/*
=========================================================================================================
  Module      : ユーザマスタモデル (UserModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.MallCooperationSetting;
using w2.Domain.User.Helper;
using w2.Domain.UserCreditCard;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザマスタモデル
	/// </summary>
	public partial class UserModel
	{
		#region メソッド
		/// <summary>
		/// ユーザー拡張項目取得
		/// </summary>
		/// <returns>モデル</returns>
		public UserExtendModel GetUserExtend()
		{
			return new UserService().GetUserExtend(this.UserId);
		}

		/// <summary>
		/// データ補正
		/// </summary>
		public void Corrected()
		{
			// 性別補正
			switch (this.Sex)
			{
				case Constants.FLG_USER_SEX_MALE:
				case Constants.FLG_USER_SEX_FEMALE:
					break;

				default:
					this.Sex = Constants.FLG_USER_SEX_UNKNOWN;
					break;
			}
		}

		/// <summary>
		/// ユーザ名がブランクの場合の補完ユーザ名を生成します。
		/// </summary>
		/// <param name="userName">ユーザ名</param>
		/// <param name="mailAddr1">PCメールアドレス</param>
		/// <param name="mailAddr2">モバイルメールアドレス</param>
		/// <returns>補完ユーザ名</returns>
		public static string CreateComplementUserName(
			string userName,
			string mailAddr1,
			string mailAddr2)
		{
			if (string.IsNullOrEmpty(userName) == false) return userName;

			// PCメールアドレス＞モバイルメールアドレス
			var mailAddr = (string.IsNullOrEmpty(mailAddr1) == false) ? mailAddr1 : mailAddr2;

			var completionText = Constants.USER_NAME_COMPLETION_TEXT;
			completionText = completionText.Replace("@@MailAddress@@", mailAddr);
			completionText = completionText.Replace("@@MailAddressBeforeAtMark@@", mailAddr.Split('@').First());
			return completionText;
		}

		/// <summary>
		/// モール名取得
		/// </summary>
		/// <returns>モール名</returns>
		public string GetMallName()
		{
			if (m_mallName == null)
			{
				var mall = new MallCooperationSettingService().Get(Constants.CONST_DEFAULT_SHOP_ID, this.MallId);
				m_mallName = (mall != null) ? mall.MallName : "";
			}
			return m_mallName;
		}
		private string m_mallName = null;

		/// <summary>
		/// <para>住所比較</para>
		/// <para>配送情報が持つ住所と比較する</para>
		/// </summary>
		/// <param name="compareTargetModel">比較対象となる配送情報モデル</param>
		/// <param name="isShippingAddrJp">配送先住所が日本か</param>
		/// <returns>配送情報と住所が一致する</returns>
		public bool IsSameAddress(FixedPurchaseShippingModel compareTargetModel, bool isShippingAddrJp)
		{
			var isSame = true;

			isSame &= (this.Name == compareTargetModel.ShippingName);
			isSame &= (this.Name1 == compareTargetModel.ShippingName1);
			isSame &= (this.Name2 == compareTargetModel.ShippingName2);
			isSame &= (this.Zip == compareTargetModel.ShippingZip);
			isSame &= (this.Addr2 == compareTargetModel.ShippingAddr2);
			isSame &= (this.Addr3 == compareTargetModel.ShippingAddr3);
			isSame &= (this.Addr4 == compareTargetModel.ShippingAddr4);
			isSame &= (this.Tel1 == compareTargetModel.ShippingTel1);

			if (isShippingAddrJp)
			{
				isSame &= (this.NameKana == compareTargetModel.ShippingNameKana);
				isSame &= (this.NameKana1 == compareTargetModel.ShippingNameKana1);
				isSame &= (this.NameKana2 == compareTargetModel.ShippingNameKana2);
				isSame &= (this.Addr1 == compareTargetModel.ShippingAddr1);
			}
			else
			{
				isSame &= (this.Addr5 == compareTargetModel.ShippingAddr5);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				isSame &= (this.AddrCountryIsoCode == compareTargetModel.ShippingCountryIsoCode);
			}

			return isSame;
		}

		/// <summary>
		/// <para>住所セット</para>
		/// </summary>
		/// <param name="shipping">住所情報</param>
		public void SetAddress(FixedPurchaseShippingModel shipping)
		{
			this.Name = shipping.ShippingName;
			this.Name1 = shipping.ShippingName1;
			this.Name2 = shipping.ShippingName2;
			this.NameKana = shipping.ShippingNameKana;
			this.NameKana1 = shipping.ShippingNameKana1;
			this.NameKana2 = shipping.ShippingNameKana2;
			this.Zip = shipping.ShippingZip;
			this.Addr = shipping.ShippingAddr;
			this.Addr1 = shipping.ShippingAddr1;
			this.Addr2 = shipping.ShippingAddr2;
			this.Addr3 = shipping.ShippingAddr3;
			this.Addr4 = shipping.ShippingAddr4;
			this.Addr5 = shipping.ShippingAddr5;
			this.Tel1 = shipping.ShippingTel1;
			this.AddrCountryIsoCode = shipping.ShippingCountryIsoCode;

			// Zip1、Zip2を作成しセットする
			var splitedZip = shipping.ShippingZip.Split('-');
			if (splitedZip.Length == 2)
			{
				this.Zip1 = splitedZip[0];
				this.Zip2 = splitedZip[1];
			}

			// Tel1_1、Tel1_2、Tel1_3を作成しセットする
			var splitTel1 = shipping.ShippingTel1.Split('-');
			if (splitTel1.Length != 3) return;
			this.Tel1_1 = splitTel1[0];
			this.Tel1_2 = splitTel1[1];
			this.Tel1_3 = splitTel1[2];
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザー拡張項目モデル</summary>
		private UserExtendModel m_userExtendModel;
		/// <summary>
		/// ユーザー拡張項目取得
		/// </summary>
		/// <returns>モデル</returns>
		public UserExtendModel UserExtend
		{
			get
			{
				return m_userExtendModel;
			}
			set
			{
				m_userExtendModel = value;
			}
		}
		/// <summary>
		///  登録済みのユーザかどうか
		/// </summary>
		public bool IsRegisted { get; set; }
		/// <summary>
		/// 拡張項目_顧客区分分テキスト
		/// </summary>
		public string UserKbnText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, this.UserKbn);
			}
		}
		/// <summary>
		/// 拡張項目_退会ユーザーか判定
		/// </summary>
		public bool IsDeleted
		{
			get { return (this.DelFlg == Constants.FLG_USER_DELFLG_DELETED); }
		}
		/// <summary>
		/// 拡張項目_会員か判定
		/// </summary>
		public bool IsMember
		{
			get
			{
				switch (this.UserKbn)
				{
					case Constants.FLG_USER_USER_KBN_PC_USER:
					case Constants.FLG_USER_USER_KBN_MOBILE_USER:
					case Constants.FLG_USER_USER_KBN_SMARTPHONE_USER:
					case Constants.FLG_USER_USER_KBN_OFFLINE_USER:
						return true;

					default:
						return false;
				}
			}
		}
		/// <summary>
		/// 拡張項目_性別テキスト
		/// </summary>
		public string SexText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, this.Sex);
			}
		}
		/// <summary>
		/// 拡張項目_メール配信フラグテキスト
		/// </summary>
		public string MailFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, this.MailFlg);
			}
		}
		/// <summary>
		/// 拡張項目_メール配信OKか判定
		/// </summary>
		public bool IsSendMail
		{
			get { return (this.MailFlg == Constants.FLG_USER_MAILFLG_OK); }
		}
		/// <summary>
		/// 拡張項目_統合済みユーザーか判定
		/// </summary>
		public bool IsIntegrated
		{
			get { return (this.IntegratedFlg == Constants.FLG_USER_INTEGRATED_FLG_DONE); }
		}
		/// <summary>決済カード連携情報が存在する?</summary>
		public bool HasCreditCards
		{
			get { return (this.UserCreditCards.Length > 0); }
		}
		/// <summary>決済カード連携情報リスト</summary>
		public UserCreditCardModel[] UserCreditCards
		{
			get { return (UserCreditCardModel[])this.DataSource["EX_UserCreditCards"]; }
			set { this.DataSource["EX_UserCreditCards"] = value; }
		}
		/// <summary>暗号化されていないパスワード</summary>
		public string PasswordDecrypted
		{
			get { return string.IsNullOrEmpty(this.Password) ? "" : UserPassowordCryptor.PasswordDecrypt(this.Password); }
			set { this.Password = string.IsNullOrEmpty(value) ? "" : UserPassowordCryptor.PasswordEncrypt(value); }
		}
		/// <summary>ユーザ名がブランクの場合の補完ユーザ名</summary>
		public string ComplementUserName
		{
			get { return CreateComplementUserName(this.Name1 + this.Name2, this.MailAddr, this.MailAddr2); }
		}
		/// <summary>定期会員か？</summary>
		public bool IsFixedPurchaseMember
		{
			get { return (this.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON); }
		}
		/// <summary>Default Convenience Store Id</summary>
		public string DefaultConvenienceStoreId { get; set; }
		/// <summary>Default Convenience Store Use Flg</summary>
		public string DefaultConvenienceStoreUseFlg { get; set; }
		/// <summary>Default Shipping Receiving Store Type</summary>
		public string DefaultShippingReceivingStoreType { get; set; }
		#endregion
	}
}
