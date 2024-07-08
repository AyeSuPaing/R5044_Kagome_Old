/*
=========================================================================================================
  Module      :  オプション訴求コンテンツゲッター(OptionContentsGetter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.OptionAppeal
{
	/// <summary>
	/// オプション訴求コンテンツゲッター
	/// </summary>
	public class OptionContentsGetter
	{
		/// <summary>ドメインURL</summary>
		static readonly string _urlDomain = "https://support.w2solution.com/";
		/// <summary>オプションURL</summary>
		static readonly string _urlOption = _urlDomain + "optionAppeal/config/";
		/// <summary>オプションベースURL</summary>
		static readonly string _urlOptionBase = _urlOption + "option/base/OptionSetting.xml";
		/// <summary>スライダーベースURL</summary>
		static readonly string _urlSliderBase = _urlOption + "display/base/DisplaySample.xml";
		/// <summary>メール設定URL</summary>
		static readonly string _urlMailSetting = _urlOption + "mail/MailSetting.xml";

		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>オプション情報</returns>
		public string Get()
		{
			var optionContents = new OptionContents(_urlOptionBase);
			var result = optionContents.GetData();
			return result;
		}

		/// <summary>
		/// オプション情報取得
		/// </summary>
		/// <param name="url">取得先</param>
		/// <returns>オプション情報</returns>
		private string GetOption(string url)
		{
			var optionContents = new OptionContents(url);
			var result = optionContents.GetData();
			return result;
		}

		/// <summary>
		/// ベースオプション情報取得
		/// </summary>
		/// <returns>オプション情報</returns>
		public string GetOptionBase()
		{
			return GetOption(_urlOptionBase);
		}

		/// <summary>
		/// バージョン別オプション情報取得
		/// </summary>
		/// <param name="version">バージョン情報</param>
		/// <returns>オプション情報</returns>
		public string GetOptionVersion(string version)
		{
			var url = _urlOption + "option/version/" + version + ".xml";
			return GetOption(url);
		}

		/// <summary>
		/// スライダーベース取得
		/// </summary>
		/// <returns>スライダー情報</returns>
		public string GetSliderBase()
		{
			return GetOption(_urlSliderBase);
		}

		/// <summary>
		/// プラン別スライダー取得
		/// </summary>
		/// <param name="plan">プラン</param>
		/// <returns>スライダー情報</returns>
		public string GetSliderPlan(string plan)
		{
			var url = _urlOption + "display/plan/" + plan + ".xml";
			return GetOption(url);
		}

		/// <summary>
		/// 案件別スライダー取得
		/// </summary>
		/// <param name="project">案件</param>
		/// <returns>スライダー情報</returns>
		public string GetSliderProject(string project)
		{
			var url = _urlOption + "display/project/" + project + ".xml";
			return GetOption(url);
		}

		/// <summary>
		/// メール設定取得
		/// </summary>
		/// <returns>メール設定</returns>
		public string GetMailSetting()
		{
			return GetOption(_urlMailSetting);
		}
	}
}
