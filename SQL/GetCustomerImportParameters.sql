use CLIENTLIST
go
Create procedure GetCustomerImportParameters
as
select 
	DatabaseName,
	[Name],
	vfs_path,
	Data_Format,
	Delimeter,
	Data_Has_Header,
	Failure_Email_Addresses,
	Approved_Folder
from [CLIENT].[IMPEX_CONFIGURATIONS]
where 
	enabled = 1
