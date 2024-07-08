/*
=========================================================================================================
  Module      : ユーザリポジトリ (UserRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Coupon.Helper;
using w2.Domain.Point;
using w2.Domain.TwUserInvoice;
using w2.Domain.User.Helper;
using w2.Domain.UserCreditCard;
using w2.Domain.UserEasyRegisterSetting;
using w2.Domain.UserExtendSetting;
using w2.Domain.UserShipping;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザリポジトリ
	/// </summary>
	public class UserRepository : RepositoryBase
	{
		/// <summary>ユーザーSQLファイル</summary>
		private const string XML_KEY_NAME = "User";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ユーザーマスタ
		/// <summary>
		/// ユーザー検索結果数取得
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>検索結果件数</returns>
		public int GetSearchHitCount(UserSearchCondition cond)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetSearchHitCount",
				cond.CreateHashtableParams(),
				replaces: CreateReplaceKvpsForSearch(cond));
			return (int)dv[0][0];
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>検索結果（0件の場合は0配列）</returns>
		public UserSearchResult[] Search(UserSearchCondition cond)
		{
			var dv = Get(
				XML_KEY_NAME,
				"Search",
				cond.CreateHashtableParams(),
				replaces: CreateReplaceKvpsForSearch(cond));
			return dv.Count == 0
						? new UserSearchResult[] { }
						: dv.Cast<DataRowView>().Select(drv => new UserSearchResult(drv)).ToArray();
		}

		/// <summary>
		/// 検索向けキーバリューペア作成
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>変換キーペア</returns>
		private KeyValuePair<string, string>[] CreateReplaceKvpsForSearch(UserSearchCondition cond)
		{
			var replace = new[]
			{
				new KeyValuePair<string, string>(
					"@@ user_extend_field_name @@",
					string.Format(
						"{0}.{1}",
						Constants.TABLE_USEREXTEND,
						string.IsNullOrEmpty(cond.UserExtendName)
							? Constants.FIELD_USEREXTEND_USER_ID
							: cond.UserExtendName))
			};
			return replace;
		}

		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, statementName, input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="masterKbn"></param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;

			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING: // ユーザ配送先マスタ表示
					statement = "CheckUserShippingFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE: // ユーザー電子発票管理マスタ
					statement = "CheckUserInvoiceFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER: // ユーザーマスタ表示
					statement = "CheckFieldsForGetMaster";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		public UserModel Get(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "Get", input);
			if (dv.Count == 0) return null;
			return new UserModel(dv[0]);
		}

		/// <summary>
		/// ユーザー情報の件数取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>件数</returns>
		public int Count(string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
			};
			var dv = Get(XML_KEY_NAME, "Count", ht);
			return (int)dv[0][0];
		}

		/// <summary>
		/// 複数ユーザ取得
		/// </summary>
		/// <param name="userIds">ユーザIDの列挙</param>
		/// <returns>モデル</returns>
		public IEnumerable<UserModel> GetUsers(IEnumerable<string> userIds)
		{
			// 大量のパラメータ数があるとSQL文が長くなりエラーとなるため分割する
			foreach (var chunkIds in userIds.Distinct().Chunk(20000))
			{
				var ids = string.Join(",", chunkIds.Select(id => string.Format("'{0}'", id)));
				var replace = new KeyValuePair<string, string>("@@ user_ids @@", ids);
				var userDatas = GetByDataReader(XML_KEY_NAME, "GetUsers", null, replace);
				foreach (var ht in userDatas)
				{
					yield return new UserModel(ht);
				}
			}
		}

		/// <summary>
		/// ユーザー情報を拡張項目の値から取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <returns>モデル</returns>
		public UserModel GetByExtendColumn(string columnName, string value)
		{
			var users = GetUsersByExtendColumn(columnName, value);
			return users.FirstOrDefault();
		}

		/// <summary>
		/// 拡張項目の値が一致するユーザー一覧を取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <returns>モデル配列</returns>
		internal UserModel[] GetUsersByExtendColumn(string columnName, string value)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetByExtendColumn",
				replaces: new[]
				{
					new KeyValuePair<string, string>("@@ column_name @@", columnName),
					new KeyValuePair<string, string>("@@ value @@", value.Replace("'", "''")),
				});
			return dv.Cast<DataRowView>().Select(drv => new UserModel(drv)).ToArray();
		}

		/// <summary>
		/// 退会していないユーザー情報を拡張項目の値から取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <returns>モデル</returns>
		public UserModel GetRegisteredUserByExtendColumn(string columnName, string value)
		{
			var dv = Get(XML_KEY_NAME, "GetRegisteredUserByExtendColumn", replaces: new[]
			{
				new KeyValuePair<string, string>("@@ column_name @@", columnName),
				new KeyValuePair<string, string>("@@ value @@", value.Replace("'", "''")),
			});
			return (dv.Count != 0) ? new UserModel(dv[0]) : null;
		}

		/// <summary>
		/// 会員ランクID取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>会員ランクID</returns>
		internal string GetMemberRankId(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetMemberRankId", input);
			return (dv.Count != 0) ? (string)dv[0][0] : null;
		}

		/// <summary>
		/// メール配信のためユーザ情報取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>メール送信のためユーザ情報クラス</returns>
		public UserForMailSend GetUserForMailSend(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserForMailSend", input);
			if (dv.Count == 0) return null;
			return new UserForMailSend(dv[0]);
		}

		/// <summary>
		/// マスタアップロードバッチ用
		/// ログインID重複ユーザ情報取得
		/// </summary>
		/// <returns>ログインID重複ユーザモデル列</returns>
		public DuplicationLoginId[] GetDuplicationLoginIdList()
		{
			var dv = Get(XML_KEY_NAME, "GetDuplicationLoginIdList", null);
			return dv.Cast<DataRowView>().Select(drv => new DuplicationLoginId(drv)).ToArray();
		}

		/// <summary>
		/// ユーザーシンボル取得
		/// </summary>
		/// <param name="userIds">ユーザーIDリスト</param>
		/// <returns>ユーザーシンボルリスト（0件の場合は0配列）</returns>
		public UserSymbols[] GetUserSymbols(params string[] userIds)
		{
			string input = string.Join(",", userIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ params @@", input);
			var dv = Get(XML_KEY_NAME, "GetUserSymbols", null, replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new UserSymbols(drv)).ToArray();

		}

		/// <summary>
		/// ログインIDとパスワードでユーザ取得
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="passwordEncrypt">パスワード</param>
		/// <returns>モデル</returns>
		public UserModel UserLogin(string loginId, string passwordEncrypt)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_LOGIN_ID, loginId},
				{Constants.FIELD_USER_PASSWORD, passwordEncrypt},
			};
			var dv = Get(XML_KEY_NAME, "UserLogin", input);
			return (dv.Count != 0) ? new UserModel(dv[0]) : null;
		}

		/// <summary>
		/// ログインIDとパスワードでユーザ取得
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="passwordEncrypt">パスワード</param>
		/// <returns>モデル</returns>
		public UserModel UserLoginUsedMailAddress(string loginId, string passwordEncrypt)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_LOGIN_ID, loginId},
				{Constants.FIELD_USER_PASSWORD, passwordEncrypt},
			};
			var dv = Get(XML_KEY_NAME, "UserLoginUsedMailAddress", input);
			return (dv.Count != 0) ? new UserModel(dv[0]) : null;
		}

		/// <summary>
		/// メールアドレスでユーザー情報取得
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル配列</returns>
		public UserModel[] GetUsersByMailAddr(string mailAddr)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_MAIL_ADDR, mailAddr},
			};
			var dv = Get(XML_KEY_NAME, "GetUserByMailAddr", input);
			return dv.Cast<DataRowView>().Select(drv => new UserModel(drv)).ToArray();
		}

		/// <summary>
		/// ログインIDでユーザー情報取得
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>モデル配列</returns>
		public UserModel[] GetUserByLoginId(string loginId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_LOGIN_ID, loginId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserByLoginId", input);
			return dv.Cast<DataRowView>().Select(drv => new UserModel(drv)).ToArray();
		}

		/// <summary>
		/// メールアドレスでメールマガジン登録用のユーザー情報取得（ゲスト会員は含めない）
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		public UserModel GetUserByMailAddrForMailMagazineRegister(string mailAddr)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_MAIL_ADDR, mailAddr},
				{Constants.FIELD_USER_MALL_ID, Constants.FLG_USER_MALL_ID_OWN_SITE},
			};
			var dv = Get(XML_KEY_NAME, "GetUserByMailAddrForMailMagazineRegister", input);
			if (dv.Count == 0) return null;
			return new UserModel(dv[0]);
		}

		/// <summary>
		/// メールアドレスでメールマガジン解除用のユーザー情報取得（会員区分に関わらず取得する）
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		public UserModel GetUserByMailAddrForMailMagazineCancel(string mailAddr)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_MAIL_ADDR, mailAddr },
			};
			var dv = Get(
				XML_KEY_NAME,
				"GetUserByMailAddrForMailMagazineCancel",
				input,
				null,
				new KeyValuePair<string, string>(
					"@@ mall_ids @@",
					new[]
					{
						Constants.FLG_USER_MALL_ID_OWN_SITE,
						Constants.FLG_USER_MALL_ID_URERU_AD,
					}.JoinToString("','")));
			return (dv.Count != 0) ? new UserModel(dv[0]) : null;
		}

		/// <summary>
		/// メールアドレスでユーザー情報取得 (AmazonPay用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		public UserModel[] GetRegisteredUserByMailAddress(string mailAddr)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_MAIL_ADDR, mailAddr},
			};
			var dv = Get(XML_KEY_NAME, "GetRegisteredUserByMailAddress", input);
			return dv.Cast<DataRowView>().Select(drv => new UserModel(drv)).ToArray();
		}

		/// <summary>
		/// ユーザID取得（モールID、メールアドレスから）
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>ユーザID</returns>
		public string GetUserId(string mallId, string mailAddr)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_MALL_ID, mallId},
				{Constants.FIELD_USER_MAIL_ADDR, mailAddr},
			};
			var dv = Get(XML_KEY_NAME, "GetUserId", input);
			if (dv.Count == 0) return null;
			return (string)dv[0][Constants.FIELD_USER_USER_ID];
		}

		/// <summary>
		/// パスワードリマインダー用ユーザ情報取得
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="mailaddressEnabled">メールアドレスのログインID利用有無</param>
		/// <returns>モデル</returns>
		public UserModel GetUserForReminder(string loginId, string mailAddr, bool mailaddressEnabled)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_LOGIN_ID, loginId},
				{Constants.FIELD_USER_MAIL_ADDR, mailAddr},
			};
			string strStatement = mailaddressEnabled ? "PasswordReminderUsedMailAddress" : "PasswordReminder";
			var dv = Get(XML_KEY_NAME, strStatement, input);
			if (dv.Count != 0)
			{
				return Get((string)dv[0][Constants.FIELD_USER_USER_ID]);
			}
			return null;
		}

		/// <summary>
		/// ユーザーを関連テーブルとともに取得（更新履歴で利用）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザー関連データ</returns>
		public UserRelationDatas GetWithUserRelationDatas(string userId, SqlAccessor accessor = null)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
				{Constants.FIELD_USERCOUPON_DEPT_ID, Constants.CONST_DEFAULT_DEPT_ID},
			};

			var ds = GetWithChilds(
				new[]
				{
					new KeyValuePair<string, string>(XML_KEY_NAME, "Get"),
					new KeyValuePair<string, string>(XML_KEY_NAME, "GetUserExtend"),
					new KeyValuePair<string, string>("UserCreditCard", "GetUsable"),
					new KeyValuePair<string, string>("Point", "GetUserPoint"),
					new KeyValuePair<string, string>("Coupon", "GetUserCoupons"),
					new KeyValuePair<string, string>("UserShipping", "GetAll"),
					new KeyValuePair<string, string>("TwUserInvoice", "GetAllUserInvoiceByUserId"),
				},
				input);

			var userRelationData = new UserRelationDatas();

			var user = (ds.Tables[0].DefaultView.Count != 0) ? new UserModel(ds.Tables[0].DefaultView[0]) : null;
			if (user == null) return userRelationData;

			var userExendSettingList = new UserExtendSettingList(false, "", new UserService().GetUserExtendSettingArray());
			user.UserExtend = (ds.Tables[1].DefaultView.Count != 0)
				? new UserExtendModel(ds.Tables[1].DefaultView[0], userExendSettingList)
				: new UserExtendModel(userExendSettingList);
			user.UserCreditCards =
				ds.Tables[2].DefaultView.Cast<DataRowView>().Select(drv => new UserCreditCardModel(drv)).ToArray();
			userRelationData.User = user;

			userRelationData.UserPoint = ds.Tables[3].DefaultView.Cast<DataRowView>().Select(drv => new UserPointModel(drv)).ToList();
			userRelationData.UserCouponDetail = ds.Tables[4].DefaultView.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToList();
			userRelationData.UserShipping =
				ds.Tables[5].DefaultView.Cast<DataRowView>()
					.Select(drv => new UserShippingModel(drv))
					.OrderByDescending(m => m.ShippingNo)
					.ToList();

			userRelationData.UserInvoice =
				ds.Tables[6].DefaultView.Cast<DataRowView>()
					.Select(row => new TwUserInvoiceModel(row))
					.OrderByDescending(item => item.TwInvoiceNo)
					.ToList();

			return userRelationData;
		}

		/// <summary>
		/// Get Modify User Ids
		/// </summary>
		/// <param name="updateAt">Update At</param>
		/// <param name="limit">Limit</param>
		/// <param name="offset">Offset</param>
		/// <param name="sortType">Sort Type</param>
		/// <returns>Modify User Ids</returns>
		public string[] GetModifyUsers(DateTime updateAt, int limit, int offset, string sortType)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_DATE_CHANGED, updateAt },
				{ "limit", limit },
				{ "offset", offset },
				{ "sort_type", sortType }
			};
			var data = Get(XML_KEY_NAME, "GetModifyUsers", input);
			var result = data.Cast<DataRowView>()
				.Select(row => row[0].ToString())
				.ToArray();
			return result;
		}

		/// <summary>
		/// Get users for Letro
		/// </summary>
		/// <param name="userIds">User ids</param>
		/// <returns>Users for Letro</returns>
		public DataView GetUsersForLetro(IEnumerable<string> userIds)
		{
			var ids = string.Join(",", userIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ user_ids @@", ids);
			var results = Get(
				XML_KEY_NAME,
				"GetUsersForLetro",
				input: null,
				replaces: replace);
			return results;
		}

		/// <summary>
		/// ログインＩＤ重複チェック（登録用）
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="mailAddressEnabled">メールアドレスのログインID利用有無（1:有効 、0:無効）</param>
		/// <returns>登録件数</returns>
		public int CheckDuplicationLoginIdRegist(string loginId, string mailAddressEnabled)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_LOGIN_ID, loginId},
				{"login_id_use_mailaddress_flg", mailAddressEnabled},
			};
			var dv = Get(XML_KEY_NAME, "CheckDuplicationLoginIdRegist", input);
			if (dv.Count != 0)
			{
				return (int)dv[0]["count"];
			}
			return 0;
		}

		/// <summary>
		/// ログインＩＤ重複チェック（更新用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="mailaddressEnabled">メールアドレスのログインID利用有無（1:有効 、0:無効）</param>
		/// <returns>登録件数</returns>
		public int CheckDuplicationLoginIdModify(string userId, string loginId, string mailaddressEnabled)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
				{Constants.FIELD_USER_LOGIN_ID, loginId},
				{"login_id_use_mailaddress_flg", mailaddressEnabled},
			};
			var dv = Get(XML_KEY_NAME, "CheckDuplicationLoginIdModify", input);
			if (dv.Count != 0)
			{
				return (int)dv[0]["count"];
			}
			return 0;
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(UserModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}

		/// <summary>
		/// モバイルID更新
		/// </summary>
		/// <param name="input">データ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateMobileUID(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "UpdateMobileUID", input);
			return result;
		}

		/// <summary>
		/// 退会処理
		/// </summary>
		/// <param name="input">データ</param>
		/// <returns>影響を受けた件数</returns>
		public int UserWithdrawal(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "UserWithdrawal", input);
			return result;
		}

		/// <summary>
		/// メール送信フラグ更新
		/// </summary>
		/// <param name="remoteAddr">リモートIPアドレス</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="mailFlg">メールフラグ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateMailFlg(string remoteAddr, string mailAddr, string mailFlg)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_REMOTE_ADDR, remoteAddr},
				{Constants.FIELD_USER_MAIL_ADDR, mailAddr},
				{Constants.FIELD_USER_MAIL_FLG, mailFlg},
				{Constants.FIELD_USER_MALL_ID, Constants.FLG_USER_MALL_ID_OWN_SITE},
			};
			var result = Exec(XML_KEY_NAME, "UpdateMailFlg", input);
			return result;
		}

		/// <summary>
		/// メルマガ解除
		/// </summary>
		/// <param name="input">データ</param>
		/// <returns>影響を受けた件数</returns>
		public int MailMagazineCancel(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "MailMagazineCancel", input);
			return result;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
		}

		/// <summary>
		/// 外部ユーザーIDでユーザー情報取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="externalUserId">外部ユーザーID</param>
		/// <returns>ユーザーモデル</returns>
		public UserModel GetUserByExternalUserId(string columnName, string externalUserId)
		{
			var replace = new KeyValuePair<string, string>("@@ ExtendColumn @@", Constants.TABLE_USEREXTEND + "." + columnName);
			var input = new Hashtable
			{
				{"external_user_id", externalUserId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserByExternalUserId", input, replaces: replace);
			if (dv.Count == 0) return null;
			return new UserModel(dv[0]);
		}

		/// <summary>
		/// 最終誕生日ポイント付与年更新
		/// </summary>
		/// <param name="user">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateUserBirthdayPointAddYear(UserModel user)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUserBirthdayPointAddYear", user.DataSource);
			return result;
		}

		/// <summary>
		/// 誕生日クーポン発行年更新
		/// </summary>
		/// <param name="user">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateUserBirthdayCouponPublishYear(UserModel user)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUserBirthdayCouponPublishYear", user.DataSource);
			return result;
		}

		/// <summary>
		/// 誕生日クーポン発行年更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateUserBirthdayCouponPublishYearByCouponInfo(string userId, string lastChanged)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
				{Constants.FIELD_USER_LAST_CHANGED, lastChanged},
			};

			var result = Exec(XML_KEY_NAME, "UpdateUserBirthdayCouponPublishYearByCouponInfo", ht);
			return result;
		}

		/// <summary>
		/// リアルタイム累計購入回数更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="realTimeOrderCount">累計購入回数</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateRealTimeOrderCount(string userId, int realTimeOrderCount)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
				{Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, realTimeOrderCount},
			};

			var result = Exec(XML_KEY_NAME, "UpdateRealTimeOrderCount", ht);
			return result;
		}

		/// <summary>
		/// Get Users For Line
		/// </summary>
		/// <param name="columnName">Column Name</param>
		/// <param name="userId">User Id</param>
		/// <param name="name">Name</param>
		/// <param name="nameKana">Name Kana</param>
		/// <param name="tel">Tel</param>
		/// <param name="mailId">Mail Id</param>
		/// <param name="isSearchWithLineUserId">Is Search With Line User Id</param>
		/// <returns>User models</returns>
		public UserModel[] GetUsersForLine(
			string columnName,
			string userId,
			string name,
			string nameKana,
			string tel,
			string mailId,
			bool isSearchWithLineUserId)
		{
			if (isSearchWithLineUserId && string.IsNullOrEmpty(columnName)) return null;
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
				{ Constants.FIELD_USER_NAME, name },
				{ Constants.FIELD_USER_NAME_KANA, nameKana },
				{ Constants.FIELD_USER_TEL1, tel },
				{ Constants.FIELD_USER_MAIL_ADDR, mailId }
			};
			var userSearchCondition = isSearchWithLineUserId
				? string.Format("<@@hasval:{0}@@> AND w2_UserExtend.{0} = @user_id </@@hasval:{0}@@>", columnName)
				: "<@@hasval:user_id@@> AND w2_User.user_id = @user_id </@@hasval:user_id@@>";
			var replace = new KeyValuePair<string, string>("@@ user_search_condition @@", userSearchCondition);
			var result = Get(XML_KEY_NAME, "GetUsersForLine", input, replaces: replace);
			return result
				.Cast<DataRowView>()
				.Select(item => new UserModel(item))
				.ToArray();
		}

		/// <summary>
		/// 一時テーブルのユーザー情報を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		public UserModel GetWorkUser(string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
			};
			var dv = Get(XML_KEY_NAME, "GetWorkUser", input);
			return (dv.Count != 0) ? new UserModel(dv[0]) : null;
		}
		#endregion

		#region ユーザー拡張項目
		/// <summary>
		/// ユーザー拡張項目取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="userExtendSettingList">ユーザー拡張項目設定リスト</param>
		/// <returns>モデル列</returns>
		public UserExtendModel GetUserExtend(string userId, UserExtendSettingList userExtendSettingList)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USEREXTEND_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserExtend", input);
			if (dv.Count == 0) return UserExtendModel.CreateEmpty(userId, userExtendSettingList);
			return new UserExtendModel(dv[0], userExtendSettingList);
		}

		/// <summary>
		/// ユーザー拡張項目取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル列</returns>
		public UserExtendModel GetUserExtend(string userId)
		{
			var userExtendSettingList = new UserExtendSettingList(false, "", GetUserExtendSettingList());
			return GetUserExtend(userId, userExtendSettingList);
		}

		/// <summary>
		/// カラムの存在チェック
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <returns>true:存在する、false:存在しない</returns>
		public bool UserExtendColumnExists(string settingId)
		{
			var replace = new KeyValuePair<string, string>("@@ field @@", settingId);
			var dv = Get(XML_KEY_NAME, "UserExtendColumnExists", null, replaces: replace);
			return (dv.Count != 0);
		}

		/// <summary>
		/// ユーザー拡張項目登録
		/// </summary>
		/// <param name="userExtendModel">ユーザー拡張項目モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUserExtend(UserExtendModel userExtendModel)
		{
			var inputParam = new Hashtable(userExtendModel.UserExtendDataValue);
			inputParam.Add(Constants.FIELD_USEREXTEND_USER_ID, userExtendModel.UserId);
			inputParam.Add(Constants.FIELD_USEREXTEND_LAST_CHANGED, userExtendModel.LastChanged);

			var userExtendDataValue = new Hashtable(userExtendModel.UserExtendDataValue);

			// 動的に増減するカラムをインサートするのに必要なパラメータを設定
			StringBuilder fields = new StringBuilder();
			StringBuilder values = new StringBuilder();
			foreach (string key in userExtendModel.UserExtendColumns)
			{
				fields.Append(key).Append(",");
				values.Append("@").Append(key).Append(",");
			}

			List<KeyValuePair<string, string>> replace = new List<KeyValuePair<string, string>>();
			replace.Add(new KeyValuePair<string, string>("@@ fields @@", fields.ToString()));
			replace.Add(new KeyValuePair<string, string>("@@ values @@", values.ToString()));
			var result = ExecExtend(XML_KEY_NAME, "InsertUserExtend", inputParam, replace.ToArray(), userExtendDataValue);
			return result;
		}

		/// <summary>
		/// 実行する
		/// </summary>
		/// <param name="pageName">対象テーブル名（XMLファイル名）</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="input">入力値</param>
		/// <param name="replaces">置換値</param>
		/// <param name="userExtendDataValue">ステートメントのパラメーター作成用</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// リリースビルド時はDLLリソースからXMLを読み込むが、
		/// デバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </remarks>
		private int ExecExtend(string pageName, string statementName, Hashtable input, KeyValuePair<string, string>[] replaces, Hashtable userExtendDataValue)
		{
#if DEBUG
			using (var statement = new SqlStatement(XmlPath.Debug, pageName, statementName))
#else
			using (var statement = new SqlStatement(Properties.Resources.ResourceManager, pageName, pageName, statementName))
#endif
			{
				foreach (var replace in replaces)
				{
					statement.Statement = statement.Statement.Replace(replace.Key, replace.Value);
				}
				foreach (string key in userExtendDataValue.Keys)
				{
					statement.AddInputParameters(key, SqlDbType.NVarChar, "MAX");
				}
				var updated = statement.ExecStatement(this.Accessor, input);
				return updated;
			}
		}

		/// <summary>
		/// ユーザー拡張項目更新
		/// </summary>
		/// <param name="userExtendModel">ユーザー拡張項目モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateUserExtend(UserExtendModel userExtendModel)
		{
			var inputParam = new Hashtable(userExtendModel.UserExtendDataValue);
			var userExtendDataValue = new Hashtable(userExtendModel.UserExtendDataValue);

			inputParam.Add(Constants.FIELD_USEREXTEND_USER_ID, userExtendModel.UserId);
			inputParam.Add(Constants.FIELD_USEREXTEND_LAST_CHANGED, userExtendModel.LastChanged);

			// 動的に増減するカラムをインサートするのに必要なパラメータを設定
			StringBuilder fields = new StringBuilder();
			StringBuilder values = new StringBuilder();
			StringBuilder updatefields = new StringBuilder();
			foreach (string key in userExtendModel.UserExtendColumns)
			{
				fields.Append(key).Append(",");
				values.Append("@").Append(key).Append(",");
				updatefields.Append(key).Append("=@").Append(key).Append(",");
			}

			List<KeyValuePair<string, string>> replace = new List<KeyValuePair<string, string>>();
			replace.Add(new KeyValuePair<string, string>("@@ fields @@", fields.ToString()));
			replace.Add(new KeyValuePair<string, string>("@@ values @@", values.ToString()));
			replace.Add(new KeyValuePair<string, string>("@@ updatefields @@", updatefields.ToString()));

			var result = ExecExtend(XML_KEY_NAME, "UpdateUserExtend", inputParam, replace.ToArray(), userExtendDataValue);
			return result;
		}

		/// <summary>
		/// 一時テーブル（w2_UserExtend_tmp）を作成
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		public int CreateUserExtendTempTable()
		{
			var result = Exec(XML_KEY_NAME, "CreateUserExtendTempTable", null);
			return result;
		}

		/// <summary>
		/// 一時テーブルに項目追加
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <returns>影響を受けた件数</returns>
		public int AddUserExtendColumn(string settingId)
		{
			var replace = new KeyValuePair<string, string>("@@ field @@", settingId);
			var result = Exec(XML_KEY_NAME, "AddUserExtendColumn", null, replace);
			return result;
		}

		/// <summary>
		/// 一時テーブルから項目削除
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <returns>影響を受けた件数</returns>
		public int RemoveUserExtendColumn(string settingId)
		{
			var replace = new KeyValuePair<string, string>("@@ field @@", settingId);
			var result = Exec(XML_KEY_NAME, "RemoveUserExtendColumn", null, replace);
			return result;
		}

		/// <summary>
		/// 一時テーブルの追加・削除対象ではない項目に対してデフォルト制約追加
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>影響を受けた件数</returns>
		public int ReDefinitionUserExtendColumn(string settingId)
		{
			var replace = new KeyValuePair<string, string>("@@ field @@", settingId);
			var result = Exec(XML_KEY_NAME, "ReDefinitionUserExtendColumn", null, replace);
			return result;
		}

		/// <summary>
		/// 元テーブル（w2_UserExtend）をDROPし、一時テーブルをリネーム（w2_UserExtend_tmp -> w2_UserExtend）
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		public int ReDefinitionUserExtend()
		{
			var result = Exec(XML_KEY_NAME, "ReDefinitionUserExtend", null);
			return result;
		}

		/// <summary>
		/// カラムのリネーム
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		public int RenameUserExtendColumn()
		{
			var result = Exec(XML_KEY_NAME, "RenameUserExtendColumn", null);
			return result;
		}
		#endregion

		#region ユーザ拡張項目設定マスタ
		/// <summary>
		/// ユーザ拡張項目設定一覧取得
		/// </summary>
		/// <returns>モデル列</returns>
		public UserExtendSettingModel[] GetUserExtendSettingList()
		{
			var dv = Get(XML_KEY_NAME, "GetUserExtendSettingList");
			return dv.Cast<DataRowView>().Select(drv => new UserExtendSettingModel(drv)).ToArray();
		}

		/// <summary>
		/// ユーザ拡張項目設定取得
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>モデル</returns>
		public UserExtendSettingModel GetUserExtendSetting(string settingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USEREXTENDSETTING_SETTING_ID, settingId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserExtendSetting", ht);
			if (dv.Count == 0) return null;
			return new UserExtendSettingModel(dv[0]);
		}

		/// <summary>
		/// ユーザ拡張項目設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUserExtendSetting(UserExtendSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertUserExtendSetting", model.DataSource);
			return result;
		}

		/// <summary>
		/// ユーザ拡張項目設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateUserExtendSetting(UserExtendSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUserExtendSetting", model.DataSource);
			return result;
		}

		/// <summary>
		/// ユーザ拡張項目設定削除
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteUserExtendSetting(string settingId)
		{
			var ht = new Hashtable
				{
				{Constants.FIELD_USEREXTENDSETTING_SETTING_ID, settingId},
				};
			var result = Exec(XML_KEY_NAME, "DeleteUserExtendSetting", ht);
			return result;
		}
		#endregion

		#region かんたん会員登録設定マスタ
		/// <summary>
		/// かんたん会員登録設定取得
		/// </summary>
		/// <param name="itemId">項目ID</param>
		/// <returns>かんたん会員登録設定</returns>
		public UserEasyRegisterSettingModel GetUserEasyRegisterSetting(string itemId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USEREASYREGISTERSETTING_ITEM_ID, itemId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserEasyRegisterSetting", ht);
			return (dv.Count == 0) ? null : new UserEasyRegisterSettingModel(dv[0]);
		}

		/// <summary>
		/// かんたん会員登録設定全件取得
		/// </summary>
		/// <returns>かんたん会員登録設定全件</returns>
		public UserEasyRegisterSettingModel[] GetUserEasyRegisterSettingList()
		{
			var dv = Get(XML_KEY_NAME, "GetUserEasyRegisterSettingList");
			return dv.Cast<DataRowView>().Select(drv => new UserEasyRegisterSettingModel(drv)).ToArray();
		}

		/// <summary>
		/// かんたん会員登録設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateUserEasyRegisterSetting(UserEasyRegisterSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUserEasyRegisterSetting", model.DataSource);
			return result;
		}
		#endregion

		#region ユーザー属性マスタ
		#region ~GetUsreAttribute ユーザー属性取得
		/// <summary>
		/// ユーザー属性取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>更新件数</returns>
		internal UserAttributeModel GetUsreAttribute(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERATTRIBUTE_USER_ID, userId}
			};
			var dv = Get(XML_KEY_NAME, "GetUserAttribute", ht);
			return (dv.Count != 0) ? new UserAttributeModel(dv[0]) : null;
		}
		#endregion

		#region ~MergeUserAttributeOrderInfo ユーザー属性受注情報MERGE
		/// <summary>
		/// ユーザー属性受注情報MERGE
		/// </summary>
		/// <param name="model">UserAttributeModel</param>
		/// <returns>更新件数</returns>
		internal int MergeUserAttributeOrderInfo(UserAttributeModel model)
		{
			return Exec(XML_KEY_NAME, "MergeUserAttributeOrderInfo", model.DataSource);
		}
		#endregion

		#region ~CreateEmptyUserAttribute 空のユーザー属性作成
		/// <summary>
		/// 空のユーザー属性作成
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新件数</returns>
		internal int CreateEmptyUserAttribute(string lastChanged)
		{
			return Exec(XML_KEY_NAME, "CreateEmptyUserAttribute", new Hashtable
			{
				{Constants.FIELD_USERATTRIBUTE_LAST_CHANGED, lastChanged}
			});
		}
		#endregion

		#region +GetUserIdsForSetCpmCluster CPMクラスタ付与向けユーザーID取得
		/// <summary>
		/// CPMクラスタ付与向けユーザーID取得
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <param name="cpmClusterSettings">CPMクラスタ設定</param>
		/// <returns>ユーザーIDリスト</returns>
		internal string[] GetUserIdsForSetCpmCluster(DateTime targetStart, DateTime targetEnd, CpmClusterSettings cpmClusterSettings)
		{
			var ht = new Hashtable();
			ht["begin"] = targetStart;
			ht["end"] = targetEnd;

			var awayDays = new KeyValuePair<string, string>(
				"@@ away_days @@",
				string.Join(",",
					cpmClusterSettings.Settings2.Where(s => s.AwayDays.HasValue).Select(s => s.AwayDays.Value.ToString()).ToArray())
			);
			var dv = Get(XML_KEY_NAME, "GetUserIdsForSetCpmCluster", ht, replaces: awayDays);
			return dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray();
		}
		#endregion

		#region ~UpdateUserAttributeCpmCluster ユーザー属性受注情報MERGE
		/// <summary>
		/// ユーザー属性受注情報MERGE
		/// </summary>
		/// <param name="model">UserAttributeModel</param>
		/// <returns>更新件数</returns>
		internal int UpdateUserAttributeCpmCluster(UserAttributeModel model)
		{
			return Exec(XML_KEY_NAME, "UpdateUserAttributeCpmCluster", model.DataSource);
		}
		#endregion

		#region -GetUserCpmClusterReportData CPMクラスタカウントレポートデータ取得
		/// <summary>
		/// CPMクラスタカウントレポートデータ取得
		/// </summary>
		/// <returns></returns>
		public CpmClusterReport GetUserCpmClusterReportData()
		{
			var dv = Get(XML_KEY_NAME, "GetUserCpmClusterReportData");
			return new CpmClusterReport(dv.Cast<DataRowView>().Select(drv => new CpmClusterReportItem(drv)).ToArray());
		}
		#endregion

		#region DeleteUserAttributeCpmCluster ユーザ属性情報削除
		/// <summary>
		/// ユーザ属性情報削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>削除件数</returns>
		public int DeleteUserAttributeCpmCluster(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USERATTRIBUTE_USER_ID, userId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteUserAttributeCpmCluster", input);
			return result;
		}
		#endregion

		#endregion

		#region ~GetUserIdsByOrderChanged 注文更新日から対象ユーザーID取得
		/// <summary>
		/// 注文更新日から対象ユーザーID取得
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <returns>ユーザーID</returns>
		internal string[] GetUserIdsByOrderChanged(DateTime targetStart, DateTime targetEnd)
		{
			var minDateTime = DateTime.Parse("1900/01/01");
			var ht = new Hashtable
				{
					{"begin", targetStart < minDateTime ? minDateTime : targetStart},
					{"end", targetEnd},
				};
			var dv = Get(XML_KEY_NAME, "GetUserIdsByOrderChanged", ht);
			return dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray();
		}
		#endregion

		#region パスワードリマインダーマスタ
		/// <summary>
		/// パスワードリマインダー取得
		/// </summary>
		/// <param name="authenticationKey">認証キー</param>
		/// <returns>モデル</returns>
		public PasswordReminderModel GetPasswordReminder(string authenticationKey)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_PASSWORDREMINDER_AUTHENTICATION_KEY, authenticationKey},
			};
			var dv = Get(XML_KEY_NAME, "GetPasswordReminder", input);
			if (dv.Count == 0) return null;
			return new PasswordReminderModel(dv[0]);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="userId">ユーザID</param>
		public int DeletePasswordReminder(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_PASSWORDREMINDER_USER_ID, userId},
			};
			var result = Exec(XML_KEY_NAME, "DeletePasswordReminder", input);
			return result;
		}

		/// <summary>
		/// パスワードリマインダー情報削除及び挿入
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <param name="changeUserPasswordTrialLimitCount">変更試行回数制限</param>
		/// <param name="dateCreated">作成日</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteInsertPasswordReminder(string userId, string authenticationKey, int changeUserPasswordTrialLimitCount, DateTime dateCreated)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_PASSWORDREMINDER_USER_ID, userId},
				{Constants.FIELD_PASSWORDREMINDER_AUTHENTICATION_KEY, authenticationKey},
				{Constants.FIELD_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT, changeUserPasswordTrialLimitCount},
				{Constants.FIELD_PASSWORDREMINDER_DATE_CREATED, dateCreated},
			};
			var result = Exec(XML_KEY_NAME, "DeleteInsertPasswordReminder", input);
			return result;
		}

		/// <summary>
		/// 試行可能回数更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="changeUserPasswordTrialLimitCount">変更可能回数</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateChangeUserPasswordTrialLimitCount(string userId, int changeUserPasswordTrialLimitCount)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_PASSWORDREMINDER_USER_ID, userId},
				{Constants.FIELD_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT, changeUserPasswordTrialLimitCount},
			};
			var result = Exec(XML_KEY_NAME, "UpdateChangeUserPasswordTrialLimitCount", input);
			return result;
		}
		#endregion

		#region +GetIntegrationTargetUserList ユーザー統合対象ユーザーリスト取得
		/// <summary>
		/// ユーザー統合対象ユーザーリスト取得
		/// </summary>
		/// <returns>モデル列</returns>
		public UserModel[] GetIntegrationTargetUserList()
		{
			var dv = Get(XML_KEY_NAME, "GetIntegrationTargetUserList");
			return dv.Cast<DataRowView>().Select(drv => new UserModel(drv)).ToArray();
		}
		#endregion

		#region メールエラーアドレスマスタ
		/// <summary>
		/// エラーポイント取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		public UserErrorPoint GetErrorPoint(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetErrorPoint", input);
			if (dv.Count == 0) return null;
			return new UserErrorPoint(dv[0]);
		}
		#endregion

		#region ユーザーアクティビティ
		/// <summary>
		/// ユーザーアクティビティを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <returns>モデル</returns>
		internal UserActivityModel GetUserActivity(string userId, string masterKbn, string masterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
				{Constants.FIELD_USERACTIVITY_MASTER_KBN, masterKbn},
				{Constants.FIELD_USERACTIVITY_MASTER_ID, masterId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserActivity", ht);
			if (dv.Count == 0) return null;
			return new UserActivityModel(dv[0]);
		}

		/// <summary>
		/// 管理画面用のアクティビティ数を取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="masterId">マスターID</param>
		/// <returns>検索結果列</returns>
		internal int GetUserActivityCountForManager(string masterKbn, string masterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_MASTER_KBN, masterKbn},
				{Constants.FIELD_USERACTIVITY_MASTER_ID, masterId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserActivityCountForManager", ht);
			return (int)dv[0][0];
		}

		/// <summary>
		/// ユーザーアクティビティを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		internal UserActivityModel[] GetUserActivityByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserActivityByUserId", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserActivityModel(drv)).ToArray();
		}

		/// <summary>
		/// ユーザーアクティビティを登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUserActivity(UserActivityModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertUserActivity", model.DataSource);
			return result;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteUserActivity(string userId, string masterKbn, string masterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
				{Constants.FIELD_USERACTIVITY_MASTER_KBN, masterKbn},
				{Constants.FIELD_USERACTIVITY_MASTER_ID, masterId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteUserActivity", ht);
			return result;
		}

		/// <summary>
		/// 管理画面用削除
		/// </summary>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteUserActivityForManager(string masterKbn, string masterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_MASTER_KBN, masterKbn},
				{Constants.FIELD_USERACTIVITY_MASTER_ID, masterId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteUserActivityForManager", ht);
			return result;
		}

		/// <summary>
		/// ユーザーIDで削除
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteUserActivityByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteUserActivityByUserId", ht);
			return result;
		}

		#endregion

		#region +UserManagementLevelUserCount ユーザー管理レベルのユーザーへの割当てられている件数
		/// <summary>
		/// ユーザー管理レベルのユーザーへの割当てられている件数
		/// </summary>
		/// <param name="userManagementLevelId">ユーザ管理画面ID</param>
		/// <returns>件数</returns>
		public int UserManagementLevelUserCount(string userManagementLevelId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, userManagementLevelId}
			};
			var dv = Get(XML_KEY_NAME, "UserManagementLevelUserCount", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~SearchUsersForAutosuggest サジェスト用ユーザー検索
		/// <summary>
		/// サジェスト用ユーザー検索
		/// </summary>
		/// <param name="searchWord">検索ワード</param>
		/// <param name="maxCountForDisplay">最大表示件数</param>
		/// <returns>ユーザー一覧</returns>
		internal UserModel[] SearchUsersForAutoSuggest(string searchWord, int maxCountForDisplay)
		{
			var dv = Get(
				XML_KEY_NAME,
				"SearchUsersForAutoSuggest",
				new Hashtable
				{
					{ "search_word_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchWord) },
					{ "max_count_for_display", maxCountForDisplay },
				});
			var users = dv.Cast<DataRowView>()
				.Select(drv => new UserModel(drv))
				.ToArray();
			return users;
		}
		#endregion

		#region ~GetUserByReferralCode
		/// <summary>
		/// Get user by referral code
		/// </summary>
		/// <param name="referralCode">Referral code</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>User id</returns>
		internal UserModel GetUserByReferralCode(string referralCode, string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_REFERRAL_CODE, referralCode },
				{ Constants.FIELD_USER_USER_ID, userId },
			};

			var dv = Get(XML_KEY_NAME, "GetUserByReferralCode", input);
			if (dv.Count == 0) return null;
			return new UserModel(dv[0]);
		}
		#endregion

		#region ~GetReferredUserId
		/// <summary>
		/// Get referred user id
		/// </summary>
		/// <param name="userId">User id</param>
		/// <returns>Referred user id</returns>
		internal string GetReferredUserId(string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
			};

			var referredUserId = Get(XML_KEY_NAME, "GetReferredUserId", input);
			return (string)referredUserId[0][0];
		}
		#endregion

		#region ~UpdateUserReferralCode
		/// <summary>
		/// Update user referral code
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="referralCode">Referral code</param>
		/// <param name="lastChanged">Last changed</param>
		/// <returns>True if any user is updated: False</returns>
		internal bool UpdateUserReferralCode(string userId, string referralCode, string lastChanged)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
				{ Constants.FIELD_USER_REFERRAL_CODE, referralCode },
				{ Constants.FIELD_USER_LAST_CHANGED, lastChanged },
			};

			var result = Exec(XML_KEY_NAME, "UpdateUserReferralCode", input);
			return (result > 0);
		}
		#endregion

		#region ~UpdateReferredUserId
		/// <summary>
		/// Update referred user id
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="referredUserId">Referred user id</param>
		/// <param name="lastChanged">Last changed</param>
		/// <returns>True if any user is updated: False</returns>
		internal bool UpdateReferredUserId(string userId, string referredUserId, string lastChanged)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
				{ Constants.FIELD_USER_REFERRED_USER_ID, referredUserId },
				{ Constants.FIELD_USER_LAST_CHANGED, lastChanged },
			};

			var result = Exec(XML_KEY_NAME, "UpdateReferredUserId", input);
			return (result > 0);
		}
		#endregion
	}
}
