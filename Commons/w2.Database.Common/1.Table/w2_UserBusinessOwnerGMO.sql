if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserBusinessOwnerGMO]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserBusinessOwnerGMO]
GO
/*
=========================================================================================================
  Module      : UserBusinessOwner (w2_UserBusinessOwnerGMO.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

CREATE TABLE [dbo].[w2_UserBusinessOwnerGMO] (
	[user_id] [nvarchar](30) NOT NULL,
	[owner_name1] [nvarchar](21) NOT NULL  DEFAULT(N''),
	[owner_name2] [nvarchar](21) NOT NULL  DEFAULT(N''),
	[owner_name_kana1] [nvarchar](25) NOT NULL  DEFAULT(N''),
	[owner_name_kana2] [nvarchar](25) NOT NULL  DEFAULT(N''),
	[owner_birth] [datetime],
	[request_budget] [decimal](8,0) NOT NULL  DEFAULT(0),
	[gmo_shop_customer_id] [nvarchar](50) NOT NULL  DEFAULT(N''),
	[credit_status] [nvarchar](20) NOT NULL DEFAULT(N'NG'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())

) ON [PRIMARY]

ALTER TABLE [dbo].[w2_UserBusinessOwnerGMO] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserBusinessOwnerGMO] PRIMARY KEY CLUSTERED
	(
		[user_id]
	) ON [PRIMARY]