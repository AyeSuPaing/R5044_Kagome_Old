/*
=========================================================================================================
  Module      : サイト情報ワーカーサービス(SiteInformationWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Castle.Core.Internal;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.SiteInformation;
using w2.Common.Logger;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// サイト情報ワーカーサービス
	/// </summary>
	public class SiteInformationWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 編集ビューモデル作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		internal ModifyViewModel CreateModifyVm()
		{
			return new ModifyViewModel();
		}

		/// <summary>
		/// サイト情報更新
		/// </summary>
		/// <param name="input">サイト情報入力</param>
		/// <returns>エラーがあればエラーメッセージ</returns>
		internal string UpdateShopMessageXml(SiteInformationInput input)
		{
			var physicalFilePathSiteInformationXml = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.FILE_XML_SHOP_MESSAGE);

			var xmlDocument = new XmlDocument();
			xmlDocument.Load(physicalFilePathSiteInformationXml);

			foreach (var xmlInfo in input.XmlInfo)
			{
				var xmlNode = xmlDocument.DocumentElement.SelectSingleNode(xmlInfo.NodeName + "/Text");

				if (CheckXmlInputText(xmlInfo.Text) == false
					|| CheckXmlInputText(xmlInfo.Description) == false
					|| CheckXmlInputText(xmlInfo.TagName) == false)
				{
					return WebMessages.SiteInfomationXmlTextError.Replace("@@ 1 @@", "]]>");
				}

				if (xmlNode != null) xmlNode.InnerXml = "<![CDATA[" + xmlInfo.Text + "]]>";

				// 既存の項目の場合は処理を終わる
				xmlNode = xmlDocument.DocumentElement.SelectSingleNode(xmlInfo.NodeName + "/AddTag");
				if (xmlNode == null) continue;

				if (xmlInfo.InputNodeName.Trim().IsNullOrEmpty())
				{
					return WebMessages.SiteInfomationIdEmptyError;
				}

				xmlNode = xmlDocument.DocumentElement.SelectSingleNode(xmlInfo.NodeName + "/TextBoxRows");
				if (xmlNode != null)
				{
					var matche = Regex.Matches(xmlInfo.Text, "\r\n");
					xmlNode.InnerText = matche.Count != 0 ? "10" : "1";
				}
				
				xmlNode = xmlDocument.DocumentElement.SelectSingleNode(xmlInfo.NodeName + "/Description");
				if (xmlNode != null)
				{
					xmlNode.InnerXml = "<![CDATA[" + xmlInfo.Description + "]]>";
				}

				xmlNode = xmlDocument.DocumentElement.SelectSingleNode(xmlInfo.NodeName + "/Name");
				if (xmlNode != null)
				{
					xmlNode.InnerXml = "<![CDATA[" + xmlInfo.TagName + "]]>";
				}
				
				// ノード名が変更されていない場合は処理をここで終わる
				if (xmlInfo.NodeName == xmlInfo.InputNodeName.Trim()) continue;

				if ((Regex.Match(xmlInfo.InputNodeName.Trim(), "^[0-9].*")).Success)
				{
					return WebMessages.SiteInfomationIdFirstLetterError;
				}

				if (!(Regex.Match(xmlInfo.InputNodeName.Trim(), "^[a-zA-Z0-9]+$")).Success)
				{
					return WebMessages.SiteInfomationIdFormatError;
				}

				// ノード名重複チェック
				if (xmlDocument.DocumentElement.SelectSingleNode(xmlInfo.InputNodeName.Trim()) != null)
				{
					return WebMessages.SiteInfomationIdDuplicateError;
				}

				var node = xmlDocument.DocumentElement.SelectSingleNode(xmlInfo.NodeName);
				var parentNode = xmlDocument.CreateElement(xmlInfo.InputNodeName.Trim());
				var childNodes = node.ChildNodes;
				var nun = childNodes.Count;
				for (var i = 0; i < nun; i++)
				{
					parentNode.AppendChild(childNodes.Item(0));
				}
				xmlDocument.DocumentElement.InsertAfter(parentNode, node);
				xmlDocument.DocumentElement.RemoveChild(node);
			}

			// XMLファイル更新
			try
			{
				xmlDocument.Save(physicalFilePathSiteInformationXml);
			}
			catch (Exception ex)
			{
				// ファイルが読み取り専用の場合
				if ((File.GetAttributes(physicalFilePathSiteInformationXml) & FileAttributes.ReadOnly) != 0)
				{
					return WebMessages.FileReadOnlyError.Replace("@@ 1 @@", "(ROOT)" + Constants.FILE_XML_SHOP_MESSAGE);
				}
				// 読み取り専用以外のエラーの場合
				FileLogger.WriteError(ex);
				return WebMessages.FileUpdateError.Replace("@@ 1 @@", "(ROOT)" + Constants.FILE_XML_SHOP_MESSAGE);
			}
			this.SessionWrapper.SiteInformationStatus = "Update";
			return string.Empty;
		}

		/// <summary>
		/// 追加
		/// </summary>
		/// <returns>メッセージ</returns>
		internal string AddShopMessageXml()
		{
			var physicalFilePathSiteInformationXml = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.FILE_XML_SHOP_MESSAGE);
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(physicalFilePathSiteInformationXml);
			var xmlNode = xmlDocument.DocumentElement;
			var newNodeName = "NEW";
			var count = 1;
			// NodeNameが重複していた場合は名前の末に数字を付ける
			while (xmlDocument.DocumentElement.SelectSingleNode(newNodeName) != null)
			{
				newNodeName = "NEW" + (count++);
			}
			var node = xmlDocument.CreateElement(newNodeName);
			var ele = xmlDocument.CreateElement("Name");
			ele.InnerXml = "<![CDATA[]]>";
			node.AppendChild(ele);
			ele = xmlDocument.CreateElement("TextBoxRows");
			ele.InnerXml = "1";
			node.AppendChild(ele);
			ele = xmlDocument.CreateElement("Text");
			ele.InnerXml = "<![CDATA[]]>";
			node.AppendChild(ele);
			ele = xmlDocument.CreateElement("Description");
			ele.InnerXml = "<![CDATA[]]>";
			node.AppendChild(ele);
			ele = xmlDocument.CreateElement("AddTag");
			ele.InnerXml = "<![CDATA[]]>";
			node.AppendChild(ele);
			xmlNode.AppendChild(node);

			// XMLファイル更新
			try
			{
				xmlDocument.Save(physicalFilePathSiteInformationXml);
			}
			catch (Exception ex)
			{
				// ファイルが読み取り専用の場合
				if ((File.GetAttributes(physicalFilePathSiteInformationXml) & FileAttributes.ReadOnly) != 0)
				{
					return WebMessages.FileReadOnlyError.Replace("@@ 1 @@", "(ROOT)" + Constants.FILE_XML_SHOP_MESSAGE);
				}
				// 読み取り専用以外のエラーの場合
				FileLogger.WriteError(ex);
				return WebMessages.FileUpdateError.Replace("@@ 1 @@", "(ROOT)" + Constants.FILE_XML_SHOP_MESSAGE);
			}

			this.SessionWrapper.SiteInformationStatus = "Add";
			return string.Empty;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="nodeName">ノード名</param>
		/// <returns>メッセージ</returns>
		public string DeleteShopMessageXml(string nodeName)
		{
			var physicalFilePathSiteInformationXml = Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				Constants.FILE_XML_SHOP_MESSAGE);

			var xmlDocument = new XmlDocument();
			xmlDocument.Load(physicalFilePathSiteInformationXml);
			var xmlNode = xmlDocument.DocumentElement.SelectSingleNode(nodeName);
			if (xmlNode == null) return WebMessages.SiteInfomationDeleteError;

			xmlDocument.DocumentElement.RemoveChild(xmlNode);
			// XMLファイル更新
			try
			{
				xmlDocument.Save(physicalFilePathSiteInformationXml);
			}
			catch (Exception ex)
			{
				// ファイルが読み取り専用の場合
				if ((File.GetAttributes(physicalFilePathSiteInformationXml) & FileAttributes.ReadOnly) != 0)
				{
					return WebMessages.FileReadOnlyError.Replace("@@ 1 @@", "(ROOT)" + Constants.FILE_XML_SHOP_MESSAGE);
				}
				// 読み取り専用以外のエラーの場合
				FileLogger.WriteError(ex);
				return WebMessages.FileUpdateError.Replace("@@ 1 @@", "(ROOT)" + Constants.FILE_XML_SHOP_MESSAGE);
			}

			this.SessionWrapper.SiteInformationStatus = "Delete";
			return "";
		}

		/// <summary>
		/// ステータスを確認
		/// </summary>
		/// <returns>ステータス</returns>
		internal string CheckStatus()
		{
			// どの処理を終えた状態かを返す。
			var status = this.SessionWrapper.SiteInformationStatus;
			this.SessionWrapper.SiteInformationStatus = "";
			return status;
		}

		/// <summary>
		/// 入力値チェック(]]>が含まれていないか)
		/// </summary>
		/// <param name="text">入力値</param>
		/// <returns>エラーメッセージ</returns>
		private bool CheckXmlInputText(string text)
		{
			if ((text.IsNullOrEmpty() == false) 
				&& text.Contains("]]>"))
			{
				return false;
			}
			return true;
		}
	}
}