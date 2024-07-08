/*
=========================================================================================================
  Module      : テキストボックスイベント生成クラス(TextBoxEventScriptCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// テキストボックスイベント生成クラス
/// </summary>
/// <remarks>
/// <para>w2.textboxevents.jsと一緒に利用します。</para>
/// <para>RegisterInitializeScript(Page page)利用必須です。</para>
/// </remarks>
public class TextBoxEventScriptCreator
{
	/// <summary>
	/// Use for Link Button Constructor
	/// </summary>
	public TextBoxEventScriptCreator()
	{
		this.OnClientClickScript = CreateOnClientClickScript();
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="postbackEventControl">ポストバック対象コントロール</param>
	public TextBoxEventScriptCreator(WrappedControl postbackEventControl)
	{
		this.OnEnterPressPostbackScript = CreateOnEnterPressPostbackScript(postbackEventControl);
		this.OnBlurPostbackScript = CreateOnBlurPostbackScript(postbackEventControl);
		this.OnClickPostbackScript = CreateOnClickPostbackScript(postbackEventControl);
	}

	/// <summary>
	/// 非同期ポストバック起動スクリプト登録
	/// </summary>
	/// <param name="page">スクリプト登録対象ページ</param>
	public void RegisterInitializeScript(Page page)
	{
		ScriptManager.RegisterStartupScript(page, page.GetType(), "myscript", "InitializeLastBlurOnEnterTime();", true);
	}

	/// <summary>
	/// コントロールにスクリプト追加（チェックボックス）
	/// </summary>
	/// <param name="target">対象コントロール</param>
	public void AddScriptToControl(WrappedCheckBox target)
	{
		target.Attributes["onclick"] = this.OnClickPostbackScript;
	}

	/// <summary>
	/// コントロールにスクリプト追加（ラジオボタン）
	/// </summary>
	/// <param name="target">対象コントロール</param>
	public void AddScriptToControl(WrappedRadioButton target)
	{
		target.Attributes["onclick"] = this.OnClickPostbackScript;
	}

	/// <summary>
	/// コントロールにスクリプト追加（現状テキストボックスのみ）
	/// </summary>
	/// <param name="target">対象コントロール</param>
	public void AddScriptToControl(WrappedTextBox target)
	{
		target.Attributes["onkeypress"] = this.OnEnterPressPostbackScript;
		target.Attributes["onblur"] = this.OnBlurPostbackScript;
	}

	/// <summary>
	/// Script added to the control
	/// </summary>
	/// <param name="target">Target control</param>
	public void AddScriptToControl(WrappedLinkButton target)
	{
		// IE11でリンクボタンイベントを連続クリックすると
		// RegisterStartupScriptが実行されないため、InitializeLastBlurOnEnterTimeを追加
		target.OnClientClick = "InitializeLastBlurOnEnterTime(); " + this.OnClientClickScript;
	}

	/// <summary>
	/// Create scripts focus out postback events(Only use for LinkButton)
	/// </summary>
	/// <returns>Attribute set for the script string of onblur</returns>
	private string CreateOnClientClickScript()
	{
		// Event is run by the executable or determined to not run in double
		return "if (CheckBlurOnEnterEnabled()) { ResetLastBlurOnEnterTime(); return true; } return false;";
	}

	/// <summary>
	/// フォーカスアウトポストバックイベント用スクリプト作成
	/// </summary>
	/// <param name="postbackEventControl">ポストバック対象コントロール</param>
	/// <returns>onblur属性セット用スクリプト文字列</returns>
	private string CreateOnClickPostbackScript(WrappedControl postbackEventControl)
	{
		// 二重でイベントが走らないよう実行可能か判断して実行する
		return "if (CheckBlurOnEnterEnabled()) { ResetLastBlurOnEnterTime(); " + CreatePostBackScript(postbackEventControl) + " }";
	}

	/// <summary>
	/// Enterキー押下ポストバックイベント用スクリプト作成
	/// </summary>
	/// <param name="postbackEventControl">ポストバック対象コントロール</param>
	/// <returns>onkeypress属性セット用スクリプト文字列</returns>
	private string CreateOnEnterPressPostbackScript(WrappedControl postbackEventControl)
	{
		// 二重でイベントが走らないよう実行可能か判断して実行する
		return "if (CheckBlurOnEnterEnabled() && (event.keyCode == 13)) { ResetLastBlurOnEnterTime(); " + CreatePostBackScript(postbackEventControl) + " return false;}";
	}

	/// <summary>
	/// フォーカスアウトポストバックイベント用スクリプト作成
	/// </summary>
	/// <param name="postbackEventControl">ポストバック対象コントロール</param>
	/// <returns>onblur属性セット用スクリプト文字列</returns>
	private string CreateOnBlurPostbackScript(WrappedControl postbackEventControl)
	{
		// 二重でイベントが走らないよう実行可能か判断して実行する
		return "if (CheckBlurOnEnterEnabled()) { ResetLastBlurOnEnterTime(); " + CreatePostBackScript(postbackEventControl) + " }";
	}

	/// <summary>
	/// ポストバックスクリプト作成
	/// </summary>
	/// <param name="control"></param>
	/// <returns></returns>
	private string CreatePostBackScript(WrappedControl control)
	{
		return "if (__doPostBack) { __doPostBack('" + control.UniqueId + "',''); }";
	}

	/// <summary>クリックポストバックイベント用スクリプト</summary>
	private string OnClickPostbackScript { get; set; }
	/// <summary>Enterキー押下ポストバックイベント用スクリプト</summary>
	private string OnEnterPressPostbackScript { get; set; }
	/// <summary>フォーカスアウトポストバックイベント用スクリプト</summary>
	private string OnBlurPostbackScript { get; set; }
	/// <summary>Script of OnClientClick</summary>
	private string OnClientClickScript { get; set; }
}