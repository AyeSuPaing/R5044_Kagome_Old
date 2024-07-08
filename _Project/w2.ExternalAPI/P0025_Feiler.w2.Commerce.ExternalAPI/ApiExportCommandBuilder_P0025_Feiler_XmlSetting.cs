/*
=========================================================================================================
  Module      : Feiler XmlSetting Memo Content Match(ApiExportCommandBuilder_P0025_Feiler_XmlSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common;

namespace P0025_Feiler.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// Feiler XmlSetting Memo Content Match
	/// </summary>
	public class ApiExportCommandBuilder_P0025_Feiler_XmlSetting
	{
		public const string XML_TAG_ITEM = "item";
		public const string XML_TAG_MEMO = "memo";
		public const string XML_TAG_NUMBER = "number";
		public const string XML_TAG_COMPARE = "compare";

		#region Public Method

		/// <summary>
		/// Gets the wrapping bag type setting.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns></returns>
		public string GetWrappingBagTypeSetting(DataRowView record)
		{
			var memoString = string.Empty;
			string header;

			switch (record["mall_id"].ToString())
			{
				case CustomConstants.FLG_ORDER_MALL_ID_OWN_SITE:
					header = "包装紙";
					memoString = ApiCommon.GetMemoContent(header, record["memo"].ToString());
					break;

				case CustomConstants.FLG_ORDER_MALL_ID_RAKUTEN:
					header = "ラッピング";
					memoString = ApiCommon.GetMemoContent(header, record["relation_memo"].ToString());

					var startString = "(包装紙:";
					var	endString = ")";
					if (memoString.Trim().StartsWith(startString) && memoString.Trim().EndsWith(endString))
					{
						memoString = ApiCommon.GetMemoContentInSection(startString, endString, memoString);
					}
					break;

				case CustomConstants.FLG_ORDER_MALL_ID_YAHOO:
					header = "■カスタムフィールド■" + Environment.NewLine + "－－ギフト包装の種類－－";
					var footer = "－－のし－－";
					memoString = ApiCommon.GetMemoContentInSection(header, footer, record["memo"].ToString());
					break;
			}

			for (var i = 0; i < WrappingBagTypeSettingString.Count; i++)
			{
				if ((WrappingBagTypeSettingCompare[i] == "contain")
					&& memoString.Contains(WrappingBagTypeSettingString[i]))
				{
					return WrappingBagTypeSettingNumber[i];
				}
				else if (WrappingBagTypeSettingString[i] == memoString)
				{
					return WrappingBagTypeSettingNumber[i];
				}
			}

			return "00";
		}

		#endregion

		#region Singleton Implement

		/// <summary> Thread Sync Lock </summary>
		private static object _syncLock = new Object();
		/// <summary> Singleton instance </summary>
		private static ApiExportCommandBuilder_P0025_Feiler_XmlSetting _instance;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value> The instance. </value>
		public static ApiExportCommandBuilder_P0025_Feiler_XmlSetting Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncLock)
					{
						if (_instance == null)
						{
							_instance = new ApiExportCommandBuilder_P0025_Feiler_XmlSetting();
						}
					}
				}

				return _instance;
			}
		}

		/// <summary>
		/// Private Contructor
		/// </summary>
		private ApiExportCommandBuilder_P0025_Feiler_XmlSetting()
		{
			LoadSetting();
		}

		/// <summary>
		/// Loads the setting.
		/// </summary>
		private void LoadSetting()
		{
			WrappingBagTypeSettingString = new List<string>();
			WrappingBagTypeSettingNumber = new List<string>();
			WrappingBagTypeSettingCompare = new List<string>();

			var iniFilePath = Path.Combine(Constants.PHYSICALDIRPATH_EXTERNALAPI_STORAGE_LOCATION, CustomConstants.EXPORT_ORDERSHIPPING_SETTING_FILENAME);
			if (File.Exists(iniFilePath) == false) { return; }

			var settingFileContent = File.ReadAllText(iniFilePath);
			var findSetting = settingFileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).FirstOrDefault(line => line.StartsWith("setting="));
			if (findSetting == null) { return; }

			var settingFilePath = findSetting.Split('=')[1];
			if (File.Exists(settingFilePath) == false) { return; }

			try
			{
				var items = XDocument.Load(settingFilePath).Descendants(XML_TAG_ITEM);

				WrappingBagTypeSettingString.AddRange(items.Select(item => item.Element(XML_TAG_MEMO) != null ? item.Element(XML_TAG_MEMO).Value : string.Empty));
				WrappingBagTypeSettingNumber.AddRange(items.Select(item => item.Element(XML_TAG_NUMBER) != null ? item.Element(XML_TAG_NUMBER).Value : string.Empty));
				WrappingBagTypeSettingCompare.AddRange(items.Select(item => item.Element(XML_TAG_COMPARE) != null ? item.Element(XML_TAG_COMPARE).Value : string.Empty));
			}
			catch (Exception)
			{
			}
		}

		#endregion

		#region Property

		/// <summary> The wrapping bag type setting string. </summary>
		private List<string> WrappingBagTypeSettingString;
		/// <summary> The wrapping bag type value string. </summary>
		private List<string> WrappingBagTypeSettingNumber;
		/// <summary> The wrapping bag type setting compare. </summary>
		private List<string> WrappingBagTypeSettingCompare;

		#endregion
	}
}
