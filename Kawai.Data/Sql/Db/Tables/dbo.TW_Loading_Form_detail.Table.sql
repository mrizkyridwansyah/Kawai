SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TW_Loading_Form_detail](
	[SI_No] [char](15) NULL,
	[Class] [char](3) NULL,
	[Seq_No] [int] NULL,
	[Item_Code] [char](18) NULL,
	[Serial_No] [char](8) NULL,
	[Qty] [int] NULL,
	[Scan_Status] [char](1) NULL,
	[Export_Status] [char](1) NULL
) ON [PRIMARY]
GO
