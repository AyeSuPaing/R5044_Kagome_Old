/*
=========================================================================================================
  Module      : CrossPoint API ユーザーサービスクラス (CrossPointUserApiService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.CrossPoint.User.Helper;
using w2.Domain.User;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// ユーザーサービスクラス
	/// </summary>
	public class CrossPointUserApiService : ServiceBase
	{
		/// <summary>
		/// ポイントカード情報登録
		/// </summary>
		/// <param name="userId">更新対象のユーザーID</param>
		/// <param name="newCardNo">新しいカードの番号</param>
		/// <param name="newCardPinCode">新しいカードのPINコード</param>
		/// <returns>処理結果</returns>
		public ResultStatus RegisterPointCard(
			string userId,
			string newCardNo,
			string newCardPinCode)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BASE_NET_SHOP_MEMBER_ID, userId },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_DEL_REAL_SHOP_CARD_NO, newCardNo },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_DEL_PIN_CODE, newCardPinCode },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BASE_MEMBER_ID_REPLACE, Constants.CROSS_POINT_FLG_BASE_MEMBER_ID_REPLACE },
			};
			var result = GetResultSet<MergeResponse>(
					Constants.CROSS_POINT_URL_MERGE_USER,
					param,
					() => new CrossPointUserApiRepository().Merge(param))
				.ResultStatus;

			return result;
		}

		/// <summary>
		/// ポイントカード情報更新
		/// </summary>
		/// <param name="userId">更新対象のユーザーID</param>
		/// <param name="newCardNo">新しいカードの番号</param>
		/// <param name="newCardPinCode">新しいカードのPINコード</param>
		/// <returns>処理結果</returns>
		public ResultStatus UpdatePointCard(
			string userId,
			string newCardNo,
			string newCardPinCode)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BASE_NET_SHOP_MEMBER_ID, userId },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_DEL_REAL_SHOP_CARD_NO, newCardNo },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_DEL_PIN_CODE, newCardPinCode },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BASE_MEMBER_ID_REPLACE, Constants.CROSS_POINT_FLG_BASE_MEMBER_ID_REPLACE },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_NEW_REAL_SHOP_CARD_NO, newCardNo },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_NEW_PIN_CODE, newCardPinCode },
			};

			var result = GetResultSet<MergeResponse>(
					Constants.CROSS_POINT_URL_MERGE_USER,
					param,
					() => new CrossPointUserApiRepository().Merge(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// 使用可能なカードか確認する
		/// </summary>
		/// <param name="cardNo">カード番号</param>
		/// <param name="pinCd">PINコード</param>
		/// <returns>使用可能なカードか</returns>
		public bool CheckValidCard(string cardNo, string pinCd)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_MEMBER_ID, cardNo },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PIN_CODE, pinCd },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_EXCLUDE_WITHDRAW, Constants.CROSS_POINT_FLG_EXCLUDE_WITHDRAW_EXCLUDE },
			};
			var resultSet = GetResultSet<SearchResponse>(
				Constants.CROSS_POINT_URL_GET_USER_LIST,
				param,
				() => new CrossPointUserApiRepository().Search(param));

			if (resultSet.TotalResult == 0) return false;

			var result = resultSet.Result.All(item => string.IsNullOrEmpty(item.NetShopMemberId));
			return result;
		}

		/// <summary>
		/// パスワード情報更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="newPassword">変更後パスワード</param>
		/// <returns>処理結果</returns>
		public ResultStatus UpdateUserPassword(string userId, string newPassword)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_NET_SHOP_MEMBER_ID, userId },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PASSWORD, UserConversion.GetHash(newPassword).ToLower() },
			};
			var result = GetResultSet<UpdateResponse>(
					Constants.CROSS_POINT_URL_UPDATE_USER,
					param,
					() => new CrossPointUserApiRepository().Update(param))
				.ResultStatus;

			return result;
		}

		/// <summary>
		/// 退会
		/// </summary>
		/// <param name="userModel">ユーザーモデル</param>
		/// <returns>処理結果</returns>
		public ResultStatus Withdraw(UserModel userModel)
		{
			var updateParam = new UserApiInput(userModel).GetParam();
			var updateResult = GetResultSet<UpdateResponse>(
				Constants.CROSS_POINT_URL_UPDATE_USER,
				updateParam,
				() => new CrossPointUserApiRepository().Update(updateParam));
			if (updateResult.ResultStatus.IsSuccess == false) return updateResult.ResultStatus;

			var deleteResult = Delete(userModel.UserId);
			return deleteResult;
		}

		/// <summary>
		/// ユーザー情報更新
		/// </summary>
		/// <param name="model">ユーザーモデル</param>
		/// <returns>処理結果</returns>
		public ResultStatus Update(UserModel model)
		{
			var param = new UserApiInput(model).GetParam();
			var result = GetResultSet<UpdateResponse>(
					Constants.CROSS_POINT_URL_UPDATE_USER,
					param,
					() => new CrossPointUserApiRepository().Update(param))
				.ResultStatus;

			return result;
		}

		/// <summary>
		/// ユーザー退会処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>処理結果</returns>
		private ResultStatus Delete(string userId)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_NET_SHOP_MEMBER_ID, userId },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BASE_DEL_MEMBER, Constants.CROSS_POINT_FLG_WITHDRAWAL_ON },
			};

			var result = GetResultSet<DeleteResponse>(
					Constants.CROSS_POINT_URL_DELETE_USER,
					param,
					() => new CrossPointUserApiRepository().Delete(param))
				.ResultStatus;

			return result;
		}

		/// <summary>
		/// ユーザー情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーモデル</returns>
		public UserApiResult Get(string userId)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_MEMBER_ID, userId },
			};

			var result = GetResult<GetResponse>(
				Constants.CROSS_POINT_URL_GET_USER,
				param,
				() => new CrossPointUserApiRepository().Get(param));

			return result;
		}

		/// <summary>
		/// ユーザー情報検索
		/// </summary>
		/// <param name="param">検索条件</param>
		/// <returns>ユーザーモデル</returns>
		public ResultSet<UserApiResult> Search(Dictionary<string, string> param)
		{
			var result = GetResultSet<SearchResponse>(
				Constants.CROSS_POINT_URL_GET_USER_LIST,
				param,
				() => new CrossPointUserApiRepository().Search(param));

			return result;
		}

		/// <summary>
		/// ユーザー情報登録
		/// </summary>
		/// <param name="model">ユーザーモデル</param>
		/// <returns>処理結果</returns>
		public ResultStatus Insert(UserModel model)
		{
			var param = new UserApiInput(model).GetParam();
			var result = GetResultSet<InsertResponse>(
					Constants.CROSS_POINT_URL_INSERT_USER,
					param,
					() => new CrossPointUserApiRepository().Insert(param))
				.ResultStatus;

			return result;
		}

		/// <summary>
		/// ユーザ情報名寄せ処理
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="newCardNo">新カードNo</param>
		/// <param name="newCardPinCode">新カードPINコード</param>
		/// <param name="appMemberFlg">アプリ会員フラグ</param>
		/// <returns>APIの処理結果</returns>
		public ResultStatus Merge(
			string userId,
			string newCardNo,
			string newCardPinCode,
			string appMemberFlg = null)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BASE_NET_SHOP_MEMBER_ID, userId },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_DEL_REAL_SHOP_CARD_NO, newCardNo },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_DEL_PIN_CODE, newCardPinCode },
			};

			if (appMemberFlg == Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_ON)
			{
				param.Add(Constants.CROSS_POINT_PARAM_MEMBER_INFO_BASE_MEMBER_ID_REPLACE, appMemberFlg);
			}
			else
			{
				param.Add(Constants.CROSS_POINT_PARAM_MEMBER_INFO_NEW_PIN_CODE, newCardPinCode);
				param.Add(Constants.CROSS_POINT_PARAM_MEMBER_INFO_NEW_REAL_SHOP_CARD_NO, newCardNo);
			}

			var result = GetResultSet<MergeResponse>(
					Constants.CROSS_POINT_URL_MERGE_USER,
					param,
					() => new CrossPointUserApiRepository().Merge(param))
				.ResultStatus;

			return result;
		}

		/// <summary>
		/// Get user id
		/// </summary>
		/// <param name="newCardNo">New card no</param>
		/// <param name="newCardPinCode">New card pin code</param>
		/// <returns>Processing result</returns>
		public bool GetUserId(
			string newCardNo,
			string newCardPinCode)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_MEMBER_ID, newCardNo },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PIN_CODE, newCardPinCode },
			};

			var resultSet = GetResultSet<SearchResponse>(
				Constants.CROSS_POINT_URL_GET_USER_LIST,
				param,
				() => new CrossPointUserApiRepository().Search(param));

			var result = (resultSet.TotalResult != 0);
			return result;
		}

		/// <summary>
		/// 店舗カード番号・PINコードが一致するユーザリストを取得(退会会員は除外する)
		/// </summary>
		/// <param name="cardNo">店舗カード番号</param>
		/// <param name="cardPinCode">PINコード</param>
		/// <returns>ユーザモデル</returns>
		public ResultSet<UserApiResult> GetUserByCardNoAndCardPinCodeExcludingWithdrawal(
			string cardNo,
			string cardPinCode)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_MEMBER_ID, cardNo },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PIN_CODE, cardPinCode },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_EXCLUDE_WITHDRAW, Constants.CROSS_POINT_FLG_EXCLUDE_WITHDRAW_EXCLUDE },
			};
			var result = Search(param);
			return result;
		}

		/// <summary>
		/// ユーザー情報取得
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>ユーザーモデル</returns>
		private UserApiResult GetResult<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<UserApiResult>, new()
		{
			var resultSet = GetResultSet<TResponse>(name, param, function);
			var result = (resultSet.TotalResult > 0)
				? resultSet.Result[0]
				: null;

			return result;
		}

		/// <summary>
		/// ユーザー情報取得 共通処理
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>結果セット</returns>
		private ResultSet<UserApiResult> GetResultSet<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<UserApiResult>, new()
		{
			WriteStartLog(name, param);

			var resultSet = new TResponse().GetResultSet<TResponse>(function());

			// 異常系エラーの場合、もう一度試行する
			if ((resultSet.ResultStatus.IsSuccess == false)
				&& resultSet.ResultStatus.Error.Any(error => error.IsAbnormalError))
			{
				resultSet = new TResponse().GetResultSet<TResponse>(function());
			}

			resultSet.ResultStatus.RequestParameter = param;
			WriteEndLog(
				name,
				resultSet.ResultStatus,
				resultSet.XmlResponse);

			return resultSet;
		}
	}
}
