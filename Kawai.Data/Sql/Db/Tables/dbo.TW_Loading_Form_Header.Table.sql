SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TW_Loading_Form_Header](
	[Stuffing_Date] [datetime] NULL,
	[SI_No] [char](15) NULL,
	[Dest_1] [char](35) NULL,
	[Dest_2] [char](35) NULL,
	[Remarks] [char](5) NULL,
	[Container_Status] [char](5) NULL,
	[Complete] [char](1) NULL
) ON [PRIMARY]
GO
