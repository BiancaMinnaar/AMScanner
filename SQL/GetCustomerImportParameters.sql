create procedure GetCustomerImportParameters
as
select 
	name
	,[vfs_path]
	,[data_format]
	,[delimeter]
	,[data_has_header]
	,[failure_email_addresses]
from [IMPEX].[CONFIGURATIONS]
where 
	enabled = 1
	and [impex_type] = 'IMPORT'