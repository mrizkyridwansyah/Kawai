SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process_Master](
	[Item_Code] [char](25) NOT NULL,
	[SeqNo] [char](2) NULL,
	[Process_Cls] [char](6) NOT NULL,
	[Standard_Time] [numeric](18, 5) NULL,
	[Trade_Code] [char](15) NULL,
	[Currency_Code] [char](2) NULL,
	[Cost_Minute] [numeric](18, 5) NULL,
 CONSTRAINT [PK_Process_Master] PRIMARY KEY CLUSTERED 
(
	[Item_Code] ASC,
	[Process_Cls] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Process_Master]  WITH CHECK ADD  CONSTRAINT [FK_Process_Master_Curr_Cls] FOREIGN KEY([Currency_Code])
REFERENCES [Curr_Cls] ([Curr_Cls])
GO
ALTER TABLE [Process_Master] CHECK CONSTRAINT [FK_Process_Master_Curr_Cls]
GO
ALTER TABLE [Process_Master]  WITH CHECK ADD  CONSTRAINT [FK_Process_Master_Process_Cls] FOREIGN KEY([Process_Cls])
REFERENCES [Process_Cls] ([Process_Cls])
GO
ALTER TABLE [Process_Master] CHECK CONSTRAINT [FK_Process_Master_Process_Cls]
GO
ALTER TABLE [Process_Master]  WITH CHECK ADD  CONSTRAINT [FK_Process_Master_Process_Master1] FOREIGN KEY([Item_Code], [Process_Cls])
REFERENCES [Process_Master] ([Item_Code], [Process_Cls])
GO
ALTER TABLE [Process_Master] CHECK CONSTRAINT [FK_Process_Master_Process_Master1]
GO
ALTER TABLE [Process_Master]  WITH CHECK ADD  CONSTRAINT [FK_Process_Master_Trade_Master] FOREIGN KEY([Trade_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [Process_Master] CHECK CONSTRAINT [FK_Process_Master_Trade_Master]
GO
