if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductGroup]
GO
/*
=========================================================================================================
  Module      : ���i�O���[�v�}�X�^ (w2_ProductGroup.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductGroup] (
	[product_group_id] [nvarchar] (30) NOT NULL,
	[product_group_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[begin_date] [datetime] NOT NULL,
	[end_date] [datetime],
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[product_group_page_contents_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[product_group_page_contents] [nvarchar] (max),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductGroup] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductGroup] PRIMARY KEY  CLUSTERED
	(
		[product_group_id]
	) ON [PRIMARY]
GO