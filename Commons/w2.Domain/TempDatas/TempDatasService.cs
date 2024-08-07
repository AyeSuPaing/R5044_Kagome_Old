/*
=========================================================================================================
  Module      : テンポラリデータサービス (TempDatasService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using w2.Common.Sql;

namespace w2.Domain.TempDatas
{
	/// <summary>
	/// テンポラリデータサービス
	/// </summary>
	public class TempDatasService : ServiceBase
	{
		/// <summary>テンポラリタイプ</summary>
		public enum TempType
		{
			/// <summary>セッションID変更</summary>
			ChangeSessionId,
			/// <summary>別サイト遷移</summary>
			GoToOtherSite,
			/// <summary>ログイン失敗情報(IP+ログインID)</summary>
			LoginErrorInfoLoginId,
			/// <summary>ログイン失敗情報(IP+パスワード)</summary>
			LoginErrorInfoPassword,
			/// <summary>クレジットカード認証失敗情報(ユーザーID)</summary>
			CreditAuthErrorInfoUserId,
			/// <summary>ボットチャット試行回数</summary>
			LineApiNumberOfTrials,
			/// <summary>ネクストエンジン連携トークン</summary>
			NextEngineToken,
			/// <summary>Credit auth error information IP address</summary>
			CreditAuthErrorInfoIpAddress,
			/// <summary>Auth code</summary>
			AuthCode,
			/// <summary>Auth code send ip a ddress</summary>
			AuthCodeSendIpAddress,
			/// <summary>Auth code try ip address</summary>
			AuthCodeTryIpAddress,
			/// <summary>Auth code try send tel</summary>
			AuthCodeTrySendTel,
			/// <summary>LetroApi試行回数</summary>
			LetroApiNumberOfTrials,
		}

		#region +DecreaseCountForCountValue カウントを1つ減らす（存在しなければデフォルトカウントから1つ減らしたものを格納）
		/// <summary>
		/// カウント値を-1する（存在しなければデフォルトカウントから-1したものを格納）
		/// </summary>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <param name="key">キー</param>
		/// <param name="initialCount">初期値</param>
		/// <param name="validMinutes">テンポラリ有効期限(分)</param>
		public void DecreaseCountForCountValue(TempType tempType, string key, int initialCount, int validMinutes)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				// DBからエラー情報取得（取得できない場合はnull）
				var errorInfo = Resotre(tempType, key, validMinutes, accessor);

				// エラー情報保存（カウント-1）
				var updateCount = ((errorInfo != null) ? (int)errorInfo.TempDataDeserialized : initialCount) - 1;
				Save(tempType, key, updateCount, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Save 保存（上書き）
		/// <summary>
		/// 保存
		/// </summary>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <param name="key">保存キー（テンポラリタイプに対して一意）</param>
		/// <param name="data">保存したいデータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Save(TempType tempType, string key, object data, SqlAccessor accessor = null)
		{
			// メモリへ格納
			byte[] dataBytes;
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, data);
				dataBytes = stream.ToArray();
			}

			// DB削除＆格納
			DeleteInsert(
				new TempDatasModel
				{
					TempType = GetTempTypeString(tempType),
					TempKey = key,
					TempData = dataBytes,
				},
				accessor);
		}
		#endregion

		#region +Resotre 復元（データだけ取得）
		/// <summary>
		/// 復元（データだけ取得）
		/// </summary>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <param name="key">保存キー（テンポラリタイプに対して一意）</param>
		/// <param name="availableMinutes">有効期間(分)</param>
		/// <param name="accessor">SQLアクセサ</param>
		public TempDatasModel Resotre(TempType tempType, string key, int? availableMinutes = null, SqlAccessor accessor = null)
		{
			// ＤＢからデータ取得
			var model = Get(tempType, key, accessor);
			if (model == null) return null;

			// 有効期間を過ぎていたら削除
			if ((availableMinutes.HasValue)
			    && (model.DateCreated.AddMinutes(availableMinutes.Value) < DateTime.Now))
			{
				Delete(tempType, key, accessor);
				return null;
			}

			// デシリアイズ
			model.DeserializeTempData();

			return model;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <param name="tempKey">テンポラリキー</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		private TempDatasModel Get(TempType tempType, string tempKey, SqlAccessor accessor = null)
		{
			using (var repository = new TempDatasRepository(accessor))
			{
				var model = repository.Get(GetTempTypeString(tempType), tempKey);
				return model;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <param name="tempKey">テンポラリキー</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(TempType tempType, string tempKey, SqlAccessor accessor = null)
		{
			using (var repository = new TempDatasRepository(accessor))
			{
				repository.Delete(GetTempTypeString(tempType), tempKey);
			}
		}
		#endregion

		#region +DeleteInsert 削除・登録
		/// <summary>
		/// 削除・登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void DeleteInsert(TempDatasModel model, SqlAccessor accessor = null)
		{
			using (var repository = new TempDatasRepository(accessor))
			{
				repository.Delete(model.TempType, model.TempKey);
				repository.Insert(model);
			}
		}
		#endregion

		#region -GetTempTypeString テンポラリタイプ文字列取得
		/// <summary>
		/// テンポラリタイプ文字列取得
		/// </summary>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <returns>テンポラリタイプ文字列</returns>
		private static string GetTempTypeString(TempType tempType)
		{
			switch (tempType)
			{
				case TempType.ChangeSessionId:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_CHANGE_SESSION_ID;

				case TempType.GoToOtherSite:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_GO_TO_OTHER_SITE;

				case TempType.LoginErrorInfoLoginId:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_LOGIN_ERROR_INFO_LOGIN_ID;

				case TempType.LoginErrorInfoPassword:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_LOGIN_ERROR_INFO_PASSWORD;

				case TempType.CreditAuthErrorInfoUserId:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_CREDIT_AUTH_ERROR_INFO_USER_ID;

				case TempType.LineApiNumberOfTrials:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_LINE_AUTH_KEY_ERROR_INFO_IP;

				case TempType.NextEngineToken:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_NE_TOKEN;

				case TempType.CreditAuthErrorInfoIpAddress:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_CREDIT_AUTH_ERROR_INFO_IP_ADDRESS;

				case TempType.AuthCode:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE;

				case TempType.AuthCodeSendIpAddress:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE_SEND_IPADDRESS;

				case TempType.AuthCodeTryIpAddress:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE_TRY_IPADDRESS;

				case TempType.AuthCodeTrySendTel:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE_TRY_SENDTEL;

				case TempType.LetroApiNumberOfTrials:
					return Constants.FLG_TEMPDATAS_TEMP_TYPE_LETRO_AUTH_KEY_ERROR_INFO_IP;

				default:
					throw new ArgumentException("不明なTempTypeです。:" + tempType.ToString());
			}
		}
		#endregion
	}
}
