SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Email_Config](
	[smtp_server] [varchar](30) NOT NULL,
	[smtp_desc] [varchar](75) NULL,
	[port_number] [numeric](18, 0) NULL,
	[smtp_timeout] [numeric](18, 0) NULL,
	[email_address] [varchar](50) NULL,
	[user_email] [varchar](50) NULL,
	[pass_email] [varchar](50) NULL,
	[timer] [numeric](18, 0) NULL,
	[subject] [varchar](75) NULL,
	[mail_header] [varchar](50) NULL,
	[mail_content] [varchar](150) NULL,
	[mail_footer] [varchar](150) NULL,
	[mail_sign] [varchar](20) NULL,
	[ccemail_address] [varchar](50) NULL,
	[bccemail_address] [varchar](500) NULL,
 CONSTRAINT [PK_Email_Config] PRIMARY KEY CLUSTERED 
(
	[smtp_server] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
