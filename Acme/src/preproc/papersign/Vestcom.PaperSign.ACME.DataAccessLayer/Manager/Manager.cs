using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Vestcom.Core.Database.Configuration;
using Vestcom.Data;
using Vestcom.PaperSign.ACME.DataAccessLayer.Converters;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.DataAccessLayer.Manager
{
    /// <summary>
    /// Manager class
    /// </summary>
    /// <seealso cref="Vestcom.Core.Database.Configuration.DataManager" />
    /// <seealso cref="Vestcom.PaperSign.ACME.DataAccessLayer.Manager.IManager" />
    public class Manager : DataManager, IManager
    {
        protected readonly IConverter converter;

        public Manager()
        {
            converter = new Converter();
        }

        /// <summary>
        /// Gets the acme signs.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public InputRecords GetAcmeSigns(int clientId, string orderId)
        {
            SqlCommand command = new SqlCommand
            {
                CommandText = Constants.GetAcmeInput.Name,
                CommandType = CommandType.StoredProcedure,
            };
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeInput.Columns.ClientId, clientId);
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeInput.Columns.OrderId, orderId);
            return this.GetAll<InputRecords>(command, delegate(IDataReader reader, ref List<InputRecords> items)
            {
                items = new List<InputRecords>();
                InputRecords item = new InputRecords
                {
                    InputFileRecords = new List<InputFile>(),
                    SignLayouts = new List<SignLayout>(),
                    Departments = new List<Department>(),
                    Headings = new List<Heading>(),
                    Images = new List<Image>(),
                    PromoPGMs = new List<PromoPGM>(),
                    UnitPriceEntrys = new List<UnitPriceEntry>(),
                    SubstituteStock = new List<SubstituteStock>(),
                    Exceptions = new List<ExceptionReport>()
                };
                //Reading Signs
                converter.Convert(reader, item.InputFileRecords);
                reader.NextResult();
                converter.Convert(reader, item.Departments);
                reader.NextResult();
                converter.Convert(reader, item.Headings);
                reader.NextResult();
                converter.Convert(reader, item.SignLayouts);
                reader.NextResult();
                converter.Convert(reader, item.Images);
                reader.NextResult();
                converter.Convert(reader, item.PromoPGMs);
                reader.NextResult();
                converter.Convert(reader, item.UnitPriceEntrys);
                reader.NextResult();
                converter.Convert(reader, item.SubstituteStock);
                items.Add(item);
            }).FirstOrDefault();
        }


        //usp_GetACMEHolidayInputSigns

        public HolidayInputRecords GetAcmeHolidaySigns(int clientId, string orderId)
        {
            SqlCommand command = new SqlCommand
            {
                CommandText = Constants.GetAcmeHolidayInput.Name,
                CommandType = CommandType.StoredProcedure,
            };
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeInput.Columns.ClientId, clientId);
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeInput.Columns.OrderId, orderId);
            return this.GetAll<HolidayInputRecords>(command, delegate (IDataReader reader, ref List<HolidayInputRecords> items)
            {
                items = new List<HolidayInputRecords>();
                HolidayInputRecords item = new HolidayInputRecords
                {
                    HolidayInputFileRecords = new List<HolidayInput>(),
                    SignLayouts = new List<SignLayout>(),
                    Departments = new List<Department>(),
                    Headings = new List<Heading>(),
                    Images = new List<Image>(),
                    PromoPGMs = new List<PromoPGM>(),
                    UnitPriceEntrys = new List<UnitPriceEntry>(),
                    SubstituteStock = new List<SubstituteStock>(),
                    Exceptions = new List<ExceptionReport>()
                };
                //Reading Signs
                converter.Convert(reader, item.HolidayInputFileRecords);
                reader.NextResult();
                converter.Convert(reader, item.Departments);
                reader.NextResult();
                converter.Convert(reader, item.Headings);
                reader.NextResult();
                converter.Convert(reader, item.SignLayouts);
                reader.NextResult();
                converter.Convert(reader, item.Images);
                reader.NextResult();
                converter.Convert(reader, item.PromoPGMs);
                reader.NextResult();
                converter.Convert(reader, item.UnitPriceEntrys);
                reader.NextResult();
                converter.Convert(reader, item.SubstituteStock);
                items.Add(item);
            }).FirstOrDefault();
        }



        /// <summary>
        /// Gets the downback signs.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="BatchNumber">The batch number.</param>
        /// <param name="Action">The action.</param>
        /// <returns></returns>
        public ACMEDownbackRecords GetDownbackSigns(int clientId, string BatchNumber, string Action, int signId)
        {
            SqlCommand command = new SqlCommand
            {
                CommandText = Constants.GetAcmeDownBackInput.Name,
                CommandType = CommandType.StoredProcedure,
            };
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeDownBackInput.Columns.ClientId, clientId);
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeDownBackInput.Columns.BatchNumber, BatchNumber);
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeDownBackInput.Columns.Action, Action);
            DbHelper.AddParameterToCommand(command, Constants.GetAcmeDownBackInput.Columns.SignId, signId);

            return this.GetAll<ACMEDownbackRecords>(command, delegate(IDataReader reader, ref List<ACMEDownbackRecords> items)
            {
                items = new List<ACMEDownbackRecords>();
                ACMEDownbackRecords item = new ACMEDownbackRecords
                {
                    DownBackInputRecords = new List<AcmeRecord>(),
                    SignLayouts = new List<SignLayout>(),
                    Departments = new List<Department>(),
                    Headings = new List<Heading>(),
                    Images = new List<Image>(),
                    UnitPriceEntrys = new List<UnitPriceEntry>(),
                    SubstituteStock = new List<SubstituteStock>()
                };
                //Reading Signs
                converter.Convert(reader, item.DownBackInputRecords);
                reader.NextResult();
                converter.Convert(reader, item.Departments);
                reader.NextResult();
                converter.Convert(reader, item.Headings);
                reader.NextResult();
                converter.Convert(reader, item.SignLayouts);
                reader.NextResult();
                converter.Convert(reader, item.Images);
                reader.NextResult();
                converter.Convert(reader, item.UnitPriceEntrys);
                reader.NextResult();
                converter.Convert(reader, item.SubstituteStock);
                items.Add(item);
            }).FirstOrDefault();

        }

        /// <summary>
        /// Saves the input.
        /// </summary>
        /// <param name="dtInputRecords">The dt input records.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns></returns>
        public bool SaveInput(DataTable dtInputRecords, IList<ExceptionReport> exceptions)
        {
            this.Insert<string>(string.Empty, delegate(IDbCommand command, string item)
            {
                command.CommandText = Constants.AcmeSaveInputStoredProcedure.Name;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Structured,
                    Value = dtInputRecords,
                    ParameterName = Constants.AcmeSaveInputStoredProcedure.Columns.ACMEInputTypes
                });
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Structured,
                    Value = ConvertToDatatable(exceptions),
                    ParameterName = Constants.AcmeSaveInputStoredProcedure.Columns.KVATExceptions

                });
            }, delegate(string item, int uniqueId)
            {
                uniqueId = int.MaxValue;
            });
            return true;
        }

        /// <summary>
        /// Updates the department identifier.
        /// </summary>
        /// <param name="orderNumber">The order number.</param>
        /// <returns></returns>
        public bool UpdateDepartmentId(string orderNumber)
        {
            try
            {
                StringBuilder commnandText = new StringBuilder();
                string query = string.Format(@"UPDATE adSD SET adSD.DepartmentId = DL.DepartmentId FROM Signs adSD 
                                        INNER JOIN Orders adSO ON adSO.OrderId = adSD.OrderId Inner JOIN adSignACME_RAW adSACME
                                        ON adSD.Id = adSACME.SignId INNER JOIN DepartmentLookup DL ON DL.LookupKey = adSACME.Department                     
                                        WHERE adSD.DepartmentId IS NULL AND adSO.OrderNumber ='{0}'", orderNumber);

                commnandText.AppendLine(query);

                this.Update<string>(string.Empty, delegate(IDbCommand command, string item)
                {
                    command.CommandText = Convert.ToString(commnandText);
                    command.CommandType = CommandType.Text; 
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the type of the tag.
        /// </summary>
        /// <param name="downbackRecords">The downback records.</param>
        public void UpdateTagType(ACMEDownbackRecords downbackRecords)
        {
            var distinctTagTypes = downbackRecords.DownBackInputRecords.Select(m => new { m.DATA_NUM_IN, m.D_Vestcom_Tag_Type }).ToList();
            foreach (var record in distinctTagTypes)
            {
                StringBuilder commnandText = new StringBuilder();
                string query = string.Format(@"UPDATE Signs SET TagType = '{0}' where Id={1} ", record.D_Vestcom_Tag_Type, record.DATA_NUM_IN);

                commnandText.AppendLine(query);

                this.Update<string>(string.Empty, delegate (IDbCommand command, string item)
                {
                    command.CommandText = Convert.ToString(commnandText);
                    command.CommandType = CommandType.Text;
                });
            }

        }

        /// <summary>
        /// Converts to datatable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public new DataTable ConvertToDatatable<T>(IList<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            IEnumerable<PropertyInfo> Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes((false)).Any(a => a.GetType() == typeof(HeaderAttribute)));

            foreach (var property in Props)
            {
                var attributes = property.GetCustomAttributes(false);
                var columnMapping = attributes
                    .FirstOrDefault(a => a.GetType() == typeof(HeaderAttribute));
                if (columnMapping != null)
                {
                    var mapsto = columnMapping as HeaderAttribute;
                    dataTable.Columns.Add(mapsto.Header);
                }
            }
            foreach (T item in items)
            {
                var values = new object[Props.Count()];
                for (int i = 0; i < Props.Count(); i++)
                {
                    values[i] = Props.ElementAtOrDefault(i).GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        /// Update FileLog
        /// </summary>
        /// <param name="fileLog"></param>
        /// <returns></returns>
        public bool UpdateFileLog(FileLog fileLog)
        {
            try
            {
                this.Insert<string>(string.Empty, delegate (IDbCommand command, string item)
                {
                    command.CommandText = Constants.UpdateFileLogStatus.Name;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Int,
                        Value = fileLog.FileLogKey,
                        ParameterName = Constants.UpdateFileLogStatus.Columns.FileLogKey
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Int,
                        Value = fileLog.Status,
                        ParameterName = Constants.UpdateFileLogStatus.Columns.Status
                    });

                    command.Parameters.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.VarChar,
                        Value = fileLog.ErrorMesssage,
                        ParameterName = Constants.UpdateFileLogStatus.Columns.ErrorMesssage
                    });

                    command.Parameters.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.VarChar,
                        Value = fileLog.OrderNumber,
                        ParameterName = Constants.UpdateFileLogStatus.Columns.OrderNumber
                    });

                }, delegate (string item, int uniqueId)
                {
                    uniqueId = int.MaxValue;
                });

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error at UpdateFileLog", ex);
            }
        }

        /// <summary>
        /// Add File Log Data
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <param name="SubClientId"></param>
        /// <param name="FileLogKey"></param>
        /// <returns></returns>
        public bool AddFileLogData(int ClientId, string FileName, string FilePath, int SubClientId, out int FileLogKey)
        {
            SqlCommand commandRef = null;
            this.Insert<string>(string.Empty, delegate (IDbCommand command, string item)
            {

                command.CommandText = Constants.AddFileLogData.Name;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Int,
                    Value = ClientId,
                    ParameterName = Constants.AddFileLogData.Columns.ClientId
                });
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.VarChar,
                    Value = FileName,
                    ParameterName = Constants.AddFileLogData.Columns.FileName
                });
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.VarChar,
                    Value = FilePath,
                    ParameterName = Constants.AddFileLogData.Columns.FilePath
                });
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Int,
                    Value = SubClientId,
                    ParameterName = Constants.AddFileLogData.Columns.SubClientId
                });
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = Constants.AddFileLogData.Columns.FileLogKey
                });
                commandRef = command as SqlCommand;
            }, delegate (string item, int uniqueId)
            {
                uniqueId = int.MaxValue;
            });
            FileLogKey = Convert.ToInt32(commandRef.Parameters[4].Value, CultureInfo.InvariantCulture);
            return true;
        }

        public bool CreateOrder(int ClientId, string FileName, out string OrderNumber)
        {
            OrderNumber = string.Empty;
            SqlCommand commandRef = null;
            this.Insert<string>(string.Empty, delegate (IDbCommand command, string item)
            {
                command.CommandText = Constants.CreateOrder.Name;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Int,
                    Value = ClientId,
                    ParameterName = Constants.CreateOrder.Columns.ClientId
                });
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    Value = FileName,
                    ParameterName = Constants.CreateOrder.Columns.fileName
                });
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = Constants.CreateOrder.Columns.WorkOrder
                });
                commandRef = command as SqlCommand;
            }, delegate (string item, int uniqueId)
            {
                uniqueId = int.MaxValue;
            });
            OrderNumber = Convert.ToString(commandRef.Parameters[2].Value, CultureInfo.InvariantCulture);
            return true;
        }

        public bool ProcessData(string OrderNumber, bool IsISM, bool IsHoliday = false, bool IsKeHeFile=false)
        {
            this.Insert<string>(string.Empty, delegate (IDbCommand command, string item)
            {
                command.CommandText = IsISM ? Constants.ProcessData.ISMProcessSPName : IsHoliday? Constants.ProcessData.HolidayProcessSPName : IsKeHeFile? Constants.ProcessData.AcmeKeHeProcessSPName : Constants.ProcessData.AcmeProcessSPName;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.VarChar,
                    Value = OrderNumber,
                    ParameterName = Constants.ProcessData.Columns.OrderNumber
                });
            }, delegate (string item, int uniqueId)
            {
                uniqueId = int.MaxValue;
            });
            return true;
        }

        public bool SaveRawData(DataTable dt, bool IsISM, bool IsHoliday = false)
        {
            SqlConnection _dconn;
            string connectionString = this.DataContext.ConnectionString;
            using (_dconn = new SqlConnection(connectionString))
            {
                _dconn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    s.DestinationTableName = IsISM?Constants.ISMRawTable: IsHoliday? Constants.HolidayRawTable : Constants.AcmeRawTable;

                    foreach (var column in dt.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());

                    s.WriteToServer(dt);
                }
            }
            return true;
        }
        public bool SaveISMRawData(DataTable dt)
        {
            SqlConnection _dconn;
            string connectionString = this.DataContext.ConnectionString;
            using (_dconn = new SqlConnection(connectionString))
            {
                _dconn.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    s.DestinationTableName = Constants.ISMRawTable;

                    foreach (var column in dt.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());

                    s.WriteToServer(dt);
                }
            }
            return true;
        }

        public bool SaveHolidayInput(List<HolidayInputTypes> holidayinputtypes, IList<ExceptionReport> exceptions)
        {
             try
            {
                this.Insert<string>(string.Empty, delegate (IDbCommand command, string item)
                {
                    command.CommandText = Constants.AcmeSaveInputStoredProcedure.HolidayName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Structured,
                        Value = ConvertToDatatable(holidayinputtypes),
                        ParameterName = Constants.AcmeSaveInputStoredProcedure.Columns.ACMEHolidayInputTypes
                    });
                    command.Parameters.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Structured,
                        Value = ConvertToDatatable(exceptions),
                        ParameterName = Constants.AcmeSaveInputStoredProcedure.Columns.KVATExceptions

                    });                  
                }, delegate (string item, int uniqueId)
                {
                    uniqueId = int.MaxValue;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error at SaveInput", ex);

            }
            return true;
        }


        public void UpdateTagType(ACMEHolidayDownbackRecords downbackRecords)
        {
            var distinctTagTypes = downbackRecords.DownBackInputRecords.Select(m => new { m.DATA_NUM_IN, m.D_Vestcom_Tag_Type }).ToList();
            foreach (var record in distinctTagTypes)
            {
                StringBuilder commnandText = new StringBuilder();
                string query = string.Format(@"UPDATE Signs SET TagType = '{0}' where Id={1} ", record.D_Vestcom_Tag_Type, record.DATA_NUM_IN);

                commnandText.AppendLine(query);

                this.Update<string>(string.Empty, delegate (IDbCommand command, string item)
                {
                    command.CommandText = Convert.ToString(commnandText);
                    command.CommandType = CommandType.Text;
                });
            }

        }

    }
}
