/*
=========================================================================================================
  Module      : Base O-MOTION Request(BaseOMotionRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.User.OMotion
{
	/// <summary>
	/// Base O-MOTION request
	/// </summary>
	public abstract class BaseOMotionRequest : IHttpApiRequestData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="apiUrl">Api url</param>
		/// <param name="loginId">Login id</param>
		protected BaseOMotionRequest(string authoriId, string apiUrl, string loginId)
		{
			this.AuthoriId = authoriId;
			this.ApiUrl = apiUrl;
			this.Useridhashed = OMotionUtility.CreateHashOMotionReqId(loginId);
			this.Signature = Constants.OMOTION_SIGNATURE;

			this.Headers = new Dictionary<string, string>();
			this.Posts = new Dictionary<string, object>();

			CreateHeaders();
		}

		/// <summary>
		/// Create post string
		/// </summary>
		/// <returns>Post string</returns>
		public string CreatePostString()
		{
			return this.PostString;
		}

		/// <summary>
		/// Set post string
		/// </summary>
		public void SetPostString()
		{
			CreatePosts();

			this.PostString = (this.Posts.Count == 0) ? "" : SerializeHelper.SerializeJson(this.Posts);
		}

		/// <summary>
		/// Create headers
		/// </summary>
		public virtual void CreateHeaders()
		{
			this.Headers.Add(OMotionConstants.REQUEST_HEADER_KEY_SIGNATURE, this.Signature);
			this.Headers.Add(OMotionConstants.REQUEST_HEADER_KEY_USERIDHASHED, this.Useridhashed);
		}

		/// <summary>
		/// Create posts
		/// </summary>
		public virtual void CreatePosts()
		{
		}

		/// <summary>
		/// Create log message
		/// </summary>
		/// <returns></returns>
		public string CreateLogMessage()
		{
			var result = new StringBuilder();
			result.AppendLine(string.Format("authori id:{0}", this.AuthoriId));
			result.AppendLine(string.Format("api url:{0}", this.ApiUrl));
			result.AppendLine(string.Format("header:{0}", string.Join(",", this.Headers.Select(header => string.Format("{0}={1}", header.Key, header.Value)))));
			result.AppendLine(string.Format("Post:{0}", this.PostString));

			return result.ToString();
		}

		/// <summary>Authori id</summary>
		public string AuthoriId { get; set; }
		/// <summary>Api url</summary>
		public string ApiUrl { get; set; }
		/// <summary>Method type</summary>
		public string MethodType { get; set; }
		/// <summary>Signature</summary>
		public string Signature { get; set; }
		/// <summary>Useridhashed</summary>
		public string Useridhashed { get; set; }
		/// <summary>Headers</summary>
		public IDictionary<string, string> Headers { get; set; }
		/// <summary>Posts</summary>
		public IDictionary<string, object> Posts { get; set; }
		/// <summary>Post string</summary>
		public string PostString { get; set; }
	}
}
