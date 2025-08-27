SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Delivery_Place](
	[Trade_Code] [char](15) NOT NULL,
	[Location_Code] [char](15) NOT NULL,
	[Location_Name] [char](25) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Delivery_Place] PRIMARY KEY CLUSTERED 
(
	[Trade_Code] ASC,
	[Location_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Delivery_Place]  WITH NOCHECK ADD  CONSTRAINT [FK_Delivery_Place_Trade_Master] FOREIGN KEY([Trade_Code])
REFERENCES [Trade_Master] ([Trade_Code])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Delivery_Place] CHECK CONSTRAINT [FK_Delivery_Place_Trade_Master]
GO
