/*
=========================================================================================================
  Module      : ユーザー統合登録のためのヘルパクラス (UserIntegrationRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.User;
using w2.Domain.UserIntegration;
using w2.Domain.UserIntegration.Setting;

namespace w2.Domain.UserIntegration.Helper
{
	#region +ユーザー統合登録のためのヘルパクラス
	/// <summary>
	/// ユーザー統合登録のためのヘルパクラス
	/// </summary>
	internal class UserIntegrationRegister
	{
		#region -定数
		/// <summary>メールアドレスキーリスト</summary>
		private static string[] USER_MAIL_ADDR_KEY_LIST = new string[] { Constants.FIELD_USER_MAIL_ADDR, Constants.FIELD_USER_MAIL_ADDR2 };
		/// <summary>電話番号キーリスト</summary>
		private static string[] USER_TEL_KEY_LIST = new string[] { Constants.FIELD_USER_TEL1, Constants.FIELD_USER_TEL2 };
		#endregion

		#region -列挙体
		/// <summary>一致区分</summary>
		private enum MatchKbn
		{
			/// <summary>比較対象外</summary>
			NonTarget,
			/// <summary>一致</summary>
			Match,
			/// <summary>不一致</summary>
			Unmatch
		}
		#endregion

		#region +名寄せを行いユーザー統合情報の登録・更新・削除を行う
		/// <summary>
		/// 名寄せを行いユーザー統合情報の登録・更新・削除を行う
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <param name="parallelWorkerThreads">並列処理ワーカースレッド数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>実行結果メッセージ</returns>
		internal string RegisterUserIntegrationAfterDuplicateIdentification(DateTime targetStart, DateTime targetEnd, int parallelWorkerThreads, string lastChanged)
		{
			// 現在のユーザー統合情報取得
			WriteLog("1.現在のユーザー統合情報取得");
			var allUserIntegrationList = new UserIntegrationService().GetAll();

			// ユーザー統合対象ユーザー取得
			WriteLog("2.ユーザー統合対象ユーザー取得");
			var userList = GetIntegrationTargetUserList(allUserIntegrationList);

			// 名寄せ元のユーザー取得（本バッチの最終実行日時以降に更新されてるユーザー）
			WriteLog("3.名寄せ元のユーザー取得（本バッチの最終実行日時以降に更新されてるユーザー）");
			var targetUserList = userList.Where(u => u.DateChanged >= targetStart && u.DateChanged <= targetEnd).ToList();

			// 名寄せ実行
			WriteLog("4.名寄せ実行");
			SetGroupId(userList, targetUserList, parallelWorkerThreads);

			// ユーザー統合情報更新
			WriteLog("5.ユーザー統合情報更新");
			var execResult = UpdateUserIntegration(userList, targetUserList, allUserIntegrationList, lastChanged);

			// 実行結果メッセージを返す
			WriteLog("6.実行結果メッセージを返す");
			return execResult.GetExecResultMessage();
		}
		#endregion

		#region 名寄せ関連
		/// <summary>
		/// ユーザー統合対象ユーザーリスト取得
		/// </summary>
		/// <param name="allUserIntegrationList">ユーザー統合情報リスト</param>
		/// <returns>ユーザー統合対象ユーザーリスト</returns>
		private UserData[] GetIntegrationTargetUserList(UserIntegrationModel[] allUserIntegrationList)
		{
			// ステータスが「統合保留」のユーザーIDリスト取得
			var suspendUserIdList = allUserIntegrationList.Where(u => u.IsSuspendStatus).SelectMany(u => u.Users).Select(u => u.UserId).ToArray();

			// ユーザー統合対象ユーザー取得
			// ステータスが「統合保留」のユーザーは除外する
			var tempUserList =
				new UserService().GetIntegrationTargetUserList().Where(u => suspendUserIdList.Contains(u.UserId) == false);

			// 利用するデータのみ変数に保持
			var keys = UserIntegrationSetting.MatchCondition.GetUseKeys();
			var userList = tempUserList.Select(u => new UserData(u, keys));

			// メモリを解放
			// 10万件のユーザー情報の場合、400MB => 320MBに圧縮（※メールアドレス、氏名、電話番号、郵便番号の場合）
			// Console.WriteLine(GC.GetTotalMemory(false));	// 利用メモリ確認
			tempUserList = null;
			GC.Collect();

			return userList.ToArray();
		}

		/// <summary>
		/// ユーザー統合情報更新
		/// </summary>
		/// <param name="userList">ユーザー統合対象ユーザーリスト</param>
		/// <param name="targetUserList">名寄せ元ユーザーリスト</param>
		/// <param name="allUserIntegrationList">ユーザー統合情報リスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>実行結果</returns>
		private ExecResult UpdateUserIntegration(UserData[] userList, List<UserData> targetUserList, UserIntegrationModel[] allUserIntegrationList, string lastChanged)
		{
			var execResult = new ExecResult(targetUserList.Count);

			// ステータスが「統合未確定」、「除外」のユーザー統合情報取得
			var noneAndExcludedUserIntegrationList = allUserIntegrationList.Where(u => (u.IsNoneStatus || u.IsExcludedStatus)).ToArray();

			// グループIDでループ
			var service = new UserIntegrationService();
			var targetGroupList = targetUserList.GroupBy(u => u.GroupId).Select(g => new { GroupId = g.Key, Count = g.Count() }).ToArray();
			var targetCount = 0;
			foreach (var targetGroup in targetGroupList)
			{
				targetCount++;
				var messages = string.Format("処理件数：{0}/{1}", targetCount, targetGroupList.Length);

				// 名寄せ（グルーピング）ユーザーリスト取得
				var groupUserList = userList.Where(u => u.GroupId == targetGroup.GroupId).ToArray();
				var groupUserIdList = groupUserList.Select(g => g.UserId).OrderBy(u => u).ToArray();

				// ユーザーの組み合わせが同じ場合、更新処理を行わない
				var userIntegrationList = noneAndExcludedUserIntegrationList.Where(ui => ui.Users.Any(u => groupUserIdList.Contains(u.UserId))).ToArray();
				if (userIntegrationList.Any())
				{
					if (userIntegrationList
						.Select(userIntegration => userIntegration.Users.Select(u => u.UserId).OrderBy(u => u))
						.All(userIdList => groupUserIdList.SequenceEqual(userIdList)))
					{
						WriteLog("5-1." + messages);
						Console.WriteLine(messages);
						continue;
					}
				}
				// ユーザー統合情報更新処理
				using (var accessor = new SqlAccessor())
				{
					// トランザクション：開始
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// ユーザーが2つ以上ある場合は登録
					if (targetGroup.Count >= 2)
					{
						var userIntegration = new UserIntegrationModel
						{
							LastChanged = lastChanged
						};
						userIntegration.Users
							= groupUserList.Select(u =>
								new UserIntegrationUserModel
								{
									UserId = u.UserId,
									LastChanged = lastChanged
								}
								).ToArray();
						service.Insert(userIntegration, accessor);

						// 実行結果格納
						execResult.Set(userIntegration.UserIntegrationNo, ExecResultKbn.Insert);
					}

					// ユーザー統合情報（ステータス：統合未確定 or 除外）に含まれている場合は更新する
					// (1)ユーザーが2件以上の場合：ユーザー統合ユーザー情報を削除
					// (2)ユーザーが1件以下の場合：ユーザー統合情報を削除
					foreach (var userIntegration in userIntegrationList)
					{
						// 対象ユーザーを除外しユーザーリストをセット
						userIntegration.Users = userIntegration.Users.Where(u => (groupUserIdList.Contains(u.UserId) == false)).ToArray();

						// ユーザーが2件以上の場合は更新
						if (userIntegration.Users.Length >= 2)
						{
							userIntegration.LastChanged = lastChanged;
							service.Update(userIntegration, accessor);

							// 実行結果格納
							execResult.Set(userIntegration.UserIntegrationNo, ExecResultKbn.Update);
						}
						// ユーザーが1件以下の場合は削除
						else
						{
							// 削除
							userIntegration.Users = new UserIntegrationUserModel[0];
							service.Delete(userIntegration.UserIntegrationNo, accessor);

							// 実行結果格納
							execResult.Set(userIntegration.UserIntegrationNo, ExecResultKbn.Delete);
						}
					}

					// トランザクション：コミット
					accessor.CommitTransaction();
				}

				WriteLog("5-1." + messages);
				Console.WriteLine(messages);
			}

			return execResult;
		}

		/// <summary>
		/// 一致条件に該当したユーザーにグループIDをセット
		/// </summary>
		/// <param name="userList">ユーザー統合対象ユーザーリスト</param>
		/// <param name="targetUserList">名寄せ元ユーザーリスト</param>
		/// <param name="parallelWorkerThreads">並列処理ワーカースレッド数</param>
		private void SetGroupId(UserData[] userList, List<UserData> targetUserList, int parallelWorkerThreads)
		{
			// ユーザー統合設定取得
			var matchCondition = UserIntegrationSetting.MatchCondition;

			// ★ストップウォッチ：インスタンス作成★
			// var stopwatch = new System.Diagnostics.Stopwatch();

			// 全ての対象ユーザーが名寄せ処理済みになるまで繰り返す
			var groupId = 1;
			while (targetUserList.Any(u => u.IsMatch == false))
			{
				// グループIDが未セットの場合、グループIDをセット
				var targetUser = targetUserList.First(u => (u.IsMatch == false));
				if (targetUser.IsSetGroupId == false)
				{
					targetUser.GroupId = groupId;
					groupId++;
				}

				// ★ストップウォッチ：開始★
				// stopwatch.Start();
				// Console.WriteLine("実行開始：" + stopwatch.Elapsed.ToString());

				// 一致条件に該当するユーザーリスト取得
				var matchUserList = userList.Where(u => (u.UserId != targetUser.UserId) && (u.IsSetGroupId == false));
				if (parallelWorkerThreads > 1)
				{
					// 並列処理
					matchUserList = matchUserList.AsParallel().WithDegreeOfParallelism(parallelWorkerThreads).Where(u => MatchUser(targetUser, u, matchCondition.LogicalOperators));
				}
				else
				{
					// 直列処理
					matchUserList = matchUserList.Where(u => MatchUser(targetUser, u, matchCondition.LogicalOperators));
				}

				// 一致条件に該当するユーザーにグループIDをセット
				foreach (var matchUser in matchUserList)
				{
					matchUser.GroupId = targetUser.GroupId;

					// 一致条件に該当したユーザーを追加し名寄せを行うようにする
					if (targetUserList.Contains(matchUser) == false)
					{
						matchUser.IsMatch = true;
						targetUserList.Add(matchUser);
					}
				}

				// 名寄せ処理済みにする
				targetUser.IsMatch = true;

				// ★ストップウォッチ：終了★
				// Console.WriteLine("実行終了：" + stopwatch.Elapsed.ToString());
				// stopwatch.Stop();
				// stopwatch.Reset();

				var messages = string.Format("処理件数：{0}/{1}", targetUserList.Count(u => u.IsMatch), targetUserList.Count);
				WriteLog("4-1." + messages);
				Console.WriteLine(messages);
			}
		}

		/// <summary>
		/// 一致条件に該当するユーザーかどうか？を返す
		/// </summary>
		/// <param name="targetUser">対象ユーザー情報</param>
		/// <param name="user">比較ユーザー情報</param>
		/// <param name="logicalOperators">論理演算子情報</param>
		/// <returns>一致条件に該当する：true、該当しない：false</returns>
		private bool MatchUser(UserData targetUser, UserData user, LogicalOperatorsData logicalOperators)
		{
			var operators = logicalOperators.Operator;
			var isMatch = (operators == OperatorKbn.And);
			foreach (var match in logicalOperators.MatchList)
			{
				//比較対象外の場合は次の処理へ
				var matchKbn = MatchValue(targetUser, user, match);
				if (matchKbn == MatchKbn.NonTarget) continue;

				// 一致しているか？チェック
				var tempIsMatch = (matchKbn == MatchKbn.Match);
				if (tempIsMatch && (operators == OperatorKbn.Or))
				{
					isMatch = true;
					break;
				}
				if ((tempIsMatch == false) && (operators == OperatorKbn.And))
				{
					isMatch = false;
					break;
				}
			}

			// 一致しているか？チェック（再帰処理）
			foreach (var childLogicalOperators in logicalOperators.LogicalOperatorsList)
			{
				// 一致しているか？チェック
				var tempIsMatch = MatchUser(targetUser, user, childLogicalOperators);
				if (tempIsMatch && (operators == OperatorKbn.Or))
				{
					isMatch = true;
					break;
				}
				if ((tempIsMatch == false) && (operators == OperatorKbn.And))
				{
					isMatch = false;
					break;
				}
			}
			return isMatch;
		}

		/// <summary>
		/// 値が一致しているか？を返す
		/// </summary>
		/// <param name="targetUser">対象ユーザー情報</param>
		/// <param name="user">比較ユーザー情報</param>
		/// <param name="match">一致情報</param>
		/// <returns>一致区分</returns>
		private MatchKbn MatchValue(UserData targetUser, UserData user, MatchData match)
		{
			var targetValue = string.Empty;
			var value = string.Empty;
			var noTarget = true;
			switch (match.Key)
			{
				// メールアドレス（or メールアドレス2）の場合、それぞれ比較
				case Constants.FIELD_USER_MAIL_ADDR:
				case Constants.FIELD_USER_MAIL_ADDR2:
					foreach (var mailAddrKey in USER_MAIL_ADDR_KEY_LIST)
					{
						foreach (var mailAddrKey2 in USER_MAIL_ADDR_KEY_LIST)
						{
							// 比較値がブランクの場合は次の処理へ
							targetValue = StringUtility.ToEmpty(targetUser.Data[mailAddrKey]);
							value = StringUtility.ToEmpty(user.Data[mailAddrKey2]);
							if ((targetValue == string.Empty) && (value == string.Empty))
							{
								continue;
							}

							// ここまでくれば、1度は比較するため、比較対象とする
							noTarget = false;
							if (Match(targetValue, value, match))
							{
								return MatchKbn.Match;
							}
						}
					}
					if (noTarget) return MatchKbn.NonTarget;
					return MatchKbn.Unmatch;

				// 電話番号（or 電話番号2）の場合、それぞれ比較
				case Constants.FIELD_USER_TEL1:
				case Constants.FIELD_USER_TEL2:
					foreach (var telKey in USER_TEL_KEY_LIST)
					{
						foreach (var telKey2 in USER_TEL_KEY_LIST)
						{
							// 比較値がブランクの場合は次の処理へ
							targetValue = StringUtility.ToEmpty(targetUser.Data[telKey]);
							value = StringUtility.ToEmpty(user.Data[telKey2]);
							if ((targetValue == string.Empty) && (value == string.Empty))
							{
								continue;
							}

							// ここまでくれば、1度は比較するため、比較対象とする
							noTarget = false;
							if (Match(targetValue, value, match))
							{
								return MatchKbn.Match;
							}
						}
					}
					if (noTarget) return MatchKbn.NonTarget;
					return MatchKbn.Unmatch;
				default:
					// 値を結合
					foreach (var tempKey in match.Key.Split('+'))
					{
						targetValue += StringUtility.ToEmpty(targetUser.Data[tempKey]);
						value += StringUtility.ToEmpty(user.Data[tempKey]);
					}
					// 比較値がブランクの場合は比較対象外とする
					if ((targetValue == string.Empty) && (value == string.Empty))
					{
						return MatchKbn.NonTarget;
					}
					return (Match(targetValue, value, match)) ? MatchKbn.Match : MatchKbn.Unmatch;
			}
		}

		/// <summary>
		/// 比較区分に応じて値が一致しているか？を返す
		/// </summary>
		/// <param name="targetValue">対象値</param>
		/// <param name="value">比較値</param>
		/// <param name="match">一致情報</param>
		/// <returns>一致する：true、一致しない：false</returns>
		private bool Match(string targetValue, string value, MatchData match)
		{
			switch (match.Qry)
			{
				// 等しいかを返す
				case QryKbn.Eq:
					return (targetValue == value);
				// ○文字まで一致しているかを返す
				case QryKbn.Sw:
					var length = match.Length.Value;
					if ((targetValue.Length >= length) && (value.Length >= length))
					{
						return (targetValue.Substring(0, length) == value.Substring(0, length));
					}
					return false;
			}
			return false;
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="messages">メッセージ</param>
		private void WriteLog(string messages)
		{
			FileLogger.WriteInfo(messages);
			Console.WriteLine(messages);
		}

		#endregion

		#region ユーザーデータクラス
		private class UserData
		{
			#region +コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="user">ユーザー情報</param>
			/// <param name="keys">利用キー列</param>
			public UserData(UserModel user, string[] keys)
			{
				this.UserId = user.UserId;
				this.DateChanged = user.DateChanged;
				this.Data = new Hashtable();
				foreach (var key in keys) this.Data[key] = user.DataSource[key];
				this.GroupId = 0;
				this.IsMatch = false;
			}
			#endregion

			#region プロパティ
			/// <summary>ユーザーID</summary>
			public string UserId { get; set; }
			/// <summary>更新日時</summary>
			public DateTime DateChanged { get; set; }
			/// <summary>データ</summary>
			public Hashtable Data { get; set; }
			/// <summary>グループID</summary>
			public int GroupId { get; set; }
			/// <summary>グループIDセット済みか?</summary>
			public bool IsSetGroupId
			{
				get { return this.GroupId != 0; }
			}
			/// <summary>名寄せ処理済みか?</summary>
			public bool IsMatch { get; set; }
			#endregion
		}
		#endregion

		#region 実行結果格納用クラス
		/// <summary>実行結果区分</summary>
		private enum ExecResultKbn
		{
			/// <summary>登録</summary>
			Insert,
			/// <summary>更新</summary>
			Update,
			/// <summary>削除</summary>
			Delete
		}

		/// <summary>
		/// 実行結果格納用クラス
		/// </summary>
		private class ExecResult
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="targetUserListCount">対象ユーザーリスト数</param>
			public ExecResult(int targetUserListCount)
			{
				this.TargetUserListCount = targetUserListCount;
				this.ExecResultDataList = new List<ExecResultData>();
			}

			/// <summary>
			/// 実行結果をセット
			/// </summary>
			/// <param name="userIntegrationNo">ユーザー統合No</param>
			/// <param name="execResultKbn">実行結果区分</param>
			public void Set(long userIntegrationNo, ExecResultKbn execResultKbn)
			{
				IEnumerable<ExecResultData> execResult;
				switch (execResultKbn)
				{
					case ExecResultKbn.Insert:
						this.ExecResultDataList.Add(new ExecResultData(userIntegrationNo, execResultKbn));
						break;
					case ExecResultKbn.Update:
						execResult = this.ExecResultDataList.Where(e => e.UserIntegrationNo == userIntegrationNo).ToArray();
						if (execResult.Any() == false)
						{
							this.ExecResultDataList.Add(new ExecResultData(userIntegrationNo, execResultKbn));
						}
						break;
					case ExecResultKbn.Delete:
						// 更新した実行結果が存在する場合は削除
						execResult = this.ExecResultDataList.Where(e => e.UserIntegrationNo == userIntegrationNo);
						if (execResult.Any())
						{
							this.ExecResultDataList.Remove(execResult.ToArray()[0]);
						}
						this.ExecResultDataList.Add(new ExecResultData(userIntegrationNo, execResultKbn));
						break;
				}
			}

			/// <summary>
			/// 実行結果メッセージ取得
			/// </summary>
			/// <returns>実行結果メッセージ</returns>
			public string GetExecResultMessage()
			{
				return string.Format(
						"■ユーザー情報\r\n"
						+ "名寄せ対象数：{0}件\r\n"
						+ "\r\n"
						+ "■ユーザー統合情報\r\n"
						+ "登録数：{1}件\r\n"
						+ "更新数：{2}件\r\n"
						+ "削除数：{3}件\r\n",
						this.TargetUserListCount,
						this.ExecResultDataList.Where(e => e.ExecResultKbn == ExecResultKbn.Insert).ToArray().Length,
						this.ExecResultDataList.Where(e => e.ExecResultKbn == ExecResultKbn.Update).ToArray().Length,
						this.ExecResultDataList.Where(e => e.ExecResultKbn == ExecResultKbn.Delete).ToArray().Length
					);
			}

			#region プロパティ
			/// <summary>対象ユーザーリスト数</summary>
			public int TargetUserListCount { get; set; }
			/// <summary>実行結果格納用リスト</summary>
			public List<ExecResultData> ExecResultDataList { get; set; }
			#endregion
		}

		/// <summary>
		/// 実行結果データ格納用クラス
		/// </summary>
		private class ExecResultData
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="userIntegrationNo">ユーザー統合No</param>
			/// <param name="execResultKbn">実行結果区分</param>
			public ExecResultData(long userIntegrationNo, ExecResultKbn execResultKbn)
			{
				this.UserIntegrationNo = userIntegrationNo;
				this.ExecResultKbn = execResultKbn;
			}

			#region プロパティ
			/// <summary>ユーザー統合No</summary>
			public long UserIntegrationNo { get; set; }
			/// <summary>実行区分</summary>
			public ExecResultKbn ExecResultKbn { get; set; }
			#endregion
		}
		#endregion
	}
	#endregion
}
