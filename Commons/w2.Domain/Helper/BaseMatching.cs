/*
=========================================================================================================
  Module      : マッチング基底クラス (BaseMatching.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Util;

namespace w2.Domain.Helper
{
	/// <summary>マッチング基底クラス</summary>
	public class BaseMatching
	{
		#region -定数
		/// <summary>メールアドレスキーリスト</summary>
		private static readonly string[] USER_MAIL_ADDR_KEY_LIST = { Constants.FIELD_USER_MAIL_ADDR, Constants.FIELD_USER_MAIL_ADDR2 };
		/// <summary>電話番号キーリスト</summary>
		private static readonly string[] USER_TEL_KEY_LIST = { Constants.FIELD_USER_TEL1, Constants.FIELD_USER_TEL2 };
		#endregion

		#region #列挙体
		/// <summary>一致区分</summary>
		protected enum MatchKbn
		{
			/// <summary>比較対象外</summary>
			NonTarget,
			/// <summary>一致</summary>
			Match,
			/// <summary>不一致</summary>
			Unmatch
		}
		/// <summary>実行結果区分</summary>
		protected enum ExecResultKbn
		{
			/// <summary>登録</summary>
			Insert,
			/// <summary>更新</summary>
			Update,
			/// <summary>削除</summary>
			Delete
		}
		#endregion

		#region マッチング基底クラス
		/// <summary>
		/// 一致条件に該当するユーザーかどうか？を返す
		/// </summary>
		/// <param name="targetUser">対象情報</param>
		/// <param name="user">比較情報</param>
		/// <param name="logicalOperators">論理演算子情報</param>
		/// <returns>一致条件に該当する：true、該当しない：false</returns>
		protected bool MatchUser(BaseData targetUser, BaseData user, LogicalOperatorsData logicalOperators)
		{
			var operators = logicalOperators.Operator;
			var isMatch = (operators == OperatorKbn.And);
			foreach (var match in logicalOperators.MatchDatas)
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
			foreach (var childLogicalOperators in logicalOperators.LogicalOperatorsDatas)
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
		/// <param name="targetUser">対象情報</param>
		/// <param name="user">比較情報</param>
		/// <param name="match">一致情報</param>
		/// <returns>一致区分</returns>
		protected MatchKbn MatchValue(BaseData targetUser, BaseData user, MatchData match)
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
		protected bool Match(string targetValue, string value, MatchData match)
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
		#endregion

		#region 基底データクラス
		/// <summary>基底データクラス</summary>
		protected class BaseData
		{
			#region #コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public BaseData()
			{
				this.Data = new Hashtable();
			}
			#endregion

			#region プロパティ
			/// <summary>データ</summary>
			public Hashtable Data { get; set; }
			#endregion
		}
		#endregion

	}
}
