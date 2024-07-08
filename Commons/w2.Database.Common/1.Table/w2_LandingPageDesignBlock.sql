if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LandingPageDesignBlock]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_LandingPageDesignBlock]
GO
/*
=========================================================================================================
  Module      : Lpページブロックデザイン (w2_LandingPageDesignBlock.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_LandingPageDesignBlock] (
	[page_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[design_type] [nvarchar] (20) NOT NULL DEFAULT (N'PC'),
	[block_index] [int] NOT NULL DEFAULT (0),
	[block_class_name] [nvarchar] (512) NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_LandingPageDesignBlock] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_LandingPageDesignBlock] PRIMARY KEY  CLUSTERED
	(
		[page_id],
		[design_type],
		[block_index]
	) ON [PRIMARY]
GO
