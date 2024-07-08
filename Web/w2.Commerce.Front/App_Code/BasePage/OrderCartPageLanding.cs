/*
=========================================================================================================
  Module      : 注文カートランディング系基底ページ(OrderCartPageLanding.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order;
using w2.App.Common.Web.Process;

///*********************************************************************************************
/// <summary>
/// 注文カートランディング系基底ページ
/// </summary>
///*********************************************************************************************
public class OrderCartPageLanding : OrderCartPage
{
	/// <summary>
	/// ページ初期化
	/// </summary>
	protected new void Page_Init(object sender, EventArgs e)
	{
		this.Process.Page_Init(sender, e);
	}

	/// <summary>
	/// 遷移元チェック
	/// </summary>
	public new void CheckOrderUrlSession()
	{
		this.Process.CheckOrderUrlSession();
	}

	/// <summary>
	/// パラメタに格納されたNextUrl取得
	/// </summary>
	/// <returns>NextUrlの値（パラメタ無しの場合はnull）</returns>
	public new string GetNextUrlForCheck()
	{
		return this.Process.GetNextUrlForCheck();
	}

	/// <summary>
	/// カートに商品追加時のエラー発生時処理（プロセスから呼ばれる）
	/// </summary>
	/// <param name="product">追加NGの商品</param>
	/// <param name="errorMsg">エラーメッセージ</param>
	public void ExecAddProductError(LandingCartProduct product, string errorMsg)
	{
		OnAddProductError(product, errorMsg);
	}

	/// <summary>
	/// カートに商品追加時のエラー発生時処理（デザイン側でオーバーライドされる）
	/// </summary>
	/// <param name="product">追加NGの商品</param>
	/// <param name="errorMsg">エラーメッセージ</param>
	protected virtual void OnAddProductError(LandingCartProduct product, string errorMsg)
	{
	}

	/// <summary>
	/// セッションよりバックアップを復元
	/// </summary>
	protected void RestoreCartFromSession()
	{
		this.Process.RestoreCartFromSession();
	}

	/// <summary>プロセス</summary>
	protected new OrderLandingProcess Process
	{
		get { return (OrderLandingProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new OrderLandingProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
	/// <summary>ランディングカート入力画面URL</summary>
	public string LandingCartInputUrl { get { return this.Process.LandingCartInputUrl; } }
	/// <summary>ランディングカート保持用セッションキー</summary>
	public string LadingCartSessionKey { get { return this.Process.LadingCartSessionKey; } }
	/// <summary>ランディングカート保持用画面遷移正当性チェック用セッションキー</summary>
	public string LadingCartNextPageForCheck { get { return this.Process.LadingCartNextPageForCheck; } }
	/// <summary>CMSランディングページ用 確認画面スキップフラグ用セッションキー</summary>
	public string LadingCartConfirmSkipFlgSessionKey { get { return this.Process.LadingCartConfirmSkipFlgSessionKey; } }
	/// <summary>CMSランディングページ用 商品セット選択用セッションキー</summary>
	public string LadingCartProductSetSelectSessionKey { get { return this.Process.LadingCartProductSetSelectSessionKey; } }
	/// <summary>ランディングカート入力画面絶対パス</summary>
	public string LandingCartInputAbsolutePath
	{
		get { return this.Process.LandingCartInputAbsolutePath; }
		set { this.Process.LandingCartInputAbsolutePath = value; }
	}
	/// <summary>カートリストランディングページかどうか</summary>
	public bool IsCartListLp { get { return this.Process.IsCartListLp; } }
	/// <summary>ランディングカート保持用セッションキー（バックアップ）</summary>
	public string BackupLadingCartSessionKey { get { return this.Process.BackupLadingCartSessionKey; } }
	/// <summary>バックアップがあるか？</summary>
	public bool IsBackupLandingCartSession { get { return this.Process.IsBackupLandingCartSession; } }
}
