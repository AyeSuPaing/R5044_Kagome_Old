/*
=========================================================================================================
  Module      : ログインアカウントロックマネージャクラス(LoginAccountLockManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Domain.TempDatas;

namespace w2.App.Common.User
{
	/// <summary>
	/// ログインアカウントロックマネージャ
	/// </summary>
	public class LoginAccountLockManager
	{
		/// <summary>ロックオブジェクト</summary>
		private static readonly object m_lockObject = new object();
		/// <summary>実体</summary>
		private static readonly LoginAccountLockManager m_accountLockManager = new LoginAccountLockManager();
		/// <summary>テンポラリデータサービス</summary>
		private readonly TempDatasService m_tempDataService = new TempDatasService();

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private LoginAccountLockManager()
		{
		}

		/// <summary>
		/// インスタンス取得
		/// </summary>
		/// <returns>インスタンス</returns>
		public static LoginAccountLockManager GetInstance()
		{
			return m_accountLockManager;
		}

		/// <summary>
		/// アカウントロックデータのログイン試行可能回数更新
		/// </summary>
		/// <param name="ip">IPアドレス</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>アカウントロックがかかっていない：true　アカウントロックがかかっている：false</returns>
		public void UpdateLockPossibleTrialLoginCount(string ip, string loginId, string password)
		{
			// 排他制御
			lock (m_lockObject)
			{
				// TempDataキーの設定（「IP＋ログインID」、「IP＋パスワード」）
				var loginErrorInfoKeyForLoginId = CreateErrorInfoKeyForLoginId(ip, loginId);
				var loginErrorInfoKeyForPassword = CreateErrorInfoKeyForPassword(ip, password);

				// ログインエラー情報取得
				var loginErrorInfos = GetLoginErrorInfos(loginErrorInfoKeyForLoginId, loginErrorInfoKeyForPassword);
				var loginErrorInfoForLoginId = loginErrorInfos.Item1;
				var loginErrorInfoForPassword = loginErrorInfos.Item2;

				// 新規チェック（「IP＋ログインID」、「IP＋パスワード」の両方ともキーが存在しない場合、初期値を設定）
				if ((loginErrorInfoForLoginId == null) && (loginErrorInfoForPassword == null))
				{
					m_tempDataService.Save(
						TempDatasService.TempType.LoginErrorInfoLoginId,
						loginErrorInfoKeyForLoginId,
						Constants.CONST_POSSIBLE_TRIAL_LOGIN_COUNT - 1);
					m_tempDataService.Save(
						TempDatasService.TempType.LoginErrorInfoPassword,
						loginErrorInfoKeyForPassword,
						Constants.CONST_POSSIBLE_TRIAL_LOGIN_COUNT - 1);
				}
				// 「IP＋ログインID」、「IP＋パスワード」の個別チェック
				else
				{
					// IP＋ログインIDでエラーログイン情報が存在した場合、ログイン試行可能回数を１減らす
					if (loginErrorInfoForLoginId != null)
					{
						m_tempDataService.Save(
							TempDatasService.TempType.LoginErrorInfoLoginId,
							loginErrorInfoKeyForLoginId,
							(int)loginErrorInfoForLoginId.TempDataDeserialized - 1);
					}

					// IP＋パスワードでエラーログイン情報が存在した場合、ログイン試行可能回数を１減らす
					if (loginErrorInfoForPassword != null)
					{
						m_tempDataService.Save(
							TempDatasService.TempType.LoginErrorInfoPassword,
							loginErrorInfoKeyForPassword,
							(int)loginErrorInfoForPassword.TempDataDeserialized - 1);
					}
				}
			}
		}

		/// <summary>
		/// ログインアカウントロックチェック
		/// </summary>
		/// <param name="ip">IPアドレス</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>アカウントロックフラグ</returns>
		public bool IsAccountLocked(string ip, string loginId, string password)
		{
			// 排他制御
			lock (m_lockObject)
			{
				// アカウントロックデータキーの設定（「IP＋ログインID」、「IP＋パスワード」）
				var loginErrorInfoKeyForLoginId = CreateErrorInfoKeyForLoginId(ip, loginId);
				var loginErrorInfoKeyForPassword = CreateErrorInfoKeyForPassword(ip, password);
				var loginErrorInfos = GetLoginErrorInfos(loginErrorInfoKeyForLoginId, loginErrorInfoKeyForPassword);
				var loginErrorInfoForLoginId = loginErrorInfos.Item1;
				var loginErrorInfoForPassword = loginErrorInfos.Item2;

				// 「IP＋ログインID」、「IP＋パスワード」のどちらかでログイン試行可能回数が0の場合はアカウントロック中
				if (((loginErrorInfoForLoginId != null) && ((int)loginErrorInfoForLoginId.TempDataDeserialized <= 0))
					|| ((loginErrorInfoForPassword != null) && ((int)loginErrorInfoForPassword.TempDataDeserialized <= 0)))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// ログインエラー情報取得
		/// </summary>
		/// <param name="ipAndLoginId">IP＋ログインIDキー</param>
		/// <param name="ipAndPassword">IP＋パスワードキー</param>
		/// <returns>ログインエラー情報（IP+ログインID、IP+パスワード）</returns>
		private Tuple<TempDatasModel, TempDatasModel> GetLoginErrorInfos(string ipAndLoginId, string ipAndPassword)
		{
			return new Tuple<TempDatasModel, TempDatasModel>(
				m_tempDataService.Resotre(TempDatasService.TempType.LoginErrorInfoLoginId, ipAndLoginId, Constants.CONST_ACCOUNT_LOCK_VALID_MINUTES),
				m_tempDataService.Resotre(TempDatasService.TempType.LoginErrorInfoPassword, ipAndPassword, Constants.CONST_ACCOUNT_LOCK_VALID_MINUTES));
		}

		/// <summary>
		/// アカウントロック解除
		/// </summary>
		/// <param name="ip">IPアドレス</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		public void CancelAccountLock(string ip, string loginId, string password)
		{
			// 排他制御
			lock (m_lockObject)
			{
				m_tempDataService.Delete(TempDatasService.TempType.LoginErrorInfoLoginId, CreateErrorInfoKeyForLoginId(ip, loginId));
				m_tempDataService.Delete(TempDatasService.TempType.LoginErrorInfoPassword, CreateErrorInfoKeyForPassword(ip, password));
			}
		}

		/// <summary>
		/// ログインID向けログインエラー情報アカウントロックデータキー作成
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="loginId">ログインID</param>
		/// <returns>アカウントロックデータキー</returns>
		private string CreateErrorInfoKeyForLoginId(string ip, string loginId)
		{
			return ip + Constants.ACOUNT_LOCK_KEY_LOGINERRORINFO_MIDDLE_STRING_LOGIN_ID + loginId;
		}

		/// <summary>
		/// パスワード向けログインエラー情報アカウントロックデータキー作成
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="password">パスワード</param>
		/// <returns>アカウントロックデータキー</returns>
		private string CreateErrorInfoKeyForPassword(string ip, string password)
		{
			return ip + Constants.ACOUNT_LOCK_KEY_LOGINERRORINFO_MIDDLE_STRING_PASSWORD + password;
		}
	}
}
