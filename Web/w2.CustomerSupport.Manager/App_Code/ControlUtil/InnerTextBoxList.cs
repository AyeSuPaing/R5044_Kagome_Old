/*
=========================================================================================================
  Module      : InnerTextBoxListクラス(InnerTextBoxList.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// InnerTextBoxListクラス
///   指定したControlが含んでいる、TextBoxのみを抽出したリストを表す。
///   （Controlが持つControlをすべて検索して抽出する）
/// </summary>
public class InnerTextBoxList : InnerSpecificControlList<TextBox>
{
	/// <summary>
	/// コンストラクタ
	///   (指定したControlが含んでいる、TextBoxのみを抽出したリストを返す)
	/// </summary>
	/// <param name="cRootControl">検索対象とするControl</param>
	public InnerTextBoxList(Control cRootControl)
		: base(cRootControl)
	{
	}
	/// <summary>
	/// コンストラクタ
	///   (指定したControlが含んでいる、TextBoxのみを抽出したリストを返す)
	/// </summary>
	/// <param name="cRootControl">検索対象とするControlのリスト</param>
	public InnerTextBoxList(List<Control> lRootControls) 
		: base(lRootControls)
	{
	}

	/// <summary>
	/// 一行入力である全てのTextBoxに対してAttributeを設定する
	/// </summary>
	/// <param name="strKey">AttributeのKey</param>
	/// <param name="strValue">AttributeのValue</param>
	public void SetAttributeToSingleLineTextBox(string strKey, string strValue)
	{
		GetAllSingleLineTextBoxList()
			.ForEach((tbTarget) => 
			{
				if (tbTarget.Attributes[strKey] == null)
				{
					tbTarget.Attributes.Add(strKey, strValue); 
				}
			});
	}

	/// <summary>
	/// 一行入力である全てのTextBoxを取得する
	/// </summary>
	private List<TextBox> GetAllSingleLineTextBoxList()
	{
		return this.FindAll((tbTarget) => { return tbTarget.TextMode != TextBoxMode.MultiLine; });
	}

	/// <summary>
	/// 全てのTextBoxの特定のAttributeを取り除く
	/// </summary>
	/// <param name="strKey">AttributeのKey</param>
	public void RemoveAttributeFromSingleLineTextBox(string strKey)
	{
		GetAllSingleLineTextBoxList()
			.ForEach((tbTarget) =>
			{
				tbTarget.Attributes.Remove(strKey);
			});
	}

	/// <summary>
	/// 押された時にイベントをキャンセルするスクリプトを設定する
	/// </summary>
	public void SetKeyPressEventCancelEnterKey()
	{
		// イベントとスクリプトを設定
		string strKeyPressEvent = "onkeypress";
		string strScript = "if (event.keyCode==13){ return false;}";
		RemoveAttributeFromSingleLineTextBox(strKeyPressEvent);
		SetAttributeToSingleLineTextBox(strKeyPressEvent, strScript);
	}
}