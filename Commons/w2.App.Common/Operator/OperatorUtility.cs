/*
=========================================================================================================
  Module      : オペレーター情報ユーティリティ(OperatorUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Web;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Operator
{
	public class OperatorUtility
	{
		/// <summary>
		/// 表示行件数取得
		/// </summary>
		/// <param name="pageNo">ページ番号</param>
		/// <returns>表示行件数</returns>
		public static Dictionary<string, int> GetRowNumber(int pageNo)
		{
			var bgnRow = (pageNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			var endRow = pageNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;

			var rowNumber = new Dictionary<string, int>
			{
				{ Constants.FIELD_COMMON_BEGIN_NUM, bgnRow },
				{ Constants.FIELD_COMMON_END_NUM, endRow },
			};

			return rowNumber;
		}

		/// <summary>
		/// 検索値取得
		/// </summary>
		/// <param name="input">検索情報</param>
		/// <returns>検索情報</returns>
		public static Hashtable GetSearchSqlInfo(Dictionary<string, string> input)
		{
			var result = new Hashtable
			{
				{
					Constants.FIELD_SHOPOPERATOR_OPERATOR_ID + Constants.FLG_LIKE_ESCAPED,
					StringUtility.SqlLikeStringSharpEscape(input[Constants.REQUEST_KEY_OPERATOR_ID])
				},
				{
					Constants.FIELD_SHOPOPERATOR_NAME + Constants.FLG_LIKE_ESCAPED,
					StringUtility.SqlLikeStringSharpEscape(input[Constants.REQUEST_KEY_OPERATOR_NAME])
				}
			};

			switch (StringUtility.ToEmpty(input[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL]))
			{
				case "":
					result.Add(Constants.FLG_CONDITION_MENU_ACCESS_LEVEL, string.Empty);
					break;

				case Constants.FLG_NO_AUTHORITY_VALUE:
					result.Add(Constants.FLG_CONDITION_MENU_ACCESS_LEVEL, Constants.FLG_SHOPOPERATOR_NO_AUTHORITY_VALUE);
					break;

				default:
					result.Add(
						Constants.FLG_CONDITION_MENU_ACCESS_LEVEL,
						StringUtility.ToEmpty(input[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL]));
					break;
			}

			result.Add(
				Constants.FIELD_SHOPOPERATOR_VALID_FLG,
				StringUtility.SqlLikeStringSharpEscape(input[Constants.REQUEST_KEY_OPERATOR_VALID_FLG]));
			result.Add(
				Constants.FLG_SHOPOPERATOR_SORT_KBN,
				StringUtility.ToEmpty(input[Constants.REQUEST_KEY_SORT_KBN]));

			return result;
		}

		/// <summary>
		/// オペレータ一覧パラメタ取得
		/// </summary>
		/// <param name="request">HttpRequest</param>
		/// <param name="pageNumber">ページ番号</param>
		/// <returns>パラメタが格納されたHashtable</returns>
		public static Dictionary<string, string> GetParameters(Dictionary<string, string> request, string pageNumber)
		{
			var result = new Dictionary<string, string>();
			var currentPageNo = 1;
			var hasParamError = false;

			try
			{
				result.Add(Constants.REQUEST_KEY_OPERATOR_ID, StringUtility.ToEmpty(
					request[Constants.REQUEST_KEY_OPERATOR_ID]));
				result.Add(Constants.REQUEST_KEY_OPERATOR_NAME, HttpUtility.UrlDecode(StringUtility.ToEmpty(
					request[Constants.REQUEST_KEY_OPERATOR_NAME])));
				result.Add(Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL, StringUtility.ToEmpty(
					request[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL]));
				result.Add(Constants.REQUEST_KEY_OPERATOR_VALID_FLG, StringUtility.ToEmpty(
					request[Constants.REQUEST_KEY_OPERATOR_VALID_FLG]));
				var sortKbn = string.Empty;
				switch (StringUtility.ToEmpty(request[Constants.REQUEST_KEY_SORT_KBN]))
				{
					case Constants.FLG_SORT_OPERATOR_LIST_ID_ASC:
					case Constants.FLG_SORT_OPERATOR_LIST_ID_DESC:
					case Constants.FLG_SORT_OPERATOR_LIST_NAME_ASC:
					case Constants.FLG_SORT_OPERATOR_LIST_NAME_DESC:
						sortKbn = request[Constants.REQUEST_KEY_SORT_KBN];
						break;

					case "":
						sortKbn = Constants.FLG_SORT_OPERATOR_LIST_ID_DESC;
						break;

					default:
						hasParamError = true;
						break;
				}
				result.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);

				currentPageNo = (string.IsNullOrEmpty(StringUtility.ToEmpty(pageNumber)))
					? 1
					: int.Parse(pageNumber);
			}
			catch
			{
				hasParamError = true;
			}

			result.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNo.ToString());
			result.Add(Constants.FLG_ERROR_REQUEST_PRAMETER, hasParamError.ToString());

			return result;
		}

		/// <summary>
		/// データバインド用詳細画面URL作成
		/// </summary>
		/// <param name="strOperatorId">オペレータID</param>
		/// <returns>詳細画面URL</returns>
		public static string CreateDetailUrl(string strOperatorId)
		{
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_CONFIRM)
				.AddParam(Constants.REQUEST_KEY_OPERATOR_ID, strOperatorId)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
				.CreateUrl();
			return url;
		}

		/// <summary>
		/// 一覧画面URL作成
		/// </summary>
		/// <param name="parameters">パラメータ</param>
		/// <returns>一覧遷移URL</returns>
		public static string CreateListUrl(List<string> parameters)
		{
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST)
				.AddParam(Constants.REQUEST_KEY_OPERATOR_ID, HttpUtility.UrlEncode(parameters[0]))
				.AddParam(Constants.REQUEST_KEY_OPERATOR_NAME, HttpUtility.UrlEncode(parameters[1]))
				.AddParam(Constants.REQUEST_KEY_SORT_KBN, HttpUtility.UrlEncode(parameters[2]))
				.AddParam(Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL, HttpUtility.UrlEncode(parameters[3]))
				.AddParam(Constants.REQUEST_KEY_OPERATOR_VALID_FLG, HttpUtility.UrlEncode(parameters[4])).CreateUrl();
			return url;
		}
	}
}
