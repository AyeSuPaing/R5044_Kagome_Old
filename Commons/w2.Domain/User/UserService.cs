/*
=========================================================================================================
  Module      : ユーザサービス (UserService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User.Helper;
using w2.Domain.UserCreditCard;
using w2.Domain.UserEasyRegisterSetting;
using w2.Domain.UserExtendSetting;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザサービス
	/// </summary>
	public class UserService : ServiceBase, IUserService
	{
		#region ユーザーマスタ
		/// <summary>
		/// ユーザー検索
		/// </summary>
		/// <param name="cond">ユーザー検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		public UserSearchResult[] Search(UserSearchCondition cond)
		{
			using (var rep = new UserRepository())
			{
				var result = rep.Search(cond);
				return result;
			}
		}

		/// <summary>
		/// ユーザー検索件数取得
		/// </summary>
		/// <param name="cond">ユーザー検索条件クラス</param>
		/// <returns>検索件数</returns>
		public int GetSearchHitCount(UserSearchCondition cond)
		{
			using (var rep = new UserRepository())
			{
				var count = rep.GetSearchHitCount(cond);
				return count;
			}
		}

		/// <summary>
		///  CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var accessor = new SqlAccessor())
			using (var repository = new UserRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
				new KeyValuePair<string, string>("@@ user_extend_field_name @@",
					string.Format("{0}.{1}",
						Constants.TABLE_USEREXTEND,
						string.IsNullOrEmpty((string)input["user_extend_name"])
							? Constants.FIELD_USEREXTEND_USER_ID
							: (string)input["user_extend_name"]))))
			{
				new MasterExportCsv().Exec(setting, reader, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
			}
			return true;
		}

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		public bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var repository = new UserRepository())
			{
				var dv = repository.GetMaster(input,
					statementName,
					new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
					new KeyValuePair<string, string>("@@ user_extend_field_name @@",
						string.Format("{0}.{1}",
							Constants.TABLE_USEREXTEND,
							string.IsNullOrEmpty((string)input["user_extend_name"])
								? Constants.FIELD_USEREXTEND_USER_ID
								: (string)input["user_extend_name"])));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ区分で該当するStatementNameを取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>StatementName</returns>
		private string GetStatementNameInfo(string masterKbn)
		{
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER: // ユーザーマスタ
					return "GetMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING: // 配送先情報
					return "GetUserShippingMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE: // ユーザー電子発票管理マスタ
					return "GetUserTaiwanInvoice";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames, string masterKbn)
		{
			try
			{
				using (var repository = new UserRepository())
				{
					repository.CheckFieldsForGetMaster(
						new Hashtable(),
						masterKbn,
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}
			return true;
		}

		/// <summary>
		/// ユーザー情報取得 (SqlAccessor指定)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>モデル</returns>
		public UserModel Get(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				// ユーザー情報
				var user = repository.Get(userId);

				if (user != null)
				{
					// ユーザー拡張項目情報
					user.UserExtend = repository.GetUserExtend(userId);

					// 決済カード連携情報セット
					user.UserCreditCards = new UserCreditCardService().GetUsable(userId, accessor);
				}

				return user;
			}
		}

		/// <summary>
		/// ユーザー情報の件数取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int Count(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var user = repository.Count(userId);
				return user;
			}
		}

		/// <summary>
		/// 複数ユーザ取得
		/// </summary>
		/// <param name="userIds">取得対象のユーザの配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// <returns>モデル</returns>
		public IEnumerable<UserModel> GetUsers(IEnumerable<string> userIds, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				return repository.GetUsers(userIds);
			}
		}

		/// <summary>
		/// ユーザー情報を拡張項目の値から取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザー</returns>
		public UserModel GetByExtendColumn(string columnName, string value, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var user = repository.GetByExtendColumn(columnName, value);
				if (user != null)
				{
					// ユーザー拡張項目情報
					user.UserExtend = repository.GetUserExtend(user.UserId);

					// 決済カード連携情報セット
					user.UserCreditCards = new UserCreditCardService().GetUsable(user.UserId, accessor);
				}
				return user;
			}
		}

		/// <summary>
		/// 拡張項目の値が一致するユーザー一覧を取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル配列</returns>
		public UserModel[] GetUsersByExtendColumn(string columnName, string value, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var users = repository.GetUsersByExtendColumn(columnName, value);

				var userCreditCardService = new UserCreditCardService();
				foreach (var user in users)
				{
					// ユーザー拡張項目情報
					user.UserExtend = repository.GetUserExtend(user.UserId);

					// 決済カード連携情報セット
					user.UserCreditCards = userCreditCardService.GetUsable(user.UserId, accessor);
				}

				return users;
			}
		}

		/// <summary>
		/// 退会していないユーザー情報を拡張項目の値から取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザー</returns>
		public UserModel GetRegisteredUserByExtendColumn(string columnName, string value, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var user = repository.GetRegisteredUserByExtendColumn(columnName, value);
				if (user != null)
				{
					// ユーザー拡張項目情報
					user.UserExtend = repository.GetUserExtend(user.UserId);

					// 決済カード連携情報セット
					user.UserCreditCards = new UserCreditCardService().GetUsable(user.UserId, accessor);
				}
				return user;
			}
		}

		/// <summary>
		/// 会員ランクID取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>会員ランクID</returns>
		public string GetMemberRankId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var memberRankId = repository.GetMemberRankId(userId);
				return memberRankId;
			}
		}

		/// <summary>
		/// メール配信のためユーザ情報取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>メール配信用のユーザー情報</returns>
		public UserForMailSend GetUserForMailSend(string userId)
		{
			using (var repository = new UserRepository())
			{
				var user = repository.GetUserForMailSend(userId);
				return user;
			}
		}

		/// <summary>
		/// マスタアップロードバッチ用 
		/// ログインID重複ユーザ情報取得
		/// </summary>
		/// <returns>ログインID重複ユーザモデル列</returns>
		public DuplicationLoginId[] GetDuplicationLoginIdList()
		{
			using (var repository = new UserRepository())
			{
				var result = repository.GetDuplicationLoginIdList();
				return result;
			}
		}

		/// <summary>
		/// ユーザーシンボル取得
		/// </summary>
		/// <param name="userIds">ユーザーIDリスト</param>
		/// <returns>ユーザーシンボルリスト（0件の場合は0配列）</returns>
		public UserSymbols[] GetUserSymbols(params string[] userIds)
		{
			using (var repository = new UserRepository())
			{
				var result = repository.GetUserSymbols(userIds);
				return result;
			}
		}

		/// <summary>
		/// ログイン試行
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="rawPassword">パスワード(raw)</param>
		/// <param name="mailaddressEnabled">メールアドレスのログインID利用有無</param>
		/// <returns>ユーザー情報:成功、null:失敗</returns>
		public UserModel TryLogin(string loginId, string rawPassword, bool mailaddressEnabled)
		{
			// ログインIDまたはパスワードが空の時はログインさせない
			if ((loginId == string.Empty) || (rawPassword == string.Empty)) return null;

			string passwordEncrypt = UserPassowordCryptor.PasswordEncrypt(rawPassword);
			using (var repository = new UserRepository())
			{
				var loginUser = mailaddressEnabled
					? repository.UserLoginUsedMailAddress(loginId, passwordEncrypt)
					: repository.UserLogin(loginId, passwordEncrypt);
				if (loginUser == null) return null;

				var user = Get(loginUser.UserId);
				return user;
			}
		}

		/// <summary>
		/// メールアドレスでユーザー情報取得 (Batch.CsMailReceiver用、CSManager用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserModel GetUserByMailAddr(string mailAddr, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var users = repository.GetUsersByMailAddr(mailAddr);
				return (users.Length != 0) ? users[0] : null;
			}
		}

		/// <summary>
		/// ログインIDでユーザー情報取得
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserModel GetUserByLoginId(string loginId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var users = repository.GetUserByLoginId(loginId);
				return (users.Length != 0) ? users[0] : null;
			}
		}

		/// <summary>
		/// メールアドレスでユーザー情報取得 (複数件)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル配列</returns>
		public UserModel[] GetUsersByMailAddr(string mailAddr, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var users = repository.GetUsersByMailAddr(mailAddr);
				return users;
			}
		}

		/// <summary>
		/// メールアドレスでユーザー情報取得 (メールマガジン登録用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		public UserModel GetUserByMailAddrForMailMagazineRegister(string mailAddr)
		{
			using (var repository = new UserRepository())
			{
				var user = repository.GetUserByMailAddrForMailMagazineRegister(mailAddr);
				return user;
			}
		}

		/// <summary>
		/// メールアドレスでユーザー情報取得 (メールマガジン解除用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		public UserModel GetUserByMailAddrForMailMagazineCancel(string mailAddr)
		{
			using (var repository = new UserRepository())
			{
				var user = repository.GetUserByMailAddrForMailMagazineCancel(mailAddr);
				return user;
			}
		}

		/// <summary>
		/// メールアドレスでユーザー情報取得 (AmazonPay用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		public UserModel GetRegisteredUserByMailAddress(string mailAddr)
		{
			using (var repository = new UserRepository())
			{
				var users = repository.GetRegisteredUserByMailAddress(mailAddr);
				return (users.Length == 1) ? users[0] : null;
			}
		}

		/// <summary>
		/// ユーザID取得 (MallBatch.MailOrderGetter)
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザーID</returns>
		public string GetUserId(string mallId, string mailAddr, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				string result = repository.GetUserId(mallId, mailAddr);
				return result;
			}
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
			using (var repository = new UserRepository())
			{
				var user = repository.GetUserForReminder(loginId, mailAddr, mailaddressEnabled);
				return user;
			}
		}

		/// <summary>
		/// ユーザー情報取得 (SqlAccessor指定)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>ユーザー関連データ</returns>
		public UserRelationDatas GetWithUserRelationDatas(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				// ユーザー情報
				var userWithUserRelationDatas = repository.GetWithUserRelationDatas(userId);
				return userWithUserRelationDatas;
			}
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
			using (var repository = new UserRepository())
			{
				var result = repository.GetModifyUsers(updateAt, limit, offset, sortType);
				return result;
			}
		}

		/// <summary>
		/// Get users for Letro
		/// </summary>
		/// <param name="userIds">User ids</param>
		/// <returns>Users for Letro</returns>
		public DataView GetUsersForLetro(IEnumerable<string> userIds)
		{
			using (var repository = new UserRepository())
			{
				return repository.GetUsersForLetro(userIds);
			}
		}

		/// <summary>
		/// ログインＩＤ重複チェック
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="mailAddressEnabled">メールアドレスのログインID利用有無（1:有効 、0:無効）</param>
		/// <param name="userId">ユーザーID（指定しない場合：登録チェック、指定する場合：更新チェック）</param>
		/// <returns>true:重複しない、false:重複する</returns>
		public bool CheckDuplicationLoginId(string loginId, string mailAddressEnabled, string userId = null)
		{
			using (var repository = new UserRepository())
			{
				int result = (userId == null)
							 ? repository.CheckDuplicationLoginIdRegist(loginId, mailAddressEnabled)
							 : repository.CheckDuplicationLoginIdModify(userId, loginId, mailAddressEnabled);
				return (result == 0);
			}
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		public bool InsertWithUserExtend(
			UserModel user,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = InsertWithUserExtend(user, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public bool InsertWithUserExtend(
			UserModel user,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 登録
			var updated = InsertWithUserExtend(user, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(user.UserId, lastChanged, accessor);
			}
			return (updated > 0);
		}
		/// <summary>
		/// ユーザー情報とユーザー拡張項目情報登録
		/// </summary>
		/// <param name="user">ユーザー情報（※パスワード登録を行う場合、生で指定してください）</param>
		/// <param name="accessor">登録</param>
		/// <returns>true:成功、false:失敗</returns>
		private int InsertWithUserExtend(UserModel user, SqlAccessor accessor = null)
		{
			using (var scope = new TransactionScope())
			using (var repository = new UserRepository(accessor))
			{
				// ユーザー情報登録
				var updated = Insert(user, repository.Accessor);

				// ユーザー拡張項目情報登録
				if (user.UserExtend != null)
				{
					user.UserExtend.UserId = user.UserId;
					user.UserExtend.LastChanged = user.LastChanged;
					repository.InsertUserExtend(user.UserExtend);
				}
				scope.Complete();

				return updated;
			}
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
			using (var repository = new UserRepository())
			{
				var users = repository.GetUsersForLine(
					columnName,
					userId,
					name,
					nameKana,
					tel,
					mailId,
					isSearchWithLineUserId);

				foreach (var user in users)
				{
					user.UserExtend = repository.GetUserExtend(user.UserId);
				}
				return users;
			}
		}

		#region +Insert ユーザー情報のみ登録
		/// <summary>
		/// ユーザー情報のみ登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(UserModel user, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor = null)
		{
			// データ補正
			user.Corrected();

			// 登録
			var result = Insert(user, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(user.UserId, user.LastChanged, accessor);
			}
			return result;
		}
		#endregion
		#region -Insert ユーザー情報のみ更新
		/// <summary>
		/// ユーザー情報のみ更新
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		private int Insert(UserModel user, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.Insert(user);
				return result;
			}
		}
		#endregion

		#region +UpdateWithUserExtend ユーザー情報更新（拡張項目とともに）
		/// <summary>
		/// ユーザー情報更新（拡張項目とともに）
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		public bool UpdateWithUserExtend(
			UserModel user,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = UpdateWithUserExtend(user, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion
		#region +UpdateWithUserExtend ユーザー情報更新（拡張項目とともに）
		/// <summary>
		/// ユーザー情報更新（拡張項目とともに）
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UpdateWithUserExtend(
			UserModel user,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				// データ補正
				user.Corrected();

				// ユーザー情報更新
				Update(user, repository.Accessor);

				// ユーザー拡張項目情報更新(更新履歴の登録も実行)
				UpdateUserExtend(user.UserExtend, user.UserId, user.LastChanged, updateHistoryAction, accessor);

				return true;
			}
		}
		#endregion

		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(
			string userId,
			Action<UserModel> updateAction,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(userId, updateAction, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(
			string userId,
			Action<UserModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 最新データ取得
			var user = Get(userId, accessor);

			// ユーザー情報がない場合、「0」で戻す
			if (user == null) return 0;

			// モデル内容更新
			updateAction(user);

			// 更新
			int updated = Update(user, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, user.LastChanged, accessor);
			}
			return updated;
		}
		#endregion

		#region +ModifyUserExtend 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新結果</returns>
		public bool ModifyUserExtend(
			string userId,
			Action<UserExtendModel> updateAction,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = ModifyUserExtend(userId, updateAction, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +ModifyUserExtend 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		public bool ModifyUserExtend(
			string userId,
			Action<UserExtendModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 最新データ取得
			var userExtend = GetUserExtend(userId, accessor);

			// モデル内容更新
			updateAction(userExtend);

			// 更新
			var updated = UpdateUserExtend(
				userExtend,
				userId,
				userExtend.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, userExtend.LastChanged, accessor);
			}
			return updated;
		}
		#endregion

		#region +Update ユーザー情報のみ更新
		/// <summary>
		/// ユーザー情報のみ更新
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserModel user, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// データ補正
			user.Corrected();

			// 更新
			var result = Update(user, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(user.UserId, user.LastChanged, accessor);
			}
			return result;
		}
		#endregion
		#region -Update ユーザー情報のみ更新
		/// <summary>
		/// ユーザー情報のみ更新
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		private int Update(UserModel user, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				// データ補正
				user.Corrected();

				var result = repository.Update(user);
				return result;
			}
		}
		#endregion

		#region +UpdateMailFlg メール配信フラグ更新
		/// <summary>
		/// メール配信フラグ更新
		/// </summary>
		/// <param name="userId">ユーザーID（履歴更新用）</param>
		/// <param name="remoteAddr">リモートIPアドレス</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="mailFlg">メールフラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UpdateMailFlg(
			string userId,
			string remoteAddr,
			string mailAddr,
			string mailFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				bool updated;
				using (var repository = new UserRepository(accessor))
				{
					updated = (repository.UpdateMailFlg(remoteAddr, mailAddr, mailFlg) > 0);
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				// トランザクションコミット
				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion

		#region +UpdateLoginDate 最終ログイン日時更新
		/// <summary>
		/// 最終ログイン日時更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UpdateLoginDate(string userId, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = UpdateLoginDate(userId, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion
		#region -UpdateLoginDate 最終ログイン日時更新
		/// <summary>
		/// 最終ログイン日時更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		private bool UpdateLoginDate(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				userId,
				model =>
				{
					model.DateLastLoggedin = DateTime.Now;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return (updated > 0);
		}
		#endregion

		/// <summary>
		/// ユーザ退会
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UserWithdrawal(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var updated = UserWithdrawal(
					userId,
					lastChanged,
					updateHistoryAction,
					accessor);

				// トランザクションコミット
				accessor.CommitTransaction();

				return updated;
			}
		}
		/// <summary>
		/// ユーザ退会
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UserWithdrawal(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = UserWithdrawal(userId, lastChanged, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return updated;
		}

		/// <summary>
		/// ユーザ退会
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		private bool UserWithdrawal(string userId, string lastChanged, SqlAccessor accessor)
		{
			var maskPassword = UserPassowordCryptor.PasswordEncrypt("****");
			using (var repository = new UserRepository(accessor))
			{
				var input = new Hashtable
				{
					{Constants.FIELD_USER_USER_ID, userId},
					{Constants.FIELD_USER_PASSWORD, maskPassword},
					{Constants.FIELD_USER_LAST_CHANGED, lastChanged},
				};
				var result = repository.UserWithdrawal(input);
				return (result > 0);
			}
		}

		/// <summary>
		/// モバイルユーザID更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="career">キャリアID</param>
		/// <param name="mobileUID">モバイルユーザID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UpdateMobileUID(
			string userId,
			string career,
			string mobileUID,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var result = UpdateMobileUID(userId, career, mobileUID, accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				accessor.CommitTransaction();
				return result;
			}
		}
		/// <summary>
		/// モバイルユーザID更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="career">キャリアID</param>
		/// <param name="mobileUID">モバイルユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		private bool UpdateMobileUID(string userId, string career, string mobileUID, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_USER_USER_ID, userId },
					{ Constants.FIELD_USER_CAREER_ID, career },
					{ Constants.FIELD_USER_MOBILE_UID, mobileUID },
				};
				var updated = repository.UpdateMobileUID(input);
				return (updated > 0);
			}
		}

		/// <summary>
		/// メールマガジン解除
		/// </summary>
		/// <param name="userId">ユーザーID（履歴更新用）</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool MailMagazineCancel(
			string userId,
			string mailAddr,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var result = MailMagazineCancel(mailAddr, accessor);

				// 更新履歴登録
				if (result && updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				// トランザクションコミット
				accessor.CommitTransaction();
				return result;
			}
		}
		/// <summary>
		/// メールマガジン解除
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		private bool MailMagazineCancel(string mailAddr, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var input = new Hashtable
				{
					{Constants.FIELD_USER_MAIL_ADDR, mailAddr},
				};
				var result = repository.MailMagazineCancel(input);
				return (result > 0);
			}
		}

		/// <summary>
		/// ユーザー情報削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true: 成功、false:失敗</returns>
		public bool Delete(string userId, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 更新
			var updated = Delete(userId, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return updated;
		}
		/// <summary>
		/// ユーザー情報削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true: 成功、false:失敗</returns>
		private bool Delete(string userId, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.Delete(userId);
				return (result > 0);
			}
		}

		/// <summary>
		/// 初回広告コードを更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UpdateAdvCodeFirst(
			string userId,
			string advCodeFirst,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(
					userId,
					model =>
					{
						model.AdvcodeFirst = advCodeFirst;
						model.LastChanged = lastChanged;
					},
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return (updated > 0);
			}
		}

		/// <summary>
		/// ユーザメモ＆ユーザー管理レベルID更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userMemo">ユーザメモ</param>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新したか</returns>
		public bool UpdateUserMemoAndUserManagementLevelId(
			string userId,
			string userMemo,
			string userManagementLevelId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var updated = UpdateUserMemoAndUserManagementLevelId(
					userId,
					userMemo,
					userManagementLevelId,
					lastChanged,
					updateHistoryAction,
					accessor);

				// トランザクションコミット
				accessor.CommitTransaction();

				return updated;
			}
		}
		/// <summary>
		/// ユーザメモ＆ユーザー管理レベルID更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userMemo">ユーザメモ</param>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQアクセサ</param>
		/// <returns>更新したか</returns>
		public bool UpdateUserMemoAndUserManagementLevelId(
			string userId,
			string userMemo,
			string userManagementLevelId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				userId,
				model =>
				{
					model.UserMemo = userMemo;
					model.UserManagementLevelId = userManagementLevelId;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return (updated > 0);
		}

		/// <summary>
		/// 外部ユーザーIDでユーザー情報取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="externalUserId">外部ユーザーID</param>
		/// <returns>ユーザーモデル</returns>
		public UserModel GetUserByExternalUserId(string columnName, string externalUserId)
		{
			using (var repository = new UserRepository())
			{
				var user = repository.GetUserByExternalUserId(columnName, externalUserId);
				if (user == null) return null;

				// ユーザー拡張項目情報
				user.UserExtend = repository.GetUserExtend(user.UserId);
				return user;
			}
		}

		/// <summary>
		/// 最終誕生日ポイント付与年更新
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateUserBirthdayPointAddYear(
			UserModel user,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.UpdateUserBirthdayPointAddYear(user);

				// 更新履歴登録
				if ((result > 0) && (updateHistoryAction == UpdateHistoryAction.Insert))
				{
					new UpdateHistoryService().InsertForUser(user.UserId, user.LastChanged, accessor);
				}

				return result;
			}
		}

		/// <summary>
		/// 誕生日クーポン発行年更新
		/// </summary>
		/// <param name="user">モデル</param>
		/// <param name="updateHistoryAction">更新するか</param>
		/// <param name="accessor">アクセサー</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新件数</returns>
		public int UpdateUserBirthdayCouponPublishYear(
			UserModel user,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null,
			string lastChanged = "")
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.UpdateUserBirthdayCouponPublishYear(user);

				// 更新履歴登録
				if ((result > 0) && (updateHistoryAction == UpdateHistoryAction.Insert))
				{
					new UpdateHistoryService().InsertForUser(
						user.UserId,
						string.IsNullOrEmpty(lastChanged) ? user.LastChanged : lastChanged,
						accessor);
				}

				return result;
			}
		}

		/// <summary>
		/// 誕生日クーポン発行年更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>更新件数件数</returns>
		public int UpdateUserBirthdayCouponPublishYearByCouponInfo(
			string userId,
			string lastChanged,
			SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.UpdateUserBirthdayCouponPublishYearByCouponInfo(userId, lastChanged);

				return result;
			}
		}

		/// <summary>
		/// リアルタイム累計購入回数更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="realTimeOrderCount">累計購入回数</param>
		/// <param name="accessor">アクセサー</param>
		/// <returns>更新件数</returns>
		public int UpdateRealTimeOrderCount(string userId, int realTimeOrderCount, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.UpdateRealTimeOrderCount(userId, realTimeOrderCount);
				return result;
			}
		}

		/// <summary>
		/// 一時テーブルからユーザー情報を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサー</param>
		/// <returns>ユーザーモデル</returns>
		public UserModel GetWorkUser(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var model = repository.GetWorkUser(userId);
				return model;
			}
		}
		#endregion

		#region ユーザー拡張項目マスタ
		/// <summary>
		/// ユーザー拡張項目取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserExtendModel GetUserExtend(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var model = repository.GetUserExtend(userId, this.GetUserExtendSettingList(accessor));
				return model;
			}
		}

		/// <summary>
		/// ユーザー拡張項目更新
		/// </summary>
		/// <param name="model">ユーザー拡張項目</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateUserExtend(UserExtendModel model, string userId, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateUserExtend(model, userId, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
			}
		}
		/// <summary>
		/// ユーザー拡張項目更新(更新履歴登録も実行)
		/// </summary>
		/// <param name="model">ユーザー拡張項目</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public bool UpdateUserExtend(UserExtendModel model, string userId, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var updated = false;
			using (var repository = new UserRepository(accessor))
			{
				if (model != null)
				{
					model.UserId = userId;
					model.LastChanged = lastChanged;
					updated = (repository.UpdateUserExtend(model) > 0);
				}

				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}
			}
			return updated;
		}

		/// <summary>
		/// カラムの存在チェック
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <returns>true:存在する、false:存在しない</returns>
		public bool UserExtendColumnExists(string settingId)
		{
			using (var repository = new UserRepository())
			{
				return repository.UserExtendColumnExists(settingId);
			}
		}

		/// <summary>
		/// 一時テーブル（w2_UserExtend_tmp）を作成
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		public bool CreateUserExtendTempTable(SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.CreateUserExtendTempTable();
				return (result > 0);
			}
		}

		/// <summary>
		/// 一時テーブルから項目削除
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		public bool RemoveUserExtendColumn(string settingId, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.RemoveUserExtendColumn(settingId);
				return (result > 0);
			}
		}

		/// <summary>
		/// 一時テーブルに項目追加
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		public bool AddUserExtendColumn(string settingId, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.AddUserExtendColumn(settingId);
				return (result > 0);
			}
		}

		/// <summary>
		/// 一時テーブルの追加・削除対象ではない項目に対してデフォルト制約追加
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		public bool ReDefinitionUserExtendColumn(string settingId, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.ReDefinitionUserExtendColumn(settingId);
				return (result > 0);
			}
		}

		/// <summary>
		/// 元テーブル（w2_UserExtend）をDROPし、一時テーブルをリネーム（w2_UserExtend_tmp -> w2_UserExtend）
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		public bool ReDefinitionUserExtend(SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.ReDefinitionUserExtend();
				return (result > 0);
			}
		}

		/// <summary>
		/// カラムのリネーム
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		public bool RenameUserExtendColumn(SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.RenameUserExtendColumn();
				return (result > 0);
			}
		}
		#endregion

		#region ユーザ拡張項目設定マスタ
		/// <summary>
		/// ユーザ拡張項目設定の削除/新規/更新 処理
		/// </summary>
		/// <param name="userExtendSettingList">ユーザ拡張項目設定一覧</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		public bool UserExtendSettingExecuteAll(UserExtendSettingList userExtendSettingList)
		{
			string message = "";
			bool result = false;

			using (var scope = new TransactionScope())
			using (var repository = new UserRepository())
			{
				try
				{
					//------------------------------------------------------
					// ユーザ拡張項目設定マスタ(w2_UserExtendSetting)の更新
					//------------------------------------------------------
					foreach (var settingData in userExtendSettingList.Items)
					{
						if (settingData.ActionType == UserExtendActionType.Insert)
						{
							message = "ユーザ拡張項目設定マスタ(w2_UserExtendSetting)の追加:" + settingData.SettingId;
							repository.InsertUserExtendSetting(settingData);
						}
						else if (settingData.ActionType == UserExtendActionType.Update)
						{
							message = "ユーザ拡張項目設定マスタ(w2_UserExtendSetting)の更新:" + settingData.SettingId;
							repository.UpdateUserExtendSetting(settingData);
						}
						else if (settingData.ActionType == UserExtendActionType.Delete)
						{
							message = "ユーザ拡張項目設定マスタ(w2_UserExtendSetting)の削除:" + settingData.SettingId;
							repository.DeleteUserExtendSetting(settingData.SettingId);
						}
					}

					//------------------------------------------------------
					// ユーザ拡張項目マスタ(w2_UserExtend)の更新
					//------------------------------------------------------
					// 削除または追加があるか？
					if (CheckActionType(userExtendSettingList, UserExtendActionType.Delete)
						|| CheckActionType(userExtendSettingList, UserExtendActionType.Insert))
					{
						message = "1.一時テーブル（w2_UserExtend_tmp）を作成";
						repository.CreateUserExtendTempTable();

						foreach (var settingData in userExtendSettingList.Items)
						{
							if (settingData.ActionType == UserExtendActionType.Insert)
							{
								message = "2-1.一時テーブルに項目追加:" + settingData.SettingId;
								repository.AddUserExtendColumn(settingData.SettingId);
							}
							else if (settingData.ActionType == UserExtendActionType.Update)
							{
								message = "2-2.一時テーブルの追加・削除対象ではない項目に対してデフォルト制約追加:" + settingData.SettingId;
								repository.ReDefinitionUserExtendColumn(settingData.SettingId);
							}
							else if (settingData.ActionType == UserExtendActionType.Delete)
							{
								message = "2-3.一時テーブルから項目削除:" + settingData.SettingId;
								repository.RemoveUserExtendColumn(settingData.SettingId);
							}
						}

						message = "3.元テーブル（w2_UserExtend）をDROPし、一時テーブルをリネーム（w2_UserExtend_tmp -> w2_UserExtend）";
						repository.ReDefinitionUserExtend();
					}

					// トランザクションコミット
					scope.Complete();
					result = true;
				}
				catch (Exception ex)
				{
					// ログ書き込み
					AppLogger.WriteError(message, ex);
					return false;
				}
			}

			//------------------------------------------------------
			// 削除項目をリストからも削除
			//------------------------------------------------------
			// 削除項目リストを作成後、ユーザ拡張項目設定リストから削除
			List<UserExtendSettingModel> deletedDataList = new List<UserExtendSettingModel>();
			foreach (var deleteData in userExtendSettingList.Items.Where(settingData => settingData.ActionType == UserExtendActionType.Delete))
			{
				deletedDataList.Add(deleteData);
			}
			deletedDataList.ForEach(deleteData => userExtendSettingList.Items.Remove(deleteData));

			//------------------------------------------------------
			//登録状態更新
			//------------------------------------------------------
			userExtendSettingList.Items.ForEach(settingData => settingData.UpdateRegistrationStatus());

			return result;
		}

		/// <summary>
		/// 設定リストに指定したアクションタイプが存在するか
		/// </summary>
		/// <param name="userExtendSettingList">ユーザ拡張項目設定リスト</param>
		/// <param name="actionType">アクションタイプ</param>
		/// <returns>存在すればTrue</returns>
		private bool CheckActionType(UserExtendSettingList userExtendSettingList, UserExtendActionType actionType)
		{
			return userExtendSettingList.Items.Any(settingData => settingData.ActionType == actionType);
		}

		/// <summary>
		/// ユーザ拡張項目設定一覧取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザ拡張項目モデル一覧</returns>
		public UserExtendSettingList GetUserExtendSettingList(SqlAccessor accessor = null)
		{
			var userExtendSettings = GetUserExtendSettingArray(accessor);
			var result = new UserExtendSettingList();
			result.Items.AddRange(userExtendSettings);
			return result;
		}

		/// <summary>
		/// ユーザ拡張項目設定一覧取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザ拡張項目モデル配列</returns>
		public UserExtendSettingModel[] GetUserExtendSettingArray(SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var userExtendSettings = repository.GetUserExtendSettingList();
				return userExtendSettings;
			}
		}

		/// <summary>
		/// ユーザ拡張項目設定取得
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>モデル</returns>
		public UserExtendSettingModel GetUserExtendSetting(string settingId)
		{
			using (var repository = new UserRepository())
			{
				var model = repository.GetUserExtendSetting(settingId);
				return model;
			}
		}

		/// <summary>
		/// ユーザ拡張項目設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public bool InsertUserExtendSetting(UserExtendSettingModel model, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.InsertUserExtendSetting(model);
				return (result > 0);
			}
		}

		/// <summary>
		/// ユーザ拡張項目設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public bool UpdateUserExtendSetting(UserExtendSettingModel model, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.UpdateUserExtendSetting(model);
				return (result > 0);
			}
		}

		/// <summary>
		/// ユーザ拡張項目設定削除
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true: 成功、false:失敗</returns>
		public bool DeleteUserExtendSetting(string settingId, SqlAccessor accessor)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.DeleteUserExtendSetting(settingId);
				return (result > 0);
			}
		}
		#endregion

		#region かんたん会員登録設定マスタ
		/// <summary>
		/// かんたん会員登録設定取得
		/// </summary>
		/// <param name="itemId">項目ID</param>
		/// <returns>モデル</returns>
		public UserEasyRegisterSettingModel GetUserEasyRegisterSetting(string itemId)
		{
			using (var repository = new UserRepository())
			{
				var model = repository.GetUserEasyRegisterSetting(itemId);
				return model;
			}
		}

		/// <summary>
		/// かんたん会員登録設定取得
		/// </summary>
		/// <returns>モデルの配列</returns>
		public UserEasyRegisterSettingModel[] GetUserEasyRegisterSettingList()
		{
			using (var repository = new UserRepository())
			{
				var modelList = repository.GetUserEasyRegisterSettingList();
				return modelList;
			}
		}

		/// <summary>
		/// かんたん会員登録設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		public bool UpdateUserEasyRegisterSetting(UserEasyRegisterSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.UpdateUserEasyRegisterSetting(model);
				return (result > 0);
			}
		}
		#endregion
		#region ユーザー属性マスタ
		#region +GetUserAttribute ユーザー属性取得
		/// <summary>
		/// ユーザー属性取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザー属性モデル</returns>
		public UserAttributeModel GetUserAttribute(string userId)
		{
			using (var repository = new UserRepository())
			{
				var model = repository.GetUsreAttribute(userId);
				return model;
			}
		}
		#endregion

		#region +GetUserIdsByOrderChanged 注文更新日から対象ユーザーID取得
		/// <summary>
		/// 注文更新日から対象ユーザーID取得
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <returns>ユーザーID</returns>
		public string[] GetUserIdsByOrderChanged(DateTime targetStart, DateTime targetEnd)
		{
			using (var repository = new UserRepository { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var ids = repository.GetUserIdsByOrderChanged(targetStart, targetEnd);
				return ids;
			}
		}
		#endregion

		#region +InsertUpdateUserAttributeOrderInfo ユーザー属性受注情報作成＆更新
		/// <summary>
		///  ユーザー属性受注情報作成＆更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新したか</returns>
		public bool InsertUpdateUserAttributeOrderInfo(string userId, string lastChanged)
		{
			// ユーザー属性基本情報作成
			var userAttributesModel = UserAttributeOrderInfoCalculator.GetInstance().Calculate(userId);
			userAttributesModel.LastChanged = lastChanged;

			// 登録更新
			using (var repository = new UserRepository())
			{
				var result = repository.MergeUserAttributeOrderInfo(userAttributesModel);
				return (result > 0);
			}
		}
		#endregion

		#region +CreateEmptyUserAttribute 空のユーザー属性作成
		/// <summary>
		/// 空のユーザー属性作成
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>実行件数</returns>
		public bool CreateEmptyUserAttribute(string lastChanged)
		{
			using (var repository = new UserRepository())
			{
				var result = repository.CreateEmptyUserAttribute(lastChanged);
				return result > 0;
			}
		}
		#endregion

		#region -GetUserIdsForSetCpmCluster CPMクラスタ付与向けユーザーID取得
		/// <summary>
		/// CPMクラスタ付与向けユーザーID取得
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <param name="cpmClusterSettings">CPMクラスタ値設定</param>
		/// <returns>ユーザーIDリスト</returns>
		public string[] GetUserIdsForSetCpmCluster(DateTime targetStart, DateTime targetEnd,
			CpmClusterSettings cpmClusterSettings)
		{
			using (var repository = new UserRepository { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var minDateTime = DateTime.Parse("1900/01/01");

				var ids = repository.GetUserIdsForSetCpmCluster(
					targetStart < minDateTime ? minDateTime : targetStart,
					targetEnd,
					cpmClusterSettings);
				return ids;
			}
		}
		#endregion

		#region +CalculateCpmClusterAndSave CPMクラスタを計算して付与
		/// <summary>
		/// CPMクラスタを計算して付与
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cpmClusterSettings">CPMクラスタ設定</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新したか（変更が無いと更新しない）</returns>
		public bool CalculateCpmClusterAndSave(string userId, CpmClusterSettings cpmClusterSettings, string lastChanged)
		{
			// 更新
			using (var scope = new TransactionScope())
			using (var repository = new UserRepository())
			{
				// ユーザー属性取得
				var attr = repository.GetUsreAttribute(userId);

				// クラスタ算出
				var cluster = CpmCalculator.GetInstance().Calculate(attr, cpmClusterSettings);

				// クラスタ割り当て無し（注文無しなど） or 変更がなければ保存しない
				if ((cluster == null)
					|| ((cluster.Item1.Id == attr.CpmClusterAttribute1)
						&& (cluster.Item2.Id == attr.CpmClusterAttribute2)))
				{
					if (cluster == null)
					{
						repository.DeleteUserAttributeCpmCluster(userId);
						scope.Complete();
					}
					return false;
				}

				// クラスタ更新（先に元の値を待避）
				attr.CpmClusterAttribute1Before = attr.CpmClusterAttribute1;
				attr.CpmClusterAttribute2Before = attr.CpmClusterAttribute2;
				attr.CpmClusterAttribute1 = cluster.Item1.Id;
				attr.CpmClusterAttribute2 = cluster.Item2.Id;
				attr.CpmClusterChangedDate = DateTime.Now;
				attr.LastChanged = lastChanged;
				repository.UpdateUserAttributeCpmCluster(attr);

				scope.Complete();
				return true;
			}
		}
		#endregion

		#region -GetUserCpmClusterReport CPMクラスタレポート取得
		/// <summary>
		/// CPMクラスタレポート取得
		/// </summary>
		/// <returns></returns>
		public CpmClusterReport GetUserCpmClusterReport(CpmClusterSettings settings)
		{
			// データ取得
			var report = GetUserCpmClusterReportData();

			// パーセンテージ計算（データがないものは空のものを作成）
			foreach (var setting1 in settings.Settings1)
			{
				foreach (var setting2 in settings.Settings2)
				{
					var id = setting1.Id + setting2.Id;
					var repotyItem = report.Get(id);
					if (repotyItem == null)
					{
						repotyItem = new CpmClusterReportItem(id);
						report.Items.Add(repotyItem);
					}
					if (repotyItem.Count.HasValue == false) continue;
				}
			}
			return report;
		}
		#endregion

		#region -GetUserCpmClusterReportData CPMクラスタレポートデータ取得
		/// <summary>
		/// CPMクラスタレポートデータ取得
		/// </summary>
		/// <returns></returns>
		private CpmClusterReport GetUserCpmClusterReportData()
		{
			using (var repository = new UserRepository())
			{
				return repository.GetUserCpmClusterReportData();
			}
		}
		#endregion
		#endregion

		#region パスワードリマインダーマスタ
		/// <summary>
		/// パスワードリマインダー取得
		/// </summary>
		/// <param name="authenticationKey">認証キー</param>
		/// <returns>モデル</returns>
		public PasswordReminderModel GetPasswordReminder(string authenticationKey)
		{
			using (var repository = new UserRepository())
			{
				var model = repository.GetPasswordReminder(authenticationKey);
				return model;
			}
		}

		/// <summary>
		/// ユーザーパスワード変更とパスワードリマインダー情報削除も行う
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="password">ユーザーパスワード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateUserAndDeletePasswordReminder(
			string userId,
			string password,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// パスワード変更
				Modify(
					userId,
					model =>
					{
						model.Password = UserPassowordCryptor.PasswordEncrypt(password);
						model.LastChanged = lastChanged;
					},
					updateHistoryAction,
					accessor);

				// パスワードリマインダー情報削除
				DeletePasswordReminder(userId, accessor);

				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// パスワードリマインダー削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeletePasswordReminder(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				repository.DeletePasswordReminder(userId);
			}
		}

		/// <summary>
		/// パスワードリマインダー情報削除・挿入
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <param name="changeUserPasswordTrialLimitCount">変更試行回数制限</param>
		/// <param name="dateCreated">作成日</param>
		public void DeleteInsertPasswordReminder(
			string userId,
			string authenticationKey,
			int changeUserPasswordTrialLimitCount,
			DateTime dateCreated)
		{
			using (var repository = new UserRepository())
			{
				repository.DeleteInsertPasswordReminder(userId, authenticationKey, changeUserPasswordTrialLimitCount, dateCreated);
			}
		}

		/// <summary>
		/// 試行可能回数更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="changeUserPasswordTrialLimitCount">変更可能回数</param>
		public void UpdateChangeUserPasswordTrialLimitCount(string userId, int changeUserPasswordTrialLimitCount)
		{
			using (var repository = new UserRepository())
			{
				repository.UpdateChangeUserPasswordTrialLimitCount(userId, changeUserPasswordTrialLimitCount);
			}
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
			using (var repository = new UserRepository())
			{
				var userErrorPoint = repository.GetErrorPoint(userId);
				return userErrorPoint;
			}
		}
		#endregion

		#region ユーザー統合
		/// <summary>
		/// ユーザー統合フラグを「統合済み」に更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateOnIntegratedFlg(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = UpdateOffIntegratedFlg(
				userId,
				Constants.FLG_USER_INTEGRATED_FLG_DONE,
				lastChanged,
				updateHistoryAction,
				accessor);
			return updated;
		}

		/// <summary>
		/// ユーザー統合フラグを「通常」に更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateOffIntegratedFlg(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = UpdateOffIntegratedFlg(
				userId,
				Constants.FLG_USER_INTEGRATED_FLG_NONE,
				lastChanged,
				updateHistoryAction,
				accessor);
			return updated;
		}

		/// <summary>
		/// ユーザー統合フラグを「通常」に更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="integratedFlg">ユーザー統合フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		private int UpdateOffIntegratedFlg(
			string userId,
			string integratedFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				userId,
				model =>
				{
					model.IntegratedFlg = integratedFlg;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}

		/// <summary>
		/// ユーザー統合対象ユーザーリスト取得
		/// </summary>
		/// <returns>モデル列</returns>
		public UserModel[] GetIntegrationTargetUserList()
		{
			using (var repository = new UserRepository())
			{
				return repository.GetIntegrationTargetUserList();
			}
		}
		#endregion

		#region その他
		/// <summary>
		/// 会員か判定
		/// </summary>
		/// <param name="userKbn">顧客区分</param>
		/// <returns>会員か</returns>
		public static bool IsUser(string userKbn)
		{
			switch (userKbn)
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

		/// <summary>
		/// ゲストか判定
		/// </summary>
		/// <param name="userKbn">顧客区分</param>
		/// <returns>ゲストか</returns>
		public static bool IsGuest(string userKbn)
		{
			switch (userKbn)
			{
				case Constants.FLG_ORDEROWNER_OWNER_KBN_PC_GUEST:
				case Constants.FLG_ORDEROWNER_OWNER_KBN_MOBILE_GUEST:
				case Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_GUEST:
				case Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_GUEST:
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// PCサイト会員か判定（各サイトでのメールアドレス入力チェックなどに利用）
		/// </summary>
		/// <param name="userKbn">顧客区分</param>
		/// <returns>PCサイト会員か</returns>
		public static bool IsPcSiteOrOfflineUser(string userKbn)
		{
			switch (userKbn)
			{
				case Constants.FLG_USER_USER_KBN_PC_USER:
				case Constants.FLG_USER_USER_KBN_SMARTPHONE_USER:
				case Constants.FLG_USER_USER_KBN_OFFLINE_USER:
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// 年齢計算
		/// </summary>
		/// <param name="birthday">生年月日（yyyy/MM/dd）</param>
		/// <returns>年齢</returns>
		public static int CalculateAgeFromBirth(string birthday)
		{
			// 引数の数値変換に失敗した場合は0を返却
			int birthNumber = 0;
			if (int.TryParse(birthday.Replace("/", ""), out birthNumber) == false)
			{
				return 0;
			}

			// 閏年補正（2月29日の場合は2月28日とみなす）
			if (birthNumber.ToString().EndsWith("229"))
			{
				birthNumber--;
			}

			// 現在日を取得
			return (int.Parse(DateTime.Now.ToString("yyyyMMdd")) - birthNumber) / 10000;
		}

		/// <summary>
		/// 新ユーザーID発行
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userIdKey">ユーザーIDキー</param>
		/// <param name="header">ユーザーIDのヘッダー</param>
		/// <param name="length">ユーザーIDの桁数</param>
		/// <returns>新ユーザーID</returns>
		public static string CreateNewUserId(string shopId, string userIdKey, string header, int length)
		{
			long lNewNumber = NumberingUtility.CreateNewNumber(shopId, userIdKey);
			return header + lNewNumber.ToString().PadLeft(length, '0');
		}

		/// <summary>
		/// Create Phone No from 3 part No
		/// </summary>
		/// <param name="telNo1">Tel1</param>
		/// <param name="telNo2">Tel2</param>
		/// <param name="telNo3">Tel3</param>
		/// <returns>Phone format</returns>
		public static string CreatePhoneNo(string telNo1, string telNo2, string telNo3)
		{
			if (string.IsNullOrEmpty(telNo1)) return "";
			if (string.IsNullOrEmpty(telNo2)) return "";
			if (string.IsNullOrEmpty(telNo3)) return "";
			return string.Format("{0}-{1}-{2}", telNo1, telNo2, telNo3);
		}
		#endregion

		#region +UpdateFixedPurchaseMemberFlg 定期会員フラグ更新
		/// <summary>
		/// 定期会員フラグ更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseMemberFlg">定期会員フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UpdateFixedPurchaseMemberFlg(
			string userId,
			string fixedPurchaseMemberFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = UpdateFixedPurchaseMemberFlg(
					userId,
					fixedPurchaseMemberFlg,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion
		#region +UpdateFixedPurchaseMemberFlg 定期会員フラグ更新
		/// <summary>
		/// 定期会員フラグ更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseMemberFlg">定期会員フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		public bool UpdateFixedPurchaseMemberFlg(
			string userId,
			string fixedPurchaseMemberFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				userId,
				model =>
				{
					model.FixedPurchaseMemberFlg = fixedPurchaseMemberFlg;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return (updated > 0);
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
		public UserActivityModel GetUserActivity(string userId, string masterKbn, string masterId)
		{
			using (var repository = new UserRepository())
			{
				var model = repository.GetUserActivity(userId, masterKbn, masterId);
				return model;
			}
		}

		/// <summary>
		/// 管理画面用のアクティビティ数を取得
		/// </summary>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <returns>モデル</returns>
		public int GetUserActivityCountForManager(string masterKbn, string masterId)
		{
			using (var repository = new UserRepository())
			{
				var count = repository.GetUserActivityCountForManager(masterKbn, masterId);
				return count;
			}
		}

		/// <summary>
		/// 管理画面用のアクティビティ数を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>配列</returns>
		public UserActivityModel[] GetUserActivityByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var count = repository.GetUserActivityByUserId(userId);
				return count;
			}
		}

		/// <summary>
		/// ユーザーアクティビティを登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertUserActivity(UserActivityModel model, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				repository.InsertUserActivity(model);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteUserActivity(string userId, string masterKbn, string masterId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				repository.DeleteUserActivity(userId, masterKbn, masterId);
			}
		}

		/// <summary>
		/// 管理画面用削除
		/// </summary>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteUserActivityForManager(string masterKbn, string masterId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				repository.DeleteUserActivityForManager(masterKbn, masterId);
			}
		}

		/// <summary>
		/// ユーザーIDで削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		public void DeleteUserActivityByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				repository.DeleteUserActivityByUserId(userId);
			}
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
			using (var repository = new UserRepository())
			{
				var count = repository.UserManagementLevelUserCount(userManagementLevelId);
				return count;
			}
		}
		#endregion

		#region +SearchUsersForAutosuggest サジェスト用ユーザー検索
		/// <summary>
		/// サジェスト用ユーザー検索
		/// </summary>
		/// <param name="searchWord">検索ワード</param>
		/// <param name="maxCountForDisplay">最大表示件数</param>
		/// <returns>ユーザー一覧</returns>
		public UserModel[] SearchUsersForAutoSuggest(string searchWord, int maxCountForDisplay)
		{
			try
			{
				using (var repository = new UserRepository())
				{
					repository.CommandTimeout = Constants.ORDERREGISTINPUT_SUGGEST_QUERY_TIMEOUT;
					var users = repository.SearchUsersForAutoSuggest(searchWord, maxCountForDisplay);
					return users;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion

		#region +GetUserByReferralCode
		/// <summary>
		/// Get user by referral code
		/// </summary>
		/// <param name="referralCode">Referral code</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>User id</returns>
		public UserModel GetUserByReferralCode(string referralCode, string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserRepository(accessor))
			{
				var result = repository.GetUserByReferralCode(referralCode, userId);
				return result;
			}
		}
		#endregion

		#region +GetReferredUserId
		/// <summary>
		/// Get referred user id
		/// </summary>
		/// <param name="userId">User id</param>
		/// <returns>Referred user id</returns>
		public string GetReferredUserId(string userId)
		{
			using (var repository = new UserRepository())
			{
				var result = repository.GetReferredUserId(userId);
				return result;
			}
		}
		#endregion

		#region +UpdateUserReferralCode
		/// <summary>
		/// Update user referral code
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="referralCode">Referral code</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <returns>True if any user is updated: False</returns>
		public bool UpdateUserReferralCode(
			string userId,
			string referralCode,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var userRepository = new UserRepository())
			{
				var result = userRepository.UpdateUserReferralCode(
					userId,
					referralCode,
					lastChanged);

				// Update history
				if (result
					&& (updateHistoryAction == UpdateHistoryAction.Insert))
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged);
				}

				return result;
			}
		}
		#endregion

		#region +UpdateReferredUserId
		/// <summary>
		/// Update referred user id
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="referredUserId">Referred user id</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <returns>True if any user is updated: False</returns>
		public bool UpdateReferredUserId(
			string userId,
			string referredUserId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var userRepository = new UserRepository())
			{
				var result = userRepository.UpdateReferredUserId(
					userId,
					referredUserId,
					lastChanged);

				// Update history
				if (result && (updateHistoryAction == UpdateHistoryAction.Insert))
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged);
				}

				return result;
			}
		}
		#endregion
	}
}
