/*
=========================================================================================================
  Module      : 特集エリアタイプ入力クラス(FeatureAreaTypeImput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.FeatureArea;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 特集エリアタイプ入力クラス
	/// </summary>
	public class FeatureAreaTypeInput : InputBase<FeatureAreaTypeModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureAreaTypeInput()
		{
			this.ActionType = Constants.FLG_FEATUREAREATYPE_ACTION_TYPE_VERTICAL;
			this.PcTagInput = new FeatureAreaTypeDetailTagInput()
			{
				IsPc = true,
			};
			this.SpTagInput = new FeatureAreaTypeDetailTagInput()
			{
				IsPc = false,
			};
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override FeatureAreaTypeModel CreateModel()
		{
			var model = new FeatureAreaTypeModel()
			{
				AreaTypeId = this.AreaTypeId,
				AreaTypeName = this.AreaTypeName,
				ActionType = this.ActionType,
				InternalMemo = this.InternalMemo,
				PcStartTag = this.PcTagInput.StartTag,
				PcRepeatTag = this.PcTagInput.RepeatTag,
				PcEndTag = this.PcTagInput.EndTag,
				PcScriptTag = this.PcTagInput.ScriptTag,
				SpStartTag = this.SpTagInput.StartTag,
				SpRepeatTag = this.SpTagInput.RepeatTag,
				SpEndTag = this.SpTagInput.EndTag,
				SpScriptTag = this.SpTagInput.ScriptTag,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate(bool isUpdate)
		{
			var errorMessage = Validator.Validate(
				isUpdate ? "FeatureAreaTypeModify" : "FeatureAreaTypeRegister",
				CreateModel().DataSource);

			return errorMessage;
		}

		/// <summary> 特集エリアタイプID </summary>
		public string AreaTypeId { get; set; }
		/// <summary> 特集エリアタイプ名</summary>
		public string AreaTypeName { get; set; }
		/// <summary> 動作タイプ</summary>
		public string ActionType { get; set; }
		/// <summary> サムネイルファイル名 </summary>
		public string FileName { get; set; }
		/// <summary> 内部メモ </summary>
		public string InternalMemo { get; set; }
		/// <summary>PCタグ入力クラス</summary>
		public FeatureAreaTypeDetailTagInput PcTagInput { get; set; }
		/// <summary>SPタグ入力クラス</summary>
		public FeatureAreaTypeDetailTagInput SpTagInput { get; set; }
	}

	/// <summary>
	/// 特集エリアタイプ詳細入力クラス
	/// </summary>
	public class FeatureAreaTypeDetailTagInput
	{
		/// <summary> PC/SP </summary>
		public bool IsPc { get; set; }
		/// <summary> 開始タグ </summary>
		public string StartTag { get; set; }
		/// <summary> 繰り返しタグ </summary>
		public string RepeatTag { get; set; }
		/// <summary> 終了タグ </summary>
		public string EndTag { get; set; }
		/// <summary>スクリプトタグ </summary>
		public string ScriptTag { get; set; }
	}
}