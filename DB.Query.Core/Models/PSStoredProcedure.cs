namespace DB.Query.Core.Models
{
    /// <summary>
    /// TReturnType indica o tipo de valor tratado a ser retornado.
    /// Sendo lista ou datatable
    /// </summary>
    /// <typeparam name="TReturnType"></typeparam>
    public partial class PSStoredProcedure<TReturnType> : StoredProcedureBase where TReturnType : class
    {
    }
}
