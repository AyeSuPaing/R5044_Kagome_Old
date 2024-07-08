/*
=========================================================================================================
  Module      : ページングコンテンツ (ProductListPaginationContent.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// ページングコンテンツ
/// </summary>
[Serializable]
public class ProductListPaginationContent
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="pageNo">ページ番号</param>
	public ProductListPaginationContent(int pageNo)
	{
		this.PageNo = pageNo;
		this.IsLoaded = false;
	}

	/// <summary>
	/// ロード済みに変更
	/// </summary>
	public void ToLoaded()
	{
		this.IsLoaded = true;
	}

	/// <summary>
	/// 未ロードに変更
	/// </summary>
	public void UnLoaded()
	{
		this.IsLoaded = false;
	}

	/// <summary>ページ番号</summary>
	public int PageNo { get; set; }
	/// <summary>ロード済みか</summary>
	public bool IsLoaded { get; set; }
}
