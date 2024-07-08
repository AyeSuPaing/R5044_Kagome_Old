/*
=========================================================================================================
  Module      : ページ管理 ページ詳細入力(PageDesignPageInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using w2.App.Common.Design;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Domain;
using w2.Domain.PageDesign;
using Validator = w2.Cms.Manager.Codes.Common.Validator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// ページ管理 ページ詳細入力
	/// </summary>
	public class PageDesignPageInput : InputBase<PageDesignModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageDesignPageInput()
		{
			this.SourcePageId = null;
			this.ManagementTitle = "";
			this.FileNameNoExtension = "";
			this.Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PUBLIC;
			this.PcContentInput = new PageDesignContentInput();
			this.SpContentInput = new PageDesignContentInput();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ページデザイン管理モデル</param>
		public PageDesignPageInput(PageDesignModel model)
			: this()
		{
			this.PageId = model.PageId;
			this.ManagementTitle = model.ManagementTitle;
			this.FileNameNoExtension = Path.GetFileNameWithoutExtension(model.FileName);
			this.GroupId = model.GroupId;
			this.PcContentInput.NoUse = (((model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC)
				|| (model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP)) == false);
			this.SpContentInput.NoUse = (((model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP)
				|| (model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP)) == false);
			this.PageType = model.PageType;
			this.Publish = model.Publish;
			this.MetadataDesc = model.MetadataDesc;

			var modalPublishModel = new ReleaseRangeSettingModel
			{
				ConditionPublishDateFrom = model.ConditionPublishDateFrom,
				ConditionPublishDateTo = model.ConditionPublishDateTo,
				ConditionMemberOnlyType = model.ConditionMemberOnlyType,
				ConditionMemberRankId = model.ConditionMemberRankId,
				ConditionTargetListIds = model.ConditionTargetListIds,
				ConditionTargetListType = model.ConditionTargetListType
			};

			this.ReleaseRangeSettingInput = new ReleaseRangeSettingInput(
				"Page",
				"Input.ReleaseRangeSettingInput",
				modalPublishModel);
		}

		/// <summary>
		/// モデル生成
		/// </summary>
		/// <returns>モデル</returns>
		public override PageDesignModel CreateModel()
		{
			var model = new PageDesignModel
			{
				PageId = this.PageId,
				GroupId = this.GroupId,
				PageType = this.PageType,
				ManagementTitle = this.ManagementTitle
			};

			if (this.IsCustomPage)
			{
				model.Publish = this.Publish;
				var releaseRangeSettingModel = this.ReleaseRangeSettingInput.CreateModel();
				model.ConditionPublishDateFrom = releaseRangeSettingModel.ConditionPublishDateFrom;
				model.ConditionPublishDateTo = releaseRangeSettingModel.ConditionPublishDateTo;
				model.ConditionMemberOnlyType = releaseRangeSettingModel.ConditionMemberOnlyType;
				model.ConditionMemberRankId = releaseRangeSettingModel.ConditionMemberRankId;
				model.ConditionTargetListIds = releaseRangeSettingModel.ConditionTargetListIds;
				model.ConditionTargetListType = releaseRangeSettingModel.ConditionTargetListType;
				model.MetadataDesc = this.MetadataDesc;
			}

			if (this.IsHtmlPage || this.IsCustomPage)
			{
				model.FileName = this.FileName;
				model.UseType = ((this.PcContentInput.NoUse == false) && (this.SpContentInput.NoUse == false))
					? Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP
					: (this.PcContentInput.NoUse && (this.SpContentInput.NoUse == false))
						? Constants.FLG_PAGEDESIGN_USE_TYPE_SP
						: Constants.FLG_PAGEDESIGN_USE_TYPE_PC;
			}

			return model;
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <returns>結果</returns>
		public string Validate()
		{
			if (this.IsNormalPage) return string.Empty;

			var errorMessage = string.Empty;
			if (this.PcContentInput.NoUse && this.SpContentInput.NoUse)
			{
				errorMessage += WebMessages.PageDesignDeviceUseCheck;
			}
			var ht = new Hashtable
			{
				{"ManagementTitle", this.ManagementTitle},
				{"PageTitlePc", this.PcContentInput.PageTitle},
				{"PageTitleSp", this.SpContentInput.PageTitle},
				{"FileNameNoExtension", this.FileNameNoExtension},
			};
			if (w2.App.Common.Constants.SMARTPHONE_OPTION_ENABLED == false)
			{
				ht.Remove("PageTitleSp");
			}
			errorMessage += Validator.Validate("PageDesignContentDetailModify", ht);

			if (this.FileNameNoExtension.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				errorMessage += WebMessages.PageDesignFileNameError;
			}

			if (System.Text.RegularExpressions.Regex.IsMatch(
				this.FileName,
				@"(^|\\|/)(CON|PRN|AUX|NUL|CLOCK\$|COM[0-9]|LPT[0-9])(\.|\\|/|$)",
				System.Text.RegularExpressions.RegexOptions.IgnoreCase))
			{
				errorMessage += WebMessages.PageDesignFileNameUnusableError
					.Replace("@@ 1 @@", this.FileName);
			}

			if (this.IsHtmlPage == false) errorMessage += this.ReleaseRangeSettingInput.Validate();

			return errorMessage;
		}

		#region プロパティ
		/// <summary>ページID</summary>
		public long PageId { get; set; }
		/// <summary>コピー元ページID</summary>
		public long? SourcePageId { get; set; }
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle { get; set; }
		/// <summary>PCコンテンツ編集内容</summary>
		public PageDesignContentInput PcContentInput { get; set; }
		/// <summary>SPコンテンツ編集内容</summary>
		public PageDesignContentInput SpContentInput { get; set; }
		/// <summary>公開範囲設定入力内容</summary>
		public ReleaseRangeSettingInput ReleaseRangeSettingInput { get; set; }
		/// <summary>ファイル名(拡張子なし)</summary>
		public string FileNameNoExtension { get; set; }
		/// <summary>ファイル名(拡張子含む)</summary>
		private string FileName
		{
			get
			{
				string fileName;
				if (this.IsHtmlPage)
				{
					fileName = this.FileNameNoExtension.Replace(DesignCommon.PAGE_FILE_EXTENSION, string.Empty)
						+ DesignCommon.HTML_FILE_EXTENSION;
				}
				else
				{
					fileName = this.FileNameNoExtension.Replace(DesignCommon.PAGE_FILE_EXTENSION, string.Empty)
						+ DesignCommon.PAGE_FILE_EXTENSION;
				}

				return fileName;
			}
		}
		/// <summary>グループ識別ID</summary>
		public long GroupId { get; set; }
		/// <summary>ページの公開状態</summary>
		public string Publish { get; set; }
		/// <summary>ページタイプ</summary>
		public string PageType { get; set; }
		/// <summary>ディスクリプション</summary>
		public string MetadataDesc { get; set; }
		/// <summary>PC版を編集するか(falseでSP版編集)</summary>
		public bool IsPc { get; set; }
		/// <summary>新規登録か</summary>
		public bool IsRegister { get; set; }
		/// <summary>カスタムページか</summary>
		public bool IsCustomPage
		{
			get { return this.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM; }
		}
		/// <summary>HTMLページか</summary>
		public bool IsHtmlPage
		{
			get { return this.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML; }
		}
		/// <summary>標準ページか</summary>
		public bool IsNormalPage
		{
			get { return this.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL; }
		}
		#endregion
	}

	/// <summary>
	/// ページ管理 レイアウト・コンテンツ入力内容
	/// </summary>
	public class PageDesignContentInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageDesignContentInput()
		{
			this.LayoutEditInput = new LayoutEditInput();
		}

		/// <summary>ページタイトル</summary>
		public string PageTitle { get; set; }
		/// <summary>ディスクリプション</summary>
		public string Description { get; set; }
		/// <summary>コンテンツ内容</summary>
		public Dictionary<string, string> Content { get; set; }
		/// <summary>利用しない?</summary>
		public bool NoUse { get; set; }
		/// <summary>レイアウト内容</summary>
		public LayoutEditInput LayoutEditInput { get; set; }
	}
}