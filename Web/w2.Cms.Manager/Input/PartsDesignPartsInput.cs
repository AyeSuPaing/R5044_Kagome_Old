/*
=========================================================================================================
  Module      : パーツ管理 詳細入力(PartsDesignPartsInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Domain;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// パーツ管理 詳細入力
	/// </summary>
	public class PartsDesignPartsInput : InputBase<PartsDesignModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsDesignPartsInput()
		{
			this.ManagementTitle = "";
			this.PcContentInput = new PartsDesignContentInput();
			this.SpContentInput = new PartsDesignContentInput();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">パーツ管理モデル</param>
		public PartsDesignPartsInput(PartsDesignModel model) : this()
		{
			this.PartsId = model.PartsId;
			this.GroupId = model.GroupId;
			this.PcContentInput.NoUse = (((model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC)
				|| (model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP)) == false);
			this.SpContentInput.NoUse = (((model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP)
				|| (model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP)) == false);
			this.PartsType = model.PartsType;
			this.Publish = model.Publish;
			this.ManagementTitle = model.ManagementTitle;

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
				"Parts",
				"Input.ReleaseRangeSettingInput",
				modalPublishModel);
		}

		/// <summary>
		/// パーツ管理モデル生成
		/// </summary>
		/// <returns>パーツ管理モデル</returns>
		public override PartsDesignModel CreateModel()
		{
			var model = new PartsDesignModel
			{
				PartsId = this.PartsId,
				GroupId = this.GroupId,
				PartsType = this.PartsType,
				ManagementTitle = this.ManagementTitle
			};

			if (this.IsCustomPage)
			{
				model.UseType = ((this.PcContentInput.NoUse == false) && (this.SpContentInput.NoUse == false))
					?　Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP
					: (this.PcContentInput.NoUse && (this.SpContentInput.NoUse == false))
						? Constants.FLG_PAGEDESIGN_USE_TYPE_SP
						: Constants.FLG_PAGEDESIGN_USE_TYPE_PC;
				model.Publish = this.Publish;

				var releaseRangeSettingModel = this.ReleaseRangeSettingInput.CreateModel();
				model.ConditionPublishDateFrom = releaseRangeSettingModel.ConditionPublishDateFrom;
				model.ConditionPublishDateTo = releaseRangeSettingModel.ConditionPublishDateTo;
				model.ConditionMemberOnlyType = releaseRangeSettingModel.ConditionMemberOnlyType;
				model.ConditionMemberRankId = releaseRangeSettingModel.ConditionMemberRankId;
				model.ConditionTargetListIds = releaseRangeSettingModel.ConditionTargetListIds;
				model.ConditionTargetListType = releaseRangeSettingModel.ConditionTargetListType;
			}

			return model;
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <returns>結果</returns>
		public string Validate()
		{
			if (this.IsCustomPage == false) return string.Empty;

			var errorMessage = string.Empty;
			if (this.PcContentInput.NoUse && this.SpContentInput.NoUse)
			{
				errorMessage += WebMessages.PageDesignDeviceUseCheck;
			}

			var ht = new Hashtable
			{
				{ "ManagementTitle", this.ManagementTitle },
			};
			errorMessage += Validator.Validate("PartsDesignContentDetailModify", ht);

			errorMessage += this.ReleaseRangeSettingInput.Validate();
			return errorMessage;
		}

		/// <summary>パーツID</summary>
		public long PartsId { get; set; }
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle { get; set; }
		/// <summary>PCコンテンツ編集内容</summary>
		public PartsDesignContentInput PcContentInput { get; set; }
		/// <summary>SPコンテンツ編集内容</summary>
		public PartsDesignContentInput SpContentInput { get; set; }
		/// <summary>公開範囲設定入力内容</summary>
		public ReleaseRangeSettingInput ReleaseRangeSettingInput { get; set; }
		/// <summary>グループ識別ID</summary>
		public long GroupId { get; set; }
		/// <summary>公開状態</summary>
		public string Publish { get; set; }
		/// <summary>パーツタイプ</summary>
		public string PartsType { get; set; }
		/// <summary>カスタムパーツか</summary>
		public bool IsCustomPage
		{
			get { return this.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM; }
		}
		/// <summary>新規作成ファイル名</summary>
		public string CreateNewFileName { get; set; }
		/// <summary>新規作成 テンプレートファイルパス</summary>
		public string TemplateFilePath { get; set; }
		/// <summary>パーツディレクトリ</summary>
		public string FileDirPath { get; set; }
	}

	/// <summary>
	/// パーツ管理 コンテンツ入力内容
	/// </summary>
	public class PartsDesignContentInput
	{
		/// <summary>コンテンツ内容</summary>
		public Dictionary<string, string> Content { get; set; }
		/// <summary>利用しない?</summary>
		public bool NoUse { get; set; }
	}
}