<%--
=========================================================================================================
  Module      : Get Users (GetUsers.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Letro.User.GetUsers" %>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using w2.App.Common;
using w2.Domain;

namespace Letro.User
{
	/// <summary>
	/// Get users
	/// </summary>
	public class GetUsers : LetroBase, IHttpHandler
	{
		/// <summary>User limit count</summary>
		protected const int USER_LIMIT_COUNT = 100;
		/// <summary>Regex pattern user id</summary>
		protected const string REGEX_PATTERN_USER_ID = "^[a-zA-Z0-9]{1,30}$";

		/// <summary>
		/// Process request
		/// </summary>
		/// <param name="context">Context</param>
		public override void ProcessRequest(HttpContext context)
		{
			this.CurrentContext = context;
			GetRequest();
			WriteResponse();
		}

		/// <summary>
		/// Get request method
		/// </summary>
		protected override void GetRequest()
		{ 
			var userIds = (string)this.CurrentContext.Request[Constants.REQUEST_KEY_USERS] ?? string.Empty;

			this.UserIds = userIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct()
				.ToArray();
		}

		/// <summary>
		/// Get response data
		/// </summary>
		/// <returns>A response object</returns>
		protected override object GetResponseData()
		{
			var responseData = new LetroUsersGetResponse();
			if (this.Users != null) responseData.Users = this.Users;

			return responseData;
		}

		/// <summary>
		/// Is valid authorization
		/// </summary>
		/// <returns>True if parameter is valid, otherwise false</returns>
		protected override bool IsValidParameters()
		{
			if (this.UserIds.Length == 0)
			{
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_USER_ID_FORMAT_ERROR);
				return false;
			}

			if (this.UserIds.Length > USER_LIMIT_COUNT)
			{
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_USER_LIMIT_OVER);
				return false;
			}

			var regex = new Regex(REGEX_PATTERN_USER_ID);
			foreach (var userId in this.UserIds)
			{
				if (regex.IsMatch(userId)) continue;

				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_USER_ID_FORMAT_ERROR);
				return false;
			}

			var users = DomainFacade.Instance.UserService.GetUsersForLetro(this.UserIds);
			this.Users = users.Cast<DataRowView>().Select(data => new UserDetail(data)).ToArray();

			return true;
		}

		/// <summary>
		/// Is exist user
		/// </summary>
		/// <returns>True if exist, otherwise false</returns>
		protected bool IsExistUser()
		{
			if (this.Users == null) return false;

			var userIdsNotExist = new List<string>();
			var userIdsAfterGetFromDB = this.Users.Select(user => user.UserId).ToArray();

			foreach (var userId in this.UserIds)
			{
				if (userIdsAfterGetFromDB.Contains(userId)) continue;
				userIdsNotExist.Add(userId);
			}

			if (userIdsNotExist.Count == 0) return true;

			this.UserIdsNotExist = new[] { string.Join(",", userIdsNotExist) };
			return false;
		}

		/// <summary>User ids from request</summary>
		public string[] UserIds { get; set; }
		/// <summary>Users</summary>
		public UserDetail[] Users { get; set; }
		/// <summary>User ids not exist</summary>
		public string[] UserIdsNotExist { get; set; }
		/// <summary>Is reusable</summary>
		public bool IsReusable { get { return false; } }
	}
}
