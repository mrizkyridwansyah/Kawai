SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Manufacture_Line](
	[Manufacture_Code] [char](15) NOT NULL,
	[Line_Code] [char](15) NOT NULL,
	[Line_Name] [char](20) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Company_Code] [char](25) NULL,
 CONSTRAINT [PK_Manufacture_Line] PRIMARY KEY CLUSTERED 
(
	[Manufacture_Code] ASC,
	[Line_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Manufacture_Line]  WITH NOCHECK ADD  CONSTRAINT [FK_Manufacture_Line_Trade_Master] FOREIGN KEY([Manufacture_Code])
REFERENCES [Trade_Master] ([Trade_Code])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Manufacture_Line] CHECK CONSTRAINT [FK_Manufacture_Line_Trade_Master]
GO
