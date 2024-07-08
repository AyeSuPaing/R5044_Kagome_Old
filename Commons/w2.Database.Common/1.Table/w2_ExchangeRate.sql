if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ExchangeRate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ExchangeRate]
GO
/*
=========================================================================================================
  Module      : �בփ��[�g�}�X�^ (w2_ExchangeRate.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ExchangeRate] (
	[src_currency_code] [char] (3) NOT NULL DEFAULT (N''),
	[dst_currency_code] [char] (3) NOT NULL DEFAULT (N''),
	[exchange_rate] [decimal] (24,12) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ExchangeRate] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ExchangeRate] PRIMARY KEY  CLUSTERED
	(
		[src_currency_code],
		[dst_currency_code]
	) ON [PRIMARY]
GO