SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Machine_Capacity](
	[Factory_Code] [char](15) NOT NULL,
	[Line_Code] [char](15) NOT NULL,
	[Machine_Code] [char](15) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[CycleTime] [numeric](5, 2) NULL,
	[Efficiency] [numeric](5, 2) NULL,
	[QtyMachine] [numeric](5, 2) NULL,
	[QtyProcess] [numeric](5, 2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Machine_Capacity] PRIMARY KEY CLUSTERED 
(
	[Factory_Code] ASC,
	[Line_Code] ASC,
	[Machine_Code] ASC,
	[Item_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
