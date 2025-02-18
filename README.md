# DBQUERY

O **DB Query** é uma ORM (Object-Relational Mapping) leve e eficiente, desenvolvida para substituir trechos de código que utilizam ADO.NET em sistemas existentes. Diferente de outras ORMs que carregam dados em memória para manipulação, o **DB Query** executa as queries diretamente no banco de dados, garantindo maior desempenho e compatibilidade com múltiplos bancos de dados. Atualmente, o DB Query é compatível com SQL Server.

### Objetivos Principais

- **Substituição de ADO.NET:** Facilitar a migração de sistemas que utilizam ADO.NET puro para uma abordagem mais moderna e orientada a objetos.
- **Execução Direta no Banco:** As queries são executadas diretamente no banco de dados, evitando o carregamento desnecessário de dados em memória.
- **Simplicidade e Flexibilidade:** Oferece uma sintaxe fluente e intuitiva, permitindo a construção de consultas complexas de forma fácil e organizada.
- **Gerador de Classes CLI:** O DB Query também oferece uma ferramenta CLI (Command Line Interface) para gerar automaticamente classes baseadas no banco de dados, agilizando a integração e a criação de modelos.

---

## Por que usar o DB Query?

- **Performance:** Como as queries são executadas diretamente no banco, o consumo de memória é reduzido, resultando em maior eficiência.
- **Facilidade de Migração:** Ideal para sistemas legados que utilizam ADO.NET, permitindo uma transição suave para uma abordagem mais moderna.
- **Manutenção Simplificada:** Código mais limpo e organizado, com menos boilerplate e maior clareza nas operações de banco de dados.
- **Geração Automática de Classes:** O gerador de classes a partir do banco de dados torna a integração entre a aplicação e o banco mais ágil e sem erros, com a criação automática de modelos de dados baseados nas tabelas existentes.

## Configurando

Para configurar a conexão do DB Query com o seu banco de dados, use o seguinte código:

```
    DBQueryStarter.Use(dbConnection: yourdbConnection);
```

## Trabalhando com transações 

O DB Query oferece suporte a transações para garantir a consistência dos dados durante operações múltiplas no banco. Para utilizá-las, basta herdar a classe DBQueryTransaction e seguir o exemplo abaixo:

```
    OnTransaction((transaction) =>
    {
        transaction.Query<Product>()
            .Insert(new Product
            {
                ProductName = productName,
                ProductDescription = productDescription,
                Category = category,
                ProductCode = productCode,
                ProductPrice = productPrice,
                StockQuantity = stockQuantity
            })
            .Execute();

        var products = transaction.ExecuteStored<GetProductsByNameAndDescriptionResult>(
            new GetProductsByNameAndDescriptionParameters
            {
                ProductName = productName,
                ProductDescription = productDescription,
            });

        return products;
    });
```
Também é possível injetar o comportamento de transação usando o helper DBQuery.OnTransaction:

```
    DBQuery.OnTransaction((transaction) =>
    {
        transaction.Query<Product>()
            .Insert(new Product
            {
                ProductName = productName,
                ProductDescription = productDescription,
                Category = category,
                ProductCode = productCode,
                ProductPrice = productPrice,
                StockQuantity = stockQuantity
            })
            .Execute();

        var products = transaction.ExecuteStored<GetProductsByNameAndDescriptionResult>(
            new GetProductsByNameAndDescriptionParameters
            {
                ProductName = productName,
                ProductDescription = productDescription,
            });

        return products;
    });
```

## Insert

A operação de **Insert** é usada para inserir um novo registro na tabela correspondente. Quando você utiliza o método `Insert()`, o DB Query executa um **INSERT simples** sem realizar verificações adicionais, como duplicatas ou validações de integridade.

Exemplo de uso:

```
    transaction.Query<Product>()
        .Insert(productInstance)  
        .Execute(); 
```

## InsertIfNotExists

A operação **InsertIfNotExists** realiza um **INSERT** com uma validação de existência antes de efetuar a inserção. 

```
    transaction.Query<Product>()
        .InsertIfNotExists(productInstance)
        .Where(Product => Product.Name == "Product")
        .Execute(); 
```
## InsertOrUpdate
Indica que a ação a ser realizada será um INSERT, com uma validação de existência para efetuação do mesmo. Caso já exista é realizado o UPDATE.

```
    transaction.Query<Product>()
        .InsertOrUpdate(productInstance)
        .SetColumns(Product => new 
        { 
            Product.Name,
            ProductCategory
        })
        .Where(Product => Product.Name == "Product")
        .Execute();
```

## DeleteAndInsert

A operação **DeleteAndInsert** realiza um **DELETE** seguido de um **INSERT**. Primeiro, o DB Query exclui o(s) registro(s) existente(s) que correspondem aos critérios especificados, e em seguida, insere um novo registro. Essa operação é útil quando você deseja substituir um registro existente por um novo, sem precisar fazer verificações de existência manualmente.

```
    transaction.Query<Product>()
        .DeleteAndInsert(productInstance)
        .Where(Product => Product.Name == "Product")
        .Execute();
```

## Select
A operação **Select** realiza um **SELECT simples** na tabela correspondente à entidade, sem filtros ou condições adicionais. É basicamente um espelho da tabela representada pela entidade, equivalente ao comando SQL `SELECT * FROM TEntity`.

```
transaction.Query<Product>().Select().Where(Product => Product.Id == 1).ToList();

transaction.Query<Product>().Select().Top(10).Where(Product => Product.Id == 1).ToList();

transaction.Query<Product>().Select().Distinct().Where(Product => Product.Id == 1).ToList();

transaction.Query<Product>()
    .Select()
    .Where(Product => Product.Codigo == 1)
    .OrderBy(Product => Product.Id)
    .ToList();

transaction.Query<Product>()
    .Select()
    .Where(Product => Product.Codigo == 1)
    .OrderBy(Product => new 
    {
        Product.Id,
        Product.Name
    })
    .OrderByDesc(Product => Product.Category)
    .ToList();

transaction.Query<Product>()
    .Select()
    .Where(Product => Product.Codigo == 1)
    .OrderBy(Product => new 
    {
        Product.Id,
        Product.Name
    })
    .OrderByDesc(Product => Product.Category)
    .Pagination(pageNumber: 1, pageSize: 10)
    .ToList();
```

## Custom Select

A operação **Custom Select** realiza um **SELECT tipado**, onde as colunas selecionadas são definidas diretamente pela consulta, e os dados retornados são mapeados para o tipo especificado no método `ToList<T>`. Isso permite que você tenha controle total sobre quais dados serão selecionados e como eles serão mapeados para um tipo customizado, como um DTO.

```
    transaction.Query<User>()
        .Select<User>(User => new
        {
            User.Name,
            TotalOrders = SQL<int>(
                transaction.Query<Order>()
                    .Select<User>(User => Count())
                    .Where<Order, User>(
                        (Order, User) => Order.UserId == User.Id)
                    .GetQuery())
        })
        .LeftJoin<User, ShipmentInfo>(
            (User, ShipmentInfo) => User.Id == ShipmentInfo.Id)
        .Join<User, PaymentInfo>(
            (User, PaymentInfo) => User.Id == PaymentInfo.UserId)
        .Join<PaymentInfo, Order>(
            (PaymentInfo, Order) => PaymentInfo.OrderId == Order.Id)
        .Join<Order, OrderItem>(
            (Order, OrderItem) => Order.Id == OrderItem.Id)
        .Where<User, PaymentInfo, ShipmentInfo>(
            (User, PaymentInfo, ShipmentInfo) =>
                PaymentInfo.Currency.IN("USD", "BRL")
                && User.Email.LIKE("%dbquery.com")
                && PaymentInfo.PaymentDate == null
                && ShipmentInfo.Id == null)
        .ToList<TotalOrdersDTO>();  
```

## Mapeia suas Stored Procedures
Você pode facilmente gerar o mapeamento das suas **Stored Procedures** utilizando a ferramenta de linha de comando `DB.Query.Cli`. Com ela, você consegue mapear suas stored procedures diretamente para modelos de dados em C# de forma prática e rápida, facilitando a integração entre a sua aplicação e o banco de dados.


    ```
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

    ```

## Trabalhando Com Stored Procedures

```
    var products = transaction.ExecuteStored<GetProductsByNameAndDescriptionResult>(
    new GetProductsByNameAndDescriptionParameters
    {
        ProductName = productName,
        ProductDescription = productDescription,
    });
```


## Contribuições e Dúvidas

O **DB Query** está aberto a contribuições! Se você tiver sugestões de melhorias, correções ou quiser adicionar novas funcionalidades, fique à vontade para fazer um **fork** do repositório e abrir um **pull request**. Será um prazer revisar e integrar novas ideias!

### Como Contribuir:
    1. Faça um fork do projeto.
    2. Crie uma branch para a sua modificação (`git checkout -b feature/nova-funcionalidade`).
    3. Faça suas alterações e adicione testes (se necessário).
    4. Envie um pull request com uma descrição clara das mudanças.

Se você tiver dúvidas sobre como usar o DB Query ou precisar de ajuda para implementá-lo em seu projeto, **não hesite em me chamar**. Farei o possível para ajudar! :)

Você pode me contatar diretamente através do repositório ou abrir uma **issue** com suas perguntas.

### Contato:
- **Email**: [lcseverton@gmail.com](lcseverton@gmail.com)

## Licença   
[MIT](https://choosealicense.com/licenses/mit/)

