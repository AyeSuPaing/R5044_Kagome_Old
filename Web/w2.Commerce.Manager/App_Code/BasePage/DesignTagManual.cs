/*
=========================================================================================================
  Module      : デザインタグマニュアルページ(DesignTagManual.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Xml;

/// <summary>
/// DesignTagManual の概要の説明です
/// </summary>
public class DesignTagManual : BasePage
{
	// オプション属性
	private const string OPTION_MENU_ATTRIBUTE = "option";

	/// <summary>
	/// デザインタグ説明取得
	/// </summary>
	/// <param name="strTargetXmlFilePath"></param>
	/// <returns></returns>
	protected List<KeyValuePair<string, List<KeyValuePair<string, string>>>> GetNameAndDesignTagDescriptions(string strTargetXmlFilePath)
	{
		List<KeyValuePair<string, List<KeyValuePair<string, string>>>> lDesignTagDescriptions = new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();

		XmlDocument xdDesignXml = new XmlDocument();
		xdDesignXml.Load(strTargetXmlFilePath);
		foreach (XmlNode xnRoot in xdDesignXml.ChildNodes)
		{
			if (xnRoot.NodeType == XmlNodeType.Comment)
			{
				continue;
			}

			// ページセッティング？
			if (xnRoot.SelectSingleNode("PageSetting") != null)
			{
				foreach (XmlNode xnTarget in xnRoot.SelectNodes("PageSetting"))
				{
					lDesignTagDescriptions.Add(
						new KeyValuePair<string, List<KeyValuePair<string, string>>>(
							xnTarget.Attributes["name"].InnerText,
							GetDesignTagDescriptions(xnTarget)));
				}
			}
			// パーツセッティング？
			else if (xnRoot.SelectSingleNode("PartsSetting") != null)
			{
				foreach (XmlNode xnTarget in xnRoot.SelectNodes("PartsSetting"))
				{
					lDesignTagDescriptions.Add(
						new KeyValuePair<string, List<KeyValuePair<string, string>>>(
							xnTarget.Attributes["name"].InnerText,
							GetDesignTagDescriptions(xnTarget)));
				}
			}
		}

		return lDesignTagDescriptions;
	}

	/// <summary>
	/// デザインパーツタグ説明取得
	/// </summary>
	protected List<KeyValuePair<string, string>> GetNameAndDesignPartsTagDescriptions()
	{
		List<KeyValuePair<string, string>> lResult = new List<KeyValuePair<string, string>>();

		// 標準パーツタグ説明追加
		XmlDocument xdDesignParts = new XmlDocument();
		xdDesignParts.Load(AppDomain.CurrentDomain.BaseDirectory + @"Xml\Design\Design_Parts_03Others.xml");
		foreach (XmlNode xnPartsSetting in xdDesignParts.SelectNodes("/Design_Parts/PartsSetting"))
		{
			if (xnPartsSetting.NodeType != XmlNodeType.Comment)
			{
				try
				{
					foreach (XmlNode xnTag in xnPartsSetting.SelectNodes("LayoutParts"))
					{
						lResult.Add(new KeyValuePair<string, string>(xnTag.SelectSingleNode("Value").InnerText,
							"パーツ　「" + xnTag.SelectSingleNode("Name").InnerText + "」　を出力します。")
						);
					}
				}
				catch (Exception ex)
				{
					throw new Exception("ノード「" + xnPartsSetting.InnerText + "」の解析に失敗しました。", ex);
				}
			}
		}

		// カスタムパーツタグ説明追加
		string strTitle;
		foreach (string strFilePath in Directory.GetFiles(Constants.PHYSICALDIRPATH_FRONT_PC + @"Page\Parts\"))
		{
			strTitle = StringUtility.ToEmpty(DesignBasePage.GetAspxTitle(strFilePath));
			string strKey = DesignBasePage.TAG_PARTS_BGN + strTitle + DesignBasePage.TAG_PARTS_END;
			string strValue = "パーツ　「" + strTitle + "」　を出力します。";

			if (lResult.Contains(new KeyValuePair<String, String>(strKey, strValue)))
			{
				int iCount = 1;
				while (true)
				{
					strKey = DesignBasePage.TAG_PARTS_BGN + strTitle + "(" + iCount.ToString() + ")" + DesignBasePage.TAG_PARTS_END;
					strValue = "パーツ　「" + strTitle + "(" + iCount.ToString() + ")」　を出力します。";
					if (lResult.Contains(new KeyValuePair<String, String>(strKey, strValue)) == false)
					{
						break;
					}
					iCount++;
				}
			}
			lResult.Add(new KeyValuePair<string, string>(strKey, strValue));
		}

		return lResult;
	}

	/// <summary>
	/// デザインタグ説明取得
	/// </summary>
	/// <param name="xnTargetSettings"></param>
	/// <returns></returns>
	protected List<KeyValuePair<string, string>> GetDesignTagDescriptions(XmlNode xnTargetSettings)
	{
		List<KeyValuePair<string, string>> lDesignTagDescriptions = new List<KeyValuePair<string, string>>();
		if (xnTargetSettings.SelectNodes("TagSetting") != null)
		{
			foreach (XmlNode xnSetting in xnTargetSettings.SelectNodes("TagSetting"))
			{
				XmlNode xnDescription = xnSetting.SelectSingleNode("Description");
				if (xnDescription != null)
				{
					StringBuilder sbTagString = new StringBuilder();
					if (xnSetting.SelectSingleNode("Raw") != null)
					{
						sbTagString.Append(xnSetting.SelectSingleNode("Raw").InnerText);
					}
					else if (xnSetting.SelectSingleNode("RawHead") != null)
					{
						sbTagString.Append(xnSetting.SelectSingleNode("RawHead").InnerText);
						sbTagString.Append("****");
						sbTagString.Append(xnSetting.SelectSingleNode("RawFoot").InnerText);
					}
					else if (xnSetting.SelectSingleNode("RawBgn") != null)
					{
						sbTagString.Append(xnSetting.SelectSingleNode("RawBgn").InnerText);
					}

					if (xnSetting.SelectSingleNode("RawItemBgn") != null)
					{
						sbTagString.Append("\r\n");
						sbTagString.Append("****\r\n");
						sbTagString.Append(xnSetting.SelectSingleNode("RawItemBgn").InnerText);
					}
					if (xnSetting.SelectSingleNode("RawItemEnd") != null)
					{
						sbTagString.Append("\r\n");
						sbTagString.Append("****\r\n");
						sbTagString.Append(xnSetting.SelectSingleNode("RawItemEnd").InnerText);
					}

					if (xnSetting.SelectSingleNode("RawEnd") != null)
					{
						sbTagString.Append("\r\n");
						sbTagString.Append("****\r\n");
						sbTagString.Append(xnSetting.SelectSingleNode("RawEnd").InnerText);
					}

					if (sbTagString.Length != 0)
					{
						lDesignTagDescriptions.Add(
							new KeyValuePair<string, string>(
								sbTagString.ToString(),
								xnDescription.InnerText.Trim()));
					}
				}
			}
		}

		return lDesignTagDescriptions;
	}
}
