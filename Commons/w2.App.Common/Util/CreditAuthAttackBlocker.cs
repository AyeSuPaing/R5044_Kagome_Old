/*
=========================================================================================================
  Module      : クレジット与信アタックブロッカー(CreditAuthAttackBlocker.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using w2.Domain.TempDatas;

namespace w2.App.Common.Util
{
	/// <summary>
	/// クレジット与信アタックブロックマネージャ
	/// </summary>
	public class CreditAuthAttackBlocker
	{
		/// <summary>テンポラリデータサービス</summary>
		private readonly TempDatasService m_tempDataService = new TempDatasService();
		/// <summary>テンポラリデータタイプ</summary>
		private readonly TempDatasService.TempType m_tempType;
		/// <summary>インスタンス</summary>
		private static CreditAuthAttackBlocker m_instantce;

		/// <summary>
		/// コンストラクタ（外部から利用させない）
		/// </summary>
		private CreditAuthAttackBlocker(Constants.CreditAuthAtackBlockMethod method)
		{
			switch (method)
			{
				case Constants.CreditAuthAtackBlockMethod.User:
					m_tempType = TempDatasService.TempType.CreditAuthErrorInfoUserId;
					break;

				case Constants.CreditAuthAtackBlockMethod.IP:
				default:
					m_tempType = TempDatasService.TempType.CreditAuthErrorInfoIpAddress;
					break;
			}
		}

		/// <summary>
		/// 試行可能回数を-1する
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="ipAddresss">IPアドレス</param>
		/// <returns>ロックがかかっているか？</returns>
		public void DecreasePossibleTrialCount(string userId, string ipAddresss)
		{
			if (Constants.CONST_POSSIBLE_TRIAL_CREDIT_AUTH_ATTACK_COUNT.HasValue == false) return;
			if (IsGuestUserByCheckUserId(userId)) return;

			var tempKey = IsLockUserId() ? userId : ipAddresss;
			m_tempDataService.DecreaseCountForCountValue(
				m_tempType,
				tempKey,
				Constants.CONST_POSSIBLE_TRIAL_CREDIT_AUTH_ATTACK_COUNT.Value,
				Constants.CONST_CREDIT_AUTH_ATTACK_LOCK_VALID_MINUTES);
		}

		/// <summary>
		/// ロックされているか
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="ipAddresss">IPアドレス</param>
		/// <returns>アカウントロックフラグ</returns>
		public bool IsLocked(string userId, string ipAddresss)
		{
			if (Constants.CONST_POSSIBLE_TRIAL_CREDIT_AUTH_ATTACK_COUNT.HasValue == false) return false;
			if (IsGuestUserByCheckUserId(userId)) return false;

			var tempKey = IsLockUserId() ? userId : ipAddresss;
			var errorInfo = m_tempDataService.Resotre(
				m_tempType,
				tempKey,
				Constants.CONST_CREDIT_AUTH_ATTACK_LOCK_VALID_MINUTES);
			var result = ((errorInfo != null) && ((int)errorInfo.TempDataDeserialized <= 0));
			return result;
		}

		/// <summary>
		/// ロック解除
		/// </summary>
		/// <param name="cache">キャッシュ</param>
		/// <param name="ipAddresss">IPアドレス</param>
		/// <param name="userId">ユーザーID</param>
		public void CancelLock(Cache cache, string userId, string ipAddresss)
		{
			if (Constants.CONST_POSSIBLE_TRIAL_CREDIT_AUTH_ATTACK_COUNT.HasValue == false) return;
			if (IsGuestUserByCheckUserId(userId)) return;

			var tempKey = IsLockUserId() ? userId : ipAddresss;
			m_tempDataService.Delete(m_tempType, tempKey);
		}

		/// <summary>
		/// Check lock User or IP address
		/// </summary>
		/// <returns>True: Lock UserId, False: Lock IP address</returns>
		private bool IsLockUserId()
		{
			var result = (m_tempType == TempDatasService.TempType.CreditAuthErrorInfoUserId);
			return result;
		}

		/// <summary>
		/// ユーザーIDチェックモードでのゲストユーザーかどうか
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ゲストユーザーかどうか</returns>
		private bool IsGuestUserByCheckUserId(string userId)
		{
			if (IsLockUserId() == false) return false;
			if (string.IsNullOrEmpty(userId) == false) return false;
			return true;
		}

		/// <summary>
		/// インスタンス取得
		/// </summary>
		/// <returns>インスタンス</returns>
		public static CreditAuthAttackBlocker Instance
		{
			get
			{
				if (m_instantce == null)
				{
					m_instantce = new CreditAuthAttackBlocker(Constants.CONST_CREDIT_AUTH_ATTACK_BLOCK_METHOD.Value);
				}

				return m_instantce;
			}
		}
	}
}
