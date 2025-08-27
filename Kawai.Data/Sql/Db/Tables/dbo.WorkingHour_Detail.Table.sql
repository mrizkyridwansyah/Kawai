SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [WorkingHour_Detail](
	[Factory_Code] [char](15) NOT NULL,
	[Line_Code] [char](15) NOT NULL,
	[WorkingDate] [datetime] NOT NULL,
	[Shift] [char](1) NOT NULL,
	[StopTime_Cls] [char](2) NOT NULL,
	[StopTime] [numeric](18, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_WorkingHour_Detail] PRIMARY KEY CLUSTERED 
(
	[Factory_Code] ASC,
	[Line_Code] ASC,
	[WorkingDate] ASC,
	[Shift] ASC,
	[StopTime_Cls] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
