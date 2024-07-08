/*
=========================================================================================================
  Module      : ユーザーサービスのインタフェース(IUserService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User.Helper;
using w2.Domain.UserEasyRegisterSetting;
using w2.Domain.UserExtendSetting;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザーサービスのインタフェース
	/// </summary>
	public interface IUserService : IService
	{
		/// <summary>
		/// ユーザー検索
		/// </summary>
		/// <param name="cond">ユーザー検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		UserSearchResult[] Search(UserSearchCondition cond);

		/// <summary>
		/// ユーザー検索件数取得
		/// </summary>
		/// <param name="cond">ユーザー検索条件クラス</param>
		/// <returns>検索件数</returns>
		int GetSearchHitCount(UserSearchCondition cond);

		/// <summary>
		/// ユーザー情報取得 (SqlAccessor指定)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>モデル</returns>
		UserModel Get(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 複数ユーザ取得
		/// </summary>
		/// <param name="userIds">取得対象のユーザの配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// <returns>モデル</returns>
		IEnumerable<UserModel> GetUsers(IEnumerable<string> userIds, SqlAccessor accessor);

		/// <summary>
		/// ユーザー情報を拡張項目の値から取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザー</returns>
		UserModel GetByExtendColumn(string columnName, string value, SqlAccessor accessor = null);

		/// <summary>
		/// 退会していないユーザー情報を拡張項目の値から取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="value">値</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザー</returns>
		UserModel GetRegisteredUserByExtendColumn(string columnName, string value, SqlAccessor accessor = null);

		/// <summary>
		/// 会員ランクID取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>会員ランクID</returns>
		string GetMemberRankId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// メール配信のためユーザ情報取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>メール配信用のユーザー情報</returns>
		UserForMailSend GetUserForMailSend(string userId);

		/// <summary>
		/// マスタアップロードバッチ用 
		/// ログインID重複ユーザ情報取得
		/// </summary>
		/// <returns>ログインID重複ユーザモデル列</returns>
		DuplicationLoginId[] GetDuplicationLoginIdList();

		/// <summary>
		/// ユーザーシンボル取得
		/// </summary>
		/// <param name="userIds">ユーザーIDリスト</param>
		/// <returns>ユーザーシンボルリスト（0件の場合は0配列）</returns>
		UserSymbols[] GetUserSymbols(params string[] userIds);

		/// <summary>
		/// ログイン試行
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="rawPassword">パスワード(raw)</param>
		/// <param name="mailaddressEnabled">メールアドレスのログインID利用有無</param>
		/// <returns>ユーザー情報:成功、null:失敗</returns>
		UserModel TryLogin(string loginId, string rawPassword, bool mailaddressEnabled);

		/// <summary>
		/// メールアドレスでユーザー情報取得 (Batch.CsMailReceiver用、CSManager用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserModel GetUserByMailAddr(string mailAddr, SqlAccessor accessor = null);

		/// <summary>
		/// メールアドレスでユーザー情報取得 (複数件)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル配列</returns>
		UserModel[] GetUsersByMailAddr(string mailAddr, SqlAccessor accessor = null);

		/// <summary>
		/// メールアドレスでユーザー情報取得 (メールマガジン登録用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		UserModel GetUserByMailAddrForMailMagazineRegister(string mailAddr);

		/// <summary>
		/// メールアドレスでユーザー情報取得 (メールマガジン解除用)
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>モデル</returns>
		UserModel GetUserByMailAddrForMailMagazineCancel(string mailAddr);

		/// <summary>
		/// ユーザID取得 (MallBatch.MailOrderGetter)
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザーID</returns>
		string GetUserId(string mallId, string mailAddr, SqlAccessor accessor);

		/// <summary>
		/// パスワードリマインダー用ユーザ情報取得
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="mailaddressEnabled">メールアドレスのログインID利用有無</param>
		/// <returns>モデル</returns>
		UserModel GetUserForReminder(string loginId, string mailAddr, bool mailaddressEnabled);

		/// <summary>
		/// ユーザー情報取得 (SqlAccessor指定)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>ユーザー関連データ</returns>
		UserRelationDatas GetWithUserRelationDatas(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// Get Modify User Ids
		/// </summary>
		/// <param name="updateAt">Update At</param>
		/// <param name="limit">Limit</param>
		/// <param name="offset">Offset</param>
		/// <param name="sortType">Sort Type</param>
		/// <returns>Modify User Ids</returns>
		string[] GetModifyUsers(DateTime updateAt, int limit, int offset, string sortType);

		/// <summary>
		/// Get users for Letro
		/// </summary>
		/// <param name="userIds">User ids</param>
		/// <returns>Users for Letro</returns>
		DataView GetUsersForLetro(IEnumerable<string> userIds);

		/// <summary>
		/// ログインＩＤ重複チェック
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="mailAddressEnabled">メールアドレスのログインID利用有無（1:有効 、0:無効）</param>
		/// <param name="userId">ユーザーID（指定しない場合：登録チェック、指定する場合：更新チェック）</param>
		/// <returns>true:重複しない、false:重複する</returns>
		bool CheckDuplicationLoginId(string loginId, string mailAddressEnabled, string userId = null);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		bool InsertWithUserExtend(
			UserModel user,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		bool InsertWithUserExtend(
			UserModel user,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

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
		UserModel[] GetUsersForLine(
			string columnName,
			string userId,
			string name,
			string nameKana,
			string tel,
			string mailId,
			bool isSearchWithLineUserId);

		/// <summary>
		/// ユーザー情報のみ登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Insert(UserModel user, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザー情報更新（拡張項目とともに）
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		bool UpdateWithUserExtend(
			UserModel user,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// ユーザー情報更新（拡張項目とともに）
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		bool UpdateWithUserExtend(
			UserModel user,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			string userId,
			Action<UserModel> updateAction,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			string userId,
			Action<UserModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新結果</returns>
		bool ModifyUserExtend(
			string userId,
			Action<UserExtendModel> updateAction,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		bool ModifyUserExtend(
			string userId,
			Action<UserExtendModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザー情報のみ更新
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Update(UserModel user, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

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
		bool UpdateMailFlg(
			string userId,
			string remoteAddr,
			string mailAddr,
			string mailFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 最終ログイン日時更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		bool UpdateLoginDate(string userId, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// ユーザ退会
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		bool UserWithdrawal(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// モバイルユーザID更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="career">キャリアID</param>
		/// <param name="mobileUID">モバイルユーザID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		bool UpdateMobileUID(
			string userId,
			string career,
			string mobileUID,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// メールマガジン解除
		/// </summary>
		/// <param name="userId">ユーザーID（履歴更新用）</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		bool MailMagazineCancel(
			string userId,
			string mailAddr,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// ユーザー情報削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true: 成功、false:失敗</returns>
		bool Delete(string userId, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 初回広告コードを更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		bool UpdateAdvCodeFirst(
			string userId,
			string advCodeFirst,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// ユーザメモ＆ユーザー管理レベルID更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userMemo">ユーザメモ</param>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新したか</returns>
		bool UpdateUserMemoAndUserManagementLevelId(
			string userId,
			string userMemo,
			string userManagementLevelId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

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
		bool UpdateUserMemoAndUserManagementLevelId(
			string userId,
			string userMemo,
			string userManagementLevelId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 外部ユーザーIDでユーザー情報取得
		/// </summary>
		/// <param name="columnName">カラム名</param>
		/// <param name="externalUserId">外部ユーザーID</param>
		/// <returns>ユーザーモデル</returns>
		UserModel GetUserByExternalUserId(string columnName, string externalUserId);

		/// <summary>
		/// 最終誕生日ポイント付与年更新
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateUserBirthdayPointAddYear(
			UserModel user,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 誕生日クーポン発行年更新
		/// </summary>
		/// <param name="user">モデル</param>
		/// <param name="updateHistoryAction">更新するか</param>
		/// <param name="accessor">アクセサー</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新件数</returns>
		int UpdateUserBirthdayCouponPublishYear(
			UserModel user,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null,
			string lastChanged = "");

		/// <summary>
		/// 誕生日クーポン発行年更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>更新件数件数</returns>
		int UpdateUserBirthdayCouponPublishYearByCouponInfo(
			string userId,
			string lastChanged,
			SqlAccessor accessor);

		/// <summary>
		/// リアルタイム累計購入回数更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="realTimeOrderCount">累計購入回数</param>
		/// <param name="accessor">アクセサー</param>
		/// <returns>更新件数</returns>
		int UpdateRealTimeOrderCount(string userId, int realTimeOrderCount, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザー拡張項目取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserExtendModel GetUserExtend(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザー拡張項目更新
		/// </summary>
		/// <param name="model">ユーザー拡張項目</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateUserExtend(UserExtendModel model, string userId, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// ユーザー拡張項目更新(更新履歴登録も実行)
		/// </summary>
		/// <param name="model">ユーザー拡張項目</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		bool UpdateUserExtend(UserExtendModel model, string userId, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// カラムの存在チェック
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <returns>true:存在する、false:存在しない</returns>
		bool UserExtendColumnExists(string settingId);

		/// <summary>
		/// 一時テーブル（w2_UserExtend_tmp）を作成
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		bool CreateUserExtendTempTable(SqlAccessor accessor);

		/// <summary>
		/// 一時テーブルから項目削除
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		bool RemoveUserExtendColumn(string settingId, SqlAccessor accessor);

		/// <summary>
		/// 一時テーブルに項目追加
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		bool AddUserExtendColumn(string settingId, SqlAccessor accessor);

		/// <summary>
		/// 一時テーブルの追加・削除対象ではない項目に対してデフォルト制約追加
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		bool ReDefinitionUserExtendColumn(string settingId, SqlAccessor accessor);

		/// <summary>
		/// 元テーブル（w2_UserExtend）をDROPし、一時テーブルをリネーム（w2_UserExtend_tmp -> w2_UserExtend）
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		bool ReDefinitionUserExtend(SqlAccessor accessor);

		/// <summary>
		/// カラムのリネーム
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		bool RenameUserExtendColumn(SqlAccessor accessor);

		/// <summary>
		/// ユーザ拡張項目設定の削除/新規/更新 処理
		/// </summary>
		/// <param name="userExtendSettingList">ユーザ拡張項目設定一覧</param>
		/// <returns>true : 成功 / false : 失敗</returns>
		bool UserExtendSettingExecuteAll(UserExtendSettingList userExtendSettingList);

		/// <summary>
		/// ユーザ拡張項目設定一覧取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザ拡張項目モデル一覧</returns>
		UserExtendSettingList GetUserExtendSettingList(SqlAccessor accessor = null);

		/// <summary>
		/// ユーザ拡張項目設定一覧取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザ拡張項目モデル配列</returns>
		UserExtendSettingModel[] GetUserExtendSettingArray(SqlAccessor accessor = null);

		/// <summary>
		/// ユーザ拡張項目設定取得
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>モデル</returns>
		UserExtendSettingModel GetUserExtendSetting(string settingId);

		/// <summary>
		/// ユーザ拡張項目設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		bool InsertUserExtendSetting(UserExtendSettingModel model, SqlAccessor accessor);

		/// <summary>
		/// ユーザ拡張項目設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		bool UpdateUserExtendSetting(UserExtendSettingModel model, SqlAccessor accessor);

		/// <summary>
		/// ユーザ拡張項目設定削除
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true: 成功、false:失敗</returns>
		bool DeleteUserExtendSetting(string settingId, SqlAccessor accessor);

		/// <summary>
		/// かんたん会員登録設定取得
		/// </summary>
		/// <param name="itemId">項目ID</param>
		/// <returns>モデル</returns>
		UserEasyRegisterSettingModel GetUserEasyRegisterSetting(string itemId);

		/// <summary>
		/// かんたん会員登録設定取得
		/// </summary>
		/// <returns>モデルの配列</returns>
		UserEasyRegisterSettingModel[] GetUserEasyRegisterSettingList();

		/// <summary>
		/// かんたん会員登録設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		bool UpdateUserEasyRegisterSetting(UserEasyRegisterSettingModel model, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザー属性取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザー属性モデル</returns>
		UserAttributeModel GetUserAttribute(string userId);

		/// <summary>
		/// 注文更新日から対象ユーザーID取得
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <returns>ユーザーID</returns>
		string[] GetUserIdsByOrderChanged(DateTime targetStart, DateTime targetEnd);

		/// <summary>
		///  ユーザー属性受注情報作成＆更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新したか</returns>
		bool InsertUpdateUserAttributeOrderInfo(string userId, string lastChanged);

		/// <summary>
		/// 空のユーザー属性作成
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>実行件数</returns>
		bool CreateEmptyUserAttribute(string lastChanged);

		/// <summary>
		/// CPMクラスタ付与向けユーザーID取得
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <param name="cpmClusterSettings">CPMクラスタ値設定</param>
		/// <returns>ユーザーIDリスト</returns>
		string[] GetUserIdsForSetCpmCluster(DateTime targetStart, DateTime targetEnd,
			CpmClusterSettings cpmClusterSettings);

		/// <summary>
		/// CPMクラスタを計算して付与
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cpmClusterSettings">CPMクラスタ設定</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新したか（変更が無いと更新しない）</returns>
		bool CalculateCpmClusterAndSave(string userId, CpmClusterSettings cpmClusterSettings, string lastChanged);

		/// <summary>
		/// CPMクラスタレポート取得
		/// </summary>
		/// <returns></returns>
		CpmClusterReport GetUserCpmClusterReport(CpmClusterSettings settings);

		/// <summary>
		/// パスワードリマインダー取得
		/// </summary>
		/// <param name="authenticationKey">認証キー</param>
		/// <returns>モデル</returns>
		PasswordReminderModel GetPasswordReminder(string authenticationKey);

		/// <summary>
		/// ユーザーパスワード変更とパスワードリマインダー情報削除も行う
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="password">ユーザーパスワード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateUserAndDeletePasswordReminder(
			string userId,
			string password,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// パスワードリマインダー削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeletePasswordReminder(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// パスワードリマインダー情報削除・挿入
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <param name="changeUserPasswordTrialLimitCount">変更試行回数制限</param>
		/// <param name="dateCreated">作成日</param>
		void DeleteInsertPasswordReminder(
			string userId,
			string authenticationKey,
			int changeUserPasswordTrialLimitCount,
			DateTime dateCreated);

		/// <summary>
		/// 試行可能回数更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="changeUserPasswordTrialLimitCount">変更可能回数</param>
		void UpdateChangeUserPasswordTrialLimitCount(string userId, int changeUserPasswordTrialLimitCount);

		/// <summary>
		/// エラーポイント取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		UserErrorPoint GetErrorPoint(string userId);

		/// <summary>
		/// ユーザー統合対象ユーザーリスト取得
		/// </summary>
		/// <returns>モデル列</returns>
		UserModel[] GetIntegrationTargetUserList();

		/// <summary>
		/// 定期会員フラグ更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseMemberFlg">定期会員フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true:成功、false:失敗</returns>
		bool UpdateFixedPurchaseMemberFlg(
			string userId,
			string fixedPurchaseMemberFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 定期会員フラグ更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseMemberFlg">定期会員フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true:成功、false:失敗</returns>
		bool UpdateFixedPurchaseMemberFlg(
			string userId,
			string fixedPurchaseMemberFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザーアクティビティを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <returns>モデル</returns>
		UserActivityModel GetUserActivity(string userId, string masterKbn, string masterId);

		/// <summary>
		/// 管理画面用のアクティビティ数を取得
		/// </summary>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <returns>モデル</returns>
		int GetUserActivityCountForManager(string masterKbn, string masterId);

		/// <summary>
		/// 管理画面用のアクティビティ数を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>配列</returns>
		UserActivityModel[] GetUserActivityByUserId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーアクティビティを登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertUserActivity(UserActivityModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeleteUserActivity(string userId, string masterKbn, string masterId, SqlAccessor accessor = null);

		/// <summary>
		/// 管理画面用削除
		/// </summary>
		/// <param name="masterKbn">マスター区分</param>
		/// <param name="masterId">マスターID</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeleteUserActivityForManager(string masterKbn, string masterId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーIDで削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		void DeleteUserActivityByUserId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// サジェスト用ユーザー検索
		/// </summary>
		/// <param name="searchWord">検索ワード</param>
		/// <param name="maxCountForDisplay">最大表示件数</param>
		/// <returns>ユーザー一覧</returns>
		UserModel[] SearchUsersForAutoSuggest(string searchWord, int maxCountForDisplay);

		/// <summary>
		/// Get user id by referral code
		/// </summary>
		/// <param name="referralCode">Referral code</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>User id</returns>
		UserModel GetUserByReferralCode(string referralCode, string userId, SqlAccessor accessor = null);

		/// <summary>
		/// Get referred user id
		/// </summary>
		/// <param name="userId">User id</param>
		/// <returns>Referred user id</returns>
		string GetReferredUserId(string userId);

		/// <summary>
		/// Update user referral code
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="referralCode">Referral code</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <returns>True if any user is updated: False</returns>
		bool UpdateUserReferralCode(
			string userId,
			string referralCode,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// Update referred user id
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="referredUserId">Referred user id</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <returns>True if any user is updated: False</returns>
		bool UpdateReferredUserId(
			string userId,
			string referredUserId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);
	}
}
