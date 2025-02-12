namespace DB.Query.Core.Contants
{
    public class DBKeysConstants
    {
        public const string DELETE = "DELETE FROM {0}{1}";

        public const string SELECT = "SELECT {0} FROM {1} {2}";

        public const string INNER_JOIN = "INNER JOIN {0} ON {1} ";

        public const string LEFT_JOIN = "LEFT JOIN {0} ON {1} ";

        public const string INSERT = "INSERT INTO {0} ({1}) {2} VALUES ({3})";

        public const string INSERT_NOT_EXISTS = "IF NOT EXISTS(SELECT * FROM {0} {1}) BEGIN {2} END ";

        public const string INSERT_NOT_EXISTS_ELSE_ACTION = "IF NOT EXISTS(SELECT * FROM {0} {1}) BEGIN {2} END ELSE BEGIN {3} END";

        public const string UPDATE = "UPDATE {0} SET {1} {2}";

        public const string ALL_COLUMNS = "*";

        public const string ASC = "ASC";

        public const string DESC = "DESC";

        public const string SELECT_ALL = "SELECT *";

        public const string SELECT_KEY = "SELECT";

        public const string SELECT_DISTINCT = "SELECT DISTINCT";

        public const string SELECT_TOP = "SELECT TOP({0})";

        public const string SELECT_DISTINCT_TOP = "SELECT DISTINCT TOP({0})";

        public const string OUTPUT_INSERTED = "OUTPUT Inserted.";

        public const string T_A = "..";

        public const string EQUALS = "=";

        public const string AND = "AND";

        public const string AND_WITH_SPACE = " AND ";

        public const string OR = "OR";

        public const string NOT_EQUAL = "<>";

        public const string GREATER_THAN = ">";

        public const string LESS_THAN = "<";

        public const string LESS_THAN_OR_EQUAL = "<=";

        public const string MULTIPLY = "*";

        public const string DIVIDE = "/";

        public const string SUBSTRACT = "-";

        public const string ADD = "+";

        public const string GREATER_THAN_OR_EQUAL = ">=";

        public const string NULL = "NULL";

        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public const string DATE_OFF_SET_FORMAT = "yyyy-MM-dd HH:mm:ss.fffffffzzz";

        public const string DECIMAL_FORMAT = "0.##########";

        public const string EQUALS_WITH_SPACE = " = ";

        public const string COUNT = "COUNT(*)";

        public const string COUNT_ELEMENT = "COUNT({0})";

        public const string SINGLE_POINT = ".";

        public const string CONVERT_VARCHAR = "CONVERT(varchar(Max), {0})";

        public const string CONCAT = "CONCAT({0})";

        public const string LIKE = "LIKE";

        public const string NOT_LIKE = "NOT LIKE";

        public const string IN = "IN";

        public const string NOT_IN = "NOT IN";

        public const string IS = "IS";

        public const string IS_NOT = "IS NOT";

        public const string ORDER_BY = "ORDER BY";

        public const string ORDER_BY_WITH_SPACE = " ORDER BY ";

        public const string GROUP_BY = "GROUP BY";

        public const string GROUP_BY_WITH_SPACE = " GROUP BY ";

        public const string WHERE = "WHERE";

        public const string WHERE_WITH_SPACE = " WHERE ";

        public const string AS = "AS";

        public const string AS_WITH_SPACE = " AS ";

        public const string DISTINCT_ALL = " DISTINCT *";

        public const string DISTINCT_WITH_SPACE = " DISTINCT ";

        public const string OFFSET = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";

        public const string TRUE_VALUE = "1";

        public const string ISNULL = "ISNULL({0}, {1})";

        public const string ISNULL_VALUE_WITH_SPACE = " IS NULL";

        public const string FALSE_VALUE = "0";

        public const string LIKE_VALUE = "LIKE '%{0}%'";

        public const string NOT_LIKE_VALUE = "NOT LIKE '%{0}%'";

        public const string DOT_ALL_COLUMNS = ".*";

        public const string CLAUSE_EMPRESA_FILIAL = " AND {0}.Codigo_Empresa = {1}.Codigo_Empresa AND {0}.Codigo_Filial = {1}.Codigo_Filial ";
    }
}
