/*
=========================================================================================================
  Module      : ソーシャルログイン 機能区分(SocialLoginApiFunctionType.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// ソーシャルログイン 機能区分
	/// </summary>
	public enum SocialLoginApiFunctionType
	{
		/// <summary>ログイン・新規登録　併用</summary>
		Authenticate,
		/// <summary>既存エンドユーザに対してプロバイダを追加で紐づけ</summary>
		AuthenticateAssociate,
		/// <summary>ログイン処理のみ（新規登録させない）</summary>
		AuthenticateLogin,
		/// <summary>新規登録のみ（既存ユーザは許可しない）</summary>
		AuthenticateRegistration,
		/// <summary>認証対象のソーシャルプラスID取得</summary>
		AuthenticatedUser,
		/// <summary>ソーシャルプラスIDとw2のユーザID紐づけ</summary>
		Map,
		/// <summary>ソーシャルプラスIDとw2のユーザID紐づけ解除</summary>
		Unmap,
		/// <summary>ユーザがログインしたことのあるプロバイダ一覧取得</summary>
		ProvidersOfUser,
		/// <summary>ユーザのプロバイダ認証情報一括取得</summary>
		UserAttribute,
		/// <summary>プロバイダとの紐づけ削除</summary>
		Dissociate,
		/// <summary>ユーザ作成</summary>
		CreateUser,
		/// <summary>ユーザの結合</summary>
		MergeUser,
		/// <summary>ユーザ削除</summary>
		DeleteUser,
		/// <summary>アクセストークン取得</summary>
		AccessToken,
		/// <summary>プロバイダ追加用トークン発行</summary>
		AssociationToken,
		/// <summary>ソーシャルプラスID一括取得</summary>
		Users,
		/// <summary>個人情報の取得</summary>
		Profile,
		/// <summary>プロバイダから取得した個人情報の取得</summary>
		ProfileFromProviders,
		/// <summary>個人情報の取得更新</summary>
		UpdateProfile,
		/// <summary>個人情報削除</summary>
		DeleteProfile,
		/// <summary>個人情報取得禁止の解除</summary>
		GrantProfile,
		/// <summary>URLの同期シェア</summary>
		ShareLink,
		/// <summary>URLの非同期シェア</summary>
		Share,
		/// <summary>非同期シェアの結果取得</summary>
		ShareStatus,
		/// <summary>特定のユーザが指定のURLをシェア済み確認</summary>
		CheckShared,
		/// <summary>シェア一覧取得</summary>
		Shares,
		/// <summary>表示/非表示および検閲済みフラウの変更</summary>
		ChangeActivityStatus,
		/// <summary>コメント一覧取得</summary>
		Comments,
		/// <summary>表示/非表示および検閲済みフラウの変更</summary>
		ChangeCommentStatus,
		/// <summary>ログイン可能なプロバイダ一覧</summary>
		Providers,
		/// <summary>API情報取得</summary>
		Appinfo,
		/// <summary>プロバイダ側シークレットキー取得</summary>
		SecretKey,
		/// <summary>Twitterアカウントのフォロー</summary>
		Follow,
		/// <summary>登録コンバージョン確定</summary>
		Conversion,
		/// <summary>LINE友達追加</summary>
		LineFriends,
	}
}
