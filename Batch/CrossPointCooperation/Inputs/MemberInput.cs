/*
=========================================================================================================
  Module      : Member input (MemberInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Domain.Point;
using w2.Domain.User;

namespace w2.Commerce.Batch.CrossPointCooperation.Inputs
{
	/// <summary>
	/// Member input
	/// </summary>
	public class MemberInput : InputBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		public MemberInput(Hashtable source)
			: base(source)
		{
			this.DataSource = source;
		}

		/// <summary>
		/// Create user for update
		/// </summary>
		/// <param name="oldUser">Old user</param>
		/// <returns>User for update</returns>
		public UserModel CreateUserForUpdate(UserModel oldUser)
		{
			var user = new UserModel(oldUser.DataSource);
			return user;
		}

		/// <summary>
		/// Create user point for update
		/// </summary>
		/// <param name="oldUserPoint">Old user point</param>
		/// <returns>User point for update</returns>
		public UserPointModel CreateUserForUpdate(UserPointModel oldUserPoint)
		{
			var userPoint = new UserPointModel(oldUserPoint.DataSource);
			return userPoint;
		}

		/// <summary>会員ID</summary>
		public string MemberId
		{
			get { return this.DataSource["member_id"].ToString(); }
		}
		/// <summary>入会ショップコード</summary>
		public string AdmissionShopCd
		{
			get { return this.DataSource["admissioin_shop_cd"].ToString(); }
		}
		/// <summary>入会ショップ名</summary>
		public string AdmissionShopName
		{
			get { return this.DataSource["admission_shop_name"].ToString(); }
		}
		/// <summary>店舗カード番号</summary>
		public string RealShopCardNo
		{
			get { return this.DataSource["real_shop_card_no"].ToString(); }
		}
		/// <summary>PINコード</summary>
		public string PinCd
		{
			get { return this.DataSource["pin_cd"].ToString(); }
		}
		/// <summary>店舗カード番号登録日</summary>
		public string RealShopCardNoInsertDate
		{
			get { return this.DataSource["real_shop_card_no_insert_date"].ToString(); }
		}
		/// <summary>ネットショップ名会員ID</summary>
		public string NetShopMemberIdShopCd
		{
			get { return this.DataSource["net_shop_member_id_" + App.Common.Constants.CROSS_POINT_AUTH_SHOP_CODE.ToLower()].ToString(); }
		}
		/// <summary>ネットショップ名会員登録日</summary>
		public string NetShopMemberIdShopCdInsertDate
		{
			get { return this.DataSource["net_shop_member_id_" + App.Common.Constants.CROSS_POINT_AUTH_SHOP_CODE.ToLower() + "_insert_date"].ToString(); }
		}
		/// <summary>氏</summary>
		public string LastName
		{
			get { return this.DataSource["last_name"].ToString(); }
		}
		/// <summary>名</summary>
		public string FirstName
		{
			get { return this.DataSource["first_name"].ToString(); }
		}
		/// <summary>氏（カナ）</summary>
		public string LastNameKana
		{
			get { return this.DataSource["last_name_kana"].ToString(); }
		}
		/// <summary>名（カナ）</summary>
		public string FirstNameKana
		{
			get { return this.DataSource["first_name_kana"].ToString(); }
		}
		/// <summary>性別</summary>
		public string Sex
		{
			get { return this.DataSource["sex"].ToString(); }
		}
		/// <summary>生年月日</summary>
		public string Birthday
		{
			get { return this.DataSource["birthday"].ToString(); }
		}
		/// <summary>郵便番号</summary>
		public string PostCode
		{
			get { return this.DataSource["postcode"].ToString(); }
		}
		/// <summary>都道府県名</summary>
		public string PrefName
		{
			get { return this.DataSource["pref_name"].ToString(); }
		}
		/// <summary>市区町村</summary>
		public string City
		{
			get { return this.DataSource["city"].ToString(); }
		}
		/// <summary>町域</summary>
		public string Town
		{
			get { return this.DataSource["town"].ToString(); }
		}
		/// <summary>番地</summary>
		public string Address
		{
			get { return this.DataSource["address"].ToString(); }
		}
		/// <summary>ビル等</summary>
		public string Building
		{
			get { return this.DataSource["building"].ToString(); }
		}
		/// <summary>電話番号</summary>
		public string Tel
		{
			get { return this.DataSource["tel"].ToString(); }
		}
		/// <summary>携帯電話</summary>
		public string MbTel
		{
			get { return this.DataSource["mb_tel"].ToString(); }
		}
		/// <summary>PCメールアドレス</summary>
		public string PcMail
		{
			get { return this.DataSource["pc_mail"].ToString(); }
		}
		/// <summary>携帯メールアドレス</summary>
		public string MbMail
		{
			get { return this.DataSource["mb_mail"].ToString(); }
		}
		/// <summary>会員ランクID</summary>
		public string MemberRankId
		{
			get { return this.DataSource["member_rank_id"].ToString(); }
		}
		/// <summary>郵便DM不要</summary>
		public string PostcardDmUnnecessaryFlg
		{
			get { return this.DataSource["postcard_dm_unnecessary_flg"].ToString(); }
		}
		/// <summary>メールDM不要</summary>
		public string EmailDmUnnecessaryFlg
		{
			get { return this.DataSource["email_dm_unnecessary_flg"].ToString(); }
		}
		/// <summary>ブラックフラグ</summary>
		public string BlackFlg
		{
			get { return this.DataSource["black_flg"].ToString(); }
		}
		/// <summary>有効ポイント</summary>
		public string EffectivePoint
		{
			get { return this.DataSource["effective_point"].ToString(); }
		}
		/// <summary>現時点仮付与ポイント</summary>
		public string TempGrantPoint
		{
			get { return this.DataSource["temp_grant_point"].ToString(); }
		}
	}
}