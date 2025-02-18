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


namespace DB.Query.SignEst.Storeds
{
    
    [Database("SignEst")]
    [Procedure("PS_Consulta_Invoice")]
    [Timeout(60)]
    public partial class SPConsultaInvoicesParameters : StoredProcedureBase
    {
        /// <summary>
        /// Propiedade mapeada para a definição de Codigo_Empresa
        /// </summary>
        [Paremeter("Codigo_Empresa", System.Data.SqlDbType.TinyInt)]
        public int CodigoEmpresa { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Codigo_Filial
        /// </summary>
        [Paremeter("Codigo_Filial", System.Data.SqlDbType.TinyInt)]
        public int CodigoFilial { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de DataInicio
        /// </summary>
        [Paremeter("DataInicio", System.Data.SqlDbType.SmallDateTime)]
        public System.DateTime DataInicio { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de DataFim
        /// </summary>
        [Paremeter("DataFim", System.Data.SqlDbType.SmallDateTime)]
        public System.DateTime DataFim { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de CnpjFornecedor
        /// </summary>
        [Paremeter("CnpjFornecedor", System.Data.SqlDbType.Text)]
        public string CnpjFornecedor { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de NumPed
        /// </summary>
        [Paremeter("NumPed", System.Data.SqlDbType.VarChar)]
        public string NumPed { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Rcm
        /// </summary>
        [Paremeter("Rcm", System.Data.SqlDbType.VarChar)]
        public string Rcm { get; set; }
        #region Custom Implementation
        // Declare your implementation here

        #endregion
        
    }
    [Procedure("PS_Consulta_Invoice")]
    public partial class SPConsultaInvoicesParametersResult
    {
        /// <summary>
        /// Propiedade mapeada para a definição de Data_Emissao
        /// </summary>
        public System.Nullable<System.DateTime> Data_Emissao { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Razao
        /// </summary>
        public string Razao { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de UF
        /// </summary>
        public string UF { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de CNPJ
        /// </summary>
        public string CNPJ { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Documento
        /// </summary>
        public System.Nullable<int> Documento { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Moeda
        /// </summary>
        public string Moeda { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Data_Mov
        /// </summary>
        public System.Nullable<System.DateTime> Data_Mov { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ValorTotal
        /// </summary>
        public System.Nullable<decimal> ValorTotal { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de CotacaoMoeda
        /// </summary>
        public System.Nullable<decimal> CotacaoMoeda { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ValorReais
        /// </summary>
        public System.Nullable<decimal> ValorReais { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Aplicacao
        /// </summary>
        public string Aplicacao { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ContaContabil
        /// </summary>
        public string ContaContabil { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de Item
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de RCM
        /// </summary>
        public string RCM { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de PO
        /// </summary>
        public string PO { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de R06
        /// </summary>
        public string R06 { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de NOCP
        /// </summary>
        public System.Nullable<int> NOCP { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de No_Parcela
        /// </summary>
        public string No_Parcela { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de DataVencimento
        /// </summary>
        public System.Nullable<System.DateTime> DataVencimento { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ValorParcela
        /// </summary>
        public System.Nullable<decimal> ValorParcela { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ValorParcelaReais
        /// </summary>
        public System.Nullable<decimal> ValorParcelaReais { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ValorPTax
        /// </summary>
        public System.Nullable<decimal> ValorPTax { get; set; }
        /// <summary>
        /// Propiedade mapeada para a definição de ValorPTaxFechamento
        /// </summary>
        public System.Nullable<decimal> ValorPTaxFechamento { get; set; }
        #region Custom Implementation
        // Declare your implementation here

        #endregion
        
    }
}
