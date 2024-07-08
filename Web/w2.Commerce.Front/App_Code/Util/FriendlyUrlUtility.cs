/*
=========================================================================================================
  Module      : フレンドリーURLユーティリティ(FriendlyUrlUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

//*********************************************************************************************
/// <summary>
/// フレンドリーURLユーティリティ
/// </summary>
//*********************************************************************************************
public class FriendlyUrlUtility
{
	/// <summary>フレンドリーURLルール</summary>
	private static List<KeyValuePair<string, string>> m_lFriendlyUrlRules = new List<KeyValuePair<string, string>>();

	//=============================================================================================
	/// <summary>
	/// スタティックコンストラクタ
	/// </summary>
	//=============================================================================================
	static FriendlyUrlUtility()
	{
		//------------------------------------------------------
		// フレンドリーURLルール読み込み
		//------------------------------------------------------
		List<KeyValuePair<string, string>> lFriendlyUrlRulesTmp = new List<KeyValuePair<string, string>>();
		if (Constants.FRIENDLY_URL_ENABLED)
		{
			lock (lFriendlyUrlRulesTmp)
			{
				XmlDocument xdRules = new XmlDocument();
				xdRules.Load(AppDomain.CurrentDomain.BaseDirectory + "Xml/FriendlyUrlRules.xml");
				foreach (XmlNode xn in xdRules.SelectNodes("/FriendlyUrlRules/Rule"))
				{
					lFriendlyUrlRulesTmp.Add(
						new KeyValuePair<string, string>(
							xn.SelectSingleNode("Pattern").InnerText,
							xn.SelectSingleNode("RealUrl").InnerText));
				}
			}
			m_lFriendlyUrlRules = lFriendlyUrlRulesTmp;
		}
	}

	//=============================================================================================
	/// <summary>
	/// リアルURL取得
	/// </summary>
	/// <param name="Request">HTTPリクエスト</param>
	/// <returns>リアルURL（マッチングしなかったらnull）</returns>
	//=============================================================================================
	public static string GetRealUrl(HttpRequest Request)
	{
		string strTargetApplicationPathTmp = "~" + w2.Common.Web.WebUtility.GetRawUrl(Request)
			.Substring((Request.ApplicationPath == "/") ? 0 : Request.ApplicationPath.Length);
		if ((strTargetApplicationPathTmp != "~/")
			&& (strTargetApplicationPathTmp.StartsWith("~/form/", true, null) == false)
			&& (strTargetApplicationPathTmp.StartsWith("~/default.aspx", true, null) == false))
		{
			foreach (KeyValuePair<string, string> kvp in m_lFriendlyUrlRules)
			{
				Match match = Regex.Match(strTargetApplicationPathTmp, kvp.Key, RegexOptions.IgnoreCase);
				if (match.Success)
				{
					StringBuilder sbUrl = new StringBuilder(kvp.Value);
					for (int iLoop = 1; iLoop < match.Groups.Count; iLoop++)
					{
						sbUrl.Replace("@@" + iLoop + "@@", match.Groups[iLoop].Value);
					}
					if (strTargetApplicationPathTmp.Contains("?"))
					{
						sbUrl.Append(strTargetApplicationPathTmp.Split('?')[1]);
					}
					return sbUrl.ToString();
				}
			}
		}

		return null;
	}

	//=============================================================================================
	/// <summary>
	/// フレンドリURL判定
	/// </summary>
	/// <param name="strTargetApplicationPath">対象パス（「~/」or「/」からはじまる）</param>
	/// <returns>フレンドリURLか</returns>
	//=============================================================================================
	public static bool CheckFriendlyUrl(string strTargetApplicationPath)
	{
		string strTargetApplicationPathTmp = strTargetApplicationPath;

		if (strTargetApplicationPathTmp.StartsWith("/"))
		{
			strTargetApplicationPathTmp = "~/" + strTargetApplicationPath.Substring(Constants.PATH_ROOT.Length);
		}

		if ((strTargetApplicationPathTmp != "~/")
			&& (strTargetApplicationPathTmp.StartsWith("~/form/", true, null) == false)
			&& (strTargetApplicationPathTmp.StartsWith("~/default.aspx", true, null) == false))
		{
			foreach (KeyValuePair<string, string> kvp in m_lFriendlyUrlRules)
			{
				Match match = Regex.Match(strTargetApplicationPathTmp, kvp.Key, RegexOptions.IgnoreCase);
				if (match.Success)
				{
					return true;
				}
			}
		}

		return false;
	}

	//=============================================================================================
	/// <summary>
	/// フレンドリ名取得
	/// </summary>
	/// <param name="Request">HTTPリクエスト</param>
	/// <returns>フレンドリ名</returns>
	//=============================================================================================
	public static string GetCurrentFriendlyName(HttpRequest Request)
	{
		// フレンドリーURLであればフレンドリ名取得
		if (FriendlyUrlUtility.CheckFriendlyUrl(w2.Common.Web.WebUtility.GetRawUrl(Request)))
		{
			return w2.Common.Web.WebUtility.GetRawUrl(Request).Substring(Request.ApplicationPath.Length).Split('/')[1];
		}

		return "";
	}
}