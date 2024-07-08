/*
=========================================================================================================
  Module      : トップページページャーユーザーコントロール処理(ListPager.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_Top_ListPager : BaseUserControl
{
	#region デリゲート・イベント
	/// <summary>
	/// ページ変更イベントデリゲート
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void OnPageChanged(object sender, BasePageCs.PagerEventArgs e);
	/// <summary>
	/// ページ変更イベント
	/// </summary>
	public event OnPageChanged PageChanged;
	#endregion

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}
	#endregion

	#region +DispPager ページャ表示
	/// <summary>
	/// ページャ表示
	/// </summary>
	/// <param name="totalCount">全件数</param>
	/// <param name="currentPageNo">現在のページ番号</param>
	public void DispPager(int totalCount, int currentPageNo)
	{
		this.CurrentPageNumber = currentPageNo;
		int lastPageNo = ((totalCount - 1) / this.DispListMax) + 1;
		lbPagerBack.Visible = (this.CurrentPageNumber > 1);
		imgPagerBackNoLink.Visible = (lbPagerBack.Visible == false);
		lbPagerNext.Visible = (this.CurrentPageNumber < lastPageNo);
		imgPagerNextNoLink.Visible = (lbPagerNext.Visible == false);
		var pagerList = new List<int>();
		if (lastPageNo < this.DispPagerAroundPageCount * 2 + 1)
		{
			for (int i = 1; i <= lastPageNo; i++)
			{
				pagerList.Add(i);
			}
		}
		else
		{
			// 1ページ目は必ず記述
			int tmp = 1;
			pagerList.Add(tmp++);
			if (this.CurrentPageNumber - this.DispPagerAroundPageCount > tmp)
			{
				pagerList.Add(-1);	// 「...」
				tmp = this.CurrentPageNumber - this.DispPagerAroundPageCount;
			}
			// 現在のページの - this.DispPagerAroundPageCount ～ + this.DispPagerAroundPageCount までページNO追加
			while((tmp <= this.CurrentPageNumber + this.DispPagerAroundPageCount) && (tmp < lastPageNo))
			{
				pagerList.Add(tmp++);
			}
			// ラストに満たなければ「...」 追加
			if (tmp < lastPageNo)
			{
				pagerList.Add(-1);	// 「...」
			}
			// ラストページ追加
			pagerList.Add(lastPageNo);
		}

		rPagerPageLink.DataSource = pagerList;
		rPagerPageLink.DataBind();
	}
	#endregion

	#region #lbPagerBack_Click ページ戻るボタンクリック
	/// <summary>
	/// ページ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPagerBack_Click(object sender, EventArgs e)
	{
		ChangePage(this.CurrentPageNumber - 1);
	}
	#endregion

	#region #lbPagerNext_Click ページ進むボタンクリック
	/// <summary>
	/// ページ進むボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPagerNext_Click(object sender, EventArgs e)
	{
		ChangePage(this.CurrentPageNumber + 1);
	}
	#endregion

	#region #rPagerPageLink_ItemCommand ページリンククリック
	/// <summary>
	/// ページリンククリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rPagerPageLink_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		ChangePage(int.Parse(e.CommandArgument.ToString()));
	}
	#endregion

	#region -ChangePage ページ変更
	/// <summary>
	/// ページ変更
	/// </summary>
	/// <param name="pageNo">ページ番号</param>
	private void ChangePage(int pageNo)
	{
		PageChanged(this, new BasePageCs.PagerEventArgs(pageNo));
	}
	#endregion

	#region プロパティ
	/// <summary>ページNO</summary>
	protected int CurrentPageNumber
	{
		get { return (int)(ViewState["CurrentPageNumber"] ?? 1); }
		set { ViewState["CurrentPageNumber"] = value; }
	}
	/// <summary>表示最大件数</summary>
	public int DispListMax
	{
		get { return (int)(ViewState["DispListMax"] ?? 100); }
		set { ViewState["DispListMax"] = value; }
	}
	/// <summary>
	/// ページャ前後表示件数（前後 X 件表示）
	/// </summary>
	/// <remarks>
	/// （2の場合で6ページ目表示のとき | 1 | ... | 4 | 5 | 6 | 7 | 8 |  ... | 103 |となる。
	/// </remarks>
	public int DispPagerAroundPageCount
	{
		get { return (int)(ViewState["DispPagerAroundPageCount"] ?? 4); }
		set { ViewState["DispPagerAroundPageCount"] = value; }
	}
	#endregion
}