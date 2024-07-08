if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_User]
GO
/*
=========================================================================================================
  Module      : ユーザマスタ(w2_User.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_User] (
	[user_id] [nvarchar] (30) NOT NULL,
	[user_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC_USER'),
	[mall_id] [nvarchar] (30) NOT NULL DEFAULT (N'OWN_SITE'),
	[name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[name1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[name2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[name_kana] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[nick_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mail_addr2] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[zip] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[zip1] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[zip2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[addr] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[addr1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[addr2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[addr3] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[addr4] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[tel1] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[tel1_1] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[tel1_2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tel1_3] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tel2] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[tel2_1] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[tel2_2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tel2_3] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tel3] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[tel3_1] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[tel3_2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tel3_3] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[fax] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[fax_1] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[fax_2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[fax_3] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[sex] [nvarchar] (10) NOT NULL DEFAULT (N'UNKNOWN'),
	[birth] [datetime],
	[birth_year] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[birth_month] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[birth_day] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[company_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[company_post_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[company_exective_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[advcode_first] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[attribute1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute6] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute7] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute8] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute9] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute10] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[login_id] [nvarchar] (256) COLLATE Japanese_CS_AS_KS_WS NOT NULL DEFAULT (N''),
	[password] [nvarchar] (100) COLLATE Japanese_CS_AS_KS_WS NOT NULL DEFAULT (N''),
	[question] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[answer] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[career_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mobile_uid] [nvarchar] (50) COLLATE Japanese_CS_AS_KS_WS NOT NULL DEFAULT (N''),
	[remote_addr] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_flg] [nvarchar] (10) NOT NULL DEFAULT (N'UNKNOWN'),
	[user_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[member_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_uid] [nvarchar] (100) DEFAULT (N''),
	[date_last_loggedin] [datetime],
	[user_management_level_id] [nvarchar] (30) NOT NULL DEFAULT (N'normal'),
	[integrated_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[easy_register_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[fixed_purchase_member_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[access_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[access_language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[access_language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[access_currency_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[access_currency_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[last_birthday_point_add_year] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[addr_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[addr_country_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[addr5] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[last_birthday_coupon_publish_year] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[order_count_order_realtime] [int] NOT NULL DEFAULT (0),
	[order_count_old] [int] NOT NULL DEFAULT (0),
	[referral_code] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[referred_user_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_User] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_User] PRIMARY KEY  CLUSTERED
	(
		[user_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_User_1] ON [dbo].[w2_User]([login_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_User_2] ON [dbo].[w2_User]([name_kana]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_User_3] ON [dbo].[w2_User]([career_id], [mobile_uid]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_User_4] ON [dbo].[w2_User]([advcode_first]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_User_5] ON [dbo].[w2_User]([del_flg], [integrated_flg]) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_w2_User_6] ON [dbo].[w2_User] ([del_flg],[integrated_flg],[referral_code]) INCLUDE ([user_id]) ON [PRIMARY]
GO

/*
-- v5.11
ALTER TABLE [w2_User] ADD [integrated_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');

EXEC w2_AlterColumnTypeSp 'w2_User', 'user_memo', 'nvarchar(MAX) NOT NULL';
ALTER TABLE [w2_User] ADD [referral_code] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_User] ADD [referred_user_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/