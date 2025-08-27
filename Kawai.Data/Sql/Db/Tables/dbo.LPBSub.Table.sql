SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [LPBSub](
	[TanggalTOS] [datetime] NULL,
	[TanggalINT] [datetime] NULL,
	[Doku] [nvarchar](50) NULL,
	[Tanggal] [smalldatetime] NULL,
	[Kode_Dept] [nvarchar](20) NULL,
	[Doku_PO] [nvarchar](25) NULL,
	[Kode_Supplier] [nvarchar](20) NULL,
	[SuratJalan] [nvarchar](50) NULL,
	[Kode] [nvarchar](20) NULL,
	[Jumlah] [float] NULL,
	[Kode_Satuan] [nvarchar](20) NULL,
	[Harga] [float] NULL,
	[HargaJasa] [float] NULL,
	[HargaMaterial] [float] NULL,
	[Diskon] [float] NULL,
	[Nilai] [float] NULL,
	[bPPN] [bit] NULL,
	[PPN] [float] NULL,
	[Keterangan] [nvarchar](255) NULL,
	[StatusBC] [nvarchar](20) NULL,
	[Doku_BC] [nvarchar](20) NULL,
	[NoUrut] [float] NULL,
	[EntryDate] [nvarchar](25) NULL,
	[UserID] [nvarchar](25) NULL,
	[Hapus] [nvarchar](25) NULL,
	[Nama] [nvarchar](255) NULL,
	[NamaSatuan] [nvarchar](50) NULL,
	[Status] [char](1) NULL,
	[DataReceivedCls] [char](1) NULL,
	[DataReceivedDate] [datetime] NULL,
	[SuratJalan_No] [nvarchar](50) NULL,
	[PO_No] [nvarchar](50) NULL,
	[Seq_No] [numeric](18, 0) NULL,
	[Item_Code] [char](15) NULL,
	[Last_Update] [datetime] NULL
) ON [PRIMARY]
GO
