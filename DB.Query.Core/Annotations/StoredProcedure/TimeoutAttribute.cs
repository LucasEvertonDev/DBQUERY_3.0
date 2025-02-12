using System;

namespace DB.Query.Core.Annotations.StoredProcedure
{
    public partial class TimeoutAttribute : Attribute
    {
        public int TimeOut { get; set; }
        public TimeoutAttribute(int timeout)
        {
            TimeOut = timeout;
        }
    }
}
