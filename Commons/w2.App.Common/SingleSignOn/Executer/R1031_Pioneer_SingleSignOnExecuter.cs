/*
=========================================================================================================
  Module      : シングルサインオン実行（R1031_Pioneer用）クラス(R1031_Pioneer_SingleSignOnExecuter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using w2.App.Common.Option;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.SingleSignOn.Executer
{
	/// <summary>
	/// シングルサインオン実行（R1031_Pioneer用）クラス
	/// </summary>
	public class R1031_Pioneer_SingleSignOnExecuter : BaseSingleSignOnExecuter
	{
		#region 定数
		/// <summary>JAFログイン連携：暗号化会員番号</summary>
		public const string JAF_LOGIN_ENCRYPTKNNO = "EncryptKnNo";
		/// <summary>JAFログイン連携：ステータスコード</summary>
		public const string JAF_LOGIN_STATUS = "Status";
		/// <summary>JAFログイン連携：エラーコード</summary>
		public const string JAF_LOGIN_ERROR = "Error";
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="context">HTTPコンテンツ</param>
		public R1031_Pioneer_SingleSignOnExecuter(HttpContext context)
			: base(context)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// シングルサインオン実行
		/// </summary>
		/// <returns>シングルサインオンリザルト</returns>
		protected override SingleSignOnResult OnExecute()
		{
			var parameters = new CallBackParameters(this.Context.Request);

			//検証
			var isSuccessValidate = parameters.Validate();
			if (isSuccessValidate == false)
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_LOGIN_FOR_SINGLESIGNON));
			}

			// 会員ランク存在チェック
			var memberRankName = MemberRankOptionUtility.GetMemberRankName(Constants.JAF_RANK_ID);
			if (string.IsNullOrEmpty(memberRankName))
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_RANK_NOT_EXISTS));
			}
			
			return ProcessUserExtendAndMemberRank(parameters);
		}
		
		/// <summary>
		/// ユーザー拡張項目とユーザーランク処理
		/// </summary>
		/// <param name="parameters">コールバック情報</param>
		/// <returns>シングルサインオンリザルト</returns>
		private SingleSignOnResult ProcessUserExtendAndMemberRank(CallBackParameters parameters)
		{
			var userService = new UserService();
			var user = userService.Get(parameters.UserId);
			if (user == null)
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_LOGIN_FOR_SINGLESIGNON));
			}

			if (user.UserExtend.UserExtendColumns.Contains(Constants.JAF_ENCRYPTKNNO_USEREXTEND_COLUMN_NAME) == false
				|| user.UserExtend.UserExtendColumns.Contains(Constants.JAF_STATUS_USEREXTEND_COLUMN_NAME) == false
				|| user.UserExtend.UserExtendColumns.Contains(Constants.JAF_ERROR_USEREXTEND_COLUMN_NAME) == false)
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_EXTENDS_NOT_EXISTS));
			}

			// 送信データ作成
			var postData = string.Format(
				"qServiceId={0}&qSsnId={1}&qItemId={2}",
				HttpUtility.UrlEncode(Constants.JAF_SERVICE_ID),
				HttpUtility.UrlEncode(parameters.SsnId),
				HttpUtility.UrlEncode("del_flg"));

			// レスポンス情報を取得
			var xmlDoc = GetResponse(Constants.JAF_MEMBER_API_URL, postData);
			var xElement = XElement.Parse(xmlDoc);
			var encryptKnNo = (string)xElement.Element(JAF_LOGIN_ENCRYPTKNNO) ?? string.Empty;
			var status = (string)xElement.Element(JAF_LOGIN_STATUS) ?? string.Empty;
			var error = (string)xElement.Element(JAF_LOGIN_ERROR) ?? string.Empty;

			if ((status == "0") || (string.IsNullOrEmpty(encryptKnNo)))
			{
				switch (error)
				{
					case "1":
						error = "プロトコル不正エラーが発生しました。";
						break;

					case "2":
						error = "パラメータ不正エラーが発生しました。";
						break;

					case "3":
						error = "サービスID指定エラーが発生しました。";
						break;

					case "4":
						error = "システムエラーが発生しました。";
						break;

					default:
						break;
				}
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					error);
			}

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				
				user.UserExtend.UserExtendDataValue[Constants.JAF_ENCRYPTKNNO_USEREXTEND_COLUMN_NAME] = encryptKnNo;
				user.UserExtend.UserExtendDataValue[Constants.JAF_STATUS_USEREXTEND_COLUMN_NAME] = status;
				user.UserExtend.UserExtendDataValue[Constants.JAF_ERROR_USEREXTEND_COLUMN_NAME] = error;

				userService.Modify(
					user.UserId,
					userModel => { userModel.MemberRankId = Constants.JAF_RANK_ID; },
					UpdateHistoryAction.Insert,
					accessor);

				userService.UpdateUserExtend(
					user.UserExtend,
					user.UserId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 会員ランク更新履歴へ格納
				MemberRankOptionUtility.InsertUserMemberRankHistory(
					user.UserId,
					user.MemberRankId,
					Constants.JAF_RANK_ID,
					"",
					user.UserId,
					accessor);

				accessor.CommitTransaction();
			}

			// 連携したユーザーでログイン
			var newUser = userService.Get(parameters.UserId);
			return new SingleSignOnResult(
				SingleSignOnDetailTypes.Success,
				newUser,
				parameters.NextUrl);
		}

		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="url">遷移先URL</param>
		/// <param name="postData">送信データ</param>
		/// <returns>レスポンス</returns>
		private string GetResponse(string url, string postData)
		{
			var xmlDoc = string.Empty;
			
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			var requestBytes = Encoding.UTF8.GetBytes(postData);
			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ContentLength = requestBytes.Length;

			using (var requestStream = webRequest.GetRequestStream())
			{
				requestStream.Write(requestBytes, 0, requestBytes.Length);
			}

			using (var responseStream = (HttpWebResponse)webRequest.GetResponse())
			{
				if (responseStream.StatusCode == HttpStatusCode.OK)
				{
					var responseXml = XDocument.Load(responseStream.GetResponseStream());
					xmlDoc = responseXml.ToString();
				}
			}

			return xmlDoc;
		}
		#endregion

		#region JAFサイト コールバックパラメータクラス
		/// <summary>
		/// JAFサイト コールバックパラメータクラス
		/// </summary>
		public class CallBackParameters
		{
			#region 定数
			/// <summary>遷移先URL</summary>
			private const string REQUEST_URL = "nurl";
			/// <summary>ユーザーID</summary>
			private const string REQUEST_USER_ID = "uid";
			/// <summary>セッションID</summary>
			private const string REQUEST_SSNID = "SsnId";
			#endregion

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="request">リクエスト</param>
			public CallBackParameters(HttpRequest request)
			{
				this.NextUrl = StringUtility.ToEmpty(request[REQUEST_URL]);
				this.UserId = StringUtility.ToEmpty(request[REQUEST_USER_ID]);
				this.SsnId = StringUtility.ToEmpty(request[REQUEST_SSNID]);
			}
			#endregion

			#region メソッド
			/// <summary>
			/// 検証
			/// </summary>
			/// <returns>成功：true、失敗：false</returns>
			public bool Validate()
			{
				return (this.SsnId != string.Empty);
			}

			#endregion

			#region プロパティ
			/// <summary>遷移先URL</summary>
			public string NextUrl { get; private set; }
			/// <summary>ユーザーID</summary>
			public string UserId { get; private set; }
			/// <summary>セッションID</summary>
			public string SsnId { get; private set; }
			#endregion
		}
		#endregion
	}
}