//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DB.Query.Core.Annotations;
using DB.Query.Core.Annotations.Entity;
using DB.Query.Core.Annotations.StoredProcedure;
using DB.Query.Core.Entities;
using DB.Query.Core.Models;


namespace DB.Query.CommercialDb.Storeds
{
    
    [Database("CommercialDb")]
    [Procedure("GetProductsByNameAndDescription")]
    [Timeout(60)]
    public partial class GetProductsByNameAndDescriptionParameters : StoredProcedureBase
    {
        /// <summary>
        /// Propiedade mapeada para a definição de ProductName
        /// </summary>
        [Paremeter("ProductName", System.Data.SqlDbType.NVarChar)]
        public string ProductName { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ProductDescription
        /// </summary>
        [Paremeter("ProductDescription", System.Data.SqlDbType.NVarChar)]
        public string ProductDescription { get; set; }

        #region Custom Implementation
        // Declare your implementation here

        #endregion
    }

    [Procedure("GetProductsByNameAndDescription")]
    public partial class GetProductsByNameAndDescriptionResult
    {
        /// <summary>
        /// Propiedade mapeada para a definição de ProductId
        /// </summary>
        public System.Nullable<int> ProductId { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ProductName
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ProductDescription
        /// </summary>
        public string ProductDescription { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ProductPrice
        /// </summary>
        public System.Nullable<decimal> ProductPrice { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de StockQuantity
        /// </summary>
        public System.Nullable<int> StockQuantity { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ProductCode
        /// </summary>
        public string ProductCode { get; set; }

        #region Custom Implementation
        // Declare your implementation here

        #endregion
    }
}
