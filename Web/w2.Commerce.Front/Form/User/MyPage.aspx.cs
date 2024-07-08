/*
=========================================================================================================
  Module      : マイページ画面処理(MyPage.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Option;
using w2.App.Common.Option.PointOption;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.User;

public partial class Form_User_MyPage : BasePage
{
	#region ラップ済コントロール宣言
	/// <summary>利用可能期間内期間限定ポイントリピータ</summary>
	private WrappedRepeater WrUsableLimitedTermPoint
	{
		get
		{
			return m_wrUsableLimitedTermPoint = m_wrUsableLimitedTermPoint
				?? GetWrappedControl<WrappedRepeater>("rUsableLimitedTermPoint");
		}
	}
	private WrappedRepeater m_wrUsableLimitedTermPoint;
	/// <summary>利用可能期間前期間限定ポイントリピータ</summary>
	private WrappedRepeater WrUnusableLimitedTermPoint
	{
		get
		{
			return m_wrUnusableLimitedTermPoint = m_wrUnusableLimitedTermPoint
				?? GetWrappedControl<WrappedRepeater>("rUnusableLimitedTermPoint");
		}
	}
	private WrappedRepeater m_wrUnusableLimitedTermPoint;
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		InitializeComponents();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// ユーザ情報を最新の状態に更新
		this.LoginUser = new UserService().Get(this.LoginUserId);
		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Adjust point and member rank by Cross Point api
			UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
			this.LoginMemberRankInfo = (this.LoginUserMemberRankId != null)
				? MemberRankOptionUtility.GetMemberRankList().FirstOrDefault(memberRank => (memberRank.MemberRankId == LoginUserMemberRankId))
				: null;
		}

		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			// Update LoginUserPoint
			this.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);

			// 利用可能期間前の期間限定ポイント(ポイント発行日＆有効期限でグループ化)
			this.WrUnusableLimitedTermPoint.DataSource = this.LoginUserPoint.LimitedTermPoint
				.Where(p => p.IsPointTypeComp && (DateTime.Now <= p.EffectiveDate))
				.OrderBy(p => p.PointKbnNo)
				.GroupBy(p => new
				{
					p.EffectiveDate,
					p.ExpiryDate,
				})
				.Select(p => new LimitedTermPoint
				{
					EffectiveDate = p.Key.EffectiveDate,
					ExpiryDate = p.Key.ExpiryDate,
					Point = p.Sum(x => x.Point),
				});

			// 利用可能期間内の期間限定ポイント(ポイント発行日＆有効期限でグループ化)
			this.WrUsableLimitedTermPoint.DataSource = this.LoginUserPoint.LimitedTermPoint
				.Where(p => p.IsPointTypeComp && (p.EffectiveDate <= DateTime.Now))
				.OrderBy(p => p.PointKbnNo)
				.GroupBy(p => new
				{
					p.EffectiveDate,
					p.ExpiryDate,
				})
				.Select(p => new LimitedTermPoint
				{
					EffectiveDate = p.Key.EffectiveDate,
					ExpiryDate = p.Key.ExpiryDate,
					Point = p.Sum(x => x.Point),
				});
		}

		DataBind();
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示
	/// <summary>期間限定ポイントが使用可能か</summary>
	protected bool IsLimitedTermPointUsable
	{
		get { return Constants.CROSS_POINT_OPTION_ENABLED == false; }
	}
}
