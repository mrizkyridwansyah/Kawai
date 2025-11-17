SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [IQC_SamplingBarcodeDetail](
	[SamplingID] [int] IDENTITY(1,1) NOT NULL,
	[InspectionID] [int] NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[CurrentStock] [numeric](18, 2) NOT NULL,
	[SampleQTY] [numeric](18, 2) NOT NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[SamplingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [IQC_SamplingBarcodeDetail] ADD  DEFAULT (getdate()) FOR [RegisterDate]
GO
