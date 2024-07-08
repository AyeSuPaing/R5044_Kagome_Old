/*
=========================================================================================================
  Module      : デザイン設定(DesignSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Helper;
using w2.Common.Helper.Attribute;

namespace w2.Cms.Manager.Codes.Settings
{
	/// <summary>
	/// デザイン設定
	/// </summary>
	public class DesignSetting
	{
		/// <summary>管理画面デザインタイプ</summary>
		private enum ManagerDesignSettingType
		{
			/// <summary>製品</summary>
			[EnumTextName("w2")]
			W2,
			/// <summary>リピートプラス</summary>
			[EnumTextName("Repeat")]
			Repeat,
			/// <summary>リピートフード</summary>
			[EnumTextName("RepeatFood")]
			RepeatFood
		};

		/// <summary>サイトCSSクラス名</summary>
		private enum SiteCssClass
		{
			/// <summary>製品</summary>
			[EnumTextName("w2cms")]
			W2cms,
			/// <summary>リピートプラス</summary>
			[EnumTextName("repeatplus")]
			Repeatplus,
			/// <summary>リピートフード</summary>
			[EnumTextName("RepeatFood")]
			RepeatFood
		}

		/// <summary>デザイン設定ディレクトリ</summary>
		private enum ManagerDesignSettingDir
		{
			/// <summary>製品</summary>
			[EnumTextName("w2")]
			W2,
			/// <summary>リピートプラス</summary>
			[EnumTextName("Repeat")]
			Repeat,
			/// <summary>リピートフード</summary>
			[EnumTextName("RepeatFood")]
			RepeatFood
		}

		/// <summary>w2CMSか</summary>
		public static bool IsW2Cms
		{
			get { return (Constants.MANAGER_DESIGN_SETTING.ToLower() == ManagerDesignSettingType.W2.ToText().ToLower()); }
		}
		/// <summary>RepeatPlusか</summary>
		public static bool IsRepeatPlus
		{
			get { return Constants.MANAGER_DESIGN_SETTING.ToLower() == ManagerDesignSettingType.Repeat.ToText().ToLower(); }
		}
		/// <summary>RepeatFoodか</summary>
		public static bool IsRepeatFood
		{
			get { return (Constants.MANAGER_DESIGN_SETTING.ToLower() == ManagerDesignSettingType.RepeatFood.ToText().ToLower()); }
		}
		/// <summary>w2製品か</summary>
		public static bool IsW2Product
		{
			get { return (IsW2Cms || IsRepeatPlus || IsRepeatFood); }
		}
		/// <summary>サイトCSSクラス名</summary>
		public static string SiteCssClassName
		{
			get
			{
				if (IsW2Cms)
				{
					return SiteCssClass.W2cms.ToText();
				}
				else if (IsRepeatPlus)
				{
					return SiteCssClass.Repeatplus.ToText();
				}
				else if (IsRepeatFood)
				{
					return SiteCssClass.RepeatFood.ToText();
				}
				else
				{
					return "hanyou";
				}
			}
		}
		/// <summary>管理画面デザイン設定ディレクトリ名</summary>
		public static string ManagerDesignSettingDirName
		{
			get
			{
				if (IsW2Cms)
				{
					return ManagerDesignSettingDir.W2.ToText();
				}
				else if (IsRepeatPlus)
				{
					return ManagerDesignSettingDir.Repeat.ToText();
				}
				else if (IsRepeatFood)
				{
					return ManagerDesignSettingDir.RepeatFood.ToText();
				}
				else
				{
					return Constants.MANAGER_DESIGN_SETTING;
				}
			}
		}
	}
}
