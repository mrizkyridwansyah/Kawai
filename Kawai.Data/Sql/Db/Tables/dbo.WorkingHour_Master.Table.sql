SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [WorkingHour_Master](
	[Factory_Code] [char](15) NOT NULL,
	[Line_Code] [char](15) NOT NULL,
	[WorkingDate] [datetime] NOT NULL,
	[Shift] [char](1) NOT NULL,
	[OperationTime] [numeric](18, 5) NULL,
	[OverTime] [numeric](18, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_WorkingHour_Master] PRIMARY KEY CLUSTERED 
(
	[Factory_Code] ASC,
	[Line_Code] ASC,
	[WorkingDate] ASC,
	[Shift] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
