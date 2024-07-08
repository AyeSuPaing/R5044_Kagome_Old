/*
=========================================================================================================
  Module      : ラップ済カスタムバリデータ(WrappedCustomValidator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WrappedContols
{
	///*********************************************************************************************
	/// <summary>
	/// ラップ済カスタムバリデータ
	/// </summary>
	///*********************************************************************************************
	public class WrappedCustomValidator : WrappedWebControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">VIEWSTATE</param>
		public WrappedCustomValidator(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState)
			: base(
				parentControlID,
				controlID,
				hfForm,
				sbViewState)
		{
			// なにもしない //
		}

		/// <summary>
		/// カスタムバリデータの有効無効と表示非表示を制御する
		/// ※カスタムバリデータの無効化は、表示非表示をセットで行う必要がある（しないと余分なスペースを吐き出す仕様のため）
		/// </summary>
		public void DisableAndHideCustomValidator()
		{
			this.Display = ValidatorDisplay.Dynamic;
			this.EnableClientScript = false;
		}

		/// <summary>内部コントロール取得</summary>
		public new CustomValidator InnerControl
		{
			get { return (CustomValidator)base.InnerControl; }
		}
		/// <summary>検証成功</summary>
		public bool IsValid
		{
			get { return (this.HasInnerControl == false) || this.InnerControl.IsValid; }
			set { if (this.HasInnerControl) this.InnerControl.IsValid = value; }
		}
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return this.HasInnerControl ? this.InnerControl.ErrorMessage : ""; }
			set { if (this.HasInnerControl) this.InnerControl.ErrorMessage = value; }
		}
		/// <summary>クライアント検証有効か</summary>
		public bool EnableClientScript
		{
			get
			{
				return this.HasInnerControl
					? this.InnerControl.EnableClientScript
					: (bool)(this.ViewState[this.UniqueValueName + ".EnableClientScript"] ?? true);
			}
			set
			{
				if (this.HasInnerControl) this.InnerControl.EnableClientScript = value;
				else this.ViewState[this.UniqueValueName + ".EnableClientScript"] = value;
			}
		}
		/// <summary>検証コントロールのエラーメッセージの表示動作を指定する</summary>
		public ValidatorDisplay Display
		{
			get
			{
				return this.HasInnerControl
					? this.InnerControl.Display
					: (ValidatorDisplay)(this.ViewState[this.UniqueValueName + ".Display"] ?? "");
			}
			set
			{
				if (this.HasInnerControl) this.InnerControl.Display = value;
				else this.ViewState[this.UniqueValueName + ".Display"] = value;
			}
		}
		/// <summary>有効か</summary>
		public bool Enabled
		{
			get
			{
				return this.HasInnerControl
					? this.InnerControl.Enabled
					: (bool)(this.ViewState[this.UniqueValueName + ".Enabled"] ?? true);
			}
			set
			{
				if (this.HasInnerControl) this.InnerControl.Enabled = value;
				else this.ViewState[this.UniqueValueName + ".Enabled"] = value;
			}
		}
		/// <summary>検証グループ</summary>
		public string ValidationGroup
		{
			get
			{
				return this.HasInnerControl
					? this.InnerControl.ValidationGroup
					: ((string)(this.ViewState[this.UniqueValueName + ".ValidationGroup"] ?? ""));
			}
			set
			{
				if (this.HasInnerControl) this.InnerControl.ValidationGroup = value;
				else this.ViewState[this.UniqueValueName + ".ValidationGroup"] = value;
			}
		}
		/// <summary>検証対象</summary>
		public string ControlToValidate
		{
			get
			{
				return this.HasInnerControl
					? this.InnerControl.ControlToValidate
					: ((string)(this.ViewState[this.UniqueValueName + ".ControlToValidate"] ?? ""));
			}
			set
			{
				if (this.HasInnerControl) this.InnerControl.ControlToValidate = value;
				else this.ViewState[this.UniqueValueName + ".ControlToValidate"] = value;
			}
		}
	}
}