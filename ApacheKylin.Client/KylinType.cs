namespace ApacheKylin.Client
{
    public enum KylinType
    {
        // Summary:
        //     Maps to SQL_GUID.
        ODBC_Guid = -11,
        //
        // Summary:
        //     Maps to SQL_WLONGVARCHAR.  Native Type: System.String
        ODBC_WLongVarChar = -10,
        //
        // Summary:
        //     Maps to SQL_WVARCHAR.  Native Type: System.String
        ODBC_WVarChar = -9,
        //
        // Summary:
        //     Maps to SQL_WCHAR.  Native Type: System.String
        ODBC_WChar = -8,
        //
        // Summary:
        //     Maps to SQL_BIT.  Native Type: System.Boolean
        ODBC_Bit = -7,
        //
        // Summary:
        //     Maps to SQL_TINYINT.  Native Type: System.SByte
        ODBC_TinyInt = -6,
        //
        // Summary:
        //     Maps to SQL_BIGINT.  Native Type: System.Int64
        ODBC_BigInt = -5,
        //
        // Summary:
        //     Maps to SQL_LONGVARBINARY.  Native Type: array[System.Byte]
        ODBC_LongVarBinary = -4,
        //
        // Summary:
        //     Maps to SQL_VARBINARY.  Native Type: array[System.Byte]
        ODBC_VarBinary = -3,
        //
        // Summary:
        //     Maps to SQL_BINARY.  Native Type: array[System.Byte]
        ODBC_Binary = -2,
        //
        // Summary:
        //     Maps to SQL_LONGVARCHAR.  Native Type: System.String
        ODBC_LongVarChar = -1,
        //
        // Summary:
        //     Maps to SQL_CHAR.  Native Type: System.String
        ODBC_Char = 1,
        //
        // Summary:
        //     Maps to SQL_NUMERIC.  Native Type: System.Decimal
        ODBC_Numeric = 2,
        //
        // Summary:
        //     Maps to SQL_DECIMAL.  Native Type: System.Decimal
        ODBC_Decimal = 3,
        //
        // Summary:
        //     Maps to SQL_INTEGER.  Native Type: System.Int32
        ODBC_Integer = 4,
        //
        // Summary:
        //     Maps to SQL_SMALLINT.  Native Type: System.Int16
        ODBC_SmallInt = 5,
        //
        // Summary:
        //     Maps to SQL_FLOAT.  Native Type: System.Double
        ODBC_Float = 6,
        //
        // Summary:
        //     Maps to SQL_REAL.  Native Type: System.Single
        ODBC_Real = 7,
        //
        // Summary:
        //     Maps to SQL_DOUBLE.  Native Type: System.Double
        ODBC_Double = 8,
        //
        // Summary:
        //     Maps to SQL_DATETIME. This type should NOT be used with CreateTypeMetadata,
        //     as it is not a valid type identifier.
        ODBC_DateTime = 9,
        //
        // Summary:
        //     Maps to SQL_INTERVAL. Not a valid type identifier.
        ODBC_Interval = 10,
        //
        // Summary:
        //     Maps to SQL_VARCHAR.  Native Type: System.String
        ODBC_VarChar = 12,
        //
        // Summary:
        //     Does not map to an ODBC SQL type.  Native Type: System.DateTimeOffset
        ODBC_DateTimeOffset = 36,
        //
        // Summary:
        //     Maps to SQL_TYPE_DATE.  Native Type: System.DateTime
        ODBC_Type_Date = 91,
        //
        // Summary:
        //     Maps to SQL_TYPE_TIME.  Native Type: System.DateTime
        ODBC_Type_Time = 92,
        //
        // Summary:
        //     Maps to SQL_TYPE_TIMESTAMP.  Native Type: System.DateTime
        ODBC_Type_Timestamp = 93,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_YEAR.  Native Type: Simba.DotNetDSI.DataEngine.DSIMonthSpan
        ODBC_Interval_Year = 101,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_MONTH.  Native Type: Simba.DotNetDSI.DataEngine.DSIMonthSpan
        ODBC_Interval_Month = 102,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_DAY.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Day = 103,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_HOUR.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Hour = 104,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_MINUTE.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Minute = 105,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_SECOND.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Second = 106,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_YEAR_TO_MONTH.  Native Type: Simba.DotNetDSI.DataEngine.DSIMonthSpan
        ODBC_Interval_Year_To_Month = 107,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_DAY_TO_HOUR.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Day_To_Hour = 108,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_DAY_TO_MINUTE.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Day_To_Minute = 109,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_DAY_TO_SECOND.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Day_To_Second = 110,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_HOUR_TO_MINUTE.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Hour_To_Minute = 111,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_HOUR_TO_SECOND.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Hour_To_Second = 112,
        //
        // Summary:
        //     Maps to SQL_INTERVAL_MINUTE_TO_SECOND.  Native Type: Simba.DotNetDSI.DataEngine.DSITimeSpan
        ODBC_Interval_Minute_To_Second = 113
    }
}